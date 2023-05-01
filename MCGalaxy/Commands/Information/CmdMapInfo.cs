/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using System.Text;
using MCGalaxy.Commands.World;
using MCGalaxy.DB;
using MCGalaxy.Games;
using MCGalaxy.Levels.IO;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Info 
{
    public sealed class CmdMapInfo : Command2 
    {
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
            MapInfo info = new MapInfo();
            
            // User provided specific map name
            if (lvl == null) {
                map = Matcher.FindMaps(p, map);
                if (map == null) return;
                lvl = LevelInfo.FindExact(map);
            }
            
            if (lvl != null) {
                info.FromLevel(lvl);
            } else {
                info.FromMap(map);
            }

            // shouldn't be able to see env of levels can't vsit
            if (env && map.Length > 0 && !info.Visit.CheckDetailed(p, data.Rank)) {
                p.Message("Hence, you cannot see its environment settings"); return;
            }
            
            if (env) ShowEnv(p, info, info.Config);
            else ShowNormal(p, info, info.Config);
        }
        
        void ShowNormal(Player p, MapInfo data, LevelConfig cfg) {
            p.Message("&bAbout {0}&S: Width={1} Height={2} Length={3}", 
                      cfg.Color + data.Name, data.Width, data.Height, data.Length);
            
            string physicsState = CmdPhysics.states[cfg.Physics];
            p.Message("  Physics are {0}&S, gun usage is {1}",
                      physicsState, cfg.Guns ? "&aenabled" : "&cdisabled");

            DateTime createTime  = File.GetCreationTimeUtc(LevelInfo.MapPath(data.MapName));
            TimeSpan createDelta = DateTime.UtcNow - createTime;
            string backupPath    = LevelInfo.BackupBasePath(data.MapName);
            
            if (Directory.Exists(backupPath)) {
                int latest = LevelInfo.LatestBackup(data.MapName);
                DateTime backupTime = File.GetCreationTimeUtc(LevelInfo.BackupFilePath(data.MapName, latest.ToString()));
                TimeSpan backupDelta = DateTime.UtcNow - backupTime;
                p.Message("  Created {2} ago, last backup ({1} ago): &a{0}",
                          latest, backupDelta.Shorten(), createDelta.Shorten());
            } else {
                p.Message("  Created {0} ago, no backups yet", createDelta.Shorten());
            }
            
            string dbFormat = "  BlockDB (Used for /b) is {0} &Swith {1} entries";
            if (data.BlockDBEntries == -1) dbFormat = "  BlockDB (Used for /b) is {0}";
            p.Message(dbFormat, 
                      cfg.UseBlockDB ? "&aEnabled" : "&cDisabled", data.BlockDBEntries);
            
            ShowPermissions(p, data, cfg);
            p.Message("Use &T/mi env {0} &Sto see environment settings.", data.MapName);
            ShowGameInfo(p, data, cfg);
        }
        
        void ShowPermissions(Player p, MapInfo data, LevelConfig cfg) {
            PrintRanks(p, data.Visit, "  Visitable by ");
            PrintRanks(p, data.Build, "  Modifiable by ");
            
            string realmOwner = cfg.RealmOwner;
            if (String.IsNullOrEmpty(cfg.RealmOwner)) {
                realmOwner = LevelInfo.DefaultRealmOwner(data.MapName);
            }
            if (String.IsNullOrEmpty(realmOwner)) return;
            
            string[] owners = realmOwner.SplitComma();
            p.Message("  This map is a personal realm of {0}", owners.Join(n => p.FormatNick(n)));
        }
        
        static void PrintRanks(Player p, AccessController access, string initial) {
            StringBuilder perms = new StringBuilder(initial);
            access.Describe(p, perms);
            p.Message(perms.ToString());
        }
        
        void ShowGameInfo(Player p, MapInfo data, LevelConfig cfg) {
            IGame game = GetAssociatedGame(data.MapName);
            if (game == null) return;
            
            IGame.OutputMapSummary(p, cfg); // TODO: Always show this info?
            game.OutputMapInfo(p, data.MapName, cfg);
        }
        
        static IGame GetAssociatedGame(string map)
        {
            IGame[] games = IGame.RunningGames.Items;
            foreach (IGame game in games)
            {
                if (game.ClaimsMap(map)) return game;
            }
            return null;
        }
        
        void ShowEnv(Player p, MapInfo data, LevelConfig cfg) {
            string url = cfg.Terrain.Length > 0 ? cfg.Terrain : Server.Config.DefaultTerrain;
            if (url.Length > 0) {
                p.Message("Terrain: &b" + url);
            } else {
                p.Message("No custom terrain set for this map.");
            }
            
            url = cfg.TexturePack.Length > 0 ? cfg.TexturePack : Server.Config.DefaultTexture;
            if (url.Length > 0) {
                p.Message("Texture pack: &b" + url);
            } else {
                p.Message("No custom texture pack set for this map.");
            }
            
            p.Message("Colors: &eFog {0}, &eSky {1}, &eClouds {2}, &eSunlight {3}, &eShadowlight {4}", 
                      Color(cfg.FogColor), Color(cfg.SkyColor),
                      Color(cfg.CloudColor), Color(cfg.LightColor), Color(cfg.ShadowColor));           
            p.Message("Water level: &b{0}&S, Bedrock offset: &b{1}&S, Clouds height: &b{2}&S, Max fog distance: &b{3}",
                      data.Get(EnvProp.EdgeLevel),   data.Get(EnvProp.SidesOffset), 
                      data.Get(EnvProp.CloudsLevel), data.Get(EnvProp.MaxFog));
            p.Message("Edge Block: &b{0}&S, Horizon Block: &b{1}", 
                      Block.GetName(p, (BlockID)data.Get(EnvProp.SidesBlock)),
                      Block.GetName(p, (BlockID)data.Get(EnvProp.EdgeBlock)));
            p.Message("Clouds speed: &b{0}%&S, Weather speed: &b{1}%",
                      (data.Get(EnvProp.CloudsSpeed)  / 256f).ToString("F2"),
                      (data.Get(EnvProp.WeatherSpeed) / 256f).ToString("F2"));
            p.Message("Weather fade rate: &b{0}%&S, Exponential fog: {1}",
                      (data.Get(EnvProp.WeatherFade) / 128f).ToString("F2"),
                      data.Get(EnvProp.ExpFog) > 0 ? "&aON" : "&cOFF");
            p.Message("Skybox rotations: Horizontal &b{0}&S, Vertical &b{1}",
                      data.GetSkybox(EnvProp.SkyboxHorSpeed), 
                      data.GetSkybox(EnvProp.SkyboxVerSpeed));
        }
        
        class MapInfo {
            public ushort Width, Height, Length;
            public string Name, MapName;
            public long BlockDBEntries = -1;
            public AccessController Visit, Build;
            public LevelConfig Config;

            public void FromLevel(Level lvl) {
                Name = lvl.name; MapName = lvl.MapName;
                Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
                BlockDBEntries = lvl.BlockDB.TotalEntries();
                Config = lvl.Config;
                
                Visit = lvl.VisitAccess; 
                Build = lvl.BuildAccess;
            }
            
            public void FromMap(string map) {
                this.Name = map; MapName = map;
                string path  = LevelInfo.MapPath(map);
                Vec3U16 dims = IMapImporter.GetFor(path).ReadDimensions(path);
                
                Width = dims.X; Height = dims.Y; Length = dims.Z;
                BlockDBEntries = BlockDBFile.CountEntries(map);

                path = LevelInfo.PropsPath(map);
                LevelConfig cfg = new LevelConfig();
                cfg.Load(path);
                
                Config = cfg;
                Visit = new LevelAccessController(cfg, map, true);
                Build = new LevelAccessController(cfg, map, false);
            }
            
            public int Get(EnvProp i) {
                int value    = Config.GetEnvProp(i);
                bool block   = i == EnvProp.EdgeBlock || i == EnvProp.SidesBlock;
                int default_ = block ? Block.Invalid : EnvConfig.ENV_USE_DEFAULT;
                return value != default_ ? value : EnvConfig.DefaultEnvProp(i, Height);
            }
            
            public string GetSkybox(EnvProp i) {
                int angle = Get(i);
                return angle == 0 ? "none" : (angle / 1024.0).ToString("F3") + "/s";
            }
        }
        
        static string Color(string src) {
            return (src == null || src.Length == 0 || src == "-1") ? "&bnone" : "&b" + src;
        }
        
        public override void Help(Player p)  {
            p.Message("&T/MapInfo [level]");
            p.Message("&HDisplay details of [level]");
            p.Message("&T/MapInfo env [level]");
            p.Message("&HDisplay environment details of [level]");
        }
    }
}
