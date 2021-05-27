/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Windows.Forms;
using MCGalaxy.Scripting;

namespace MCGalaxy.Gui.Popups {
    public partial class CustomCommands : Form {
        
        public CustomCommands() {
            InitializeComponent();

            //Sigh. I wish there were SOME event to help me.
            foreach (Command cmd in Command.allCmds) {
                if (!Command.IsCore(cmd)) lstCommands.Items.Add(cmd.name);
            }
        }
        
        void CreateCommand(ICompiler engine) {
            string cmdName = txtCmdName.Text.Trim();
            if (cmdName.Length == 0) {
                Popup.Warning("Command must have a name"); return;
            }
            
            string path = engine.CommandPath(cmdName);
            if (File.Exists(path)) {
                Popup.Warning("Command already exists"); return;
            }
            
            try {
                string source = engine.GenExampleCommand(cmdName);
                File.WriteAllText(path, source);
            } catch (Exception ex) {
                Logger.LogError(ex);
                Popup.Error("Failed to generate command. Check error logs for more details.");
                return;
            }
            Popup.Message("Command Cmd" + cmdName + engine.FileExtension + " created.");
        }
        
        void btnCreateCS_Click(object sender, EventArgs e) { CreateCommand(ICompiler.CS); }
        void btnCreateVB_Click(object sender, EventArgs e) { CreateCommand(ICompiler.VB); }
        
        void btnLoad_Click(object sender, EventArgs e) {
            Assembly lib;
            string path;
            
            using (FileDialog dialog = new OpenFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.Filter = GetFilterText();
                
                if (dialog.ShowDialog() != DialogResult.OK) return;
                path = dialog.FileName;
            }
            if (!File.Exists(path)) return;
            
            if (path.CaselessEnds(".dll")) {
                lib = IScripting.LoadAssembly(path);
            } else {
                lib = CompileCommands(path);
            }
            
            if (lib == null) return;
            LoadCommands(lib);
        }

        void btnUnload_Click(object sender, EventArgs e) {
            string cmdName = lstCommands.SelectedItem.ToString();
            Command cmd = Command.Find(cmdName);
            if (cmd == null) {
                Popup.Warning("Command " + cmdName + " is not loaded."); return;
            }

            lstCommands.Items.Remove(cmd.name);
            Command.Unregister(cmd);
            Popup.Message("Command successfully unloaded.");
        }
        
        void lstCommands_SelectedIndexChanged(object sender, EventArgs e) {
            btnUnload.Enabled = lstCommands.SelectedIndex != -1;
        }
        
        
        static ICompiler GetCompiler(string path) {
            foreach (ICompiler c in ICompiler.Compilers) {
                if (path.CaselessEnds(c.FileExtension)) return c;
            }
            return null;
        }
        
        Assembly CompileCommands(string path) {
            ICompiler compiler = GetCompiler(path);
            if (compiler == null) {
                Popup.Warning("Unsupported file '" + path + "'");
                return null;
            }
            
            ConsoleHelpPlayer p    = new ConsoleHelpPlayer();
            CompilerResults result = compiler.Compile(path, null);
            if (!result.Errors.HasErrors) return result.CompiledAssembly;
            
            ICompiler.SummariseErrors(result, p);
            string body = "\r\n\r\n" + Colors.StripUsed(p.Messages);
            Popup.Error("Compilation error. See logs/errors/compiler.log for more details." + body);
            return null;
        }
        
        void LoadCommands(Assembly assembly) {
            List<Command> commands = IScripting.LoadTypes<Command>(assembly);
            if (commands == null) {
                Popup.Error("Error compiling files. Check logs for more details"); return;
            }
            
            for (int i = 0; i < commands.Count; i++) {
                Command cmd = commands[i];

                if (lstCommands.Items.Contains(cmd.name)) {
                    Popup.Warning("Command " + cmd.name + " already exists, so was not loaded");
                    continue;
                }

                lstCommands.Items.Add(cmd.name);
                Command.Register(cmd);
                Logger.Log(LogType.SystemActivity, "Added " + cmd.name + " to commands");
            }
        }
        
        
        static string ListCompilers(StringFormatter<ICompiler> formatter) {
            return ICompiler.Compilers.Join(formatter, "");
        }
               
        static string GetFilterText() {
            StringBuilder sb = new StringBuilder();
            // Returns e.g. "Accepted File Types (*.cs, *.dll)|*.cs;*.dll|C# Source (*.cs)|*.cs|.NET Assemblies (*.dll)|*.dll";
            
            sb.AppendFormat("Accepted File Types ({0}*.dll)|",
                            ListCompilers(c => string.Format("*{0}, ", c.FileExtension)));
            
            sb.AppendFormat("{0}*.dll|",
                            ListCompilers(c => string.Format("*{0};", c.FileExtension)));
            
            sb.AppendFormat("{0}.NET Assemblies (*.dll)|*.dll",
                            ListCompilers(c => string.Format("{0} Source (*{1})|*{1}|", c.FullName, c.FileExtension)));
            return sb.ToString();
        }
    }
}
