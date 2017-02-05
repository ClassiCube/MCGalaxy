/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
 
namespace MCGalaxy.Commands {
    public sealed class CmdVoteKick : Command {
        public override string name { get { return "votekick"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVoteKick() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "" || message.IndexOf(' ') != -1) { Help(p); return; }
            if (!MessageCmd.CanSpeak(p, name)) return;

            if (Server.voteKickInProgress) { Player.Message(p, "Please wait for the current vote to finish!"); return; }

            Player who = PlayerInfo.FindMatches(p, message);
            if (who == null) return;

            if (who.Rank >= p.Rank) {
                Player.SendChatFrom(p, p.ColoredName + " %Stried to votekick " + who.ColoredName + " %Sbut failed!", false);
                return;
            }

            Chat.MessageOps(p.ColoredName + " %Sused &a/votekick");
            Chat.MessageAll("&9A vote to kick {0} %Shas been called!", who.ColoredName);
            Chat.MessageAll("&9Type &aY %Sor &cN %Sto vote.");

            // 1/3rd of the players must vote or nothing happens
            // Keep it at 0 to disable min number of votes
            Server.voteKickVotesNeeded = 3; //(int)(PlayerInfo.players.Count / 3) + 1;
            Server.voteKickInProgress = true;
            Server.MainScheduler.QueueOnce(VoteTimerCallback, who.name, TimeSpan.FromSeconds(30));
        }
        
        void VoteTimerCallback(SchedulerTask task) {
            Server.voteKickInProgress = false;
            int votesYes = 0, votesNo = 0;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.voteKickChoice == VoteKickChoice.Yes) votesYes++;
                if (pl.voteKickChoice == VoteKickChoice.No) votesNo++;
                pl.voteKickChoice = VoteKickChoice.NotYetVoted;
            }
            
            string name = (string)task.State;
            Player who = PlayerInfo.FindExact(name);
            if (who == null) {
                Chat.MessageAll("{0} was not kicked, as they already left the server.", name); return;
            }

            int netVotesYes = votesYes - votesNo;
            // Should we also send this to players?
            Chat.MessageOps("Vote Ended. Results: &aY: " + votesYes + " &cN: " + votesNo);
            Server.s.Log("VoteKick results for " + who.DisplayName + ": " + votesYes + " yes and " + votesNo + " no votes.");

            if (votesYes + votesNo < Server.voteKickVotesNeeded) {
                Chat.MessageAll("Not enough votes were made. {0} %Sshall remain!", who.ColoredName);
            } else if (netVotesYes > 0) {
                Chat.MessageAll("The people have spoken, {0} %Sis gone!", who.ColoredName);
                who.Kick("Vote-Kick: The people have spoken!");
            } else {
                Chat.MessageAll("{0} %Sshall remain!", who.ColoredName);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/votekick [player]");
            Player.Message(p, "%HCalls a 30sec vote to kick [player]");
        }
    }
}
