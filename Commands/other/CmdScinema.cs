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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdSCinema : Command
    {
        StreamWriter cin;
        String Filepath = "";

        public override string name { get { return "scinema"; } }
        public override string shortcut { get { return "sc"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override void Use(Player p, string message)
        {

            if (message.Length == 0)
            {
                //no message
                Help(p); return;
            }
            else
            {
                //message found. propably filename
                Filepath = "extra/cin/" + message + ".cin";
                if (!File.Exists(Filepath))
                {
                    if (!Directory.Exists("extra/cin/"))
                    {
                        Directory.CreateDirectory("extra/cin/");
                    }
                    //File has to be created then append
                    FileStream damn = File.Create(Filepath);
                    damn.Close();
                    damn.Dispose();
                    StreamWriter temp = File.AppendText(Filepath);
                    temp.WriteLine(String.Format("{0:00000}", 0)); //number of last frame Frame. in this case 0
                    temp.Flush();
                    temp.Close();
                    temp.Dispose();
                }
                //just append
                //have to add this otherwise will crash
                CatchPos cpos;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
                Player.SendMessage(p, "Place two blocks to determine the edges.");
                p.ClearBlockchange();
                //happens when block is changed. then call blockchange1
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
            }
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            //com(p, "get the type of the changed block");
            byte b = p.level.GetTile(x, y, z);
            //com(p, "undo the change2");
            p.SendBlockchange(x, y, z, b);
            //com(p, "blockundone making Catchpos bp");
            CatchPos bp = (CatchPos)p.blockchangeObject;
            //com(p, "copy the coordinates");
            p.copystart[0] = x;
            p.copystart[1] = y;
            p.copystart[2] = z;
            //com(p, "saving the coordinates");
            com(p, x + "," + y + "," + z);
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            //com(p, "wait for next blockchange");
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        void com(Player p, String lol)
        {
            Player.SendMessage(p, lol);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            //com(p, "get the type of the changed block");
            byte b = p.level.GetTile(x, y, z);
            //com(p, "undo the change");
            p.SendBlockchange(x, y, z, b);
            //getting the startpos of copy stored in blockchangeobject
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            List<Player.CopyPos> CBuffer = new List<Player.CopyPos>();

            CBuffer.Clear();
            //com(p, "copy stuff");
            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
            {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                {
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        b = p.level.GetTile(xx, yy, zz);
                        BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), b, CBuffer);
                    }
                }
            }
            //com(p, "stuff is copied. now append to file");
            //com(p, "get the number of next frame");
            int FrameNumber = 0;
            Byte[] ba = new Byte[5];
			using (FileStream ReadStream = File.OpenRead(Filepath))
			{
				String temp = "";
				for (int j = 0; j < 5; j++)
				{
					temp += (Char)ReadStream.ReadByte();
				}
				FrameNumber = int.Parse(temp);
				//framecount aquired(hopefully)
				//now we have to add 1 to that and write it back in the file
				FrameNumber++;
				int Fnum = FrameNumber;
				for (int i = 4; i >= 0; i--)
				{
					ba[i] = Byte.Parse((Fnum % 10).ToString());
					ba[i] += 48;
					//ba[i] = (Byte)49;
					Fnum /= 10;
				}
			}

			using (FileStream WriteStream = File.OpenWrite(Filepath))
			{
				WriteStream.Write(ba, 0, 5);
				//written new number in file
			}
			
            cin = File.AppendText(Filepath);
            cin.Write("[Frame" + String.Format("{0:00000}", FrameNumber) + "]{");
            //written frameheader
            foreach (Player.CopyPos CP in CBuffer)
            {
                String tBlock = "";
                tBlock += CP.x + ";";
                tBlock += CP.y + ";";
                tBlock += CP.z + ";";
                //written coordinates in string
                tBlock += CP.type + "|";
                cin.Write(tBlock);
            }
            cin.Write("}" + Environment.NewLine);
            //work done. saved frame in file
            cin.Flush();
            cin.Close();
            cin.Dispose();
            com(p, "Saved Blocks to File");
        }

        // This one controls what happens when you use /help [commandname].
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/sCinema [name] - Saves a given Frame to the File. Can be Played by pCinema");
        }

        void BufferAdd(Player p, ushort x, ushort y, ushort z, byte type, List<Player.CopyPos> Buf)
        {
            Player.CopyPos pos;
            pos.x = x;
            pos.y = y;
            pos.z = z;
            pos.type = type;
            Buf.Add(pos);
        }

        struct CatchPos { public ushort x, y, z; }


    }
}

