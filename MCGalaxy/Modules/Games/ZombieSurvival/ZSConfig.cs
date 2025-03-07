/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
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

namespace MCGalaxy.Modules.Games.ZS
{    
    public sealed class ZSConfig : RoundsGameConfig 
    {
        [ConfigInt("infection-start-countdown", "Round", 30, 0)]
        public int InfectionCountdown = 30; 
        
        [ConfigFloat("zombie-hitbox-distance", "Collisions", 1f)]
        public float HitboxDist = 1f;
        [ConfigFloat("zombie-max-move-distance", "Collisions", 1.5625f)]
        public float MaxMoveDist = 1.5625f;
        [ConfigInt("collisions-check-interval", "Collisions", 150, 20, 2000)]
        public int CollisionsCheckInterval = 150;
        
        [ConfigString("human-tablist-group", "Human", "&fHumans")]
        public string HumanTabListGroup = "&fHumans";
        [ConfigBool("infect-upon-death", "Human", true)]
        public bool InfectUponDeath = true;

        [ConfigBool("no-pillaring-during-zombie", "Zombie", true)]
        public bool NoPillaring = true;
        [ConfigString("zombie-name-while-infected", "Zombie", "", true)]
        public string ZombieName = "";
        [ConfigString("zombie-model-while-infected", "Zombie", "zombie")]
        public string ZombieModel = "zombie";
        [ConfigString("zombie-tablist-group", "Zombie", "&cZombies")]
        public string ZombieTabListGroup = "&cZombies";       
        
        [ConfigInt("zombie-invisibility-duration", "Zombie", 7, 1)]
        public int InvisibilityDuration = 7;
        [ConfigInt("zombie-invisibility-potions", "Zombie",  7, 1)]
        public int InvisibilityPotions = 7;
        [ConfigInt("zombie-zinvisibility-duration", "Zombie", 5, 1)]
        public int ZombieInvisibilityDuration = 5;
        [ConfigInt("zombie-zinvisibility-potions", "Zombie", 4, 1)]
        public int ZombieInvisibilityPotions = 4;
        
        [ConfigString("revive-notime-msg", "Revive",
                      "It's too late. The humans do not have enough time left to make more revive potions.")]
        public string ReviveNoTimeMessage = "It's too late. The humans do not have enough time left to produce more revive potions.";
        [ConfigInt("revive-no-time", "Revive", 120, 0)]
        public int ReviveNoTime = 120;
        
        [ConfigString("revive-fewzombies-msg", "Revive",
                      "There aren't enough zombies for it to be worthwhile to produce revive potions.")]
        public string ReviveFewZombiesMessage = "There aren't enough zombies for it to be worthwhile to produce revive potions.";
        [ConfigInt("revive-fewzombies", "Revive", 3, 0)]
        public int ReviveFewZombies = 3;
        [ConfigInt("revive-tooslow", "Revive", 60, 0)]
        public int ReviveTooSlow = 60;
        [ConfigInt("revive-chance", "Revive", 80, 0, 100)]
        public int ReviveChance = 80;
        [ConfigInt("revive-times", "Revive", 1, 0)]
        public int ReviveTimes = 1;
        
        [ConfigInt("zombies-win-reward-min", "Zombie rewards", 1, 0)]
        public int ZombiesRewardMin = 1;
        [ConfigInt("zombies-win-reward-max", "Zombie rewards", 5, 0)]
        public int ZombiesRewardMax = 5;
        [ConfigInt("zombies-win-infected-multiplier", "Zombie rewards", 1, 0)]
        public int ZombiesRewardMultiplier = 1;
        
        [ConfigInt("sole-human-reward-min", "Human rewards",  5, 0)]
        public int SoleHumanRewardMin =  5;
        [ConfigInt("sole-human-reward-max", "Human rewards", 10, 0)]
        public int SoleHumanRewardMax = 10;
        [ConfigInt("humans-win-reward-min", "Human rewards", 2, 0)]
        public int HumansRewardMin = 2;
        [ConfigInt("humans-win-reward-max", "Human rewards", 6, 0)]
        public int HumansRewardMax = 6;
        
