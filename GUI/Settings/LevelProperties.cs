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
using System.ComponentModel;

namespace MCGalaxy.Gui {
    public sealed class LevelProperties {     
        readonly Level lvl;
        readonly LevelConfig cfg;
        
        public LevelProperties(Level lvl) {
            this.lvl = lvl;
            cfg = lvl.Config;
        }
        
        [Description("Message shown to users when they join this level.")]
        [Category("General")]
        [DisplayName("MOTD")]
        public string MOTD { get { return cfg.motd; } set { cfg.motd = value == "" ? "ignore" : value; } }
        
        [Description("Whether this map automatically loads when the server is started.")]
        [Category("General")]
        [DisplayName("Load on startup")]
        public bool AutoLoad { get { return GetAutoload(); } set { SetAutoload(value); } }
        
        [Description("Whether if a player uses /goto on this map and this map is not " +
                     "loaded, the server then automatically loads this map.")]
        [Category("General")]
        [DisplayName("Load on /goto")]
        public bool LoadOnGoto { get { return cfg.loadOnGoto; } set { cfg.loadOnGoto = value; } }
        
        [Description("Whether this level should be automatically unloaded " +
                     "if there are no players on it anymore.")]
        [Category("General")]
        [DisplayName("Unload when empty")]
        public bool UnloadWhenEmpty { get { return cfg.unload; } set { cfg.unload = value; } }
        
        [Description("Whether chat on this level is only shown to players in the level," +
                     "and not to any other levels or IRC (if enabled).")]
        [Category("General")]
        [DisplayName("Level only chat")]
        public bool LevelOnlyChat { get { return !cfg.worldChat; } set { cfg.worldChat = !value; } }
        
        [Description("If this is 0 then physics is disabled. If this is 5 then only door physics are used." +
                     "Values between 1-4 are varying levels of physics.")]
        [Category("General")]
        [DisplayName("Physics level")]
        public int PhysicsLevel { get { return lvl.physics; } 
            set { lvl.physics = Math.Min(Math.Max(value, 0), 5); } 
        }
        

        [Description("If physics is active, whether liquids spread infinitely or not.")]
        [Category("Physics")]
        [DisplayName("Finite liquids")]
        public bool FiniteLiquids { get { return cfg.finite; } set { cfg.finite = value; } }
        
        [Description("If physics is active, whether saplings should randomly grow into trees.")]
        [Category("Physics")]
        [DisplayName("Tree growing")]
        public bool TreeGrowing { get { return cfg.growTrees; } set { cfg.growTrees = value; } }
        
        [Description("If physics is active, whether leaf blocks not connected to trees should decay.")]
        [Category("Physics")]
        [DisplayName("Leaf decay")]
        public bool LeafDecay { get { return cfg.leafDecay; } set { cfg.leafDecay = value; } }
        
        [Description("If physics is active, whether spreading liquids randomly pick a direction to flow in." +
                     "Otherwise, spreads in all directions.")]
        [Category("Physics")]
        [DisplayName("Random flow")]
        public bool RandomFlow { get { return cfg.randomFlow; } set { cfg.randomFlow = value; } }
        
        [Description("If physics is active, water flows from the map edges into the world.")]
        [Category("Physics")]
        [DisplayName("Edge water")]
        public bool EdgeWater { get { return cfg.edgeWater; } set { cfg.edgeWater = value; } }
        
        [Description("If physics is active, whether 'animal' (bird, fish, etc) blocks " +
                     "should move towards the nearest player.")]
        [Category("Physics")]
        [DisplayName("Animal hunt AI")]
        public bool AnimalHuntAI { get { return cfg.ai; } set { cfg.ai = value; } }

        [Description("Whether dirt grows into grass.")]
        [Category("Physics")]
        [DisplayName("Grass growth")]
        public bool GrassGrowth { get { return cfg.GrassGrow; } set { cfg.GrassGrow = value; } }

        [Description("Whether gun usage is allowed.")]
        [Category("Survival")]
        [DisplayName("Guns")]
        public bool Guns { get { return cfg.guns; } set { cfg.guns = value; } }
        
        [Description("Whether certain blocks can kill players.")]
        [Category("Survival")]
        [DisplayName("Killer blocks")]
        public bool KillerBlocks { get { return cfg.Killer; } set { cfg.Killer = value; } }
        
        [Description("Whether players can die from drowning and falling from too high.")]
        [Category("Survival")]
        [DisplayName("Survival death")]
        public bool SurvivalDeath { get { return cfg.Death; } set { cfg.Death = value; } }
        
        [Description("Time taken before players drown, in tenths of a second.")]
        [Category("Survival")]
        [DisplayName("Drown time")]
        public int DrownTime { get { return cfg.drown; } set { cfg.drown = value; } }
        
        [Description("Falling more than this number of blocks results in death.")]
        [Category("Survival")]
        [DisplayName("Fall height")]
        public int FallHeight { get { return cfg.fall; } set { cfg.fall = value; } }
        
        
        bool GetAutoload() {
            return Server.AutoloadMaps.Contains(lvl.name);
        }
        
        void SetAutoload(bool value) {
            if (value) {
                Server.AutoloadMaps.AddOrReplace(lvl.name, lvl.physics.ToString());
            } else {
                Server.AutoloadMaps.Remove(lvl.name);
            }
            Server.AutoloadMaps.Save();
        }
    }
}
