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
using System;
using System.Threading;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Commands.World;
using MCGalaxy.DB;

namespace MCGalaxy {
    public static class PlayerOperations {
        
        /// <summary> Attempts to change the login message of the target player </summary>
        /// <remarks> Not allowed when players who cannot speak (e.g. muted) </remarks>
        public static bool SetLoginMessage(Player p, string target, string message) {
            if (message.Length == 0) {
                p.Message("Login message of {0} &Swas removed", p.FormatNick(target));
            } else {
                // Don't allow changing while muted
                if (!p.CheckCanSpeak("change login messages")) return false;
                
                p.Message("Login message of {0} &Swas changed to: {1}",
                          p.FormatNick(target), message);
            }
            
            PlayerDB.SetLoginMessage(target, message);
            return true;
        }
        
        /// <summary> Attempts to change the logout message of the target player </summary>
        /// <remarks> Not allowed when players who cannot speak (e.g. muted) </remarks>
        public static bool SetLogoutMessage(Player p, string target, string message) {
            if (message.Length == 0) {
                p.Message("Logout message of {0} &Swas removed", p.FormatNick(target));
            } else {
                // Don't allow changing while muted
                if (!p.CheckCanSpeak("change logout messages")) return false;
                
                p.Message("Loggout message of {0} &Swas changed to: {1}",
                          p.FormatNick(target), message);
            }
            
            PlayerDB.SetLogoutMessage(target, message);
            return true;
        }
    }
}
