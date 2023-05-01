// Part of fCraft | Copyright 2009-2015 Matvei Stefarov <me@matvei.org> | BSD-3 | See LICENSE.txt
using System;
using MCGalaxy.Commands;
using MCGalaxy.Generator.Foliage;
using BlockID = System.UInt16;

namespace MCGalaxy.Generator.fCraft {

    /// <summary> Map generator template. Templates define landscape shapes and features. </summary>
    public enum MapGenTemplate {
        Archipelago, Atoll, Bay, Dunes, Hills, Ice, Island2,
        Lake, Mountains2, Peninsula, Random, River, Streams,
        Count
    }


    /// <summary> Provides functionality for generating map files. </summary>
    public sealed class fCraftMapGen 
    {
        readonly fCraftMapGenArgs args;
        readonly Random rand;
        readonly Noise noise;
        float[] heightmap, slopemap;
        ushort[] surfaceMap; // height of tallest non-air block
        int _width, _length;

        // theme-dependent vars
        byte bGroundSurface, bWater, bGround, bSeaFloor, bBedrock, bCliff;
        int groundThickness = 5;
        const int SeaFloorThickness = 3;
        Tree tree;

        public fCraftMapGen( fCraftMapGenArgs generatorArgs ) {
            args  = generatorArgs;
            rand  = new Random( args.Seed );
            noise = new Noise( args.Seed, NoiseInterpolationMode.Bicubic );
        }


        public void Generate(Level map) {
            _width  = map.Width;
            _length = map.Length;
            
            ApplyBiome(map);
            GenerateHeightmap(map);
            GenerateMap(map);
        }

        void ReportProgress(int relativeIncrease, string message) {
            Logger.Log(LogType.SystemActivity, message);
        }
        
        void ApplyBiome(Level map) {
            MapGenBiome biome = MapGenBiome.Get(args.Biome);
            bGroundSurface    = biome.Surface;
            bWater    = biome.Water;
            bGround   = biome.Ground;
            bSeaFloor = biome.BeachSandy;
            bBedrock  = biome.Bedrock;
            bCliff    = biome.Cliff;
            
            args.AddWater = biome.Water != Block.Air;
            args.AddTrees = biome.TreeType != null;
            
            // TODO: temp hack, need a better solution
            if (args.Biome == MapGenBiomeName.Arctic) groundThickness = 1;
            
            tree = biome.GetTreeGen("fCraft");
        }
        

        #region Heightmap Processing

        void GenerateHeightmap(Level map) {
            ReportProgress( 10, "Heightmap: Priming" );
            heightmap  = new float[_width * _length];
            surfaceMap = new ushort[_width * _length];

            noise.PerlinNoise( heightmap, _width, _length, 
                              args.FeatureScale, args.DetailScale, args.Roughness );

            if( args.UseBias && !args.DelayBias ) {
                ReportProgress( 2, "Heightmap: Biasing" );
                Noise.Normalize( heightmap );
                ApplyBias();
            }

            Noise.Normalize( heightmap );

            if( args.MarbledHeightmap ) {
                ReportProgress( 1, "Heightmap: Marbling" );
                Noise.Marble( heightmap );
            }

            if( args.InvertHeightmap ) {
                ReportProgress( 1, "Heightmap: Inverting" );
                Noise.Invert( heightmap );
            }

            if( args.UseBias && args.DelayBias ) {
                ReportProgress( 2, "Heightmap: Biasing" );
                Noise.Normalize( heightmap );
                ApplyBias();
            }
            Noise.Normalize( heightmap );
        }


