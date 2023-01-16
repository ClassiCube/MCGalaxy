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
using MCGalaxy.Config;
using MCGalaxy.Games;

namespace MCGalaxy.Modules.Games.Countdown
{
    public sealed class CountdownConfig : RoundsGameConfig 
    {
        [ConfigEnum("default-speed", "Defaults", CountdownSpeed.Normal, typeof(CountdownSpeed))]
        public CountdownSpeed DefaultSpeed = CountdownSpeed.Normal;
        
        [ConfigInt("winner-reward-min", "Rewards",  5, 0)]
        public int RewardMin =  5;
        [ConfigInt("winner-reward-max", "Rewards", 10, 0)]
        public int RewardMax = 10;
        
        public override bool AllowAutoload { get { return true; } }
        protected override string GameName { get { return "Countdown"; } }
        
        public override void Load() {
            base.Load();
            if (Maps.Count == 0) Maps.Add("countdown");
        }
    }
    
    public enum CountdownSpeed
    {
        Slow, Normal, Fast, Extreme, Ultimate
    }
}
