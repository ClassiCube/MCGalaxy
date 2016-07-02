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

namespace MCGalaxy.Commands {
    public sealed class Cmd8Ball : Command {
        public override string name { get { return "8ball"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public Cmd8Ball() { }
        static string[] answers = { "Not likely." , "Very likely." , "Impossible!" , "Probably." , "Ask again later." , "No." , "Maybe." };
        DateTime nextUse;
        
        public override void Use(Player p, string message) {
            if (String.IsNullOrEmpty(message)) { Help(p); return; }
            if (p.joker || p.muted) { Player.Message(p, "Cannot use 8ball while muted or jokered."); return; }
            
            TimeSpan delta = nextUse - DateTime.UtcNow;
            if (delta.TotalSeconds > 0) {
                Player.Message(p, "The 8-ball is still recharging, wait another {0} seconds.",
                               (int)Math.Ceiling(delta.TotalSeconds));
                return;
            }
            nextUse = DateTime.UtcNow.AddSeconds(10);
            
            Random random = new Random();
            Chat.GlobalChatLevel(p, p.color + "*" + Colors.StripColors(p.DisplayName) + " asked the question " + message, false);
            Chat.GlobalChatLevel(p, p.color + "*" + "The answer was " + answers[random.Next(answers.Length)], false);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/8ball (message)");
            Player.Message(p, "%HGives you a meaningless response to a question.");
        }
    }
}
