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
using System;
using MCGalaxy.Blocks;

namespace MCGalaxy
{
    public sealed partial class Block
    {
        public static bool Walkthrough(byte type) {
			return type == air || type == shrub || (type >= water && type <= lavastill) 
			    && (type >= yellowflower && type <= redmushroom);
        }

        public static bool AllowBreak(byte type)
        {	
            switch (type)
            {
                case Block.blue_portal:
                case Block.orange_portal:

                case Block.MsgWhite:
                case Block.MsgBlack:

                case Block.door_tree:
                case Block.door_obsidian:
                case Block.door_glass:
                case Block.door_stone:
                case Block.door_leaves:
                case Block.door_sand:
                case Block.door_wood:
                case Block.door_green:
                case Block.door_tnt:
                case Block.door_stair:
                case door_iron:
                case door_gold:
                case door_dirt:
                case door_grass:
                case door_blue:
                case door_book:
                case door_cobblestone:
                case door_red:

                case door_orange:
                case door_yellow:
                case door_lightgreen:
                case door_aquagreen:
                case door_cyan:
                case door_lightblue:
                case door_purple:
                case door_lightpurple:
                case door_pink:
                case door_darkpink:
                case door_darkgrey:
                case door_lightgrey:
                case door_white:

                case Block.c4:
                case Block.smalltnt:
                case Block.bigtnt:
                case Block.nuketnt:
                case Block.rocketstart:
                case Block.firework:

                case zombiebody:
                case creeper:
                case zombiehead:
                    return true;
            }
            return false;
        }

        public static bool Placable(byte type) {
        	return !(type == blackrock || (type >= water && type <= lavastill))
			    && type < Block.CpeCount;
        }

        public static bool RightClick(byte type, bool countAir = false) {
            if (countAir && type == Block.air) return true;
            return type >= water && type <= lavastill;			
        }

        public static bool OPBlocks(byte type) { return Properties[type].OPBlock; }

        public static bool Death(byte type)
        {
            switch (type)
            {
                case Block.tntexplosion:

                case Block.deathwater:
                case Block.deathlava:
                case Block.deathair:
                case activedeathlava:
                case activedeathwater:
                case fastdeathlava:

                case Block.magma:
                case Block.geyser:

                case Block.birdkill:
                case fishshark:
                case fishlavashark:

                case train:

                case snake:

                case lava_fire:
                case rockethead:

                case creeper:
                case zombiebody:
                    //case zombiehead:
                    return true;
            }
            return false;
        }

        public static bool BuildIn(byte type)
        {
            if (type == op_water || type == op_lava || Block.portal(type) || Block.mb(type)) return false;
			type = Block.Convert(type);
			return type >= water && type <= lavastill;
        }

        public static bool Mover(byte type) { return walkthroughHandlers[type] != null; }

        public static bool FireKill(byte type) { return type != air && Properties[type].KilledByLava; }
        
        public static bool LavaKill(byte type) { return Properties[type].KilledByLava; }
		
        public static bool WaterKill(byte type) { return Properties[type].KilledByWater; }

        public static bool LightPass(byte type, byte extType, BlockDefinition[] defs) {
            switch (Convert(type)) {
                case Block.air:
                case Block.glass:
                case Block.leaf:
                case Block.redflower:
                case Block.yellowflower:
                case Block.mushroom:
                case Block.redmushroom:
                case Block.shrub:
                case Block.rope:
                    return true;
                case Block.custom_block:
                    BlockDefinition def = defs[extType];
                    return def == null ? false : !def.BlocksLight;
                default:
                    return false;
            }
        }

        public static bool NeedRestart(byte type)
        {
            switch (type)
            {
                case train:

                case snake:
                case snaketail:

                case lava_fire:
                case rockethead:
                case firework:

                case creeper:
                case zombiebody:
                case zombiehead:

                case birdblack:
                case birdblue:
                case birdkill:
                case birdlava:
                case birdred:
                case birdwater:
                case birdwhite:

                case fishbetta:
                case fishgold:
                case fishsalmon:
                case fishshark:
                case fishlavashark:
                case fishsponge:

                case tntexplosion:
                    return true;
            }
            return false;
        }

        public static bool portal(byte type) { return Properties[type].IsPortal; }
        
        public static bool mb(byte type) { return Properties[type].IsMessageBlock; }

        public static bool Physics(byte type)   //returns false if placing block cant actualy cause any physics to happen
        {
            switch (type)
            {
                case Block.rock:
                case Block.stone:
                case Block.blackrock:
                case Block.waterstill:
                case Block.lavastill:
                case Block.goldrock:
                case Block.ironrock:
                case Block.coal:

                case Block.goldsolid:
                case Block.iron:
                case Block.staircasefull:
                case Block.brick:
                case Block.tnt:
                case Block.stonevine:
                case Block.obsidian:

                case Block.op_glass:
                case Block.opsidian:
                case Block.op_brick:
                case Block.op_stone:
                case Block.op_cobblestone:
                case Block.op_air:
                case Block.op_water:

                case Block.door_tree:
                case Block.door_obsidian:
                case Block.door_glass:
                case Block.door_stone:
                case Block.door_leaves:
                case Block.door_sand:
                case Block.door_wood:
                case Block.door_green:
                case Block.door_tnt:
                case Block.door_stair:
                case door_iron:
                case door_gold:
                case door_dirt:
                case door_grass:
                case door_blue:
                case door_book:
                case door_cobblestone:
                case door_red:

                case door_orange:
                case door_yellow:
                case door_lightgreen:
                case door_aquagreen:
                case door_cyan:
                case door_lightblue:
                case door_purple:
                case door_lightpurple:
                case door_pink:
                case door_darkpink:
                case door_darkgrey:
                case door_lightgrey:
                case door_white:

                case tdoor:
                case tdoor2:
                case tdoor3:
                case tdoor4:
                case tdoor5:
                case tdoor6:
                case tdoor7:
                case tdoor8:
                case tdoor9:
                case tdoor10:
                case tdoor11:
                case tdoor12:
                case tdoor13:

                case air_door:
                case Block.air_switch:
                case Block.water_door:
                case lava_door:

                case Block.MsgAir:
                case Block.MsgWater:
                case Block.MsgLava:
                case Block.MsgBlack:
                case Block.MsgWhite:

                case Block.blue_portal:
                case Block.orange_portal:
                case Block.air_portal:
                case Block.water_portal:
                case Block.lava_portal:

                case Block.deathair:
                case Block.deathlava:
                case Block.deathwater:

                case flagbase:
                    return false;

                default:
                    return true;
            }
        }
        
        public static byte DoorAirs(byte b) { return Properties[b].DoorAirId; }

        public static bool tDoor(byte type) { return Properties[type].IsTDoor; }

        public static byte odoor(byte type) { return Properties[type].ODoorId; }
    }
}
