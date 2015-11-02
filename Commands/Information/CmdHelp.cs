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
            try
            {
                switch (message.ToLower())
                {
                    case "":
                        if (Server.oldHelp)
                        {
                            goto case "old";
                        }
                        if (Server.menustyle != 1)
                        {
                            Player.SendMessage(p, "Use &b/help ranks" + Server.DefaultColor + " for a list of ranks.");
                            Player.SendMessage(p, "Use &b/help build" + Server.DefaultColor + " for a list of building commands.");
                            Player.SendMessage(p, "Use &b/help chat" + Server.DefaultColor + " for a list of chat commands.");
                            Player.SendMessage(p, "Use &b/help economy" + Server.DefaultColor + " for a list of economy commands.");
                            Player.SendMessage(p, "Use &b/help games" + Server.DefaultColor + " for a list of game commands.");
                            Player.SendMessage(p, "Use &b/help information" + Server.DefaultColor + " for a list of information commands.");
                            Player.SendMessage(p, "Use &b/help mod" + Server.DefaultColor + " for a list of moderation commands.");
                            Player.SendMessage(p, "Use &b/help other" + Server.DefaultColor + " for a list of other commands.");
                            Player.SendMessage(p, "Use &b/help world" + Server.DefaultColor + " for a list of world/map commands.");
                            
                            Player.SendMessage(p, "Use &b/help colors" + Server.DefaultColor + " to view the color codes.");
                            Player.SendMessage(p, "Use &b/help shortcuts" + Server.DefaultColor + " for a list of shortcuts.");
                            Player.SendMessage(p, "Use &b/help old" + Server.DefaultColor + " to view the Old help menu.");
                            Player.SendMessage(p, "Use &b/help [command] or /help [block] " + Server.DefaultColor + "to view more info.");
                        }
                        else
                        {
                            Player.SendMessage(p, Server.DefaultColor + "  To see a list of all commands, write %a/Help List");
                            Player.SendMessage(p, Server.DefaultColor + "  To see detailed help for a command, write %a/Help Command");
                            Player.SendMessage(p, Server.DefaultColor + "  To see your stats, write %a/Whois");
                            Player.SendMessage(p, Server.DefaultColor + "  To see loaded maps, write %a/Maps");
                            Player.SendMessage(p, Server.DefaultColor + "  To view your personal world options, use %a/OS");
                            Player.SendMessage(p, Server.DefaultColor + "  To join a Map, write %a/Goto WorldName");
                            Player.SendMessage(p, Server.DefaultColor + "  To send private messages, write %a@PlayerName Message");

                        }
                        break;
                    case "list":
                        message = "";
                        Player.SendMessage(p, Server.DefaultColor + "  To view all commands in a category, write %a/Help Category");
                        Player.SendMessage(p, Server.DefaultColor + "Command Categories:");            
                        Player.SendMessage(p, "    %aBuilding");
                        Player.SendMessage(p, "    %aChat");
                        Player.SendMessage(p, "    %aEconomy");
                        Player.SendMessage(p, "    %aGames");
                        Player.SendMessage(p, "    %aInfo");
                        Player.SendMessage(p, "    %aModeration");
                        Player.SendMessage(p, "    %aOther");
                        Player.SendMessage(p, "    %aWorld");
                        
                        Player.SendMessage(p, "    %aRanks");
                        Player.SendMessage(p, "    %aColors");
                        Player.SendMessage(p, "    %aShortcuts");
                        Player.SendMessage(p, "    %aOldMenu");
                        break;
                    case "ranks":
                        message = "";
                        foreach (Group grp in Group.GroupList)
                        {
                            if (grp.name != "nobody") // Note that -1 means max undo.  Undo anything and everything.
                                Player.SendMessage(p, grp.color + grp.name + " - &bCmd: " + grp.maxBlocks + " - &2Undo: " + ((grp.maxUndo != -1) ? grp.maxUndo.ToString() : "max") + " - &cPerm: " + (int)grp.Permission);
                        }
                        break;
                    case "build":
                    case "building":
                        PrintHelpForGroup(p, "build", "Building" );
                        break;
                    case "chat":
                        PrintHelpForGroup(p, "chat", "Chat" );
                        break;
                    case "eco":
                    case "economy":
                        PrintHelpForGroup(p, "eco", "Economy" );
                        break;
                    case "mod":
                    case "moderation":
                        PrintHelpForGroup(p, "mod", "Moderation" );
                        break;
                    case "info":
                    case "information":
                        PrintHelpForGroup(p, "info", "Information" );
                        break;
                    case "game": 
                    case "games": 
                        PrintHelpForGroup(p, "game", "Game" );
                        break;
                    case "other":
                    case "others":
                        PrintHelpForGroup(p, "other", "Other" );
                        break;
                    case "map":
                    case "world":
                        PrintHelpForGroup(p, "world", "World" );
                        break;
                    case "short":
                    case "shortcut":
                    case "shortcuts":
                    case "short 1":
                    case "shortcut 1":
                    case "shortcuts 1":
                    case "short 2":
                    case "shortcut 2":
                    case "shortcuts 2":
                        bool list1 = true;
                        try { if (message.Split()[1] == "2") list1 = false; } catch { }
                        message = "";
                        List<string> shortcuts = new List<string>();
                        foreach (Command comm in Command.all.commands)
                            if (p == null || p.group.commands.All().Contains(comm))
                                if (comm.shortcut != "") shortcuts.Add(", &b" + comm.shortcut + " " + Server.DefaultColor + "[" + comm.name + "]");
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
                            Player.SendMessage(p, "0 - &0Black " + Server.DefaultColor + "| 8 - &8Gray");
                            Player.SendMessage(p, "1 - &1Navy " + Server.DefaultColor + "| 9 - &9Blue");
                            Player.SendMessage(p, "2 - &2Green " + Server.DefaultColor + "| a - &aLime");
                            Player.SendMessage(p, "3 - &3Teal " + Server.DefaultColor + "| b - &bAqua");
                            Player.SendMessage(p, "4 - &4Maroon " + Server.DefaultColor + "| c - &cRed");
                            Player.SendMessage(p, "5 - &5Purple " + Server.DefaultColor + "| d - &dPink");
                            Player.SendMessage(p, "6 - &6Gold " + Server.DefaultColor + "| e - &eYellow");
                            Player.SendMessage(p, "7 - &7Silver " + Server.DefaultColor + "| f - &fWhite");
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
                        if (!Server.oldHelp) Player.SendMessage(p, "%bIf you can't see all commands, type %f/help %band choose a help type.");
                        break;
                    default:
                        Command cmd = Command.all.Find(message);
                        if (cmd != null)
                        {
                            cmd.Help(p);
                            string foundRank = Level.PermissionToName(GrpCommands.allowedCommands.Find(grpComm => grpComm.commandName == cmd.name).lowestRank);
                            Player.SendMessage(p, "Rank needed: " + getColor(cmd.name) + foundRank);
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
            catch (Exception e) { Server.ErrorLog(e); Player.SendMessage(p, "An error occured"); }
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
