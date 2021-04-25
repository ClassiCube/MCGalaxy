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
            string fileName;
            using (FileDialog dialog = new OpenFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.Filter = "Accepted File Types (*.cs, *.vb, *.dll)|*.cs;*.vb;*.dll|C# Source (*.cs)|*.cs|Visual Basic Source (*.vb)|*.vb|.NET Assemblies (*.dll)|*.dll";
                if (dialog.ShowDialog() != DialogResult.OK) return;
                fileName = dialog.FileName;
            }
            
            if (fileName.CaselessEnds(".dll")) {
                Assembly lib = IScripting.LoadAssembly(fileName);
                LoadCommands(lib);
                return;
            }
            
            ICompiler engine = fileName.CaselessEnds(".cs") ? ICompiler.CS : ICompiler.VB;
            if (!File.Exists(fileName)) return;
            
            ConsoleHelpPlayer p    = new ConsoleHelpPlayer();          
            CompilerResults result = engine.Compile(fileName, null);

            if (result.Errors.HasErrors) {
                ICompiler.SummariseErrors(result, p);
                string body = "\r\n\r\n" + Colors.StripUsed(p.Messages);
                Popup.Error("Compilation error. See logs/errors/compiler.log for more details." + body);
                return;
            }
            LoadCommands(result.CompiledAssembly);            
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
    }
}
