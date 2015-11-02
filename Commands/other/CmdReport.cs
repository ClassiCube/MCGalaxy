/*
 * Written By Jack1312

	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdReport : Command
    {
        public override string name { get { return "report"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdReport() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "This command can not be used in console!");
                return;
            }
            if (message == "")
            {
                Help(p);
                return;
            }
            int length = message.Split(' ').Length;
            try
            {
                switch (message.Split()[0])
                {
                    case "list":
                        if (length == 1 && (int)p.group.Permission >= CommandOtherPerms.GetPerm(this))
                        {
                            if (!Directory.Exists("extra/reported"))
                                Directory.CreateDirectory("extra/reported");
                            bool foundone = false;
                            FileInfo[] fi = new DirectoryInfo("extra/reported").GetFiles("*.txt");
                            Player.SendMessage(p, "The following players have been reported:");
                            foreach (FileInfo file in fi)
                            {
                                foundone = true;
                                var parsed = file.Name.Replace(".txt", "");
                                Player.SendMessage(p, "- %c" + parsed);
                            }
                            if (foundone)
                            {
                                Player.SendMessage(p, "Use %f/report check [Player] " + Server.DefaultColor + "to view report info.");
                                Player.SendMessage(p, "Use %f/report delete [Player] " + Server.DefaultColor + "to delete a report");
                            }
                            else
                                Player.SendMessage(p, "%cNo reports were found!");
                        }
                        else
                            Player.SendMessage(p, "%cYou cannot use 'list' as a report name!");
                        break;
                    case "view":
                    case "read":
                    case "check":
                        if (message.Split().Length == 2 && (int)p.group.Permission >= CommandOtherPerms.GetPerm(this))
                        {
                            if (!File.Exists("extra/reported/" + message.Split()[1] + ".txt"))
                            {
                                Player.SendMessage(p, "%cThe player you specified has not been reported!");
                                return;
                            }
                            var readtext = File.ReadAllText("extra/reported/" + message.Split()[1] + ".txt");
                            Player.SendMessage(p, readtext);
                        }
                        else
                            Player.SendMessage(p, "%cYou cannot use 'check' as a report name! ");
                        break;
                    case "delete":
                    case "remove":
                        if (message.Split().Length == 2 && (int)p.group.Permission >= CommandOtherPerms.GetPerm(this))
                        {
                            string msg = message.Split()[1];
                            if (!File.Exists("extra/reported/" + msg + ".txt"))
                            {
                                Player.SendMessage(p, "%cThe player you specified has not been reported!");
                                return;
                            }
                            if (!Directory.Exists("extra/reportedbackups"))
                                Directory.CreateDirectory("extra/reportedbackups");
                            if (File.Exists("extra/reportedbackups/" + msg + ".txt"))
                                File.Delete("extra/reportedbackups/" + msg + ".txt");
                            File.Move("extra/reported/" + msg + ".txt", "extra/reportedbackups/" + msg + ".txt");
                            Player.SendMessage(p, "%a" + msg + "'s report has been deleted.");
                            Player.GlobalMessageOps(p.prefix + p.color + p.name + Server.DefaultColor + " deleted " + msg + "'s report.");
                            Server.s.Log(msg + "'s report has been deleted by " + p.name);
                        }
                        else
                            Player.SendMessage(p, "%cYou cannot use 'delete' as a report name! ");
                        break;
                    case "clear":
                        if (length == 1 && (int)p.group.Permission >= CommandOtherPerms.GetPerm(this))
                        {
                            if (!Directory.Exists("extra/reported"))
                                Directory.CreateDirectory("extra/reported");
                            FileInfo[] fi = new DirectoryInfo("extra/reported").GetFiles("*.txt");
                            foreach (FileInfo file in fi)
                            {
                                if (File.Exists("extra/reportedbackups/" + file.Name))
                                    File.Delete("extra/reportedbackups/" + file.Name);
                                file.MoveTo("extra/reportedbackups/" + file.Name);
                            }
                            Player.SendMessage(p, "%aYou have cleared all reports!");
                            Player.GlobalMessageOps(p.prefix + p.name + "%c cleared ALL reports!");
                            Server.s.Log(p.name + " cleared ALL reports!");
                        }
                        else
                            Player.SendMessage(p, "%cYou cannot use 'clear' as a report name! ");
                        break;
                    default:
                        string msg1 = "";
                        string msg2 = "";
                        try
                        {
                            msg1 = message.Substring(0, message.IndexOf(' ')).ToLower();
                            msg2 = message.Substring(message.IndexOf(' ') + 1).ToLower();
                        }
                        catch { return; }
                        if (File.Exists("extra/reported/" + msg1 + ".txt"))
                        {
                            File.WriteAllText("extra/reported/" + msg1 + "(2).txt", msg2 + " - Reported by " + p.name + "." + " DateTime: " + DateTime.Now);
                            Player.SendMessage(p, "%aYour report has been sent, it should be viewed when an operator is online!");
                            break;
                        }
                        if (File.Exists("extra/reported/" + msg1 + "(2).txt"))
                        {
                            Player.SendMessage(p, "%cThe player you've reported has already been reported 2 times! Please wait patiently untill an OP+ has reviewed the reports!");
                            break;
                        }
                        if (!Directory.Exists("extra/reported"))
                            Directory.CreateDirectory("extra/reported");
                        File.WriteAllText("extra/reported/" + msg1 + ".txt", msg2 + " - Reported by " + p.name + "." + " DateTime: " + DateTime.Now);
                        Player.SendMessage(p, "%aYour report has been sent, it should be viewed when an operator is online!");
                        Player.GlobalMessageOps(p.prefix + p.name + Server.DefaultColor + " has made a report, view it with %f/report list ");
                        break;
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }



        public override void Help(Player p)
        {
            Player.SendMessage(p, "%f/report [Player] [Reason] " + Server.DefaultColor + "- Reports the specified player for the reason");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this))
            {
                Player.SendMessage(p, "%f/report list " + Server.DefaultColor + "- Checks the reported list!");
                Player.SendMessage(p, "%f/report check [Player] " + Server.DefaultColor + "- View the report on the specified player");
                Player.SendMessage(p, "%f/report delete [Player] " + Server.DefaultColor + "- Delete the report on the specified player");
                Player.SendMessage(p, "%f/report clear " + Server.DefaultColor + "- %c!!!Clears all reports!!!");
            }
        }
    }
}
