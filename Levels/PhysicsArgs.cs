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
using System.Runtime.InteropServices;

namespace MCGalaxy.BlockPhysics {

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PhysicsArgs {
        public uint Raw;
        
        // Flags        
        /// <summary> Whether this physics item should wait before performing its other arguments. </summary>
        public bool Wait {
            get { return (Raw & (1u << 0)) != 0; }
            set { Raw &= ~(1u << 0);
                Raw |= (value ? 1u : 0u) << 0; }
        }
        
        /// <summary> Whether this physics item should randomly drop downards. </summary>
        public bool Drop {
            get { return (Raw & (1u << 1)) != 0; }
            set { Raw &= ~(1u << 1);
                Raw |= (value ? 1u : 0u) << 1; }
        }
        
        /// <summary> Whether this physics item should randomly convert back into air. </summary>
        public bool Dissipate {
            get { return (Raw & (1u << 2)) != 0; }
            set { Raw &= ~(1u << 2);
                Raw |= (value ? 1u : 0u) << 2; }
        }
        
        /// <summary> Whether this physics item should revert back into the given block id. </summary>
        public bool Revert {
            get { return (Raw & (1u << 3)) != 0; }
            set { Raw &= ~(1u << 3);
                Raw |= (value ? 1u : 0u) << 3; }
        }        
        
        /// <summary> Whether this physics item should check itself and its neighbours for tdoor activation. </summary>
        public bool Door {
            get { return (Raw & (1u << 4)) != 0; }
            set { Raw &= ~(1u << 4);
                Raw |= (value ? 1u : 0u) << 4; }
        }
        
        /// <summary> Whether this physics item should randomly explode. </summary>
        public bool Explode {
            get { return (Raw & (1u << 5)) != 0; }
            set { Raw &= ~(1u << 5);
                Raw |= (value ? 1u : 0u) << 5; }
        }
        
        /// <summary> Whether this physics update should have a rainbow affect applied. </summary>
        public bool Rainbow {
            get { return (Raw & (1u << 6)) != 0; }
            set { Raw &= ~(1u << 6);
                Raw |= (value ? 1u : 0u) << 6; }
        }
        
        // Data
        public bool RandomRainbow {
            get { return (Raw & (1u << 7)) != 0; }
            set { Raw |= (value ? 1u : 0u) << 7; }
        }
        
        public byte Value1 {
            get { return (byte)(Raw >> 8); }
            set { Raw &= ~(0xFFu << 8);
                Raw |= (uint)value << 8; }
        }
        
        public byte Value2 {
            get { return (byte)(Raw >> 16); }
            set { Raw &= ~(0xFFu << 16);
                Raw |= (uint)value << 16; }
        }
        
        public byte Value3 {
            get { return (byte)(Raw >> 24); }
            set { Raw &= ~(0xFFu << 24);
                Raw |= (uint)value << 24; }
        }
    }
}
