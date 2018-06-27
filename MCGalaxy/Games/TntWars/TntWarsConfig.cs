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
using MCGalaxy.Config;

namespace MCGalaxy.Games {
    public sealed class TntWarsConfig {
        
        [ConfigBool("grace-period", null, true)]
        public bool InitialGracePeriod = true;
        [ConfigInt("grace-time", null, 30)]
        public int GracePeriodSeconds = 30;
        
        [ConfigInt("max-active-tnt", null, 1)]
        public int MaxPlayerActiveTnt = 1;
        
        [ConfigBool("team-balance", null, true)]
        public bool BalanceTeams = true;
        [ConfigBool("team-kill", null, false)]
        public bool TeamKills;
        
        [ConfigInt("score-needed", null, 150)]
        public int ScoreRequired = 150;
        [ConfigInt("scores-per-kill", null, 10)]
        public int ScorePerKill = 10;
        [ConfigInt("score-assist", null, 5)]
        public int AssistScore = 5;
        [ConfigInt("score-multi-kill-bonus", null, 5)]
        public int MultiKillBonus = 5; // Amount of extra points per player killed (if more than one) per TNT
        
        [ConfigBool("streaks", null, true)]
        public bool Streaks = true;
        public int StreakOneAmount = 3;
        public float StreakOneMultiplier = 1.25f;
        public int StreakTwoAmount = 5;
        public float StreakTwoMultiplier = 1.5f;
        public int StreakThreeAmount = 7;
        public float StreakThreeMultiplier = 2f;
        
        public static TntWarsConfig Default = new TntWarsConfig();
    }
}