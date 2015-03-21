/*
 
	Copyright 2012 MCGalaxy
		
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
namespace MCGalaxy.Commands
{
    public sealed class CmdPass : Command
    {
        public override string name { get { return "pass"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }

        public CmdPass() { }

        public override void Use(Player p, string message)
        {

            if (p.group.Permission < Server.verifyadminsrank)
            {
                Player.SendMessage(p, "You do not have the &crequired rank to use this command!");
                return;
            }

            if (!Server.verifyadmins)
            {
                Player.SendMessage(p, "Verification of admins is &cdisabled!");
                return;
            }

            if (!p.adminpen)
            {
                Player.SendMessage(p, "You have &calready verified.");
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
                Player.SendMessage(p, "Your password must be &cone " + Server.DefaultColor + "word!");
                return;
            }

            if (!Directory.Exists("extra/passwords"))
            {
                Player.SendMessage(p, "You have not &cset a password, " + Server.DefaultColor + "use &a/setpass [Password] &cto set one!");
                return;
            }

            DirectoryInfo di = new DirectoryInfo("extra/passwords/");
            FileInfo[] fi = di.GetFiles("*.dat");
            if (!File.Exists("extra/passwords/" + p.name + ".dat"))
            {
                Player.SendMessage(p, "You have not &cset a password, " + Server.DefaultColor + "use &a/setpass [Password] &cto set one!");
                return;
            }

            if (PasswordHasher.MatchesPass(p.name, message))
            {
                Player.SendMessage(p, "Thank you, " + p.color + p.name + Server.DefaultColor + "! You have now &averified " + Server.DefaultColor + "and have &aaccess to admin commands and features!");
                if (p.adminpen)
                {
                    p.adminpen = false;
                }
                return;
            }

            p.passtries++;
            Player.SendMessage(p, "&cWrong Password. " + Server.DefaultColor + "Remember your password is &ccase sensitive!");
            Player.SendMessage(p, "Forgot your password? " + Server.DefaultColor + "Contact the owner so they can reset it!");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pass [Password] - If you are an admin, use this command to verify");
            Player.SendMessage(p, "your login. You will need to use this to be given access to commands");
            Player.SendMessage(p, "Note: If you do not have a password, use /setpass [Password]");
        }
    }
}