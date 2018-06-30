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
using MCGalaxy.Events.ServerEvents;

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
        /// <summary> Messages all players in (or spying on) a particular chatroom. </summary>
        Chatroom,
        /// <summary> Messages all players in all chatrooms. </summary>
        AllChatrooms,
        
        /// <summary> Messages all players of a given rank </summary>
        Rank,
        /// <summary> Messages all players who can use an ItemPerms argument. </summary>
        Perms,
        /// <summary> Message to a specific player </summary>
        PM,
    }
    
    public delegate bool ChatMessageFilter(Player pl, object arg);
    public static class Chat {
        
        public static ItemPerms OpchatPerms { 
            get { 
                ItemPerms perms = CommandExtraPerms.Find("OpChat", 1);
                return perms != null ? perms : new ItemPerms(LevelPermission.Operator, null, null);
            }
        }
        
        public static ItemPerms AdminchatPerms {
            get { 
                ItemPerms perms = CommandExtraPerms.Find("AdminChat", 1);
                return perms != null ? perms : new ItemPerms(LevelPermission.Admin, null, null);
            }
        }
        
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
        
        /// <summary> Returns true if the target player is ignoring chat messags by source. </summary>
        public static bool Ignoring(Player target, Player source) {
            if (target.Ignores.All) return source != target; // don't ignore messages from self
            return source != null && target.Ignores.Names.CaselessContains(source.name);
        }
        

        public static bool FilterAll(Player pl, object arg) { return true; }
        public static bool FilterGlobal(Player pl, object arg) {
            return pl.level.SeesServerWideChat && !pl.Ignores.All && pl.Chatroom == null;
        }
        
        public static bool FilterLevel(Player pl, object arg) {
            return pl.level == arg && !pl.Ignores.All && pl.Chatroom == null;
        }
        
        public static bool FilterChatroom(Player pl, object arg) {
            string room = (string)arg;
            return pl.Chatroom == room || pl.spyChatRooms.CaselessContains(room);
        }
        public static bool FilterAllChatrooms(Player pl, object arg) { return pl.Chatroom != null; }
        
        public static bool FilterRank(Player pl, object arg) { return pl.Rank == (LevelPermission)arg; }
        public static bool FilterPerms(Player pl, object arg) { return ((ItemPerms)arg).UsableBy(pl.Rank); }
        public static bool FilterPM(Player pl, object arg) { return pl == arg; }
        
        public static ChatMessageFilter[] scopeFilters = new ChatMessageFilter[] {
            FilterAll, FilterGlobal, FilterLevel, FilterChatroom, 
            FilterAllChatrooms, FilterRank, FilterPerms, FilterPM,
        };
        
        public static ChatMessageFilter FilterVisible(Player source) {
            return (pl, obj) => Entities.CanSee(pl, source);
        }
 
        
        public static void MessageAll(string msg) { Message(ChatScope.All, msg, null, null); }
        public static void MessageGlobal(string msg) { Message(ChatScope.Global, msg, null, null); }
        public static void MessageOps(string msg) {
            Message(ChatScope.Perms, msg, OpchatPerms, null);
        }
        
        public static void MessageGlobal(string message, object a0) {
            MessageGlobal(string.Format(message, a0));
        }
        
        public static void MessageGlobal(string message, object a0, object a1) {
            MessageGlobal(string.Format(message, a0, a1));
        }
        
        public static void Message(ChatScope scope, string msg, object arg,
                                   ChatMessageFilter filter, bool irc = false) {
            Player[] players = PlayerInfo.Online.Items;
            ChatMessageFilter scopeFilter = scopeFilters[(int)scope];
            
            OnChatSysEvent.Call(scope, msg, arg, ref filter, irc);
            foreach (Player pl in players) {
                if (!scopeFilter(pl, arg)) continue;
                if (filter != null && !filter(pl, arg)) continue;
                
                Player.Message(pl, msg);
            }
        }
        
        
        public static void MessageFromLevel(Player source, string msg) {
            MessageFrom(ChatScope.Level, source, msg, source.level, null);
        }
        
        public static void MessageFromOps(Player source, string msg) { 
            MessageFrom(ChatScope.Perms, source, msg, OpchatPerms, null);  
        }
        
        public static void MessageFrom(Player source, string msg,
                                       ChatMessageFilter filter = null, bool irc = false) {
            if (source.level.SeesServerWideChat) {
                MessageFrom(ChatScope.Global, source, msg, null, filter, irc);
            } else {
                string prefix = ServerConfig.ServerWideChat ? "<Map>" : "";
                MessageFrom(ChatScope.Level, source, prefix + msg, source.level, filter);
            }
        }
        
        public static void MessageFrom(ChatScope scope, Player source, string msg, object arg,
                                       ChatMessageFilter filter, bool irc = false) {
            Player[] players = PlayerInfo.Online.Items;
            ChatMessageFilter scopeFilter = scopeFilters[(int)scope];
            if (source == null) source = ConsolePlayer.Instance;
            
            OnChatFromEvent.Call(scope, source, msg, arg, ref filter, irc);
            foreach (Player pl in players) {
                if (!scopeFilter(pl, arg)) continue;
                if (filter != null && !filter(pl, arg)) continue;
                
                if (Ignoring(pl, source)) continue;
                Player.Message(pl, UnescapeMessage(pl, source, msg));
            }
        }
        
        
        public static void MessageChat(Player source, string msg,
                                       ChatMessageFilter filter = null, bool irc = false) {
            if (source.level.SeesServerWideChat) {
                MessageChat(ChatScope.Global, source, msg, null, filter, irc);
            } else {
                string prefix = ServerConfig.ServerWideChat ? "<Map>" : "";
                MessageChat(ChatScope.Level, source, prefix + msg, source.level, filter);
            }
        }
        
        public static void MessageChat(ChatScope scope, Player source, string msg, object arg,
                                       ChatMessageFilter filter, bool irc = false) {
            Player[] players = PlayerInfo.Online.Items;
            ChatMessageFilter scopeFilter = scopeFilters[(int)scope];           
            if (source == null) source = ConsolePlayer.Instance;
            
            OnChatEvent.Call(scope, source, msg, arg, ref filter, irc);
            foreach (Player pl in players) {
                if (Ignoring(pl, source)) continue;
                // Always show message to self too (unless ignoring self)
                if (pl != source) {
                    if (!scopeFilter(pl, arg)) continue;
                    if (filter != null && !filter(pl, arg)) continue;
                } else {
                    // don't send PM back to self
                    if (scope == ChatScope.PM) { continue; }
                }
                
                Player.Message(pl, UnescapeMessage(pl, source, msg));
            }
            source.CheckForMessageSpam();
        }
        
                
        static string UnescapeMessage(Player pl, Player src, string msg) {
            if (pl.Ignores.Nicks && pl.Ignores.Titles) {
                string srcColoredTruename = src.color + src.truename;
                return msg
                    .Replace("λFULL", src.GroupPrefix + srcColoredTruename)
                    .Replace("λNICK", srcColoredTruename);
            } else if (pl.Ignores.Nicks) {
                return msg
                    .Replace("λFULL", src.color + src.prefix + src.truename)
                    .Replace("λNICK", src.color + src.truename);
            } else if (pl.Ignores.Titles) {
                return msg
                    .Replace("λFULL", src.GroupPrefix + src.ColoredName)
                    .Replace("λNICK", src.ColoredName);
            } else {
                return msg
                    .Replace("λFULL", src.FullName)
                    .Replace("λNICK", src.ColoredName);
            }
        }
    }
}
