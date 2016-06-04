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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Commands {
    
    public sealed class CmdEat : Command {
        public override string name { get { return "eat"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        static char[] trimChars = {' '};
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            
            if (DateTime.UtcNow < p.NextEat) {
                Player.Message(p, "You're still full - you need to wait at least " +
                                   "10 seconds between snacks."); return;
            }
            if (Economy.Enabled && p.money < 1) {
                Player.Message(p, "You need to have at least 1 &3" + Server.moneys + 
                                   " %Sto purchase a snack."); return;
            }
            if (p.muted) {
                Player.Message(p, "You cannot use this command while muted."); return;
            }
            
            p.NextEat = DateTime.UtcNow.AddSeconds(10);
            if (Economy.Enabled) {
                p.money -= 1; p.OnMoneyChanged();
            }            
            if (!File.Exists("text/eatmessages.txt")) {
                File.WriteAllLines("text/eatmessages.txt", defMessages);
            }
            
            List<string> actions = CP437Reader.ReadAllLines("text/eatmessages.txt");
            string action = "ate some food";
            if (actions.Count > 0)
                action = actions[new Random().Next(actions.Count)];
            
            if (!p.level.worldChat) {
                Chat.GlobalChatLevel(p, "<Level>" + p.ColoredName + " %S" + action, false);
            } else {
                Player.SendChatFrom(p, p.ColoredName + " %S" + action, false);
            }      
        }
        
        static string[] defMessages = { "guzzled a grape", "chewed a cherry", "ate an avocado" };
        
        public override void Help(Player p) {
            Player.Message(p, "%T/eat %H- Eats a random snack.");
            Player.Message(p, "%HIf economy is enabled, costs 1 &3" + Server.moneys);
        }
    }
}
