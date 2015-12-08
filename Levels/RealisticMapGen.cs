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
namespace MCGalaxy
{
    public sealed class RealisticMapGen
    {
        static bool Inuse = false;
        static float[] terrain;
        static float[] overlay;
        static float[] overlay2;
        static float divide;

        public bool GenerateMap(Level Lvl, string type, int seed = 0, bool useSeed = false)
        {
            DateTime startTime = DateTime.Now;

            Server.s.Log("Attempting map gen");
            if (Inuse) { Server.s.Log("Generator in use"); return false; }
            Random rand = useSeed ? new System.Random(seed) : new System.Random();
            try
            {
                Inuse = true;
                terrain = new float[Lvl.Width * Lvl.Length];  //hmm 
                overlay = new float[Lvl.Width * Lvl.Length];

                if (!type.Equals("ocean"))
                { overlay2 = new float[Lvl.Width * Lvl.Length]; }

                //float dispAux, pd;
                ushort WaterLevel = (ushort)(Lvl.Height / 2 + 2);
                ushort LavaLevel = 5;

                if (type.Equals("ocean"))
                {
                    WaterLevel = (ushort)(Lvl.Height * 0.85f);
                }
                //Generate the level
                GenerateFault(terrain, Lvl, type, rand);

                //APPLY FILTER to terrain
                FilterAverage(Lvl);

                //CREATE OVERLAY
                //GenerateFault(overlay, Lvl, "overlay", rand);
                Server.s.Log("Creating overlay");
                GeneratePerlinNoise(overlay, Lvl, "", rand);

                if (!type.Equals("ocean") && type != "desert")
                {
                    Server.s.Log("Planning trees");
                    GeneratePerlinNoise(overlay2, Lvl, "", rand);
                }

                Server.s.Log("Converting height map");
                Server.s.Log("And applying overlays");
                float RangeLow = 0.2f;
                float RangeHigh = 0.8f;
                float TreeDens = 0.35f;
                short TreeDist = 3;
                //changes the terrain range based on type, also tree threshold
                switch (type)
                {
                    case "hell":
                        RangeLow = .3f;
                        RangeHigh = 1.3f;
                        break;
                    case "island":
                        RangeLow = 0.4f;
                        RangeHigh = 0.75f;
                        break;
                    case "forest":
                        RangeLow = 0.45f;
                        RangeHigh = 0.8f;
                        TreeDens = 0.7f;
                        TreeDist = 2;
                        break;
                    case "mountains":
                        RangeLow = 0.3f;
                        RangeHigh = 0.9f;
                        TreeDist = 4;
                        break;
                    case "ocean":
                        RangeLow = 0.1f;
                        RangeHigh = 0.6f;
                        break;
                    case "desert":
                        RangeLow = 0.5f;
                        RangeHigh = 0.85f;
                        WaterLevel = 0;
                        TreeDist = 24;
                        break;
                    default:
                        break;
                }

                //loops though evey X/Z coordinate
                for (int bb = 0; bb < terrain.Length; bb++)
                {
                    ushort x = (ushort)(bb % Lvl.Width);
                    ushort y = (ushort)(bb / Lvl.Width);
                    ushort z;
                    if (type.Equals("island"))
                    {
                        z = Evaluate(Lvl, Range(terrain[bb], RangeLow - NegateEdge(x, y, Lvl), RangeHigh - NegateEdge(x, y, Lvl)));
                    }
                    else
                    {
                        z = Evaluate(Lvl, Range(terrain[bb], RangeLow, RangeHigh));
                    }
                    if (type != "hell")
                    {
                        #region nonLavaWorld
                        if (z > WaterLevel)
                        {
                            for (ushort zz = 0; z - zz >= 0; zz++)
                            {
                                if (type == "desert")
                                {
                                    Lvl.SetTile(x, (ushort)(z - zz), y, Block.sand);
                                }
                                else if (overlay[bb] < 0.72f)    //If not zoned for rocks or gravel
                                {
                                    if (type.Equals("island"))      //increase sand height for island
                                    {
                                        if (z > WaterLevel + 2)
                                        {
                                            if (zz == 0) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.grass); }      //top layer
                                            else if (zz < 3) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.dirt); }   //next few
                                            else { Lvl.SetTile(x, (ushort)(z - zz), y, Block.rock); }               //ten rock it
                                        }
                                        else
                                        {
                                            Lvl.SetTile(x, (ushort)(z - zz), y, Block.sand);                        //SAAAND extra for islands
                                        }
                                    }
                                    else if (type == "desert")
                                    {
                                        Lvl.SetTile(x, (ushort)(z - zz), y, Block.sand);
                                    }
                                    else
                                    {
                                        if (zz == 0) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.grass); }
                                        else if (zz < 3) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.dirt); }
                                        else { Lvl.SetTile(x, (ushort)(z - zz), y, Block.rock); }
                                    }
                                }
                                else
                                {

                                    Lvl.SetTile(x, (ushort)(z - zz), y, Block.rock);

                                }
                            }

                            if (overlay[bb] < 0.25f && type != "desert")    //Zoned for flowers
                            {
                                switch (rand.Next(12))
                                {
                                    case 10:
                                        Lvl.SetTile(x, (ushort)(z + 1), y, Block.redflower);
                                        break;
                                    case 11:
                                        Lvl.SetTile(x, (ushort)(z + 1), y, Block.yellowflower);
                                        break;
                                    default:
                                        break;
                                }
                            }



                            if (!type.Equals("ocean"))
                            {
                                if (overlay[bb] < 0.65f && overlay2[bb] < TreeDens)
                                {
                                    if (Lvl.GetTile(x, (ushort)(z + 1), y) == Block.air)
                                    {
                                        if (Lvl.GetTile(x, z, y) == Block.grass || type == "desert")
                                        {
                                            if (rand.Next(13) == 0)
                                            {
                                                if (!TreeCheck(Lvl, x, z, y, TreeDist))
                                                {
                                                    if (type == "desert")
                                                        AddCactus(Lvl, x, (ushort)(z + 1), y, rand);
                                                    else
                                                        AddTree(Lvl, x, (ushort)(z + 1), y, rand);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else    //Must be on/under the water line then
                        {
                            for (ushort zz = 0; WaterLevel - zz >= 0; zz++)
                            {

                                if (WaterLevel - zz > z)
                                { Lvl.SetTile(x, (ushort)(WaterLevel - zz), y, Block.water); }    //better fill the water aboce me
                                else if (WaterLevel - zz > z - 3)
                                {
                                    if (overlay[bb] < 0.75f)
                                    {
                                        Lvl.SetTile(x, (ushort)(WaterLevel - zz), y, Block.sand);   //sand top
                                    }
                                    else
                                    {
                                        Lvl.SetTile(x, (ushort)(WaterLevel - zz), y, Block.gravel);  //zoned for gravel
                                    }
                                }
                                else
                                {
                                    Lvl.SetTile(x, (ushort)(WaterLevel - zz), y, Block.rock);
                                }
                            }

                        }
                        #endregion
                    }
                    else //all of lava world generation
                    {
                       
                        if (z > LavaLevel)
                        {
                            for (ushort zz = 0; z - zz >= 0; zz++)
                            {
                                if (z > (LavaLevel - 1))
                                {
                                    if (zz == 0) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.rock); }      //top layer
                                    else if (zz < 3) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.rock); }
                                    else if (zz < 2) { Lvl.SetTile(x, (ushort)(z - zz), y, Block.lava); }//next few
                                    else { Lvl.SetTile(x, (ushort)(z - zz), y, Block.obsidian); }
                                }
                                else
                                {
                                    Lvl.SetTile(x, (ushort)(z - zz), y, Block.lava);                       
                                }
                                if (overlay[bb] < 0.3f) 
                                {
                                    switch (rand.Next(13))
                                    {
                                        case 9:
                                        case 10:
                                        case 11:
                                        case 12:
                                            Lvl.SetTile(x, (ushort)(z + 1), y, Block.lava); //change to lava when time
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                // if (zz == z) Lvl.skipChange(x, (ushort)(z - zz), y, Block.opsidian);
                                Lvl.SetTile(x, (ushort)(z), y, (rand.Next(100) % 3 == 1 ? Block.darkgrey : Block.obsidian));

                            }
                        }

                        else
                        {
                            for (ushort zz = 0; LavaLevel - zz >= 0; zz++)
                            {

                                if (LavaLevel - zz > z - 1)
                                { /*if (Lvl.GetTile(x, z, y) == Block.air)*/ Lvl.SetTile(x, (ushort)(LavaLevel - zz), y, Block.lava); }    //better fill the water aboce me
                                else if (LavaLevel - zz > z - 3)
                                {
                                    if (overlay[bb] < .9f)
                                    {
                                        if (zz < z) Lvl.SetTile(x, (ushort)(z - zz), (ushort)(y), Block.lava);
                                        else Lvl.SetTile(x, (ushort)(z - zz), y, Block.rock);
                                    }
                                    else
                                    {
                                        Lvl.SetTile(x, (ushort)(LavaLevel - zz), (ushort)(y - 5), Block.lava);  //killer lava
                                    }
                                }
                                else
                                {
                                    Lvl.SetTile(x, (ushort)(LavaLevel - zz), y, Block.stone); //and just make the rest cobblestone
                                }
                            }
                        }

                        
                    }
                }
                Server.s.Log("Total time was " + (DateTime.Now - startTime).TotalSeconds.ToString() + " seconds.");


            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
                Server.s.Log("Gen Fail");
                Inuse = false;
                return false;
            }

            terrain = new float[0]; //Derp
            overlay = new float[0]; //Derp
            overlay2 = new float[0]; //Derp

            Inuse = false;

            return true;
        }

        //condensed fault generator
        #region ==FaultGen==
        void GenerateFault(float[] array, Level Lvl, string type, Random rand)
        {
            float startheight = 0.5f;
            float dispAux;
            ushort i, j, k, halfX, halfZ;
            float a, b, c, w, d;

            float DispMax, DispMin, DispChange;
            DispMax = 0.01f;
            DispChange = -0.0025f;
            if (type.Equals("mountains"))
            {
                DispMax = 0.02f;
                startheight = 0.6f;
            }
            else if (type.Equals("hell"))
            {
                DispMax = 0.02f;
                startheight = 0.04f;
            }
            else if (type.Equals("overlay"))
            {
                DispMax = 0.02f;
                DispChange = -0.01f;
            }

            for (int x = 0; x < array.Length; x++)
            {
                array[x] = startheight;
                //overlay[x] = 0.5f;
            }
            DispMin = -DispMax;
            float disp = DispMax;
            //if (terrainHeights == NULL)
            //    return (TERRAIN_ERROR_NOT_INITIALISED);


            halfX = (ushort)(Lvl.Width / 2);
            halfZ = (ushort)(Lvl.Length / 2);
            int numIterations = (int)((Lvl.Width + Lvl.Length));
            Server.s.Log("Iterations = " + numIterations.ToString());
            for (k = 0; k < numIterations; k++)
            {
                //s.Log("itteration " + k.ToString());
                d = (float)Math.Sqrt(halfX * halfX + halfZ * halfZ);
                w = (float)(rand.NextDouble() * 360);
                //w = (float)(rand.NextDouble()*90);
                a = (float)Math.Cos(w);
                b = (float)Math.Sin(w);

                c = ((float)rand.NextDouble()) * 2 * d - d;
                //c = ((float)rand.NextDouble() / 1) * 2 * d - d;
                //float disp = (float)(rand.NextDouble()* 0.02f - 0.01f);
                //iterationsDone++;
                //if (iterationsDone < itMinDisp)
                //    disp = maxDisp + (iterationsDone / (itMinDisp + 0.0)) * (minDisp - maxDisp);
                //else
                //    disp = minDisp;
                for (i = 0; i < Lvl.Length; i++)
                {
                    for (j = 0; j < Lvl.Width; j++)
                    {
                        //switch (terrainFunction)
                        //{
                        //case STEP:
                        if ((i - halfZ) * a + (j - halfX) * b + c > 0)
                            dispAux = disp;
                        else
                            dispAux = -disp;
                        //    break;
                        /*case SIN:
                            pd = ((i - halfZ) * a + (j - halfX) * b + c) / terrainWaveSize;
                            if (pd > 1.57) pd = 1.57;
                            else if (pd < 0) pd = 0;
                            dispAux = -disp / 2 + sin(pd) * disp;
                            break;
                        case COS:
                            pd = ((i - halfZ) * a + (j - halfX) * b + c) / terrainWaveSize;
                            if (pd > 3.14) pd = 3.14;
                            else if (pd < -3.14) pd = -3.14;
                            dispAux = disp - (terrainWaveSize / (terrainGridWidth + 0.0)) + cos(pd) * disp;
                            break;
                    }*/
                        //s.Log("adding " + dispAux.ToString());
                        AddTerrainHeight(array, j, i, Lvl.Width, dispAux);
                        //terrainHeights[i * terrainGridWidth + j] += dispAux;
                    }
                }

                disp += DispChange;
                if (disp < DispMin) { disp = DispMax; }
            }
        }
        #endregion

        //hur hur, more copy pasted code :/
        #region ==PerlinGen==
        void GeneratePerlinNoise(float[] array, Level Lvl, string type, Random rand)
        {
            GenerateNormalized(array, 0.7f, 8, Lvl.Width, Lvl.Length, rand.Next(), 64);
        }

        void GenerateNormalized(float[] array, float persistence, int octaves, int width, int height, int seed, float zoom)
        {
            float min = 0;
            float max = 0;
            //float * pDataFloat = new float[width * height];

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
                        total = total + InterpolatedNoise(x * frequency / zoom, y * frequency / zoom, seed) * amplitude;
                        frequency *= 2;
                        amplitude *= persistence;
                    }

                    array[y * width + x] = total;

                    min = total < min ? total : min;
                    max = total > max ? total : max;
                }
            }

            //Normalize
            for (int i = 0; i < width * height; ++i)
            {
                array[i] = (array[i] - min) / (max - min);
                //array[i] = (255 << 24) | ((unsigned char) (red * ((pDataFloat[i] - min) / (max - min)) * 255) << 16) | 
                //((unsigned char) (green * ((pDataFloat[i] - min) / (max - min)) * 255) << 8) | (unsigned char) (blue * ((pDataFloat[i] - min) / (max - min)) * 255);
            }
        }

        float Noise(int x, int y, int seed)
        {
            int n = x + y * 57 + seed;
            n = (n << 13) ^ n;
            return (float)(1.0 - ((n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0);
            //return value is always in range [-1.0, 1.0]
        }

        float SmoothNoise(int x, int y, int seed)
        {
            float corners = (Noise(x - 1, y - 1, seed) + Noise(x + 1, y - 1, seed) + Noise(x - 1, y + 1, seed) + Noise(x + 1, y + 1, seed)) / 16;
            float sides = (Noise(x - 1, y, seed) + Noise(x + 1, y, seed) + Noise(x, y - 1, seed) + Noise(x, y + 1, seed) / 8);
            float center = Noise(x, y, seed) / 4;
            return corners + sides + center;
        }

        float Interpolate(float a, float b, float x)
        {
            float ft = x * 3.1415927f;
            float f = (float)(1 - Math.Cos(ft)) * .5f;

            return a * (1 - f) + b * f;
        }

        float InterpolatedNoise(float x, float y, int seed)
        {
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

        #endregion

        //
        public void AddTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte height = (byte)Rand.Next(5, 8);
            short top = (short)(height - Rand.Next(2, 4));
            ushort xxx, yyy, zzz;
            for (ushort yy = 0; yy < top + height - 1; yy++)
            {
                if (overwrite || Lvl.GetTile(x, (ushort)(y + yy), z) == Block.air || (y + yy == y && Lvl.GetTile(x, (ushort)(y + yy), z) == Block.shrub))
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, (ushort)(y + yy), z, Block.trunk);
                        else Lvl.Blockchange(p, x, (ushort)(y + yy), z, Block.trunk);
                    else Lvl.SetTile(x, (ushort)(y + yy), z, Block.trunk);
            }


            for (short xx = (short)-top; xx <= top; ++xx)
            {
                for (short yy = (short)-top; yy <= top; ++yy)
                {
                    for (short zz = (short)-top; zz <= top; ++zz)
                    {
                        short Dist = (short)(Math.Sqrt(xx * xx + yy * yy + zz * zz));
                        if (Dist < top + 1)
                        {
                            if (Rand.Next((int)(Dist)) < 2)
                            {
                                try
                                {
                                    xxx = (ushort)(x + xx);
                                    yyy = (ushort)(y + yy + height);
                                    zzz = (ushort)(z + zz);

                                    if ((xxx != x || zzz != z || yy >= top - 1) && (overwrite || Lvl.GetTile(xxx, yyy, zzz) == Block.air))
                                        if (blockChange)
                                            if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                            else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                                        else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }

        public void AddNotchTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte dist, tile;
            byte height = (byte)Rand.Next(3, 7);
            byte top = (byte)(height - 2);
            short xx, yy, zz;
            ushort xxx, yyy, zzz;
            for (yy = 0; yy <= height; yy++)
            {
                yyy = (ushort)(y + yy);
                tile = Lvl.GetTile(x, yyy, z);
                if (overwrite || tile == Block.air || (yyy == y && tile == Block.shrub))
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, yyy, z, Block.trunk);
                        else Lvl.Blockchange(p, x, yyy, z, Block.trunk);
                    else Lvl.SetTile(x, yyy, z, Block.trunk);
            }

            for (yy = top; yy <= height + 1; yy++)
            {
                dist = yy > height - 1 ? (byte)1 : (byte)2;
                for (xx = (short)-dist; xx <= dist; xx++)
                {
                    for (zz = (short)-dist; zz <= dist; zz++)
                    {
                        xxx = (ushort)(x + xx);
                        yyy = (ushort)(y + yy);
                        zzz = (ushort)(z + zz);
                        tile = Lvl.GetTile(xxx, yyy, zzz);
                        //Server.s.Log(String.Format("{0} {1} {2}", xxx, yyy, zzz));

                        if ((xxx == x && zzz == z && yy <= height) || (!overwrite && tile != Block.air))
                            continue;

                        if (Math.Abs(xx) == dist && Math.Abs(zz) == dist)
                        {
                            if (yy > height)
                                continue;

                            if (Rand.Next(2) == 0)
                            {
                                if (blockChange)
                                    if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                    else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                                else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                            }
                        }
                        else
                        {
                            if (blockChange)
                                if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                            else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                        }
                    }
                }
            }
        }

        public void AddNotchBigTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            throw new NotImplementedException();
        }

        public void AddNotchPineTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            throw new NotImplementedException();
            //byte height = (byte)Rand.Next(7, 12);
        }

        public void AddNotchSwampTree(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte dist, tile;
            byte height = (byte)Rand.Next(4, 8);
            byte top = (byte)(height - 2);
            short xx, yy, zz;
            ushort xxx, yyy, zzz;
            for (yy = 0; yy <= height; yy++)
            {
                yyy = (ushort)(y + yy);
                tile = Lvl.GetTile(x, yyy, z);
                if (overwrite || tile == Block.air || (yyy == y && tile == Block.shrub))
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, yyy, z, Block.trunk);
                        else Lvl.Blockchange(p, x, yyy, z, Block.trunk);
                    else Lvl.SetTile(x, yyy, z, Block.trunk);
            }

            for (yy = top; yy <= height + 1; yy++)
            {
                dist = yy > height - 1 ? (byte)2 : (byte)3;
                for (xx = (short)-dist; xx <= dist; xx++)
                {
                    for (zz = (short)-dist; zz <= dist; zz++)
                    {
                        xxx = (ushort)(x + xx);
                        yyy = (ushort)(y + yy);
                        zzz = (ushort)(z + zz);
                        tile = Lvl.GetTile(xxx, yyy, zzz);
                        //Server.s.Log(String.Format("{0} {1} {2}", xxx, yyy, zzz));

                        if ((xxx == x && zzz == z && yy <= height) || (!overwrite && tile != Block.air))
                            continue;

                        if (Math.Abs(xx) == dist && Math.Abs(zz) == dist)
                        {
                            if (yy > height)
                                continue;

                            if (Rand.Next(2) == 0)
                            {
                                if (blockChange)
                                    if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                    else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                                else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                            }
                        }
                        else
                        {
                            if (blockChange)
                                if (p == null) Lvl.Blockchange(xxx, yyy, zzz, Block.leaf);
                                else Lvl.Blockchange(p, xxx, yyy, zzz, Block.leaf);
                            else Lvl.SetTile(xxx, yyy, zzz, Block.leaf);
                        }
                    }
                }
            }
        }

        public void AddCactus(Level Lvl, ushort x, ushort y, ushort z, Random Rand, bool blockChange = false, bool overwrite = true, Player p = null)
        {
            byte height = (byte)Rand.Next(3, 6);
            ushort yy;

            for (yy = 0; yy <= height; yy++)
            {
                if (overwrite || Lvl.GetTile(z, (ushort)(y + yy), z) == Block.air)
                    if (blockChange)
                        if (p == null) Lvl.Blockchange(x, (ushort)(y + yy), z, Block.green);
                        else Lvl.Blockchange(p, x, (ushort)(y + yy), z, Block.green);
                    else Lvl.SetTile(x, (ushort)(y + yy), z, Block.green);
            }

            int inX = 0, inZ = 0;

            switch (Rand.Next(1, 3))
            {
                case 1: inX = -1; break;
                case 2:
                default: inZ = -1; break;
            }

            for (yy = height; yy <= Rand.Next(height + 2, height + 5); yy++)
            {
                if (overwrite || Lvl.GetTile((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ)) == Block.air)
                    if (blockChange)
                        if (p == null) Lvl.Blockchange((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ), Block.green);
                        else Lvl.Blockchange(p, (ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ), Block.green);
                    else Lvl.SetTile((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ), Block.green);
            }
            for (yy = height; yy <= Rand.Next(height + 2, height + 5); yy++)
            {
                if (overwrite || Lvl.GetTile((ushort)(x + inX), (ushort)(y + yy), (ushort)(z + inZ)) == Block.air)
                    if (blockChange)
                        if (p == null) Lvl.Blockchange((ushort)(x - inX), (ushort)(y + yy), (ushort)(z - inZ), Block.green);
                        else Lvl.Blockchange(p, (ushort)(x - inX), (ushort)(y + yy), (ushort)(z - inZ), Block.green);
                    else Lvl.SetTile((ushort)(x - inX), (ushort)(y + yy), (ushort)(z - inZ), Block.green);
            }
        }

        private bool TreeCheck(Level Lvl, ushort x, ushort z, ushort y, short dist)         //return true if tree is near
        {
            byte foundTile;
            for (short xx = (short)-dist; xx <= +dist; ++xx)
            {
                for (short yy = (short)-dist; yy <= +dist; ++yy)
                {
                    for (short zz = (short)-dist; zz <= +dist; ++zz)
                    {
                        foundTile = Lvl.GetTile((ushort)(x + xx), (ushort)(z + zz), (ushort)(y + yy));
                        if (foundTile == Block.trunk || foundTile == Block.green)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        void AddTerrainHeight(float[] array, ushort x, ushort y, ushort width, float height)
        {
            int temp = x + y * width;
            if (temp < 0) return;
            if (temp > array.Length) return;

            array[temp] += height;

            if (array[temp] > 1.0f) array[temp] = 1.0f;
            if (array[temp] < 0.0f) array[temp] = 0.0f;
        }

        //converts the float into a ushort for map height
        ushort Evaluate(Level lvl, float height)
        {
            ushort temp = (ushort)(height * lvl.Height);
            if (temp < 0) return 0;
            if (temp > lvl.Height - 1) return (ushort)(lvl.Height - 1);
            return temp;
        }

        //applys the average filter
        void FilterAverage(Level Lvl)
        {
            Server.s.Log("Applying average filtering");

            float[] filtered = new float[terrain.Length];

            for (int bb = 0; bb < terrain.Length; bb++)
            {
                ushort x = (ushort)(bb % Lvl.Width);
                ushort y = (ushort)(bb / Lvl.Width);
                filtered[bb] = GetAverage9(x, y, Lvl);
            }

            for (int bb = 0; bb < terrain.Length; bb++)
            {
                terrain[bb] = filtered[bb];
            }
        }

        //Averages over 5 points
        float GetAverage5(ushort x, ushort y, Level Lvl)
        {
            divide = 0.0f;
            float temp = GetPixel(x, y, Lvl);
            temp += GetPixel((ushort)(x + 1), y, Lvl);
            temp += GetPixel((ushort)(x - 1), y, Lvl);
            temp += GetPixel(x, (ushort)(y + 1), Lvl);
            temp += GetPixel(x, (ushort)(y - 1), Lvl);

            return temp / divide;
        }
        //Averages over 9 points
        float GetAverage9(ushort x, ushort y, Level Lvl)
        {
            divide = 0.0f;
            float temp = GetPixel(x, y, Lvl);
            temp += GetPixel((ushort)(x + 1), y, Lvl);
            temp += GetPixel((ushort)(x - 1), y, Lvl);
            temp += GetPixel(x, (ushort)(y + 1), Lvl);
            temp += GetPixel(x, (ushort)(y - 1), Lvl);

            temp += GetPixel((ushort)(x + 1), (ushort)(y + 1), Lvl);
            temp += GetPixel((ushort)(x - 1), (ushort)(y + 1), Lvl);
            temp += GetPixel((ushort)(x + 1), (ushort)(y - 1), Lvl);
            temp += GetPixel((ushort)(x - 1), (ushort)(y - 1), Lvl);

            return temp / divide;
        }

        //returns the valve of a x,y terrain coordinate
        float GetPixel(ushort x, ushort y, Level Lvl)
        {
            if (x < 0) { return 0.0f; }
            if (x >= Lvl.Width) { return 0.0f; }
            if (y < 0) { return 0.0f; }
            if (y >= Lvl.Length) { return 0.0f; }
            divide += 1.0f;
            return terrain[x + y * Lvl.Width];
        }

        //converts the height into a range
        float Range(float input, float low, float high)
        {
            if (high <= low) { return low; }
            return low + (input * (high - low));
        }

        //Forces the edge of a map to slope lower for island map types
        float NegateEdge(ushort x, ushort y, Level Lvl)
        {
            float tempx = 0.0f, tempy = 0.0f;
            float temp;
            if (x != 0) { tempx = ((float)x / (float)Lvl.Width) * 0.5f; }
            if (y != 0) { tempy = ((float)y / (float)Lvl.Length) * 0.5f; }
            tempx = Math.Abs(tempx - 0.25f);
            tempy = Math.Abs(tempy - 0.25f);
            if (tempx > tempy)
            {
                temp = tempx - 0.15f;
            }
            else
            {
                temp = tempy - 0.15f;
            }

            //s.Log("temp = " + temp.ToString());
            if (temp > 0.0f) { return temp; }
            return 0.0f;
        }
    }
}