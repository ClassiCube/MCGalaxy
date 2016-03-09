/*
	Copyright 2011 MCForge
		
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
using System.IO;
using System.Text.RegularExpressions;
namespace MCGalaxy.Commands
{
    public sealed class CmdText : Command
    {
        public override string name { get { return "text"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public CmdText() { }

        public override void Use(Player p, string message)
        {
            if (message.ToLower() == "superops" || message.ToLower() == "ops" || message.ToLower() == "advbuilders" || message.ToLower() == "builders")
            {
                Player.SendMessage(p, "You cannot try to promote yourself with /text! You have been reported to all Ops!");
                Chat.GlobalMessageOps(p.color + p.DisplayName + Server.DefaultColor + " tried to promote themselves by using /text!!");
                Server.s.Log(p.name + " tried to promote themselves using /text!!");
                return;
            }
            
            // Create the directory if it doesn't exist
            if (!Directory.Exists("extra/text/"))
                Directory.CreateDirectory("extra/text");

            // Show the help if the message doesn't contain enough parameters
            if (message.IndexOf(' ') == -1)
            {
                Help(p);
                return;
            }

            string[] param = message.Split(' ');

            try
            {
                if (param[0].ToLower() == "delete")
                {
                    string filename = SanitizeFileName(param[1]) + ".txt";
                    if (File.Exists("extra/text/" + filename))
                    {
                        File.Delete("extra/text/" + filename);
                        Player.SendMessage(p, "Deleted file: " + filename);
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "Could not find file: " + filename);
                        return;
                    }
                }
                else
                {
                    bool again = false;
                    string filename = SanitizeFileName(param[0]) + ".txt";
                    string path = "extra/text/" + filename;
                    
                    // See if we match the group
                    string group = Group.findPerm(LevelPermission.Guest).name;
                    if (Group.Find(param[1]) != null)
                    {
                        group = Group.Find(param[1]).name;
                        again = true;
                    }

                    message = message.Substring(message.IndexOf(' ') + 1);
                    if (again)
                        message = message.Substring(message.IndexOf(' ') + 1);

                    string contents = message;
                    if (contents == "")
                    {
                        Help(p);
                        return;
                    }

                    if (!File.Exists(path))
                        contents = "#" + group + System.Environment.NewLine + contents;
                    else
                        contents = " " + contents;

                    File.AppendAllText(path, contents);
                    Player.SendMessage(p, "Added text to: " + filename);
                }
            } catch { Help(p); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/text [file] [rank] [message] - Makes a /view-able text");
            Player.SendMessage(p, "The [rank] entered is the minimum rank to view the file");
            Player.SendMessage(p, "The [message] is entered into the text file");
            Player.SendMessage(p, "If the file already exists, text will be added to the end");
        }

        private string SanitizeFileName(string filename)
        {
            return Regex.Replace(filename, @"[^\d\w\-]", "");
        }
    }
}
