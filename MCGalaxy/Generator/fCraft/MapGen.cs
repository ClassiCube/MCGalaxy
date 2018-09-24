// Part of fCraft | Copyright 2009-2015 Matvei Stefarov <me@matvei.org> | BSD-3 | See LICENSE.txt
using System;
using MCGalaxy.Commands;
using BlockID = System.UInt16;

namespace MCGalaxy.Generator {

    /// <summary> Map generator themes. A theme defines what type of blocks are used to fill the map. </summary>
    public enum MapGenBiome {
        Forest, Arctic, Desert, Hell, Swamp
    }


    /// <summary> Map generator template. Templates define landscape shapes and features. </summary>
    public enum MapGenTheme {
        Archipelago, Atoll, Bay, Dunes, Hills, Ice, Island2,
        Lake, Mountains2, Peninsula, Random, River, Streams
    }


    /// <summary> Provides functionality for generating map files. </summary>
    public sealed class fCraftMapGen {
        readonly fCraftMapGenArgs args;
        readonly Random rand;
        readonly Noise noise;
        float[,] heightmap, slopemap;

        // theme-dependent vars
        internal byte bWaterSurface, bGroundSurface, bWater, bGround, bSeaFloor, bBedrock, bDeepWaterSurface, bCliff;

        internal int groundThickness = 5;
        const int SeaFloorThickness = 3;

        public fCraftMapGen( fCraftMapGenArgs generatorArgs ) {
            if( generatorArgs == null ) throw new ArgumentNullException( "generatorArgs" );
            args = generatorArgs;
            rand = new Random( args.Seed );
            noise = new Noise( args.Seed, NoiseInterpolationMode.Bicubic );
            args.ApplyTheme( this );
        }


        public void Generate(Level map) {
            GenerateHeightmap(map);
            GenerateMap(map);
        }

        void ReportProgress( int relativeIncrease, string message ) {
            Logger.Log(LogType.SystemActivity, message );
        }
        

        #region Heightmap Processing

        void GenerateHeightmap(Level map) {
            ReportProgress( 10, "Heightmap: Priming" );
            heightmap = new float[map.Width, map.Length];

            noise.PerlinNoise( heightmap, args.FeatureScale, args.DetailScale, args.Roughness, 0, 0 );

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
            Noise.ApplyBias( heightmap, corners[0], corners[1], corners[2], corners[3], midpoint );
        }

        #endregion


        #region Map Processing

