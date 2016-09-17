/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
    Dual-licensed under the Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands {    
    public sealed class CmdHasirc : Command {
        public override string name { get { return "hasirc"; } }
        public override string shortcut { get { return "irc"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdHasirc() { }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }

            if (Server.irc) {
                Player.Message(p, "IRC is &aEnabled%S.");
                Player.Message(p, "Location: " + Server.ircServer + " > " + Server.ircChannel);
            } else {
                Player.Message(p, "IRC is &cDisabled%S.");
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/hasirc");
            Player.Message(p, "%HOutputs whether the server has IRC enabled or not.");
            Player.Message(p, "%HIf IRC is enabled, server and channel are also displayed.");
        }
    }
}
