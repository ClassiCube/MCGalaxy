using System;

namespace MCGalaxy.Commands {

    public class CmdDemote : Command {

        public override string name { get { return "demote"; } }
        public override string shortcut { get { return "de"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdDemote() { }

        public override void Use(Player p, string message) {
            string demoter = (p == null) ? "&a<CONSOLE>" : p.color + p.name;
            string[] args = message.Split( new char[] { ' ' }, 2 );
            Player target = null;
            
            if (args.Length == 0 || (target = Player.Find(args[0])) == null) {
                Help(p); 
                return;
            }
            
            string reason = args.Length == 1 ? "Unknown" : args[1];    
            Group next = null;
            int index = Group.GroupList.IndexOf(target.group);
            if (index > 0) {
                Group nextLower = Group.GroupList[index - 1];
                if (nextLower.Permission > LevelPermission.Banned)
                    next = nextLower;
            }
            
            if (next != null) {
                Command.all.Find("setrank").Use(p, target.name + " " + next.name + " " + Server.customDemoteMessage);
                target.RankReason(DateTime.Now, "&a[DEMOTED]", next.name, reason, demoter);
            } else {
                Player.SendMessage(p, "No lower ranks exist");
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/demote <name> <reason> - Demotes <name> down a rank");
            Player.SendMessage(p, "If <reason> is left blank, the server will use \"Unknown\"");
        }
    }
}