        public void GenerateMap(Level map) {
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
            if( args.CliffSmoothing ) {
                ReportProgress( 2, "Heightmap Processing: Smoothing" );
                slopemap = Noise.CalculateSlope( Noise.GaussianBlur5X5( heightmap ) );
            } else {
                slopemap = Noise.CalculateSlope( heightmap );
            }

            float[,] altmap = null;
            if( args.MaxHeightVariation != 0 || args.MaxDepthVariation != 0 ) {
                ReportProgress( 5, "Heightmap Processing: Randomizing" );
                altmap = new float[map.Width, map.Length];
                int blendmapDetailSize = (int)Math.Log( Math.Max( map.Width, map.Length ), 2 ) - 2;
                new Noise( rand.Next(), NoiseInterpolationMode.Cosine ).PerlinNoise( altmap, 3, blendmapDetailSize, 0.5f, 0, 0 );
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

            ReportProgress( 0, "Generation complete" );
        }
        
        void Fill( Level map, float desiredWaterLevel, float aboveWaterMultiplier, float[,] altmap ) {
            int width = map.Width, length = map.Length, mapHeight = map.Height;
            int snowStartThreshold = args.SnowAltitude - args.SnowTransition;
            int snowThreshold = args.SnowAltitude;
            
            for( int x = 0; x < heightmap.GetLength( 0 ); x++ )
                for( int z = 0; z < heightmap.GetLength( 1 ); z++ )
            {
                int level;
                float slope;
                if( heightmap[x, z] < desiredWaterLevel ) {
                    float depth = args.MaxDepth;
                    if( altmap != null ) {
                        depth += altmap[x, z] * args.MaxDepthVariation;
                    }
                    slope = slopemap[x, z] * depth;
                    level = args.WaterLevel - (int)Math.Round( Math.Pow( 1 - heightmap[x, z] / desiredWaterLevel, args.BelowFuncExponent ) * depth );

                    if( args.AddWater ) {
                        int index = (args.WaterLevel * length + z) * width + x;
                        if( args.WaterLevel >= 0 && args.WaterLevel < mapHeight ) {
                            if( args.WaterLevel - level > 3 ) {
                                map.blocks[index] = bDeepWaterSurface;
                            } else {
                                map.blocks[index] = bWaterSurface;
                            }
                        }
                        for( int yy = args.WaterLevel; yy > level && yy >= 0; yy-- ) {
                            if( yy >= 0 && yy < mapHeight )
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
                        height = args.MaxHeight + altmap[x, z] * args.MaxHeightVariation;
                    } else {
                        height = args.MaxHeight;
                    }
                    slope = slopemap[x, z] * height;
                    if( height != 0 ) {
                        level = args.WaterLevel + (int)Math.Round( Math.Pow( heightmap[x, z] - desiredWaterLevel, args.AboveFuncExponent ) * aboveWaterMultiplier / args.MaxHeight * height );
                    } else {
                        level = args.WaterLevel;
                    }

                    bool snow = args.AddSnow &&
                        (level > snowThreshold ||
                         (level > snowStartThreshold && rand.NextDouble() < (level - snowStartThreshold) / (double)(snowThreshold - snowStartThreshold)));

                    int index = (level * length + z) * width + x;
                    if( level >= 0 && level < mapHeight ) {
                        if( slope < args.CliffThreshold ) {
                            map.blocks[index] = (snow ? Block.White : bGroundSurface);
                        } else {
                            map.blocks[index] = bCliff;
                        }
                    }

                    for( int yy = level - 1; yy >= 0; yy-- ) {
                        index -= length * width;
                        if( yy >= mapHeight ) continue;
                        
                        if( level - yy < groundThickness ) {
                            if( slope < args.CliffThreshold ) {
                                if( snow ) {
                                    map.blocks[index] = Block.White;
                                } else {
                                    map.blocks[index] = bGround;
                                }
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
            if( map == null ) throw new ArgumentNullException( "map" );
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
                    if( block == bWater || block == bWaterSurface ) {
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
            if( map == null ) throw new ArgumentNullException( "map" );
            int minHeight = args.TreeHeightMin;
            int maxHeight = args.TreeHeightMax;
            int minTrunkPadding = args.TreeSpacingMin;
            int maxTrunkPadding = args.TreeSpacingMax;
            const int topLayers = 2;
            const double odds = 0.618;

            Random rn = new Random();
            short[,] shadows = ComputeHeightmap( map );

            for( int x = 0; x < map.Width; x += rn.Next( minTrunkPadding, maxTrunkPadding + 1 ) ) {
                for( int z = 0; z < map.Length; z += rn.Next( minTrunkPadding, maxTrunkPadding + 1 ) ) {
                    int nx = x + rn.Next( -(minTrunkPadding / 2), (maxTrunkPadding / 2) + 1 );
                    int nz = z + rn.Next( -(minTrunkPadding / 2), (maxTrunkPadding / 2) + 1 );
                    if( nx < 0 || nx >= map.Width || nz < 0 || nz >= map.Length ) continue;
                    int ny = shadows[nx, nz];

                    if( (map.GetBlock( (ushort)nx, (ushort)ny, (ushort)nz ) == bGroundSurface) && slopemap[nx, nz] < .5 ) {
                        // Pick a random height for the tree between Min and Max,
                        // discarding this tree if it would breach the top of the map
                        int nh;
                        if( (nh = rn.Next( minHeight, maxHeight + 1 )) + ny + nh / 2 > map.Height )
                            continue;

                        // Generate the trunk of the tree
                        for( int dy = 1; dy <= nh; dy++ )
                            map.SetTile( (ushort)nx, (ushort)(ny + dy), (ushort)nz, Block.Log );

                        for( int i = -1; i < nh / 2; i++ ) {
                            // Should we draw thin (2x2) or thicker (4x4) foliage
                            int radius = (i >= (nh / 2) - topLayers) ? 1 : 2;
                            // Draw the foliage
                            for( int dx = -radius; dx < radius + 1; dx++ )
                                for( int dz = -radius; dz < radius + 1; dz++ )
                            {
                                // Drop random leaves from the edges
                                if( rn.NextDouble() > odds && Math.Abs( dx ) == Math.Abs( dz ) && Math.Abs( dx ) == radius )
                                    continue;
                                // By default only replace an existing block if its air
                                if( map.GetBlock( (ushort)(nx + dx), (ushort)(ny + nh + i), (ushort)(nz + dz) ) == Block.Air ) {
                                    map.SetTile( (ushort)(nx + dx), (ushort)(ny + nh + i), (ushort)(nz + dz), Block.Leaves );
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
        
        
        static short[,] ComputeHeightmap(Level map) {
            short[,] shadows = new short[map.Width, map.Length];
            for( int x = 0; x < map.Width; x++ )
                for( int z = 0; z < map.Length; z++ )
            {
                int index = (map.Height * map.Length + z) * map.Width + x;
                for( int y = (map.Height - 1); y >= 0; y-- ) {
                    index -= map.Length * map.Width;
                    switch( map.blocks[index] ) {
                        case Block.Air:
                        case Block.Mushroom:
                        case Block.Glass:
                        case Block.Leaves:
                        case Block.Rose:
                        case Block.RedMushroom:
                        case Block.Sapling:
                        case Block.Dandelion:
                            continue;
                        default:
                            shadows[x, z] = (short)y;
                            break;
                    }
                    break;
                }
            }
            return shadows;
        }
        
        
        public static void RegisterGenerators() {
            string[] names = Enum.GetNames(typeof(MapGenBiome));
            string desc = "%HSeed specifies biome of the map. " +
                 "It must be one of the following: &f" + names.Join();
                                                                                   
            foreach (MapGenTheme theme in Enum.GetValues(typeof(MapGenTheme))) {
                MapGen.Register(theme.ToString(), GenType.fCraft,
                                (p, lvl, seed) => Gen(p, lvl, seed, theme), desc);
            }
        }
        
        static bool Gen(Player p, Level lvl, string seed, MapGenTheme theme) {
            MapGenBiome biome = MapGenBiome.Forest;
            if (seed.Length > 0 && !CommandParser.GetEnum(p, seed, "Seed", ref biome)) return false;
            fCraftMapGenArgs args = fCraftMapGenArgs.MakeTemplate(theme);
            
            float ratio = lvl.Height / 96.0f;
            args.MaxHeight    = (int)Math.Round(args.MaxHeight    * ratio);
            args.MaxDepth     = (int)Math.Round(args.MaxDepth     * ratio);
            args.SnowAltitude = (int)Math.Round(args.SnowAltitude * ratio);
            
            args.Biome      = biome;
            args.AddTrees   = biome == MapGenBiome.Forest;
            args.AddWater   = biome != MapGenBiome.Desert;
            args.WaterLevel = (lvl.Height - 1) / 2;

            new fCraftMapGen(args).Generate(lvl);
            return true;
        }
    }
}