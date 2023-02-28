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
using MCGalaxy.Generator.Foliage;

namespace MCGalaxy.Generator
{
    public enum MapGenBiomeName
    {
        Forest, Arctic, Desert, Hell, Swamp, Mine, Sandy, Plains, Space
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
        public byte Bedrock;
        public byte BeachSandy, BeachRocky;
        
        public string CloudColor;
        public string SkyColor;
        public string FogColor;
        public byte Horizon;
        public byte Border;
        public string TreeType;
        
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
                case MapGenBiomeName.Sandy:  return Sandy;
                case MapGenBiomeName.Plains: return Plains;
                case MapGenBiomeName.Space:  return Space;
            }
            return Forest;
        }
        
        public Tree GetTreeGen(string defaultType) {
            if (TreeType == null) return null;
            
            string type = TreeType;
            if (type.Length == 0) type = defaultType;
            return Tree.TreeTypes[type]();
        }
        
                
        public static MapGenBiome Forest = new MapGenBiome()
        {
            Surface    = Block.Grass,
            Ground     = Block.Dirt,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            Bedrock    = Block.Stone,
            BeachSandy = Block.Sand,
            BeachRocky = Block.Gravel,
            TreeType   = "", // "use default for generator"
        };
        
        public static MapGenBiome Arctic = new MapGenBiome()
        {
            Surface    = Block.White,
            Ground     = Block.White,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            Bedrock    = Block.Stone,
            BeachSandy = Block.White,
            BeachRocky = Block.Stone,
            CloudColor = "#8E8E8E",
            SkyColor   = "#8E8E8E",
            FogColor   = "#AFAFAF",
        };
        
        public static MapGenBiome Desert = new MapGenBiome()
        {
            Surface    = Block.Sand,
            Ground     = Block.Sand,
            Cliff      = Block.Gravel,
            Water      = Block.Air,
            Bedrock    = Block.Stone,
            BeachSandy = Block.Sand,
            BeachRocky = Block.Gravel,
            CloudColor = "#FFEE88",
            SkyColor   = "#FFEE88",
            FogColor   = "#FFEE88",
            Horizon    = Block.Sand,
            Border     = Block.Sandstone,
            TreeType   = "Cactus",
        };
        
        public static MapGenBiome Hell = new MapGenBiome()
        {
            Surface    = Block.Obsidian,
            Ground     = Block.Stone,
            Cliff      = Block.Stone,
            Water      = Block.StillLava,
            Bedrock    = Block.Stone,
            BeachSandy = Block.Obsidian,
            BeachRocky = Block.Obsidian,
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
            Bedrock    = Block.Stone,
            BeachSandy = Block.Leaves,
            BeachRocky = Block.Dirt,
        };
        
        public static MapGenBiome Mine = new MapGenBiome()
        {
            Surface    = Block.Gravel,
            Ground     = Block.Cobblestone,
            Cliff      = Block.Stone,
            Water      = Block.StillWater,
            Bedrock    = Block.Bedrock,
            BeachSandy = Block.Stone,
            BeachRocky = Block.Cobblestone,
            CloudColor = "#444444",
            SkyColor   = "#444444",
            FogColor   = "#777777",
        };
        
        public static MapGenBiome Sandy = new MapGenBiome()
        {
            Surface    = Block.Sand,
            Ground     = Block.Sand,
            Cliff      = Block.Gravel,
            Water      = Block.StillWater,
            Bedrock    = Block.Stone,
            BeachSandy = Block.Sand,
            BeachRocky = Block.Gravel,
            CloudColor = "#52C6F7",
            Border     = Block.Sand,
            TreeType   = "Palm",
        };
                
        public static MapGenBiome Plains = new MapGenBiome()
        {
            Surface    = Block.Grass,
            Ground     = Block.Dirt,
            Cliff      = Block.Stone,
            Water      = Block.Air,
            Bedrock    = Block.Stone,
            BeachSandy = Block.Grass,
            BeachRocky = Block.Grass,
            TreeType   = "", // "use default for generator"
            Horizon    = Block.Grass,
        };
        
        public static MapGenBiome Space = new MapGenBiome()
        {
            Surface    = Block.Obsidian,
            Ground     = Block.Iron,
            Cliff      = Block.Iron,
            Water      = Block.Air,
            Bedrock    = Block.Bedrock,
            BeachSandy = Block.Obsidian,
            BeachRocky = Block.Obsidian,
            SkyColor   = "#000000",
            FogColor   = "#000000",
            Horizon    = Block.Obsidian,
            Border     = Block.Obsidian,
        };
    }
}