        void ApplyBias() {
            // set corners and midpoint
            float[] corners = new float[4];
            int c = 0;
            for( int i = 0; i < args.RaisedCorners; i++ ) {
                corners[c++] = args.Bias;
            }
            for( int i = 0; i < args.LoweredCorners; i++ ) {
                corners[c++] = -args.Bias;
            }
            float midpoint = (args.MidPoint * args.Bias);

            // shuffle corners
            int[] keys = new int[corners.Length];
            for (int i = 0; i < corners.Length; i++) { keys[i] = rand.Next(); }
            Array.Sort(keys, corners);

            // overlay the bias
            Noise.ApplyBias( heightmap, _width, _length,
                            corners[0], corners[1], corners[2], corners[3], midpoint );
        }

        #endregion


        #region Map Processing

        void GenerateMap(Level map) {
            // Match water coverage
            float desiredWaterLevel = .5f;
            if( args.MatchWaterCoverage ) {
                ReportProgress( 2, "Heightmap Processing: Matching water coverage" );
                desiredWaterLevel = Noise.FindThreshold( heightmap, args.WaterCoverage );
            }


            // Calculate above/below water multipliers
            float aboveWaterMultiplier = 0;
            if( desiredWaterLevel != 1 ) {
                aboveWaterMultiplier = (args.MaxHeight / (1 - desiredWaterLevel));
            }

            // Calculate the slope
            float[] blurred;
            if( args.CliffSmoothing ) {
                ReportProgress( 2, "Heightmap Processing: Smoothing" );
                blurred  = Noise.GaussianBlur5X5( heightmap, _width, _length );
                slopemap = Noise.CalculateSlope( blurred, _width, _length );
            } else {
                slopemap = Noise.CalculateSlope( heightmap, _width, _length );
            }

            float[] altmap = null;
            if( args.MaxHeightVariation != 0 || args.MaxDepthVariation != 0 ) {
                ReportProgress( 5, "Heightmap Processing: Randomizing" );
                altmap = new float[_width * _length];
                int blendmapDetailSize = (int)Math.Log( Math.Max( _width, _length ), 2 ) - 2;
                
                new Noise( rand.Next(), NoiseInterpolationMode.Cosine )
                    .PerlinNoise( altmap, _width, _length, 3, blendmapDetailSize, 0.5f );
                Noise.Normalize( altmap, -1, 1 );
            }


            ReportProgress( 10, "Filling" );
            Fill( map, desiredWaterLevel, aboveWaterMultiplier, altmap );

            if( args.AddBeaches ) {
                ReportProgress( 5, "Processing: Adding beaches" );
                AddBeaches( map );
            }

            if( args.AddTrees ) {
                ReportProgress( 5, "Processing: Planting trees" );
                GenerateTrees( map );
            }
        }
        
