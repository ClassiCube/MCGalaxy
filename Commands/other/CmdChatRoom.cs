/*
	Copyright 2011 MCGalaxy
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdChatRoom : Command
    {
        public override string name { get { return "chatroom"; } }
        public override string shortcut { get { return "cr"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Server.s.Log("'null' or console tried to use '/chatroom', This command is limited to ingame, sorry!!");
                return;
            }

            string[] command = message.ToLower().Split(' ');
            string par0 = String.Empty;
            string par1 = String.Empty;
            string par2 = String.Empty;
            string par3 = String.Empty;
            try
            {
                par0 = command[0];
                par1 = command[1];
                par2 = command[2];
                par3 = command[3];
            }
            catch { }

            if (message == null || par0 == null || message.Trim() == "" || par0.Trim() == "")
            {
                if (Server.Chatrooms.Count == 0)
                {
                    Player.SendMessage(p, "There are currently no rooms");
                    return;
                }
                else
                {
                    Player.SendMessage(p, "The current rooms are:");
                    foreach (string room in Server.Chatrooms)
                    {
                        Player.SendMessage(p, room);
                    }
                    return;
                }
            }
            else if (par0 == "join")
            {
                if (Server.Chatrooms.Contains(par1))
                {
                    if (p.spyChatRooms.Contains(par1))
                    {
                        Player.SendMessage(p, "The chat room '" + par1 + "' has been removed from your spying list because you are joining the room.");
                        p.spyChatRooms.Remove(par1);
                    }
                    Player.SendMessage(p, "You've joined the chat room '" + par1 + "'");
                    Player.ChatRoom(p, p.color + p.name + Server.DefaultColor + " has joined your chat room", false, par1);
                    p.Chatroom = par1;
                    return;
                }
                else
                {
                    Player.SendMessage(p, "Sorry, '" + par1 + "' is not a chat room");
                    return;
                }
            }
            else if (par0 == "leave")
            {
                Player.SendMessage(p, "You've left the chat room '" + p.Chatroom + "'");
                Player.ChatRoom(p, p.color + p.name + Server.DefaultColor + " has left the chat room", false, p.Chatroom);
                Player.GlobalMessage(p.color + p.name + Server.DefaultColor + " has left their chat room " + p.Chatroom);
                p.Chatroom = null;
                return;
            }
            else if (par0 == "create" || par0 == "make")
            {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                {
                    if (Server.Chatrooms.Contains(par1))
                    {
                        Player.SendMessage(p, "Sorry, '" + par1 + "' already exists");
                        return;
                    }
                    else
                    {
                        Server.Chatrooms.Add(par1);
                        Player.GlobalMessage("The chat room '" + par1 + "' has been created");
                        return;
                    }
                }
                else
                {
                    Player.SendMessage(p, "Sorry, You aren't a high enough rank to do that");
                    return;
                }
            }
            else if (par0 == "delete" || par0 == "remove")
            {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 3))
                {
                    if (Server.Chatrooms.Contains(par1))
                    {
                        Player.GlobalMessage(par1 + " is being deleted");
                        if (p.Chatroom == par1)
                        {
                            Command.all.Find("chatroom").Use(p, "leave");
                        }
                        Server.Chatrooms.Remove(par1);
                        foreach (Player pl in Player.players)
                        {
                            if (pl.Chatroom == par1)
                            {
                                pl.Chatroom = null;
                                Player.SendMessage(pl, "You've left the room '" + par1 + "' because it is being deleted");
                            }
                            if (pl.spyChatRooms.Contains(par1))
                            {
                                pl.spyChatRooms.Remove(par1);
                                pl.SendMessage("Stopped spying on chat room '" + par1 + "' because it is being deleted by: " + p.color + p.name);
                            }
                        }
                        Player.GlobalMessage("The chatroom '" + par1 + "' has been " + (par0 + "d"));
                        Player.SendMessage(p, (par0 + "d ") + " room '" + par1 + "'");
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "Sorry, '" + par1 + "' doesn't exist");
                        return;
                    }
                }
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2))
                {
                    if (Server.Chatrooms.Contains(par1))
                    {
                        foreach (Player pl in Player.players)
                        {
                            if (pl != p)
                            {
                                if (pl.Chatroom == par1)
                                {
                                    Player.SendMessage(p, "Sorry, someone else is in the room");
                                    return;
                                }
                            }
                        }
                        if (p.Chatroom == par1)
                        {
                            Command.all.Find("chatroom").Use(p, "leave");
                        }
                        Server.Chatrooms.Remove(par1);
                        foreach (Player pl in Player.players)
                        {
                            if (pl.spyChatRooms.Contains(par1))
                            {
                                pl.spyChatRooms.Remove(par1);
                                pl.SendMessage("Stopped spying on chat room '" + par1 + "' because it is being deleted by: " + p.color + p.name);
                            }
                        }
                        Player.SendMessage(p, (par0 + "d ") + " room '" + par1 + "'");

                    }
                    else
                    {
                        Player.SendMessage(p, "Sorry, '" + par1 + "' doesn't exist");
                        return;
                    }
                }
                else
                {
                    Player.SendMessage(p, "Sorry, You aren't a high enough rank to do that");
                    return;
                }
            }
            else if (par0 == "spy")
            {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 4))
                {
                    if (Server.Chatrooms.Contains(par1))
                    {
                        if (p.Chatroom != par1)
                        {
                            if (p.spyChatRooms.Contains(par1))
                            {
                                Player.SendMessage(p, "'" + par1 + "' is already on your spying list!!");
                                return;
                            }
                            else
                            {
                                p.spyChatRooms.Add(par1);
                                Player.SendMessage(p, "'" + par1 + "' has been added to your chat room spying list");
                                return;
                            }
                        }
                        else
                        {
                            Player.SendMessage(p, "Sorry, you can't spy on your own room");
                            return;
                        }
                    }
                    else
                    {
                        Player.SendMessage(p, "Sorry, '" + par1 + "' isn't a room");
                        return;
                    }
                }
                else
                {
                    Player.SendMessage(p, "Sorry, '" + par0 + "' Wasn't a correct command addition and it wasn't a room. Sorry");
                    return;
                }
            }
            else if (par0 == "forcejoin") //[player] [room]
            {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 5))
                {
                    Player pl = Player.Find(par1);
                    if (pl == null)
                    {
                        Player.SendMessage(p, "Sorry, '" + par1 + "' isn't a player");
                        return;
                    }
                    if (!Server.Chatrooms.Contains(par2))
                    {
                        Player.SendMessage(p, "Sorry, '" + par2 + " isn't a room");
                        return;
                    }
                    if (pl.group.Permission >= p.group.Permission)
                    {
                        Player.SendMessage(p, "Sorry, You can't do that to someone of higher or equal rank");
                        return;
                    }
                    else
                    {
                        if (Server.Chatrooms.Contains(par2))
                        {
                            if (pl.spyChatRooms.Contains(par2))
                            {
                                Player.SendMessage(pl, "The chat room '" + par2 + "' has been removed from your spying list because you are force joining the room '" + par2 + "'");
                                pl.spyChatRooms.Remove(par2);
                            }
                            Player.SendMessage(pl, "You've been forced to join the chat room '" + par2 + "'");
                            Player.ChatRoom(pl, pl.color + pl.name + Server.DefaultColor + " has force joined your chat room", false, par2);
                            pl.Chatroom = par2;
                            Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " has been forced to join the chatroom '" + par2 + "' by you");
                            return;
                        }
                    }
                }
                else
                {
                    Player.SendMessage(p, "Sorry, You aren't a high enough rank to do that");
                    return;
                }
            }
            else if (par0 == "kick" || par0 == "forceleave")
            {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 6))
                {
                    Player pl = Player.Find(par1);
                    if (pl == null)
                    {
                        Player.SendMessage(p, "Sorry, '" + par1 + "' isn't a player");
                        return;
                    }
                    if (pl.group.Permission >= p.group.Permission)
                    {
                        Player.SendMessage(p, "Sorry, You can't do that to someone of higher or equal rank");
                        return;
                    }
                    else
                    {
                        Player.SendMessage(pl, "You've been kicked from the chat room '" + pl.Chatroom + "'");
                        Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " has been kicked from the chat room '" + pl.Chatroom + "'");
                        Player.ChatRoom(pl, pl.color + pl.name + Server.DefaultColor + " has been kicked from your chat room", false, pl.Chatroom);
                        pl.Chatroom = null;
                    }
                }
                else
                {
                    Player.SendMessage(p, "Sorry, You aren't a high enough rank to do that");
                    return;
                }
            }
            else if (par0 == "globalmessage" || par0 == "global" || par0 == "all")
            {
                string globalmessage = message.Replace(par0 + " ", "");
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 7))
                {
                    Player.GlobalChatRoom(p, globalmessage, true);
                    return;
                }
                else
                {
                    if (p.lastchatroomglobal.AddSeconds(30) < DateTime.Now)
                    {
                        Player.GlobalChatRoom(p, globalmessage, true);
                        p.lastchatroomglobal = DateTime.Now;
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "Sorry, You must wait 30 seconds inbetween each global chatroom message!!");
                        return;
                    }
                }
            }
            else if (par0 == "help")
            {
                Help(p);
                return;
            }
            else if (Server.Chatrooms.Contains(par0))
            {
                Player.SendMessage(p, "Players in '" + par0 + "' :");
                foreach (Player pl in Player.players)
                {
                    if (pl.Chatroom == par0)
                    {
                        Player.SendMessage(p, pl.color + pl.name);
                    }
                }
                return;
            }
            else
            {
                Player.SendMessage(p, "Sorry, '" + par0 + "' Wasn't a correct command addition and it wasn't a room. Sorry");
                Help(p);
                return;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/chatroom - gets a list of all the current rooms");
            Player.SendMessage(p, "/chatroom [room] - gives you details about the room");
            Player.SendMessage(p, "/chatroom join [room] - joins a room");
            Player.SendMessage(p, "/chatroom leave [room] - leaves a room");
            {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                {
                    Player.SendMessage(p, "/chatroom create [room] - creates a new room");
                }
                {
                    if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 3))
                    {
                        Player.SendMessage(p, "/chatroom delete [room] - deletes a room");
                    }
                    else if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2))
                    {
                        Player.SendMessage(p, "/chatroom delete [room] - deletes a room if all people have left");
                    }
                    
                }
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 4))
                {
                    Player.SendMessage(p, "/chatroom spy [room] - spy on a chatroom");
                }
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 5))
                {
                    Player.SendMessage(p, "/chatroom forcejoin [player] [room] - forces a player to join a room");
                }
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 6))
                {
                    Player.SendMessage(p, "/chatroom kick [player] - kicks the player from their current room");
                }
                {
                    if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 7))
                    {
                        Player.SendMessage(p, "/chatroom globalmessage [message] - sends a global message to all rooms");
                    }
                    else
                    {
                        Player.SendMessage(p, "/chatroom globalmessage [message] - sends a global message to all rooms (limited to 1 every 30 seconds)");
                    }
                }
            }
        }
    }
}
