// Part of fCraft | Copyright 2009-2015 Matvei Stefarov <me@matvei.org> | BSD-3 | See LICENSE.txt
using System;
using MCGalaxy;

namespace MCGalaxy.Generator {
    /// <summary> Contains parameters for advanced map generation. </summary>
    public sealed class fCraftMapGenArgs {
        public string MapName;

        public MapGenBiome Biome = MapGenBiome.Forest;
        public int   Seed, // 0
        MaxHeight = 20,
        MaxDepth = 12,
        MaxHeightVariation = 4,
        MaxDepthVariation; // 0

        public bool  AddWater = true,
        MatchWaterCoverage; // false
        public int   WaterLevel = 48;
        public float WaterCoverage = .5f;

        public bool  UseBias,        // false
        DelayBias;      // false
        public float Bias;           // 0
        public int   RaisedCorners,  // 0
        LoweredCorners, // 0
        MidPoint;       // 0

        public int   DetailScale = 7,
        FeatureScale = 1;
        public float Roughness = .5f;
        public bool  MarbledHeightmap, // false
        InvertHeightmap;  // false
        public float AboveFuncExponent = 1,
        BelowFuncExponent = 1;

        public bool  AddTrees = true,
        AddGiantTrees; // false
        public int   TreeSpacingMin = 7,
        TreeSpacingMax = 11,
        TreeHeightMin = 5,
        TreeHeightMax = 7;

        public bool  AddSnow; // false
        public int   SnowAltitude = 70,
        SnowTransition = 7;

        public bool  AddCliffs = true,
        CliffSmoothing = true;
        public float CliffThreshold = 1;

        public bool  AddBeaches; // false
        public int   BeachExtent = 6,
        BeachHeight = 2;

        public fCraftMapGenArgs() {
            Seed = (new Random()).Next();
        }
        
        public void ApplyTheme( fCraftMapGen gen ) {
            switch( Biome ) {
                case MapGenBiome.Arctic:
                    gen.bWaterSurface = Block.Glass;
                    gen.bDeepWaterSurface = Block.StillWater;
                    gen.bGroundSurface = Block.White;
                    gen.bWater = Block.StillWater;
                    gen.bGround = Block.White;
                    gen.bSeaFloor = Block.White;
                    gen.bBedrock = Block.Stone;
                    gen.bCliff = Block.Stone;
                    gen.groundThickness = 1;
                    break;
                    
                case MapGenBiome.Desert:
                    gen.bWaterSurface = Block.StillWater;
                    gen.bDeepWaterSurface = Block.StillWater;
                    gen.bGroundSurface = Block.Sand;
                    gen.bWater = Block.StillWater;
                    gen.bGround = Block.Sand;
                    gen.bSeaFloor = Block.Sand;
                    gen.bBedrock = Block.Stone;
                    gen.bCliff = Block.Gravel;
                    break;
                    
                case MapGenBiome.Hell:
                    gen.bWaterSurface = Block.StillLava;
                    gen.bDeepWaterSurface = Block.StillLava;
                    gen.bGroundSurface = Block.Obsidian;
                    gen.bWater = Block.StillLava;
                    gen.bGround = Block.Stone;
                    gen.bSeaFloor = Block.Obsidian;
                    gen.bBedrock = Block.Stone;
                    gen.bCliff = Block.Stone;
                    break;
                    
                case MapGenBiome.Forest:
                    gen.bWaterSurface = Block.StillWater;
                    gen.bDeepWaterSurface = Block.StillWater;
                    gen.bGroundSurface = Block.Grass;
                    gen.bWater = Block.StillWater;
                    gen.bGround = Block.Dirt;
                    gen.bSeaFloor = Block.Sand;
                    gen.bBedrock = Block.Stone;
                    gen.bCliff = Block.Stone;
                    break;
                    
                case MapGenBiome.Swamp:
                    gen.bWaterSurface = Block.StillWater;
                    gen.bDeepWaterSurface = Block.StillWater;
                    gen.bGroundSurface = Block.Dirt;
                    gen.bWater = Block.StillWater;
                    gen.bGround = Block.Dirt;
                    gen.bSeaFloor = Block.Leaves;
                    gen.bBedrock = Block.Stone;
                    gen.bCliff = Block.Stone;
                    break;
            }
        }


