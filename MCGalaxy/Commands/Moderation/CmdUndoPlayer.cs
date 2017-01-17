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
    public class CmdUndoPlayer : Command {
        public override string name { get { return "undoplayer"; } }
        public override string shortcut { get { return "up"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xundo", null, "all") }; }
        }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "player name")) return;
            if (message == "") { Player.Message(p, "You need to provide a player name."); return; }
            
            string[] parts = message.Split(' ');
            parts[0] = PlayerInfo.FindOfflineNameMatches(p, parts[0]);
            if (parts[0] == null) return;
            
            TimeSpan delta = CmdUndo.GetDelta(p, parts[0], parts, 1);
            if (delta == TimeSpan.MinValue) return;

            Vec3S32[] marks = new Vec3S32[] { Vec3U16.MaxVal, Vec3U16.MaxVal };
            UndoPlayer(p, delta, parts[0], marks);
        }

        protected void UndoPlayer(Player p, TimeSpan delta, string name, Vec3S32[] marks) {
            if (p != null && !CheckUndoPerms(p, name)) return;
            
            UndoDrawOp op = new UndoDrawOp();
            if (p != null && p.name.CaselessEq(name))
                op = new UndoSelfDrawOp();
            
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = name;
            DrawOp.DoDrawOp(op, null, p, marks);

            if (op.found) {
                Chat.MessageAll("Undid {1}%S's changes for the past &b{0}",
                                delta.Shorten(true), PlayerInfo.GetColoredName(p, name));
                Server.s.Log(name + "'s actions for the past " + delta.Shorten(true) + " were undone.");
            } else {
                Player.Message(p, "No changes found by {1} %Sin the past &b{0}",
                               delta.Shorten(true), PlayerInfo.GetColoredName(p, name));
            }
        }
        
        protected virtual bool CheckUndoPerms(Player p, string name) {
            if (p != null && p.name.CaselessEq(name)) return true;
            Group grp = Group.findPlayerGroup(name);
            
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "undo other players."); return false; }
            if (grp.Permission >= p.Rank) { MessageTooHighRank(p, "undo", false); return false; }
            return true;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/undoplayer [player] <timespan>");
            Player.Message(p, "%HUndoes the blockchanges made by [player] in the past <timespan>.");
            Player.Message(p, "%H  If <timespan> is not given, undoes 30 minutes.");
            Player.Message(p, "%H  e.g. to undo the past 90 minutes, <timespan> would be %S1h30m");
            if (p == null || p.group.maxUndo == -1 || p.group.maxUndo == int.MaxValue)
                Player.Message(p, "%T/undoplayer [player] all &c- Undoes 68 years for [player]");
        }
    }
}
