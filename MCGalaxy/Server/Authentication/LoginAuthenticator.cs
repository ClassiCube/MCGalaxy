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
using System.Net;
using System.Security.Cryptography;
using System.Text;
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    /// <summary> Authenticates a player logging in </summary>
    public abstract class LoginAuthenticator 
    {        
        public static List<LoginAuthenticator> Authenticators = new List<LoginAuthenticator>()
        {
            new MppassAuthenticator(), new MojangAuthenticator()
        };
        
        public abstract bool Verify(Player p, string mppass);
        
        
        /// <summary> Checks if the given player is allowed to login </summary>
        public static bool VerifyLogin(Player p, string mppass) {
            foreach (LoginAuthenticator auth in Authenticators)
            {
                if (auth.Verify(p, mppass)) return true;
            }
            
            return !Server.Config.VerifyNames || IPUtil.IsPrivate(p.IP);
        }
    }
    
    /// <summary> Authenticates a player using the provided mppass </summary>
    public class MppassAuthenticator : LoginAuthenticator 
    {
        public override bool Verify(Player p, string mppass) {
            foreach (AuthService auth in AuthService.Services)
            {
                if (Authenticate(auth, p, mppass)) return true;
            }
            return false;
        }
        
        static bool Authenticate(AuthService auth, Player p, string mppass) {
            string calc = Server.CalcMppass(p.truename, auth.Beat.Salt);
            if (!mppass.CaselessEq(calc)) return false;

            auth.AcceptPlayer(p);
            return true;
        }
    }   
    
    /// <summary> Authenticates a player using the Mojang session verification API </summary>
    public class MojangAuthenticator : LoginAuthenticator 
    {
        public override bool Verify(Player p, string mppass) {
            foreach (AuthService auth in AuthService.Services)
            {
                if (Authenticate(auth, p, mppass)) return true;
            }
            return false;
        }
        
        static bool Authenticate(AuthService auth, Player p, string mppass) {
            if (!auth.Config.MojangAuth) return false;
            if (!HasJoined(p.truename))  return false;
                
            auth.AcceptPlayer(p);
            return true;
        }
        
        
        const string HAS_JOINED_URL = "https://sessionserver.mojang.com/session/minecraft/hasJoined?username={0}&serverId={1}";
        public static bool HasJoined(string username) {
            string url = string.Format(HAS_JOINED_URL, username, GetServerID());
            try
            {
                HttpWebRequest req   = HttpUtil.CreateRequest(url);
                req.Timeout          = 5 * 1000;
                req.ReadWriteTimeout = 5 * 1000;

                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            } catch (Exception ex) {
                HttpUtil.DisposeErrorResponse(ex);
                Logger.LogError("Verifying Mojang session for " + username, ex);
            }

            return false;
        }
        
        static string GetServerID() {
            UpdateExternalIP();
            byte[] data = Encoding.UTF8.GetBytes(externalIP + ":" + Server.Config.Port);
            byte[] hash = new SHA1Managed().ComputeHash(data);
            
            // TODO this is bad, redo it
            return hash.Join(b => b.ToString("x2"), "");
        }
        
        static string externalIP;
        static void UpdateExternalIP() {
            if (externalIP != null) return;

            try {
                HttpWebRequest req = HttpUtil.CreateRequest("http://classicube.net/api/myip/");
                
                using (WebResponse response = req.GetResponse())
                {
                    externalIP = HttpUtil.GetResponseText(response);
                }
            } catch (Exception ex) {
                Logger.LogError("Retrieving external IP", ex);
            }
        }
    }
}