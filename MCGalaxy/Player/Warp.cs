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
using System.IO;

namespace MCGalaxy {
    
    public class Warp {
        public int x, y, z;
        public byte rotx, roty;
        public string name;
        public string lvlname;
    }
    
    public sealed class WarpList {        
        public static WarpList Global = new WarpList(false);
        
        public List<Warp> Items = new List<Warp>();
        bool playerWarp;
        
        public WarpList(bool playerWarp) {
            this.playerWarp = playerWarp;
        }
        
        public Warp Find(string name) {
            foreach (Warp wp in Items) {
                if (wp.name.CaselessEq(name)) return wp;
            }
            return null;
        }
        
        public void Goto(string warp, Player p) {
            Warp wp = Find(warp);
            if (wp == null) return;
            
            Level lvl = LevelInfo.FindExact(wp.lvlname);
            if (p.level != lvl)
                PlayerActions.ChangeMap(p, lvl);
            
            if (p.level.name.CaselessEq(wp.lvlname)) {
                p.SendPos(Entities.SelfID, 
                          new Position(wp.x, wp.y, wp.z),
                          new Orientation(wp.rotx, wp.roty));
                Player.Message(p, "Sent you to waypoint/warp");
            } else {
                Player.Message(p, "Unable to send you to the warp as the map it is on is not loaded.");
            }
        }

        public void Create(string warp, Player p) {
            Warp wp = new Warp();
            wp.x = p.Pos.X; wp.y = p.Pos.Y; wp.z = p.Pos.Z;
            wp.rotx = p.Rot.RotY; wp.roty = p.Rot.HeadX;
            
            wp.name = warp;
            wp.lvlname = p.level.name;
            Items.Add(wp);
            Save(p);
        }

        public void Update(string warp, Player p) {
            Warp wp = Find(warp);
            Items.Remove(wp);
            Create(warp, p);
        }

        public void Remove(string warp, Player p) {
            Warp wp = Find(warp);
            Items.Remove(wp);
            Save(p);
        }

        public bool Exists(string warp) {
            foreach (Warp wp in Items) {
                if (wp.name.CaselessEq(warp)) return true;
            }
            return false;
        }
        

        public void Load(Player p) {
            string file = playerWarp ? "extra/Waypoints/" + p.name + ".save" : "extra/warps.save";
            if (!File.Exists(file)) return;
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    line = line.ToLower().Trim();
                    if (line.StartsWith("#") || !line.Contains(":")) continue;
                    
                    string[] parts = line.ToLower().Split(':');
                    Warp wp = new Warp();
                    try {
                        wp.name = parts[0];
                        wp.lvlname = parts[1];
                        wp.x = ushort.Parse(parts[2]);
                        wp.y = ushort.Parse(parts[3]);
                        wp.z = ushort.Parse(parts[4]);
                        wp.rotx = byte.Parse(parts[5]);
                        wp.roty = byte.Parse(parts[6]);                        
                        Items.Add(wp);
                    } catch {
                        Server.s.Log("Couldn't load a warp.");
                    }
                }
            }
        }

        public void Save(Player p) {
            string file = playerWarp ? "extra/Waypoints/" + p.name + ".save" : "extra/warps.save";            
            using (StreamWriter w = new StreamWriter(file)) {
                foreach (Warp wp in Items) {
                    w.WriteLine(wp.name + ":" + wp.lvlname + ":" + wp.x + ":" + wp.y + ":" + wp.z + ":" + wp.rotx + ":" + wp.roty);
                }
            }
        }
    }
}