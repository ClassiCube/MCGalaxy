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
using MCGalaxy.Undo;

namespace MCGalaxy.Commands.Building {
    public class CmdUndo : Command {
        public override string name { get { return "undo"; } }
        public override string shortcut { get { return "u"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can undo other players"),
                    new CommandPerm(LevelPermission.AdvBuilder, "+ can undo physics"),
                }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xundo", null, "all") }; }
        }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "player name")) return;
            int ignored = 0;
            if (message == "") {
                UndoSelf(p); return;
            } else if (p != null && int.TryParse(message, out ignored)) {
                message = p.name.ToLower() + " " + message;
            }
            
            string[] parts = message.Split(' ');
            bool undoPhysics = parts[0].CaselessEq("physics");
            Player who = undoPhysics ? null : PlayerInfo.Find(parts[0]);
            TimeSpan delta = GetDelta(p, who, parts.Length > 1 ? parts[1] : "30m");
            if (delta == TimeSpan.MinValue) return;
            
            if (parts.Length > 1 && parts[1].CaselessEq("update")) {
                UndoFormat.UpgradePlayerUndoFiles(parts[0]);
                Player.Message(p, "Updated undo files for " + parts[0] + " to the new binary format.");
                return;
            }

            Vec3S32[] marks = new Vec3S32[] { Vec3U16.MaxVal, Vec3U16.MaxVal };
            if (who != null)
                UndoOnlinePlayer(p, delta, who, marks);
            else if (undoPhysics)
                UndoLevelPhysics(p, delta);
            else
                UndoOfflinePlayer(p, delta, parts[0], marks);
        }
        
        void UndoSelf(Player p) {
            UndoDrawOpEntry[] entries = p.DrawOps.Items;
            if (entries.Length == 0) {
                Player.Message(p, "You have no draw operations to undo.");
                Player.Message(p, "Try using %T/undo <seconds> %Sinstead.");
                return;
            }
            
            for (int i = entries.Length - 1; i >= 0; i--) {
                UndoDrawOpEntry entry = entries[i];
                if (entry.DrawOpName == "UndoSelf") continue;
                p.DrawOps.Remove(entry);
                
                UndoSelfDrawOp op = new UndoSelfDrawOp();
                op.who = p;
                op.Start = entry.Start; op.End = entry.End;
                DrawOp.DoDrawOp(op, null, p, new Vec3S32[] { Vec3U16.MaxVal, Vec3U16.MaxVal } );
                Player.Message(p, "Undo performed.");
                return;
            }
            
            Player.Message(p, "Unable to undo any draw operations, as all of the " +
                               "past 50 draw operations are %T/undo%S or %T/undo <seconds>.");
            Player.Message(p, "Try using %T/undo <seconds> %Sinstead.");
        }
        
        void UndoLevelPhysics(Player p, TimeSpan delta) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, "undo physics.", 2); return; }
            if (p != null && !p.group.CanExecute("physics")) {
                Player.Message(p, "You can only undo physics if you can use /physics."); return;
            }
            CmdPhysics.SetPhysics(p.level, 0);
            UndoPhysicsDrawOp op = new UndoPhysicsDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            DrawOp.DoDrawOp(op, null, p, new Vec3S32[] { Vec3U16.MaxVal, Vec3U16.MaxVal } );
            
            Chat.MessageAll("Physics were undone &b{0}", delta.Shorten());
            Server.s.Log("Physics were undone &b" + delta.Shorten());
            p.level.Save(true);
        }
        
        
        const int undoMax = -1; // allows everything to be undone.
        protected TimeSpan GetDelta(Player p, Player who, string param) {
            TimeSpan delta;
            bool canAll = p == null || p == who || p.group.maxUndo == undoMax;
            
            if (param.CaselessEq("all")) {
                return TimeSpan.FromSeconds(canAll ? int.MaxValue : p.group.maxUndo);
            } else if (!param.TryParseShort(p, 's', "undo the past", out delta)) {
                Player.Message(p, "Undoing for 30 seconds.");
                return TimeSpan.MinValue;
            }

            if (delta.TotalSeconds == 0) 
                delta = TimeSpan.FromMinutes(90);
            if (!canAll && delta.TotalSeconds > p.group.maxUndo) {
                Player.Message(p, "{0}%Ss may only undo up to {1} seconds.",
                               p.group.ColoredName, p.group.maxUndo);
                return TimeSpan.FromSeconds(p.group.maxUndo);
            }
            return delta;
        }
                
        protected void UndoOnlinePlayer(Player p, TimeSpan delta, Player who, Vec3S32[] marks) {
            if (p != null && p != who && !CheckUndoPerms(p, who.group)) return;
            
            UndoOnlineDrawOp op;
            if (p == who) op = new UndoSelfDrawOp();
            else op = new UndoOnlineDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = who;
            DrawOp.DoDrawOp(op, null, p, marks);
            
            if (p == who) {
                Player.Message(p, "Undid your actions for the past &b" + delta.Shorten(true) + "%S.");
            } else {
                Player.SendChatFrom(who, who.ColoredName + "%S's actions for the past &b" + delta.Shorten(true) + " %Swere undone.", false);
            }
            Server.s.Log(who.name + "'s actions for the past " + delta.Shorten(true) + " were undone.");
        }
        
        protected void UndoOfflinePlayer(Player p, TimeSpan delta, string whoName, Vec3S32[] marks) {
            if (p != null && !CheckUndoPerms(p, Group.findPlayerGroup(whoName))) return;
            
            UndoOfflineDrawOp op = new UndoOfflineDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.whoName = whoName;
            DrawOp.DoDrawOp(op, null, p, marks);

            if (op.found) {
                Chat.MessageAll("{0}%S's actions for the past &b{1} %S were undone.", 
                                PlayerInfo.GetColoredName(p, whoName), delta.Shorten(true));
                Server.s.Log(whoName + "'s actions for the past " + delta.Shorten(true) + " were undone.");
                if (p != null) p.level.Save(true);
            } else {
                Player.Message(p, "Could not find player specified.");
            }
        }
        
        protected virtual bool CheckUndoPerms(Player p, Group grp) {
             if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "undo other players."); return false; }
             if (grp.Permission >= p.Rank) { MessageTooHighRank(p, "undo", false); return false; }
             return true;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/undo [player] <timespan>");
            Player.Message(p, "%HUndoes the blockchanges made by [player] in the past <timespan>. " +
                           "If <timespan> is not given, undoes 30 minutes.");
            Player.Message(p, "%H e.g. to undo the past 90 minutes, <timespan> would be %S1h30m");
            if (p == null || p.group.maxUndo == -1 || p.group.maxUndo == int.MaxValue)
                Player.Message(p, "%T/undo [player] all &c- Undoes 68 years for [player]");
            Player.Message(p, "%T/undo %H- Undoes your last draw operation.");
            Player.Message(p, "%T/undo physics [seconds] %H- Undoes physics on current map");
        }
    }
}