        static ConfigElement[] cfg;
        public override bool AllowAutoload { get { return true; } }
        protected override string GameName { get { return "Zombie Survival"; } }
        
        public override void Save() {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(ZSConfig));
            
            using (StreamWriter w = FileIO.CreateGuarded(Path)) 
            {
                w.WriteLine("#   no-pillaring-during-zombie    = Disables pillaring while Zombie Survival is activated.");
                w.WriteLine("#   zombie-name-while-infected    = Sets the zombies name while actived if there is a value.");
                w.WriteLine();
                ConfigElement.Serialise(cfg, w, this);
            }
        }
        
        public override void Load() {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(ZSConfig));
            PropertiesFile.Read(Path, ProcessConfigLine);
        }
        
        void ProcessConfigLine(string key, string value) {
            // backwards compatibility
            if (key.CaselessEq("zombie-levels-list")) {
                Maps = new List<string>(value.SplitComma());
            } else {
                ConfigElement.Parse(cfg, this, key, value);
            }
        }

        public const string InfectZombiePlaceholder = "<zombie>";
        public const string InfectHumanPlaceholder = "<human>";
        const string InfectZombieObjectPlaceholder = "<zobject>";
        const string InfectHumanObjectPlaceholder = "<hobject>";

        static string[] defMessages = new string[] { "<zombie> WIKIWOO'D <human>", "<zombie> stuck <zobject> teeth into <human>",
            "<zombie> licked <human>'s brain ", "<zombie> danubed <human>", "<zombie> made <human> meet <hobject> maker", "<zombie> tripped <human>",
            "<zombie> made some zombie babies with <human>", "<zombie> made <human> see the dark side", "<zombie> tweeted <human>",
            "<zombie> made <human> open source", "<zombie> infected <human>", "<zombie> iDotted <human>", "<human> got nommed on",
            "<zombie> transplanted <human>'s living brain" };

        public static string FormatInfectMessage(string infectMsg, Player pKiller, Player pAlive) {
            return infectMsg
                .Replace(InfectZombiePlaceholder, "&c" + pKiller.DisplayName + "&S")
                .Replace(InfectHumanPlaceholder, pAlive.ColoredName + "&S")
                .Replace(InfectZombieObjectPlaceholder, pKiller.pronouns.Object)
                .Replace(InfectHumanObjectPlaceholder, pAlive.pronouns.Object);
        }

        public static List<string> LoadInfectMessages() {
            List<string> msgs = new List<string>();
            try {
                if (!File.Exists("text/infectmessages.txt")) {
                    File.WriteAllLines("text/infectmessages.txt", defMessages);
                }
                msgs = Utils.ReadAllLinesList("text/infectmessages.txt");
            } catch (Exception ex) {
                Logger.LogError("Error loading infect messages list", ex);
            }
            
            if (msgs.Count == 0) msgs = new List<string>(defMessages);
            return ConvertInfectMessages(msgs);
        }
        
        static string InfectPath(string name) { return "text/infect/" + name.ToLower() + ".txt"; }
        public static List<string> LoadPlayerInfectMessages(string name) {
            string path = InfectPath(name);
            if (!File.Exists(path)) return null;

            List<string> msgs = Utils.ReadAllLinesList(path);
            return ConvertInfectMessages(msgs);
        }
        
        public static void AppendPlayerInfectMessage(string name, string msg) {
            if (!Directory.Exists("text/infect"))
                Directory.CreateDirectory("text/infect");
            
            string path = InfectPath(name);
            File.AppendAllText(path, msg + Environment.NewLine);
        }

        static List<string> ConvertInfectMessages(List<string> messages) {
            for (int i = 0; i < messages.Count; i++)
            {
                messages[i] = messages[i]
                                .Replace("{0}", InfectZombiePlaceholder)
                                .Replace("{1}", InfectHumanPlaceholder);
            }
            return messages;
        }
    }
}
