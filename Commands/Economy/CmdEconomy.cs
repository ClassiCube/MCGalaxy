/*
    Copyright 2011 MCForge
    
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
using MCGalaxy.Eco;

namespace MCGalaxy.Commands {
    
    /// <summary> Economy Beta v1.0 QuantumHive  </summary>
    public sealed class CmdEconomy : Command {
        public override string name { get { return "economy"; } }
        public override string shortcut { get { return "eco"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can setup the economy") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] raw = message.Split(' ');
            string[] args = { "", "", "", "", "", "", "", "" };
            for (int i = 0; i < Math.Min(args.Length, raw.Length); i++)
                args[i] = raw[i];
            HandleSetup(p, args);
        }
        
        void HandleSetup(Player p, string[] args) {
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "setup the economy."); return; }
            
            if (args[0].CaselessEq("apply")) {
                Economy.Load();
                Player.Message(p, "%aApplied changes");
            } else if (args[0].CaselessEq("enable")) {
                Player.Message(p, "%aThe economy system is now enabled");
                Economy.Enabled = true; Economy.Save();
            } else if (args[0].CaselessEq("disable")) {
                Player.Message(p, "%aThe economy system is now disabled");
                Economy.Enabled = false; Economy.Save();
            } else if (args[0].CaselessEq("help")) {
                SetupHelp(p, args);
            } else {
                Item item = Economy.GetItem(args[0]);
                if (item != null) {
                    item.OnSetupCommand(p, args);
                    Economy.Save(); return;
                }
                
                if (args[1] == "") { SetupHelp(p, args); return; }
                Player.Message(p, "%cThat wasn't a valid command addition!");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%cMost commands have been removed from /economy, " +
                           "use the appropriate command from %T/help economy %cinstead.");
            if (CheckExtraPerm(p)) {
                Player.Message(p, "%T/eco <type> %H- to setup economy");
                Player.Message(p, "%T/eco help %H- get more specific help for setting up the economy");
            }
        }

        void SetupHelp(Player p, string[] args) {
            if (args[1] == "") {
                Player.Message(p, "%T/eco apply %H- Reload changes made to 'economy.properties'");
                Player.Message(p, "%T/eco enable/disable %H- Enables/disables the economy system");
                Player.Message(p, "%T/eco help [item] %H- Outputs help for setting up that item");
                Player.Message(p, "   %HAll items: %S" + Economy.Items.Join(item => item.Name));
            } else {
                Item item = Economy.GetItem(args[1]);
                if (item == null) { Player.Message(p, "No item by that name, see %T/eco help %Sfor a list of items."); return; }
                item.OnSetupCommandHelp(p);
            }
        }
    }
}
