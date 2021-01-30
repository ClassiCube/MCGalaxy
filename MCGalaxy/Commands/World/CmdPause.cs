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
using MCGalaxy.Tasks;

namespace MCGalaxy.Commands.World {
    public sealed class CmdPause : Command2 {
        public override string name { get { return "Pause"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            int seconds = 30;
            Level lvl = p.IsSuper ? Server.mainLevel : p.level;
            
            if (message.Length > 0) {
                string[] parts = message.SplitSpaces();
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
            
            if (!LevelInfo.Check(p, data.Rank, lvl, "pause physics on this level")) return;
            bool enabled = lvl.PhysicsPaused;
            lvl.PhysicsPaused = !lvl.PhysicsPaused;
            
            if (enabled) {
                Chat.MessageGlobal("Physics on {0} &Swere re-enabled.", lvl.ColoredName);
            } else {
                Server.MainScheduler.QueueOnce(PauseCallback, lvl.name,
                                               TimeSpan.FromSeconds(seconds));
                Chat.MessageGlobal("Physics on {0} &Swere temporarily disabled.", lvl.ColoredName);
            }
        }
        
        static void PauseCallback(SchedulerTask task) {
            string lvlName = (string)task.State;
            Level lvl = LevelInfo.FindExact(lvlName);
            if (lvl == null) return;
            
            lvl.PhysicsPaused = false;
            Chat.MessageGlobal("Physics on {0} &Swere re-enabled.", lvl.ColoredName);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Pause [level] [seconds]");
            p.Message("&HPauses physics on the given level for the given number of seconds.");
            p.Message("&H  If [level] is not given, pauses physics on the current level.");
            p.Message("&H  If [seconds] is not given, pauses physics for 30 seconds.");
        }
    }
}
