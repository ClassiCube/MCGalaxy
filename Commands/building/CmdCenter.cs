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
            p.blockchangeObject = new Vec3U16(x, y, z);
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            Vec3U16 start = (Vec3U16)p.blockchangeObject;
            int xCen = (start.X + x) / 2, yCen = (start.Y + y) / 2, zCen = (start.Z + z) / 2;
            p.level.UpdateBlock(p, (ushort)xCen, (ushort)yCen, (ushort)zCen, Block.goldsolid, 0);
            Player.SendMessage(p, "A gold block was placed at (" + xCen + ", " + yCen + ", " + zCen + ").");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/center - places a block at the center of your selection");
        }
    }
}
