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
using System.Collections.Generic;
using MCGalaxy.Commands.Building;
using MCGalaxy.DB;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Moderation {
    public class CmdUndoPlayer : Command2 {
        public override string name { get { return "UndoPlayer"; } }
        public override string shortcut { get { return "up"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("XUndo","{args} all"),
                    new CommandAlias("UndoArea", "-area"), new CommandAlias("ua", "-area") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            bool area = message.CaselessStarts("-area");
            if (area) {
                message = message.Substring("-area".Length).TrimStart();
            }

            if (CheckSuper(p, message, "player name")) return;
            if (message.Length == 0) { p.Message("You need to provide a player name."); return; }
            
            string[] parts = message.SplitSpaces(), names = null;
            int[] ids = GetIds(p, parts, data, out names);
            if (ids == null) return;
            
            TimeSpan delta = CmdUndo.GetDelta(p, parts[0], parts, 1);
            if (delta == TimeSpan.MinValue) return;

            if (!area) {
                Vec3S32[] marks = new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal };
                UndoPlayer(p, delta, names, ids, marks);
            } else {
                p.Message("Place or break two blocks to determine the edges.");
                UndoAreaArgs args = new UndoAreaArgs();
                args.ids = ids; args.names = names; args.delta = delta;
                p.MakeSelection(2, "Selecting region for &SUndo player", args, DoUndoArea);
            }
        }
        
        bool DoUndoArea(Player p, Vec3S32[] marks, object state, BlockID block) {
            UndoAreaArgs args = (UndoAreaArgs)state;
            UndoPlayer(p, args.delta, args.names, args.ids, marks);
            return false;
        }

        struct UndoAreaArgs { public string[] names; public int[] ids; public TimeSpan delta; }
        

        static void UndoPlayer(Player p, TimeSpan delta, string[] names, int[] ids, Vec3S32[] marks) {
            UndoDrawOp op = new UndoDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = names[0]; op.ids = ids;
            op.AlwaysUsable = true;
            
            if (p.IsSuper) {
                // undo them across all loaded levels
                Level[] levels = LevelInfo.Loaded.Items;
                
                foreach (Level lvl in levels) {
                    op.Setup(p, lvl, marks);
                    DrawOpPerformer.Execute(p, op, null, marks);
                }
                p.level = null;
            } else {
                DrawOpPerformer.Do(op, null, p, marks);
            }

            string namesStr = names.Join(name => p.FormatNick(name));
            if (op.found) {
                Chat.MessageGlobal("Undid {1}&S's changes for the past &b{0}", delta.Shorten(true), namesStr);
                Logger.Log(LogType.UserActivity, "Actions of {0} for the past {1} were undone.", names.Join(), delta.Shorten(true));
            } else {
                p.Message("No changes found by {1} &Sin the past &b{0}", delta.Shorten(true), namesStr);
            }
        }
        
        int[] GetIds(Player p, string[] parts, CommandData data, out string[] names) {
            int count = Math.Max(1, parts.Length - 1);
            List<int> ids = new List<int>();
            names = new string[count];
            
            for (int i = 0; i < names.Length; i++) {
                names[i] = PlayerDB.MatchNames(p, parts[i]);
                if (names[i] == null) return null;
                
                Group grp = PlayerInfo.GetGroup(names[i]);
                if (!CheckRank(p, data, names[i], grp.Permission, "undo", false)) return null;
                ids.AddRange(NameConverter.FindIds(names[i]));
            }
            return ids.ToArray();
        }

        public override void Help(Player p) {
            p.Message("&T/UndoPlayer [player1] <player2..> <timespan>");
            p.Message("&HUndoes the block changes of [players] in the past <timespan>");
            p.Message("&T/UndoPlayer -area [player1] <player2..> <timespan>");
            p.Message("&HOnly undoes block changes in the specified region.");
            p.Message("&H  If <timespan> is not given, undoes 30 minutes.");
        }
    }
}
