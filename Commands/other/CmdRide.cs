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
namespace MCGalaxy.Commands
{
    public sealed class CmdRide : Command
    {
        public override string name { get { return "ride"; } }
        public override string shortcut { get { return ""; } }
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
            while (p.onTrain) {
                Thread.Sleep(3);
                ushort x = (ushort)(p.pos[0] / 32);
                ushort y = (ushort)(p.pos[1] / 32);
                ushort z = (ushort)(p.pos[2] / 32);

                for (ushort xx = (ushort)(x - 1); xx <= x + 1; xx++)
                    for (ushort yy = (ushort)(y - 1); yy <= y + 1; yy++)
                        for (ushort zz = (ushort)(z - 1); zz <= z + 1; zz++)
                {
                    if (p.level.GetTile(xx, yy, zz) != Block.train) continue;
                    p.invincible = true; p.trainGrab = true;
                    byte yaw = 0, pitch = 0;

                    if (y - yy == -1) pitch = 240;
                    else if (y - yy == 0) pitch = 0;
                    else pitch = 8;
                    ushort xT = (ushort)(xx * 32 + 16);
                    ushort yT = (ushort)((yy + 1) * 32 - 2);
                    ushort zT = (ushort)(zz * 32 + 16);

                    if (x - xx == -1) {
                        if (z - zz == -1) yaw = 96;
                        else if (z - zz == 0) yaw = 64;
                        else yaw = 32;
                    } else if (x - xx == 0) {
                        if (z - zz == -1) yaw = 128;
                        else if (z - zz == 0) { goto skip; }
                        else yaw = 0;
                    } else {
                        if (z - zz == -1) yaw = 160;
                        else if (z - zz == 0) yaw = 192;
                        else yaw = 224;
                    }
                    p.SendPos(0xFF, xT, yT, zT, yaw, pitch);
                    goto skip;
                }

                Thread.Sleep(3);
                p.invincible = false;
                p.trainGrab = false;
            skip:
                ;
            }

            Player.Message(p, "Dismounted");
            Thread.Sleep(1000);
            p.invincible = false;
            p.trainGrab = false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ride");
            Player.Message(p, "%HRides a nearby train.");
        }
    }
}
