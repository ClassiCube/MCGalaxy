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
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Chatting {  
    public sealed class CmdEat : MessageCmd {
        public override string name { get { return "eat"; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message) {
            if (DateTime.UtcNow < p.NextEat) {
                Player.Message(p, "You're still full - you need to wait at least " +
                                   "10 seconds between snacks."); return;
            }
            if (Economy.Enabled && p.money < 1) {
                Player.Message(p, "You need to have at least 1 &3" + Server.moneys + 
                                   " %Sto purchase a snack."); return;
            }            
                      
        	TextFile eatFile = TextFile.Files["Eat"];
        	eatFile.EnsureExists();
            
        	string[] actions = eatFile.GetText();
            string action = "ate some food";
            if (actions.Length > 0)
                action = actions[new Random().Next(actions.Length)];
            
            if (!TryMessage(p, p.ColoredName + " %S" + action)) return;
            p.NextEat = DateTime.UtcNow.AddSeconds(10);
            if (Economy.Enabled)
                p.SetMoney(p.money - 1);  
        }
     
        public override void Help(Player p) {
            Player.Message(p, "%T/eat %H- Eats a random snack.");
            Player.Message(p, "%HIf economy is enabled, costs 1 &3" + Server.moneys);
        }
    }
}
