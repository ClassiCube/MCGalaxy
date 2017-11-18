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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdHelp : Command {
        public override string name { get { return "Help"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
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
                if (grp.Permission >= LevelPermission.Nobody) continue; // Note that -1 means max undo.  Undo anything and everything.
                int count = grp.Players.Count;
                Player.Message(p, "{0} %S- Cmd: {2}, Undo: {3}, Perm: {4}",
                               grp.ColoredName, count, grp.DrawLimit,
                               grp.MaxUndo == -1 ? "max" : grp.MaxUndo.ToString(), (int)grp.Permission);
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
            byte b = Block.Byte(message);
            if (b == Block.Invalid) return false;
            
            //give more useful help messages for doors and other physics blocks and killer blocks
            switch (message.ToLower())
            {
                case "door":
                    Player.Message(p, "Door can be used as an 'openable' block if physics are enabled, will automatically toggle back to closed after a few seconds. door_green toggles to red instead of air - also see, odoor and tdoor."); break;
                case "odoor":
                    Player.Message(p, "Odoor behaves like a user togglable door, does not auto close.  Needs to be opened with a normal /door of any type and touched by other physics blocks, such as air_door to work."); break;
                case "tdoor":
                    Player.Message(p, "Tdoor behaves like a regular /door, but allows physics blocks, e.g. active_water to flow through when opened."); break;
                case "air_switch":
                    Player.Message(p, "Air_switch can be placed in front of doors to act as an automatic door opener when the player walks into the air_switch block."); break;
                case "fire":
                    Player.Message(p, "Fire blocks burn through wood and temporarily leaves coal and obsidian behind."); break;
                case "nerve_gas":
                    Player.Message(p, "Nerve gas is an invisible, killer, static block."); break;
                case "train":
                    Player.Message(p, "Place a train on red wool and it will move with physics on. Can ride with /ride."); break;
                case "snake":
                    Player.Message(p, "Snake crawls along the ground and kills players it touches if physics are on."); break;
                case "zombie":
                    Player.Message(p, "Place a zombie on the map. Moves with physics and kills players on touch"); break;
                case "creeper":
                    Player.Message(p, "Place a creeper on the map. Moves with physics and kills players on touch, also explodes like tnt."); break;
                case "firework":
                    Player.Message(p, "Place a firework. Left click to send a firework into the sky, which explodes into different colored wool."); break;
                case "rocketstart":
                    Player.Message(p, "Place a rocket starter. Left click to fire, explodes like tnt."); break;
                case "finite_faucet":
                    Player.Message(p, "Place a faucet block which spews out and places water on the map a few blocks at a time."); break;
                case "water_faucet":
                case "lava_faucet":
                    Player.Message(p, "Place a faucet block, which water, or lava will come out of.  Works like water/lavafall but water/lava disappears and is redropped periodically."); break;
                case "waterfall":
                case "lavafall":
                    Player.Message(p, "Waterfall and lavafall flow straight down, catch them at the bottom, or they will flood the map like regular active_water/lava."); break;
                case "finite_water":
                case "finite_lava":
                    Player.Message(p, "Finite water and lava flow like active_water/lava, but never create more blocks than you place."); break;
                case "hot_lava":
                case "cold_water":
                    Player.Message(p, "Hot lava and cold water are nonmoving killer blocks which kill players on touch."); break;
                case "active_water":
                case "acw":
                case "geyser":
                case "active_cold_water":
                    Player.Message(p, "Active_water flows horizontally through the map, active_cold_water and geyser kill players, geyser flows upwards."); break;
                case "active_lava":
                case "ahl":
                case "magma":
                case "active_hot_lava":
                case "fast_hot_lava":
                case "lava_fast":
                    Player.Message(p, "Active_lava and its fast counterparts flow horizontally through the map, active_hot_lava and magma kill players, magma flows upwards slowly if it is placed in a spot where it cannot flow then broken out."); break;
                case "shark":
                case "lava_shark":
                case "goldfish":
                case "sea_sponge":
                case "salmon":
                case "betta_fish":
                    Player.Message(p, "The fish blocks are different colored blocks that swim around in active_water (lava_shark in active_lava), sharks and lava sharks eat players they touch."); break;
                case "phoenix":
                case "killer_phoenix":
                case "dove":
                case "blue_bird":
                case "red_robin":
                case "pidgeon":
                case "duck":
                    Player.Message(p, "The bird blocks are different colored blocks that fly through the air if physics is on. Killer_phoenix kills players it touches"); break;
                default:
                    Player.Message(p, "Block \"" + message + "\" appears as &b" + Block.Name(Block.Convert(b))); break;
            }
            
            BlockPerms.List[b].MessageCannotUse(p, "use");
            return true;
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
