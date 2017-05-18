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

namespace MCGalaxy.Commands.Moderation {
    public class CmdUndoPlayer : Command {
        public override string name { get { return "undoplayer"; } }
        public override string shortcut { get { return "up"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("xundo", null, "all"), 
                    new CommandAlias("undoarea", "area"), new CommandAlias("ua", "area") }; }
        }

        public override void Use(Player p, string message) {
            bool area = message.CaselessStarts("area ");
            if (area) message = message.Substring("area ".Length);
            
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; } // TODO: fix this to work from IRC and Console
            if (CheckSuper(p, message, "player name")) return;
            if (message == "") { Player.Message(p, "You need to provide a player name."); return; }
            
            string[] parts = message.SplitSpaces(), names = null;
            int[] ids = GetIds(p, parts, out names);
            if (ids == null) return;
            
            TimeSpan delta = CmdUndo.GetDelta(p, parts[0], parts, 1);
            if (delta == TimeSpan.MinValue) return;

            if (!area) {
                Vec3S32[] marks = new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal };
                UndoPlayer(p, delta, names, ids, marks);
            } else {
                Player.Message(p, "Place or break two blocks to determine the edges.");
                UndoAreaArgs args = new UndoAreaArgs();
                args.ids = ids; args.names = names; args.delta = delta;
                p.MakeSelection(2, args, DoUndoArea);
            }
        }
        
        bool DoUndoArea(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            UndoAreaArgs args = (UndoAreaArgs)state;
            UndoPlayer(p, args.delta, args.names, args.ids, marks);
            return false;
        }

        struct UndoAreaArgs { public string[] names; public int[] ids; public TimeSpan delta; }
        

        static void UndoPlayer(Player p, TimeSpan delta, string[] names, int[] ids, Vec3S32[] marks) {
            UndoDrawOp op = new UndoDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = names[0]; op.ids = ids;
            DrawOpPerformer.Do(op, null, p, marks);

            string namesStr = names.Join(name => PlayerInfo.GetColoredName(p, name));
            if (op.found) {
                Chat.MessageGlobal("Undid {1}%S's changes for the past &b{0}", delta.Shorten(true), namesStr);
                Server.s.Log(names.Join() + "'s actions for the past " + delta.Shorten(true) + " were undone.");
            } else {
                Player.Message(p, "No changes found by {1} %Sin the past &b{0}", delta.Shorten(true), namesStr);
            }
        }
        
        int[] GetIds(Player p, string[] parts, out string[] names) {
            int count = Math.Max(1, parts.Length - 1);
            List<int> ids = new List<int>();
            names = new string[count];
            
            for (int i = 0; i < names.Length; i++) {
                names[i] = PlayerInfo.FindOfflineNameMatches(p, parts[i]);
                if (names[i] == null) return null;
                
                Group grp = Group.findPlayerGroup(names[i]);
                if (p != null && grp.Permission >= p.Rank) {
                    MessageTooHighRank(p, "undo", false); return null;
                }

                ids.AddRange(NameConverter.FindIds(names[i]));
            }
            return ids.ToArray();
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/undoplayer [player1] <player2..> <timespan>");
            Player.Message(p, "%HUndoes the block changes of [players] in the past <timespan>");
            Player.Message(p, "%T/undoplayer area [player1] <player2..> <timespan>");
            Player.Message(p, "%HOnly undoes block changes in the specified region.");
            Player.Message(p, "%H  If <timespan> is not given, undoes 30 minutes.");
            if (p == null || p.group.maxUndo == -1 || p.group.maxUndo == int.MaxValue)
                Player.Message(p, "%H  if <timespan> is all, &cundoes for 68 years");
        }
    }
}
