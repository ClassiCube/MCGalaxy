/*
    Copyright 2011 MCForge
    Created by Techjar (Jordan S.)
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdLavaSurvival : RoundsGameCmd {
        public override string name { get { return "LavaSurvival"; } }
        public override string shortcut { get { return "LS"; } }
        protected override RoundsGame Game { get { return Server.lava; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage lava survival") }; }
        }
        
        protected override void HandleStatus(Player p, RoundsGame game) {
            if (!game.Running) { Player.Message(p, "Lava survival is not running"); return; }
            if (!game.RoundInProgress) { Player.Message(p, "The round of Lava Survival hasn't started yet."); return; }
            game.OutputStatus(p);
        }

        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            if (!CheckExtraPerm(p, 1)) return;
            
            if (p == null) { Player.Message(p, "/{0} setup can only be used in-game.", name); return; }
            if (args.Length < 2) { SetupHelp(p); return; }
            string group = args[1];
            
            LSGame ls = (LSGame)game;
            if (group.CaselessEq("map")) {
                HandleSetupMap(p, args, ls);
            } else if (group.CaselessEq("block")) {
                HandleSetupBlock(p, args, ls);
            } else if (group.CaselessEq("safe") || group.CaselessEq("safezone")) {
                HandleSetupSafeZone(p, args, ls);
            } else if (group.CaselessEq("settings")) {
                HandleSetupSettings(p, args, ls);
            } else if (group.CaselessEq("mapsettings")) {
                HandleSetupMapSettings(p, args, ls);
            } else {
                SetupHelp(p);
            }
        }
        
        void HandleSetupMap(Player p, string[] args, LSGame game) {
            if (args.Length < 3) { SetupHelp(p, "map"); return; }
            Level lvl = Matcher.FindLevels(p, args[2]);
            if (lvl == null) return;
            if (lvl == Server.mainLevel) { Player.Message(p, "You cannot use the main map for Lava Survival."); return; }
            
            if (game.HasMap(lvl.name)) {
                game.RemoveMap(lvl.name);
                lvl.Config.AutoUnload = true;
                lvl.Config.LoadOnGoto = true;
                Player.Message(p, "Map {0} %Shas been removed.", lvl.ColoredName);
            } else {
                game.AddMap(lvl.name);
                lvl.Config.LoadOnGoto = false;
                Player.Message(p, "Map {0} %Shas been added.", lvl.ColoredName);
            }
            Level.SaveSettings(lvl);
        }

        void HandleSetupBlock(Player p, string[] args, LSGame game) {
            if (!game.HasMap(p.level.name)) { Player.Message(p, "Add the map before configuring it."); return; }
            if (args.Length < 3) { SetupHelp(p, "block"); return; }

            if (args[2] == "flood") {
                Player.Message(p, "Place or destroy the block you want to be the total flood block spawn point.");
                p.MakeSelection(1, game, SetFloodPos);
            } else if (args[2] == "layer") {
                Player.Message(p, "Place or destroy the block you want to be the layer flood base spawn point.");
                p.MakeSelection(1, game, SetFloodLayerPos);
            } else {
                SetupHelp(p, "block");
            }
        }
        
        void HandleSetupSafeZone(Player p, string[] args, LSGame game) {
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, game, SetSafeZone);
        }
        
        void HandleSetupSettings(Player p, string[] args, LSGame game) {
            Player.Message(p, "Maps: &b" + LSGame.Config.Maps.Join(", "));
            Player.Message(p, "Start on server startup: " + (LSGame.Config.StartImmediately ? "&aON" : "&cOFF"));
        }
        
        static bool ParseChance(Player p, string arg, string input, ref int value) {
            if (!CommandParser.GetInt(p, "Chance", input, ref value, 0, 100)) return false;
            Player.Message(p, "{0} chance: &b{1}%", arg, value);
            return true;
        }
        
        static bool ParseTimespan(Player p, string arg, string input, ref float value) {
            TimeSpan span = default(TimeSpan);
            if (!CommandParser.GetTimespan(p, input, ref span, "set " + arg + " to", "m")) return false;
            
            value = (float)span.TotalMinutes;
            Player.Message(p, "{0}: &b{1}", arg, span.Shorten(true));
            return true;
        }
        
        void HandleSetupMapSettings(Player p, string[] args, LSGame game) {
            if (!game.HasMap(p.level.name)) { Player.Message(p, "Add the map before configuring it."); return; }
            LSMapConfig cfg = RetrieveConfig(p);
            
            if (args.Length < 4) {
                Player.Message(p, "Fast lava chance: &b" + cfg.FastChance + "%");
                Player.Message(p, "Killer lava/water chance: &b" + cfg.KillerChance + "%");
                Player.Message(p, "Destroy blocks chance: &b" + cfg.DestroyChance + "%");
                Player.Message(p, "Water flood chance: &b" + cfg.WaterChance + "%");
                Player.Message(p, "Layer flood chance: &b" + cfg.LayerChance + "%");
                Player.Message(p, "Layer height: &b" + cfg.LayerHeight + " blocks");
                Player.Message(p, "Layer count: &b" + cfg.LayerCount);
                Player.Message(p, "Layer time: &b" + cfg.LayerIntervalMins + " minutes");
                Player.Message(p, "Round time: &b" + cfg.RoundTimeMins + " minutes");
                Player.Message(p, "Flood time: &b" + cfg.RoundTimeMins + " minutes");
                Player.Message(p, "Flood position: &b" + cfg.FloodPos);
                Player.Message(p, "Layer position: &b" + cfg.LayerPos);
                Player.Message(p, "Safe zone: &b({0}) ({1})", 
                               cfg.SafeZoneMin, cfg.SafeZoneMax);
                return;
            }

            string type = args[2], value = args[3];
            bool ok = false;
            if (type == "fast") {
                ok = ParseChance(p, "Fast lava", value, ref cfg.FastChance);
            } else if (type == "killer") {
                ok = ParseChance(p, "Killer lava/water", value, ref cfg.KillerChance);
            } else if (type == "destroy") {
                ok = ParseChance(p, "Destroy blocks", value, ref cfg.DestroyChance);
            } else if (type == "water") {
               ok = ParseChance(p, "Water flood", value, ref cfg.WaterChance);
            } else if (type == "layer") {
                ok = ParseChance(p, "Layer flood", value, ref cfg.LayerChance);
            } else if (type == "layerheight") {
                if (!CommandParser.GetInt(p, value, "Height", ref cfg.LayerHeight, 0)) return;
                Player.Message(p, "Layer height: &b" + cfg.LayerHeight + " blocks");
            } else if (type == "layercount") {
                if (!CommandParser.GetInt(p, value, "Count", ref cfg.LayerCount, 0)) return;
                Player.Message(p, "Layer count: &b" + cfg.LayerCount);
            } else if (type == "layertime") {
                ok = ParseTimespan(p, "Layer time", value, ref cfg.LayerIntervalMins);
            } else if (type == "roundtime") {
                ok = ParseTimespan(p, "Round time", value, ref cfg.RoundTimeMins);
            } else if (type == "floodtime") {
                ok = ParseTimespan(p, "Flood time", value, ref cfg.FloodTimeMins);
            } else {
                SetupHelp(p, "mapsettings");
            }
            
            if (ok) UpdateConfig(p, cfg);
        }

        void SetupHelp(Player p, string mode = "") {
            switch (mode) {
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
                    Player.Message(p, "View settings for Lava Survival.");
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
        

        bool SetFloodPos(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = RetrieveConfig(p);
            cfg.FloodPos = (Vec3U16)m[0];
            UpdateConfig(p, cfg);

            Player.Message(p, "Flood position set to &b({0})", m[0]);
            return false;
        }
        
        bool SetFloodLayerPos(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = RetrieveConfig(p);
            cfg.LayerPos = (Vec3U16)m[0];
            UpdateConfig(p, cfg);

            Player.Message(p, "Layer position set to &b({0})", m[0]);
            return false;
        }
        
        bool SetSafeZone(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = RetrieveConfig(p);
            cfg.SafeZoneMin = (Vec3U16)Vec3S32.Min(m[0], m[1]);
            cfg.SafeZoneMax = (Vec3U16)Vec3S32.Max(m[0], m[1]);
            UpdateConfig(p, cfg);

            Player.Message(p, "Safe zone set! &b({0}) ({1})", 
                           cfg.SafeZoneMin, cfg.SafeZoneMax);
            return false;
        }
        
        static LSMapConfig RetrieveConfig(Player p) {
            LSMapConfig cfg = new LSMapConfig();
            cfg.SetDefaults(p.level);
            cfg.Load(p.level.name);
            return cfg;
        }
        
        static void UpdateConfig(Player p, LSMapConfig cfg) {
            cfg.Save(p.level.name);
            if (p.level == Server.lava.Map) Server.lava.UpdateMapConfig();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/LS start <map> %H- Starts Lava Survival, optionally on the given map");
            Player.Message(p, "%T/LS stop %H- Stops the current Lava Survival game");
            Player.Message(p, "%T/LS end %H- End the current round or vote");
            Player.Message(p, "%T/LS setup %H- Setup lava survival, use it for more info");
            Player.Message(p, "%T/LS info %H- View current round info and time");
            Player.Message(p, "%T/LS go %H- Join the fun!");
        }
    }
}
