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
using MCGalaxy.Blocks.Physics;

namespace MCGalaxy {

    public sealed partial class Level : IDisposable {
        
        public delegate void OnLevelLoad(string level);

        public delegate void OnLevelLoaded(Level l);

        public delegate void OnLevelSave(Level l);

        public delegate void OnLevelUnload(Level l);

        public delegate void OnPhysicsUpdate(ushort x, ushort y, ushort z, PhysicsArgs args, Level l);

        public delegate void OnPhysicsStateChanged(object sender, PhysicsState state);

        public static event OnPhysicsStateChanged PhysicsStateChanged;

        public event OnPhysicsUpdate PhysicsUpdate;
        
        public static event OnLevelUnload LevelUnload;
        
        public static event OnLevelSave LevelSave;

        public event OnLevelUnload onLevelUnload;
        
        public static event OnLevelLoad LevelLoad;
        
        public static event OnLevelLoaded LevelLoaded;
    }
}