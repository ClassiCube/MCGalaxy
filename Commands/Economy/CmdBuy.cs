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
    
    /// <summary> Economy Beta v1.0 QuantumHive </summary>
    public sealed class CmdBuy : Command {
        public override string name { get { return "buy"; } }
        public override string shortcut { get { return "purchase"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool Enabled { get { return Economy.Enabled; } }
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            string[] parts = message.Split(' ');

            foreach (Item item in Economy.Items)
                foreach (string alias in item.Aliases)
            {
                if (parts[0].CaselessEquals(alias)) {
                    if (!item.Enabled) {
                        Player.SendMessage(p, "%c" + item.Name + "s are not enabled for the economy system."); return;
                    }
                    item.OnBuyCommand(this, p, message, parts); 
                    return;
                }
            }
            Help(p);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/buy <title/color/tcolor/rank/map> [value] <map name>");
            Player.SendMessage(p, "%Hvalue is either [title/color/tcolor/map_preset]");
            Player.SendMessage(p, "%Hmap name is only used for %T/buy map%H.");
            Player.SendMessage(p, "%HUse %T/store <type> %Hto see the cost of an item.");
        }
    }
}
