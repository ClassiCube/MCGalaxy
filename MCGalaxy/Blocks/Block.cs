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
using MCGalaxy.Maths;

namespace MCGalaxy {
    public sealed partial class Block {

        public static bool Walkthrough(byte block) {
            return block == air || block == shrub || block == Block.snow
                || block == fire || block == rope
                || (block >= water && block <= lavastill)
                || (block >= yellowflower && block <= redmushroom);
        }

        public static bool AllowBreak(byte block) {
            switch (block) {
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

        public static bool Placable(byte block) {
            return !(block == blackrock || (block >= water && block <= lavastill))
                && block < CpeCount;
        }

        public static bool RightClick(byte block, bool countAir = false) {
            if (countAir && block == air) return true;
            return block >= water && block <= lavastill;
        }

        public static bool BuildIn(byte block) {
            if (block == op_water || block == op_lava
                || Props[block].IsPortal || Props[block].IsMessageBlock) return false;
            block = Convert(block);
            return block >= water && block <= lavastill;
        }

        public static bool LightPass(byte block) {
            switch (Convert(block)) {
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
            }
            return false;
        }

        public static bool NeedRestart(byte block)
        {
            switch (block)
            {
                case train:

                case snake:
                case snaketail:

                case air_flood:
                case air_flood_down:
                case air_flood_up:
                case air_flood_layer:
                    
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
        
        public static AABB BlockAABB(ExtBlock block, Level lvl) {
            BlockDefinition def = lvl.GetBlockDef(block);
            if (def != null) {
                return new AABB(def.MinX * 2, def.MinZ * 2, def.MinY * 2,
                                def.MaxX * 2, def.MaxZ * 2, def.MaxY * 2);
            }
            
            if (block.BlockID == Block.custom_block)
                return new AABB(0, 0, 0, 32, 32, 32);
            
            byte core = Block.Convert(block.BlockID);
            return new AABB(0, 0, 0, 32, DefaultSet.Height(core) * 2, 32);
        }
        
        public static void SetBlocks() {
            SetCoreProperties();
            BlockProps.Load("core", Block.Props, false);
            BlockPerms.Load();
            
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                lvl.UpdateBlockHandlers();
            }
        }
        
        [Obsolete("Use BlockPerms.CanModify()")]
        public static bool canPlace(Player p, byte type) { return BlockPerms.UsableBy(p, type); }
        
        [Obsolete("Use BlockPerms.CanModify()")]
        public static bool canPlace(LevelPermission perm, byte type) { return BlockPerms.UsableBy(perm, type); }
    }
}
