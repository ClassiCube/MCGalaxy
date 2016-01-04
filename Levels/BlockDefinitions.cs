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
using System.Linq;
using Newtonsoft.Json;

namespace MCGalaxy {
    
    public sealed class BlockDefinition {
        
        public byte ID;
        public string Name;
        public byte Solidity;
        public byte MovementSpeed;
        public byte TopT;
        public byte SideT;
        public byte BottomT;
        public byte TransmitsLight;
        public byte WalkSound;
        public byte FullBright;
        public byte Shape;
        public byte BlockDraw;
        public byte FogD;
        public byte FogR;
        public byte FogG;
        public byte FogB;
        public byte FallBack;
        
        public static BlockDefinition[] GlobalDefinitions = new BlockDefinition[256];
        
        public static void LoadGlobal(string path) {
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    GlobalDefinitions = JsonConvert.DeserializeObject<BlockDefinition[]>(json);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        	// TODO: temp block for debugging.
            GlobalDefinitions[130] = new BlockDefinition();
            GlobalDefinitions[130].Name = "To infinity and beyond!";
            GlobalDefinitions[130].Shape = 14;
            GlobalDefinitions[130].ID = 130;
        }
        
        public static void SaveGlobal(string path)  {
            string json = JsonConvert.SerializeObject(GlobalDefinitions);
            File.WriteAllText(path, json);
        }
        
        public static void AddGlobal(BlockDefinition bd) {
            GlobalDefinitions[bd.ID] = bd;
            foreach (Player pl in Player.players) {
                if (pl.HasExtension(CpeExt.BlockDefinitions)) {
                    pl.SendBlockDefinitions(bd);
                    pl.SendSetBlockPermission(bd.ID, 1, 1);
                }
            }
        }
        
        public static void SendAll(Player pl) {
            Console.WriteLine( "TESTING: " + pl.HasExtension(CpeExt.BlockDefinitions));
            if (!pl.HasExtension(CpeExt.BlockDefinitions))
                return;
            for (int i = 0; i < GlobalDefinitions.Length; i++) {
                BlockDefinition def = GlobalDefinitions[i];
                if (def == null) continue;
                
                pl.SendBlockDefinitions(def);
                pl.SendSetBlockPermission(def.ID, 1, 1);
            }
        }
        
        public static byte Fallback(byte customTile) {
            return Block.orange; // TODO: implement this
        }
    }
}
