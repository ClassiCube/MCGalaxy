/*
    Written by Jack1312
    Copyright 2011-2012 MCForge
        
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

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdPass : Command2 {
        public override string name { get { return "Pass"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool LogUsage { get { return false; } }
        public override bool UpdatesLastCmd { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("SetPass", "set"), new CommandAlias("ResetPass", "reset") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (!Directory.Exists("extra/passwords"))
                Directory.CreateDirectory("extra/passwords");
            if (data.Rank < Server.Config.VerifyAdminsRank) {
                Formatter.MessageNeedMinPerm(p, "+ can verify or set a password", Server.Config.VerifyAdminsRank); return;
            }
            
            if (!Server.Config.verifyadmins) { p.Message("Admin verification is not currently enabled."); return; }
            if (message.Length == 0) { Help(p); return; }
            
            string[] args = message.SplitSpaces(2);
            if (args.Length == 2 && args[0].CaselessEq("set")) {
                SetPassword(p, args[1]);
            } else if (args.Length == 2 && args[0].CaselessEq("reset")) {
                ResetPassword(p, args[1]);
            } else {
                VerifyPassword(p, message);
            }
        }
        
        void VerifyPassword(Player p, string password) {
            if (!p.Unverified) { p.Message("&WYou are already verified."); return; }
            if (p.passtries >= 3) { p.Kick("Did you really think you could keep on guessing?"); return; }

            if (password.IndexOf(' ') >= 0) { p.Message("Your password must be &Wone &Sword!"); return; }
            if (!HasPassword(p.name)) {
                p.Message("You have not &Wset a password, &Suse &T/SetPass [Password] &Wto set one!"); return;
            }

            if (CheckHash(p.name, password)) {
                p.Message("You are now &averified &Sand have &aaccess to admin commands and features.");
                p.verifiedPass = true;
                p.Unverified   = false;
            } else {
                p.passtries++;
                p.Message("&WWrong Password. &SRemember your password is &Wcase sensitive.");
                p.Message("Forgot your password? Contact &W{0} &Sto &Wreset it.", Server.Config.OwnerName);
            }
        }
        
        void SetPassword(Player p, string password) {
            string path = HashPath(p.name);
            if (p.Unverified && File.Exists(path)) {
                p.Message("&WYou must first verify with &T/pass [Password] &Wbefore you can change your password.");
                p.Message("Forgot your password? Contact &W{0} &Sto &Wreset it.", Server.Config.OwnerName);
                return;
            }
            if (password.IndexOf(' ') >= 0) { p.Message("&WPassword must be one word."); return; }
            
            byte[] computed = ComputeHash(p.name, password);
            File.WriteAllBytes(path, computed);
            p.Message("Your password was &aset to: &c" + password);
        }
        
        void ResetPassword(Player p, string name) {
            if (name.Length == 0) { Help(p); return; }
            Player target = PlayerInfo.FindMatches(p, name);
            if (target == null) return;
            
            if (!p.IsConsole && p.Unverified) {
                p.Message("&WYou must first verify with &T/Pass [Password]"); return;
            }
            if (!p.IsConsole && !Server.Config.OwnerName.CaselessEq(p.name))  {
                p.Message("&WOnly console and the server owner may reset passwords."); return;
            }
            
            string path = HashPath(target.name);
            if (!File.Exists(path)) {
                p.Message("{0} &Sdoes not have a password.", p.FormatNick(target));
            } else {
                File.Delete(path);
                p.Message("Reset password for {0}", p.FormatNick(target));
            }
        }

        static string HashPath(string name)  { return "extra/passwords/" + name + ".dat"; }
        static bool HasPassword(string name) { return File.Exists(HashPath(name)); }

        static byte[] ComputeHash(string name, string pass) {
            // Pointless, but kept for backwards compatibility
            pass = pass.Replace("<", "(");
            pass = pass.Replace(">", ")");

            MD5 hash = MD5.Create();
            byte[] nameB = hash.ComputeHash(Encoding.ASCII.GetBytes(name));
            byte[] dataB = hash.ComputeHash(Encoding.ASCII.GetBytes(pass));
            
            byte[] result = new byte[nameB.Length + dataB.Length];
            Array.Copy(nameB, 0, result, 0,            nameB.Length);
            Array.Copy(dataB, 0, result, nameB.Length, dataB.Length);
            return hash.ComputeHash(result);
        }

        static bool CheckHash(string name, string pass) {
            byte[] stored   = File.ReadAllBytes(HashPath(name));
            byte[] computed = ComputeHash(name, pass);
            // Old passwords stored UTF8 string instead of just the raw 16 byte hashes
            // We need to support both since this behaviour was accidentally changed
            if (stored.Length != computed.Length) {
                return Encoding.UTF8.GetString(stored) == Encoding.UTF8.GetString(computed);
            }
            
            for (int i = 0; i < stored.Length; i++) {
                if (stored[i] != computed[i]) return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Pass reset [Player] &H- Resets the password for that player");
            p.Message("&H Note: Can only be used by console and the server owner.");
            p.Message("&T/Pass set [Password] &H- Sets your password to [password]");
            p.Message("&H Note: &WDo NOT set this as your Minecraft password!");
            p.Message("&T/Pass [Password]");
            p.Message("&HIf you are an admin, use this command to verify your login.");
            p.Message("&H You will need to be verified to be able to use commands.");
        }
    }
}
