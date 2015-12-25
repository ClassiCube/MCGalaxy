/*
	Copyright 2010 MCLawl (Modified for use with MCGalaxy)
	
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
namespace MCGalaxy.Commands
{
    public sealed class CmdCrashServer : Command
    {
        public override string name { get { return "crashserver"; } }
        public override string shortcut { get { return "crash"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdCrashServer() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            Chat.GlobalMessageOps(p.color + p.DisplayName + Server.DefaultColor + " used &b/crashserver");
            p.Kick("Server crash! Error code 0x" + Convert.ToString(p.random.Next(int.MinValue, int.MaxValue), 16).ToUpper());
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/crashserver - Crash the server with a generic error");
        }
    }

}
