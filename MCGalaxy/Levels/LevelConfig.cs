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
using MCGalaxy.Config;
using MCGalaxy.Games;

namespace MCGalaxy {
    public sealed class LevelConfig {

        [ConfigString("MOTD", "General", null, "ignore", true, null, 128)]
        public string MOTD = "ignore";
        [ConfigBool("LoadOnGoto", "General", null, true)]
        public bool LoadOnGoto = true;
        [ConfigString("Theme", "General", null, "Normal", true)]
        public string Theme = "Normal";
        [ConfigBool("Unload", "General", null, true)]
        public bool AutoUnload = true;
        /// <summary> true if this map may see server-wide chat, false if this map has level-only/isolated chat </summary>
        [ConfigBool("WorldChat", "General", null, true)]
        public bool ServerWideChat = true;

        [ConfigBool("UseBlockDB", "Other", null, true)]
        public bool UseBlockDB = true;
        [ConfigInt("LoadDelay", "Other", null, 0, 0, 2000)]
        public int LoadDelay = 0;
        
        public byte jailrotx, jailroty;
        [ConfigInt("JailX", "Jail", null, 0, 0, 65535)]
        public int JailX;
        [ConfigInt("JailY", "Jail", null, 0, 0, 65535)]
        public int JailY;
        [ConfigInt("JailZ", "Jail", null, 0, 0, 65535)]
        public int JailZ;       

        // Environment settings
        [ConfigInt("Weather", "Env", null, 0, 0, 2)]
        public int Weather;       
        [ConfigString("Texture", "Env", null, "", true, null, NetUtils.StringSize)]
        public string Terrain = "";
        [ConfigString("TexturePack", "Env", null, "", true, null, NetUtils.StringSize)]
        public string TexturePack = "";
        /// <summary> Color of the clouds (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("CloudColor", "Env", null, "", true)]
        public string CloudColor = "";
        /// <summary> Color of the fog (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("FogColor", "Env", null, "", true)]
        public string FogColor = "";
        /// <summary> Color of the sky (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("SkyColor", "Env", null, "", true)]
        public string SkyColor = "";
        /// <summary> Color of the blocks in shadows (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("ShadowColor", "Env", null, "", true)]
        public string ShadowColor = "";
        /// <summary> Color of the blocks in the light (RGB packed into an int). Set to -1 to use client defaults. </summary>
        [ConfigString("LightColor", "Env", null, "", true)]
        public string LightColor = "";
        
        /// <summary> Elevation of the "ocean" that surrounds maps. Default is map height / 2. </summary>
        [ConfigInt("EdgeLevel", "Env", null, -1, short.MinValue, short.MaxValue)]
        public int EdgeLevel;
        /// <summary> Offset of the "bedrock" that surrounds map sides from edge level. Default is -2. </summary>
        [ConfigInt("SidesOffset", "Env", null, -2, short.MinValue, short.MaxValue)]
        public int SidesOffset = -2;
        /// <summary> Elevation of the clouds. Default is map height + 2. </summary>
        [ConfigInt("CloudsHeight", "Env", null, -1, short.MinValue, short.MaxValue)]
        public int CloudsHeight;
        
        /// <summary> Max fog distance the client can see. Default is 0, means use client-side defined max fog distance. </summary>
        [ConfigInt("MaxFog", "Env", null, 0, short.MinValue, short.MaxValue)]
        public int MaxFogDistance;
        /// <summary> Clouds speed, in units of 256ths. Default is 256 (1 speed). </summary>
        [ConfigInt("clouds-speed", "Env", null, 256, short.MinValue, short.MaxValue)]
        public int CloudsSpeed = 256;
        /// <summary> Weather speed, in units of 256ths. Default is 256 (1 speed). </summary>
        [ConfigInt("weather-speed", "Env", null, 256, short.MinValue, short.MaxValue)]
        public int WeatherSpeed = 256;
        /// <summary> Weather fade, in units of 256ths. Default is 256 (1 speed). </summary>
        [ConfigInt("weather-fade", "Env", null, 128, short.MinValue, short.MaxValue)]
        public int WeatherFade = 128;
        /// <summary> The block which will be displayed on the horizon. </summary>
        [ConfigInt("HorizonBlock", "Env", null, Block.water, 0, 255)]
        public int HorizonBlock = Block.water;
        /// <summary> The block which will be displayed on the edge of the map. </summary>
        [ConfigInt("EdgeBlock", "Env", null, Block.blackrock, 0, 255)]
        public int EdgeBlock = Block.blackrock;
         /// <summary> Whether exponential fog mode is used client-side. </summary>
        [ConfigBool("ExpFog", "Env", null, false)]
        public bool ExpFog;
        
        // Permission settings
        [ConfigString("RealmOwner", "Permissions", null, "", true)]
        public string RealmOwner = "";
        [ConfigBool("Buildable", "Permissions", null, true)]
        public bool Buildable = true;
        [ConfigBool("Deletable", "Permissions", null, true)]
        public bool Deletable = true;

