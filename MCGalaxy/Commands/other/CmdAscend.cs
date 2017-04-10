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

namespace MCGalaxy.Commands {
    public class CmdAscend : Command {
        public override string name { get { return "ascend"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdAscend() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (!Hacks.CanUseHacks(p, p.level)) {
                Player.Message(p, "You cannot use /ascend on this map."); return;
            }
            ushort x = (ushort)(p.pos[0] / 32), y = (ushort)(p.pos[1] / 32), z = (ushort)(p.pos[2] / 32);
            
            while (y < p.level.Height) {
                y++;
                byte block = p.level.GetTile(x, y, z);
                if (!(Block.Convert(block) == Block.air || block == Block.Invalid)) continue;               
                byte above = p.level.GetTile(x, (ushort)(y + 1), z);             
                if (!(Block.Convert(above) == Block.air || above == Block.Invalid)) continue;

                byte below = p.level.GetTile(x, (ushort)(y - 1), z);
                if (Solid(Block.Convert(below))) {
                    Player.Message(p, "Teleported you up.");
                    p.SendOwnFeetPos(p.pos[0], (ushort)(y * 32), p.pos[2], p.rot[0], p.rot[1]);
                    return;
                }
            }
            Player.Message(p, "No free spaces found above you");
        }
        
        static bool Solid(byte b) {
            return b != Block.air && (b < Block.water || b > Block.lavastill) && b != Block.Invalid
                && b != Block.shrub && (b < Block.yellowflower || b > Block.redmushroom);
        }
        
        public override void Help(Player p) {
            string name = Group.GetColoredName(LevelPermission.Operator);
            Player.Message(p, "%T/ascend");
            Player.Message(p, "%HTeleports you to the first free space above you.");
            Player.Message(p, "%H  Does not work on maps which have -hax in their motd. " +
                           "(unless you are {0}%H+ and the motd also has +ophax)", name);
        }
    }
}
