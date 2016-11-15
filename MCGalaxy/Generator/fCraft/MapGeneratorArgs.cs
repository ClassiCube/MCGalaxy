// Part of fCraft | Copyright 2009-2015 Matvei Stefarov <me@matvei.org> | BSD-3 | See LICENSE.txt
using System;
using MCGalaxy;

namespace fCraf2t {
    /// <summary> Contains parameters for advanced map generation. </summary>
    public sealed class MapGeneratorArgs {
        public string MapName;

        public MapGenTheme Theme = MapGenTheme.Forest;
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

        public MapGeneratorArgs() {
            Seed = (new Random()).Next();
        }
        
        public void ApplyTheme( MapGenerator gen ) {
            switch( Theme ) {
                case MapGenTheme.Arctic:
                    gen.bWaterSurface = Block.glass;
                    gen.bDeepWaterSurface = Block.waterstill;
                    gen.bGroundSurface = Block.white;
                    gen.bWater = Block.waterstill;
                    gen.bGround = Block.white;
                    gen.bSeaFloor = Block.white;
                    gen.bBedrock = Block.rock;
                    gen.bCliff = Block.rock;
                    gen.groundThickness = 1;
                    break;
                    
                case MapGenTheme.Desert:
                    gen.bWaterSurface = Block.waterstill;
                    gen.bDeepWaterSurface = Block.waterstill;
                    gen.bGroundSurface = Block.sand;
                    gen.bWater = Block.waterstill;
                    gen.bGround = Block.sand;
                    gen.bSeaFloor = Block.sand;
                    gen.bBedrock = Block.rock;
                    gen.bCliff = Block.gravel;
                    break;
                    
                case MapGenTheme.Hell:
                    gen.bWaterSurface = Block.lavastill;
                    gen.bDeepWaterSurface = Block.lavastill;
                    gen.bGroundSurface = Block.obsidian;
                    gen.bWater = Block.lavastill;
                    gen.bGround = Block.rock;
                    gen.bSeaFloor = Block.obsidian;
                    gen.bBedrock = Block.rock;
                    gen.bCliff = Block.rock;
                    break;
                    
                case MapGenTheme.Forest:
                    gen.bWaterSurface = Block.waterstill;
                    gen.bDeepWaterSurface = Block.waterstill;
                    gen.bGroundSurface = Block.grass;
                    gen.bWater = Block.waterstill;
                    gen.bGround = Block.dirt;
                    gen.bSeaFloor = Block.sand;
                    gen.bBedrock = Block.rock;
                    gen.bCliff = Block.rock;
                    break;
                    
                case MapGenTheme.Swamp:
                    gen.bWaterSurface = Block.waterstill;
                    gen.bDeepWaterSurface = Block.waterstill;
                    gen.bGroundSurface = Block.dirt;
                    gen.bWater = Block.waterstill;
                    gen.bGround = Block.dirt;
                    gen.bSeaFloor = Block.leaf;
                    gen.bBedrock = Block.rock;
                    gen.bCliff = Block.rock;
                    break;
            }
        }


        public static MapGeneratorArgs MakeTemplate( MapGenTemplate template ) {
            switch( template ) {
                case MapGenTemplate.Archipelago:
                    return new MapGeneratorArgs {
                        MaxHeight = 8,
                        MaxDepth = 20,
                        FeatureScale = 3,
                        Roughness = .46f,
                        MatchWaterCoverage = true,
                        WaterCoverage = .85f
                    };

                case MapGenTemplate.Atoll:
                    return new MapGeneratorArgs {
                        Theme = MapGenTheme.Desert,
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

                case MapGenTemplate.Bay:
                    return new MapGeneratorArgs {
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

                case MapGenTemplate.Dunes:
                    return new MapGeneratorArgs {
                        AddTrees = false,
                        AddWater = false,
                        Theme = MapGenTheme.Desert,
                        MaxHeight = 12,
                        MaxDepth = 7,
                        FeatureScale = 2,
                        DetailScale = 3,
                        Roughness = .44f,
                        MarbledHeightmap = true,
                        InvertHeightmap = true
                    };

                case MapGenTemplate.Hills:
                    return new MapGeneratorArgs {
                        AddWater = false,
                        MaxHeight = 8,
                        MaxDepth = 8,
                        FeatureScale = 2,
                        TreeSpacingMin = 7,
                        TreeSpacingMax = 13
                    };

                case MapGenTemplate.Ice:
                    return new MapGeneratorArgs {
                        AddTrees = false,
                        Theme = MapGenTheme.Arctic,
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

                case MapGenTemplate.Island:
                    return new MapGeneratorArgs {
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

                case MapGenTemplate.Lake:
                    return new MapGeneratorArgs {
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

                case MapGenTemplate.Mountains:
                    return new MapGeneratorArgs {
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

                case MapGenTemplate.Random:
                    return new MapGeneratorArgs();

                case MapGenTemplate.River:
                    return new MapGeneratorArgs {
                        MaxHeight = 22,
                        MaxDepth = 8,
                        FeatureScale = 0,
                        DetailScale = 6,
                        MarbledHeightmap = true,
                        MatchWaterCoverage = true,
                        WaterCoverage = .31f
                    };

                case MapGenTemplate.Streams:
                    return new MapGeneratorArgs {
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

                case MapGenTemplate.Peninsula:
                    return new MapGeneratorArgs {
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