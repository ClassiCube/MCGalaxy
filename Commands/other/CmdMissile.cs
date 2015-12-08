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
using System.Threading;
namespace MCGalaxy.Commands
{
	public sealed class CmdMissile : Command
	{
		public override string name { get { return "missile"; } }
		public override string shortcut { get { return ""; } }
		public override string type { get { return CommandTypes.Other; } }
		public override bool museumUsable { get { return false; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		public CmdMissile() { }
		public override void Use(Player p, string message)
		{
			Level foundLevel;
			foundLevel = p.level;
			if (foundLevel.guns == false)
			{
				Player.SendMessage(p, "Guns and missiles cannot be used on this map!");
				return;
			}
			Pos cpos;

			if (p.aiming)
			{
				if (message == "")
				{
					p.aiming = false;
					Player.SendMessage(p, "Disabled missiles");
					return;
				}
			}

			cpos.ending = 0;
			if (message.ToLower() == "destroy") cpos.ending = 1;
			if (p.allowTnt == false)
			{
				if (message.ToLower() == "explode")
				{
					Player.SendMessage(p, "Since tnt usage is currently disabled, normal missile enabled"); cpos.ending = 1;
				}
				else if (message.ToLower() == "teleport" || message.ToLower() == "tp") cpos.ending = -1;
				else if (message != "") { Help(p); return; }
			}
			else if (message.ToLower() == "explode") cpos.ending = 2;
			else if (message.ToLower() == "teleport" || message.ToLower() == "tp") cpos.ending = -1;
			else if (message != "") { Help(p); return; }

			cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
			p.ClearBlockchange();
			p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);

			p.SendMessage("Missile mode engaged, fire and guide!");

			if (p.aiming)
			{
				return;
			}

			p.aiming = true;
			Thread aimThread = new Thread(new ThreadStart(delegate
			{
				CatchPos pos;
				List<CatchPos> buffer = new List<CatchPos>();
				while (p.aiming)
				{
					List<CatchPos> tempBuffer = new List<CatchPos>();

					double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
					double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
					double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

					try
					{
						ushort x = (ushort)(p.pos[0] / 32);
						x = (ushort)Math.Round(x + (double)(a * 3));

						ushort y = (ushort)(p.pos[1] / 32 + 1);
						y = (ushort)Math.Round(y + (double)(c * 3));

						ushort z = (ushort)(p.pos[2] / 32);
						z = (ushort)Math.Round(z + (double)(b * 3));

						if (x > p.level.Width || y > p.level.Height || z > p.level.Length) throw new Exception();
						if (x < 0 || y < 0 || z < 0) throw new Exception();

						for (ushort xx = x; xx <= x + 1; xx++)
						{
							for (ushort yy = (ushort)(y - 1); yy <= y; yy++)
							{
								for (ushort zz = z; zz <= z + 1; zz++)
								{
									if (p.level.GetTile(xx, yy, zz) == Block.air)
									{
										pos.x = xx; pos.y = yy; pos.z = zz;
										tempBuffer.Add(pos);
									}
								}
							}
						}

						List<CatchPos> toRemove = new List<CatchPos>();
						foreach (CatchPos cP in buffer)
						{
							if (!tempBuffer.Contains(cP))
							{
								p.SendBlockchange(cP.x, cP.y, cP.z, Block.air);
								toRemove.Add(cP);
							}
						}

						foreach (CatchPos cP in toRemove)
						{
							buffer.Remove(cP);
						}

						foreach (CatchPos cP in tempBuffer)
						{
							if (!buffer.Contains(cP))
							{
								buffer.Add(cP);
								p.SendBlockchange(cP.x, cP.y, cP.z, Block.glass);
							}
						}

						tempBuffer.Clear();
						toRemove.Clear();
					}
					catch { }
					Thread.Sleep(20);
				}

				foreach (CatchPos cP in buffer)
				{
					p.SendBlockchange(cP.x, cP.y, cP.z, Block.air);
				}
			}));
			aimThread.Name = "MCG_AimMissile";
			aimThread.Start();
		}
		public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
		{
			if (!p.staticCommands)
			{
				p.ClearBlockchange();
				p.aiming = false;
			}
			byte by = p.level.GetTile(x, y, z);
			p.SendBlockchange(x, y, z, by);
			Pos bp = (Pos)p.blockchangeObject;

			List<CatchPos> previous = new List<CatchPos>();
			List<CatchPos> allBlocks = new List<CatchPos>();
			CatchPos pos;

			if (p.modeType != Block.air)
				type = p.modeType;

			Thread gunThread = new Thread(new ThreadStart(delegate
			{
				ushort startX = (ushort)(p.pos[0] / 32);
				ushort startY = (ushort)(p.pos[1] / 32);
				ushort startZ = (ushort)(p.pos[2] / 32);
				pos.x = startX;
				pos.y = startY;
				pos.z = startZ;

				int total = 0;

				while (true)
				{
					startX = (ushort)(p.pos[0] / 32);
					startY = (ushort)(p.pos[1] / 32);
					startZ = (ushort)(p.pos[2] / 32);

					total++;
					double a = Math.Sin(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
					double b = Math.Cos(((double)(128 - p.rot[0]) / 256) * 2 * Math.PI);
					double c = Math.Cos(((double)(p.rot[1] + 64) / 256) * 2 * Math.PI);

					CatchPos lookedAt;
					int i;
					for (i = 1; true; i++)
					{
						lookedAt.x = (ushort)Math.Round(startX + (double)(a * i));
						lookedAt.y = (ushort)Math.Round(startY + (double)(c * i));
						lookedAt.z = (ushort)Math.Round(startZ + (double)(b * i));

						by = p.level.GetTile(lookedAt.x, lookedAt.y, lookedAt.z);

						if (by == Block.Zero)
							break;

						if (by != Block.air && !allBlocks.Contains(lookedAt))
						{
							if (p.level.physics < 2 || bp.ending <= 0)
							{
								break;
							}
							else
							{
								if (bp.ending == 1)
								{
									if ((!Block.FireKill(by) && !Block.NeedRestart(by)) && by != Block.glass)
									{
										break;
									}
								}
								else if (p.level.physics >= 3)
								{
									if (by != Block.glass)
									{
										break;
									}
								}
								else
								{
									break;
								}
							}
						}

						bool comeInner = false;
						foreach (Player pl in Player.players)
						{
							if (pl.level == p.level && pl != p)
							{
								if ((ushort)(pl.pos[0] / 32) == lookedAt.x || (ushort)(pl.pos[0] / 32 + 1) == lookedAt.x || (ushort)(pl.pos[0] / 32 - 1) == lookedAt.x)
								{
									if ((ushort)(pl.pos[1] / 32) == lookedAt.y || (ushort)(pl.pos[1] / 32 + 1) == lookedAt.y || (ushort)(pl.pos[1] / 32 - 1) == lookedAt.y)
									{
										if ((ushort)(pl.pos[2] / 32) == lookedAt.z || (ushort)(pl.pos[2] / 32 + 1) == lookedAt.z || (ushort)(pl.pos[2] / 32 - 1) == lookedAt.z)
										{
											lookedAt.x = (ushort)(pl.pos[0] / 32);
											lookedAt.y = (ushort)(pl.pos[1] / 32);
											lookedAt.z = (ushort)(pl.pos[2] / 32);
											comeInner = true;
											break;
										}
									}
								}
							}
						}
						if (comeInner) break;
					}

					lookedAt.x = (ushort)Math.Round(startX + (double)(a * (i - 1)));
					lookedAt.y = (ushort)Math.Round(startY + (double)(c * (i - 1)));
					lookedAt.z = (ushort)Math.Round(startZ + (double)(b * (i - 1)));

					findNext(lookedAt, ref pos);

					by = p.level.GetTile(pos.x, pos.y, pos.z);
					if (total > 3)
					{
						if (by != Block.air && !allBlocks.Contains(pos))
						{
							if (p.level.physics < 2 || bp.ending <= 0)
							{
								break;
							}
							else
							{
								if (bp.ending == 1)
								{
									if ((!Block.FireKill(by) && !Block.NeedRestart(by)) && by != Block.glass)
									{
										break;
									}
								}
								else if (p.level.physics >= 3)
								{
									if (by != Block.glass)
									{
										if (p.allowTnt)
										{
											p.level.MakeExplosion(pos.x, pos.y, pos.z, 1);
											break;
										}
										break;
									}
								}
								else
								{
									break;
								}
							}
						}

						p.level.Blockchange(pos.x, pos.y, pos.z, type);
						previous.Add(pos);
						allBlocks.Add(pos);

						bool comeOut = false;
						foreach (Player pl in Player.players)
						{
							if (pl.level == p.level && pl != p)
							{
								if ((ushort)(pl.pos[0] / 32) == pos.x || (ushort)(pl.pos[0] / 32 + 1) == pos.x || (ushort)(pl.pos[0] / 32 - 1) == pos.x)
								{
									if ((ushort)(pl.pos[1] / 32) == pos.y || (ushort)(pl.pos[1] / 32 + 1) == pos.y || (ushort)(pl.pos[1] / 32 - 1) == pos.y)
									{
										if ((ushort)(pl.pos[2] / 32) == pos.z || (ushort)(pl.pos[2] / 32 + 1) == pos.z || (ushort)(pl.pos[2] / 32 - 1) == pos.z)
										{
											if (p.level.physics >= 3 && bp.ending >= 2)
												pl.HandleDeath(Block.stone, " was blown up by " + p.color + p.name, true);
											else
												pl.HandleDeath(Block.stone, " was hit a missile from " + p.color + p.name);
											comeOut = true;
										}
									}
								}
							}
						}
						if (comeOut) break;

						if (pos.x == lookedAt.x && pos.y == lookedAt.y && pos.z == lookedAt.z)
						{
							if (p.level.physics >= 3 && bp.ending >= 2)
							{
								if (p.allowTnt)
								{
									p.level.MakeExplosion(lookedAt.x, lookedAt.y, lookedAt.z, 2);
									break;
								}
							}
						}

						if (previous.Count > 12)
						{
							p.level.Blockchange(previous[0].x, previous[0].y, previous[0].z, Block.air);
							previous.Remove(previous[0]);
						}
						Thread.Sleep(100);
					}
				}

				foreach (CatchPos pos1 in previous)
				{
					p.level.Blockchange(pos1.x, pos1.y, pos1.z, Block.air);
					Thread.Sleep(100);
				}
			}));
			gunThread.Name = "MCG_Missile";
			gunThread.Start();
		}
		public override void Help(Player p)
		{
			Player.SendMessage(p, "/missile [at end] - Allows you to fire missiles at people");
			Player.SendMessage(p, "Available [at end] values: &cexplode, destroy");
			Player.SendMessage(p, "Differs from /gun in that the missile is guided");
		}

		struct CatchPos { public ushort x, y, z; }
		struct Pos { public ushort x, y, z; public int ending; }

		void findNext(CatchPos lookedAt, ref CatchPos pos)
		{
			int dx, dy, dz, l, m, n, x_inc, y_inc, z_inc, err_1, err_2, dx2, dy2, dz2;
			int[] pixel = new int[3];

			pixel[0] = pos.x; pixel[1] = pos.y; pixel[2] = pos.z;
			dx = lookedAt.x - pos.x; dy = lookedAt.y - pos.y; dz = lookedAt.z - pos.z;

			x_inc = (dx < 0) ? -1 : 1; l = Math.Abs(dx);
			y_inc = (dy < 0) ? -1 : 1; m = Math.Abs(dy);
			z_inc = (dz < 0) ? -1 : 1; n = Math.Abs(dz);

			dx2 = l << 1; dy2 = m << 1; dz2 = n << 1;

			if ((l >= m) && (l >= n))
			{
				err_1 = dy2 - l;
				err_2 = dz2 - l;

				pixel[0] += x_inc;
				if (err_1 > 0)
				{
					pixel[1] += y_inc;
					err_1 -= dx2;
				}
				if (err_2 > 0)
				{
					pixel[2] += z_inc;
					err_2 -= dx2;
				}
				err_1 += dy2;
				err_2 += dz2;

				pos.x = (ushort)pixel[0];
				pos.y = (ushort)pixel[1];
				pos.z = (ushort)pixel[2];
			}
			else if ((m >= l) && (m >= n))
			{
				err_1 = dx2 - m;
				err_2 = dz2 - m;

				pixel[1] += y_inc;
				if (err_1 > 0)
				{
					pixel[0] += x_inc;
					err_1 -= dy2;
				}
				if (err_2 > 0)
				{
					pixel[2] += z_inc;
					err_2 -= dy2;
				}
				err_1 += dx2;
				err_2 += dz2;

				pos.x = (ushort)pixel[0];
				pos.y = (ushort)pixel[1];
				pos.z = (ushort)pixel[2];
			}
			else
			{
				err_1 = dy2 - n;
				err_2 = dx2 - n;

				pixel[2] += z_inc;
				if (err_1 > 0)
				{
					pixel[1] += y_inc;
					err_1 -= dz2;
				}
				if (err_2 > 0)
				{
					pixel[0] += x_inc;
					err_2 -= dz2;
				}
				err_1 += dy2;
				err_2 += dx2;

				pos.x = (ushort)pixel[0];
				pos.y = (ushort)pixel[1];
				pos.z = (ushort)pixel[2];
			}
		}
	}
}
