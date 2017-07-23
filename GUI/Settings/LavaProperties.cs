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
using System.Collections.Generic;
using System.ComponentModel;

namespace MCGalaxy.Gui {
    public sealed class LavaProperties {
        
        [DisplayName("Lives")]
        public int Lives { get; set; }

        [DisplayName("Vote time (minutes)")]
        public double VoteTime { get; set; }
        
        [DisplayName("Vote maps count")]
        public byte VoteCount { get; set; }
        
        [DisplayName("Start on server start")]
        public bool StartImmediately { get; set; }
        
        [DisplayName("Send AFK to main")]
        public bool AFKToMain { get; set; }
        
        public void LoadFromServer() {            
            Lives = Server.lava.lifeNum;
            VoteTime = Server.lava.voteTime;
            VoteCount = Server.lava.voteCount;
            StartImmediately = Server.lava.startOnStartup;
            AFKToMain = Server.lava.sendAfkMain;
        }
        
        public void ApplyToServer() {
            Server.lava.lifeNum = Lives;
            Server.lava.voteTime = VoteTime;
            Server.lava.voteCount = VoteCount;
            Server.lava.startOnStartup = StartImmediately;
            Server.lava.sendAfkMain = AFKToMain;
        }    
    }
}
