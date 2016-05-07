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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using MCGalaxy.Drawing;

namespace MCGalaxy.Commands
{
    public sealed class CmdImageprint : Command
    {
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
            CatchPos cpos = default(CatchPos);
            cpos.layer = layer;
            cpos.bitmapLoc = bitmapLoc;
            cpos.popType = popType;
            p.blockchangeObject = cpos;
            Player.Message(p, "Place two blocks to determine direction.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        bool DownloadWebFile(string url, Player p) {
            if (!(url.StartsWith("http://") || url.StartsWith("https://"))) {
                url = "http://" + url;
            }
            
            try {
                using (WebClient web = new WebClient()) {
                    Player.Message(p, "Downloading file from: &f" + url);
                    web.DownloadFile(url, "extra/images/tempImage_" + p.name + ".bmp");
                }
                Player.Message(p, "Finished downloading image.");
                return true;
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Player.Message(p, "&cFailed to download the image from the given url.");
                Player.Message(p, "&cThe url may need to end with its extension (such as .jpg).");
                return false;
            }
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            if (x == cpos.x && z == cpos.z) { Player.Message(p, "No direction was selected"); return; }

            int direction;
            if (Math.Abs(cpos.x - x) > Math.Abs(cpos.z - z))
                direction = x <= cpos.x ? 1 : 0;
            else
                direction = z <= cpos.z ? 3 : 2;
            
            Thread thread = new Thread(() => DoDrawImage(p, cpos, direction));
            thread.Name = "MCG_ImagePrint";
            thread.Start();
        }
        
        void DoDrawImage(Player p, CatchPos cpos, int direction) {
            Bitmap bmp = null;
            try {
                bmp = new Bitmap("extra/images/" + cpos.bitmapLoc + ".bmp");
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                if (bmp != null) bmp.Dispose();
                Player.Message(p, "&cThere was an error reading the downloaded image.");
                Player.Message(p, "&cThe url may need to end with its extension (such as .jpg).");
                return;
            }
            
            byte popType = cpos.popType;
            bool layer = cpos.layer;
            if (layer) {
                if (popType == 1) popType = 2;
                if (popType == 3) popType = 4;
            }
            ColorBlock[] palette = ImagePalette.GetPalette(popType);
            ColorBlock cur = default(ColorBlock);
            Vec3U16 P;
            
            IPalette selector = null;
            if (popType == 6) selector = new GrayscalePalette();
            else selector = new RGBPalette();
            selector.SetAvailableBlocks(palette);

            for (int yy = 0; yy < bmp.Height; yy++)
                for (int xx = 0; xx < bmp.Width; xx++)
            {
                if (layer) {
                    P.Y = cpos.y;
                    if (direction <= 1) {
                        if (direction == 0) { P.X = (ushort)(cpos.x + xx); P.Z = (ushort)(cpos.z - yy); }
                        else { P.X = (ushort)(cpos.x - xx); P.Z = (ushort)(cpos.z + yy); }
                    } else {
                        if (direction == 2) { P.Z = (ushort)(cpos.z + xx); P.X = (ushort)(cpos.x + yy); }
                        else { P.Z = (ushort)(cpos.z - xx); P.X = (ushort)(cpos.x - yy); }
                    }
                } else {
                    P.Y = (ushort)(cpos.y + yy);
                    if (direction <= 1) {
                        if (direction == 0) P.X = (ushort)(cpos.x + xx);
                        else P.X = (ushort)(cpos.x - xx);
                        P.Z = cpos.z;
                    } else {
                        if (direction == 2) P.Z = (ushort)(cpos.z + xx);
                        else P.Z = (ushort)(cpos.z - xx);
                        P.X = cpos.x;
                    }
                }

                Color col = bmp.GetPixel(xx, yy);
                cur.r = col.R; cur.g = col.G; cur.b = col.B; cur.a = col.A;
                int position;
                cur.type = selector.BestMatch(cur, out position);
                if (popType == 1 || popType == 3) {
                    int threshold = popType == 1 ? 20 : 3;
                    if (position <= threshold) {
                        if (direction == 0)
                            P.Z = (ushort)(P.Z + 1);
                        else if (direction == 2)
                            P.X = (ushort)(P.X - 1);
                        else if (direction == 1)
                            P.Z = (ushort)(P.Z - 1);
                        else if (direction == 3)
                            P.X = (ushort)(P.X + 1);
                    }
                }

                if (cur.a < 20) cur.type = Block.air;
                p.level.UpdateBlock(p, P.X, P.Y, P.Z, cur.type, 0);
            }
            
            if (cpos.bitmapLoc == "tempImage_" + p.name)
                File.Delete("extra/images/tempImage_" + p.name + ".bmp");
            Player.Message(p, "Finished printing image using " + ImagePalette.Names[popType]);
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

        struct CatchPos { public bool layer; public byte popType; public string bitmapLoc; public ushort x, y, z; }
    }
}