        public static fCraftMapGenArgs MakeTemplate( MapGenTheme template ) {
            switch( template ) {
                case MapGenTheme.Archipelago:
                    return new fCraftMapGenArgs {
                        MaxHeight = 8,
                        MaxDepth = 20,
                        FeatureScale = 3,
                        Roughness = .46f,
                        MatchWaterCoverage = true,
                        WaterCoverage = .85f
                    };

                case MapGenTheme.Atoll:
                    return new fCraftMapGenArgs {
                        Biome = MapGenBiome.Desert,
                        MaxHeight = 2,
                        MaxDepth = 39,
                        UseBias = true,
                        Bias = .9f,
                        MidPoint = 1,
                        LoweredCorners = 4,
                        FeatureScale = 2,
                        DetailScale = 5,
                        MarbledHeightmap = true,
                        InvertHeightmap = true,
                        MatchWaterCoverage = true,
                        WaterCoverage = .95f
                    };

                case MapGenTheme.Bay:
                    return new fCraftMapGenArgs {
                        MaxHeight = 22,
                        MaxDepth = 12,
                        UseBias = true,
                        Bias = 1,
                        MidPoint = -1,
                        RaisedCorners = 3,
                        LoweredCorners = 1,
                        TreeSpacingMax = 12,
                        TreeSpacingMin = 6,
                        MarbledHeightmap = true,
                        DelayBias = true
                    };

                case MapGenTheme.Dunes:
                    return new fCraftMapGenArgs {
                        AddTrees = false,
                        AddWater = false,
                        Biome = MapGenBiome.Desert,
                        MaxHeight = 12,
                        MaxDepth = 7,
                        FeatureScale = 2,
                        DetailScale = 3,
                        Roughness = .44f,
                        MarbledHeightmap = true,
                        InvertHeightmap = true
                    };

                case MapGenTheme.Hills:
                    return new fCraftMapGenArgs {
                        AddWater = false,
                        MaxHeight = 8,
                        MaxDepth = 8,
                        FeatureScale = 2,
                        TreeSpacingMin = 7,
                        TreeSpacingMax = 13
                    };

                case MapGenTheme.Ice:
                    return new fCraftMapGenArgs {
                        AddTrees = false,
                        Biome = MapGenBiome.Arctic,
                        MaxHeight = 2,
                        MaxDepth = 2032,
                        FeatureScale = 2,
                        DetailScale = 7,
                        Roughness = .64f,
                        MarbledHeightmap = true,
                        MatchWaterCoverage = true,
                        WaterCoverage = .3f,
                        MaxHeightVariation = 0
                    };

                case MapGenTheme.Island2:
                    return new fCraftMapGenArgs {
                        MaxHeight = 16,
                        MaxDepth = 39,
                        UseBias = true,
                        Bias = .7f,
                        MidPoint = 1,
                        LoweredCorners = 4,
                        FeatureScale = 3,
                        DetailScale = 7,
                        MarbledHeightmap = true,
                        DelayBias = true,
                        AddBeaches = true,
                        Roughness = 0.45f
                    };

                case MapGenTheme.Lake:
                    return new fCraftMapGenArgs {
                        MaxHeight = 14,
                        MaxDepth = 20,
                        UseBias = true,
                        Bias = .65f,
                        MidPoint = -1,
                        RaisedCorners = 4,
                        FeatureScale = 2,
                        Roughness = .56f,
                        MatchWaterCoverage = true,
                        WaterCoverage = .3f
                    };

                case MapGenTheme.Mountains2:
                    return new fCraftMapGenArgs {
                        AddWater = false,
                        MaxHeight = 40,
                        MaxDepth = 10,
                        FeatureScale = 1,
                        DetailScale = 7,
                        MarbledHeightmap = true,
                        AddSnow = true,
                        MatchWaterCoverage = true,
                        WaterCoverage = .5f,
                        Roughness = .55f,
                        CliffThreshold = .9f
                    };

                case MapGenTheme.Random:
                    return new fCraftMapGenArgs();

                case MapGenTheme.River:
                    return new fCraftMapGenArgs {
                        MaxHeight = 22,
                        MaxDepth = 8,
                        FeatureScale = 0,
                        DetailScale = 6,
                        MarbledHeightmap = true,
                        MatchWaterCoverage = true,
                        WaterCoverage = .31f
                    };

                case MapGenTheme.Streams:
                    return new fCraftMapGenArgs {
                        MaxHeight = 5,
                        MaxDepth = 4,
                        FeatureScale = 2,
                        DetailScale = 7,
                        Roughness = .55f,
                        MarbledHeightmap = true,
                        MatchWaterCoverage = true,
                        WaterCoverage = .25f,
                        TreeSpacingMin = 8,
                        TreeSpacingMax = 14
                    };

                case MapGenTheme.Peninsula:
                    return new fCraftMapGenArgs {
                        MaxHeight = 22,
                        MaxDepth = 12,
                        UseBias = true,
                        Bias = .5f,
                        MidPoint = -1,
                        RaisedCorners = 3,
                        LoweredCorners = 1,
                        TreeSpacingMax = 12,
                        TreeSpacingMin = 6,
                        InvertHeightmap = true,
                        WaterCoverage = .5f
                    };

                default:
                    throw new ArgumentOutOfRangeException( "template" );
            }
        }
    }
}