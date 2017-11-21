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
using System.Text;
using MCGalaxy.Tasks;
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Chatting {
    public sealed class Cmd8Ball : Command {
        public override string name { get { return "8ball"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool SuperUseable { get { return false; } }

        static DateTime nextUse;
        static TimeSpan delay = TimeSpan.FromSeconds(2);
        
        public override void Use(Player p, string message) {
            if (!MessageCmd.CanSpeak(p, name)) return;
            if (message.Length == 0) { Help(p); return; }
            
            TimeSpan delta = nextUse - DateTime.UtcNow;
            if (delta.TotalSeconds > 0) {
                Player.Message(p, "The 8-ball is still recharging, wait another {0} seconds.",
                               (int)Math.Ceiling(delta.TotalSeconds));
                return;
            }
            nextUse = DateTime.UtcNow.AddSeconds(10 + 2);
           
            StringBuilder builder = new StringBuilder(message.Length);
            foreach (char c in message) {
                if (Char.IsLetterOrDigit(c)) builder.Append(c);
            }
           
            string final = builder.ToString();
            Chat.MessageWhere("{0} %Sasked the &b8-Ball: &f{1}", Sees8Ball, p.ColoredName, message);
            Server.MainScheduler.QueueOnce(EightBallCallback, final, delay);
        }
        
        static void EightBallCallback(SchedulerTask task) {
            string final = (string)task.State;
            Random random = new Random(final.ToLower().GetHashCode());
            
            TextFile file = TextFile.Files["8ball"];
            file.EnsureExists();
            string[] messages = file.GetText();
            Chat.MessageWhere("The &b8-Ball %Ssays: &f{0}", Sees8Ball, messages[random.Next(messages.Length)]);
        }
        
        static bool Sees8Ball(Player p) {
            return !p.Ignores.All && !p.Ignores.EightBall && p.level.SeesServerWideChat && p.Chatroom == null;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/8ball [yes or no question]");
            Player.Message(p, "%HGet an answer from the all-knowing 8-Ball!");
        }
    }
}
