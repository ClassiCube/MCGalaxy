/*
	Copyright 2011 MCGalaxy
	
	Written by Frederik Gelder
		
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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdpCinema : Command
    {
        CmdpCinema2[] cmdPC = new CmdpCinema2[100]; //reserving space for 100 movies.
        bool[] used = new bool[100];

        public override string name { get { return "pcinema"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override void Use(Player p, string message)
        {
            String[] tempmsg = message.Split(' ');
            String send = "";
            int movnum = 0;
            if (tempmsg.Length < 2 || tempmsg.Length > 3)
            {
                Help(p);
                return;
            }

            if (tempmsg.Length == 2)
            {
                if (tempmsg[0].ToLower() == "abort")
                {
                    try
                    {
                        cmdPC[int.Parse(tempmsg[1])].abort();

                        used[int.Parse(tempmsg[1])] = false;
                    }
                    catch
                    {
                        Help(p);
                        return;
                    }
                    Player.SendMessage(p, "movie " + tempmsg[1] + " was aborted.");
                    return;
                }
                else if (tempmsg[0].ToLower() == "delete")
                {
                    if(System.IO.File.Exists("extra/cin/" + tempmsg[1] + ".cin")){
                        System.IO.File.Delete("extra/cin/" + tempmsg[1] + ".cin");
                    }
                    return;
                }
                //no frametime. use default 1000. but that does pcinema2 for us
                send = tempmsg[1];
            }
            else if (tempmsg.Length == 3)
            {
                //frametime given
                send = tempmsg[1] + " " + tempmsg[2];
            }

            try
            {
                movnum = int.Parse(tempmsg[0]);
            }
            catch
            {
                Help(p);
                return;
            }

            if (used[movnum])
            {
                Player.SendMessage(p, "Movie is already used. stop it by using /pcinema abort [movienumber]");
                return;
            }
            else
            {
                //cmdPC[movnum] = new CmdpCinema2();
                try
                {
                    cmdPC[movnum].Use(p, send);//better not use a new instance. it worked but they were not stopable.
                }
                catch
                {
                    cmdPC[movnum] = new CmdpCinema2();
                    cmdPC[movnum].Use(p, send);
                }
                used[movnum] = true;
            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pCinema [movienumber] [filename] <frametime> - movienumber can be 0-99. filename explains itself. frametime is the time in ms each rame is displayed.no values under 200 accepted.else set to 200. You can delete unwanted movies with /pcinema delete <filename>");
        }
    }

    public sealed class CmdpCinema2 : Command
    {
        String FilePath;
        int frameLonging;
        String temp = "";
        CFrame[] Frames;
        int FrameCount = 0;
        int Framenow = 0;
        System.Timers.Timer lool;
        Player MEEE;
        public bool running;
        Level plLevel;


        public override string name { get { return "pcinema2"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public void abort()
        {
            FilePath = "";
            Frames = null;
            Framenow = 0;
            lool.Enabled = false;//even i dispose it,i disable it because i dont know if it really stops
            lool.Dispose();
            running = false;
        }

        public override void Use(Player p, string message)
        {
            if (message.Length == 0)
            {
                Help(p); return;
            }
            String[] tempa = message.Split(' ');
            if (tempa.Length > 2)
            {
                Help(p); return;
            }
            else if (tempa.Length == 1)
            {
                if (tempa[0].ToLower() == "abort")
                {
                    //player wants to abort the running action
                    if (!running)
                    {
                        Player.SendMessage(p, "There is no movie running");
                        return;
                    }
                    //abortion
                    abort();
                    return;
                }
                else if (running)
                {
                    Player.SendMessage(p, "Movie is already running.Abort first");
                    return;
                }
                //frametime not given << default
                frameLonging = 1000;
            }
            else
            {
                frameLonging = int.Parse(tempa[1]);
                if (frameLonging < 200) frameLonging = 200;
            }
            FilePath = "extra/cin/" + tempa[0] + ".cin";
            if (!File.Exists(FilePath))
            {
                Player.SendMessage(p, "File does not exist");
                return;
            }
            MEEE = p;
            //temp[0] is the name of the file and temp[1] in ms is the time each frame is displayed
            Player.SendMessage(p, "Place a Block to determine the position");
            p.ClearBlockchange();
            //happens when block is changed. then call blockchange1
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);

        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            //com(p, "get the type of the changed block");
            byte b = p.level.GetTile(x, y, z);
            //com(p, "undo the change2");
            p.SendBlockchange(x, y, z, b);
            //now we have to load the stuff from the temp string
            Player.SendMessage(p, "Reading file, please wait...");
            temp = readFile(FilePath);
            Player.SendMessage(p, "File read");
            String[] textFrames = temp.Split('[');
            //[0] is the framecount. rest is in this format: FrameXXXXX]{0,0,0,0|0,0,0,0|}
            FrameCount = int.Parse(textFrames[0]);
            Frames = new CFrame[FrameCount];
            plLevel = p.level;
            for (int i = 0; i < FrameCount; i++)
            {
                Player.SendMessage(p, String.Format("Frame {0:00000}...", i + 1));
                textFrames[i + 1] = textFrames[i + 1].Replace(String.Format("Frame{0:00000}]", i + 1), "");
                textFrames[i + 1] = textFrames[i + 1].Replace("{", "");
                textFrames[i + 1] = textFrames[i + 1].Replace("}", "");
                String[] Coords = textFrames[i + 1].Split('|');
                Frames[i].BlockCollection = new List<Blok>();
                //coords now in array with format: 0,0,0,0
                //now split by , and write in XYZ,type
                for (int j = 0; j < Coords.Length - 1; j++)
                {
                    String[] tempXYZ = Coords[j].Split(';');
                    Frames[i].BlockCollection.Add(new Blok((ushort)(x + int.Parse(tempXYZ[0])),
                        (ushort)(y + int.Parse(tempXYZ[1])),
                        (ushort)(z + int.Parse(tempXYZ[2])),
                        (Byte)int.Parse(tempXYZ[3])));
                    //frame has now its blocks.
                }
                Player.SendMessage(p, "Done!");
            }
            //frames are all aquired
            Player.SendMessage(p, "Frames ready! start Playing");

            //better using timers.timer. forms timer doesnt work sometimes for fcking reasons o.O
            lool = new System.Timers.Timer(frameLonging);
            lool.Elapsed += new System.Timers.ElapsedEventHandler(nextFrameUpdate);
            lool.Enabled = true;
            running = true;
        }

        void nextFrameUpdate(object sender, EventArgs e)
        {
            //update the frames
            foreach (Blok Bl in Frames[Framenow].BlockCollection)
            {
                Bl.applyBlockToMap(MEEE, plLevel);
            }
            //blaahhh write blocks to map
            Framenow++;
            if (Framenow >= FrameCount)
            {
                Framenow = 0;//begin from start again
            }

        }

        String readFile(String Path)
        {
            if (File.Exists(Path))
            {
                return File.ReadAllText(Path);
            }
            else
            {
                return null;
            }
        }

        struct CFrame
        {
            public List<Blok> BlockCollection;
        }

        struct Blok
        {
            public ushort X, Y, Z;
            public byte Type;
            public Blok(ushort x, ushort y, ushort z, byte typ)
            {
                X = x;
                Y = y;
                Z = z;
                Type = typ;
            }
            public void applyBlockToMap(Player p, Level l)
            {
                l.Blockchange(p, X, Y, Z, Type);
                //p.SendBlockchange(X, Y, Z, Type);
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pcinema2 should not be used directly. better use pcinema.");
        }
    }
}