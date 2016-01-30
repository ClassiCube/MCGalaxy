/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
    
    public sealed class RealisticMapGen {
        
        float[] terrain, overlay, overlay2;

        float treeDens;
        short treeDist;
        Random rand;
        ushort WaterLevel;
        MapGenParams genParams;
        
        public bool GenerateMap(Level Lvl, string type, int seed = 0, bool useSeed = false) {
            DateTime startTime = DateTime.UtcNow;
            Server.s.Log("Attempting map gen");
            rand = useSeed ? new System.Random(seed) : new System.Random();
            try
            {
                terrain = new float[Lvl.Width * Lvl.Length];  //hmm
                overlay = new float[Lvl.Width * Lvl.Length];
                if (type != "ocean")
                    overlay2 = new float[Lvl.Width * Lvl.Length];

                //float dispAux, pd;
                WaterLevel = (ushort)(Lvl.Height / 2 + 2);

                if (type == "ocean")
                    WaterLevel = (ushort)(Lvl.Height * 0.85f);
                else if (type == "desert")
                    WaterLevel = 0;
                else if (type == "hell")
                    WaterLevel = 5;
                //Generate the level
                GenerateFault(terrain, Lvl, type, rand);

                //APPLY FILTER to terrain
                FilterAverage(Lvl);

                //CREATE OVERLAY
                //GenerateFault(overlay, Lvl, "overlay", rand);
                Server.s.Log("Creating overlay");
                GeneratePerlinNoise(overlay, Lvl, rand);

                if (!type.Equals("ocean") && type != "desert")
                {
                    Server.s.Log("Planning trees");
                    GeneratePerlinNoise(overlay2, Lvl, rand);
                }

                Server.s.Log("Converting height map, and applying overlays");
                
                genParams = null;
                if (!MapGenParams.Themes.TryGetValue(type, out genParams))
                    genParams = new MapGenParams();
                float rangeLo = genParams.RangeLow;
                float rangeHi = genParams.RangeHigh;
                treeDens = genParams.TreeDens;
                treeDist = genParams.TreeDist;

                //loops though evey X/Z coordinate
                for (int bb = 0; bb < terrain.Length; bb++) {
                    ushort x = (ushort)(bb % Lvl.Width);
                    ushort z = (ushort)(bb / Lvl.Width);
                    ushort y;
                    if (type == "island") {
                        float offset = NegateEdge(x, z, Lvl);
                        y = Evaluate(Lvl, Range(terrain[bb], rangeLo - offset, rangeHi - offset));
                    } else {
                        y = Evaluate(Lvl, Range(terrain[bb], rangeLo, rangeHi));
                    }
                    
                    if (type != "hell")
                        GenNonLavaColumn(x, y, z, Lvl, bb, type);
                    else
                        GenLavaColumn(x, y, z, Lvl, bb, type);
                }
                Server.s.Log("Total time was " + (DateTime.UtcNow - startTime).TotalSeconds.ToString() + " seconds.");
            } catch (Exception e) {
                Server.ErrorLog(e);
                Server.s.Log("Gen Fail");
                return false;
            }
            return true;
        }

        void GenNonLavaColumn(ushort x, ushort y, ushort z, Level Lvl, int bb, string type) {
            if (y > WaterLevel) {
                for (ushort yy = 0; y - yy >= 0; yy++) {
                    if (type == "desert") {
                        Lvl.SetTile(x, (ushort)(y - yy), z, Block.sand);
                    } else if (overlay[bb] < 0.72f) {
                        if (type == "island") { //increase sand height for island
                            if (y > WaterLevel + 2) {
                                if (yy == 0) Lvl.SetTile(x, (ushort)(y - yy), z, Block.grass);     //top layer
                                else if (yy < 3) Lvl.SetTile(x, (ushort)(y - yy), z, Block.dirt);  //next few
                                else Lvl.SetTile(x, (ushort)(y - yy), z, Block.rock);              //ten rock it
                            } else {
                                Lvl.SetTile(x, (ushort)(y - yy), z, Block.sand);                   //SAAAND extra for islands
                            }
                        } else {
                            if (yy == 0) Lvl.SetTile(x, (ushort)(y - yy), z, Block.grass);
                            else if (yy < 3) Lvl.SetTile(x, (ushort)(y - yy), z, Block.dirt);
                            else Lvl.SetTile(x, (ushort)(y - yy), z, Block.rock);
                        }
                    } else {
                        Lvl.SetTile(x, (ushort)(y - yy), z, Block.rock);
                    }
                }

                if (genParams.HasFlowers && overlay[bb] < 0.25f) {
                    switch (rand.Next(12)) {
                        case 10:
                            Lvl.SetTile(x, (ushort)(y + 1), z, Block.redflower); break;
                        case 11:
                            Lvl.SetTile(x, (ushort)(y + 1), z, Block.yellowflower); break;
                        default:
                            break;
                    }
                }
            
                if (genParams.GenTrees && overlay[bb] < 0.65f && overlay2[bb] < treeDens) {
                    if (Lvl.GetTile(x, (ushort)(y + 1), z) == Block.air) {
                        if (Lvl.GetTile(x, y, z) == Block.grass || type == "desert")
                        {
                            if (rand.Next(13) == 0 && !TreeGen.TreeCheck(Lvl, x, y, z, treeDist))
                                if (type == "desert")
                                    TreeGen.AddCactus(Lvl, x, (ushort)(y + 1), z, rand);
                                else
                                    TreeGen.AddTree(Lvl, x, (ushort)(y + 1), z, rand);
                        }
                    }
                }
            } else { //Must be on/under the water line then
                for (ushort yy = 0; WaterLevel - yy >= 0; yy++) {
                    if (WaterLevel - yy > y) {
                        Lvl.SetTile(x, (ushort)(WaterLevel - yy), z, Block.water);    //better fill the water above me
                    } else if (WaterLevel - yy > y - 3) {
                        byte block = overlay[bb] < 0.75f ? Block.sand : Block.gravel; // sand on top
                        Lvl.SetTile(x, (ushort)(WaterLevel - yy), z, block);
                    } else {
                        Lvl.SetTile(x, (ushort)(WaterLevel - yy), z, Block.rock);
                    }
                }
            }
        }
        
        void GenLavaColumn(ushort x, ushort y, ushort z, Level Lvl, int bb, string type) {
            if (y > WaterLevel) {
                for (ushort yy = 0; y - yy >= 0; yy++) {
                    if (y > (WaterLevel - 1)) {
                        if (yy == 0) Lvl.SetTile(x, (ushort)(y - yy), z, Block.rock);      //top layer
                        else if (yy < 3) Lvl.SetTile(x, (ushort)(y - yy), z, Block.rock);
                        else if (yy < 2) Lvl.SetTile(x, (ushort)(y - yy), z, Block.lava);//next few
                        else Lvl.SetTile(x, (ushort)(y - yy), z, Block.obsidian);
                    } else {
                        Lvl.SetTile(x, (ushort)(y - yy), z, Block.lava);
                    }
                    
                    if (overlay[bb] < 0.3f) {
                        switch (rand.Next(13)) {
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                                Lvl.SetTile(x, (ushort)(y + 1), z, Block.lava); //change to lava when time
                                break;
                            default:
                                break;
                        }
                    }
                    Lvl.SetTile(x, (ushort)(y), z, (rand.Next(100) % 3 == 1 ? Block.darkgrey : Block.obsidian));
                }
            } else {
                for (ushort yy = 0; WaterLevel - yy >= 0; yy++) {
                    if (WaterLevel - yy > y - 1) { 
                        Lvl.SetTile(x, (ushort)(WaterLevel - yy), z, Block.lava); //better fill the water above me
                    } else if (WaterLevel - yy > y - 3) {
                        if (overlay[bb] < 0.9f) {
                            byte block = yy < yy ? Block.lava : Block.rock;
                            Lvl.SetTile(x, (ushort)(y - yy), z, block);
                        } else {
                            Lvl.SetTile(x, (ushort)(WaterLevel - yy), (ushort)(z - 5), Block.lava);  //killer lava
                        }
                    } else {
                        Lvl.SetTile(x, (ushort)(WaterLevel - yy), z, Block.stone); //and just make the rest cobblestone
                    }
                }
            }
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

        void GeneratePerlinNoise(float[] array, Level Lvl, Random rand) {
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
            ushort y = (ushort)(height * lvl.Height);
            if (y < 0) return 0;
            if (y > lvl.Height - 1) return (ushort)(lvl.Height - 1);
            return y;
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
        float GetAverage5(ushort x, ushort y, Level Lvl) {
            int points = 0;
            float sum = GetPixel(ref points, x, y, Lvl);
            sum += GetPixel(ref points, (ushort)(x + 1), y, Lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), y, Lvl);
            sum += GetPixel(ref points, x, (ushort)(y + 1), Lvl);
            sum += GetPixel(ref points, x, (ushort)(y - 1), Lvl);

            return sum / points;
        }
        //Averages over 9 points
        float GetAverage9(ushort x, ushort y, Level Lvl) {
            int points = 0;
            float sum = GetPixel(ref points, x, y, Lvl);
            sum += GetPixel(ref points, (ushort)(x + 1), y, Lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), y, Lvl);
            sum += GetPixel(ref points, x, (ushort)(y + 1), Lvl);
            sum += GetPixel(ref points, x, (ushort)(y - 1), Lvl);

            sum += GetPixel(ref points, (ushort)(x + 1), (ushort)(y + 1), Lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), (ushort)(y + 1), Lvl);
            sum += GetPixel(ref points, (ushort)(x + 1), (ushort)(y - 1), Lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), (ushort)(y - 1), Lvl);

            return sum / points;
        }

        //returns the value of a x,y terrain coordinate
        float GetPixel(ref int points, ushort x, ushort y, Level Lvl) {
            if (x < 0 || x >= Lvl.Width || y < 0 || y >= Lvl.Length)
                return 0;
            points++;
            return terrain[x + y * Lvl.Width];
        }

        //converts the height into a range
        float Range(float input, float low, float high) {
            if (high <= low) return low;
            return low + (input * (high - low));
        }

        //Forces the edge of a map to slope lower for island map types
        float NegateEdge(ushort x, ushort y, Level Lvl) {
            float tempx = 0.0f, tempy = 0.0f;
            float temp;
            if (x != 0) { tempx = ((float)x / (float)Lvl.Width) * 0.5f; }
            if (y != 0) { tempy = ((float)y / (float)Lvl.Length) * 0.5f; }
            tempx = Math.Abs(tempx - 0.25f);
            tempy = Math.Abs(tempy - 0.25f);
            
            if (tempx > tempy)
                temp = tempx - 0.15f;
            else
                temp = tempy - 0.15f;
            return temp > 0 ? temp : 0;
        }
    }
}