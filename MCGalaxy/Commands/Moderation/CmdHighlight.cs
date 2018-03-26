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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdHighlight : Command {
        public override string name { get { return "Highlight"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }        
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("HighlightArea", "area") }; }
        }

        public override void Use(Player p, string message) {
            TimeSpan delta = TimeSpan.Zero;
            bool area = message.CaselessStarts("area ");
            if (area) message = message.Substring("area ".Length);
            
            if (message.Length == 0) message = p.name + " 1800";
            string[] parts = message.SplitSpaces();

            if (parts.Length >= 2) {
                if (!CommandParser.GetTimespan(p, parts[1], ref delta, "highlight the past", "s")) return;
            } else if (ParseTimespan(parts[0], out delta)) {
                parts[0] = p.name;
            } else {
                delta = TimeSpan.FromMinutes(30);
            }
            
            parts[0] = PlayerInfo.FindOfflineNameMatches(p, parts[0]);
            if (parts[0] == null) return;
            int[] ids = NameConverter.FindIds(parts[0]);
            
            if (!area) {
                Vec3S32[] marks = new Vec3S32[] { Vec3U16.MinVal, Vec3U16.MaxVal };
                HighlightPlayer(p, delta, parts[0], ids, marks);
            } else {
                Player.Message(p, "Place or break two blocks to determine the edges.");
                HighlightAreaArgs args = new HighlightAreaArgs();
                args.ids = ids; args.who = parts[0]; args.delta = delta;
                p.MakeSelection(2,  "Selecting region for %SHighlight", args, DoHighlightArea);
            }
        }
        
        bool DoHighlightArea(Player p, Vec3S32[] marks, object state, BlockID block) {
            HighlightAreaArgs args = (HighlightAreaArgs)state;
            HighlightPlayer(p, args.delta, args.who, args.ids, marks);
            return false;
        }

        struct HighlightAreaArgs { public string who; public int[] ids; public TimeSpan delta; }
        
        
        static void HighlightPlayer(Player p, TimeSpan delta, string who, int[] ids, Vec3S32[] marks) {
            HighlightDrawOp op = new HighlightDrawOp();
            op.Start = DateTime.UtcNow.Subtract(delta);
            op.who = who; op.ids = ids;
            DrawOpPerformer.Setup(op, p, marks);
            
            BufferedBlockSender buffer = new BufferedBlockSender(p);
            op.Perform(marks, null,
                       P => {
                           int index = p.level.PosToInt(P.X, P.Y, P.Z);
                           buffer.Add(index, P.Block);
                       });
            buffer.Send(true);
            
            if (op.found) {
                Player.Message(p, "Now highlighting past &b{0} %Sfor {1}",
                               delta.Shorten(true), PlayerInfo.GetColoredName(p, who));
                Player.Message(p, "&cUse /reload to un-highlight");
            } else {
                Player.Message(p, "No changes found by {1} %Sin the past &b{0}",
                               delta.Shorten(true), PlayerInfo.GetColoredName(p, who));
            }
        }
        
        static bool ParseTimespan(string input, out TimeSpan delta) {
            delta = TimeSpan.Zero;
            try { delta = input.ParseShort("s"); return true;
            } catch (ArgumentException) { return false;
            } catch (FormatException) { return false;
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Highlight [player] <timespan>");
            Player.Message(p, "%HHighlights blocks changed by [player] in the past <timespan>");
            Player.Message(p, "%T/Highlight area [player] <timespan>");
            Player.Message(p, "%HOnly highlights in the specified region.");
            Player.Message(p, "%H If <timespan> is not given, highlights for last 30 minutes");
            Player.Message(p, "&c/Highlight cannot be disabled, use /reload to un-highlight");
        }
    }
}
