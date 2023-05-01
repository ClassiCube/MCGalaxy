/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
// Copyright 2009, 2010 Matvei Stefarov <me@matvei.org>
/*
This generator was developed by Neko_baron.

Ideas, concepts, and code were used from the following two sources:
1) Isaac McGarvey's 'perlin noise generator' code
2) http://www.lighthouse3d.com/opengl/terrain/index.php3?introduction

 */
using System;
namespace MCGalaxy.Generator.Realistic 
{
    public static class NoiseGen 
    {
        public static void GenerateNormalized(float[] array, float persistence, int octaves, int width, int height, int seed, float zoom) {
            float min = float.MaxValue, max = float.MinValue;

            //Generate raw float data
            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
            {
                float sum = 0, frequency = 1, amplitude = 1;

                for (int i = 0; i < octaves; ++i) {
                    sum += InterpolatedNoise(x * frequency / zoom, y * frequency / zoom, seed) * amplitude;
                    frequency *= 2;
                    amplitude *= persistence;
                }

                array[y * width + x] = sum;
                min = sum < min ? sum : min;
                max = sum > max ? sum : max;
            }

            //Normalize
            float range = (max - min);
            for (int i = 0; i < array.Length; ++i)
                array[i] = (array[i] - min) / range;
        }
        
        static float Interpolate(float a, float b, float x) {
            float ft = x * 3.1415927f;
            float f = (float)(1 - Math.Cos(ft)) * 0.5f;
            return a * (1 - f) + b * f;
        }
        static float Noise(int x, int y, int seed) {
            int n = x + y * 57 + seed;
            n = (n << 13) ^ n;
            return (float)(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
            //return value is always in range [-1.0, 1.0]
        }


        unsafe static float InterpolatedNoise(float x, float y, int seed) {
            int wholeX = (int)x, wholeY = (int)y, i = 0;
            float fracX = x - wholeX, fracY = y - wholeY;

            // Calculates the smoothed noise for the 4 cells around the given point,
            //  then interpolates between the smoothed noise results of the 4 cells 

            //   -1 0 1 2
            // -1 * * * *
            //  0 * X X *
            //  1 * X X *
            //  2 * * * *
            float* noise = stackalloc float[RANGE * RANGE];
            for (int yy = -1; yy <= 2; yy++)
                for (int xx = -1; xx <= 2; xx++)
                {
                    noise[i++] = Noise(wholeX + xx, wholeY + yy, seed);
                }

            float n00 = SmoothNoise(noise, (0 + 1) * ONEX + (0 + 1) * ONEY); // x=0, y=0
            float n10 = SmoothNoise(noise, (1 + 1) * ONEX + (0 + 1) * ONEY); // x=1, y=0
            float n01 = SmoothNoise(noise, (0 + 1) * ONEX + (1 + 1) * ONEY); // x=0, y=1
            float n11 = SmoothNoise(noise, (1 + 1) * ONEX + (1 + 1) * ONEY); // x=1, y=1

            float N0 = Interpolate(n00, n10, fracX);
            float N1 = Interpolate(n01, n11, fracX);
            return Interpolate(N0, N1, fracY);
        }

        const int RANGE = 4;
        const int ONEX  = 1;
        const int ONEY  = RANGE;

        /// <summary> Calculates smoothed noise across the given cell and its 8 neighbours </summary>
        unsafe static float SmoothNoise(float* noise, int i) {
            float corners = (noise[i - ONEX - ONEY] + noise[i + ONEX - ONEY] + noise[i - ONEX + ONEY] + noise[i + ONEX + ONEY]) / 16;
            float sides   = (noise[i - ONEX] + noise[i + ONEX] + noise[i - ONEY] + noise[i + ONEY] / 8); // should be outside brackets, but kept for backwards compatibility
            float center  = noise[i] / 4;
            return corners + sides + center;
        }
    }
}