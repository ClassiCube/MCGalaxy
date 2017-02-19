/*
    Copyright 2011 MCForge
        
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Drawing {
    
    /// <summary> Represents a mapping of block ids to RGB colours. </summary>
    public sealed class ImagePalette {
        
        /// <summary> The name of this palette. </summary>
        public string Name;
        
        /// <summary> Relative file path on disc. </summary>
        public string FileName { get { return "extra/palettes/" + Name + ".pal"; } }
        
        /// <summary> Blocks in the front, used in vertical and layer mode. </summary>
        public PaletteEntry[] FrontLayer;
        
        /// <summary> Blocks in the back, used only in two-layer vertical mode. </summary>
        public PaletteEntry[] BackLayer;
        
        public ImagePalette(string name, PaletteEntry[] front, PaletteEntry[] back) {
            Name = name; FrontLayer = front; BackLayer = back;
        }
        
        
        /// <summary> All supported palettes. </summary>
        public static List<ImagePalette> Palettes = new List<ImagePalette>();
        
        public static ImagePalette Find(string name) {
            foreach (ImagePalette entry in Palettes) {
                if (entry.Name.CaselessEq(name)) return entry;
            }
            return null;
        }
        
        public static void Load() {
            Palettes.Clear();
            Palettes.Add(new ImagePalette("Color", Color_Front, Color_Back));
            Palettes.Add(new ImagePalette("Grayscale", Grayscale_Front, Grayscale_Back));
            Palettes.Add(new ImagePalette("BlackWhite", BlackWhite_Front, null));
            Palettes.Add(new ImagePalette("SimpleGrayscale", Grayscale_Mathematical, null));
            
            if (!Directory.Exists("extra/palettes"))
                Directory.CreateDirectory("extra/palettes");
            string[] files = Directory.GetFiles("extra/palettes");
            foreach (string file in files)
                LoadPalette(file);
        }

        static void LoadPalette(string file) {
            string name = Path.GetFileNameWithoutExtension(file);
            ImagePalette palette = Find(name);
            if (palette != null) Palettes.Remove(palette);
            palette = new ImagePalette(name, null, null);
            
            string[] lines = File.ReadAllLines(file);
            List<PaletteEntry> front = new List<PaletteEntry>();
            List<PaletteEntry> back = new List<PaletteEntry>();
            
            foreach (string line in lines) {
                if (line.StartsWith("#") || line.Length == 0) continue;
                
                string[] parts = line.Split(':');
                if (parts.Length != 5) continue;
                
                if (parts[0].CaselessEq("front")) {
                    front.Add(ParseEntry(parts));
                } else if (parts[0].CaselessEq("back")) {
                    back.Add(ParseEntry(parts));
                }
            }
            
            palette.FrontLayer = front.ToArray();
            if (back.Count > 0) palette.BackLayer = back.ToArray();
            Palettes.Add(palette);
        }
        
        static PaletteEntry ParseEntry(string[] parts) {
            byte r = byte.Parse(parts[4]), g = byte.Parse(parts[3]);
            byte b = byte.Parse(parts[2]), block = byte.Parse(parts[1]);
            return new PaletteEntry(r, g, b, block);
        }
        
        public static void SavePalette(ImagePalette palette) {
            using (StreamWriter w = new StreamWriter(palette.FileName)) {
                w.WriteLine("#Line layout - type:block:red:green:blue");
                
                if (palette.FrontLayer != null) {
                    foreach (PaletteEntry e in palette.FrontLayer)
                        w.WriteLine("front:" + e.Block + ":" + e.R + ":" + e.G + ":" + e.B);
                }
                
                if (palette.BackLayer != null) {
                    foreach (PaletteEntry e in palette.BackLayer)
                        w.WriteLine("back:" + e.Block + ":" + e.R + ":" + e.G + ":" + e.B);
                }
            }
        }
        
        public static void Add(string name) {
            ImagePalette palette = new ImagePalette(name, null, null);
            Palettes.Add(palette);
            using (File.Create(palette.FileName)) { }
        }
        
        public static void Remove(ImagePalette palette) {
            Palettes.Remove(palette);
            if (!File.Exists(palette.FileName)) return;
            File.Delete(palette.FileName);
        }

        
        static PaletteEntry[] Color_Front = {
            new PaletteEntry(128, 86,   57, Block.dirt),
            new PaletteEntry(162, 129,  75, Block.wood),
            new PaletteEntry(244, 237, 174, Block.sand),
            new PaletteEntry(226,  31,  38, Block.red),
            new PaletteEntry(223, 135,  37, Block.orange),
            new PaletteEntry(230, 241,  25, Block.yellow),
            new PaletteEntry(127, 234,  26, Block.lightgreen),
            new PaletteEntry(25,  234,  20, Block.green),
            new PaletteEntry(31,  234, 122, Block.aquagreen),
            new PaletteEntry(27,  239, 225, Block.cyan),
            new PaletteEntry(99,  166, 226, Block.lightblue),
            new PaletteEntry(111, 124, 235, Block.blue),
            new PaletteEntry(126,  34, 218, Block.purple),
            new PaletteEntry(170,  71, 219, Block.lightpurple),
            new PaletteEntry(227,  39, 225, Block.pink),
            new PaletteEntry(234,  39, 121, Block.darkpink),
            new PaletteEntry(46,   68,  47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
            new PaletteEntry(163, 163, 163, Block.staircasefull),
            new PaletteEntry(0,    0,    0, Block.obsidian),
        };
        /*Turns out the back layer blocks are handled awfully.
            new PaletteEntry(217, 131, 155, 55),
            new PaletteEntry(56,   77,  24, 56),
            new PaletteEntry(86,   51,  28, 57),
            new PaletteEntry(39,   51, 154, 58),
            new PaletteEntry(39,  117, 149, 59),*/

        static PaletteEntry[] Color_Back = {
            new PaletteEntry(57,   38,  25, Block.dirt),
            new PaletteEntry(72,   57,  33, Block.wood),
            new PaletteEntry(109, 105,  77, Block.sand),
            new PaletteEntry(41,   31,  16, Block.trunk),
            new PaletteEntry(101,  13,  16, Block.red),
            new PaletteEntry(99,   60,  16, Block.orange),
            new PaletteEntry(102, 107,  11, Block.yellow),
            new PaletteEntry(56,  104,  11, Block.lightgreen),
            new PaletteEntry(11,  104,   8, Block.green),
            new PaletteEntry(13,  104,  54, Block.aquagreen),
            new PaletteEntry(12,  106, 100, Block.cyan),
            new PaletteEntry(44,   74, 101, Block.lightblue),
            new PaletteEntry(49,   55, 105, Block.blue),
            new PaletteEntry(56,   15,  97, Block.purple),
            new PaletteEntry(75,   31,  97, Block.lightpurple),
            new PaletteEntry(101,  17, 100, Block.pink),
            new PaletteEntry(104,  17,  54, Block.darkpink),
            new PaletteEntry(20,   30,  21, Block.darkgrey),
            new PaletteEntry(60,   64,  58, Block.lightgrey),
            new PaletteEntry(102, 107, 100, Block.white),
            new PaletteEntry(0, 0, 0, Block.obsidian),
        };
        
        static PaletteEntry[] Grayscale_Front = {
            new PaletteEntry(0,     0,   0, Block.obsidian),
            new PaletteEntry(46,   68,  47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
        };
        
        static PaletteEntry[] Grayscale_Back = {
            new PaletteEntry(0,     0,   0, Block.obsidian),
            new PaletteEntry(20,   30,  21, Block.darkgrey),
            new PaletteEntry(60,   64,  58, Block.lightgrey),
            new PaletteEntry(102, 107, 100, Block.white),
        };
        
        static PaletteEntry[] Grayscale_Mathematical = {
            new PaletteEntry(32,   32,  32, Block.obsidian),
            new PaletteEntry(96,   96,  96, Block.darkgrey),
            new PaletteEntry(160, 160, 160, Block.lightgrey),
            new PaletteEntry(224, 224, 224, Block.white),
        };
        
        static PaletteEntry[] BlackWhite_Front = {
            new PaletteEntry(255, 255, 255, Block.white),
            new PaletteEntry(0,     0,   0, Block.obsidian),
        };
    }
    
    public struct PaletteEntry  {
        public byte R, G, B, Block;
        
        public PaletteEntry(byte r, byte g, byte b, byte block) {
            R = r; G = g; B = b; Block = block;
        }
    }
}
