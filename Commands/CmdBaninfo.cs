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
namespace MCGalaxy.Commands
{
    public sealed class CmdBaninfo : Command
    {
        public override string name { get { return "baninfo"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdBaninfo() { }

        public override void Use(Player p, string message)
        {
            string[] data;
            if (message == "") { Help(p); return; }
            if (message.Length <= 3) { Help(p); }
            else
            {
                if (Ban.Isbanned(message))
                {
                    data = Ban.Getbandata(message);
                    // string[] end = { bannedby, reason, timedate, oldrank, stealth };
                    // usefull to know :-)
                    string reason = data[1].Replace("%20", " ");
                    string datetime = data[2].Replace("%20", " ");
                    Player.SendMessage(p, "&9User: &e" + message);
                    Player.SendMessage(p, "&9Banned by: &e" + data[0]);
                    Player.SendMessage(p, "&9Reason: &e" + reason);
                    Player.SendMessage(p, "&9Date and time: &e" + datetime);
                    Player.SendMessage(p, "&9Old rank: &e" + data[3]);
                    string stealth; if (data[4] == "true") stealth = "&aYes"; else stealth = "&cNo";
                    Player.SendMessage(p, "&9Stealth banned: " + stealth);
                }
                else if (!Group.findPerm(LevelPermission.Banned).playerList.Contains(message)) Player.SendMessage(p, "That player isn't banned");
                else if (!Ban.Isbanned(message)) Player.SendMessage(p, "Couldn't find ban info about " + message + ".");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/baninfo <player> - returns info about banned player.");
        }
    }
}