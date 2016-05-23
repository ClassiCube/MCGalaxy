using System;

namespace MCGalaxy.Commands.Moderation {
    public class CmdDemote : Command {
        public override string name { get { return "demote"; } }
        public override string shortcut { get { return "de"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdDemote() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            string demoter = (p == null) ? "&a<CONSOLE>" : p.color + p.name;
            string[] args = message.Split(trimChars, 2);
            Player target = null;
            
            if (args.Length == 0 || (target = PlayerInfo.Find(args[0])) == null) {
                Help(p); return;
            }
            
            string reason = args.Length == 1 ? Server.defaultDemoteMessage : args[1];    
            Group next = null;
            int index = Group.GroupList.IndexOf(target.group);
            if (index > 0) {
                Group nextLower = Group.GroupList[index - 1];
                if (nextLower.Permission > LevelPermission.Banned)
                    next = nextLower;
            }
            
            if (next != null) {
                Command.all.Find("setrank").Use(p, target.name + " " + next.name + " " + reason);
            } else {
                Player.Message(p, "No lower ranks exist");
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "/demote <name> <reason> - Demotes <name> down a rank");
            Player.Message(p, "If <reason> is left blank, the server will use \"Unknown\"");
        }
    }
}
