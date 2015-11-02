/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
    public sealed class CmdCopy : Command
    {
        public override string name { get { return "copy"; } }
        public override string shortcut { get { return "c"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public int allowoffset = 0;
        public CmdCopy() { }

        public override void Use(Player p, string message)
        {
            if (message.Split(' ')[0].ToLower() == "save")
            {
                if (message.Split(' ').Length != 2 || String.IsNullOrEmpty(message.Split(' ')[1])) { Help(p); return; }
                Savecopy(p, message.Split(' ')[1]); return;
            }
            if (message.Split(' ')[0].ToLower() == "load")
            {
                if (message.Split(' ').Length != 2 || String.IsNullOrEmpty(message.Split(' ')[1])) { Help(p); return; }
                Loadcopy(p, message.Split(' ')[1]); return;
            }
            if (message.Split(' ')[0].ToLower() == "delete")
            {
                if (message.Split(' ').Length != 2 || String.IsNullOrEmpty(message.Split(' ')[1])) { Help(p); return; }
                message = message.Split(' ')[1];
                if (!File.Exists("extra/savecopy/" + p.name + "/" + message + ".cpy")) { Player.SendMessage(p, "No such copy exists"); return; }
                File.Delete("extra/savecopy/" + p.name + "/" + message + ".cpy");
                Player.SendMessage(p, "Deleted copy " + message); return;
            }
            if (message.ToLower() == "list")
            {
                if (!Directory.Exists("extra/savecopy/" + p.name)) { Player.SendMessage(p, "No such directory exists"); return; }
                FileInfo[] fin = new DirectoryInfo("extra/savecopy/" + p.name).GetFiles();
                for (int i = 0; i < fin.Length; i++)
                {
                    Player.SendMessage(p, fin[i].Name.Replace(".cpy", ""));
                }
                return;
            }
            CatchPos cpos;
            cpos.ignoreTypes = new List<byte>();
            cpos.type = 0;
            p.copyoffset[0] = 0; p.copyoffset[1] = 0; p.copyoffset[2] = 0;
            allowoffset = (message.IndexOf('@'));
            if (allowoffset != -1) { message = message.Replace("@ ", ""); }
            if (message.ToLower() == "cut") { cpos.type = 1; message = ""; }
            else if (message.ToLower() == "air") { cpos.type = 2; message = ""; }
            else if (message == "@") { message = ""; }
            else if (message.IndexOf(' ') != -1)
            {
                if (message.Split(' ')[0] == "ignore")
                {
                    foreach (string s in message.Substring(message.IndexOf(' ') + 1).Split(' '))
                    {
                        if (Block.Byte(s) != Block.Zero)
                        {
                            cpos.ignoreTypes.Add(Block.Byte(s));
                            Player.SendMessage(p, "Ignoring &b" + s);
                        }
                    }
                }
                else
                {
                    Help(p); return;
                }
                message = "";
            }

            cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;

            if (message != "") { Help(p); return; }

            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/copy - Copies the blocks in an area.");
            Player.SendMessage(p, "/copy save <save_name> - Saves what you have copied.");
            Player.SendMessage(p, "/copy load <load_name> - Loads what you have saved.");
            Player.SendMessage(p, "/copy delete <delete_name> - Deletes the specified copy.");
            Player.SendMessage(p, "/copy list - Lists all you have copied.");
            Player.SendMessage(p, "/copy cut - Copies the blocks in an area, then removes them.");
            Player.SendMessage(p, "/copy air - Copies the blocks in an area, including air.");
            Player.SendMessage(p, "/copy ignore <block1> <block2>.. - Ignores <blocks> when copying");
            Player.SendMessage(p, "/copy @ - @ toggle for all the above, gives you a third click after copying that determines where to paste from");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            p.copystart[0] = x;
            p.copystart[1] = y;
            p.copystart[2] = z;

            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;



            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }

        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            p.CopyBuffer.Clear();
            int TotalAir = 0;
            if (cpos.type == 2) p.copyAir = true; else p.copyAir = false;

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        b = p.level.GetTile(xx, yy, zz);
                        if (Block.canPlace(p, b))
                        {
                            if (b == Block.air && cpos.type != 2 || cpos.ignoreTypes.Contains(b)) TotalAir++;

                            if (cpos.ignoreTypes.Contains(b)) BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), Block.air);
                            else BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), b);
                        }
                        else BufferAdd(p, (ushort)(xx - cpos.x), (ushort)(yy - cpos.y), (ushort)(zz - cpos.z), Block.air);
                    }

            if ((p.CopyBuffer.Count - TotalAir) > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to copy " + p.CopyBuffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot copy more than " + p.group.maxBlocks + ".");
                p.CopyBuffer.Clear();
                return;
            }

            if (cpos.type == 1)
                for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
                    for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                        for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                        {
                            b = p.level.GetTile(xx, yy, zz);
                            if (b != Block.air && Block.canPlace(p, b))
                                p.level.Blockchange(p, xx, yy, zz, Block.air);
                        }

            Player.SendMessage(p, (p.CopyBuffer.Count - TotalAir) + " blocks copied.");
            if (allowoffset != -1)
            {
                Player.SendMessage(p, "Place a block to determine where to paste from");
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange3);
            }

        }

        public void Blockchange3(Player p, ushort x, ushort y, ushort z, byte type)
        {

            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;


            p.copyoffset[0] = (p.copystart[0] - x);
            p.copyoffset[1] = (p.copystart[1] - y);
            p.copyoffset[2] = (p.copystart[2] - z);

        }

        void Savecopy(Player p, string message)
        {
            if (message.EndsWith("#"))
            {
                if (!p.group.CanExecute(Command.all.Find("copysavenet")))
                {
                    Player.SendMessage(p, "You are not allowed to save to network locations.");
                    return;
                }
                message = message.Remove(message.Length - 1);
                byte[] cnt = new byte[p.CopyBuffer.Count * 7];
                int k = 0;
                for (int i = 0; i < p.CopyBuffer.Count; i++)
                {
                    BitConverter.GetBytes(p.CopyBuffer[i].x).CopyTo(cnt, 0 + k);
                    BitConverter.GetBytes(p.CopyBuffer[i].y).CopyTo(cnt, 2 + k);
                    BitConverter.GetBytes(p.CopyBuffer[i].z).CopyTo(cnt, 4 + k);
                    cnt[6 + k] = p.CopyBuffer[i].type;  //BitConverter.GetBytes(p.CopyBuffer[i].type).CopyTo(cnt, 6 + k);
                    k = k + 7;
                }
                cnt = cnt.GZip();
                if (!message.StartsWith("http://", true, System.Globalization.CultureInfo.CurrentCulture)) message = "http://" + message;
                try
                {
                    string savefile = "temp" + p.name + new Random().Next(999) + ".cpy";
                    using (FileStream fs = new FileStream(savefile, FileMode.Create))
                    {
                        fs.Write(cnt, 0, cnt.Length);
                    }
                    using (System.Net.WebClient webup = new System.Net.WebClient())
                    {
                        webup.UploadFile(message, savefile);
                        //webup.UploadData(message, cnt);
                    }
                    File.Delete(savefile);
                    Player.SendMessage(p, "Saved copy to " + message + "/" + savefile);
                }
                catch (Exception e) { Player.SendMessage(p, "Failed to upload " + message + e); }
                return;
            }

            if (Player.ValidName(message))
            {
                if (!Directory.Exists("extra/savecopy")) Directory.CreateDirectory("extra/savecopy");
                if (!Directory.Exists("extra/savecopy/" + p.name)) Directory.CreateDirectory("extra/savecopy/" + p.name);
                if (Directory.GetFiles("extra/savecopy/" + p.name).Length > 14) { Player.SendMessage(p, "You can only save 15 copy's. /copy delete some."); return; }
                using (FileStream fs = new FileStream("extra/savecopy/" + p.name + "/" + message + ".cpy", FileMode.Create))
                {
                    byte[] cnt = new byte[p.CopyBuffer.Count * 7];
                    int k = 0;
                    for (int i = 0; i < p.CopyBuffer.Count; i++)
                    {
                        BitConverter.GetBytes(p.CopyBuffer[i].x).CopyTo(cnt, 0 + k);
                        BitConverter.GetBytes(p.CopyBuffer[i].y).CopyTo(cnt, 2 + k);
                        BitConverter.GetBytes(p.CopyBuffer[i].z).CopyTo(cnt, 4 + k);
                        cnt[6 + k] = p.CopyBuffer[i].type;  //BitConverter.GetBytes(p.CopyBuffer[i].type).CopyTo(cnt, 6 + k);
                        k = k + 7;
                    }
                    cnt = cnt.GZip();
                    fs.Write(cnt, 0, cnt.Length);
                    fs.Flush();
                    fs.Close();
                }
                Player.SendMessage(p, "Saved copy as " + message);
            }
            else Player.SendMessage(p, "Bad file name");
        }

        void Loadcopy(Player p, string message)
        {
            if (message.EndsWith("#"))
            {
                if (!p.group.CanExecute(Command.all.Find("copyloadnet")))
                {
                    Player.SendMessage(p, "You are not allowed to load from network locations.");
                    return;
                }
                try
                {
                    p.CopyBuffer.Clear();
                    message = message.Remove(message.Length - 1);
                    if (!message.StartsWith("http://", true, System.Globalization.CultureInfo.CurrentCulture)) message = "http://" + message;
                    using (System.Net.WebClient webget = new System.Net.WebClient())
                    {
                        byte[] cnt = webget.DownloadData(message);
                        cnt = cnt.Decompress();
                        int k = 0;
                        for (int i = 0; i < cnt.Length / 7; i++)
                        {
                            p.CopyBuffer.Add(new Player.CopyPos() { x = BitConverter.ToUInt16(cnt, 0 + k), y = BitConverter.ToUInt16(cnt, 2 + k), z = BitConverter.ToUInt16(cnt, 4 + k), type = cnt[6 + k] });
                            k = k + 7;
                        }
                        Player.SendMessage(p, "Loaded copy from " + message);
                    }
                }
                catch { Player.SendMessage(p, "Failed to load copy from " + message); }
                return;
            }
            if (!File.Exists("extra/savecopy/" + p.name + "/" + message + ".cpy")) { Player.SendMessage(p, "No such copy exists"); return; }
            p.CopyBuffer.Clear();
            using (FileStream fs = new FileStream("extra/savecopy/" + p.name + "/" + message + ".cpy", FileMode.Open))
            {
                byte[] cnt = new byte[fs.Length];
                fs.Read(cnt, 0, (int)fs.Length);
                cnt = cnt.Decompress();
                int k = 0;
                for (int i = 0; i < cnt.Length / 7; i++)
                {
                    p.CopyBuffer.Add(new Player.CopyPos() { x = BitConverter.ToUInt16(cnt, 0 + k), y = BitConverter.ToUInt16(cnt, 2 + k), z = BitConverter.ToUInt16(cnt, 4 + k), type = cnt[6 + k] });
                    k = k + 7;
                }
                fs.Flush();
                fs.Close();
            }
            Player.SendMessage(p, "Loaded copy as " + message);
        }

        void BufferAdd(Player p, ushort x, ushort y, ushort z, byte type)
        {
            Player.CopyPos pos; pos.x = x; pos.y = y; pos.z = z; pos.type = type;
            p.CopyBuffer.Add(pos);
        }
        struct CatchPos { public ushort x, y, z; public int type; public List<byte> ignoreTypes; }
    }

    public class CmdCopyLoadNet : Command
    {
        public override string name { get { return "copyloadnet"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdCopyLoadNet() { }

        public override void Use(Player p, string message)
        {
            Command.all.Find("copy").Use(p, "load " + message + "#");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/copyloadnet - Loads a copy from network location. Example below:");
            Player.SendMessage(p, "/copyloadnet servername.com/directory/savename.cpy");
        }
    }

    public class CmdCopySaveNet : Command
    {
        public override string name { get { return "copysavenet"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdCopySaveNet() { }

        public override void Use(Player p, string message)
        {
            Command.all.Find("copy").Use(p, "save " + message + "#");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/copysavenet - Saves a copy to a network location. Example below:");
            Player.SendMessage(p, "/copysavenet servername.com/directory/");
        }
    }
}
