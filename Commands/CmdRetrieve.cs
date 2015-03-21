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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdRetrieve : Command
    {
        public override string name { get { return "retrieve"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public List<CopyOwner> list = new List<CopyOwner>();
        public CmdRetrieve() { }

        public override void Use(Player p, string message)
        {
            try
            {
                if (!File.Exists("extra/copy/index.copydb")) { File.Create("extra/copy/index.copydb").Dispose(); /*Player.SendMessage(p, "No copy index found! Save something before trying to retrieve it!"); return;*/ }
                if (message == "") { Help(p); return; }
                if (message.Split(' ')[0] == "info")
                {
                    if (message.IndexOf(' ') != -1)
                    {
                        message = message.Split(' ')[1];
                        if (File.Exists("extra/copy/" + message + ".copy"))
                        {
							using (StreamReader sR = new StreamReader("extra/copy/" + message + ".copy"))
							{
								string infoline = sR.ReadLine();
								Player.SendMessage(p, infoline);
							}
                            return;
                        }
                    }
                    else
                    {
                        Help(p);
                        return;
                    }
                }
                if (message.Split(' ')[0] == "find")
                {
                    message = message.Replace("find", "");
                    string storedcopies = ""; int maxCopies = 0; int findnum = 0; int currentnum = 0;
                    bool isint = int.TryParse(message, out findnum);
                    if (message == "") { goto retrieve; }
                    if (!isint)
                    {
                        message = message.Trim();
                        list.Clear();
                        foreach (string s in File.ReadAllLines("extra/copy/index.copydb"))
                        {
                            CopyOwner cO = new CopyOwner();
                            cO.file = s.Split(' ')[0];
                            cO.name = s.Split(' ')[1];
                            list.Add(cO);
                        }
                        List<CopyOwner> results = new List<CopyOwner>();
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].name.ToLower() == message.ToLower())
                            {
                                storedcopies += ", " + list[i].file;
                            }
                        }
                        if (storedcopies == "") { Player.SendMessage(p, "No saves found for player: " + message); }
                        else
                        {
                            Player.SendMessage(p, "Saved copy files: ");
                            Player.SendMessage(p, "&f " + storedcopies.Remove(0, 2));
                        }
                        return;
                    }

                    // SEARCH BASED ON NAME STUFF ABOVE HERE
                    if (isint)
                    {
                        maxCopies = findnum * 50; currentnum = maxCopies - 50;
                    }
        retrieve:   DirectoryInfo di = new DirectoryInfo("extra/copy/");
                    FileInfo[] fi = di.GetFiles("*.copy");

                    if (maxCopies == 0)
                    {
                        foreach (FileInfo file in fi)
                        {
                            storedcopies += ", " + file.Name.Replace(".copy", "");
                        }
                        if (storedcopies != "")
                        {
                            Player.SendMessage(p, "Saved copy files: ");
                            Player.SendMessage(p, "&f " + storedcopies.Remove(0, 2));
                            if (fi.Length > 50) { Player.SendMessage(p, "For a more structured list, use /retrieve find <1/2/3/...>"); }
                        }
                        else { Player.SendMessage(p, "There are no saved copies."); }
                    }
                    else
                    {
                        if (maxCopies > fi.Length) maxCopies = fi.Length;
                        if (currentnum > fi.Length) { Player.SendMessage(p, "No saved copies beyond number " + fi.Length); return; }

                        Player.SendMessage(p, "Saved copies (" + currentnum + " to " + maxCopies + "):");
                        for (int i = currentnum; i < maxCopies; i++)
                        {
                            storedcopies += ", " + fi[i].Name.Replace(".copy", "");
                        }
                        if (storedcopies != "")
                        {
                            Player.SendMessage(p, "&f" + storedcopies.Remove(0, 2));
                        }
                        else Player.SendMessage(p, "There are no saved copies.");
                    }
                }
                else
                {
                    if (message.IndexOf(' ') == -1)
                    {
                        message = message.Split(' ')[0];
                        if (File.Exists("extra/copy/" + message + ".copy"))
                        {
                            p.CopyBuffer.Clear();
                            bool readFirst = false;
                            foreach (string s in File.ReadAllLines("extra/copy/" + message + ".copy"))
                            {
                                if (readFirst)
                                {
                                    Player.CopyPos cP;
                                    cP.x = Convert.ToUInt16(s.Split(' ')[0]);
                                    cP.y = Convert.ToUInt16(s.Split(' ')[1]);
                                    cP.z = Convert.ToUInt16(s.Split(' ')[2]);
                                    cP.type = Convert.ToByte(s.Split(' ')[3]);
                                    p.CopyBuffer.Add(cP);
                                }
                                else readFirst = true;
                            }
                            Player.SendMessage(p, "&f" + message + Server.DefaultColor + " has been placed copybuffer.  Paste away!");
                        }
                        else
                        {
                            Player.SendMessage(p, "Could not find copy specified");
                            return;
                        }
                    }
                    else 
                    { 
                        Help(p); 
                        return; 
                    }
                }
            }
            catch (Exception e) { Player.SendMessage(p, "An error occured"); Server.ErrorLog(e); }
        }

        public class CopyOwner
        {
            public string name;
            public string file;
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/retrieve <filename> - Retrieves saved copy file to your copy buffer. /paste to place it!");
            Player.SendMessage(p, "/retrieve info <filename> - Gets information about the saved file.");
            Player.SendMessage(p, "/retrieve find - Prints a list of all saved copies.");
            Player.SendMessage(p, "/retrieve find <1/2/3/..> - Shows a compact list.");
            Player.SendMessage(p, "/retrieve find <name> - Prints a list of all saved copies made by player <name>.");
            return;
        }
    }
}

    
        