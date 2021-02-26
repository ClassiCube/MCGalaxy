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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdSPlace : DrawCmd {
        public override string name { get { return "SPlace"; } }
        public override string shortcut { get { return "set"; } }

        protected override string SelectionType { get { return "points"; } }
        protected override string PlaceMessage { get { return "Place or break two blocks to determine direction."; } }
        
        protected override DrawOp GetDrawOp(DrawArgs dArgs) {
            ushort distance = 0, interval = 0;
            Player p       = dArgs.Player;
            string message = dArgs.Message;
            if (message.Length == 0) { Help(p); return null; }
            
            string[] parts = message.SplitSpaces();
            if (!CommandParser.GetUShort(p, parts[0], "Distance", ref distance, 1)) return null;
            if (parts.Length > 1 && !CommandParser.GetUShort(p, parts[1], "Interval", ref interval, 1)) return null;

            if (interval >= distance) {
                p.Message("&WThe Interval cannot be greater than the distance."); return null;
            }

            SPlaceDrawOp op = new SPlaceDrawOp();
            op.Distance = distance; op.Interval = interval;
            return op;
        }
        
        protected override void GetMarks(DrawArgs dArgs, ref Vec3S32[] m) {
            Player p = dArgs.Player;
            if (m[0] == m[1]) { p.Message("No direction was selected"); m = null; }
        }
        
        protected override void GetBrush(DrawArgs dArgs) {
            SPlaceDrawOp op = (SPlaceDrawOp)dArgs.Op;
            int count = 1;
            if (op.Interval != 0) count++;
            dArgs.BrushArgs = dArgs.Message.Splice(count, 0);
        }

        public override void Help(Player p) {
            p.Message("&T/SPlace [distance] <interval>");
            p.Message("&HMeasures a set [distance] and places your held block at each end.");
            p.Message("&HOptionally place a block at set <interval> between them.");
        }
    }
}
