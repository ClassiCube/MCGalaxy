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
using MCGalaxy.Bots;
using MCGalaxy.Generator;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands.World {
    public sealed class CmdResizeLvl : Command2 {
        public override string name { get { return "ResizeLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WResize"), new CommandAlias("WorldResize") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length < 4) { Help(p); return; }
            
            bool needConfirm;
            if (DoResize(p, args, data, out needConfirm)) return;
            
            if (!needConfirm) return;
            p.Message("Type &T/ResizeLvl {0} {1} {2} {3} confirm &Sif you're sure.",
                      args[0], args[1], args[2], args[3]);
        }
        
        public static bool DoResize(Player p, string[] args, CommandData data, out bool needConfirm) {
            needConfirm = false;
            Level lvl = Matcher.FindLevels(p, args[0]);
            
            if (lvl == null) return true;
            if (!LevelInfo.Check(p, data.Rank, lvl, "resize this level")) return false;
            
            ushort x = 0, y = 0, z = 0;
            if (!MapGen.GetDimensions(p, args, 1, ref x, ref y, ref z)) return false;
            
            bool confirmed = args.Length > 4 && args[4].CaselessEq("confirm");
            if (!confirmed && (x < lvl.Width || y < lvl.Height || z < lvl.Length)) {
                p.Message("New level dimensions are smaller than the current dimensions, &Wyou will lose blocks&S.");
                needConfirm = true;
                return false;
            }
            
            Level resized = ResizeLevel(lvl, x, y, z);
            LevelActions.Replace(lvl, resized);
            return true;
        }
        
        static Level ResizeLevel(Level lvl, int width, int height, int length) {
            Level res = new Level(lvl.name, (ushort)width, (ushort)height, (ushort)length);
            res.hasPortals       = lvl.hasPortals;
            res.hasMessageBlocks = lvl.hasMessageBlocks;
            byte[] src = lvl.blocks, dst = res.blocks;
            
            // Copy blocks in bulk
            width  = Math.Min(lvl.Width,  res.Width);
            height = Math.Min(lvl.Height, res.Height);
            length = Math.Min(lvl.Length, res.Length);
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    int srcI = lvl.Width * (z + y * lvl.Length);
                    int dstI = res.Width * (z + y * res.Length);
                    Buffer.BlockCopy(src, srcI, dst, dstI, width);
                }
            }
            
            // Copy extended blocks in bulk
            width  = Math.Min(lvl.ChunksX, res.ChunksX);
            height = Math.Min(lvl.ChunksY, res.ChunksY);
            length = Math.Min(lvl.ChunksZ, res.ChunksZ);
            for (int cy = 0; cy < height; cy++)
                for (int cz = 0; cz < length; cz++)
                    for (int cx = 0; cx < width; cx++)
            {
                src = lvl.CustomBlocks[(cy * lvl.ChunksZ + cz) * lvl.ChunksX + cx];
                if (src == null) continue;
                
                dst = new byte[16 * 16 * 16];
                res.CustomBlocks[(cy * res.ChunksZ + cz) * res.ChunksX + cx] = dst;
                Buffer.BlockCopy(src, 0, dst, 0, 16 * 16 * 16);
            }
            
            // TODO: This copying is really ugly and probably not 100% right
            res.spawnx = lvl.spawnx; res.spawny = lvl.spawny; res.spawnz = lvl.spawnz;
            res.rotx = lvl.rotx; res.roty = lvl.roty;
            
            lock (lvl.saveLock) {
                lvl.Backup(true);
                
                // Make sure zones are kept
                res.Zones = lvl.Zones;
                lvl.Zones = new VolatileArray<Zone>(false);
            
                IMapExporter.Formats[0].Write(LevelInfo.MapPath(lvl.name), res);
                lvl.SaveChanges = false;
            }
            
            res.backedup = true;
            Level.LoadMetadata(res);
            BotsFile.Load(res);
            return res;
        }
        
        public override void Help(Player p) {
            p.Message("&T/ResizeLvl [level] [width] [height] [length]");
            p.Message("&HResizes the given level.");
        }
    }
}
