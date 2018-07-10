/*
    Copyright 2011 MCForge
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Collections.Generic;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdChatRoom : Command2 {
        public override string name { get { return "ChatRoom"; } }
        public override string shortcut { get { return "cr"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.AdvBuilder, "can create chatrooms"),
                    new CommandPerm(LevelPermission.AdvBuilder, "can delete empty chatrooms"),
                    new CommandPerm(LevelPermission.Operator, "can manage chatrooms"),
                    new CommandPerm(LevelPermission.Operator, "can message all chatrooms without delay"),
                }; }
        }
        
        static List<string> Chatrooms = new List<string>();
        
        public override void Use(Player p, string message, CommandData data) {
            string[] parts = message.ToLower().SplitSpaces(2);
            
            if (message.Length == 0) {
                if (Chatrooms.Count == 0) {
                    p.Message("There are currently no chatrooms");
                } else {
                    p.Message("Current chatrooms are:");
                    foreach (string room in Chatrooms)
                        p.Message(room);
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
            if (parts.Length > 1 && Chatrooms.CaselessContains(parts[1])) {
                string room = parts[1];
                if (p.spyChatRooms.CaselessRemove(room)) {
                    p.Message("The chat room '{0}' has been removed " +
                                   "from your spying list because you are joining the room.", room);
                }
                
                p.Message("You joined the chat room '{0}'", room);
                Chat.MessageFrom(ChatScope.Chatroom, p, "λNICK %Sjoined your chat room", room, null);
                p.Chatroom = room;
            } else {
                p.Message("There is no chat room with that name");
            }
        }
        
        void HandleLeave(Player p) {
            p.Message("You left the chat room '{0}'", p.Chatroom);
            Chat.MessageFrom(ChatScope.Chatroom, p, "λNICK %Sleft your chat room", p.Chatroom, null);
            Chat.MessageFrom(p, "λNICK %Sleft their chat room " + p.Chatroom);
            p.Chatroom = null;
        }
        
        void HandleCreate(Player p, string[] parts) {
            if (!CheckExtraPerm(p, 1)) return;
            if (parts.Length <= 1) {
                p.Message("You need to provide a new chatroom name."); return;
            }
            
            string room = parts[1];
            if (Chatrooms.CaselessContains(parts[1])) {
                p.Message("The chatoom '{0}' already exists", room);
            } else {
                Chatrooms.Add(room);
                Chat.MessageGlobal("A new chat room '{0}' has been created", room);
            }
        }
        
        void HandleDelete(Player p, string[] parts) {
            if (parts.Length <= 1) {
                p.Message("You need to provide a chatroom name to delete.");
                return;
            }
            string room = parts[1];
            bool canDeleteForce = HasExtraPerm(p, 3);
            bool canDelete = HasExtraPerm(p, 2);
            if (!canDelete && !canDeleteForce) {
                p.Message("You aren't a high enough rank to delete a chatroon.");
                return;
            }

            if (!Chatrooms.CaselessContains(room)) {
                p.Message("There is no chatroom with the name '{0}'", room); return;
            }
            
            if (!canDeleteForce) {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl != p && pl.Chatroom == room) {
                        p.Message("Sorry, someone else is in the chatroom"); return;
                    }
                }
            }
            
            Chat.MessageGlobal("{0} is being deleted", room);
            if (p.Chatroom == room)
                HandleLeave(p);
            Chatrooms.CaselessRemove(room);
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                if (pl.Chatroom == room) {
                    pl.Chatroom = null;
                    pl.Message("You left the chatroom '{0}' because it is being deleted", room);
                }
                
                if (pl.spyChatRooms.CaselessRemove(room)) {
                    pl.Message("Stopped spying on chatroom '{0}' because it was deleted by: {1}",
            		           room, p.ColoredName);
                }
            }
            Chat.MessageGlobal("The chatroom '{0}' has been deleted", room);
        }
        
        void HandleSpy(Player p, string[] parts) {
            if (!CheckExtraPerm(p, 3)) return;
            if (parts.Length <= 1) {
                p.Message("You need to provide a chatroom name to spy on."); return;
            }
            
            string room = parts[1];
            if (Chatrooms.CaselessContains(room)) {
                if (p.Chatroom == room) {
                    p.Message("You cannot spy on your own room"); return;
                }
                
                if (p.spyChatRooms.CaselessContains(room)) {
                    p.Message("'{0}' is already in your spying list.", room);
                } else {
                    p.spyChatRooms.Add(room);
                    p.Message("'{0}' has been added to your chat room spying list", room);
                }
            } else {
                p.Message("There is no chatroom with the name '{0}'", room);
            }
        }
        
        void HandleForceJoin(Player p, string[] parts) {
            if (!CheckExtraPerm(p, 3)) return;
            if (parts.Length <= 2) {
                p.Message("You need to provide a player name, then a chatroom name."); return;
            }
            
            string name = parts[1], room = parts[2];
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;
            
            if (!Chatrooms.CaselessContains(room)) {
                p.Message("There is no chatroom with the name '{0}'", room); return;
            }
            if (!CheckRank(p, pl, "force-join", false)) return;
            
            if (pl.spyChatRooms.CaselessRemove(room)) {
                pl.Message("The chat room '{0}' has been removed from your spying list " +
            	           "because you are force joining the room '{0}'", room);
            }
            
            pl.Message("You've been forced to join the chat room '{0}'", room);
            Chat.MessageFrom(ChatScope.Chatroom, pl, "λNICK %Shas force joined your chat room", room, null);
            
            pl.Chatroom = room;
            p.Message(pl.ColoredName + " %Swas forced to join the chatroom '{0}' by you", room);
        }
        
        void HandleKick(Player p, string[] parts) {
            if (!CheckExtraPerm(p, 3)) return;
            if (parts.Length <= 1) {
                p.Message("You need to provide a player name.");
                return;
            }
            
            string name = parts[1];
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;
            if (!CheckRank(p, pl, "kick from a chatroom", false)) return;
            
            pl.Message("You were kicked from the chat room '" + pl.Chatroom + "'");
            p.Message(pl.ColoredName + " %Swas kicked from the chat room '" + pl.Chatroom + "'");          
            Chat.MessageFrom(ChatScope.Chatroom, pl, "λNICK %Swas kicked from your chat room", pl.Chatroom, null);
            pl.Chatroom = null;
        }
        
        void HandleAll(Player p, string[] parts, string message) {
            message = parts.Length > 1 ? parts[1] : ""; // TODO: don't let you send empty message            
            bool canSend = HasExtraPerm(p, 4) || p.lastchatroomglobal.AddSeconds(30) < DateTime.UtcNow;
            
            if (canSend) {
                Logger.Log(LogType.ChatroomChat, "<GlobalChatRoom>{0}: {1}", p.name, message);
                message = "<GlobalChatRoom> λNICK: &f" + message;
                
                Chat.MessageChat(ChatScope.AllChatrooms, p, message, null, null);
                p.lastchatroomglobal = DateTime.UtcNow;
            } else {
                p.Message("Sorry, you must wait 30 seconds in between each global chatroom message!!");
            }
        }
        
        void HandleOther(Player p, string[] parts) {
            string room = parts[0];
            if (Chatrooms.CaselessContains(room)) {
                p.Message("Players in room '" + room + "' :");
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.Chatroom == room)
                        p.Message(pl.ColoredName);
                }
            } else {
                p.Message("There is no command with the type '" + room + "'," +
                               "nor is there a chat room with that name.");
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            p.Message("/chatroom - gets a list of all the current rooms");
            p.Message("/chatroom [room] - gives you details about the room");
            p.Message("/chatroom join [room] - joins a room");
            p.Message("/chatroom leave [room] - leaves a room");
            
            if (HasExtraPerm(p, 1))
                p.Message("/chatroom create [room] - creates a new room");
            if (HasExtraPerm(p, 3))
                p.Message("/chatroom delete [room] - deletes a room");
            else if (HasExtraPerm(p, 2))
                p.Message("/chatroom delete [room] - deletes a room only if all people have left");
            
            if (HasExtraPerm(p, 3)) {
                p.Message("/chatroom spy [room] - spy on a chatroom");
                p.Message("/chatroom forcejoin [player] [room] - forces a player to join a room");
                p.Message("/chatroom kick [player] - kicks the player from their current room");
            }
            
            if (HasExtraPerm(p, 4))
                p.Message("/chatroom all [message] - sends a message to all chatrooms");
            else
                p.Message("/chatroom all [message] - sends a message to all chatrooms " +
                               "(limited to 1 every 30 seconds)");
        }
    }
}
