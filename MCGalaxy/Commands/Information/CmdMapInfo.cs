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
using MCGalaxy.Levels.IO;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdMapInfo : Command {
        public override string name { get { return "MapInfo"; } }
        public override string shortcut { get { return "mi"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WInfo"), new CommandAlias("WorldInfo") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            bool env = args[0].CaselessEq("env");
            string map = env ? (args.Length > 1 ? args[1] : "") : args[0];

            Level lvl = map.Length == 0 ? p.level : null;
            MapInfoData data = new MapInfoData();
            
            if (lvl == null) {
                if (!GetFromMap(p, data, map)) return;
            } else {
                data.FromOnlineLevel(lvl);
            }
            
            if (env) ShowEnv(p, data, data.Config);
            else ShowNormal(p, data, data.Config);
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
            Player.Message(p, "&bAbout {0}%S: Width={1} Height={2} Length={3}", data.Name, data.Width, data.Height, data.Length);
            string physicsState = CmdPhysics.states[cfg.Physics];
            if (p == null || p.group.CanExecute("gun")) {
                Player.Message(p, "  Physics are {0}%S, gun usage %Sis {1}",
                               physicsState, cfg.Guns ? "&aenabled" : "&cdisabled");
            } else {
                Player.Message(p, "  Physics are {0}", physicsState);
            }

            DateTime createTime = File.GetCreationTimeUtc(LevelInfo.MapPath(data.Name));
            TimeSpan createDelta = DateTime.UtcNow - createTime;
            if (Directory.Exists(ServerConfig.BackupDirectory + "/" + data.Name)) {
                int latest = Directory.GetDirectories(ServerConfig.BackupDirectory + "/" + data.Name).Length;
                DateTime backupTime = File.GetCreationTimeUtc(LevelInfo.BackupPath(data.Name, latest.ToString()));
                TimeSpan backupDelta = DateTime.UtcNow - backupTime;
                Player.Message(p, "  Created {2} ago, last backup ({1} ago): &a{0}",
                               latest, backupDelta.Shorten(), createDelta.Shorten());
            } else {
                Player.Message(p, "  Created {0} ago, no backups yet", createDelta.Shorten());
            }
            
            if (data.BlockDBEntries != -1) {
                Player.Message(p, "  BlockDB (Used for /b) is {0} %Swith {1} entries",
                               cfg.UseBlockDB ? "&aEnabled" : "&cDisabled", data.BlockDBEntries);
            } else {
                Player.Message(p, "  BlockDB (Used for /b) is {0}",
                               cfg.UseBlockDB ? "&aEnabled" : "&cDisabled");
            }
            
            ShowPermissions(p, data, cfg);
            Player.Message(p, "Use %T/mi env {0} %Sto see environment settings.", data.Name);
            ShowZombieSurvival(p, data, cfg);
        }
        
        void ShowPermissions(Player p, MapInfoData data, LevelConfig cfg) {
            PrintRanks(p, data.Visit, "  Visitable by ");
            PrintRanks(p, data.Build, "  Modifiable by ");
            
            string realmOwner = cfg.RealmOwner;
            if (String.IsNullOrEmpty(cfg.RealmOwner))
                realmOwner = GetRealmMapOwner(data.Name);
            if (String.IsNullOrEmpty(realmOwner)) return;
            
            string[] owners = realmOwner.Replace(" ", "").Split(',');
            Player.Message(p, "  This map is a personal realm of {0}",
                           owners.Join(n => PlayerInfo.GetColoredName(p, n)));
        }
        
        static void PrintRanks(Player p, LevelAccessController access, string initial) {
            StringBuilder perms = new StringBuilder(initial);
            perms.Append(Group.GetColoredName(access.Min) + "%S+");
            if (access.Max != LevelPermission.Nobody)
                perms.Append(" up to " + Group.GetColoredName(access.Max));
            
            List<string> whitelist = access.Whitelisted;
            foreach (string name in whitelist)
                perms.Append(", " + PlayerInfo.GetColoredName(p, name));

            List<string> blacklist = access.Blacklisted;
            if (blacklist.Count == 0) { Player.Message(p, perms.ToString()); return; }
            
            perms.Append( " %S(except ");
            foreach (string name in blacklist)
                perms.Append(PlayerInfo.GetColoredName(p, name) + ", ");
            perms.Remove(perms.Length - 2, 2);
            perms.Append("%S)");
            Player.Message(p, perms.ToString());
        }
        
        void ShowZombieSurvival(Player p, MapInfoData data, LevelConfig cfg) {
            if (!Server.zombie.IsZombieMap(data.Name)) return;
            
            string[] authors = cfg.Authors.Replace(" ", "").Split(',');
            Player.Message(p, "Map authors: {0}",
                           authors.Join(n => PlayerInfo.GetColoredName(p, n)));
            int winChance = cfg.RoundsPlayed == 0 ? 100 : (cfg.RoundsHumanWon * 100) / cfg.RoundsPlayed;
            Player.Message(p, "&a{0} %Srounds played total, &a{1}% %Swin chance for humans.",
                           cfg.RoundsPlayed, winChance);
            Player.Message(p, "This map has &a{0} likes %Sand &c{1} dislikes",
                           cfg.Likes, cfg.Dislikes);
        }
        
        static string GetRealmMapOwner(string lvlName) {
            bool plus = ServerConfig.ClassicubeAccountPlus;
            // Early out when accounts have + and map doesn't.
            if (plus && lvlName.IndexOf('+') == -1) return null;
            
            while (lvlName.Length > 0 && Char.IsNumber(lvlName[lvlName.Length - 1])) {
                // If the server does not have account with +, we have to account for the
                // that say Player123's second level is Player1232, and the realm owner is Player123
                string pName = plus ? null : PlayerInfo.FindName(lvlName);
                if (pName != null) return pName;
                lvlName = lvlName.Substring(0, lvlName.Length - 1);
            }
            return PlayerInfo.FindName(lvlName);
        }
        
        void ShowEnv(Player p, MapInfoData data, LevelConfig cfg) {
            if (data.TerrainUrl != "")
                Player.Message(p, "Texture: %b" + data.TerrainUrl);
            else
                Player.Message(p, "No custom texture set for this map.");
            
            if (data.TextureUrl != "")
                Player.Message(p, "Texture pack: %b" + data.TextureUrl);
            else
                Player.Message(p, "No custom texture pack set for this map.");
            
            const string format = "Colors: Fog {0}, Sky {1}, Clouds {2}, Sunlight {3}, Shadowlight {4}";
            Player.Message(p, format, Color(cfg.FogColor), Color(cfg.SkyColor), 
                           Color(cfg.CloudColor), Color(cfg.LightColor), Color(cfg.ShadowColor));
            
            Player.Message(p, "Water level: &b{0}%S, Bedrock offset: &b{1}%S, Clouds height: &b{2}%S, Max fog distance: &b{3}",
                           cfg.EdgeLevel,cfg.SidesOffset, cfg.CloudsHeight, cfg.MaxFogDistance);
            Player.Message(p, "Edge Block: &b{0}%S, Horizon Block: &b{1}", 
                           cfg.EdgeBlock, cfg.HorizonBlock);
            Player.Message(p, "Clouds speed: &b{0}%%S, Weather speed: &b{1}%",
                           (cfg.CloudsSpeed / 256f).ToString("F2"),
                           (cfg.WeatherSpeed / 256f).ToString("F2"));
            Player.Message(p, "Weather fade rate: &b{0}%%S, Exponential fog: {1}",
                           (cfg.WeatherFade / 128).ToString("F2"),
                           cfg.ExpFog ? "&aON" : "&cOFF");
        }
        
        class MapInfoData {
            public ushort Width, Height, Length;
            public string Name;
            public long BlockDBEntries = -1;
            public string TerrainUrl, TextureUrl;
            public LevelAccessController Visit, Build;            
            public LevelConfig Config;

            public void FromOnlineLevel(Level lvl) {
                Name = lvl.name;
                Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
                BlockDBEntries = lvl.BlockDB.TotalEntries();
                LoadConfig(lvl.Config);
                
                Visit = lvl.VisitAccess; Build = lvl.BuildAccess;
            }
            
            public void FromOfflineLevel(string name) {
                this.Name = name;
                string path = LevelInfo.MapPath(name);
                Vec3U16 dims = IMapImporter.Formats[0].ReadDimensions(path);
                Width = dims.X; Height = dims.Y; Length = dims.Z;
                BlockDBEntries = BlockDBFile.CountEntries(name);

                path = LevelInfo.FindPropertiesFile(name);
                LevelConfig cfg = new LevelConfig();
                cfg.EdgeLevel = Height / 2; cfg.CloudsHeight = Height + 2;
                LevelConfig.Load(path, cfg);
                LoadConfig(cfg);
                
                Visit = new LevelAccessController(cfg, name, true);
                Build = new LevelAccessController(cfg, name, false);
            }
            
            void LoadConfig(LevelConfig cfg) {
                Config = cfg;              
                TerrainUrl = cfg.Terrain != "" ? cfg.Terrain : ServerConfig.DefaultTerrain;
                TextureUrl = cfg.TexturePack != "" ? cfg.TexturePack : ServerConfig.DefaultTexture;
            }
        }
        
        static string Color(string src) {
            return (src == null || src.Length == 0 || src == "-1") ? "%bnone%e" : "%b" + src + "%e";
        }
        
        public override void Help(Player p)  {
            Player.Message(p, "%T/MapInfo [map]");
            Player.Message(p, "%HDisplay details of [map]");
            Player.Message(p, "%T/MapInfo env [map]");
            Player.Message(p, "%HDisplay environment details of [map]");
        }
    }
}
