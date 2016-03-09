/*
	Copyright 2011 MCForge
		
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
namespace MCGalaxy.Commands
{
    public sealed class CmdTree : Command
    {
        public override string name { get { return "tree"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdTree() { }

        public override void Use(Player p, string message)
        {
            p.ClearBlockchange();
            switch (message.ToLower())
            {
                case "2":
                case "cactus": p.Blockchange += new Player.BlockchangeEventHandler(AddCactus); break;
                case "3":
                case "notch": p.Blockchange += new Player.BlockchangeEventHandler(AddNotchTree); break;
                case "4":
                case "swamp": p.Blockchange += new Player.BlockchangeEventHandler(AddNotchSwampTree); break;
                /*case "5":
                case "big": p.Blockchange += new Player.BlockchangeEventHandler(AddNotchBigTree); break;
                case "6":
                case "pine": p.Blockchange += new Player.BlockchangeEventHandler(AddNotchPineTree); break;*/
                default: p.Blockchange += new Player.BlockchangeEventHandler(AddTree); break;
            }
            Player.SendMessage(p, "Select where you wish your tree to grow");
            p.painting = false;
        }

        void AddTree(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            TreeGen.AddTree(p.level, x, y, z, p.random, true, true, p);
            if (!p.staticCommands) p.ClearBlockchange();
        }
        void AddNotchTree(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            TreeGen.AddNotchTree(p.level, x, y, z, p.random, true, true, p);
            if (!p.staticCommands) p.ClearBlockchange();
        }
        void AddNotchBigTree(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            TreeGen.AddNotchBigTree(p.level, x, y, z, p.random, true, true, p);
            if (!p.staticCommands) p.ClearBlockchange();
        }
        void AddNotchPineTree(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            TreeGen.AddNotchPineTree(p.level, x, y, z, p.random, true, true, p);
            if (!p.staticCommands) p.ClearBlockchange();
        }
        void AddNotchSwampTree(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            TreeGen.AddNotchSwampTree(p.level, x, y, z, p.random, true, true, p);
            if (!p.staticCommands) p.ClearBlockchange();
        }
        void AddCactus(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            TreeGen.AddCactus(p.level, x, y, z, p.random, true, true, p);
            if (!p.staticCommands) p.ClearBlockchange();
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tree [type] - Turns tree mode on or off.");
            Player.SendMessage(p, "Types - (Fern | 1), (Cactus | 2), (Notch | 3), (Swamp | 4)");
        }
    }
}
