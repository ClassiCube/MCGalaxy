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
                using (StreamWriter writer = new StreamWriter(path))
                    WriteLevelProperties(level, writer);
            } catch (Exception ex) {
                Logger.Log(LogType.Warning, "Failed to save level properties!");
                Logger.LogError(ex);
                return;
            }
        }
        
        static void WriteLevelProperties(Level level, StreamWriter w) {
            w.WriteLine("#Level properties for " + level.name);
            w.WriteLine("#Drown-time in seconds is [drown time] * 200 / 3 / 1000");
            w.WriteLine("Physics = " + level.physics);        
            ConfigElement.Serialise(Server.levelConfig, " settings", w, level);
        }
        
        
        public static void Load(Level level, string path) {
            PropertiesFile.Read(path, ref level, PropLineProcessor);
        }
        
        public static void LoadEnv(Level level) {
            string path = "levels/level properties/" + level.MapName + ".env";
            PropertiesFile.Read(path, ref level, EnvLineProcessor);
        }
        
        static void EnvLineProcessor(string key, string value, ref Level level) {
            switch (key.ToLower()) {
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
        }
        
        static void PropLineProcessor(string key, string value, ref Level level) {
            switch (key.ToLower()) {
                case "physics":
                    level.setPhysics(int.Parse(value)); break;
                default:
                    if (!ConfigElement.Parse(Server.levelConfig, key, value, level)) {
                        Logger.Log(LogType.Warning, "\"{0}\" was not a recognised level property key.", key);
                    }
                    break;
            }
        }
    }
}