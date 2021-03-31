/*
    Copyright 2011 MCForge
        
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Generator;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdImageprint : Command2 {
        public override string name { get { return "ImagePrint"; } }
        public override string shortcut { get { return "Img"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ImgPrint"), new CommandAlias("PrintImg"),
                    new CommandAlias("ImgDraw"), new CommandAlias("DrawImg"),
                    new CommandAlias("DrawImage"), new CommandAlias("PrintImage") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (!Directory.Exists("extra/images/"))
                Directory.CreateDirectory("extra/images/");
            if (message.Length == 0) { Help(p); return; }
            string[] parts = message.SplitSpaces(5);
            
            DrawArgs dArgs = new DrawArgs();
            dArgs.Pal = ImagePalette.Find("color");
            if (dArgs.Pal == null) dArgs.Pal = ImagePalette.Palettes[0];
            
            if (parts.Length > 1) {
                dArgs.Pal = ImagePalette.Find(parts[1]);
                if (dArgs.Pal == null) {
                    p.Message("Palette {0} not found.", parts[1]); return;
                }
                
                if (dArgs.Pal.Entries == null || dArgs.Pal.Entries.Length == 0) {
                    p.Message("Palette {0} does not have any entries", dArgs.Pal.Name);
                    p.Message("Use &T/Palette &Sto add entries to it"); return;
                }
            }
            
            if (parts.Length > 2) {
                string mode = parts[2];
                if (mode.CaselessEq("horizontal"))     dArgs.Layer = true;
                if (mode.CaselessEq("vertical2layer")) dArgs.Dual  = true;
            }
            
            if (parts.Length > 4) {
                if (!CommandParser.GetInt(p, parts[3], "Width",  ref dArgs.Width,  1, 1024)) return;
                if (!CommandParser.GetInt(p, parts[4], "Height", ref dArgs.Height, 1, 1024)) return;
            }
            
            if (parts[0].IndexOf('.') != -1) {
                dArgs.Data = HttpUtil.DownloadImage(parts[0], p);
                if (dArgs.Data == null) return;
            } else {
                string path = "extra/images/" + parts[0] + ".bmp";
                if (!File.Exists(path)) { p.Message("{0} does not exist", path); return; }
                dArgs.Data = File.ReadAllBytes(path);
            }

            p.Message("Place or break two blocks to determine direction.");
            p.MakeSelection(2, "Selecting direction for &SImagePrint", dArgs, DoImage);
        }
        
        bool DoImage(Player p, Vec3S32[] m, object state, BlockID block) {
            if (m[0].X == m[1].X && m[0].Z == m[1].Z) { p.Message("No direction was selected"); return false; }

            Thread thread = new Thread(() => DoDrawImage(p, m, (DrawArgs)state));
            thread.Name = "MCG_ImagePrint";
            thread.Start();
            return false;
        }
        
        void DoDrawImage(Player p, Vec3S32[] m, DrawArgs dArgs) {
            try {
                DoDrawImageCore(p, m, dArgs);
            } catch (Exception ex) {
                Logger.LogError("Error drawing image", ex);
                // Do not want it taking down the whole server if error occurs
            }
        }
        
        void DoDrawImageCore(Player p, Vec3S32[] marks, DrawArgs dArgs) {
            Bitmap bmp = HeightmapGen.DecodeImage(dArgs.Data, p);
            if (bmp == null) return;

            ImagePrintDrawOp op = new ImagePrintDrawOp();
            op.LayerMode = dArgs.Layer; op.DualLayer = dArgs.Dual;
            op.CalcState(marks);
            
            int width  = dArgs.Width  == 0 ? bmp.Width  : dArgs.Width;
            int height = dArgs.Height == 0 ? bmp.Height : dArgs.Height;
            Clamp(p, marks, op, ref width, ref height);
            
            if (width < bmp.Width || height < bmp.Height) {
                bmp = Resize(bmp, width, height);
            }
            
            op.SetLevel(p.level);
            op.Player = p; op.Source = bmp; op.Palette = dArgs.Pal;
            DrawOpPerformer.Do(op, null, p, marks, false);
        }
        
        void Clamp(Player p, Vec3S32[] m, ImagePrintDrawOp op, ref int width, ref int height) {
            Level lvl = p.level;
            Vec3S32 xEnd = m[0] + op.dx * (width  - 1);
            Vec3S32 yEnd = m[0] + op.dy * (height - 1);
            if (lvl.IsValidPos(xEnd.X, xEnd.Y, xEnd.Z) && lvl.IsValidPos(yEnd.X, yEnd.Y, yEnd.Z)) return;
            
            int resizedWidth  = width  - LargestDelta(lvl, xEnd);
            int resizedHeight = height - LargestDelta(lvl, yEnd);
            
            // Preserve aspect ratio of image
            float ratio   = Math.Min(resizedWidth / (float)width, resizedHeight / (float)height);
            resizedWidth  = Math.Max(1, (int)(width  * ratio));
            resizedHeight = Math.Max(1, (int)(height * ratio));
            
            p.Message("&WImage is too large ({0}x{1}), resizing to ({2}x{3})",
                      width, height, resizedWidth, resizedHeight);
            width = resizedWidth; height = resizedHeight;
        }
        
        static Bitmap Resize(Bitmap bmp, int width, int height) {
            Bitmap resized = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(resized)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode     = SmoothingMode.HighQuality;
                g.PixelOffsetMode   = PixelOffsetMode.HighQuality;
                g.DrawImage(bmp, 0, 0, width, height);
            }
            
            bmp.Dispose();
            return resized;
        }
        
        static int LargestDelta(Level lvl, Vec3S32 point) {
            Vec3S32 clamped = lvl.ClampPos(point);
            int dx = Math.Abs(point.X - clamped.X);
            int dy = Math.Abs(point.Y - clamped.Y);
            int dz = Math.Abs(point.Z - clamped.Z);
            return Math.Max(dx, Math.Max(dy, dz));
        }
        
        public override void Help(Player p) {
            p.Message("&T/ImagePrint [file/url] [palette] <mode> <width height>");
            p.Message("&HPrints image from given URL, or from a .bmp file in /extra/images/ folder");
            p.Message("&HPalettes: &f{0}", ImagePalette.Palettes.Join(pal => pal.Name));
            p.Message("&HModes: &fVertical, Vertical2Layer, Horizontal");
            p.Message("&H  <width height> optionally resize the printed image");
        }

        class DrawArgs { public bool Layer, Dual; public ImagePalette Pal; public byte[] Data; public int Width, Height; }
    }
}

