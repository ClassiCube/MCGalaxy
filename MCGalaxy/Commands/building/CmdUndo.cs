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
    public class CmdUndo : Command {
        public override string name { get { return "Undo"; } }
        public override string shortcut { get { return "u"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can undo physics") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { UndoLastDrawOp(p); return; }
            string[] parts = message.SplitSpaces();
            bool undoPhysics = parts[0].CaselessEq("physics");
            
            TimeSpan delta = GetDelta(p, p.name, parts, undoPhysics ? 1 : 0);
            if (delta == TimeSpan.MinValue || (!undoPhysics && parts.Length > 1)) {
                Player.Message(p, "If you are trying to undo another player, use %T/UndoPlayer");
                return;
            }
            
            if (undoPhysics) { UndoPhysics(p, delta); }
            else { UndoSelf(p, delta); }
        }
        
        void UndoLastDrawOp(Player p) {
            UndoDrawOpEntry[] entries = p.DrawOps.Items;
            if (entries.Length == 0) {
                Player.Message(p, "You have no draw operations to undo.");
                Player.Message(p, "Try using %T/Undo [seconds] %Sinstead.");
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
                Player.Message(p, "Undo performed.");
                return;
            }
            
            Player.Message(p, "Unable to undo any draw operations, as all of the " +
                               "past 50 draw operations are %T/Undo %Sor %T/Undo [seconds].");
            Player.Message(p, "Try using %T/Undo [seconds] %Sinstead.");
        }
        
        void UndoPhysics(Player p, TimeSpan delta) {
            if (!CheckExtraPerm(p, 1)) return;
            if (p != null && !p.group.CanExecute("physics")) {
                Player.Message(p, "You can only undo physics if you can use %T/Physics."); return;
            }
            
            CmdPhysics.SetPhysics(p.level, 0);
            UndoPhysicsDrawOp op = new UndoPhysicsDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            DrawOpPerformer.Do(op, null, p, new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal } );
            
            Chat.MessageGlobal("Physics were undone &b{0}", delta.Shorten());
            Logger.Log(LogType.UserActivity, "Physics were undone &b" + delta.Shorten());
            p.level.Save(true);
        }
        
        void UndoSelf(Player p, TimeSpan delta) {
            UndoDrawOp op = new UndoSelfDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = p.name; op.ids = NameConverter.FindIds(p.name);
            
            DrawOpPerformer.Do(op, null, p, new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal });
            if (op.found) {
                Player.Message(p, "Undid your changes for the past &b{0}", delta.Shorten(true));
                Logger.Log(LogType.UserActivity, "{0} undid their own actions for the past {1}", 
                           p.name, delta.Shorten(true));
            } else {
                Player.Message(p, "No changes found by you in the past &b{0}", delta.Shorten(true));
            }
        }
        
                
        const int undoMax = -1; // allows everything to be undone.
        internal static TimeSpan GetDelta(Player p, string name, string[] parts, int offset) {
            TimeSpan delta = TimeSpan.Zero;
            string timespan = parts.Length > offset ? parts[parts.Length - 1] : "30m";
            bool canAll = p == null || p.name.CaselessEq(name) || p.group.MaxUndo == undoMax;            
            
            if (timespan.CaselessEq("all")) {
                return TimeSpan.FromSeconds(canAll ? int.MaxValue : p.group.MaxUndo);
            } else if (!CommandParser.GetTimespan(p, timespan, ref delta, "undo the past", "s")) {
                return TimeSpan.MinValue;
            }

            if (delta.TotalSeconds == 0) 
                delta = TimeSpan.FromMinutes(90);
            if (!canAll && delta.TotalSeconds > p.group.MaxUndo) {
                Player.Message(p, "{0}%Ss may only undo up to {1} seconds.",
                               p.group.ColoredName, p.group.MaxUndo);
                return TimeSpan.FromSeconds(p.group.MaxUndo);
            }
            return delta;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Undo %H- Undoes your last draw operation");
            Player.Message(p, "%T/Undo [timespan]");
            Player.Message(p, "%HUndoes your blockchanges in the past [timespan]");
            if (p == null || p.group.MaxUndo == -1 || p.group.MaxUndo == int.MaxValue)
                Player.Message(p, "%H  if <timespan> is all, &cundoes for 68 years");
            Player.Message(p, "%T/Undo physics [seconds] %H- Undoes physics on current map");
        }
    }
}
