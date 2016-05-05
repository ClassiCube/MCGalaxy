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
using System.IO;
using System.Threading;

namespace MCGalaxy {
    
    public class Waypoint {
        public ushort x;
        public ushort y;
        public ushort z;
        public byte rotx;
        public byte roty;
        public string name;
        public string lvlname;
    }
    
    public static class WaypointList {
        
        public static Waypoint Find(string name, Player p) {
            Waypoint wpfound = null;
            bool found = false;
            foreach ( Waypoint wp in p.Waypoints ) {
                if ( wp.name.ToLower() == name.ToLower() ) {
                    wpfound = wp;
                    found = true;
                }
            }
            if ( found ) { return wpfound; }
            else { return null; }
        }
        
        public static void Goto(string waypoint, Player p) {
            if ( !Exists(waypoint, p) ) return;
            Waypoint wp = Find(waypoint, p);
            Level lvl = LevelInfo.Find(wp.lvlname);
            if ( wp == null ) return;
            if ( lvl != null ) {
                if ( p.level != lvl ) {
                    Command.all.Find("goto").Use(p, lvl.name);
                }
                p.SendPos(0xFF, wp.x, wp.y, wp.z, wp.rotx, wp.roty);
                Player.Message(p, "Sent you to waypoint");
            }
            else { Player.Message(p, "The map that that waypoint is on isn't loaded right now (" + wp.lvlname + ")"); return; }
        }

        public static void Create(string waypoint, Player p) {
            Waypoint wp = new Waypoint();
            wp.x = p.pos[0]; wp.y = p.pos[1]; wp.z = p.pos[2];
            wp.rotx = p.rot[0]; wp.roty = p.rot[1];
            wp.name = waypoint;
            wp.lvlname = p.level.name;
            p.Waypoints.Add(wp);
            Save();
        }

        public static void Update(string waypoint, Player p) {
            Waypoint wp = Find(waypoint, p);
            p.Waypoints.Remove(wp);
            {
                wp.x = p.pos[0];
                wp.y = p.pos[1];
                wp.z = p.pos[2];
                wp.rotx = p.rot[0];
                wp.roty = p.rot[1];
                wp.name = waypoint;
                wp.lvlname = p.level.name;
            }
            p.Waypoints.Add(wp);
            Save();
        }

        public static void Remove(string waypoint, Player p) {
            Waypoint wp = Find(waypoint, p);
            p.Waypoints.Remove(wp);
            Save();
        }

        public static bool Exists(string waypoint, Player p) {
            foreach (Waypoint wp in p.Waypoints) {
				if (wp.name.CaselessEq(waypoint)) return true;
            }
            return false;
        }

        public static void Load(Player p) {
            if ( File.Exists("extra/Waypoints/" + p.name + ".save") ) {
                using ( StreamReader SR = new StreamReader("extra/Waypoints/" + p.name + ".save") ) {
                    bool failed = false;
                    string line;
                    while ( SR.EndOfStream == false ) {
                        line = SR.ReadLine().ToLower().Trim();
                        if ( !line.StartsWith("#") && line.Contains(":") ) {
                            failed = false;
                            string[] LINE = line.ToLower().Split(':');
                            Waypoint wp = new Waypoint();
                            try {
                                wp.name = LINE[0];
                                wp.lvlname = LINE[1];
                                wp.x = ushort.Parse(LINE[2]);
                                wp.y = ushort.Parse(LINE[3]);
                                wp.z = ushort.Parse(LINE[4]);
                                wp.rotx = byte.Parse(LINE[5]);
                                wp.roty = byte.Parse(LINE[6]);
                            }
                            catch {
                                Server.s.Log("Couldn't load a Waypoint!");
                                failed = true;
                            }
                            if ( failed == false ) {
                                p.Waypoints.Add(wp);
                            }
                        }
                    }
                    SR.Dispose();
                }
            }
        }

        public static void Save() {
			Player[] players = PlayerInfo.Online.Items;
            foreach ( Player p in players ) {
                if ( p.Waypoints.Count >= 1 ) {
                    using ( StreamWriter SW = new StreamWriter("extra/Waypoints/" + p.name + ".save") ) {
                        foreach ( Waypoint wp in p.Waypoints ) {
                            SW.WriteLine(wp.name + ":" + wp.lvlname + ":" + wp.x + ":" + wp.y + ":" + wp.z + ":" + wp.rotx + ":" + wp.roty);
                        }
                    }
                }
            }
        }
    }
}
