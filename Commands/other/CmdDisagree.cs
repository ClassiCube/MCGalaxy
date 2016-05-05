/*
    Written by Jack1312
  
	Copyright 2011 MCForge
		
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
namespace MCGalaxy.Commands
{
    public sealed class CmdDisagree : Command
    {
        public override string name { get { return "disagree"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdDisagree() { }

        public override void Use(Player p, string message)
        {
        	if (p == null) { MessageInGameOnly(p); return; }
            if (!Server.agreetorulesonentry)
            {
                Player.Message(p, "This command can only be used if agree-to-rules-on-entry is enabled in the console!");
                return;
            }
            if (p.group.Permission > LevelPermission.Guest)
            {
                Player.Message(p, "Your awesomeness prevents you from using this command");
                return;
            }
            const string msg = "If you don't agree with the rules, consider playing elsewhere.";
            p.LeaveServer(msg, msg);
        }

        public override void Help(Player p)
        {
            Player.Message(p, "/disagree - Disagree to the rules when entering the server");
        }
    }
}
