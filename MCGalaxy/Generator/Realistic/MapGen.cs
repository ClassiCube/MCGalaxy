/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
using MCGalaxy.Generator.Foliage;

namespace MCGalaxy.Generator.Realistic 
{
    public sealed class RealisticMapGen 
    {
        float[] terrain, overlay, overlay2;
        float treeDens;
        short treeDist;
        Random rng;
        ushort waterHeight;
        RealisticMapGenArgs args;
        
        public bool Gen(Player p, Level lvl, string seed, RealisticMapGenArgs args) {
            this.args = args;
            int rng_seed;
            if (!MapGen.ParseArgs(p, seed, out rng_seed, ref args.Theme)) return false;
            
            rng = new Random(rng_seed);
            MapGenTheme theme = MapGenTheme.Get(args.Theme);
            theme.ApplyEnv(lvl.Config);
            
            terrain = new float[lvl.Width * lvl.Length];
            overlay = new float[lvl.Width * lvl.Length];
            if (args.GenTrees) overlay2 = new float[lvl.Width * lvl.Length];
            
            waterHeight = args.GetLiquidLevel(lvl.Height);
            waterHeight = (ushort)Math.Min(waterHeight, lvl.MaxY);

            GenerateFault(terrain, lvl);
            FilterAverage(lvl);
            Logger.Log(LogType.SystemActivity, "Creating overlay");
            GeneratePerlinNoise(overlay, lvl);

            if (args.GenOverlay2) {
                Logger.Log(LogType.SystemActivity, "Planning trees");
                GeneratePerlinNoise(overlay2, lvl);
            }

            Logger.Log(LogType.SystemActivity, "Converting height map, and applying overlays");
            float rangeLo = args.RangeLow;
            float rangeHi = args.RangeHigh;
            treeDens = args.TreeDensity;
            treeDist = args.TreeDistance;

            //loops though evey X/Z coordinate
            for (int i = 0; i < terrain.Length; i++) {
                ushort x = (ushort)(i % lvl.Width);
                ushort z = (ushort)(i / lvl.Width); // TODO don't % /
                ushort height;
                
                if (args.FalloffEdges) {
                    float offset = NegateEdge(x, z, lvl);
                    height = Evaluate(lvl, Range(terrain[i], rangeLo, rangeHi) - offset);
                } else {
                    height = Evaluate(lvl, Range(terrain[i], rangeLo, rangeHi));
                }
                
                if (!args.UseLavaLiquid)
                    GenNonLavaColumn(x, height, z, lvl, i);
                else
                    GenLavaColumn(x, height, z, lvl, i);
            }
            return true;
        }

