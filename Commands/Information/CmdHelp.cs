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
namespace MCGalaxy.Commands
{
    public sealed class CmdHelp : Command
    {
        public override string name { get { return "help"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
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
                    PrintHelpForGroup(p, "build", "Building" ); break;
                case "chat":
                    PrintHelpForGroup(p, "chat", "Chat" ); break;
                case "eco":
                case "economy":
                    PrintHelpForGroup(p, "eco", "Economy" ); break;
                case "mod":
                case "moderation":
                    PrintHelpForGroup(p, "mod", "Moderation" ); break;
                case "info":
                case "information":
                    PrintHelpForGroup(p, "info", "Information" ); break;
                case "game":
                case "games":
                    PrintHelpForGroup(p, "game", "Game" ); break;
                case "other":
                case "others":
                    PrintHelpForGroup(p, "other", "Other" );  break;
                case "maps":
                case "world":
                    PrintHelpForGroup(p, "world", "World" ); break;
                case "short":
                case "shortcut":
                case "shortcuts":
                case "short 1":
                case "shortcut 1":
                case "shortcuts 1":
                case "short 2":
                case "shortcut 2":
                case "shortcuts 2":
                    bool list1 = message[message.Length - 1] != '2';
                    message = "";
                    List<string> shortcuts = new List<string>();
                    foreach (Command comm in Command.all.commands)
                        if (p == null || p.group.commands.All().Contains(comm))
                            if (comm.shortcut != "") shortcuts.Add(", &b" + comm.shortcut + " %S[" + comm.name + "]");
                    int top = list1 ? shortcuts.Count / 2 : shortcuts.Count;
                    for (int i = list1 ? 0 : shortcuts.Count / 2; i < top; i++)
                        message += shortcuts[i];
                    if (list1) {
                        Player.SendMessage(p, "Available shortcuts (1):");
                        Player.SendMessage(p, message.Remove(0, 2));
                        Player.SendMessage(p, "%bType %f/help shortcuts 2%b to view the rest of the list ");
                    } else {
                        Player.SendMessage(p, "Available shortcuts (2):");
                        Player.SendMessage(p, message.Remove(0, 2));
                        Player.SendMessage(p, "%bType %f/help shortcuts 1%b to view the rest of the list ");
                    }
                    break;
                case "colours":
                case "colors":
                    Player.SendMessage(p, "&fTo use a color simply put a '%' sign symbol before you put the color code.");
                    Player.SendMessage(p, "Colors Available:");
                    Player.SendMessage(p, "0 - &0Black %S| 8 - &8Gray");
                    Player.SendMessage(p, "1 - &1Navy %S| 9 - &9Blue");
                    Player.SendMessage(p, "2 - &2Green %S| a - &aLime");
                    Player.SendMessage(p, "3 - &3Teal %S| b - &bAqua");
                    Player.SendMessage(p, "4 - &4Maroon %S| c - &cRed");
                    Player.SendMessage(p, "5 - &5Purple %S| d - &dPink");
                    Player.SendMessage(p, "6 - &6Gold %S| e - &eYellow");
                    Player.SendMessage(p, "7 - &7Silver %S| f - &fWhite");
                    break;
                case "old":
                case "oldmenu":
                case "commands":
                case "command":
                    string commandsFound = "";
                    foreach (Command comm in Command.all.commands)
                        if (p == null || p.group.commands.All().Contains(comm))
                            try { commandsFound += ", " + comm.name; } catch { }
                    Player.SendMessage(p, "Available commands:");
                    Player.SendMessage(p, commandsFound.Remove(0, 2));
                    Player.SendMessage(p, "Type \"/help <command>\" for more help.");
                    Player.SendMessage(p, "Type \"/help shortcuts\" for shortcuts.");
                    Player.SendMessage(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
                    break;
                case "commandsall":
                case "commandall":
                    string commandsAllFound = "";
                    foreach (Command comm in Command.all.commands)
                        try { commandsAllFound += ", " + getColor(comm.name) + comm.name; } catch { }
                    Player.SendMessage(p, "All commands:");
                    Player.SendMessage(p, commandsAllFound.Remove(0, 2));
                    Player.SendMessage(p, "Type \"/help <command>\" for more help.");
                    Player.SendMessage(p, "Type \"/help shortcuts\" for shortcuts.");
                    Player.SendMessage(p, "%bIf you can't see all commands, type %f/help %band choose a help category.");
                    break;                   
                default:
                    Command cmd = Command.all.Find(message);
                    if (cmd != null)
                    {
                        cmd.Help(p);
                        string foundRank = Level.PermissionToName(GrpCommands.allowedCommands.Find(grpComm => grpComm.commandName == cmd.name).lowestRank);
                        Player.SendMessage(p, "Rank needed: " + getColor(cmd.name) + foundRank);
                        if (!String.IsNullOrEmpty(cmd.shortcut))
                            Player.SendMessage(p, "Shortcut: /" + cmd.shortcut);
                        return;
                    }
                    byte b = Block.Byte(message);
                    if (b != Block.Zero)
                    {
                        Player.SendMessage(p, "Block \"" + message + "\" appears as &b" + Block.Name(Block.Convert(b)));
                        Group foundRank = Group.findPerm(Block.BlockList.Find(bs => bs.type == b).lowestRank);
                        Player.SendMessage(p, "Rank needed: " + foundRank.color + foundRank.name);
                        return;
                    }
                    Plugin plugin = null;
                    foreach (Plugin p1 in Plugin.all)
                    {
                        if (p1.name.ToLower() == message.ToLower())
                        {
                            plugin = p1;
                            break;
                        }
                    }
                    if (plugin != null)
                    {
                        plugin.Help(p);
                    }
                    Player.SendMessage(p, "Could not find command, plugin or block specified.");
                    break;
            }
        }
        
        static void PrintHelpForGroup(Player p, string typeName, string typeTitle) {
            string message = "";
            foreach (Command c in Command.all.commands) {
                if (p == null || p.group.commands.All().Contains(c)) {
                    if (c.type.Contains(typeName))
                        message += ", " + getColor(c.name) + c.name;
                }
            }
            
            if (message == "") {
                Player.SendMessage(p, "No commands of this type are available to you.");
            } else {
                Player.SendMessage(p, typeTitle + " commands you may use:");
                Player.SendMessage(p, message.Remove(0, 2) + ".");
            }
        }

        private static string getColor(string commName)
        {
            foreach (GrpCommands.rankAllowance aV in GrpCommands.allowedCommands)
            {
                if (aV.commandName == commName)
                {
                    if (Group.findPerm(aV.lowestRank) != null)
                        return Group.findPerm(aV.lowestRank).color;
                }
            }

            return "&f";
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "...really? Wow. Just...wow.");
        }
    }
}
