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
using System.IO;
using MCGalaxy.Config;

namespace MCGalaxy.Commands.CPE {
    public sealed class CmdEnvironment : Command2 {
        public override string name { get { return "Environment"; } }
        public override string shortcut { get { return "Env"; } }
        public override string type { get { return CommandTypes.World; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.CaselessEq("preset")) {
                MessagePresets(p); return;
            }
            
            Level lvl     = null;
            EnvConfig cfg = null;
            string area   = "server";
            
            if (message.CaselessStarts("global ")) {
                message = message.Substring("global ".Length);
                cfg = Server.Config;
            } else if (message.CaselessStarts("level ")) {
                message = message.Substring("level ".Length);
            }
            
            // Work on current level by default
            if (cfg == null) {
                lvl = p.level; cfg = lvl.Config;
                area = lvl.ColoredName;
                if (!LevelInfo.Check(p, data.Rank, lvl, "set env settings of this level")) return;
            }
            
            string[] args = message.SplitSpaces();
            string opt = args[0], value = args.Length > 1 ? args[1] : "";
            if (!Handle(p, lvl, opt, value, cfg, area)) { Help(p); }
        }
        
        internal static bool Handle(Player p, Level lvl, string type, string value, EnvConfig cfg, string area) {
            if (type.CaselessEq("preset")) {
                EnvPreset preset = FindPreset(value);
                if (preset == null) { MessagePresets(p); return false; }
                
                cfg.SkyColor    = preset.Sky;
                cfg.CloudColor  = preset.Clouds;
                cfg.FogColor    = preset.Fog;
                cfg.ShadowColor = preset.Shadow;
                cfg.LightColor  = preset.Sun;
            } else if (type.CaselessEq("normal")) {
                cfg.ResetEnv();
                p.Message("Reset environment for {0} %Sto normal", area);
            } else {
                EnvOption opt = EnvOptions.Find(type);
                if (opt == null) return false;
                opt.SetFunc(p, area, cfg, value);
            }
            
            if (lvl == null) {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    pl.SendCurrentEnv();
                }
                SrvProperties.Save();
            } else {
                SendEnv(lvl);
                lvl.SaveSettings();
            }
            return true;
        }
        
        static void SendEnv(Level lvl) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl) continue;
                pl.SendCurrentEnv();
            }
        }
        
        static EnvPreset FindPreset(string value) {
            // fog, sky, clouds, sun, shadow
            if (value.CaselessEq("cartoon"))  return new EnvPreset("00FFFF 1E90FF 00BFFF F5DEB3 F4A460");
            if (value.CaselessEq("noir"))     return new EnvPreset("000000 1F1F1F 000000 696969 1F1F1F");
            if (value.CaselessEq("trippy"))   return new EnvPreset("4B0082 FFD700 006400 7CFC00 B22222");
            if (value.CaselessEq("watery"))   return new EnvPreset("5F9EA0 008080 008B8B E0FFFF 008B8B");
            if (value.CaselessEq("gloomy"))   return new EnvPreset("6A80A5 405875 405875 444466 3B3B59");
            if (value.CaselessEq("cloudy"))   return new EnvPreset("AFAFAF 8E8E8E 8E8E8E 9B9B9B 8C8C8C");
            if (value.CaselessEq("sunset"))   return new EnvPreset("FFA322 836668 9A6551 7F6C60 46444C");
            if (value.CaselessEq("midnight")) return new EnvPreset("131947 070A23 1E223A 181828 0F0F19");
            
            if (value.CaselessEq("normal")) {
                return new EnvPreset("    ");
            } else if (File.Exists("presets/" + value.ToLower() + ".env")) {
                string text = File.ReadAllText("presets/" + value.ToLower() + ".env");
                return new EnvPreset(text);
            }
            return null;
        }
        
        class EnvPreset {
            public string Fog, Sky, Clouds, Sun, Shadow;
            
            public EnvPreset(string raw) {
                string[] args = raw.SplitSpaces();
                Fog = args[0]; Sky = args[1]; Clouds = args[2]; Sun = args[3]; Shadow = args[4];
            }
        }

        static void MessagePresets(Player p) {
            p.Message("%T/Env preset [type] %H- Applies an env preset on the map");
            p.Message("%HPresets: &fCartoon/Midnight/Noir/Normal/Trippy/Watery/Sunset/Gloomy/Cloudy");
            if (!Directory.Exists("presets")) return;
            
            string[] files = Directory.GetFiles("presets", "*.env");
            string all = files.Join(f => Path.GetFileNameWithoutExtension(f));
            if (all.Length > 0) p.Message("%HCustom presets: &f" + all);
        }
        
        public override void Help(Player p) {
            p.Message("%T/Environment global/level [variable] [value]");
            p.Message("%HChanges server default or current level's environment.");
            p.Message("%HSee %T/Help env variables %Hfor list of variables");
            p.Message("%T/Environment global/level normal");
            p.Message("%HResets all environment variables to default");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("variable") || message.CaselessEq("variables")) {
                p.Message("%HVariables: &f{0}", EnvOptions.Options.Join(o => o.Name));
                p.Message("%HUse %T/Help env [variable] %Hto see details for that variable");
                return;
            }
            
            EnvOption opt = EnvOptions.Find(message);
            if (opt != null) {
                p.Message("%T/Env {0} [value]", opt.Name);
                p.Message(opt.Help);
                p.Message("%HUse 'normal' for [value] to reset to default");
            } else {
                p.Message("%WUnrecognised property \"{0}\"", message);
            }
        }
    }
}
