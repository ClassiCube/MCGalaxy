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
    public enum ChatScope {
        /// <summary> Messages all players on the server </summary>
        All,
        /// <summary> Messages all players on levels which see server-wide chat </summary>
        /// <remarks> Excludes players who are ignoring all chat, or are in a chatroom </remarks>
        Global,
        /// <summary> Messages all players on a particular level </summary>
        /// <remarks> Excludes players who are ignoring all chat, or are in a chatroom </remarks>
        Level,
        
        /// <summary> Messages all players of a given rank </summary>
        Rank,
        /// <summary> Messages all players of or above a given rank (e.g. /opchat) </summary>
        AboveOrSameRank,
        /// <summary> Messages all players below a given rank </summary>
        BelowRank,
    }
    
    public delegate bool ChatMessageFilter(Player pl, object arg);
    public static class Chat {
        
        public static bool FilterAll(Player pl, object arg) { return true; }
        public static bool FilterGlobal(Player pl, object arg) { 
            return pl.level.SeesServerWideChat && !pl.Ignores.All && pl.Chatroom == null;
        }
        public static bool FilterLevel(Player pl, object arg) { 
            return pl.level == arg && !pl.Ignores.All && pl.Chatroom == null;
        }
        
        public static bool FilterRank(Player pl, object arg) { return pl.Rank == (LevelPermission)arg; }
        public static bool FilterAboveOrSameRank(Player pl, object arg) { return pl.Rank >= (LevelPermission)arg; }       
        public static bool FilterBelowRank(Player pl, object arg) { return pl.Rank < (LevelPermission)arg; }

        public static ChatMessageFilter FilterVisible(Player source) { 
            return (pl, obj) => Entities.CanSee(pl, source); 
        }
        
        static ChatMessageFilter[] scopeFilters = new ChatMessageFilter[] {
            FilterAll, FilterGlobal, FilterLevel,
            FilterRank, FilterAboveOrSameRank, FilterBelowRank,
        };
        
        
        public static void MessageAll(string msg) { Message(ChatScope.All, msg, null, null); }
        public static void MessageGlobal(string msg) { Message(ChatScope.Global, msg, null, null); }
        public static void MessageLevel(Level lvl, string msg) { Message(ChatScope.Level, msg, lvl, null); }
        
        public static void MessageRank(LevelPermission rank, string msg) {
            Message(ChatScope.Rank, msg, rank, null);
        }
        public static void MessageAboveOrSameRank(LevelPermission rank, string msg) {
            Message(ChatScope.AboveOrSameRank, msg, rank, null);
        }       
        public static void MessageBelowRank(LevelPermission rank, string msg) {
            Message(ChatScope.BelowRank, msg, rank, null);
        }
        
        public static void Message(ChatScope scope, string msg, object arg, 
                                   ChatMessageFilter filter, bool irc = false) {
            Player[] players = PlayerInfo.Online.Items;
            ChatMessageFilter scopeFilter = scopeFilters[(int)scope];
                
            foreach (Player pl in players) {
                if (!scopeFilter(pl, arg)) continue;
                if (filter != null && !filter(pl, arg)) continue;
                
                Player.Message(pl, msg);
            }
            if (irc) Server.IRC.Say(msg); // TODO: check scope filter here
        }
        
        
        
        public static void MessageGlobal(Player source, string msg) { 
            MessageFrom(ChatScope.Global, source, msg, null, null); 
        }
        public static void MessageLevel(Player source, string msg) { 
            MessageFrom(ChatScope.Level, source, msg, source.level, null); 
        }
        
        public static void MessageFrom(ChatScope scope, Player source, string msg, object arg, 
                                       ChatMessageFilter filter, bool irc = false) {
            Player[] players = PlayerInfo.Online.Items;
            ChatMessageFilter scopeFilter = scopeFilters[(int)scope];
                
            foreach (Player pl in players) {
                if (!NotIgnoring(pl, source)) continue;
                // Always show message to self (unless ignoring self)
                if (pl == source) { Player.Message(pl, msg); continue; }
                
                if (!scopeFilter(pl, arg)) continue;
                if (filter != null && !filter(pl, arg)) continue;
                Player.Message(pl, msg);
            }
            if (irc) Server.IRC.Say(msg); // TODO: check scope filter here
        }

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
        
        /// <summary> Sends a message to all players who are have the permission to read opchat. </summary>
        public static void MessageOps(string msg) {
            LevelPermission rank = CommandExtraPerms.MinPerm("OpChat", LevelPermission.Operator);
            MessageAboveOrSameRank(rank, msg);
        }
        
        /// <summary> Sends a message to all players who are have the permission to read adminchat. </summary>
        public static void MessageAdmins(string msg) {
            LevelPermission rank = CommandExtraPerms.MinPerm("AdminChat", LevelPermission.Admin);
            MessageAboveOrSameRank(rank, msg);
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
        static bool FilterIRC(Player pl, object arg) {
            return pl.Ignores.IRC || pl.Ignores.IRCNicks.Contains((string)arg);
        }
        static ChatMessageFilter filterIRC = FilterIRC;
        
        public static void MessageGlobalOrLevel(Player source, string msg, 
                                                ChatMessageFilter filter, bool irc = false) {
            if (source.level.SeesServerWideChat) {
                MessageFrom(ChatScope.Global, source, msg, null, filter, irc);
            } else {
                MessageFrom(ChatScope.Level, source, "<Map>" + msg, source.level, filter);
            }
        }
        
        public static void MessageGlobalIRC(string srcNick, string message) {
            Message(ChatScope.Global, message, srcNick, filterIRC);
        }
        
        public static void MessageGlobal(string message, object a0) {
            MessageGlobal(string.Format(message, a0));
        }
        
        public static void MessageGlobal(string message, object a0, object a1) {
            MessageGlobal(string.Format(message, a0, a1));
        }
        
        public static void MessageGlobal(string message, object a0, object a1, object a2) {
            MessageGlobal(string.Format(message, a0, a1, a2));
        }
        
        public static void MessageGlobal(string message, params object[] args) {
            MessageGlobal(string.Format(message, args));
        }
        #endregion
    }
}
