/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
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
    public sealed class CmdView : Command
    {
        public override string name { get { return "view"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdView() { }

        public override void Use(Player p, string message)
        {
            if (!Directory.Exists("extra/text/")) Directory.CreateDirectory("extra/text");
            if (message == "")
            {
                DirectoryInfo di = new DirectoryInfo("extra/text/");
                string allFiles = "";
                foreach (FileInfo fi in di.GetFiles("*.txt"))
                {
                    try
                    {
                        string firstLine = File.ReadAllLines("extra/text/" + fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length) + ".txt")[0];
                        if (firstLine[0] == '#')
                        {
                            if (Group.Find(firstLine.Substring(1)).Permission <= p.group.Permission)
                            {
                                allFiles += ", " + fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
                            }
                        }
                        else
                        {
                            allFiles += ", " + fi.Name;
                        }
                    } catch (Exception e) { Server.ErrorLog(e); Player.SendMessage(p, "Error"); }
                }

                if (allFiles == "")
                {
                    Player.SendMessage(p, "No files are viewable by you");
                }
                else
                {
                    Player.SendMessage(p, "Available files:");
                    Player.SendMessage(p, allFiles.Remove(0, 2));
                }
            }
            else
            {
                Player who = null;
                if (message.IndexOf(' ') != -1)
                {
                    who = PlayerInfo.Find(message.Split(' ')[message.Split(' ').Length - 1]);
                    if (who != null)
                        message = message.Substring(0, message.LastIndexOf(' '));
                }
                if (who == null) who = p;

                if (File.Exists("extra/text/" + message + ".txt"))
                {
                    try
                    {
                        string[] allLines = File.ReadAllLines("extra/text/" + message + ".txt");
                        if (allLines[0][0] == '#')
                        {
                            if (Group.Find(allLines[0].Substring(1)).Permission <= p.group.Permission)
                            {
                                for (int i = 1; i < allLines.Length; i++)
                                {
                                    Player.SendMessage(who, allLines[i]);
                                }
                            }
                            else
                            {
                                Player.SendMessage(p, "You cannot view this file");
                            }
                        }
                        else
                        {
                            for (int i = 1; i < allLines.Length; i++)
                            {
                                Player.SendMessage(who, allLines[i]);
                            }
                        }
                    } catch { Player.SendMessage(p, "An error occurred when retrieving the file"); }
                }
                else
                {
                    Player.SendMessage(p, "File specified doesn't exist");
                }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/view [file] [player] - Views [file]'s contents");
            Player.SendMessage(p, "/view by itself will list all files you can view");
            Player.SendMessage(p, "If [player] is given, that player is shown the file");
        }
    }
}
