/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Network;
using Sharkbite.Irc;

namespace MCGalaxy.Modules.Relay {
    
    /// <summary> Manages a connection to an external communication service </summary>
    public abstract class RelayBot {
     
        /// <summary> List of commands that cannot be used by relay bot controllers. </summary>
        public List<string> BannedCommands;
        
        readonly Player fakeGuest = new Player("RelayBot");
        readonly Player fakeStaff = new Player("RelayBot");
        
        protected void SetDefaultBannedCommands() {
            BannedCommands = new List<string>() { "resetbot", "resetirc", "oprules", "irccontrollers", "ircctrl" };
        }
        
        protected void LoadBannedCommands() {
            SetDefaultBannedCommands();
            
            if (!File.Exists("text/irccmdblacklist.txt")) {
                File.WriteAllLines("text/irccmdblacklist.txt", new string[] {
                                       "#Here you can put commands that cannot be used from the IRC bot.",
                                       "#Lines starting with \"#\" are ignored." });
            }
            
            foreach (string line in File.ReadAllLines("text/irccmdblacklist.txt")) {
                if (!line.IsCommentLine()) BannedCommands.Add(line);
            }
        }
        
        
        protected void HookEvents() {
            OnChatEvent.Register(HandleChat, Priority.Low);
            OnChatSysEvent.Register(HandleChatSys, Priority.Low);
            OnChatFromEvent.Register(HandleChatFrom, Priority.Low);
        }
        
        protected void UnhookEvents() {
            OnChatEvent.Unregister(HandleChat);
            OnChatSysEvent.Unregister(HandleChatSys);
            OnChatFromEvent.Unregister(HandleChatFrom);
        }
        
        protected abstract void SendPublicMessage(string msg);
        protected abstract void SendStaffMessage(string msg);
        
       
        static string Unescape(Player p, string msg) {
            string full = Server.Config.IRCShowPlayerTitles ? p.FullName : p.group.Prefix + p.ColoredName;
            return msg.Replace("λFULL", full).Replace("λNICK", p.ColoredName);
        }
        
        void MessageToRelay(ChatScope scope, string msg, object arg, ChatMessageFilter filter) {
            ChatMessageFilter scopeFilter = Chat.scopeFilters[(int)scope];            
            fakeGuest.group = Group.DefaultRank;
            
            if (scopeFilter(fakeGuest, arg) && (filter == null || filter(fakeGuest, arg))) {
                SendPublicMessage(msg); return;
            }
            
            fakeStaff.group = Group.Find(Server.Config.IRCControllerRank);
            if (fakeStaff.group == null) fakeStaff.group = Group.NobodyRank;
            
            if (scopeFilter(fakeStaff, arg) && (filter == null || filter(fakeStaff, arg))) {
                SendStaffMessage(msg);
            }
        }

        void HandleChatSys(ChatScope scope, string msg, object arg,
                           ref ChatMessageFilter filter, bool relay) {
            if (relay) MessageToRelay(scope, msg, arg, filter);
        }
        
        void HandleChatFrom(ChatScope scope, Player source, string msg,
                            object arg, ref ChatMessageFilter filter, bool relay) {
            if (relay) MessageToRelay(scope, Unescape(source, msg), arg, filter);
        }
        
        void HandleChat(ChatScope scope, Player source, string msg,
                        object arg, ref ChatMessageFilter filter, bool relay) {
            if (relay) MessageToRelay(scope, Unescape(source, msg), arg, filter);
        }
    }
}
