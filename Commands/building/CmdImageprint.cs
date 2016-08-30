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
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdImageprint : Command {
        public override string name { get { return "imageprint"; } }
        public override string shortcut { get { return "img"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdImageprint() { }

        public override void Use(Player p, string message) {
            if (!Directory.Exists("extra/images/"))
                Directory.CreateDirectory("extra/images/");
            bool layer = false;
            byte popType = 1;
            string bitmapLoc = null;
            if (message == "") { Help(p); return; }
            
            if (message.IndexOf(' ') != -1) {
                string[] args = message.Split(' ');
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "layer" || args[i] == "l") layer = true;
                    else if (args[i] == "1" || args[i] == "2color") popType = 1;
                    else if (args[i] == "2" || args[i] == "1color") popType = 2;
                    else if (args[i] == "3" || args[i] == "2gray") popType = 3;
                    else if (args[i] == "4" || args[i] == "1gray") popType = 4;
                    else if (args[i] == "5" || args[i] == "bw") popType = 5;
                    else if (args[i] == "6" || args[i] == "gray") popType = 6;
                }
                message = args[args.Length - 1];
            }
            
            if (message.IndexOf('/') == -1 && message.IndexOf('.') != -1) {
                if (!DownloadWebFile("http://www.imgur.com/" + message, p)) return;
                bitmapLoc = "tempImage_" + p.name;
            } else if (message.IndexOf('.') != -1) {
                if (!DownloadWebFile(message, p)) return;
                bitmapLoc = "tempImage_" + p.name;
            } else {
                bitmapLoc = message;
            }

            if (!File.Exists("extra/images/" + bitmapLoc + ".bmp")) {
                Player.Message(p, "The URL entered was invalid!"); return;
            }
            
            DrawArgs dArgs = default(DrawArgs);
            dArgs.layer = layer;
            dArgs.name = bitmapLoc;
            dArgs.popType = popType;
            Player.Message(p, "Place two blocks to determine direction.");
            p.MakeSelection(2, dArgs, DoImage);
        }
        
        bool DownloadWebFile(string url, Player p) {
            return HeightmapGen.DownloadImage(url, "extra/images/", p);
        }
        
        bool DoImage(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            if (m[0].X == m[1].X && m[0].Z == m[1].Z) { Player.Message(p, "No direction was selected"); return false; }

            int dir;
            if (Math.Abs(m[1].X - m[0].X) > Math.Abs(m[1].Z - m[0].Z))
                dir = m[1].X <= m[0].X ? 1 : 0;
            else
                dir = m[1].Z <= m[0].Z ? 3 : 2;
            
            Thread thread = new Thread(() => DoDrawImage(p, m[0], (DrawArgs)state, dir));
            thread.Name = "MCG_ImagePrint";
            thread.Start();
            return false;
        }
        
        void DoDrawImage(Player p, Vec3S32 p0, DrawArgs dArgs, int direction) {
            Bitmap bmp = HeightmapGen.ReadBitmap(dArgs.name, "extra/images/", p);
            if (bmp == null) return;
            try {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                bmp.Dispose();
                return;
            }
            
            byte popType = dArgs.popType;
            bool layer = dArgs.layer;
            if (layer) {
                if (popType == 1) popType = 2;
                if (popType == 3) popType = 4;
            }
            PaletteEntry[] palette = ImagePalette.GetPalette(popType);
            PaletteEntry cur = default(PaletteEntry);
            
            IPalette selector = null;
            if (popType == 6) selector = new GrayscalePalette();
            else selector = new RgbPalette();
            selector.SetAvailableBlocks(palette);
            
            Vec3S32 dx, dy, adj;
            CalcMul(layer, direction, out dx, out dy, out adj);

            for (int yy = 0; yy < bmp.Height; yy++)
                for (int xx = 0; xx < bmp.Width; xx++)
            {
                ushort X = (ushort)(p0.X + dx.X * xx + dy.X * yy);
                ushort Y = (ushort)(p0.Y + dx.Y * xx + dy.Y * yy);
                ushort Z = (ushort)(p0.Z + dx.Z * xx + dy.Z * yy);

                Color col = bmp.GetPixel(xx, yy);
                cur.R = col.R; cur.G = col.G; cur.B = col.B;
                int position;
                cur.Block = selector.BestMatch(cur, out position);
                if (popType == 1 || popType == 3) {
                    int threshold = popType == 1 ? 20 : 3;
                    // Back layer block
                    if (position <= threshold) {
                        X = (ushort)(X + adj.X);
                        Z = (ushort)(Z + adj.Z);
                    }
                }

                if (col.A < 20) cur.Block = Block.air;
                p.level.UpdateBlock(p, X, Y, Z, cur.Block, 0, true);
            }
            
            if (dArgs.name == "tempImage_" + p.name)
                File.Delete("extra/images/tempImage_" + p.name + ".bmp");
            Player.Message(p, "Finished printing image using " + ImagePalette.Names[popType]);
        }
        
        void CalcMul(bool layer, int dir,
                     out Vec3S32 signX, out Vec3S32 signY, out Vec3S32 adj) {
            signX = default(Vec3S32); signY = default(Vec3S32); adj = default(Vec3S32);
            
            // Calculate back layer offset
            if (dir == 0) adj.Z = 1;
            if (dir == 1) adj.Z = -1;
            if (dir == 2) adj.X = -1;
            if (dir == 3) adj.X = 1;
            
            if (layer) {
                if (dir == 0) { signX.X = 1; signY.Z = -1; }
                if (dir == 1) { signX.X = -1; signY.Z = 1; }
                if (dir == 2) { signX.Z = 1; signY.X = 1; }
                if (dir == 3) { signX.Z = -1; signY.X = -1; }
            } else {
                signY.Y = 1; // Oriented upwards
                if (dir == 0) signX.X = 1;
                if (dir == 1) signX.X = -1;
                if (dir == 2) signX.Z = 1;
                if (dir == 3) signX.Z = -1;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/imageprint <switch> <localfile> - Print local file in extra/images/ folder.  Must be type .bmp, type filename without extension.");
            Player.Message(p, "/imageprint <switch> <imgurfile.extension> - Print IMGUR stored file.  Example: /i piCCm.gif will print www.imgur.com/piCCm.gif. Case-sensitive");
            Player.Message(p, "/imageprint <switch> <webfile> - Print web file in format domain.com/folder/image.jpg. Does not need http:// or www.");
            Player.Message(p, "Available switches: (&f1%S) 2-Layer Color image, (&f2%S) 1-Layer Color Image, " +
                           "(&f3%S) 2-Layer Grayscale, (&f4%S) 1-Layer Grayscale, (%f5%S) Black and White, (&f6%S) Mathematical Grayscale");
            Player.Message(p, "Local filetypes: .bmp.   Remote Filetypes: .gif .png .jpg .bmp.  PNG and GIF may use transparency");
            Player.Message(p, "Use switch (&flayer%S) or (&fl%S) to print horizontally.");
        }

        struct DrawArgs { public bool layer; public byte popType; public string name; }
    }
}

