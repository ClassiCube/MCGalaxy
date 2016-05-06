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
            get { return new[] { new CommandAlias("cmdlist", "commands") }; }
        }
        public CmdCommands() { }

        public override void Use(Player p, string message) {
            if (!DoCommand(p, message)) Help(p);
        }
        
        internal static bool DoCommand(Player p, string message) {
            switch (message.ToLower()) {
                case "build":
                case "building":
                    PrintHelpForGroup(p, "build", "Building"); break;
                case "chat":
                    PrintHelpForGroup(p, "chat", "Chat"); break;
                case "eco":
                case "economy":
                    PrintHelpForGroup(p, "eco", "Economy"); break;
                case "mod":
                case "moderation":
                    PrintHelpForGroup(p, "mod", "Moderation"); break;
                case "info":
                case "information":
                    PrintHelpForGroup(p, "info", "Information"); break;
                case "game":
                case "games":
                    PrintHelpForGroup(p, "game", "Game"); break;
                case "other":
                case "others":
                    PrintHelpForGroup(p, "other", "Other");  break;
                case "maps":
                case "world":
                    PrintHelpForGroup(p, "world", "World"); break;
                case "short":
                case "shortcut":
                case "shortcuts":
                case "short 1":
                case "shortcut 1":
                case "shortcuts 1":
                case "short 2":
                case "shortcut 2":
                case "shortcuts 2":
                    PrintShortcuts(p, message); break;
                case "old":
                case "oldmenu":
                case "commands":
                case "command":
                case "":
                    Group pGroup = p != null ? p.group : Group.findPerm(LevelPermission.Nobody);
                    PrintRankCommands(p, pGroup, true); break;
                case "commandsall":
                case "commandall":
                case "all":
                    PrintAllCommands(p); break;
                default:
                    return false;
            }
            return true;
        }
        
        static void PrintShortcuts(Player p, string message) {
            bool list1 = message[message.Length - 1] != '2';
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
        
        static void PrintRankCommands(Player p, Group group, bool colors) {
            StringBuilder cmds = new StringBuilder();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (!group.CanExecute(c) || disabled != null) continue;
                if (c.name == null) continue;
                
                if (!colors) cmds.Append(", ").Append(c.name);
                else cmds.Append(", ").Append(CmdHelp.GetColor(c)).Append(c.name);
            }
            
            Player.Message(p, "Available commands:");
            Player.Message(p, cmds.ToString(2, cmds.Length - 2));
            Player.Message(p, "Type %T/help <command> %Sfor more help on a command.");
            Player.Message(p, "Type %T/cmds shortcuts %Sfor a list of command shortcuts.");
            Player.Message(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
        }
        
        static void PrintAllCommands(Player p) {
            StringBuilder cmds = new StringBuilder();
            foreach (Command c in Command.all.commands) {
                if (c.name == null) continue;
                cmds.Append(", ").Append(CmdHelp.GetColor(c)).Append(c.name);
            }

            Player.Message(p, "All commands:");
            Player.Message(p, cmds.ToString(2, cmds.Length - 2));
            Player.Message(p, "Type \"/help <command>\" for more help.");
            Player.Message(p, "Type \"/help shortcuts\" for shortcuts.");
            Player.Message(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
        }
        
        static void PrintHelpForGroup(Player p, string typeName, string typeTitle) {
            StringBuilder cmds = new StringBuilder();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (p == null || p.group.CanExecute(c) && disabled == null) {
                    if (!c.type.Contains(typeName) || c.name == null) continue;
                    cmds.Append(", ").Append(CmdHelp.GetColor(c)).Append(c.name);
                }
            }
            
            if (cmds.Length == 0) {
                Player.Message(p, "No commands of this type are available to you.");
            } else {
                Player.Message(p, typeTitle + " commands you may use:");
                Player.Message(p, cmds.ToString(2, cmds.Length - 2) + ".");
            }
        }

        public override void Help(Player p) {
        	Player.Message(p, "%T/commands [category]");
        	Player.Message(p, "%H\"all\" category will output all commands.");
        	Player.Message(p, "%HNo category outputs commands you can use.");
            Player.Message(p, "%HOther Categories:");
            Player.Message(p, "  &aBuilding Chat Economy Games Info Moderation Other World");
        }
    }
}
