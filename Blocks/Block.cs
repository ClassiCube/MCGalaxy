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
			    || (type >= yellowflower && type <= redmushroom) || type == rope;
        }

        public static bool AllowBreak(byte type)
        {	
            switch (type)
            {
                case blue_portal:
                case orange_portal:

                case MsgWhite:
                case MsgBlack:

                case door_tree:
                case door_obsidian:
                case door_glass:
                case door_stone:
                case door_leaves:
                case door_sand:
                case door_wood:
                case door_green:
                case door_tnt:
                case door_stair:
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

                case c4:
                case smalltnt:
                case bigtnt:
                case nuketnt:
                case rocketstart:
                case firework:

                case zombiebody:
                case creeper:
                case zombiehead:
                    return true;
            }
            return false;
        }

        public static bool Placable(byte type) {
        	return !(type == blackrock || (type >= water && type <= lavastill))
			    && type < CpeCount;
        }

        public static bool RightClick(byte type, bool countAir = false) {
            if (countAir && type == air) return true;
            return type >= water && type <= lavastill;			
        }

        public static bool OPBlocks(byte type) { return Properties[type].OPBlock; }

        public static bool Death(byte type)
        {
            switch (type)
            {
                case tntexplosion:

                case deathwater:
                case deathlava:
                case deathair:
                case activedeathlava:
                case activedeathwater:
                case fastdeathlava:

                case magma:
                case geyser:

                case birdkill:
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

        public static bool BuildIn(byte type) {
            if (type == op_water || type == op_lava || portal(type) || mb(type)) return false;
			type = Convert(type);
			return type >= water && type <= lavastill;
        }

        public static bool Mover(byte type) { return walkthroughHandlers[type] != null; }

        public static bool FireKill(byte type) { return type != air && Properties[type].LavaKills; }
        
        public static bool LavaKill(byte type) { return Properties[type].LavaKills; }
		
        public static bool WaterKill(byte type) { return Properties[type].WaterKills; }

        public static bool LightPass(byte type, byte extType, BlockDefinition[] defs) {
            switch (Convert(type)) {
                case air:
                case glass:
                case leaf:
                case redflower:
                case yellowflower:
                case mushroom:
                case redmushroom:
                case shrub:
                case rope:
                    return true;
                case custom_block:
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
                case rock:
                case stone:
                case blackrock:
                case waterstill:
                case lavastill:
                case goldrock:
                case ironrock:
                case coal:

                case goldsolid:
                case iron:
                case staircasefull:
                case brick:
                case tnt:
                case stonevine:
                case obsidian:

                case op_glass:
                case opsidian:
                case op_brick:
                case op_stone:
                case op_cobblestone:
                case op_air:
                case op_water:

                case door_tree:
                case door_obsidian:
                case door_glass:
                case door_stone:
                case door_leaves:
                case door_sand:
                case door_wood:
                case door_green:
                case door_tnt:
                case door_stair:
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
                case air_switch:
                case water_door:
                case lava_door:

                case MsgAir:
                case MsgWater:
                case MsgLava:
                case MsgBlack:
                case MsgWhite:

                case blue_portal:
                case orange_portal:
                case air_portal:
                case water_portal:
                case lava_portal:

                case deathair:
                case deathlava:
                case deathwater:

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
