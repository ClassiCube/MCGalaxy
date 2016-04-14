/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
        
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
using MCGalaxy.Games;

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
        
        public static void DoTntExplosion(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;			
            if (rand.Next(1, 11) <= 7)
                lvl.AddUpdate(C.b, Block.air);
        }
		
        public static void DoLargeTnt(Level lvl, ref Check C, int power) {
            Random rand = lvl.physRandom;			
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            
            if (lvl.physics < 3) {
                lvl.Blockchange(x, y, z, Block.air);
            } else {
                if (C.data.Data < 5 && lvl.physics == 3) {
                    C.data.Data++;
                    ShowWarningFuse(lvl, x, y, z);
                    return;
                }
                MakeExplosion(lvl, x, y, z, power);
            }
        }
        
        public static void DoSmallTnt(Level lvl, ref Check C) {
            Random rand = lvl.physRandom;            
            ushort x, y, z;
            lvl.IntToPos(C.b, out x, out y, out z);
            Player p = GetPlayer(ref C.data);
            
            if (p != null && p.PlayingTntWars) {
                int power = 2, threshold = 3;
                TntWarsGame game = TntWarsGame.GetTntWarsGame(p);
                switch (game.GameDifficulty) {
                    case TntWarsGame.TntWarsDifficulty.Easy:
                        threshold = 7; break;
                    case TntWarsGame.TntWarsDifficulty.Normal:
                        threshold = 5; break;
                    case TntWarsGame.TntWarsDifficulty.Extreme:
                        power = 3; break;
                }
                
                if (C.data.Data < threshold) {
                    C.data.Data++;
                    lvl.Blockchange(x, (ushort)(y + 1), z, lvl.GetTile(x, (ushort)(y + 1), z) == Block.lavastill
                                    ? Block.air : Block.lavastill);
                    return;
                }
                if (p.TntWarsKillStreak >= TntWarsGame.Properties.DefaultStreakTwoAmount && game.Streaks)
                    power++;
                MakeExplosion(lvl, x, y, z, power - 2, true, game);
                
                List<Player> Killed = new List<Player>();
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player p1 in players) {
                    if (p1.level == lvl && p1.PlayingTntWars && p1 != p
                        && Math.Abs((int)(p1.pos[0] / 32) - x) + Math.Abs((int)(p1.pos[1] / 32) - y) + Math.Abs((int)(p1.pos[2] / 32) - z) < ((power * 3) + 1)) {
                        Killed.Add(p1);
                    }
                }
                game.HandleKill(p, Killed);
            } else {
                if (lvl.physics < 3) {
                    lvl.Blockchange(x, y, z, Block.air);
                } else {
                    if (C.data.Data < 5 && lvl.physics == 3) {
                        C.data.Data++;
                        lvl.Blockchange(x, (ushort)(y + 1), z, lvl.GetTile(x, (ushort)(y + 1), z) == Block.lavastill
                                        ? Block.air : Block.lavastill);
                        return;
                    }
                    MakeExplosion(lvl, x, y, z, 0);
                }
            }
        }
        
        static Player GetPlayer(ref PhysicsArgs args) {
            if (args.Type1 != PhysicsArgs.TntWars) return null;
            
            int id = args.Value1 | args.Value2 << 8 | args.Data << 16;
            Player[] players = PlayerInfo.Online.Items;
            for (int i = 0; i < players.Length; i++) {
                if (players[i].tntWarsUuid == id) return players[i];
            }
            return null;
        }
        
        public static void MakeExplosion(Level lvl, ushort x, ushort y, ushort z, int size,
                                         bool force = false, TntWarsGame game = null) {
            Random rand = new Random();
            if ((lvl.physics < 2 || lvl.physics == 5) && !force) return;
            int index = lvl.PosToInt(x, y, z);
            if (index >= 0 && !Block.Properties[lvl.blocks[index]].OPBlock)
                lvl.AddUpdate(index, Block.tntexplosion, true);

            Explode(lvl, x, y, z, size + 1, rand, -1, game);
            Explode(lvl, x, y, z, size + 2, rand, 7, game);
            Explode(lvl, x, y, z, size + 3, rand, 3, game);
        }
        
        static void Explode(Level lvl, ushort x, ushort y, ushort z, 
                     int size, Random rand, int prob, TntWarsGame game) {
            for (int xx = (x - size); xx <= (x + size ); ++xx)
                for (int yy = (y - size ); yy <= (y + size); ++yy)
                    for (int zz = (z - size); zz <= (z + size); ++zz)
            {
                int index = lvl.PosToInt((ushort)xx, (ushort)yy, (ushort)zz);
                if (index < 0) continue;
                byte b = lvl.blocks[index];
                
                bool doDestroy = prob < 0 || rand.Next(1, 10) < prob;
                if (doDestroy && Block.Convert(b) != Block.tnt) {
                    if (game != null && b != Block.air) {
                        if (game.InZone((ushort)xx, (ushort)yy, (ushort)zz, false))
                            continue;
                    }
                    
                    int mode = rand.Next(1, 11);
                    if (mode <= 4) {
                        lvl.AddUpdate(index, Block.tntexplosion);
                    } else if (mode <= 8) {
                        lvl.AddUpdate(index, Block.air);
                    } else {
                    	PhysicsArgs args = default(PhysicsArgs);
					    args.Type1 = PhysicsArgs.Drop; args.Value1 = 50;
					    args.Type2 = PhysicsArgs.Dissipate; args.Value2 = 8;
					    lvl.AddCheck(index, false, args);
                    }
                } else if (b == Block.tnt) {
                    lvl.AddUpdate(index, Block.smalltnt);
                } else if (b == Block.smalltnt || b == Block.bigtnt || b == Block.nuketnt) {
                    lvl.AddCheck(index);
                }
            }
        }
    }
}
