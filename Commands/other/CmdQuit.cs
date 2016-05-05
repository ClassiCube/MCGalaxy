using System;

namespace MCGalaxy.Commands {
	
    public sealed class CmdQuit : Command {
		
        public override string name { get { return "quit"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            string msg = message != "" ? "Left the game: " + message : "Left the game.";
            p.LeaveServer(msg, msg);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/quit <reason> - Leave the server.");
        }
    }
}
