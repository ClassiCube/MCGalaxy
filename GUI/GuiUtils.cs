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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MCGalaxy.Gui 
{
    // NET 2.0 doesn't include the "Action delegate without parameters" type
    public delegate void UIAction();
	
    /// <summary> Shortcuts for MessageBox.Show </summary>
    public static class Popup 
    {
        public static void Message(string message, string title = "") {
            MessageBox.Show(message, title);
        }
        
        public static void Error(string message) {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        
        public static void Warning(string message) {
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        
        public static bool OKCancel(string message, string title) {
            return MessageBox.Show(message, title, MessageBoxButtons.OKCancel,
                                  MessageBoxIcon.Warning) == DialogResult.OK;
        }
        
        public static bool YesNo(string message, string title) {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, 
                                   MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
    
    public static class GuiUtils 
    {   
        /// <summary> MCGalaxy window icon (shared) </summary>
        public static Icon WinIcon;
        
        public static void SetIcon(Form form) {
            try { form.Icon = WinIcon; } catch { }
        }
        
        /// <summary> Opens the given url in the system's default web browser </summary>
        /// <remarks> Catches and logs any unhandled errors </remarks>
        public static void OpenBrowser(string url) {
            try { 
                Process.Start(url);
            } catch (Exception ex) {
                Logger.LogError("Opening url in browser", ex);
                Popup.Error("Failed to open " + url);
            }
        }
    }
    
    public static class ColorUtils
    {
        struct RGB { public double R, G, B; }
        struct HSV { public double H, S, V; }
        
        
        /// <summary> Returns black or white color depending on brightness of the given color </summary>
        public static Color CalcBackgroundColor(Color color) {
            // https://stackoverflow.com/questions/596216/formula-to-determine-perceived-brightness-of-rgb-color
            RGB c = sRGBToLinear(color);
            double L = 0.2126 * c.R + 0.7152 * c.G + 0.0722 * c.B;
            return L > 0.179 ? Color.Black : Color.White;
        }
        
        
        /// <summary> Converts gamma corrected RGB to linear RGB </summary>
        static RGB sRGBToLinear(Color c) {
            RGB rgb;
            rgb.R = Linear(c.R);
            rgb.G = Linear(c.G);
            rgb.B = Linear(c.B);
            return rgb;
        }
        
        /// <summary> Converts gamma corrected value to linear value </summary>
        static double Linear(double c) {
            c /= 255.0;
            if (c <= 0.03928) return c / 12.92;
            return Math.Pow((c + 0.055) / 1.055, 2.4);
        }
    }
}

