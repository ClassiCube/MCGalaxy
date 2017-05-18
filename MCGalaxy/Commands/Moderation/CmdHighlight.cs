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
using MCGalaxy.DB;
using MCGalaxy.Network;
using MCGalaxy.Undo;
using MCGalaxy.Maths;
using System;
using System.Collections.Generic;
using MCGalaxy.Commands.Building;
using MCGalaxy.DB;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Undo;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdHighlight : Command {
        public override string name { get { return "highlight"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdHighlight() { }

        public override void Use(Player p, string message) {
            TimeSpan delta = TimeSpan.Zero;
            bool area = message.CaselessStarts("area ");
            if (area) message = message.Substring("area ".Length);
            
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") message = p.name + " 1800";
            string[] args = message.SplitSpaces();

            if (args.Length >= 2) {
                if (!CommandParser.GetTimespan(p, args[1], ref delta, "highlight the past", 's')) return;
            } else if (ParseTimespan(args[0], out delta)) {
                args[0] = p.name;
            } else {
                delta = TimeSpan.FromMinutes(30);
            }
            args[0] = PlayerInfo.FindOfflineNameMatches(p, args[0]);
            if (args[0] == null) return;
            
            HighlightDrawOp op = new HighlightDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = args[0]; op.ids = NameConverter.FindIds(args[0]);
            
            Vec3S32[] marks = new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal };
            DrawOpPerformer.Setup(op, p, marks);
            
            BufferedBlockSender buffer = new BufferedBlockSender(p);
            op.Perform(marks, null,
                       P => {
                           int index = p.level.PosToInt(P.X, P.Y, P.Z);
                           buffer.Add(index, P.Block.BlockID, P.Block.ExtID);
                       });
            buffer.Send(true);
            
            if (op.found) {
                Player.Message(p, "Now highlighting past &b{0} %Sfor {1}",
                               delta.Shorten(true), PlayerInfo.GetColoredName(p, args[0]));
                Player.Message(p, "&cUse /reload to un-highlight");
            } else {
                Player.Message(p, "No changes found by {1} %Sin the past &b{0}",
                               delta.Shorten(true), PlayerInfo.GetColoredName(p, args[0]));
            }
        }
        
        static bool ParseTimespan(string input, out TimeSpan delta) {
            delta = TimeSpan.Zero;
            try { delta = input.ParseShort('s'); return true;
            } catch (ArgumentException) { return false;
            } catch (FormatException) { return false;
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/highlight [player] <timespan>");
            Player.Message(p, "%HHighlights blocks changed by [player] in the past <timespan>");
            Player.Message(p, "%T/highlight area [player] <timespan>");
            Player.Message(p, "%HHighlights only in a specified area.");
            Player.Message(p, "%H If <timespan> is not given, highlights for last 30 minutes");
            Player.Message(p, "&c/highlight cannot be disabled, use /reload to un-highlight");
        }
    }
}
