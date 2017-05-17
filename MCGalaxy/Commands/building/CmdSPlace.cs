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
using System;
using MCGalaxy.DB;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {    
    public sealed class CmdSPlace : Command {       
        public override string name { get { return "splace"; } }
        public override string shortcut { get { return "set"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdSPlace() { }

        public override void Use(Player p, string message) {
            ushort distance = 0, interval = 0;
            if (message == "") { Help(p); return; }
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            
            string[] parts = message.SplitSpaces();
            if (!CommandParser.GetUShort(p, parts[0], "Distance", ref distance)) return;
            if (parts.Length > 1 && !CommandParser.GetUShort(p, parts[1], "Interval", ref interval)) return;

            if (distance < 1) {
                Player.Message(p, "Enter a distance greater than 0."); return;
            }
            if (interval >= distance) {
                Player.Message(p, "The Interval cannot be greater than the distance."); return;
            }

            DrawArgs dArgs = default(DrawArgs);
            dArgs.distance = distance; dArgs.interval = interval;
            Player.Message(p, "Place or break two blocks to determine direction.");
            p.MakeSelection(2, dArgs, DoSPlace);
        }
        
        bool DoSPlace(Player p, Vec3S32[] m, object state, ExtBlock block) {
            DrawArgs dArgs = (DrawArgs)state;
            ushort distance = dArgs.distance, interval = dArgs.interval;
            if (m[0] == m[1]) { Player.Message(p, "No direction was selected"); return false; }
            
            int dirX = 0, dirY = 0, dirZ = 0;
            int dx = Math.Abs(m[1].X - m[0].X), dy = Math.Abs(m[1].Y - m[0].Y), dz = Math.Abs(m[1].Z - m[0].Z);
            if (dy > dx && dy > dz) 
                dirY = m[1].Y > m[0].Y ? 1 : -1;
            else if (dx > dz) 
                dirX = m[1].X > m[0].X ? 1 : -1;
            else 
                dirZ = m[1].Z > m[0].Z ? 1 : -1;
            
            ushort endX = (ushort)(m[0].X + dirX * distance);
            ushort endY = (ushort)(m[0].Y + dirY * distance);
            ushort endZ = (ushort)(m[0].Z + dirZ * distance);
            
            ExtBlock held = p.GetHeldBlock();
            p.level.UpdateBlock(p, endX, endY, endZ, held, BlockDBFlags.Drawn, true);
            
            if (interval > 0) {
                int x = m[0].X, y = m[0].Y, z = m[0].Z;
                int delta = 0;
                while (x >= 0 && y >= 0 && z >= 0 && x < p.level.Width && y < p.level.Height && z < p.level.Length && delta < distance) {
                    p.level.UpdateBlock(p, (ushort)x, (ushort)y, (ushort)z, held, BlockDBFlags.Drawn, true);
                    x += dirX * interval; y += dirY * interval; z += dirZ * interval;
                    delta = Math.Abs(x - m[0].X) + Math.Abs(y - m[0].Y) + Math.Abs(z - m[0].Z);
                }
            } else {
                p.level.UpdateBlock(p, (ushort)m[0].X, (ushort)m[0].Y, (ushort)m[0].Z, held, BlockDBFlags.Drawn, true);
            }

            Player.Message(p, "Placed stone blocks {0} apart.", interval > 0 ? interval : distance);
            return true;
        }
        
        struct DrawArgs { public ushort distance, interval; }

        public override void Help(Player p) {
            Player.Message(p, "%T/splace [distance] <interval>");
            Player.Message(p, "%HMeasures a set [distance] and places your held block at each end.");
            Player.Message(p, "%HOptionally place a block at set <interval> between them.");
        }
    }
}
