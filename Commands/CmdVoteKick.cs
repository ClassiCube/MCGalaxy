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
    public sealed class CmdVoteKick : Command
    {
        public override string name { get { return "votekick"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVoteKick() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (message == "" || message.IndexOf(' ') != -1) { Help(p); return; }

            if (Server.voteKickInProgress == true) { p.SendMessage("Please wait for the current vote to finish!"); return; }

            Player who = Player.Find(message);
            if (who == null)
            {
                Player.SendMessage(p, "Could not find player specified!");
                return;
            }

            if (who.group.Permission >= p.group.Permission)
            {
                Player.GlobalChat(p, p.color + p.name + " " + Server.DefaultColor + "tried to votekick " + who.color + who.name + " " + Server.DefaultColor + "but failed!", false);
                return;
            }

            Player.GlobalMessageOps(p.color + p.name + Server.DefaultColor + " used &a/votekick");
            Player.GlobalMessage("&9A vote to kick " + who.color + who.name + " " + Server.DefaultColor + "has been called!");
            Player.GlobalMessage("&9Type &aY " + Server.DefaultColor + "or &cN " + Server.DefaultColor + "to vote.");

            // 1/3rd of the players must vote or nothing happens
            // Keep it at 0 to disable min number of votes
            Server.voteKickVotesNeeded = 3; //(int)(Player.players.Count / 3) + 1;
            Server.voteKickInProgress = true;

            System.Timers.Timer voteTimer = new System.Timers.Timer(30000);

            voteTimer.Elapsed += delegate
            {
                voteTimer.Stop();

                Server.voteKickInProgress = false;

                int votesYes = 0;
                int votesNo = 0;

                Player.players.ForEach(delegate(Player pl)
                {
                    // Tally the votes
                    if (pl.voteKickChoice == VoteKickChoice.Yes) votesYes++;
                    if (pl.voteKickChoice == VoteKickChoice.No) votesNo++;
                    // Reset their choice
                    pl.voteKickChoice = VoteKickChoice.HasntVoted;
                });

                int netVotesYes = votesYes - votesNo;

                // Should we also send this to players?
                Player.GlobalMessageOps("Vote Ended.  Results: &aY: " + votesYes + " &cN: " + votesNo);
                Server.s.Log("VoteKick results for " + who.name + ": " + votesYes + " yes and " + votesNo + " no votes.");

                if (votesYes + votesNo < Server.voteKickVotesNeeded)
                {
                    Player.GlobalMessage("Not enough votes were made. " + who.color + who.name + " " + Server.DefaultColor + "shall remain!");
                }
                else if (netVotesYes > 0)
                {
                    Player.GlobalMessage("The people have spoken, " + who.color + who.name + " " + Server.DefaultColor + "is gone!");
                    who.Kick("Vote-Kick: The people have spoken!");
                }
                else
                {
                    Player.GlobalMessage(who.color + who.name + " " + Server.DefaultColor + "shall remain!");
                }

                voteTimer.Dispose();
            };

            voteTimer.Start();
        }
        public override void Help(Player p)
        {
            p.SendMessage("/votekick <player> - Calls a 30sec vote to kick <player>");
        }
    }
}
