/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Data;
using MCGalaxy.DB;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy.Commands {
    public sealed class CmdAbout : Command {
        public override string name { get { return "about"; } }
        public override string shortcut { get { return "b"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdAbout() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            Player.Message(p, "Break/build a block to display information.");
            p.ClearBlockchange();
            p.Blockchange += PlacedBlock;
        }

        void PlacedBlock(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            if (!p.staticCommands) p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            if (b == Block.Invalid) { Player.Message(p, "Invalid Block ({0}, {1}, {2}).", x, y, z); return; }
            p.RevertBlock(x, y, z);
            
            byte id = b;
            if (b == Block.custom_block)
                id = p.level.GetExtTile(x, y, z);
            Dictionary<int, string> names = new Dictionary<int, string>();
            
            string blockName = p.level.BlockName(b, id);
            Player.Message(p, "Block ({0}, {1}, {2}): &f{3} = {4}%S.", x, y, z, id, blockName);
            bool foundAny = false;
            
            ListFromDatabase(p, ref foundAny, names, x, y, z);
            p.level.BlockDB.FindChangesAt(x, y, z, 
                                          entry => OutputEntry(p, ref foundAny, names, entry));
            ListInMemory(p, ref foundAny, names, x, y, z);
            
            if (!foundAny) Player.Message(p, "No block change records found for this block.");
            OutputMessageBlock(p, b, id, x, y, z);
            OutputPortal(p, b, id, x, y, z);
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        static void ListFromDatabase(Player p, ref bool foundAny, Dictionary<int, string> names,
                                     ushort x, ushort y, ushort z) {
            DataTable Blocks = Database.Backend.GetRows("Block" + p.level.name, "*",
                                                        "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
            DateTime now = DateTime.Now;
            
            for (int i = 0; i < Blocks.Rows.Count; i++) {
                foundAny = true;
                DataRow row = Blocks.Rows[i];
                string name = row["Username"].ToString().Trim();
                DateTime time = DateTime.Parse(row["TimePerformed"].ToString());
                byte block = byte.Parse(row["Type"].ToString());
                
                byte flags = ParseFlags(row["Deleted"].ToString());
                bool deleted = (flags & 1) != 0, isExt = (flags & 2) != 0;
                Output(p, name, block, isExt, deleted, now - time);
            }
            Blocks.Dispose();
        }
        
        static byte ParseFlags(string value) {
            // This used to be a 'deleted' boolean, so we need to make sure we account for that
            if (value.CaselessEq("true")) return 1;
            if (value.CaselessEq("false")) return 0;
            return byte.Parse(value);
        }
        
        static void OutputEntry(Player p, ref bool foundAny, Dictionary<int, string> names, BlockDBEntry entry) {
            DateTime now = DateTime.UtcNow;
            string name = null;
            if (!names.TryGetValue(entry.PlayerID, out name)) {
                name = NameConverter.FindName(entry.PlayerID);
                names[entry.PlayerID] = name;
            }
            foundAny = true;
            
            DateTime time = BlockDB.Epoch.AddSeconds(entry.TimeDelta);
            bool deleted = entry.NewRaw == 0;
            bool extBlock = (entry.Flags & 0x8000) != 0;
            Output(p, name, entry.NewRaw, extBlock, deleted, now - time);
        }
        
        static void ListInMemory(Player p, ref bool foundAny, Dictionary<int, string> names,
                                 ushort x, ushort y, ushort z) {
            int index = p.level.PosToInt(x, y, z);
            FastList<BlockDBEntry> entries = p.level.blockCache;
            
            for (int i = 0; i < entries.Count; i++) {
                if (entries.Items[i].Index != index) continue;
                OutputEntry(p, ref foundAny, names, entries.Items[i]);
            }
        }
        
        
        static void Output(Player p, string user, byte raw, bool isExt,
                           bool deleted, TimeSpan delta) {
            byte block = isExt ? Block.custom_block : raw;
            byte extBlock = isExt ? raw : (byte)0;
            
            string blockName = p.level.BlockName(block, extBlock);
            if (raw == Block.custom_block && !isExt) // Before started tracking IsExt in BlockDB
                blockName = Block.Name(raw);
            
            Player.Message(p, "{0} ago {1} {2}",
                           delta.Shorten(true, false), PlayerInfo.GetColoredName(p, user),
                           deleted ? "&4deleted %S(using " + blockName + ")" : "&3placed %S" + blockName);
        }
        
        static void OutputMessageBlock(Player p, byte block, byte extBlock,
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
        
        static void OutputPortal(Player p, byte block, byte extBlock,
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
        
        public override void Help(Player p) {
            Player.Message(p, "%T/about");
            Player.Message(p, "%HOutputs the change/edit history for a block.");
        }
    }
}
