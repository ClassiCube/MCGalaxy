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
    
    public struct BlockProps {
        
        /// <summary> ID of block these properties are associated with. </summary>
        public byte BlockId;
        
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
        
        /// <summary> Whether the properties for this block have been modified and hence require saving. </summary>
        public bool Changed;
        
        public BlockProps(byte block) {
            this = default(BlockProps);
            BlockId = block;
            ConvertId = block;
            Name = "unknown";
            ODoorId = Block.Invalid;
        }
        
        
        public static void Save(string group, BlockProps[] scope) {
            if (!Directory.Exists("blockprops"))
                Directory.CreateDirectory("blockprops");
            
            using (StreamWriter w = new StreamWriter("blockprops/" + group + ".txt")) {
                w.WriteLine("# This represents the physics properties for blocks, in the format of:");
                w.WriteLine("# id : Is rails : Is tdoor : Is door : Is message block : Is portal : " +
                            "Killed by water : Killed by lava : Kills players : death message");
                for (int i = 0; i < scope.Length; i++) {
                    if (!scope[i].Changed) continue;
                    BlockProps props = scope[i];
                    
                    string deathMsg = props.DeathMessage == null ? "" : props.DeathMessage.Replace(":", "\\;");
                    w.WriteLine(i + ":" + props.IsRails + ":" + props.IsTDoor + ":" + props.IsDoor + ":"
                                + props.IsMessageBlock + ":" + props.IsPortal + ":" + props.WaterKills + ":" 
                                + props.LavaKills + ":" + props.KillerBlock + ":" + deathMsg);
                }
            }
        }
        
        public static void Load(string group, BlockProps[] scope) {
            if (!Directory.Exists("blockprops")) return;
            if (!File.Exists("blockprops/" + group + ".txt")) return;
            
            string[] lines = File.ReadAllLines("blockprops/" + group + ".txt");
            for (int i = 0; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                
                string[] parts = line.Split(':');
                if (parts.Length != 10) {
                    Server.s.Log("Invalid line \"" + line + "\" in " + group + " block properties");
                    continue;
                }
                byte id;
                if (!Byte.TryParse(parts[0], out id)) {
                    Server.s.Log("Invalid line \"" + line + "\" in " + group + " block properties");
                    continue;                   
                }
                
                bool.TryParse(parts[1], out scope[id].IsRails);
                bool.TryParse(parts[2], out scope[id].IsTDoor);
                bool.TryParse(parts[3], out scope[id].IsDoor);
                bool.TryParse(parts[4], out scope[id].IsMessageBlock);
                bool.TryParse(parts[5], out scope[id].IsPortal);
                bool.TryParse(parts[6], out scope[id].WaterKills);
                bool.TryParse(parts[7], out scope[id].LavaKills);
                bool.TryParse(parts[8], out scope[id].KillerBlock);
                
                scope[id].Changed = true;
                scope[id].DeathMessage = parts[9].Replace("\\;", ":");
                if (scope[id].DeathMessage == "")
                    scope[id].DeathMessage = null;
            }
        }
    }
}
