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

namespace MCGalaxy.Games.ZS {
    internal static class LevelPicker {
        
        internal static void ChooseNextLevel(ZombieGame game) {
            if (game.QueuedLevel != null) { game.ChangeLevel(game.QueuedLevel); return; }
            if (!ZombieGame.ChangeLevels) return;
            
            try {
                List<string> levels = GetCandidateLevels();
                if (levels == null) return;

                string picked1 = "", picked2 = "";
                Random r = new Random();

            LevelChoice:
                string level = levels[r.Next(0, levels.Count)];
                string level2 = levels[r.Next(0, levels.Count)];

                if (level == game.lastLevel1 || level == game.lastLevel2 || level == game.CurLevelName ||
                    level2 == game.lastLevel1 || level2 == game.lastLevel2 || level2 == game.CurLevelName ||
                    level == picked1) {
                    goto LevelChoice;
                } else if (picked1 == "") {
                    picked1 = level; goto LevelChoice;
                } else {
                    picked2 = level2;
                }

                game.Level1Vote = 0; game.Level2Vote = 0; game.Level3Vote = 0;
                game.lastLevel1 = picked1; game.lastLevel2 = picked2;
                if (!game.Running || game.Status == ZombieGameStatus.LastRound) return;

                Server.votingforlevel = true;
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level != game.CurLevel) continue;
                    SendVoteMessage(pl, picked1, picked2);
                }
                System.Threading.Thread.Sleep(15000);
                Server.votingforlevel = false;

                if (!game.Running || game.Status == ZombieGameStatus.LastRound) return;
                MoveToNextLevel(r, levels, game, picked1, picked2);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
            }
        }
        
		/// <summary> Moves all players to the level which has the highest number of votes. </summary>
        internal static void MoveToNextLevel(Random r, List<string> levels, ZombieGame game,
                                             string picked1, string picked2) {
            int v1 = game.Level1Vote, v2 = game.Level2Vote, v3 = game.Level3Vote;
            
            if (v1 >= v2) {
                if (v3 > v1 && v3 > v2) {
                    game.ChangeLevel(GetRandomLevel(r, levels, game));
                } else {
                    game.ChangeLevel(picked1);
                }
            } else {
                if (v3 > v1 && v3 > v2) {
                    game.ChangeLevel(GetRandomLevel(r, levels, game));
                } else {
                    game.ChangeLevel(picked2);
                }
            }
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player pl in online)
                pl.voted = false;
        }
        
        internal static string GetRandomLevel(Random r, List<string> levels, ZombieGame game) {
            for (int i = 0; i < 100; i++) {
                string lvl = levels[r.Next(0, levels.Count)];
                if (!lvl.CaselessEq(game.CurLevelName)) return lvl;
            }
            return levels[r.Next(0, levels.Count)];
        }
        
        /// <summary> Returns a list of maps that can be used for a round of zombie survival. </summary>
        /// <returns> null if not enough levels are available, otherwise the list of levels. </returns>
        internal static List<string> GetCandidateLevels() {
            List<string> maps = ZombieGame.LevelList.Count > 0 ? ZombieGame.LevelList : GetAllMaps();
            foreach (string ignore in ZombieGame.IgnoredLevelList)
                maps.Remove(ignore);
            
            bool useLevelList = ZombieGame.LevelList.Count > 0;
            if (maps.Count <= 2 && !useLevelList) { 
                Server.s.Log("You must have more than 2 levels to change levels in Zombie Survival"); return null; }
            if (maps.Count <= 2 && useLevelList) { 
                Server.s.Log("You must have more than 2 levels in your level list to change levels in Zombie Survival"); return null; }
            return maps;
        }
        
        /// <summary> Returns a list of all possible maps (exclusing personal realms if 'ignore realms' setting is true) </summary>
        internal static List<string> GetAllMaps() {
            List<string> maps = new List<string>();
            string[] files = Directory.GetFiles("levels", "*.lvl");
            foreach (string file in files) {
                string name = Path.GetFileNameWithoutExtension(file);
                if (name.IndexOf('+') >= 0 && ZombieGame.IgnorePersonalWorlds)
                    continue;
                maps.Add(name);
            }
            return maps;
        }
        
        /// <summary> Sends the formatted vote message to the player (using bottom right if supported) </summary>
        internal static void SendVoteMessage(Player p, string lvl1, string lvl2) {
            const string line1 = "&eLevel vote - type &a1&e, &c2&e or &93";
            string line2 = "&a" + lvl1 + "&e, &c" + lvl2 + "&e, &9random";
            
            if (p.HasCpeExt(CpeExt.MessageTypes)) {
                p.SendCpeMessage(CpeMessageType.BottomRight2, line1);
                p.SendCpeMessage(CpeMessageType.BottomRight1, line2);
            } else {
                p.SendMessage(line1);
                p.SendMessage(line2);
            }
        }
    }
}
