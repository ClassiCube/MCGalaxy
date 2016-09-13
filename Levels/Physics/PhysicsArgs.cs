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
        
        public const uint TypeMask = 0x3F;
        public const uint TypeBitsMask = 0x07;
        public const uint ValueBitsMask = 0xFF;
        
        public byte Type1 {
            get { return (byte)(Raw & TypeBitsMask); }
            set { Raw &= ~TypeBitsMask;
                Raw |= (uint)value << 0; }
        }
        
        public byte Type2 {
            get { return (byte)((Raw >> 3) & TypeBitsMask); }
            set { Raw &= ~(TypeBitsMask << 3);
                Raw |= (uint)value << 3; }
        }
        
        public byte Value1 {
            get { return (byte)(Raw >> 6); }
            set { Raw &= ~(ValueBitsMask << 6);
                Raw |= (uint)value << 6; }
        }
        
        public byte Value2 {
            get { return (byte)(Raw >> 14); }
            set { Raw &= ~(ValueBitsMask << 14);
                Raw |= (uint)value << 14; }
        }
        
        public byte Data {
            get { return (byte)(Raw >> 22); }
            set { Raw &= ~(ValueBitsMask << 22);
                Raw |= (uint)value << 22; }
        }
        
        public bool TDoor {
            get { return (Raw & (1u << 30)) != 0; }
            set { Raw &= ~(1u << 30);
                Raw |= (value ? 1u : 0u) << 30; }
        }
        
        public bool HasWait {
            get { return (Raw & TypeBitsMask) == Wait 
                    || ((Raw >> 3) & TypeBitsMask) == Wait;
            }
        }
        
        public void ResetTypes() { Raw &= ~TypeMask; }
        // TODO: what to do with last bit
        
        /// <summary> No special action is performed. </summary>
        public const byte None = 0;
        /// <summary> A specified action will be delayed for a certain time. </summary>
        public const byte Wait = 1;
        /// <summary> Reverts the block in the map back into the specified block id. </summary>
        public const byte Revert = 2;
        /// <summary> Randomly converts this physics item back into air. </summary>
        public const byte Dissipate = 3;
        /// <summary> Randomly causes this physics item to move down one block. </summary>
        public const byte Drop = 4;
        /// <summary> Randomly causes this physics item to create an explosion. </summary>
        public const byte Explode = 5;
        /// <summary> Causes this physics item to iterate through the 'rainbow' wool 
        /// block ids in either sequential or random order. </summary>
        public const byte Rainbow = 6;
        /// <summary> TNT block placed in tnt wars. </summary>
        public const byte TntWars = 7;
    }
}