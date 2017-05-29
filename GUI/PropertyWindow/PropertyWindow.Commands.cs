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
using MCGalaxy.Gui.Popups;

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
        
        void cmd_btnCustom_Click(object sender, EventArgs e) {
            using (CustomCommands form = new CustomCommands()) {
                form.ShowDialog();
            }
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
