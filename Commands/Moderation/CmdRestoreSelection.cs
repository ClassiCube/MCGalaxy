/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.IO;
using System.IO.Compression;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands
{
    public sealed class CmdRestoreSelection : Command
    {
        public override string name { get { return "rs"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message)
        {
            if (message != "")
                if (File.Exists(@Server.backupLocation + "/" + p.level.name + "/" + message + "/" + p.level.name + ".lvl"))
                {
                    try
                    {
                        p.blockchangeObject = new CatchPos() { backup = int.Parse(message) };
                        p.ClearBlockchange();
                        p.Blockchange += Blockchange1;
                        p.SendMessage("Select two corners for restore.");
                    }
                    catch { Server.s.Log("Restore fail"); }
                }
                else Player.SendMessage(p, "Backup " + message + " does not exist."); 
            else Help(p);
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            p.SendBlockchange(x, y, z, p.level.GetTile(x, y, z));
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += Blockchange2;
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            p.SendBlockchange(x, y, z, p.level.GetTile(x, y, z));
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            string path = @Server.backupLocation + "/" + p.level.name + "/" + cpos.backup + "/" + p.level.name + ".lvl";
            
            try {
            	using(Level other = LvlFile.Load("tempLevel", path, false)) {
            		if (!CopyBlocks(p, other, x, y, z, cpos)) return;
            	}
                if (p.staticCommands) 
                	p.Blockchange += Blockchange1;
            } catch { 
            	Server.s.Log("Restore selection failed"); 
            }
        }
        
        static bool CopyBlocks(Player p, Level other, ushort x, ushort y, ushort z, CatchPos cpos) {
            byte[] blocks = other.blocks;
            if (blocks.Length != p.level.blocks.Length) { 
                p.SendMessage("Cant restore selection of different size maps.");
                return false;
            }
            int width = other.Width, height = other.Length;

            if (p.level.bufferblocks && !p.level.Instant) {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                {
                    BlockQueue.Addblock(p, xx, yy, zz, blocks[xx + (zz * width) + (yy * width * height)]);
                }
            } else {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        
                        for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                {
                    p.level.Blockchange(p, xx, yy, zz, blocks[xx + (zz * width) + (yy * width * height)]);
                }
            }
            return true;
        }

        struct CatchPos
        {
            public int backup;
            public ushort x, y, z;
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/restoreselection <number> - restores a previous backup of the current selection");
        }
    }
}
