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
using System.Text;
using Newtonsoft.Json;

namespace MCGalaxy {
    
    public sealed class BlockDefinition {
        
        public byte BlockID;
        public string Name;
        public byte CollideType;
        public float Speed;
        public byte TopTex, SideTex, BottomTex;
        public bool BlocksLight;
        public byte WalkSound;
        public bool FullBright;
        public byte Shape;
        public byte BlockDraw;
        public byte FogDensity,FogR, FogG, FogB;  
        public byte FallBack;        
        // BlockDefinitionsExt fields
        public byte MinX, MinY, MinZ;
        public byte MaxX, MaxY, MaxZ;
        
        public static BlockDefinition[] GlobalDefinitions = new BlockDefinition[256];
        
        public static void LoadGlobal(string path) {
            try {
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    GlobalDefinitions = JsonConvert.DeserializeObject<BlockDefinition[]>(json);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                GlobalDefinitions = new BlockDefinition[256];
            }            
            for (int i = 0; i < 256; i++) {
                if (GlobalDefinitions[i] != null && GlobalDefinitions[i].Name == null)
                    GlobalDefinitions[i] = null;
            }
            
            GlobalDefinitions[0] = new BlockDefinition();
            GlobalDefinitions[0].Name = "Air fallback";
            SaveGlobal("blocks.json");
        }
        
        public static void SaveGlobal(string path)  {
            string json = JsonConvert.SerializeObject(GlobalDefinitions);
            File.WriteAllText(path, json);
        }
        
        public static void AddGlobal(BlockDefinition def) {
            GlobalDefinitions[def.BlockID] = def;
            foreach (Player pl in Player.players) {
                if (!pl.HasCpeExt(CpeExt.BlockDefinitions)) continue;
                    
                if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt) && def.Shape != 0)
                    SendDefineBlockExt(pl, def);
                else
                    SendDefineBlock(pl, def);
                pl.SendSetBlockPermission(def.BlockID, 1, 1);
            }
        }
        
        public static void RemoveGlobal(BlockDefinition def) {
            GlobalDefinitions[def.BlockID] = null;
            foreach (Player p in Player.players) {
                if (p.HasCpeExt(CpeExt.BlockDefinitions))
                    p.SendRaw(Opcode.CpeRemoveBlockDefinition, def.BlockID);
            }
            SaveGlobal("blocks.json");
        }
        
        public static void SendAll(Player pl) {
            if (!pl.HasCpeExt(CpeExt.BlockDefinitions)) return;
            for (int i = 1; i < GlobalDefinitions.Length; i++) {
                BlockDefinition def = GlobalDefinitions[i];
                if (def == null) continue;
                
                if (pl.HasCpeExt(CpeExt.BlockDefinitionsExt) && def.Shape != 0)
                    SendDefineBlockExt(pl, def);
                else
                    SendDefineBlock(pl, def);
                pl.SendSetBlockPermission(def.BlockID, 1, 1);
            }
        }
        
        public static byte GetBlock(string msg) {
             for (int i = 1; i < 255; i++) {
                BlockDefinition def = GlobalDefinitions[i];
                if (def == null) continue;
                
                if (def.Name.Replace(" ", "").Equals(msg, StringComparison.OrdinalIgnoreCase))
                    return def.BlockID;
            }
            
            byte type;
            if (!byte.TryParse(msg, out type) || BlockDefinition.GlobalDefinitions[type] == null)
                return Block.Zero;
            return type;
        }
        
        public static byte Fallback(byte tile) {
            BlockDefinition def = GlobalDefinitions[tile];
            return def == null ? Block.air : def.FallBack;
        }
        
        static void SendDefineBlock(Player p, BlockDefinition def) {
            byte[] buffer = new byte[80];
            buffer[0] = Opcode.CpeDefineBlock;
            MakeDefineBlockStart(def, buffer);
            buffer[74] = def.Shape;
            MakeDefineBlockEnd(def, 75, buffer);
            p.SendRaw(buffer);
        }
        
        static void SendDefineBlockExt(Player p, BlockDefinition def) {
            byte[] buffer = new byte[85];
            buffer[0] = Opcode.CpeDefineBlockExt;
            MakeDefineBlockStart(def, buffer);
            buffer[74] = def.MinX;
            buffer[75] = def.MinZ;
            buffer[76] = def.MinY;
            buffer[77] = def.MaxX;
            buffer[78] = def.MaxZ;
            buffer[79] = def.MaxY;
            MakeDefineBlockEnd(def, 80, buffer);
            p.SendRaw(buffer);
        }
        
        static void MakeDefineBlockStart(BlockDefinition def, byte[] buffer) {
            // speed = 2^((raw - 128) / 64);
            // therefore raw = 64log2(speed) + 128
            byte rawSpeed = (byte)(64 * Math.Log(def.Speed, 2) + 128);
            
            buffer[1] = def.BlockID;
            Encoding.ASCII.GetBytes(def.Name.PadRight(64), 0, 64, buffer, 2);
            buffer[66] = def.CollideType;
            buffer[67] = rawSpeed;
            buffer[68] = def.TopTex;
            buffer[69] = def.SideTex;
            buffer[70] = def.BottomTex;
            buffer[71] = (byte)(def.BlocksLight ? 0 : 1);
            buffer[72] = def.WalkSound;
            buffer[73] = (byte)(def.FullBright ? 1 : 0);
        }
        
        static void MakeDefineBlockEnd(BlockDefinition def, int offset, byte[] buffer) {
            buffer[offset + 0] = def.BlockDraw;
            buffer[offset + 1] = def.FogDensity;
            buffer[offset + 2] = def.FogR;
            buffer[offset + 3] = def.FogG;
            buffer[offset + 4] = def.FogB;
        }
    }
}
