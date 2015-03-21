/*
	Copyright 2010 MCSLawl team - Written by Valek (Modified by fenderrock87 for use with MCForge)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Text;

namespace MCLawl
{
    static class Scripting
    {
        private static CodeDomProvider compiler = CodeDomProvider.CreateProvider("CSharp");
        private static CompilerParameters parameters = new CompilerParameters();
        private static CompilerResults results;
        private static string sourcepath = "extra/commands/source/";
        private static string dllpath = "extra/commands/dll/";

        /// <summary>
        /// Creates a new, empty command class.
        /// </summary>
        public static void CreateNew(string CmdName)
        {
            if (!Directory.Exists(sourcepath))
            {
                Directory.CreateDirectory(sourcepath);
            }
            
            StreamWriter sw = new StreamWriter(File.Create(sourcepath + "Cmd" + CmdName + ".cs"));
            sw.Write(
                "/*" + Environment.NewLine +
                "\tAuto-generated command skeleton class." + Environment.NewLine +
                Environment.NewLine +
                "\tUse this as a basis for custom commands implemented via the MCForge scripting framework." + Environment.NewLine +
                "\tFile and class should be named a specific way.  For example, /update is named 'CmdUpdate.cs' for the file, and 'CmdUpdate' for the class." + Environment.NewLine +
                "*/" + Environment.NewLine +
                Environment.NewLine +
                "// Add any other using statements you need up here, of course." + Environment.NewLine +
                "// As a note, MCForge is designed for .NET 3.5." + Environment.NewLine +
                "using System;" + Environment.NewLine +
                Environment.NewLine +
                "namespace MCLawl" + Environment.NewLine +
                "{" + Environment.NewLine +
                "\tpublic class " + ClassName(CmdName) + " : Command" + Environment.NewLine +
                "\t{" + Environment.NewLine +
                "\t\t// The command's name, in all lowercase.  What you'll be putting behind the slash when using it." + Environment.NewLine +
                "\t\tpublic override string name { get { return \"" + CmdName.ToLower() + "\"; } }" + Environment.NewLine +
                Environment.NewLine +
                "\t\t// Command's shortcut (please take care not to use an existing one, or you may have issues." + Environment.NewLine +
                "\t\tpublic override string shortcut { get { return \"\"; } }" + Environment.NewLine +
                Environment.NewLine +
                "\t\t// Determines which submenu the command displays in under /help." + Environment.NewLine +
                "\t\tpublic override string type { get { return \"other\"; } }" + Environment.NewLine +
                Environment.NewLine +
                "\t\t// Determines whether or not this command can be used in a museum.  Block/map altering commands should be made false to avoid errors." + Environment.NewLine +
                "\t\tpublic override bool museumUsable { get { return false; } }" + Environment.NewLine +
                Environment.NewLine +
                "\t\t// Determines the command's default rank.  Valid values are:" + Environment.NewLine +
                "\t\t// LevelPermission.Nobody, LevelPermission.Banned, LevelPermission.Guest" + Environment.NewLine +
                "\t\t// LevelPermission.Builder, LevelPermission.AdvBuilder, LevelPermission.Operator, LevelPermission.Admin" + Environment.NewLine +
                "\t\tpublic override LevelPermission defaultRank { get { return LevelPermission.Banned; } }" + Environment.NewLine +
                Environment.NewLine +
                "\t\t// This is where the magic happens, naturally." + Environment.NewLine +
                "\t\t// p is the player object for the player executing the command.  message is everything after the command invocation itself." + Environment.NewLine +
                "\t\tpublic override void Use(Player p, string message)" + Environment.NewLine +
                "\t\t{" + Environment.NewLine +
                "\t\t\tPlayer.SendMessage(p, \"Hello World!\");" + Environment.NewLine +
                "\t\t}" + Environment.NewLine +
                Environment.NewLine +
                "\t\t// This one controls what happens when you use /help [commandname]." + Environment.NewLine +
                "\t\tpublic override void Help(Player p)" + Environment.NewLine +
                "\t\t{" + Environment.NewLine +
                "\t\t\tPlayer.SendMessage(p, \"/" + CmdName.ToLower() + " - Does stuff.  Example command.\");" + Environment.NewLine +
                "\t\t}" + Environment.NewLine +
                "\t}" + Environment.NewLine +
                "}");
            sw.Dispose();
        }

        /// <summary>
        /// Compiles a written function from source into a DLL.
        /// </summary>
        /// <param name="commandName">Name of the command file to be compiled (without the extension)</param>
        /// <returns>True on successful compile, false on failure.</returns>
        public static bool Compile(string commandName)
        {
            string divider = new string('-', 25);
            if (!File.Exists(sourcepath + "Cmd" + commandName + ".cs"))
            {
                bool check = File.Exists("logs/errors/compiler.log");
                StreamWriter errlog = new StreamWriter("logs/errors/compiler.log", check);
                if (check)
                {
                    errlog.WriteLine();
                    errlog.WriteLine(divider);
                    errlog.WriteLine();
                }
                errlog.WriteLine("File not found: Cmd" + commandName + ".cs");
                errlog.Dispose();
                return false;
            }
            if (!Directory.Exists(dllpath))
            {
                Directory.CreateDirectory(dllpath);
            }
            parameters.GenerateExecutable = false;
            parameters.MainClass = commandName;
            parameters.OutputAssembly = dllpath + "Cmd" + commandName + ".dll";
            parameters.ReferencedAssemblies.Add("MCLawl_.dll");
            StreamReader sr = new StreamReader(sourcepath + "cmd" + commandName + ".cs");
            results = compiler.CompileAssemblyFromSource(parameters, sr.ReadToEnd());
            sr.Dispose();
            switch (results.Errors.Count)
            {
                case 0:
                    return true;
                case 1:
                    CompilerError error = results.Errors[0];
                    bool exists = (File.Exists("logs/errors/compiler.log")) ? true : false;
                    StringBuilder sb = new StringBuilder();
                    if (exists)
                    {
                        sb.AppendLine();
                        sb.AppendLine(divider);
                        sb.AppendLine();
                    }
                    sb.AppendLine("Error " + error.ErrorNumber);
                    sb.AppendLine("Message: " + error.ErrorText);
                    sb.AppendLine("Line: " + error.Line);
                    StreamWriter sw = new StreamWriter("logs/errors/compiler.log", exists);
                    sw.Write(sb.ToString());
                    sw.Dispose();
                    return false;
                default:
                    exists = (File.Exists("logs/errors/compiler.log")) ? true : false;
                    sb = new StringBuilder();
                    bool start = true;
                    if(exists)
                    {
                        sb.AppendLine();
                        sb.AppendLine(divider);
                        sb.AppendLine();
                    }
                    foreach (CompilerError err in results.Errors)
                    {
                        if (!start)
                        {
                            sb.AppendLine();
                            sb.AppendLine(divider);
                            sb.AppendLine();
                        }
                        sb.AppendLine("Error #" + err.ErrorNumber);
                        sb.AppendLine("Message: " + err.ErrorText);
                        sb.AppendLine("Line: " + err.Line);
                        if (start)
                        {
                            start = false;
                        }
                    }
                    sw = new StreamWriter("logs/errors/compiler.log", exists);
                    sw.Write(sb.ToString());
                    sw.Dispose();
                    return false;
            }
        }

        public static void Autoload()
        {
            if (!File.Exists("text/cmdautoload.txt"))
            {
                File.Create("text/cmdautoload.txt");
                return;
            }
            string[] autocmds = File.ReadAllLines("text/cmdautoload.txt");
            foreach (string cmd in autocmds)
            {
                if (cmd == "")
                {
                    continue;
                }
                string error = Scripting.Load("Cmd" + cmd.ToLower());
                if (error != null)
                {
                    Server.s.Log(error);
                    error = null;
                    continue;
                }
                Server.s.Log("AUTOLOAD: Loaded " + cmd.ToLower() + ".dll");
            }
        }

        /// <summary>
        /// Loads a command for use on the server.
        /// </summary>
        /// <param name="command">Name of the command to be loaded (make sure it's prefixed by Cmd before bringing it in here or you'll have problems).</param>
        /// <returns>Error string on failure, null on success.</returns>
        public static string Load(string command)
        {
            if (command.Length < 3 || command.Substring(0, 3).ToLower() != "cmd")
            {
                return "Invalid command name specified.";
            }
            try
            {
                Assembly asm = Assembly.LoadFrom("extra/commands/dll/" + command + ".dll");
                Type type = asm.GetTypes()[0];
                object instance = Activator.CreateInstance(type);
                Command.all.Add((Command)instance);
            }
            catch (FileNotFoundException e)
            {
                Server.ErrorLog(e);
                return command + ".dll does not exist in the DLL folder, or is missing a dependency.  Details in the error log.";
            }
            catch (BadImageFormatException e)
            {
                Server.ErrorLog(e);
                return command + ".dll is not a valid assembly, or has an invalid dependency.  Details in the error log.";
            }
            catch (PathTooLongException)
            {
                return "Class name is too long.";
            }
            catch (FileLoadException e)
            {
                Server.ErrorLog(e);
                return command + ".dll or one of its dependencies could not be loaded.  Details in the error log.";
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                return "An unknown error occured and has been logged.";
            }
            return null;
        }

        /// <summary>
        /// Creates a class name from the given string.
        /// </summary>
        /// <param name="name">String to convert to an MCForge class name.</param>
        /// <returns>Successfully generated class name.</returns>
        private static string ClassName(string name)
        {
            char[] conv = name.ToCharArray();
            conv[0] = char.ToUpper(conv[0]);
            return "Cmd" + new string(conv);
        }
    }
}
