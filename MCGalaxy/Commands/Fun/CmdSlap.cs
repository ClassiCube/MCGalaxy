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
using MCGalaxy.Blocks;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdSlap : Command2 {
        public override string name { get { return "Slap"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            int matches;
            Player who = PlayerInfo.FindMatches(p, message, out matches);
            if (matches > 1) return;
            
            if (who == null) {
                Level lvl = Matcher.FindLevels(p, message);
                if (lvl == null) {
                    p.Message("Could not find player or map specified"); return;
                }
                
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level == lvl && pl.Rank < data.Rank) DoSlap(p, pl);
                }
                return;
            }
            
            if (!CheckRank(p, data, who, "slap", true)) return;
            DoSlap(p, who);
        }
        
        void DoSlap(Player p, Player who) {
            int x = who.Pos.BlockX, y = who.Pos.BlockY, z = who.Pos.BlockZ;
            if (y < 0) y = 0;            
            Position pos = who.Pos;
            
            if (who.level.IsValidPos(x, y, z)) {
                pos.Y = FindYAbove(who.level, (ushort)x, (ushort)y, (ushort)z);
                if (pos.Y != -1) {
                    Chat.MessageFromLevel(who, "λNICK &Swas slapped into the roof by " + p.ColoredName);
                    who.SendPos(Entities.SelfID, pos, who.Rot);
                    return;
                }
            }
            
            pos.Y = 1000 * 32;
            Chat.MessageFromLevel(who, "λNICK &Swas slapped sky high by " + p.ColoredName);
            who.SendPos(Entities.SelfID, pos, who.Rot);
        }
        
        static int FindYAbove(Level lvl, ushort x, ushort y, ushort z) {
            for (; y <= lvl.Height; y++) {
                BlockID above = lvl.GetBlock(x, (ushort)(y + 1), z);
                if (above == Block.Invalid) continue;
                if (!CollideType.IsSolid(lvl.CollideType(above))) continue;
                
                int posY = (y + 1) * 32 - 6;
                BlockDefinition def = lvl.GetBlockDef(above);
                if (def != null) posY += def.MinZ * 2;
                
                return posY;
            }
            return -1;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Slap [name]");
            p.Message("&HSlaps [name], knocking them into the air");
            p.Message("&T/Slap [level]");
            p.Message("&HSlaps all players on [level] that are a lower rank, knocking them into the air");
        }
    }
}
