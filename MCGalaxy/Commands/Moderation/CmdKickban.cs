/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdKickban : Command {
        public override string name { get { return "kickban"; } }
        public override string shortcut { get { return "kb"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdKickban() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Command.all.Find("ban").Use(p, message);
            Command.all.Find("kick").Use(p, message);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/kickban [player] <reason>");
            Player.Message(p, "%HKicks and bans a player with an optional message.");
        }
    }
}
