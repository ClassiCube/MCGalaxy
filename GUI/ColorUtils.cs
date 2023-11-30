/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Drawing;

namespace MCGalaxy.Gui
{
    public static class ColorUtils
    {
        struct RGB { public double R, G, B; }
        struct HSV { public double H, S, V; }
        
        
        /// <summary> Calculates the appropriate text colour for the given background colour </summary>
        /// <remarks> Returns black or white colour depending on brightness of the given colour </remarks>
        public static Color CalcTextColor(ColorDesc bgColor) {
            // https://stackoverflow.com/questions/596216/formula-to-determine-perceived-brightness-of-rgb-color
            RGB c = sRGBToLinear(bgColor);
            double L = 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;
            return L > 0.179 ? Color.Black : Color.White;
        }
        
        
        /// <summary> Converts gamma corrected RGB to linear RGB </summary>
        static RGB sRGBToLinear(ColorDesc c) {
            RGB rgb;
            rgb.R = ToLinear(c.R);
            rgb.G = ToLinear(c.G);
            rgb.B = ToLinear(c.B);
            return rgb;
        }
        
        /// <summary> Converts a gamma corrected value to a linear value </summary>
        static double ToLinear(double c) {
            c /= 255.0;
            if (c <= 0.03928) return c / 12.92;
            return Math.Pow((c + 0.055) / 1.055, 2.4);
        }
        
        
        /// <summary> Adjust text colour to be easier on the eye on a light or dark background </summary>
        public static Color AdjustBrightness(ColorDesc color, bool nightMode) {
            double brightness;
            RGB rgb    = sRGBToLinear(color);
            HSV hsv    = RGBToHSV(rgb);
            brightness = WeightedW3C(rgb);
            // TODO is there a better way of doingthis
            
            if (nightMode) {
                // lighten very dark colors
                while (brightness < 0.1 && hsv.V >= 0 && hsv.V <= 0.99) 
                {
                    hsv.V += 0.01;
                    brightness = WeightedW3C(HSVToRGB(hsv));
                }
            } else {
                // darken overly bright colors
                while (brightness > 0.5 && hsv.V >= 0.01 && hsv.V <= 1) 
                {
                    hsv.V -= 0.01;
                    brightness = WeightedW3C(HSVToRGB(hsv));
                }
            }            
            
            rgb = HSVToRGB(hsv);
            int R = ToRGB(rgb.R);
            int G = ToRGB(rgb.G);
            int B = ToRGB(rgb.B);
            return Color.FromArgb(R, G, B);
        }
        
        static double sRGB(double c) {
            if (c <= 0.00305) return c * 12.92;
            return 1.055 * Math.Pow(c, 1.0 / 2.4) - 0.055;
        }
        
        static int ToRGB(double c) {
            c = sRGB(c);
            if (c < 0.0) return 0;
            if (c > 1.0) return 255;
            return (int)(c * 255);
        }

        static double WeightedW3C(RGB rgb) {
            return rgb.R * 0.299 + rgb.G * 0.587 + rgb.B * 0.11;
        }
        
        // https://stackoverflow.com/questions/3018313/algorithm-to-convert-rgb-to-hsv-and-hsv-to-rgb-in-range-0-255-for-both
        static HSV RGBToHSV(RGB rgb) {
            HSV hsv = default(HSV);
            double maxc = Math.Max(rgb.R, Math.Max(rgb.G, rgb.B));
            double minc = Math.Min(rgb.R, Math.Min(rgb.G, rgb.B));
            
            hsv.V = maxc;
            if (minc == maxc) return hsv;
            double D = maxc - minc;
            
            if (maxc == 0.0) return hsv;
            hsv.S = D / maxc;
            
            double rc = (maxc - rgb.R) / D;
            double gc = (maxc - rgb.G) / D;
            double bc = (maxc - rgb.B) / D;
            
            if (rgb.R == maxc) {
                hsv.H = 0.0 + bc - gc;
            } else if (rgb.G == maxc) {
                hsv.H = 2.0 + rc - bc;
            } else {
                hsv.H = 4.0 + gc - rc;
            }
            
            hsv.H *= 60.0;
            if (hsv.H < 0.0) hsv.H += 360.0;
            return hsv;
        }
        
        static RGB HSVToRGB(HSV hsv) {
            double hh, p, q, t, ff;
            int    i;
            RGB    rgb = default(RGB);

            if (hsv.S <= 0.0) {
                rgb.R = hsv.V;
                rgb.G = hsv.V;
                rgb.B = hsv.V;
                return rgb;
            }
            
            hh = hsv.H;
            if(hh >= 360.0) hh = 0.0;
            hh /= 60.0;
            
            i = (int)hh;
            ff = hh - i;
            p = hsv.V * (1.0 - hsv.S);
            q = hsv.V * (1.0 - (hsv.S * ff));
            t = hsv.V * (1.0 - (hsv.S * (1.0 - ff)));

            switch (i)
            {
                case 0:
                    rgb.R = hsv.V;
                    rgb.G = t;
                    rgb.B = p;
                    break;
                case 1:
                    rgb.R = q;
                    rgb.G = hsv.V;
                    rgb.B = p;
                    break;
                case 2:
                    rgb.R = p;
                    rgb.G = hsv.V;
                    rgb.B = t;
                    break;

                case 3:
                    rgb.R = p;
                    rgb.G = q;
                    rgb.B = hsv.V;
                    break;
                case 4:
                    rgb.R = t;
                    rgb.G = p;
                    rgb.B = hsv.V;
                    break;
                case 5:
                    rgb.R = hsv.V;
                    rgb.G = p;
                    rgb.B = q;
                    break;
            }
            return rgb;
        }
    }
}