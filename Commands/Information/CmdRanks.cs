using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MCGalaxy.Commands
{
    public class CmdRanks : Command
    {
        public override string name { get { return "ranks"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdRanks() { }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player checking = null;
            if (message.Contains(' ')) checking = Player.Find(message.Substring(0, message.IndexOf(' ')));
            else checking = Player.Find(message);
            string checkname;
            if (checking == null)
            {
                Player.SendMessage(p, c.red + "Player not online, searching for the full name");
                if (message.Contains(' ')) checkname = message.Substring(0, message.IndexOf(' '));
                else checkname = message;
            }
            else checkname = checking.name;
            string path = "ranks/reasons/" + checkname + ".txt";
            if (!File.Exists(path)) { Player.SendMessage(p, c.red + "No rank changes found for &b" + checkname); return; }
            string[] lines = File.ReadAllLines(path);
            Player.SendMessage(p, "Player " + Server.FindColor(checkname.Trim()) + checkname + Server.DefaultColor + " has &b" + lines.Count() + Server.DefaultColor + " rank notes:");
            if (message.Contains(' '))
            {
                bool breaks = false;
                if (lines.Count() > 6) breaks = true;
                int count = 0;
                foreach (string line in lines)
                {
                    Player.SendMessage(p, c.red + line);
                    if (breaks)
                    {
                        count++;
                        if (count == 6)
                        {
                            Thread.Sleep(4000);
                            count = 0;
                        }
                    }
                }
            }
            else
            {
                if (lines.Count() > 5)
                {
                    for (int a = lines.Count() - 5; a < lines.Count(); a++)
                    {
                        Player.SendMessage(p, c.red + lines[a]);
                    }
                }
                else
                {
                    for (int a = 0; a < lines.Count(); a++)
                    {
                        Player.SendMessage(p, c.red + lines[a]);
                    }
                }
            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/ranks <playername> - shows recent promotions or demotions for the player");
            Player.SendMessage(p, "/ranks <playername> /all - shows all rank changes");
        }
    }
}
