/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.Gui {

    public sealed class ZombieSettings {
        
        [Description("Whether at the end of each round, different levels are randomly picked for the next round. " +
                     "You should generallly leave this as true.")]
        [Category("Levels settings")]
        public bool ChangeLevels { get; set; }
        
        [Description("Comma separated list of levels that are never used for zombie survival. (e.g. main,spawn)")]
        [Category("Levels settings")]
        public string IgnoredLevelsList { get; set; }
        
        [Description("Comma separated list of levels to use for zombie survival. (e.g. map1,map2,map3)")]
        [Category("Levels settings")]
        public string LevelsList { get; set; }
        
        [Description("Whether changes made to a map during a round of zombie survival are permanently saved. " +
                     "It is HIGHLY recommended that you leave this as false.")]
        [Category("Levels settings")]
        public bool SaveZombieLevelChanges { get; set; }
        
        [Description("Whether the specified list of levels should be used. If this is false, then all levels are used.")]
        [Category("Levels settings")]
        public bool UseLevelList { get; set; }
        
        
        [Description("Name to show above infected players. If this is left blank, then the player's name is used instead.")]
        [Category("General settings")]
        public string InfectedName { get; set; }
        
        [Description("Whether players are allowed to pillar in zombie survival. " +
                     "Note this can be overriden for specific maps using /mset.")]
        [Category("General settings")]
        public bool Pillaring { get; set; }
        
        [Description("Whether players are allowed to use /spawn in zombie survival. " +
                     "You should generallly leave this as false.")]
        [Category("Levels settings")]
        public bool Respawning { get; set; }
        
        [Description("Whether the main/spawn level is always set to the current level of zombie survival. " +
                     "You should set this to true if the server is purely for zombie survival. ")]
        [Category("General settings")]
        public bool SetMainLevel { get; set; }
        
        [Description("Whether zombie survival should start when the server starts.")]
        [Category("General settings")]
        public bool StartImmediately { get; set; }
        
        public void LoadFromServer() {
            ChangeLevels = Server.zombie.ChangeLevels;
            // TODO: Ignored level list
            LevelsList = String.Join(",", Server.zombie.LevelList);
            // TODO: Save level zombie changes
            UseLevelList = Server.zombie.UseLevelList;
            
            InfectedName = Server.zombie.ZombieName;
            Pillaring = !Server.zombie.noPillaring;
            Respawning = !Server.zombie.noRespawn;
            SetMainLevel = Server.ZombieOnlyServer;
            StartImmediately = Server.startZombieModeOnStartup;
        }
        
        public void ApplyToServer() {
            Server.zombie.UseLevelList = UseLevelList;
            // TODO: Ignored level list
            string list = LevelsList.Replace(" ", "");
            Server.zombie.LevelList = new List<string>(list.Split(','));
            // TODO: Save level zombie changes
            Server.zombie.UseLevelList = UseLevelList;
            
            Server.zombie.ZombieName = InfectedName;
            Server.zombie.noPillaring = !Pillaring;
            Server.zombie.noRespawn = !Respawning;
            Server.ZombieOnlyServer = SetMainLevel;
            Server.startZombieModeOnStartup = StartImmediately;
        }
    }
}
