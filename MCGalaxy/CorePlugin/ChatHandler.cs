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
using MCGalaxy.Commands.Chatting;

namespace MCGalaxy.Core {

    internal static class ChatHandler {
        
        internal static void HandleOnChat(ChatScope scope, Player source, ref string msg, 
                                          object arg, ref ChatMessageFilter filter, bool irc) {
            msg = msg.Replace("λFULL", source.name).Replace("λNICK", source.name);
            LogType logType = LogType.PlayerChat;
            
            if (scope == ChatScope.AboveEqRank) {
               logType = LogType.StaffChat;
            } else if (scope == ChatScope.Chatroom || scope == ChatScope.AllChatrooms) {
                logType = LogType.ChatroomChat;
            } else if (scope == ChatScope.Rank) {
                logType = LogType.RankChat;
            }
            
            Logger.Log(logType, msg);
            Player[] players = PlayerInfo.Online.Items;
            ChatMessageFilter scopeFilter = Chat.scopeFilters[(int)scope];
            
            foreach (Player pl in players) {
                if (pl == source || !Chat.NotIgnoring(pl, source)) continue;
                if (!scopeFilter(pl, arg)) continue;
                if (filter != null && !filter(pl, arg)) continue;
                
                source.TotalMessagesSent++; break;
            }
        }
        
        internal static void HandleCommand(Player p, string cmd, string args) {
            if (!ServerConfig.CoreSecretCommands) return;

            // DO NOT REMOVE THE TWO COMMANDS BELOW, /PONY AND /RAINBOWDASHLIKESCOOLTHINGS. -EricKilla
            if (cmd == "pony") {
                p.cancelcommand = true;
                if (!MessageCmd.CanSpeak(p, cmd)) return;
                int used = p.Extras.GetInt("MCG_PONY");
                            
                if (used < 2) {
                    Chat.MessageFrom(p, "λNICK %Sjust so happens to be a proud brony! Everyone give λNICK %Sa brohoof!");
                    Logger.Log(LogType.CommandUsage, "{0} used /{1}", p.name, cmd);
                } else {
                    Player.Message(p, "You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                
                p.Extras.PutInt("MCG_PONY", used + 1);
            } else if (cmd == "rainbowdashlikescoolthings") {
                p.cancelcommand = true;
                if (!MessageCmd.CanSpeak(p, cmd)) return;
                int used = p.Extras.GetInt("MCG_RD");
                
                if (used < 2) {
                    Chat.MessageGlobal("&4T&6H&eI&aS&3 S&9E&1R&4V&6E&eR &aJ&3U&9S&1T &4G&6O&eT &a2&30 &9P&1E&4R&6C&eE&aN&3T &9C&1O&4O&6L&eE&aR&3!");
                    Logger.Log(LogType.CommandUsage, "{0} used /{1}", p.name, cmd);
                } else {
                    Player.Message(p, "You have used this command 2 times. You cannot use it anymore! Sorry, Brony!");
                }
                
                p.Extras.PutInt("MCG_RD", used + 1);
            }
        }
    }
}
