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
using MCGalaxy.Undo;

namespace MCGalaxy.Commands {
    
    public sealed class CmdHighlight : Command {
        
        public override string name { get { return "highlight"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdHighlight() { }

        public override void Use(Player p, string message) {
            TimeSpan delta;
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") message = p.name + " 1800";
            string[] args = message.Split(' ');

            if (args.Length >= 2) {
                if (!args[1].TryParseShort(p, 's', "highlight the past", out delta)) return;
            } else if (ParseTimespan(args[0], out delta)) {
                args[0] = p.name;
            } else {
                delta = TimeSpan.FromMinutes(30);
            }
            args[0] = PlayerInfo.FindOfflineNameMatches(p, args[0]);
            if (args[0] == null) return;
            
            
            DateTime start = DateTime.UtcNow.Subtract(delta);
            int[] ids = NameConverter.FindIds(args[0]);
            bool found = false, done = false;
            if (ids.Length > 0) {
                HighlightHelper helper = new HighlightHelper();
                done = helper.DoHighlight(ids, start, p);
                found = helper.found;
            }
            
            if (!done) {
                UndoFormatArgs undoArgs = new UndoFormatArgs(p, start, DateTime.MaxValue, null);
                UndoFormat.DoHighlight(args[0].ToLower(), ref found, undoArgs);
            }
            
            if (found) {
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
        
        class HighlightHelper {
            Player p;
            BufferedBlockSender buffer;
            Vec3U16 dims;
            public bool found;
            
            public bool DoHighlight(int[] ids, DateTime start, Player p) {
                buffer = new BufferedBlockSender(p);
                this.p = p;
                bool reachedStart = false;
                
                using (IDisposable rLock = p.level.BlockDB.Locker.AccquireRead()) {
                    reachedStart = p.level.BlockDB.FindChangesBy(ids, start, DateTime.MaxValue,
                                                                 out dims, HighlightBlock);
                }
                buffer.Send(true);
                
                buffer.player = null;
                buffer.level = null;
                buffer = null;
                this.p = null;
                return reachedStart;
            }
            
            void HighlightBlock(BlockDBEntry entry) {
                byte oldBlock = entry.OldRaw, newBlock = entry.NewRaw;
                if ((entry.Flags & BlockDBFlags.OldCustom) != 0) oldBlock = Block.custom_block;
                if ((entry.Flags & BlockDBFlags.NewCustom) != 0) newBlock = Block.custom_block;
                if (oldBlock == Block.Invalid) return; // Exported BlockDB SQL table entries don't have previous block
                found = true;
                
                byte highlight = (newBlock == Block.air
                                  || Block.Convert(oldBlock) == Block.water || oldBlock == Block.waterstill
                                  || Block.Convert(oldBlock) == Block.lava || oldBlock == Block.lavastill)
                    ? Block.red : Block.green;
                
                int x = entry.Index % dims.X;
                int y = (entry.Index / dims.X) / dims.Z;
                int z = (entry.Index / dims.X) % dims.Z;
                int index = p.level.PosToInt((ushort)x, (ushort)y, (ushort)z);
                buffer.Add(index, highlight, 0);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/highlight [player] <timespan>");
            Player.Message(p, "%HHighlights blocks modified by [player] in the past <timespan>");
            Player.Message(p, "%H If <timespan> is not given, highlights for last 30 minutes");
            Player.Message(p, "%H e.g. to highlight for 90 minutes, <timespan> would be %S1h30m");
            Player.Message(p, "&c/highlight cannot be disabled, use /reload to un-highlight");
        }
    }
}
