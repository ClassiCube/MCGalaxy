/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)

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
    public sealed class CmdHeartbeat : Command
    {
        public override string name { get { return "heartbeat"; } }
        public override string shortcut { get { return "beat"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public CmdHeartbeat() { }

        public override void Use(Player p, string message)
        {
            try
            {
                Heart.Pump(new ClassiCubeBeat());
            }
            catch (Exception e)
            {
                Server.s.Log("Error with MCGalaxy pump.");
                Server.ErrorLog(e);
            }
            Player.SendMessage(p, "Heartbeat pump sent.");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/heartbeat - Forces a pump for the ClassiCube heartbeat.  DEBUG PURPOSES ONLY.");
        }
    }
}
