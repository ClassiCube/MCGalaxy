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
    public sealed class Cmd8Ball : Command2 {
        public override string name { get { return "8ball"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool SuperUseable { get { return false; } }

        static DateTime nextUse;
        static TimeSpan delay = TimeSpan.FromSeconds(2);
        
        public override void Use(Player p, string question, CommandData data) {
            if (!MessageCmd.CanSpeak(p, name)) return;
            if (question.Length == 0) { Help(p); return; }
            
            TimeSpan delta = nextUse - DateTime.UtcNow;
            if (delta.TotalSeconds > 0) {
                p.Message("The 8-ball is still recharging, wait another {0} seconds.",
                               (int)Math.Ceiling(delta.TotalSeconds));
                return;
            }
            nextUse = DateTime.UtcNow.AddSeconds(10 + 2);
           
            StringBuilder builder = new StringBuilder(question.Length);
            foreach (char c in question) {
                if (Char.IsLetterOrDigit(c)) builder.Append(c);
            }
           
            string msg = p.ColoredName + " &Sasked the &b8-Ball: &f" + question;
            Chat.Message(ChatScope.Global, msg, null, Filter8Ball);
            
            string final = builder.ToString();
            Server.MainScheduler.QueueOnce(EightBallCallback, final, delay);
        }
        
        static void EightBallCallback(SchedulerTask task) {
            string final = (string)task.State;
            Random random = new Random(final.ToLower().GetHashCode());
            
            TextFile file = TextFile.Files["8ball"];
            file.EnsureExists();
            string[] messages = file.GetText();
            
            string msg = "The &b8-Ball &Ssays: &f" + messages[random.Next(messages.Length)];
            Chat.Message(ChatScope.Global, msg, null, Filter8Ball);
        }
        
        static bool Filter8Ball(Player p, object arg) { return !p.Ignores.EightBall; }
        public override void Help(Player p) {
            p.Message("&T/8ball [yes or no question]");
            p.Message("&HGet an answer from the all-knowing 8-Ball!");
        }
    }
}
