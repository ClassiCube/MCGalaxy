/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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

        public override void Use(Player p, string message)
        {
            if (!Directory.Exists("extra/images/")) { Directory.CreateDirectory("extra/images/"); }
            layer = false;
            popType = 1;
            if (message == "") { Help(p); return; }
            if (message.IndexOf(' ') != -1)     //Yay parameters
            {
                string[] parameters = message.Split(' ');

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] == "layer" || parameters[i] == "l") layer = true;
                    else if (parameters[i] == "1" || parameters[i] == "2color") popType = 1;
                    else if (parameters[i] == "2" || parameters[i] == "1color") popType = 2;
                    else if (parameters[i] == "3" || parameters[i] == "2gray") popType = 3;
                    else if (parameters[i] == "4" || parameters[i] == "1gray") popType = 4;
                    else if (parameters[i] == "5" || parameters[i] == "bw") popType = 5;
                    else if (parameters[i] == "6" || parameters[i] == "gray") popType = 6;
                }

                message = parameters[parameters.Length - 1];
            }
            if (message.IndexOf('/') == -1 && message.IndexOf('.') != -1)
            {
                try
                {
					using (WebClient web = new WebClient())
					{
						Player.SendMessage(p, "Downloading IMGUR file from: &fhttp://www.imgur.com/" + message);
						web.DownloadFile("http://www.imgur.com/" + message, "extra/images/tempImage_" + p.name + ".bmp");
					}
                    Player.SendMessage(p, "Download complete.");
                    bitmaplocation = "tempImage_" + p.name;
                    message = bitmaplocation;   
                }
                catch { }
            }
            else if (message.IndexOf('.') != -1)
            {
                try
                {
					using (WebClient web = new WebClient())
					{
						if (message.Substring(0, 4) != "http")
						{
							message = "http://" + message;
						}
						Player.SendMessage(p, "Downloading file from: &f" + message + Server.DefaultColor + ", please wait.");
						web.DownloadFile(message, "extra/images/tempImage_" + p.name + ".bmp");
					}
                    Player.SendMessage(p, "Download complete.");
                    bitmaplocation = "tempImage_" + p.name;
                }
                catch { }
            }
            else
            {
                bitmaplocation = message;
            }

            if (!File.Exists("extra/images/" + bitmaplocation + ".bmp")) { Player.SendMessage(p, "The URL entered was invalid!"); return; }

            CatchPos cpos;

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine direction.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            RevertAndClearState(p, x, y, z);
            Bitmap myBitmap = new Bitmap("extra/images/" + bitmaplocation + ".bmp"); 
            myBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            if (x == cpos.x && z == cpos.z) { Player.SendMessage(p, "No direction was selected"); return; }

            int direction;
            if (Math.Abs(cpos.x - x) > Math.Abs(cpos.z - z))
            {
                direction = 0;
                if (x <= cpos.x)
                {
                    direction = 1;
                }
            }
            else
            {
                direction = 2;
                if (z <= cpos.z)
                {
                    direction = 3;
                }
            }
            if (layer)
            {
                if (popType == 1) popType = 2;
                if (popType == 3) popType = 4;
            }
            List<FindReference.ColorBlock> refCol = FindReference.popRefCol(popType);
            FindReference.ColorBlock colblock;
            p.SendMessage("" + direction);
            Thread printThread = new Thread(new ThreadStart(delegate
            {
                double[] distance = new double[refCol.Count]; // Array of distances between color pulled from image to the referance colors.

                int position; // This is the block selector for when we find which distance is the shortest.

                for (int k = 0; k < myBitmap.Width; k++)
                {

                    for (int i = 0; i < myBitmap.Height; i++)
                    {
                        if (layer)
                        {
                            colblock.y = cpos.y;
                            if (direction <= 1)
                            {
                                if (direction == 0) { colblock.x = (ushort)(cpos.x + k); colblock.z = (ushort)(cpos.z - i); }
                                else { colblock.x = (ushort)(cpos.x - k); colblock.z = (ushort)(cpos.z + i); }
                                //colblock.z = (ushort)(cpos.z - i);
                            }
                            else
                            {
                                if (direction == 2) { colblock.z = (ushort)(cpos.z + k); colblock.x = (ushort)(cpos.x + i); }
                                else { colblock.z = (ushort)(cpos.z - k); colblock.x = (ushort)(cpos.x - i); }
                                //colblock.x = (ushort)(cpos.x - i);
                            }
                        }
                        else
                        {
                            colblock.y = (ushort)(cpos.y + i);
                            if (direction <= 1)
                            {

                                if (direction == 0) colblock.x = (ushort)(cpos.x + k);
                                else colblock.x = (ushort)(cpos.x - k);
                                colblock.z = cpos.z;
                            }
                            else
                            {
                                if (direction == 2) colblock.z = (ushort)(cpos.z + k);
                                else colblock.z = (ushort)(cpos.z - k);
                                colblock.x = cpos.x;
                            }
                        }


                        colblock.r = myBitmap.GetPixel(k, i).R;
                        colblock.g = myBitmap.GetPixel(k, i).G;
                        colblock.b = myBitmap.GetPixel(k, i).B;
                        colblock.a = myBitmap.GetPixel(k, i).A;

                        if (popType == 6)
                        {
                            if ((colblock.r + colblock.g + colblock.b) / 3 < (256 / 4))
                            {
                                colblock.type = Block.obsidian;
                            }
                            else if (((colblock.r + colblock.g + colblock.b) / 3) >= (256 / 4) && ((colblock.r + colblock.g + colblock.b) / 3) < (256 / 4) * 2)
                            {
                                colblock.type = Block.darkgrey;
                            }
                            else if (((colblock.r + colblock.g + colblock.b) / 3) >= (256 / 4) * 2 && ((colblock.r + colblock.g + colblock.b) / 3) < (256 / 4) * 3)
                            {
                                colblock.type = Block.lightgrey;
                            }
                            else
                            {
                                colblock.type = Block.white;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < distance.Length; j++) // Calculate distances between the colors in the image and the set referance colors, and store them.
                            {
                                distance[j] = Math.Sqrt(Math.Pow((colblock.r - refCol[j].r), 2) + Math.Pow((colblock.b - refCol[j].b), 2) + Math.Pow((colblock.g - refCol[j].g), 2));
                            }

                            position = 0;
                            double minimum = distance[0];
                            for (int h = 1; h < distance.Length; h++) // Find the smallest distance in the array of distances.
                            {
                                if (distance[h] < minimum)
                                {
                                    minimum = distance[h];
                                    position = h;
                                }
                            }


                            colblock.type = refCol[position].type; // Set the block we found closest to the image to the block we are placing.

                            if (popType == 1)
                            {
                                if (position <= 20)
                                {
                                    if (direction == 0)
                                    {
                                        colblock.z = (ushort)(colblock.z + 1);
                                    }
                                    else if (direction == 2)
                                    {
                                        colblock.x = (ushort)(colblock.x - 1);
                                    }
                                    else if (direction == 1)
                                    {
                                        colblock.z = (ushort)(colblock.z - 1);
                                    }
                                    else if (direction == 3)
                                    {
                                        colblock.x = (ushort)(colblock.x + 1);
                                    }
                                }
                            }
                            else if (popType == 3)
                            {
                                if (position <= 3)
                                {
                                    if (direction == 0)
                                    {
                                        colblock.z = (ushort)(colblock.z + 1);
                                    }
                                    else if (direction == 2)
                                    {
                                        colblock.x = (ushort)(colblock.x - 1);
                                    }
                                    else if (direction == 1)
                                    {
                                        colblock.z = (ushort)(colblock.z - 1);
                                    }
                                    else if (direction == 3)
                                    {
                                        colblock.x = (ushort)(colblock.x + 1);
                                    }
                                }
                            }
                        }

                        //ALPHA HANDLING (REAL HARD STUFF, YO)
                        if (colblock.a < 20) colblock.type = Block.air;

                        p.level.UpdateBlock(p, colblock.x, colblock.y, colblock.z, colblock.type, 0);
                    }
                }
                if (bitmaplocation == "tempImage_" + p.name) File.Delete("extra/images/tempImage_" + p.name + ".bmp");

                string printType;
                switch (popType)
                {
                    case 1: printType = "2-layer color"; break;
                    case 2: printType = "1-layer color"; break;
                    case 3: printType = "2-layer grayscale"; break;
                    case 4: printType = "1-layer grayscale"; break;
                    case 5: printType = "Black and White"; break;
                    case 6: printType = "Mathematical grayscale"; break;
                    default: printType = "Something unknown"; break;
                }

                Player.SendMessage(p, "Finished printing image using " + printType);
            }));
            printThread.Name = "MCG_ImagePrint";
            printThread.Start();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/imageprint <switch> <localfile> - Print local file in extra/images/ folder.  Must be type .bmp, type filename without extension.");
            Player.SendMessage(p, "/imageprint <switch> <imgurfile.extension> - Print IMGUR stored file.  Example: /i piCCm.gif will print www.imgur.com/piCCm.gif. Case-sensitive");
            Player.SendMessage(p, "/imageprint <switch> <webfile> - Print web file in format domain.com/folder/image.jpg. Does not need http:// or www.");
            Player.SendMessage(p, "Available switches: (&f1" + Server.DefaultColor + ") 2-Layer Color image, (&f2" + Server.DefaultColor + ") 1-Layer Color Image, (&f3" + Server.DefaultColor + ") 2-Layer Grayscale, (&f4" + Server.DefaultColor + ") 1-Layer Grayscale, (%f5" + Server.DefaultColor + ") Black and White, (&f6" + Server.DefaultColor + ") Mathematical Grayscale");
            Player.SendMessage(p, "Local filetypes: .bmp.   Remote Filetypes: .gif .png .jpg .bmp.  PNG and GIF may use transparency");
            Player.SendMessage(p, "Use switch (&flayer" + Server.DefaultColor + ") or (&fl" + Server.DefaultColor + ") to print horizontally.");
        }

        struct CatchPos { public ushort x, y, z; }

        string bitmaplocation;
        bool layer = false;
        byte popType = 1;
    }
}

