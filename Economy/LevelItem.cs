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
using MCGalaxy.Generator;

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
        
        public override void Parse(string line, string[] args) {
            if (!args[1].CaselessEq("levels")) return;
            
            LevelPreset preset = FindPreset(args[2]);
            if (preset == null) {
                preset = new LevelPreset();
                Presets.Add(preset);
            }
            
            switch (args[3]) {
                case "name": preset.name = args[4]; break;
                case "price": preset.price = int.Parse(args[4]); break;
                case "x": preset.x = args[4]; break;
                case "y": preset.y = args[4]; break;
                case "z": preset.z = args[4]; break;
                case "type": preset.type = args[4]; break;
            }
        }
        
        public override void Serialise(StreamWriter writer) {
            writer.WriteLine("level:enabled:" + Enabled);
            writer.WriteLine("level:purchaserank:" + (int)PurchaseRank);
            
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
            if (preset == null) { Player.Message(p, "%cThat isn't a level preset"); return; }
            
            if (p.money < preset.price) {
                Player.Message(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy that map"); return;
            }
            string name = p.name + "_" + args[2];
            
            try {
                Command.all.Find("newlvl").Use(null, name + " " + preset.x + " " + preset.y + " " + preset.z + " " + preset.type);
                Player.Message(p, "%aCreating level: '%f" + name + "%a' . . .");
                
                Command.all.Find("load").Use(null, name);
                Thread.Sleep(250);
                
                Level level = LevelInfo.Find(name);
                if (level.permissionbuild > p.Rank) { level.permissionbuild = p.Rank; }
                if (level.permissionvisit > p.Rank) { level.permissionvisit = p.Rank; }
                PlayerActions.ChangeMap(p, name);

                Player.Message(p, "%aSuccessfully created your map: '%f" + name + "%a'");
                try {
                    Level.Zone zn = default(Level.Zone);
                    zn.bigX = (ushort)(level.Width - 1);
                    zn.bigY = (ushort)(level.Height - 1);
                    zn.bigZ = (ushort)(level.Length - 1);
                    zn.Owner = p.name;
                    level.ZoneList.Add(zn);
                    LevelDB.CreateZone(level.name, zn);
                    Player.Message(p, "%aZoning Succesful");
                } catch { Player.Message(p, "%cZoning Failed"); }
            } catch {
                Player.Message(p, "%cSomething went wrong, Money untouched"); return;
            }
            Economy.MakePurchase(p, preset.price, "%3Map: %f" + preset.name);
        }
        
        protected internal override void OnSetupCommandOther(Player p, string[] args) {
            LevelPreset preset = FindPreset(args[2]);
            switch (args[1].ToLower()) {
                case "new":
                case "create":
                case "add":
                    if (preset != null) { Player.Message(p, "%cThat preset level already exists"); return; }
                    
                    preset = new LevelPreset();
                    preset.name = args[2];
                    if (OkayAxis(args[3]) && OkayAxis(args[4]) && OkayAxis(args[5])) {
                        preset.x = args[3]; preset.y = args[4]; preset.z = args[5];
                    } else { Player.Message(p, "%cDimension must be a power of 2"); break; }
                    
                    if (!MapGen.IsRecognisedTheme(args[6])) {
                        MapGen.PrintThemes(p); return;
                    }                    
                    preset.type = args[6].ToLower();
                    if (!int.TryParse(args[7], out preset.price)) {
                        Player.Message(p, "\"" + args[9] + "\" is not a valid integer."); return;
                    }

                    Presets.Add(preset);
                    Player.Message(p, "%aSuccessfully added the following map preset:");
                    Player.Message(p, "Name: %f" + preset.name);
                    Player.Message(p, "x:" + preset.x + ", y:" + preset.y + ", z:" + preset.z);
                    Player.Message(p, "Map Type: %f" + preset.type);
                    Player.Message(p, "Map Price: %f" + preset.price + " %3" + Server.moneys);
                    break;

                case "delete":
                case "remove":
                    if (preset == null) { Player.Message(p, "%cThat preset level doesn't exist"); return; }
                    Presets.Remove(preset);
                    Player.Message(p, "%aSuccessfully removed preset: %f" + preset.name);
                    break;

                case "edit":
                case "change":
                    if (preset == null) { Player.Message(p, "%cThat preset level doesn't exist"); return; }
                    
                    switch (args[3]) {
                        case "name":
                        case "title":
                            preset.name = args[4];
                            Player.Message(p, "%aSuccessfully changed preset name to %f" + preset.name);
                            break;

                        case "x":
                            if (OkayAxis(args[4])) {
                                preset.x = args[4];
                                Player.Message(p, "%aSuccessfully changed preset x size to %f" + preset.x);
                            } else { Player.Message(p, "%cDimension was wrong, it must be a power of 2"); break; }
                            break;

                        case "y":
                            if (OkayAxis(args[4])) {
                                preset.y = args[4];
                                Player.Message(p, "%aSuccessfully changed preset y size to %f" + preset.y);
                            } else { Player.Message(p, "%cDimension was wrong, it must be a power of 2"); break; }
                            break;

                        case "z":
                            if (OkayAxis(args[4])) {
                                preset.z = args[4];
                                Player.Message(p, "%aSuccessfully changed preset z size to %f" + preset.z);
                            } else { Player.Message(p, "%cDimension was wrong, it must be a power of 2"); break; }
                            break;

                        case "type":
                            if (MapGen.IsRecognisedTheme(args[4])) {
                                preset.type = args[4].ToLower();
                            } else {
                                MapGen.PrintThemes(p); return;
                            }
                            Player.Message(p, "%aSuccessfully changed preset type to %f" + preset.type);
                            break;

                        case "price":
                            int newPrice = 0;
                            if (!int.TryParse(args[4], out newPrice)) {
                                Player.Message(p, "\"" + args[4] + "\" is not a valid integer."); return;
                            }
                            if (newPrice < 0) {
                                Player.Message(p, "%cAmount of %3" + Server.moneys + "%c cannot be negative"); return;
                            }
                            preset.price = newPrice;
                            Player.Message(p, "%aSuccessfully changed preset price to %f" + preset.price + " %3" + Server.moneys);
                            break;

                        default:
                            Player.Message(p, "Supported properties to edit: name, title, x, y, z, type, price");
                            break;
                    }
                    break;

                default:
                    OnSetupCommandHelp(p); break;
            }
        }
        
        protected internal override void OnSetupCommandHelp(Player p) {
            base.OnSetupCommandHelp(p);
            Player.Message(p, "%T/eco level add [name] [x] [y] [z] [type] [price]");
            Player.Message(p, "%T/eco level remove [name]");
            Player.Message(p, "%T/eco level edit [name] [name/x/y/z/type/price] [value]");
            Player.Message(p, "%HAdds, removes, or edits a level preset.");
        }
        
        protected internal override void OnStoreOverview(Player p) {
        	Player.Message(p, "Maps - see /store maps");
        }
        
        protected internal override void OnStoreCommand(Player p) {
            Player.Message(p, "%aAvailable maps to buy:");
            if (Presets.Count == 0) {
                Player.Message(p, "%8-None-");
            } else {
                foreach (LevelPreset preset in Presets) {
                    Player.Message(p, preset.name + " (" + preset.x + "," + preset.y + "," + preset.z + ") " + 
                                       preset.type + ": %f" + preset.price + " %3" + Server.moneys);
                }
            }
        }
        
        public LevelPreset FindPreset(string name) {
            foreach (LevelPreset preset in Presets) {
                if (preset.name != null && preset.name.CaselessEq(name))
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
