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
    public sealed class CmdFakeRank : Command
    {
        public override string name { get { return "fakerank"; } }
        public override string shortcut { get { return "frk"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (message == "" || args.Length < 2) { Help(p); return; }
            Player who = PlayerInfo.FindMatches(p, args[0]);
            Group grp = Group.FindMatches(p, args[1]);            
            if (who == null || grp == null) return;
            
            if (grp.Permission == LevelPermission.Banned) {
                string banner = p == null ? "console" : p.ColoredName;
                Chat.MessageAll("{0} %Swas &8banned %Sby {1}%S.", 
                                who.ColoredName, banner);
            } else {
                Chat.MessageAll("{0}%S's rank was set to {1}%S. (Congratulations!)", 
            	                who.ColoredName, grp.ColoredName);
                Player.Message(who, "You are now ranked {0}%S, type /help for your new set of commands.", 
            	               grp.ColoredName);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/fakerank [name] [rank]");
            Player.Message(p, "%HSends a fake rank change message.");
        }
    }
}
