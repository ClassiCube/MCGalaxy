/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using System.Text;
using MCGalaxy.Commands;

namespace MCGalaxy {
    public static class Chat {

        #region Player messaging

        /// <summary> Sends a message to all players, who are not in a chatroom, are not ignoring all chat,
        /// are not on a level that does not have isolated/level only chat, and are not ignoring source. </summary>
        public static void MessageGlobal(Player source, string message, bool showPrefix, 
                                         bool visibleOnly = false) {
            string msg_NT = message, msg_NN = message, msg_NNNT = message;
            if (showPrefix) {
                string msg = ": &f" + message;
                string pre = source.color + source.prefix;
                message = pre + source.DisplayName + msg; // Titles + Nickname
                msg_NN = pre + source.truename + msg; // Titles + Account name
                
                pre = source.group.Prefix.Length == 0 ? "" : "&f" + source.group.Prefix;
                msg_NT = pre + source.color + source.DisplayName + msg; // Nickname
                msg_NNNT = pre + source.color + source.truename + msg; // Account name
            }
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!NotIgnoring(pl, source)) continue;
                if (visibleOnly && !Entities.CanSee(pl, source)) continue;
                if (!pl.level.SeesServerWideChat || pl.Chatroom != null) continue;
                
                if (pl.Ignores.Nicks && pl.Ignores.Titles) Player.Message(pl, msg_NNNT);
                else if (pl.Ignores.Nicks) Player.Message(pl, msg_NN);
                else if (pl.Ignores.Titles) Player.Message(pl, msg_NT);
                else Player.Message(pl, message);
            }
        }
        
        /// <summary> Sends a message to all players who in the player's level,
        /// and are not ignoring source player or in a chatroom. </summary>
        /// <remarks> Optionally prefixes message by &lt;Level&gt; [source name]: </remarks>
        public static void MessageLevel(Player source, string message, bool showPrefix, 
                                        Level lvl, bool visibleOnly = false) {
            if (showPrefix)
                message = "<Level>" + source.FullName + ": &f" + message;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!NotIgnoring(p, source)) continue;
                if (visibleOnly && !Entities.CanSee(p, source)) continue;
                
                if (p.level == lvl && p.Chatroom == null)
                    Player.Message(p, message);
            }
        }

        /// <summary> Sends a message to all players who are in any chatroom,
        /// and are not ignoring source player. </summary>
        /// <remarks> Optionally prefixes message by &lt;GlobalChatRoom&gt; [source name]: </remarks>
        public static void MessageAllChatRooms(Player source, string message, bool showPrefix) {
            Logger.Log(LogType.ChatroomChat, "<GlobalChatRoom>{0}: {1}", source.name, message);
            if (showPrefix)
                message = "<GlobalChatRoom> " + source.FullName + ": &f" + message;

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!NotIgnoring(p, source)) continue;
                
                if (p.Chatroom != null)
                    Player.Message(p, message);
            }
        }
        
        /// <summary> Sends a message to all players who are either in or spying on the given chatroom,
        /// and are not ignoring source player. </summary>
        /// <remarks> Optionally prefixes message by &lt;ChatRoom: [chatRoom]&gt; [source name]: </remarks>
        public static void MessageChatRoom(Player source, string message, bool showPrefix, string chatRoom) {
            Logger.Log(LogType.ChatroomChat, "<ChatRoom {0}>{1}: {2}", 
                       chatRoom, source.name, message);
            string spyMessage = "<ChatRoomSPY: " + chatRoom + "> " + source.FullName + ": &f" + message;
            if (showPrefix)
                message = "<ChatRoom: " + chatRoom + "> " + source.FullName + ": &f" + message;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!NotIgnoring(p, source)) continue;
                
                if (p.Chatroom == chatRoom) {
                    Player.Message(p, message);
                } else if (p.spyChatRooms.CaselessContains(chatRoom)) {
                    Player.Message(p, spyMessage);
                }
            }
        }
        
        #endregion
        
        
        #region Server messaging
        
        /// <summary> Sends a message to all players who match the given filter. </summary>
        public static void MessageWhere(string message, Predicate<Player> filter) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (filter(p)) Player.Message(p, message);
            }
        }
        
        /// <summary> Sends a message to all players who are on the given level,
        /// are not in a chatroom, and are not ignoring all chat. </summary>
        public static void MessageLevel(Level lvl, string message) {
            MessageWhere(message, pl => !pl.Ignores.All && pl.level == lvl && pl.Chatroom == null);
        }
        
        /// <summary> Sends a message to all players who are have the permission to read opchat. </summary>
        public static void MessageOps(string message) {
            LevelPermission rank = CommandExtraPerms.MinPerm("opchat", LevelPermission.Operator);
            MessageWhere(message, pl => pl.Rank >= rank);
        }
        
        /// <summary> Sends a message to all players who are have the permission to read adminchat. </summary>
        public static void MessageAdmins(string message) {
            LevelPermission rank = CommandExtraPerms.MinPerm("adminchat", LevelPermission.Admin);
            MessageWhere(message, pl => pl.Rank >= rank);
        }
        
        /// <summary> Sends a message to all players, who are not in a chatroom, are not ignoring all chat,
        /// and are not on a level that does not have isolated/level only chat. </summary>
        public static void MessageGlobal(string message) {
            MessageWhere(message, pl => !pl.Ignores.All && pl.level.SeesServerWideChat && pl.Chatroom == null);
        }
        
        /// <summary> Sends a message to everyone, regardless of their level, chatroom or ignoring all chat. </summary>
        /// <remarks> Typically used for situations such as shutting down or updating the server. </remarks>
        public static void MessageAll(string message) {
            MessageWhere(message, pl => true);
        }
        
        #endregion
        
        
        public static string Format(string message, Player p, bool tokens = true, bool emotes = true) {
            message = Colors.Escape(message);
            StringBuilder sb = new StringBuilder(message);
            Colors.Cleanup(sb, p.hasTextColors);
            
            if (tokens) ChatTokens.Apply(sb, p);
            if (!emotes) return sb.ToString();
            
            if (p.parseEmotes) {
                sb.Replace(":)", "(darksmile)");
                sb.Replace(":D", "(smile)");
                sb.Replace("<3", "(heart)");
            }
            message = EmotesHandler.Replace(sb.ToString());
            return message;
        }
        
        /// <summary> Returns true if the target player can see chat messags by source. </summary>
        public static bool NotIgnoring(Player target, Player source) {
            if (target.Ignores.All) return source == target; // don't ignore messages from self
            
            return source == null || !target.Ignores.Names.CaselessContains(source.name);
        }
        
        
        #region Format helpers
        
        public static void MessageGlobal(string message, object a0) {
            MessageGlobal(String.Format(message, a0));
        }
        
        public static void MessageGlobal(string message, object a0, object a1) {
            MessageGlobal(String.Format(message, a0, a1));
        }
        
        public static void MessageGlobal(string message, object a0, object a1, object a2) {
            MessageGlobal(String.Format(message, a0, a1, a2));
        }
        
        public static void MessageGlobal(string message, params object[] args) {
            MessageGlobal(String.Format(message, args));
        }
        
        public static void MessageWhere(string message, Predicate<Player> filter, object a0) {
            MessageWhere(String.Format(message, a0), filter);
        }
        
        public static void MessageWhere(string message, Predicate<Player> filter, object a0, object a1) {
            MessageWhere(String.Format(message, a0, a1), filter);
        }
        
        public static void MessageWhere(string message, Predicate<Player> filter, object a0, object a1, object a2) {
            MessageWhere(String.Format(message, a0, a1, a2), filter);
        }
        
        public static void MessageWhere(string message, Predicate<Player> filter, params object[] args) {
            MessageWhere(String.Format(message, args), filter);
        }
        
        #endregion
    }
}
