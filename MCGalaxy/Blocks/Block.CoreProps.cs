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

namespace MCGalaxy {
    
    public sealed partial class Block {
        
        public static BlockProps[] Props = new BlockProps[Block.Count];
        public static readonly object CorePropsLock = new object();
        public static Dictionary<string, byte> Aliases = new Dictionary<string, byte>();
        
        static void SetCoreProperties() {
            for (int i = 0; i < Block.Count; i++)
                Props[i] = BlockProps.MakeDefault();
            for (int i = 0; i < Block.Count; i++) {                
                if ((i >= Op_Glass && i <= Op_Lava) || i == Invalid || i == RocketStart || i == Bedrock) {
                    Props[i].OPBlock = true;
                }
                
                if ((i >= tDoor_Log && i <= tDoor_Green) || (i >= tDoor_TNT && i <= tDoor_Lava)) {
                    Props[i].IsTDoor = true;
                }                
                if (i >= MB_White && i <= MB_Lava) {
                    Props[i].IsMessageBlock = true;
                }             
                if (i == Portal_Blue || i == Portal_Orange || (i >= Portal_Air && i <= Portal_Lava)) {
                    Props[i].IsPortal = true;
                }
                
                // oDoor blocks
                if (i >= oDoor_Log && i <= oDoor_Wood) {
                    Props[i].oDoorBlock = (ushort)(oDoor_Log_air + (i - oDoor_Log));
                }
                if (i >= oDoor_Green && i <= oDoor_Water) {
                    Props[i].oDoorBlock = (ushort)(oDoor_Green_air + (i - oDoor_Green));
                }
                if (i >= oDoor_Log_air && i <= oDoor_Wood_air) {
                    Props[i].oDoorBlock = (ushort)(oDoor_Log + (i - oDoor_Log_air));
                }
                if (i >= oDoor_Green_air && i <= oDoor_Water_air) {
                    Props[i].oDoorBlock = (ushort)(oDoor_Green + (i - oDoor_Green_air));
                }
                
                if ((i >= Red && i <= White) || (i >= LightPink && i <= turquoise)) {
                    Props[i].LavaKills = true;
                }
                if (i == Air || i == Sapling || (i >= Dandelion && i <= RedMushroom)) {
                    Props[i].LavaKills = true;
                    Props[i].WaterKills = true;
                }
                
                // Door blocks
                if (i >= Door_Obsidian && i <= Door_Slab) Props[i].IsDoor = true;
                if (i >= Door_Iron && i <= Door_Bookshelf) Props[i].IsDoor = true;
                if (i >= Door_Orange && i <= Door_White) Props[i].IsDoor = true;
            }
            
            // Other door blocks, since they aren't in a consistent order
            Props[Door_Log].IsDoor = true;
            Props[Door_Red].IsDoor = true;
            Props[Door_Cobblestone].IsDoor = true;
            Props[Door_Gold].IsDoor = true;
            Props[Door_Air].IsDoor = true;
            Props[Door_AirActivatable].IsDoor = true;
            Props[Door_Water].IsDoor = true;
            Props[Door_Lava].IsDoor = true;
            
            // Block specific properties
            Props[Wood].LavaKills = true; Props[Log].LavaKills = true;
            Props[Sponge].LavaKills = true; Props[Bookshelf].LavaKills = true;
            Props[Leaves].LavaKills = true; Props[Crate].LavaKills = true;
            Props[Red].IsRails = true; Props[Op_Air].IsRails = true;
            Props[Slab].StackBlock = DoubleSlab;
            Props[CobblestoneSlab].StackBlock = Cobblestone;
            Props[Water].Drownable = true; Props[StillWater].Drownable = true;
            Props[Lava].Drownable = true; Props[StillLava].Drownable = true;
            Props[Dirt].GrassBlock = Block.Grass; Props[Grass].DirtBlock = Block.Dirt;
            
            // Block specific physics properties
            Props[Block.Bird_Black].AnimalAI = AnimalAI.Fly;
            Props[Block.Bird_White].AnimalAI = AnimalAI.Fly;
            Props[Block.Bird_Lava].AnimalAI = AnimalAI.Fly;
            Props[Block.Bird_Water].AnimalAI = AnimalAI.Fly;
            
            Props[Block.Bird_Red].AnimalAI = AnimalAI.KillerAir;
            Props[Block.Bird_Blue].AnimalAI = AnimalAI.KillerAir;
            Props[Block.Bird_Killer].AnimalAI = AnimalAI.KillerAir;

            Props[Block.Fish_Betta].AnimalAI = AnimalAI.KillerWater;
            Props[Block.Fish_Shark].AnimalAI = AnimalAI.KillerWater;
            Props[Block.Fish_LavaShark].AnimalAI = AnimalAI.KillerLava;
            
            Props[Block.Fish_Gold].AnimalAI = AnimalAI.FleeWater;
            Props[Block.Fish_Salmon].AnimalAI = AnimalAI.FleeWater;
            Props[Block.Fish_Sponge].AnimalAI = AnimalAI.FleeWater;
            
            SetDefaultNames();
            SetDefaultDeaths();
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
            // Add old MCGalaxy names
            Aliases["purple"] = Indigo; Aliases["blueviolet"] = Blue;
            Aliases["adminium"] = Bedrock; Aliases["bookcase"] = Bookshelf;
            Aliases["plant"] = Sapling; Aliases["mossy_cobblestone"] = MossyRocks;
            Aliases["springgreen"] = Teal; Aliases["greenyellow"] = Lime;
            Aliases["red_flower"] = Rose; Aliases["yellow_flower"] = Dandelion;
            Aliases["stair"] = Slab; Aliases["double_stair"] = DoubleSlab;
            // Add other aliases
            Aliases["planks"] = Wood; Aliases["tree"] = Log;
            Aliases["stairs"] = Slab; Aliases["slab"] = Slab;
            Aliases["doubleslab"] = DoubleSlab; Aliases["slabfull"] = DoubleSlab;
            Aliases["solid"] = Bedrock; Aliases["admintite"] = Bedrock;
            Aliases["blackrock"] = Bedrock; Aliases["activewater"] = Water;
            Aliases["activelava"] = Lava; Aliases["fhl"] = Deadly_FastLava;
            Aliases["water_door"] = Door_Water; Aliases["lava_door"] = Door_Lava;
            Aliases["acw"] = Deadly_ActiveWater; Aliases["ahl"] = Deadly_ActiveLava;
            
            Aliases["door_tree"] = Door_Log; Aliases["door2"] = Door_Obsidian;
            Aliases["door3"] = Door_Glass; Aliases["door4"] = Door_Stone;
            Aliases["door5"] = Door_Leaves; Aliases["door6"] = Door_Sand;
            Aliases["door7"] = Door_Wood; Aliases["door8"] = Door_Green;
            Aliases["door9"] = Door_TNT; Aliases["door10"] = Door_Slab;
            Aliases["door11"] = Door_Iron; Aliases["door12"] = Door_Dirt;
            Aliases["door13"] = Door_Grass; Aliases["door14"] = Door_Blue;
            Aliases["door15"] = Door_Bookshelf; Aliases["door16"] = Door_Gold;
            Aliases["door17"] = Door_Cobblestone; Aliases["door18"] = Door_Red;
            
            Aliases["tdoor_tree"] = tDoor_Log; Aliases["tdoor2"] = tDoor_Obsidian;
            Aliases["tdoor3"] = tDoor_Glass; Aliases["tdoor4"] = tDoor_Stone;
            Aliases["tdoor5"] = tDoor_Leaves; Aliases["tdoor6"] = tDoor_Sand;
            Aliases["tdoor7"] = tDoor_Wood; Aliases["tdoor8"] = tDoor_Green;
            Aliases["tdoor9"] = tDoor_TNT; Aliases["tdoor10"] = tDoor_Slab;
            Aliases["tair_switch"] = tDoor_Air; Aliases["tdoor11"] = tDoor_Air;
            Aliases["tdoor12"] = tDoor_Water; Aliases["tdoor13"] = tDoor_Lava;
            
            Aliases["odoor_tree"] = oDoor_Log; Aliases["odoor2"] = oDoor_Obsidian;
            Aliases["odoor3"] = oDoor_Glass; Aliases["odoor4"] = oDoor_Stone;
            Aliases["odoor5"] = oDoor_Leaves; Aliases["odoor6"] = oDoor_Sand;
            Aliases["odoor7"] = oDoor_Wood; Aliases["odoor8"] = oDoor_Green;
            Aliases["odoor9"] = oDoor_TNT; Aliases["odoor10"] = oDoor_Slab;
            Aliases["odoor11"] = oDoor_Lava; Aliases["odoor12"] = oDoor_Water;
            
            Aliases["steps"] = Slab; Aliases["double_steps"] = DoubleSlab;
            Aliases["step"] = Slab; Aliases["double_step"] = DoubleSlab;
            Aliases["grey"] = Gray; Aliases["door_darkgray"] = Door_Black;
            Aliases["door_lightgray"] = Door_Gray;
        }
        
