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
        protected CompilerParameters args = new CompilerParameters();
        protected CompilerResults results;
        
        public abstract string Ext { get; }
        public abstract string ProviderName { get; }
        public abstract string CommandSkeleton { get; }
        
        public static IScripting CS = new ScriptingCS();
        public static IScripting VB = new ScriptingVB();
        
        public IScripting() {
            compiler = CodeDomProvider.CreateProvider(ProviderName);
            if (compiler == null) {
                Logger.Log(LogType.Warning, 
                           "WARNING: Provider {0} is missing, you will be unable to compile {1} commands.", 
                           ProviderName, Ext);
                // TODO: Should we log "You must have .net developer tools. (You need a visual studio)" ?
            }
        }
        
        public bool SourceFileExists(string cmdName) {
            return File.Exists(SourceDir + "Cmd" + cmdName + Ext);
        }
        
        public void CreateNew(string cmdName) {
            if (!Directory.Exists(SourceDir))
                Directory.CreateDirectory(SourceDir);
            cmdName = cmdName.ToLower();
            
            string syntax = CommandSkeleton;
            // Make sure we are using the OS's line endings
            syntax = syntax.Replace(@"\t", "\t");
            syntax = syntax.Replace("\r\n", "\n");
            syntax = syntax.Replace("\n", Environment.NewLine);
            syntax = String.Format(syntax, cmdName.Capitalize(), cmdName);
            
            string path = SourceDir + "Cmd" + cmdName + Ext;
            using (StreamWriter sw = new StreamWriter(path))
                sw.WriteLine(syntax);
        }
        
        /// <summary> Compiles a command from source into a DLL. </summary>
        /// <param name="commandName">Name of the command file to be compiled (without the extension)</param>
        /// <returns> True on successful compile, false on failure. </returns>
        public bool Compile(string cmdName) {
            if (!Directory.Exists(DllDir)) {
                Directory.CreateDirectory(DllDir);
            }
            return Compile(SourceDir + "Cmd" + cmdName, DllDir + "Cmd" + cmdName);
        }
        
        /// <summary> Compiles a written function from source into a DLL. </summary>
        /// <param name="baseName"> Path to file to be compiled (**without** the extension) </param>
        /// <param name="baseOutName"> Path of file to be compiled to (**without** the extension) </param>
        /// <returns> True on successful compile, false on failure. </returns>
        public bool Compile(string baseName, string baseOutName) {
            string path = baseName + Ext;
            StringBuilder sb = null;
            bool exists = File.Exists(ErrorPath);
            
            if (!File.Exists(path)) {
                sb = new StringBuilder();
                using (StreamWriter w = new StreamWriter(ErrorPath, exists)) {
                    AppendDivider(sb, exists);
                    sb.AppendLine("File not found: " + path);
                    w.Write(sb.ToString());
                }
                return false;
            }

            CompilerParameters args = new CompilerParameters();
            args.GenerateExecutable = false;
            args.OutputAssembly = baseOutName + ".dll";
            
            List<string> source = ReadSourceCode(path, args);
            results = CompileSource(source.Join(Environment.NewLine), args);
            if (!results.Errors.HasErrors) return true;

            sb = new StringBuilder();
            AppendDivider(sb, exists);
            bool first = true;
            foreach (CompilerError err in results.Errors) {
                if (!first) AppendDivider(sb, true);
                
                sb.AppendLine("Error on line " + err.Line + ":");
                if (err.Line > 0) sb.AppendLine(source[err.Line - 1]);
                if (err.Column > 0) sb.Append(' ', err.Column - 1);
                sb.AppendLine("^-- Error #" + err.ErrorNumber + " - " + err.ErrorText);
                first = false;
            }
            using (StreamWriter w = new StreamWriter(ErrorPath, exists)) {
                w.Write(sb.ToString());
            }
            return false;
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
        
        
        /// <summary> Automatically loads all .dll commands specified in the autoload file. </summary>
        public static void Autoload() {
            if (!File.Exists(AutoloadFile)) { File.Create(AutoloadFile); return; }        
            string[] list = File.ReadAllLines(AutoloadFile);
            
            foreach (string cmdName in list) {
                if (cmdName.Length == 0) continue;
                string error = IScripting.Load("Cmd" + cmdName);
                if (error != null) { Logger.Log(LogType.Warning, error); continue; }
                
                Logger.Log(LogType.SystemActivity, "AUTOLOAD: Loaded Cmd{0}.dll", cmdName);
            }
        }
        
        /// <summary> Loads a command for use on the server. </summary>
        /// <param name="command">Name of the command to be loaded (make sure it's prefixed by Cmd before bringing it in here or you'll have problems).</param>
        /// <returns>Error string on failure, null on success.</returns>
        public static string Load(string command) {
            if (!command.CaselessStarts("cmd")) return "Invalid command name specified.";
            string file = command + ".dll";
            
            try {
                byte[] data = File.ReadAllBytes(DllDir + file);
                Assembly lib = Assembly.Load(data); // TODO: Assembly.LoadFile instead?
                List<Command> commands = LoadTypes<Command>(lib);
                
                if (commands.Count == 0) return "No commands in dll file";
                foreach (Command cmd in commands) { Command.all.Add(cmd); }
            } catch (Exception e) {
                Logger.LogError(e);
                
                if (e is FileNotFoundException) {
                    return file + " does not exist in the DLL folder, or is missing a dependency. Details in the error log.";
                } else if (e is BadImageFormatException) {
                    return file + " is not a valid assembly, or has an invalid dependency. Details in the error log.";
                } else if (e is PathTooLongException) {
                    return "Class name is too long.";
                } else if (e is FileLoadException) {
                    return file + " or one of its dependencies could not be loaded. Details in the error log.";
                }
                return "An unknown error occured and has been logged.";
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
