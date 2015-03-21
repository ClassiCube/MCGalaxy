/*
	Copyright 2011 MCGalaxy
	Created by Techjar (Jordan S.)
		
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
namespace MCGalaxy.Commands
{
	public sealed class CmdLavaSurvival : Command
	{
		public override string name { get { return "lavasurvival"; } }
		public override string shortcut { get { return "ls"; } }
		public override string type { get { return "game"; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public CmdLavaSurvival() { }

		public override void Use(Player p, string message)
		{
			if (String.IsNullOrEmpty(message)) { Help(p); return; }
			string[] s = message.ToLower().Split(' ');
			if (p == null && (s[0] == "go" || s[0] == "setup")) { Player.SendMessage(p, "The \"" + s[0] + "\" command can only be used in-game!"); return; }

			if (s[0] == "go")
			{
				if (!Server.lava.active) { Player.SendMessage(p, "There is no Lava Survival game right now."); return; }
				Command.all.Find("goto").Use(p, Server.lava.map.name);
				return;
			}
			if (s[0] == "info")
			{
				if (!Server.lava.active) { Player.SendMessage(p, "There is no Lava Survival game right now."); return; }
				if (!Server.lava.roundActive) { Player.SendMessage(p, "The round of Lava Survival hasn't started yet."); return; }
				Server.lava.AnnounceRoundInfo(p, p == null);
				Server.lava.AnnounceTimeLeft(!Server.lava.flooded, true, p, p == null);
				return;
			}
			if (p == null || p.group.Permission >= Server.lava.controlRank)
			{
				if (s[0] == "start")
				{
					switch (Server.lava.Start(s.Length > 1 ? s[1] : ""))
					{
						case 0:
							Player.GlobalMessage("Lava Survival has started! Join the fun with /ls go");
							return;
						case 1:
							Player.SendMessage(p, "There is already an active Lava Survival game.");
							return;
						case 2:
							Player.SendMessage(p, "You must have at least 3 configured maps to play Lava Survival.");
							return;
						case 3:
							Player.SendMessage(p, "The specified map doesn't exist.");
							return;
						default:
							Player.SendMessage(p, "An unknown error occurred.");
							return;
					}
				}
				if (s[0] == "stop")
				{
					switch (Server.lava.Stop())
					{
						case 0:
							Player.GlobalMessage("Lava Survival has ended! We hope you had fun!");
							return;
						case 1:
							Player.SendMessage(p, "There isn't an active Lava Survival game.");
							return;
						default:
							Player.SendMessage(p, "An unknown error occurred.");
							return;
					}
				}
				if (s[0] == "end")
				{
					if (!Server.lava.active) { Player.SendMessage(p, "There isn't an active Lava Survival game."); return; }
					if (Server.lava.roundActive) Server.lava.EndRound();
					else if (Server.lava.voteActive) Server.lava.EndVote();
					else Player.SendMessage(p, "There isn't an active round or vote to end.");
					return;
				}
			}
			if (p == null || p.group.Permission >= Server.lava.setupRank)
			{
				if (s[0] == "setup")
				{
					if (s.Length < 2) { SetupHelp(p); return; }
					if (Server.lava.active) { Player.SendMessage(p, "You cannot configure Lava Survival while a game is active."); return; }
					if (s[1] == "map")
					{
						if (s.Length < 3) { SetupHelp(p, "map"); return; }
						Level foundLevel = Level.Find(s[2]);
						if (foundLevel == null)
						{
							Player.SendMessage(p, "The level must be loaded to add/remove it.");
							return;
						}
						else
						{
							if (foundLevel == Server.mainLevel) { Player.SendMessage(p, "You cannot use the main map for Lava Survival."); return; }
							if (Server.lava.HasMap(foundLevel.name))
							{
								Server.lava.RemoveMap(foundLevel.name);
								foundLevel.motd = "ignore";
								foundLevel.overload = 1500;
								foundLevel.unload = true;
								foundLevel.loadOnGoto = true;
								Level.SaveSettings(foundLevel);
								Player.SendMessage(p, "Map \"" + foundLevel.name + "\" has been removed.");
								return;
							}
							else
							{
								Server.lava.AddMap(foundLevel.name);

								LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(foundLevel.name);
								settings.blockFlood = new LavaSurvival.Pos((ushort)(foundLevel.width / 2), (ushort)(foundLevel.depth - 1), (ushort)(foundLevel.height / 2));
								settings.blockLayer = new LavaSurvival.Pos(0, (ushort)(foundLevel.depth / 2), 0);
								ushort x = (ushort)(foundLevel.width / 2), y = (ushort)(foundLevel.depth / 2), z = (ushort)(foundLevel.height / 2);
								settings.safeZone = new LavaSurvival.Pos[] { new LavaSurvival.Pos((ushort)(x - 3), y, (ushort)(z - 3)), new LavaSurvival.Pos((ushort)(x + 3), (ushort)(y + 4), (ushort)(z + 3)) };
								Server.lava.SaveMapSettings(settings);

								foundLevel.motd = "Lava Survival: " + foundLevel.name.Capitalize();
								foundLevel.overload = 1000000;
								foundLevel.unload = false;
								foundLevel.loadOnGoto = false;
								Level.SaveSettings(foundLevel);
								Player.SendMessage(p, "Map \"" + foundLevel.name + "\" has been added.");
								return;
							}
						}
					}
					if (s[1] == "block")
					{
						if (!Server.lava.HasMap(p.level.name)) { Player.SendMessage(p, "Add the map before configuring it."); return; }
						if (s.Length < 3) { SetupHelp(p, "block"); return; }

						if (s[2] == "flood")
						{
							Player.SendMessage(p, "Place or destroy the block you want to be the total flood block spawn point.");
							CatchPos cpos; cpos.mode = 0;
							cpos.x = 0; cpos.y = 0; cpos.z = 0;
							p.blockchangeObject = cpos;
							p.ClearBlockchange();
							p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
							return;
						}
						if (s[2] == "layer")
						{
							Player.SendMessage(p, "Place or destroy the block you want to be the layer flood base spawn point.");
							CatchPos cpos; cpos.mode = 1;
							cpos.x = 0; cpos.y = 0; cpos.z = 0;
							p.blockchangeObject = cpos;
							p.ClearBlockchange();
							p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
							return;
						}

						SetupHelp(p, "block");
						return;
					}
					if (s[1] == "safezone" || s[1] == "safe")
					{
						Player.SendMessage(p, "Place two blocks to determine the edges.");
						CatchPos cpos; cpos.mode = 2;
						cpos.x = 0; cpos.y = 0; cpos.z = 0;
						p.blockchangeObject = cpos;
						p.ClearBlockchange();
						p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
						return;
					}
					if (s[1] == "settings")
					{
						if (s.Length < 3)
						{
							Player.SendMessage(p, "Maps: &b" + Server.lava.Maps.Concatenate(", "));
							Player.SendMessage(p, "Setup rank: " + Group.findPerm(Server.lava.setupRank).color + Group.findPerm(Server.lava.setupRank).trueName);
							Player.SendMessage(p, "Control rank: " + Group.findPerm(Server.lava.controlRank).color + Group.findPerm(Server.lava.controlRank).trueName);
							Player.SendMessage(p, "Start on server startup: " + (Server.lava.startOnStartup ? "&aON" : "&cOFF"));
							Player.SendMessage(p, "Send AFK to main: " + (Server.lava.sendAfkMain ? "&aON" : "&cOFF"));
							Player.SendMessage(p, "Vote count: &b" + Server.lava.voteCount);
							Player.SendMessage(p, "Vote time: &b" + Server.lava.voteTime + " minute" + (Server.lava.voteTime == 1 ? "" : "s"));
							return;
						}

						try
						{
							switch (s[2])
							{
								case "sendafkmain":
									Server.lava.sendAfkMain = !Server.lava.sendAfkMain;
									Player.SendMessage(p, "Send AFK to main: " + (Server.lava.sendAfkMain ? "&aON" : "&cOFF"));
									break;
								case "votecount":
									Server.lava.voteCount = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 2, 10);
									Player.SendMessage(p, "Vote count: &b" + Server.lava.voteCount);
									break;
								case "votetime":
									Server.lava.voteTime = double.Parse(s[3]);
									Player.SendMessage(p, "Vote time: &b" + Server.lava.voteTime + "minute" + (Server.lava.voteTime == 1 ? "" : "s"));
									break;
								default:
									SetupHelp(p, "settings");
									return;
							}
							Server.lava.SaveSettings();
							return;
						}
						catch { Player.SendMessage(p, "INVALID INPUT"); return; }
					}
					if (s[1] == "mapsettings")
					{
						if (!Server.lava.HasMap(p.level.name)) { Player.SendMessage(p, "Add the map before configuring it."); return; }
						LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
						if (s.Length < 4)
						{
							Player.SendMessage(p, "Fast lava chance: &b" + settings.fast + "%");
							Player.SendMessage(p, "Killer lava/water chance: &b" + settings.killer + "%");
							Player.SendMessage(p, "Destroy blocks chance: &b" + settings.destroy + "%");
							Player.SendMessage(p, "Water flood chance: &b" + settings.water + "%");
							Player.SendMessage(p, "Layer flood chance: &b" + settings.layer + "%");
							Player.SendMessage(p, "Layer height: &b" + settings.layerHeight + " block" + (settings.layerHeight == 1 ? "" : "s"));
							Player.SendMessage(p, "Layer count: &b" + settings.layerCount);
							Player.SendMessage(p, "Layer time: &b" + settings.layerInterval + " minute" + (settings.layerInterval == 1 ? "" : "s"));
							Player.SendMessage(p, "Round time: &b" + settings.roundTime + " minute" + (settings.roundTime == 1 ? "" : "s"));
							Player.SendMessage(p, "Flood time: &b" + settings.floodTime + " minute" + (settings.floodTime == 1 ? "" : "s"));
							Player.SendMessage(p, "Flood position: &b" + settings.blockFlood.ToString(", "));
							Player.SendMessage(p, "Layer position: &b" + settings.blockLayer.ToString(", "));
							Player.SendMessage(p, String.Format("Safe zone: &b({0}) ({1})", settings.safeZone[0].ToString(", "), settings.safeZone[1].ToString(", ")));
							return;
						}

						try
						{
							switch (s[2])
							{
								case "fast":
									settings.fast = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.SendMessage(p, "Fast lava chance: &b" + settings.fast + "%");
									break;
								case "killer":
									settings.killer = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.SendMessage(p, "Killer lava/water chance: &b" + settings.killer + "%");
									break;
								case "destroy":
									settings.destroy = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.SendMessage(p, "Destroy blocks chance: &b" + settings.destroy + "%");
									break;
								case "water":
									settings.water = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.SendMessage(p, "Water flood chance: &b" + settings.water + "%");
									break;
								case "layer":
									settings.layer = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.SendMessage(p, "Layer flood chance: &b" + settings.layer + "%");
									break;
								case "layerheight":
									settings.layerHeight = int.Parse(s[3]);
									Player.SendMessage(p, "Layer height: &b" + settings.layerHeight + " block" + (settings.layerHeight == 1 ? "" : "s"));
									break;
								case "layercount":
									settings.layerCount = int.Parse(s[3]);
									Player.SendMessage(p, "Layer count: &b" + settings.layerCount);
									break;
								case "layertime":
									settings.layerInterval = double.Parse(s[3]);
									Player.SendMessage(p, "Layer time: &b" + settings.layerInterval + " minute" + (settings.layerInterval == 1 ? "" : "s"));
									break;
								case "roundtime":
									settings.roundTime = double.Parse(s[3]);
									Player.SendMessage(p, "Round time: &b" + settings.roundTime + " minute" + (settings.roundTime == 1 ? "" : "s"));
									break;
								case "floodtime":
									settings.floodTime = double.Parse(s[3]);
									Player.SendMessage(p, "Flood time: &b" + settings.floodTime + " minute" + (settings.floodTime == 1 ? "" : "s"));
									break;
								default:
									SetupHelp(p, "mapsettings");
									return;
							}
						}
						catch { Player.SendMessage(p, "INVALID INPUT"); return; }
						Server.lava.SaveMapSettings(settings);
						return;
					}
				}
			}

			Help(p);
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/lavasurvival <params> - Main command for Lava Survival.");
			Player.SendMessage(p, "The following params are available:");
			Player.SendMessage(p, "go - Join the fun!");
			Player.SendMessage(p, "info - View the current round info and time.");
			if (p == null || p.group.Permission >= Server.lava.controlRank)
			{
				Player.SendMessage(p, "start [map] - Start the Lava Survival game, optionally on the specified map.");
				Player.SendMessage(p, "stop - Stop the current Lava Survival game.");
				Player.SendMessage(p, "end - End the current round or vote.");
			}
			if (p == null || p.group.Permission >= Server.lava.setupRank)
			{
				Player.SendMessage(p, "setup - Setup lava survival, use it for more info.");
			}
		}

		public void SetupHelp(Player p, string mode = "")
		{
			switch (mode)
			{
				case "map":
					Player.SendMessage(p, "Add or remove maps in Lava Survival.");
					Player.SendMessage(p, "<mapname> - Adds or removes <mapname>.");
					break;
				case "block":
					Player.SendMessage(p, "View or set the block spawn positions.");
					Player.SendMessage(p, "flood - Set the position for the total flood block.");
					Player.SendMessage(p, "layer - Set the position for the layer flood base.");
					break;
				case "settings":
					Player.SendMessage(p, "View or change the settings for Lava Survival.");
					Player.SendMessage(p, "sendafkmain - Toggle sending AFK users to the main map when the map changes.");
					Player.SendMessage(p, "votecount <2-10> - Set how many maps will be in the next map vote.");
					Player.SendMessage(p, "votetime <time> - Set how long until the next map vote ends.");
					break;
				case "mapsettings":
					Player.SendMessage(p, "View or change the settings for a Lava Survival map.");
					Player.SendMessage(p, "fast <0-100> - Set the percent chance of fast lava.");
					Player.SendMessage(p, "killer <0-100> - Set the percent chance of killer lava/water.");
					Player.SendMessage(p, "destroy <0-100> - Set the percent chance of the lava/water destroying blocks.");
					Player.SendMessage(p, "water <0-100> - Set the percent chance of a water instead of lava flood.");
					Player.SendMessage(p, "layer <0-100> - Set the percent chance of the lava/water flooding in layers.");
					Player.SendMessage(p, "layerheight <height> - Set the height of each layer.");
					Player.SendMessage(p, "layercount <count> - Set the number of layers to flood.");
					Player.SendMessage(p, "layertime <time> - Set the time interval for another layer to flood.");
					Player.SendMessage(p, "roundtime <time> - Set how long until the round ends.");
					Player.SendMessage(p, "floodtime <time> - Set how long until the map is flooded.");
					break;
				default:
					Player.SendMessage(p, "Commands to setup Lava Survival.");
					Player.SendMessage(p, "map <name> - Add or remove maps in Lava Survival.");
					Player.SendMessage(p, "block <mode> - Set the block spawn positions.");
					Player.SendMessage(p, "safezone - Set the safe zone, which is an area that can't be flooded.");
					Player.SendMessage(p, "settings <setting> [value] - View or change the settings for Lava Survival.");
					Player.SendMessage(p, "mapsettings <setting> [value] - View or change the settings for a Lava Survival map.");
					break;
			}
		}

		public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
		{
			p.ClearBlockchange();
			p.SendBlockchange(x, y, z, p.level.GetTile(x, y, z));
			CatchPos cpos = (CatchPos)p.blockchangeObject;

			if (cpos.mode == 2)
			{
				cpos.x = x; cpos.y = y; cpos.z = z;
				p.blockchangeObject = cpos;
				p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
				return;
			}

			LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
			if (cpos.mode == 0) settings.blockFlood = new LavaSurvival.Pos(x, y, z);
			if (cpos.mode == 1) settings.blockLayer = new LavaSurvival.Pos(x, y, z);
			Server.lava.SaveMapSettings(settings);

			Player.SendMessage(p, String.Format("Position set! &b({0}, {1}, {2})", x, y, z));
		}

		public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
		{
			p.ClearBlockchange();
			p.SendBlockchange(x, y, z, p.level.GetTile(x, y, z));
			CatchPos cpos = (CatchPos)p.blockchangeObject;

			if (cpos.mode == 2)
			{
				ushort sx = Math.Min(cpos.x, x);
				ushort ex = Math.Max(cpos.x, x);
				ushort sy = Math.Min(cpos.y, y);
				ushort ey = Math.Max(cpos.y, y);
				ushort sz = Math.Min(cpos.z, z);
				ushort ez = Math.Max(cpos.z, z);

				LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
				settings.safeZone = new LavaSurvival.Pos[] { new LavaSurvival.Pos(sx, sy, sz), new LavaSurvival.Pos(ex, ey, ez) };
				Server.lava.SaveMapSettings(settings);

				Player.SendMessage(p, String.Format("Safe zone set! &b({0}, {1}, {2}) ({3}, {4}, {5})", sx, sy, sz, ex, ey, ez));
			}
		}


		struct CatchPos { public ushort x, y, z; public byte mode; }
	}
}
