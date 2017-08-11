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
    
    /// <summary> A named pair of position and orientation, located on a particular map. </summary>
    public class Warp {
        /// <summary> Position of this warp. </summary>
        public Position Pos;
        /// <summary> Orientation of this warp. </summary>
        public byte Yaw, Pitch;
        /// <summary> The name of this warp. </summary>
        public string Name;
        /// <summary> The name of the level this warp is located on. </summary>
        public string Level;
    }
    
    public sealed class WarpList {
        public static WarpList Global = new WarpList(false);
        
        public List<Warp> Items = new List<Warp>();
        bool playerWarp;
        
        public WarpList(bool playerWarp) {
            this.playerWarp = playerWarp;
        }
        
        /// <summary> Finds the warp whose name caselessly equals the given name. </summary>
        public Warp Find(string name) {
            foreach (Warp wp in Items) {
                if (wp.Name.CaselessEq(name)) return wp;
            }
            return null;
        }
        
        /// <summary> Attempts to move the given player to the given warp. </summary>
        public void Goto(Warp warp, Player p) {
            if (!p.level.name.CaselessEq(warp.Level)) {
                PlayerActions.ChangeMap(p, warp.Level);
            }
            
            if (p.level.name.CaselessEq(warp.Level)) {
                p.SendPos(Entities.SelfID, warp.Pos, new Orientation(warp.Yaw, warp.Pitch));
                Player.Message(p, "Sent you to waypoint/warp");
            } else {
                Player.Message(p, "Unable to send you to the warp as the map it is on is not loaded.");
            }
        }

        /// <summary> Creates a new warp with the given name, located at the 
        /// player's current position, orientation, and level. </summary>
        public void Create(string name, Player p) {
            Warp warp = new Warp();
            InitWarp(warp, name, p);
            Items.Add(warp);
            Save(p);
        }
        
        void InitWarp(Warp warp, string name, Player p) {
            warp.Pos = p.Pos; warp.Name = name;
            warp.Yaw = p.Rot.RotY; warp.Pitch = p.Rot.HeadX;            
            warp.Level = p.level.name;
        }

        /// <summary> Moves the given warp to the target 
        /// player's position, orientation, and map. </summary>
        public void Update(Warp warp, Player p) {
            InitWarp(warp, warp.Name, p);
            Save(p);
        }

        /// <summary> Removes the given warp. </summary>
        public void Remove(Warp warp, Player p) {
            Items.Remove(warp);
            Save(p);
        }

        /// <summary> Returns whether a warp with the given name exists. </summary>
        public bool Exists(string name) { return Find(name) != null; }
        

        public void Load(Player p) {
            string file = playerWarp ? "extra/Waypoints/" + p.name + ".save" : "extra/warps.save";
            if (!File.Exists(file)) return;
            
            using (StreamReader r = new StreamReader(file)) {
                string line;
                while ((line = r.ReadLine()) != null) {
                    line = line.Trim();
                    if (line.StartsWith("#") || !line.Contains(":")) continue;
                    
                    string[] parts = line.Split(':');
                    Warp wp = new Warp();
                    try {
                        wp.Name = parts[0];
                        wp.Level = parts[1];
                        wp.Pos.X = int.Parse(parts[2]);
                        wp.Pos.Y = int.Parse(parts[3]);
                        wp.Pos.Z = int.Parse(parts[4]);
                        wp.Yaw = byte.Parse(parts[5]);
                        wp.Pitch = byte.Parse(parts[6]);
                        Items.Add(wp);
                    } catch {
                        Logger.Log(LogType.Warning, "Failed loading a warp from " + file);
                    }
                }
            }
        }

        public void Save(Player p) {
            string file = playerWarp ? "extra/Waypoints/" + p.name + ".save" : "extra/warps.save";
            using (StreamWriter w = new StreamWriter(file)) {
                foreach (Warp warp in Items) {
                    w.WriteLine(warp.Name + ":" + warp.Level + ":" + warp.Pos.X + ":" + 
            		            warp.Pos.Y + ":" + warp.Pos.Z + ":" + warp.Yaw + ":" + warp.Pitch);
                }
            }
        }
    }
}