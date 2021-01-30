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
    public sealed class CmdTrust : Command2 {
        public override string name { get { return "Trust"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0 || message.IndexOf(' ') != -1) { Help(p); return; }
            Player target = PlayerInfo.FindMatches(p, message);
            if (target == null) return;
            
            target.ignoreGrief = !target.ignoreGrief;
            p.Message("{0}&S's trust status: " + target.ignoreGrief, p.FormatNick(target));
            target.Message("Your trust status was changed to: " + target.ignoreGrief);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Trust [name]");
            p.Message("&HTurns off the anti-grief for [name]");
        }
    }
}
