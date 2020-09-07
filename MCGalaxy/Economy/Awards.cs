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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Eco {
    
    /// <summary> Manages the awards the server has, and which players have which awards. </summary>
    public static class Awards {
        
        public struct PlayerAward { public string Name; public List<string> Awards; }
        
        public class Award { public string Name, Description; }
        
        /// <summary> List of all awards the server has. </summary>
        public static List<Award> AwardsList = new List<Award>();

        /// <summary> List of all players who have awards. </summary>
        public static List<PlayerAward> PlayerAwards = new List<PlayerAward>();
        

        #region I/O
        
        public static void Load() {
            if (!File.Exists("text/awardsList.txt")) {
                using (StreamWriter w = new StreamWriter("text/awardsList.txt")) {
                    w.WriteLine("#This is a full list of awards. The server will load these and they can be awarded as you please");
                    w.WriteLine("#Format is:");
                    w.WriteLine("# AwardName : Description of award goes after the colon");
                    w.WriteLine();
                    w.WriteLine("Gotta start somewhere : Built your first house");
                    w.WriteLine("Climbing the ladder : Earned a rank advancement");
                    w.WriteLine("Do you live here? : Joined the server a huge bunch of times");
                }
            }

            AwardsList = new List<Award>();
            PropertiesFile.Read("text/awardsList.txt", AwardsListLineProcessor, ':');
            PlayerAwards = new List<PlayerAward>();
            PropertiesFile.Read("text/playerAwards.txt", PlayerAwardsLineProcessor, ':');
        }
        
        static void AwardsListLineProcessor(string key, string value) {
            if (value.Length == 0) return;
            Add(key, value);
        }
        
        static void PlayerAwardsLineProcessor(string key, string value) {
            if (value.Length == 0) return;
            PlayerAward pl;
            pl.Name = key.ToLower();
            pl.Awards = new List<string>();
            
            if (value.IndexOf(',') != -1) {
                foreach (string award in value.Split(',')) {
                    pl.Awards.Add(award);
                }
            } else {
                pl.Awards.Add(value);
            }
            PlayerAwards.Add(pl);
        }

        static readonly object awardLock = new object();
        public static void SaveAwards() {
            lock (awardLock)
                using (StreamWriter w = new StreamWriter("text/awardsList.txt"))
            {
                w.WriteLine("# This is a full list of awards. The server will load these and they can be awarded as you please");
                w.WriteLine("# Format is:");
                w.WriteLine("# AwardName : Description of award goes after the colon");
                w.WriteLine();
                foreach (Award award in AwardsList)
                    w.WriteLine(award.Name + " : " + award.Description);
            }
        }
        
        static readonly object playerLock = new object();
        public static void SavePlayers() {
            lock (playerLock)
                using (StreamWriter w = new StreamWriter("text/playerAwards.txt"))
            {
                foreach (PlayerAward pA in PlayerAwards)
                    w.WriteLine(pA.Name.ToLower() + " : " + pA.Awards.Join(","));
            }
        }
        #endregion
        
        
        #region Player awards
        
        /// <summary> Adds the given award to that player's list of awards. </summary>
        public static bool GiveAward(string playerName, string name) {
            List<string> awards = GetPlayerAwards(playerName);
            if (awards == null) {
                awards = new List<string>();                
                PlayerAward item; item.Name = playerName; item.Awards = awards;
                PlayerAwards.Add(item);
            }
            
            if (awards.CaselessContains(name)) return false;
            awards.Add(name);
            return true;
        }
        
        public static bool TakeAward(string playerName, string name) {
            List<string> awards = GetPlayerAwards(playerName);
            return awards != null && awards.CaselessRemove(name);
        }
        
        public static string AwardAmount(string playerName) {
            int total = AwardsList.Count;
            List<string> awards = GetCurrentPlayerAwards(playerName);
            if (awards == null || total == 0) return "&f0/" + total + " (0%)";
            
            double percentHas = Math.Round(((double)awards.Count / total) * 100, 2);
            return "&f" + awards.Count + "/" + total + " (" + percentHas + "%)";
        }
        
        public static List<string> GetPlayerAwards(string name) {
            foreach (PlayerAward pl in PlayerAwards) {
                if (pl.Name.CaselessEq(name)) return pl.Awards;
            }
            return null;
        }
        
        public static List<string> GetCurrentPlayerAwards(string name) {
            List<string> awards = GetPlayerAwards(name);
            if (awards == null) return null;
            
            // Some awards may have been deleted
            for (int i = awards.Count - 1; i >= 0; i--) {
                if (!Exists(awards[i])) awards.RemoveAt(i);
            }
            return awards;
        }
        #endregion
        
        
        #region Awards management

        public static bool Add(string name, string desc) {
            if (Exists(name)) return false;

            Award award = new Award();
            award.Name = name.Trim();
            award.Description = desc.Trim();
            AwardsList.Add(award);
            return true;
        }

        public static bool Remove(string name) {
            Award award = FindExact(name);
            if (award == null) return false;
            
            AwardsList.Remove(award);
            return true;
        }

        public static bool Exists(string name) { return FindExact(name) != null; }
        
        public static Award FindExact(string name) {
            foreach (Award award in AwardsList) {
                if (award.Name.CaselessEq(name)) return award;
            }
            return null;
        }

        #endregion
    }
}
