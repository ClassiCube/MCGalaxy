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
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "can manage chatrooms"),
                    new CommandPerm(LevelPermission.Operator, "can message all chatrooms without delay"),
                }; }
        }
        
        static List<string> Chatrooms = new List<string>();
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                if (Chatrooms.Count == 0) {
                    p.Message("There are currently no chatrooms");
                } else {
                    p.Message("Current chatrooms are: " + Chatrooms.Join());
                }
                return;
            }
            
            string[] args = message.SplitSpaces(2);
            string cmd = args[0];
            
            if (cmd.CaselessEq("join")) {
                HandleJoin(p, args);
            } else if (cmd.CaselessEq("leave")) {
                HandleLeave(p);
            } else if (IsCreateCommand(cmd)) {
                HandleCreate(p, args, data);
            } else if (IsDeleteCommand(cmd)) {
                HandleDelete(p, args, data);
            } else if (cmd.CaselessEq("spy") || cmd.CaselessEq("watch")) {
                HandleSpy(p, args, data);
            } else if (cmd.CaselessEq("forcejoin")) {
                HandleForceJoin(p, args, data);
            } else if (cmd.CaselessEq("kick")) {
                HandleKick(p, args, data);
            } else if (cmd.CaselessEq("global") || cmd.CaselessEq("all")) {
                HandleAll(p, args, data);
            } else {
                HandleOther(p, args);
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
        
        bool Check(Player p, string[] parts, CommandData data, string suffix) {
            if (parts.Length <= 1) {
                p.Message("%WYou need to provide the name of the " + suffix); return false;
            }
            return CheckExtraPerm(p, data, 1);
        }
        
        void HandleCreate(Player p, string[] parts, CommandData data) {
            if (!Check(p, parts, data, "chatroom to create")) return;
            
            string room = parts[1];
            if (Chatrooms.CaselessContains(room)) {
                p.Message("The chatoom '{0}' already exists", room);
            } else {
                Chatrooms.Add(room);
                Chat.MessageGlobal("Chatroom '{0}' was created", room);
            }
        }
        
        void HandleDelete(Player p, string[] parts, CommandData data) {
            if (!Check(p, parts, data, "chatroom to delete")) return;

            string room = parts[1];
            if (!Chatrooms.CaselessRemove(room)) {
                p.Message("There is no chatroom named '{0}'", room); return;
            }
            
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) {
                if (pl.Chatroom != null && pl.Chatroom.CaselessEq(room)) {
                    pl.Chatroom = null;
                    pl.Message("You left the chatroom '{0}' because it is being deleted", room);
                }
                
                if (pl.spyChatRooms.CaselessRemove(room)) {
                    pl.Message("Stopped spying on chatroom '{0}' because it was deleted by: {1}",
                               room, p.ColoredName);
                }
            }
            Chat.MessageGlobal("Chatroom '{0}' was deleted", room);
        }
        
        void HandleSpy(Player p, string[] parts, CommandData data) {
            if (!Check(p, parts, data, "chatroom to spy on")) return;

            string room = parts[1];
            if (!Chatrooms.CaselessContains(room)) {
                p.Message("There is no chatroom named '{0}'", room); return;
            }
            
            if (p.Chatroom != null && p.Chatroom.CaselessEq(room)) {
                p.Message("You cannot spy on your own chatroom");
            } else  if (p.spyChatRooms.CaselessContains(room)) {
                p.Message("You are already spying on chatroom '{0}'", room);
            } else {
                p.spyChatRooms.Add(room);
                p.Message("You are now spying on chatroom '{0}'", room);
            }
        }
        
        void HandleForceJoin(Player p, string[] parts, CommandData data) {
            if (!Check(p, parts, data, "player to force join")) return;
            if (parts.Length <= 2) {
                p.Message("%WYou also need to provide name of a chatroom"); return;
            }
            
            string name = parts[1], room = parts[2];
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;
            
            if (!Chatrooms.CaselessContains(room)) {
                p.Message("There is no chatroom named '{0}'", room); return;
            }
            if (!CheckRank(p, data, pl, "force-join", false)) return;
            
            if (pl.spyChatRooms.CaselessRemove(room)) {
                pl.Message("The chat room '{0}' has been removed from your spying list " +
                           "because you are force joining the room '{0}'", room);
            }
            
            pl.Message("You've been forced to join the chat room '{0}'", room);
            Chat.MessageFrom(ChatScope.Chatroom, pl, "λNICK %Shas force joined your chat room", room, null);
            
            pl.Chatroom = room;
            p.Message(pl.ColoredName + " %Swas forced to join the chatroom '{0}' by you", room);
        }
        
        void HandleKick(Player p, string[] parts, CommandData data) {
            if (!Check(p, parts, data, "player to kick")) return;
            
            string name = parts[1];
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;
            if (!CheckRank(p, data, pl, "kick from a chatroom", false)) return;
            
            pl.Message("You were kicked from the chat room '" + pl.Chatroom + "'");
            p.Message(pl.ColoredName + " %Swas kicked from the chat room '" + pl.Chatroom + "'");
            Chat.MessageFrom(ChatScope.Chatroom, pl, "λNICK %Swas kicked from your chat room", pl.Chatroom, null);
            pl.Chatroom = null;
        }
        
        void HandleAll(Player p, string[] parts, CommandData data) {
            string msg = parts.Length > 1 ? parts[1] : "";
            bool can = HasExtraPerm(p, data.Rank, 2) || p.lastchatroomglobal.AddSeconds(30) < DateTime.UtcNow;
            
            if (msg.Length == 0) {
                p.Message("No message to send.");
            } else if (can) {
                Logger.Log(LogType.ChatroomChat, "<GlobalChatRoom>{0}: {1}", p.name, msg);
                msg = "<GlobalChatRoom> λNICK: &f" + msg;
                
                Chat.MessageChat(ChatScope.AllChatrooms, p, msg, null, null);
                p.lastchatroomglobal = DateTime.UtcNow;
            } else {
                p.Message("%WYou can only message all chatrooms every 30 seconds");
            }
        }
        
        void HandleOther(Player p, string[] parts) {
            string room = parts[0];
            if (Chatrooms.CaselessContains(room)) {
                p.Message("Players in room '" + room + "' :");
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.Chatroom == room) p.Message(pl.ColoredName);
                }
            } else {
                p.Message("There is no command with the type '" + room + "'," +
                          "nor is there a chat room with that name.");
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            p.Message("/chatroom - lists all the current rooms");
            p.Message("/chatroom [room] - gives you details about the room");
            p.Message("/chatroom join/leave [room] - joins/leaves a room");
            
            if (HasExtraPerm(p, p.Rank, 1)) {
                p.Message("/chatroom create/delete [room] - creates/deletes a room");
                p.Message("/chatroom spy [room] - spy on a chatroom");
                p.Message("/chatroom forcejoin [player] [room] - forces a player to join a room");
                p.Message("/chatroom kick [player] - kicks that player from their room");
            }
            
            if (HasExtraPerm(p, p.Rank, 2)) {
                p.Message("/chatroom all [message] - sends a message to all chatrooms");
            } else {
                p.Message("/chatroom all [message] - sends a message to all chatrooms " +
                          "(limited to 1 every 30 seconds)");
            }
        }
    }
}
