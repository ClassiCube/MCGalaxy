/*
    Copyright 2015 MCGalaxy team
        
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
using MCGalaxy.Generator;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands.World {
    public sealed class CmdResizeLvl : Command {
        public override string name { get { return "resizelvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("wresize"), new CommandAlias("worldresoze") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 4) { Help(p); return; }
            
            if (DoResize(p, args)) return;
            Player.Message(p, "Type %T/resizelvl {0} {1} {2} {3} confirm %Sif you're sure.",
                           args[0], args[1], args[2], args[3]);
        }
        
        public static bool DoResize(Player p, string[] args) {
            Level lvl = Matcher.FindLevels(p, args[0]);
            if (lvl == null) return true;
            
            ushort x, y, z;
            if (!UInt16.TryParse(args[1], out x) || !UInt16.TryParse(args[2], out y) || !UInt16.TryParse(args[3], out z)) {
                Player.Message(p, "Invalid dimensions."); return true;
            }
            
            if (!MapGen.OkayAxis(x)) { Player.Message(p, "width must be divisible by 16, and >= 16"); return true; }
            if (!MapGen.OkayAxis(y)) { Player.Message(p, "height must be divisible by 16, and >= 16"); return true; }
            if (!MapGen.OkayAxis(z)) { Player.Message(p, "length must be divisible by 16, and >= 16."); return true; }
            if (!CmdNewLvl.CheckMapSize(p, x, y, z)) return true;
            
            bool confirmed = args.Length > 4 && args[4].CaselessEq("confirm");
            if (!confirmed && (x < lvl.Width || y < lvl.Height || z < lvl.Length)) {
                Player.Message(p, "New level dimensions are smaller than the current dimensions, &cyou will lose blocks%S.");
                return false;
            }
            
            Level newLvl = ResizeLevel(lvl, x, y, z);
            LevelActions.Replace(lvl, newLvl);
            return true;
        }
        
        static Level ResizeLevel(Level lvl, ushort width, ushort height, ushort length) {
            using (Level temp = new Level(lvl.name, width, height, length)) {
                for (ushort y = 0; y < Math.Min(height, lvl.Height); y++)
                    for (ushort z = 0; z < Math.Min(length, lvl.Length); z++)
                        for (ushort x = 0; x < Math.Min(width, lvl.Width); x++)
                {
                    byte block = lvl.blocks[x + lvl.Width * (z + y * lvl.Length)];
                    temp.blocks[x + width * (z + y * length)] = block;
                    if (block != Block.custom_block) continue;
                    
                    byte extBlock = lvl.GetExtTile(x, y, z);
                    temp.SetExtTileNoCheck(x, y, z, extBlock);
                }
                temp.spawnx = lvl.spawnx; temp.spawny = lvl.spawny; temp.spawnz = lvl.spawnz;
                
                lock (lvl.saveLock) {
                    lvl.Backup(true);
                    IMapExporter.Formats[0].Write(LevelInfo.MapPath(lvl.name), temp);
                    lvl.saveLevel = false;
                }
            }

            Server.DoGC();
            return Level.Load(lvl.name);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/resizelvl [level] [width] [height] [length]");
            Player.Message(p, "%HResizes the given level.");
        }
    }
}
