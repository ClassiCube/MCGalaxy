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

namespace MCGalaxy.Commands
{
    public sealed class CmdHelp : Command
    {
        public override string name { get { return "help"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cmdhelp"), new CommandAlias("ranks", "ranks"),
                    new CommandAlias("cmdlist", "commands"), new CommandAlias("commands", "commands") }; }
        }
        public CmdHelp() { }

        public override void Use(Player p, string message)
        {
            switch (message.ToLower())
            {
                case "":
                    Player.SendMessage(p, "Command Categories:");
                    Player.SendMessage(p, "  %aBuilding Chat Economy Games Info Moderation Other World");
                    Player.SendMessage(p, "Other Categories:");
                    Player.SendMessage(p, "  %aRanks Colors Shortcuts Commands");
                    Player.SendMessage(p, "To view help for a category, type %T/help CategoryName");
                    Player.SendMessage(p, "To see detailed help for a command, type %T/help CommandName");
                    Player.SendMessage(p, "To see your stats, type %T/whois");
                    Player.SendMessage(p, "To see loaded maps, type %T/maps");
                    Player.SendMessage(p, "To view your personal world options, use %T/OS");
                    Player.SendMessage(p, "To join a map, type %T/goto WorldName");
                    Player.SendMessage(p, "To send private messages, type %T@PlayerName Message");
                    break;
                case "ranks":
                    message = "";
                    foreach (Group grp in Group.GroupList)
                    {
                        if (grp.Permission < LevelPermission.Nobody) // Note that -1 means max undo.  Undo anything and everything.
                            Player.SendMessage(p, grp.color + grp.name + " - &bCmd: " + grp.maxBlocks + " - &2Undo: " + ((grp.maxUndo != -1) ? grp.maxUndo.ToString() : "max") + " - &cPerm: " + (int)grp.Permission);
                    }
                    break;
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
                case "colours":
                case "colors":
                    Player.SendMessage(p, "&fTo use a color, put a '%' and then put the color code.");
                    Player.SendMessage(p, "Colors Available:");
                    Player.SendMessage(p, "0 - &0Black %S| 1 - &1Navy %S| 2 - &2Green %S| 3 - &3Teal");
                    Player.SendMessage(p, "4 - &4Maroon %S| 5 - &5Purple %S| 6 - &6Gold %S| 7 - &7Silver");
                    Player.SendMessage(p, "8 - &8Gray %S| 9 - &9Blue %S| a - &aLime %S| b - &bAqua");
                    Player.SendMessage(p, "c - &cRed %S| d - &dPink %S| e - &eYellow %S| f - &fWhite");
                    CmdCustomColors.ListHandler(p, null, true);
                    break;
                case "old":
                case "oldmenu":
                case "commands":
                case "command":
                    Group pGroup = p != null ? p.group : Group.findPerm(LevelPermission.Nobody);
                    PrintRankCommands(p, pGroup, true); break;
                case "commandsall":
                case "commandall":
                    PrintAllCommands(p); break;
                default:
                    if (ParseCommand(p, message) || ParseBlock(p, message) || ParsePlugin(p, message)) return;
                    Player.SendMessage(p, "Could not find command, plugin or block specified.");
                    break;
            }
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
                Player.SendMessage(p, "Available shortcuts (1):");
                Player.SendMessage(p, cmds.ToString(2, cmds.Length - 2));
                Player.SendMessage(p, "%bType %f/help shortcuts 2%b to view the rest of the list ");
            } else {
                Player.SendMessage(p, "Available shortcuts (2):");
                Player.SendMessage(p, cmds.ToString(2, cmds.Length - 2));
                Player.SendMessage(p, "%bType %f/help shortcuts 1%b to view the rest of the list ");
            }
        }
        
        static void PrintRankCommands(Player p, Group group, bool colors) {
            StringBuilder cmds = new StringBuilder();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (!group.CanExecute(c) || disabled != null) continue;
                if (c.name == null) continue;
                
                if (!colors) cmds.Append(", ").Append(c.name);
                else cmds.Append(", ").Append(GetColor(c.name)).Append(c.name);
            }
            
            Player.SendMessage(p, "Available commands:");
            Player.SendMessage(p, cmds.ToString(2, cmds.Length - 2));
            Player.SendMessage(p, "Type \"/help <command>\" for more help.");
            Player.SendMessage(p, "Type \"/help shortcuts\" for shortcuts.");
            Player.SendMessage(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
        }
        
        static void PrintAllCommands(Player p) {
            StringBuilder cmds = new StringBuilder();
            foreach (Command c in Command.all.commands) {
                if (c.name == null) continue;
                cmds.Append(", ").Append(GetColor(c.name)).Append(c.name);
            }

            Player.SendMessage(p, "All commands:");
            Player.SendMessage(p, cmds.ToString(2, cmds.Length - 2));
            Player.SendMessage(p, "Type \"/help <command>\" for more help.");
            Player.SendMessage(p, "Type \"/help shortcuts\" for shortcuts.");
            Player.SendMessage(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
        }
        
        static void PrintHelpForGroup(Player p, string typeName, string typeTitle) {
            StringBuilder cmds = new StringBuilder();
            foreach (Command c in Command.all.commands) {
                string disabled = Command.GetDisabledReason(c.Enabled);
                if (p == null || p.group.CanExecute(c) && disabled == null) {
                    if (!c.type.Contains(typeName) || c.name == null) continue;
                    cmds.Append(", ").Append(GetColor(c.name)).Append(c.name);
                }
            }
            
            if (cmds.Length == 0) {
                Player.SendMessage(p, "No commands of this type are available to you.");
            } else {
                Player.SendMessage(p, typeTitle + " commands you may use:");
                Player.SendMessage(p, cmds.ToString(2, cmds.Length - 2) + ".");
            }
        }
        
        bool ParseCommand(Player p, string message) {
            Command cmd = Command.all.Find(message);
            if (cmd == null) return false;
            cmd.Help(p);            
            LevelPermission minPerm = GrpCommands.allowedCommands.Find(C => C.commandName == cmd.name).lowestRank;          
            Player.SendMessage(p, "Rank needed: " + GetColoredRank(minPerm));
            PrintAliases(p, cmd);
            
            CommandPerm[] perms = cmd.AdditionalPerms;
            if (perms == null) return true;
            
            Player.SendMessage(p, "%TAdditional permissions:");
            for (int i = 0; i < perms.Length; i++) {
                var addition = CommandOtherPerms.Find(cmd, i + 1);
                LevelPermission perm = (LevelPermission)addition.Permission;
                Player.SendMessage(p, GetColoredRank(perm) + " %S- " + addition.Description);
            }
            return true;
        }
        
        void PrintAliases(Player p, Command cmd) {
            StringBuilder dst = new StringBuilder("Shortcuts: ");
            if (!String.IsNullOrEmpty(cmd.shortcut)) {
                dst.Append('/').Append(cmd.shortcut).Append(", ");
            }
            FindAliases(Alias.coreAliases, cmd, dst);
            FindAliases(Alias.aliases, cmd, dst);
            
            if (dst.Length == "Shortcuts: ".Length) return;
            Player.SendMessage(p, dst.ToString(0, dst.Length - 2));
        }
        
        void FindAliases(List<Alias> aliases, Command cmd, StringBuilder dst) {
            foreach (Alias a in aliases) {
                if (!a.Target.CaselessEq(cmd.name)) continue;
                
                dst.Append('/').Append(a.Trigger);
                if (a.Args != null) {
                    string name = String.IsNullOrEmpty(cmd.shortcut) 
                        ? cmd.name : cmd.shortcut;
                    dst.Append(" for /").Append(name + " " + a.Args);
                }                
                dst.Append(", ");
            }
        }
        
        bool ParseBlock(Player p, string message) {
            byte b = Block.Byte(message);
            if (b == Block.Zero) return false;
            
            //give more useful help messages for doors and other physics blocks and killer blocks
            if (message == "door")
            {
                Player.SendMessage(p, "Door can be used as an 'openable' block if physics are enabled, will automatically toggle back to closed after a few seconds. door_green toggles to red instead of air - also see, odoor and tdoor");
                return true;
            }
            if (message == "odoor")
            {
                Player.SendMessage(p, "Odoor behaves like a user togglable door, does not auto close.  Needs to be opened with a normal /door of any type and touched by other physics blocks, such as air_door to work.");
                return true;
            }
            if (message == "tdoor")
            {
                Player.SendMessage(p, "Tdoor behaves like a regular /door, but allows physics blocks, e.g. active_water to flow through when opened");
                return true;
            }
            if (message == "active_water" || message == "acw" || message == "geyser" || message =="active_cold_water")
            {
                Player.SendMessage(p, "active_water flows horizontally through the map, active_cold_water and geyser kill players, geyser flows upwards.");
                return true;
            }
            if (message == "active_lava" || message == "ahl" || message == "magma" || message == "active_hot_lava")
            {
                Player.SendMessage(p, "active_lava flows horizontally through the map, active_hot_lava and magma kill players, magma flows upwards slowly if it is placed in a spot where it cannot flow then broken out.");
                return true;
            }
            if (message == "fire")
            {
                Player.SendMessage(p, "Fire blocks burn through wood and temporarily leaves coal and obsidian behind");
                return true;
            }
            if (message == "nerve_gas")
            {
                Player.SendMessage(p, "Nerve gas is an invisible, killer, static block");
                return true;
            }
            
            Player.SendMessage(p, "Block \"" + message + "\" appears as &b" + Block.Name(Block.Convert(b)));
            Group foundRank = Group.findPerm(Block.BlockList[b].lowestRank);
            Player.SendMessage(p, "Rank needed: " + foundRank.color + foundRank.name);
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

        static string GetColor(string cmd) {
            LevelPermission perm = GrpCommands.allowedCommands.Find(C => C.commandName == cmd).lowestRank;
            Group grp = Group.findPerm(perm);
            return grp == null ? "&f" : grp.color;
        }
        
        static string GetColoredRank(LevelPermission perm) {
            Group grp = Group.findPerm(perm);
            string col = grp == null ? "&f" : grp.color;
            return col + Level.PermissionToName(perm);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "...really? Wow. Just...wow.");
        }
    }
}
