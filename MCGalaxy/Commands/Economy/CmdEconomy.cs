/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Commands.Economic {
    public sealed class CmdEconomy : Command {
        public override string name { get { return "economy"; } }
        public override string shortcut { get { return "eco"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can setup the economy") }; }
        }
        
        public override void Use(Player p, string message)
        {
            string[] raw = message.SplitSpaces();
            string[] args = new string[] { "", "", "", "", "", "", "", "" };
            for (int i = 0; i < Math.Min(args.Length, raw.Length); i++)
                args[i] = raw[i];
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return; }
            
            if (args[0].CaselessEq("apply")) {
                Economy.Load();
                Player.Message(p, "Reloaded economy items from disc.");
            } else if (args[0].CaselessEq("enable")) {
                Player.Message(p, "Economy is now &aenabled");
                Economy.Enabled = true; Economy.Save();
            } else if (args[0].CaselessEq("disable")) {
                Player.Message(p, "Economy is now &cdisabled");
                Economy.Enabled = false; Economy.Save();
            } else {
                Item item = Economy.GetItem(args[0]);
                if (item != null) {
                    item.Setup(p, args);
                    Economy.Save();
                } else if (args[1] == "") {
                    Help(p);
                } else {
                    Help(p, args[1]);
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/eco apply %H- Reload changes made to 'economy.properties'.");
            Player.Message(p, "%T/eco enable/disable %H- Enables/disables the economy system.");
            Player.Message(p, "%T/eco help [item] %H- Outputs help for setting up that item.");
            Player.Message(p, "   %HAll items: %S" + Economy.Items.Join(item => item.Name));
        }
        
        public override void Help(Player p, string message) {
            Item item = Economy.GetItem(message);
            if (item == null) {
                Player.Message(p, "No item has that name, see %T/eco help %Sfor a list of items.");
            } else {
                item.OnSetupCommandHelp(p);
            }
        }
    }
}
