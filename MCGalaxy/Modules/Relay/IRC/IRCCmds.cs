/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Modules.Relay.IRC 
{
    public static class IRCCmds
    {
        public static string Quit(string reason) {
            return "QUIT :" + reason;
        }
        
        
        public static string Pass(string password) {
            return "PASS :" + password;
        }
        
        public static string User(string username, string realname) {
			// 4 = IRC mode mask (invisible and not receive wallops)
            return "USER " + username + " 4 * :" + realname;
        }
        
        
        public static string Join(string target) {
            return "JOIN " + target;
        }
        
        public static string Names(string target) {
            return "NAMES " + target;
        }
    }
}

