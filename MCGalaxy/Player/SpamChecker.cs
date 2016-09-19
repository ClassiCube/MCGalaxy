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

namespace MCGalaxy {
    internal sealed class SpamChecker {
        
        public SpamChecker(Player p) {
            this.p = p;
            blockLog = new List<DateTime>(Player.spamBlockCount);
            chatLog = new List<DateTime>(Server.spamcounter);
        }
        
        Player p;
        readonly object chatLock = new object();
        readonly List<DateTime> blockLog, chatLog;
        
        public void Clear() {
            blockLog.Clear();
            lock (chatLock)
                chatLog.Clear();
            p = null;
        }
        
        public bool CheckBlockSpam() {
            if (p.ignoreGrief) return false;
            if (AddEntry(blockLog, Player.spamBlockCount, Player.spamBlockTimer)) return false;

            TimeSpan oldestDelta = DateTime.UtcNow - blockLog[0];
            p.Kick("You were kicked by antigrief system. Slow down.");
            Chat.MessageOps(p.ColoredName + " &cwas kicked for suspected griefing.");
            Server.s.Log(p.name + " was kicked for block spam (" + blockLog.Count
                         + " blocks in " + oldestDelta + " seconds)");
            return true;            
        }
        
        public bool CheckChatSpam() {
            Player.lastMSG = p.name;
            if (!Server.checkspam || p.ircNick != null) return false;
            
            lock (chatLock) {
                if (AddEntry(chatLog, Server.spamcounter, Server.spamcountreset)) return false;
                
                Command.all.Find("mute").Use(null, p.name);
                Chat.MessageAll("{0} %Shas been &0muted %Sfor spamming!", p.ColoredName);
                Server.MainScheduler.QueueOnce(UnmuteTask, p.name,
                                               TimeSpan.FromSeconds(Server.mutespamtime));
                return true;
            }
        }
        
        static bool AddEntry(List<DateTime> log, int maxEntries, int checkInterval) {
            DateTime now = DateTime.UtcNow;
            if (log.Count > 0 && log.Count >= maxEntries) 
                log.RemoveAt(0);
            log.Add(now);
            
            if (log.Count < maxEntries) return true;
            TimeSpan oldestDelta = now - log[0];
            return oldestDelta.TotalSeconds > checkInterval;
        }
        
        static void UnmuteTask(SchedulerTask task) {
            string name = (string)task.State;
            Player who = PlayerInfo.FindExact(name);
            
            if (who != null) {
                if (who.muted) Command.all.Find("mute").Use(null, who.name);
                Player.Message(who, "Remember, no &cspamming %Snext time!");
            } else {
                Server.muted.Remove(name);
                Server.muted.Save();
            }
        }
    }
}