        void GenNonLavaColumn(ushort x, ushort height, ushort z, Level lvl, int index) {
            if (height > waterHeight) {
                int pos = x + lvl.Width * (z + height * lvl.Length);
                for (ushort yy = 0; height - yy >= 0; yy++) 
                {
                    if (args.SimpleColumns) {
                        lvl.blocks[pos] = Block.Sand;
                    } else if (overlay[index] < 0.72f) {
                        if (args.IslandColumns) { //increase sand height for island
                            if (height > waterHeight + 2) {
                                if (yy == 0)     lvl.blocks[pos] = Block.Grass; //top layer
                                else if (yy < 3) lvl.blocks[pos] = Block.Dirt;  //next few
                                else lvl.blocks[pos] = Block.Stone;              //ten rock it
                            } else {
                                lvl.blocks[pos] = Block.Sand;                   //SAAAND extra for islands
                            } // TODO avoid redundant else if chain above
                        } else {
                            if (yy == 0)     lvl.blocks[pos] = Block.Grass;
                            else if (yy < 3) lvl.blocks[pos] = Block.Dirt;
                            else lvl.blocks[pos] = Block.Stone;
                        }
                    } else {
                        lvl.blocks[pos] = Block.Stone;
                    }
                    pos -= lvl.Width * lvl.Length;
                }

                if (args.GenFlowers && overlay[index] < 0.25f) {
                    switch (rng.Next(12)) {
                        case 10:
                            lvl.SetTile(x, (ushort)(height + 1), z, Block.Rose); break;
                        case 11:
                            lvl.SetTile(x, (ushort)(height + 1), z, Block.Dandelion); break;
                        default:
                            break;
                    }
                }
                
                if (args.GenTrees && overlay[index] < 0.65f && overlay2[index] < treeDens) {
                    if (lvl.IsAirAt(x, (ushort)(height + 1), z)) {
                        if (lvl.GetBlock(x, height, z) == Block.Grass || args.UseCactus) {
                            if (rng.Next(13) == 0 && !Tree.TreeCheck(lvl, x, height, z, treeDist)) {
                                Tree tree = null;
                                if (args.UseCactus) tree = new CactusTree();
                                else tree = new NormalTree();
                                
                                tree.SetData(rng, tree.DefaultSize(rng));
                                tree.Generate(x, (ushort)(height + 1), z, (xT, yT, zT, bT) =>
                                            {
                                                if (lvl.IsAirAt(xT, yT, zT))
                                                    lvl.SetTile(xT, yT, zT, (byte)bT);
                                            });
                            }
                        }
                    }
                }
            } else { //Must be on/under the water line then
                int pos = x + lvl.Width * (z + waterHeight * lvl.Length);
                for (ushort yy = 0; waterHeight - yy >= 0; yy++) 
                {
                    if (waterHeight - yy > height) {
                        lvl.blocks[pos] = Block.StillWater;    //better fill the water above me
                    } else if (waterHeight - yy > height - 3) {
                        byte block = overlay[index] < 0.75f ? Block.Sand : Block.Gravel; // sand on top
                        lvl.blocks[pos] = block;
                    } else {
                        lvl.blocks[pos] = Block.Stone;
                    }
                    pos -= lvl.Width * lvl.Length;
                }
            }
        }
        
        void GenLavaColumn(ushort x, ushort height, ushort z, Level lvl, int index) {
            byte block;
            if (height > waterHeight) {
                for (ushort yy = 0; height - yy >= 0; yy++) 
                {
            		block = yy < 3 ? Block.Stone : Block.Obsidian;
                    lvl.SetTile(x, (ushort)(height - yy), z, block);
                    
                    // NOTE: Although your natural assumption would be that the following
                    //  code should be outside the for loop, for backwards compatibility
                    //  it must remain in the for loop (moving it out changes terrain generation)
                    
                    // add occasional lava pools on top of surface
                    if (overlay[index] < 0.3f && rng.Next(13) >= 9) {
                        lvl.SetTile(x, (ushort)(height + 1), z, Block.Lava);
                    }
                    lvl.SetTile(x, height, z, (rng.Next(100) % 3 == 1 ? Block.Black : Block.Obsidian));
                }
            } else {
                for (ushort yy = 0; waterHeight - yy >= 0; yy++) 
                {
                    if (waterHeight - yy > height - 1) {
                        lvl.SetTile(x, (ushort)(waterHeight - yy), z, Block.Lava); //better fill the water above me
                    } else if (waterHeight - yy > height - 3) {
                        if (overlay[index] < 0.9f) {
                            block = yy < height ? Block.Lava : Block.Stone;
                            lvl.SetTile(x, (ushort)(height - yy), z, block);
                        } else {
                            lvl.SetTile(x, (ushort)(waterHeight - yy), (ushort)(z - 5), Block.Lava);  //killer lava
                        }
                    } else {
                        lvl.SetTile(x, (ushort)(waterHeight - yy), z, Block.Cobblestone); //and just make the rest cobblestone
                    }
                }
            }
        }


