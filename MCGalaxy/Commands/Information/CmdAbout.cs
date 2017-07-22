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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdAbout : Command {
        public override string name { get { return "about"; } }
        public override string shortcut { get { return "b"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("binfo"), new CommandAlias("whodid") }; }
        }
            
        public override void Use(Player p, string message) {
            Player.Message(p, "Break/build a block to display information.");
            p.MakeSelection(1, "Selecting location for %SBlock info", null, PlacedMark);
        }

        bool PlacedMark(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            ushort x = (ushort)marks[0].X, y = (ushort)marks[0].Y, z = (ushort)marks[0].Z;
            block = p.level.GetBlock(x, y, z);
            p.RevertBlock(x, y, z);            
            Dictionary<int, string> names = new Dictionary<int, string>();

            Player.Message(p, "Retrieving block change records..");
            bool foundAny = false;            
            ListFromDatabase(p, ref foundAny, x, y, z);
            using (IDisposable rLock = p.level.BlockDB.Locker.AccquireRead(30 * 1000)) {
                if (rLock != null) {
                    p.level.BlockDB.FindChangesAt(x, y, z,
                                                  entry => OutputEntry(p, ref foundAny, names, entry));
                } else {
                    Player.Message(p, "&cUnable to accquire read lock on BlockDB after 30 seconds, aborting.");
                    return false;
                }
            }
            
            if (!foundAny) Player.Message(p, "No block change records found for this block.");
            
            string blockName = p.level.BlockName(block);
            Player.Message(p, "Block ({0}, {1}, {2}): &f{3} = {4}%S.", 
                           x, y, z, block.RawID, blockName);
            
            BlockDBChange.OutputMessageBlock(p, block, x, y, z);
            BlockDBChange.OutputPortal(p, block, x, y, z);           
            Server.DoGC();
            return true;
        }
        
        static void ListFromDatabase(Player p, ref bool foundAny, ushort x, ushort y, ushort z) {
            if (!Database.TableExists("Block" + p.level.name)) return;
            using (DataTable Blocks = Database.Backend.GetRows("Block" + p.level.name, "*",
                                                               "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z)) {
                BlockDBEntry entry = default(BlockDBEntry);
                entry.OldRaw = Block.Invalid;
                
                for (int i = 0; i < Blocks.Rows.Count; i++) {
                    foundAny = true;
                    DataRow row = Blocks.Rows[i];
                    string name = row["Username"].ToString().Trim();
                    
                    DateTime time = DateTime.Parse(row["TimePerformed"].ToString());
                    TimeSpan delta = time - BlockDB.Epoch;
                    entry.TimeDelta = (int)delta.TotalSeconds;
                    entry.Flags = BlockDBFlags.ManualPlace;
                    
                    byte flags = ParseFlags(row["Deleted"].ToString());
                    if ((flags & 1) == 0) { // block was placed
                        entry.NewRaw = byte.Parse(row["Type"].ToString());
                        entry.Flags |= (flags & 2) != 0 ? BlockDBFlags.NewCustom : BlockDBFlags.None;
                    }
                    BlockDBChange.Output(p, name, entry);
                }
            }
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
            BlockDBChange.Output(p, name, entry);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/about");
            Player.Message(p, "%HOutputs the change/edit history for a block.");
        }
    }
}
