/*
    Copyright 2011 MCForge
        
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
using System;
namespace MCGalaxy.Commands {
    public sealed class CmdPause : Command {
        public override string name { get { return "pause"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPause() { }

        public override void Use(Player p, string message) {
            int seconds = 30;
            Level lvl = p != null ? p.level : Server.mainLevel;
            if (message != "") {
                string[] parts = message.Split(' ');
                if (parts.Length == 1) {
                    if (!int.TryParse(parts[0], out seconds)) {
                        seconds = 30;
                        lvl = Matcher.FindLevels(p, parts[0]);
                        if (lvl == null) return;
                    }
                } else {
                	if (!CommandParser.GetInt(p, parts[0], "Pause time", ref seconds, 0)) return;
                	
                    lvl = Matcher.FindLevels(p, parts[1]);
                    if (lvl == null) return;
                }
            }
            
            bool enabled = lvl.physPause;
            lvl.PhysicsEnabled = enabled;
            lvl.physPause = !lvl.physPause;
            if (enabled) {
                Chat.MessageAll("Physics on {0} %Swere re-enabled.", lvl.ColoredName);
            } else {
                Server.MainScheduler.QueueOnce(PauseCallback, lvl.name,
                                               TimeSpan.FromSeconds(seconds));
                Chat.MessageAll("Physics on {0} %Swere temporarily disabled.", lvl.ColoredName);
            }
        }
        
        static void PauseCallback(SchedulerTask task) {
            string lvlName = (string)task.State;
            Level lvl = LevelInfo.FindExact(lvlName);
            if (lvl == null) return;
            
            lvl.PhysicsEnabled = true;
            Chat.MessageAll("Physics on {0} %Swere re-enabled.", lvl.ColoredName);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/pause [map] [seconds]");
            Player.Message(p, "%HPauses physics on the given map for the specified number of seconds.");
            Player.Message(p, "%H  If [map] is not given, pauses physics on the current map.");
            Player.Message(p, "%H  If [seconds] is not given, pauses physics for 30 seconds.");
        }
    }
}
