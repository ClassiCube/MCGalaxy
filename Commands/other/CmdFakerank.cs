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
            Player who = PlayerInfo.FindOrShowMatches(p, args[0]);
            Group grp = Group.FindOrShowMatches(p, args[1]);            
            if (who == null || grp == null) return;
            
            if (grp.Permission == LevelPermission.Banned) {
                string banner = p == null ? "console" : p.ColoredName;
                Player.GlobalMessage(who.ColoredName + " %Swas &8banned %Sby " + banner + "%S.");
            } else {
                Player.GlobalMessage(who.ColoredName + "%S's rank was set to " + 
                                     grp.ColoredName + "%S. (Congratulations!)");
                who.SendMessage("You are now ranked " + grp.color + grp.name + "%S, type /help for your new set of commands.");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/fakerank <name> <rank> - Sends a fake rank change message.");
        }
    }
}
