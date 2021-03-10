/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
   
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
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.World {
    public sealed class CmdFixGrass : Command2 {
        public override string name { get { return "FixGrass"; } }
        public override string shortcut { get { return "fg"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            int totalFixed = 0;
            Level lvl = p.level;
            if (!LevelInfo.Check(p, data.Rank, lvl, "use &T/fixgrass &Son this level")) return;
            
            if (message.Length == 0) {
                Fix(p, lvl, ref totalFixed, true, true);
            } else if (message.CaselessEq("light")) {
                FixLight(p, lvl, ref totalFixed);
            } else if (message.CaselessEq("grass")) {
                Fix(p, lvl, ref totalFixed, true, false);
            } else if (message.CaselessEq("dirt")) {
                Fix(p, lvl, ref totalFixed, false, true);
            } else {
                Help(p); return;
            }
            
            p.Message("Fixed " + totalFixed + " blocks.");
        }        
        
        static void Fix(Player p, Level lvl, ref int totalFixed, bool fixGrass, bool fixDirt) {
            int index = 0, maxY = lvl.Height - 1, oneY = lvl.Width * lvl.Length;
            BufferedBlockSender buffer = new BufferedBlockSender(lvl);
            BlockID above, block;
            
            for (ushort y = 0; y < lvl.Height; y++)
                for (ushort z = 0; z < lvl.Length; z++)
                    for (ushort x = 0; x < lvl.Width; x++)
            {
                block = lvl.FastGetBlock(index);
                if (fixGrass && lvl.Props[block].GrassBlock != Block.Invalid) {
                    above = y == maxY ? Block.Air : lvl.FastGetBlock(index + oneY);
                    BlockID grass = lvl.Props[block].GrassBlock;
                    
                    if (lvl.LightPasses(above) && p.level.TryChangeBlock(p, x, y, z, grass) == ChangeResult.Modified) {
                        buffer.Add(index, grass);
                        totalFixed++;
                    }
                } else if (fixDirt && lvl.Props[block].DirtBlock != Block.Invalid) {
                    above = y == maxY ? Block.Air : lvl.FastGetBlock(index + oneY);
                    BlockID dirt = lvl.Props[block].DirtBlock;
                    
                    if (!lvl.LightPasses(above) && p.level.TryChangeBlock(p, x, y, z, dirt) == ChangeResult.Modified) {
                        buffer.Add(index, dirt);
                        totalFixed++;
                    }
                }
                index++;
            }
            buffer.Flush();
        }
        
        static void FixLight(Player p, Level lvl, ref int totalFixed) {
            int index = 0, oneY = lvl.Width * lvl.Length;
            BufferedBlockSender buffer = new BufferedBlockSender(lvl);
            BlockID above, block;
            
            for (ushort y = 0; y < lvl.Height - 1; y++)
                for (ushort z = 0; z < lvl.Length; z++)
                    for (ushort x = 0; x < lvl.Width; x++)
            {
                block = lvl.FastGetBlock(index);
                bool inShadow = false;
                
                if (lvl.Props[block].GrassBlock != Block.Invalid) {
                    for (int i = 1; i < (lvl.Height - y); i++) {
                        above = lvl.FastGetBlock(index + oneY * i);
                        if (!lvl.LightPasses(above)) { inShadow = true; break; }
                    }
                    
                    BlockID grass = lvl.Props[block].GrassBlock;
                    if (!inShadow && p.level.TryChangeBlock(p, x, y, z, grass) == ChangeResult.Modified) {
                        buffer.Add(lvl.PosToInt(x, y, z), grass);
                        totalFixed++;
                    }
                } else if (lvl.Props[block].DirtBlock != Block.Invalid) {
                    for (int i = 1; i < (lvl.Height - y); i++) {
                        above = lvl.FastGetBlock(index + oneY * i);
                        if (!lvl.LightPasses(above)) { inShadow = true; break; }
                    }
                    
                    BlockID dirt = lvl.Props[block].DirtBlock;
                    if (inShadow && p.level.TryChangeBlock(p, x, y, z, dirt) == ChangeResult.Modified) {
                        buffer.Add(lvl.PosToInt(x, y, z), dirt);
                        totalFixed++;
                    }
                }
                index++;
            }
            buffer.Flush();
        }

        public override void Help(Player p) {
            p.Message("&T/FixGrass [mode] &H- Fixes grass based on mode");
            p.Message("&H[mode] is \"\": Any grass with something on top is made into dirt, dirt with nothing on top is made grass");
            p.Message("&H[mode] is \"light\": Only dirt/grass in sunlight becomes grass");
            p.Message("&H[mode] is \"grass\": Only turns grass to dirt when under stuff");
            p.Message("&H[mode] is \"dirt\": Only turns dirt with nothing on top to grass");
        }
    }
}
