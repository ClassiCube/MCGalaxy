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
            return block == Air || block == Sapling || block == Block.Snow
                || block == Fire || block == Rope
                || (block >= Water && block <= StillLava)
                || (block >= Dandelion && block <= RedMushroom);
        }

        public static bool AllowBreak(byte block) {
            switch (block) {
                case Portal_Blue:
                case Portal_Orange:

                case MB_White:
                case MB_Black:

                case Door_Log:
                case Door_Obsidian:
                case Door_Glass:
                case Door_Stone:
                case Door_Leaves:
                case Door_Sand:
                case Door_Wood:
                case Door_Green:
                case Door_TNT:
                case Door_Slab:
                case Door_Iron:
                case Door_Gold:
                case Door_Dirt:
                case Door_Grass:
                case Door_Blue:
                case Door_Bookshelf:
                case Door_Cobblestone:
                case Door_Red:

                case Door_Orange:
                case Door_Yellow:
                case Door_Lime:
                case Door_Teal:
                case Door_Aqua:
                case Door_Cyan:
                case Door_Indigo:
                case Door_Purple:
                case Door_Magenta:
                case Door_Pink:
                case Door_Black:
                case Door_Gray:
                case Door_White:

                case C4:
                case TNT_Small:
                case TNT_Big:
                case TNT_Nuke:
                case RocketStart:
                case Fireworks:

                case ZombieBody:
                case Creeper:
                case ZombieHead:
                    return true;
            }
            return false;
        }

        public static bool Placable(byte block) {
            return !(block == Bedrock || (block >= Water && block <= StillLava))
                && block < CpeCount;
        }

        public static bool BuildIn(byte block) {
            if (block == Op_Water || block == Op_Lava
                || Props[block].IsPortal || Props[block].IsMessageBlock) return false;
            block = Convert(block);
            return block >= Water && block <= StillLava;
        }

        public static bool LightPass(byte block) {
            switch (Convert(block)) {
                case Air:
                case Glass:
                case Leaves:
                case Rose:
                case Dandelion:
                case Mushroom:
                case RedMushroom:
                case Sapling:
                case Rope:
                    return true;
            }
            return false;
        }

        public static bool NeedRestart(byte block)
        {
            switch (block)
            {
                case Train:

                case Snake:
                case SnakeTail:

                case Air_Flood:
                case Air_FloodDown:
                case Air_FloodUp:
                case Air_FloodLayer:
                    
                case LavaFire:
                case RocketHead:
                case Fireworks:

                case Creeper:
                case ZombieBody:
                case ZombieHead:

                case Bird_Black:
                case Bird_Blue:
                case Bird_Killer:
                case Bird_Lava:
                case Bird_Red:
                case Bird_Water:
                case Bird_White:

                case Fish_Betta:
                case Fish_Gold:
                case Fish_Salmon:
                case Fish_Shark:
                case Fish_LavaShark:
                case Fish_Sponge:

                case TNT_Explosion:
                    return true;
            }
            return false;
        }
        
        public static AABB BlockAABB(ushort block, Level lvl) {
            BlockDefinition def = lvl.GetBlockDef(block);
            if (def != null) {
                return new AABB(def.MinX * 2, def.MinZ * 2, def.MinY * 2,
                                def.MaxX * 2, def.MaxZ * 2, def.MaxY * 2);
            }
            
            if (block.BlockID == Block.custom_block)
                return new AABB(0, 0, 0, 32, 32, 32);
            
            byte core = Convert(block.BlockID);
            return new AABB(0, 0, 0, 32, DefaultSet.Height(core) * 2, 32);
        }
        
        public static void SetBlocks() {
            SetCoreProperties();
            BlockProps.Load("core", Props, CorePropsLock, false);
            BlockDefinition.UpdateGlobalBlockProps();
            BlockPerms.Load();
            UpdateLoadedLevels();
        }
        
        public static void UpdateLoadedLevels() {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                lvl.UpdateBlockProps();
                lvl.UpdateBlockHandlers();
            }            
        }
        
        public static ushort FromRaw(byte raw) {
            return raw < Block.CpeCount ? raw : (byte)(Block.Extended + raw);
        }
        
        public static ushort FromRaw(byte raw, bool extended) {
            return (ushort)(raw | (extended ? Block.Extended : Block.Air));
        }
    }
}
