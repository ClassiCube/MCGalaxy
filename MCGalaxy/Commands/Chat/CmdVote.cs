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
using MCGalaxy.Tasks;

namespace MCGalaxy.Commands.Chatting {
    public sealed class CmdVote : Command2 {
        public override string name { get { return "Vote"; } }
        public override string shortcut { get { return "vo"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            if (!MessageCmd.CanSpeak(p, name)) return;
            
            if (Server.voting) {
                p.Message("A vote is in progress!"); return;
            }
            Server.voting = true;
            Server.NoVotes = 0; Server.YesVotes = 0;
            Chat.MessageGlobal("&2 VOTE: &S{0} &S(&2Yes &S/&cNo&S)", message);
            Server.MainScheduler.QueueOnce(VoteCallback, null, TimeSpan.FromSeconds(15));
        }
        
        void VoteCallback(SchedulerTask task) {
            Server.voting = false;
            Chat.MessageGlobal("The votes are in! &2Y: {0} &cN: {1}", Server.YesVotes, Server.NoVotes);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) pl.voted = false;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Vote [message]");
            p.Message("&HStarts a vote for 15 seconds.");
            p.Message("&HType &TY &Hor &TN &Hinto chat to vote.");
        }
    }
}
