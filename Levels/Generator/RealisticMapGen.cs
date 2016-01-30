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
                                                if (!TreeGen.TreeCheck(Lvl, x, z, y, TreeDist))
                                                {
                                                    if (type == "desert")
                                                        TreeGen.AddCactus(Lvl, x, (ushort)(z + 1), y, rand);
                                                    else
                                                        TreeGen.AddTree(Lvl, x, (ushort)(z + 1), y, rand);
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

        void GeneratePerlinNoise(float[] array, Level Lvl, string type, Random rand) {
            NoiseGen.GenerateNormalized(array, 0.7f, 8, Lvl.Width, Lvl.Length, rand.Next(), 64);
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