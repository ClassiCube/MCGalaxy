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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdImageprint : Command {
        public override string name { get { return "imageprint"; } }
        public override string shortcut { get { return "img"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("imgprint"), new CommandAlias("printimg"),
                    new CommandAlias("imgdraw"), new CommandAlias("drawimg"),
                    new CommandAlias("drawimage"), new CommandAlias("printimage") }; }
        }

        public override void Use(Player p, string message) {
            if (!Directory.Exists("extra/images/"))
                Directory.CreateDirectory("extra/images/");
            if (message == "") { Help(p); return; }
            string[] parts = message.SplitSpaces(3);
            
            DrawArgs dArgs = default(DrawArgs);
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
                    Player.Message(p, "Use %T/palette %Sto add entries to it"); return;
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
            p.MakeSelection(2, dArgs, DoImage);
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
            op.Player = p; op.Source = bmp;
            op.LayerMode = dArgs.layer; op.DualLayer = dArgs.dualLayered;
            op.Palette = dArgs.palette; op.Filename = dArgs.name;
            DrawOpPerformer.Do(op, null, p, m, false);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/imageprint [file] [palette] <mode> %H- Prints image in extra/images/ folder. (File must have .bmp extension)");
            Player.Message(p, "%T/imageprint [url] [palette] <mode> %H- Downloads then prints the given image. (transparency supported)");
            Player.Message(p, "%HPalettes: &f{0}", ImagePalette.Palettes.Join(pal => pal.Name));
            Player.Message(p, "%HModes: &fVertical, Vertical2Layer, Horizontal");
        }

        struct DrawArgs { public bool layer, dualLayered; public ImagePalette palette; public string name; }
    }
}

