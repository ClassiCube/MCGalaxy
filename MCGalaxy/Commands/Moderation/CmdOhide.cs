/*
    Copyright 2011 MCForge
    
    Written by Valek / MCLawl team
        
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
namespace MCGalaxy.Commands.Moderation
{
    public sealed class CmdOHide : Command
    {
        public override string name { get { return "ohide"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            
            string[] args = message.SplitSpaces();           
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p != null && who.Rank >= p.Rank) {
                MessageTooHighRank(p, "hide", false); return;
            }
            
            if (args.Length >= 2 && args[1].CaselessEq("myrank")) {
                who.oHideRank = p == null ? LevelPermission.Admin : p.Rank;
                Command.all.Find("hide").Use(who, "myrank");
                Player.Message(p, "Used /hide myrank on " + who.ColoredName + "%S.");
            } else {
                Command.all.Find("hide").Use(who, "");
                Player.Message(p, "Used /hide on " + who.ColoredName + "%S.");
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/ohide [player] %H- Hides/unhides the player specified.");
            Player.Message(p, "%T/ohide [player] myrank %H- Hides/unhides the player specified to players below your rank.");
            Player.Message(p, "%HOnly works on players of lower rank.");
        }
    }
}
