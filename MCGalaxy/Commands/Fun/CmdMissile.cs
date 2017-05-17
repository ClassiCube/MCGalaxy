/*
    Copyright 2011 MCForge
        
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
using System.Threading;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Fun {   
    public sealed class CmdMissile : WeaponCmd {

        public override string name { get { return "missile"; } }
        protected override string Weapon { get { return "Missile"; } }

        protected override void PlacedMark(Player p, ushort x, ushort y, ushort z, ExtBlock block) {
            if (!p.staticCommands) {
                p.ClearBlockchange();
                p.aiming = false;
            }
            p.RevertBlock(x, y, z);
            if (!CommandParser.IsBlockAllowed(p, "place ", block)) return;

            Thread gunThread = new Thread(() => DoShoot(p, block));
            gunThread.Name = "MCG_Missile";
            gunThread.Start();
        }

        void DoShoot(Player p, ExtBlock block) {
            CatchPos bp = (CatchPos)p.blockchangeObject;
            List<Vec3U16> previous = new List<Vec3U16>(), allBlocks = new List<Vec3U16>();
            Vec3U16 pos = MakePos(p);

            int total = 0;
            List<Vec3S32> buffer = new List<Vec3S32>(2);
            while (true) {
                Vec3U16 start = MakePos(p);
                total++;
                Vec3F32 dir = DirUtils.GetFlatDirVector(p.Rot.RotY, p.Rot.HeadX);

                Vec3U16 lookedAt;
                int i;
                for (i = 1; ; i++) {
                    lookedAt.X = (ushort)Math.Round(start.X + (double)(dir.X * i));
                    lookedAt.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * i));
                    lookedAt.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * i));

                    byte tile = p.level.GetTile(lookedAt.X, lookedAt.Y, lookedAt.Z);
                    if (tile == Block.Invalid) break;

                    if (tile != Block.air && !allBlocks.Contains(lookedAt) && HandlesHitBlock(p, tile, bp, pos, false))
                        break;

                    Player hit = GetPlayer(p, lookedAt, true);
                    if (hit != null) {
                        lookedAt = MakePos(hit); break;
                    }
                }

                lookedAt.X = (ushort)Math.Round(start.X + (double)(dir.X * (i - 1)));
                lookedAt.Y = (ushort)Math.Round(start.Y + (double)(dir.Y * (i - 1)));
                lookedAt.Z = (ushort)Math.Round(start.Z + (double)(dir.Z * (i - 1)));
                FindNext(lookedAt, ref pos, buffer);

                byte by = p.level.GetTile(pos.X, pos.Y, pos.Z);
                if (total > 3) {
                    if (by != Block.air && !allBlocks.Contains(pos) && HandlesHitBlock(p, by, bp, pos, true))
                        break;

                    p.level.Blockchange(pos.X, pos.Y, pos.Z, block);
                    previous.Add(pos);
                    allBlocks.Add(pos);

                    Player hitP = GetPlayer(p, pos, true);
                    if (hitP != null) {
                        if (p.level.physics >= 3 && bp.ending >= EndType.Explode) {
                    	    hitP.HandleDeath((ExtBlock)Block.stone, " was blown up by " + p.ColoredName, true);
                        } else {
                            hitP.HandleDeath((ExtBlock)Block.stone, " was hit a missile from " + p.ColoredName);
                        }
                        break;
                    }

                    if (pos.X == lookedAt.X && pos.Y == lookedAt.Y && pos.Z == lookedAt.Z) {
                        if (p.level.physics >= 3 && bp.ending >= EndType.Explode && p.allowTnt) {
                            p.level.MakeExplosion(lookedAt.X, lookedAt.Y, lookedAt.Z, 2);
                            break;
                        }
                    }

                    if (previous.Count > 12) {
                        p.level.Blockchange(previous[0].X, previous[0].Y, previous[0].Z, ExtBlock.Air, true);
                        previous.RemoveAt(0);
                    }
                    Thread.Sleep(100);
                }
            }

            if (bp.ending == EndType.Teleport) {
                int index = previous.Count - 3;
                if (index >= 0 && index < previous.Count)
                    DoTeleport(p, previous[index]);
            }
            foreach (Vec3U16 pos1 in previous) {
                p.level.Blockchange(pos1.X, pos1.Y, pos1.Z, ExtBlock.Air, true);
                Thread.Sleep(100);
            }
        }
        
        static Vec3U16 MakePos(Player p) {
            return (Vec3U16)p.Pos.BlockCoords;
        }
        
        void FindNext(Vec3U16 lookedAt, ref Vec3U16 pos, List<Vec3S32> buffer) {
            LineDrawOp.DrawLine(pos.X, pos.Y, pos.Z, 2, lookedAt.X, lookedAt.Y, lookedAt.Z, buffer);
            Vec3U16 end = (Vec3U16)buffer[buffer.Count - 1];
            pos.X = end.X; pos.Y = end.Y; pos.Z = end.Z;
            buffer.Clear();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/missile [at end]");
            Player.Message(p, "%HAllows you to fire missiles at people. Differs from /gun in that the missile is guided.");
            Player.Message(p, "%HAvailable [at end] types: %Sexplode, destroy, tp");
        }
    }
}
