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
using MCGalaxy.SQL;

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
            if (b == Block.Zero) { Player.Message(p, "Invalid Block ({0}, {1}, {2}).", x, y, z); return; }
            p.RevertBlock(x, y, z);
            
            byte id = b;
            if (b == Block.custom_block)
                id = p.level.GetExtTile(x, y, z);
            
            string blockName = p.level.BlockName(b, id);
            Player.Message(p, "Block ({0}, {1}, {2}): &f{3} = {4}%S.", x, y, z, id, blockName);
            DateTime now = DateTime.Now;
            bool foundOne = false;
            
            //safe against SQL injections because no user input is given here
            DataTable Blocks = Database.Fill("SELECT * FROM `Block" + p.level.name + 
                                             "` WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
            for (int i = 0; i < Blocks.Rows.Count; i++) {
                foundOne = true;
                DataRow row = Blocks.Rows[i];
                string user = row["Username"].ToString().Trim();
                DateTime time = DateTime.Parse(row["TimePerformed"].ToString());
                byte block = Convert.ToByte(row["Type"]);
                bool deleted = Convert.ToBoolean(row["Deleted"]);
                Output(p, user, block, deleted, now - time);
            }
            Blocks.Dispose();

            int bpIndex = p.level.PosToInt(x, y, z);
            List<Level.BlockPos> inCache = p.level.blockCache.FindAll(bP => bP.index == bpIndex);
            for (int i = 0; i < inCache.Count; i++) {
                foundOne = true;
                string user = inCache[i].name.Trim();
                DateTime time = Server.StartTimeLocal.AddSeconds(inCache[i].flags >> 2);
                byte block = (inCache[i].flags & 2) != 0 ? Block.custom_block : inCache[i].rawType;
                bool deleted = (inCache[i].flags & 1) != 0;
                Output(p, user, block, deleted, now - time);
            }

            if (!foundOne)
                Player.Message(p, "No block change records found for this block.");
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        static void Output(Player p, string user, byte block, bool deleted, TimeSpan delta) {
            string bName = Block.Name(block);
            user = Server.FindColor(user) + user;
            
            Player.Message(p, "{0} ago {1} {2}", delta.Shorten(true, false), user,
                           deleted ? "&4deleted%S (using " + bName + ")" : "&3placed%S " + bName);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/about");
            Player.Message(p, "%HOutputs the change/edit history for a block.");
        }
    }
}
