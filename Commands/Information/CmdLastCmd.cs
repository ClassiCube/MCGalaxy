/*
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
    public sealed class CmdLastCmd : Command
    {
        public override string name { get { return "lastcmd"; } }
        public override string shortcut { get { return "last"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLastCmd() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
            	Player[] players = PlayerInfo.Online;
                foreach (Player pl in players)
                {
                    if (pl.lastCMD.Contains("setpass") || pl.lastCMD.Contains("pass"))
                    {
                        pl.lastCMD = "";
                    }
                    if (Player.CanSee(p, pl))
                    {
                        Player.SendMessage(p, pl.color + pl.DisplayName + " %Slast used \"" + pl.lastCMD + "\"");
                    }
                }
            }
            else
            {
                Player who = PlayerInfo.FindOrShowMatches(message);
                if (who == null) return;
                if (who.lastCMD.Contains("setpass") || who.lastCMD.Contains("pass"))
                {
                    who.lastCMD = "";
                }
                Player.SendMessage(p, who.color + who.DisplayName + " %Slast used \"" + who.lastCMD + "\"");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/last [user] - Shows last command used by [user]");
            Player.SendMessage(p, "/last by itself will show all last commands (SPAMMY)");
        }
    }
}
