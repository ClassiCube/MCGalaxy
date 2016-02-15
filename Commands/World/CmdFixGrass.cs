/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
   
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdFixGrass : Command
    {
        public override string name { get { return "fixgrass"; } }
        public override string shortcut { get { return "fg"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            int totalFixed = 0;
            Level lvl = p.level;
            switch (message.ToLower())
            {
                case "":
                    FixDirtAndGrass(p, lvl, ref totalFixed); break;
                case "light":
                    FixLight(p, lvl, ref totalFixed); break;
                case "grass":
                    FixGrass(p, lvl, ref totalFixed); break;
                case "dirt":
                    FixDirt(p, lvl, ref totalFixed); break;
                default:
                    Help(p); return;
            }
            Player.SendMessage(p, "Fixed " + totalFixed + " blocks.");
        }
        
        
        static void FixDirtAndGrass(Player p, Level lvl, ref int totalFixed) {
            int index = 0;
            for (int y = 0; y < lvl.Height - 1; y++)
                for (int z = 0; z < lvl.Length; z++)
                    for (int x = 0; x < lvl.Width; x++)
            {
                byte block = lvl.blocks[index];
                if (block == Block.dirt) {
                    byte above = lvl.blocks[index + lvl.Width * lvl.Length], extAbove = 0;
                    if (above == Block.custom_block)
                        extAbove = lvl.GetExtTile((ushort)x, (ushort)(y + 1), (ushort)z);
                    
                    if (Block.LightPass(above, extAbove, lvl.CustomBlockDefs)) {
                        p.level.Blockchange(p, (ushort)x, (ushort)y, (ushort)z, Block.grass);
                        totalFixed++;
                    }
                } else if (block == Block.grass) {
                    byte above = lvl.blocks[index + lvl.Width * lvl.Length], extAbove = 0;
                    if (above == Block.custom_block)
                        extAbove = lvl.GetExtTile((ushort)x, (ushort)(y + 1), (ushort)z);
                    
                    if (!Block.LightPass(above, extAbove, lvl.CustomBlockDefs)) {
                        p.level.Blockchange(p, (ushort)x, (ushort)y, (ushort)z, Block.dirt);
                        totalFixed++;
                    }
                }
                index++;
            }
        }
        
        static void FixLight(Player p, Level lvl, ref int totalFixed) {
            int index = 0;
            for (int y = 0; y < lvl.Height - 1; y++)
                for (int z = 0; z < lvl.Length; z++)
                    for (int x = 0; x < lvl.Width; x++)
            {
                byte block = lvl.blocks[index];
                bool inShadow = false;
                if (block == Block.dirt) {
                    for (int i = 1; i < (lvl.Height - y); i++) {
                        byte above = lvl.blocks[index + (lvl.Width * lvl.Length) * i], extAbove = 0;
                        if (above == Block.custom_block)
                            extAbove = lvl.GetExtTile((ushort)x, (ushort)(y + i), (ushort)z);
                        
                        if (!Block.LightPass(above, extAbove, lvl.CustomBlockDefs)) { 
                            inShadow = true; break; 
                        }
                    }
                    
                    if (!inShadow) {
                        p.level.Blockchange(p, (ushort)x, (ushort)y, (ushort)z, Block.grass);
                        totalFixed++;
                    }
                } else if (block == Block.grass) {
                    for (int i = 1; i < (lvl.Height - y); i++) {
                        byte above = lvl.blocks[index + (lvl.Width * lvl.Length) * i], extAbove = 0;
                        if (above == Block.custom_block)
                            extAbove = lvl.GetExtTile((ushort)x, (ushort)(y + i), (ushort)z);
                        
                        if (!Block.LightPass(above, extAbove, lvl.CustomBlockDefs)) { 
                            inShadow = true; break; 
                        }
                    }
                    
                    if (inShadow) {
                        p.level.Blockchange(p, (ushort)x, (ushort)y, (ushort)z, Block.dirt);
                        totalFixed++;
                    }
                }
                index++;
            }
        }
        
        static void FixDirt(Player p, Level lvl, ref int totalFixed) {
            int index = 0;
            for (int y = 0; y < lvl.Height - 1; y++)
                for (int z = 0; z < lvl.Length; z++)
                    for (int x = 0; x < lvl.Width; x++)
            {
                byte block = lvl.blocks[index];
                if (block != Block.dirt) { index++; continue; }
                byte above = lvl.blocks[index + lvl.Width * lvl.Length], extAbove = 0;
                if (above == Block.custom_block)
                    extAbove = lvl.GetExtTile((ushort)x, (ushort)(y + 1), (ushort)z);
                
                if (Block.LightPass(above, extAbove, lvl.CustomBlockDefs)) {
                    p.level.Blockchange(p, (ushort)x, (ushort)y, (ushort)z, Block.grass);
                    totalFixed++;
                }
                index++;
            }
        }
        
        static void FixGrass(Player p, Level lvl, ref int totalFixed) {
            int index = 0;
            for (int y = 0; y < lvl.Height - 1; y++)
                for (int z = 0; z < lvl.Length; z++)
                    for (int x = 0; x < lvl.Width; x++)
            {
                byte block = lvl.blocks[index];
                if (block != Block.grass) { index++; continue; }
                byte above = lvl.blocks[index + lvl.Width * lvl.Length], extAbove = 0;
                if (above == Block.custom_block)
                    extAbove = lvl.GetExtTile((ushort)x, (ushort)(y + 1), (ushort)z);
                
                if (!Block.LightPass(above, extAbove, lvl.CustomBlockDefs)) {
                    p.level.Blockchange(p, (ushort)x, (ushort)y, (ushort)z, Block.dirt);
                    totalFixed++;
                }
                index++;
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/fixgrass <type> - Fixes grass based on type");
            Player.SendMessage(p, "<type> as \"\": Any grass with something on top is made into dirt, dirt with nothing on top is made grass");
            Player.SendMessage(p, "<type> as \"light\": Only dirt/grass in sunlight becomes grass");
            Player.SendMessage(p, "<type> as \"grass\": Only turns grass to dirt when under stuff");
            Player.SendMessage(p, "<type> as \"dirt\": Only turns dirt with nothing on top to grass");
        }
    }
}
