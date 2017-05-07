/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

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

namespace MCGalaxy.Games.ZS {
    internal static class LevelPicker {
        
        internal static void ChooseNextLevel(ZombieGame game) {
            if (game.QueuedLevel != null) { game.ChangeLevel(game.QueuedLevel); return; }
            if (!ZombieGameProps.ChangeLevels) return;
            
            try {
                List<string> maps = GetCandidateLevels();
                if (maps == null) return;
                RemoveRecentLevels(maps, game);
                game.Votes1 = 0; game.Votes2 = 0; game.Votes3 = 0;
                
                Random r = new Random();
                game.Candidate1 = GetRandomLevel(r, maps);
                game.Candidate2 = GetRandomLevel(r, maps);
                game.Candidate3 = GetRandomLevel(r, maps);
                
                if (!game.Running || game.Status == ZombieGameStatus.LastRound) return;
                DoLevelVote(game);

                if (!game.Running || game.Status == ZombieGameStatus.LastRound) return;
                MoveToNextLevel(r, maps, game);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
        
        static void RemoveRecentLevels(List<string> maps, ZombieGame game) {
            // Try to avoid recently played levels, avoiding most recent
            List<string> recent = game.RecentMaps;
            for (int i = recent.Count - 1; i >= 0; i--) {
                if (maps.Count > 3 && maps.CaselessContains(recent[i]))
                    maps.CaselessRemove(recent[i]);
            }
            
            // Try to avoid maps voted last round if possible
            if (maps.Count > 3 && maps.CaselessContains(game.Candidate1))
                maps.CaselessRemove(game.Candidate1);
            if (maps.Count > 3 && maps.CaselessContains(game.Candidate2))
                maps.CaselessRemove(game.Candidate2);
            if (maps.Count > 3 && maps.CaselessContains(game.Candidate3))
                maps.CaselessRemove(game.Candidate3);
        }
        
        static void DoLevelVote(ZombieGame game) {
            Server.votingforlevel = true;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != game.CurLevel) continue;
                SendVoteMessage(pl, game);
            }
            
            VoteCountdown(game);
            Server.votingforlevel = false;
        }
        
        static void VoteCountdown(ZombieGame game) {
            // Show message for non-CPE clients
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != game.CurLevel || pl.HasCpeExt(CpeExt.MessageTypes)) continue;
                pl.SendMessage("You have 20 seconds to vote for the next map");
            }
            
            for (int i = 0; i < 20; i++) {
                players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level != game.CurLevel || !pl.HasCpeExt(CpeExt.MessageTypes)) continue;
                    pl.SendCpeMessage(CpeMessageType.BottomRight1, "&e" + (20 - i) + "s %Sleft to vote");
                }
                Thread.Sleep(1000);
            }
        }
        
        
        /// <summary> Moves all players to the level which has the highest number of votes. </summary>
        static void MoveToNextLevel(Random r, List<string> levels, ZombieGame game) {
            int v1 = game.Votes1, v2 = game.Votes2, v3 = game.Votes3;
            
            if (v1 >= v2) {
                if (v3 > v1 && v3 > v2) {
                    game.ChangeLevel(game.Candidate3);
                } else {
                    game.ChangeLevel(game.Candidate1);
                }
            } else {
                if (v3 > v1 && v3 > v2) {
                    game.ChangeLevel(game.Candidate3);
                } else {
                    game.ChangeLevel(game.Candidate2);
                }
            }
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online)
                pl.voted = false;
        }
        
        internal static string GetRandomLevel(Random r, List<string> maps) {
            int i = r.Next(0, maps.Count);
            string map = maps[i];
            
            maps.RemoveAt(i);
            return map;
        }
        
        /// <summary> Returns a list of maps that can be used for a round of zombie survival. </summary>
        /// <returns> null if not enough levels are available, otherwise the list of levels. </returns>
        internal static List<string> GetCandidateLevels() {
            List<string> maps = null;
            if (ZombieGameProps.LevelList.Count > 0) {
                maps = new List<string>(ZombieGameProps.LevelList);
            } else {
                maps = GetAllMaps();
            }
            foreach (string ignore in ZombieGameProps.IgnoredLevelList)
                maps.Remove(ignore);
            
            bool useLevelList = ZombieGameProps.LevelList.Count > 0;
            if (maps.Count <= 3 && !useLevelList) {
                Server.s.Log("You must have more than 3 levels to change levels in Zombie Survival"); return null; }
            if (maps.Count <= 3 && useLevelList) {
                Server.s.Log("You must have more than 3 levels in your level list to change levels in Zombie Survival"); return null; }
            return maps;
        }
        
        /// <summary> Returns a list of all possible maps (exclusing personal realms if 'ignore realms' setting is true) </summary>
        internal static List<string> GetAllMaps() {
            List<string> maps = new List<string>();
            string[] files = LevelInfo.AllMapFiles();
            
            foreach (string file in files) {
                string name = Path.GetFileNameWithoutExtension(file);
                if (name.IndexOf('+') >= 0 && ZombieGameProps.IgnorePersonalWorlds)
                    continue;
                maps.Add(name);
            }
            return maps;
        }
        
        /// <summary> Sends the formatted vote message to the player (using bottom right if supported) </summary>
        internal static void SendVoteMessage(Player p, ZombieGame game) {
            const string line1 = "&eLevel vote - type &a1&e, &b2&e or &c3";
            string line2 = "&a" + game.Candidate1 + "&e, &b"
                + game.Candidate2 + "&e, &c" + game.Candidate3;
            
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
