/*
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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Undo;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {   
    public sealed class CmdRedo : Command2 {   
        public override string name { get { return "Redo"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length > 0) { Help(p); return; }
            PerformRedo(p);
        }
        
        static void PerformRedo(Player p) {
            UndoDrawOpEntry[] entries = p.DrawOps.Items;
            if (entries.Length == 0) {
                p.Message("You have no &T/Undo &Sor &T/Undo [seconds] &Sto redo."); return;
            }
            
            for (int i = entries.Length - 1; i >= 0; i--) {
                UndoDrawOpEntry entry = entries[i];
                if (entry.DrawOpName != "UndoSelf") continue;
                p.DrawOps.Remove(entry);
                
                RedoSelfDrawOp op = new RedoSelfDrawOp();
                op.Start = entry.Start; op.End = entry.End;
                DrawOpPerformer.Do(op, null, p, new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal });
                p.Message("Redo performed.");
                return;
            }          
            p.Message("No &T/Undo &Sor &T/Undo [timespan] &Scalls were " +
                               "found in the last 200 draw operations.");
        }

        public override void Help(Player p) {
            p.Message("&T/Redo");
            p.Message("&HRedoes last &T/Undo &Hor &T/Undo [timespan] &Hyou performed");
        }
    }
}
