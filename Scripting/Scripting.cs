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
using System.IO;
using System.Reflection;
using System.Text;

namespace MCGalaxy {
    
    /// <summary> Compiles source code files from a particular language into a .dll file. </summary>
    public abstract class Scripting {
        
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
        
        public static Scripting CS = new ScriptingCS();
        public static Scripting VB = new ScriptingVB();
        
        public Scripting() {
            compiler = CodeDomProvider.CreateProvider(ProviderName);
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
            using (var sw = new StreamWriter(path))
                sw.WriteLine(syntax);
        }
        
        /// <summary> Compiles a written function from source into a DLL. </summary>
        /// <param name="commandName">Name of the command file to be compiled (without the extension)</param>
        /// <returns> True on successful compile, false on failure. </returns>
        public bool Compile(string cmdName) {
            string path = SourceDir + "Cmd" + cmdName + Ext;
            StringBuilder sb = null;
            bool exists = File.Exists(ErrorPath);
            
            if (!File.Exists(path)) {
                sb = new StringBuilder();
                using (StreamWriter w = new StreamWriter(ErrorPath, exists)) {
                    AppendDivider(sb, exists);
                    sb.AppendLine("File not found: Cmd" + cmdName + ".cs");
                    w.Write(sb.ToString());
                }
                return false;
            }
            if (!Directory.Exists(DllDir))
                Directory.CreateDirectory(DllDir);
            
            args.GenerateExecutable = false;
            args.MainClass = cmdName;
            args.OutputAssembly = DllDir + "Cmd" + cmdName + ".dll";
            args.ReferencedAssemblies.Add("MCGalaxy_.dll");
            string code = File.ReadAllText(path);
            results = compiler.CompileAssemblyFromSource(args, code.Replace("MCLawl", "MCGalaxy"));
            
            switch (results.Errors.Count) {
                case 0:
                    return true;
                case 1:
                    sb = new StringBuilder();
                    CompilerError error = results.Errors[0];
                    AppendDivider(sb, exists);
                    
                    sb.AppendLine("Error " + error.ErrorNumber);
                    sb.AppendLine("Message: " + error.ErrorText);
                    sb.AppendLine("Line: " + error.Line);
                    using (StreamWriter w = new StreamWriter(ErrorPath, exists))
                        w.Write(sb.ToString());
                    return false;
                default:
                    sb = new StringBuilder();
                    AppendDivider(sb, exists);
                    bool first = true;
                    
                    foreach (CompilerError err in results.Errors) {
                        if (!first) AppendDivider(sb, true);
                        sb.AppendLine("Error #" + err.ErrorNumber);
                        sb.AppendLine("Message: " + err.ErrorText);
                        sb.AppendLine("Line: " + err.Line);
                        first = false;
                    }
                    using (StreamWriter w = new StreamWriter(ErrorPath, exists))
                        w.Write(sb.ToString());
                    return false;
            }
        }
        
        void AppendDivider(StringBuilder sb, bool exists) {
            if (!exists) return;
            sb.AppendLine();
            sb.AppendLine(divider);
            sb.AppendLine();
        }
        
        /// <summary> Automatically loads all .dll commands specified in the autoload file. </summary>
        public static void Autoload() {
            if (!File.Exists(AutoloadFile)) { File.Create(AutoloadFile); return; }        
            string[] list = File.ReadAllLines(AutoloadFile);
            
            foreach (string cmd in list) {
                if (cmd == "") continue;
                string error = Scripting.Load("Cmd" + cmd.ToLower());
                if (error != null) { Server.s.Log(error); continue; }
                Server.s.Log("AUTOLOAD: Loaded " + cmd.ToLower() + ".dll");

            }
        }
        
        /// <summary> Loads a command for use on the server. </summary>
        /// <param name="command">Name of the command to be loaded (make sure it's prefixed by Cmd before bringing it in here or you'll have problems).</param>
        /// <returns>Error string on failure, null on success.</returns>
        public static string Load(string command) {
            if (command.Length < 3 || command.Substring(0, 3).ToLower() != "cmd")
                return "Invalid command name specified.";
            try {
                //Allows unloading and deleting dlls without server restart
                byte[] data = File.ReadAllBytes(DllDir + command + ".dll");
                Assembly lib = Assembly.Load(data);
                
                foreach (Type t in lib.GetTypes()) {
                    if (t.BaseType != typeof(Command)) continue;
                    object instance = Activator.CreateInstance(t);
                    
                    if (instance == null) {
                        Server.s.Log("The command " + command + " couldn't be loaded!");
                        throw new BadImageFormatException();
                    }
                    Command.all.Add((Command)instance);
                }
            } catch (FileNotFoundException e) {
                Server.ErrorLog(e);
                return command + ".dll does not exist in the DLL folder, or is missing a dependency. Details in the error log.";
            } catch (BadImageFormatException e) {
                Server.ErrorLog(e);
                return command + ".dll is not a valid assembly, or has an invalid dependency. Details in the error log.";
            } catch (PathTooLongException) {
                return "Class name is too long.";
            } catch (FileLoadException e) {
                Server.ErrorLog(e);
                return command + ".dll or one of its dependencies could not be loaded. Details in the error log.";
            } catch (InvalidCastException e) {
                //if the structure of the code is wrong, or it has syntax error or other code problems
                Server.ErrorLog(e);
                return command + ".dll has invalid code structure, please check code again for errors.";
            } catch (Exception e) {
                Server.ErrorLog(e);
                return "An unknown error occured and has been logged.";
            }
            return null;
        }
    }
}
