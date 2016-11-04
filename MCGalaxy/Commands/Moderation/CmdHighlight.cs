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
using System.IO;
using MCGalaxy.Undo;

namespace MCGalaxy.Commands {
    
    public sealed class CmdHighlight : Command {
        
        public override string name { get { return "highlight"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdHighlight() { }

        public override void Use(Player p, string message) {
            TimeSpan delta;
            bool found = false;
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
            
            DateTime start = DateTime.UtcNow.Subtract(delta);
            Player who = PlayerInfo.Find(args[0]);
            bool done = false;
            if (who != null) {
                found = true;
                UndoCache cache = who.UndoBuffer;
                using (IDisposable locker = cache.ClearLock.AccquireReadLock()) {
                    done = HighlightBlocks(p, start, cache);
                }
            }          
            if (!done) UndoFormat.DoHighlight(p, args[0].ToLower(), start, ref found);
            
            if (found) {
                Player.Message(p, "Now highlighting past &b{0} %Sfor {1}",
                               delta.Shorten(), PlayerInfo.GetColoredName(p, args[0]));
                Player.Message(p, "&cUse /reload to un-highlight");
            } else {
                Player.Message(p, "Could not find player specified.");
            }
        }
        
        static bool ParseTimespan(string input, out TimeSpan delta) {
            delta = TimeSpan.Zero;
            try { delta = input.ParseShort('s'); return true;
            } catch (ArgumentException) { return false; 
            } catch (FormatException) { return false;
            }
        }
        
        static bool HighlightBlocks(Player p, DateTime start, UndoCache cache) {
            UndoFormatArgs args = new UndoFormatArgs(p, start);
            UndoFormat format = new UndoFormatOnline(cache);
            UndoFormat.DoHighlight(null, format, args);
            return args.Stop;
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
