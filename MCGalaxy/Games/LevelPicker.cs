/*
    Copyright 2015 MCGalaxy

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
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
using System.Threading;

namespace MCGalaxy.Games {
    public abstract class LevelPicker {
        
        /// <summary> Level specifically chosen to be used in the next round. </summary>
        public string QueuedMap;
        
        /// <summary> List of maps that have been recently played in this game. </summary>
        public List<string> RecentMaps = new List<string>();
        
        internal string Candidate1 = "", Candidate2 = "", Candidate3 = "";
        internal int Votes1 = 0, Votes2 = 0, Votes3 = 0;
        
        public string ChooseNextLevel(ZSGame game) {
            if (QueuedMap != null) return QueuedMap;
            
            try {
                List<string> maps = GetCandidateLevels();
                if (maps == null) return null;
                
                RemoveRecentLevels(maps);
                Votes1 = 0; Votes2 = 0; Votes3 = 0;
                
                Random r = new Random();
                Candidate1 = GetRandomLevel(r, maps);
                Candidate2 = GetRandomLevel(r, maps);
                Candidate3 = GetRandomLevel(r, maps);
                
                if (!game.Running) return null;
                DoLevelVote(game);
                if (!game.Running) return null;
                
                return NextLevel(r, maps, game);
            } catch (Exception ex) {
                Logger.LogError(ex);
                return null;
            }
        }
        
        void RemoveRecentLevels(List<string> maps) {
            // Try to avoid recently played levels, avoiding most recent
            List<string> recent = RecentMaps;
            for (int i = recent.Count - 1; i >= 0; i--) {
                if (maps.Count > 3 && maps.CaselessContains(recent[i]))
                    maps.CaselessRemove(recent[i]);
            }
            
            // Try to avoid maps voted last round if possible
            if (maps.Count > 3) maps.CaselessRemove(Candidate1);
            if (maps.Count > 3) maps.CaselessRemove(Candidate2);
            if (maps.Count > 3) maps.CaselessRemove(Candidate3);
        }
        
        void DoLevelVote(IGame game) {
            Server.votingforlevel = true;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != game.Map) continue;
                SendVoteMessage(pl);
            }
            
            VoteCountdown(game);
            Server.votingforlevel = false;
        }
        
        void VoteCountdown(IGame game) {
            // Show message for non-CPE clients
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != game.Map || pl.HasCpeExt(CpeExt.MessageTypes)) continue;
                pl.SendMessage("You have 20 seconds to vote for the next map");
            }
            
            for (int i = 0; i < 20; i++) {
                players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level != game.Map || !pl.HasCpeExt(CpeExt.MessageTypes)) continue;
                    pl.SendCpeMessage(CpeMessageType.BottomRight1, "&e" + (20 - i) + "s %Sleft to vote");
                }
                Thread.Sleep(1000);
            }
        }
                
        string NextLevel(Random r, List<string> levels, ZSGame game) {
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online) pl.voted = false;
            
            if (Votes1 >= Votes2) {
                if (Votes3 > Votes1 && Votes3 > Votes2) {
                   return Candidate3;
                } else {
                    return Candidate1;
                }
            } else {
                if (Votes3 > Votes1 && Votes3 > Votes2) {
                    return Candidate3;
                } else {
                    return Candidate2;
                }
            }
            
        }
        
        internal static string GetRandomLevel(Random r, List<string> maps) {
            int i = r.Next(0, maps.Count);
            string map = maps[i];
            
            maps.RemoveAt(i);
            return map;
        }
        
        /// <summary> Returns a list of maps that can be used for a round of this game. </summary>
        /// <returns> null if not enough levels are available, otherwise the list of levels. </returns>
        public abstract List<string> GetCandidateLevels();
        
        /// <summary> Sends the formatted vote message to the player (using bottom right if supported) </summary>
        public void SendVoteMessage(Player p) {
            const string line1 = "&eLevel vote - type &a1&e, &b2&e or &c3";
            string line2 = "&a" + Candidate1 + "&e, &b" + Candidate2 + "&e, &c" + Candidate3;
            
            if (p.HasCpeExt(CpeExt.MessageTypes)) {
                p.SendCpeMessage(CpeMessageType.BottomRight3, line1);
                p.SendCpeMessage(CpeMessageType.BottomRight2, line2);
            } else {
                Player.Message(p, line1);
                Player.Message(p, line2);
            }
        }
    }
}