        void Fill( Level map, float desiredWaterLevel, float aboveWaterMultiplier, float[] altmap ) {
            int width = map.Width, length = map.Length, mapHeight = map.Height;
            int snowStartThreshold = args.SnowAltitude - args.SnowTransition;
            int snowThreshold = args.SnowAltitude;
            
            for( int x = 0, i = 0; x < width; x++ )
                for( int z = 0; z < length; z++, i++ )
            {
                int level;
                float slope;
                if( heightmap[i] < desiredWaterLevel ) {
                    float depth = args.MaxDepth;
                    if( altmap != null ) {
                        depth += altmap[i] * args.MaxDepthVariation;
                    }
                    slope = slopemap[i] * depth;
                    level = args.WaterLevel - (int)Math.Round( ( 1 - heightmap[i] / desiredWaterLevel ) * depth );
                    surfaceMap[i] = (ushort)args.WaterLevel;

                    if( args.AddWater ) {
                        int index = (args.WaterLevel * length + z) * width + x;
                        if( args.WaterLevel >= 0 && args.WaterLevel < mapHeight ) {
                            map.blocks[index] = bWater;
                        }

                        for( int yy = args.WaterLevel; yy > level && yy >= 0; yy-- ) {
                            if( yy < mapHeight )
                                map.blocks[index] = bWater; // TODO: Might be a bug? Probably waterLevel - 1.
                            index -= width * length;
                        }
                        
                        index = ((level + 1) * length + z) * width + x;
                        for( int yy = level; yy >= 0; yy-- ) {
                            index -= length * width;
                            if( yy >= mapHeight ) continue;
                            
                            if( level - yy < SeaFloorThickness ) {
                                map.blocks[index] = bSeaFloor;
                            } else {
                                map.blocks[index] = bBedrock;
                            }
                        }
                    } else {
                        int index = (level * length + z) * width + x;
                        if( level >= 0 && level < mapHeight ) {
                            if( slope < args.CliffThreshold ) {
                                map.blocks[index] = bGroundSurface;
                            } else {
                                map.blocks[index] = bCliff;
                            }
                        }

                        for( int yy = level - 1; yy >= 0; yy-- ) {
                            index -= length * width;
                            if( yy >= mapHeight ) continue;
                            
                            if( level - yy < groundThickness ) {
                                if( slope < args.CliffThreshold ) {
                                    map.blocks[index] = bGround;
                                } else {
                                    map.blocks[index] = bCliff;
                                }
                            } else {
                                map.blocks[index] = bBedrock;
                            }
                        }
                    }

                } else {
                    float height;
                    if( altmap != null ) {
                        height = args.MaxHeight + altmap[i] * args.MaxHeightVariation;
                    } else {
                        height = args.MaxHeight;
                    }
                    slope = slopemap[i] * height;
                    if( height != 0 ) {
                        level = args.WaterLevel + (int)Math.Round( ( heightmap[i] - desiredWaterLevel ) * aboveWaterMultiplier / args.MaxHeight * height );
                    } else {
                        level = args.WaterLevel;
                    }
                    surfaceMap[i] = (ushort)level;

                    bool snow = args.AddSnow &&
                        (level > snowThreshold ||
                         (level > snowStartThreshold && rand.NextDouble() < (level - snowStartThreshold) / (double)(snowThreshold - snowStartThreshold)));

                    int index = (level * length + z) * width + x;
                    if( level >= 0 && level < mapHeight ) {
                        if( slope < args.CliffThreshold ) {
                            map.blocks[index] = snow ? Block.White : bGroundSurface;
                        } else {
                            map.blocks[index] = bCliff;
                        }
                    }

                    for( int yy = level - 1; yy >= 0; yy-- ) {
                        index -= length * width;
                        if( yy >= mapHeight ) continue;
                        
                        if( level - yy < groundThickness ) {
                            if( slope < args.CliffThreshold ) {
                                map.blocks[index] = snow ? Block.White : bGround;
                            } else {
                                map.blocks[index] = bCliff;
                            }
                        } else {
                            map.blocks[index] = bBedrock;
                        }
                    }
                }
            }
        }


        void AddBeaches( Level map ) {
            int beachExtentSqr = (args.BeachExtent + 1) * (args.BeachExtent + 1);
            for( int x = 0; x < map.Width; x++ )
                for( int z = 0; z < map.Length; z++ )
                    for( int y = args.WaterLevel; y <= args.WaterLevel + args.BeachHeight; y++ )
            {
                if( map.GetBlock( (ushort)x, (ushort)y, (ushort)z ) != bGroundSurface ) continue;
                bool found = false;
                for( int dx = -args.BeachExtent; !found && dx <= args.BeachExtent; dx++ )
                    for( int dz = -args.BeachExtent; !found && dz <= args.BeachExtent; dz++ )
                        for( int dy = -args.BeachHeight; dy <= 0; dy++ )
                {
                    if( dx * dx + dy * dy + dz * dz > beachExtentSqr ) continue;
                    int xx = x + dx, yy = y + dy, zz = z + dz;
                    if( xx < 0 || xx >= map.Width || yy < 0 || yy >= map.Height || zz < 0 || zz >= map.Length ) continue;
                    
                    BlockID block = map.GetBlock( (ushort)xx, (ushort)yy, (ushort)zz );
                    if( block == bWater ) {
                        found = true;
                        break;
                    }
                }
                
                if( found ) {
                    map.SetTile( (ushort)x, (ushort)y, (ushort)z, bSeaFloor );
                    if( y > 0 && map.GetBlock( (ushort)x, (ushort)(y - 1), (ushort)z ) == bGround ) {
                        map.SetTile( (ushort)x, (ushort)(y - 1), (ushort)z, bSeaFloor );
                    }
                }
            }
        }


