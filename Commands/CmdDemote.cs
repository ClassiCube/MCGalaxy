using System;
using System.IO;

namespace MCGalaxy.Commands
{
    public sealed class CmdDemote : Command
    {
        public override string name { get { return "demote"; } }
        public override string shortcut { get { return "de"; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdDemote() { }

        public override void Use(Player p, string message)
        {
            string name;
            string reason;
            Group group;
            string demoter;
            demoter = (p == null) ? "&a<CONSOLE>" : p.color + p.name;
            Player player = Player.Find(message.Split(new char[] { ' ' })[0]);

            if (message == "")
            {
                Help(p); return;
            }
            if (player == null)
            {
                Help(p); return;
            }
            else
            {
                name = player.name;
                group = player.group;
            }

            if (message.Split(new char[] { ' ' }).Length == 1)
            {
                reason = "Unknown";
                Group group2 = null;
                bool flag = false;
                for (int i = Group.GroupList.Count - 1; i >= 0; i--)
                {
                Group group3 = Group.GroupList[i];
                if (flag)
                {
                    if (group3.Permission <= LevelPermission.Banned) break;
                    group2 = group3;
                    break;
                }
                if (group3 == group)
                    flag = true;
                }
                if (group2 != null)
                {
                    Command.all.Find("setrank").Use(p, name + " " + group2.name + " " + Server.customPromoteMessage);
                    player.RankReason(DateTime.Now, "&c[DEMOTED]", group2.name, reason, demoter);
                }
                else
                {
                    Player.SendMessage(p, "No higher ranks exist");
                }
            }
            else
            {
                reason = message.Substring(message.IndexOf(' ') + 1).Trim();
                Group group2 = null;
                bool flag = false;
                for (int i = Group.GroupList.Count - 1; i >= 0; i--)
                {
                    Group group3 = Group.GroupList[i];
                    if (flag)
                    {
                        if (group3.Permission <= LevelPermission.Banned) break;
                        group2 = group3;
                        break;
                    }
                    if (group3 == group)
                    flag = true;
                }
                if (group2 != null)
                {
                    Command.all.Find("setrank").Use(p, name + " " + group2.name + " " + Server.customPromoteMessage);
                    player.RankReason(DateTime.Now, "&c[DEMOTED]", group2.name, reason, demoter);
                }
                else
                {
                    Player.SendMessage(p, "No higher ranks exist");
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/demote <name> <reason> - Demotes <name> down a rank");
            Player.SendMessage(p, "If <reason> is left blank, the server will use \"Unknown\"");
        }
    }

}
