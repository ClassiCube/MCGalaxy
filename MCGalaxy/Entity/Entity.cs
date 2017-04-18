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

namespace MCGalaxy {
    
    public abstract class Entity {
        
        // Raw orientation/position - NOT threadsafe
        protected Orientation _rot;
        protected Position _pos;
        
        // Last sent orientation/position, for delta calculation
        protected Orientation lastRot;
        protected Position lastPos;
        
        /// <summary> Gets or sets the orientation of this entity. </summary>
        public Orientation Rot {
            get { return _rot; }
            set { _rot = value; OnSetRot(); }
        }
        
        /// <summary> Gets or sets the position of this entity. </summary>
        public Position Pos {
            get { return _pos; }
            set { _pos = value; OnSetPos(); }
        }
        
        protected virtual void OnSetPos() { }
        
        protected virtual void OnSetRot() { }
        
    }
    
    /// <summary> Represents the position of an entity in the world. </summary>
    public struct Position {
        
        /// <summary> X fixed-point location in the world. </summary>
        public int X;
        
        /// <summary> Y fixed-point location in the world. (vertical) </summary>
        public int Y;
        
        /// <summary> Z fixed-point location in the world. </summary>
        public int Z;
        
        /// <summary> World/block coordinate of this position. </summary>
        public Vec3S32 BlockCoords { get { return new Vec3S32(X >> 5, Y >> 5, Z >> 5); } }
    }
    
    /// <summary> Represents orientation / rotation of an entity. </summary>
    public struct Orientation {
        
        /// <summary> Rotation around X axis in packed form. </summary>
        public byte RotX;
        
        /// <summary> Rotation around Y axis in packed form. (yaw) </summary>
        public byte RotY;
        
        /// <summary> Rotation around Z axis in packed form. </summary>
        public byte RotZ;
        
        /// <summary> Rotation of head around X axis in packed form. (pitch) </summary>
        public byte HeadX;
        
        
        /// <summary> Converts angle in range [0, 256) into range [0, 360). </summary>
        public static short PackedToDegrees(byte packed) {
            return (short)(packed * 360 / 256);
        }
        
        /// <summary> Converts angle in degrees into range [0, 256) </summary>
        public static byte DegreesToPacked(short degrees) {
            return (byte)(degrees * 256 / 360);
        }
    }
}
