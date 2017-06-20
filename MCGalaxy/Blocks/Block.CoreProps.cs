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
        public static Dictionary<string, byte> Aliases = new Dictionary<string, byte>();
        
        static void SetCoreProperties() {
            for (int i = 0; i < Block.Count; i++)
                Props[i] = BlockProps.MakeDefault();
            for (int i = 0; i < Block.Count; i++) {                
                if ((i >= op_glass && i <= op_lava) || i == Invalid || i == rocketstart || i == blackrock)
                    Props[i].OPBlock = true;
                
                if ((i >= tdoor && i <= tdoor8) || (i >= tdoor9 && i <= tdoor13))
                    Props[i].IsTDoor = true;
                
                if (i >= MsgWhite && i <= MsgLava)
                    Props[i].IsMessageBlock = true;
                
                if (i == blue_portal || i == orange_portal || (i >= air_portal && i <= lava_portal))
                    Props[i].IsPortal = true;
                
                // ODoor blocks
                if (i >= odoor1 && i <= odoor7)
                    Props[i].ODoorId = (byte)(odoor1_air + (i - odoor1));
                if (i >= odoor8 && i <= odoor12)
                    Props[i].ODoorId = (byte)(odoor8_air + (i - odoor8));
                if (i >= odoor1_air && i <= odoor7_air)
                    Props[i].ODoorId = (byte)(odoor1 + (i - odoor1_air));
                if (i >= odoor8_air && i <= odoor12_air)
                    Props[i].ODoorId = (byte)(odoor8 + (i - odoor8_air));
                
                if ((i >= red && i <= white) || (i >= lightpink && i <= turquoise))
                    Props[i].LavaKills = true;
                if (i == air || i == shrub || (i >= yellowflower && i <= redmushroom)) {
                    Props[i].LavaKills = true;
                    Props[i].WaterKills = true;
                }
                
                // Door blocks
                if (i >= door_obsidian && i <= door_stair)
                    Props[i].IsDoor = true;
                if (i >= door_iron && i <= door_book)
                    Props[i].IsDoor = true;
                if (i >= door_orange && i <= door_white)
                    Props[i].IsDoor = true;
            }
            
            // Other door blocks, since they aren't in a consistent order
            Props[door_tree].IsDoor = true;
            Props[door_red].IsDoor = true;
            Props[door_cobblestone].IsDoor = true;
            Props[door_gold].IsDoor = true;
            Props[air_door].IsDoor = true;
            Props[air_switch].IsDoor = true;
            Props[water_door].IsDoor = true;
            Props[lava_door].IsDoor = true;
            
            // Block specific properties
            Props[wood].LavaKills = true; Props[trunk].LavaKills = true;
            Props[sponge].LavaKills = true; Props[bookcase].LavaKills = true;
            Props[leaf].LavaKills = true; Props[crate].LavaKills = true;
            Props[red].IsRails = true; Props[op_air].IsRails = true;
            
            // Block specific physics properties
            Props[Block.birdblack].AnimalAI = AnimalAI.Fly;
            Props[Block.birdwhite].AnimalAI = AnimalAI.Fly;
            Props[Block.birdlava].AnimalAI = AnimalAI.Fly;
            Props[Block.birdwater].AnimalAI = AnimalAI.Fly;
            
            Props[Block.birdred].AnimalAI = AnimalAI.KillerAir;
            Props[Block.birdblue].AnimalAI = AnimalAI.KillerAir;
            Props[Block.birdkill].AnimalAI = AnimalAI.KillerAir;

            Props[Block.fishbetta].AnimalAI = AnimalAI.KillerWater;
            Props[Block.fishshark].AnimalAI = AnimalAI.KillerWater;
            Props[Block.fishlavashark].AnimalAI = AnimalAI.KillerLava;
            
            Props[Block.fishgold].AnimalAI = AnimalAI.FleeWater;
            Props[Block.fishsalmon].AnimalAI = AnimalAI.FleeWater;
            Props[Block.fishsponge].AnimalAI = AnimalAI.FleeWater;
            
            SetDefaultNames();
            SetDefaultDeaths();
        }
        
        internal static void SetDefaultNames() {
            string[] names = { "air", "stone", "grass", "dirt", "cobblestone", "wood", "sapling",
                "bedrock", "active_water", "water", "active_lava", "lava", "sand", "gravel",
                "gold_ore", "iron_ore", "coal", "log", "leaves", "sponge", "glass", "red",
                "orange", "yellow", "lime", "green", "teal", "aqua", "cyan",
                "blue", "indigo", "violet", "magenta", "pink", "black", "gray", "white",
                "dandelion", "rose", "brown_shroom", "red_shroom", "gold", "iron",
                "doubleslab", "slab", "brick", "tnt", "bookshelf", "mossyrocks",
                "obsidian", "cobblestoneslab", "rope", "sandstone", "snow", "fire", "lightpink",
                "forestgreen", "brown", "deepblue", "turquoise", "ice", "ceramictile", "magmablock",
                "pillar", "crate", "stonebrick", null, null,
                null, null, "flagbase", null, null,
                "fast_hot_lava", "c4", "c4_det", null, null, null, null,
                "door_cobblestone", null, null, "door_red", null,
                "door_orange", "door_yellow", "door_lightgreen", null, "door_aquagreen",
                "door_cyan", "door_lightblue", "door_purple", "door_lightpurple", "door_pink",
                "door_darkpink", "door_darkgrey", "door_lightgrey", "door_white", null,
                "op_glass", "opsidian", "op_brick", "op_stone", "op_cobblestone", "op_air",
                "op_water", "op_lava", null, "lava_sponge", "wood_float", "door",
                "lava_fast", "door_obsidian", "door_glass", "door_stone", "door_leaves", "door_sand",
                "door_wood", "door_green", "door_tnt", "door_stair", "tdoor", "tdoor_obsidian",
                "tdoor_glass", "tdoor_stone", "tdoor_leaves", "tdoor_sand", "tdoor_wood",
                "tdoor_green", "white_message", "black_message", "air_message", "water_message",
                "lava_message", "tdoor_tnt", "tdoor_stair", "tdoor_air", "tdoor_water", "tdoor_lava",
                "waterfall", "lavafall", null, "water_faucet", "lava_faucet", "finite_water",
                "finite_lava", "finite_faucet", "odoor", "odoor_obsidian", "odoor_glass",
                "odoor_stone", "odoor_leaves", "odoor_sand", "odoor_wood", "odoor_green",
                "odoor_tnt", "odoor_stair", "odoor_lava", "odoor_water", "air_portal", "water_portal",
                "lava_portal", "custom_block", "air_door", "air_switch", "door_water", "door_lava",
                "odoor_wood_air", "odoor_obsidian_air", "odoor_glass_air", "odoor_stone_air",
                "odoor_leaves_air", "odoor_sand_air", "odoor_wood_air", "blue_portal", "orange_portal",
                "odoor_red", "odoor_tnt_air", "odoor_stair_air", "odoor_lava_air", "odoor_water_air",
                "small_tnt", "big_tnt", "tnt_explosion", "lava_fire", "nuke_tnt", "rocketstart",
                "rockethead", "firework", "hot_lava", "cold_water", "nerve_gas", "active_cold_water",
                "active_hot_lava", "magma", "geyser", "checkpoint", null, null, "air_flood",
                "door_air", "air_flood_layer", "air_flood_down", "air_flood_up", null,
                null, null, null, null, null, "door8_air",
                "door9_air", null, null, null, null, null,
                null, null, "door_iron", "door_dirt", "door_grass", "door_blue", "door_book",
                null, null, null, null, null,
                "train", "creeper", "zombie", "zombie_head", null, "dove", "pidgeon", "duck",
                "phoenix", "red_robin", "blue_bird", null, "killer_phoenix", null, null,
                "goldfish", "sea_sponge", "shark", "salmon", "betta_fish", "lava_shark", "snake",
                "snake_tail", "door_gold", null, null };
            
            Aliases.Clear();
            SetDefaultAliases();
            for (int i = 0; i < names.Length; i++) {
                string name = names[i];
                if (name == null) name = "unknown";
                if (i > 0 && i < Block.CpeCount) {
                    BlockDefinition def = BlockDefinition.GlobalDefs[i];
                    if (def != null) name = def.Name;
                }               
                Props[i].Name = name;
                
                name = name.ToLower();
                if (name != "unknown")
                    Aliases[name] = (byte)i;
                if (name.IndexOf('_') >= 0)
                    Aliases[name.Replace("_", "")] = (byte)i;
            }
        }
        
        static void SetDefaultAliases() {
            // Add old MCGalaxy names
            Aliases["purple"] = purple; Aliases["blueviolet"] = blue;
            Aliases["adminium"] = blackrock; Aliases["bookcase"] = bookcase;
            Aliases["plant"] = shrub; Aliases["mossy_cobblestone"] = stonevine;
            Aliases["springgreen"] = aquagreen; Aliases["greenyellow"] = lightgreen;
            Aliases["red_flower"] = redflower; Aliases["yellow_flower"] = yellowflower;
            Aliases["stair"] = staircasestep; Aliases["double_stair"] = staircasefull;
            // Add other aliases
            Aliases["planks"] = wood; Aliases["tree"] = trunk;
            Aliases["stairs"] = staircasestep; Aliases["slab"] = staircasestep;
            Aliases["doubleslab"] = staircasefull; Aliases["slabfull"] = staircasefull;
            Aliases["solid"] = blackrock; Aliases["admintite"] = blackrock;
            Aliases["blackrock"] = blackrock; Aliases["activewater"] = water;
            Aliases["activelava"] = lava; Aliases["fhl"] = fastdeathlava;
            Aliases["water_door"] = water_door; Aliases["lava_door"] = lava_door;
            Aliases["acw"] = activedeathwater; Aliases["ahl"] = activedeathlava;
            
            Aliases["door_tree"] = door_tree; Aliases["door2"] = door_obsidian;
            Aliases["door3"] = door_glass; Aliases["door4"] = door_stone;
            Aliases["door5"] = door_leaves; Aliases["door6"] = door_sand;
            Aliases["door7"] = door_wood; Aliases["door8"] = door_green;
            Aliases["door9"] = door_tnt; Aliases["door10"] = door_stair;
            Aliases["door11"] = door_iron; Aliases["door12"] = door_dirt;
            Aliases["door13"] = door_grass; Aliases["door14"] = door_blue;
            Aliases["door15"] = door_book; Aliases["door16"] = door_gold;
            Aliases["door17"] = door_cobblestone; Aliases["door18"] = door_red;
            
            Aliases["tdoor_tree"] = tdoor; Aliases["tdoor2"] = tdoor2;
            Aliases["tdoor3"] = tdoor3; Aliases["tdoor4"] = tdoor4;
            Aliases["tdoor5"] = tdoor5; Aliases["tdoor6"] = tdoor6;
            Aliases["tdoor7"] = tdoor7; Aliases["tdoor8"] = tdoor8;
            Aliases["tdoor9"] = tdoor9; Aliases["tdoor10"] = tdoor10;
            Aliases["tair_switch"] = tdoor11; Aliases["tdoor11"] = tdoor11;
            Aliases["tdoor12"] = tdoor12; Aliases["tdoor13"] = tdoor13;
            
            Aliases["odoor_tree"] = odoor1; Aliases["odoor2"] = odoor2;
            Aliases["odoor3"] = odoor3; Aliases["odoor4"] = odoor4;
            Aliases["odoor5"] = odoor5; Aliases["odoor6"] = odoor6;
            Aliases["odoor7"] = odoor7; Aliases["odoor8"] = odoor8;
            Aliases["odoor9"] = odoor9; Aliases["odoor10"] = odoor10;
            Aliases["odoor11"] = odoor11; Aliases["odoor12"] = odoor12;
            
            Aliases["steps"] = staircasestep; Aliases["double_steps"] = staircasefull;
            Aliases["step"] = staircasestep; Aliases["double_step"] = staircasefull;
            Aliases["grey"] = lightgrey; Aliases["door_darkgray"] = door_darkgrey;
            Aliases["door_lightgray"] = door_lightgrey;
        }
        
        static void SetDefaultDeaths() {
            SetDeath(Block.tntexplosion, "@p %S&cblew into pieces.");
            SetDeath(Block.deathair, "@p %Swalked into &cnerve gas and suffocated.");
            SetDeath(Block.deathwater, "@p %Sstepped in &dcold water and froze.");
            SetDeath(Block.activedeathwater, Props[Block.deathwater].DeathMessage);
            SetDeath(Block.deathlava, "@p %Sstood in &cmagma and melted.");
            SetDeath(Block.activedeathlava, Props[Block.deathlava].DeathMessage);
            SetDeath(Block.fastdeathlava, Props[Block.deathlava].DeathMessage);
            
            SetDeath(Block.magma, "@p %Swas hit by &cflowing magma and melted.");
            SetDeath(Block.geyser, "@p %Swas hit by &cboiling water and melted.");
            SetDeath(Block.birdkill, "@p %Swas hit by a &cphoenix and burnt.");
            SetDeath(Block.train, "@p %Swas hit by a &ctrain.");
            SetDeath(Block.fishshark, "@p %Swas eaten by a &cshark.");
            SetDeath(Block.lava_fire, "@p %Sburnt to a &ccrisp.");
            SetDeath(Block.rockethead, "@p %Swas &cin a fiery explosion.");
            SetDeath(Block.zombiebody, "@p %Sdied due to lack of &5brain.");
            SetDeath(Block.creeper, "@p %Swas killed &cb-SSSSSSSSSSSSSS");
            SetDeath(Block.fishlavashark, "@p %Swas eaten by a ... LAVA SHARK?!");
            SetDeath(Block.snake, "@p %Swas bit by a deadly snake.");
            
            SetDeath(Block.air, "@p %Shit the floor &chard.", false);
            SetDeath(Block.water, "@p %S&cdrowned.", false);
            SetDeath(Block.Invalid, "@p %Swas &cterminated.", false);
        }
        
        static void SetDeath(byte block, string message, bool collideKill = true) {
            Props[block].DeathMessage = message;
            Props[block].KillerBlock = collideKill;
        }
    }
}
