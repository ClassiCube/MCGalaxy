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
using System.Windows.Forms;
using MCGalaxy.Commands;
using MCGalaxy.Gui.Popups;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        
        ItemPermsHelper commandItems = new ItemPermsHelper();
        ComboBox[] commandExtraBoxes;
        Label[] commandExtraLabels;
        Command cmd;
        
        // need to keep a list of changed command perms, because we don't want
        // to modify the server's live permissions if user clicks 'discard'
        CommandPerms commandPermsOrig, commandPermsCopy;
        List<CommandExtraPerms> extraPermsList;
        List<CommandPerms> commandPermsChanged = new List<CommandPerms>();
        List<CommandExtraPerms> commandExtraPermsChanged = new List<CommandExtraPerms>();
        
        void LoadCommands() {
            cmd_list.Items.Clear();
            List<Command> all = Command.CopyAll();
            all.Sort((a, b) => a.name.CompareTo(b.name));
            
            foreach (Command cmd in all) {
                cmd_list.Items.Add(cmd.name);
            }

            commandItems.GetCurPerms = CommandGetOrAddPermsChanged;
            if (cmd_list.SelectedIndex == -1)
                cmd_list.SelectedIndex = 0;
        }
        
        void SaveCommands() {
            if (!CommandsChanged()) { LoadCommands(); return; }
            
            foreach (CommandPerms changed in commandPermsChanged) {
                CommandPerms.Set(changed.CmdName, changed.MinRank,
                                 changed.Allowed, changed.Disallowed);
            }            
            foreach (CommandExtraPerms changed in commandExtraPermsChanged) {
                CommandExtraPerms orig = CommandExtraPerms.Find(changed.CmdName, changed.Num);
                orig.MinRank = changed.MinRank;
            }
            
            CommandExtraPerms.Save();
            CommandPerms.Save();
            CommandPerms.Load();
            LoadCommands();
        }
        
        bool CommandsChanged() {
            return commandExtraPermsChanged.Count > 0 || commandPermsChanged.Count > 0;
        }        
        
        void cmd_list_SelectedIndexChanged(object sender, EventArgs e) {
            string cmdName = cmd_list.SelectedItem.ToString();        
            CommandInitSpecificArrays();          
            cmd = Command.Find(cmdName);
            if (cmd == null) return;
            
            commandPermsOrig = CommandPerms.Find(cmdName);
            commandPermsCopy = commandPermsChanged.Find(p => p.CmdName.CaselessEq(cmdName));
            
            // fix for when command is added to server but doesn't have permissions defined
            if (commandPermsOrig == null) {
                commandPermsOrig = new CommandPerms(cmdName, cmd.defaultRank, null, null);
            }
            
            commandItems.SupressEvents = true;
            CommandInitExtraPerms();
            CommandPerms perms = commandPermsCopy != null ? commandPermsCopy : commandPermsOrig;
            commandItems.Update(perms);
        }
        
        void CommandInitSpecificArrays() {
            if (commandItems.MinBox != null) return;
            commandItems.MinBox = cmd_cmbMin;
            commandItems.AllowBoxes = new ComboBox[] { cmd_cmbAlw1, cmd_cmbAlw2, cmd_cmbAlw3 };
            commandItems.DisallowBoxes = new ComboBox[] { cmd_cmbDis1, cmd_cmbDis2, cmd_cmbDis3 };
            commandItems.FillInitial();
            
            commandExtraBoxes = new ComboBox[] { cmd_cmbExtra1, cmd_cmbExtra2, cmd_cmbExtra3, 
                cmd_cmbExtra4, cmd_cmbExtra5, cmd_cmbExtra6, cmd_cmbExtra7 };
            commandExtraLabels = new Label[] { cmd_lblExtra1, cmd_lblExtra2, cmd_lblExtra3,
                cmd_lblExtra4, cmd_lblExtra5, cmd_lblExtra6, cmd_lblExtra7 };
            GuiPerms.FillRanks(commandExtraBoxes, false);
        }
        
        ItemPerms CommandGetOrAddPermsChanged() {
            if (commandPermsCopy != null) return commandPermsCopy;
            commandPermsCopy = commandPermsOrig.Copy();
            commandPermsChanged.Add(commandPermsCopy);
            return commandPermsCopy;
        }
        
        
        void cmd_cmbMin_SelectedIndexChanged(object sender, EventArgs e) {
            commandItems.OnMinRankChanged((ComboBox)sender);
        }
        
        void cmd_cmbSpecific_SelectedIndexChanged(object sender, EventArgs e) {
            commandItems.OnSpecificChanged(((ComboBox)sender));
        }
        
        void cmd_btnHelp_Click(object sender, EventArgs e) {
            GetHelp(cmd_list.SelectedItem.ToString());
        }
        
        void cmd_btnCustom_Click(object sender, EventArgs e) {
            using (CustomCommands form = new CustomCommands()) {
                form.ShowDialog();
            }
        }
        
        
        void CommandInitExtraPerms() {
            extraPermsList = CommandExtraPerms.FindAll(cmd.name);
            for (int i = 0; i < commandExtraBoxes.Length; i++) {
                commandExtraBoxes[i].Visible = false;
                commandExtraLabels[i].Visible = false;
            }            
            if (cmd.ExtraPerms == null) extraPermsList.Clear();
            
            int height = 12;
            for (int i = 0; i < extraPermsList.Count; i++) {
                CommandExtraPerms perms = LookupExtraPerms(extraPermsList[i].CmdName, extraPermsList[i].Num);
                if (perms == null) perms = extraPermsList[i];
                
                GuiPerms.SetDefaultIndex(commandExtraBoxes[i], perms.MinRank);
                commandExtraBoxes[i].Visible = true;
                commandExtraLabels[i].Text = "+ " + perms.Desc;
                commandExtraLabels[i].Visible = true;
                height = commandExtraBoxes[i].Bottom + 12;
            }
            cmd_grpExtra.Visible = extraPermsList.Count > 0;
            cmd_grpExtra.Height = height;
        }
        
        CommandExtraPerms LookupExtraPerms(string cmdName, int number) {
            return commandExtraPermsChanged.Find(
                p => p.CmdName == cmdName && p.Num == number);
        }
        
        void cmd_cmbExtra_SelectedIndexChanged(object sender, EventArgs e) {
            ComboBox box = (ComboBox)sender;
            if (commandItems.SupressEvents) return;
            int idx = box.SelectedIndex;
            if (idx == -1) return;
            
            int boxIdx = Array.IndexOf<ComboBox>(commandExtraBoxes, box);
            CommandExtraPerms orig = extraPermsList[boxIdx];
            CommandExtraPerms copy = LookupExtraPerms(orig.CmdName, orig.Num);
            
            if (copy == null) {
                copy = orig.Copy();
                commandExtraPermsChanged.Add(copy);
            }            
            copy.MinRank = GuiPerms.RankPerms[idx];
        }
    }
}
