/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified by MCGalaxy)

    Edited for use with MCGalaxy
 
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */

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

#if !MCG_STANDALONE
using System;
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
#if !NETSTANDARD
    /// <summary> Compiles source code files from a particular language, using a CodeDomProvider for the compiler </summary>
    public static class ICodeDomCompiler
    {   
        public static CompilerParameters PrepareInput(string[] srcPaths, string dstPath, string commentPrefix) {
            CompilerParameters args = new CompilerParameters();
            args.GenerateExecutable      = false;
            args.IncludeDebugInformation = true;
            args.OutputAssembly          = dstPath;

            List<string> referenced = ICompiler.ProcessInput(srcPaths, commentPrefix);
            foreach (string assembly in referenced)
            {
                args.ReferencedAssemblies.Add(assembly);
            }
            return args;
        }

        // Lazy init compiler when it's actually needed
        public static void InitCompiler(ICompiler c, string language, ref CodeDomProvider compiler) {
            if (compiler != null) return;
            compiler = CodeDomProvider.CreateProvider(language);
            if (compiler != null) return;
                
            Logger.Log(LogType.Warning,
                       "WARNING: {0} compiler is missing, you will be unable to compile {1} files.",
                       c.FullName, c.FileExtension);
                // TODO: Should we log "You must have .net developer tools. (You need a visual studio)" ?
        }

        public static ICompilerErrors Compile(CompilerParameters args, string[] srcPaths, CodeDomProvider compiler) {
            CompilerResults results = compiler.CompileAssemblyFromFile(args, srcPaths);
            ICompilerErrors errors  = new ICompilerErrors();

            foreach (CompilerError error in results.Errors)
            {
                ICompilerError ce = new ICompilerError();
                ce.Line        = error.Line;
                ce.Column      = error.Column;
                ce.ErrorNumber = error.ErrorNumber;
                ce.ErrorText   = error.ErrorText;
                ce.IsWarning   = error.IsWarning;
                ce.FileName    = error.FileName;

                errors.Add(ce);
            }
            return errors;
        }
    }
#else
    /// <summary> Compiles C# source code files, using Roslyn for the compiler </summary>
    public static class RoslynCSharpCompiler 
    {
        static Regex outputRegWithFileAndLine;
        static Regex outputRegSimple;

        public static ICompilerErrors Compile(string[] srcPaths, string dstPath, List<string> referenced) {         
            string args    = GetCommandLineArguments(srcPaths, dstPath, referenced);
            string netPath = GetBinaryFile("MCG_DOTNET_PATH", "'dotnet' executable - e.g. /home/test/.dotnet/dotnet");
            string cscPath = GetBinaryFile("MCG_COMPILER_PATH", "'csc.dll' file - e.g. /home/test/.dotnet/sdk/6.0.300/Roslyn/bincore/csc.dll");

            ICompilerErrors errors = new ICompilerErrors();
            List<string> output    = new List<string>();
            int retValue = Compile(netPath, cscPath, args, output);

            // Only look for errors/warnings if the compile failed
            // TODO still log warnings anyways error when success?
            if (retValue != 0) {
                foreach (string line in output) 
                {
                    ProcessCompilerOutputLine(errors, line);
                }
            }
            return errors;
        }

        static string Quote(string value) { return "\"" + value + "\""; }

        static string GetBinaryFile(string varName, string desc) {
            string path = Environment.GetEnvironmentVariable(varName);
            if (string.IsNullOrEmpty(path))
                throw new InvalidOperationException("Env variable '" + varName + " must specify the path to " + desc);

            // make sure file exists
            using (Stream tmp = File.OpenRead(path)) { }
            return path;
        }

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

        static void ProcessCompilerOutputLine(ICompilerErrors errors, string line) {
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
            ICompilerError ce = new ICompilerError();

            if (full) {
                ce.FileName = m.Groups[2].Value;
                ce.Line     = int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture);
                ce.Column   = int.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);
            }

            ce.IsWarning   = m.Groups[full ? 6 : 1].Value.CaselessEq("warning");
            ce.ErrorNumber = m.Groups[full ? 7 : 2].Value;
            ce.ErrorText   = m.Groups[full ? 8 : 3].Value;
            errors.Add(ce);
        }

        static string GetCommandLineArguments(string[] srcPaths, string dstPath, List<string> referencedAssemblies) {
            StringBuilder sb = new StringBuilder();
            sb.Append("/t:library ");

            sb.Append("/utf8output ");
            sb.Append("/noconfig ");
            sb.Append("/fullpaths ");

            // If we don't reference netstandard, System.Runtime, and System.Private.CoreLib, get an error when compiling
            //  "The type 'Object' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=..."
            // https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting
            // https://stackoverflow.com/questions/58840995/roslyn-compilation-how-to-reference-a-net-standard-2-0-class-library
            // https://luisfsgoncalves.wordpress.com/2017/03/20/referencing-system-assemblies-in-roslyn-compilations/
            // https://github.com/dotnet/roslyn/issues/34111

            string coreAssemblyFileName = typeof(object).Assembly.Location;
            string[] sysAssemblyPaths   = GetSystemAssemblyPaths();

            if (!string.IsNullOrWhiteSpace(coreAssemblyFileName)) {
                sb.Append("/nostdlib+ ");
                sb.AppendFormat("/R:{0} ", Quote(coreAssemblyFileName.Trim()));
            }

            AddReferencedAssembly(sb, sysAssemblyPaths, "System.Runtime.dll");
            AddReferencedAssembly(sb, sysAssemblyPaths, "netstandard.dll");

            foreach (string path in referencedAssemblies) 
            {
                AddReferencedAssembly(sb, sysAssemblyPaths, path);
            }
            sb.AppendFormat("/out:{0} ", Quote(dstPath));

            // debug information
            sb.Append("/D:DEBUG ");
            sb.Append("/debug+ ");
            sb.Append("/optimize- ");

            sb.Append("/warnaserror- ");
            sb.Append("/unsafe ");

            foreach (string path in srcPaths) 
            {
                sb.AppendFormat("{0} ", Quote(path));
            }
            return sb.ToString();
        }

        static string[] GetSystemAssemblyPaths() {
            string assemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
            if (string.IsNullOrEmpty(assemblies)) return new string[0];

            return assemblies.Split(Path.PathSeparator);
        }

        static void AddReferencedAssembly(StringBuilder sb, string[] sysAssemblyPaths, string path) {
            path = MapAssembly(sysAssemblyPaths, path);
            sb.AppendFormat("/R:{0} ", Quote(path));
        }

        // Try to use full system .dll path (otherwise roslyn may not always find the .dll)
        static string MapAssembly(string[] sysAssemblyPaths, string file) {
            foreach (string sysPath in sysAssemblyPaths)
            {
                if (file == Path.GetFileName(sysPath)) return sysPath;
            }
            return file;
        }
        
    }
#endif
}
#endif