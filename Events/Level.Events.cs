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
using System.Linq;
using MCGalaxy.BlockPhysics;

namespace MCGalaxy {

    public sealed partial class Level : IDisposable {
		
        public delegate void OnLevelLoad(string level);

        public delegate void OnLevelLoaded(Level l);

        public delegate void OnLevelSave(Level l);

        public delegate void OnLevelUnload(Level l);

        public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, PhysicsArgs args, Level l);

        public delegate void OnPhysicsStateChanged(object sender, PhysicsState state);

        public static event OnPhysicsStateChanged PhysicsStateChanged;

        [Obsolete("Please use OnPhysicsUpdate.Register()")]
        public event OnPhysicsUpdate PhysicsUpdate;
        
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public static event OnLevelUnload LevelUnload;
        
        [Obsolete("Please use OnLevelSaveEvent.Register()")]
        public static event OnLevelSave LevelSave;

        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public event OnLevelUnload onLevelUnload;
        
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public static event OnLevelLoad LevelLoad;
        
        [Obsolete("Please use OnLevelUnloadEvent.Register()")]
        public static event OnLevelLoaded LevelLoaded;
    }
}