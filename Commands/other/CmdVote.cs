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
    public sealed class CmdVote : Command
    {
        public override string name { get { return "vote"; } }
        public override string shortcut { get { return "vo"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVote() { }

        public override void Use(Player p, string message)
        {
            if (message != "")
            {
                if (!Server.voting)
                {
                    string temp = message.Substring(0, 1) == "%" ? "" : Server.DefaultColor;
                    Server.voting = true;
                    Server.NoVotes = 0;
                    Server.YesVotes = 0;
                    Player.GlobalMessage(" " + c.green + "VOTE: " + temp + message + "(" + c.green + "Yes " + Server.DefaultColor + "/" + c.red + "No" + Server.DefaultColor + ")");
                    System.Threading.Thread.Sleep(15000);
                    Server.voting = false;
                    Player.GlobalMessage("The vote is in! " + c.green + "Y: " + Server.YesVotes + c.red + " N: " + Server.NoVotes);
                    Player.players.ForEach(delegate(Player winners)
                    {
                        winners.voted = false;
                    });
                }
                else
                {
                    p.SendMessage("A vote is in progress!");
                }
            }
            else
            {
                Help(p);
            }
        }
        public override void Help(Player p)
        {
            p.SendMessage("/vote [message] - Obviously starts a vote!");
        }
    }
}
