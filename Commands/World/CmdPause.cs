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
namespace MCGalaxy.Commands
{
    public sealed class CmdPause : Command
    {
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
                        lvl = LevelInfo.Find(parts[0]);
                    }
                } else {
                    if (!int.TryParse(parts[0], out seconds)) {
                        Player.SendMessage(p, "You must specify pause time in seconds"); return;
                    }
                    lvl = LevelInfo.Find(parts[1]);
                }
            }

            if (lvl == null) {
                Player.SendMessage(p, "Could not find entered level."); return;
            }
            
            bool enabled = lvl.physPause;
            lvl.PhysicsEnabled = enabled;
            lvl.physPause = !lvl.physPause;
            lvl.physResume = DateTime.UtcNow;
            if (enabled) {
                Player.GlobalMessage("Physics on " + lvl.name + " were re-enabled.");
            } else {
                lvl.physResume = lvl.physResume.AddSeconds(seconds);
                Player.GlobalMessage("Physics on " + lvl.name + " were temporarily disabled.");
                StartPauseCheck(lvl);
            }
        }
        
        static void StartPauseCheck(Level lvl) {
            lvl.physTimer.Elapsed += delegate
            {
                if (DateTime.UtcNow > lvl.physResume) {
                    lvl.physPause = false;
                    lvl.PhysicsEnabled = true;
                    
                    Player.GlobalMessage("Physics on " + lvl.name + " were re-enabled.");
                    lvl.physTimer.Stop();
                    lvl.physTimer.Dispose();
                }
            };
            lvl.physTimer.Start();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/pause [map] [seconds]");
            Player.SendMessage(p, "%HPauses physics on the given map for the specified number of seconds.");
            Player.SendMessage(p, "%H  If no map name is given, pauses physics on the current map.");
            Player.SendMessage(p, "%H  If no or non-numerical seconds are given, pauses physics for 30 seconds.");    
        }
    }
}
