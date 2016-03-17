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
namespace MCGalaxy.Commands
{
    public sealed class CmdGun : Command
    {
        public override string name { get { return "gun"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdGun() { }
        
        public override void Use(Player p, string message) {
            Level foundLevel = p.level;
            if (!p.level.guns) {
                Player.SendMessage(p, "Guns and missiles cannot be used on this map!"); return;
            }
            if (p.hasflag != null) { Player.SendMessage(p, "You can't use a gun while you have the flag!"); return; }

            if (p.aiming && message == "") {
                p.aiming = false;
                p.ClearBlockchange();
                Player.SendMessage(p, "Disabled gun");
                return;
            }

            CatchPos cpos = default(CatchPos);
            cpos.ending = GetMode(p, message);
            if (cpos.ending == GunType.Invalid) return;
            p.blockchangeObject = cpos;
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);

            p.SendMessage("Gun mode engaged, fire at will");
            if (p.aiming) return;

            p.aiming = true;
            Thread aimThread = new Thread(() => DoAim(p));
            aimThread.Name = "MCG_GunAim";
            aimThread.Start();
        }
        
        GunType GetMode(Player p, string mode) {
            if (mode == "") return GunType.Normal;
            if (mode.CaselessEquals("destroy")) return GunType.Destroy;
            if (mode.CaselessEquals("tp") || mode.CaselessEquals("teleport")) return GunType.Teleport;
            
            if (mode.CaselessEquals("explode")) {
                if (!p.allowTnt) Player.SendMessage(p, "Tnt usage is currently disallowed, switching to normal gun.");
                return p.allowTnt ? GunType.Explode : GunType.Destroy;
            }
            if (mode.CaselessEquals("laser")) {
                if (!p.allowTnt) Player.SendMessage(p, "Tnt usage is currently disallowed, switching to normal gun.");
                return p.allowTnt ? GunType.Laser : GunType.Destroy;
            }
            Help(p);
            return GunType.Invalid;
        }
        
        void DoAim(Player p) {
            Pos pos;
            List<Pos> lastSent = new List<Pos>(), toSend = new List<Pos>();
            while (p.aiming) {
                double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

                try {
                    ushort x = (ushort)Math.Round((ushort)(p.pos[0] / 32) + a * 3);
                    ushort y = (ushort)Math.Round((ushort)(p.pos[1] / 32) + c * 3);
                    ushort z = (ushort)Math.Round((ushort)(p.pos[2] / 32) + b * 3);

                    if (x >= p.level.Width || y >= p.level.Height || z >= p.level.Length) {
                        Thread.Sleep(20); continue;
                    }

                    for (ushort xx = x; xx <= x + 1; xx++)
                        for (ushort yy = (ushort)(y - 1); yy <= y; yy++)
                            for (ushort zz = z; zz <= z + 1; zz++)
                    {
                        if (p.level.GetTile(xx, yy, zz) == Block.air) {
                            pos.x = xx; pos.y = yy; pos.z = zz;
                            toSend.Add(pos);
                        }
                    }

                    // Revert all glass blocks now not in the ray from the player's direction
                    for (int i = 0; i < lastSent.Count; i++) {
                        Pos cP = lastSent[i];
                        if (toSend.Contains(cP)) continue;
                        
                        p.SendBlockchange(cP.x, cP.y, cP.z, Block.air);
                        lastSent.RemoveAt(i); i--;
                    }

                    // Place the new glass blocks that are in the ray from the player's direction
                    foreach (Pos cP in toSend) {
                        if (lastSent.Contains(cP)) continue;
                        lastSent.Add(cP);
                        p.SendBlockchange(cP.x, cP.y, cP.z, Block.glass);
                    }
                    toSend.Clear();
                }
                catch { }
                Thread.Sleep(20);
            }
            
            foreach (Pos cP in lastSent)
                p.SendBlockchange(cP.x, cP.y, cP.z, Block.air);
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.RevertBlock(x, y, z);
            if (p.modeType != Block.air)
                type = p.modeType;

            Thread gunThread = new Thread(() => DoShoot(p, type, extType));
            gunThread.Name = "MCG_Gun";
            gunThread.Start();
        }
        
        void DoShoot(Player p, byte type, byte extType) {
            CatchPos bp = (CatchPos)p.blockchangeObject;
            double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
            double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
            double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

            double bigDiag = Math.Sqrt(Math.Sqrt(p.level.Width * p.level.Width + p.level.Length * p.level.Length) + p.level.Height * p.level.Height + p.level.Width * p.level.Width);

            List<Pos> previous = new List<Pos>();
            List<Pos> allBlocks = new List<Pos>();
            Pos pos;
            
            ushort startX = (ushort)(p.pos[0] / 32);
            ushort startY = (ushort)(p.pos[1] / 32);
            ushort startZ = (ushort)(p.pos[2] / 32);

            pos.x = (ushort)Math.Round(startX + (double)(a * 3));
            pos.y = (ushort)Math.Round(startY + (double)(c * 3));
            pos.z = (ushort)Math.Round(startZ + (double)(b * 3));

            for (double t = 4; bigDiag > t; t++) {
                pos.x = (ushort)Math.Round(startX + (double)(a * t));
                pos.y = (ushort)Math.Round(startY + (double)(c * t));
                pos.z = (ushort)Math.Round(startZ + (double)(b * t));

                byte by = p.level.GetTile(pos.x, pos.y, pos.z);
                if (by != Block.air && !allBlocks.Contains(pos) && HandlesHitBlock(p, by, bp, pos))
                    break;

                p.level.Blockchange(pos.x, pos.y, pos.z, type, extType);
                previous.Add(pos);
                allBlocks.Add(pos);

                if (HandlesPlayers(p, bp, pos)) break;

                if (t > 12 && bp.ending != GunType.Laser) {
                    pos = previous[0];
                    p.level.Blockchange(pos.x, pos.y, pos.z, Block.air);
                    previous.RemoveAt(0);
                }
                
                if (bp.ending != GunType.Laser) Thread.Sleep(20);
            }

            if (bp.ending == GunType.Teleport) {
                int index = previous.Count - 3;
                if (index >= 0 && index < previous.Count)
                	DoTeleport(p, previous[index]);
            }
            if (bp.ending == GunType.Laser) Thread.Sleep(400);

            foreach (Pos pos1 in previous) {
                p.level.Blockchange(pos1.x, pos1.y, pos1.z, Block.air);
                if (bp.ending != GunType.Laser) Thread.Sleep(20);
            }
        }
        
        bool HandlesHitBlock(Player p, byte type, CatchPos bp, Pos pos) {
            if (p.level.physics < 2 || bp.ending == GunType.Teleport
                || bp.ending == GunType.Normal) return true;
            
            if (bp.ending == GunType.Destroy) {
                if ((!Block.FireKill(type) && !Block.NeedRestart(type)) && type != Block.glass) {
                    return true;
                }
            } else if (p.level.physics >= 3) {
                if (type != Block.glass && p.allowTnt) {
                    p.level.MakeExplosion(pos.x, pos.y, pos.z, 1);
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }
        
        bool HandlesPlayers(Player p, CatchPos bp, Pos pos) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level) continue;
                
                if (Math.Abs((pl.pos[0] / 32) - pos.x) <= 1
                    && Math.Abs((pl.pos[1] / 32) - pos.y) <= 1
                    && Math.Abs((pl.pos[2] / 32) - pos.z) <= 1)
                {
                    if (p.level.physics >= 3 && bp.ending >= GunType.Explode)
                        pl.HandleDeath(Block.stone, " was blown up by " + p.color + p.DisplayName, true);
                    else
                        pl.HandleDeath(Block.stone, " was shot by " + p.color + p.DisplayName);
                    return true;
                }
            }
            return false;
        }
        
        static void DoTeleport(Player p, Pos pos) {
            try {
                p.SendPos(0xFF, (ushort)(pos.x * 32), (ushort)(pos.y * 32 + 32), (ushort)(pos.z * 32), p.rot[0], p.rot[1]);
            } catch {
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/gun [at end] - Allows you to fire bullets at people");
            Player.SendMessage(p, "Available [at end] values: &cexplode, destroy, laser, tp");
        }

        struct Pos { public ushort x, y, z; }
        struct CatchPos { public ushort x, y, z; public GunType ending; }
        enum GunType { Invalid, Normal, Destroy, Teleport, Explode, Laser };
    }
}
