/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
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
using System.Threading;
using MCGalaxy.Config;

namespace MCGalaxy.Games {
    
    public static class ZSConfig {
        
        /// <summary> How precise collision detection is between alive and dead players. (Where 1 block = 32 units) </summary>
        [ConfigInt("zombie-hitbox-precision", "Zombie", 32)]
        public static int HitboxPrecision = 32;
        
        /// <summary> The maximum distance a player is allowed to move between movement packets. </summary>
        [ConfigInt("zombie-maxmove-distance", "Zombie", 50)]
        public static int MaxMoveDistance = 50;
        
        /// <summary> Whether the server's main level should be set to the current level at the end of each round. </summary>
        [ConfigBool("zombie-survival-only-server", "Zombie", false)]
        public static bool SetMainLevel;
        
        /// <summary> Whether zombie survival should start upon server startup. </summary>
        [ConfigBool("zombie-on-server-start", "Zombie", false)]
        public static bool StartImmediately;
        
        /// <summary> Whether maps with '+' in their name are ignored when choosing levels for the next round. </summary>
        [ConfigBool("zombie-ignore-personalworlds", "Zombie", true)]
        public static bool IgnorePersonalWorlds = true;
        
        /// <summary> Whether the current level name should be shown in the heartbeats sent. </summary>
        [ConfigBool("zombie-map-inheartbeat", "Zombie", false)]
        public static bool IncludeMapInHeartbeat = false;

        [ConfigBool("no-respawning-during-zombie", "Zombie", true)]
        public static bool NoRespawn = true;
        [ConfigBool("no-pillaring-during-zombie", "Zombie", true)]
        public static bool NoPillaring = true;
        [ConfigString("zombie-name-while-infected", "Zombie", "", true)]
        public static string ZombieName = "";
        [ConfigString("zombie-model-while-infected", "Zombie", "zombie")]
        public static string ZombieModel = "zombie";
        
        [ConfigInt("zombie-invisibility-duration", "Zombie", 7, 1)]
        public static int InvisibilityDuration = 7;
        [ConfigInt("zombie-invisibility-potions", "Zombie",  7, 1)]
        public static int InvisibilityPotions = 7;
        [ConfigInt("zombie-zinvisibility-duration", "Zombie", 5, 1)]
        public static int ZombieInvisibilityDuration = 5;
        [ConfigInt("zombie-zinvisibility-potions", "Zombie", 4, 1)]
        public static int ZombieInvisibilityPotions = 4;
        [ConfigBool("enable-changing-levels", "Zombie", true)]
        public static bool ChangeLevels = true;
        
        [ConfigString("revive-notime-msg", "Revive",
                      "It's too late. The humans do not have enough time left to make more revive potions.")]
        public static string ReviveNoTimeMessage = "It's too late. The humans do not have enough time left to produce more revive potions.";        
        [ConfigInt("revive-no-time", "Revive", 120, 0)]
        public static int ReviveNoTime = 120;
        
        [ConfigString("revive-fewzombies-msg", "Revive",
                      "There aren't enough zombies for it to be worthwhile to produce revive potions.")]
        public static string ReviveFewZombiesMessage = "There aren't enough zombies for it to be worthwhile to produce revive potions.";        
        [ConfigInt("revive-fewzombies", "Revive", 3, 0)]
        public static int ReviveFewZombies = 3;       
        [ConfigInt("revive-tooslow", "Revive", 60, 0)]
        public static int ReviveTooSlow = 60;      
        [ConfigInt("revive-chance", "Revive", 80, 0, 100)]
        public static int ReviveChance = 80;        
        [ConfigInt("revive-times", "Revive", 1, 0)]
        public static int ReviveTimes = 1;       
        [ConfigString("revive-success", "Revive", "used a revive potion. &aIt was super effective!")]
        public static string ReviveSuccessMessage = "used a revive potion. &aIt was super effective!";
        [ConfigString("revive-failure", "Revive", "tried using a revive potion. &cIt was not very effective..")]
        public static string ReviveFailureMessage = "tried using a revive potion. &cIt was not very effective..";
        
        /// <summary> List of levels that are randomly picked for zombie survival.
        /// If this left blank, then all level files are picked from instead. </summary>
        [ConfigStringList("zombie-levels-list", "Zombie")]
        public static List<string> LevelList = new List<string>();
        
        /// <summary> List of levels that are never picked for zombie survival. </summary>
        [ConfigStringList("zombie-ignores-list", "Zombie")]
        public static List<string> IgnoredLevelList = new List<string>();
        
        public static void SaveSettings() {
            using (StreamWriter w = new StreamWriter("properties/zombiesurvival.properties")) {
                w.WriteLine("#   zombie-on-server-start        = Starts Zombie Survival when server is started.");
                w.WriteLine("#   no-respawning-during-zombie   = Disables respawning (Pressing R) while Zombie is on.");
                w.WriteLine("#   no-pillaring-during-zombie    = Disables pillaring while Zombie Survival is activated.");
                w.WriteLine("#   zombie-name-while-infected    = Sets the zombies name while actived if there is a value.");
                w.WriteLine("#   enable-changing-levels        = After a Zombie Survival round has finished, will change the level it is running on.");
                w.WriteLine("#   zombie-survival-only-server   = EXPERIMENTAL! Makes the server only for Zombie Survival (etc. changes main level)");
                w.WriteLine("#   use-level-list                = Only gets levels for changing levels in Zombie Survival from zombie-level-list.");
                w.WriteLine("#   zombie-level-list             = List of levels for changing levels (Must be comma seperated, no spaces. Must have changing levels and use level list enabled.)");
                w.WriteLine();
                ConfigElement.Serialise(Server.zombieConfig, " options", w, null);
            }
        }
        
        public static void LoadSettings() {
            PropertiesFile.Read("properties/zombiesurvival.properties", ZSLineProcessor);
        }
        
        static void ZSLineProcessor(string key, string value) {
            if (!ConfigElement.Parse(Server.zombieConfig, key, value, null)) {
                Logger.Log(LogType.Warning, "\"{0}\" was not a recognised zombie survival property key.", key);
            }
        }
    }
}
