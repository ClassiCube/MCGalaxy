/*
 
	Copyright 2012 MCForge
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public CmdPass() { }

        public override void Use(Player p, string message)
        {

            if (p.group.Permission < Server.verifyadminsrank)
            {
                Player.Message(p, "You do not have the &crequired rank to use this command!");
                return;
            }

            if (!Server.verifyadmins)
            {
                Player.Message(p, "Verification of admins is &cdisabled!");
                return;
            }

            if (!p.adminpen)
            {
                Player.Message(p, "You have &calready verified.");
                return;
            }

            if (p.passtries >= 3)
            {
                p.Kick("Did you really think you could keep on guessing?");
                return;
            }

            if (String.IsNullOrEmpty(message.Trim()))
            {
                Help(p);
                return;
            }

            int number = message.Split(' ').Length;

            if (number > 1)
            {
                Player.Message(p, "Your password must be &cone %Sword!");
                return;
            }

            if (!Directory.Exists("extra/passwords"))
            {
                Player.Message(p, "You have not &cset a password, %Suse &a/setpass [Password] &cto set one!");
                return;
            }

            if (!File.Exists("extra/passwords/" + p.name + ".dat"))
            {
                Player.Message(p, "You have not &cset a password, %Suse &a/setpass [Password] &cto set one!");
                return;
            }

            if (PasswordHasher.MatchesPass(p.name, message))
            {
                Player.Message(p, "Thank you, " + p.color + p.name + "%S! You have now &averified %Sand have &aaccess to admin commands and features!");
                p.adminpen = false;
                return;
            }

            p.passtries++;
            Player.Message(p, "&cWrong Password. %SRemember your password is &ccase sensitive!");
            Player.Message(p, "Forgot your password? %SContact the owner so they can reset it!");
        }

        public override void Help(Player p)
        {
            Player.Message(p, "/pass [Password] - If you are an admin, use this command to verify");
            Player.Message(p, "your login. You will need to use this to be given access to commands");
            Player.Message(p, "Note: If you do not have a password, use /setpass [Password]");
        }
    }
}
