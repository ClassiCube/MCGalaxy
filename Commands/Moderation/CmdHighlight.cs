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
            long seconds;
            bool found = false;
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") message = p.name + " 300";
            string[] args = message.Split(' ');
            string name = args[0];

            if (args.Length >= 2) {
                if (!long.TryParse(args[1], out seconds)) {
                    Player.Message(p, "Invalid seconds."); return;
                }
            } else if (long.TryParse(args[0], out seconds)) {
                args[0] = p.name;
            } else {
                seconds = 300;
            }
            if (seconds <= 0) seconds = 5400;
            DateTime start = DateTime.UtcNow.AddTicks(-seconds * TimeSpan.TicksPerSecond);

            Player who = PlayerInfo.Find(name);
            bool done = false;
            if (who != null) {
                found = true;
                UndoCache cache = who.UndoBuffer;
                using (IDisposable locker = cache.ClearLock.AccquireReadLock()) {
                    done = HighlightBlocks(p, start, cache);
                }
            }          
            if (!done) UndoFormat.HighlightPlayer(p, name.ToLower(), start, ref found);
            
            if (found) {
                Player.Message(p, "Now highlighting &b" + seconds +  " %Sseconds for " + Server.FindColor(name) + name);
                Player.Message(p, "&cUse /reload to un-highlight");
            } else {
                Player.Message(p, "Could not find player specified.");
            }
        }
        
        static bool HighlightBlocks(Player p, DateTime start, UndoCache cache) {
            UndoEntriesArgs args = new UndoEntriesArgs(p, start);
            UndoFormatOnline format = new UndoFormatOnline();
            format.Cache = cache;           
            UndoFormat.DoHighlight(null, format, args);
            return args.Stop;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/highlight [player] [seconds]");
            Player.Message(p, "%HHighlights blocks modified by [player] in the last [seconds]");
            Player.Message(p, "%HIf no [seconds] is given, highlights for last 30 minutes");
            Player.Message(p, "&c/highlight cannot be disabled, you must use /reload to un-highlight");
        }
    }
}
