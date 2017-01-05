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
using System.Text;
using MCGalaxy.Commands.CPE;

namespace MCGalaxy.Commands {
    public sealed class CmdHelp : Command {
        public override string name { get { return "help"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cmdhelp"), new CommandAlias("ranks", "ranks"),
                    new CommandAlias("colors", "colors") }; }
        }
        public CmdHelp() { }

        public override void Use(Player p, string message) {
            switch (message.ToLower()) {
                case "":
                    Player.Message(p, "Command Categories:");
                    Player.Message(p, "  %aBuilding Chat Economy Games Info Moderation Other World");
                    Player.Message(p, "Other Categories:");
                    Player.Message(p, "  %aRanks Colors Shortcuts Commands");
                    Player.Message(p, "To view help for a category, type %T/help CategoryName");
                    Player.Message(p, "To see detailed help for a command, type %T/help CommandName");
                    Player.Message(p, "To see your stats, type %T/whois");
                    Player.Message(p, "To see loaded maps, type %T/maps");
                    Player.Message(p, "To view your personal world options, use %T/OS");
                    Player.Message(p, "To join a map, type %T/goto WorldName");
                    Player.Message(p, "To send private messages, type %T@PlayerName Message");
                    break;
                case "ranks":
                    PrintRanks(p); break;
                case "colours":
                case "colors":
                    Player.Message(p, "&fTo use a color, put a '%' and then put the color code.");
                    Player.Message(p, "Colors Available:");
                    Player.Message(p, "0 - &0Black %S| 1 - &1Navy %S| 2 - &2Green %S| 3 - &3Teal");
                    Player.Message(p, "4 - &4Maroon %S| 5 - &5Purple %S| 6 - &6Gold %S| 7 - &7Silver");
                    Player.Message(p, "8 - &8Gray %S| 9 - &9Blue %S| a - &aLime %S| b - &bAqua");
                    Player.Message(p, "c - &cRed %S| d - &dPink %S| e - &eYellow %S| f - &fWhite");
                    CmdCustomColors.ListHandler(p, null, true);
                    break;
                default:
                    if (CmdCommands.DoCommand(p, message)) break;
                    if (ParseCommand(p, message) || ParseBlock(p, message) || ParsePlugin(p, message)) return;
                    Player.Message(p, "Could not find command, plugin or block specified.");
                    break;
            }
        }
        
        static void PrintRanks(Player p) {
            foreach (Group grp in Group.GroupList) {
                if (grp.Permission >= LevelPermission.Nobody) continue; // Note that -1 means max undo.  Undo anything and everything.
                int count = grp.playerList.Count;
                Player.Message(p, "{0} %S- Cmd: {2}, Undo: {3}, Perm: {4}", 
                               grp.ColoredName, count, grp.maxBlocks, 
                               grp.maxUndo == -1 ? "max" : grp.maxUndo.ToString(), (int)grp.Permission);
            }
        }
        
        bool ParseCommand(Player p, string message) {
            string[] args = message.SplitSpaces(2);
            Alias alias = Alias.Find(args[0].ToLower());
            if (alias != null) args[0] = alias.Target;
            
            Command cmd = Command.all.Find(args[0]);
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
            Formatter.MessageBlock(p, "use ", b);
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
            LevelPermission perm = GrpCommands.MinPerm(cmd);
            return Group.GetColor(perm);
        }

        public override void Help(Player p) {
            Player.Message(p, "...really? Wow. Just...wow.");
        }
    }
}
