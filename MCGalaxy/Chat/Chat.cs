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
                
                pre = source.group.prefix == "" ? "" : "&f" + source.group.prefix;
                msg_NT = pre + source.color + source.DisplayName + msg; // Nickname
                msg_NNNT = pre + source.color + source.truename + msg; // Account name
            }
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!NotIgnoring(source, p)) continue;
                if (visibleOnly && !Entities.CanSee(p, source)) continue;
                if (!p.level.worldChat || p.Chatroom != null) continue;
                
                if (p.ignoreNicks && p.ignoreTitles) Player.Message(p, msg_NNNT);
                else if (p.ignoreNicks) Player.Message(p, msg_NN);
                else if (p.ignoreTitles) Player.Message(p, msg_NT);
                else Player.Message(p, message);
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
                if (!NotIgnoring(source, p)) continue;
                if (visibleOnly && !Entities.CanSee(p, source)) continue;
                
                if (p.level == lvl && p.Chatroom == null)
                    Player.Message(p, message);
            }
        }

        /// <summary> Sends a message to all players who are in any chatroom,
        /// and are not ignoring source player. </summary>
        /// <remarks> Optionally prefixes message by &lt;GlobalChatRoom&gt; [source name]: </remarks>
        public static void MessageAllChatRooms(Player source, string message, bool showPrefix) {
            Server.s.Log("<GlobalChatRoom>" + source.name + ": " + message);
            if (showPrefix)
                message = "<GlobalChatRoom> " + source.FullName + ": &f" + message;

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!NotIgnoring(source, p)) continue;
                
                if (p.Chatroom != null)
                    Player.Message(p, message);
            }
        }
        
        /// <summary> Sends a message to all players who are either in or spying on the given chatroom,
        /// and are not ignoring source player. </summary>
        /// <remarks> Optionally prefixes message by &lt;ChatRoom: [chatRoom]&gt; [source name]: </remarks>
        public static void MessageChatRoom(Player source, string message, bool showPrefix, string chatRoom) {
            Server.s.Log("<ChatRoom " + chatRoom + ">" + source.name + ": " + message);
            string spyMessage = "<ChatRoomSPY: " + chatRoom + "> " + source.FullName + ": &f" + message;
            if (showPrefix)
                message = "<ChatRoom: " + chatRoom + "> " + source.FullName + ": &f" + message;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!NotIgnoring(source, p)) continue;
                
                if (p.Chatroom == chatRoom)
                    Player.Message(p, message);
                if (p.spyChatRooms.Contains(chatRoom) && p.Chatroom != chatRoom)
                    Player.Message(p, spyMessage);
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
            MessageWhere(message, pl => !pl.ignoreAll && pl.level == lvl && pl.Chatroom == null);
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
            MessageWhere(message, pl => !pl.ignoreAll && pl.level.worldChat && pl.Chatroom == null);
        }
        
        /// <summary> Sends a message to everyone, regardless of their level, chatroom or ignoring all chat. </summary>
        /// <remarks> Typically used for situations such as shutting down or updating the server. </remarks>
        public static void MessageAll(string message) {
            MessageWhere(message, pl => true);
        }
        
        #endregion
        
        
        public static string Format(string message, Player p, bool colors = true,
                                    bool tokens = true, bool emotes = true) {
            if (colors) message = Colors.EscapeColors(message);
            StringBuilder sb = new StringBuilder(message);
            if (colors) ParseColors(p, sb);
            if (tokens) ChatTokens.Apply(sb, p);
            if (!emotes) return sb.ToString();
            
            if (p.parseEmotes) {
                sb.Replace(":)", "(darksmile)");
                sb.Replace(":D", "(smile)");
                sb.Replace("<3", "(heart)");
            }
            message = EmotesHandler.Replace(sb.ToString());
            message = FullCP437Handler.Replace(message);
            return message;
        }
        
        static void ParseColors(Player p, StringBuilder sb) {
            for (int i = 0; i < sb.Length; i++) {
                char c = sb[i];
                if (c != '&' || i == sb.Length - 1) continue;
                
                char code = sb[i + 1];
                if (Colors.IsStandardColor(code)) {
                    if (code >= 'A' && code <= 'F')
                        sb[i + 1] += ' '; // WoM does not work with uppercase color codes.
                } else {
                    char fallback = Colors.GetFallback(code);
                    if (fallback == '\0') {
                        sb.Remove(i, 2); i--; // now need to check char at i again
                    } else if (!p.hasTextColors) {
                        sb[i + 1] = fallback;
                    }
                }
            }
        }
        
        /// <summary> Returns true if the target player can see chat messags by source. </summary>
        public static bool NotIgnoring(Player source, Player target) {
            if (target.ignoreAll) return source == target; // don't ignore messages from self
            
            return source == null || !target.listignored.Contains(source.name);
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
