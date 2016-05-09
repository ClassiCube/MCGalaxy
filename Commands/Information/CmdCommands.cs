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

namespace MCGalaxy.Commands {
    public sealed class CmdCommands : Command {
        
        public override string name { get { return "commands"; } }
        public override string shortcut { get { return "cmds"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cmdlist") }; }
        }
        public CmdCommands() { }

        public override void Use(Player p, string message) {
            if (!DoCommand(p, message)) Help(p);
        }
        
        internal static bool DoCommand(Player p, string message) {
            string[] args = message.Split(' ');
            string sort = args.Length > 1 ? args[1].ToLower() : null;
            
            switch (args[0].ToLower()) {
                case "build":
                case "building":
                    PrintHelpForGroup(p, sort, "build", "Building"); break;
                case "chat":
                    PrintHelpForGroup(p, sort, "chat", "Chat"); break;
                case "eco":
                case "economy":
                    PrintHelpForGroup(p, sort, "eco", "Economy"); break;
                case "mod":
                case "moderation":
                    PrintHelpForGroup(p, sort, "mod", "Moderation"); break;
                case "info":
                case "information":
                    PrintHelpForGroup(p, sort, "info", "Information"); break;
                case "game":
                case "games":
                    PrintHelpForGroup(p, sort, "game", "Game"); break;
                case "other":
                case "others":
                    PrintHelpForGroup(p, sort, "other", "Other");  break;
                case "maps":
                case "world":
                    PrintHelpForGroup(p, sort, "world", "World"); break;
                case "short":
                case "shortcut":
                case "shortcuts":
                    PrintShortcuts(p, sort); break;
                case "old":
                case "oldmenu":
                case "command":
                case "":
                    Group pGroup = p != null ? p.group : Group.findPerm(LevelPermission.Nobody);
                    PrintRankCommands(p, sort, pGroup, true); break;
                case "commandsall":
                case "commandall":
                case "all":
                    PrintAllCommands(p, sort); break;
                default:
                    Group grp = Group.Find(args[0]);
                    if (grp == null) return false;
                    PrintRankCommands(p, sort, grp, false); break;
            }
            return true;
        }
        
        static void PrintShortcuts(Player p, string sort) {
            bool list1 = sort == null || sort != "2";
            List<string> shortcuts = new List<string>();
            foreach (Command c in Command.all.commands) {
                if (p != null && !p.group.CanExecute(c) || c.shortcut == "") continue;
                shortcuts.Add(c.shortcut + " %S[" + c.name + "]");
            }
            
            int top = list1 ? shortcuts.Count / 2 : shortcuts.Count;
            StringBuilder cmds = new StringBuilder();
            for (int i = list1 ? 0 : shortcuts.Count / 2; i < top; i++)
                cmds.Append(", &b").Append(shortcuts[i]);
            
            if (list1) {
                Player.Message(p, "Available shortcuts (1):");
                Player.Message(p, cmds.ToString(2, cmds.Length - 2));
                Player.Message(p, "Type %T/cmds shortcuts 2%S to view the rest of the list ");
            } else {
                Player.Message(p, "Available shortcuts (2):");
                Player.Message(p, cmds.ToString(2, cmds.Length - 2));
                Player.Message(p, "Type %T/cmds shortcuts 1%S to view the rest of the list ");
            }
        }
        
        static void PrintRankCommands(Player p, string sort, Group group, bool own) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (!group.CanExecute(c) || disabled != null || c.name == null) continue;
                cmds.Add(c);
            }
            
            StringBuilder list = FormatCommands(cmds, sort);
            if (own)
                Player.Message(p, "Available commands:");
            else
                Player.Message(p, "Commands available to " + group.ColoredName + " %Srank:");
            
            Player.Message(p, list.ToString(2, list.Length - 2));
            Player.Message(p, "Type %T/help <command> %Sfor more help on a command.");
            Player.Message(p, "Type %T/cmds shortcuts %Sfor a list of command shortcuts.");
            Player.Message(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
        }
        
        static void PrintAllCommands(Player p, string sort) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.all.commands) {
                if (c.name == null) continue;
                cmds.Add(c);
            }

            StringBuilder list = FormatCommands(cmds, sort);
            Player.Message(p, "All commands:");
            Player.Message(p, list.ToString(2, list.Length - 2));
            Player.Message(p, "Type %T/help <command> %Sfor more help on a command.");
            Player.Message(p, "Type %T/cmds shortcuts %Sfor a list of command shortcuts.");
            Player.Message(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
        }
        
        static void PrintHelpForGroup(Player p, string sort, 
                                      string typeName, string typeTitle) {
            List<Command> cmds = new List<Command>();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (p == null || p.group.CanExecute(c) && disabled == null) {
                    if (!c.type.Contains(typeName) || c.name == null) continue;
                    cmds.Add(c);
                }
            }
            
            StringBuilder list = FormatCommands(cmds, sort);
            if (list.Length == 0) {
                Player.Message(p, "You cannot use any of the " + typeTitle + " commands.");
            } else {
                Player.Message(p, typeTitle + " commands you may use:");
                Player.Message(p, list.ToString(2, list.Length - 2) + ".");
            }
        }
        
        static StringBuilder FormatCommands(List<Command> cmds, string sort) {
            if (sort != null && (sort == "name" || sort == "names")) {
                cmds.Sort((a, b) => a.name
                          .CompareTo(b.name));
            }
            if (sort != null && (sort == "rank" || sort == "ranks")) {
                cmds.Sort((a, b) => GrpCommands.MinPerm(a)
                          .CompareTo(GrpCommands.MinPerm(b)));
            }
            
            StringBuilder list = new StringBuilder();
            foreach (Command c in cmds)
                list.Append(", ").Append(CmdHelp.GetColor(c)).Append(c.name);
            return list;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/commands [category] [sort]");
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
