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
using MCGalaxy.Network;

namespace MCGalaxy {
	/// <summary> Authenticates the mppass provided at login 
	/// and optionally requires additional verification </summary>
    public abstract class Authenticator {
        
        /// <summary> Checks if the given player is allowed to login with the given mppass </summary>
        public virtual bool VerifyLogin(Player p, string mppass) {
            if (!Server.Config.VerifyNames) return true;
            string calculated = Server.CalcMppass(p.truename);
            
            if (!mppass.CaselessEq(calculated)) {
                if (!IPUtil.IsPrivate(p.IP)) return false;
            } else {
                p.verifiedName = true;
            }
            return true;
        }
        
        /// <summary> Informs the given player that they must first
        /// verify before they can perform the given action </summary>
        public abstract void RequiresVerification(Player p, string action);
        
        /// <summary> Informs the given player that they should verify, 
        /// otherwise they will be unable to perform some actions </summary>
        public abstract void NeedVerification(Player p);
        
        /// <summary> The currently/actively used authenticator </summary>
        public static Authenticator Current = new DefaultAuthenticator();
    }
    
    public sealed class DefaultAuthenticator : Authenticator {
        
        /// <summary> Checks if the given player is allowed to login with the given mppass </summary>
        public override bool VerifyLogin(Player p, string mppass) {
            if (!Server.Config.VerifyNames) return true;
            string calculated = Server.CalcMppass(p.truename);
            
            if (!mppass.CaselessEq(calculated)) {
                if (!IPUtil.IsPrivate(p.IP)) return false;
            } else {
                p.verifiedName = true;
            }
            return true;
        }
        
        public override void RequiresVerification(Player p, string action) {
            p.Message("&WYou must first verify with &T/Pass [Password] &Wbefore you can {0}", action);
        }
        
        public override void NeedVerification(Player p) {
            if (!Commands.Moderation.CmdPass.HasPassword(p.name)) {
                p.Message("&WPlease set your admin verification password with &T/SetPass [password]!");
            } else {
                p.Message("&WPlease complete admin verification with &T/Pass [password]!");
            }
        }
    }
}