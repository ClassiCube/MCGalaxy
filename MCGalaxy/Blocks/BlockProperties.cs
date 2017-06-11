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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Blocks {
    
    /// <summary> Type of animal this block behaves as. </summary>
    public enum AnimalAI : byte {
        None, Fly, FleeAir, KillerAir, FleeWater, KillerWater, FleeLava, KillerLava,
    }
    
    /// <summary> Extended and physics properties of a block. </summary>
    public struct BlockProps {
        
        /// <summary> Standard block id sent to clients in map and block update packets. </summary>
        public byte ConvertId;
        
        /// <summary> Block name used for in commands. </summary>
        public string Name;
        
        /// <summary> Message shown to the level when the player is killed by this block. Can be null. </summary>
        public string DeathMessage;
        
        /// <summary> Whether colliding/walking through this block kills the player. </summary>
        public bool KillerBlock;
        
        /// <summary> Whether this block is considered a tdoor. </summary>
        public bool IsTDoor;
        /// <summary> Block id this block is converted to when toggled by a neighbouring door. </summary>
        public byte ODoorId;
        /// <summary> Whether this block is considered a door. </summary>
        public bool IsDoor;
        
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
        
        /// <summary> Whether the properties for this block have been modified and hence require saving. </summary>
        public bool Changed;
        
        public BlockProps(byte block) {
            this = default(BlockProps);
            ConvertId = block;
            Name = "unknown";
            ODoorId = Block.Invalid;
        }
        
        
        public static void Save(string group, BlockProps[] scope, bool custom) {
            if (!Directory.Exists("blockprops"))
                Directory.CreateDirectory("blockprops");
            
            using (StreamWriter w = new StreamWriter("blockprops/" + group + ".txt")) {
                w.WriteLine("# This represents the physics properties for blocks, in the format of:");
                w.WriteLine("# id : Is rails : Is tdoor : Is door : Is message block : Is portal : " +
                            "Killed by water : Killed by lava : Kills players : death message : Animal AI type");
                for (int i = 0; i < scope.Length; i++) {
                    if (!scope[i].Changed) continue;
                    BlockProps props = scope[i];
                    
                    // Don't save physics blocks
                    if (custom && (i > Block.CpeCount && i < Block.Count)) continue;
                    // Convert ext to raw ids
                    int id = i >= Block.Count ? (i - Block.Count) : i;
                    
                    string deathMsg = props.DeathMessage == null ? "" : props.DeathMessage.Replace(":", "\\;");
                    w.WriteLine(id + ":" + props.IsRails + ":" + props.IsTDoor + ":" + props.IsDoor + ":"
                                + props.IsMessageBlock + ":" + props.IsPortal + ":" + props.WaterKills + ":" 
                                + props.LavaKills + ":" + props.KillerBlock + ":" + deathMsg + ":" 
                                + (byte)props.AnimalAI);
                }
            }
        }
        
        public static void Load(string group, BlockProps[] scope, bool custom) {
            if (!Directory.Exists("blockprops")) return;
            if (!File.Exists("blockprops/" + group + ".txt")) return;
            
            string[] lines = File.ReadAllLines("blockprops/" + group + ".txt");
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                
                string[] parts = line.Split(':');
                if (parts.Length < 10) {
                    Server.s.Log("Invalid line \"" + line + "\" in " + group + " block properties");
                    continue;
                }
                
                byte raw;
                if (!Byte.TryParse(parts[0], out raw)) {
                    Server.s.Log("Invalid line \"" + line + "\" in " + group + " block properties");
                    continue;                   
                }
                int idx = raw;
                if (custom && raw >= Block.CpeCount) idx += Block.Count;
                
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
                if (scope[idx].DeathMessage == "")
                    scope[idx].DeathMessage = null;
                
                if (parts.Length > 10) {
                    byte ai; byte.TryParse(parts[10], out ai);
                    scope[idx].AnimalAI = (AnimalAI)ai;
                }
            }
        }
    }
}
