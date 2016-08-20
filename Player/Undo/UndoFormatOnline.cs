/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Undo {

    public sealed class UndoFormatOnline : UndoFormat {
        
        protected override string Ext { get { return null; } }
        public UndoCache Cache;

        protected override void Save(List<Player.UndoPos> buffer, string path) {
            throw new NotSupportedException("UndoFileOnline is read only.");
        }
        
        protected override void Save(UndoCache buffer, string path) {
            throw new NotSupportedException("UndoFileOnline is read only.");
        }
        
        protected override IEnumerable<Player.UndoPos> GetEntries(Stream s, UndoEntriesArgs args) {
            UndoCacheNode node = Cache.Tail;
            if (node == null) yield break;
            
            Player.UndoPos pos;
            bool super = args.Player == null || args.Player.ircNick != null;
            DateTime start = args.StartRange;
            
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (!super && !args.Player.level.name.CaselessEq(node.MapName)) continue;
                List<UndoCacheItem> items = node.Items;
                pos.mapName = node.MapName;
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];
                    DateTime time = node.BaseTime.AddTicks(item.TimeDelta * TimeSpan.TicksPerSecond);
                    if (time < start) { args.Stop = true; yield break; }
                    pos.timeDelta = (int)time.Subtract(Server.StartTime).TotalSeconds;
                    
                    node.Unpack(item.Index, out pos.x, out pos.y, out pos.z);
                    item.GetBlock(out pos.type, out pos.extType);
                    item.GetNewBlock(out pos.newtype, out pos.newExtType);
                    yield return pos;
                }
                node = node.Prev;
            }
        }
    }
}
