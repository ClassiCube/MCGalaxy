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
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    /// <summary> Authenticates a player using the mppass provided at login,
    /// and optionally manages additional verification for certain users </summary>
    public abstract class Authenticator 
    {        
        /// <summary> The currently/actively used authenticator </summary>
        public static Authenticator Current = new DefaultAuthenticator();
        
        
        /// <summary> Checks if the given player is allowed to login with the given mppass </summary>
        public virtual bool VerifyLogin(Player p, string mppass) {
            foreach (AuthService auth in AuthService.Services)
            {
                if (auth.Authenticate(p, mppass)) return true;
            }
            
            foreach (AuthService auth in AuthService.Services)
            {
                if (auth.FallbackAuthenticate(p)) return true;
            }
            
            return !Server.Config.VerifyNames || IPUtil.IsPrivate(p.IP);
        }
        
        /// <summary> Informs the given player that they must first
        /// verify before they can perform the given action </summary>
        public virtual void RequiresVerification(Player p, string action) {
            p.Message("&WYou must first verify with &T/Pass [Password] &Wbefore you can {0}", action);
        }
        
        /// <summary> Informs the given player that they should verify, 
        /// otherwise they will be unable to perform some actions </summary>
        public virtual void NeedVerification(Player p) {
            if (!HasPassword(p.name)) {
                p.Message("&WPlease set your account verification password with &T/SetPass [password]!");
            } else {
                p.Message("&WPlease complete account verification with &T/Pass [password]!");
            }
        }
        
        
        /// <summary> Returns whether the given player has a stored password </summary>
        public abstract bool HasPassword(string name);
        
        /// <summary> Sets the stored password for the given player </summary>
        public abstract void StorePassword(string name, string password);
        
        /// <summary> Removes the stored password for the given player </summary>
        /// <returns> Whether the given player actually had a stored password </returns>
        public abstract bool ResetPassword(string name);
        
        /// <summary> Returns whether the given pasword equals 
        /// the stored password for the given player </summary>
        public abstract bool VerifyPassword(string name, string password);
        
        
        public static bool VerifyPassword(Player p, string password) {
            if (!Current.VerifyPassword(p.name, password))
                return false;
            
            p.Message("You are now &averified &Sand can now &ause commands, modify blocks, and chat.");
            p.verifiedPass = true;
            p.Unverified   = false;
            return true;
        }
    }
    
    /// <summary> Authenticator that loads/stores passwords in /extra/passwords folder </summary>
    public class DefaultAuthenticator : Authenticator 
    {
        const string PASS_FOLDER = "extra/passwords/";
        
        public override bool HasPassword(string name) { return GetHashPath(name) != null; }
        
        public override bool ResetPassword(string name) {
            string path = GetHashPath(name);
            if (path == null) return false;
            
            File.Delete(path);
            return true;
        }
        
        public override bool VerifyPassword(string name, string password) { 
            string path = GetHashPath(name);
            if (path == null) return false;
            
            return CheckHash(path, name, password);
        }
        
        public override void StorePassword(string name, string password) {
            byte[] hash = ComputeHash(name, password);
            
            Directory.CreateDirectory(PASS_FOLDER);
            File.WriteAllBytes(HashPath(name), hash);
        }
        
        
        static string GetHashPath(string name) {
            string path = HashPath(name);
            return File.Exists(path) ? path : null;
        }

        static string HashPath(string name) {
            // unfortunately necessary for backwards compatibility
            name = Server.ToRawUsername(name);
            
            return PASS_FOLDER + name.ToLower() + ".pwd";
        }

        static bool CheckHash(string path, string name, string pass) {
            byte[] stored   = File.ReadAllBytes(path);
            byte[] computed = ComputeHash(name, pass);
            return ArraysEqual(computed, stored);
        }

        static byte[] ComputeHash(string name, string pass) {
            // The constant string added to the username salt is to mitigate
            // rainbow tables. We should really have a unique salt for each
            // user, but this is close enough.
            byte[] data = Encoding.UTF8.GetBytes("0bec662b-416f-450c-8f50-664fd4a41d49" + name.ToLower() + " " + pass);
            return SHA256.Create().ComputeHash(data);
        }
        
        static bool ArraysEqual(byte[] a, byte[] b) {
            if (a.Length != b.Length) return false;
            
            for (int i = 0; i < a.Length; i++) 
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
    }
}