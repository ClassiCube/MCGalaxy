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
            int lenX = m[0].X + m[1].X, lenY = m[0].Y + m[1].Y, lenZ = m[0].Z + m[1].Z;
            int x = lenX / 2, y = lenY / 2, z = lenZ / 2;
            
            Place(p, x, y, z);
            if ((lenX & 1) == 1) Place(p, x + 1, y, z);
            if ((lenZ & 1) == 1) Place(p, x, y, z + 1);
            if ((lenX & 1) == 1 && (lenZ & 1) == 1) Place(p, x + 1, y, z + 1);
            
            // Top layer blocks
            if ((lenY & 1) == 1) {
                Place(p, x, y + 1, z);
                if ((lenX & 1) == 1) Place(p, x + 1, y + 1, z);
                if ((lenZ & 1) == 1) Place(p, x, y + 1, z + 1);
                if ((lenX & 1) == 1 && (lenZ & 1) == 1) Place(p, x + 1, y + 1, z + 1);
            }
            
            Player.Message(p, "Gold blocks were placed at ({0}, {1}, {2}).", x, y, z);
            return false;
        }
        
        static void Place(Player p, int x, int y, int z) {
            p.level.UpdateBlock(p, (ushort)x, (ushort)y, (ushort)z, Block.goldsolid, 0, true);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/center");
            Player.Message(p, "%HPlaces gold blocks at the center of your selection");
        }
    }
}
