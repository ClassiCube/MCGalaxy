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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdCommands : Command {       
        public override string name { get { return "Commands"; } }
        public override string shortcut { get { return "Cmds"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("CmdList") }; }
        }

        public override void Use(Player p, string message) {
            if (!ListCommands(p, message)) Help(p);
        }
        
        internal static bool ListCommands(Player p, string message) {
            string[] args = message.SplitSpaces();
            string sort = args.Length > 1 ? args[1].ToLower() : "";
            string modifier = args.Length > 2 ? args[2] : sort;
            
            // if user only provided name/names/rank/ranks, don't treat that as the modifier
            if (args.Length == 2) {
                if (modifier == "name" || modifier == "names" || modifier == "rank" || modifier == "ranks") {
                    modifier = "";
                } else {
                    sort = "";
                }
            }

            switch (args[0].ToLower()) {
                case "build":
                case "building":
                    PrintHelpForGroup(p, sort, modifier, "build", "Building"); break;
                case "chat":
                    PrintHelpForGroup(p, sort, modifier, "chat", "Chat"); break;
                case "eco":
                case "economy":
                    PrintHelpForGroup(p, sort, modifier, "eco", "Economy"); break;
                case "mod":
                case "moderation":
                    PrintHelpForGroup(p, sort, modifier, "mod", "Moderation"); break;
                case "info":
                case "information":
                    PrintHelpForGroup(p, sort, modifier, "info", "Information"); break;
                case "game":
                case "games":
                    PrintHelpForGroup(p, sort, modifier, "game", "Game"); break;
                case "other":
                case "others":
                    PrintHelpForGroup(p, sort, modifier, "other", "Other");  break;
                case "maps":
                case "world":
                    PrintHelpForGroup(p, sort, modifier, "world", "World"); break;
                case "short":
                case "shortcut":
                case "shortcuts":
                    PrintShortcuts(p, sort); break;
                case "old":
                case "oldmenu":
                case "command":
                case "":
                    Group pGroup = p != null ? p.group : Group.NobodyRank;
                    PrintRankCommands(p, sort, modifier, pGroup, true); break;
                case "commandsall":
                case "commandall":
                case "all":
                    PrintAllCommands(p, sort, modifier); break;
                default:
                    Group grp = Group.Find(args[0]);
                    if (grp == null) return false;
                    PrintRankCommands(p, sort, modifier, grp, false); break;
            }
            return true;
        }
        
        static void PrintShortcuts(Player p, string modifier) {
            List<Command> shortcuts = new List<Command>();
            foreach (Command cmd in Command.all.commands) {
                if (cmd.shortcut.Length == 0) continue;
                if (p != null && !p.group.CanExecute(cmd)) continue;
                shortcuts.Add(cmd);
            }
            
            MultiPageOutput.Output(p, shortcuts,
                                   (cmd) => "&b" + cmd.shortcut + " %S[" + cmd.name + "]",
                                   "Commands shortcuts", "shortcuts", modifier, false);
        }
        
        static void PrintRankCommands(Player p, string sort, string modifier, Group group, bool own) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (!group.CanExecute(c) || disabled != null || c.name == null) continue;
                cmds.Add(c);
            }   
            
            if (cmds.Count == 0) {
                Player.Message(p, "{0} %Scannot use any commands.", group.ColoredName); return;
            }            
            SortCommands(cmds, sort);            
            if (own)
                Player.Message(p, "Available commands:");
            else
                Player.Message(p, "Commands available to " + group.ColoredName + " %Srank:");
            
            string type = "Cmds " + group.Name;
            if (sort.Length > 0) type += " " + sort;
            MultiPageOutput.Output(p, cmds,
                                   (cmd) => CmdHelp.GetColor(cmd) + cmd.name,
                                   type, "commands", modifier, false);
            Player.Message(p, "Type %T/Help <command> %Sfor more help on a command.");
        }
        
        static void PrintAllCommands(Player p, string sort, string modifier) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.all.commands) {
                if (c.name == null) continue;
                cmds.Add(c);
            }

            SortCommands(cmds, sort);
            Player.Message(p, "All commands:");
            
            string type = "Commands all";
            if (sort.Length > 0) type += " " + sort;
            MultiPageOutput.Output(p, cmds,
                                   (cmd) => CmdHelp.GetColor(cmd) + cmd.name,
                                   type, "commands", modifier, false);            
            Player.Message(p, "Type %T/Help <command> %Sfor more help on a command.");
        }
        
        static void PrintHelpForGroup(Player p, string sort, string modifier,
                                      string type, string category) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if ((p == null || p.group.CanExecute(c)) && disabled == null) {
                    if (!c.type.CaselessContains(type) || c.name == null) continue;
                    cmds.Add(c);
                }
            }    
            
            if (cmds.Count == 0) {
                Player.Message(p, "You cannot use any of the " + category + " commands."); return;
            }            
            SortCommands(cmds, sort);
            Player.Message(p, category + " commands you may use:");

            type = "Commands " + category;
            if (sort.Length > 0) type += " " + sort;
            MultiPageOutput.Output(p, cmds,
                                   (cmd) => CmdHelp.GetColor(cmd) + cmd.name,
                                   type, "commands", modifier, false);
            Player.Message(p, "Type %T/Help <command> %Sfor more help on a command.");
        }
        
        static void SortCommands(List<Command> cmds, string sort) {
            if (sort == "name" || sort == "names") {
                cmds.Sort((a, b) => a.name
                          .CompareTo(b.name));
            }
            if (sort == "rank" || sort == "ranks") {
                cmds.Sort((a, b) => CommandPerms.MinPerm(a)
                          .CompareTo(CommandPerms.MinPerm(b)));
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Commands [category] <sort>");
            Player.Message(p, "%HIf no category is given, outputs all commands you can use.");
            Player.Message(p, "  %H\"shortcuts\" category outputs all command shortcuts.");
            Player.Message(p, "  %H\"all\" category outputs all commands.");
            Player.Message(p, "  %HIf category is a rank name, outputs all commands that rank can use.");
            Player.Message(p, "%HOther command categories:");
            Player.Message(p, "  %HBuilding Chat Economy Games Info Moderation Other World");
            Player.Message(p, "%HSort is optional, and can be either \"name\" or \"rank\"");
        }
    }
}
