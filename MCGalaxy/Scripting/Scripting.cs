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
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace MCGalaxy.Scripting {
    
    /// <summary> Compiles source code files from a particular language into a .dll file. </summary>
    public abstract class IScripting {
        
        public const string AutoloadFile = "text/cmdautoload.txt";
        public const string SourceDir = "extra/commands/source/";
        public const string DllDir = "extra/commands/dll/";
        public const string ErrorPath = "logs/errors/compiler.log";
        
        static readonly string divider = new string('-', 25);
        protected CodeDomProvider compiler;
        
        public abstract string Ext { get; }
        public abstract string ProviderName { get; }
        public abstract string CommandSkeleton { get; }
        
        public static IScripting CS = new ScriptingCS();
        public static IScripting VB = new ScriptingVB();
        
        public IScripting() {
            compiler = CodeDomProvider.CreateProvider(ProviderName);
            if (compiler == null) {
                Logger.Log(LogType.Warning, 
                           "WARNING: {0} compiler is missing, you will be unable to compile {1} commands.", 
                           ProviderName, Ext);
                // TODO: Should we log "You must have .net developer tools. (You need a visual studio)" ?
            }
        }
        
        public static string DllPath(string cmdName) { return DllDir + "Cmd" + cmdName + ".dll"; }
        public static string PluginPath(string name) { return "plugins/" + name + ".dll"; }
        public string SourcePath(string cmdName) { return SourceDir + "Cmd" + cmdName + Ext; }
        
        public void CreateNew(string path, string cmdName) {
            cmdName = cmdName.ToLower();
            
            string syntax = CommandSkeleton;
            // Make sure we use the OS's line endings
            syntax = syntax.Replace(@"\t", "\t");
            syntax = syntax.Replace("\r\n", "\n");
            syntax = syntax.Replace("\n", Environment.NewLine);
            syntax = string.Format(syntax, cmdName.Capitalize(), cmdName);
            
            using (StreamWriter sw = new StreamWriter(path)) {
                sw.WriteLine(syntax);
            }
        }

        public bool Compile(string srcPath, string dstPath) {
            StringBuilder sb = null;
            bool exists = File.Exists(ErrorPath);
            
            if (!File.Exists(srcPath)) {
                sb = new StringBuilder();
                using (StreamWriter w = new StreamWriter(ErrorPath, exists)) {
                    AppendDivider(sb, exists);
                    sb.AppendLine("File not found: " + srcPath);
                    w.Write(sb.ToString());
                }
                return false;
            }

            CompilerParameters args = new CompilerParameters();
            args.GenerateExecutable = false;
            args.OutputAssembly = dstPath;
            
            List<string> source = ReadSourceCode(srcPath, args);
            CompilerResults results = CompileSource(source.Join(Environment.NewLine), args);
            if (results.Errors.Count == 0) return true;

            sb = new StringBuilder();
            AppendDivider(sb, exists);
            bool first = true;
            
            foreach (CompilerError err in results.Errors) {
                if (!first) AppendDivider(sb, true);
                string type = err.IsWarning ? "Warning" : "Error";
                
                sb.AppendLine(type + " on line " + err.Line + ":");
                if (err.Line > 0) sb.AppendLine(source[err.Line - 1]);
                if (err.Column > 0) sb.Append(' ', err.Column - 1);
                sb.AppendLine("^-- " + type + " #" + err.ErrorNumber + " - " + err.ErrorText);
                first = false;
            }
            
            using (StreamWriter w = new StreamWriter(ErrorPath, exists)) {
                w.Write(sb.ToString());
            }
            return !results.Errors.HasErrors;
        }
        
        List<string> ReadSourceCode(string path, CompilerParameters args) {
            List<string> lines = Utils.ReadAllLinesList(path);
            // Allow referencing other assemblies using 'Reference [assembly name]' at top of the file
            for (int i = 0; i < lines.Count; i++) {
                if (!lines[i].CaselessStarts("reference ")) break;
                
                int index = lines[i].IndexOf(' ') + 1;
                string assem = lines[i].Substring(index);
                args.ReferencedAssemblies.Add(assem);
                lines.RemoveAt(i); i--;
            }
            return lines;
        }
        
        void AppendDivider(StringBuilder sb, bool exists) {
            if (!exists) return;
            sb.AppendLine();
            sb.AppendLine(divider);
            sb.AppendLine();
        }
        
        public CompilerResults CompileSource(string source, CompilerParameters args) {
            args.ReferencedAssemblies.Add("MCGalaxy_.dll");
            source = source.Replace("MCLawl", "MCGalaxy");
            source = source.Replace("MCForge", "MCGalaxy");
            return compiler.CompileAssemblyFromSource(args, source);
        }
        
        
        public static void Autoload() {
            if (!File.Exists(AutoloadFile)) { File.Create(AutoloadFile); return; }        
            string[] list = File.ReadAllLines(AutoloadFile);
            
            foreach (string cmdName in list) {
                if (cmdName.Length == 0) continue;
                string path = DllPath(cmdName);
                string error = IScripting.Load(path);
                
                if (error != null) { Logger.Log(LogType.Warning, error); continue; }
                Logger.Log(LogType.SystemActivity, "AUTOLOAD: Loaded Cmd{0}.dll", cmdName);
            }
        }
        
        public static string Load(string path) {
            try {
                byte[] data = File.ReadAllBytes(path);
                Assembly lib = Assembly.Load(data);
                List<Command> commands = LoadTypes<Command>(lib);
                
                if (commands.Count == 0) return "No commands in dll file";
                foreach (Command cmd in commands) { Command.Register(cmd); }
            } catch (Exception ex) {
                Logger.LogError("Error loading commands from " + path, ex);
                
                string file = Path.GetFileName(path);
                if (ex is FileNotFoundException) {
                    return file + " does not exist in the DLL folder, or is missing a dependency. Details in the error log.";
                } else if (ex is BadImageFormatException) {
                    return file + " is not a valid assembly, or has an invalid dependency. Details in the error log.";
                } else if (ex is PathTooLongException) {
                    return "Class name is too long.";
                } else if (ex is FileLoadException) {
                    return file + " or one of its dependencies could not be loaded. Details in the error log.";
                }
                return "An unknown error occured. Details in the error log.";
            }
            return null;
        }
        
        public static List<T> LoadTypes<T>(Assembly lib) {
            List<T> instances = new List<T>();
            
            foreach (Type t in lib.GetTypes()) {
                if (t.IsAbstract || t.IsInterface || !t.IsSubclassOf(typeof(T))) continue;
                object instance = Activator.CreateInstance(t);
                
                if (instance == null) {
                    Logger.Log(LogType.Warning, "{0} \"{1}\" could not be loaded", typeof(T).Name, t.Name);
                    throw new BadImageFormatException();
                }
                instances.Add((T)instance);
            }
            return instances;
        }
    }
}
