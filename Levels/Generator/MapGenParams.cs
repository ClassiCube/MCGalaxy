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
using System.Collections.Generic;

namespace MCGalaxy {
	
	public sealed class MapGenParams {
		
		public float RangeLow = 0.2f;
        public float RangeHigh = 0.8f;
        public float TreeDens = 0.35f;
        public short TreeDist = 3;
        public bool HasFlowers = true, GenTrees = true;
        
        public static Dictionary<string, MapGenParams> Themes = new Dictionary<string, MapGenParams>() {
        	{ "hell", new MapGenParams() { RangeLow = 0.3f, RangeHigh = 1.3f } },
        	{ "island", new MapGenParams() { RangeLow = 0.4f, RangeHigh = 0.75f } },
        	{ "forest", new MapGenParams() { RangeLow = 0.45f, RangeHigh = 0.8f, 
        			TreeDens = 0.7f, TreeDist = 2 } },
        	{ "mountains", new MapGenParams() { RangeLow = 0.3f, RangeHigh = 0.9f, 
        			TreeDist = 4 } },
        	{ "ocean", new MapGenParams() { RangeLow = 0.1f, RangeHigh = 0.6f, 
        			GenTrees = false } },
        	{ "desert", new MapGenParams() { RangeLow = 0.5f, RangeHigh = 0.85f, 
        			TreeDist = 24, HasFlowers = false } },
        };
	}
}
