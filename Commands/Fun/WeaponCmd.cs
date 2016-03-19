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

namespace MCGalaxy.Commands {
    
    public abstract class WeaponCmd : Command {
        
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected abstract string Weapon { get; }
        
        public override void Use(Player p, string message) {
            if (!p.level.guns) {
                Player.SendMessage(p, Weapon + "s cannot be used on this map!"); return;
            }
            if (p.hasflag != null) {
                Player.SendMessage(p, "You can't use a " + Weapon.ToLower() + " while you have the flag!"); return;
            }

            if (p.aiming && message == "") {
                p.aiming = false;
                p.ClearBlockchange();
                Player.SendMessage(p, "Disabled " + Weapon.ToLower() );
                return;
            }

            CatchPos cpos = default(CatchPos);
            cpos.ending = GetEnd(p, message);
            if (cpos.ending == EndType.Invalid) return;
            p.blockchangeObject = cpos;
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);

            p.SendMessage(Weapon + " mode engaged, fire at will");
            if (p.aiming) return;

            p.aiming = true;
            Thread aimThread = new Thread(() => DoAim(p));
            aimThread.Name = "MCG_WeaponAim";
            aimThread.Start();
        }
        
        EndType GetEnd(Player p, string mode) {
            if (mode == "") return EndType.Normal;
            if (mode.CaselessEquals("destroy")) return EndType.Destroy;
            if (mode.CaselessEquals("tp") || mode.CaselessEquals("teleport")) return EndType.Teleport;
            
            if (mode.CaselessEquals("explode")) {
                if (!p.allowTnt) Player.SendMessage(p, "Tnt usage is currently disallowed, switching to normal gun.");
                return p.allowTnt ? EndType.Explode : EndType.Destroy;
            }
            if (mode.CaselessEquals("laser")) {
                if (!p.allowTnt) Player.SendMessage(p, "Tnt usage is currently disallowed, switching to normal gun.");
                return p.allowTnt ? EndType.Laser : EndType.Destroy;
            }
            Help(p);
            return EndType.Invalid;
        }
        
        void DoAim(Player p) {
            List<Pos> lastSent = new List<Pos>(), toSend = new List<Pos>();
            while (p.aiming) {
                double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
                double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

                try {                    
                    ushort y = (ushort)Math.Round((ushort)(p.pos[1] / 32) + c * 3);
                    ushort x = (ushort)Math.Round((ushort)(p.pos[0] / 32) + a * 3);
                    ushort z = (ushort)Math.Round((ushort)(p.pos[2] / 32) + b * 3);

                    int dirX = Math.Sign(a) >= 0 ? 1 : -1;
                    int dirZ = Math.Sign(b) >= 0 ? 1 : -1;
                    CheckTile(p.level, toSend, x, y, z);
                    CheckTile(p.level, toSend, x + dirX, y, z);
                    CheckTile(p.level, toSend, x, y, z + dirZ);
                    CheckTile(p.level, toSend, x + dirX, y, z + dirZ);

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
        
        void CheckTile(Level lvl, List<Pos> toSend, int x, int y, int z) {
            Pos pos;
            if (lvl.GetTile((ushort)x, (ushort)(y - 1), (ushort)z) == Block.air) {
                pos.x = (ushort)x; pos.y = (ushort)y; pos.z = (ushort)z;
                toSend.Add(pos);
            }
            if (lvl.GetTile((ushort)x, (ushort)y, (ushort)z) == Block.air) {
                pos.x = (ushort)x; pos.y = (ushort)y; pos.z = (ushort)z;
                toSend.Add(pos);
            }
        }
        
        protected abstract void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType);
        
        protected Player GetPlayer(Player p, Pos pos, bool skipSelf) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level) continue;
                if (p == pl && skipSelf) continue;
                
                if (Math.Abs((pl.pos[0] / 32) - pos.x) <= 1
                    && Math.Abs((pl.pos[1] / 32) - pos.y) <= 1
                    && Math.Abs((pl.pos[2] / 32) - pos.z) <= 1)
                {
                    return pl;
                }
            }
            return null;
        }
        
        protected static void DoTeleport(Player p, Pos pos) {
            try {
                p.SendPos(0xFF, (ushort)(pos.x * 32), (ushort)(pos.y * 32 + 32), (ushort)(pos.z * 32), p.rot[0], p.rot[1]);
            } catch {
            }
        }
        
        protected bool HandlesHitBlock(Player p, byte type, CatchPos bp, Pos pos, bool doExplode) {
            if (p.level.physics < 2 || bp.ending == EndType.Teleport
                || bp.ending == EndType.Normal) return true;
            
            if (bp.ending == EndType.Destroy) {
                if ((!Block.FireKill(type) && !Block.NeedRestart(type)) && type != Block.glass) {
                    return true;
                }
            } else if (p.level.physics >= 3) {
                if (type != Block.glass && p.allowTnt) {
                    if (doExplode)
                        p.level.MakeExplosion(pos.x, pos.y, pos.z, 1);
                    return true;
                }
            } else {
                return true;
            }
            return false;
        }

        protected struct Pos { public ushort x, y, z; }
        protected struct CatchPos { public ushort x, y, z; public EndType ending; }
        protected enum EndType { Invalid, Normal, Destroy, Teleport, Explode, Laser };
    }
}
