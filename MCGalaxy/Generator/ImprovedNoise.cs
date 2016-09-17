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

namespace MCGalaxy.Generator {
    
    /// <summary> Implements improved perlin noise as described in http://mrl.nyu.edu/~perlin/noise/ </summary>
    public sealed class ImprovedNoise {
        
        public float Frequency = 1;
        public float Amplitude = 1;
        public float Lacunarity = 2;
        public float Persistence = 2;
        public int Octaves = 1;
        
        byte[] p = new byte[512];
        
        public ImprovedNoise( Random rnd ) {
            for( int i = 0; i < 256; i++ )
                p[i] = (byte)i;
            
            for( int i = 0; i < 256; i++ ) {
                int j = rnd.Next( i, 256 );
                byte temp = p[i]; p[i] = p[j]; p[j] = temp;
            }
            for( int i = 0; i < 256; i++ )
                p[i + 256] = p[i];
        }
        
        public float NormalisedNoise(float x, float y, float z) {
            float sum = 0;
            float freq = Frequency, amp = Amplitude;
            float scale = 0;
            
            for( int i = 0; i < Octaves; i++) {
                sum += Noise(x * freq, y * freq, z * freq) * amp;
                scale += amp;
                amp *= Persistence;
                freq *= Lacunarity;
            }
            return sum / scale;
        }
        
        public float OctaveNoise(float x, float y, float z) {
            float sum = 0;
            float freq = Frequency, amp = Amplitude;
            for(int i = 0; i < Octaves;i++) {
                sum += Noise(x * freq, y * freq, z * freq) * amp;
                
                amp *= Persistence;
                freq *= Lacunarity;
            }
            return sum;
        }
        
        public float Noise(float x, float y, float z) {
            int xFloor = x >= 0 ? (int)x : (int)x - 1;
            int yFloor = y >= 0 ? (int)y : (int)y - 1;
            int zFloor = z >= 0 ? (int)z : (int)z - 1;
            int X = xFloor & 0xFF, Y = yFloor & 0xFF, Z = zFloor & 0xFF;
            x -= xFloor; y -= yFloor; z -= zFloor;
            
            float u = Fade(x), v = Fade(y), w = Fade(z);
            int A = p[X] + Y, AA = p[A] + Z, AB = p[A + 1] + Z;
            int B = p[X + 1] + Y, BA = p[B] + Z, BB = p[B + 1] + Z;
            
            return Lerp(
                Lerp(
                    Lerp(Grad(p[AA], x, y, z),
                         Grad(p[BA], x - 1, y, z),
                         u),
                    Lerp(Grad(p[AB], x, y - 1, z),
                         Grad(p[BB], x - 1, y - 1, z),
                         u),
                    v),
                Lerp(
                    Lerp(Grad(p[AA + 1], x, y, z - 1),
                         Grad(p[BA + 1], x - 1, y, z - 1),
                         u),
                    Lerp(Grad(p[AB + 1], x, y - 1, z - 1),
                         Grad(p[BB + 1], x - 1, y - 1, z - 1),
                         u),
                    v),
                w);
        }
        
        
        static float Fade(float t) {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }
        
        static float Grad(int hash, float x, float y, float z) {
            switch (hash & 0xF) {
                    case 0x0: return  x + y;
                    case 0x1: return -x + y;
                    case 0x2: return  x - y;
                    case 0x3: return -x - y;
                    case 0x4: return  x + z;
                    case 0x5: return -x + z;
                    case 0x6: return  x - z;
                    case 0x7: return -x - z;
                    case 0x8: return  y + z;
                    case 0x9: return -y + z;
                    case 0xA: return  y - z;
                    case 0xB: return -y - z;
                    case 0xC: return  y + x;
                    case 0xD: return -y + z;
                    case 0xE: return  y - x;
                    case 0xF: return -y - z;
                    default: return 0; // never happens
            }
        }
        
        static float Lerp(float a, float b, float t) {
            return a + t * (b - a);
        }
    }
}
