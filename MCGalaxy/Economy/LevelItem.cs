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
using MCGalaxy.Commands.World;
using MCGalaxy.Generator;

namespace MCGalaxy.Eco {
    
    public sealed class LevelItem : Item {
        
        public LevelItem() {
            Aliases = new string[] { "level", "levels", "map", "maps" };
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
        
        protected internal override void OnBuyCommand(Player p, string message, string[] args) {
            if (args.Length < 3) { OnStoreCommand(p); return; }
            LevelPreset preset = FindPreset(args[1]);
            if (preset == null) { Player.Message(p, "%cThat isn't a level preset"); return; }
            
            if (p.money < preset.price) {
                Player.Message(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy that map"); return;
            }
            string name = p.name + "_" + args[2];
            
            try {
                Command.all.Find("newlvl").Use(null, name + " " + preset.x + " " + preset.y + " " + preset.z + " " + preset.type);
                Player.Message(p, "%aCreating level: '%f" + name + "%a' . . .");
                
                CmdLoad.LoadLevel(null, name);
                Level level = LevelInfo.FindExact(name);
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
        
        protected internal override void OnSetupCommand(Player p, string[] args) {
            LevelPreset preset = FindPreset(args[2]);
            switch (args[1].ToLower()) {
                case "new":
                case "create":
                case "add":
                    AddPreset(p, args, preset); break;
                case "delete":
                case "remove":
                    RemovePreset(p, args, preset); break;
                case "edit":
                case "change":
                    EditPreset(p, args, preset); break;
                default:
                    OnSetupCommandHelp(p); break;
            }
        }
        
        void AddPreset(Player p, string[] args, LevelPreset preset) {
            if (preset != null) { Player.Message(p, "%cThat preset level already exists"); return; }
            
            preset = new LevelPreset();
            preset.name = args[2];
            if (OkayAxis(args[3]) && OkayAxis(args[4]) && OkayAxis(args[5])) {
                preset.x = args[3]; preset.y = args[4]; preset.z = args[5];
            } else {
                Player.Message(p, "%cDimension must be a power of 2"); return;
            }
            
            if (!MapGen.IsRecognisedTheme(args[6])) {
                MapGen.PrintThemes(p); return;
            }
            preset.type = args[6].ToLower();
            if (!CommandParser.GetInt(p, args[7], "Price", ref preset.price, 0)) return;

            Presets.Add(preset);
            Player.Message(p, "%aSuccessfully added the following map preset:");
            Player.Message(p, "Name: %f" + preset.name);
            Player.Message(p, "x:" + preset.x + ", y:" + preset.y + ", z:" + preset.z);
            Player.Message(p, "Map Type: %f" + preset.type);
            Player.Message(p, "Map Price: %f" + preset.price + " %3" + Server.moneys);
        }
        
        void RemovePreset(Player p, string[] args, LevelPreset preset) {
            if (preset == null) { Player.Message(p, "%cThat preset level doesn't exist"); return; }
            Presets.Remove(preset);
            Player.Message(p, "%aSuccessfully removed preset: %f" + preset.name);
        }
        
        void EditPreset(Player p, string[] args, LevelPreset preset) {
            if (preset == null) { Player.Message(p, "%cThat preset level doesn't exist"); return; }
            
            if (args[3] == "name" || args[3] == "title") {
                preset.name = args[4];
                Player.Message(p, "%aSuccessfully changed preset name to %f" + preset.name);
            } else if (args[3] == "x" || args[3] == "y" || args[3] == "z") {
                if (!OkayAxis(args[4])) { Player.Message(p, "%cDimension was wrong, it must be a power of 2"); return; }

                if (args[3] == "x") preset.x = args[4];
                if (args[3] == "y") preset.y = args[4];
                if (args[3] == "z") preset.z = args[4];
                Player.Message(p, "%aSuccessfully changed preset {0} size to %f{1}", args[3], args[4]);
            } else if (args[3] == "type" || args[3] == "theme") {
                if (!MapGen.IsRecognisedTheme(args[4])) { MapGen.PrintThemes(p); return; }
                
                preset.type = args[4].ToLower();
                Player.Message(p, "%aSuccessfully changed preset type to %f" + preset.type);
            } else if (args[3] == "price") {
                int newPrice = 0;
                if (!CommandParser.GetInt(p, args[4], "Price", ref newPrice, 0)) return;
                
                preset.price = newPrice;
                Player.Message(p, "%aSuccessfully changed preset price to %f" + preset.price + " %3" + Server.moneys);
            } else {
                Player.Message(p, "Supported properties to edit: name, title, x, y, z, type, price");
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
            Player.Message(p, "&6Maps %S- see /store maps");
        }
        
        protected internal override void OnStoreCommand(Player p) {
            Player.Message(p, "&aAvailable maps to buy:");
            if (Presets.Count == 0) {
                Player.Message(p, "&6-None-"); return;
            }
            
            foreach (LevelPreset preset in Presets) {
                Player.Message(p, "&6{0} %S({1}, {2}, {3}) {4}: &a{5} %S{6}",
                               preset.name, preset.x, preset.y, preset.z,
                               preset.type, preset.price, Server.moneys);
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
