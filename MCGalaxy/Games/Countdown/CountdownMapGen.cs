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

namespace MCGalaxy.Games {
    
    /// <summary> Generates a map for countdown. </summary>
    public static class CountdownMapGen {
        
        public static Level Generate(int width, int height, int length) {
            Level lvl = new Level("countdown", (ushort)width, (ushort)height, (ushort)length);
            MakeBoundaries(lvl);
            MakeViewAreaRoof(lvl);
            MakeViewAreaWalls(lvl);
            MakeViewAreaFloor(lvl);
            MakeChutesAndElevators(lvl);
            MakeSquares(lvl);
            
            lvl.VisitAccess.Min = LevelPermission.Guest;
            lvl.BuildAccess.Min = LevelPermission.Nobody;
            lvl.Config.Deletable = false;
            lvl.Config.Buildable = false;
            lvl.Config.MOTD = "Welcome to the Countdown map! -hax";
            
            lvl.spawnx = (ushort)(lvl.Width / 2);
            lvl.spawny = (ushort)(lvl.Height / 2 + 4);
            lvl.spawnz = (ushort)(lvl.Length / 2);
            lvl.Config.MOTD = "-hax";
            return lvl;
        }
        
        static void MakeBoundaries(Level lvl) {
            int maxX = lvl.Width - 1, maxY = lvl.Height - 1, maxZ = lvl.Length - 1;
            Cuboid(0, maxY, 0, maxX, maxY, maxZ, Block.glass, lvl);
            Cuboid(0, 0, 0, maxX, 0, maxZ, Block.rock, lvl);
            Cuboid(0, 1, 0, maxX, 1, maxZ, Block.magma, lvl);
            
            Cuboid(0, 0, 0, maxX, maxY, 0, Block.rock, lvl);
            Cuboid(0, 0, maxZ, maxX, maxY, maxZ, Block.rock, lvl);
            Cuboid(0, 0, 0, 0, maxY, maxZ, Block.rock, lvl);
            Cuboid(maxX, 0, 0, maxX, maxY, maxZ, Block.rock, lvl);
        }
        
        static void MakeViewAreaRoof(Level lvl) {
            int maxX = lvl.Width - 1, midY = lvl.Height / 2, maxZ = lvl.Length - 1;
            Cuboid(1, midY, 1, maxX - 1, midY, maxZ - 1, Block.glass, lvl);
            Cuboid(1, midY, 0, 3, midY, maxZ, Block.rock, lvl);
            Cuboid(maxX - 3, midY, 1, maxX - 1, midY, maxZ, Block.rock, lvl);
            Cuboid(0, midY, 1, maxX, midY, 3, Block.rock, lvl);
            Cuboid(0, midY, maxZ - 3, maxX, midY, maxZ - 1, Block.rock, lvl);
        }
        
        static void MakeViewAreaWalls(Level lvl) {
            int maxX = lvl.Width - 1, maxZ = lvl.Length - 1;
            Cuboid(3, 4, 3, 3, 10, maxZ - 3, Block.rock, lvl);
            Cuboid(maxX - 3, 4, 3, maxX - 3, 10, maxZ - 3, Block.rock, lvl);
            Cuboid(3, 4, 3, maxX - 3, 10, 3, Block.rock, lvl);
            Cuboid(3, 4, maxZ - 3, maxX - 3, 10, maxZ - 3, Block.rock, lvl);
            
            Cuboid(3, 6, 3, 3, 7, maxZ - 3, Block.glass, lvl);
            Cuboid(maxX - 3, 6, 3, maxX - 3, 7, maxZ - 3, Block.glass, lvl);
            Cuboid(3, 6, 3, maxX - 3, 7, 3, Block.glass, lvl);
            Cuboid(3, 6, maxZ - 3, maxX - 3, 7, maxZ - 3, Block.glass, lvl);
        }
        
        static void MakeViewAreaFloor(Level lvl) {
            int maxX = lvl.Width - 1, maxZ = lvl.Length - 1;
            Cuboid(1, 4, 0, 3, 4, maxZ, Block.rock, lvl);
            Cuboid(maxX - 3, 4, 1, maxX - 1, 4, maxZ, Block.rock, lvl);
            Cuboid(0, 4, 1, maxX, 4, 3, Block.rock, lvl);
            Cuboid(0, 4, maxZ - 3, maxX, 4, maxZ - 1, Block.rock, lvl);
        }
        
        static void MakeChutesAndElevators(Level lvl) {
            int maxX = lvl.Width - 1, maxY = lvl.Height - 1, maxZ = lvl.Length - 1;
            Cuboid(1, 5, 1, 1, maxY - 1, 1, Block.waterstill, lvl);
            Cuboid(maxX - 1, 5, 1, maxX - 1, maxY - 1, 1, Block.waterstill, lvl);
            Cuboid(1, 5, maxZ - 1, 1, maxY - 1, maxZ - 1, Block.waterstill, lvl);
            Cuboid(maxX - 1, 5, maxZ - 1, maxX - 1, maxY - 1, maxZ - 1, Block.waterstill, lvl);
            
            int midX = lvl.Width / 2, midY = lvl.Height / 2, midZ = lvl.Length / 2;
            Cuboid(midX - 2, midY + 1, midZ - 2, midX + 1, maxY, midZ - 2, Block.glass, lvl);
            Cuboid(midX - 2, midY + 1, midZ + 1, midX + 1, maxY, midZ + 1, Block.glass, lvl);
            Cuboid(midX - 2, midY + 1, midZ - 2, midX - 2, maxY, midZ + 1, Block.glass, lvl);
            Cuboid(midX + 1, midY + 1, midZ - 2, midX + 1, maxY, midZ + 1, Block.glass, lvl);
            // make some holes in the chutes
            Cuboid(midX - 1, maxY, midZ - 1, midX, maxY, midZ, Block.air, lvl);
            Cuboid(midX - 1, midY + 1, midZ - 2, midX, midY + 2, midZ - 2, Block.air, lvl);
            Cuboid(midX - 1, midY + 1, midZ + 1, midX, midY + 2, midZ + 1, Block.air, lvl);
            Cuboid(midX - 2, midY + 1, midZ - 1, midX - 2, midY + 2, midZ, Block.air, lvl);
            Cuboid(midX + 1, midY + 1, midZ - 1, midX + 1, midY + 2, midZ, Block.air, lvl);
        }
        
        static void MakeSquares(Level lvl) {
            int maxX = lvl.Width - 1, maxZ = lvl.Length - 1;
            Cuboid(4, 4, 4, maxX - 4, 4, maxZ - 4, Block.glass, lvl);        
            for(int zz = 6; zz < lvl.Length - 6; zz += 3)
                for (int xx = 6; xx < lvl.Width - 6; xx += 3)
                    Cuboid(xx, 4, zz, xx + 1, 4, zz + 1, Block.green, lvl);
        }
        
        static void Cuboid(int x1, int y1, int z1, int x2, int y2, int z2, byte block, Level lvl) {
            for (int y = y1; y <= y2; y++)
                for (int z = z1; z <= z2; z++)
                    for (int x = x1; x <= x2; x++)
            {
                lvl.SetTile((ushort)x, (ushort)y, (ushort)z, block);
            }
        }
    }
}
