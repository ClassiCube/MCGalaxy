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
    public sealed class CmdVoteKick : Command
    {
        public override string name { get { return "votekick"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVoteKick() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "" || message.IndexOf(' ') != -1) { Help(p); return; }

            if (Server.voteKickInProgress) { Player.SendMessage(p, "Please wait for the current vote to finish!"); return; }

            Player who = PlayerInfo.FindOrShowMatches(p, message);
            if (who == null) return;

            if (who.group.Permission >= p.group.Permission)
            {
                Player.SendChatFrom(p, p.color + p.DisplayName + " " + Server.DefaultColor + "tried to votekick " + who.color + who.DisplayName + " " + Server.DefaultColor + "but failed!", false);
                return;
            }

            Chat.GlobalMessageOps(p.color + p.DisplayName + Server.DefaultColor + " used &a/votekick");
            Player.GlobalMessage("&9A vote to kick " + who.color + who.DisplayName + " " + Server.DefaultColor + "has been called!");
            Player.GlobalMessage("&9Type &aY " + Server.DefaultColor + "or &cN " + Server.DefaultColor + "to vote.");

            // 1/3rd of the players must vote or nothing happens
            // Keep it at 0 to disable min number of votes
            Server.voteKickVotesNeeded = 3; //(int)(PlayerInfo.players.Count / 3) + 1;
            Server.voteKickInProgress = true;

            System.Timers.Timer voteTimer = new System.Timers.Timer(30000);

            voteTimer.Elapsed += delegate
            {
                voteTimer.Stop();

                Server.voteKickInProgress = false;

                int votesYes = 0;
                int votesNo = 0;

                Player[] players = PlayerInfo.Online.Items; 
                foreach (Player pl in players) {
                    // Tally the votes
                    if (pl.voteKickChoice == VoteKickChoice.Yes) votesYes++;
                    if (pl.voteKickChoice == VoteKickChoice.No) votesNo++;
                    // Reset their choice
                    pl.voteKickChoice = VoteKickChoice.HasntVoted;
                }

                int netVotesYes = votesYes - votesNo;

                // Should we also send this to players?
                Chat.GlobalMessageOps("Vote Ended.  Results: &aY: " + votesYes + " &cN: " + votesNo);
                Server.s.Log("VoteKick results for " + who.DisplayName + ": " + votesYes + " yes and " + votesNo + " no votes.");

                if (votesYes + votesNo < Server.voteKickVotesNeeded)
                {
                    Player.GlobalMessage("Not enough votes were made. " + who.color + who.DisplayName + " " + Server.DefaultColor + "shall remain!");
                }
                else if (netVotesYes > 0)
                {
                    Player.GlobalMessage("The people have spoken, " + who.color + who.DisplayName + " " + Server.DefaultColor + "is gone!");
                    who.Kick("Vote-Kick: The people have spoken!");
                }
                else
                {
                    Player.GlobalMessage(who.color + who.DisplayName + " " + Server.DefaultColor + "shall remain!");
                }

                voteTimer.Dispose();
            };

            voteTimer.Start();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/votekick <player> - Calls a 30sec vote to kick <player>");
        }
    }
}
