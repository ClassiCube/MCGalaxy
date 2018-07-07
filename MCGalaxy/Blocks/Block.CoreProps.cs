/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Blocks;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public static partial class Block {
        
        public static BlockProps[] Props = new BlockProps[Block.ExtendedCount];
        public static readonly object PropsLock = new object();
        public static Dictionary<string, byte> Aliases = new Dictionary<string, byte>();
        
        internal static void ChangeGlobalProps(BlockID block, BlockProps props) {
            Level[] loaded = LevelInfo.Loaded.Items;
            Block.Props[block] = props;
            
            foreach (Level lvl in loaded) {
                if (lvl.HasCustomProps(block)) continue;
                lvl.Props[block] = props;
                lvl.UpdateBlockHandler(block);
            }
        }
        
        internal static void MakeDefaultProps(BlockProps[] props) {
            for (int b = 0; b < props.Length; b++) {
                props[b] = BlockProps.MakeDefault();
                if ((b >= Op_Glass && b <= Op_Lava) || b == Invalid || b == RocketStart || b == Bedrock) {
                    props[b].OPBlock = true;
                }
                
                if ((b >= tDoor_Log && b <= tDoor_Green) || (b >= tDoor_TNT && b <= tDoor_Lava)) {
                    props[b].IsTDoor = true;
                }                
                if (b >= MB_White && b <= MB_Lava) {
                    props[b].IsMessageBlock = true;
                }             
                if (b == Portal_Blue || b == Portal_Orange || (b >= Portal_Air && b <= Portal_Lava)) {
                    props[b].IsPortal = true;
                }
                
                // oDoor blocks
                if (b >= oDoor_Log && b <= oDoor_Wood) {
                    props[b].oDoorBlock = (ushort)(oDoor_Log_air + (b - oDoor_Log));
                }
                if (b >= oDoor_Green && b <= oDoor_Water) {
                    props[b].oDoorBlock = (ushort)(oDoor_Green_air + (b - oDoor_Green));
                }
                if (b >= oDoor_Log_air && b <= oDoor_Wood_air) {
                    props[b].oDoorBlock = (ushort)(oDoor_Log + (b - oDoor_Log_air));
                }
                if (b >= oDoor_Green_air && b <= oDoor_Water_air) {
                    props[b].oDoorBlock = (ushort)(oDoor_Green + (b - oDoor_Green_air));
                }
                
                if ((b >= Red && b <= White) || (b >= LightPink && b <= turquoise)) {
                    props[b].LavaKills = true;
                }
                if (b == Air || b == Sapling || (b >= Dandelion && b <= RedMushroom)) {
                    props[b].LavaKills = true;
                    props[b].WaterKills = true;
                }
                
                // Door blocks
                if (b >= Door_Obsidian && b <= Door_Slab) props[b].IsDoor = true;
                if (b >= Door_Iron && b <= Door_Bookshelf) props[b].IsDoor = true;
                if (b >= Door_Orange && b <= Door_White) props[b].IsDoor = true;
            }
            
            // Other door blocks, since they aren't in a consistent order
            props[Door_Log].IsDoor = true;
            props[Door_Red].IsDoor = true;
            props[Door_Cobblestone].IsDoor = true;
            props[Door_Gold].IsDoor = true;
            props[Door_Air].IsDoor = true;
            props[Door_AirActivatable].IsDoor = true;
            props[Door_Water].IsDoor = true;
            props[Door_Lava].IsDoor = true;
            
            // Block specific properties
            props[Wood].LavaKills = true; props[Log].LavaKills = true;
            props[Sponge].LavaKills = true; props[Bookshelf].LavaKills = true;
            props[Leaves].LavaKills = true; props[Crate].LavaKills = true;
            props[Red].IsRails = true; props[Op_Air].IsRails = true;
            props[Slab].StackBlock = DoubleSlab;
            props[CobblestoneSlab].StackBlock = Cobblestone;
            props[Water].Drownable = true; props[StillWater].Drownable = true;
            props[Lava].Drownable = true; props[StillLava].Drownable = true;
            props[Dirt].GrassBlock = Block.Grass; props[Grass].DirtBlock = Block.Dirt;
            
            // Block specific physics properties
            props[Block.Bird_Black].AnimalAI = AnimalAI.Fly;
            props[Block.Bird_White].AnimalAI = AnimalAI.Fly;
            props[Block.Bird_Lava].AnimalAI = AnimalAI.Fly;
            props[Block.Bird_Water].AnimalAI = AnimalAI.Fly;
            
            props[Block.Bird_Red].AnimalAI = AnimalAI.KillerAir;
            props[Block.Bird_Blue].AnimalAI = AnimalAI.KillerAir;
            props[Block.Bird_Killer].AnimalAI = AnimalAI.KillerAir;

            props[Block.Fish_Betta].AnimalAI = AnimalAI.KillerWater;
            props[Block.Fish_Shark].AnimalAI = AnimalAI.KillerWater;
            props[Block.Fish_LavaShark].AnimalAI = AnimalAI.KillerLava;
            
            props[Block.Fish_Gold].AnimalAI = AnimalAI.FleeWater;
            props[Block.Fish_Salmon].AnimalAI = AnimalAI.FleeWater;
            props[Block.Fish_Sponge].AnimalAI = AnimalAI.FleeWater;
            
            // Block specific death messages and killer states
            const string drowned = "@p %S&cdrowned.";
            const string coldDied = "@p %Sstepped in &dcold water and froze.";
            const string lavaDied = "@p %Sstood in &cmagma and melted.";
            
            SetKiller(props, Block.TNT_Explosion, "@p %S&cblew into pieces.");
            SetKiller(props, Block.Deadly_Air, "@p %Swalked into &cnerve gas and suffocated.");     
            SetKiller(props, Block.Deadly_Water, coldDied);
            SetKiller(props, Block.Deadly_ActiveWater, coldDied);           
            SetKiller(props, Block.Deadly_Lava, lavaDied);
            SetKiller(props, Block.Deadly_ActiveLava, lavaDied);
            SetKiller(props, Block.Deadly_FastLava, lavaDied);
                                  
            SetKiller(props, Block.Magma, "@p %Swas hit by &cflowing magma and melted.");
            SetKiller(props, Block.Geyser, "@p %Swas hit by &cboiling water and melted.");
            SetKiller(props, Block.Bird_Killer, "@p %Swas hit by a &cphoenix and burnt.");            
            SetKiller(props, Block.Train, "@p %Swas hit by a &ctrain.");
            SetKiller(props, Block.Fish_Shark, "@p %Swas eaten by a &cshark.");
            SetKiller(props, Block.LavaFire, "@p %Sburnt to a &ccrisp.");           
            SetKiller(props, Block.RocketHead, "@p %Swas &cin a fiery explosion.");
            SetKiller(props, Block.ZombieBody, "@p %Sdied due to lack of &5brain.");
            SetKiller(props, Block.Creeper, "@p %Swas killed &cb-SSSSSSSSSSSSSS");           
            SetKiller(props, Block.Fish_LavaShark, "@p %Swas eaten by a ... LAVA SHARK?!");
            SetKiller(props, Block.Snake, "@p %Swas bit by a deadly snake.");
            
            props[Block.Air].DeathMessage = "@p %Shit the floor &chard.";
            props[Block.Water].DeathMessage = drowned;
            props[Block.StillWater].DeathMessage = drowned;
            props[Block.Lava].DeathMessage = drowned;
            props[Block.StillLava].DeathMessage = drowned;
        }
        
        static void SetKiller(BlockProps[] props, BlockID block, string msg) {
            props[block].DeathMessage = msg;
            props[block].KillerBlock = true;
        }
        
        internal static void SetDefaultNames() {
            string[] names = new string[] { "Air", "Stone", "Grass", "Dirt", "Cobblestone", 
                "Wood", "Sapling", "Bedrock", "Active_Water", "Water", "Active_Lava", "Lava", 
                "Sand", "Gravel", "Gold_Ore", "Iron_Ore", "Coal", "Log", "Leaves", "Sponge", 
                "Glass", "Red", "Orange", "Yellow", "Lime", "Green", "Teal", "Aqua", "Cyan",
                "Blue", "Indigo", "Violet", "Magenta", "Pink", "Black", "Gray", "White",
                "Dandelion", "Rose", "Brown_Shroom", "Red_Shroom", "Gold", "Iron",
                "DoubleSlab", "Slab", "Brick", "TNT", "BookShelf", "MossyRocks",
                "Obsidian", "CobblestoneSlab", "Rope", "SandStone", "Snow", "Fire", "LightPink",
                "ForestGreen", "Brown", "DeepBlue", "Turquoise", "Ice", "CeramicTile", "MagmaBlock",
                "Pillar", "Crate", "StoneBrick", null, null,
                null, null, "FlagBase", null, null,
                "Fast_Hot_Lava", "C4", "C4_Det", null, null, null, null,
                "Door_Cobblestone", null, null, "Door_Red", null,
                "Door_Orange", "Door_Yellow", "Door_LightGreen", null, "Door_AquaGreen",
                "Door_Cyan", "Door_LightBlue", "Door_Purple", "Door_LightPurple", "Door_Pink",
                "Door_DarkPink", "Door_DarkGrey", "Door_LightGrey", "Door_White", null,
                "Op_Glass", "Opsidian", "Op_Brick", "Op_Stone", "Op_Cobblestone", "Op_Air",
                "Op_Water", "Op_Lava", null, "Lava_Sponge", "Wood_Float", "Door",
                "Lava_Fast", "Door_Obsidian", "Door_Glass", "Door_Stone", "Door_Leaves", "Door_Sand",
                "Door_Wood", "Door_Green", "Door_TNT", "Door_Stair", "tDoor", "tDoor_Obsidian",
                "tDoor_Glass", "tDoor_Stone", "tDoor_Leaves", "tDoor_Sand", "tDoor_Wood",
                "tDoor_Green", "White_Message", "Black_Message", "Air_Message", "Water_Message",
                "Lava_Message", "tDoor_TNT", "tDoor_Stair", "tDoor_Air", "tDoor_Water", "tDoor_lava",
                "Waterfall", "Lavafall", null, "Water_Faucet", "Lava_Faucet", "Finite_Water",
                "Finite_Lava", "Finite_Faucet", "oDoor", "oDoor_Obsidian", "oDoor_Glass",
                "oDoor_Stone", "oDoor_Leaves", "oDoor_Sand", "oDoor_Wood", "oDoor_Green",
                "oDoor_TNT", "oDoor_Stair", "oDoor_Lava", "oDoor_Water", "Air_Portal", "Water_Portal",
                "Lava_Portal", "Custom_Block", "Air_Door", "Air_Switch", "Door_Water", "Door_Lava",
                "oDoor_Air", "oDoor_Obsidian_Air", "oDoor_Glass_Air", "oDoor_Stone_Air",
                "oDoor_Leaves_Air", "oDoor_Sand_Air", "oDoor_Wood_Air", "Blue_Portal", "Orange_Portal",
                "oDoor_Red", "oDoor_TNT_Air", "oDoor_Stair_Air", "oDoor_Lava_Air", "oDoor_Water_Air",
                "Small_TNT", "Big_TNT", "TNT_Explosion", "Lava_Fire", "Nuke_TNT", "RocketStart",
                "RocketHead", "Firework", "Hot_Lava", "Cold_Water", "Nerve_Gas", "Active_Cold_Water",
                "Active_Hot_Lava", "Magma", "Geyser", "Checkpoint", null, null, "Air_Flood",
                "Door_Air", "Air_Flood_Layer", "Air_Flood_Down", "Air_Flood_Up", null,
                null, null, null, null, null, "Door8_Air",
                "Door9_Air", null, null, null, null, null,
                null, null, "Door_Iron", "Door_Dirt", "Door_Grass", "Door_Blue", "Door_Book",
                null, null, null, null, null,
                "Train", "Creeper", "Zombie", "Zombie_Head", null, "Dove", "Pidgeon", "Duck",
                "Phoenix", "Red_Robin", "Blue_Bird", null, "Killer_Phoenix", null, null,
                "GoldFish", "Sea_Sponge", "Shark", "Salmon", "Betta_Fish", "Lava_Shark", "Snake",
                "Snake_Tail", "Door_Gold", null, null };
            
            Aliases.Clear();
            SetDefaultAliases();
            for (int i = 0; i < names.Length; i++) {
                string name = names[i];
                if (name == null) name = "unknown";
                if (i > 0 && i < Block.CpeCount) {
                    BlockDefinition def = BlockDefinition.GlobalDefs[i];
                    if (def != null) name = def.Name;
                }               
                coreNames[i] = name;
                
                name = name.ToLower();
                if (name != "unknown")
                    Aliases[name] = (byte)i;
                if (name.IndexOf('_') >= 0)
                    Aliases[name.Replace("_", "")] = (byte)i;
            }
        }
        
        static void SetDefaultAliases() {
            Dictionary<string, byte> aliases = Aliases;
            // Add old MCGalaxy names
            aliases["purple"] = Indigo; aliases["blueviolet"] = Blue;
            aliases["adminium"] = Bedrock; aliases["bookcase"] = Bookshelf;
            aliases["plant"] = Sapling; aliases["mossy_cobblestone"] = MossyRocks;
            aliases["springgreen"] = Teal; aliases["greenyellow"] = Lime;
            aliases["red_flower"] = Rose; aliases["yellow_flower"] = Dandelion;
            aliases["stair"] = Slab; aliases["double_stair"] = DoubleSlab;
            // Add other aliases
            aliases["planks"] = Wood; aliases["tree"] = Log;
            aliases["stairs"] = Slab; aliases["slab"] = Slab;
            aliases["doubleslab"] = DoubleSlab; aliases["slabfull"] = DoubleSlab;
            aliases["solid"] = Bedrock; aliases["admintite"] = Bedrock;
            aliases["blackrock"] = Bedrock; aliases["activewater"] = Water;
            aliases["activelava"] = Lava; aliases["fhl"] = Deadly_FastLava;
            aliases["water_door"] = Door_Water; aliases["lava_door"] = Door_Lava;
            aliases["acw"] = Deadly_ActiveWater; aliases["ahl"] = Deadly_ActiveLava;
            
            aliases["door_tree"] = Door_Log; aliases["door2"] = Door_Obsidian;
            aliases["door3"] = Door_Glass; aliases["door4"] = Door_Stone;
            aliases["door5"] = Door_Leaves; aliases["door6"] = Door_Sand;
            aliases["door7"] = Door_Wood; aliases["door8"] = Door_Green;
            aliases["door9"] = Door_TNT; aliases["door10"] = Door_Slab;
            aliases["door11"] = Door_Iron; aliases["door12"] = Door_Dirt;
            aliases["door13"] = Door_Grass; aliases["door14"] = Door_Blue;
            aliases["door15"] = Door_Bookshelf; aliases["door16"] = Door_Gold;
            aliases["door17"] = Door_Cobblestone; aliases["door18"] = Door_Red;
            
            aliases["tdoor_tree"] = tDoor_Log; aliases["tdoor2"] = tDoor_Obsidian;
            aliases["tdoor3"] = tDoor_Glass; aliases["tdoor4"] = tDoor_Stone;
            aliases["tdoor5"] = tDoor_Leaves; aliases["tdoor6"] = tDoor_Sand;
            aliases["tdoor7"] = tDoor_Wood; aliases["tdoor8"] = tDoor_Green;
            aliases["tdoor9"] = tDoor_TNT; aliases["tdoor10"] = tDoor_Slab;
            aliases["tair_switch"] = tDoor_Air; aliases["tdoor11"] = tDoor_Air;
            aliases["tdoor12"] = tDoor_Water; aliases["tdoor13"] = tDoor_Lava;
            
            aliases["odoor_tree"] = oDoor_Log; aliases["odoor2"] = oDoor_Obsidian;
            aliases["odoor3"] = oDoor_Glass; aliases["odoor4"] = oDoor_Stone;
            aliases["odoor5"] = oDoor_Leaves; aliases["odoor6"] = oDoor_Sand;
            aliases["odoor7"] = oDoor_Wood; aliases["odoor8"] = oDoor_Green;
            aliases["odoor9"] = oDoor_TNT; aliases["odoor10"] = oDoor_Slab;
            aliases["odoor11"] = oDoor_Lava; aliases["odoor12"] = oDoor_Water;
            
            aliases["steps"] = Slab; aliases["double_steps"] = DoubleSlab;
            aliases["step"] = Slab; aliases["double_step"] = DoubleSlab;
            aliases["grey"] = Gray; aliases["door_darkgray"] = Door_Black;
            aliases["door_lightgray"] = Door_Gray;
        }
    }
}
