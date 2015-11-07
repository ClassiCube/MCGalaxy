/*
    Written By Jack1312

	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdLogoutMessage : Command
    {
        public override string name { get { return "logoutmessage"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLogoutMessage() { }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/logoutmessage [Player] [Message] - Customize your logout message.");
            if (Server.mono)
            {
                Player.SendMessage(p, "Please note that if the player is offline, the name is case sensitive.");
            }
        }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            int number = message.Split(' ').Length;
            if (number > 18) { Help(p); return; }
            if (number >= 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos);
                string s = message.Substring(pos + 1);
                Player target = Player.Find(t);
                if (target != null)
                {
                    if (!File.Exists("text/logout/" + target.name + ".txt"))
                    {
                        Player.SendMessage(p, "The player you specified does not exist!");
                        return;
                    }
                    else
                    {
                        CP437Writer.WriteAllText("text/logout/" + target.name + ".txt", s);
                    }
                    Player.SendMessage(p, "The logout message of " + target.color + target.DisplayName + Server.DefaultColor + " has been changed to:");
                    Player.SendMessage(p, s);
                    if (p != null)
                    {
                        Server.s.Log(p.name + " changed " + target.name + "'s logout message to:");
                    }
                    else
                    {
                        Server.s.Log("The Console changed " + target.name + "'s logout message to:");
                    }
                    Server.s.Log(s);
                }
                else
                {
                    if (!File.Exists("text/logout/" + t + ".txt"))
                    {
                        Player.SendMessage(p, "The player you specified does not exist!");
                        return;
                    }
                    else
                    {
                        CP437Writer.WriteAllText("text/logout/" + t + ".txt", s);
                    }
                    Player.SendMessage(p, "The logout message of " + t + " has been changed to:");
                    Player.SendMessage(p, s);
                    if (p != null)
                    {
                        Server.s.Log(p.name + " changed " + t + "'s logout message to:");
                    }
                    else
                    {
                        Server.s.Log("The Console changed " + t + "'s logout message to:");
                    }
                    Server.s.Log(s);
                }
            }
            /*
            if (number == 1)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos);
                string s = message.Substring(pos + 1);
                if (!File.Exists("text/logout/" + p.name + ".txt"))
                {
                    Player.SendMessage(p, "You do not exist!");
                    return;
                }
                else
                    File.WriteAllText("text/logout/" + p.name + ".txt", message);
                Player.SendMessage(p, "Your logout message has now been changed to:");
                Player.SendMessage(p, message);
                Server.s.Log(p.name + " changed their logout message to:");
                Server.s.Log(t);




            }*/
        }
    }
}
