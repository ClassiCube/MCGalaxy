/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Threading;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdRide : Command {
        public override string name { get { return "ride"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdRide() { }

        public override void Use(Player p, string message) {
            p.onTrain = !p.onTrain;
            if (!p.onTrain) return;
            Thread trainThread = new Thread(() => DoRide(p));
            trainThread.Name = "MCG_RideTrain";
            trainThread.Start();
            Player.Message(p, "Stand near a train to mount it");
        }
        
        void DoRide(Player p) {
           p.trainInvincible = true;
            while (p.onTrain) {
                Thread.Sleep(10);
                Vec3S32 P = p.Pos.BlockCoords;

                for (int dx = -1; dx <= 1; dx++)
                    for (int dy = -1; dy <= 1; dy++)
                        for (int dz = -1; dz <= 1; dz++)
                {
                    ushort xx = (ushort)(P.X + dx), yy = (ushort)(P.Y + dy), zz = (ushort)(P.Z + dz);
                    if (p.level.GetTile(xx, yy, zz) != Block.train) continue;
                    p.trainGrab = true;
                    
                    byte yaw, pitch;
                    Vec3F32 dir = new Vec3F32(dx, 0, dz);
                    DirUtils.GetYawPitch(dir, out yaw, out pitch);
                    
                    if (dy == 1) pitch = 240;
                    else if (dy == 0) pitch = 0;
                    else pitch = 8;
                    
                    if (dx != 0 || dy != 0 || dz != 0) {
                        PlayerActions.MoveCoords(p, P.X + dx, P.Y + dy, P.Z + dz, yaw, pitch);
                    }
                    goto skip;
                }

                Thread.Sleep(10);
                p.trainGrab = false;
            skip:
                ;
            }

            p.trainGrab = false;
            Player.Message(p, "Dismounted");
            Thread.Sleep(1000);
            p.trainInvincible = false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ride");
            Player.Message(p, "%HRides a nearby train.");
        }
    }
}
