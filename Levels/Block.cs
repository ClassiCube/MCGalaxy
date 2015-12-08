/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy
{
    public sealed class Block
    {
        public const byte air = (byte)0;
        public const byte rock = (byte)1;
        public const byte grass = (byte)2;
        public const byte dirt = (byte)3;
        public const byte stone = (byte)4;
        public const byte wood = (byte)5;
        public const byte shrub = (byte)6;
        public const byte blackrock = (byte)7;// adminium
        public const byte water = (byte)8;
        public const byte waterstill = (byte)9;
        public const byte lava = (byte)10;
        public const byte lavastill = (byte)11;
        public const byte sand = (byte)12;
        public const byte gravel = (byte)13;
        public const byte goldrock = (byte)14;
        public const byte ironrock = (byte)15;
        public const byte coal = (byte)16;
        public const byte trunk = (byte)17;
        public const byte leaf = (byte)18;
        public const byte sponge = (byte)19;
        public const byte glass = (byte)20;
        public const byte red = (byte)21;
        public const byte orange = (byte)22;
        public const byte yellow = (byte)23;
        public const byte lightgreen = (byte)24;
        public const byte green = (byte)25;
        public const byte aquagreen = (byte)26;
        public const byte cyan = (byte)27;
        public const byte lightblue = (byte)28;
        public const byte blue = (byte)29;
        public const byte purple = (byte)30;
        public const byte lightpurple = (byte)31;
        public const byte pink = (byte)32;
        public const byte darkpink = (byte)33;
        public const byte darkgrey = (byte)34;
        public const byte lightgrey = (byte)35;
        public const byte white = (byte)36;
        public const byte yellowflower = (byte)37;
        public const byte redflower = (byte)38;
        public const byte mushroom = (byte)39;
        public const byte redmushroom = (byte)40;
        public const byte goldsolid = (byte)41;
        public const byte iron = (byte)42;
        public const byte staircasefull = (byte)43;
        public const byte staircasestep = (byte)44;
        public const byte brick = (byte)45;
        public const byte tnt = (byte)46;
        public const byte bookcase = (byte)47;
        public const byte stonevine = (byte)48;
        public const byte obsidian = (byte)49;
		public const byte cobblestoneslab = (byte)50;
		public const byte rope = (byte)51;
		public const byte sandstone = (byte)52;
		public const byte snow = (byte)53;
		public const byte fire = (byte)54;
		public const byte lightpink = (byte)55;
		public const byte forestgreen = (byte)56;
		public const byte brown = (byte)57;
		public const byte deepblue = (byte)58;
		public const byte turquoise = (byte)59;
		public const byte ice = (byte)60;
		public const byte ceramictile = (byte)61;
		public const byte magmablock = (byte)62;
		public const byte pillar = (byte)63;
		public const byte crate = (byte)64;
		public const byte stonebrick = (byte)65;
        public const byte Zero = 0xff;

        public const byte flagbase = (byte)70;

        //Seasons
        //public const byte fallsnow = (byte)71;
        //public const byte snow = (byte)72;

        public const byte fastdeathlava = (byte)73;

        public const byte c4 = (byte)74;
        public const byte c4det = (byte)75;

        public const byte door_cobblestone = (byte)80;
        public const byte door_cobblestone_air = (byte)81;
        public const byte door_red = (byte)83;
        public const byte door_red_air = (byte)84;

        public const byte door_orange = (byte)85;
        public const byte door_yellow = (byte)86;
        public const byte door_lightgreen = (byte)87;
        public const byte door_aquagreen = (byte)89;
        public const byte door_cyan = (byte)90;
        public const byte door_lightblue = (byte)91;
        public const byte door_purple = (byte)92;
        public const byte door_lightpurple = (byte)93;
        public const byte door_pink = (byte)94;
        public const byte door_darkpink = (byte)95;
        public const byte door_darkgrey = (byte)96;
        public const byte door_lightgrey = (byte)97;
        public const byte door_white = (byte)98;

        public const byte op_glass = (byte)100;
        public const byte opsidian = (byte)101;
        public const byte op_brick = (byte)102;
        public const byte op_stone = (byte)103;
        public const byte op_cobblestone = (byte)104;
        public const byte op_air = (byte)105;
        public const byte op_water = (byte)106;
        public const byte op_lava = (byte)107;

        public const byte griefer_stone = (byte)108;
        public const byte lava_sponge = (byte)109;

        public const byte wood_float = (byte)110;
        public const byte door = (byte)111;
        public const byte lava_fast = (byte)112;
        public const byte door2 = (byte)113;
        public const byte door3 = (byte)114;
        public const byte door4 = (byte)115;
        public const byte door5 = (byte)116;
        public const byte door6 = (byte)117;
        public const byte door7 = (byte)118;
        public const byte door8 = (byte)119;
        public const byte door9 = (byte)120;
        public const byte door10 = (byte)121;

        public const byte tdoor = (byte)122;
        public const byte tdoor2 = (byte)123;
        public const byte tdoor3 = (byte)124;
        public const byte tdoor4 = (byte)125;
        public const byte tdoor5 = (byte)126;
        public const byte tdoor6 = (byte)127;
        public const byte tdoor7 = (byte)128;
        public const byte tdoor8 = (byte)129;

        //Messages
        public const byte MsgWhite = (byte)130;
        public const byte MsgBlack = (byte)131;
        public const byte MsgAir = (byte)132;
        public const byte MsgWater = (byte)133;
        public const byte MsgLava = (byte)134;

        public const byte tdoor9 = (byte)135;
        public const byte tdoor10 = (byte)136;
        public const byte tdoor11 = (byte)137;
        public const byte tdoor12 = (byte)138;
        public const byte tdoor13 = (byte)139;

        //"finite"
        public const byte WaterDown = (byte)140;
        public const byte LavaDown = (byte)141;
        public const byte WaterFaucet = (byte)143;
        public const byte LavaFaucet = (byte)144;

        public const byte finiteWater = (byte)145;
        public const byte finiteLava = (byte)146;
        public const byte finiteFaucet = (byte)147;

        public const byte odoor1 = (byte)148;
        public const byte odoor2 = (byte)149;
        public const byte odoor3 = (byte)150;
        public const byte odoor4 = (byte)151;
        public const byte odoor5 = (byte)152;
        public const byte odoor6 = (byte)153;
        public const byte odoor7 = (byte)154;
        public const byte odoor8 = (byte)155;
        public const byte odoor9 = (byte)156;
        public const byte odoor10 = (byte)157;
        public const byte odoor11 = (byte)158;
        public const byte odoor12 = (byte)159;

        //movement
        public const byte air_portal = (byte)160;
        public const byte water_portal = (byte)161;
        public const byte lava_portal = (byte)162;

        //BlockDefinitions
        public const byte block_definitions = (byte)163;
        //Movement doors
        public const byte air_door = (byte)164;
        public const byte air_switch = (byte)165;
        public const byte water_door = (byte)166;
        public const byte lava_door = (byte)167;

        public const byte odoor1_air = (byte)168;
        public const byte odoor2_air = (byte)169;
        public const byte odoor3_air = (byte)170;
        public const byte odoor4_air = (byte)171;
        public const byte odoor5_air = (byte)172;
        public const byte odoor6_air = (byte)173;
        public const byte odoor7_air = (byte)174;

        //portals
        public const byte blue_portal = (byte)175;
        public const byte orange_portal = (byte)176;

        public const byte odoor8_air = (byte)177;
        public const byte odoor9_air = (byte)178;
        public const byte odoor10_air = (byte)179;
        public const byte odoor11_air = (byte)180;
        public const byte odoor12_air = (byte)181;

        //Explosions
        public const byte smalltnt = (byte)182;
        public const byte bigtnt = (byte)183;
        public const byte tntexplosion = (byte)184;

        public const byte lava_fire = (byte)185;
		
		public const byte nuketnt = (byte)186;

        public const byte rocketstart = (byte)187;
        public const byte rockethead = (byte)188;
        public const byte firework = (byte)189;

        //Death
        public const byte deathlava = (byte)190;
        public const byte deathwater = (byte)191;
        public const byte deathair = (byte)192;

        public const byte activedeathwater = (byte)193;
        public const byte activedeathlava = (byte)194;

        public const byte magma = (byte)195;
        public const byte geyser = (byte)196;

        public const byte air_flood = (byte)200;
        public const byte door_air = (byte)201;
        public const byte air_flood_layer = (byte)202;
        public const byte air_flood_down = (byte)203;
        public const byte air_flood_up = (byte)204;
        public const byte door2_air = (byte)205;
        public const byte door3_air = (byte)206;
        public const byte door4_air = (byte)207;
        public const byte door5_air = (byte)208;
        public const byte door6_air = (byte)209;
        public const byte door7_air = (byte)210;
        public const byte door8_air = (byte)211;
        public const byte door9_air = (byte)212;
        public const byte door10_air = (byte)213;
        public const byte door11_air = (byte)214;
        public const byte door12_air = (byte)215;
        public const byte door13_air = (byte)216;
        public const byte door14_air = (byte)217;

        public const byte door_iron = (byte)220;
        public const byte door_dirt = (byte)221;
        public const byte door_grass = (byte)222;
        public const byte door_blue = (byte)223;
        public const byte door_book = (byte)224;
        public const byte door_iron_air = (byte)225;
        public const byte door_dirt_air = (byte)226;
        public const byte door_grass_air = (byte)227;
        public const byte door_blue_air = (byte)228;
        public const byte door_book_air = (byte)229;

        public const byte train = (byte)230;

        public const byte creeper = (byte)231;
        public const byte zombiebody = (byte)232;
        public const byte zombiehead = (byte)233;

        public const byte birdwhite = (byte)235;
        public const byte birdblack = (byte)236;
        public const byte birdwater = (byte)237;
        public const byte birdlava = (byte)238;
        public const byte birdred = (byte)239;
        public const byte birdblue = (byte)240;
        public const byte birdkill = (byte)242;

        public const byte fishgold = (byte)245;
        public const byte fishsponge = (byte)246;
        public const byte fishshark = (byte)247;
        public const byte fishsalmon = (byte)248;
        public const byte fishbetta = (byte)249;
        public const byte fishlavashark = (byte)250;

        public const byte snake = (byte)251;
        public const byte snaketail = (byte)252;
		
		public const byte door_gold = (byte)253;
		public const byte door_gold_air = (byte)254;

        public static List<Blocks> BlockList = new List<Blocks>();
        public class Blocks
        {
            public byte type;
            public LevelPermission lowestRank;
            public List<LevelPermission> disallow = new List<LevelPermission>();
            public List<LevelPermission> allow = new List<LevelPermission>();

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

        public static void SetBlocks()
        {
            BlockList = new List<Blocks>();
            Blocks b = new Blocks();
            b.lowestRank = LevelPermission.Guest;

            for (int i = 0; i < 256; i++)
            {
                b = new Blocks();
                b.type = (byte)i;
                BlockList.Add(b);
            }

            List<Blocks> storedList = new List<Blocks>();

            foreach (Blocks bs in BlockList)
            {
                b = new Blocks();
                b.type = bs.type;

                switch (bs.type)
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

                    case door_air:
                    case door2_air:
                    case door3_air:
                    case door4_air:
                    case door5_air:
                    case door6_air:
                    case door7_air:
                    case door8_air:
                    case door9_air:
                    case door10_air:
                    case door11_air:
                    case door12_air:
                    case door13_air:
                    case door14_air:
                    case door_iron_air:
					case door_gold_air:
                    case door_cobblestone_air:
                    case door_red_air:
                    case door_grass_air:
                    case door_dirt_air:
                    case door_blue_air:
                    case door_book_air:

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

                    case train:

                    case birdwhite:
                    case birdblack:
                    case birdwater:
                    case birdlava:
                        b.lowestRank = LevelPermission.AdvBuilder;
                        break;

                    case door:
                    case door2:
                    case door3:
                    case door4:
                    case door5:
                    case door6:
                    case door7:
                    case door8:
                    case door9:
                    case door10:
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

                storedList.Add(b);
            }

            //CHECK FOR SPECIAL RANK ALLOWANCES SET BY USER
            if (File.Exists("properties/block.properties"))
            {
                string[] lines = File.ReadAllLines("properties/block.properties");

                //if (lines.Length == 0) ; // this is useless?
                /*else */if (lines[0] == "#Version 2")
                {
                    string[] colon = new string[] { " : " };
                    foreach (string line in lines)
                    {
                        if (line != "" && line[0] != '#')
                        {
                            //Name : Lowest : Disallow : Allow
                            string[] block = line.Split(colon, StringSplitOptions.None);
                            Blocks newBlock = new Blocks();

                            if (Block.Byte(block[0]) == Block.Zero)
                            {
                                continue;
                            }
                            newBlock.type = Block.Byte(block[0]);

                            string[] disallow = new string[0];
                            if (block[2] != "")
                                disallow = block[2].Split(',');
                            string[] allow = new string[0];
                            if (block[3] != "")
                                allow = block[3].Split(',');

                            try
                            {
                                newBlock.lowestRank = (LevelPermission)int.Parse(block[1]);
                                foreach (string s in disallow) { newBlock.disallow.Add((LevelPermission)int.Parse(s)); }
                                foreach (string s in allow) { newBlock.allow.Add((LevelPermission)int.Parse(s)); }
                            }
                            catch
                            {
                                Server.s.Log("Hit an error on the block " + line);
                                continue;
                            }

                            int current = 0;
                            foreach (Blocks bS in storedList)
                            {
                                if (newBlock.type == bS.type)
                                {
                                    storedList[current] = newBlock;
                                    break;
                                }
                                current++;
                            }
                        }
                    }
                }
                else
                {
                    foreach (string s in lines)
                    {
                        if (s[0] != '#')
                        {
                            try
                            {
                                Blocks newBlock = new Blocks();
                                newBlock.type = Block.Byte(s.Split(' ')[0]);
                                newBlock.lowestRank = Level.PermissionFromName(s.Split(' ')[2]);
                                if (newBlock.lowestRank != LevelPermission.Null)
                                    storedList[storedList.FindIndex(sL => sL.type == newBlock.type)] = newBlock;
                                else
                                    throw new Exception();
                            }
                            catch { Server.s.Log("Could not find the rank given on " + s + ". Using default"); }
                        }
                    }
                }
            }

            BlockList.Clear();
            BlockList.AddRange(storedList);
            SaveBlocks(BlockList);
        }
        public static void SaveBlocks(List<Blocks> givenList)
        {
            try
            {
				using (StreamWriter w = File.CreateText("properties/block.properties"))
				{
					w.WriteLine("#Version 2");
					w.WriteLine("#   This file dictates what levels may use what blocks");
					w.WriteLine("#   If someone has royally screwed up the ranks, just delete this file and let the server restart");
					w.WriteLine("#   Allowed ranks: " + Group.concatList(false, false, true));
					w.WriteLine("#   Disallow and allow can be left empty, just make sure there's 2 spaces between the colons");
					w.WriteLine("#   This works entirely on permission values, not names. Do not enter a rank name. Use it's permission value");
					w.WriteLine("#   BlockName : LowestRank : Disallow : Allow");
					w.WriteLine("#   lava : 60 : 80,67 : 40,41,55");
					w.WriteLine("");

					foreach (Blocks bs in givenList)
					{
						if (bs.IncludeInBlockProperties())
						{
							string line = Block.Name(bs.type) + " : " + (int)bs.lowestRank + " : " + GrpCommands.getInts(bs.disallow) + " : " + GrpCommands.getInts(bs.allow);
							w.WriteLine(line);
						}
					}
				}
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }

        public static bool canPlace(Player p, byte b) { return canPlace(p.group.Permission, b); }
        public static bool canPlace(LevelPermission givenPerm, byte givenBlock)
        {
            foreach (Blocks b in BlockList)
            {
                if (givenBlock == b.type)
                {
                    if ((b.lowestRank <= givenPerm && !b.disallow.Contains(givenPerm)) || b.allow.Contains(givenPerm)) return true;
                    return false;
                }
            }

            return false;
        }

        public static bool Walkthrough(byte type)
        {
            switch (type)
            {
                case air:
                case water:
                case waterstill:
                case lava:
                case lavastill:
                case yellowflower:
                case redflower:
                case mushroom:
                case redmushroom:
                case shrub:
                    return true;
            }
            return false;
        }

        public static bool AnyBuild(byte type)
        {
            switch (type)
            {
                case Block.air:
                case Block.rock:
                case Block.grass:
                case Block.dirt:
                case Block.stone:
                case Block.wood:
                case Block.shrub:
                case Block.sand:
                case Block.gravel:
                case Block.goldrock:
                case Block.ironrock:
                case Block.coal:
                case Block.trunk:
                case Block.leaf:
                case Block.sponge:
                case Block.glass:
                case Block.red:
                case Block.orange:
                case Block.yellow:
                case Block.lightgreen:
                case Block.green:
                case Block.aquagreen:
                case Block.cyan:
                case Block.lightblue:
                case Block.blue:
                case Block.purple:
                case Block.lightpurple:
                case Block.pink:
                case Block.darkpink:
                case Block.darkgrey:
                case Block.lightgrey:
                case Block.white:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                case Block.goldsolid:
                case Block.iron:
                case Block.staircasefull:
                case Block.staircasestep:
                case Block.brick:
                case Block.tnt:
                case Block.bookcase:
                case Block.stonevine:
                case Block.obsidian:
                    return true;
            }
            return false;
        }

        public static bool AllowBreak(byte type)
        {
            switch (type)
            {
                case Block.blue_portal:
                case Block.orange_portal:

                case Block.MsgWhite:
                case Block.MsgBlack:

                case Block.door:
                case Block.door2:
                case Block.door3:
                case Block.door4:
                case Block.door5:
                case Block.door6:
                case Block.door7:
                case Block.door8:
                case Block.door9:
                case Block.door10:
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

        public static bool Placable(byte type)
        {
            switch (type)
            {
                //				case Block.air:
                //				case Block.grass:
						case Block.blackrock:
						case Block.water:
						case Block.waterstill:
						case Block.lava:
						case Block.lavastill:
								return false;
            }

            if (type > 65) { return false; }
            return true;
        }

        public static bool RightClick(byte type, bool countAir = false)
        {
            if (countAir && type == Block.air) return true;

            switch (type)
            {
                case Block.water:
                case Block.lava:
                case Block.waterstill:
                case Block.lavastill:
                    return true;
            }
            return false;
        }

        public static bool OPBlocks(byte type)
        {
            switch (type)
            {
                case Block.blackrock:
                case Block.op_air:
                case Block.op_brick:
                case Block.op_cobblestone:
                case Block.op_glass:
                case Block.op_stone:
                case Block.op_water:
                case Block.op_lava:
                case Block.opsidian:
                case Block.rocketstart:

                case Block.Zero:
                    return true;
            }
            return false;
        }

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

            switch (Block.Convert(type))
            {
                case water:
                case lava:
                case waterstill:
                case lavastill:
                    return true;
            }
            return false;
        }

        public static bool Mover(byte type)
        {
            switch (type)
            {
                case Block.air_portal:
                case Block.water_portal:
                case Block.lava_portal:

                case Block.air_switch:
                case Block.water_door:
                case Block.lava_door:

                case Block.MsgAir:
                case Block.MsgWater:
                case Block.MsgLava:

                case Block.flagbase:
                    return true;
            }
            return false;
        }

        public static bool FireKill(byte type) {
        	return type != Block.air && LavaKill(type);
        }
        
        public static bool LavaKill(byte type)
        {
            switch (type)
            {
            	case Block.air:
                case Block.wood:
                case Block.shrub:
                case Block.trunk:
                case Block.leaf:
                case Block.sponge:
                case Block.red:
                case Block.orange:
                case Block.yellow:
                case Block.lightgreen:
                case Block.green:
                case Block.aquagreen:
                case Block.cyan:
                case Block.lightblue:
                case Block.blue:
                case Block.purple:
                case Block.lightpurple:
                case Block.pink:
                case Block.darkpink:
                case Block.darkgrey:
                case Block.lightgrey:
                case Block.white:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                case Block.bookcase:
                    return true;
            }
            return false;
        }
        public static bool WaterKill(byte type)
        {
            switch (type)
            {
                case Block.air:
                case Block.shrub:
                case Block.leaf:
                case Block.yellowflower:
                case Block.redflower:
                case Block.mushroom:
                case Block.redmushroom:
                    return true;
            }
            return false;
        }

        public static bool LightPass(byte type)
        {
            switch (Convert(type))
            {
                case Block.air:
                case Block.glass:
                case Block.leaf:
                case Block.redflower:
                case Block.yellowflower:
                case Block.mushroom:
                case Block.redmushroom:
                case Block.shrub:
                    return true;

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

        public static bool portal(byte type)
        {
            switch (type)
            {
                case Block.blue_portal:
                case Block.orange_portal:
                case Block.air_portal:
                case Block.water_portal:
                case Block.lava_portal:
                    return true;
            }
            return false;
        }
        public static bool mb(byte type)
        {
            switch (type)
            {
                case Block.MsgAir:
                case Block.MsgWater:
                case Block.MsgLava:
                case Block.MsgBlack:
                case Block.MsgWhite:
                    return true;
            }
            return false;
        }

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

                case Block.door:
                case Block.door2:
                case Block.door3:
                case Block.door4:
                case Block.door5:
                case Block.door6:
                case Block.door7:
                case Block.door8:
                case Block.door9:
                case Block.door10:
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

        public static string Name(byte type)
        {
            switch (type)
            {
                case 0: return "air";
                case 1: return "stone";
                case 2: return "grass";
                case 3: return "dirt";
                case 4: return "cobblestone";
                case 5: return "wood";
                case 6: return "plant";
                case 7: return "adminium";
                case 8: return "active_water";
                case 9: return "water";
                case 10: return "active_lava";
                case 11: return "lava";
                case 12: return "sand";
                case 13: return "gravel";
                case 14: return "gold_ore";
                case 15: return "iron_ore";
                case 16: return "coal";
                case 17: return "tree";
                case 18: return "leaves";
                case 19: return "sponge";
                case 20: return "glass";
                case 21: return "red";
                case 22: return "orange";
                case 23: return "yellow";
                case 24: return "greenyellow";
                case 25: return "green";
                case 26: return "springgreen";
                case 27: return "cyan";
                case 28: return "blue";
                case 29: return "blueviolet";
                case 30: return "indigo";
                case 31: return "purple";
                case 32: return "magenta";
                case 33: return "pink";
                case 34: return "black";
                case 35: return "gray";
                case 36: return "white";
                case 37: return "yellow_flower";
                case 38: return "red_flower";
                case 39: return "brown_shroom";
                case 40: return "red_shroom";
                case 41: return "gold";
                case 42: return "iron";
                case 43: return "double_stair";
                case 44: return "stair";
                case 45: return "brick";
                case 46: return "tnt";
                case 47: return "bookcase";
                case 48: return "mossy_cobblestone";
                case 49: return "obsidian";
				case 50: return "cobblestoneslab";
				case 51: return "rope";
				case 52: return "sandstone";
				case 53: return "snow";
				case 54: return "fire";
				case 55: return "lightpink";
				case 56: return "forestgreen";
				case 57: return "brown";
				case 58: return "deepblue";
				case 59: return "turquoise";
				case 60: return "ice";
				case 61: return "ceramictile";
				case 62: return "magmablock";
				case 63: return "pillar";
				case 64: return "crate";
				case 65: return "stonebrick";
                case 70: return "flagbase";
                case 73: return "fast_hot_lava";
                case 100: return "op_glass";
                case 101: return "opsidian";              //TODO Add command or just use bind?
                case 102: return "op_brick";              //TODO
                case 103: return "op_stone";              //TODO
                case 104: return "op_cobblestone";        //TODO
                case 105: return "op_air";                //TODO
                case 106: return "op_water";              //TODO
                case 107: return "op_lava";

                case 108: return "griefer_stone";
                case 109: return "lava_sponge";

                case wood_float: return "wood_float";            //TODO
                case door: return "door_wood";
                case lava_fast: return "lava_fast";
                case door2: return "door_obsidian";
                case door3: return "door_glass";
                case door4: return "door_stone";
                case door5: return "door_leaves";
                case door6: return "door_sand";
                case door7: return "door_wood";
                case door8: return "door_green";
                case door9: return "door_tnt";
                case door10: return "door_stair";
                case door_iron: return "door_iron";
				case door_gold: return "door_gold";
                case door_cobblestone: return "door_cobblestone";
                case door_red: return "door_red";
                case door_grass: return "door_grass";
                case door_dirt: return "door_dirt";
                case door_blue: return "door_blue";
                case door_book: return "door_book";

                case door_orange: return "door_orange";
                case door_yellow: return "door_yellow";
                case door_lightgreen: return "door_lightgreen";
                case door_aquagreen: return "door_aquagreen";
                case door_cyan: return "door_cyan";
                case door_lightblue: return "door_lightblue";
                case door_purple: return "door_purple";
                case door_lightpurple: return "door_lightpurple";
                case door_pink: return "door_pink";
                case door_darkpink: return "door_darkpink";
                case door_darkgrey: return "door_darkgrey";
                case door_lightgrey: return "door_lightgrey";
                case door_white: return "door_white";

                case tdoor: return "tdoor_wood";
                case tdoor2: return "tdoor_obsidian";
                case tdoor3: return "tdoor_glass";
                case tdoor4: return "tdoor_stone";
                case tdoor5: return "tdoor_leaves";
                case tdoor6: return "tdoor_sand";
                case tdoor7: return "tdoor_wood";
                case tdoor8: return "tdoor_green";
                case tdoor9: return "tdoor_tnt";
                case tdoor10: return "tdoor_stair";
                case tdoor11: return "tdoor_air";
                case tdoor12: return "tdoor_water";
                case tdoor13: return "tdoor_lava";

                case odoor1: return "odoor_wood";
                case odoor2: return "odoor_obsidian";
                case odoor3: return "odoor_glass";
                case odoor4: return "odoor_stone";
                case odoor5: return "odoor_leaves";
                case odoor6: return "odoor_sand";
                case odoor7: return "odoor_wood";
                case odoor8: return "odoor_green";
                case odoor9: return "odoor_tnt";
                case odoor10: return "odoor_stair";
                case odoor11: return "odoor_lava";
                case odoor12: return "odoor_water";

                case odoor1_air: return "odoor_wood_air";
                case odoor2_air: return "odoor_obsidian_air";
                case odoor3_air: return "odoor_glass_air";
                case odoor4_air: return "odoor_stone_air";
                case odoor5_air: return "odoor_leaves_air";
                case odoor6_air: return "odoor_sand_air";
                case odoor7_air: return "odoor_wood_air";
                case odoor8_air: return "odoor_red";
                case odoor9_air: return "odoor_tnt_air";
                case odoor10_air: return "odoor_stair_air";
                case odoor11_air: return "odoor_lava_air";
                case odoor12_air: return "odoor_water_air";

                case 130: return "white_message";
                case 131: return "black_message";
                case 132: return "air_message";
                case 133: return "water_message";
                case 134: return "lava_message";

                case 140: return "waterfall";
                case 141: return "lavafall";
                case WaterFaucet: return "water_faucet";
                case LavaFaucet: return "lava_faucet";

                case finiteWater: return "finite_water";
                case finiteLava: return "finite_lava";
                case finiteFaucet: return "finite_faucet";

                case 160: return "air_portal";
                case 161: return "water_portal";
                case 162: return "lava_portal";
                case block_definitions: return "custom_block";
                case air_door: return "air_door";
                case air_switch: return "air_switch";
                case water_door: return "door_water";
                case lava_door: return "door_lava";

                case 175: return "blue_portal";
                case 176: return "orange_portal";

                case c4: return "c4";
                case c4det: return "c4_det";
                case 182: return "small_tnt";
                case 183: return "big_tnt";
                case 186: return "nuke_tnt";
                case 184: return "tnt_explosion";

                case lava_fire: return "lava_fire";

                case rocketstart: return "rocketstart";
                case rockethead: return "rockethead";
                case firework: return "firework";

                case 190: return "hot_lava";
                case 191: return "cold_water";
                case 192: return "nerve_gas";
                case activedeathwater: return "active_cold_water";
                case activedeathlava: return "active_hot_lava";

                case 195: return "magma";
                case 196: return "geyser";

                //Blocks after this are converted before saving
                case 200: return "air_flood";
                case 201: return "door_air";
                case 202: return "air_flood_layer";
                case 203: return "air_flood_down";
                case 204: return "air_flood_up";
                case 205: return "door2_air";
                case 206: return "door3_air";
                case 207: return "door4_air";
                case 208: return "door5_air";
                case 209: return "door6_air";
                case 210: return "door7_air";
                case 211: return "door8_air";
                case 212: return "door9_air";
                case 213: return "door10_air";
                case 214: return "door11_air";
                case 215: return "door12_air";
                case 216: return "door13_air";
                case 217: return "door14_air";
                case door_iron_air: return "door_iron_air";
				case door_gold_air: return "door_gold_air";
                case door_dirt_air: return "door_dirt_air";
                case door_grass_air: return "door_grass_air";
                case door_blue_air: return "door_blue_air";
                case door_book_air: return "door_book_air";
                case door_cobblestone_air: return "door_cobblestone_air";
                case door_red_air: return "door_red_air";

                //"AI" blocks
                case train: return "train";

                case snake: return "snake";
                case snaketail: return "snake_tail";

                case creeper: return "creeper";
                case zombiebody: return "zombie";
                case zombiehead: return "zombie_head";

                case Block.birdblue: return "blue_bird";
                case Block.birdred: return "red_robin";
                case Block.birdwhite: return "dove";
                case Block.birdblack: return "pidgeon";
                case Block.birdwater: return "duck";
                case Block.birdlava: return "phoenix";
                case Block.birdkill: return "killer_phoenix";

                case fishbetta: return "betta_fish";
                case fishgold: return "goldfish";
                case fishsalmon: return "salmon";
                case fishshark: return "shark";
                case fishsponge: return "sea_sponge";
                case fishlavashark: return "lava_shark";

                default: return "unknown";
            }
        }
        public static byte Byte(string type)
        {
            switch (type.ToLower())
            {
                case "0":
                case "air": return 0;
                case "1":
                case "stone": return 1;
                case "2":
                case "grass": return 2;
                case "3":
                case "dirt": return 3;
                case "4":
                case "cobblestone": return 4;
                case "5":
                case "wood": return 5;
                case "6":
                case "plant": return 6;
                case "solid":
                case "admintite":
                case "blackrock":
                case "7":
                case "adminium": return 7;
                case "activewater":
                case "8":
                case "active_water": return 8;
                case "9":
                case "water": return 9;
                case "activelava":
                case "10":
                case "active_lava": return 10;
                case "11":
                case "lava": return 11;
                case "12":
                case "sand": return 12;
                case "13":
                case "gravel": return 13;
                case "14":
                case "gold_ore": return 14;
                case "15":
                case "iron_ore": return 15;
                case "16":
                case "coal": return 16;
                case "17":
                case "tree": return 17;
                case "18":
                case "leaves": return 18;
                case "19":
                case "sponge": return 19;
                case "20":
                case "glass": return 20;
                case "21":
                case "red": return 21;
                case "22":
                case "orange": return 22;
                case "23":
                case "yellow": return 23;
                case "24":
                case "greenyellow": return 24;
                case "25":
                case "green": return 25;
                case "26":
                case "springgreen": return 26;
                case "27":
                case "cyan": return 27;
                case "28":
                case "blue": return 28;
                case "29":
                case "blueviolet": return 29;
                case "30":
                case "indigo": return 30;
                case "31":
                case "purple": return 31;
                case "32":
                case "magenta": return 32;
                case "33":
                case "pink": return 33;
                case "34":
                case "black": return 34;
                case "35":
                case "gray": return 35;
                case "36":
                case "white": return 36;
                case "37":
                case "yellow_flower": return 37;
                case "38":
                case "red_flower": return 38;
                case "39":
                case "brown_shroom": return 39;
                case "40":
                case "red_shroom": return 40;
                case "41":
                case "gold": return 41;
                case "42":
                case "iron": return 42;
                case "43":
                case "double_stair": return 43;
                case "44":
                case "stair": return 44;
                case "45":
                case "brick": return 45;
                case "46":
                case "tnt": return 46;
                case "47":
                case "bookcase": return 47;
                case "48":
                case "mossy_cobblestone": return 48;
                case "49":
                case "obsidian": return 49;
                case "50":
                case "cobblestoneslab": return 50;
                case "51":
                case "rope": return 51;
                case "52":
                case "sandstone": return 52;
                case "53":
                case "snow": return 53;
                case "54":
                case "fire": return 54;
                case "55":
                case "lightpink": return 55;
                case "56":
                case "forestgreen": return 56;
                case "57":
                case "brown": return 57;
                case "58":
                case "deepblue": return 58;
                case "59":
                case "turquoise": return 59;
                case "60":
                case "ice": return 60;
                case "61":
                case "ceramictile": return 61;
                case "62":
                case "magmablock": return 62;
                case "63":
                case "pillar": return 63;
                case "64":
                case "crate": return 64;
                case "65":
                case "stonebrick": return 65;
                case "fhl":
                case "fast_hot_lava": return 73;
                case "op_glass": return 100;
                case "opsidian": return 101;              //TODO Add command or just use bind?
                case "op_brick": return 102;              //TODO
                case "op_stone": return 103;              //TODO
                case "op_cobblestone": return 104;        //TODO
                case "op_air": return 105;                //TODO
                case "op_water": return 106;              //TODO
                case "op_lava": return 107;

                case "griefer_stone": return 108;
                case "lava_sponge": return 109;

                case "wood_float": return 110;            //TODO
                case "lava_fast": return 112;

                case "door_tree":
                case "door": return door;
                case "door_obsidian":
                case "door2": return door2;
                case "door_glass":
                case "door3": return door3;
                case "door_stone":
                case "door4": return door4;
                case "door_leaves":
                case "door5": return door5;
                case "door_sand":
                case "door6": return door6;
                case "door_wood":
                case "door7": return door7;
                case "door_green":
                case "door8": return door8;
                case "door_tnt":
                case "door9": return door9;
                case "door_stair":
                case "door10": return door10;
                case "door11":
                case "door_iron": return door_iron;
                case "door12":
                case "door_dirt": return door_dirt;
                case "door13":
                case "door_grass": return door_grass;
                case "door14":
                case "door_blue": return door_blue;
                case "door15":
                case "door_book": return door_book;
				case "door16":
				case "door_gold": return door_gold;
                case "door17":
                case "door_cobblestone": return door_cobblestone;
                case "door18":
                case "door_red": return door_red;

                case "door_orange": return door_orange;
                case "door_yellow": return door_yellow;
                case "door_lightgreen": return door_lightgreen;
                case "door_aquagreen": return door_aquagreen;
                case "door_cyan": return door_cyan;
                case "door_lightblue": return door_lightblue;
                case "door_purple": return door_purple;
                case "door_lightpurple": return door_lightpurple;
                case "door_pink": return door_pink;
                case "door_darkpink": return door_darkpink;
                case "door_darkgrey": return door_darkgrey;
                case "door_lightgrey": return door_lightgrey;
                case "door_white": return door_white;

                case "tdoor_tree":
                case "tdoor": return tdoor;
                case "tdoor_obsidian":
                case "tdoor2": return tdoor2;
                case "tdoor_glass":
                case "tdoor3": return tdoor3;
                case "tdoor_stone":
                case "tdoor4": return tdoor4;
                case "tdoor_leaves":
                case "tdoor5": return tdoor5;
                case "tdoor_sand":
                case "tdoor6": return tdoor6;
                case "tdoor_wood":
                case "tdoor7": return tdoor7;
                case "tdoor_green":
                case "tdoor8": return tdoor8;
                case "tdoor_tnt":
                case "tdoor9": return tdoor9;
                case "tdoor_stair":
                case "tdoor10": return tdoor10;
                case "tair_switch":
                case "tdoor11": return tdoor11;
                case "tdoor_water":
                case "tdoor12": return tdoor12;
                case "tdoor_lava":
                case "tdoor13": return tdoor13;

                case "odoor_tree":
                case "odoor": return odoor1;
                case "odoor_obsidian":
                case "odoor2": return odoor2;
                case "odoor_glass":
                case "odoor3": return odoor3;
                case "odoor_stone":
                case "odoor4": return odoor4;
                case "odoor_leaves":
                case "odoor5": return odoor5;
                case "odoor_sand":
                case "odoor6": return odoor6;
                case "odoor_wood":
                case "odoor7": return odoor7;
                case "odoor_green":
                case "odoor8": return odoor8;
                case "odoor_tnt":
                case "odoor9": return odoor9;
                case "odoor_stair":
                case "odoor10": return odoor10;
                case "odoor_lava":
                case "odoor11": return odoor11;
                case "odoor_water":
                case "odoor12": return odoor12;
                case "odoor_red": return odoor8_air;

                case "white_message": return 130;
                case "black_message": return 131;
                case "air_message": return 132;
                case "water_message": return 133;
                case "lava_message": return 134;

                case "waterfall": return 140;
                case "lavafall": return 141;
                case "water_faucet": return WaterFaucet;
                case "lava_faucet": return LavaFaucet;

                case "finite_water": return finiteWater;
                case "finite_lava": return finiteLava;
                case "finite_faucet": return finiteFaucet;

                case "air_portal": return 160;
                case "water_portal": return 161;
                case "lava_portal": return 162;

                case "air_door": return air_door;
                case "air_switch": return air_switch;
                case "door_water":
                case "water_door": return water_door;
                case "door_lava":
                case "lava_door": return lava_door;

                case "blue_portal": return 175;
                case "orange_portal": return 176;

                case "c4": return c4;
                case "c4_det": return c4det;
                case "small_tnt": return 182;
                case "big_tnt": return 183;
                case "nuke_tnt": return 186;
                case "tnt_explosion": return 184;

                case "lava_fire": return lava_fire;

                case "rocketstart": return rocketstart;
                case "rockethead": return rockethead;
                case "firework": return firework;

                case "hot_lava": return 190;
                case "cold_water": return 191;
                case "nerve_gas": return 192;
                case "acw":
                case "active_cold_water": return activedeathwater;
                case "ahl":
                case "active_hot_lava": return activedeathlava;

                case "magma": return 195;
                case "geyser": return 196;

                //Blocks after this are converted before saving
                case "air_flood": return air_flood;
                case "air_flood_layer": return air_flood_layer;
                case "air_flood_down": return air_flood_down;
                case "air_flood_up": return air_flood_up;
                case "door_air": return door_air;
                case "door2_air": return door2_air;
                case "door3_air": return door3_air;
                case "door4_air": return door4_air;
                case "door5_air": return door5_air;
                case "door6_air": return door6_air;
                case "door7_air": return door7_air;
                case "door8_air": return door8_air;
                case "door9_air": return door9_air;
                case "door10_air": return door10_air;
                case "door11_air": return door11_air;
                case "door12_air": return door12_air;
                case "door13_air": return door13_air;
                case "door14_air": return door14_air;
                case "door_iron_air": return door_iron_air;
                case "door_dirt_air": return door_dirt_air;
                case "door_grass_air": return door_grass_air;
                case "door_blue_air": return door_blue_air;
                case "door_book_air": return door_book_air;
				case "door_gold_air": return door_gold_air;
                case "door_cobblestone_air": return door_cobblestone_air;
                case "door_red_air": return door_red_air;

                case "train": return train;

                case "snake": return snake;
                case "snake_tail": return snaketail;

                case "creeper": return creeper;
                case "zombie": return zombiebody;
                case "zombie_head": return zombiehead;

                case "blue_bird": return Block.birdblue;
                case "red_robin": return Block.birdred;
                case "dove": return Block.birdwhite;
                case "pidgeon": return Block.birdblack;
                case "duck": return Block.birdwater;
                case "phoenix": return Block.birdlava;
                case "killer_phoenix": return Block.birdkill;

                case "betta_fish": return fishbetta;
                case "goldfish": return fishgold;
                case "salmon": return fishsalmon;
                case "shark": return fishshark;
                case "sea_sponge": return fishsponge;
                case "lava_shark": return fishlavashark;

                default: return Zero;
            }
        }
		public static byte ConvertCPE( byte b ) {
			switch ( b ) {
				case 50: return 44;
				case 51: return 39;
				case 52: return 12;
				case 53: return 0;
				case 54: return 10;
				case 55: return 33;
				case 56: return 25;
				case 57: return 3;
				case 58: return 29;
				case 59: return 28;
				case 60: return 20;
				case 61: return 42;
				case 62: return 49;
				case 63: return 36;
				case 64: return 5;
				case 65: return 1;
				default:
				return b;
			}
		}
        public static byte Convert(byte b)
        {
            switch (b)
            {
                case flagbase: return mushroom; //CTF Flagbase
                case 100: return (byte)20; //Op_glass
                case 101: return (byte)49; //Opsidian
                case 102: return (byte)45; //Op_brick
                case 103: return (byte)1; //Op_stone
                case 104: return (byte)4; //Op_cobblestone
                case 105: return (byte)0; //Op_air - Must be cuboided / replaced
                case 106: return waterstill; //Op_water
                case 107: return lavastill; //Op_lava

                case 108: return Server.grieferStoneType; //Griefer_stone
                case 109: return (byte)19; //Lava_sponge

                case 110: return (byte)5; //wood_float
                case 112: return (byte)10;
                case 71:
                case 72:
                    return Block.white;
                case door: return trunk;//door show by treetype
                case door2: return obsidian;//door show by obsidian
                case door3: return glass;//door show by glass
                case door4: return rock;//door show by stone
                case door5: return leaf;//door show by leaves
                case door6: return sand;//door show by sand
                case door7: return wood;//door show by wood
                case door8: return green;
                case door9: return tnt;//door show by TNT
                case door10: return staircasestep;//door show by Stair
                case door_iron: return iron;
                case door_dirt: return dirt;
                case door_grass: return grass;
                case door_blue: return blue;
                case door_book: return bookcase;
				case door_gold: return goldsolid;
                case door_cobblestone: return 4;
                case door_red: return red;

                case door_orange: return Block.orange;
                case door_yellow: return yellow;
                case door_lightgreen: return lightgreen;
                case door_aquagreen: return aquagreen;
                case door_cyan: return cyan;
                case door_lightblue: return lightblue;
                case door_purple: return purple;
                case door_lightpurple: return lightpurple;
                case door_pink: return pink;
                case door_darkpink: return darkpink;
                case door_darkgrey: return darkgrey;
                case door_lightgrey: return lightgrey;
                case door_white: return white;

                case tdoor: return trunk;//tdoor show by treetype
                case tdoor2: return obsidian;//tdoor show by obsidian
                case tdoor3: return glass;//tdoor show by glass
                case tdoor4: return rock;//tdoor show by stone
                case tdoor5: return leaf;//tdoor show by leaves
                case tdoor6: return sand;//tdoor show by sand
                case tdoor7: return wood;//tdoor show by wood
                case tdoor8: return green;
                case tdoor9: return tnt;//tdoor show by TNT
                case tdoor10: return staircasestep;//tdoor show by Stair
                case tdoor11: return air;
                case tdoor12: return waterstill;
                case tdoor13: return lavastill;

                case odoor1: return trunk;//odoor show by treetype
                case odoor2: return obsidian;//odoor show by obsidian
                case odoor3: return glass;//odoor show by glass
                case odoor4: return rock;//odoor show by stone
                case odoor5: return leaf;//odoor show by leaves
                case odoor6: return sand;//odoor show by sand
                case odoor7: return wood;//odoor show by wood
                case odoor8: return green;
                case odoor9: return tnt;//odoor show by TNT
                case odoor10: return staircasestep;//odoor show by Stair
                case odoor11: return lavastill;
                case odoor12: return waterstill;

                case 130: return (byte)36;  //upVator
                case 131: return (byte)34;  //upVator
                case 132: return (byte)0;   //upVator
                case MsgWater: return waterstill;   //upVator
                case MsgLava: return lavastill;  //upVator

                case 140: return (byte)8;
                case 141: return (byte)10;
                case WaterFaucet: return Block.cyan;
                case LavaFaucet: return Block.orange;

                case finiteWater: return water;
                case finiteLava: return lava;
                case finiteFaucet: return lightblue;

                case 160: return (byte)0;//air portal
                case 161: return waterstill;//water portal
                case 162: return lavastill;//lava portal

                case air_door: return air;
                case air_switch: return air;//air door
                case water_door: return waterstill;//water door
                case lava_door: return lavastill;

                case 175: return (byte)28;//blue portal
                case 176: return (byte)22;//orange portal

                case c4: return (byte)46;
                case c4det: return (byte)red;
                case 182: return (byte)46;//smalltnt
                case 183: return (byte)46;//bigtnt
                case 186: return (byte)46;//nuketnt
                case 184: return (byte)10;//explosion

                case lava_fire: return lava;

                case rocketstart: return glass;
                case rockethead: return goldsolid;
                case firework: return iron;

                case Block.deathwater: return waterstill;
                case Block.deathlava: return lavastill;
                case Block.deathair: return (byte)0;
                case activedeathwater: return water;
                case activedeathlava: return lava;
                case fastdeathlava: return lava;

                case Block.magma: return Block.lava;
                case Block.geyser: return Block.water;

                case 200: //air_flood
                case 201: //door_air
                case 202: //air_flood_layer
                case 203: //air_flood_down
                case 204: //air_flood_up
                case 205: //door2_air
                case 206: //door3_air
                case 207: //door4_air
                case 208: //door5_air
                case 209: //door6_air
                case 210: //door7_air
                case 213: //door10_air
                case 214: //door10_air
                case 215: //door10_air
                case 216: //door10_air
                case door14_air:
                case door_iron_air:
				case door_gold_air:
                case door_cobblestone_air:
                case door_red_air: 
                case door_dirt_air:
                case door_grass_air:
                case door_blue_air:
                case door_book_air:
                    return (byte)0;
                case door9_air: return lava;
                case door8_air: return red;

                case odoor1_air:
                case odoor2_air:
                case odoor3_air:
                case odoor4_air:
                case odoor5_air:
                case odoor6_air:
                case odoor7_air:
                case odoor10_air:
                case odoor11_air:
                case odoor12_air:
                    return air;
                case odoor8_air: return red;
                case odoor9_air: return lavastill;

                case train: return cyan;

                case snake: return darkgrey;
                case snaketail: return coal;

                case creeper: return tnt;
                case zombiebody: return stonevine;
                case zombiehead: return lightgreen;

                case birdwhite: return white;
                case birdblack: return darkgrey;
                case birdlava: return lava;
                case birdred: return red;
                case birdwater: return water;
                case birdblue: return blue;
                case birdkill: return lava;

                case fishbetta: return blue;
                case fishgold: return goldsolid;
                case fishsalmon: return red;
                case fishshark: return lightgrey;
                case fishsponge: return sponge;
                case fishlavashark: return obsidian;

                default:
                    if (b < 66) return b; else return 22;
            }
        }
        public static byte SaveConvert(byte b)
        {
            switch (b)
            {
                case 200:
                case 202:
                case 203:
                case 204:
                    return (byte)0; //air_flood must be converted to air on save to prevent issues
                case 201: return (byte)111; //door_air back into door
                case 205: return (byte)113; //door_air back into door
                case 206: return (byte)114; //door_air back into door
                case 207: return (byte)115; //door_air back into door
                case 208: return (byte)116; //door_air back into door
                case 209: return (byte)117; //door_air back into door
                case 210: return (byte)118; //door_air back into door
                case 211: return (byte)119; //door_air back into door
                case 212: return (byte)120; //door_air back into door
                case 213: return (byte)121; //door_air back into door
                case 214: return (byte)165; //door_air back into door
                case 215: return (byte)166; //door_air back into door
                case 216: return (byte)167; //door_air back into door
                case 217: return air_door; //door_air back into door
                case door_iron_air: return door_iron;
				case door_gold_air: return door_gold;
                case door_dirt_air: return door_dirt;
                case door_grass_air: return door_grass;
                case door_blue_air: return door_blue;
                case door_book_air: return door_book;
                case door_cobblestone_air: return door_cobblestone;
                case door_red_air: return door_red;

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
                    return odoor(b);

                default: return b;
            }
        }
        public static byte DoorAirs(byte b)
        {
            switch (b)
            {
                case door: return door_air;
                case door2: return door2_air;
                case door3: return door3_air;
                case door4: return door4_air;
                case door5: return door5_air;
                case door6: return door6_air;
                case door7: return door7_air;
                case door8: return door8_air;
                case door9: return door9_air;
                case door10: return door10_air;
                case air_switch: return door11_air;
                case water_door: return door12_air;
                case lava_door: return door13_air;
                case air_door: return door14_air;
                case door_iron: return door_iron_air;
				case door_gold: return door_gold_air;
                case door_dirt: return door_dirt_air;
                case door_grass: return door_grass_air;
                case door_blue: return door_blue_air;
                case door_book: return door_book_air;
                case door_cobblestone: return door_cobblestone_air;
                case door_red: return door_red_air;
                default: return 0;
            }
        }

        public static bool tDoor(byte b)
        {
            switch (b)
            {
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
                    return true;
            }
            return false;
        }

        public static byte odoor(byte b)
        {
            switch (b)
            {
                case odoor1: return odoor1_air;
                case odoor2: return odoor2_air;
                case odoor3: return odoor3_air;
                case odoor4: return odoor4_air;
                case odoor5: return odoor5_air;
                case odoor6: return odoor6_air;
                case odoor7: return odoor7_air;
                case odoor8: return odoor8_air;
                case odoor9: return odoor9_air;
                case odoor10: return odoor10_air;
                case odoor11: return odoor11_air;
                case odoor12: return odoor12_air;

                case odoor1_air: return odoor1;
                case odoor2_air: return odoor2;
                case odoor3_air: return odoor3;
                case odoor4_air: return odoor4;
                case odoor5_air: return odoor5;
                case odoor6_air: return odoor6;
                case odoor7_air: return odoor7;
                case odoor8_air: return odoor8;
                case odoor9_air: return odoor9;
                case odoor10_air: return odoor10;
                case odoor11_air: return odoor11;
                case odoor12_air: return odoor12;
            }
            return Zero;
        }
    }
}
