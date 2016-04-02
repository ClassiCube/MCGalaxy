/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;

namespace MCGalaxy
{
    public sealed partial class Block
    {
        public static Blocks[] BlockList = new Blocks[256];
        public class Blocks
        {
            public LevelPermission lowestRank;
            public List<LevelPermission> disallow = null;
            public List<LevelPermission> allow = null;
            public byte type;

            public bool IncludeInBlockProperties()
            {
                if (Block.Name(type).ToLower() == "unknown")
                    return false;
                if(type == Block.flagbase)
                    return false;
                if (type >= Block.odoor1_air && type <= Block.odoor7_air)
                    return false;
                if (type >= Block.odoor8_air && type <= Block.odoor12_air)
                    return false;
                return true;
            }
        }

        public static void SetBlocks() {
            SetupCoreHandlers();
            InitDefaults();
            // Custom permissions set by the user.
            if (File.Exists("properties/block.properties")) {
                string[] lines = File.ReadAllLines("properties/block.properties");
                if (lines.Length > 0 && lines[0] == "#Version 2") {
                    LoadVersion2(lines);
                } else {
                    LoadVersion1(lines);
                }
            }
            SaveBlocks(BlockList);
        }
        
        static void InitDefaults() {
            for (int i = 0; i < 256; i++) {
                Blocks b = new Blocks();
                b.type = (byte)i;
                switch (i)
                {
                    case Zero:
                        b.lowestRank = LevelPermission.Admin;
                        break;

                    case op_glass:
                    case opsidian:
                    case op_brick:
                    case op_stone:
                    case op_cobblestone:
                    case op_air:
                    case op_water:
                    case op_lava:
                    case blackrock:

                    case griefer_stone:

                    case air_flood:
                    case air_flood_down:
                    case air_flood_layer:
                    case air_flood_up:

                    case bigtnt:
                    case nuketnt:
                    case rocketstart:
                    case rockethead:

                    case creeper:
                    case zombiebody:
                    case zombiehead:

                    case birdred:
                    case birdkill:
                    case birdblue:

                    case fishgold:
                    case fishsponge:
                    case fishshark:
                    case fishsalmon:
                    case fishbetta:
                    case fishlavashark:

                    case snake:
                    case snaketail:
                    case flagbase:

                        b.lowestRank = LevelPermission.Operator;
                        break;

                    case wood_float:
                    case lava_sponge:

                    case door_tree_air:
                    case door_obsidian_air:
                    case door_glass_air:
                    case door_stone_air:
                    case door_leaves_air:
                    case door_sand_air:
                    case door_wood_air:
                    case door_green_air:
                    case door_tnt_air:
                    case door_stair_air:
                    case air_switch_air:
                    case water_door_air:
                    case lava_door_air:
                    case air_door_air:
                    case door_iron_air:
                    case door_gold_air:
                    case door_cobblestone_air:
                    case door_grass_air:
                    case door_dirt_air:
                    case door_blue_air:
                    case door_book_air:
                    case door_red_air:
                    case door_darkpink_air:
                    case door_darkgrey_air:
                    case door_lightgrey_air:
                    case door_white_air:
                        
                    case odoor1_air:
                    case odoor2_air:
                    case odoor3_air:
                    case odoor4_air:
                    case odoor5_air:
                    case odoor6_air:
                    case odoor7_air:
                    case odoor8_air:
                    case odoor9_air:
                    case odoor10_air:
                    case odoor11_air:
                    case odoor12_air:

                    case MsgAir:
                    case MsgBlack:
                    case MsgLava:
                    case MsgWater:
                    case MsgWhite:
                    case air_portal:
                    case water_portal:
                    case lava_portal:
                    case blue_portal:
                    case orange_portal:

                    case water:
                    case lava:
                    case lava_fast:
                    case WaterDown:
                    case LavaDown:
                    case WaterFaucet:
                    case LavaFaucet:
                    case finiteWater:
                    case finiteLava:
                    case finiteFaucet:
                    case magma:
                    case geyser:
                    case deathlava:
                    case deathwater:
                    case deathair:
                    case activedeathwater:
                    case activedeathlava:
                    case fastdeathlava:
                    case lava_fire:

                    case c4:
                    case c4det:
                    case smalltnt:
                    case tntexplosion:
                    case firework:
                    case checkpoint:
                    case train:

                    case birdwhite:
                    case birdblack:
                    case birdwater:
                    case birdlava:
                        b.lowestRank = LevelPermission.AdvBuilder;
                        break;

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
                    case air_door:
                    case air_switch:
                    case water_door:
                    case lava_door:
                    case door_iron:
                    case door_gold:
                    case door_grass:
                    case door_dirt:
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

                    case odoor1:
                    case odoor2:
                    case odoor3:
                    case odoor4:
                    case odoor5:
                    case odoor6:
                    case odoor7:
                    case odoor8:
                    case odoor9:
                    case odoor10:
                    case odoor11:
                    case odoor12:
                        b.lowestRank = LevelPermission.Builder;
                        break;

                    default:
                        b.lowestRank = LevelPermission.Banned;
                        break;
                }
                BlockList[i] = b;
            }
        }
        
