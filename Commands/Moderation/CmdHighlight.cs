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
using MCGalaxy.Util;

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
            bool FoundUser = false;
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

            Player who = PlayerInfo.Find(name);
            if (who != null) {
                FoundUser = true;
                UndoCache cache = who.UndoBuffer;
                using (IDisposable locker = cache.ClearLock.AccquireReadLock()) {
                    HighlightBlocks(p, seconds, cache);
                }
            }

            DateTime start = DateTime.UtcNow.AddTicks(-seconds * TimeSpan.TicksPerSecond);
            UndoFile.HighlightPlayer(p, name.ToLower(), start, ref FoundUser);
            if (FoundUser) {
                Player.Message(p, "Now highlighting &b" + seconds +  " %Sseconds for " + Server.FindColor(name) + name);
                Player.Message(p, "&cUse /reload to un-highlight");
            } else {
                Player.Message(p, "Could not find player specified.");
            }
        }
        
        static void HighlightBlocks(Player p, long seconds, UndoCache cache) {
            UndoCacheNode node = cache.Tail;
            if (node == null) return;
            
            BufferedBlockSender sender = new BufferedBlockSender(p);
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (lvl != p.level) { node = node.Prev; continue; }
                List<UndoCacheItem> items = node.Items;
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];
                    ushort x, y, z;
                    node.Unpack(item.Index, out x, out y, out z);
                    DateTime time = node.BaseTime.AddSeconds(item.TimeDelta + seconds);
                    if (time < DateTime.UtcNow) { sender.CheckIfSend(true); return; }
                    
                    byte newBlock = 0, newExtBlock = 0;
                    item.GetNewBlock(out newBlock, out newExtBlock);
                    int index = lvl.PosToInt(x, y, z);
                    
                    byte highlightBlock = newBlock == Block.air ? Block.red : Block.green;
                    sender.Add(index, highlightBlock, 0);
                    sender.CheckIfSend(false);
                }
                node = node.Prev;
            }
            sender.CheckIfSend(true);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/highlight [player] [seconds]");
            Player.Message(p, "%HHighlights blocks modified by [player] in the last [seconds]");
            Player.Message(p, "%HIf no [seconds] is given, highlights for last 30 minutes");
            Player.Message(p, "&c/highlight cannot be disabled, you must use /reload to un-highlight");
        }
    }
}
