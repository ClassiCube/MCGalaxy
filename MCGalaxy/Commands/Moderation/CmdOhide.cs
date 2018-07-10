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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdOHide : Command2 {
        public override string name { get { return "OHide"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            
            string[] args = message.SplitSpaces();
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (!CheckRank(p, who, "hide", false)) return;
            
            bool own = args.Length >= 2 && args[1].CaselessEq("myrank");
            if (own) who.oHideRank = data.Rank;
            
            Command.Find("Hide").Use(who, "", data);
            p.Message("Hidden {0} %Sfrom players below {1} rank",
                           who.ColoredName, own ? "your" : "their");
        }

        public override void Help(Player p) {
            p.Message("%T/OHide [player] %H- Hides/unhides the player specified.");
            p.Message("%T/OHide [player] myrank %H- Hides/unhides the player specified to players below your rank.");
            p.Message("%HOnly works on players of lower rank.");
        }
    }
}
