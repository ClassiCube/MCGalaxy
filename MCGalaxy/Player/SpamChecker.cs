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
using MCGalaxy.Events;
using MCGalaxy.Tasks;

namespace MCGalaxy {
    internal sealed class SpamChecker {
        
        public SpamChecker(Player p) {
            this.p = p;
            blockLog = new List<DateTime>(Server.BlockSpamCount);
            chatLog = new List<DateTime>(Server.spamcounter);
            cmdLog = new List<DateTime>(Server.CmdSpamCount);
        }
        
        Player p;
        readonly object chatLock = new object(), cmdLock = new object();
        readonly List<DateTime> blockLog, chatLog, cmdLog;
        
        public void Clear() {
            blockLog.Clear();
            lock (chatLock)
                chatLog.Clear();
            lock (cmdLock)
                cmdLog.Clear();
        }
        
        public bool CheckBlockSpam() {
            if (p.ignoreGrief || !Server.BlockSpamCheck) return false;
            if (blockLog.AddSpamEntry(Server.BlockSpamCount, Server.BlockSpamInterval)) 
                return false;

            TimeSpan oldestDelta = DateTime.UtcNow - blockLog[0];
            Chat.MessageOps(p.ColoredName + " &cwas kicked for suspected griefing.");
            Server.s.Log(p.name + " was kicked for block spam (" + blockLog.Count
                         + " blocks in " + oldestDelta + " seconds)");
            p.Kick("You were kicked by antigrief system. Slow down.");
            return true;            
        }
        
        public bool CheckChatSpam() {
            Player.lastMSG = p.name;
            if (!Server.checkspam || Player.IsSuper(p)) return false;
            
            lock (chatLock) {
                if (chatLog.AddSpamEntry(Server.spamcounter, Server.spamcountreset)) 
                    return false;
                
                TimeSpan duration = TimeSpan.FromSeconds(Server.mutespamtime);
                ModAction action = new ModAction(p.name, null, ModActionType.Muted, "&0Auto mute for spamming", duration);
                OnModActionEvent.Call(action);
                return true;
            }
        }
        
        public bool CheckCommandSpam() {
            if (!Server.CmdSpamCheck || Player.IsSuper(p)) return false;
            
            lock (cmdLock) {
                if (cmdLog.AddSpamEntry(Server.CmdSpamCount, Server.CmdSpamInterval)) 
                    return false;
                
                Player.Message(p, "You have been blocked from using commands for " +
                              Server.CmdSpamBlockTime + " seconds due to spamming");
                p.cmdUnblocked = DateTime.UtcNow.AddSeconds(Server.CmdSpamBlockTime);
                return true;
            }
        }
    }
}
