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
using MCGalaxy.Games;

namespace MCGalaxy.Gui {

    public sealed class LevelSettings {
        
		readonly Level lvl;
		public LevelSettings(Level lvl) {
			this.lvl = lvl;
		}
		
        [Description("Message shown to users when they join this level.")]
        [Category("General")]
        public string MOTD { get { return lvl.motd; } set { lvl.motd = value; } }
        
        [Description("Whether this map automatically loads when the server as started.")]
        [Category("General")]        
        public bool AutoLoad { get; set; } // TODO: autoload
        
        [Description("Whether if a player uses /goto on this map and this map is not " +
                     "loaded, the server then automatically loads this map.")]
        [Category("General")]
        public bool LoadOnGoto { get { return lvl.loadOnGoto; } set { lvl.loadOnGoto = value; } }
        
        [Description("Whether this level should be automatically unloaded " +
                     "if there are no players on it anymore.")]
        [Category("General")]
        public bool UnloadWhenEmpty { get { return lvl.unload; } set { lvl.unload = value; } }
        
        
        [Description("If physics is active, whether saplings should randomly grow into trees.")]
        [Category("Physics")]
        public bool TreeGrowing { get { return lvl.growTrees; } set { lvl.growTrees = value; } }
        
        [Description("If physics is active, whether leaf blocks not connected to trees should decay.")]
        [Category("Physics")]
        public bool LeafDecay { get { return lvl.leafDecay; } set { lvl.leafDecay = value; } }
        
        [Description("If physics is active, whether spreading liquids randomly pick a direction to flow in." +
                     "Otherwise, spreads in all directions.")]
        [Category("Physics")]
        public bool RandomFlow { get { return lvl.randomFlow; } set { lvl.randomFlow = value; } }
    }
}