        [ConfigPerm("PerVisit", "Permissions", null, LevelPermission.Guest)]
        public LevelPermission VisitMin = LevelPermission.Guest;
        [ConfigPerm("PerVisitMax", "Permissions", null, LevelPermission.Nobody)]
        public LevelPermission VisitMax = LevelPermission.Nobody;
        [ConfigPerm("PerBuild", "Permissions", null, LevelPermission.Guest)]
        public LevelPermission BuildMin = LevelPermission.Guest;
        [ConfigPerm("PerBuildMax", "Permissions", null, LevelPermission.Nobody)]
        public LevelPermission BuildMax = LevelPermission.Nobody;
   
        // Other blacklists/whitelists
        [ConfigStringList("VisitWhitelist", "Permissions", null)]
        public List<string> VisitWhitelist = new List<string>();
        [ConfigStringList("VisitBlacklist", "Permissions", null)]
        public List<string> VisitBlacklist = new List<string>();
        [ConfigStringList("BuildWhitelist", "Permissions", null)]
        public List<string> BuildWhitelist = new List<string>();
        [ConfigStringList("BuildBlacklist", "Permissions", null)]
        public List<string> BuildBlacklist = new List<string>();

        // Physics settings
        [ConfigInt("Physics", "Physics", null, 0, 0, 5)]
        public int Physics;
        [ConfigInt("Physics overload", "Physics", null, 250)]
        public int PhysicsOverload = 1500;
        [ConfigInt("Physics speed", "Physics", null, 250)]
        public int PhysicsSpeed = 250;
        [ConfigBool("RandomFlow", "Physics", null, true)]
        public bool RandomFlow = true;
        [ConfigBool("LeafDecay", "Physics", null, false)]
        public bool LeafDecay;
        [ConfigBool("Finite mode", "Physics", null, false)]
        public bool FiniteLiquids;
        [ConfigBool("GrowTrees", "Physics", null, false)]
        public bool GrowTrees;
        [ConfigBool("Animal AI", "Physics", null, true)]
        public bool AnimalHuntAI = true;
        [ConfigBool("GrassGrowth", "Physics", null, true)]
        public bool GrassGrow = true;
        [ConfigString("TreeType", "Physics", null, "fern", false)]
        public string TreeType = "fern";
        
        // Survival settings
        [ConfigInt("Drown", "Survival", null, 70)]
        public int DrownTime = 70;
        [ConfigBool("Edge water", "Survival", null, true)]
        public bool EdgeWater;
        [ConfigInt("Fall", "Survival", null, 9)]
        public int FallHeight = 9;
        [ConfigBool("Guns", "Survival", null, false)]
        public bool Guns = false;
        [ConfigBool("Survival death", "Survival", null, false)]
        public bool SurvivalDeath;
        [ConfigBool("Killer blocks", "Survival", null, true)]
        public bool KillerBlocks = true;       
        
        // Games settings
        [ConfigInt("Likes", "Game", null, 0)]
        public int Likes;
        [ConfigInt("Dislikes", "Game", null, 0)]
        public int Dislikes;
        [ConfigString("Authors", "Game", null, "", true)]
        public string Authors = "";
        [ConfigBool("Pillaring", "Game", null, false)]
        public bool Pillaring = !ZombieGameProps.NoPillaring;
        
        [ConfigEnum("BuildType", "Game", null, BuildType.Normal, typeof(BuildType))]
        public BuildType BuildType = BuildType.Normal;
        
        [ConfigInt("MinRoundTime", "Game", null, 4)]
        public int MinRoundTime = 4;
        [ConfigInt("MaxRoundTime", "Game", null, 7)]
        public int MaxRoundTime = 7;
        [ConfigBool("DrawingAllowed", "Game", null, true)]
        public bool DrawingAllowed = true;
        [ConfigInt("RoundsPlayed", "Game", null, 0)]
        public int RoundsPlayed = 0;
        [ConfigInt("RoundsHumanWon", "Game", null, 0)]
        public int RoundsHumanWon = 0;
        
        
		public static void Load(string path, LevelConfig config) {
            PropertiesFile.Read(path, ref config, LineProcessor);
        }
        
        static void LineProcessor(string key, string value, ref LevelConfig config) {
            if (!ConfigElement.Parse(Server.levelConfig, key, value, config)) {
                Logger.Log(LogType.Warning, "\"{0}\" was not a recognised level property key.", key);
            }
        }
        
        public static void Save(string path, LevelConfig config, string lvlname) {
            try {
                using (StreamWriter w = new StreamWriter(path)) {
                    w.WriteLine("#Level properties for " + lvlname);
                    w.WriteLine("#Drown-time in seconds is [drown time] * 200 / 3 / 1000");
                    ConfigElement.Serialise(Server.levelConfig, " settings", w, config);
                }
            } catch (Exception ex) {
                Logger.Log(LogType.Warning, "Failed to save level properties!");
                Logger.LogError(ex);
            }
        }
    }
}