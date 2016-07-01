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
namespace MCGalaxy.Commands
{
    public sealed class CmdVote : Command
    {
        public override string name { get { return "vote"; } }
        public override string shortcut { get { return "vo"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdVote() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (p.muted) { Player.Message(p, "You cannot start votes while muted."); }
            
            if (Server.voting) {
                Player.Message(p, "A vote is in progress!"); return;
            }
            Server.voting = true;
            Server.NoVotes = 0; Server.YesVotes = 0;
            Player.GlobalMessage(Colors.green + " VOTE: %S" + message + "%S(" + Colors.green + "Yes" + " %S/ " + Colors.red + "No" + "%S)");
            System.Threading.Thread.Sleep(15000);
            
            Server.voting = false;
            Player.GlobalMessage("The votes are in! " + Colors.green + "Y: " + Server.YesVotes + Colors.red + " N: " + Server.NoVotes);
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) pl.voted = false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/vote [message]");
            Player.Message(p, "%HStarts a vote for 15 seconds.");
        }
    }
}
