/*
    Copyright 2015 MCGalaxy
        
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
using System.ComponentModel;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public sealed class ZombieProperties {  
        
        [Description("Whether players are allowed to pillar in zombie survival. " +
                     "Note this can be overriden for specific maps using /ZS set.")]
        [Category("General settings")]
        [DisplayName("Pillaring allowed")]
        public bool Pillaring { get; set; }
        
        
        [Description("Max distance players are allowed to move between packets. (for speedhack detection)")]
        [Category("Other settings")]
        [DisplayName("Max move distance")]
        public float MaxMoveDistance { get; set; }
        
        [Description("Distance between players before they are considered to have 'collided'. (for infecting)")]
        [Category("Other settings")]
        [DisplayName("Hitbox precision")]
        public float HitboxPrecision { get; set; }
        
        [Description("Whether the current map's name is included when a hearbeat is sent. " +
                     "This means it shows up in the server list as: \"Server name (current map name)\"")]
        [Category("Other settings")]
        [DisplayName("Include map in heartbeat")]
        public bool IncludeMapInHeartbeat { get; set; }
        

        [Description("Name to show above infected players. If this is left blank, then the player's name is used instead.")]
        [Category("Zombie settings")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Description("Model to use for infected players. If this is left blank, then 'zombie' model is used.")]
        [Category("Zombie settings")]
        [DisplayName("Model")]
        public string Model { get; set; }
        

        [Description("How many seconds an invisibility potion bought using /buy invisibility lasts.")]
        [Category("Human settings")]
        [DisplayName("Invisibility duration")]
        public int InvisibilityDuration { get; set; }
        
        [Description("Maximum number of invisibility potions a human is allowed to buy in a round.")]
        [Category("Human settings")]
        [DisplayName("Invisibility potions")]        
        public int InvisibilityPotions { get; set; }
        
        [Description("How many seconds an invisibility potion bought using /buy zinvisibility lasts.")]
        [Category("Zombie settings")]
        [DisplayName("Invisibility duration")]
        public int ZInvisibilityDuration { get; set; }
        
        [Description("Maximum number of invisibility potions a zombie is allowed to buy in a round.")]
        [Category("Zombie settings")]
        [DisplayName("Invisibility potions")]        
        public int ZInvisibilityPotions { get; set; }
        
        
        [Description("The percentage chance that a revive potion will actually disinfect a zombie.")]
        [Category("Revive settings")]
        [DisplayName("Chance")]
        public int Chance { get; set; }  
        
        [Description("The minimum number of seconds left in a round, below which /buy revive will not work.")]
        [Category("Revive settings")]
        [DisplayName("Insufficient time")]
        public int InsufficientTime { get; set; }
        
        [Description("Message shown when using /buy revive and the seconds left in a round is less than 'InsufficientTime'.")]
        [Category("Revive settings")]
        [DisplayName("Insufficient time message")]
        public string InsufficientTimeMessage { get; set; }
        
        [Description("The maximum number of seconds after a human is infected, after which /buy revive will not work.")]
        [Category("Revive settings")]
        [DisplayName("Expiry time")]
        public int ExpiryTime { get; set; }
        
        public void LoadFromServer() {
            ZSConfig cfg = ZSGame.Config;
            
            Pillaring = !cfg.NoPillaring;          
            MaxMoveDistance = cfg.MaxMoveDist;
            HitboxPrecision = cfg.HitboxDist;
            IncludeMapInHeartbeat = cfg.MapInHeartbeat;
            
            Name = cfg.ZombieName;
            Model = cfg.ZombieModel;
            InvisibilityDuration = cfg.InvisibilityDuration;
            InvisibilityPotions = cfg.InvisibilityPotions;
            ZInvisibilityDuration = cfg.ZombieInvisibilityDuration;
            ZInvisibilityPotions = cfg.ZombieInvisibilityPotions;
            
            Chance = cfg.ReviveChance;
            InsufficientTime = cfg.ReviveNoTime;
            InsufficientTimeMessage = cfg.ReviveNoTimeMessage;
            ExpiryTime = cfg.ReviveTooSlow;
        }
        
        public void ApplyToServer() {
            ZSConfig cfg = ZSGame.Config;

            cfg.NoPillaring = !Pillaring;            
            cfg.MaxMoveDist = MaxMoveDistance;
            cfg.HitboxDist = HitboxPrecision;
            cfg.MapInHeartbeat = IncludeMapInHeartbeat;
            
            cfg.ZombieName = Name.Trim();
            cfg.ZombieModel = Model.Trim();
            if (cfg.ZombieModel.Length == 0)
                cfg.ZombieModel = "zombie";
            cfg.InvisibilityDuration = InvisibilityDuration;
            cfg.InvisibilityPotions = InvisibilityPotions;
            cfg.ZombieInvisibilityDuration = ZInvisibilityDuration;
            cfg.ZombieInvisibilityPotions = ZInvisibilityPotions; 
            
            cfg.ReviveChance = Chance;
            cfg.ReviveNoTime = InsufficientTime;
            cfg.ReviveNoTimeMessage = InsufficientTimeMessage;
            cfg.ReviveTooSlow = ExpiryTime;
        }
    }
}
