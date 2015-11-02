using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCGalaxy.Commands
{
    public sealed class CmdQuit : Command
    {
        public override string name { get { return "quit"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdQuit() { }

        public override void Use(Player p, string message)
        {
            p.totalKicked = p.totalKicked - 1;
            if (message != "")
            {
                p.Kick("Left the game: " + message);
            }
            else
            {
                p.Kick("Left the game.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/quit <reason> - Leave the server.");
        }
    }
}
