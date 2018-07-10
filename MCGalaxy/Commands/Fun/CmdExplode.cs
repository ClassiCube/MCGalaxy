/*
    Written by Jack1312
  
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
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdExplode : Command2 {
        public override string name { get { return "Explode"; } }
        public override string shortcut { get { return "ex"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            if (!(args.Length == 1 || args.Length == 3)) { Help(p); return; }
            if (message.CaselessEq("me") && p != null) args[0] = p.name;
            
            ushort x = 0, y = 0, z = 0;
            if (args.Length == 1) {
                Player who = PlayerInfo.FindMatches(p, args[0]);
                if (who == null) return;
                
                x = (ushort)who.Pos.BlockX;
                y = (ushort)who.Pos.BlockY;
                z = (ushort)who.Pos.BlockZ;
                if (DoExplode(p, who.level, x, y, z))
                    p.Message(who.ColoredName + " %Shas been exploded!");
            } else if (args.Length == 3) {
                if (!CommandParser.GetUShort(p, args[0], "X", ref x)) return;
                if (!CommandParser.GetUShort(p, args[1], "Y", ref y)) return;
                if (!CommandParser.GetUShort(p, args[2], "Z", ref z)) return;

                if (y >= p.level.Height) y = (ushort)(p.level.Height - 1);
                if (DoExplode(p, p.level, x, y, z))
                    p.Message("An explosion was made at {0}, {1}, {2}).", x, y, z);
            } else {
                Help(p);
            }
        }
        
        static bool DoExplode(Player p, Level lvl, ushort x, ushort y, ushort z) {
            if (lvl.physics < 3 || lvl.physics == 5) {
                p.Message("The physics on this level are not sufficient for exploding!"); return false;
            }
            
            BlockID old = lvl.GetBlock(x, y, z);
            if (!lvl.CheckAffectPermissions(p, x, y, z, old, Block.TNT)) return false;
            lvl.MakeExplosion(x, y, z, 1);
            return true;
        }
        
        public override void Help(Player p) {
            p.Message("%T/Explode %H- Satisfying all your exploding needs :)");
            p.Message("%T/Explode me %H- Explodes at your location");
            p.Message("%T/Explode [Player] %H- Explode the specified player");
            p.Message("%T/Explode [x y z] %H- Explode at the specified co-ordinates");
        }
    }
}
