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
        
        /// <summary> Block mapping in this paleete. </summary>
        public PaletteEntry[] Entries;
        
        public ImagePalette(string name, PaletteEntry[] entries) {
            Name = name; Entries = entries;
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
            Palettes.Add(new ImagePalette("Color", Color));
            Palettes.Add(new ImagePalette("Grayscale", Grayscale));
            Palettes.Add(new ImagePalette("BlackWhite", BlackWhite));
            Palettes.Add(new ImagePalette("SimpleGrayscale", GrayscaleSimple));
            
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

            string[] lines = File.ReadAllLines(file);
            List<PaletteEntry> entries = new List<PaletteEntry>();
            
            foreach (string line in lines) {
                if (line.StartsWith("#") || line.Length == 0) continue;
                
                string[] parts = line.Split(':');
                if (parts.Length != 4) continue;
                entries.Add(ParseEntry(parts));
            }

            palette = new ImagePalette(name, entries.ToArray());
            Palettes.Add(palette);
        }
        
        static PaletteEntry ParseEntry(string[] parts) {
            byte r = byte.Parse(parts[1]), g = byte.Parse(parts[2]);
            byte b = byte.Parse(parts[3]), block = byte.Parse(parts[0]);
            return new PaletteEntry(r, g, b, block);
        }
        
        
        public void Save() {
            using (StreamWriter w = new StreamWriter(FileName)) {
                w.WriteLine("#Line layout - block:red:green:blue");
                
                if (Entries == null) return;
                foreach (PaletteEntry e in Entries)
                    w.WriteLine(e.Block + ":" + e.R + ":" + e.G + ":" + e.B);
            }
        }
        
        public static void Add(string name) {
            ImagePalette palette = new ImagePalette(name, null);
            Palettes.Add(palette);
            using (File.Create(palette.FileName)) { }
        }
        
        public static void Remove(ImagePalette palette) {
            Palettes.Remove(palette);
            if (!File.Exists(palette.FileName)) return;
            File.Delete(palette.FileName);
        }

        
        static PaletteEntry[] Color = {
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
        
        static PaletteEntry[] Grayscale = {
            new PaletteEntry(0,     0,   0, Block.obsidian),
            new PaletteEntry(46,   68,  47, Block.darkgrey),
            new PaletteEntry(135, 145, 130, Block.lightgrey),
            new PaletteEntry(230, 240, 225, Block.white),
        };
        
        static PaletteEntry[] GrayscaleSimple = {
            new PaletteEntry(32,   32,  32, Block.obsidian),
            new PaletteEntry(96,   96,  96, Block.darkgrey),
            new PaletteEntry(160, 160, 160, Block.lightgrey),
            new PaletteEntry(224, 224, 224, Block.white),
        };
        
        static PaletteEntry[] BlackWhite = {
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
