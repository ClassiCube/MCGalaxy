/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Bots;
using MCGalaxy.Generator;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands.World {
    public sealed class CmdResizeLvl : Command2 {
        public override string name { get { return "ResizeLvl"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WResize"), new CommandAlias("WorldResize") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length < 4) { Help(p); return; }
            
            bool needConfirm;
            if (DoResize(p, args, data, out needConfirm)) return;
            
            if (!needConfirm) return;
            p.Message("Type &T/ResizeLvl {0} {1} {2} {3} confirm &Sif you're sure.",
                      args[0], args[1], args[2], args[3]);
        }
        
        internal static bool DoResize(Player p, string[] args, CommandData data, out bool needConfirm) {
            needConfirm = false;
            Level lvl = Matcher.FindLevels(p, args[0]);
            
            if (lvl == null) return true;
            if (!LevelInfo.Check(p, data.Rank, lvl, "resize this level")) return false;
            
            ushort x = 0, y = 0, z = 0;
            if (!MapGen.GetDimensions(p, args, 1, ref x, ref y, ref z)) return false;
            
            bool confirmed = args.Length > 4 && args[4].CaselessEq("confirm");
            if (!confirmed && (x < lvl.Width || y < lvl.Height || z < lvl.Length)) {
                p.Message("New level dimensions are smaller than the current dimensions, &Wyou will lose blocks&S.");
                needConfirm = true;
                return false;
            }
            
            LevelActions.Resize(ref lvl, x, y, z);
            return true;
        }
        
        public override void Help(Player p) {
            p.Message("&T/ResizeLvl [level] [width] [height] [length]");
            p.Message("&HResizes the given level.");
        }
    }
}
