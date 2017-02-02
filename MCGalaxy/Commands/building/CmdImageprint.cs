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
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdImageprint : Command {
        public override string name { get { return "imageprint"; } }
        public override string shortcut { get { return "img"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new[] {  new CommandAlias("imgprint"), new CommandAlias("printimg"),
                    new CommandAlias("imgdraw"), new CommandAlias("drawimg"),
                    new CommandAlias("drawimage"), new CommandAlias("printimage") }; }
        }
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

            Thread thread = new Thread(() => DoDrawImage(p, m, (DrawArgs)state));
            thread.Name = "MCG_ImagePrint";
            thread.Start();
            return false;
        }
        
        void DoDrawImage(Player p, Vec3S32[] m, DrawArgs dArgs) {
            try {
                DoDrawImageCore(p, m, dArgs);
            } catch (Exception ex) {
                Server.ErrorLog(ex); // Do not want it taking down the whole server if error occurs
            }
        }
        
        void DoDrawImageCore(Player p, Vec3S32[] m, DrawArgs dArgs) {
            Bitmap bmp = HeightmapGen.ReadBitmap(dArgs.name, "extra/images/", p);
            if (bmp == null) return;
            try {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                bmp.Dispose();
                return;
            }

            ImagePrintDrawOp op = new ImagePrintDrawOp();
            if (Math.Abs(m[1].X - m[0].X) > Math.Abs(m[1].Z - m[0].Z)) {
                op.Direction = m[1].X <= m[0].X ? 1 : 0;
            } else {
                op.Direction = m[1].Z <= m[0].Z ? 3 : 2;
            }
            
            op.SetLevel(p.level);
            op.Player = p;
            op.Source = bmp;
            op.Layer = dArgs.layer;
            op.Mode = dArgs.popType;
            op.Filename = dArgs.name;
            
            if (op.Layer) {
                if (op.Mode == 1) op.Mode = 2;
                if (op.Mode == 3) op.Mode = 4;
            }
            DrawOpPerformer.Do(op, null, p, m, false);
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

