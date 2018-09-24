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
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Generator.Foliage;

namespace MCGalaxy.Generator {    
    public sealed class RealisticMapGen {        
        float[] terrain, overlay, overlay2;
        float treeDens;
        short treeDist;
        Random rng;
        ushort LiquidLevel;
        RealisticMapGenArgs args;
        
        public bool Gen(Player p, Level lvl, string seed, RealisticMapGenArgs args) {
            DateTime start = DateTime.UtcNow;
            Logger.Log(LogType.SystemActivity, "Attempting map gen");
            rng = MapGen.MakeRng(seed);
            this.args = args;
            
            try {
                terrain = new float[lvl.Width * lvl.Length];
                overlay = new float[lvl.Width * lvl.Length];
                if (args.GenTrees) overlay2 = new float[lvl.Width * lvl.Length];
                LiquidLevel = args.GetLiquidLevel(lvl.Height);

                GenerateFault(terrain, lvl);
                FilterAverage(lvl);
                Logger.Log(LogType.SystemActivity, "Creating overlay");
                GeneratePerlinNoise(overlay, lvl);

                if (args.GenerateOverlay2) {
                    Logger.Log(LogType.SystemActivity, "Planning trees");
                    GeneratePerlinNoise(overlay2, lvl);
                }

                Logger.Log(LogType.SystemActivity, "Converting height map, and applying overlays");
                float rangeLo = args.RangeLow;
                float rangeHi = args.RangeHigh;
                treeDens = args.TreeDensity;
                treeDist = args.TreeDistance;

                //loops though evey X/Z coordinate
                for (int index = 0; index < terrain.Length; index++) {
                    ushort x = (ushort)(index % lvl.Width);
                    ushort z = (ushort)(index / lvl.Width);
                    ushort y;
                    if (args.FalloffEdges) {
                        float offset = NegateEdge(x, z, lvl);
                        y = Evaluate(lvl, Range(terrain[index], rangeLo - offset, rangeHi - offset));
                    } else {
                        y = Evaluate(lvl, Range(terrain[index], rangeLo, rangeHi));
                    }
                    
                    if (!args.UseLavaLiquid)
                        GenNonLavaColumn(x, y, z, lvl, index);
                    else
                        GenLavaColumn(x, y, z, lvl, index);
                }
                Logger.Log(LogType.SystemActivity, "Total time was {0} seconds.", (DateTime.UtcNow - start).TotalSeconds);
            } catch (Exception e) {
                Logger.LogError(e);
                p.Message("%WGeneration failed. See error logs.");
                return false;
            }
            return true;
        }

        void GenNonLavaColumn(ushort x, ushort y, ushort z, Level Lvl, int index) {
            if (y > LiquidLevel) {
                int pos = x + Lvl.Width * (z + y * Lvl.Length);
                for (ushort yy = 0; y - yy >= 0; yy++) {
                    if (args.SimpleColumns) {
                        Lvl.blocks[pos] = Block.Sand;
                    } else if (overlay[index] < 0.72f) {
                        if (args.IslandColumns) { //increase sand height for island
                            if (y > LiquidLevel + 2) {
                                if (yy == 0) Lvl.blocks[pos] = Block.Grass;     //top layer
                                else if (yy < 3) Lvl.blocks[pos] = Block.Dirt;  //next few
                                else Lvl.blocks[pos] = Block.Stone;              //ten rock it
                            } else {
                                Lvl.blocks[pos] = Block.Sand;                   //SAAAND extra for islands
                            }
                        } else {
                            if (yy == 0) Lvl.blocks[pos] = Block.Grass;
                            else if (yy < 3) Lvl.blocks[pos] = Block.Dirt;
                            else Lvl.blocks[pos] = Block.Stone;
                        }
                    } else {
                        Lvl.blocks[pos] = Block.Stone;
                    }
                    pos -= Lvl.Width * Lvl.Length;
                }

                if (args.GenFlowers && overlay[index] < 0.25f) {
                    switch (rng.Next(12)) {
                        case 10:
                            Lvl.SetTile(x, (ushort)(y + 1), z, Block.Rose); break;
                        case 11:
                            Lvl.SetTile(x, (ushort)(y + 1), z, Block.Dandelion); break;
                        default:
                            break;
                    }
                }
                
                if (args.GenTrees && overlay[index] < 0.65f && overlay2[index] < treeDens) {
                    if (Lvl.IsAirAt(x, (ushort)(y + 1), z)) {
                        if (Lvl.GetBlock(x, y, z) == Block.Grass || args.UseCactus) {
                            if (rng.Next(13) == 0 && !Tree.TreeCheck(Lvl, x, y, z, treeDist)) {
                                Tree tree = null;
                                if (args.UseCactus) tree = new CactusTree();
                                else tree = new NormalTree();
                                
                                tree.SetData(rng, tree.DefaultSize(rng));
                                tree.Generate(x, (ushort)(y + 1), z, (xT, yT, zT, bT) =>
                                            {
                                                if (Lvl.IsAirAt(xT, yT, zT))
                                                    Lvl.SetTile(xT, yT, zT, bT);
                                            });
                            }
                        }
                    }
                }
            } else { //Must be on/under the water line then
                int pos = x + Lvl.Width * (z + LiquidLevel * Lvl.Length);
                for (ushort yy = 0; LiquidLevel - yy >= 0; yy++) {
                    if (LiquidLevel - yy > y) {
                        Lvl.blocks[pos] = Block.StillWater;    //better fill the water above me
                    } else if (LiquidLevel - yy > y - 3) {
                        byte block = overlay[index] < 0.75f ? Block.Sand : Block.Gravel; // sand on top
                        Lvl.blocks[pos] = block;
                    } else {
                        Lvl.blocks[pos] = Block.Stone;
                    }
                    pos -= Lvl.Width * Lvl.Length;
                }
            }
        }
        
