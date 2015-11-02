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
    public sealed class CmdLoginMessage : Command
    {
        public override string name { get { return "loginmessage"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLoginMessage() { }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/loginmessage [Player] [Message] - Customize your login message.");
            if(Server.mono)
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
                    if (!File.Exists("text/login/" + target.name + ".txt"))
                    {
                        Player.SendMessage(p, "The player you specified does not exist!");
                        return;
                    }
                    else
                    {
                        File.WriteAllText("text/login/" + target.name + ".txt", s);
                    }
                    Player.SendMessage(p, "The login message of " + target.color + target.DisplayName + Server.DefaultColor + " has been changed to:");
                    Player.SendMessage(p, s);
                    if (p != null)
                    {
                        Server.s.Log(p.name + " changed " + target.name + "'s login message to:");
                    }
                    else
                    {
                        Server.s.Log("The Console changed " + target.name + "'s login message to:");
                    }
                    Server.s.Log(s);
                }
                else
                {
                    if (!File.Exists("text/login/" + t + ".txt"))
                    {
                        Player.SendMessage(p, "The player you specified does not exist!");
                        return;
                    }
                    else
                    {
                        File.WriteAllText("text/login/" + t + ".txt", s);
                    }
                    Player.SendMessage(p, "The login message of " + t + " has been changed to:");
                    Player.SendMessage(p, s);
                    if (p != null)
                    {
                        Server.s.Log(p.name + " changed " + t + "'s login message to:");
                    }
                    else
                    {
                        Server.s.Log("The Console changed " + t + "'s login message to:");
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
                if (!File.Exists("text/login/" + p.name + ".txt"))
                {
                    Player.SendMessage(p, "You do not exist!");
                    return;
                }
                else
                    File.WriteAllText("text/login/" + p.name + ".txt", message);
                Player.SendMessage(p, "Your login message has now been changed to:");
                Player.SendMessage(p, message);
                Server.s.Log(p.name + " changed their login message to:");
                Server.s.Log(s);




            }*/
        }
    }
}
