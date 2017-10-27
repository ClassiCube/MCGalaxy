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
    public sealed class CmdAbort : Command {
        public override string name { get { return "Abort"; } }
        public override CommandAlias[] Aliases {

        	get { return new[] { new CommandAlias("a"), new CommandAlias("cancel"), new CommandAlias("nvm") }; }

        }
        
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            p.ClearBlockchange();
            p.painting = false;
            p.cmdTimer = false;
            p.staticCommands = false;
            p.deleteMode = false;
            p.ModeBlock = ExtBlock.Air;
            p.aiming = false;
            p.onTrain = false;
            p.isFlying = false;
            p.BrushName = "normal";
            p.DefaultBrushArgs = "";
            p.Transform = NoTransform.Instance;
            
            lock (p.level.queueLock)
                p.level.blockqueue.RemoveAll(b => (int)((b >> 9) & Player.SessionIDMask) == p.SessionID);
            Player.Message(p, "Every toggle or action was aborted.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Abort");
            Player.Message(p, "%HCancels an action.");
        }
    }
}
