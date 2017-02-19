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

namespace MCGalaxy.Commands.Building {
    public sealed class CmdPalette : Command {
        public override string name { get { return "palette"; } }
        public override string shortcut { get { return "imgpalette"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces(5);
            if (message == "") { Help(p); return; }
            
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
                Player.Message(p, "Palette {0} already exists.", args[1]);
            } else {
                ImagePalette.Add(args[1]);
                Player.Message(p, "Created palette {0}", args[1]);
            }
        }
        
        void HandleDelete(Player p, string[] args) {
            if (args.Length != 2) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                Player.Message(p, "Palette {0} does not exist.", args[1]);
            } else {
                ImagePalette.Remove(palette);
                Player.Message(p, "Removed palette {0}", args[1]);
            }
        }
        
        void HandleAdd(Player p, string[] args) {
            if (args.Length != 4) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                Player.Message(p, "Palette {0} does not exist.", args[1]); return;
            }
            
            byte block = GetBlock(p, args[2]);
            if (block == Block.Invalid) return;
            
            if (!Utils.CheckHex(p, ref args[3])) return;
            CustomColor rgb = Colors.ParseHex(args[3]);
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
            Player.Message(p, "Added block to entries of palette {0}", palette.Name);
        }
        
        void HandleRemove(Player p, string[] args) {
            if (args.Length != 3) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                Player.Message(p, "Palette {0} does not exist.", args[1]); return;
            }
            
            byte block = GetBlock(p, args[2]);
            if (block == Block.Invalid) return;
            RemoveEntry(p, palette, block);
        }
        
        
        static void RemoveEntry(Player p, ImagePalette palette, byte block) {
            PaletteEntry[] entries = palette.Entries;
            if (entries == null) {
                Player.Message(p, "Block not found in entries of palette {0}", palette.Name);
            }
            
            List<PaletteEntry> newEntries = new List<PaletteEntry>();
            foreach (PaletteEntry entry in entries) {
                if (entry.Block == block) continue;
                newEntries.Add(entry);
            }
            
            if (newEntries.Count == entries.Length) {
                Player.Message(p, "Block not found in entries of palette {0}", palette.Name); return;
            }
            
            palette.Entries = newEntries.ToArray();
            palette.Save();
            Player.Message(p, "Removed block from entries of palette {0}", palette.Name);
        }

        static byte GetBlock(Player p, string name) {
            byte extBlock;
            int block = DrawCmd.GetBlock(p, name, out extBlock);
            
            if (block == -1) return Block.Invalid;
            if (block == Block.Invalid) {
                Player.Message(p, "Skip block may not be used for palettes."); return Block.Invalid;
            }
            if (block >= Block.CpeCount) {
                Player.Message(p, "Physics blocks may not be used for palettes."); return Block.Invalid;
            }
            
            return block == Block.custom_block ? extBlock : (byte)block;
        }
        
        void HandleEntries(Player p, string[] args) {
            if (args.Length < 2 || args.Length > 3) { Help(p); return; }
            
            ImagePalette palette = ImagePalette.Find(args[1]);
            if (palette == null) {
                Player.Message(p, "Palette {0} does not exist.", args[1]); return;
            }
            
            string modifer = args.Length > 2 ? args[2] : "";
            MultiPageOutput.Output(p, palette.Entries, (e) => FormatEntry(e, p.level), 
                                   "palette entries", "entries", modifer, true);
        }
        
        static string FormatEntry(PaletteEntry e, Level lvl) {
            byte block = e.Block, extBlock = 0;
            if (block >= Block.CpeCount) { extBlock = block; block = Block.CpeCount; }
            return lvl.BlockName(block, extBlock) + " - " + Utils.Hex(e.R, e.G, e.B);
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/palette create/delete [name]");
            Player.Message(p, "%HCreates or deletes a palette for %T/imageprint");
            Player.Message(p, "%T/palette add [name] [block] [hex color]");
            Player.Message(p, "%HAdds a block to a palette's entries.");
            Player.Message(p, "%T/palette remove [name] [block]");
            Player.Message(p, "%HRemoves a block from a palette's entries.");
            Player.Message(p, "%T/palette entries [name]");
            Player.Message(p, "%HLists the entries of that palette.");
            Player.Message(p, "%HPalettes: &f{0}", ImagePalette.Palettes.Join(pal => pal.Name));
        }
    }
}

