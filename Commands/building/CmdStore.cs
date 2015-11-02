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
    public sealed class CmdStore : Command
    {
        public override string name { get { return "store"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public List<CopyOwner> list = new List<CopyOwner>();
        public CmdStore() { }

        public override void Use(Player p, string message)
        {
            try
            {
                if (message == "") { Help(p); return; }

                if (message.IndexOf(' ') == -1)
                {
                    if (File.Exists("extra/copy/" + message + ".copy"))
                    {
                        Player.SendMessage(p, "File: &f" + message + Server.DefaultColor + " already exists.  Delete first");
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "Storing: " + message);
						File.Create("extra/copy/" + message + ".copy").Dispose();
						using (StreamWriter sW = File.CreateText("extra/copy/" + message + ".copy"))
						{
							sW.WriteLine("Saved by: " + p.name + " at " + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss "));
							for (int k = 0; k < p.CopyBuffer.Count; k++)
							{
								sW.WriteLine(p.CopyBuffer[k].x + " " + p.CopyBuffer[k].y + " " + p.CopyBuffer[k].z + " " + p.CopyBuffer[k].type);
							}
						}
						using (StreamWriter sW = File.AppendText("extra/copy/index.copydb"))
						{
							sW.WriteLine(message + " " + p.name);
						}
                    }
                }
                else
                {
                    if (message.Split(' ')[0] == "delete")
                    {
                        message = message.Split(' ')[1];
                        list.Clear();
                        foreach (string s in File.ReadAllLines("extra/copy/index.copydb"))
                        {
                            CopyOwner cO = new CopyOwner();
                            cO.file = s.Split(' ')[0];
                            cO.name = s.Split(' ')[1];
                            list.Add(cO);
                        }
                        CopyOwner result = list.Find(
                            delegate(CopyOwner cO) {
                                return cO.file == message;
                            }
                        );

                        if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this) || result.name == p.name)
                        {
                            if (File.Exists("extra/copy/" + message + ".copy"))
                            {
                                try
                                {
                                    if (File.Exists("extra/copyBackup/" + message + ".copy")) { File.Delete("extra/copyBackup/" + message + ".copy"); }
                                    File.Move("extra/copy/" + message + ".copy", "extra/copyBackup/" + message + ".copy");
                                }
                                catch { }
                                Player.SendMessage(p, "File &f" + message + Server.DefaultColor + " has been deleted.");
                                list.Remove(result);
                                File.Create("extra/copy/index.copydb").Dispose();
								using (StreamWriter sW = File.CreateText("extra/copy/index.copydb"))
								{
									foreach (CopyOwner cO in list)
									{
										sW.WriteLine(cO.file + " " + cO.name);
									}
								}
                            }
                            else
                            {
                                Player.SendMessage(p, "File does not exist.");
                            }
                        }
                        else
                        {
                            Player.SendMessage(p, "You must be an " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+ or file owner to delete a save.");
                            return;
                        }
                    }
                    else { Help(p); return; }
                }

            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
        public class CopyOwner
        {
            public string name;
            public string file;
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/store <filename> - Stores your copied item to the server as <filename>.");
            Player.SendMessage(p, "/store delete <filename> - Deletes saved copy file.  Only " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+ and file creator may delete.");
        }
    }
}
