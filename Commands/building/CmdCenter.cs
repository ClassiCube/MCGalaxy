using System;

namespace MCGalaxy.Commands {
	
    public sealed class CmdCenter : Command {
		
        public override string name { get { return "center"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdCenter() { }
        
        public override void Use(Player p, string message) {
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

       void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType)  {
            RevertAndClearState(p, x, y, z);
            p.centerstart[0] = x;
            p.centerstart[1] = y;
            p.centerstart[2] = z;

            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            p.centerend[0] = x;
            p.centerend[1] = y;
            p.centerend[2] = z;

            int xCen = (int)((p.centerstart[0] + p.centerend[0]) / 2);
            int yCen = (int)((p.centerstart[1] + p.centerend[1]) / 2);
            int zCen = (int)((p.centerstart[2] + p.centerend[2]) / 2);
            p.level.UpdateBlock(p, (ushort)xCen, (ushort)yCen, (ushort)zCen, (byte)Block.goldsolid, 0);
            Player.SendMessage(p, "A gold block was placed at (" + xCen + ", " + yCen + ", " + zCen + ").");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/center - places a block at the center of your selection");
        }
    }
}
