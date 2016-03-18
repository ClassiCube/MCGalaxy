/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Threading;
using MCGalaxy.SQL;

namespace MCGalaxy.Eco {
	
	public sealed class LevelItem : Item {
		
		public LevelItem() {
			Aliases = new [] { "level", "levels", "map", "maps" };
		}
		
		public override string Name { get { return "Level"; } }
		
		public List<LevelPreset> Presets = new List<LevelPreset>();
		public class LevelPreset {
			public int price;
			public string name;
			public string x, y, z;
			public string type;
		}
		
		public override void Parse(string line, string[] split) {
			if (split[1] == "enabled") {
				Enabled = split[2].CaselessEquals("true");
			} else if (split[1] == "levels") {
				LevelPreset preset = FindPreset(split[2]);
				if (preset == null) {
					preset = new LevelPreset();
					Presets.Add(preset);
				}
				
				switch (split[3]) {
					case "name":
						preset.name = split[4]; break;
					case "price":
						preset.price = int.Parse(split[4]); break;
					case "x":
						preset.x = split[4]; break;
					case "y":
						preset.y = split[4]; break;
					case "z":
						preset.z = split[4]; break;
					case "type":
						preset.type = split[4]; break;
				}
			}
		}
		
		public override void Serialise(StreamWriter writer) {
			writer.WriteLine("level:enabled:" + Enabled);
			foreach (LevelPreset preset in Presets) {
				writer.WriteLine();
				writer.WriteLine("level:levels:" + preset.name + ":name:" + preset.name);
				writer.WriteLine("level:levels:" + preset.name + ":price:" + preset.price);
				writer.WriteLine("level:levels:" + preset.name + ":x:" + preset.x);
				writer.WriteLine("level:levels:" + preset.name + ":y:" + preset.y);
				writer.WriteLine("level:levels:" + preset.name + ":z:" + preset.z);
				writer.WriteLine("level:levels:" + preset.name + ":type:" + preset.type);
			}
		}
		
		protected internal override void OnBuyCommand(Command cmd, Player p, 
		                                              string message, string[] args) {
			if (args.Length < 3) { cmd.Help(p); return; }
			LevelPreset preset = FindPreset(args[1]);
			if (preset == null) { Player.SendMessage(p, "%cThat isn't a level preset"); return; }
			
			if (!p.EnoughMoney(preset.price)) {
				Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy that map"); return;
			}
			string name = p.name + "_" + args[2];
			
			try {
				Command.all.Find("newlvl").Use(null, name + " " + preset.x + " " + preset.y + " " + preset.z + " " + preset.type);
				Player.SendMessage(p, "%aCreating level: '%f" + name + "%a' . . .");
				
				Command.all.Find("load").Use(null, name);
				Thread.Sleep(250);
				
				Level level = LevelInfo.Find(name);
				if (level.permissionbuild > p.group.Permission) { level.permissionbuild = p.group.Permission; }
				if (level.permissionvisit > p.group.Permission) { level.permissionvisit = p.group.Permission; }
				Command.all.Find("goto").Use(p, name);

				Player.SendMessage(p, "%aSuccessfully created your map: '%f" + name + "%a'");
				try {
					//safe against SQL injections, but will be replaced soon by a new feature
					Database.executeQuery("INSERT INTO `Zone" + level.name + "` (SmallX, SmallY, SmallZ, BigX, BigY, BigZ, Owner) parts[1]S " +
					                      "(0,0,0," + (level.Width - 1) + "," + (level.Height - 1) + "," + (level.Length - 1) + ",'" + p.name + "')");
					Player.SendMessage(p, "%aZoning Succesful");
				} catch { Player.SendMessage(p, "%cZoning Failed"); }
			} catch {
				Player.SendMessage(p, "%cSomething went wrong, Money untouchred"); return;
			}
			MakePurchase(p, preset.price, "%3Map: %f" + preset.name);
		}
		
