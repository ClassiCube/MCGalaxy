/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Util;

namespace MCGalaxy.Commands {
    
    public sealed class CmdRedo : Command {
        
        public override string name { get { return "redo"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdRedo() { }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            PerformRedo(p, p.RedoBuffer);
            Player.SendMessage(p, "Redo performed.");
        }
        
        static void PerformRedo(Player p, UndoCache cache) {
            UndoCacheNode node = cache.Tail;
            if (node == null) return;
            
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (lvl == null) { node = node.Prev; continue; }
                List<UndoCacheItem> items = node.Items;
                BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];                    
                    ushort x, y, z;
                    node.Unpack(item.Index, out x, out y, out z);
                    
                    byte type = lvl.GetTile(x, y, z), extType = 0;
                    if (type == Block.custom_block)
                        extType = lvl.GetExtTile(x, y, z);
                    if (lvl.DoBlockchange(p, x, y, z, item.Type, item.ExtType)) {
                        buffer.Add(lvl.PosToInt(x, y, z), item.Type, item.ExtType);
                        buffer.CheckIfSend(false);
                    }
                }
                buffer.CheckIfSend(true);
                node = node.Prev;
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/redo - Redoes the Undo you just performed.");
        }
    }
}
