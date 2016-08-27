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

namespace MCGalaxy {  
	public struct Vec3U16 : IEquatable<Vec3U16> {
        public ushort X, Y, Z;
        public static Vec3U16 Zero = new Vec3U16(0);
        public static Vec3U16 MinVal = new Vec3U16(ushort.MinValue);
        public static Vec3U16 MaxVal = new Vec3U16(ushort.MaxValue);
        
        public Vec3U16(ushort x, ushort y, ushort z) {
            X = x; Y = y; Z = z;
        }
        
        public Vec3U16(ushort value) {
            X = value; Y = value; Z = value;
        }
        
        public override bool Equals(object obj) {
            return (obj is Vec3U16) && Equals((Vec3U16)obj);
        }
        
        public bool Equals(Vec3U16 other) {
            return X == other.X & Y == other.Y && Z == other.Z;
        }
        
        public override int GetHashCode() {
            int hashCode = 0;
            hashCode += 1000000007 * X;
            hashCode += 1000000009 * Y;
            hashCode += 1000000021 * Z;
            return hashCode;
        }
        
        public int LengthSquared { get { return X * X + Y * Y + Z * Z; } }
        
        public float Length { get { return (float)Math.Sqrt( X * X + Y * Y + Z * Z ); } }
        
        /// <summary> Clamps the given block coordinates to inside the map. </summary>
        public static Vec3U16 Clamp(ushort x, ushort y, ushort z, Level lvl) {
            Vec3U16 P = new Vec3U16(x, y, z);
            if (P.X >= 32768) P.X = 0;
            if (P.Y >= 32768) P.Y = 0;
            if (P.Z >= 32768) P.Z = 0;
            
            if (P.X >= lvl.Width) P.X = (ushort)(lvl.Width - 1);
            if (P.Y >= lvl.Height) P.Y = (ushort)(lvl.Height - 1);
            if (P.Z >= lvl.Length) P.Z = (ushort)(lvl.Length - 1);
            return P;
        }
        
        /// <summary> Clamps the given player position coordinates to inside the map. </summary>
        public static Vec3U16 ClampPos(ushort x, ushort y, ushort z, Level lvl) {
            Vec3U16 P = new Vec3U16(x, y, z);
            if (P.X >= 32768) P.X = 0;
            if (P.Y >= 32768) P.Y = 0;
            if (P.Z >= 32768) P.Z = 0;
            
            if (P.X >= lvl.Width * 32) P.X = (ushort)(32 * lvl.Width - 32);
            if (P.Y >= lvl.Height * 32) P.Y = (ushort)(32 * lvl.Height - 32);
            if (P.Z >= lvl.Length * 32) P.Z = (ushort)(32 * lvl.Length - 32);
            return P;
        }
        
        public static bool operator == (Vec3U16 a, Vec3U16 b) {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        
        public static bool operator != (Vec3U16 a, Vec3U16 b) {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }
        
        public static explicit operator Vec3U16(Vec3S32 a) {
            return new Vec3U16((ushort)a.X, (ushort)a.Y, (ushort)a.Z);
        }
        
        public override string ToString() {
            return X + "," + Y + "," + Z;
        }
        
        public string ToString(string separator) {
            return String.Format("{1}{0}{2}{0}{3}", separator, X, Y, Z);
        }
    }
    
    public struct Vec3S32 : IEquatable<Vec3S32> {        
        public int X, Y, Z;
        
        public Vec3S32(int x, int y, int z) {
            X = x; Y = y; Z = z;
        }
        
        public override bool Equals(object obj) {
            return (obj is Vec3S32) && Equals((Vec3S32)obj);
        }
        
        public bool Equals(Vec3S32 other) {
            return X == other.X & Y == other.Y && Z == other.Z;
        }
        
        public override int GetHashCode() {
            int hashCode = 0;
            hashCode += 1000000007 * X;
            hashCode += 1000000009 * Y;
            hashCode += 1000000021 * Z;
            return hashCode;
        }
        
        public int LengthSquared { get { return X * X + Y * Y + Z * Z; } }
        
