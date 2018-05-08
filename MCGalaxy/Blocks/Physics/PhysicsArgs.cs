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

namespace MCGalaxy.Blocks.Physics {

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct PhysicsArgs {
        public uint Raw;
        
        public const uint TypeMask = 0x3F;
        public const uint TypeBitsMask = 0x07;
        public const uint ValueBitsMask = 0xFF;
        public const uint ExtBit = 1u << 30;
        public const uint ExtBits = (1u << 30) | (1u << 31);
        
        /// <summary> Indicates that this physics entry should be removed from the list of 
        /// entries that are checked for physics, at the end of the current tick. </summary>
        public const byte RemoveFromChecks = 255;
        
        public byte Type1 {
            get { return (byte)(Raw & TypeBitsMask); }
            set { Raw &= ~TypeBitsMask; Raw |= (uint)value << 0; }
        }
        
        public byte Type2 {
            get { return (byte)((Raw >> 3) & TypeBitsMask); }
            set { Raw &= ~(TypeBitsMask << 3); Raw |= (uint)value << 3; }
        }
        
        public byte Value1 {
            get { return (byte)(Raw >> 6); }
            set { Raw &= ~(ValueBitsMask << 6); Raw |= (uint)value << 6; }
        }
        
        public byte Value2 {
            get { return (byte)(Raw >> 14); }
            set { Raw &= ~(ValueBitsMask << 14); Raw |= (uint)value << 14; }
        }
        
        public byte Data {
            get { return (byte)(Raw >> 22); }
            set { Raw &= ~(ValueBitsMask << 22); Raw |= (uint)value << 22; }
        }
        
        public byte ExtBlock {
            get { return (byte)(Raw >> 30); }
            set { Raw &= ~ExtBits; Raw |= (uint)value << 30; }
        }
        
        
        public bool HasWait {
            get { return (Raw & TypeBitsMask) == Wait || ((Raw >> 3) & TypeBitsMask) == Wait; }
        }
        
        public void ResetTypes() { Raw &= ~TypeMask; }
        
        /// <summary> No special action is performed. </summary>
        public const byte None = 0;
        /// <summary> Another action will be executed after the given delay. </summary>
        public const byte Wait = 1;
        /// <summary> Reverts the block in the map back into the specified block. </summary>
        public const byte Revert = 2;
        /// <summary> Randomly converts back into air. </summary>
        public const byte Dissipate = 3;
        /// <summary> Randomly moves down one block. </summary>
        public const byte Drop = 4;
        /// <summary> Randomly creates an explosion. </summary>
        public const byte Explode = 5;
        /// <summary> Iterates through the 'rainbow' wool block in either sequential or random order. </summary>
        public const byte Rainbow = 6;
        /// <summary> Action is handled by the block's PhysicsHandler. </summary>
        public const byte Custom = 7;
    }
}