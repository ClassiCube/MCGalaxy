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

        protected override void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            if (!p.staticCommands) {
                p.ClearBlockchange();
                p.aiming = false;
            }
            p.RevertBlock(x, y, z);
            if (!Block.canPlace(p, type)) { Player.SendMessage(p, "You cannot place this block."); return; }

            Thread gunThread = new Thread(() => DoShoot(p, type, extType));
            gunThread.Name = "MCG_Missile";
            gunThread.Start();
        }

        void DoShoot(Player p, byte type, byte extType) {
            CatchPos bp = (CatchPos)p.blockchangeObject;
            List<Pos> previous = new List<Pos>(), allBlocks = new List<Pos>();
            Pos pos = MakePos(p);

            int total = 0;
            List<FillPos> buffer = new List<FillPos>(2);
            while (true) {
                Pos start = MakePos(p);
                total++;
                double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

                Pos lookedAt;
                int i;
                for (i = 1; ; i++) {
                    lookedAt.x = (ushort)Math.Round(start.x + (double)(a * i));
                    lookedAt.y = (ushort)Math.Round(start.y + (double)(c * i));
                    lookedAt.z = (ushort)Math.Round(start.z + (double)(b * i));

                    byte tile = p.level.GetTile(lookedAt.x, lookedAt.y, lookedAt.z);
                    if (tile == Block.Zero) break;

                    if (tile != Block.air && !allBlocks.Contains(lookedAt) && HandlesHitBlock(p, tile, bp, pos, false))
                        break;

                    Player hit = GetPlayer(p, lookedAt, true);
                    if (hit != null) {
                        lookedAt = MakePos(hit); break;
                    }
                }

                lookedAt.x = (ushort)Math.Round(start.x + (double)(a * (i - 1)));
                lookedAt.y = (ushort)Math.Round(start.y + (double)(c * (i - 1)));
                lookedAt.z = (ushort)Math.Round(start.z + (double)(b * (i - 1)));
                FindNext(lookedAt, ref pos, buffer);

                byte by = p.level.GetTile(pos.x, pos.y, pos.z);
                if (total > 3) {
                    if (by != Block.air && !allBlocks.Contains(pos) && HandlesHitBlock(p, by, bp, pos, true))
                        break;

                    p.level.Blockchange(pos.x, pos.y, pos.z, type, extType);
                    previous.Add(pos);
                    allBlocks.Add(pos);

                    Player hitP = GetPlayer(p, pos, true);
                    if (hitP != null) {
                        if (p.level.physics >= 3 && bp.ending >= EndType.Explode)
                            hitP.HandleDeath(Block.stone, " was blown up by " + p.color + p.name, true);
                        else
                            hitP.HandleDeath(Block.stone, " was hit a missile from " + p.color + p.name);
                        break;
                    }

                    if (pos.x == lookedAt.x && pos.y == lookedAt.y && pos.z == lookedAt.z) {
                        if (p.level.physics >= 3 && bp.ending >= EndType.Explode && p.allowTnt) {
                            p.level.MakeExplosion(lookedAt.x, lookedAt.y, lookedAt.z, 2);
                            break;
                        }
                    }

                    if (previous.Count > 12) {
                        p.level.Blockchange(previous[0].x, previous[0].y, previous[0].z, Block.air, true);
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
            foreach (Pos pos1 in previous) {
                p.level.Blockchange(pos1.x, pos1.y, pos1.z, Block.air, true);
                Thread.Sleep(100);
            }
        }
        
        static Pos MakePos(Player p) {
            Pos pos;
            pos.x = (ushort)(p.pos[0] / 32);
            pos.y = (ushort)(p.pos[1] / 32);
            pos.z = (ushort)(p.pos[2] / 32);
            return pos;
        }
        
        void FindNext(Pos lookedAt, ref Pos pos, List<FillPos> buffer) {
            LineDrawOp.DrawLine(pos.x, pos.y, pos.z, 2, lookedAt.x, lookedAt.y, lookedAt.z, buffer);
            FillPos end = buffer[buffer.Count - 1];
            pos.x = end.X; pos.y = end.Y; pos.z = end.Z;
            buffer.Clear();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/missile [at end] - Allows you to fire missiles at people");
            Player.SendMessage(p, "Available [at end] values: &cexplode, destroy, tp");
            Player.SendMessage(p, "Differs from /gun in that the missile is guided");
        }
    }
}
