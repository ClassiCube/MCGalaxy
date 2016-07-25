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

namespace MCGalaxy.Commands {
    
    public sealed class CmdMissile : WeaponCmd {

        public override string name { get { return "missile"; } }
        public override string shortcut { get { return ""; } }
        protected override string Weapon { get { return "Missile"; } }

        protected override void PlacedMark(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            if (!p.staticCommands) {
                p.ClearBlockchange();
                p.aiming = false;
            }
            p.RevertBlock(x, y, z);
            if (!Block.canPlace(p, type)) { Formatter.MessageBlock(p, "place ", type); return; }

            Thread gunThread = new Thread(() => DoShoot(p, type, extType));
            gunThread.Name = "MCG_Missile";
            gunThread.Start();
        }

        void DoShoot(Player p, byte type, byte extType) {
            CatchPos bp = (CatchPos)p.blockchangeObject;
            List<Vec3U16> previous = new List<Vec3U16>(), allBlocks = new List<Vec3U16>();
            Vec3U16 pos = MakePos(p);

            int total = 0;
            List<Vec3U16> buffer = new List<Vec3U16>(2);
            while (true) {
                Vec3U16 start = MakePos(p);
                total++;
                double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

                Vec3U16 lookedAt;
                int i;
                for (i = 1; ; i++) {
                    lookedAt.X = (ushort)Math.Round(start.X + (double)(a * i));
                    lookedAt.Y = (ushort)Math.Round(start.Y + (double)(c * i));
                    lookedAt.Z = (ushort)Math.Round(start.Z + (double)(b * i));

                    byte tile = p.level.GetTile(lookedAt.X, lookedAt.Y, lookedAt.Z);
                    if (tile == Block.Zero) break;

                    if (tile != Block.air && !allBlocks.Contains(lookedAt) && HandlesHitBlock(p, tile, bp, pos, false))
                        break;

                    Player hit = GetPlayer(p, lookedAt, true);
                    if (hit != null) {
                        lookedAt = MakePos(hit); break;
                    }
                }

                lookedAt.X = (ushort)Math.Round(start.X + (double)(a * (i - 1)));
                lookedAt.Y = (ushort)Math.Round(start.Y + (double)(c * (i - 1)));
                lookedAt.Z = (ushort)Math.Round(start.Z + (double)(b * (i - 1)));
                FindNext(lookedAt, ref pos, buffer);

                byte by = p.level.GetTile(pos.X, pos.Y, pos.Z);
                if (total > 3) {
                    if (by != Block.air && !allBlocks.Contains(pos) && HandlesHitBlock(p, by, bp, pos, true))
                        break;

                    p.level.Blockchange(pos.X, pos.Y, pos.Z, type, extType);
                    previous.Add(pos);
                    allBlocks.Add(pos);

                    Player hitP = GetPlayer(p, pos, true);
                    if (hitP != null) {
                        if (p.level.physics >= 3 && bp.ending >= EndType.Explode)
                            hitP.HandleDeath(Block.stone, " was blown up by " + p.ColoredName, true);
                        else
                            hitP.HandleDeath(Block.stone, " was hit a missile from " + p.ColoredName);
                        break;
                    }

                    if (pos.X == lookedAt.X && pos.Y == lookedAt.Y && pos.Z == lookedAt.Z) {
                        if (p.level.physics >= 3 && bp.ending >= EndType.Explode && p.allowTnt) {
                            p.level.MakeExplosion(lookedAt.X, lookedAt.Y, lookedAt.Z, 2);
                            break;
                        }
                    }

                    if (previous.Count > 12) {
                        p.level.Blockchange(previous[0].X, previous[0].Y, previous[0].Z, Block.air, true);
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
                p.level.Blockchange(pos1.X, pos1.Y, pos1.Z, Block.air, true);
                Thread.Sleep(100);
            }
        }
        
        static Vec3U16 MakePos(Player p) {
            Vec3U16 pos;
            pos.X = (ushort)(p.pos[0] / 32);
            pos.Y = (ushort)(p.pos[1] / 32);
            pos.Z = (ushort)(p.pos[2] / 32);
            return pos;
        }
        
        void FindNext(Vec3U16 lookedAt, ref Vec3U16 pos, List<Vec3U16> buffer) {
            LineDrawOp.DrawLine(pos.X, pos.Y, pos.Z, 2, lookedAt.X, lookedAt.Y, lookedAt.Z, buffer);
            Vec3U16 end = buffer[buffer.Count - 1];
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
