/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using System.IO.Compression;
using MCGalaxy.Blocks;
using MCGalaxy.DB;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Brushes;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdCopy : Command {
        public override string name { get { return "copy"; } }
        public override string shortcut { get { return "c"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("cut", "cut") }; }
        }

        public override void Use(Player p, string message) {
            int offsetIndex = message.IndexOf('@');
            if (offsetIndex != -1)
                message = message.Replace("@ ", "").Replace("@", "");
            
            string[] parts = message.SplitSpaces();
            string opt = parts[0].ToLower();
            
            if (opt == "save") {
                if (parts.Length != 2) { Help(p); return; }
                if (!Formatter.ValidName(p, parts[1], "saved copy")) return;
                SaveCopy(p, parts[1]);
            } else if (opt == "load") {
                if (parts.Length != 2) { Help(p); return; }
                if (!Formatter.ValidName(p, parts[1], "saved copy")) return;
                LoadCopy(p, parts[1]);
            } else if (opt == "delete") {
                if (parts.Length != 2) { Help(p); return; }
                if (!Formatter.ValidName(p, parts[1], "saved copy")) return;
                
                string path = FindCopy(p.name, parts[1]);
                if (path == null) { Player.Message(p, "No such copy exists."); return; }                
                File.Delete(path);
                Player.Message(p, "Deleted copy " + parts[1]);
            } else if (opt == "list") {
                string dir = "extra/savecopy/" + p.name;
                if (!Directory.Exists(dir)) {
                    Player.Message(p, "No such directory exists"); return;
                }
                
                string[] files = Directory.GetFiles(dir);
                for (int i = 0; i < files.Length; i++) {
                    Player.Message(p, Path.GetFileNameWithoutExtension(files[i]));
                }
            } else {
                HandleOther(p, opt, parts, offsetIndex);
            }
        }
        
        void HandleOther(Player p, string opt, string[] parts, int offsetIndex) {
            CopyArgs cArgs = default(CopyArgs);
            cArgs.offsetIndex = offsetIndex;
            
            if (opt == "cut") {
                cArgs.type = 1;
            } else if (opt == "air") {
                cArgs.type = 2;
            } else if (!String.IsNullOrEmpty(opt)) {
                Help(p); return;
            }

            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, "Selecting region for %SCopy", cArgs, DoCopy);
        }

        bool DoCopy(Player p, Vec3S32[] m, object state, ExtBlock block) {
            CopyArgs cArgs = (CopyArgs)state;
            ushort minX = (ushort)Math.Min(m[0].X, m[1].X), maxX = (ushort)Math.Max(m[0].X, m[1].X);
            ushort minY = (ushort)Math.Min(m[0].Y, m[1].Y), maxY = (ushort)Math.Max(m[0].Y, m[1].Y);
            ushort minZ = (ushort)Math.Min(m[0].Z, m[1].Z), maxZ = (ushort)Math.Max(m[0].Z, m[1].Z);
            
            CopyState cState = new CopyState(minX, minY, minZ, maxX - minX + 1,
                                            maxY - minY + 1, maxZ - minZ + 1);
            cState.OriginX = m[0].X; cState.OriginY = m[0].Y; cState.OriginZ = m[0].Z;
            
            int index = 0; cState.UsedBlocks = 0;
            cState.PasteAir = cArgs.type == 2;
            
            for (ushort yy = minY; yy <= maxY; ++yy)
                for (ushort zz = minZ; zz <= maxZ; ++zz)
                    for (ushort xx = minX; xx <= maxX; ++xx)
            {
                block = p.level.GetBlock(xx, yy, zz);
                if (!p.group.Blocks[block.BlockID]) { index++; continue; } // TODO: will need to fix this when extblock permissions
                
                if (block.BlockID != Block.Air || cState.PasteAir) 
                    cState.UsedBlocks++;
                cState.Set(block, index);
                index++;
            }
            
            if (cState.UsedBlocks > p.group.MaxBlocks) {
                Player.Message(p, "You tried to copy {0} blocks. You cannot copy more than {1} blocks.", 
                               cState.UsedBlocks, p.group.MaxBlocks);
                cState.Clear(); cState = null;
                return false;
            }
            
            p.CopyBuffer = cState;
            if (cArgs.type == 1) {
                DrawOp op = new CuboidDrawOp();
                op.Flags = BlockDBFlags.Cut;
                Brush brush = new SolidBrush(ExtBlock.Air);
                
                Vec3S32[] marks = new Vec3S32[] { 
                    new Vec3S32(minX, minY, minZ),
                    new Vec3S32(maxX, maxY, maxZ) 
                };
                DrawOpPerformer.Do(op, brush, p, marks, false);
            }

            string format = "Copied &a{0} %Sblocks." +
                (cState.PasteAir ? "" : " To also copy air blocks, use %T/copy air");
            Player.Message(p, format, cState.UsedBlocks);
            if (cArgs.offsetIndex != -1) {
                Player.Message(p, "Place a block to determine where to paste from");
                p.MakeSelection(1, "Selecting paste offset for %SCopy", null, PlacedOffsetMark);
            }
            return false;
        }

        bool PlacedOffsetMark(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            CopyState copy = p.CopyBuffer;          
            copy.Offset.X = copy.OriginX - marks[0].X;
            copy.Offset.Y = copy.OriginY - marks[0].Y;
            copy.Offset.Z = copy.OriginZ - marks[0].Z;
            Player.Message(p, "Set offset of where to paste from.");
            return false;
        }

        struct CopyArgs { public int type, offsetIndex; }
        
        
        void SaveCopy(Player p, string file) {
            if (!Directory.Exists("extra/savecopy"))
                Directory.CreateDirectory("extra/savecopy");
            if (!Directory.Exists("extra/savecopy/" + p.name))
                Directory.CreateDirectory("extra/savecopy/" + p.name);
            if (Directory.GetFiles("extra/savecopy/" + p.name).Length > 15) {
                Player.Message(p, "You can only save a maxmium of 15 copies. /copy delete some.");
                return;
            }
            
            string path = "extra/savecopy/" + p.name + "/" + file + ".cpb";
            using (FileStream fs = File.Create(path))
                using(GZipStream gs = new GZipStream(fs, CompressionMode.Compress))
            {
                p.CopyBuffer.SaveTo(gs);
            }
            Player.Message(p, "Saved copy as " + file);
        }

        void LoadCopy(Player p, string file) {
            string path = FindCopy(p.name, file);
            if (path == null) { Player.Message(p, "No such copy exists"); return; }

            using (FileStream fs = File.OpenRead(path))
                using (GZipStream gs = new GZipStream(fs, CompressionMode.Decompress))
            {
                CopyState state = new CopyState(0, 0, 0, 0, 0, 0);
                if (path.CaselessEnds(".cpb")) {
                    state.LoadFrom(gs);
                } else {
                    state.LoadFromOld(gs, fs);
                }
                p.CopyBuffer = state;
            }
            Player.Message(p, "Loaded copy as " + file);
        }
        
        static string FindCopy(string name, string file) {
            string path = "extra/savecopy/" + name + "/" + file;
            bool existsNew = File.Exists(path + ".cpb");
            bool existsOld = File.Exists(path + ".cpy");
            
            if (!existsNew && !existsOld) return null;
            string ext = existsNew ? ".cpb" : ".cpy";
            return path + ext;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/copy %H- Copies the blocks in an area.");
            Player.Message(p, "%T/copy save [name] %H- Saves what you have copied.");
            Player.Message(p, "%T/copy load [name] %H- Loads what you have saved.");
            Player.Message(p, "%T/copy delete [name] %H- Deletes the specified copy.");
            Player.Message(p, "%T/copy list %H- Lists all saved copies you have");
            Player.Message(p, "%T/copy cut %H- Copies the blocks in an area, then removes them.");
            Player.Message(p, "%T/copy air %H- Copies the blocks in an area, including air.");
            Player.Message(p, "/copy @ - @ toggle for all the above, gives you a third click after copying that determines where to paste from");
        }
    }
}
