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

namespace MCGalaxy.Commands.World {
    public sealed class CmdFixGrass : Command {
        public override string name { get { return "FixGrass"; } }
        public override string shortcut { get { return "fg"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            int totalFixed = 0;
            Level lvl = p.level;
            if (!LevelInfo.ValidateAction(p, lvl.name, "use %T/fixgrass %Son this level")) return;
            
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
            
            Player.Message(p, "Fixed " + totalFixed + " blocks.");
        }        
        
        static void Fix(Player p, Level lvl, ref int totalFixed, bool fixGrass, bool fixDirt) {
            int index = 0, maxY = lvl.Height - 1, oneY = lvl.Width * lvl.Length;
            BufferedBlockSender buffer = new BufferedBlockSender(lvl);
            ExtBlock above = default(ExtBlock), block = default(ExtBlock);
            
            for (ushort y = 0; y < lvl.Height; y++)
                for (ushort z = 0; z < lvl.Length; z++)
                    for (ushort x = 0; x < lvl.Width; x++)
            {
                block.BlockID = lvl.blocks[index];
                block.ExtID = 0;
                if (block.BlockID == Block.custom_block)
                    block.ExtID = lvl.GetExtTile(x, y, z);
                
                if (fixGrass && lvl.Props[block.Index].GrassIndex != Block.Invalid) {
                    above.BlockID = y == maxY ? Block.Air : lvl.blocks[index + oneY];
                    above.ExtID = 0;
                    if (above.BlockID == Block.custom_block)
                        above.ExtID = lvl.GetExtTile(x, (ushort)(y + 1), z);
                    
                    ExtBlock grass = ExtBlock.FromIndex(lvl.Props[block.Index].GrassIndex);
                    if (lvl.LightPasses(above) && p.level.DoBlockchange(p, x, y, z, grass) == 2) {
                        buffer.Add(index, grass.BlockID, grass.ExtID);
                        totalFixed++;
                    }
                } else if (fixDirt && lvl.Props[block.Index].DirtIndex != Block.Invalid) {
                    above.BlockID = y == maxY ? Block.Air : lvl.blocks[index + oneY];
                    above.ExtID = 0;
                    if (above.BlockID == Block.custom_block)
                        above.ExtID = lvl.GetExtTile(x, (ushort)(y + 1), z);
                    
                    ExtBlock dirt = ExtBlock.FromIndex(lvl.Props[block.Index].DirtIndex);
                    if (!lvl.LightPasses(above) && p.level.DoBlockchange(p, x, y, z, dirt) == 2) {
                        buffer.Add(index, dirt.BlockID, dirt.ExtID);
                        totalFixed++;
                    }
                }
                index++;
            }
            buffer.Send(true);
        }
        
        static void FixLight(Player p, Level lvl, ref int totalFixed) {
            int index = 0;
            BufferedBlockSender buffer = new BufferedBlockSender(lvl);
            ExtBlock above = default(ExtBlock), block = default(ExtBlock);
            
            for (ushort y = 0; y < lvl.Height - 1; y++)
                for (ushort z = 0; z < lvl.Length; z++)
                    for (ushort x = 0; x < lvl.Width; x++)
            {
                block.BlockID = lvl.blocks[index];
                block.ExtID = 0;
                bool inShadow = false;
                if (block.BlockID == Block.custom_block)
                    block.ExtID = lvl.GetExtTile(x, y, z);
                
                if (lvl.Props[block.Index].GrassIndex != Block.Invalid) {
                    for (int i = 1; i < (lvl.Height - y); i++) {
                        above.BlockID = lvl.blocks[index + (lvl.Width * lvl.Length) * i];
                        above.ExtID = 0;
                        if (above.BlockID == Block.custom_block)
                            above.ExtID = lvl.GetExtTile(x, (ushort)(y + i), z);
                        
                        if (!lvl.LightPasses(above)) { inShadow = true; break; }
                    }
                    
                    ExtBlock grass = ExtBlock.FromIndex(lvl.Props[block.Index].GrassIndex);
                    if (!inShadow && p.level.DoBlockchange(p, x, (ushort)y, z, grass) == 2) {
                        buffer.Add(index, grass.BlockID, grass.ExtID);
                        totalFixed++;
                    }
                } else if (lvl.Props[block.Index].DirtIndex != Block.Invalid) {
                    for (int i = 1; i < (lvl.Height - y); i++) {
                        above.BlockID = lvl.blocks[index + (lvl.Width * lvl.Length) * i];
                        above.ExtID = 0;
                        if (above.BlockID == Block.custom_block)
                            above.ExtID = lvl.GetExtTile(x, (ushort)(y + i), z);
                        
                        if (!lvl.LightPasses(above)) { inShadow = true; break; }
                    }
                    
                    ExtBlock dirt = ExtBlock.FromIndex(lvl.Props[block.Index].DirtIndex);
                    if (inShadow && p.level.DoBlockchange(p, x, (ushort)y, z, dirt) == 2) {
                        buffer.Add(index, dirt.BlockID, dirt.ExtID);
                        totalFixed++;
                    }
                }
                index++;
            }
            buffer.Send(true);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/FixGrass [type] %H- Fixes grass based on type");
            Player.Message(p, "%H[type] is \"\": Any grass with something on top is made into dirt, dirt with nothing on top is made grass");
            Player.Message(p, "%H[type] is \"light\": Only dirt/grass in sunlight becomes grass");
            Player.Message(p, "%H[type] is \"grass\": Only turns grass to dirt when under stuff");
            Player.Message(p, "%H[type] is \"dirt\": Only turns dirt with nothing on top to grass");
        }
    }
}
