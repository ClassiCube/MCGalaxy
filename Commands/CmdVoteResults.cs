/*
	Copyright 2011 MCGalaxy
		
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
    /// <summary>
    /// This is the command /voteresults
    /// use /help voteresults in-game for more info
    /// </summary>
    public sealed class CmdVoteResults : Command
    {
        public override string name { get { return "voteresults"; } }
        public override string shortcut { get { return "vr"; } }
        public override string type { get { return ""; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdVoteResults() { }
        public override void Use(Player p, string message)
        {
            Player who = null;
            if (message == "") { who = p; message = p.name; } else { who = Player.Find(message); }
            if (Server.YesVotes >= 1 || Server.NoVotes >= 1)
            {
                p.SendMessage(c.green + "Y: " + Server.YesVotes + c.red + " N: " + Server.NoVotes);
                return;
            }
            else
            {
                p.SendMessage("There hasn't been a vote yet!");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/voteresults - see the results of the last vote!");
        }
    }
}