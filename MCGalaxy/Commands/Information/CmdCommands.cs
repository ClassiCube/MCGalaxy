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
    public sealed class CmdCommands : Command2 {       
        public override string name { get { return "Commands"; } }
        public override string shortcut { get { return "Cmds"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("CmdList") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
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

            string type = args[0].ToLower();
            if (type == "short" || type == "shortcut" || type == "shortcuts") {
                 PrintShortcuts(p, sort);
            } else if (type == "old" || type == "oldmenu" || type == "" || type == "command") {
                 PrintRankCommands(p, sort, modifier, p.group, true);
            } else if (type == "all" || type == "commandall" || type == "commandsall") {
                 PrintAllCommands(p, sort, modifier);
            } else {
                bool any = PrintCategoryCommands(p, sort, modifier, type);
                if (any) return true;
                
                // list commands a rank can use                
                Group grp = Group.Find(type);
                if (grp == null) return false;
                PrintRankCommands(p, sort, modifier, grp, false);
            }
            return true;
        }
        
        static void PrintShortcuts(Player p, string modifier) {
            List<Command> shortcuts = new List<Command>();
            foreach (Command cmd in Command.allCmds) {
                if (cmd.shortcut.Length == 0) continue;
                if (!p.CanUse(cmd)) continue;
                shortcuts.Add(cmd);
            }
            
            MultiPageOutput.Output(p, shortcuts,
                                   (cmd) => "&b" + cmd.shortcut + " &S[" + cmd.name + "]",
                                   "Commands shortcuts", "shortcuts", modifier, false);
        }
        
        static void PrintRankCommands(Player p, string sort, string modifier, Group group, bool own) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.allCmds) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (disabled == null && group.Commands.Contains(c)) cmds.Add(c);
            }   
            
            if (cmds.Count == 0) {
                p.Message("{0} &Scannot use any commands.", group.ColoredName); return;
            }            
            SortCommands(cmds, sort);            
            if (own)
                p.Message("Available commands:");
            else
                p.Message("Commands available to " + group.ColoredName + " &Srank:");
            
            string type = "Cmds " + group.Name;
            if (sort.Length > 0) type += " " + sort;
            MultiPageOutput.Output(p, cmds,
                                   (cmd) => CmdHelp.GetColor(cmd) + cmd.name,
                                   type, "commands", modifier, false);
            p.Message("Type &T/Help <command> &Sfor more help on a command.");
        }
        
        static void PrintAllCommands(Player p, string sort, string modifier) {
            List<Command> cmds = Command.CopyAll();
            SortCommands(cmds, sort);
            p.Message("All commands:");
            
            string type = "Commands all";
            if (sort.Length > 0) type += " " + sort;
            MultiPageOutput.Output(p, cmds,
                                   (cmd) => CmdHelp.GetColor(cmd) + cmd.name,
                                   type, "commands", modifier, false);            
            p.Message("Type &T/Help <command> &Sfor more help on a command.");
        }
        
        // common shortcuts people tend to use 
        static string GetCategory(string type) {
            if (type.CaselessEq("building"))   return CommandTypes.Building;
            if (type.CaselessEq("eco"))        return CommandTypes.Economy;
            if (type.CaselessEq("games"))      return CommandTypes.Games;
            if (type.CaselessEq("info"))       return CommandTypes.Information;
            if (type.CaselessEq("moderation")) return CommandTypes.Moderation;
            if (type.CaselessEq("others"))     return CommandTypes.Other;
            if (type.CaselessEq("maps"))       return CommandTypes.World;
            return type;
        }
        
        static bool PrintCategoryCommands(Player p, string sort, string modifier, string type) {
            List<Command> cmds = new List<Command>();
            string category = GetCategory(type);
            bool foundAny = false;

            foreach (Command c in Command.allCmds) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (!c.type.CaselessEq(category)) continue;
                
                if (disabled == null && p.CanUse(c)) cmds.Add(c);
                foundAny = true;
            }
            if (!foundAny) return false;
            
            if (cmds.Count == 0) {
                p.Message("You cannot use any of the " + category + " commands."); return true;
            }            
            SortCommands(cmds, sort);
            p.Message(category.Capitalize() + " commands you may use:");

            type = "Commands " + type;
            if (sort.Length > 0) type += " " + sort;
            MultiPageOutput.Output(p, cmds,
                                   (cmd) => CmdHelp.GetColor(cmd) + cmd.name,
                                   type, "commands", modifier, false);
            
            p.Message("Type &T/Help <command> &Sfor more help on a command.");
            return true;
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
            p.Message("&T/Commands [category] <sort>");
            p.Message("&HIf no category is given, outputs all commands you can use.");
            p.Message("  &H\"shortcuts\" category outputs all command shortcuts.");
            p.Message("  &H\"all\" category outputs all commands.");
            p.Message("  &HIf category is a rank name, outputs all commands that rank can use.");
            p.Message("&HOther command categories:");
            p.Message("  &HBuilding Chat Economy Games Info Moderation Other World");
            p.Message("&HSort is optional, and can be either \"name\" or \"rank\"");
        }
    }
}
