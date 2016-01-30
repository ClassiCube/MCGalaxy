/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy {
	
    public sealed class NoiseGen {

        public static void GenerateNormalized(float[] array, float persistence, int octaves, int width, int height, int seed, float zoom) {
            float min = float.MaxValue, max = float.MinValue;

            //Generate raw float data
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    float total = 0;
                    float frequency = 1;
                    float amplitude = 1;

                    for (int i = 0; i < octaves; ++i)
                    {
                        total += InterpolatedNoise(x * frequency / zoom, y * frequency / zoom, seed) * amplitude;
                        frequency *= 2;
                        amplitude *= persistence;
                    }

                    array[y * width + x] = total;

                    min = total < min ? total : min;
                    max = total > max ? total : max;
                }
            }

            //Normalize
            float adj = (max - min);
            for (int i = 0; i < array.Length; ++i)
                array[i] = (array[i] - min) / adj;
        }

        static float Noise(int x, int y, int seed) {
            int n = x + y * 57 + seed;
            n = (n << 13) ^ n;
            return (float)(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
            //return value is always in range [-1.0, 1.0]
        }

        static float SmoothNoise(int x, int y, int seed) {
            float corners = (Noise(x - 1, y - 1, seed) + Noise(x + 1, y - 1, seed) + Noise(x - 1, y + 1, seed) + Noise(x + 1, y + 1, seed)) / 16;
            float sides = (Noise(x - 1, y, seed) + Noise(x + 1, y, seed) + Noise(x, y - 1, seed) + Noise(x, y + 1, seed) / 8);
            float center = Noise(x, y, seed) / 4;
            return corners + sides + center;
        }

        static float Interpolate(float a, float b, float x) {
            float ft = x * 3.1415927f;
            float f = (float)(1 - Math.Cos(ft)) * .5f;

            return a * (1 - f) + b * f;
        }

        static float InterpolatedNoise(float x, float y, int seed) {
            int wholePartX = (int)x;
            float fractionPartX = x - wholePartX;

            int wholePartY = (int)y;
            float fractionPartY = y - wholePartY;

            float v1 = SmoothNoise(wholePartX, wholePartY, seed);
            float v2 = SmoothNoise(wholePartX + 1, wholePartY, seed);
            float v3 = SmoothNoise(wholePartX, wholePartY + 1, seed);
            float v4 = SmoothNoise(wholePartX + 1, wholePartY + 1, seed);

            float i1 = Interpolate(v1, v2, fractionPartX);
            float i2 = Interpolate(v3, v4, fractionPartX);

            return Interpolate(i1, i2, fractionPartY);
        }
    }
}