// Copyright 2014-2017 ClassicalSharp | Licensed under BSD-3
// Based on: https://github.com/UnknownShadow200/ClassiCube/wiki/Minecraft-Classic-map-generation-algorithm
using System;
using MCGalaxy.Generator.Classic;

namespace MCGalaxy.Generator.Foliage 
{
    public sealed class ClassicTree : Tree 
    {
        public JavaRandom rng;
        // 61 = max number of leaves possible, +1 for extra log
        public override long EstimateBlocksAffected() { return height + 65; }
                
        public override int DefaultSize(Random rnd) { EnsureRNG(rnd); return 5 + rng.Next(3); }
        
        public override void SetData(Random rnd, int value) {
            height = value;
            size   = 2;
            EnsureRNG(rnd);
        }
        
        void EnsureRNG(Random rnd) {
            if (rng == null) rng = new JavaRandom(rnd.Next());
        }
        
        public override void Generate(ushort x, ushort y, ushort z, TreeOutput output) {
            int baseHeight = height - 4;
            int topStartY  = y + baseHeight + 2;
            
            // trunk
            for (int dy = 0; dy < height - 1; dy++) 
            {
                output(x, (ushort)(y + dy), z, Block.Log);
            }
            
            // leaves bottom layer
            for (int yy = y + baseHeight; yy < topStartY; yy++)
                for (int dz = -2; dz <= 2; dz++)
                    for (int dx = -2; dx <= 2; dx++)
            {
                ushort xx = (ushort)(x + dx);
                ushort zz = (ushort)(z + dz);
                
                if (Math.Abs(dx) == 2 && Math.Abs(dz) == 2) {
                    if (rng.NextFloat() >= 0.5)
                        output(xx, (ushort)yy, zz, Block.Leaves);
                } else {
                    output(xx, (ushort)yy, zz, Block.Leaves);
                }
            }
            
            // leaves top layer
            for (int yy = topStartY; yy < y + height; yy++)
                for (int dz = -1; dz <= 1; dz++)
                    for (int dx = -1; dx <= 1; dx++)
            {
                ushort xx = (ushort)(x + dx);
                ushort zz = (ushort)(z + dz);

                if (dx == 0 || dz == 0) {
                    output(xx, (ushort)yy, zz, Block.Leaves);
                } else if (yy == topStartY && rng.NextFloat() >= 0.5) {
                    output(xx, (ushort)yy, zz, Block.Leaves);
                }
            }
        }
    }
}
