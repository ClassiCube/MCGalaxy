/*
	Copyright 2011 MCForge
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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


namespace MCGalaxy.Commands
{
    public class CmdAscend : Command
    {
        public override string name { get { return "ascend"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdAscend() { }

        public override void Use(Player p, string message)
        {
            ushort max = p.level.Height;
            ushort posy = (ushort)(p.pos[1] / 32);
            bool found = false;
            ushort xpos = (ushort)(p.pos[0] / 32);
            ushort zpos = (ushort)(p.pos[2] / 32);
            while (!found && posy < max)
            {
                posy++;
                byte block = p.level.GetTile(xpos, posy, zpos);
                if (block == Block.air || block == Block.air_door || block == Block.air_switch || block == Block.Zero)
                {
                    ushort blockabove = (ushort) (posy + 1);
                    ushort blockunder = (ushort) (posy - 1);
                    if (p.level.GetTile(xpos, blockabove, zpos) == Block.air || p.level.GetTile(xpos, blockabove, zpos) == Block.air_door || p.level.GetTile(xpos, blockabove, zpos) == Block.air_switch || p.level.GetTile(xpos, blockabove, zpos) == Block.Zero)
                    {
                        if (p.level.GetTile(xpos, blockunder, zpos) != Block.air && p.level.GetTile(xpos, blockunder, zpos) != Block.air_switch && p.level.GetTile(xpos, blockunder, zpos) != Block.air_door && p.level.GetTile(xpos, blockunder, zpos) != Block.air_flood && p.level.GetTile(xpos, blockunder, zpos) != Block.air_flood_down && p.level.GetTile(xpos, blockunder, zpos) != Block.air_flood_layer && p.level.GetTile(xpos, blockunder, zpos) != Block.air_flood_up && p.level.GetTile(xpos, blockunder, zpos) != Block.air_portal && p.level.GetTile(xpos, blockunder, zpos) != Block.redflower && p.level.GetTile(xpos, blockunder, zpos) != Block.yellowflower && p.level.GetTile(xpos, blockunder, zpos) != Block.finiteWater && p.level.GetTile(xpos, blockunder, zpos) != Block.finiteLava && p.level.GetTile(xpos, blockunder, zpos) != Block.fire && p.level.GetTile(xpos, blockunder, zpos) != Block.water && p.level.GetTile(xpos, blockunder, zpos) != Block.water_door && p.level.GetTile(xpos, blockunder, zpos) != Block.water_portal && p.level.GetTile(xpos, blockunder, zpos) != Block.WaterDown && p.level.GetTile(xpos, blockunder, zpos) != Block.WaterFaucet && p.level.GetTile(xpos, blockunder, zpos) != Block.lava && p.level.GetTile(xpos, blockunder, zpos) != Block.lava_door && p.level.GetTile(xpos, blockunder, zpos) != Block.lava_fast && p.level.GetTile(xpos, blockunder, zpos) != Block.lava_portal && p.level.GetTile(xpos, blockunder, zpos) != Block.LavaDown && p.level.GetTile(xpos, blockunder, zpos) != Block.lavastill && p.level.GetTile(xpos, blockunder, zpos) != Block.Zero)
                        {
                            Player.Message(p, "Teleported you up!");
                            p.SendPos(0xFF, p.pos[0], (ushort)((posy + 1) * 32), p.pos[2], p.rot[0], p.rot[1]);
                            found = true;
                        }
                    }
                }
            }
            if (!found)
            {
                Player.Message(p, "No free spaces found above you");
            }
        }
        public override void Help(Player p)
        {
            Player.Message(p, "%T/ascend");
            Player.Message(p, "%HTeleports you to the first free space above you.");
        }
    }
}
