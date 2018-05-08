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
using System.ComponentModel;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public sealed class LavaMapProperties {
        internal readonly LSGame.MapSettings m;
        
        public LavaMapProperties(LSGame.MapSettings m) {
            this.m = m;
        }

        [DisplayName("Fast lava chance")]
        public byte FastLava { get { return m.fast; } set { m.fast = Clamp(value); } }
        
        [DisplayName("Killer liquids chance")]
        public byte Killer { get { return m.killer; } set { m.killer = Clamp(value); } }

        [DisplayName("Destroy blocks chance")]        
        public byte Destroy { get { return m.destroy; } set { m.destroy = Clamp(value); } }

        [DisplayName("Water flood chance")]
        public byte Water { get { return m.water; } set { m.water = Clamp(value); } }

        [DisplayName("Layer flood chance")]
        public byte Layer { get { return m.layer; } set { m.layer = Clamp(value); } }
        

        [DisplayName("Layer height")]
        public int LayerHeight { get { return m.LayerHeight; } set { m.LayerHeight = ClampI(value); } }

        [DisplayName("Layer count")]
        public int LayerCount { get { return m.LayerCount; } set { m.LayerCount = ClampI(value); } }

        [DisplayName("Layer time (mins)")]
        public double LayerTime { get { return m.layerInterval; } set { m.layerInterval = ClampD(value); } }
        
        [DisplayName("Round time (mins)")]
        public double RoundTime { get { return m.roundTime; } set { m.roundTime = ClampD(value); } }
        
        [DisplayName("Flood time (mins)")]
        public double FloodTime { get { return m.floodTime; } set { m.floodTime = ClampD(value); } }
        
        static byte Clamp(byte value) { return Math.Min(value, (byte)100); }
        static int ClampI(int value) { return Math.Max(0, Math.Min(value, 1000)); }
        static double ClampD(double value) { return Math.Max(0, Math.Min(value, 10)); }
    }
}
