/*
    Copyright 2015 MCGalaxy
        
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

namespace MCGalaxy.Games {

    public static class ZombieAwards {
        
        internal const string buyQueue = "Wishes";
        internal const string infectedEnd = "Unlucky";
        internal const string onlyHuman = "Can't touch this";
        internal const string killLastHuman = "Finisher";
        internal const string killInvisHuman = "The sixth sense";
        internal const string killHumanTwice = "Deja Vu";
        internal const string reviveSurvive = "Second Chance";
        internal const string survive5Rounds = "Bear Grylls";
        internal const string luckyNumber7 = "Lucky Number 7";
        internal const string lowWinChance = "Impossible";
        internal const string starKiller = "Dream destroyer";
        internal const string afkKiller = "Assassin";
        internal const string humanKiller = "Chuck Norris";
        
        public static void AddDefaults() {
            Awards.Add(buyQueue, "Buy a queue level");
            Awards.Add(infectedEnd, "Get infected in last 5 seconds of a round");
            Awards.Add(onlyHuman, "Be the sole human survivor");
            Awards.Add(killLastHuman, "Infect the last human left");
            Awards.Add(killInvisHuman, "Infect a human who is invisible");
            Awards.Add(killHumanTwice, "Infect a human twice in the same round");
            Awards.Add(reviveSurvive, "Survive a round after using a revive");
            Awards.Add(survive5Rounds, "Survive 5 rounds in a row");
            Awards.Add(luckyNumber7, "Win lottery when exactly 7 players joined it");
            Awards.Add(lowWinChance, "Survive on a map with a win chance of 10% or les");
            Awards.Add(starKiller, "Kill a human with a golden star.");
            Awards.Add(afkKiller, "Infect an auto-afk human");
            Awards.Add(humanKiller, "Infect 10 humans in a row");
        }
        
        public static void Give(Player p, string award, ZombieGame game) {
            if (!ZombieGame.UseAwards) return;
            bool awarded = Awards.GiveAward(p.name, award);
            if (awarded && game.CurLevel != null)
                game.CurLevel.ChatLevel(p.ColoredName + " %Searned the achievement &a" + award);            
        }
    }
}
