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
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands {    
    public sealed class CmdRestoreSelection : Command {        
        public override string name { get { return "rs"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            
            if (LevelInfo.ExistsBackup(p.level.name, message)) {
                p.SendMessage("Select two corners for restore.");
                p.MakeSelection(2, message, DoRestore);
            } else {
                Player.Message(p, "Backup " + message + " does not exist.");
            }
        }
        
        bool DoRestore(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            string path = LevelInfo.BackupPath(p.level.name, (string)state);           
            try {
                using (Level other = LvlFile.Load("tempLevel", path))
                    return CopyBlocks(p, other, marks);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Restore selection failed");
                return false;
            }
        }
        
        static bool CopyBlocks(Player p, Level other, Vec3S32[] m) {
            byte[] blocks = other.blocks;
            if (blocks.Length != p.level.blocks.Length) {
                p.SendMessage("Cant restore selection of different size maps.");
                return false;
            }
            
            int width = other.Width, length = other.Length;
            for (int y = Math.Min(m[0].Y, m[1].Y); y <= Math.Max(m[0].Y, m[1].Y); y++)
                for (int z = Math.Min(m[0].Z, m[1].Z); z <= Math.Max(m[0].Z, m[1].Z); z++)
                    for (int x = Math.Min(m[0].X, m[1].X); x <= Math.Max(m[0].X, m[1].X); x++)
            {
                byte block = blocks[x + width * (z + y * length)], extBlock = 0;
                if (block == Block.custom_block) 
                    extBlock = other.GetExtTile((ushort)x, (ushort)y, (ushort)z);
                p.level.UpdateBlock(p, (ushort)x, (ushort)y, (ushort)z, block, extBlock, true);
            }
            return true;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/restoreselection <number>");
            Player.Message(p, "%Hrestores a previous backup of the current selection");
        }
    }
}
