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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands
{
    public sealed class CmdAbout : Command
    {
        public override string name { get { return "about"; } }
        public override string shortcut { get { return "b"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdAbout() { }

        public override void Use(Player p, string message)
        {
        	if (p == null) { MessageInGameOnly(p); return; }

            Player.SendMessage(p, "Break/build a block to display information.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(AboutBlockchange);            
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/about - Displays information about a block.");
        }

        public void AboutBlockchange(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            if (!p.staticCommands) p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            if (b == Block.Zero) { Player.SendMessage(p, "Invalid Block(" + x + "," + y + "," + z + ")!"); return; }
            p.SendBlockchange(x, y, z, b);

            string message = "Block (" + x + "," + y + "," + z + "): ";
            message += "&f" + b + " = " + Block.Name(b);
            Player.SendMessage(p, message + Server.DefaultColor + ".");
            message = p.level.foundInfo(x, y, z);
            if (message != "") Player.SendMessage(p, "Physics information: &a" + message);

            //safe against SQL injections because no user input is given here
            DataTable Blocks = Database.fillData("SELECT * FROM `Block" + p.level.name + "` WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z); 

            string Username, TimePerformed, BlockUsed;
            bool Deleted, foundOne = false;
            for (int i = 0; i < Blocks.Rows.Count; i++)
            {
                foundOne = true;
                Username = Blocks.Rows[i]["Username"].ToString();
                TimePerformed = DateTime.Parse(Blocks.Rows[i]["TimePerformed"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                //Server.s.Log(Blocks.Rows[i]["Type"].ToString());
                BlockUsed = Block.Name(Convert.ToByte(Blocks.Rows[i]["Type"])).ToString();
                Deleted = Convert.ToBoolean(Blocks.Rows[i]["Deleted"]);

                if (!Deleted)
                    Player.SendMessage(p, "&3Created by " + Server.FindColor(Username.Trim()) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed);
                else
                    Player.SendMessage(p, "&4Destroyed by " + Server.FindColor(Username.Trim()) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed);
                Player.SendMessage(p, "Date and time modified: &2" + TimePerformed);
            }

            int bpIndex = p.level.PosToInt(x, y, z);
            List<Level.BlockPos> inCache = p.level.blockCache.FindAll(bP => bP.index == bpIndex);

            for (int i = 0; i < inCache.Count; i++)
            {
                foundOne = true;
                Deleted = (inCache[i].flags & 1) != 0;
                Username = inCache[i].name;
                DateTime time = Server.StartTimeLocal.AddSeconds(inCache[i].flags >> 2);
                TimePerformed = time.ToString("yyyy-MM-dd HH:mm:ss");
                byte inBlock = (inCache[i].flags & 2) != 0 ? Block.custom_block : inCache[i].rawType;
                BlockUsed = Block.Name(inBlock);

                if (!Deleted)
                    Player.SendMessage(p, "&3Created by " + Server.FindColor(Username.Trim()) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed);
                else
                    Player.SendMessage(p, "&4Destroyed by " + Server.FindColor(Username.Trim()) + Username.Trim() + Server.DefaultColor + ", using &3" + BlockUsed);
                Player.SendMessage(p, "Date and time modified: &2" + TimePerformed);
            }

            if (!foundOne)
                Player.SendMessage(p, "This block has not been modified since the map was cleared.");
 
            Blocks.Dispose();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
