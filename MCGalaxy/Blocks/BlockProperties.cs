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
using System.IO;

namespace MCGalaxy.Blocks {
    
    /// <summary> Type of animal this block behaves as. </summary>
    public enum AnimalAI : byte {
        None, Fly, FleeAir, KillerAir, FleeWater, KillerWater, FleeLava, KillerLava,
    }
    
    /// <summary> Extended and physics properties of a block. </summary>
    public struct BlockProps {
        
        /// <summary> Message shown to the level when the player is killed by this block. Can be null. </summary>
        public string DeathMessage;
        
        /// <summary> Whether colliding/walking through this block kills the player. </summary>
        public bool KillerBlock;
        
        /// <summary> Whether this block is considered a tDoor. </summary>
        public bool IsTDoor;
        /// <summary> Whether this block is considered a door. </summary>
        public bool IsDoor;
        /// <summary> Block index of the block this is converted to when toggled by a neighbouring door. </summary>
        public ushort oDoorIndex;
        
        /// <summary> Whether this block is considered a message block. </summary>
        public bool IsMessageBlock;
        /// <summary> Whether this block is considered a portal. </summary>
        public bool IsPortal;
        
        /// <summary> Whether this block is overwritten/killed by water blocks. </summary>
        public bool WaterKills;
        /// <summary> Whether this block is overwritten/killed by lava blocks. </summary>
        public bool LavaKills;
        
        /// <summary> Whether this block is an OP block (cannot be replaced by physics changes). </summary>
        public bool OPBlock;
        
        /// <summary> Whether this block should allow trains to go over them. </summary>
        public bool IsRails;
        
        /// <summary> Animal AI behaviour of this block. </summary>
        public AnimalAI AnimalAI;
        
        /// <summary> ID of the block that is placed when two of this block are placed on top of each other. </summary>
        /// <remarks> e.g. slabs and cobblestone slabs. </remarks>
        public byte StackId;
        
        /// <summary> Whether players can drown inside this block (e.g. water). </summary>
        public bool Drownable;
        
        /// <summary> The block ID this is changed into when exposed to sunlight. </summary>
        public ushort GrassIndex;
        
        /// <summary> The block ID this is changed into when no longer exposed to sunlight. </summary>
        public ushort DirtIndex;
        
        /// <summary> Whether the properties for this block have been modified and hence require saving. </summary>
        public bool Changed;
        
        public static BlockProps MakeDefault() {
            BlockProps props = default(BlockProps);
            props.oDoorIndex = Block.Invalid;
            props.GrassIndex = Block.Invalid;
            props.DirtIndex = Block.Invalid;
            return props;
        }
        
        
        public static void Save(string group, BlockProps[] scope, object locker, Predicate<int> selector) {
            lock (locker) {
                if (!Directory.Exists("blockprops"))
                    Directory.CreateDirectory("blockprops");
                SaveCore(group, scope, selector);
            }
        }
        
        static void SaveCore(string group, BlockProps[] scope, Predicate<int> selector) {
            using (StreamWriter w = new StreamWriter("blockprops/" + group + ".txt")) {
                w.WriteLine("# This represents the physics properties for blocks, in the format of:");
                w.WriteLine("# id : Is rails : Is tdoor : Is door : Is message block : Is portal : " +
                            "Killed by water : Killed by lava : Kills players : death message : " +
                            "Animal AI type : Stack block : Is OP block : oDoor block : Drownable : " +
                            "Grass block : Dirt block");
                for (int i = 0; i < scope.Length; i++) {
                    if (!scope[i].Changed || (selector != null && !selector(i))) continue;
                    BlockProps props = scope[i];
                    // Convert ext to raw ids
                    int id = i >= Block.Count ? (i - Block.Count) : i;
                    
                    string deathMsg = props.DeathMessage == null ? "" : props.DeathMessage.Replace(":", "\\;");
                    w.WriteLine(id + ":" + props.IsRails + ":" + props.IsTDoor + ":" + props.IsDoor    + ":"
                                + props.IsMessageBlock + ":" + props.IsPortal + ":" + props.WaterKills + ":"
                                + props.LavaKills + ":" + props.KillerBlock + ":" + deathMsg           + ":"
                                + (byte)props.AnimalAI + ":" + props.StackId + ":" + props.OPBlock     + ":"
                                + props.oDoorIndex + ":" + props.Drownable + ":" + props.GrassIndex + ":" + props.DirtIndex);
                }
            }
        }
        
        public static void Load(string group, BlockProps[] scope, object locker, bool lbScope) {
            lock (locker) {
                if (!Directory.Exists("blockprops")) return;
                if (!File.Exists("blockprops/" + group + ".txt")) return;
                LoadCore(group, scope, lbScope);
            }
        }
        
        static void LoadCore(string group, BlockProps[] scope, bool lbScope) {
            string[] lines = File.ReadAllLines("blockprops/" + group + ".txt");
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                
                string[] parts = line.Split(':');
                if (parts.Length < 10) {
                    Logger.Log(LogType.Warning, "Invalid line \"{0}\" in {1} block properties", line, group);
                    continue;
                }
                
                byte raw;
                if (!Byte.TryParse(parts[0], out raw)) {
                    Logger.Log(LogType.Warning, "Invalid line \"{0}\" in {1} block properties", line, group);
                    continue;
                }
                int idx = raw;
                if (lbScope && raw >= Block.CpeCount) idx += Block.Count;
                
                bool.TryParse(parts[1], out scope[idx].IsRails);
                bool.TryParse(parts[2], out scope[idx].IsTDoor);
                bool.TryParse(parts[3], out scope[idx].IsDoor);
                bool.TryParse(parts[4], out scope[idx].IsMessageBlock);
                bool.TryParse(parts[5], out scope[idx].IsPortal);
                bool.TryParse(parts[6], out scope[idx].WaterKills);
                bool.TryParse(parts[7], out scope[idx].LavaKills);
                bool.TryParse(parts[8], out scope[idx].KillerBlock);
                
                scope[idx].Changed = true;
                scope[idx].DeathMessage = parts[9].Replace("\\;", ":");
                if (scope[idx].DeathMessage.Length == 0)
                    scope[idx].DeathMessage = null;
                
                if (parts.Length > 10) {
                    byte ai; byte.TryParse(parts[10], out ai);
                    scope[idx].AnimalAI = (AnimalAI)ai;
                }
                if (parts.Length > 11) {
                    byte.TryParse(parts[11], out  scope[idx].StackId);
                }
                if (parts.Length > 12) {
                    bool.TryParse(parts[12], out scope[idx].OPBlock);
                }
                if (parts.Length > 13) {
                    ushort.TryParse(parts[13], out scope[idx].oDoorIndex);
                }
                if (parts.Length > 14) {
                    bool.TryParse(parts[14], out scope[idx].Drownable);
                }
                if (parts.Length > 15) {
                    ushort.TryParse(parts[15], out scope[idx].GrassIndex);
                }
                if (parts.Length > 16) {
                    ushort.TryParse(parts[16], out scope[idx].DirtIndex);
                }
            }
        }
    }
}
