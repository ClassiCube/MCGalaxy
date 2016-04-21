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
using System.IO;
using MCGalaxy.Levels.IO;

namespace MCGalaxy.Commands {
    
    public sealed class CmdMapInfo : Command {
        
        public override string name { get { return "mapinfo"; } }
        public override string shortcut { get { return "winfo"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("worldinfo") }; }
        }
        
        public override void Use(Player p, string message) {
            Level lvl = message == "" ? p.level : LevelInfo.Find(message);
            MapInfoData data = new MapInfoData();
            if (lvl != null) {
                data.FromOnlineLevel(lvl);
            } else if (LevelInfo.ExistsOffline(message)) {
                data.FromOfflineLevel(message);
            } else {
                Player.SendMessage(p, "Could not find specified level."); return;
            }
            ShowNormal(p, data);
        }
        
        void ShowNormal(Player p, MapInfoData data) {
            Player.SendMessage(p, "&b" + data.name + "%S: Width=" + data.Width + " Height=" + data.Height + " Depth=" + data.Length);
            string physicsState = CmdPhysics.states[data.physics];
            Player.SendMessage(p, "Physics are " + physicsState + Server.DefaultColor + " on &b" + data.name);
            ShowPermissions(p, data);

            string gunStatus = data.guns ? "&aonline" : "&coffline";
            Player.SendMessage(p, "&cGuns &eare " + gunStatus + " &eon " + data.name + ".");

            if (Directory.Exists(Server.backupLocation + "/" + data.name)) {
                int latestBackup = Directory.GetDirectories(Server.backupLocation + "/" + data.name).Length;
                DateTime time = Directory.GetCreationTime(LevelInfo.BackupPath(data.name, latestBackup.ToString()));
                Player.SendMessage(p, "Latest backup: &a" + latestBackup + " %Sat &a" + time.ToString("yyyy-MM-dd HH:mm:ss"));
            } else {
                Player.SendMessage(p, "No backups for this map exist yet.");
            }
            ShowEnv(p, data);
        }
        
        void ShowPermissions(Player p, MapInfoData data) {
            Player.SendMessage(p, "Build rank = " + Group.findPerm(data.build).color + Group.findPerm(data.build).trueName +
                               " %S: Visit rank = " + Group.findPerm(data.visit).color + Group.findPerm(data.visit).trueName);
            Player.SendMessage(p, "BuildMax Rank = " + Group.findPerm(data.buildmax).color + Group.findPerm(data.buildmax).trueName +
                               " %S: VisitMax Rank = " + Group.findPerm(data.visitmax).color + Group.findPerm(data.visitmax).trueName);
        }
        
        void ShowEnv(Player p, MapInfoData data) {
            if (data.terrainUrl != "")
                Player.SendMessage(p, "Texture: %b" + data.terrainUrl);
            else
                Player.SendMessage(p, "No custom texture set for this map.");
            
            if (data.textureUrl != "")
                Player.SendMessage(p, "Texture pack: %b" + data.textureUrl);
            else
                Player.SendMessage(p, "No custom texture pack set for this map.");
            
            const string format = "Colors: Fog {0}, Sky {1}, Clouds {2}, Sunlight {3}, Shadowlight {4}";
            Player.SendMessage(p, String.Format(format, Color(data.Fog), Color(data.Sky), Color(data.Clouds),
                                                Color(data.Light), Color(data.Shadow)));
            
            Player.SendMessage(p, "Water Level: %b" + data.EdgeLevel + "%S, Clouds height: %b" + data.CloudsHeight
                               + "%S, Max fog distance: %b" + data.MaxFogDistance);
            Player.SendMessage(p, "Edge Block: %b" + data.EdgeBlock + "%S, Horizon Block: %b" + data.HorizonBlock);
        }
        
        class MapInfoData {
            
            public ushort Width, Height, Length;
            public int physics;
            public LevelPermission visit, build, visitmax, buildmax;
            public bool guns;
            public string name, terrainUrl, textureUrl;
            public string Fog, Sky, Clouds, Light, Shadow;
            public short EdgeLevel, CloudsHeight, MaxFogDistance;
            public byte EdgeBlock = Block.blackrock, HorizonBlock = Block.water;

            public void FromOnlineLevel(Level lvl) {
                name = lvl.name;
                Width = lvl.Width; Height = lvl.Height; Length = lvl.Length;
                physics = lvl.physics; guns = lvl.guns;
                visit = lvl.permissionvisit; build = lvl.permissionbuild;
                visitmax = lvl.pervisitmax; buildmax = lvl.perbuildmax;
                
                Fog = lvl.FogColor; Sky = lvl.SkyColor; Clouds = lvl.CloudColor;
                Light = lvl.LightColor; Shadow = lvl.ShadowColor;
                EdgeLevel = lvl.EdgeLevel; CloudsHeight = lvl.CloudsHeight;
                MaxFogDistance = lvl.MaxFogDistance;
                EdgeBlock = lvl.EdgeBlock; HorizonBlock = lvl.HorizonBlock;
                
                terrainUrl = lvl.terrainUrl != "" ?
                    lvl.terrainUrl : Server.defaultTerrainUrl;
                textureUrl = lvl.texturePackUrl != "" ?
                    lvl.texturePackUrl : Server.defaultTextureUrl;
            }
            
            public void FromOfflineLevel(string name) {
                this.name = name;
                LvlFile.LoadDimensions(LevelInfo.LevelPath(name),
                                       out Width, out Height, out Length);
                string path = LevelInfo.GetPropertiesPath(name);
                Server.s.Log(path);
                if (path != null)
                    PropertiesFile.Read(path, ParseProperty, '=');
                
                path = "levels/level properties/" + name + ".env";
                Server.s.Log(path);
                if (File.Exists(path))
                    PropertiesFile.Read(path, ParseEnv, '=');
            }
            
            void ParseProperty(string key, string value) {
                switch (key.ToLower()) {
                    case "physics": physics = int.Parse(value); break;
                    case "perbuild": build = GetPerm(value); break;
                    case "pervisit": visit = GetPerm(value); break;
                    case "perbuildmax": buildmax = GetPerm(value); break;
                    case "pervisitmax": visitmax = GetPerm(value); break;
                    case "guns": guns = bool.Parse(value); break;
                    case "texture": terrainUrl = value; break;
                    case "texturepack": textureUrl = value; break;
                }
            }
            
            void ParseEnv(string key, string value) {
                switch (key.ToLower()) {
                    case "cloudcolor": Clouds = value; break;
                    case "fogcolor": Fog = value; break;
                    case "skycolor": Sky = value; break;
                    case "shadowcolor": Shadow = value; break;
                    case "lightcolor": Light = value; break;
                    case "edgeblock": EdgeBlock = byte.Parse(value); break;
                    case "edgelevel": EdgeLevel = short.Parse(value); break;
                    case "cloudsheight": CloudsHeight = short.Parse(value); break;
                    case "maxfog": MaxFogDistance = short.Parse(value); break;
                    case "horizonblock": HorizonBlock = byte.Parse(value); break;
                }
            }
            
            static LevelPermission GetPerm(string value) {
                LevelPermission perm = Level.PermissionFromName(value);
                return perm != LevelPermission.Null ? perm : LevelPermission.Guest;
            }
        }
        
        static string Color(string src) {
            return (src == null || src == "" || src == "-1") ? "%bnone%e" : "%b" + src + "%e";
        }
        
        public override void Help(Player p)  {
            Player.SendMessage(p, "/mapinfo <map> - Display details of <map>");
        }
    }
}
