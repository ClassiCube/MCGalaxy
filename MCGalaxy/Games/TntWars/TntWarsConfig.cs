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
        
        public bool GracePeriodAtStart = true;        
        public int GracePeriodSeconds = 30;
        
        public int TntPerPlayerAtATime = 1;
        public bool BalanceTeams = true;
        
        public int FFAmaxScore = 75;
        public int TDMmaxScore = 150;
        public int ScorePerKill = 10;
        public int MultiKillBonus = 5; // Amount of extra points per player killed (if more than one) per TNT
        public int AssistScore = 5;
        
        public bool Streaks = true;
        public int StreakOneAmount = 3;
        public float StreakOneMultiplier = 1.25f;
        public int StreakTwoAmount = 5;
        public float StreakTwoMultiplier = 1.5f;
        public int StreakThreeAmount = 7;
        public float StreakThreeMultiplier = 2f;
        
                
        public TntWarsConfig Copy() {
            TntWarsConfig copy = new TntWarsConfig();
            copy.GracePeriodAtStart = GracePeriodAtStart;
            copy.GracePeriodSeconds = GracePeriodSeconds;
            
            copy.TntPerPlayerAtATime = TntPerPlayerAtATime;
            copy.BalanceTeams = BalanceTeams;
            
            copy.FFAmaxScore = FFAmaxScore;
            copy.TDMmaxScore = TDMmaxScore;
            copy.ScorePerKill = ScorePerKill;
            copy.MultiKillBonus = MultiKillBonus;
            copy.AssistScore = AssistScore;
            
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