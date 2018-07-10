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

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdPass : Command2 {
        public override string name { get { return "Pass"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("SetPass", "set"), new CommandAlias("ResetPass", "reset") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (!Directory.Exists("extra/passwords"))
                Directory.CreateDirectory("extra/passwords");
            if (data.Rank < ServerConfig.VerifyAdminsRank) {
                Formatter.MessageNeedMinPerm(p, "+ can verify or set a password", ServerConfig.VerifyAdminsRank); return;
            }
            
            if (!ServerConfig.verifyadmins) { p.Message("Admin verficiation is not currently enabled."); return; }
            if (message.Length == 0) { Help(p); return; }
            
            string[] args = message.SplitSpaces(2);
            if (args.Length == 2 && args[0].CaselessEq("set"))
                SetPassword(p, args[1]);
            else if (args.Length == 2 && args[0].CaselessEq("reset"))
                ResetPassword(p, args[1]);
            else
                VerifyPassword(p, message);
        }
        
        void VerifyPassword(Player p, string message) {
            if (!p.adminpen) { p.Message("%WYou are already verified."); return; }
            if (p.passtries >= 3) { p.Kick("Did you really think you could keep on guessing?"); return; }

            if (message.IndexOf(' ') >= 0) { p.Message("Your password must be %Wone %Sword!"); return; }
            if (!PasswordHasher.Exists(p.name)) {
                p.Message("You have not %Wset a password, %Suse %T/SetPass [Password] %Wto set one!"); return;
            }

            if (PasswordHasher.MatchesPass(p.name, message)) {
                p.Message("You are now &averified %Sand have &aaccess to admin commands and features.");
                p.adminpen = false;
            } else {
                p.passtries++;
                p.Message("%WWrong Password. %SRemember your password is %Wcase sensitive.");
                p.Message("Forgot your password? %SContact the owner so they can reset it.");
            }
        }
        
        void SetPassword(Player p, string message) {
            if (p.adminpen && PasswordHasher.Exists(p.name)) {
                p.Message("%WcYou already have a password set. %SYou %Wcannot change %Sit unless %Wyou verify it with &a/pass [Password]. " +
                               "%SIf you have %Wforgotten %Syour password, contact %W" + ServerConfig.OwnerName + " %Sand they can %Wreset it!");
                return;
            }
            if (message.IndexOf(' ') >= 0) { p.Message("Your password must be one word!"); return; }
            
            PasswordHasher.StoreHash(p.name, message);
            p.Message("Your password has &asuccessfully &abeen set to:");
            p.Message("&c" + message);
        }
        
        void ResetPassword(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;
            
            if (!p.IsConsole && p.adminpen) {
                p.Message("%WYou must first verify with %T/Pass [Password]"); return;
            }
            
            string owner = ServerConfig.OwnerName;
            if (!p.IsConsole && (owner.CaselessEq("Notch") || owner.Length == 0)) {
                p.Message("Please tell the server owner to set the 'Server Owner' property."); return;
            }
            if (!p.IsConsole && !owner.CaselessEq(p.name))  {
                p.Message("Only console and the server owner may reset passwords."); return;
            }
            
            if (!File.Exists("extra/passwords/" + who.name + ".dat")) {
                p.Message("That player does not have a password."); return;
            }
            File.Delete("extra/passwords/" + who.name + ".dat");
            p.Message("Password sucessfully removed for " + who.ColoredName + "%S.");
        }

        public override void Help(Player p) {         
            p.Message("%T/Pass reset [Player] %H- Resets the password for that player");
            p.Message("%H Note: Can only be used by console and the server owner.");            
            p.Message("%T/Pass set [Password] %H- Sets your password to [password]");
            p.Message("%H Note: %WDo NOT set this as your Minecraft password!");
            p.Message("%T/Pass [Password]");
            p.Message("%HIf you are an admin, use this command to verify your login.");
            p.Message("%H You will need to be verified to be able to use commands.");
        }
    }
}
