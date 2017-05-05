/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands.Fun {
    
    public sealed class CmdSlap : Command {
        public override string name { get { return "slap"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdSlap() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            int matches;
            Player who = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            
            if (who == null) {
                Level lvl = Matcher.FindLevels(p, message);
                if (lvl == null) {
                    Player.Message(p, "Could not find player or map specified"); return;
                }
                
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level == lvl && pl.Rank < p.Rank)
                        DoSlap(p, pl);
                }
                return;
            }
            if (p != null && who.Rank > p.Rank) {
                MessageTooHighRank(p, "slap", true); return;
            }
            DoSlap(p, who);
        }
        
        void DoSlap(Player p, Player who) {
            int x = who.Pos.BlockX, y = who.Pos.BlockY, z = who.Pos.BlockZ;
            if (y < 0) y = 0;
            Position pos = who.Pos;

            string src = p == null ? "(console)" : p.ColoredName;
            for (; y <= p.level.Height; y++) {
                if (!Block.Walkthrough(who.level.GetBlock(x, y, z)) && who.level.GetBlock(x, y, z) != Block.Invalid) {
                    pos.Y = (y - 1) * 32;
                    who.level.ChatLevel(who.ColoredName + " %Swas slapped into the roof by " + src);
                    who.SendPos(Entities.SelfID, pos, who.Rot);
                    return;
                }
            }
              
            pos.Y = 1000 * 32;
            who.level.ChatLevel(who.ColoredName + " %Swas slapped sky high by " + src);
            who.SendPos(Entities.SelfID, pos, who.Rot);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/slap [name]");
            Player.Message(p, "%HSlaps [name], knocking them into the air");
            Player.Message(p, "%T/slap [level]");
            Player.Message(p, "%HSlaps all players on [level] that are a lower rank, knocking them into the air");
        }
    }
}
