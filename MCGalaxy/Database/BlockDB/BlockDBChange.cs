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

namespace MCGalaxy.DB {
    
    /// <summary> Outputs information about a block and its changes to the user. </summary>
    public static class BlockDBChange {
        
        public static void Output(Player p, string name, BlockDBEntry entry) {
            byte oldBlock = entry.OldRaw, oldExt = 0, newBlock = entry.NewRaw, newExt = 0;
            if ((entry.Flags & BlockDBFlags.OldCustom) != 0) {
                oldExt = oldBlock; oldBlock = Block.custom_block;
            }
            if ((entry.Flags & BlockDBFlags.NewCustom) != 0) {
                newExt = newBlock; newBlock = Block.custom_block;
            }
            
            TimeSpan delta = DateTime.UtcNow.Subtract(entry.Time);
            name = PlayerInfo.GetColoredName(p, name);
            
            if (newBlock == Block.air) {
                Player.Message(p, "{0} ago {1} &4deleted %S{2}",
                               delta.Shorten(true, false), name, p.level.BlockName(oldBlock, oldExt));
            } else {
                Player.Message(p, "{0} ago {1} &3placed %S{2}",
                               delta.Shorten(true, false), name, p.level.BlockName(newBlock, newExt));
            }
        }
        
        public static void OutputMessageBlock(Player p, byte block, byte extBlock,
                                              ushort x, ushort y, ushort z) {
            if (block == Block.custom_block) {
                if (!p.level.CustomBlockProps[extBlock].IsMessageBlock) return;
            } else {
                if (!Block.Props[block].IsMessageBlock) return;
            }

            try {
                if (!Database.Backend.TableExists("Messages" + p.level.name)) return;
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
        
        public static void OutputPortal(Player p, byte block, byte extBlock,
                                        ushort x, ushort y, ushort z) {
            if (block == Block.custom_block) {
                if (!p.level.CustomBlockProps[extBlock].IsPortal) return;
            } else {
                if (!Block.Props[block].IsPortal) return;
            }

            try {
                if (!Database.Backend.TableExists("Portals" + p.level.name)) return;
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
    }
}
