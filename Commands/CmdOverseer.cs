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
using System.IO;
using System.Linq;
namespace MCGalaxy.Commands
{
	public sealed class CmdOverseer : Command
	{
		public override string name { get { return "overseer"; } }
		public override string shortcut { get { return "os"; } }
		public override string type { get { return "commands"; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
		public CmdOverseer() { }
		public override void Use(Player p, string message)
		{

			if (message == "") { Help(p); return; }
			Player who = Player.Find(message.Split(' ')[0]);
			string cmd = message.Split(' ')[0].ToUpper();

			string par;
			try
			{ par = message.Split(' ')[1].ToUpper(); }
			catch
			{ par = ""; }

			string par2;
			try
			{ par2 = message.Split(' ')[2]; }
			catch
			{ par2 = ""; }

			string par3;
			try
			{ par3 = message.Split(' ')[3]; }
			catch
			{ par3 = ""; }

			if (cmd == "GO")
			{
				if ((par == "1") || (par == ""))
				{
					string mapname = properMapName(p, false);
					if (!Server.levels.Any(l => l.name == mapname))
					{
						Command.all.Find("load").Use(p, mapname);
					}
					Command.all.Find("goto").Use(p, mapname);
				}
				else if (par == "2")
				{
					string mapname = p.name.ToLower() + "2";
					if (!Server.levels.Any(l => l.name == mapname))
					{
						Command.all.Find("load").Use(p, mapname);
					}
					Command.all.Find("goto").Use(p, mapname);
				}
			}
			// Set Spawn (if you are on your own map level)
			else if (cmd == "SPAWN")
			{
				if ((p.name.ToUpper() == p.level.name.ToUpper()) || (p.name.ToUpper() + "00" == p.level.name.ToUpper()) || (p.name.ToUpper() + "2" == p.level.name.ToUpper()))
				{
					Command.all.Find("setspawn").Use(p, "");
				}
				else { Player.SendMessage(p, "You can only change the Spawn Point when you are on your own map."); }
			}
			// Map Commands
			else if (cmd == "MAP")
			{
				if (par == "ADD")
				{
					if ((File.Exists(@"levels\" + p.name.ToLower() + ".lvl")) || (File.Exists(@"levels\" + p.name.ToLower() + "00.lvl")))
					{
						if (!File.Exists(@"levels\" + p.name.ToLower() + "2.lvl"))
						{
							Player.SendMessage(p, p.color + p.name + Server.DefaultColor + " you already have a map, let me create a second one for you.");
							string mType;
							if (par2.ToUpper() == "" || par2.ToUpper() == "DESERT" || par2.ToUpper() == "FLAT" || par2.ToUpper() == "FOREST" || par2.ToUpper() == "ISLAND" || par2.ToUpper() == "MOUNTAINS" || par2.ToUpper() == "OCEAN" || par2.ToUpper() == "PIXEL" || par2.ToUpper() == "SPACE")
							{
								if (par2 != "")
								{
									mType = par2;
								}
								else
								{
									mType = "flat";
								}
								Player.SendMessage(p, "Creating your 2nd map, " + p.color + p.name);
								Command.all.Find("newlvl").Use(p, p.name.ToLower() + "2 " + mSize(p) + " " + mType);
							}
							else
							{
								Player.SendMessage(p, "A wrong map type was specified. Valid map types: Desert, flat, forest, island, mountians, ocean, pixel and space.");
							}
						}
						else
						{
							Player.SendMessage(p, p.color + p.name + Server.DefaultColor + " you already have two maps.");
							Player.SendMessage(p, "If you would like to delete one type /os map delete <1 or 2>");
						}
					}
					else
					{
						string mType;
						if (par2.ToUpper() == "" || par2.ToUpper() == "DESERT" || par2.ToUpper() == "FLAT" || par2.ToUpper() == "FOREST" || par2.ToUpper() == "ISLAND" || par2.ToUpper() == "MOUNTAINS" || par2.ToUpper() == "OCEAN" || par2.ToUpper() == "PIXEL" || par2.ToUpper() == "SPACE")
						{
							if (par2 != "")
							{
								mType = par2;
							}
							else
							{
								mType = "flat";
							}
							Player.SendMessage(p, "Creating your map, " + p.color + p.name);
							Command.all.Find("newlvl").Use(p, p.name.ToLower() + " " + mSize(p) + " " + mType);
						}
						else
						{
							Player.SendMessage(p, "A wrong map type was specified. Valid map types: Desert, flat, forest, island, mountians, ocean, pixel and space.");
						}
					}

				}
				else if (par == "PHYSICS")
				{
					if ((p.level.name.ToUpper().Equals(p.name.ToUpper())) || (p.level.name.ToUpper().Equals(p.name.ToUpper() + "00")) || (p.level.name.ToUpper().Equals(p.name.ToUpper() + "2")))
					{
						if (par2 != "")
						{
							if (par2 == "0")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 0");
							}
							else if (par2 == "1")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 1");
							}
							else if (par2 == "2")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 2");
							}
							else if (par2 == "3")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 3");
							}
							else if (par2 == "4")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 4");
							}
						}
						else { Player.SendMessage(p, "You didn't enter a number! Please enter one of these numbers: 0, 1, 2, 3, 4"); }
					}
					else { Player.SendMessage(p, "You have to be on one of your maps to set its physics!"); }
				}
				// Delete your map
				else if (par == "DELETE")
				{
					if (par2 == "")
					{
						Player.SendMessage(p, "To delete one of your maps type /os map delete <1 or 2> 1 is your first map 2 is your second.");
					}
					else if (par2 == "1")
					{
						Command.all.Find("deletelvl").Use(p, properMapName(p, false));
						Player.SendMessage(p, "Your 1st map has been removed.");
						return;
					}
					else if (par2 == "2")
					{
						Command.all.Find("deletelvl").Use(p, p.name.ToLower() + "2");
						Player.SendMessage(p, "Your 2nd map has been removed.");
						return;
					}

				}
				else
				{
					//all is good here :)
					Player.SendMessage(p, "/overseer map add [type - default is flat] -- Creates your map");
					Player.SendMessage(p, "/overseer map physics -- Sets the physics on your map.");
					Player.SendMessage(p, "/overseer map delete -- Deletes your map");
					Player.SendMessage(p, "  Map Types: Desert, flat, forest, island, mountians, ocean, pixel and space");
				}
			}
			else if (cmd == "ZONE")
			{
				// List zones on a single block(dont need to touch this :) )
				if (par == "LIST")
				{
					Command zone = Command.all.Find("zone");
					zone.Use(p, "");

				}
				// Add Zone to your personal map(took a while to get it to work(it was a big derp))
				else if (par == "ADD")
				{
					if ((p.level.name.ToUpper().Equals(p.name.ToUpper())) || (p.level.name.ToUpper().Equals(p.name.ToUpper() + "00")) || (p.level.name.ToUpper().Equals(p.name.ToUpper() + "2")))
					{
						if (par2 != "")
						{
							Command.all.Find("ozone").Use(p, par2);
							Player.SendMessage(p, par2 + " has been allowed building on your map.");
						}
						else
						{
							Player.SendMessage(p, "You did not specify a name to allow building on your map.");
						}
					}
					else { Player.SendMessage(p, "You must be on one of your maps to add or delete zones"); }
				}
				else if (par == "DEL")
				{
					if ((p.level.name.ToLower().Equals(p.name.ToUpper())) || (p.level.name.ToLower().Equals(p.name.ToLower() + "00")) || (p.level.name.ToLower().Equals(p.name.ToLower() + "2")))
					{
						// I need to add the ability to delete a single zone, I need help!
						if ((par2.ToUpper() == "ALL") || (par2.ToUpper() == ""))
						{
							Command zone = Command.all.Find("zone");
							Command click = Command.all.Find("click");
							zone.Use(p, "del all");
							click.Use(p, 0 + " " + 0 + " " + 0);
						}
					}
					else { Player.SendMessage(p, "You have to be on one of your maps to delete or add zones!"); }
				}
				else
				{
					// Unknown Zone Request
					Player.SendMessage(p, "/overseer ZONE add [playername or rank] -- Add a zone for a player or a rank."); ;
					Player.SendMessage(p, "/overseer ZONE del [all] -- Deletes all zones.");
					Player.SendMessage(p, "/overseer ZONE list -- show active zones on brick.");
					Player.SendMessage(p, "You can only delete all zones for now.");
				}
			}
			//Lets player load the level
			else if (cmd == "LOAD")
			{
				if (par != "")
				{
					if (par == "1")
					{
						Command.all.Find("load").Use(p, properMapName(p, false));
						Player.SendMessage(p, "Your level is now loaded.");
					}
					else if (par == "2")
					{
						Command.all.Find("load").Use(p, p.name + "2");
						Player.SendMessage(p, "Your 2nd level is now loaded.");
					}
				}
				else { Player.SendMessage(p, "Type /os load <1 or 2> to load one of your maps"); }
			}
			else if (cmd == "KICKALL")
			{
				Command.all.Find("kickall").Use(p, "");
			}
			else
			{
				Help(p);
			}
		}

		public override void Help(Player p)
		{
			// Remember to include or exclude the spoof command(s) -- MakeMeOp
			Player.SendMessage(p, "/overseer [command string] - sends command to The Overseer");
			Player.SendMessage(p, "Accepted Commands:");
			Player.SendMessage(p, "Go, map, spawn, zone, kickall, load");
			Player.SendMessage(p, "/os - Command shortcut.");
		}

		public string properMapName(Player p, bool Ext)
		{
			/* Returns the proper name of the User Level. By default the User Level will be named
			 * "UserName" but was earlier named "UserName00". Therefore the Script checks if the old
			 * map name exists before trying the new (and correct) name. All Operations will work with
			 * both map names (UserName and UserName00)
			 * I need to figure out how to add a system to do this with the players second map.
			 */
			string r = "";
			if (File.Exists(Directory.GetCurrentDirectory() + "\\levels\\" + p.name.ToLower() + "00.lvl"))
			{
				r = p.name.ToLower() + "00";
			}
			else
			{
				r = p.name.ToLower();
			}
			if (Ext == true) { r = r + ".lvl"; }
			return r;
		}

		public string mSize(Player p)
		{

			return "128 64 128";
		}
	}
}