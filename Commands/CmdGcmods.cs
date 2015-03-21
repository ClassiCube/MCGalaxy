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
namespace MCGalaxy.Commands
{
    public sealed class CmdGcmods : Command
    {
        public override string name { get { return "gcmods"; } }
        public override string shortcut { get { return "gcmod"; } }
        public override string type { get { return "information"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdGcmods() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            string gcmodlist = "";
            string tempgcmods;
            foreach (string gcmod in Server.GCmods)
            {
                tempgcmods = gcmod.Substring(0, 1);
                tempgcmods = tempgcmods.ToUpper() + gcmod.Remove(0, 1);
                gcmodlist += tempgcmods + ", ";
            }
            gcmodlist = gcmodlist.Remove(gcmodlist.Length - 2);
            Player.SendMessage(p, "&9MCGalaxy Global Chat Moderation Team: " + Server.DefaultColor + gcmodlist + "&e.");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/gcmods - Displays the list of MCGalaxy global chat moderators.");
        }
    }
}