        void GenerateTrees( Level map ) {
            int minTrunkPadding = args.TreeSpacingMin;
            int maxTrunkPadding = args.TreeSpacingMax;

            Random rn = new Random();
            int width = _width, length = _length;

            for( int x = 0; x < width; x += rn.Next( minTrunkPadding, maxTrunkPadding + 1 ) )
                for( int z = 0; z < length; z += rn.Next( minTrunkPadding, maxTrunkPadding + 1 ) ) 
            {
                int nx = x + rn.Next( -(minTrunkPadding / 2), (maxTrunkPadding / 2) + 1 );
                int nz = z + rn.Next( -(minTrunkPadding / 2), (maxTrunkPadding / 2) + 1 );
                
                if( nx < 0 || nx >= width || nz < 0 || nz >= length ) continue;
                int ny = surfaceMap[nx * length + nz];

                if( (map.GetBlock( (ushort)nx, (ushort)ny, (ushort)nz ) == bGroundSurface) && slopemap[nx * length + nz] < .5 ) {
                    
                    // discard this tree if it would breach the top of the map                    
                    tree.SetData( rn, tree.DefaultSize( rn ) );
                    int nh = tree.height;
                    if( ny + nh + nh / 2 > map.Height )
                        continue;
                    
                    tree.Generate( (ushort)nx, (ushort)(ny + 1), (ushort)nz, (xT, yT, zT, bT) =>
                                  {
                                      if( map.IsAirAt( xT, yT, zT ) )
                                          map.SetTile( xT, yT, zT, (byte)bT );
                                  });
                }
            }
        }

        #endregion
            
        
        public static void RegisterGenerators() {
            string[] names = Enum.GetNames(typeof(MapGenBiomeName));
            string desc = "&HSeed specifies biome of the map. " +
                 "It must be one of the following: &f" + names.Join();
                                                                                   
            for (MapGenTemplate type = 0; type < MapGenTemplate.Count; type++) 
            {
                // Because of the way C# implements for loop closures, '=> Gen(p, lvl, seed, theme_)'
                //  captures the variable from the LAST iteration, not the current one
                // Hence this causes an error to get thrown later, because 'Gen' is always executed 
                //  with 'MapGenTheme.Count' theme instead of the expected theme
                // Using a local variable copy fixes this
                MapGenTemplate type_ = type;
                
                MapGen.Register(type_.ToString(), GenType.fCraft,
                                (p, lvl, args) => Gen(p, lvl, args, type_), desc);
            }
        }
        
        static bool Gen(Player p, Level lvl, MapGenArgs gen_args, MapGenTemplate type) {
            fCraftMapGenArgs args = fCraftMapGenArgs.MakeTemplate(type);
            
            gen_args.Biome = args.Biome;
            if (!gen_args.ParseArgs(p)) return false;
            args.Seed      = gen_args.Seed;
            
            float ratio = lvl.Height / 96.0f;
            args.MaxHeight    = (int)Math.Round(args.MaxHeight    * ratio);
            args.MaxDepth     = (int)Math.Round(args.MaxDepth     * ratio);
            args.SnowAltitude = (int)Math.Round(args.SnowAltitude * ratio);
            
            args.Biome      = gen_args.Biome;
            args.WaterLevel = (lvl.Height - 1) / 2;

            new fCraftMapGen(args).Generate(lvl);
            return true;
        }
    }
}