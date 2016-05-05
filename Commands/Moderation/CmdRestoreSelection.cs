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
                p.blockchangeObject = new CatchPos() { backup = message };
                p.ClearBlockchange();
                p.Blockchange += Blockchange1;
                p.SendMessage("Select two corners for restore.");
            } else {
                Player.Message(p, "Backup " + message + " does not exist.");
            }
        }

        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += Blockchange2;
        }

        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            string path = LevelInfo.BackupPath(p.level.name, cpos.backup);
            
            try {
                using(Level other = LvlFile.Load("tempLevel", path)) {
                    if (!CopyBlocks(p, other, x, y, z, cpos)) return;
                }
                if (p.staticCommands)
                    p.Blockchange += Blockchange1;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Restore selection failed");
            }
        }
        
        static bool CopyBlocks(Player p, Level other, ushort x, ushort y, ushort z, CatchPos cpos) {
            byte[] blocks = other.blocks;
            if (blocks.Length != p.level.blocks.Length) {
                p.SendMessage("Cant restore selection of different size maps.");
                return false;
            }
            
            int width = other.Width, length = other.Length;
            for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
            {
                byte tile = blocks[xx + width * (zz + yy * length)], extTile = 0;
                if (tile == Block.custom_block) extTile = other.GetExtTile(xx, yy, zz);
                p.level.UpdateBlock(p, xx, yy, zz, tile, extTile);
            }
            return true;
        }

        struct CatchPos { public string backup; public ushort x, y, z; }

        public override void Help(Player p) {
            Player.Message(p, "/restoreselection <number> - restores a previous backup of the current selection");
        }
    }
}
