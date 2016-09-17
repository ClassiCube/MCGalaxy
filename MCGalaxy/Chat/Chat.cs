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

namespace MCGalaxy {
    public static class Chat {
        
        public static void GlobalChatLevel(Player from, string message, bool showname) {
            if (showname)
                message = "<Level>" + from.FullName + ": &f" + message;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level == from.level && p.Chatroom == null)
                    SendMessage(p, from, message);
            }
        }
        
        public static void GlobalChatRoom(Player from, string message, bool showname) {
            string rawMessage = message;
            if (showname)
                message = "<GlobalChatRoom> " + from.FullName + ": &f" + message;

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.Chatroom != null)
                    SendMessage(p, from, message);
            }
            Server.s.Log("<GlobalChatRoom>" + from.name + ": " + rawMessage);
        }
        
        public static void ChatRoom(Player from, string message, bool showname, string chatroom) {
            string rawMessage = message;
            string messageforspy = "<ChatRoomSPY: " + chatroom + "> " + from.FullName + ": &f" + message;
            if (showname)
                message = "<ChatRoom: " + chatroom + "> " + from.FullName + ": &f" + message;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.Chatroom == chatroom)
                    SendMessage(p, from, message);
                if (p.spyChatRooms.Contains(chatroom) && p.Chatroom != chatroom)
                    SendMessage(p, from, message);
            }
            Server.s.Log("<ChatRoom " + chatroom + ">" + from.name + ": " + rawMessage);
        }
        
        static void SendMessage(Player p, Player from, string message) {
            if (from != null && p.listignored.Contains(from.name)) return;
            
            if (!p.ignoreAll || (from != null && from == p))
                Player.Message(p, Server.DefaultColor + message);
        }
        
        
        /// <summary> Sends a message to all players who match the given filter. </summary>
        public static void MessageWhere(string message, Predicate<Player> filter) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (filter(p)) Player.Message(p, message);
            }
        }
        
        /// <summary> Sends a message to all players who are on the given level. </summary>
        public static void MessageLevel(Level lvl, string message) {
            MessageWhere(message, pl => pl.level == lvl && pl.Chatroom == null);
        }
        
        /// <summary> Sends a message to all players who are have the permission to read opchat. </summary>
        public static void MessageOps(string message) {
            LevelPermission rank = CommandOtherPerms.FindPerm("opchat", LevelPermission.Operator);
            MessageWhere(message, pl => pl.Rank >= rank);
        }
        
        /// <summary> Sends a message to all players who are have the permission to read adminchat. </summary>
        public static void MessageAdmins(string message) {
            LevelPermission rank = CommandOtherPerms.FindPerm("adminchat", LevelPermission.Admin);
            MessageWhere(message, pl => pl.Rank >= rank);
        }
        
        /// <summary> Sends a message to all players, who do not have 
        /// isolated level/level only chat and are not in a chatroom. </summary>
        public static void MessageAll(string message) {
            message = Colors.EscapeColors(message);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!p.ignoreAll && p.level.worldChat && p.Chatroom == null)
                    p.SendMessage(message, true);
            }
        }
        
        #region Format helpers
        
        public static void MessageAll(string message, object a0) {
            MessageAll(String.Format(message, a0));
        }
        
        public static void MessageAll(string message, object a0, object a1) {
            MessageAll(String.Format(message, a0, a1));
        }
        
        public static void MessageAll(string message, object a0, object a1, object a2) {
            MessageAll(String.Format(message, a0, a1, a2));
        }
        
        public static void MessageAll(string message, params object[] args) {
            MessageAll(String.Format(message, args));
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
