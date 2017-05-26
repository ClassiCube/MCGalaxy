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
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MCGalaxy.Commands;
using MCGalaxy.Scripting;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
		
        void listCommands_SelectedIndexChanged(object sender, EventArgs e) {
            string cmdName = listCommands.SelectedItem.ToString();
            CommandPerms perms = CommandPerms.Find(cmdName);
            txtCmdLowest.Text = (int)perms.MinRank + "";

            bool foundOne = false;
            txtCmdDisallow.Text = "";
            foreach ( LevelPermission perm in perms.Disallowed ) {
                foundOne = true;
                txtCmdDisallow.Text += "," + (int)perm;
            }
            if ( foundOne ) txtCmdDisallow.Text = txtCmdDisallow.Text.Remove(0, 1);

            foundOne = false;
            txtCmdAllow.Text = "";
            foreach ( LevelPermission perm in perms.Allowed ) {
                foundOne = true;
                txtCmdAllow.Text += "," + (int)perm;
            }
            if ( foundOne ) txtCmdAllow.Text = txtCmdAllow.Text.Remove(0, 1);
        }
		
        void txtCmdLowest_TextChanged(object sender, EventArgs e) {
            fillLowest(ref txtCmdLowest, ref storedCommands[listCommands.SelectedIndex].MinRank);
        }
        void txtCmdDisallow_TextChanged(object sender, EventArgs e) {
            fillAllowance(ref txtCmdDisallow, ref storedCommands[listCommands.SelectedIndex].Disallowed);
        }
        void txtCmdAllow_TextChanged(object sender, EventArgs e) {
            fillAllowance(ref txtCmdAllow, ref storedCommands[listCommands.SelectedIndex].Allowed);
        }

        void btnCmdHelp_Click(object sender, EventArgs e) {
            getHelp(listCommands.SelectedItem.ToString());
        }
		
		

        void CrtCustCmd_Click(object sender, EventArgs e) {
            if ( txtCommandName.Text != null ) {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                    MessageBox.Show("Sorry, That command already exists!!");
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text);
                    MessageBox.Show("Command Created!!");
                }
            }
            else {
                MessageBox.Show("You didnt specify a name for the command!!");
            }
        }

        void CompileCustCmd_Click(object sender, EventArgs e) {
            if ( txtCommandName.Text != null ) {
                if ( File.Exists("extra/commands/dll/Cmd" + txtCommandName.Text + ".dll") ) {
                    MessageBox.Show("Sorry, That command already exists!!");
                }
                else {
                    Command.all.Find("compile").Use(null, txtCommandName.Text);
                    MessageBox.Show("Command Compiled!!");
                }
            }
            else {
                MessageBox.Show("You didnt specify a name for the command!!");
            }
        }

        void btnCreate_Click(object sender, EventArgs e) {
            if(String.IsNullOrEmpty(txtCommandName.Text.Trim())) {
                MessageBox.Show ( "Command must have a name" );
                return;
            }

            if ( radioButton1.Checked ) {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") || File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                    MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text.ToLower() + " vb");
                    MessageBox.Show("New Command Created: " + txtCommandName.Text.ToLower() + " Created.");
                }
            } else {
                if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") || File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") ) {
                    MessageBox.Show("Command already exists", "", MessageBoxButtons.OK);
                }
                else {
                    Command.all.Find("cmdcreate").Use(null, txtCommandName.Text.ToLower());
                    MessageBox.Show("New Command Created: " + txtCommandName.Text.ToLower() + " Created.");
                }
            }
        }

        void btnLoad_Click(object sender, EventArgs e) {
            Command[] commands = null;
            using (FileDialog dialog = new OpenFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.Filter = "Accepted File Types (*.cs, *.vb, *.dll)|*.cs;*.vb;*.dll|C# Source (*.cs)|*.cs|Visual Basic Source (*.vb)|*.vb|.NET Assemblies (*.dll)|*.dll";
                if (dialog.ShowDialog() != DialogResult.OK) return;

                string fileName = dialog.FileName;
                if (fileName.EndsWith(".dll")) {
                    Assembly lib = Assembly.LoadFile(fileName);
                    commands = IScripting.LoadFrom(lib).ToArray();
                } else {
                    IScripting engine = fileName.EndsWith(".cs") ? IScripting.CS : IScripting.VB;
                    if (!File.Exists(fileName)) return;
                    
                    CompilerParameters args = new CompilerParameters();
                    args.GenerateInMemory = true;
                    var result = engine.CompileSource(File.ReadAllText(fileName), args);
                    if (result == null) { MessageBox.Show ( "Error compiling files" ); return; }

                    if (result.Errors.HasErrors) {
                    	foreach (CompilerError err in result.Errors) {
                            Server.s.ErrorCase("Error #" + err.ErrorNumber);
                            Server.s.ErrorCase("Message: " + err.ErrorText);
                            Server.s.ErrorCase("Line: " + err.Line);
                            Server.s.ErrorCase( "=================================" );
                        }
                        MessageBox.Show("Error compiling from source. Check logs for error");
                        return;
                    }
                    commands = IScripting.LoadFrom(result.CompiledAssembly).ToArray();
                }
            }

            if (commands == null) { MessageBox.Show("Error compiling files"); return; }
            for (int i = 0; i < commands.Length; i++) {
                Command cmd = commands[i];

                if (lstCommands.Items.Contains(cmd.name)) {
                    MessageBox.Show("Command " + cmd.name + " already exists. As a result, it was not loaded");
                    continue;
                }

                lstCommands.Items.Add(cmd.name);
                Command.all.Add(cmd);
                Server.s.Log("Added " + cmd.name + " to commands");
            }
            CommandPerms.Load();
        }

        void btnUnload_Click(object sender, EventArgs e) {
            Command cmd = Command.all.Find(lstCommands.SelectedItem.ToString());
            if (cmd == null) {
                MessageBox.Show(txtCommandName.Text + " is not a valid or loaded command.", ""); return;
            }

            lstCommands.Items.Remove( cmd.name );
            Command.all.Remove(cmd);
            CommandPerms.Load();
            MessageBox.Show("Command was successfully unloaded.", "");
        }

        void btnDiscardcmd_Click(object sender, EventArgs e) {
            switch ( MessageBox.Show("Are you sure you want to discard this whole file?", "Discard?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ) {
                case DialogResult.Yes:
                    if ( radioButton1.Checked ) {
                        if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".vb") ) {
                            File.Delete("extra/commands/source/Cmd" + txtCommandName.Text + ".vb");
                        }
                        else { MessageBox.Show("File: " + txtCommandName.Text + ".vb Doesnt Exist."); }
                    } else {
                        if ( File.Exists("extra/commands/source/Cmd" + txtCommandName.Text + ".cs") ) {
                            File.Delete("extra/commands/source/Cmd" + txtCommandName.Text + ".cs");
                        }
                        else { MessageBox.Show("File: " + txtCommandName.Text + ".cs Doesnt Exist."); }
                    }
                    break;

            }
        }
		
		void lstCommands_SelectedIndexChanged ( object sender, EventArgs e ) {
            btnUnload.Enabled = lstCommands.SelectedIndex != -1;
        }
		
		

        bool skipExtraPermChanges;
        int oldnumber;
        Command oldcmd;
        void listCommandsExtraCmdPerms_SelectedIndexChanged(object sender, EventArgs e) {
            SaveOldExtraCustomCmdChanges();
            Command cmd = Command.all.Find(listCommandsExtraCmdPerms.SelectedItem.ToString());
            oldcmd = cmd;
            skipExtraPermChanges = true;
            extracmdpermnumber.Maximum = MaxExtraPermissionNumber(cmd);
            extracmdpermnumber.ReadOnly = extracmdpermnumber.Maximum == 1;
            extracmdpermnumber.Value = 1;
            skipExtraPermChanges = false;
            
            ExtraPermSetDescriptions(cmd, 1);
            oldnumber = (int)extracmdpermnumber.Value;
        }
        
        int MaxExtraPermissionNumber(Command cmd) {
            var all = CommandExtraPerms.FindAll(cmd.name);
            int maxNum = 0;
            
            foreach (CommandExtraPerms perms in all) {
                maxNum = Math.Max(maxNum, perms.Number);
            }
            return maxNum;
        }

        void SaveOldExtraCustomCmdChanges() {
            if (oldcmd == null || skipExtraPermChanges) return;
            
            CommandExtraPerms.Find(oldcmd.name, oldnumber).MinRank = (LevelPermission)int.Parse(extracmdpermperm.Text);
            CommandExtraPerms.Save();
        }

        void extracmdpermnumber_ValueChanged(object sender, EventArgs e) {
            SaveOldExtraCustomCmdChanges();
            oldnumber = (int)extracmdpermnumber.Value;
            ExtraPermSetDescriptions(oldcmd, (int)extracmdpermnumber.Value);
        }
        
        void ExtraPermSetDescriptions(Command cmd, int number) {
            CommandExtraPerms perms =  CommandExtraPerms.Find(cmd.name, number);
            extracmdpermdesc.Text = perms.Description;
            extracmdpermperm.Text = ((int)perms.MinRank).ToString();
        }
        
        void LoadExtraCmdCmds() {
            listCommandsExtraCmdPerms.Items.Clear();
            foreach ( Command cmd in Command.all.commands ) {
                if ( CommandExtraPerms.Find(cmd.name) != null ) {
                    listCommandsExtraCmdPerms.Items.Add(cmd.name);
                }
            }
            listCommandsExtraCmdPerms.Sorted = true;
            listCommandsExtraCmdPerms.Sorted = false;
        }
    }
}
