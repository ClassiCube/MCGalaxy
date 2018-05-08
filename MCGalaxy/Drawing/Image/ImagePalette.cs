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
using BlockID = System.UInt16;

namespace MCGalaxy.Drawing {
    
    /// <summary> Represents a mapping of block ids to RGB colors. </summary>
    public sealed class ImagePalette {
        public string Name;
        public string FileName { get { return "extra/palettes/" + Name + ".pal"; } }
        public PaletteEntry[] Entries;
        
        public ImagePalette(string name, PaletteEntry[] entries) {
            Name = name; Entries = entries;
        }
        
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
            string[] parts = new string[5];
            
            foreach (string line in lines) {
                if (line.StartsWith("#") || line.Length == 0) continue;
                
                line.FixedSplit(parts, ':');
                if (parts[3] == null || parts[4] != null) continue; // not a proper line
                entries.Add(ParseEntry(parts));
            }

            palette = new ImagePalette(name, entries.ToArray());
            Palettes.Add(palette);
        }
        
        static PaletteEntry ParseEntry(string[] parts) {
            BlockID block = BlockID.Parse(parts[0]);
            block = Block.MapOldRaw(block);
            
            byte r = byte.Parse(parts[1]);
            byte g = byte.Parse(parts[2]);
            byte b = byte.Parse(parts[3]);            
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

        
        static PaletteEntry[] Color = new PaletteEntry[] {
            new PaletteEntry(128, 86,   57, Block.Dirt),
            new PaletteEntry(162, 129,  75, Block.Wood),
            new PaletteEntry(244, 237, 174, Block.Sand),
            new PaletteEntry(226,  31,  38, Block.Red),
            new PaletteEntry(223, 135,  37, Block.Orange),
            new PaletteEntry(230, 241,  25, Block.Yellow),
            new PaletteEntry(127, 234,  26, Block.Lime),
            new PaletteEntry(25,  234,  20, Block.Green),
            new PaletteEntry(31,  234, 122, Block.Teal),
            new PaletteEntry(27,  239, 225, Block.Aqua),
            new PaletteEntry(99,  166, 226, Block.Cyan),
            new PaletteEntry(111, 124, 235, Block.Blue),
            new PaletteEntry(126,  34, 218, Block.Indigo),
            new PaletteEntry(170,  71, 219, Block.Violet),
            new PaletteEntry(227,  39, 225, Block.Magenta),
            new PaletteEntry(234,  39, 121, Block.Pink),
            new PaletteEntry(46,   68,  47, Block.Black),
            new PaletteEntry(135, 145, 130, Block.Gray),
            new PaletteEntry(230, 240, 225, Block.White),
            new PaletteEntry(163, 163, 163, Block.DoubleSlab),
            new PaletteEntry(0,    0,    0, Block.Obsidian),
        };      
        
        static PaletteEntry[] Grayscale = new PaletteEntry[] {
            new PaletteEntry(0,     0,   0, Block.Obsidian),
            new PaletteEntry(46,   68,  47, Block.Black),
            new PaletteEntry(135, 145, 130, Block.Gray),
            new PaletteEntry(230, 240, 225, Block.White),
        };
        
        static PaletteEntry[] GrayscaleSimple = new PaletteEntry[] {
            new PaletteEntry(32,   32,  32, Block.Obsidian),
            new PaletteEntry(96,   96,  96, Block.Black),
            new PaletteEntry(160, 160, 160, Block.Gray),
            new PaletteEntry(224, 224, 224, Block.White),
        };
        
        static PaletteEntry[] BlackWhite = new PaletteEntry[] {
            new PaletteEntry(255, 255, 255, Block.White),
            new PaletteEntry(0,     0,   0, Block.Obsidian),
        };
    }
    
    public struct PaletteEntry  {
        public byte R, G, B;
        public BlockID Block;
        
        public PaletteEntry(byte r, byte g, byte b, BlockID block) {
            R = r; G = g; B = b; Block = block;
        }
    }
}
