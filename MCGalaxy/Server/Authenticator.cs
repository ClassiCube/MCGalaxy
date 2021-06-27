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

namespace MCGalaxy {
    /// <summary> Authenticates the mppass provided at login 
    /// and optionally requires additional verification </summary>
    public abstract class Authenticator {
        
        /// <summary> The currently/actively used authenticator </summary>
        public static Authenticator Current = new DefaultAuthenticator();
        
        
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
        public virtual void RequiresVerification(Player p, string action) {
            p.Message("&WYou must first verify with &T/Pass [Password] &Wbefore you can {0}", action);
        }
        
        /// <summary> Informs the given player that they should verify, 
        /// otherwise they will be unable to perform some actions </summary>
        public virtual void NeedVerification(Player p) {
            if (!Commands.Moderation.CmdPass.HasPassword(p.name)) {
                p.Message("&WPlease set your admin verification password with &T/SetPass [password]!");
            } else {
                p.Message("&WPlease complete admin verification with &T/Pass [password]!");
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
    }
    
    public sealed class DefaultAuthenticator : Authenticator {
        const string PASS_FOLDER = "extra/passwords/";
        
        public override bool HasPassword(string name) { return FindHashPath(name) != null; }
        
        public override void StorePassword(string name, string password) {
            string oldPath = FindOldHashPath(name);
            StorePassword(name, password, oldPath);
        }
        
        public override bool ResetPassword(string name) {
            string path = FindHashPath(name);
            if (path == null) return false;
            
            File.Delete(path);
            return true;
        }
        
        public override bool VerifyPassword(string name, string password) { 
            string path = NewHashPath(name);
            // If new format exists, that is always used
            if (File.Exists(path)) return CheckNewHash(path, name, password);

            // Fallback onto old format
            path = FindOldHashPath(name);
            if (!CheckOldHash(path, name, password)) return false;
            
            // Switch password to new format
            StorePassword(name, password, path);
            return true;
        }
        
        static void StorePassword(string name, string password, string oldPath) {
            byte[] hash = ComputeNewHash(name, password);
            // In case was using old .dat password format before
            if (oldPath != null) File.Delete(oldPath);
            
            Directory.CreateDirectory(PASS_FOLDER);
            File.WriteAllBytes(NewHashPath(name), hash);
        }

        
        static byte[] ComputeOldHash(string name, string pass) {
            // Pointless, but kept for backwards compatibility
            pass = pass.Replace("<", "(");
            pass = pass.Replace(">", ")");

            MD5 hash = MD5.Create();
            byte[] nameB = hash.ComputeHash(Encoding.ASCII.GetBytes(name));
            // This line means that non-ASCII characters in passwords are
            // all encoded as the "?" character.
            byte[] dataB = hash.ComputeHash(Encoding.ASCII.GetBytes(pass));
            
            byte[] result = new byte[nameB.Length + dataB.Length];
            Array.Copy(nameB, 0, result, 0,            nameB.Length);
            Array.Copy(dataB, 0, result, nameB.Length, dataB.Length);
            return hash.ComputeHash(result);
        }

        static byte[] ComputeNewHash(string name, string pass) {
            // The constant string added to the username salt is to mitigate
            // rainbox tables. We should really have a unique salt for each
            // user, but this is close enough.
            byte[] data = Encoding.UTF8.GetBytes("0bec662b-416f-450c-8f50-664fd4a41d49" + name.ToLower() + " " + pass);
            return SHA256.Create().ComputeHash(data);
        }
        
        static bool ArraysEqual(byte[] a, byte[] b) {
            if (a.Length != b.Length) return false;
            
            for (int i = 0; i < a.Length; i++) {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        static bool CheckNewHash(string path, string name, string pass) {
            byte[] stored   = File.ReadAllBytes(path);
            byte[] computed = ComputeNewHash(name, pass);
            return ArraysEqual(computed, stored);
        }
        
        static bool CheckOldHash(string path, string name, string pass) {
            byte[] stored   = File.ReadAllBytes(path);
            byte[] computed = ComputeOldHash(name, pass);

            // Old passwords stored UTF8 string instead of just the raw 16 byte hashes
            // We need to support both since this behaviour was accidentally changed
            if (stored.Length != computed.Length) {
                return Encoding.UTF8.GetString(stored) == Encoding.UTF8.GetString(computed);
            }
            return ArraysEqual(computed, stored);
        }

        
        static string NewHashPath(string name) {
            // don't want '+' at end of names
            return PASS_FOLDER + name.RemoveLastPlus().ToLower() + ".pwd";
        }
        
        static string FindOldHashPath(string name) {
            string path = PASS_FOLDER + name + ".dat";
            if (File.Exists(path)) return path;

            // Have to fallback on this for case sensitive file systems
            string[] files = AtomicIO.TryGetFiles(PASS_FOLDER, "*.dat");
            if (files == null) return null;
            
            foreach (string file in files) {
                if (file.CaselessEq(path)) return file;
            }
            return null;
        }
        
        static string FindHashPath(string name) {
            string path = NewHashPath(name);
            if (File.Exists(path)) return path;
            return FindOldHashPath(name);
        }
    }
}