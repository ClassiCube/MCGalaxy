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
        public override string name { get { return "ResizeLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WResize"), new CommandAlias("WorldResize") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (args.Length < 4) { Help(p); return; }
            
            if (DoResize(p, args)) return;
            Player.Message(p, "Type %T/ResizeLvl {0} {1} {2} {3} confirm %Sif you're sure.",
                           args[0], args[1], args[2], args[3]);
        }
        
        public static bool DoResize(Player p, string[] args) {
            Level lvl = Matcher.FindLevels(p, args[0]);
            if (lvl == null) return true;
            if (!LevelInfo.ValidateAction(p, lvl, "resize this level")) return false;
            
            ushort x = 0, y = 0, z = 0;
            if (!CmdNewLvl.CheckMapAxis(p, args[1], "Width",  ref x)) return false;
            if (!CmdNewLvl.CheckMapAxis(p, args[2], "Height", ref y)) return false;
            if (!CmdNewLvl.CheckMapAxis(p, args[3], "Length", ref z)) return false;
            if (!CmdNewLvl.CheckMapVolume(p, x, y, z)) return true;
            
            bool confirmed = args.Length > 4 && args[4].CaselessEq("confirm");
            if (!confirmed && (x < lvl.Width || y < lvl.Height || z < lvl.Length)) {
                Player.Message(p, "New level dimensions are smaller than the current dimensions, &cyou will lose blocks%S.");
                return false;
            }
            
            Level newLvl = ResizeLevel(lvl, x, y, z);
            if (newLvl == null) { Player.Message(p, "&cError resizing map."); return false; }
            LevelActions.Replace(lvl, newLvl);
            return true;
        }
        
        static Level ResizeLevel(Level lvl, int width, int height, int length) {
            using (Level tmp = new Level(lvl.name, (ushort)width, (ushort)height, (ushort)length)) {
                byte[] src = lvl.blocks, dst = tmp.blocks;
                
                // Copy blocks in bulk
                width  = Math.Min(lvl.Width,  tmp.Width);
                height = Math.Min(lvl.Height, tmp.Height);
                length = Math.Min(lvl.Length, tmp.Length);
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < length; z++) {
                        int srcI = lvl.Width * (z + y * lvl.Length);
                        int dstI = tmp.Width * (z + y * tmp.Length);
                        Buffer.BlockCopy(src, srcI, dst, dstI, width);
                    }
                }
                
                // Copy extended blocks in bulk
                width  = Math.Min(lvl.ChunksX, tmp.ChunksX);
                height = Math.Min(lvl.ChunksY, tmp.ChunksY);
                length = Math.Min(lvl.ChunksZ, tmp.ChunksZ);
                for (int cy = 0; cy < height; cy++)
                    for (int cz = 0; cz < length; cz++)
                        for (int cx = 0; cx < width; cx++)
                {
                    src = lvl.CustomBlocks[(cy * lvl.ChunksZ + cz) * lvl.ChunksX + cx];
                    if (src == null) continue;
                    
                    dst = new byte[16 * 16 * 16];
                    tmp.CustomBlocks[(cy * tmp.ChunksZ + cz) * tmp.ChunksX + cx] = dst;
                    Buffer.BlockCopy(src, 0, dst, 0, 16 * 16 * 16);
                }
                
                tmp.spawnx = lvl.spawnx; tmp.spawny = lvl.spawny; tmp.spawnz = lvl.spawnz;
                tmp.rotx = lvl.rotx; tmp.roty = lvl.roty;
                
                lock (lvl.saveLock) {
                    lvl.Backup(true);
                    IMapExporter.Formats[0].Write(LevelInfo.MapPath(lvl.name), tmp);
                    lvl.SaveChanges = false;
                }
            }

            Server.DoGC();
            return Level.Load(lvl.name);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ResizeLvl [level] [width] [height] [length]");
            Player.Message(p, "%HResizes the given level.");
        }
    }
}