        void GenLavaColumn(ushort x, ushort y, ushort z, Level Lvl, int index) {
            if (y > LiquidLevel) {
                for (ushort yy = 0; y - yy >= 0; yy++) {
                    if (y > (LiquidLevel - 1)) {
                        if (yy == 0) Lvl.SetTile(x, (ushort)(y - yy), z, Block.Stone);      //top layer
                        else if (yy < 3) Lvl.SetTile(x, (ushort)(y - yy), z, Block.Stone);
                        else if (yy < 2) Lvl.SetTile(x, (ushort)(y - yy), z, Block.Lava);//next few
                        else Lvl.SetTile(x, (ushort)(y - yy), z, Block.Obsidian);
                    } else {
                        Lvl.SetTile(x, (ushort)(y - yy), z, Block.Lava);
                    }
                    
                    if (overlay[index] < 0.3f) {
                        switch (rng.Next(13)) {
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                                Lvl.SetTile(x, (ushort)(y + 1), z, Block.Lava); //change to lava when time
                                break;
                            default:
                                break;
                        }
                    }
                    Lvl.SetTile(x, (ushort)(y), z, (rng.Next(100) % 3 == 1 ? Block.Black : Block.Obsidian));
                }
            } else {
                for (ushort yy = 0; LiquidLevel - yy >= 0; yy++) {
                    if (LiquidLevel - yy > y - 1) {
                        Lvl.SetTile(x, (ushort)(LiquidLevel - yy), z, Block.Lava); //better fill the water above me
                    } else if (LiquidLevel - yy > y - 3) {
                        if (overlay[index] < 0.9f) {
                            byte block = yy < y ? Block.Lava : Block.Stone;
                            Lvl.SetTile(x, (ushort)(y - yy), z, block);
                        } else {
                            Lvl.SetTile(x, (ushort)(LiquidLevel - yy), (ushort)(z - 5), Block.Lava);  //killer lava
                        }
                    } else {
                        Lvl.SetTile(x, (ushort)(LiquidLevel - yy), z, Block.Cobblestone); //and just make the rest cobblestone
                    }
                }
            }
        }
        
        
        void GenerateFault(float[] array, Level Lvl) {
            float baseHeight = args.StartHeight;
            float dispMax = args.DisplacementMax;
            float dispStep = args.DisplacementStep;

            for (int i = 0; i < array.Length; i++)
                array[i] = baseHeight;
            float disp = dispMax;
            
            ushort halfX = (ushort)(Lvl.Width / 2), halfZ = (ushort)(Lvl.Length / 2);
            float d = (float)Math.Sqrt(halfX * halfX + halfZ * halfZ);
            int numIterations = Lvl.Width + Lvl.Length;
            Logger.Log(LogType.SystemActivity, "Iterations = " + numIterations);
            
            for (int iter = 0; iter < numIterations; iter++) {            
                float phi = (float)(rng.NextDouble() * 360);
                float cosPhi = (float)Math.Cos(phi);
                float sinPhi = (float)Math.Sin(phi);
                float c = ((float)rng.NextDouble()) * 2 * d - d;

                int index = 0;
                for (ushort z = 0; z < Lvl.Length; z++) {
                    float value = (z - halfZ) * cosPhi + (0 - halfX) * sinPhi + c;
                    for (ushort x = 0; x < Lvl.Width; x++) {
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
        ushort Evaluate(Level lvl, float height) {
            ushort y = (ushort)(height * lvl.Height);
            if (y < 0) return 0;
            if (y > lvl.Height - 1) return (ushort)(lvl.Height - 1);
            return y;
        }

        //applys the average filter
        void FilterAverage(Level Lvl) {
            Logger.Log(LogType.SystemActivity, "Applying average filtering");
            float[] filtered = new float[terrain.Length];

            for (int index = 0; index < terrain.Length; index++) {
                ushort x = (ushort)(index % Lvl.Width);
                ushort y = (ushort)(index / Lvl.Width);
                filtered[index] = GetAverage9(x, y, Lvl);
            }

            for (int bb = 0; bb < terrain.Length; bb++)
                terrain[bb] = filtered[bb];
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
        float NegateEdge(ushort x, ushort z, Level Lvl) {
            float tempx = 0.0f, tempy = 0.0f;
            float temp;
            if (x != 0) { tempx = ((float)x / (float)Lvl.Width) * 0.5f; }
            if (z != 0) { tempy = ((float)z / (float)Lvl.Length) * 0.5f; }
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