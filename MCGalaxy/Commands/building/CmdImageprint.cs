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
using System.IO;
using System.Net;
using System.Threading;
using MCGalaxy.Drawing;
using System.Drawing.Drawing2D;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Generator;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdImageprint : Command {
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

        public override void Use(Player p, string message) {
            if (!Directory.Exists("extra/images/"))
                Directory.CreateDirectory("extra/images/");
            if (message.Length == 0) { Help(p); return; }
            string[] parts = message.SplitSpaces(3);
            
            DrawArgs dArgs = new DrawArgs();
            dArgs.palette = ImagePalette.Find("color");
            if (dArgs.palette == null) dArgs.palette = ImagePalette.Palettes[0];
            dArgs.dualLayered = true;
            
            if (parts.Length == 3) {
                string mode = parts[2];
                if (mode.CaselessEq("horizontal")) dArgs.layer = true;
                if (mode.CaselessEq("vertical"))   dArgs.dualLayered = false;
            }
            
            if (parts.Length >= 2) {
                dArgs.palette = ImagePalette.Find(parts[1]);
                if (dArgs.palette == null) {
                    Player.Message(p, "Palette {0} not found.", parts[1]); return;
                }
                
                if (dArgs.palette.Entries == null || dArgs.palette.Entries.Length == 0) {
                    Player.Message(p, "Palette {0} does not have any entries", dArgs.palette.Name);
                    Player.Message(p, "Use %T/Palette %Sto add entries to it"); return;
                }
            }
            
            if (parts[0].IndexOf('.') != -1) {
                if (!DownloadWebFile(parts[0], p)) return;
                dArgs.name = "tempImage_" + p.name;
            } else {
                dArgs.name = parts[0];
            }

            if (!File.Exists("extra/images/" + dArgs.name + ".bmp")) {
                Player.Message(p, "The URL entered was invalid!"); return;
            }
            Player.Message(p, "Place or break two blocks to determine direction.");
            p.MakeSelection(2, "Selecting direction for %SImagePrint", dArgs, DoImage);
        }
        
        bool DownloadWebFile(string url, Player p) {
            return HeightmapGen.DownloadImage(url, "extra/images/", p);
        }
        
        bool DoImage(Player p, Vec3S32[] m, object state, ExtBlock block) {
            if (m[0].X == m[1].X && m[0].Z == m[1].Z) { Player.Message(p, "No direction was selected"); return false; }

            Thread thread = new Thread(() => DoDrawImage(p, m, (DrawArgs)state));
            thread.Name = "MCG_ImagePrint";
            thread.Start();
            return false;
        }
        
        void DoDrawImage(Player p, Vec3S32[] m, DrawArgs dArgs) {
            try {
                DoDrawImageCore(p, m, dArgs);
            } catch (Exception ex) {
                Logger.LogError(ex); // Do not want it taking down the whole server if error occurs
            }
        }
        
        void DoDrawImageCore(Player p, Vec3S32[] m, DrawArgs dArgs) {
            Bitmap bmp = HeightmapGen.ReadBitmap(dArgs.name, "extra/images/", p);
            if (bmp == null) return;
            try {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            } catch (Exception ex) {
                Logger.LogError(ex);
                bmp.Dispose();
                return;
            }

            ImagePrintDrawOp op = new ImagePrintDrawOp();
            int dir;
            if (Math.Abs(m[1].X - m[0].X) > Math.Abs(m[1].Z - m[0].Z)) {
                dir = m[1].X <= m[0].X ? 1 : 0;
            } else {
                dir = m[1].Z <= m[0].Z ? 3 : 2;
            }
            op.LayerMode = dArgs.layer; op.DualLayer = dArgs.dualLayered;
            op.CalcState(dir);
            
            ResizeImage(p, m, op, ref bmp);
            op.SetLevel(p.level);
            op.Player = p; op.Source = bmp;
            
            op.Palette = dArgs.palette; op.Filename = dArgs.name;
            DrawOpPerformer.Do(op, null, p, m, false);
        }
        
        void ResizeImage(Player p, Vec3S32[] m, ImagePrintDrawOp op, ref Bitmap bmp) {
            Level lvl = p.level;
            Vec3S32 xEnd = m[0] + op.dx * (bmp.Width  - 1);
            Vec3S32 yEnd = m[0] + op.dy * (bmp.Height - 1);
            if (lvl.IsValidPos(xEnd.X, xEnd.Y, xEnd.Z) && lvl.IsValidPos(yEnd.X, yEnd.Y, yEnd.Z)) return;
            
            int resizedWidth  = bmp.Width - LargestDelta(lvl, xEnd);
            int resizedHeight = bmp.Height - LargestDelta(lvl, yEnd);
            // Preserve aspect ratio of image
            float ratioX = resizedWidth / (float)bmp.Width, ratioY = resizedHeight / (float)bmp.Height;
            float ratio = Math.Min(ratioX, ratioY);
            resizedWidth = (int)(bmp.Width * ratio); resizedHeight = (int)(bmp.Height * ratio);
            
            Player.Message(p, "&cImage is too large ({0}x{1}), resizing to ({2}x{3})",
                           bmp.Width, bmp.Height, resizedWidth, resizedHeight);
            
            Bitmap resized = new Bitmap(resizedWidth, resizedHeight);
            using (Graphics g = Graphics.FromImage(resized)) {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.DrawImage(bmp, 0, 0, resizedWidth, resizedHeight);
            }
            bmp.Dispose();
            bmp = resized;
        }
        
        static int LargestDelta(Level lvl, Vec3S32 point) {
            Vec3S32 clamped;
            clamped.X = Math.Max(0, Math.Min(point.X, lvl.Width - 1));
            clamped.Y = Math.Max(0, Math.Min(point.Y, lvl.Height - 1));
            clamped.Z = Math.Max(0, Math.Min(point.Z, lvl.Length - 1));
            
            int dx = Math.Abs(point.X - clamped.X);
            int dy = Math.Abs(point.Y - clamped.Y);
            int dz = Math.Abs(point.Z - clamped.Z);
            return Math.Max(dx, Math.Max(dy, dz));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ImagePrint [file] [palette] <mode> %H- Prints image in extra/images/ folder. (File must have .bmp extension)");
            Player.Message(p, "%T/ImagePrint [url] [palette] <mode> %H- Downloads then prints the given image. (transparency supported)");
            Player.Message(p, "%HPalettes: &f{0}", ImagePalette.Palettes.Join(pal => pal.Name));
            Player.Message(p, "%HModes: &fVertical, Vertical2Layer, Horizontal");
        }

        class DrawArgs { public bool layer, dualLayered; public ImagePalette palette; public string name; }
    }
}

