/*
    Copyright 2011 MCForge
        
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
    public sealed class CmdMoveAll : Command2 {
        public override string name { get { return "MoveAll"; } }
        public override string shortcut { get { return "ma"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public override void Use(Player p, string message, CommandData data) {
            Level level = Matcher.FindLevels(p, message);
            if (level == null) return;
            
            Player[] players = PlayerInfo.Online.Items;           
            foreach (Player pl in players) { 
                if (pl.Rank < data.Rank)
                    PlayerActions.ChangeMap(pl, level.name);
                else
                    p.Message("You cannot move {0} &Sbecause they are of equal or higher rank", p.FormatNick(pl));
            }
        }
        
        public override void Help(Player p) { 
            p.Message("&T/MoveAll [level]");
            p.Message("&HMoves all players to that level."); 
        }
    }
}
