/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Text;
using MCGalaxy.Commands.World;
using MCGalaxy.DB;
using MCGalaxy.Games;
using MCGalaxy.Levels.IO;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdMapInfo : Command2 {
        public override string name { get { return "MapInfo"; } }
        public override string shortcut { get { return "mi"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WInfo"), new CommandAlias("WorldInfo") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            bool env = args[0].CaselessEq("env");
            string map = env ? (args.Length > 1 ? args[1] : "") : args[0];

            Level lvl = map.Length == 0 ? p.level : null;
            MapInfoData mapData = new MapInfoData();
            
            if (lvl == null) {
                if (!GetFromMap(p, mapData, map)) return;
            } else {
                mapData.FromOnlineLevel(lvl);
            }
            
            if (env) ShowEnv(p, mapData, mapData.Config);
            else ShowNormal(p, mapData, mapData.Config);
        }
        
        bool GetFromMap(Player p, MapInfoData data, string map) {
            map = Matcher.FindMaps(p, map);
            if (map == null) return false;
            
            Level lvl = LevelInfo.FindExact(map);
            if (lvl != null) {
                data.FromOnlineLevel(lvl);
            } else {
                data.FromOfflineLevel(map);
            }
            return true;
        }
        
        void ShowNormal(Player p, MapInfoData data, LevelConfig cfg) {
            p.Message("&bAbout {0}%S: Width={1} Height={2} Length={3}", 
                           cfg.Color + data.Name, data.Width, data.Height, data.Length);
            
            string physicsState = CmdPhysics.states[cfg.Physics];
            p.Message("  Physics are {0}%S, gun usage is {1}",
                           physicsState, cfg.Guns ? "&aenabled" : "&cdisabled");

            DateTime createTime = File.GetCreationTimeUtc(LevelInfo.MapPath(data.MapName));
            TimeSpan createDelta = DateTime.UtcNow - createTime;
            string backupPath = LevelInfo.BackupBasePath(data.MapName);
            
            if (Directory.Exists(backupPath)) {
                int latest = LevelInfo.LatestBackup(data.MapName);
                DateTime backupTime = File.GetCreationTimeUtc(LevelInfo.BackupFilePath(data.MapName, latest.ToString()));
                TimeSpan backupDelta = DateTime.UtcNow - backupTime;
                p.Message("  Created {2} ago, last backup ({1} ago): &a{0}",
                               latest, backupDelta.Shorten(), createDelta.Shorten());
            } else {
                p.Message("  Created {0} ago, no backups yet", createDelta.Shorten());
            }
            
            if (data.BlockDBEntries != -1) {
                p.Message("  BlockDB (Used for /b) is {0} %Swith {1} entries",
                               cfg.UseBlockDB ? "&aEnabled" : "&cDisabled", data.BlockDBEntries);
            } else {
                p.Message("  BlockDB (Used for /b) is {0}",
                               cfg.UseBlockDB ? "&aEnabled" : "&cDisabled");
            }
            
            ShowPermissions(p, data, cfg);
            p.Message("Use %T/mi env {0} %Sto see environment settings.", data.MapName);
            ShowZombieSurvival(p, data, cfg);
        }
        
        void ShowPermissions(Player p, MapInfoData data, LevelConfig cfg) {
            PrintRanks(p, data.Visit, "  Visitable by ");
            PrintRanks(p, data.Build, "  Modifiable by ");
            
            string realmOwner = cfg.RealmOwner;
            if (String.IsNullOrEmpty(cfg.RealmOwner)) {
                realmOwner = DefaultRealmOwner(data.MapName);
            }
            if (String.IsNullOrEmpty(realmOwner)) return;
            
            string[] owners = realmOwner.SplitComma();
            p.Message("  This map is a personal realm of {0}",
                           owners.Join(n => PlayerInfo.GetColoredName(p, n)));
        }
        
        static void PrintRanks(Player p, AccessController access, string initial) {
            StringBuilder perms = new StringBuilder(initial);
            access.Describe(p, perms);
            p.Message(perms.ToString());
        }
        
        void ShowZombieSurvival(Player p, MapInfoData data, LevelConfig cfg) {
            if (!ZSGame.Instance.HasMap(data.MapName)) return;
            
            string[] authors = cfg.Authors.SplitComma();
            p.Message("Map authors: {0}",
                           authors.Join(n => PlayerInfo.GetColoredName(p, n)));
            int winChance = cfg.RoundsPlayed == 0 ? 100 : (cfg.RoundsHumanWon * 100) / cfg.RoundsPlayed;
            p.Message("&a{0} %Srounds played total, &a{1}% %Swin chance for humans.",
                           cfg.RoundsPlayed, winChance);
            p.Message("This map has &a{0} likes %Sand &c{1} dislikes",
                           cfg.Likes, cfg.Dislikes);
        }
        
        static string DefaultRealmOwner(string map) {
            bool plus = ServerConfig.ClassicubeAccountPlus;
            // Early out when accounts have + and map doesn't.
            if (plus && map.IndexOf('+') == -1) return null;
            
            string name = null, origMap = map;
            while (map.Length > 0 && Char.IsNumber(map[map.Length - 1])) {
                // If the server does not have account with +, we have to account for the
                // that say Player123's second level is Player1232, and the realm owner is Player123
                name = plus ? null : PlayerInfo.FindName(map);
                if (name != null) break;
                map = map.Substring(0, map.Length - 1);
            }
            
            if (name == null) name = PlayerInfo.FindName(map);
            if (name != null && !LevelInfo.IsRealmOwner(name, origMap)) return null;
            return name;
        }
        
        void ShowEnv(Player p, MapInfoData data, LevelConfig cfg) {
            string url = cfg.Terrain.Length > 0 ? cfg.Terrain : ServerConfig.DefaultTerrain;
            if (url.Length > 0) {
                p.Message("Terrain: &b" + url);
            } else {
                p.Message("No custom terrain set for this map.");
            }
            
            url = cfg.TexturePack.Length > 0 ? cfg.TexturePack : ServerConfig.DefaultTexture;
            if (url.Length > 0) {
                p.Message("Texture pack: &b" + url);
            } else {
                p.Message("No custom texture pack set for this map.");
            }
            
            const string format = "Colors: Fog {0}, Sky {1}, Clouds {2}, Sunlight {3}, Shadowlight {4}";
            p.Message(format, Color(cfg.FogColor), Color(cfg.SkyColor), 
                           Color(cfg.CloudColor), Color(cfg.LightColor), Color(cfg.ShadowColor));
            
            p.Message("Water level: &b{0}%S, Bedrock offset: &b{1}%S, Clouds height: &b{2}%S, Max fog distance: &b{3}",
                           cfg.EdgeLevel, cfg.SidesOffset, cfg.CloudsHeight, cfg.MaxFogDistance);
            p.Message("Edge Block: &b{0}%S, Horizon Block: &b{1}", 
                           Block.GetName(p, cfg.EdgeBlock), Block.GetName(p, cfg.HorizonBlock));
            p.Message("Clouds speed: &b{0}%%S, Weather speed: &b{1}%",
                           (cfg.CloudsSpeed / 256f).ToString("F2"),
                           (cfg.WeatherSpeed / 256f).ToString("F2"));
            p.Message("Weather fade rate: &b{0}%%S, Exponential fog: {1}",
                           (cfg.WeatherFade / 128f).ToString("F2"),
                           cfg.ExpFog > 0 ? "&aON" : "&cOFF");
            p.Message("Skybox rotations: Horizontal &b{0}%S, Vertical &b{1}",
                           SkyboxSpeed(cfg.SkyboxHorSpeed), SkyboxSpeed(cfg.SkyboxVerSpeed));
        }
        
        static string SkyboxSpeed(int angle) { 
            return angle == 0 ? "none" : (angle / 1024.0).ToString("F3") + "/s";
        }
        
        class MapInfoData {
            public ushort Width, Height, Length;
            public string Name, MapName;
            public long BlockDBEntries = -1;
            public LevelAccessController Visit, Build;
            public LevelConfig Config;

            public void FromOnlineLevel(Level lvl) {
                Name = lvl.name; MapName = lvl.MapName;
                Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
                BlockDBEntries = lvl.BlockDB.TotalEntries();
                Config = lvl.Config;
                
                Visit = lvl.VisitAccess; 
                Build = lvl.BuildAccess;
            }
            
            public void FromOfflineLevel(string name) {
                this.Name = name; MapName = name;
                string path = LevelInfo.MapPath(name);
                Vec3U16 dims = IMapImporter.Formats[0].ReadDimensions(path);
                Width = dims.X; Height = dims.Y; Length = dims.Z;
                BlockDBEntries = BlockDBFile.CountEntries(name);

                path = LevelInfo.PropsPath(name);
                LevelConfig cfg = new LevelConfig();
                cfg.SetDefaults(Height);
                cfg.Load(path);
                
                Config = cfg;              
                Visit = new LevelAccessController(cfg, name, true);
                Build = new LevelAccessController(cfg, name, false);
            }
        }
        
        static string Color(string src) {
            return (src == null || src.Length == 0 || src == "-1") ? "&bnone&e" : "&b" + src + "&e";
        }
        
        public override void Help(Player p)  {
            p.Message("%T/MapInfo [map]");
            p.Message("%HDisplay details of [map]");
            p.Message("%T/MapInfo env [map]");
            p.Message("%HDisplay environment details of [map]");
        }
    }
}
