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
using MCGalaxy.Util;

namespace MCGalaxy.Commands {
    public sealed class CmdPass : Command {
        public override string name { get { return "pass"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("setpass", "set"), new CommandAlias("resetpass", "reset") }; }
        }

        public override void Use(Player p, string message) {
            if (!Directory.Exists("extra/passwords"))
                Directory.CreateDirectory("extra/passwords");
            if (p != null && p.Rank < Server.verifyadminsrank) {
                MessageNeedMinPerm(p, "verify or set a password", (int)Server.verifyadminsrank); return;
            }
            if (!Server.verifyadmins) { Player.Message(p, "Admin verficiation is not currently enabled."); return; }
            if (message == "") { Help(p); return; }
            
            string[] args = message.SplitSpaces(2);
            if (args.Length == 2 && args[0].CaselessEq("set"))
                SetPassword(p, args[1]);
            else if (args.Length == 2 && args[0].CaselessEq("reset"))
                ResetPassword(p, args[1]);
            else
                VerifyPassword(p, message);
        }
        
        void VerifyPassword(Player p, string message) {
            if (!p.adminpen) { Player.Message(p, "You are &calready verified."); return; }
            if (p.passtries >= 3) { p.Kick("Did you really think you could keep on guessing?"); return; }

            if (message.IndexOf(' ') >= 0) { Player.Message(p, "Your password must be &cone %Sword!"); return; }
            if (!File.Exists("extra/passwords/" + p.name + ".dat")) {
                Player.Message(p, "You have not &cset a password, %Suse &a/setpass [Password] &cto set one!"); return;
            }

            if (PasswordHasher.MatchesPass(p.name, message)) {
                Player.Message(p, "You are now &averified %Sand have &aaccess to admin commands and features.");
                p.adminpen = false;
            } else {
                p.passtries++;
                Player.Message(p, "&cWrong Password. %SRemember your password is &ccase sensitive.");
                Player.Message(p, "Forgot your password? %SContact the owner so they can reset it.");
            }
        }
        
        void SetPassword(Player p, string message) {
            if (p.adminpen && File.Exists("extra/passwords/" + p.name + ".dat")) {
                Player.Message(p, "&cYou already have a password set. %SYou &ccannot change %Sit unless &cyou verify it with &a/pass [Password]. " +
                               "%SIf you have &cforgotten %Syour password, contact &c" + Server.server_owner + " %Sand they can &creset it!");
                return;
            }
            if (message.IndexOf(' ') >= 0) { Player.Message(p, "Your password must be one word!"); return; }
            
            PasswordHasher.StoreHash(p.name, message);
            Player.Message(p, "Your password has &asuccessfully &abeen set to:");
            Player.Message(p, "&c" + message);
        }
        
        void ResetPassword(Player p, string message) {
            if (message == "") { Help(p); return; }
            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            if (p != null && p.adminpen) {
                Player.Message(p, "&cYou must first verify with %T/pass [Password]"); return;
            }
            
            if (p != null && (Server.server_owner == "Notch" || Server.server_owner == "")) {
                Player.Message(p, "Please tell the server owner to set the 'Server Owner' property.");
                return;
            }
            if (p != null && Server.server_owner != p.name)  {
                Player.Message(p, "Only console and the server owner may reset passwords."); return;
            }
            
            if (!File.Exists("extra/passwords/" + who.name + ".dat")) {
                Player.Message(p, "That player does not have a password."); return;
            }
            File.Delete("extra/passwords/" + who.name + ".dat");
            Player.Message(p, "Password sucessfully removed for " + who.ColoredName + "%S.");
        }

        public override void Help(Player p) {         
            Player.Message(p, "%T/pass reset [Player] %H- Resets the password for that player");
            Player.Message(p, "%H Note: Can only be used by console and the server owner.");            
            Player.Message(p, "%T/pass set [Password] %H- Sets your password to [password]");
            Player.Message(p, "%H Note: &cDo NOT set this as your Minecraft password!");
            Player.Message(p, "%T/pass [Password]");
            Player.Message(p, "%HIf you are an admin, use this command to verify your login.");
            Player.Message(p, "%H You will need to be verified to be able to use commands.");
        }
    }
}
