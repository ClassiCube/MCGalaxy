/*
    Copyright 2015 MCGalaxy
 
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace MCGalaxy.Modules.Compiling
{
    /// <summary> Compiles C# source files into a .dll by invoking a compiler executable directly </summary>
    public abstract class CommandLineCompiler
    {
        public ICompilerErrors Compile(string[] srcPaths, string dstPath, List<string> referenced) {
            string args = GetCommandLineArguments(srcPaths, dstPath, referenced);
            string exe  = GetExecutable();

            ICompilerErrors errors = new ICompilerErrors();
            List<string> output    = new List<string>();
            int retValue = Compile(exe, GetCompilerArgs(exe, args), output);

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
        
        
        protected virtual string GetCommandLineArguments(string[] srcPaths, string dstPath,
                                                         List<string> referencedAssemblies) {
            StringBuilder sb = new StringBuilder();
            sb.Append("/t:library ");

            sb.Append("/utf8output /noconfig /fullpaths ");
            
            AddCoreAssembly(sb);
            AddReferencedAssemblies(sb, referencedAssemblies);
            sb.AppendFormat("/out:{0} ", Quote(dstPath));

            sb.Append("/D:DEBUG /debug+ /optimize- ");
            sb.Append("/warnaserror- /unsafe ");

            foreach (string path in srcPaths)
            {
                sb.AppendFormat("{0} ", Quote(path));
            }
            return sb.ToString();
        }
        
        protected virtual void AddCoreAssembly(StringBuilder sb) {
            string coreAssemblyFileName = typeof(object).Assembly.Location;

            if (!string.IsNullOrEmpty(coreAssemblyFileName)) {
                sb.Append("/nostdlib+ ");
                sb.AppendFormat("/R:{0} ", Quote(coreAssemblyFileName));
            }
        }
        
        protected abstract void AddReferencedAssemblies(StringBuilder sb, List<string> referenced);

        protected static string Quote(string value) { return "\"" + value.Trim() + "\""; }
        
        protected abstract string GetExecutable();
        protected abstract string GetCompilerArgs(string exe, string args);
        

        static int Compile(string path, string args, List<string> output) {
            // https://stackoverflow.com/questions/285760/how-to-spawn-a-process-and-capture-its-stdout-in-net
            ProcessStartInfo psi = CreateStartInfo(path, args);

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
        
        protected static ProcessStartInfo CreateStartInfo(string path, string args) {
            ProcessStartInfo psi = new ProcessStartInfo(path, args);
            psi.WorkingDirectory       = Environment.CurrentDirectory;
            psi.UseShellExecute        = false;
            psi.CreateNoWindow         = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError  = true;
            return psi;
        }
        
        
        static Regex outputRegWithFileAndLine;
        static Regex outputRegSimple;
        
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
                ce.Line     = NumberUtils.ParseInt32(m.Groups[4].Value);
                ce.Column   = NumberUtils.ParseInt32(m.Groups[5].Value);
            }

            ce.IsWarning   = m.Groups[full ? 6 : 1].Value.CaselessEq("warning");
            ce.ErrorNumber = m.Groups[full ? 7 : 2].Value;
            ce.ErrorText   = m.Groups[full ? 8 : 3].Value;
            errors.Add(ce);
        }
    }

#if !MCG_DOTNET
    internal class ClassicCSharpCompiler : CommandLineCompiler
    {
        protected override void AddCoreAssembly(StringBuilder sb) {
            string coreAssemblyFileName = typeof(object).Assembly.Location;

            if (!string.IsNullOrEmpty(coreAssemblyFileName)) {
                sb.Append("/nostdlib+ ");
                sb.AppendFormat("/R:{0} ", Quote(coreAssemblyFileName));
            }
        }
        
        protected override void AddReferencedAssemblies(StringBuilder sb, List<string> referenced) {
            foreach (string path in referenced)
            {
                sb.AppendFormat("/R:{0} ", Quote(path));
            }
        }
        
        
        protected override string GetExecutable() {
            string root = RuntimeEnvironment.GetRuntimeDirectory();
            
            string[] paths = new string[] {
                // First try new C# compiler
                Path.Combine(root, "csc.exe"),
                // Then fallback to old Mono C# compiler
                Path.Combine(root, @"../../../bin/mcs"), 
                Path.Combine(root, "mcs.exe"),
                "/usr/bin/mcs",
            };
            
            foreach (string path in paths)
            {
                if (File.Exists(path)) return path;
            }
            return paths[0];
        }
        
        protected override string GetCompilerArgs(string exe, string args) {
            return args;
        }
    }
#else
    /// <summary> Compiles C# source code files, using Roslyn for the compiler </summary>
    public class RoslynCSharpCompiler : CommandLineCompiler
    {
        protected override string GetExecutable() {
            string path = Server.GetRuntimeExePath();
            if (path.EndsWith("dotnet")) return path;
            
            path = Environment.GetEnvironmentVariable("MCG_DOTNET_PATH");
            if (string.IsNullOrEmpty(path)) {
                throw new InvalidOperationException("Env variable 'MCG_DOTNET_PATH' must specify the path to 'dotnet' executable - e.g. /home/test/.dotnet/dotnet");
            }
            
            // make sure file exists
            using (Stream tmp = File.OpenRead(path)) { }
            return path;
        }
        
        protected override string GetCompilerArgs(string dotnetPath, string args) {
            ProcessStartInfo psi = CreateStartInfo(dotnetPath, "--list-sdks");
            string rootFolder    = Path.GetDirectoryName(dotnetPath);
            
            using (Process p = new Process())
            {
                p.StartInfo = psi;
                p.Start();

                string sdk = p.StandardOutput.ReadLine();
                p.WaitForExit();
                
                string compileArgs = Path.Combine(rootFolder, "sdk", sdk, "Roslyn", "bincore", "csc.dll");
                // e.g. /home/test/.dotnet/dotnet exec "/home/test/.dotnet/sdk/6.0.300/Roslyn/bincore/csc.dll" [COMPILER ARGS]
                return "exec " + Quote(compileArgs) + " " + args;
            }
        }
        
        
        protected override void AddReferencedAssemblies(StringBuilder sb, List<string> referenced) {
            string[] sysAssemblyPaths = GetSystemAssemblyPaths();
            
            // If we don't reference netstandard, System.Runtime, and System.Private.CoreLib, get an error when compiling
            //  "The type 'Object' is defined in an assembly that is not referenced. You must add a reference to assembly 'netstandard, Version=..."
            // https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/cross-platform-targeting
            // https://stackoverflow.com/questions/58840995/roslyn-compilation-how-to-reference-a-net-standard-2-0-class-library
            // https://luisfsgoncalves.wordpress.com/2017/03/20/referencing-system-assemblies-in-roslyn-compilations/
            // https://github.com/dotnet/roslyn/issues/34111
            referenced.Add("System.Runtime.dll");
            referenced.Add("netstandard.dll");
            
            referenced.Add("System.Collections.dll");    // needed for List<> etc
            referenced.Add("System.IO.Compression.dll"); // needed for GZip compression
            referenced.Add("System.Net.Primitives.dll"); // needed for IPAddress etc

            foreach (string path in referenced)
            {
                AddReferencedAssembly(sb, sysAssemblyPaths, path);
            }
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