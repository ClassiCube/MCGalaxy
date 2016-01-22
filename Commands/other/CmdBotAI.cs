/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
    public sealed class CmdBotAI : Command
    {
        public override string name { get { return "botai"; } }
        public override string shortcut { get { return "bai"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdBotAI() { }

        public override void Use(Player p, string message)
        {
        	if (p == null) { MessageInGameOnly(p); return; }
            if (message.Split(' ').Length < 2) { Help(p); return; }
            string foundPath = message.Split(' ')[1].ToLower();

            if (!Player.ValidName(foundPath)) { Player.SendMessage(p, "Invalid AI name!"); return; }
            if (foundPath == "hunt" || foundPath == "kill") { Player.SendMessage(p, "Reserved for special AI."); return; }

            try
            {
                switch (message.Split(' ')[0])
                {
                    case "add":
                        if (message.Split(' ').Length == 2) addPoint(p, foundPath);
                        else if (message.Split(' ').Length == 3) addPoint(p, foundPath, message.Split(' ')[2]);
                        else if (message.Split(' ').Length == 4) addPoint(p, foundPath, message.Split(' ')[2], message.Split(' ')[3]);
                        else addPoint(p, foundPath, message.Split(' ')[2], message.Split(' ')[3], message.Split(' ')[4]);
                        break;
                    case "del":
                        if (!Directory.Exists("bots/deleted")) Directory.CreateDirectory("bots/deleted");

                        int currentTry = 0;
                        if (File.Exists("bots/" + foundPath))
                        {
                        retry: try
                            {
                                if (message.Split(' ').Length == 2)
                                {
                                    if (currentTry == 0)
                                        File.Move("bots/" + foundPath, "bots/deleted/" + foundPath);
                                    else
                                        File.Move("bots/" + foundPath, "bots/deleted/" + foundPath + currentTry);
                                }
                                else
                                {
                                    if (message.Split(' ')[2].ToLower() == "last")
                                    {
                                        string[] Lines = File.ReadAllLines("bots/" + foundPath);
                                        string[] outLines = new string[Lines.Length - 1];
                                        for (int i = 0; i < Lines.Length - 1; i++)
                                        {
                                            outLines[i] = Lines[i];
                                        }

                                        File.WriteAllLines("bots/" + foundPath, outLines);
                                        Player.SendMessage(p, "Deleted the last waypoint from " + foundPath);
                                        return;
                                    }
                                    else
                                    {
                                        Help(p); return;
                                    }
                                }
                            }
                            catch (IOException) { currentTry++; goto retry; }
                            Player.SendMessage(p, "Deleted &b" + foundPath);
                        }
                        else
                        {
                            Player.SendMessage(p, "Could not find specified AI.");
                        }
                        break;
                    default: Help(p); return;
                }
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/botai <add/del> [AI name] <extra> - Adds or deletes [AI name]");
            Player.SendMessage(p, "Extras: walk, teleport, wait, nod, speed, spin, reset, remove, reverse, linkscript, jump");
            Player.SendMessage(p, "wait, nod and spin can have an extra '0.1 seconds' parameter");
            Player.SendMessage(p, "nod and spin can also take a 'third' speed parameter");
            Player.SendMessage(p, "speed sets a percentage of normal speed");
            Player.SendMessage(p, "linkscript takes a script name as parameter");
        }

        public void addPoint(Player p, string foundPath, string additional = "", string extra = "10", string more = "2")
        {
            string[] allLines;
            try { allLines = File.ReadAllLines("bots/" + foundPath); }
            catch { allLines = new string[1]; }

            StreamWriter SW;
            try
            {
                if (!File.Exists("bots/" + foundPath))
                {
                    Player.SendMessage(p, "Created new bot AI: &b" + foundPath);
					using (SW = File.CreateText("bots/" + foundPath))
					{
						SW.WriteLine("#Version 2");
					}
                }
                else if (allLines[0] != "#Version 2")
                {
                    Player.SendMessage(p, "File found is out-of-date. Overwriting");
					File.Delete("bots/" + foundPath);
					using (SW = File.CreateText("bots/" + foundPath))
					{
						SW.WriteLine("#Version 2");
					}
                }
                else
                {
                    Player.SendMessage(p, "Appended to bot AI: &b" + foundPath);
                }
            }
            catch { Player.SendMessage(p, "An error occurred when accessing the files. You may need to delete it."); return; }

			try
			{
				using (SW = File.AppendText("bots/" + foundPath))
				{
					switch (additional.ToLower())
					{
						case "":
						case "walk":
							SW.WriteLine("walk " + p.pos[0] + " " + p.pos[1] + " " + p.pos[2] + " " + p.rot[0] + " " + p.rot[1]);
							break;
						case "teleport":
						case "tp":
							SW.WriteLine("teleport " + p.pos[0] + " " + p.pos[1] + " " + p.pos[2] + " " + p.rot[0] + " " + p.rot[1]);
							break;
						case "wait":
							SW.WriteLine("wait " + int.Parse(extra)); break;
						case "nod":
							SW.WriteLine("nod " + int.Parse(extra) + " " + int.Parse(more)); break;
						case "speed":
							SW.WriteLine("speed " + int.Parse(extra)); break;
						case "remove":
							SW.WriteLine("remove"); break;
						case "reset":
							SW.WriteLine("reset"); break;
						case "spin":
							SW.WriteLine("spin " + int.Parse(extra) + " " + int.Parse(more)); break;
						case "reverse":
							for (int i = allLines.Length - 1; i > 0; i--) if (allLines[i][0] != '#' && allLines[i] != "") SW.WriteLine(allLines[i]);
							break;
						case "linkscript":
							if (extra != "10") SW.WriteLine("linkscript " + extra); else Player.SendMessage(p, "Linkscript requires a script as a parameter");
							break;
						case "jump":
							SW.WriteLine("jump"); break;
						default:
							Player.SendMessage(p, "Could not find \"" + additional + "\""); break;
					}
				}
			}
			catch { Player.SendMessage(p, "Invalid parameter"); }
        }
    }
}
