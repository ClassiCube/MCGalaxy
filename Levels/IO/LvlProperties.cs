/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;

namespace MCGalaxy.Levels.IO {

    public static class LvlProperties {
        
        public static void Save(Level level, string path) {
            try {
                using (StreamWriter writer = File.CreateText(path + ".properties"))
                    WriteLevelProperties(level, writer);
            } catch (Exception ex) {
                Server.s.Log("Failed to save level properties!");
                Logger.WriteError(ex);
                return;
            }
            
            try {
                using( StreamWriter writer = new StreamWriter(File.Create(path + ".env")))
                    WriteEnvProperties(level, writer);
            } catch (Exception ex) {
                Server.s.Log("Failed to save environment properties");
                Logger.WriteError(ex);
            }
        }
        
        static void WriteLevelProperties(Level level, StreamWriter writer) {
            writer.WriteLine("#Level properties for " + level.name);
            writer.WriteLine("#Drown-time in seconds is [drown time] * 200 / 3 / 1000");
            writer.WriteLine("Theme = " + level.theme);
            writer.WriteLine("Physics = " + level.physics);
            writer.WriteLine("Physics speed = " + level.speedPhysics);
            writer.WriteLine("Physics overload = " + level.overload);
            writer.WriteLine("Finite mode = " + level.finite);
            writer.WriteLine("Animal AI = " + level.ai);
            writer.WriteLine("Edge water = " + level.edgeWater);
            writer.WriteLine("Survival death = " + level.Death);
            writer.WriteLine("Fall = " + level.fall);
            writer.WriteLine("Drown = " + level.drown);
            writer.WriteLine("MOTD = " + level.motd);
            writer.WriteLine("JailX = " + level.jailx);
            writer.WriteLine("JailY = " + level.jaily);
            writer.WriteLine("JailZ = " + level.jailz);
            writer.WriteLine("Unload = " + level.unload);
            writer.WriteLine("WorldChat = " + level.worldChat);
            
            writer.WriteLine("PerBuild = " + GetName(level.permissionbuild));
            writer.WriteLine("PerVisit = " + GetName(level.permissionvisit));
            writer.WriteLine("PerBuildMax = " + GetName(level.perbuildmax));
            writer.WriteLine("PerVisitMax = " + GetName(level.pervisitmax));
            writer.WriteLine("Buildable = " + level.Buildable);
            writer.WriteLine("Deletable = " + level.Deletable);
            
            writer.WriteLine("Guns = " + level.guns);
            writer.WriteLine("LoadOnGoto = " + level.loadOnGoto);
            writer.WriteLine("LeafDecay = " + level.leafDecay);
            writer.WriteLine("RandomFlow = " + level.randomFlow);
            writer.WriteLine("GrowTrees = " + level.growTrees);
            writer.WriteLine("Weather = " + level.weather);
            writer.WriteLine("Texture = " + level.terrainUrl);
            writer.WriteLine("TexturePack = " + level.texturePackUrl);
        }
        
        static string GetName(LevelPermission perm) {
            string permName = Level.PermissionToName(perm).ToLower();
            return Group.Exists(permName) ? permName :
                Level.PermissionToName(LevelPermission.Nobody);
        }
        
        static void WriteEnvProperties(Level level, StreamWriter writer) {
            if(level.CloudColor != null)
                writer.WriteLine("CloudColor = " + level.CloudColor);
            if (level.SkyColor != null)
                writer.WriteLine("SkyColor = " + level.SkyColor);
            if (level.LightColor != null)
                writer.WriteLine("LightColor = " + level.LightColor);
            if (level.ShadowColor != null)
                writer.WriteLine("ShadowColor = " + level.ShadowColor);
            if (level.FogColor != null)
                writer.WriteLine("FogColor = " + level.FogColor);
            
            writer.WriteLine("EdgeLevel = " + level.EdgeLevel);
            writer.WriteLine("CloudsHeight = " + level.CloudsHeight);
            writer.WriteLine("MaxFog = " + level.MaxFogDistance);
            writer.WriteLine("EdgeBlock = " + level.EdgeBlock);
            writer.WriteLine("HorizonBlock = " + level.HorizonBlock);
        }
        
        
        public static void Load(Level level, string path) {
            foreach (string line in File.ReadAllLines(path)) {
                try {
                    if (line[0] == '#') continue;
                    int sepIndex = line.IndexOf(" = ");
                    if (sepIndex < 0) continue;
                    
                    string key = line.Substring(0, sepIndex).ToLower();
                    string value = line.Substring(sepIndex + 3);
                    ParseProperty(level, key, value);
                } catch (Exception e) {
                    Server.ErrorLog(e);
                }
            }
        }
        
