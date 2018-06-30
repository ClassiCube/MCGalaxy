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
        internal readonly LSMapConfig m;
        public LavaMapProperties(LSMapConfig m) { this.m = m; }

        [DisplayName("Fast lava chance")]
        public int FastLava { get { return m.FastChance; } set { m.FastChance = Chance(value); } }
        [DisplayName("Killer liquids chance")]
        public int Killer { get { return m.KillerChance; } set { m.KillerChance = Chance(value); } }
        [DisplayName("Destroy blocks chance")]
        public int Destroy { get { return m.DestroyChance; } set { m.DestroyChance = Chance(value); } }
        [DisplayName("Water flood chance")]
        public int Water { get { return m.WaterChance; } set { m.WaterChance = Chance(value); } }
        [DisplayName("Layer flood chance")]
        public int Layer { get { return m.LayerChance; } set { m.LayerChance = Chance(value); } }

        [DisplayName("Layer height")]
        public int LayerHeight { get { return m.LayerHeight; } set { m.LayerHeight = Value(value); } }
        [DisplayName("Layer count")]
        public int LayerCount { get { return m.LayerCount; } set { m.LayerCount = Value(value); } }

        [DisplayName("Layer time (mins)")]
        public double LayerTime { get { return m.LayerInterval.TotalMinutes; } set { m.LayerInterval = Time(value); } }
        [DisplayName("Round time (mins)")]
        public double RoundTime { get { return m.RoundTime.TotalMinutes; } set { m.RoundTime = Time(value); } }
        [DisplayName("Flood time (mins)")]
        public double FloodTime { get { return m.FloodTime.TotalMinutes; } set { m.FloodTime = Time(value); } }
        
        static int Chance(int value) { return Math.Max(0, Math.Min(value, 100)); }
        static int Value(int value) { return Math.Max(0, Math.Min(value, 1000)); }
        static TimeSpan Time(double value) {
            return TimeSpan.FromMinutes(Math.Max(0, Math.Min(value, 20)));
        }
    }
}
