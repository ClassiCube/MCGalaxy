/*
	Copyright 2011 MCForge
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands
{
	public sealed class CmdLavaSurvival : Command
	{
		public override string name { get { return "lavasurvival"; } }
		public override string shortcut { get { return "ls"; } }
		public override string type { get { return CommandTypes.Games; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public CmdLavaSurvival() { }

		public override void Use(Player p, string message)
		{
			if (String.IsNullOrEmpty(message)) { Help(p); return; }
			string[] s = message.ToLower().Split(' ');
			if (p == null && (s[0] == "go" || s[0] == "setup")) { Player.Message(p, "The \"" + s[0] + "\" command can only be used in-game!"); return; }

			if (s[0] == "go")
			{
				if (!Server.lava.active) { Player.Message(p, "There is no Lava Survival game right now."); return; }
				PlayerActions.ChangeMap(p, Server.lava.map.name);
				return;
			}
			if (s[0] == "info")
			{
				if (!Server.lava.active) { Player.Message(p, "There is no Lava Survival game right now."); return; }
				if (!Server.lava.roundActive) { Player.Message(p, "The round of Lava Survival hasn't started yet."); return; }
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
							Player.Message(p, "There is already an active Lava Survival game.");
							return;
						case 2:
							Player.Message(p, "You must have at least 3 configured maps to play Lava Survival.");
							return;
						case 3:
							Player.Message(p, "The specified map doesn't exist.");
							return;
						default:
							Player.Message(p, "An unknown error occurred.");
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
							Player.Message(p, "There isn't an active Lava Survival game.");
							return;
						default:
							Player.Message(p, "An unknown error occurred.");
							return;
					}
				}
				if (s[0] == "end")
				{
					if (!Server.lava.active) { Player.Message(p, "There isn't an active Lava Survival game."); return; }
					if (Server.lava.roundActive) Server.lava.EndRound();
					else if (Server.lava.voteActive) Server.lava.EndVote();
					else Player.Message(p, "There isn't an active round or vote to end.");
					return;
				}
			}
			if (p == null || p.group.Permission >= Server.lava.setupRank)
			{
				if (s[0] == "setup")
				{
					if (s.Length < 2) { SetupHelp(p); return; }
					if (Server.lava.active) { Player.Message(p, "You cannot configure Lava Survival while a game is active."); return; }
					if (s[1] == "map")
					{
						if (s.Length < 3) { SetupHelp(p, "map"); return; }
						Level foundLevel = LevelInfo.FindOrShowMatches(p, s[2]);
						if (foundLevel == null) { 
							return; 
						}
						else
						{
							if (foundLevel == Server.mainLevel) { Player.Message(p, "You cannot use the main map for Lava Survival."); return; }
							if (Server.lava.HasMap(foundLevel.name))
							{
								Server.lava.RemoveMap(foundLevel.name);
								foundLevel.motd = "ignore";
								foundLevel.overload = 1500;
								foundLevel.unload = true;
								foundLevel.loadOnGoto = true;
								Level.SaveSettings(foundLevel);
								Player.Message(p, "Map \"" + foundLevel.name + "\" has been removed.");
								return;
							}
							else
							{
								Server.lava.AddMap(foundLevel.name);

								LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(foundLevel.name);
								settings.blockFlood = new Vec3U16((ushort)(foundLevel.Width / 2), (ushort)(foundLevel.Height - 1), (ushort)(foundLevel.Length / 2));
								settings.blockLayer = new Vec3U16(0, (ushort)(foundLevel.Height / 2), 0);
								ushort x = (ushort)(foundLevel.Width / 2), y = (ushort)(foundLevel.Height / 2), z = (ushort)(foundLevel.Length / 2);
								settings.safeZone = new Vec3U16[] { new Vec3U16((ushort)(x - 3), y, (ushort)(z - 3)), new Vec3U16((ushort)(x + 3), (ushort)(y + 4), (ushort)(z + 3)) };
								Server.lava.SaveMapSettings(settings);

								foundLevel.motd = "Lava Survival: " + foundLevel.name.Capitalize();
								foundLevel.overload = 1000000;
								foundLevel.unload = false;
								foundLevel.loadOnGoto = false;
								Level.SaveSettings(foundLevel);
								Player.Message(p, "Map \"" + foundLevel.name + "\" has been added.");
								return;
							}
						}
					}
					if (s[1] == "block")
					{
						if (!Server.lava.HasMap(p.level.name)) { Player.Message(p, "Add the map before configuring it."); return; }
						if (s.Length < 3) { SetupHelp(p, "block"); return; }

						if (s[2] == "flood")
						{
							Player.Message(p, "Place or destroy the block you want to be the total flood block spawn point.");
							CatchPos cpos; cpos.mode = 0;
							cpos.x = 0; cpos.y = 0; cpos.z = 0;
							p.blockchangeObject = cpos;
							p.ClearBlockchange();
							p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
							return;
						}
						if (s[2] == "layer")
						{
							Player.Message(p, "Place or destroy the block you want to be the layer flood base spawn point.");
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
						Player.Message(p, "Place two blocks to determine the edges.");
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
							Player.Message(p, "Maps: &b" + Server.lava.Maps.Concatenate(", "));
							Player.Message(p, "Setup rank: " + Group.findPerm(Server.lava.setupRank).ColoredName);
							Player.Message(p, "Control rank: " + Group.findPerm(Server.lava.controlRank).ColoredName);
							Player.Message(p, "Start on server startup: " + (Server.lava.startOnStartup ? "&aON" : "&cOFF"));
							Player.Message(p, "Send AFK to main: " + (Server.lava.sendAfkMain ? "&aON" : "&cOFF"));
							Player.Message(p, "Vote count: &b" + Server.lava.voteCount);
							Player.Message(p, "Vote time: &b" + Server.lava.voteTime + " minute" + (Server.lava.voteTime == 1 ? "" : "s"));
							return;
						}

						try
						{
							switch (s[2])
							{
								case "sendafkmain":
									Server.lava.sendAfkMain = !Server.lava.sendAfkMain;
									Player.Message(p, "Send AFK to main: " + (Server.lava.sendAfkMain ? "&aON" : "&cOFF"));
									break;
								case "votecount":
									Server.lava.voteCount = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 2, 10);
									Player.Message(p, "Vote count: &b" + Server.lava.voteCount);
									break;
								case "votetime":
									Server.lava.voteTime = double.Parse(s[3]);
									Player.Message(p, "Vote time: &b" + Server.lava.voteTime + "minute" + (Server.lava.voteTime == 1 ? "" : "s"));
									break;
								default:
									SetupHelp(p, "settings");
									return;
							}
							Server.lava.SaveSettings();
							return;
						}
						catch { Player.Message(p, "INVALID INPUT"); return; }
					}
					if (s[1] == "mapsettings")
					{
						if (!Server.lava.HasMap(p.level.name)) { Player.Message(p, "Add the map before configuring it."); return; }
						LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
						if (s.Length < 4)
						{
							Player.Message(p, "Fast lava chance: &b" + settings.fast + "%");
							Player.Message(p, "Killer lava/water chance: &b" + settings.killer + "%");
							Player.Message(p, "Destroy blocks chance: &b" + settings.destroy + "%");
							Player.Message(p, "Water flood chance: &b" + settings.water + "%");
							Player.Message(p, "Layer flood chance: &b" + settings.layer + "%");
							Player.Message(p, "Layer height: &b" + settings.layerHeight + " block" + (settings.layerHeight == 1 ? "" : "s"));
							Player.Message(p, "Layer count: &b" + settings.layerCount);
							Player.Message(p, "Layer time: &b" + settings.layerInterval + " minute" + (settings.layerInterval == 1 ? "" : "s"));
							Player.Message(p, "Round time: &b" + settings.roundTime + " minute" + (settings.roundTime == 1 ? "" : "s"));
							Player.Message(p, "Flood time: &b" + settings.floodTime + " minute" + (settings.floodTime == 1 ? "" : "s"));
							Player.Message(p, "Flood position: &b" + settings.blockFlood.ToString(", "));
							Player.Message(p, "Layer position: &b" + settings.blockLayer.ToString(", "));
							Player.Message(p, "Safe zone: &b({0}) ({1})", settings.safeZone[0].ToString(", "), settings.safeZone[1].ToString(", "));
							return;
						}

						try
						{
							switch (s[2])
							{
								case "fast":
									settings.fast = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.Message(p, "Fast lava chance: &b" + settings.fast + "%");
									break;
								case "killer":
									settings.killer = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.Message(p, "Killer lava/water chance: &b" + settings.killer + "%");
									break;
								case "destroy":
									settings.destroy = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.Message(p, "Destroy blocks chance: &b" + settings.destroy + "%");
									break;
								case "water":
									settings.water = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.Message(p, "Water flood chance: &b" + settings.water + "%");
									break;
								case "layer":
									settings.layer = (byte)MathHelper.Clamp(decimal.Parse(s[3]), 0, 100);
									Player.Message(p, "Layer flood chance: &b" + settings.layer + "%");
									break;
								case "layerheight":
									settings.layerHeight = int.Parse(s[3]);
									Player.Message(p, "Layer height: &b" + settings.layerHeight + " block" + (settings.layerHeight == 1 ? "" : "s"));
									break;
								case "layercount":
									settings.layerCount = int.Parse(s[3]);
									Player.Message(p, "Layer count: &b" + settings.layerCount);
									break;
								case "layertime":
									settings.layerInterval = double.Parse(s[3]);
									Player.Message(p, "Layer time: &b" + settings.layerInterval + " minute" + (settings.layerInterval == 1 ? "" : "s"));
									break;
								case "roundtime":
									settings.roundTime = double.Parse(s[3]);
									Player.Message(p, "Round time: &b" + settings.roundTime + " minute" + (settings.roundTime == 1 ? "" : "s"));
									break;
								case "floodtime":
									settings.floodTime = double.Parse(s[3]);
									Player.Message(p, "Flood time: &b" + settings.floodTime + " minute" + (settings.floodTime == 1 ? "" : "s"));
									break;
								default:
									SetupHelp(p, "mapsettings");
									return;
							}
						}
						catch { Player.Message(p, "INVALID INPUT"); return; }
						Server.lava.SaveMapSettings(settings);
						return;
					}
				}
			}

			Help(p);
		}
		public override void Help(Player p)
		{
			Player.Message(p, "/lavasurvival <params> - Main command for Lava Survival.");
			Player.Message(p, "The following params are available:");
			Player.Message(p, "go - Join the fun!");
			Player.Message(p, "info - View the current round info and time.");
			if (p == null || p.group.Permission >= Server.lava.controlRank)
			{
				Player.Message(p, "start [map] - Start the Lava Survival game, optionally on the specified map.");
				Player.Message(p, "stop - Stop the current Lava Survival game.");
				Player.Message(p, "end - End the current round or vote.");
			}
			if (p == null || p.group.Permission >= Server.lava.setupRank)
			{
				Player.Message(p, "setup - Setup lava survival, use it for more info.");
			}
		}

		public void SetupHelp(Player p, string mode = "")
		{
			switch (mode)
			{
				case "map":
					Player.Message(p, "Add or remove maps in Lava Survival.");
					Player.Message(p, "<mapname> - Adds or removes <mapname>.");
					break;
				case "block":
					Player.Message(p, "View or set the block spawn positions.");
					Player.Message(p, "flood - Set the position for the total flood block.");
					Player.Message(p, "layer - Set the position for the layer flood base.");
					break;
				case "settings":
					Player.Message(p, "View or change the settings for Lava Survival.");
					Player.Message(p, "sendafkmain - Toggle sending AFK users to the main map when the map changes.");
					Player.Message(p, "votecount <2-10> - Set how many maps will be in the next map vote.");
					Player.Message(p, "votetime <time> - Set how long until the next map vote ends.");
					break;
				case "mapsettings":
					Player.Message(p, "View or change the settings for a Lava Survival map.");
					Player.Message(p, "fast <0-100> - Set the percent chance of fast lava.");
					Player.Message(p, "killer <0-100> - Set the percent chance of killer lava/water.");
					Player.Message(p, "destroy <0-100> - Set the percent chance of the lava/water destroying blocks.");
					Player.Message(p, "water <0-100> - Set the percent chance of a water instead of lava flood.");
					Player.Message(p, "layer <0-100> - Set the percent chance of the lava/water flooding in layers.");
					Player.Message(p, "layerheight <height> - Set the height of each layer.");
					Player.Message(p, "layercount <count> - Set the number of layers to flood.");
					Player.Message(p, "layertime <time> - Set the time interval for another layer to flood.");
					Player.Message(p, "roundtime <time> - Set how long until the round ends.");
					Player.Message(p, "floodtime <time> - Set how long until the map is flooded.");
					break;
				default:
					Player.Message(p, "Commands to setup Lava Survival.");
					Player.Message(p, "map <name> - Add or remove maps in Lava Survival.");
					Player.Message(p, "block <mode> - Set the block spawn positions.");
					Player.Message(p, "safezone - Set the safe zone, which is an area that can't be flooded.");
					Player.Message(p, "settings <setting> [value] - View or change the settings for Lava Survival.");
					Player.Message(p, "mapsettings <setting> [value] - View or change the settings for a Lava Survival map.");
					break;
			}
		}

		public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
		{
			RevertAndClearState(p, x, y, z);
			CatchPos cpos = (CatchPos)p.blockchangeObject;

			if (cpos.mode == 2)
			{
				cpos.x = x; cpos.y = y; cpos.z = z;
				p.blockchangeObject = cpos;
				p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
				return;
			}

			LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
			if (cpos.mode == 0) settings.blockFlood = new Vec3U16(x, y, z);
			if (cpos.mode == 1) settings.blockLayer = new Vec3U16(x, y, z);
			Server.lava.SaveMapSettings(settings);

			Player.Message(p, "Position set! &b({0}, {1}, {2})", x, y, z);
		}

		public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
		{
			RevertAndClearState(p, x, y, z);
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
				settings.safeZone = new Vec3U16[] { new Vec3U16(sx, sy, sz), new Vec3U16(ex, ey, ez) };
				Server.lava.SaveMapSettings(settings);

				Player.Message(p, "Safe zone set! &b({0}, {1}, {2}) ({3}, {4}, {5})", sx, sy, sz, ex, ey, ez);
			}
		}


		struct CatchPos { public ushort x, y, z; public byte mode; }
	}
}
