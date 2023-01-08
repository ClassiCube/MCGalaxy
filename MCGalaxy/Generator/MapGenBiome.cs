/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System;

namespace MCGalaxy.Generator
{
    public enum MapGenBiomeName
    {
        Forest, Arctic, Desert, Hell, Swamp, Mine
    }
    
    /// <summary> Contains environment settings and the types of blocks that are used to generate a map </summary>
    public struct MapGenBiome
    {
        /// <summary> The block that appears at the top of ground (e.g. Grass) </summary>
        public byte Surface;
        /// <summary> The block that makes up the bulk of ground (e.g. Dirt) </summary>
        public byte Ground;
        public byte Cliff;
        public byte Water;
        public byte SeaFloor;
        public byte Bedrock;
        
        public string CloudColor;
        public string SkyColor;
        public string FogColor;
        public byte Horizon;
        public byte Border;
        
        public void ApplyEnv(EnvConfig env) {
            if (CloudColor != null) env.CloudColor = CloudColor;
            if (SkyColor != null)   env.SkyColor   = SkyColor;
            if (FogColor != null)   env.FogColor   = FogColor;
            if (Horizon != 0)       env.HorizonBlock = Horizon;
            if (Border  != 0)       env.EdgeBlock    = Border;
        }
        
        public static MapGenBiome Get(MapGenBiomeName theme) {
            switch (theme)
            {
                case MapGenBiomeName.Arctic: return Arctic;
                case MapGenBiomeName.Desert: return Desert;
                case MapGenBiomeName.Hell:   return Hell;
                case MapGenBiomeName.Swamp:  return Swamp;
                case MapGenBiomeName.Mine:   return Mine;
            }
            return Forest;
        }
        
                
        public static MapGenBiome Forest = new MapGenBiome()
        {
            Surface    = Block.Grass,
            Ground     = Block.Dirt,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            SeaFloor   = Block.Sand,
            Bedrock    = Block.Stone,
        };
        
        public static MapGenBiome Arctic = new MapGenBiome()
        {
            Surface    = Block.White,
            Ground     = Block.White,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            SeaFloor   = Block.White,
            Bedrock    = Block.Stone,
            CloudColor = "#8E8E8E",
            SkyColor   = "#8E8E8E",
            FogColor   = "#AFAFAF",
        };
        
        public static MapGenBiome Desert = new MapGenBiome()
        {
            Surface    = Block.Sand,
            Ground     = Block.Sand,
            Cliff      = Block.Gravel,
            Water      = Block.StillWater,
            SeaFloor   = Block.Sand,
            Bedrock    = Block.Stone,
            CloudColor = "#FFEE88",
            SkyColor   = "#FFEE88",
            FogColor   = "#FFEE88",
            Horizon    = Block.Sand,
            Border     = Block.Sandstone,
        };
        
        public static MapGenBiome Hell = new MapGenBiome()
        {
            Surface    = Block.Obsidian,
            Ground     = Block.Stone,
            Cliff      = Block.Stone,
            Water      = Block.StillLava,
            SeaFloor   = Block.Obsidian,
            Bedrock    = Block.Stone,
            CloudColor = "#000000",
            SkyColor   = "#FFCC00",
            FogColor   = "#FF6600",
            Horizon    = Block.StillLava,
        };
        
        public static MapGenBiome Swamp = new MapGenBiome()
        {
            Surface    = Block.Dirt,
            Ground     = Block.Dirt,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            SeaFloor   = Block.Leaves,
            Bedrock    = Block.Stone,
        };
        
        public static MapGenBiome Mine = new MapGenBiome()
        {
            Surface    = Block.Gravel,
            Ground     = Block.Cobblestone,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            SeaFloor   = Block.Stone,
            Bedrock    = Block.Bedrock,
            CloudColor = "#444444",
            SkyColor   = "#444444",
            FogColor   = "#777777",
        };
    }
}