        public float Length { get { return (float)Math.Sqrt( X * X + Y * Y + Z * Z ); } }
        
        public float Dot(Vec3S32 b) { return X * b.X + Y * b.Y + Z * b.Z; }
        
        public static Vec3S32 Max(Vec3S32 a, Vec3S32 b) {
            return new Vec3S32(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }
        
        public static Vec3S32 Min(Vec3S32 a, Vec3S32 b) {
            return new Vec3S32(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }
        
        public static implicit operator Vec3S32(Vec3U16 a) {
            return new Vec3S32(a.X, a.Y, a.Z);
        }
        
        public static Vec3S32 operator + (Vec3S32 a, Vec3S32 b) {
            return new Vec3S32(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static Vec3S32 operator - (Vec3S32 a, Vec3S32 b) {
            return new Vec3S32(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        
        public static Vec3S32 operator * (Vec3S32 a, int b) {
            return new Vec3S32(a.X * b, a.Y * b, a.Z * b);
        }
        
        public static Vec3S32 operator / (Vec3S32 a, int b) {
            return new Vec3S32(a.X / b, a.Y / b, a.Z / b);
        }
        
        public static bool operator == (Vec3S32 a, Vec3S32 b) {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        
        public static bool operator != (Vec3S32 a, Vec3S32 b) {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }
        
        public override string ToString() {
            return X + "," + Y + "," + Z;
        }
        
        public string ToString(string separator) {
            return String.Format("{1}{0}{2}{0}{3}", separator, X, Y, Z);
        }
    }
    
    public struct Vec3F32 : IEquatable<Vec3F32> {        
        public float X, Y, Z;
        
        public Vec3F32(float x, float y, float z) {
            X = x; Y = y; Z = z;
        }
        
        public float LengthSquared {
            get { return X * X + Y * Y + Z * Z; }
        }
        
        public float Length {
            get { return (float)Math.Sqrt( X * X + Y * Y + Z * Z ); }
        }
        
        public static float Dot(Vec3F32 a, Vec3F32 b) {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }
        
        public static Vec3F32 Cross(Vec3F32 a, Vec3F32 b) {
            return new Vec3F32(a.Y * b.Z - a.Z * b.Y,
                               a.Z * b.X - a.X * b.Z,
                               a.X * b.Y - a.Y * b.X);
        }
        
        public static Vec3F32 Normalise(Vec3F32 a) {
            float invLen = 1 / a.Length;
            a.X *= invLen; a.Y *= invLen; a.Z *= invLen;
            return a;
        }        
        
        public static Vec3F32 operator * (float a, Vec3F32 b) {
            return new Vec3F32(a * b.X, a * b.Y, a * b.Z);
        }
        
        public static Vec3F32 operator - (Vec3F32 a, Vec3F32 b) {
            return new Vec3F32(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        
        public static Vec3F32 operator + (Vec3F32 a, Vec3F32 b) {
            return new Vec3F32(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static implicit operator Vec3F32(Vec3S32 a) {
            return new Vec3F32(a.X, a.Y, a.Z);
        }       
        
        public override bool Equals(object obj) {
            return (obj is Vec3F32) && Equals((Vec3F32)obj);
        }
        
        public bool Equals(Vec3F32 other) {
            return X == other.X & Y == other.Y && Z == other.Z;
        }
        
        public override int GetHashCode() {
            int hashCode = 0;
            hashCode += 1000000007 * X.GetHashCode();
            hashCode += 1000000009 * Y.GetHashCode();
            hashCode += 1000000021 * Z.GetHashCode();
            return hashCode;
        }
        
        public static bool operator == (Vec3F32 a, Vec3F32 b) {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        
        public static bool operator != (Vec3F32 a, Vec3F32 b) {
            return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
        }
        
        public override string ToString() {
            return X + "," + Y + "," + Z;
        }
        
        public string ToString(string separator) {
            return String.Format("{1}{0}{2}{0}{3}", separator, X, Y, Z);
        }
    }
}
