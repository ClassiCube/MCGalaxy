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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Fun;
using MCGalaxy.Games;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.LS 
{
    sealed class CmdLavaSurvival : RoundsGameCmd 
    {
        public override string name { get { return "LavaSurvival"; } }
        public override string shortcut { get { return "LS"; } }
        protected override RoundsGame Game { get { return LSGame.Instance; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can manage lava survival") }; }
        }

        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            string prop = args[1];
            LSMapConfig cfg = new LSMapConfig();
            LoadMapConfig(p, cfg);
            
            if (prop.CaselessEq("spawn")) {
                HandleSetSpawn(p, args, cfg);
            } else if (prop.CaselessEq("block")) {
                HandleSetBlock(p, args, cfg);
            } else if (prop.CaselessEq("other")) {
                HandleSetOther(p, args, cfg, 
                               (LSConfig)game.GetConfig());
            } else if (prop.CaselessEq("layer")) {
                HandleSetLayer(p, args, cfg, 
                               (LSConfig)game.GetConfig());
            } else {
                Help(p, "set");
            }
        }

        static bool ParseChance(Player p, string arg, string[] args, ref int value) {
            if (!CommandParser.GetInt(p, args[3], "Chance", ref value, 0, 100)) return false;
            p.Message("Set {0} chance to &b{1}%", arg, value);
            return true;
        }
        
        static bool ParseTimespan(Player p, string arg, string[] args, ref TimeSpan? span) {
            TimeSpan value = default(TimeSpan);
            if (!CommandParser.GetTimespan(p, args[3], ref value, "set " + arg + " to", "m")) return false;
            
            span = value;
            p.Message("Set {0} to &b{1}", arg, value.Shorten(true));
            return true;
        }
        
        
        void HandleSetSpawn(Player p, string[] args, LSMapConfig cfg) {
            if (args.Length < 3) {
                p.Message("Flood position: &b" + cfg.FloodPos);
                p.Message("Safe zone: &b({0}) ({1})", cfg.SafeZoneMin, cfg.SafeZoneMax);
                return;
            }
            
            string prop = args[2];
            if (prop.CaselessEq("flood")) {
                p.Message("Place or destroy the block you want to be the flood block spawn point.");
                p.MakeSelection(1, cfg, SetFloodPos);
                return;
            } else if (prop.CaselessEq("safe")) {
                p.Message("Place or break two blocks to determine the edges.");
                p.MakeSelection(2, cfg, SetSafeZone);
                return;
            }
            
            Help(p, "spawn");
        }
        
        bool SetFloodPos(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = (LSMapConfig)state;
            cfg.FloodPos    = (Vec3U16)m[0];
            SaveMapConfig(p, cfg);

            p.Message("Flood position set to &b({0})", m[0]);
            return false;
        }
        
        bool SetSafeZone(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = (LSMapConfig)state;
            cfg.SafeZoneMin = (Vec3U16)Vec3S32.Min(m[0], m[1]);
            cfg.SafeZoneMax = (Vec3U16)Vec3S32.Max(m[0], m[1]);
            SaveMapConfig(p, cfg);

            p.Message("Safe zone set! &b({0}) ({1})", cfg.SafeZoneMin, cfg.SafeZoneMax);
            return false;
        }
        
        void HandleSetBlock(Player p, string[] args, LSMapConfig cfg) {
            if (args.Length < 3) {
                p.Message("Fast lava chance: &b" + cfg.FastChance + "%");
                p.Message("Water flood chance: &b" + cfg.WaterChance + "%");
                p.Message("Flood upwards chance: &b" + cfg.FloodUpChance + "%");
                return;
            }
            
            string prop = args[2];
            if (args.Length < 4) { Help(p, "block"); return; }
            bool ok = false;
            
            if (prop.CaselessEq("fast")) {
                ok = ParseChance(p, "fast lava", args, ref cfg.FastChance);
            } else if (prop.CaselessEq("water")) {
                ok = ParseChance(p, "water flood", args, ref cfg.WaterChance);
            } else if (prop.CaselessEq("upwards")) {
                ok = ParseChance(p, "flood upwards", args, ref cfg.FloodUpChance);
            } else {
                Help(p, "block");
            }
            
            if (ok) SaveMapConfig(p, cfg);
        }
        
        void HandleSetOther(Player p, string[] args, LSMapConfig cfg, LSConfig gameCfg) {
            if (args.Length < 3) {
                p.Message("Round time: &b" + gameCfg.GetRoundTime(cfg).Shorten(true));
                p.Message("Flood time: &b" + gameCfg.GetFloodTime(cfg).Shorten(true));
                return;
            }
            
            string prop = args[2];
            if (args.Length < 4) { Help(p, "other"); return; }
            bool ok = false;
            
            if (prop.CaselessEq("round")) {
                ok = ParseTimespan(p, "round time", args, ref cfg._RoundTime);
            } else if (prop.CaselessEq("flood")) {
                ok = ParseTimespan(p, "flood time", args, ref cfg._FloodTime);
            } else {
                Help(p, "other");
            }
            
            if (ok) SaveMapConfig(p, cfg);
        }


        void HandleSetLayer(Player p, string[] args, LSMapConfig cfg, LSConfig gameCfg) {
            if (args.Length < 3) {
                p.Message("Layer flood chance: &b" + cfg.LayerChance + "%");
                p.Message("Layer time: &b" + gameCfg.GetLayerInterval(cfg).Shorten(true));
                p.Message("Layer position: &b" + cfg.LayerPos);
                p.Message("  &b{0} &Slayers, each &b{1} &Sblocks tall",
                          cfg.LayerCount, cfg.LayerHeight);
                return;
            }
            
            string prop = args[2];
            if (prop.CaselessEq("spawn")) {
                p.Message("Place or destroy the block you want to be the layer flood base spawn point.");
                p.MakeSelection(1, cfg, SetLayerPos);
                return;
            }
            
            if (args.Length < 4) { Help(p, "layer"); return; }
            bool ok = false;
            
            if (prop.CaselessEq("time")) {
                ok = ParseTimespan(p, "layer time", args, ref cfg._LayerInterval);
            } else if (prop.CaselessEq("height")) {
                ok = CommandParser.GetInt(p, args[3], "Height", ref cfg.LayerHeight);
                if (ok) p.Message("Set layer height to &b" + cfg.LayerHeight + " blocks");
            } else if (prop.CaselessEq("count")) {
                ok = CommandParser.GetInt(p, args[3], "Count", ref cfg.LayerCount, 0);
                if (ok) p.Message("Set layer count to &b" + cfg.LayerCount);
            } else if (prop.CaselessEq("chance")) {
                ok = ParseChance(p, "layer flood", args, ref cfg.LayerChance);
            } else {
                Help(p, "layer");
            }
            
            if (ok) SaveMapConfig(p, cfg);
        }
        
        bool SetLayerPos(Player p, Vec3S32[] m, object state, BlockID block) {
            LSMapConfig cfg = (LSMapConfig)state;
            cfg.LayerPos    = (Vec3U16)m[0];
            SaveMapConfig(p, cfg);

            p.Message("Layers start position set to &b({0})", m[0]);
            return false;
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                p.Message("&T/Help LS spawn &H- Views help for lava spawn settings");
                p.Message("&T/Help LS block &H- Views help for lava block settings");
                p.Message("&T/Help LS other &H- Views help for other settings");
                p.Message("&T/Help LS layer &H- Views help for layered flood settings");
            } else if (message.CaselessEq("spawn")) {
                p.Message("&T/LS set spawn &H- View lava spawns and safe zone location");
                p.Message("&T/LS set spawn flood &H- Set position lava floods from");
                p.Message("&T/LS set spawn safe &H- Sets safe area that can't be flooded");
            } else if (message.CaselessEq("block")) {
                p.Message("&T/LS set block &H- View lava block type settings");
                p.Message("&T/LS set block fast [chance] &H- Sets chance of fast lava");
                p.Message("&T/LS set block water [chance]");
                p.Message("&HSets chance of water instead of lava flood");
                p.Message("&T/LS set block upwards [chance]");
                p.Message("&HSets chance of lava/water flooding upwards");
            } else if (message.CaselessEq("other")) {
                p.Message("&T/LS set other &H- View times and safe zone location");
                p.Message("&T/LS set other flood [timespan]");
                p.Message("&HSet how long until the map is flooded");
                p.Message("&T/LS set other round [timespan]");
                p.Message("&HSets how long until the round ends");
            } else if (message.CaselessEq("layer")) {
                p.Message("&T/LS set layer &H- View layer flooding info");
                p.Message("&T/LS set layer spawn &H- Set start position layers flood from");
                p.Message("&T/LS set layer height [height] &H- Sets height of each layer");
                p.Message("&T/LS set layer count [count] &H- Sets number of layers");
                p.Message("&T/LS set layer chance [chance]");
                p.Message("&HSets chance of layer flooding occurring instead");
                p.Message("&T/LS set layer time [timespan]");
                p.Message("&HSets interval between layer floods");
            } else {
                base.Help(p, message);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/LS start <map> &H- Starts Lava Survival");
            p.Message("&T/LS stop &H- Stops Lava Survival");
            p.Message("&T/LS end &H- Ends current round of Lava Survival");
            p.Message("&T/LS add/remove &H- Adds/removes current map from map list");
            p.Message("&T/LS set [property] &H- Sets a property. See &T/Help LS set");
            p.Message("&T/LS status &H- View current round info and time");
            p.Message("&T/LS go &H- Moves you to the current Lava Survival map");
        }
    }
}
