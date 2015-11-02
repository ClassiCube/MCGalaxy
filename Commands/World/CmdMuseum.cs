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
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
namespace MCGalaxy.Commands
{
    public sealed class CmdMuseum : Command
    {
        public override string name { get { return "museum"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMuseum() { }

        public override void Use(Player p, string message)
        {

            string path;

            if (message.Split(' ').Length == 1) path = "levels/" + message + ".lvl";
            else if (message.Split(' ').Length == 2) try { path = @Server.backupLocation + "/" + message.Split(' ')[0] + "/" + int.Parse(message.Split(' ')[1]) + "/" + message.Split(' ')[0] + ".lvl"; }
                catch { Help(p); return; }
            else { Help(p); return; }

            if (File.Exists(path))
            {
                FileStream fs = File.OpenRead(path);
                try
                {

                    GZipStream gs = new GZipStream(fs, CompressionMode.Decompress);
                    byte[] ver = new byte[2];
                    gs.Read(ver, 0, ver.Length);
                    ushort version = BitConverter.ToUInt16(ver, 0);
					ushort[] vars = new ushort[6];
					byte[] rot = new byte[2];

					if (version == 1874)
					{
						byte[] header = new byte[16]; gs.Read(header, 0, header.Length);

						vars[0] = BitConverter.ToUInt16(header, 0);
						vars[1] = BitConverter.ToUInt16(header, 2);
						vars[2] = BitConverter.ToUInt16(header, 4);
						vars[3] = BitConverter.ToUInt16(header, 6);
						vars[4] = BitConverter.ToUInt16(header, 8);
						vars[5] = BitConverter.ToUInt16(header, 10);

						rot[0] = header[12];
						rot[1] = header[13];

						//level.permissionvisit = (LevelPermission)header[14];
						//level.permissionbuild = (LevelPermission)header[15];
					}
					else
					{
						byte[] header = new byte[12]; gs.Read(header, 0, header.Length);

						vars[0] = version;
						vars[1] = BitConverter.ToUInt16(header, 0);
						vars[2] = BitConverter.ToUInt16(header, 2);
						vars[3] = BitConverter.ToUInt16(header, 4);
						vars[4] = BitConverter.ToUInt16(header, 6);
						vars[5] = BitConverter.ToUInt16(header, 8);

						rot[0] = header[10];
						rot[1] = header[11];
					}

					Level level = new Level(name, vars[0], vars[2], vars[1], "empty");
					level.setPhysics(0);

					level.spawnx = vars[3];
					level.spawnz = vars[4];
					level.spawny = vars[5];
					level.rotx = rot[0];
					level.roty = rot[1];

					byte[] blocks = new byte[level.width * level.height * level.depth];
					gs.Read(blocks, 0, blocks.Length);
					level.blocks = blocks;
					gs.Close();

					level.backedup = true;
					level.permissionbuild = LevelPermission.Admin;

					level.jailx = (ushort)(level.spawnx * 32); level.jaily = (ushort)(level.spawny * 32); level.jailz = (ushort)(level.spawnz * 32);
					level.jailrotx = level.rotx; level.jailroty = level.roty;

					p.Loading = true;
					foreach (Player pl in Player.players) if (p.level == pl.level && p != pl) p.SendDie(pl.id);
					foreach (PlayerBot b in PlayerBot.playerbots) if (p.level == b.level) p.SendDie(b.id);

					Player.GlobalDie(p, true);

					p.level = level;
					p.SendMotd();

					p.SendRaw(2);
					byte[] buffer = new byte[level.blocks.Length + 4];
					BitConverter.GetBytes(IPAddress.HostToNetworkOrder(level.blocks.Length)).CopyTo(buffer, 0);
					//ushort xx; ushort yy; ushort zz;

					for (int i = 0; i < level.blocks.Length; ++i)
						buffer[4 + i] = Block.Convert(level.blocks[i]);

					buffer = buffer.GZip();
					int number = (int)Math.Ceiling(((double)buffer.Length) / 1024);
					for (int i = 1; buffer.Length > 0; ++i)
					{
						short length = (short)Math.Min(buffer.Length, 1024);
						byte[] send = new byte[1027];
						Player.HTNO(length).CopyTo(send, 0);
						Buffer.BlockCopy(buffer, 0, send, 2, length);
						byte[] tempbuffer = new byte[buffer.Length - length];
						Buffer.BlockCopy(buffer, length, tempbuffer, 0, buffer.Length - length);
						buffer = tempbuffer;
						send[1026] = (byte)(i * 100 / number);
						p.SendRaw(3, send);
						Thread.Sleep(10);
					} buffer = new byte[6];
					Player.HTNO((short)level.width).CopyTo(buffer, 0);
					Player.HTNO((short)level.depth).CopyTo(buffer, 2);
					Player.HTNO((short)level.height).CopyTo(buffer, 4);
					p.SendRaw(4, buffer);

					ushort x = (ushort)((0.5 + level.spawnx) * 32);
					ushort y = (ushort)((1 + level.spawny) * 32);
					ushort z = (ushort)((0.5 + level.spawnz) * 32);

					p.aiming = false;
					Player.GlobalSpawn(p, x, y, z, level.rotx, level.roty, true);
					p.ClearBlockchange();
					p.Loading = false;

					if (message.IndexOf(' ') == -1)
						level.name = "&cMuseum " + Server.DefaultColor + "(" + message.Split(' ')[0] + ")";
					else
						level.name = "&cMuseum " + Server.DefaultColor + "(" + message.Split(' ')[0] + " " + message.Split(' ')[1] + ")";

					if (!p.hidden)
					{
                        Player.GlobalMessage(p.color + p.prefix + p.name + Server.DefaultColor + " went to the " + level.name);
					}
				}
                catch (Exception ex) { Player.SendMessage(p, "Error loading level."); Server.ErrorLog(ex); return; }
                finally { fs.Close(); }
            }
            else { Player.SendMessage(p, "Level or backup could not be found."); return; }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/museum <map> <restore> - Allows you to access a restore of the map entered.");
            Player.SendMessage(p, "Works on offline maps");
        }
    }
}
