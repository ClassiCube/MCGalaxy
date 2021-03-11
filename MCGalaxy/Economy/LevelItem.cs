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
using MCGalaxy.Commands;
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
                preset.name = args[2];
                Presets.Add(preset);
            }
            
            switch (args[3]) {
                    case "price": preset.price = int.Parse(args[4]); break;
                    case "x": preset.x = args[4]; break;
                    case "y": preset.y = args[4]; break;
                    case "z": preset.z = args[4]; break;
                    case "type": preset.type = args[4]; break;
            }
        }
        
        public override void Serialise(StreamWriter writer) {
            foreach (LevelPreset preset in Presets) {
                writer.WriteLine();
                string prefix = "level:levels:" + preset.name;
                
                writer.WriteLine(prefix + ":price:" + preset.price);
                writer.WriteLine(prefix + ":x:" + preset.x);
                writer.WriteLine(prefix + ":y:" + preset.y);
                writer.WriteLine(prefix + ":z:" + preset.z);
                writer.WriteLine(prefix + ":type:" + preset.type);
            }
        }
        
        protected internal override void OnPurchase(Player p, string raw) {
            string[] args = raw.SplitSpaces();
            if (raw.Length == 0) { OnStoreCommand(p); return; }
            LevelPreset preset = FindPreset(args[0]);
            
            if (preset == null) { p.Message("&WThat isn't a level preset"); return; }
            if (!CheckPrice(p, preset.price, "that map")) return;
            
            string name = null;
            if (args.Length > 1) {
                name = p.name + "_" + args[1];
            } else {
                // use a numbered map by default
                for (int i = 1; i < 100; i++) {
                    name = p.name + "_" + i;
                    if (!LevelInfo.MapExists(name)) break;
                }
            }
            
            p.Message("&aCreating level: '&f" + name + "&a' . . .");
            
            ushort x = 0, y = 0, z = 0;
            string[] xyz = { preset.x, preset.y, preset.z };
            if (!MapGen.GetDimensions(p, xyz, 0, ref x, ref y, ref z, false)) return;
            
            MapGen gen = MapGen.Find(preset.type);
            Level lvl  = MapGen.Generate(p, gen, name, x, y, z, "");
            if (lvl == null) return;
            
            MapGen.SetRealmPerms(p, lvl);           
            try {
                lvl.Save(true);
            } finally {
                lvl.Dispose();
                Server.DoGC();
            }
            Economy.MakePurchase(p, preset.price, "%3Map: %f" + preset.name);
        }
        
        protected internal override void OnSetup(Player p, string[] args) {
            LevelPreset preset = FindPreset(args[2]);
            string cmd = args[1];
            
            if (Command.IsCreateCommand(cmd)) {
                AddPreset(p, args, preset);
            } else if (Command.IsDeleteCommand(cmd)) {
                RemovePreset(p, args, preset);
            } else if (Command.IsEditCommand(cmd)) {
                EditPreset(p, args, preset);
            } else {
                OnSetupHelp(p);
            }
        }
        
        void AddPreset(Player p, string[] args, LevelPreset preset) {
            if (preset != null) { p.Message("&WThat preset level already exists"); return; }
            
            preset = new LevelPreset();
            preset.name = args[2];
            
            ushort x = 0, y = 0, z = 0;
            if (!MapGen.GetDimensions(p, args, 3, ref x, ref y, ref z)) return;
            preset.x = args[3]; preset.y = args[4]; preset.z = args[5];
            
            if (MapGen.Find(args[6]) == null) {
                MapGen.PrintThemes(p); return;
            }
            preset.type = args[6];
            if (!CommandParser.GetInt(p, args[7], "Price", ref preset.price, 0)) return;

            Presets.Add(preset);
            p.Message("&aSuccessfully added the following map preset:");
            p.Message("Name: &f" + preset.name);
            p.Message("x:" + preset.x + ", y:" + preset.y + ", z:" + preset.z);
            p.Message("Map Type: &f" + preset.type);
            p.Message("Map Price: &f" + preset.price + " &3" + Server.Config.Currency);
        }
        
        void RemovePreset(Player p, string[] args, LevelPreset preset) {
            if (preset == null) { p.Message("&WThat preset level doesn't exist"); return; }
            Presets.Remove(preset);
            p.Message("&aSuccessfully removed preset: &f" + preset.name);
        }

        void EditPreset(Player p, string[] args, LevelPreset preset) {
            if (preset == null) { p.Message("&WThat preset level doesn't exist"); return; }
            
            if (args[3] == "name" || args[3] == "title") {
                preset.name = args[4];
                p.Message("&aSuccessfully changed preset name to &f" + preset.name);
            } else if (args[3] == "x" || args[3] == "y" || args[3] == "z") {
                string[] dims = new string[] { preset.x, preset.y, preset.z };
                if (args[3] == "x") dims[0] = args[4];
                if (args[3] == "y") dims[1] = args[4];
                if (args[3] == "z") dims[2] = args[4];
                
                ushort x = 0, y = 0, z = 0;
                if (!MapGen.GetDimensions(p, dims, 0, ref x, ref y, ref z)) return;
                preset.x = dims[0]; preset.y = dims[1]; preset.z = dims[2];
                
                p.Message("&aSuccessfully changed preset {0} size to &f{1}", args[3], args[4]);
            } else if (args[3] == "type" || args[3] == "theme") {
                if (MapGen.Find(args[4]) == null) { MapGen.PrintThemes(p); return; }
                
                preset.type = args[4];
                p.Message("&aSuccessfully changed preset type to &f" + preset.type);
            } else if (args[3] == "price") {
                int newPrice = 0;
                if (!CommandParser.GetInt(p, args[4], "Price", ref newPrice, 0)) return;
                
                preset.price = newPrice;
                p.Message("&aSuccessfully changed preset price to &f" + preset.price + " &3" + Server.Config.Currency);
            } else {
                p.Message("Supported properties to edit: name, title, x, y, z, type, price");
            }
        }
        
        protected internal override void OnSetupHelp(Player p) {
            base.OnSetupHelp(p);
            p.Message("&T/Eco level add [name] [x] [y] [z] [theme] [price]");
            p.Message("&T/Eco level remove [name]");
            p.Message("&T/Eco level edit [name] [name/x/y/z/type/price] [value]");
            p.Message("&HAdds, removes, or edits a level preset.");
        }
        
        protected internal override void OnStoreOverview(Player p) {
            p.Message("&6Maps &S- see &T/Store maps");
        }
        
        protected internal override void OnStoreCommand(Player p) {
            p.Message("&aAvailable maps to buy:");
            if (Presets.Count == 0) {
                p.Message("&6-None-"); return;
            }
            
            foreach (LevelPreset preset in Presets) {
                p.Message("&6{0} &S({1}, {2}, {3}) {4}: &a{5} &S{6}",
                          preset.name, preset.x, preset.y, preset.z,
                          preset.type, preset.price, Server.Config.Currency);
            }
        }
        
        public LevelPreset FindPreset(string name) {
            foreach (LevelPreset preset in Presets) {
                if (preset.name.CaselessEq(name)) return preset;
            }
            return null;
        }
    }
}
