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
using System.Drawing;
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing {

    public interface IPaletteMatcher {
        void SetPalette(PaletteEntry[] front, PaletteEntry[] back);
        BlockID BestMatch(byte R, byte G, byte B);
        BlockID BestMatch(byte R, byte G, byte B, out bool backLayer);
    }
    
    public sealed class RgbPaletteMatcher : IPaletteMatcher {
        
        PaletteEntry[] front, back;
        public void SetPalette(PaletteEntry[] front, PaletteEntry[] back) {
            this.front = front; this.back = back;
        }
        
        public BlockID BestMatch(byte R, byte G, byte B) {
            int pos;
            MinDist(R, G, B, front, out pos);
            return front[pos].Block;
        }
        
        public BlockID BestMatch(byte R, byte G, byte B, out bool backLayer) {
            int frontPos, backPos;
            int frontDist = MinDist(R, G, B, front, out frontPos);
            int backDist  = MinDist(R, G, B, back, out backPos);
            
            backLayer = backDist < frontDist;
            return backLayer ? back[backPos].Block : front[frontPos].Block;
        }
        
        
        static int MinDist(byte R, byte G, byte B, PaletteEntry[] entries, out int pos) {
            int minDist = int.MaxValue; pos = 0;
            for (int i = 0; i < entries.Length; i++) {
                PaletteEntry entry = entries[i];
                
                int dist = (R - entry.R) * (R - entry.R)
                    + (G - entry.G) * (G - entry.G)
                    + (B - entry.B) * (B - entry.B);
                
                if (dist < minDist) { minDist = dist; pos = i; }
            }
            return minDist;
        }
    }
    
    public sealed class LabPaletteMatcher : IPaletteMatcher {
        
        LabColor[] palette;
        public void SetPalette(PaletteEntry[] front, PaletteEntry[] back) {
            this.palette = new LabColor[front.Length];
            for (int i = 0; i < front.Length; i++)
                this.palette[i] = RgbToLab(front[i]);
        }
        
        public BlockID BestMatch(byte R, byte G, byte B) {
            double minDist = int.MaxValue; int pos = 0;
            LabColor col = RgbToLab(R, G, B);
            
            for (int i = 0; i < palette.Length; i++) {
                LabColor pixel = palette[i];
                // Apply CIE76 color delta formula
                double dist = (col.L - pixel.L) * (col.L - pixel.L)
                    + (col.A - pixel.A) * (col.A - pixel.A)
                    + (col.B - pixel.B) * (col.B - pixel.B);
                
                if (dist < minDist) { minDist = dist; pos = i; }
            }
            return palette[pos].Block;
        }
        
        public BlockID BestMatch(byte R, byte G, byte B, out bool backLayer) {
            backLayer = false;
            return BestMatch(R, G, B);
        }
        
        
        struct LabColor {
            public double L, A, B;
            public BlockID Block;
        }
        
        LabColor RgbToLab(PaletteEntry cur) {
            LabColor lab = RgbToLab(cur.R, cur.G, cur.B);
            lab.Block = cur.Block;
            return lab;
        }
        
        LabColor RgbToLab(byte r, byte g, byte b) {
            // First convert RGB to CIE-XYZ
            double R = r / 255.0, G = g / 255.0, B = b / 255.0;
            if (R > 0.04045) R = Math.Pow((R + 0.055) / 1.055, 2.4);
            else R = R / 12.92;
            if (G > 0.04045) G = Math.Pow((G + 0.055) / 1.055, 2.4);
            else G = G / 12.92;
            if (R > 0.04045) R = Math.Pow((B + 0.055) / 1.055, 2.4);
            else B = B / 12.92;

            double X = R * 0.4124 + G * 0.3576 + B * 0.1805;
            double Y = R * 0.2126 + G * 0.7152 + B * 0.0722;
            double Z = R * 0.0193 + G * 0.1192 + B * 0.9505;
            
            
            // Then CIE-XYZ to CIE-Lab
            X /= 95.047; Y /= 100.0; Z /= 108.883;
            
            if (X > 0.008856) X = Math.Pow(X, 1.0/3);
            else X = (7.787 * X) + (16.0 / 116);
            if (Y > 0.008856) Y = Math.Pow(Y, 1.0/3);
            else Y = (7.787 * Y) + (16.0 / 116);
            if (Z > 0.008856) Z = Math.Pow(Z, 1.0/3);
            else Z = (7.787 * Z) + (16.0 / 116);

            LabColor lab = default(LabColor);
            lab.L = 116 * Y - 16;
            lab.A = 500 * (X - Y);
            lab.B = 200 * (Y - Z);
            return lab;
        }
    }
}
