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
    public sealed class CmdFly : Command {
        public override string name { get { return "fly"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdFly() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (!Hacks.CanUseHacks(p, p.level)) {
                Player.Message(p, "You cannot use /fly on this map.");
                p.isFlying = false; return;
            }
            
            p.isFlying = !p.isFlying;
            if (!p.isFlying) return;
            Player.Message(p, "You are now flying. &cJump!");

            Thread flyThread = new Thread(() => FlyThread(p));
            flyThread.Name = "MCG_Fly";
            flyThread.Start();
        }
        
        void FlyThread(Player p) {
            Position oldpos = default(Position);
            List<Vec3U16> last = new List<Vec3U16>(), next = new List<Vec3U16>();
            while (p.isFlying && !p.disconnected)
                DoFly(p, ref oldpos, last, next);

            foreach (Vec3U16 cP in last)
                p.SendBlockchange(cP.X, cP.Y, cP.Z, Block.air, 0);
            Player.Message(p, "Stopped flying");
        }
        
        void DoFly(Player p, ref Position old, List<Vec3U16> last, List<Vec3U16> next) {
            Thread.Sleep(20);
            if (p.Pos == old) return;
            
            try {
                int x = p.Pos.BlockX, z = p.Pos.BlockZ;
                int y = (p.Pos.Y - 60) / 32;

                for (int yy = y - 1; yy <= y; yy++)
                    for (int zz = z - 2; zz <= z + 2; zz++)
                        for (int xx = x - 2; xx <= x + 2; xx++)
                {
                    ushort offX = (ushort)xx, offY = (ushort)yy, offZ = (ushort)zz;
                    if (p.level.GetTile(offX, offY, offZ) != Block.air) continue;
                    
                    Vec3U16 pos;
                    pos.X = offX; pos.Y = offY; pos.Z = offZ;
                    next.Add(pos);
                }
                
                foreach (Vec3U16 P in next) {
                    if (last.Contains(P)) continue;
                    last.Add(P);
                    p.SendBlockchange(P.X, P.Y, P.Z, Block.glass, 0);
                }
                
                for (int i = 0; i < last.Count; i++) {
                    Vec3U16 P = last[i];
                    if (next.Contains(P)) continue;
                    
                    p.SendBlockchange(P.X, P.Y, P.Z, Block.air, 0);
                    last.RemoveAt(i); i--;
                }
                next.Clear();
            } catch (Exception ex) { Server.ErrorLog(ex); }
            old = p.Pos;
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            Player.Message(p, "%T/fly");
            Player.Message(p, "%HThe old method of flight before custom clients.");
            Player.Message(p, "%HMay not work at all depending on your connection.");
            Player.Message(p, "%H  Does not work on maps which have -hax in their motd. " +
                           "(unless you are {0}%H+ and the motd also has +ophax)", name);
        }
    }
}
