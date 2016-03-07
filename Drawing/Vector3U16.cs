/*
    Copyright 2015 MCGalaxy team
    
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

namespace MCGalaxy.Drawing {
    
    public struct Vector3U16 {
        
        public ushort X, Y, Z;
        
        public Vector3U16(ushort x, ushort y, ushort z) {
            X = x; Y = y; Z = z;
        }
        
        public Vector3U16(ushort value) {
            X = value; Y = value; Z = value;
        }
        
        public override bool Equals(object obj) {
            return (obj is Vector3U16) && Equals((Vector3U16)obj);
        }
        
        public bool Equals(Vector3U16 other) {
            return X == other.X & Y == other.Y && Z == other.Z;
        }
        
        public override int GetHashCode() {
            int hashCode = 0;
            hashCode += 1000000007 * X;
            hashCode += 1000000009 * Y;
            hashCode += 1000000021 * Z;
            return hashCode;
        }
        
        public static bool operator == (Vector3U16 a, Vector3U16 b) {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        
        public static bool operator != (Vector3U16 a, Vector3U16 b) {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }
        
        public static Vector3U16 Max(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return new Vector3U16(Math.Max(x1, x2), Math.Max(y1, y2), Math.Max(z1, z2));
        }
        
        public static Vector3U16 Min(ushort x1, ushort y1, ushort z1, ushort x2, ushort y2, ushort z2) {
            return new Vector3U16(Math.Min(x1, x2), Math.Min(y1, y2), Math.Min(z1, z2));
        }
        
        public static Vector3U16 Max(Vector3U16 a, Vector3U16 b) {
            return new Vector3U16(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }
        
        public static Vector3U16 Min(Vector3U16 a, Vector3U16 b) {
            return new Vector3U16(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }
        
        public static Vector3U16 operator + (Vector3U16 a, Vector3U16 b) {
            return new Vector3U16((ushort)(a.X + b.X), (ushort)(a.Y + b.Y), (ushort)(a.Z + b.Z));
        }
        
        public static Vector3U16 operator - (Vector3U16 a, Vector3U16 b) {
            return new Vector3U16((ushort)(a.X - b.X), (ushort)(a.Y - b.Y), (ushort)(a.Z - b.Z));
        }
    }
}
