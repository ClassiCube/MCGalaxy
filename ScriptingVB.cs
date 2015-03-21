/*                                                           

 Copyright 2011 MCGalaxy
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy
{
	static class ScriptingVB
	{
		private static CodeDomProvider compiler = CodeDomProvider.CreateProvider("VisualBasic");
		private static CompilerParameters parameters = new CompilerParameters();
		private static CompilerResults results;
		private static string sourcepath = "extra/commands/source/";
		private static string dllpath = "extra/commands/dll/";
	
   
		public static void CreateNew(string CmdName)
		{
			if (!Directory.Exists(sourcepath))
			{
				Directory.CreateDirectory(sourcepath);
			}

			using (var sw = new StreamWriter(File.Create(sourcepath + "Cmd" + CmdName + ".vb")))
			{
				sw.Write("Imports MCGalaxy" + Environment.NewLine +
						 "'Auto-generated command skeleton class." + Environment.NewLine +
						 Environment.NewLine +
						 "'Use this as a basis for custom commands implemented via the MCGalaxy scripting framework." + Environment.NewLine +
						 "'File and class should be named a specific way.  For example, /update is named 'CmdUpdate.vb' for the file, and 'CmdUpdate' for the class." + Environment.NewLine +
						 "'" + Environment.NewLine +
						 Environment.NewLine +
						 "' Add any other using statements you need up here, of course." + Environment.NewLine +
						 "' As a note, MCGalaxy is designed for .NET 3.5." + Environment.NewLine +
			  
						 Environment.NewLine +
						 "Namespace MCGalaxy" + Environment.NewLine +
						 "\tPublic Class " + ClassName(CmdName) + Environment.NewLine +
						 "\t\tInherits Command " + Environment.NewLine +
						 "' The command's name, IN ALL LOWERCASE.  What you'll be putting behind the slash when using it." + Environment.NewLine +

						 "\t\tPublic Overrides ReadOnly Property name() As String" + Environment.NewLine +
						 "\t\t\tGet" + Environment.NewLine +
						 "\t\t\t\tReturn \"" + CmdName.ToLower() + "\"" + Environment.NewLine +
						 "\t\t\tEnd Get" + Environment.NewLine +
						 "\t\tEnd Property" + Environment.NewLine +
						 Environment.NewLine +
						 "' Command's shortcut (please take care not to use an existing one, or you may have issues."+  Environment.NewLine +
						 "\t\tPublic Overrides ReadOnly Property shortcut() As String" + Environment.NewLine +
						 "\t\t\tGet" + Environment.NewLine +
						 "\t\t\t\tReturn \"\"" + Environment.NewLine +
						 "\t\t\tEnd Get" + Environment.NewLine +
						 "\t\tEnd Property" + Environment.NewLine +
						 Environment.NewLine +
						 "' Determines which submenu the command displays in under /help." +   Environment.NewLine +
						 "\t\tPublic Overrides ReadOnly Property type() As String" + Environment.NewLine +
						 "\t\t\tGet" + Environment.NewLine +
						 "\t\t\t\tReturn \"other\"" + Environment.NewLine +
						 "\t\t\tEnd Get" + Environment.NewLine +
						 "\t\t End Property" + Environment.NewLine +
						 Environment.NewLine +
						 "' Determines whether or not this command can be used in a museum.  Block/map altering commands should be made false to avoid errors."+   Environment.NewLine +
						 "\t\tPublic Overrides ReadOnly Property museumUsable() As Boolean" + Environment.NewLine +
						 "\t\t\tGet" + Environment.NewLine +
						 "\t\t\t\tReturn False" + Environment.NewLine +
						 "\t\t\tEnd Get" + Environment.NewLine +
						 "\t\tEnd Property" + Environment.NewLine + 
						 Environment.NewLine +
						 "' Determines the command's default rank.  Valid values are:" +   Environment.NewLine + "' LevelPermission.Nobody, LevelPermission.Banned, LevelPermission.Guest" +
						 Environment.NewLine + "' LevelPermission.Builder, LevelPermission.AdvBuilder, LevelPermission.Operator, LevelPermission.Admin" +   Environment.NewLine +
						 "\t\tPublic Overrides ReadOnly Property defaultRank() As LevelPermission" + Environment.NewLine +
						 "\t\t\tGet" + Environment.NewLine +
						 "\t\t\t\tReturn LevelPermission.Banned" + Environment.NewLine +
						 "\t\t\tEnd Get" + Environment.NewLine +
						 "\t\tEnd Property" + Environment.NewLine +
						 Environment.NewLine +
						 "' This is where the magic happens, naturally." +   Environment.NewLine +
						 "' p is the player object for the player executing the command.  message is everything after the command invocation itself." +   Environment.NewLine +
						 "\t\tPublic Overrides Sub Use(p As Player, message As String)" + Environment.NewLine +
						 "\t\t\tPlayer.SendMessage(p, \"Hello World!\")" + Environment.NewLine +
						 "\t\tEnd Sub" + Environment.NewLine +
						 Environment.NewLine +
						 "' This one controls what happens when you use /help [commandname]." +   Environment.NewLine +
						 "\t\tPublic Overrides Sub Help(p As Player)" + Environment.NewLine +
						 "\t\t\tPlayer.SendMessage(p, \"/" + CmdName.ToLower() + " - Does stuff.  Example command.\")" + Environment.NewLine +

						 "\t\tEnd Sub" + Environment.NewLine +
						 "\tEnd Class" + Environment.NewLine +
						 "End Namespace"

					);

			}
		}

		

	   
		public static bool Compile(string commandName)
		{
			string divider = new string('-', 25);
			if (!File.Exists(sourcepath + "Cmd" + commandName + ".vb"))
			{
				bool check = File.Exists("logs/errors/compiler.log");
				StreamWriter errlog = new StreamWriter("logs/errors/compiler.log", check);
				if (check)
				{
					errlog.WriteLine();
					errlog.WriteLine(divider);
					errlog.WriteLine();
					
				}
				errlog.WriteLine("File not found: Cmd" + commandName + ".vb");
				
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
			parameters.ReferencedAssemblies.Add("MCGalaxy_.dll");
			StreamReader sr = new StreamReader(sourcepath + "cmd" + commandName + ".vb");
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
			if (!File.Exists("text/cmdautoloadVB.txt"))
			{
				File.Create("text/cmdautoloadVB.txt");
				return;
			}
			string[] autocmds = File.ReadAllLines("text/cmdautoloadVB.txt");
			foreach (string cmd in autocmds)
			{
				if (cmd == "")
				{
					continue;
				}
				string error = ScriptingVB.Load("Cmd" + cmd.ToLower());
				if (error != null)
				{
					Server.s.Log(error);
					error = null;
					continue;
				}
				Server.s.Log("AUTOLOAD: Loaded [VB] " + cmd.ToLower() + ".dll");
			}
		}

	   
		public static string Load(string command)
		{
			if (command.Length < 3 || command.Substring(0, 3).ToLower() != "cmd")
			{
				return "Invalid command name specified.";
			}
			try
			{

				Assembly asm = Assembly.LoadFrom("extra/commands/dll/" + command + ".dll");

				Type type = asm.GetTypes()[5];

				var instance = Activator.CreateInstance(type);

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
			
			catch (InvalidCastException e)
			{
				//if the structure of the code is wrong, or it has syntax error or other code problems
				Server.ErrorLog(e);
				return command + ".dll has invalid code structure, please check code again for errors.";
			}
			catch (Exception e)
			{
				Server.ErrorLog(e);
				return "An unknown error occured and has been logged.";
			}
			return null;
		}

		private static string ClassName(string name)
		{
			char[] conv = name.ToCharArray();
			conv[0] = char.ToUpper(conv[0]);
			return "Cmd" + new string(conv);
		}
	}
}
