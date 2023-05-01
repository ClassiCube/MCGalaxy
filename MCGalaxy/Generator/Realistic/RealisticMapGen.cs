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
    public delegate void PreprocessGen(Level lvl, MapGenArgs args);
	
    public sealed class RealisticMapGen 
    {
        float[] terrain, overlay, overlayT;
        float treeDens;
        short treeDist;
        Random rng;
        ushort waterHeight;
        RealisticMapGenArgs args;
        MapGenBiome biome;
        Tree tree;
        
        public bool Gen(Player p, Level lvl, MapGenArgs gen_args, 
                        RealisticMapGenArgs args, PreprocessGen preprocessor) {
            gen_args.Biome = args.Biome;
            if (!gen_args.ParseArgs(p)) return false;
            
            rng   = new Random(gen_args.Seed);
            biome = MapGenBiome.Get(gen_args.Biome);
            
            if (preprocessor != null) preprocessor(lvl, gen_args);
            terrain = new float[lvl.Width * lvl.Length];
            overlay = new float[lvl.Width * lvl.Length];
            
            if (args.GenTrees) {
                overlayT = new float[lvl.Width * lvl.Length];
                tree     = biome.GetTreeGen("Fern");
            }
            
            this.args   = args;
            waterHeight = args.GetLiquidLevel(lvl.Height);
            waterHeight = (ushort)Math.Min(waterHeight, lvl.MaxY);

            GenerateFault(terrain, lvl);
            FilterAverage(lvl);
            Logger.Log(LogType.SystemActivity, "Generating terrain..");
            GeneratePerlinNoise(overlay, lvl);

            if (args.GenOverlay2) {
                GeneratePerlinNoise(overlayT, lvl);
            }

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
                
                if (height > waterHeight) {
                    GenAboveWaterColumn(x, height, z, lvl, i);
                    GenFoliage(x, height, z, lvl, i);
                } else {
                    GenUnderwaterColumn(x, height, z, lvl, i);
                }
            }
            return true;
        }
        
        void GenAboveWaterColumn(ushort x, ushort height, ushort z, Level lvl, int index) {
            int pos = x + lvl.Width * (z + height * lvl.Length);
            
            if (args.SimpleColumns) {
                lvl.blocks[pos] = biome.Surface;
                pos -= lvl.Width * lvl.Length;
                    
                for (ushort yy = 1; height - yy >= 0; yy++) 
                {
                    lvl.blocks[pos] = biome.Ground;
                    pos -= lvl.Width * lvl.Length;
                }
            } else if (!args.UseLavaLiquid) {
                for (ushort yy = 0; height - yy >= 0; yy++) 
                {
                    if (overlay[index] < 0.72f) {
                        if (args.IslandColumns && height <= waterHeight + 2) {
                            lvl.blocks[pos] = biome.BeachSandy; // extra sand for islands
                        } else {
                            if (yy == 0)     lvl.blocks[pos] = biome.Surface;
                            else if (yy < 3) lvl.blocks[pos] = biome.Ground;
                            else lvl.blocks[pos] = biome.Cliff;
                        }
                    } else {
                        lvl.blocks[pos] = biome.Cliff;
                    }
                    pos -= lvl.Width * lvl.Length;
                }
            } else {
                byte topBlock = Block.Air;
                
                for (ushort yy = 0; height - yy >= 0; yy++) 
                {
                    lvl.blocks[pos] = yy < 3 ? biome.Cliff : biome.BeachSandy; // TODO rethink this
                    pos -= lvl.Width * lvl.Length;
                    
                    // NOTE: Although your natural assumption would be that the following
                    //  code should be outside the for loop, for backwards compatibility
                    //  it must remain in the for loop (moving it out changes terrain generation)
                    
                    // add occasional lava pools on top of surface
                    if (overlay[index] < 0.3f && rng.Next(13) >= 9) {
                        lvl.SetTile(x, (ushort)(height + 1), z, biome.Water);
                    }
                    topBlock = rng.Next(100) % 3 == 1 ? Block.Black : biome.Surface;
                }               
                lvl.SetTile(x, height, z, topBlock);
            }
        }

        void GenFoliage(ushort x, ushort height, ushort z, Level lvl, int index) {
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
            
            if (tree != null && overlay[index] < 0.65f && overlayT[index] < treeDens) {
                if (lvl.IsAirAt(x, (ushort)(height + 1), z) && lvl.GetBlock(x, height, z) == biome.Surface) {
                    if (rng.Next(13) == 0 && !Tree.TreeCheck(lvl, x, height, z, treeDist)) {
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

        void GenUnderwaterColumn(ushort x, ushort height, ushort z, Level lvl, int index) {
            int pos = x + lvl.Width * (z + waterHeight * lvl.Length);
            byte block;
            
            if (!args.UseLavaLiquid) {                
                for (ushort yy = 0; waterHeight - yy >= 0; yy++) 
                {
                    if (waterHeight - yy > height) {
                        lvl.blocks[pos] = biome.Water;
                    } else if (waterHeight - yy > height - 3) {
                        block = overlay[index] < 0.75f ? biome.BeachSandy : biome.BeachRocky;
                        lvl.blocks[pos] = block;
                    } else {
                        lvl.blocks[pos] = biome.Cliff;
                    }
                    pos -= lvl.Width * lvl.Length;
                }
            } else {
                for (ushort yy = 0; waterHeight - yy >= 0; yy++) 
                {
                    if (waterHeight - yy > height - 1) {
                        lvl.blocks[pos] = biome.Water;
                    } else if (waterHeight - yy > height - 3) {
                        if (overlay[index] < 0.9f) {
                            lvl.blocks[pos] = yy < height ? biome.Water : biome.BeachSandy;
                        } else {
                            lvl.blocks[pos] = biome.Water;
                        }
                    } else {
                        lvl.blocks[pos] = biome.Bedrock;
                    }
                    pos -= lvl.Width * lvl.Length;
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
        static float NegateEdge(ushort x, ushort z, Level lvl) {
            float xAdj = ((float)x / (float)lvl.Width ) * 0.5f;
            float zAdj = ((float)z / (float)lvl.Length) * 0.5f;
            float adj;
            
            xAdj = Math.Abs(xAdj - 0.25f);
            zAdj = Math.Abs(zAdj - 0.25f);
            
            if (xAdj > zAdj)
                adj = xAdj - 0.15f;
            else
                adj = zAdj - 0.15f;
            return adj > 0 ? adj : 0;
        }
        
        
        public static void RegisterGenerators() {
            const GenType type = GenType.Simple;
            MapGen.Register("Island",    type, GenIsland,    MapGen.DEFAULT_HELP);
            MapGen.Register("Mountains", type, GenMountains, MapGen.DEFAULT_HELP);
            MapGen.Register("Forest",    type, GenForest,    MapGen.DEFAULT_HELP);
            MapGen.Register("Ocean",     type, GenOcean,     MapGen.DEFAULT_HELP);
            MapGen.Register("Desert",    type, GenDesert,  MapGen.DEFAULT_HELP);
            MapGen.Register("Hell",      type, GenHell,    MapGen.DEFAULT_HELP);
        }
        
        static bool GenIsland(Player p, Level lvl, MapGenArgs args) {
            return GenRealistic(p, lvl, args, RealisticMapGenArgs.Island);
        }
        
        static bool GenMountains(Player p, Level lvl, MapGenArgs args) {
            return GenRealistic(p, lvl, args, RealisticMapGenArgs.Mountains);
        }
        
        static bool GenForest(Player p, Level lvl, MapGenArgs args) {
            return GenRealistic(p, lvl, args, RealisticMapGenArgs.Forest);
        }
        
        static bool GenOcean(Player p, Level lvl, MapGenArgs args) {
            return GenRealistic(p, lvl, args, RealisticMapGenArgs.Ocean);
        }
        
        static bool GenDesert(Player p, Level lvl, MapGenArgs args) {
            return GenRealistic(p, lvl, args, RealisticMapGenArgs.Desert);
        }
        
        static bool GenHell(Player p, Level lvl, MapGenArgs args) {
            return GenRealistic(p, lvl, args, RealisticMapGenArgs.Hell, PreprocessHell);
        }
        
        static void PreprocessHell(Level lvl, MapGenArgs args) {
            Random rng = new Random(args.Seed);
            int width = lvl.Width, height = lvl.Height, length = lvl.Length;
            int index = 0, oneY = width * length;
            
            MapGenBiome biome = MapGenBiome.Get(args.Biome);
            byte[] blocks = lvl.blocks;
            
            // first layer used to be bedrock, but is now skipped over
            //  (since map generation will just replace it anyways)
            index += oneY;
            
            for (int y = 1; y < height; ++y)
                for (int z = 0; z < length; ++z)
                    for (int x = 0; x < width; ++x)
            {
            	if (x == 0 || x == width - 1 || z == 0 || z == length - 1 || y == height - 1) {
                    blocks[index] = biome.BeachRocky;
                } else if (x == 1 || x == width - 2 || z == 1 || z == length - 2) {
                    if (rng.Next(1000) != 7) { index++; continue; }
                    
                    int colIndex = z * width + x;
                    for (int i = 1; i < (height - y); ++i) 
                    {
                        int yy = height - i;
                        blocks[colIndex + yy * oneY] = biome.Water;
                    }
                }
                index++;
            }
        }
        
        static bool GenRealistic(Player p, Level lvl, MapGenArgs gen_args, 
                                 RealisticMapGenArgs args, PreprocessGen preprocessor = null) {
            return new RealisticMapGen().Gen(p, lvl, gen_args, args, preprocessor);
        }
    }
}