// Part of fCraft | Copyright 2009-2015 Matvei Stefarov <me@matvei.org> | BSD-3 | See LICENSE.txt
using System;
using MCGalaxy.Generator.Foliage;

namespace MCGalaxy.Generator.fCraft
{
    sealed class fCraftTree : Tree
    {
        public override long EstimateBlocksAffected() { return 0; }
        
        public override int DefaultSize(Random rnd) { return rnd.Next(5, 8); }
        
        public override void SetData(Random rnd, int value) {
            height   = value;
            this.rnd = rnd;
        }
        
        public override void Generate(ushort x, ushort y, ushort z, TreeOutput output) {
            const int topLayers = 2;
            const double odds = 0.618;
            
            for( int dy = 1; dy <= height; dy++ )
                output( x, (ushort)(y + dy), z, Block.Log );

            for( int i = -1; i < height / 2; i++ ) {
                // Should we draw thin (2x2) or thicker (4x4) foliage
                int radius = (i >= (height / 2) - topLayers) ? 1 : 2;
                // Draw the foliage
                for( int dx = -radius; dx < radius + 1; dx++ )
                    for( int dz = -radius; dz < radius + 1; dz++ )
                {
                    // Drop random leaves from the edges
                    if( rnd.NextDouble() > odds && Math.Abs( dx ) == Math.Abs( dz ) && Math.Abs( dx ) == radius )
                        continue;
                    
                    output( (ushort)(x + dx), (ushort)(y + height + i), (ushort)(z + dz), Block.Leaves );
                }
            }
        }
    }
    
}