        static void SetDefaultDeaths() {
            SetDeath(Block.TNT_Explosion, "@p %S&cblew into pieces.");
            SetDeath(Block.Deadly_Air, "@p %Swalked into &cnerve gas and suffocated.");
            SetDeath(Block.Deadly_Water, "@p %Sstepped in &dcold water and froze.");
            SetDeath(Block.Deadly_ActiveWater, Props[Block.Deadly_Water].DeathMessage);
            SetDeath(Block.Deadly_Lava, "@p %Sstood in &cmagma and melted.");
            SetDeath(Block.Deadly_ActiveLava, Props[Block.Deadly_Lava].DeathMessage);
            SetDeath(Block.Deadly_FastLava, Props[Block.Deadly_Lava].DeathMessage);
            
            SetDeath(Block.Magma, "@p %Swas hit by &cflowing magma and melted.");
            SetDeath(Block.Geyser, "@p %Swas hit by &cboiling water and melted.");
            SetDeath(Block.Bird_Killer, "@p %Swas hit by a &cphoenix and burnt.");
            SetDeath(Block.Train, "@p %Swas hit by a &ctrain.");
            SetDeath(Block.Fish_Shark, "@p %Swas eaten by a &cshark.");
            SetDeath(Block.LavaFire, "@p %Sburnt to a &ccrisp.");
            SetDeath(Block.RocketHead, "@p %Swas &cin a fiery explosion.");
            SetDeath(Block.ZombieBody, "@p %Sdied due to lack of &5brain.");
            SetDeath(Block.Creeper, "@p %Swas killed &cb-SSSSSSSSSSSSSS");
            SetDeath(Block.Fish_LavaShark, "@p %Swas eaten by a ... LAVA SHARK?!");
            SetDeath(Block.Snake, "@p %Swas bit by a deadly snake.");
            
            const string drowned = "@p %S&cdrowned.";
            SetDeath(Block.Air, "@p %Shit the floor &chard.", false);
            SetDeath(Block.Water, drowned, false);
            SetDeath(Block.StillWater, drowned, false);
            SetDeath(Block.Lava, drowned, false);
            SetDeath(Block.StillLava, drowned, false);
        }
        
        static void SetDeath(byte block, string message, bool collideKill = true) {
            Props[block].DeathMessage = message;
            Props[block].KillerBlock = collideKill;
        }
    }
}
