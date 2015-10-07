using System;
using System.IO;

namespace MCGalaxy.Commands
{

    public class CmdPromote : Command
    {

        public override string name { get { return "promote"; } }
        public override string shortcut { get { return "pr"; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPromote() { }

        public override void Use(Player p, string message)
        {
            string name;
            string reason;
            Group group;
            string promoter;
            promoter = (p == null) ? "&a<CONSOLE>" : p.color + p.name;
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
                for (int i = 0; i < Group.GroupList.Count; i++)
                {
                    Group group3 = Group.GroupList[i];
                    if (flag)
                    {
                        if (group3.Permission < LevelPermission.Nobody)
                        {
                            group2 = group3;
                        }
                        break;
                    }
                    if (group3 == group)
                    {
                        flag = true;
                    }
                }
                if (group2 != null)
                {
                    Command.all.Find("setrank").Use(p, name + " " + group2.name + " " + Server.customPromoteMessage);
                    player.RankReason(DateTime.Now, "&a[PROMOTED]", group2.name, reason, promoter);
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
                for (int i = 0; i < Group.GroupList.Count; i++)
                {
                    Group group3 = Group.GroupList[i];
                    if (flag)
                    {
                        if (group3.Permission < LevelPermission.Nobody)
                        {
                            group2 = group3;
                        }
                        break;
                    }
                    if (group3 == group)
                    {
                        flag = true;
                    }
                }
                if (group2 != null)
                {
                    Command.all.Find("setrank").Use(p, name + " " + group2.name + " " + Server.customPromoteMessage);
                    player.RankReason(DateTime.Now, "&a[PROMOTED]", group2.name, reason, promoter);
                }
                else
                {
                    Player.SendMessage(p, "No higher ranks exist");
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/promote <name> <reason>- Promotes <name> up a rank");
            Player.SendMessage(p, "If <reason> is left blank, the server will use \"Unknown\"");
        }
    }

}
