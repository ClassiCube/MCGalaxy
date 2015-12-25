/*
    Copyright 2011 MCGalaxy
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdChatRoom : Command {
        
        public override string name { get { return "chatroom"; } }
        public override string shortcut { get { return "cr"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message) {
            if (p == null) {
                Server.s.Log("This command can only be used in-game."); return;
            }
            string[] parts = message.ToLower().Split(' ');
            if (parts.Length == 0) {
                if (Server.Chatrooms.Count == 0) {
                    Player.SendMessage(p, "There are currently no rooms");
                } else {
                    Player.SendMessage(p, "The current rooms are:");
                    foreach (string room in Server.Chatrooms)
                        Player.SendMessage(p, room);
                }
                return;
            }
            
            switch (parts[0]) {
                case "join":
                    HandleJoin(p, parts); break;
                case "leave":
                    HandleLeave(p); break;
                case "make":
                case "create":
                    HandleCreate(p, parts); break;
                case "delete":
                case "remove":
                    HandleDelete(p, parts); break;
                case "spy":
                case "watch":
                    HandleSpy(p, parts); break;
                case "forcejoin":
                    HandleForceJoin(p, parts); break;
                case "kick":
                case "forceleave":
                    HandleKick(p, parts); break;
                case "globalmessage":
                case "global":
                case "all":
                    HandleAll(p, parts, message); break;
                default:
                    HandleOther(p, parts); break;
            }
        }
        
        void HandleJoin(Player p, string[] parts) {
            if (parts.Length > 1 && Server.Chatrooms.Contains(parts[1])) {
                string room = parts[1];
                if (p.spyChatRooms.Contains(room)) {
                    Player.SendMessage(p, "The chat room '" + room + "' has been removed " +
                                       "from your spying list because you are joining the room.");
                    p.spyChatRooms.Remove(room);
                }
                
                Player.SendMessage(p, "You joined the chat room '" + room + "'");
                Player.ChatRoom(p, p.color + p.name + Server.DefaultColor + " has joined your chat room", false, room);
                p.Chatroom = room;
            } else {
                Player.SendMessage(p, "There is no chat room with that name");
            }
        }
        
        void HandleLeave(Player p) {
            Player.SendMessage(p, "You left the chat room '" + p.Chatroom + "'");
            Player.ChatRoom(p, p.color + p.name + Server.DefaultColor + " has left the chat room", false, p.Chatroom);
            Player.GlobalMessage(p.color + p.name + Server.DefaultColor + " has left their chat room " + p.Chatroom);
            p.Chatroom = null;
        }
        
        void HandleCreate(Player p, string[] parts) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 1)) {
                Player.SendMessage(p, "You aren't a high enough rank to create a chatroon.");
                return;
            }
            if (parts.Length <= 1) {
                Player.SendMessage(p, "You need to provide a new chatroom name.");
                return;
            }
            
            string room = parts[1];
            if (Server.Chatrooms.Contains(parts[1])) {
                Player.SendMessage(p, "The chatoom '" + room + "' already exists");
            } else {
                Server.Chatrooms.Add(room);
                Player.GlobalMessage("A new chat room '" + room + "' has been created");
            }
        }
        
        void HandleDelete(Player p, string[] parts) {
            if (parts.Length <= 1) {
                Player.SendMessage(p, "You need to provide a chatroom name to delete.");
                return;
            }
            string room = parts[1];
            bool canDeleteForce = (int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 3);
            bool canDelete = (int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2);
            if (!canDelete && !canDeleteForce) {
                Player.SendMessage(p, "You aren't a high enough rank to delete a chatroon.");
                return;
            }

            if (!Server.Chatrooms.Contains(room)) {
                Player.SendMessage(p, "There is no chatroom with the name '" + room + "'");
                return;
            }
            
            if (!canDeleteForce) {
                foreach (Player pl in Player.players) {
                    if (pl != p && pl.Chatroom == room) {
                        Player.SendMessage(p, "Sorry, someone else is in the chatroom");
                        return;
                    }
                }
            }
            
            Player.GlobalMessage(room + " is being deleted");
            if (p.Chatroom == room)
                HandleLeave(p);
            Server.Chatrooms.Remove(room);
            
            foreach (Player pl in Player.players) {
                if (pl.Chatroom == room) {
                    pl.Chatroom = null;
                    Player.SendMessage(pl, "You left the chatroom '" + room + "' because it is being deleted");
                }
                
                if (pl.spyChatRooms.Contains(room)) {
                    pl.spyChatRooms.Remove(room);
                    pl.SendMessage("Stopped spying on chatroom '" + room + 
                                   "' because it was deleted by: " + p.color + p.name);
                }
            }
            Player.GlobalMessage("The chatroom '" + room + "' has been deleted");
        }
        
        void HandleSpy(Player p, string[] parts) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 4)) {
                Player.SendMessage(p, "You aren't a high enough rank to spy " +
                                   "on a chatroon.");
                return;
            }
            if (parts.Length <= 1) {
                Player.SendMessage(p, "You need to provide a chatroom name to spy on.");
                return;
            }
            
            string room = parts[1];
            if (Server.Chatrooms.Contains(room)) {
                if (p.Chatroom == room) {
                    Player.SendMessage(p, "You cannot spy on your own room"); return;
                }
                
                if (p.spyChatRooms.Contains(room)) {
                    Player.SendMessage(p, "'" + room + "' is already in your spying list.");
                } else {
                    p.spyChatRooms.Add(room);
                    Player.SendMessage(p, "'" + room + "' has been added to your chat room spying list");
                }
            } else {
                Player.SendMessage(p, "There is no chatroom with the name '" + room + "'");
            }
        }
        
        void HandleForceJoin(Player p, string[] parts) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 5)) {
                Player.SendMessage(p, "You aren't a high enough rank to force " +
                                   "players to join a chatroon.");
                return;
            }
            if (parts.Length <= 2) {
                Player.SendMessage(p, "You need to provide a player name, then a chatroom name.");
                return;
            }
            
            string name = parts[1], room = parts[2];
            Player pl = Player.Find(name);
            if (pl == null) {
                Player.SendMessage(p, "There is no online player with the name '" + name + "'");
                return;
            }
            if (!Server.Chatrooms.Contains(room)) {
                Player.SendMessage(p, "There is no chatroom with the name '" + room + "'");
                return;
            }
            if (pl.group.Permission >= p.group.Permission) {
                Player.SendMessage(p, "You can't force someone of a higher or equal rank to join a chatroom.");
                return;
            }
            
            if (pl.spyChatRooms.Contains(room)) {
                Player.SendMessage(pl, "The chat room '" + room + "' has been removed from your spying list " +
                                   "because you are force joining the room '" + room + "'");
                pl.spyChatRooms.Remove(room);
            }
            Player.SendMessage(pl, "You've been forced to join the chat room '" + room + "'");
            Player.ChatRoom(pl, pl.color + pl.name + Server.DefaultColor + " has force joined your chat room", false, room);
            pl.Chatroom = room;
            Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor +
                               " was forced to join the chatroom '" + room + "' by you");
        }
        
        void HandleKick(Player p, string[] parts) {
            if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this, 6)) {
                Player.SendMessage(p, "You aren't a high enough rank to kick someone from a chatroon.");
                return;
            }
            if (parts.Length <= 1) {
                Player.SendMessage(p, "You need to provide a player name.");
                return;
            }
            
            string name = parts[1];
            Player pl = Player.Find(name);
            if (pl == null) {
                Player.SendMessage(p, "There is no online player with the name '" + name + "'");
                return;
            }
            if (pl.group.Permission >= p.group.Permission) {
                Player.SendMessage(p, "You can't kick someone of a higher or equal rank from a chatroom.");
                return;
            }
            
            Player.SendMessage(pl, "You were kicked from the chat room '" + pl.Chatroom + "'");
            Player.SendMessage(p, pl.color + pl.name + Server.DefaultColor + " was kicked from the chat room '" + pl.Chatroom + "'");
            Player.ChatRoom(pl, pl.color + pl.name + Server.DefaultColor + " was kicked from your chat room", false, pl.Chatroom);
            pl.Chatroom = null;
        }
        
        void HandleAll(Player p, string[] parts, string message) {
            int length = parts.Length > 1 ? parts[0].Length + 1 : parts[0].Length;
            message = message.Substring( length );
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 7)) {
                Player.GlobalChatRoom(p, message, true);
                return;
            }
            
            if (p.lastchatroomglobal.AddSeconds(30) < DateTime.Now) {
                Player.GlobalChatRoom(p, message, true);
                p.lastchatroomglobal = DateTime.Now;
            } else {
                Player.SendMessage(p, "Sorry, you must wait 30 seconds in between each global chatroom message!!");
            }
        }
        
        void HandleOther(Player p, string[] parts) {
            string room = parts[0];
            if (Server.Chatrooms.Contains(room)) {
                Player.SendMessage(p, "Players in room '" + room + "' :");
                foreach (Player pl in Player.players) {
                    if (pl.Chatroom == room)
                        Player.SendMessage(p, pl.color + pl.name);
                }
            } else {
                Player.SendMessage(p, "There is no command with the type '" + room + "'," +
                                   "nor is there a chat room with that name.");
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/chatroom - gets a list of all the current rooms");
            Player.SendMessage(p, "/chatroom [room] - gives you details about the room");
            Player.SendMessage(p, "/chatroom join [room] - joins a room");
            Player.SendMessage(p, "/chatroom leave [room] - leaves a room");
            
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                Player.SendMessage(p, "/chatroom create [room] - creates a new room");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 3))
                Player.SendMessage(p, "/chatroom delete [room] - deletes a room");
            else if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2))
                Player.SendMessage(p, "/chatroom delete [room] - deletes a room if all people have left");
            
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 4))
                Player.SendMessage(p, "/chatroom spy [room] - spy on a chatroom");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 5))
                Player.SendMessage(p, "/chatroom forcejoin [player] [room] - forces a player to join a room");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 6))
                Player.SendMessage(p, "/chatroom kick [player] - kicks the player from their current room");
            
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 7))
                Player.SendMessage(p, "/chatroom all [message] - sends a global message to all rooms");
            else
                Player.SendMessage(p, "/chatroom all [message] - sends a global message to all rooms " +
                                   "(limited to 1 every 30 seconds)");
        }
    }
}
