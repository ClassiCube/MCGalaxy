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
///////--|----------------------------------|--\\\\\\\
//////---|  TNT WARS - Coded by edh649      |---\\\\\\
/////----|                                  |----\\\\\
////-----|  Note: Double click on // to see |-----\\\\
///------|        them in the sidebar!!     |------\\\
//-------|__________________________________|-------\\
using System.Collections.Generic;
using System.Threading;

namespace MCGalaxy.Games {
    public sealed class TntWarsConfig {
        
        public bool InitialGracePeriod = true;        
        public int GracePeriodSeconds = 30;
        
        public int MaxPlayerActiveTnt = 1;
        public bool BalanceTeams = true;
        
        public int ScoreMaxFFA = 75;
        public int ScoreMaxTDM = 150;
        public int ScorePerKill = 10;
        public int MultiKillBonus = 5; // Amount of extra points per player killed (if more than one) per TNT
        public int AssistScore = 5;
        public bool TeamKills;
        
        public bool Streaks = true;
        public int StreakOneAmount = 3;
        public float StreakOneMultiplier = 1.25f;
        public int StreakTwoAmount = 5;
        public float StreakTwoMultiplier = 1.5f;
        public int StreakThreeAmount = 7;
        public float StreakThreeMultiplier = 2f;
        
        public static TntWarsConfig Default = new TntWarsConfig();
        
        public TntWarsConfig Copy() {
            TntWarsConfig copy = new TntWarsConfig();
            copy.InitialGracePeriod = InitialGracePeriod;
            copy.GracePeriodSeconds = GracePeriodSeconds;
            
            copy.MaxPlayerActiveTnt = MaxPlayerActiveTnt;
            copy.BalanceTeams = BalanceTeams;
            
            copy.ScoreMaxFFA = ScoreMaxFFA;
            copy.ScoreMaxTDM = ScoreMaxTDM;
            copy.ScorePerKill = ScorePerKill;
            copy.MultiKillBonus = MultiKillBonus;
            copy.AssistScore = AssistScore;
            copy.TeamKills = TeamKills;
            
            copy.Streaks = Streaks;
            copy.StreakOneAmount = StreakOneAmount;
            copy.StreakOneMultiplier = StreakOneMultiplier;
            copy.StreakTwoAmount = StreakTwoAmount;
            copy.StreakTwoMultiplier = StreakTwoMultiplier;
            copy.StreakThreeAmount = StreakThreeAmount;
            copy.StreakThreeMultiplier = StreakThreeMultiplier;
            
            return copy;
        }
    }
}