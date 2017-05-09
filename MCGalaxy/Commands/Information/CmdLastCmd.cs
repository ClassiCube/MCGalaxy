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

namespace MCGalaxy.Commands.Info {
    public sealed class CmdLastCmd : Command {
        public override string name { get { return "lastcmd"; } }
        public override string shortcut { get { return "last"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLastCmd() { }

        public override void Use(Player p, string message)
        {
            if (message == "") {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (Entities.CanSee(p, pl))
                        ShowLastCommand(p, pl);
                }
            } else {
                Player who = PlayerInfo.FindMatches(p, message);
                if (who != null)
                    ShowLastCommand(p, who);
            }
        }
        
        static void ShowLastCommand(Player p, Player who) {
            if (who.lastCMD == "") {
                Player.Message(p, "{0} %Shas not used any commands yet.", 
                               who.ColoredName);
            } else {
                TimeSpan delta = DateTime.UtcNow - who.lastCmdTime;
                Player.Message(p, "{0} %Slast used \"{1}\" {2} ago", 
                               who.ColoredName, who.lastCMD, delta.Shorten(true));
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/last [user]");
            Player.Message(p, "%H Shows last command used by [user]");
            Player.Message(p, "%T/last");
            Player.Message(p, "%HShows last commands for all users (SPAMMY)");
        }
    }
}
