using System;

namespace MCGalaxy.Commands {

    public class CmdPromote : Command {

        public override string name { get { return "promote"; } }
        public override string shortcut { get { return "pr"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPromote() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            string promoter = (p == null) ? "&a<CONSOLE>" : p.color + p.name;
            string[] args = message.Split( trimChars, 2 );
            Player target = null;
            
            if (args.Length == 0 || (target = Player.Find(args[0])) == null) {
                Help(p);  return;
            }
            
            string reason = args.Length == 1 ? Server.customPromoteMessage : args[1];    
            Group next = null;
            int index = Group.GroupList.IndexOf(target.group);
            if (index < Group.GroupList.Count - 1) {
                Group nextHigher = Group.GroupList[index + 1];
                if (nextHigher.Permission < LevelPermission.Nobody)
                    next = nextHigher;
            }
            
            if (next != null) {
                Command.all.Find("setrank").Use(p, target.name + " " + next.name + " " + reason);
            } else {
                Player.SendMessage(p, "No higher ranks exist");
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/promote <name> <reason> - Promotes <name> up a rank");
            Player.SendMessage(p, "If <reason> is left blank, the server will use \"Unknown\"");
        }
    }
}