		protected internal override void OnSetupCommand(Player p, string[] args) {
			LevelPreset preset = FindPreset(args[3]);
			switch (args[2]) {
				case "new":
				case "create":
				case "add":
					if (preset != null) { Player.SendMessage(p, "%cThat preset level already exists"); return; }
					
					preset = new LevelPreset();
					preset.name = args[3];
					if (OkayAxis(args[4]) && OkayAxis(args[5]) && OkayAxis(args[6])) {
						preset.x = args[4]; preset.y = args[5]; preset.z = args[6];
					} else { Player.SendMessage(p, "%cDimension must be a power of 2"); break; }
					
					if (!MapGen.IsRecognisedFormat(args[7])) {
						MapGen.PrintValidFormats(p); return;
					}					
					preset.type = args[7].ToLower();
                    if (!int.TryParse(args[8], out preset.price)) {
                        Player.SendMessage(p, "\"" + args[9] + "\" is not a valid integer."); return;
                    }

					Presets.Add(preset);
					Player.SendMessage(p, "%aSuccessfully added the following map preset:");
					Player.SendMessage(p, "Name: %f" + preset.name);
					Player.SendMessage(p, "x:" + preset.x + ", y:" + preset.y + ", z:" + preset.z);
					Player.SendMessage(p, "Map Type: %f" + preset.type);
					Player.SendMessage(p, "Map Price: %f" + preset.price + " %3" + Server.moneys);
					break;

				case "delete":
				case "remove":
					if (preset == null) { Player.SendMessage(p, "%cThat preset level doesn't exist"); return; }
					Presets.Remove(preset);
					Player.SendMessage(p, "%aSuccessfully removed preset: %f" + preset.name);
					break;

				case "edit":
				case "change":
					if (preset == null) { Player.SendMessage(p, "%cThat preset level doesn't exist"); return; }
					
					switch (args[4]) {
						case "name":
						case "title":
							preset.name = args[5];
							Player.SendMessage(p, "%aSuccessfully changed preset name to %f" + preset.name);
							break;

						case "x":
							if (OkayAxis(args[5])) {
								preset.x = args[5];
								Player.SendMessage(p, "%aSuccessfully changed preset x size to %f" + preset.x);
							} else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
							break;

						case "y":
							if (OkayAxis(args[5])) {
								preset.y = args[5];
								Player.SendMessage(p, "%aSuccessfully changed preset y size to %f" + preset.y);
							} else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
							break;

						case "z":
							if (OkayAxis(args[5])) {
								preset.z = args[5];
								Player.SendMessage(p, "%aSuccessfully changed preset z size to %f" + preset.z);
							} else { Player.SendMessage(p, "%cDimension was wrong, it must be a power of 2"); break; }
							break;

						case "type":
							if (MapGen.IsRecognisedFormat(args[5])) {
								preset.type = args[5].ToLower();
							} else {
								MapGen.PrintValidFormats(p); return;
							}
							Player.SendMessage(p, "%aSuccessfully changed preset type to %f" + preset.type);
							break;

						case "price":
							int newPrice = 0;
							if (!int.TryParse(args[5], out newPrice)) {
								Player.SendMessage(p, "\"" + args[5] + "\" is not a valid integer."); return;
							}
							if (newPrice < 0) {
								Player.SendMessage(p, "%cAmount of %3" + Server.moneys + "%c cannot be negative"); return;
							}
							preset.price = newPrice;
							Player.SendMessage(p, "%aSuccessfully changed preset price to %f" + preset.price + " %3" + Server.moneys);
							break;

						default:
							Player.SendMessage(p, "%cThat wasn't a valid command addition!");
							break;
					}
					break;

				case "enable":
					Player.SendMessage(p, "%aMaps are now enabled for the economy system");
					Enabled = true; break;

				case "disable":
					Player.SendMessage(p, "%aMaps are now disabled for the economy system");
					Enabled = false; break;

				default:
					Player.SendMessage(p, "%cThat wasn't a valid command addition!");
					break;
			}
		}
		
		protected internal override void OnStoreCommand(Player p) {
			Player.SendMessage(p, "%aAvailable maps to buy:");
			if (Presets.Count == 0) {
				Player.SendMessage(p, "%8-None-");
			} else {
				foreach (LevelPreset preset in Presets) {
					Player.SendMessage(p, preset.name + " (" + preset.x + "," + preset.y + "," + preset.z + ") " + 
					                   preset.type + ": %f" + preset.price + " %3" + Server.moneys);
				}
			}
		}
		
		public LevelPreset FindPreset(string name) {
			foreach (LevelPreset preset in Presets) {
				if (preset.name != null && preset.name.CaselessEquals(name))
					return preset;
			}
			return null;
		}
		
		static bool OkayAxis(string value) {
			ushort length;
			if (!ushort.TryParse(value, out length)) return false;
			return MapGen.OkayAxis(length);
		}
	}
}
