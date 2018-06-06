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
using MCGalaxy.SQL;
using BlockID = System.UInt16;

namespace MCGalaxy.DB {
    
    /// <summary> Outputs information about a block and its changes to the user. </summary>
    public static class BlockDBChange {
        
        public static void Output(Player p, string name, BlockDBEntry e) {
            BlockID oldBlock = e.OldBlock, newBlock = e.NewBlock;            
            DateTime time = BlockDB.Epoch.AddSeconds(e.TimeDelta);
            TimeSpan delta = DateTime.UtcNow.Subtract(time);
            name = PlayerInfo.GetColoredName(p, name);
            
            if (newBlock == Block.Air) {
                Player.Message(p, "{0} ago {1} &4deleted %S{2}{3}",
                               delta.Shorten(true, false), name,
                               Block.GetName(p, oldBlock),
                               FormatReason(e.Flags));
            } else {
                Player.Message(p, "{0} ago {1} &3placed %S{2}{3}",
                               delta.Shorten(true, false), name,
                               Block.GetName(p, newBlock),
                               FormatReason(e.Flags));
            }
        }
        
        public static void OutputMessageBlock(Player p, BlockID block, ushort x, ushort y, ushort z) {
            if (!p.level.Props[block].IsMessageBlock) return;

            try {
                if (!Database.TableExists("Messages" + p.level.name)) return;
                DataTable messages = Database.Backend.GetRows("Messages" + p.level.name, "*",
                                                              "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
                int last = messages.Rows.Count - 1;
                if (last == -1) { messages.Dispose(); return; }
                
                string message = messages.Rows[last]["Message"].ToString().Trim();
                message = message.Replace("\\'", "\'");
                Player.Message(p, "Message Block contents: {0}", message);
            } catch {
            }
        }
        
        public static void OutputPortal(Player p, BlockID block, ushort x, ushort y, ushort z) {
            if (!p.level.Props[block].IsPortal) return;

            try {
                if (!Database.TableExists("Portals" + p.level.name)) return;
                DataTable portals = Database.Backend.GetRows("Portals" + p.level.name, "*",
                                                             "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
                int last = portals.Rows.Count - 1;
                if (last == -1) { portals.Dispose(); return; }
                
                string exitMap = portals.Rows[last]["ExitMap"].ToString().Trim();
                ushort exitX = U16(portals.Rows[last]["ExitX"]);
                ushort exitY = U16(portals.Rows[last]["ExitY"]);
                ushort exitZ = U16(portals.Rows[last]["ExitZ"]);
                Player.Message(p, "Portal destination: ({0}, {1}, {2}) in {3}",
                               exitX, exitY, exitZ, exitMap);
            } catch {
            }
        }
        
        static ushort U16(object x) { return ushort.Parse(x.ToString()); }
        
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
