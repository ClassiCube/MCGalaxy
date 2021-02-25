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
using MCGalaxy.Drawing;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPalette : Command2 {
        public override string name { get { return "Palette"; } }
        public override string shortcut { get { return "ImgPalette"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(5);
            if (message.Length == 0) { Help(p); return; }
            
            if (args[0].CaselessEq("create")) {
                HandleCreate(p, args);
            } else if (args[0].CaselessEq("delete")) {
                HandleDelete(p, args);
            } else if (args[0].CaselessEq("add")) {
                HandleAdd(p, args);
            } else if (args[0].CaselessEq("remove")) {
                HandleRemove(p, args);
            } else if (args[0].CaselessEq("entries")) {
                HandleEntries(p, args);
            } else {
                Help(p);
            }
        }
        
        void HandleCreate(Player p, string[] args) {
            if (args.Length != 2) { Help(p); return; }
            if (!Formatter.ValidName(p, args[1], "palette")) return;
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette != null) {
                p.Message("Palette {0} already exists.", args[1]);
            } else {
                ImagePalette.Add(args[1]);
                p.Message("Created palette {0}", args[1]);
            }
        }
        
        void HandleDelete(Player p, string[] args) {
            if (args.Length != 2) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                p.Message("Palette {0} does not exist.", args[1]);
            } else {
                ImagePalette.Remove(palette);
                p.Message("Removed palette {0}", args[1]);
            }
        }
        
        void HandleAdd(Player p, string[] args) {
            if (args.Length != 4) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                p.Message("Palette {0} does not exist.", args[1]); return;
            }
            
            BlockID block;
            if (!CommandParser.GetBlock(p, args[2], out block)) return;
            
            ColorDesc rgb = default(ColorDesc);
            if (!CommandParser.GetHex(p, args[3], ref rgb)) return;
            PaletteEntry entry = new PaletteEntry(rgb.R, rgb.G, rgb.B, block);
            AddEntry(p, palette, entry);
        }
        
        static void AddEntry(Player p, ImagePalette palette, PaletteEntry entry) {
            PaletteEntry[] entries = palette.Entries;
            List<PaletteEntry> newEntries = new List<PaletteEntry>();
            if (entries != null) newEntries.AddRange(entries);
            
            newEntries.Add(entry);
            palette.Entries = newEntries.ToArray();
            palette.Save();
            p.Message("Added block to entries of palette {0}", palette.Name);
        }
        
        void HandleRemove(Player p, string[] args) {
            if (args.Length != 3) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                p.Message("Palette {0} does not exist.", args[1]); return;
            }
            
            BlockID block;
            if (!CommandParser.GetBlock(p, args[2], out block)) return;
            RemoveEntry(p, palette, block);
        }
        
        
        static void RemoveEntry(Player p, ImagePalette palette, BlockID block) {
            PaletteEntry[] entries = palette.Entries;
            if (entries == null) {
                p.Message("Block not found in entries of palette {0}", palette.Name);
            }
            
            List<PaletteEntry> newEntries = new List<PaletteEntry>();
            foreach (PaletteEntry entry in entries) {
                if (entry.Block == block) continue;
                newEntries.Add(entry);
            }
            
            if (newEntries.Count == entries.Length) {
                p.Message("Block not found in entries of palette {0}", palette.Name); return;
            }
            
            palette.Entries = newEntries.ToArray();
            palette.Save();
            p.Message("Removed block from entries of palette {0}", palette.Name);
        }
        
        void HandleEntries(Player p, string[] args) {
            if (args.Length < 2 || args.Length > 3) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                p.Message("Palette {0} does not exist.", args[1]); return;
            }
            
            string modifer = args.Length > 2 ? args[2] : "";
            MultiPageOutput.Output(p, palette.Entries, (e) => FormatEntry(e, p), 
                                   "Palette entries", "entries", modifer, true);
        }
        
        static string FormatEntry(PaletteEntry e, Player p) {
            return Block.GetName(p, e.Block) + " - " + Utils.Hex(e.R, e.G, e.B);
        }

        public override void Help(Player p) {
            p.Message("&T/Palette create/delete [name]");
            p.Message("&HCreates or deletes a palette for &T/ImagePrint");
            p.Message("&T/Palette add [name] [block] [hex color]");
            p.Message("&HAdds a block to a palette's entries.");
            p.Message("&T/Palette remove [name] [block]");
            p.Message("&HRemoves a block from a palette's entries.");
            p.Message("&T/Palette entries [name]");
            p.Message("&HLists the entries of that palette.");
            p.Message("&HPalettes: &f{0}", ImagePalette.Palettes.Join(pal => pal.Name));
        }
    }
}

