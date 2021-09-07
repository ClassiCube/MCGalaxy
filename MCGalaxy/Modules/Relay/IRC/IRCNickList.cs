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
using System.Text;
using System.Text.RegularExpressions;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Modules.Relay;
using MCGalaxy.Network;
using Sharkbite.Irc;

namespace MCGalaxy.Modules.Relay.IRC 
{    
    /// <summary> Manages a list of IRC nicks and asssociated permissions </summary>
    sealed class IRCNickList 
    {
        Dictionary<string, List<string>> userMap = new Dictionary<string, List<string>>();     
        public IRCBot bot;
        
        public void Clear() { userMap.Clear(); }
        
        public void OnLeftChannel(UserInfo user, string channel) {
            List<string> chanNicks = GetNicks(channel);
            RemoveNick(user.Nick, chanNicks);
        }

        public void OnLeft(UserInfo user) {
            foreach (var chans in userMap) {
                RemoveNick(user.Nick, chans.Value);
            }
        }
        
        public void OnChangedNick(UserInfo user, string newNick) {
            foreach (var chans in userMap) {
                int index = GetNickIndex(user.Nick, chans.Value);
                if (index >= 0) {
                    string prefix = GetPrefix(chans.Value[index]);
                    chans.Value[index] = prefix + newNick;
                } else {
                    // should never happen, but just in case
                    bot.conn.SendNames(chans.Key);
                }
            }
        }
        
        public void UpdateFor(string channel, string[] nicks) {
            List<string> chanNicks = GetNicks(channel);
            foreach (string n in nicks)
                UpdateNick(n, chanNicks);
        }
        
        
        List<string> GetNicks(string channel) {
            foreach (var chan in userMap) {
                if (chan.Key.CaselessEq(channel)) return chan.Value;
            }
            
            List<string> nicks = new List<string>();
            userMap[channel] = nicks;
            return nicks;
        }
        
        void UpdateNick(string n, List<string> chanNicks) {
            string unprefixNick = Unprefix(n);
            for (int i = 0; i < chanNicks.Count; i++ ) {
                if (unprefixNick == Unprefix(chanNicks[i])) {
                    chanNicks[i] = n; return;
                }
            }
            chanNicks.Add(n);
        }
        
        void RemoveNick(string n, List<string> chanNicks) {
            int index = GetNickIndex(n, chanNicks);
            if (index >= 0) chanNicks.RemoveAt(index);
        }
        
        int GetNickIndex(string n, List<string> chanNicks) {
            if (chanNicks == null) return -1;
            string unprefixNick = Unprefix(n);
            
            for (int i = 0; i < chanNicks.Count; i++ ) {
                if (unprefixNick == Unprefix(chanNicks[i]))
                    return i;
            }
            return -1;
        }
        
        string Unprefix(string nick) {
            return nick.Substring(GetPrefixLength(nick));
        }
        
        string GetPrefix(string nick) {
            return nick.Substring(0, GetPrefixLength(nick));
        }
        
        int GetPrefixLength(string nick) {
            int prefixChars = 0;
            for (int i = 0; i < nick.Length; i++) {
                if (!IsNickChar(nick[i]))
                    prefixChars++;
                else
                    return prefixChars;
            }
            return prefixChars;
        }
        
        bool IsNickChar(char c) {
            return (c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                c == '[' || c == ']' || c == '{' || c == '}' || c == '^' || c == '`' || c == '_' || c == '|';
        }
        
        public bool VerifyNick(string channel, string userNick, ref string error, ref bool foundAtAll) {
            List<string> chanNicks = GetNicks(channel);
            if (chanNicks.Count == 0) return false;
            
            int index = GetNickIndex(userNick, chanNicks);
            if (index == -1) return false;
            foundAtAll = true;
            
            IRCControllerVerify verify = Server.Config.IRCVerify;
            if (verify == IRCControllerVerify.None) return true;
            
            if (verify == IRCControllerVerify.HalfOp) {
                string prefix = GetPrefix(chanNicks[index]);
                if (prefix.Length == 0 || prefix == "+") {
                    error = "You must be at least a half-op on the channel to use commands from IRC."; return false;
                }
                return true;
            } else {
                foreach (string chan in bot.OpChannels) {
                    chanNicks = GetNicks(chan);
                    if (chanNicks.Count == 0) continue;
                    
                    index = GetNickIndex(userNick, chanNicks);
                    if (index != -1) return true;
                }
                error = "You must have joined the opchannel to use commands from IRC."; return false;
            }
        }
    }
}

