/* 
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
//This seems broken... - Rasmusolle
using System;
namespace MCGalaxy
{
    public sealed class CmdCTF : Command
    {
        public override string name { get { return "ctf"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "game"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdCTF() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Server.s.Log("You must be in-game");
                return;
            }
            if ((message != null && String.IsNullOrEmpty(message))) { Help(p); return; }
            if (message == "start")
            {
                //Prevent null errors :/
                if (Server.ctf == null)
                {
                    Player.SendMessage(p, "Starting CTF..");
                    Server.ctf = new Auto_CTF();
                    Player.GlobalMessage("A CTF GAME IS STARTING AT CTF! TYPE /goto CTF to join!");
                }
                else if (Server.ctf.started)
                {
                    Player.SendMessage(p, "A ctf is already in session!");
                    return;
                }
                else if (!Server.ctf.started)
                {
                    Server.ctf.Start();
                    Player.GlobalMessage("A CTF GAME IS STARTING AT CTF! TYPE /goto CTF to join!");
                }
            }
            if (message == "stop")
            {
                //Prevent null error :/
                if (Server.ctf == null)
                {
                    Player.SendMessage(p, "A ctf game isnt active!");
                    return;
                }
                else if (!Server.ctf.started)
                {
                    Player.SendMessage(p, "A ctf game isnt active!");
                    return;
                }
                else
                    Server.ctf.Stop();
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/ctf <start/stop> - Start / Stop the ctf game!");
        }
    }
}
