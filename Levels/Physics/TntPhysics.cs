/*
    Copyright 2015 MCGalaxy
    Original level physics copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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

namespace MCGalaxy.BlockPhysics {
    
    public static class TntPhysics {
        
        static void ShowWarningFuse(Level lvl, ushort x, ushort y, ushort z) {
            lvl.Blockchange(x, (ushort)(y + 1), z, lvl.GetTile(x, (ushort)(y + 1), z) == Block.lavastill ? Block.air : Block.lavastill);
            lvl.Blockchange(x, (ushort)(y - 1), z, lvl.GetTile(x, (ushort)(y - 1), z) == Block.lavastill ? Block.air : Block.lavastill);
            lvl.Blockchange((ushort)(x + 1), y, z, lvl.GetTile((ushort)(x + 1), y, z) == Block.lavastill ? Block.air : Block.lavastill);
            lvl.Blockchange((ushort)(x - 1), y, z, lvl.GetTile((ushort)(x - 1), y, z) == Block.lavastill ? Block.air : Block.lavastill);
            lvl.Blockchange(x, y, (ushort)(z + 1), lvl.GetTile(x, y, (ushort)(z + 1)) == Block.lavastill ? Block.air : Block.lavastill);
            lvl.Blockchange(x, y, (ushort)(z - 1), lvl.GetTile(x, y, (ushort)(z - 1)) == Block.lavastill ? Block.air : Block.lavastill);
        }
        
        public static void DoLargeTnt(Level lvl, Check C, Random rand, int power) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (lvl.physics < 3) {
                lvl.Blockchange(x, y, z, Block.air);
            } else {
                if (C.time < 5 && lvl.physics == 3) {
                    C.time++;
                    ShowWarningFuse(lvl, x, y, z);
                    return;
                }
                lvl.MakeExplosion(x, y, z, power);
            }
        }
        
        public static void DoSmallTnt(Level lvl, Check C, Random rand) {
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);

            if (C.p != null && C.p.PlayingTntWars) {
                int power = 2, threshold = 3;
                switch (TntWarsGame.GetTntWarsGame(C.p).GameDifficulty) {
                    case TntWarsGame.TntWarsDifficulty.Easy:
                        threshold = 7;
                        break;
                    case TntWarsGame.TntWarsDifficulty.Normal:
                        threshold = 5;
                        break;
                    case TntWarsGame.TntWarsDifficulty.Extreme:
                        power = 3;
                        break;
                }
                
                if (C.time < threshold) {
                    C.time++;
                    lvl.Blockchange(x, (ushort)(y + 1), z, lvl.GetTile(x, (ushort)(y + 1), z) == Block.lavastill
                                    ? Block.air : Block.lavastill);
                    return;
                }
                if (C.p.TntWarsKillStreak >= TntWarsGame.Properties.DefaultStreakTwoAmount 
                    && TntWarsGame.GetTntWarsGame(C.p).Streaks) {
                    power++;
                }
                lvl.MakeExplosion(x, y, z, power - 2, true, TntWarsGame.GetTntWarsGame(C.p));
                
                List<Player> Killed = new List<Player>();
                Player.players.ForEach(
                    delegate(Player p1)
                    {
                        if (p1.level == lvl && p1.PlayingTntWars && p1 != C.p
                            && Math.Abs((int)(p1.pos[0] / 32) - x) + Math.Abs((int)(p1.pos[1] / 32) - y) + Math.Abs((int)(p1.pos[2] / 32) - z) < ((power * 3) + 1)) {
                            Killed.Add(p1);
                        }
                    });
                TntWarsGame.GetTntWarsGame(C.p).HandleKill(C.p, Killed);
            } else {
                if (lvl.physics < 3) {
                    lvl.Blockchange(x, y, z, Block.air);
                } else {
                    if (C.time < 5 && lvl.physics == 3) {
                        C.time++;
                        lvl.Blockchange(x, (ushort)(y + 1), z, lvl.GetTile(x, (ushort)(y + 1), z) == Block.lavastill
                                        ? Block.air : Block.lavastill);
                        return;
                    }
                    lvl.MakeExplosion(x, y, z, 0);
                }
            }
        }
    }
}
