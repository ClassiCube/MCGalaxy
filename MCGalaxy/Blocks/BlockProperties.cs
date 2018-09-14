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
using BlockID = System.UInt16;

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
        /// <summary> Extended block ID of the block this is converted to when toggled by a neighbouring door. </summary>
        public BlockID oDoorBlock;
        
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
        
        /// <summary> The extended block ID that is placed when two of this block are placed on top of each other. </summary>
        /// <remarks> e.g. slabs and cobblestone slabs. </remarks>
        public BlockID StackBlock;
        
        /// <summary> Whether players can drown inside this block (e.g. water). </summary>
        public bool Drownable;
        
        /// <summary> The extended block ID this is changed into when exposed to sunlight. </summary>
        public BlockID GrassBlock;
        
        /// <summary> The extended block ID this is changed into when no longer exposed to sunlight. </summary>
        public BlockID DirtBlock;
        
        /// <summary> Whether the properties for this block have been modified and hence require saving. </summary>
        /// <remarks> bit 0 set means modified at global scope, bit 1 set means modified at level scope</remarks>
        public byte ChangedScope;
        
        public static BlockProps MakeEmpty() {
            BlockProps props = default(BlockProps);
            props.oDoorBlock = Block.Invalid;
            props.GrassBlock = Block.Invalid;
            props.DirtBlock  = Block.Invalid;
            return props;
        }
        
        
        public static void Save(string group, BlockProps[] list, byte scope) {
            lock (list) {
                if (!Directory.Exists("blockprops")) {
                    Directory.CreateDirectory("blockprops");
        	    }
                SaveCore(group, list, scope);
            }
        }
        
        static void SaveCore(string group, BlockProps[] list, byte scope) {
            using (StreamWriter w = new StreamWriter("blockprops/" + group + ".txt")) {
                w.WriteLine("# This represents the physics properties for blocks, in the format of:");
                w.WriteLine("# id : Is rails : Is tdoor : Is door : Is message block : Is portal : " +
                            "Killed by water : Killed by lava : Kills players : death message : " +
                            "Animal AI type : Stack block : Is OP block : oDoor block : Drownable : " +
                            "Grass block : Dirt block");
                for (int b = 0; b < list.Length; b++) {
                    if ((list[b].ChangedScope & scope) == 0) continue;
                    BlockProps props = list[b];
                    
                    string deathMsg = props.DeathMessage == null ? "" : props.DeathMessage.Replace(":", "\\;");
                    w.WriteLine(b + ":" + props.IsRails + ":" + props.IsTDoor + ":" + props.IsDoor     + ":"
                                + props.IsMessageBlock + ":" + props.IsPortal + ":" + props.WaterKills + ":"
                                + props.LavaKills + ":" + props.KillerBlock + ":" + deathMsg           + ":"
                                + (byte)props.AnimalAI + ":" + props.StackBlock + ":" + props.OPBlock  + ":"
                                + props.oDoorBlock + ":" + props.Drownable + ":" + props.GrassBlock    + ":" 
                                + props.DirtBlock);
                }
            }
        }
        
        public static string PropsPath(string group) { return "blockprops/" + group + ".txt"; }
        
        public static void Load(string group, BlockProps[] list, byte scope, bool mapOld) {
            lock (list) {
                if (!Directory.Exists("blockprops")) return;
                string path = PropsPath(group);
                if (File.Exists(path)) LoadCore(path, list, scope, mapOld);
            }
        }
        
        static void LoadCore(string path, BlockProps[] list, byte scope, bool mapOld) {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                
                string[] parts = line.Split(':');
                if (parts.Length < 10) {
                    Logger.Log(LogType.Warning, "Invalid line \"{0}\" in {1}", line, path);
                    continue;
                }
                
                BlockID b;
                if (!BlockID.TryParse(parts[0], out b)) {
                    Logger.Log(LogType.Warning, "Invalid line \"{0}\" in {1}", line, path);
                    continue;
                }
                
                if (mapOld) b = Block.MapOldRaw(b);
                if (b >= list.Length) {
                    Logger.Log(LogType.Warning, "Invalid block ID: " + b);
                    continue;
                } 
                
                bool.TryParse(parts[1], out list[b].IsRails);
                bool.TryParse(parts[2], out list[b].IsTDoor);
                bool.TryParse(parts[3], out list[b].IsDoor);
                bool.TryParse(parts[4], out list[b].IsMessageBlock);
                bool.TryParse(parts[5], out list[b].IsPortal);
                bool.TryParse(parts[6], out list[b].WaterKills);
                bool.TryParse(parts[7], out list[b].LavaKills);
                bool.TryParse(parts[8], out list[b].KillerBlock);
                
                list[b].ChangedScope = scope;
                list[b].DeathMessage = parts[9].Replace("\\;", ":");
                if (list[b].DeathMessage.Length == 0)
                    list[b].DeathMessage = null;
                
                if (parts.Length > 10) {
                    byte ai; byte.TryParse(parts[10], out ai);
                    list[b].AnimalAI = (AnimalAI)ai;
                }
                if (parts.Length > 11) {
                    BlockID.TryParse(parts[11], out list[b].StackBlock);
                    list[b].StackBlock = Block.MapOldRaw(list[b].StackBlock);
                }
                if (parts.Length > 12) {
                    bool.TryParse(parts[12], out list[b].OPBlock);
                }
                if (parts.Length > 13) {
                    BlockID.TryParse(parts[13], out list[b].oDoorBlock);
                }
                if (parts.Length > 14) {
                    bool.TryParse(parts[14], out list[b].Drownable);
                }
                if (parts.Length > 15) {
                    BlockID.TryParse(parts[15], out list[b].GrassBlock);
                }
                if (parts.Length > 16) {
                    BlockID.TryParse(parts[16], out list[b].DirtBlock);
                }
            }
        }
    }
}
