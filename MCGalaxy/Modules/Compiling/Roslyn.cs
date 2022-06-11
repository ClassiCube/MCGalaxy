// Based on https://github.com/aspnet/RoslynCodeDomProvider
// Copyright(c) Microsoft Corporation All rights reserved.
// 
// MIT License
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if !DISABLE_COMPILING
#if NETSTANDARD
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace MCGalaxy.Modules.Compiling
{
    public class RoslynCSharpCodeProvider : Microsoft.CSharp.CSharpCodeProvider
    {
        [Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class.")]
        public override ICodeCompiler CreateCompiler() { return new CSharpCompiler(); }
    }

    class CSharpCompiler : ICodeCompiler 
    {
        static Regex outputRegWithFileAndLine;
        static Regex outputRegSimple;

        public CompilerResults CompileAssemblyFromDom(CompilerParameters options, CodeCompileUnit compilationUnit) { return null; }
        public CompilerResults CompileAssemblyFromDomBatch(CompilerParameters options, CodeCompileUnit[] compilationUnits) { return null; }
        public CompilerResults CompileAssemblyFromSource(CompilerParameters options, string source) { return null; }
        public CompilerResults CompileAssemblyFromSourceBatch(CompilerParameters options, string[] sources) { return null; }


        public CompilerResults CompileAssemblyFromFile(CompilerParameters options, string fileName) {
            return CompileAssemblyFromFileBatch(options, new string[] { fileName });
        }

        public CompilerResults CompileAssemblyFromFileBatch(CompilerParameters options, string[] fileNames) {
            try {
                return FromFileBatch(options, fileNames);
            } finally {
                options.TempFiles.Delete();
            }
        }

        static CompilerResults FromFileBatch(CompilerParameters options, string[] fileNames) {
            CompilerResults results = new CompilerResults(options.TempFiles);
            List<string> output     = new List<string>();

            results.TempFiles.AddExtension("pdb"); // for .pdb debug files
            string args = GetCommandLineArguments(options, fileNames);

            string netPath = GetBinaryFile("MCG_DOTNET_PATH", "'dotnet' executable - e.g. /home/test/.dotnet/dotnet");
            string cscPath = GetBinaryFile("MCG_COMPILER_PATH", "'csc.dll' file - e.g. /home/test/.dotnet/sdk/6.0.300/Roslyn/bincore/csc.dll");

            int retValue = Compile(netPath, cscPath, args, output);
            results.NativeCompilerReturnValue = retValue;

            // only look for errors/warnings if the compile failed or the caller set the warning level
            if (retValue != 0 || options.WarningLevel > 0) {
                foreach (string line in output) 
                {
                    ProcessCompilerOutputLine(results, line);
                }
            }

            results.PathToAssembly = options.OutputAssembly;
            return results;
        }

        static string Quote(string value) { return "\"" + value + "\""; }

        static int Compile(string path, string exeArgs, string args, List<string> output) {
            // e.g. /home/test/.dotnet/dotnet exec "/home/test/.dotnet/sdk/6.0.300/Roslyn/bincore/csc.dll" [COMPILER ARGS]
            args = "exec " + Quote(exeArgs) + " " + args;

            // https://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net
            ProcessStartInfo psi = new ProcessStartInfo(path, args);
            psi.WorkingDirectory       = Environment.CurrentDirectory;
            psi.UseShellExecute        = false;
            psi.CreateNoWindow         = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError  = true;

            using (Process p = new Process())
            {
                p.OutputDataReceived += (s, e) => { if (e.Data != null) output.Add(e.Data); };
                p.ErrorDataReceived  += (s, e) => { }; // swallow stderr output

                p.StartInfo = psi;
                p.Start();

                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                if (!p.WaitForExit(120 * 1000))
                    throw new InvalidOperationException("C# compiler ran for over two minutes! Giving up..");

                return p.ExitCode;
            }
        }

        static void ProcessCompilerOutputLine(CompilerResults results, string line) {
            if (outputRegSimple == null) {
                outputRegWithFileAndLine =
                    new Regex(@"(^(.*)(\(([0-9]+),([0-9]+)\)): )(error|warning) ([A-Z]+[0-9]+) ?: (.*)");
                outputRegSimple =
                    new Regex(@"(error|warning) ([A-Z]+[0-9]+) ?: (.*)");
            }

            //First look for full file info
            Match m = outputRegWithFileAndLine.Match(line);
            bool full;
            if (m.Success) {
                full = true;
            } else {
                m = outputRegSimple.Match(line);
                full = false;
            }

            if (!m.Success) return;
            CompilerError ce = new CompilerError();

            if (full) {
                ce.FileName = m.Groups[2].Value;
                ce.Line = int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture);
                ce.Column = int.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);
            }

            if (string.Compare(m.Groups[full ? 6 : 1].Value, "warning", StringComparison.OrdinalIgnoreCase) == 0) {
                ce.IsWarning = true;
            }

            ce.ErrorNumber = m.Groups[full ? 7 : 2].Value;
            ce.ErrorText = m.Groups[full ? 8 : 3].Value;
            results.Errors.Add(ce);
        }

        static string GetCommandLineArguments(CompilerParameters parameters, string[] fileNames) {
            StringBuilder sb = new StringBuilder();
            sb.Append("/t:library ");

            sb.Append("/utf8output ");
            sb.Append("/noconfig ");
            sb.Append("/fullpaths ");

            string coreAssemblyFileName = typeof(object).Assembly.Location;
            Console.WriteLine("CORE FILE: " + coreAssemblyFileName);
            if (!string.IsNullOrWhiteSpace(coreAssemblyFileName)) {
                sb.Append("/nostdlib+ ");
                sb.AppendFormat("/R:{0} ", Quote(coreAssemblyFileName.Trim()));
            }

            // Bug 913691: Explicitly add System.Runtime as a reference.
            string systemRuntimeAssemblyPath = null;
            try {
                var systemRuntimeAssembly = Assembly.Load("System.Runtime, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
                systemRuntimeAssemblyPath = systemRuntimeAssembly.Location;
            }
            catch {
                // swallow any exceptions if we cannot find the assembly
            }

            if (systemRuntimeAssemblyPath != null) {
                sb.AppendFormat("/R:{0} ", Quote(systemRuntimeAssemblyPath));
            }

            foreach (string path in parameters.ReferencedAssemblies) {
                sb.AppendFormat("/R:{0} ", Quote(path));
            }

            sb.AppendFormat("/out:{0} ", Quote(parameters.OutputAssembly));

            // debug information
            sb.Append("/D:DEBUG ");
            sb.Append("/debug+ ");
            sb.Append("/optimize- ");

            sb.Append("/warnaserror- ");

            if (parameters.CompilerOptions != null) {
                sb.Append(parameters.CompilerOptions + " ");
            }

            foreach (string path in fileNames) {
                sb.AppendFormat("{0} ", Quote(path));
            }
            return sb.ToString();
        }

        static string GetBinaryFile(string varName, string desc) {
            string path = Environment.GetEnvironmentVariable(varName);
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Env variable '" + varName + " must specify the path to " + desc);

            // make sure file exists
            using (Stream tmp = File.OpenRead(path)) { }
            return path;
        }
    }
}
#endif
#endif