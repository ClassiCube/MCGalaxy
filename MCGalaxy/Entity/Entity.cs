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
		public Orientation Rot;
		public Position Pos;
		
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
		
		/// <summary> Rotation around X axis in degrees. </summary>
		public short RotX;
		
		/// <summary> Rotation around Y axis in degrees. (yaw) </summary>
		public short RotY;
		
		/// <summary> Rotation around Z axis in degrees. </summary>
		public short RotZ;
		
		/// <summary> Rotation of head around X axis in degrees. (pitch) </summary>
		public short HeadX;
	}
}
