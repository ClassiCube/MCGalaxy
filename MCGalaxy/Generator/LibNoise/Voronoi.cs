// 
// Copyright (c) 2013 Jason Bell
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System;

namespace LibNoise
{
    public sealed class Voronoi : IModule
    {
        public double Frequency;
        public double Displacement;
        public int Seed;

        public Voronoi()
        {
            Frequency = 1.0;
            Displacement = 1.0;
        }

        public double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            int xInt = (x > 0.0 ? (int)x : (int)x - 1);
            int yInt = (y > 0.0 ? (int)y : (int)y - 1);
            int zInt = (z > 0.0 ? (int)z : (int)z - 1);

            double minDist = 2147483647.0;
            double xCandidate = 0;
            double yCandidate = 0;
            double zCandidate = 0;

            // Inside each unit cube, there is a seed point at a random position.  Go
            // through each of the nearby cubes until we find a cube with a seed point
            // that is closest to the specified position.
            for (int zCur = zInt - 2; zCur <= zInt + 2; zCur++)
            {
                for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++)
                {
                    for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++)
                    {

                        // Calculate the position and distance to the seed point inside of
                        // this unit cube.
                        double xPos = xCur + ValueNoise(xCur, yCur, zCur, Seed);
                        double yPos = yCur + ValueNoise(xCur, yCur, zCur, Seed + 1);
                        double zPos = zCur + ValueNoise(xCur, yCur, zCur, Seed + 2);
                        double xDist = xPos - x;
                        double yDist = yPos - y;
                        double zDist = zPos - z;
                        double dist = xDist * xDist + yDist * yDist + zDist * zDist;

                        if (dist < minDist)
                        {
                            // This seed point is closer to any others found so far, so record
                            // this seed point.
                            minDist = dist;
                            xCandidate = xPos;
                            yCandidate = yPos;
                            zCandidate = zPos;
                        }
                    }
                }
            }

            int x0 = (xCandidate > 0.0 ? (int)xCandidate : (int)xCandidate - 1);
            int y0 = (yCandidate > 0.0 ? (int)yCandidate : (int)yCandidate - 1);
            int z0 = (zCandidate > 0.0 ? (int)zCandidate : (int)zCandidate - 1);

            // Return the calculated distance with the displacement value applied.
            return Displacement * (double)ValueNoise(x0, y0, z0);
        }
        
        
        const int XNoiseGen = 1619;
        const int YNoiseGen = 31337;
        const int ZNoiseGen = 6971;
        const int SeedNoiseGen = 1013;

        static int IntValueNoise(int x, int y, int z, int seed)
        {
            // All constants are primes and must remain prime in order for this noise
            // function to work correctly.
            int n = (
                XNoiseGen * x
              + YNoiseGen * y
              + ZNoiseGen * z
              + SeedNoiseGen * seed)
              & 0x7fffffff;
            n = (n >> 13) ^ n;
            return (n * (n * n * 60493 + 19990303) + 1376312589) & 0x7fffffff;
        }

        static double ValueNoise(int x, int y, int z)
        {
            return ValueNoise(x, y, z, 0);
        }

        static double ValueNoise(int x, int y, int z, int seed)
        {
            return 1.0 - ((double)IntValueNoise(x, y, z, seed) / 1073741824.0);
        }
    }
}
