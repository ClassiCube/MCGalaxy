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
using MCGalaxy.Commands.World;
using MCGalaxy.DB;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Undo;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public class CmdUndo : Command2 {
        public override string name { get { return "Undo"; } }
        public override string shortcut { get { return "u"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can undo physics") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { UndoLastDrawOp(p); return; }
            string[] parts = message.SplitSpaces();
            bool undoPhysics = parts[0].CaselessEq("physics");
            
            TimeSpan delta = GetDelta(p, p.name, parts, undoPhysics ? 1 : 0);
            if (delta == TimeSpan.MinValue || (!undoPhysics && parts.Length > 1)) {
                p.Message("If you are trying to undo another player, use &T/UndoPlayer");
                return;
            }
            
            if (undoPhysics) { UndoPhysics(p, data, delta); }
            else { UndoSelf(p, delta); }
        }
        
        void UndoLastDrawOp(Player p) {
            UndoDrawOpEntry[] entries = p.DrawOps.Items;
            if (entries.Length == 0) {
                p.Message("You have no draw operations to undo.");
                p.Message("Try using &T/Undo [timespan] &Sinstead.");
                return;
            }
            
            for (int i = entries.Length - 1; i >= 0; i--) {
                UndoDrawOpEntry entry = entries[i];
                if (entry.DrawOpName == "UndoSelf") continue;
                p.DrawOps.Remove(entry);
                
                UndoSelfDrawOp op = new UndoSelfDrawOp();
                op.who = p.name; op.ids = NameConverter.FindIds(p.name);
                
                op.Start = entry.Start; op.End = entry.End;
                DrawOpPerformer.Do(op, null, p, new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal } );
                p.Message("Undo performed.");
                return;
            }
            
            p.Message("Unable to undo any draw operations, as all of the " +
                               "past 50 draw operations are &T/Undo &Sor &T/Undo [timespan]");
            p.Message("Try using &T/Undo [timespan] &Sinstead");
        }
        
        void UndoPhysics(Player p, CommandData data, TimeSpan delta) {
            if (!CheckExtraPerm(p, data, 1)) return;
            if (!p.CanUse("Physics")) {
                p.Message("&WYou can only undo physics if you can use &T/Physics"); return;
            }
            
            CmdPhysics.SetPhysics(p.level, 0);
            UndoPhysicsDrawOp op = new UndoPhysicsDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            DrawOpPerformer.Do(op, null, p, new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal } );
            
            p.level.Message("Physics were undone &b" + delta.Shorten());
            Logger.Log(LogType.UserActivity, "Physics were undone &b" + delta.Shorten());
            p.level.Save(true);
        }
        
        void UndoSelf(Player p, TimeSpan delta) {
            UndoDrawOp op = new UndoSelfDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = p.name; op.ids = NameConverter.FindIds(p.name);
            
            DrawOpPerformer.Do(op, null, p, new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal });
            if (op.found) {
                p.Message("Undid your changes for the past &b{0}", delta.Shorten(true));
                Logger.Log(LogType.UserActivity, "{0} undid their own actions for the past {1}", 
                           p.name, delta.Shorten(true));
            } else {
                p.Message("No changes found by you in the past &b{0}", delta.Shorten(true));
            }
        }
        
                
        internal static TimeSpan GetDelta(Player p, string name, string[] parts, int offset) {
            TimeSpan delta = TimeSpan.Zero;
            string timespan = parts.Length > offset ? parts[parts.Length - 1] : "30m";
            bool self = p.name.CaselessEq(name);
            
            if (timespan.CaselessEq("all")) {
                return self ? TimeSpan.FromSeconds(int.MaxValue) : p.group.MaxUndo;
            } else if (!CommandParser.GetTimespan(p, timespan, ref delta, "undo the past", "s")) {
                return TimeSpan.MinValue;
            }

            if (delta.TotalSeconds == 0) 
                delta = TimeSpan.FromMinutes(90);
            if (!self && delta > p.group.MaxUndo) {
                p.Message("{0}&Ss may only undo up to {1}",
                          p.group.ColoredName, p.group.MaxUndo.Shorten(true, true));
                return p.group.MaxUndo;
            }
            return delta;
        }

        public override void Help(Player p) {
            p.Message("&T/Undo &H- Undoes your last draw operation");
            p.Message("&T/Undo [timespan]");
            p.Message("&HUndoes your blockchanges in the past [timespan]");
            p.Message("&T/Undo physics [timespan] &H- Undoes physics on current map");
        }
    }
}
