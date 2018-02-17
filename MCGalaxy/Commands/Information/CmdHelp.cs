/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
http://www.osedu.org/licenses/ECL-2.0
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
using MCGalaxy.Commands.CPE;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdHelp : Command {
        public override string name { get { return "Help"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("CmdHelp"), new CommandAlias("Ranks", "ranks"),
                    new CommandAlias("Colors", "colors"), new CommandAlias("Emotes", "emotes") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) {
                PrintHelpMenu(p);
            } else if (message.CaselessEq("ranks")) {
                PrintRanks(p);
            } else if (message.CaselessEq("colors") || message.CaselessEq("colours")) {
                PrintColors(p);
            } else if (message.CaselessEq("emotes") || message.CaselessStarts("emotes ")) {
                PrintEmotes(p, message);
            }  else {
                if (CmdCommands.ListCommands(p, message)) return;
                if (ParseCommand(p, message) || ParseBlock(p, message) || ParsePlugin(p, message)) return;
                Player.Message(p, "Could not find command, plugin or block specified.");
            }
        }
        
        static void PrintHelpMenu(Player p) {
            Player.Message(p, "%HCommand Categories:");
            Player.Message(p, "  %TBuilding Chat Economy Games Info Moderation Other World");
            Player.Message(p, "%HOther Categories:");
            Player.Message(p, "  %TRanks Colors Emotes Shortcuts Commands");
            Player.Message(p, "%HTo view help for a category, type %T/Help CategoryName");
            Player.Message(p, "%HTo see detailed help for a command, type %T/Help CommandName");
            Player.Message(p, "%HTo see your stats, type %T/Info");
            Player.Message(p, "%HTo see loaded maps, type %T/Maps");
            Player.Message(p, "%HTo view your personal world options, use %T/Realm");
            Player.Message(p, "%HTo join a map, type %T/Goto WorldName");
            Player.Message(p, "%HTo send private messages, type %T@PlayerName Message");
        }
        
        static void PrintRanks(Player p) {
            foreach (Group grp in Group.GroupList) {
                if (grp.Permission >= LevelPermission.Nobody) continue;
                int undo = grp.MaxUndo == -1 ? int.MaxValue : grp.MaxUndo;
                string undoTime = TimeSpan.FromSeconds(undo).Shorten(true, false);
                
                Player.Message(p, "{0} %S- Draw: {1}, Undo: {2}, Perm: {3}",
                               grp.ColoredName, grp.DrawLimit, undoTime, (int)grp.Permission);
            }
        }
        
        static void PrintColors(Player p) {
            Player.Message(p, "&fTo use a color, put a '%' and then put the color code.");
            Player.Message(p, "Colors Available:");
            
            Player.Message(p, "0 - &0{0} %S| 1 - &1{1} %S| 2 - &2{2} %S| 3 - &3{3}",
                           Colors.Name('0'), Colors.Name('1'), Colors.Name('2'), Colors.Name('3'));
            Player.Message(p, "4 - &4{0} %S| 5 - &5{1} %S| 6 - &6{2} %S| 7 - &7{3}",
                           Colors.Name('4'), Colors.Name('5'), Colors.Name('6'), Colors.Name('7'));
            
            Player.Message(p, "8 - &8{0} %S| 9 - &9{1} %S| a - &a{2} %S| b - &b{3}",
                           Colors.Name('8'), Colors.Name('9'), Colors.Name('a'), Colors.Name('b'));
            Player.Message(p, "c - &c{0} %S| d - &d{1} %S| e - &e{2} %S| f - &f{3}",
                           Colors.Name('c'), Colors.Name('d'), Colors.Name('e'), Colors.Name('f'));
            
            foreach (ColorDesc col in Colors.List) {
                if (col.Undefined || Colors.IsStandard(col.Code)) continue;
                Player.Message(p, CmdCustomColors.FormatColor(col));
            }
        }
        
        static void PrintEmotes(Player p, string message) {
            char[] emotes = EmotesHandler.ControlCharReplacements.ToCharArray();
            emotes[0] = EmotesHandler.ExtendedCharReplacements[0]; // replace NULL with house
            
            string[] args = message.SplitSpaces(2);
            string modifier = args.Length > 1 ? args[1] : "";
            MultiPageOutput.Output(p, emotes, FormatEmote,
                                   "Help emotes", "emotes", modifier, true);
        }
        
        static string FormatEmote(char emote) {
            List<string> keywords = new List<string>();
            foreach (KeyValuePair<string, char> kvp in EmotesHandler.Keywords) {
                if (kvp.Value == emote) keywords.Add("(%S" + kvp.Key + ")");
            }
            return "&f" + emote + " %S- " + keywords.Join();
        }
        
        bool ParseCommand(Player p, string message) {
            string[] args = message.SplitSpaces(2);
            string cmdName = args[0], cmdArgs = "";
            Command.Search(ref cmdName, ref cmdArgs);
            
            Command cmd = Command.all.FindByName(cmdName);
            if (cmd == null) return false;
            
            if (args.Length == 1) {
                cmd.Help(p);
                Formatter.PrintCommandInfo(p, cmd);
            } else {
                cmd.Help(p, args[1]);
            }
            return true;
        }
        
        bool ParseBlock(Player p, string message) {
            BlockID b = Block.Parse(p, message);
            if (b == Block.Invalid) return false;
            
            Player.Message(p, "Block \"{0}\" appears as &b{1}",
                           message, Block.GetName(p, Block.Convert(b)));
            BlockPerms.List[b].MessageCannotUse(p, "use");         
            DescribePhysics(p, message, b);
            return true;
        }
        
        void DescribePhysics(Player p, string message, BlockID b) {
            BlockProps props = Player.IsSuper(p) ? Block.Props[b] : p.level.Props[b];
            
            if (props.IsDoor) {
                Player.Message(p, "Door can be used as an 'openable' block if physics are enabled, will automatically toggle back to closed after a few seconds. " +
                               "door_green toggles to red instead of air - also see, odoor and tdoor.");
            }
            if (props.oDoorBlock != Block.Invalid) {
                Player.Message(p, "Odoor behaves like a user togglable door, does not auto close. " +
                               "Needs to be opened with a normal /door of any type and touched by other physics blocks, such as air_door to work.");
            }
            if (props.IsTDoor) {
                Player.Message(p, "Tdoor behaves like a regular /door, but allows physics blocks, e.g. active_water to flow through when opened.");
            }
            if (b == Block.Door_AirActivatable) {
                Player.Message(p, "Air_switch can be placed in front of doors to act as an automatic door opener when the player walks into the air_switch block.");
            }
            if (b == Block.Fire || b == Block.LavaFire) {
                Player.Message(p, "Fire blocks burn through wood and temporarily leaves coal and obsidian behind.");
            }
            if (b == Block.Deadly_Air) {
                Player.Message(p, "Nerve gas is an invisible, killer, static block.");
            }
            if (b == Block.Train) {
                Player.Message(p, "Place a train on {0} wool and it will move with physics on. Can ride with /ride.", Block.GetName(p, Block.Red));
            }
            if (b == Block.Snake || b == Block.SnakeTail) {
                Player.Message(p, "Snake crawls along the ground and kills players it touches if physics are on.");
            }
            if (b == Block.ZombieBody) {
                Player.Message(p, "Place a zombie on the map. Moves with physics and kills players on touch"); 
            }
            if (b == Block.Creeper) {
                Player.Message(p, "Place a creeper on the map. Moves with physics and kills players on touch, also explodes like tnt.");
            }
            if (b == Block.Fireworks) {
                Player.Message(p, "Place a firework. Left click to send a firework into the sky, which explodes into different colored wool.");
            }
            if (b == Block.RocketStart) {
                Player.Message(p, "Place a rocket starter. Left click to fire, explodes like tnt.");
            }
            if (b == Block.FiniteFaucet) {
                Player.Message(p, "Place a faucet block which spews out and places water on the map a few blocks at a time.");
            }
            if (b == Block.WaterFaucet || b == Block.LavaFaucet) {
                Player.Message(p, "Place a faucet block which water/lava will come out of. Works like waterfall/lavafall but water/lava disappears and is redropped periodically.");
            }
            if (b == Block.WaterDown || b == Block.LavaDown) {
                Player.Message(p, "Waterfall and lavafall flow straight down, catch them at the bottom, or they will flood the map like regular active_water/lava.");
            }
            if (b == Block.FiniteWater || b == Block.FiniteLava) {
                Player.Message(p, "Finite water and lava flow like active_water/lava, but never create more blocks than you place.");
            }
            if (b == Block.Deadly_Water || b == Block.Deadly_Lava) {
                Player.Message(p, "Hot lava and cold water are nonmoving killer blocks which kill players on touch.");
            }
            if (b == Block.Water || b == Block.Geyser || b == Block.Deadly_ActiveWater) {
                Player.Message(p, "Active_water flows horizontally through the map, active_cold_water and geyser kill players, geyser flows upwards.");
            }
            if (b == Block.Lava || b == Block.Magma || b == Block.Deadly_ActiveLava || b == Block.FastLava || b == Block.Deadly_FastLava) {
                Player.Message(p, "Active_lava and its fast counterparts flow horizontally through the map, active_hot_lava and magma kill players, " +
                               "magma flows upwards slowly if it is placed in a spot where it cannot flow then broken out.");
            }
            
            AnimalAI ai = props.AnimalAI;
            if (ai == AnimalAI.KillerAir || ai == AnimalAI.Fly || ai == AnimalAI.FleeAir) {
                Player.Message(p, "The bird blocks are different colored blocks that fly through the air if physics is on. Killer_phoenix kills players it touches");
            }
            if (ai == AnimalAI.FleeLava || ai == AnimalAI.FleeWater || ai == AnimalAI.KillerLava || ai == AnimalAI.KillerWater) {
                Player.Message(p, "The fish blocks are different colored blocks that swim around in active_water (lava_shark in active_lava), " +
                               "sharks and lava sharks eat players they touch.");
            }
        }
        
        bool ParsePlugin(Player p, string message) {
            foreach (Plugin plugin in Plugin.all) {
                if (plugin.name.CaselessEq(message)) {
                    plugin.Help(p); return true;
                }
            }
            return false;
        }

        internal static string GetColor(Command cmd) {
            LevelPermission perm = CommandPerms.MinPerm(cmd);
            return Group.GetColor(perm);
        }

        public override void Help(Player p) {
            Player.Message(p, "...really? Wow. Just...wow.");
        }
    }
}