        // https://www.lighthouse3d.com/opengl/terrain/index.php?fault
        void GenerateFault(float[] array, Level lvl) {
            float baseHeight = args.StartHeight;
            float dispMax  = args.DisplacementMax;
            float dispStep = args.DisplacementStep;

            for (int i = 0; i < array.Length; i++)
                array[i] = baseHeight;
            float disp = dispMax;
            
            ushort halfX = (ushort)(lvl.Width / 2), halfZ = (ushort)(lvl.Length / 2);
            float d = (float)Math.Sqrt(halfX * halfX + halfZ * halfZ);
            int numIterations = lvl.Width + lvl.Length;
            Logger.Log(LogType.SystemActivity, "Iterations = " + numIterations);
            
            for (int iter = 0; iter < numIterations; iter++) 
            {
                float phi = (float)(rng.NextDouble() * 360);
                float cosPhi = (float)Math.Cos(phi);
                float sinPhi = (float)Math.Sin(phi);
                float c = ((float)rng.NextDouble()) * 2 * d - d;

                int index = 0;
                for (ushort z = 0; z < lvl.Length; z++) {
                    float value = (z - halfZ) * cosPhi + (0 - halfX) * sinPhi + c;
                    for (ushort x = 0; x < lvl.Width; x++) {
                        float sum = array[index] + (value > 0 ? disp : -disp);
                        sum = sum > 1 ? 1 : sum;
                        sum = sum < 0 ? 0 : sum;
                        array[index] = sum; index++;
                        value += sinPhi;
                    }
                }
                
                disp += dispStep;
                if (disp < -dispMax) 
                    disp = dispMax;
            }
        }

        void GeneratePerlinNoise(float[] array, Level Lvl) {
            NoiseGen.GenerateNormalized(array, 0.7f, 8, Lvl.Width, Lvl.Length, rng.Next(), 64);
        }

        //converts the float into a ushort for map height
        static ushort Evaluate(Level lvl, float height) {
            ushort y = (ushort)(height * lvl.Height);
            if (y < 0) return 0;
            if (y > lvl.Height - 1) return (ushort)(lvl.Height - 1); // TODO >= lvl.Height
            return y;
        }

        //applys the average filter
        void FilterAverage(Level lvl) {
            Logger.Log(LogType.SystemActivity, "Applying average filtering");
            float[] filtered = new float[terrain.Length];

            for (int i = 0; i < filtered.Length; i++) 
            {
                ushort x = (ushort)(i % lvl.Width);
                ushort z = (ushort)(i / lvl.Width);
                filtered[i] = GetAverage9(x, z, lvl);
            }

            for (int i = 0; i < terrain.Length; i++)
                terrain[i] = filtered[i];
        }

        //Averages over 9 points
        float GetAverage9(ushort x, ushort z, Level lvl) {
            int points = 0;
            float sum = GetPixel(ref points, x, z, lvl);
            sum += GetPixel(ref points, (ushort)(x + 1), z, lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), z, lvl);
            sum += GetPixel(ref points, x, (ushort)(z + 1), lvl);
            sum += GetPixel(ref points, x, (ushort)(z - 1), lvl);

            sum += GetPixel(ref points, (ushort)(x + 1), (ushort)(z + 1), lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), (ushort)(z + 1), lvl);
            sum += GetPixel(ref points, (ushort)(x + 1), (ushort)(z - 1), lvl);
            sum += GetPixel(ref points, (ushort)(x - 1), (ushort)(z - 1), lvl);

            return sum / points;
        }

        //returns the value of a x,y terrain coordinate
        float GetPixel(ref int points, ushort x, ushort z, Level lvl) {
            if (x < 0 || x >= lvl.Width || z < 0 || z >= lvl.Length)
                return 0;
            points++;
            return terrain[x + z * lvl.Width];
        }

        //converts the height into a range
        static float Range(float input, float low, float high) {
            if (high <= low) return low;
            return low + (input * (high - low));
        }

        //Forces the edge of a map to slope lower for island map types
        float NegateEdge(ushort x, ushort z, Level lvl) {
            float tempx = 0.0f, tempy = 0.0f;
            float temp;
            if (x != 0) { tempx = ((float)x / (float)lvl.Width) * 0.5f; }
            if (z != 0) { tempy = ((float)z / (float)lvl.Length) * 0.5f; }
            
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