        public static void LoadEnv(Level level, string name) {
            string path = "levels/level properties/" + name + ".env";
            if (!File.Exists(path)) return;
            
            foreach (string line in File.ReadAllLines(path)) {
                try {
                    if (line[0] == '#') continue;
                    int sepIndex = line.IndexOf(" = ");
                    if (sepIndex < 0) continue;
                    string value = line.Substring(sepIndex + 3);

                    switch (line.Substring(0, sepIndex).ToLower())
                    {
                            case "cloudcolor": level.CloudColor = value; break;
                            case "fogcolor": level.FogColor = value; break;
                            case "skycolor": level.SkyColor = value; break;
                            case "shadowcolor": level.ShadowColor = value; break;
                            case "lightcolor": level.LightColor = value; break;
                            case "edgeblock": level.EdgeBlock = byte.Parse(value); break;
                            case "edgelevel": level.EdgeLevel = short.Parse(value); break;
                            case "cloudsheight": level.CloudsHeight = short.Parse(value); break;
                            case "maxfog": level.MaxFogDistance = short.Parse(value); break;
                            case "horizonblock": level.HorizonBlock = byte.Parse(value); break;
                    }
                } catch {
                }
            }
        }
        
        static void ParseProperty(Level level, string key, string value) {
            switch (key)
            {
                case "theme":
                    level.theme = value; break;
                case "physics":
                    level.setPhysics(int.Parse(value)); break;
                case "physics speed":
                    level.speedPhysics = int.Parse(value); break;
                case "physics overload":
                    level.overload = int.Parse(value); break;
                case "finite mode":
                    level.finite = bool.Parse(value); break;
                case "animal ai":
                    level.ai = bool.Parse(value); break;
                case "edge water":
                    level.edgeWater = bool.Parse(value); break;
                case "survival death":
                    level.Death = bool.Parse(value); break;
                case "fall":
                    level.fall = int.Parse(value); break;
                case "drown":
                    level.drown = int.Parse(value); break;
                case "motd":
                    level.motd = value; break;
                case "jailx":
                    level.jailx = ushort.Parse(value); break;
                case "jaily":
                    level.jaily = ushort.Parse(value); break;
                case "jailz":
                    level.jailz = ushort.Parse(value); break;
                case "unload":
                    level.unload = bool.Parse(value); break;
                case "worldchat":
                    level.worldChat = bool.Parse(value); break;
                case "perbuild":
                    level.permissionbuild = GetPerm(value); break;
                case "pervisit":
                    level.permissionvisit = GetPerm(value); break;
                case "perbuildmax":
                    level.perbuildmax = GetPerm(value); break;
                case "pervisitmax":
                    level.pervisitmax = GetPerm(value); break;
                case "guns":
                    level.guns = bool.Parse(value); break;
                case "loadongoto":
                    level.loadOnGoto = bool.Parse(value); break;
                case "leafdecay":
                    level.leafDecay = bool.Parse(value); break;
                case "randomflow":
                    level.randomFlow = bool.Parse(value); break;
                case "growtrees":
                    level.growTrees = bool.Parse(value); break;
                case "weather":
                    level.weather = byte.Parse(value); break;
                case "texture":
                    level.terrainUrl = value; break;
                case "texturepack":
                    level.texturePackUrl = value; break; 
                case "buildable":
                    level.Buildable = bool.Parse(value); break;
                case "deletable":
                    level.Deletable = bool.Parse(value); break;
            }
        }
        
        static LevelPermission GetPerm(string value) {
            LevelPermission perm = Level.PermissionFromName(value);
            return perm != LevelPermission.Null ? perm : LevelPermission.Guest;
        }
    }
}