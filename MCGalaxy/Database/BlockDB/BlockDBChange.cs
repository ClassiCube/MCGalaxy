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
using System.Data;
using MCGalaxy.Blocks.Extended;
using MCGalaxy.SQL;
using BlockID = System.UInt16;

namespace MCGalaxy.DB {
    
    /// <summary> Outputs information about a block and its changes to the user. </summary>
    public static class BlockDBChange {
        
        public static void Output(Player p, string name, BlockDBEntry e) {
            BlockID oldBlock = e.OldBlock, newBlock = e.NewBlock;
            DateTime time = BlockDB.Epoch.AddSeconds(e.TimeDelta);
            TimeSpan delta = DateTime.UtcNow.Subtract(time);
            name = p.FormatNick(name);
            
            if (newBlock == Block.Air) {
                p.Message("{0} ago {1} &4deleted &S{2}{3}",
                               delta.Shorten(true, false), name,
                               Block.GetName(p, oldBlock),
                               FormatReason(e.Flags));
            } else {
                p.Message("{0} ago {1} &3placed &S{2}{3}",
                               delta.Shorten(true, false), name,
                               Block.GetName(p, newBlock),
                               FormatReason(e.Flags));
            }
        }
        
        public static void OutputMessageBlock(Player p, BlockID block, ushort x, ushort y, ushort z) {
            if (!p.level.Props[block].IsMessageBlock)      return;
            if (!MessageBlock.ExistsInDB(p.level.MapName)) return;
            string message = MessageBlock.Get(p.level.MapName, x, y, z);
            
            if (message == null) return;
            p.Message("Message Block contents: {0}", message);
        }
        
        public static void OutputPortal(Player p, BlockID block, ushort x, ushort y, ushort z) {
            if (!p.level.Props[block].IsPortal)      return;
            if (!Portal.ExistsInDB(p.level.MapName)) return;
            PortalExit exit = Portal.Get(p.level.MapName, x, y, z);
            
            if (exit == null) return;
            p.Message("Portal destination: ({0}, {1}, {2}) in {3}",
                           exit.X, exit.Y, exit.Z, exit.Map);
        }
        
        static string FormatReason(ushort flags) {
            if ((flags & BlockDBFlags.Painted) != 0)   return " (Painted)";
            if ((flags & BlockDBFlags.Drawn) != 0)     return " (Drawn)";
            if ((flags & BlockDBFlags.Replaced) != 0)  return " (Replaced)";
            if ((flags & BlockDBFlags.Pasted) != 0)    return " (Pasted)";
            if ((flags & BlockDBFlags.Cut) != 0)       return " (Cut)";
            if ((flags & BlockDBFlags.Filled) != 0)    return " (Filled)";
            if ((flags & BlockDBFlags.Restored) != 0)  return " (Restored)";
            if ((flags & BlockDBFlags.UndoOther) != 0) return " (UndoneOther)";
            if ((flags & BlockDBFlags.UndoSelf) != 0)  return " (UndoneSelf)";
            if ((flags & BlockDBFlags.RedoSelf) != 0)  return " (RedoneSelf)";
            return "";
        }
    }
}
