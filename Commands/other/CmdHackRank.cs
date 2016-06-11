/*
    Copyright 2011 MCForge
    
    Made originally by 501st_commander, in something called SharpDevelop.
    Made into a safe and reasonabal command by EricKilla, in Visual Studio 2010.
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdHackRank : Command {
        public override string name { get { return "hackrank"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdHackRank() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (p == null) { MessageInGameOnly(p); return; }
            
            if (p.hackrank) {
                Player.Message(p, Colors.red + "You have already hacked a rank!"); return;
            }
            Group grp = Group.Find(message);
            if (grp != null) {
                DoFakeRank(p, grp);
            } else {
                Player.Message(p, "Invalid Rank!");
            }
        }

        void DoFakeRank(Player p, Group newRank) {
            string color = newRank.color;
            string oldrank = p.group.name;
            p.color = newRank.color;

            p.hackrank = true;
            Player.GlobalMessage(p.ColoredName + "%S's rank was set to " + newRank.ColoredName + "%S. (Congratulations!)");
            p.SendMessage("You are now ranked " + newRank.ColoredName + "%S, type /help for your new set of commands.");
            DoKick(p, newRank);
        }

        void DoKick(Player p, Group newRank) {
            if (!Server.hackrank_kick) return;
            string oldCol = p.color;
            System.Timers.Timer messageTimer = new System.Timers.Timer(Server.hackrank_kick_time * 1000);
            messageTimer.Start();
            messageTimer.Elapsed += delegate
            {
                p.Leave("You have been kicked for hacking the rank " + newRank.ColoredName);
                p.color = oldCol;
                messageTimer.Stop();
                messageTimer.Dispose();
            };
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/hackrank [rank] - Hacks a rank");
            Player.Message(p, "Available ranks: " + Group.concatList());
        }
    }
}
