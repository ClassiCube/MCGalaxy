using System;

namespace MCGalaxy.Commands.Building {	
	public sealed class CmdCenter : Command {
        public override string name { get { return "center"; } }
        public override string shortcut { get { return "centre"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdCenter() { }
        
        public override void Use(Player p, string message) {
            Player.Message(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.MakeSelection(2, null, DoCentre);
        }
        
        bool DoCentre(Player p, Vec3S32[] m, object state, byte type, byte extType) {
        	int xCen = (m[0].X + m[1].X) / 2, yCen = (m[0].Y + m[1].Y) / 2, zCen = (m[0].Z + m[1].Z) / 2;
            p.level.UpdateBlock(p, (ushort)xCen, (ushort)yCen, (ushort)zCen, Block.goldsolid, 0);
            Player.Message(p, "A gold block was placed at ({0}, {1}, {2}).", xCen, yCen, zCen);
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/center");
            Player.Message(p, "%HPlaces a block at the center of your selection");
        }
    }
}