        static void LoadVersion2(string[] lines) {
            string[] colon = new string[] { " : " };
            foreach (string line in lines) {
                if (line == "" || line[0] == '#') continue;
                //Name : Lowest : Disallow : Allow
                string[] block = line.Split(colon, StringSplitOptions.None);
                Blocks newBlock = new Blocks();

                if (Block.Byte(block[0]) == Block.Zero) continue;
                newBlock.type = Block.Byte(block[0]);

                string[] disallow = null;
                if (block[2] != "") disallow = block[2].Split(',');
                string[] allow = null;
                if (block[3] != "") allow = block[3].Split(',');

                try {
                    newBlock.lowestRank = (LevelPermission)int.Parse(block[1]);
                    if (disallow != null) {
                    	newBlock.disallow = new List<LevelPermission>();
                    	foreach (string s in disallow) { newBlock.disallow.Add((LevelPermission)int.Parse(s)); }
                    }                    
                    if (allow != null) {
                    	newBlock.allow = new List<LevelPermission>();
                        foreach (string s in allow) { newBlock.allow.Add((LevelPermission)int.Parse(s)); }
                    }
                } catch {
                    Server.s.Log("Hit an error on the block " + line);
                    continue;
                }
                BlockList[newBlock.type] = newBlock;
            }
        }
        
        static void LoadVersion1(string[] lines) {
            foreach (string s in lines) {
                if (s[0] == '#') continue;
                
                try {
                    byte type = Block.Byte(s.Split(' ')[0]);
                    LevelPermission lowestRank = Level.PermissionFromName(s.Split(' ')[2]);
                    if (lowestRank != LevelPermission.Null)
                        BlockList[type].lowestRank = lowestRank;
                    else
                        throw new Exception();
                }
                catch { Server.s.Log("Could not find the rank given on " + s + ". Using default"); }
            }
        }
        
        public static void SaveBlocks(IEnumerable<Blocks> givenList) {
            try {
                using (StreamWriter w = File.CreateText("properties/block.properties")) {
                    w.WriteLine("#Version 2");
                    w.WriteLine("#   This file dictates what levels may use what blocks");
                    w.WriteLine("#   If someone has royally screwed up the ranks, just delete this file and let the server restart");
                    w.WriteLine("#   Allowed ranks: " + Group.concatList(false, false, true));
                    w.WriteLine("#   Disallow and allow can be left empty, just make sure there's 2 spaces between the colons");
                    w.WriteLine("#   This works entirely on permission values, not names. Do not enter a rank name. Use it's permission value");
                    w.WriteLine("#   BlockName : LowestRank : Disallow : Allow");
                    w.WriteLine("#   lava : 60 : 80,67 : 40,41,55");
                    w.WriteLine("");

                    foreach (Blocks bs in givenList) {
                        if (bs.IncludeInBlockProperties()) {
                            string line = Block.Name(bs.type) + " : " + (int)bs.lowestRank + " : " + GrpCommands.getInts(bs.disallow) + " : " + GrpCommands.getInts(bs.allow);
                            w.WriteLine(line);
                        }
                    }
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        public static bool canPlace(Player p, byte type) { 
            Blocks b = BlockList[type];
            LevelPermission perm = p.group.Permission;
            return (perm >= b.lowestRank || (b.allow != null && b.allow.Contains(perm))) 
            	&& (b.disallow == null || !b.disallow.Contains(perm));
        }
        
        public static bool canPlace(LevelPermission perm, byte type) {
            Blocks b = BlockList[type];
            return (perm >= b.lowestRank || (b.allow != null && b.allow.Contains(perm))) 
            	&& (b.disallow == null || !b.disallow.Contains(perm));
        }
    }
}
