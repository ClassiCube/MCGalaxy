using System;

namespace MCGalaxy.Commands
{
    public sealed class CmdCenter : Command
    {
        public override string name { get { return "center"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdCenter() { }
        public override void Use(Player p, string message)
        {
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        private void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            p.centerstart[0] = x;
            p.centerstart[1] = y;
            p.centerstart[2] = z;

            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        private void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            p.centerend[0] = x;
            p.centerend[1] = y;
            p.centerend[2] = z;

            int pos1 = (int)((p.centerstart[0] + p.centerend[0]) / 2);
            int pos2 = (int)((p.centerstart[1] + p.centerend[1]) / 2);
            int pos3 = (int)((p.centerstart[2] + p.centerend[2]) / 2);
            Command.all.Find("place").Use(p, "gold " + pos1 + " " + pos2 + " " + pos3);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/center - places a block at the center of your selection");
        }
    }

}
