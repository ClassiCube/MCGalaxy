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
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MCGalaxy.Commands;
using MCGalaxy.Scripting;

namespace MCGalaxy.Gui.Popups {
    public partial class CustomCommands : Form {
        
        public CustomCommands() {
            InitializeComponent();

            //Sigh. I wish there were SOME event to help me.
            foreach (Command cmd in Command.all.commands) {
                if (Command.core.commands.Contains(cmd)) continue;
                lstCommands.Items.Add(cmd.name);
            }
        }
        
        void btnCreate_Click(object sender, EventArgs e) {
            if (String.IsNullOrEmpty(txtCmdName.Text.Trim())) {
                MessageBox.Show("Command must have a name");
                return;
            }
            
            string cmdName = txtCmdName.Text.Trim().ToLower();
            IScripting engine = radVB.Checked ? IScripting.VB : IScripting.CS;
            if (engine.SourceFileExists(cmdName)) {
                MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                return;
            }
            
            try {
                engine.CreateNew(cmdName);
            } catch (Exception ex) {
                Logger.LogError(ex);
                MessageBox.Show("An error occurred creating the class file.", "", MessageBoxButtons.OK);
                return;
            }
            MessageBox.Show("Command: Cmd" + cmdName + engine.Ext + " created.");
        }

        void btnLoad_Click(object sender, EventArgs e) {
            List<Command> commands = null;
            using (FileDialog dialog = new OpenFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.Filter = "Accepted File Types (*.cs, *.vb, *.dll)|*.cs;*.vb;*.dll|C# Source (*.cs)|*.cs|Visual Basic Source (*.vb)|*.vb|.NET Assemblies (*.dll)|*.dll";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string fileName = dialog.FileName;
                if (fileName.EndsWith(".dll")) {
                    byte[] data = File.ReadAllBytes(fileName);
                    Assembly lib = Assembly.Load(data);
                    commands = IScripting.LoadTypes<Command>(lib);
                } else {
                    IScripting engine = fileName.EndsWith(".cs") ? IScripting.CS : IScripting.VB;
                    if (!File.Exists(fileName)) return;
                    
                    CompilerParameters args = new CompilerParameters();
                    args.GenerateInMemory = true;
                    var result = engine.CompileSource(File.ReadAllText(fileName), args);
                    if (result == null) { MessageBox.Show("Error compiling files"); return; }

                    if (result.Errors.HasErrors) {
                        foreach (CompilerError err in result.Errors) {
                            Logger.Log(LogType.Warning, "Error #" + err.ErrorNumber);
                            Logger.Log(LogType.Warning, "Message: " + err.ErrorText);
                            Logger.Log(LogType.Warning, "Line: " + err.Line);
                            Logger.Log(LogType.Warning, "=================================");
                        }
                        MessageBox.Show("Error compiling from source. Check logs for more details.");
                        return;
                    }
                    commands = IScripting.LoadTypes<Command>(result.CompiledAssembly);
                }
            }

            if (commands == null) { MessageBox.Show("Error compiling files"); return; }
            for (int i = 0; i < commands.Count; i++) {
                Command cmd = commands[i];

                if (lstCommands.Items.Contains(cmd.name)) {
                    MessageBox.Show("Command " + cmd.name + " already exists. As a result, it was not loaded");
                    continue;
                }

                lstCommands.Items.Add(cmd.name);
                Command.all.Add(cmd);
                Logger.Log(LogType.SystemActivity, "Added " + cmd.name + " to commands");
            }
            CommandPerms.Load();
        }

        void btnUnload_Click(object sender, EventArgs e) {
            string cmdName = lstCommands.SelectedItem.ToString();
            Command cmd = Command.all.Find(cmdName);
            if (cmd == null) {
                MessageBox.Show(cmdName + " is not a valid or loaded command.", ""); return;
            }

            lstCommands.Items.Remove( cmd.name );
            Command.all.Remove(cmd);
            CommandPerms.Load();
            MessageBox.Show("Command was successfully unloaded.", "");
        }
        
        void lstCommands_SelectedIndexChanged(object sender, EventArgs e) {
            btnUnload.Enabled = lstCommands.SelectedIndex != -1;
        }
    }
}
