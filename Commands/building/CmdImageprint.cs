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
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        Player.SendMessage(p, "Downloading IMGUR file from: &fhttp://www.imgur.com/" + message);
                        web.DownloadFile("http://www.imgur.com/" + message, "extra/images/tempImage_" + p.name + ".bmp");
                    }
                    Player.SendMessage(p, "Download complete.");
                    bitmapLoc = "tempImage_" + p.name;
                }
                catch { }
            } else if (message.IndexOf('.') != -1) {
                try
                {
                    using (WebClient web = new WebClient())
                    {
                        if (message.Substring(0, 4) != "http")
                        {
                            message = "http://" + message;
                        }
                        Player.SendMessage(p, "Downloading file from: &f" + message + "%S, please wait.");
                        web.DownloadFile(message, "extra/images/tempImage_" + p.name + ".bmp");
                    }
                    Player.SendMessage(p, "Download complete.");
                    bitmapLoc = "tempImage_" + p.name;
                }
                catch { }
            } else {
                bitmapLoc = message;
            }

            if (!File.Exists("extra/images/" + bitmapLoc + ".bmp")) {
                Player.SendMessage(p, "The URL entered was invalid!"); return;
            }
            CatchPos cpos = default(CatchPos);
            cpos.layer = layer;
            cpos.bitmapLoc = bitmapLoc;
            cpos.popType = popType;
            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine direction.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
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
            if (x == cpos.x && z == cpos.z) { Player.SendMessage(p, "No direction was selected"); return; }

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
            Bitmap bmp = new Bitmap("extra/images/" + cpos.bitmapLoc + ".bmp");
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            byte popType = cpos.popType;
            bool layer = cpos.layer;
            if (layer) {
                if (popType == 1) popType = 2;
                if (popType == 3) popType = 4;
            }
            ImagePalette.ColorBlock[] palette = ImagePalette.GetPalette(popType);
            ImagePalette.ColorBlock cur;

            for (int yy = 0; yy < bmp.Height; yy++)
                for (int xx = 0; xx < bmp.Width; xx++)
            {
                if (layer) {
                    cur.y = cpos.y;
                    if (direction <= 1) {
                        if (direction == 0) { cur.x = (ushort)(cpos.x + xx); cur.z = (ushort)(cpos.z - yy); }
                        else { cur.x = (ushort)(cpos.x - xx); cur.z = (ushort)(cpos.z + yy); }
                    } else {
                        if (direction == 2) { cur.z = (ushort)(cpos.z + xx); cur.x = (ushort)(cpos.x + yy); }
                        else { cur.z = (ushort)(cpos.z - xx); cur.x = (ushort)(cpos.x - yy); }
                    }
                } else {
                    cur.y = (ushort)(cpos.y + yy);
                    if (direction <= 1) {
                        if (direction == 0) cur.x = (ushort)(cpos.x + xx);
                        else cur.x = (ushort)(cpos.x - xx);
                        cur.z = cpos.z;
                    } else {
                        if (direction == 2) cur.z = (ushort)(cpos.z + xx);
                        else cur.z = (ushort)(cpos.z - xx);
                        cur.x = cpos.x;
                    }
                }

                Color col = bmp.GetPixel(xx, yy);
                cur.r = col.R; cur.g = col.G; cur.b = col.B; cur.a = col.A;
                if (popType == 6) {
                    int brightness = (cur.r + cur.g + cur.b) / 3;
                    if (brightness < (256 / 4))
                        cur.type = Block.obsidian;
                    else if (brightness >= (256 / 4) && brightness < (256 / 4) * 2)
                        cur.type = Block.darkgrey;
                    else if (brightness >= (256 / 4) * 2 && brightness < (256 / 4) * 3)
                        cur.type = Block.lightgrey;
                    else
                        cur.type = Block.white;
                } else {
                    int minimum = int.MaxValue, position = 0;
                    for (int i = 0; i < palette.Length; i++) {
                        ImagePalette.ColorBlock pixel = palette[i];
                        int dist = (cur.r - pixel.r) * (cur.r - pixel.r) 
                            + (cur.g - pixel.g) * (cur.g - pixel.g)
                            + (cur.b - pixel.b) * (cur.b - pixel.b);
                        
                        if (dist < minimum) {
                            minimum = dist; position = i;
                        }
                    }

                    cur.type = palette[position].type;
                    if (popType == 1 || popType == 3) {
                        int threshold = popType == 1 ? 20 : 3;
                        if (position <= threshold) {
                            if (direction == 0)
                                cur.z = (ushort)(cur.z + 1);
                            else if (direction == 2)
                                cur.x = (ushort)(cur.x - 1);
                            else if (direction == 1)
                                cur.z = (ushort)(cur.z - 1);
                            else if (direction == 3)
                                cur.x = (ushort)(cur.x + 1);
                        }
                    }
                }

                if (cur.a < 20) cur.type = Block.air;
                p.level.UpdateBlock(p, cur.x, cur.y, cur.z, cur.type, 0);
            }
            
            if (cpos.bitmapLoc == "tempImage_" + p.name)
                File.Delete("extra/images/tempImage_" + p.name + ".bmp");
            Player.SendMessage(p, "Finished printing image using " + ImagePalette.Names[popType]);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/imageprint <switch> <localfile> - Print local file in extra/images/ folder.  Must be type .bmp, type filename without extension.");
            Player.SendMessage(p, "/imageprint <switch> <imgurfile.extension> - Print IMGUR stored file.  Example: /i piCCm.gif will print www.imgur.com/piCCm.gif. Case-sensitive");
            Player.SendMessage(p, "/imageprint <switch> <webfile> - Print web file in format domain.com/folder/image.jpg. Does not need http:// or www.");
            Player.SendMessage(p, "Available switches: (&f1%S) 2-Layer Color image, (&f2%S) 1-Layer Color Image, " +
                               "(&f3%S) 2-Layer Grayscale, (&f4%S) 1-Layer Grayscale, (%f5%S) Black and White, (&f6%S) Mathematical Grayscale");
            Player.SendMessage(p, "Local filetypes: .bmp.   Remote Filetypes: .gif .png .jpg .bmp.  PNG and GIF may use transparency");
            Player.SendMessage(p, "Use switch (&flayer%S) or (&fl%S) to print horizontally.");
        }

        struct CatchPos { public bool layer; public byte popType; public string bitmapLoc; public ushort x, y, z; }
    }
}

