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
using System;
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdRainbow : Command {
        public override string name { get { return "rainbow"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRainbow() { }

        public override void Use(Player p, string message) {
            Player.Message(p, "Place two blocks to determine the edges.");
            p.MakeSelection(2, null, DoRainbow);
        }
        
        bool DoRainbow(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            return DrawOp.DoDrawOp(new RainbowDrawOp(), null, p, marks);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/rainbow");
            Player.Message(p, "%HTaste the rainbow");
        }
    }
}
