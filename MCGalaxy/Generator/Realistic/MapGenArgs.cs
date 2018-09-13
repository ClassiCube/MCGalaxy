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

namespace MCGalaxy.Generator {
    public delegate ushort CalcLiquidLevel(ushort lvlHeight);
    
    public sealed class RealisticMapGenArgs {
        public float RangeLow = 0.2f;
        public float RangeHigh = 0.8f;
        public bool SimpleColumns = false, IslandColumns = false;
        public bool FalloffEdges = false;
        public bool UseLavaLiquid = false;
        public bool GenerateOverlay2 = true;
        public CalcLiquidLevel GetLiquidLevel = (lvlHeight) => (ushort)(lvlHeight / 2 + 2);
        
        // Decoration parameters
        public float TreeDensity = 0.35f;
        public short TreeDistance = 3;
        public bool GenFlowers = true, GenTrees = true;
        public bool UseCactus = false;
        
        // Fault parameters
        public float StartHeight = 0.5f;
        public float DisplacementMax = 0.01f;
        public float DisplacementStep = -0.0025f;
        
        internal static RealisticMapGenArgs hell = new RealisticMapGenArgs() {
            RangeLow = 0.3f, RangeHigh = 1.3f,
            DisplacementMax = 0.02f, StartHeight = 0.04f, UseLavaLiquid = true,
            GetLiquidLevel = (height) => 5 
        };        
        internal static RealisticMapGenArgs island = new RealisticMapGenArgs() { 
            RangeLow = 0.4f, RangeHigh = 0.75f,
            FalloffEdges = true, IslandColumns = true 
        };
        internal static RealisticMapGenArgs forest = new RealisticMapGenArgs() { 
            RangeLow = 0.45f, RangeHigh = 0.8f,
            TreeDensity = 0.7f, TreeDistance = 2 
        };
        internal static RealisticMapGenArgs mountains = new RealisticMapGenArgs() { 
            RangeLow = 0.3f, RangeHigh = 0.9f,
            TreeDistance = 4, DisplacementMax = 0.02f, StartHeight = 0.6f 
        };
        internal static RealisticMapGenArgs ocean = new RealisticMapGenArgs() { 
            RangeLow = 0.1f, RangeHigh = 0.6f,
            GenTrees = false, GenerateOverlay2 = false,
            GetLiquidLevel = (height) => (ushort)(height * 0.85f) 
        };
        internal static RealisticMapGenArgs desert = new RealisticMapGenArgs() { 
            RangeLow = 0.5f, RangeHigh = 0.85f,
            TreeDistance = 24, GenFlowers = false, GenerateOverlay2 = false,
            UseCactus = true, SimpleColumns = true,
            GetLiquidLevel = (height) => 0 
        };
    }
}