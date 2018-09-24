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
using System.IO;
using MCGalaxy.Blocks;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public static partial class Block {

        public static bool Walkthrough(BlockID block) {
            return block == Air || block == Sapling || block == Block.Snow
                || block == Fire || block == Rope
                || (block >= Water && block <= StillLava)
                || (block >= Dandelion && block <= RedMushroom);
        }

        public static bool AllowBreak(BlockID block) {
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

        public static bool LightPass(BlockID block) {
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

        public static bool NeedRestart(BlockID block) {
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
        
        public static AABB BlockAABB(BlockID block, Level lvl) {
            BlockDefinition def = lvl.GetBlockDef(block);
            if (def != null) {
                return new AABB(def.MinX * 2, def.MinZ * 2, def.MinY * 2,
                                def.MaxX * 2, def.MaxZ * 2, def.MaxY * 2);
            }
            
            if (block >= Block.Extended) return new AABB(0, 0, 0, 32, 32, 32);
            BlockID core = Convert(block);
            return new AABB(0, 0, 0, 32, DefaultSet.Height(core) * 2, 32);
        }        
        
        public static void SetBlocks() {
            BlockProps[] props = Props;
            for (int b = 0; b < props.Length; b++) {
                props[b] = MakeDefaultProps((BlockID)b);
            }
            
            SetDefaultNames();
            string propsPath = BlockProps.PropsPath("default");
                
            // backwards compatibility with older versions
            if (!File.Exists(propsPath)) {
                BlockProps.Load("core",    Props, 1, false);
                BlockProps.Load("global",  Props, 1, true);
            } else {
                BlockProps.Load("default", Props, 1, false);
            }
            
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
        
        public static BlockID FromRaw(BlockID raw) {
            return raw < CpeCount ? raw : (BlockID)(raw + Block.Extended);
        }
        
        public static BlockID ToRaw(BlockID raw) {
            return raw < CpeCount ? raw : (BlockID)(raw - Block.Extended);
        }
        
        public static BlockID FromRaw(byte raw, bool extended) {
            return (BlockID)(raw | (extended ? Block.Extended : Block.Air));
        }
        
        public static BlockID MapOldRaw(BlockID raw) {
            // old raw form was: 0 - 65 core block ids, 66 - 255 custom block ids
            // 256+ remain unchanged
            return IsPhysicsType(raw) ? ((BlockID)(raw + Block.Extended)) : raw;
        }
        
        public static bool IsPhysicsType(BlockID block) {
            return block >= Block.CpeCount && block < Block.Extended;
        }
        
        public static bool VisuallyEquals(BlockID a, BlockID b) {
            return Block.Convert(a) == Block.Convert(b);
        }
    }
}
