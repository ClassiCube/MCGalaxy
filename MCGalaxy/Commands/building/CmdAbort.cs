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
using MCGalaxy.Drawing.Transforms;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdAbort : Command2 {
        public override string name { get { return "Abort"; } }
        public override string shortcut { get { return "a"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            p.ClearBlockchange();
            p.painting = false;
            p.checkingBotInfo = false;
            p.cmdTimer = false;
            p.staticCommands = false;
            p.deleteMode = false;
            p.ModeBlock = Block.Invalid;
            p.onTrain   = false;
            p.isFlying  = false;
            p.BrushName = "normal";
            p.DefaultBrushArgs = "";
            p.Transform = NoTransform.Instance;
            
            p.level.blockqueue.RemoveAll(p);
            if (p.weapon != null) p.weapon.Disable();
            p.Message("Every toggle or action was aborted.");
        }
        
        public override void Help(Player p) {
            p.Message("&T/Abort");
            p.Message("&HCancels an action.");
        }
    }
}
