/*
    Thanks to aaron1tasker

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
namespace MCGalaxy.Commands
{
    public sealed class CmdPyramid : Command
    {
        public override string name { get { return "pyramid"; } }
        public override string shortcut { get { return "pd"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdPyramid() { }
        public static byte wait;

        public override void Use(Player p, string message)
        {
            wait = 0;
            p.pyramidblock = "stone";
            int number = message.Split(' ').Length;
            if (number > 2) { Help(p); wait = 1; return; }
            if (number == 2)
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                byte type = Block.Byte(t);
                if (type == 255) { Player.SendMessage(p, "There is no block \"" + t + "\"."); wait = 1; return; }
                if (!Block.canPlace(p, type)) { Player.SendMessage(p, "Cannot place that."); wait = 1; return; }
                SolidType solid;
                if (s == "solid") { solid = SolidType.solid; }
                else if (s == "hollow") { solid = SolidType.hollow; }
                else if (s == "reverse") { solid = SolidType.reverse; }
                else { Help(p); wait = 1; return; }
                CatchPos cpos; cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
                cpos.type = Block.Byte(message);
                p.pyramidblock = t;
            }
            else if (message != "")
            {
                SolidType solid = SolidType.hollow;
                message = message.ToLower();
                byte type; unchecked { type = (byte)-1; }
                p.pyramidblock = "stone";
                if (message == "solid") { solid = SolidType.solid; }
                else if (message == "hollow") { solid = SolidType.hollow; }
                else if (message == "reverse") { solid = SolidType.reverse; }
                else
                {
                    byte t = Block.Byte(message);
                    if (t == 255) { Player.SendMessage(p, "There is no block \"" + message + "\"."); wait = 1; return; }
                    if (!Block.canPlace(p, t)) { Player.SendMessage(p, "Cannot place that."); wait = 1; return; }
                    p.pyramidblock = message;

                } CatchPos cpos; cpos.solid = solid; cpos.type = type;
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            else
            {
                CatchPos cpos; cpos.solid = SolidType.solid; unchecked { cpos.type = (byte)-1; }
                cpos.x = 0; cpos.y = 0; cpos.z = 0; p.blockchangeObject = cpos;
            }
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            if (p.pyramidblock == "")
            {
                p.pyramidblock = "stone";
            }
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/pyramid [type] <solid/hollow/reverse> - create a pyramid of blocks.");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.pyramidx1 = bp.x;
            p.pyramidz1 = bp.y;
            p.pyramidy1 = bp.z;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            cpos.x = x; cpos.y = y; cpos.z = z; p.blockchangeObject = cpos;
            List<Pos> buffer = new List<Pos>();
            p.pyramidsilent = true;
            int total = calculate(p, cpos);
            if (total > p.group.maxBlocks)
            {
                Player.SendMessage(p, "Tried to modify " + total + " blocks, but your limit is " + p.group.maxBlocks + ".");
                return;
            }

            switch (cpos.solid)
            {
                case SolidType.solid:
                    buffer.Capacity = Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z);
                    p.pyramidx2 = cpos.x;
                    p.pyramidz2 = cpos.y;
                    p.pyramidy2 = cpos.z;

                    if (p.pyramidz1 == p.pyramidz2) //checks if pyramid is on a level surface
                    {

                        Command.all.Find("cuboid").Use(p, p.pyramidblock);
                        click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1);
                        click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);

                        for (int i = 1; i > 0; ) //looper to create pyramid
                        {
                            if (p.pyramidx1 == p.pyramidx2)
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords from text files
                                i--;
                            }
                            else if (p.pyramidy1 == p.pyramidy2) { i--; }
                            if (p.pyramidx1 > p.pyramidx2) //checks if 2 coords are the same for x and provides escape sequence if they are
                            {
                                if ((p.pyramidx1 - p.pyramidx2) == 1)
                                {
                                    Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                    click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                    click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                                    i--;
                                }
                            }
                            else if ((p.pyramidx2 - p.pyramidx1) == 1)  //checks if 2 coords are the same for y and provides escape sequence if they are
                            {
                                i--;
                            }
                            if (p.pyramidy1 > p.pyramidy2) //checks if 2 coords are the same for x and provides escape sequence if they are
                            {
                                if ((p.pyramidy1 - p.pyramidy2) == 1)
                                {
                                    Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                    click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                    click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                                    i--;
                                }
                            }
                            else if ((p.pyramidy2 - p.pyramidy1) == 1)  //checks if 2 coords are the same for y and provides escape sequence if they are
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                                i--;
                            }
                            Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                            click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                            click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                            
                            if (p.pyramidx1 > p.pyramidx2) //checks if one is greater than the other. This way it knows which one to add one two and which one to minus one from so that it reaches the middle.
                            {
                                p.pyramidx1--;
                                p.pyramidx2++;
                            }
                            else
                            {
                                p.pyramidx1++;
                                p.pyramidx2--;
                            }

                            if (p.pyramidy1 > p.pyramidy2) //does the same for the y coords
                            {
                                p.pyramidy1--;
                                p.pyramidy2++;
                            }
                            else
                            {
                                p.pyramidy1++;
                                p.pyramidy2--;
                            }
                            p.pyramidz2++;                             
                        }

                        p.pyramidx1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidy1 = 0;
                        p.pyramidy2 = 0;
                        p.pyramidz1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidblock = "";
                        p.pyramidsilent = false;
                        Player.SendMessage(p, "Pyramid completed.");
                        wait = 2;
                    }
                    else
                    {
                        p.pyramidx1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidy1 = 0;
                        p.pyramidy2 = 0;
                        p.pyramidz1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidblock = "";
                        Player.SendMessage(p, "The two edges of the pyramid must be on the same level");
                        wait = 1;
                    }

                    break;

                case SolidType.reverse:
                    p.pyramidx2 = cpos.x;
                    p.pyramidz2 = cpos.y;
                    p.pyramidy2 = cpos.z;

                    if (p.pyramidz1 == p.pyramidz2)
                    {
                        for (int i = 1; i > 0; )
                        {
                            if (p.pyramidx1 == p.pyramidx2)
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                                i--;
                            }
                            else if (p.pyramidy1 == p.pyramidy2) { i--; }
                            if (p.pyramidx1 > p.pyramidx2) //checks if 2 coords are the same for x and provides escape sequence if they are
                            {
                                if ((p.pyramidx1 - p.pyramidx2) == 1)
                                {
                                    Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                    click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                    click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                                    i--;
                                }
                            }
                            else if ((p.pyramidx2 - p.pyramidx1) == 1)  //checks if 2 coords are the same for y and provides escape sequence if they are
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2); //clicks on coords
                                i--;
                            }
                            if (p.pyramidy1 > p.pyramidy2) //checks if 2 coords are the same for x and provides escape sequence if they are
                            {
                                if ((p.pyramidy1 - p.pyramidy2) == 1)
                                {
                                    Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                    click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                    click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                    i--;
                                }
                            }
                            else if ((p.pyramidy2 - p.pyramidy1) == 1)  //checks if 2 coords are the same for y and provides escape sequence if they are
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords from text files
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                i--;
                            }
                            Command.all.Find("cuboid").Use(p, "air");
                            click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1);
                            click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                            Command.all.Find("cuboid").Use(p, p.pyramidblock + " " + "walls");
                            click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1);
                            click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);

                            if (p.pyramidx1 > p.pyramidx2)
                            {
                                p.pyramidx1--;
                                p.pyramidx2++;
                            }
                            else
                            {
                                p.pyramidx1++;
                                p.pyramidx2--;
                            }

                            if (p.pyramidy1 > p.pyramidy2)
                            {
                                p.pyramidy1--;
                                p.pyramidy2++;
                            }
                            else
                            {
                                p.pyramidy1++;
                                p.pyramidy2--;
                            }
                            p.pyramidz2--;
                        }

                        p.pyramidx1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidy1 = 0;
                        p.pyramidy2 = 0;
                        p.pyramidz1 = 0;
                        p.pyramidz2 = 0;
                        p.pyramidblock = "";
                        wait = 2;
                        p.pyramidsilent = false;
                        Player.SendMessage(p, "Pyramid Completed");
                    }
                    else
                    {
                        p.pyramidx1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidy1 = 0;
                        p.pyramidy2 = 0;
                        p.pyramidz1 = 0;
                        p.pyramidz2 = 0;
                        p.pyramidblock = "";
                        wait = 1;
                        Player.SendMessage(p, "The two edges of the pyramid must be on the same level");
                    }
                    break;

                case SolidType.hollow:
                    p.pyramidx2 = cpos.x;
                    p.pyramidz2 = cpos.y;
                    p.pyramidy2 = cpos.z;

                    if (p.pyramidz1 == p.pyramidz2)
                    {
                        Command.all.Find("cuboid").Use(p, p.pyramidblock);
                        click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1);
                        click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);

                        for (int i = 1; i > 0; )
                        {
                            if (p.pyramidx1 == p.pyramidx2)
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                i--;
                            }
                            else if (p.pyramidy1 == p.pyramidy2) { i--; }
                            if (p.pyramidx1 > p.pyramidx2) //checks if 2 coords are the same for x and provides escape sequence if they are
                            {
                                if ((p.pyramidx1 - p.pyramidx2) == 1)
                                {
                                    Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                    click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                    click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                    i--;
                                }
                            }
                            else if ((p.pyramidx2 - p.pyramidx1) == 1)  //checks if 2 coords are the same for y and provides escape sequence if they are
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                i--;
                            }
                            if (p.pyramidy1 > p.pyramidy2) //checks if 2 coords are the same for x and provides escape sequence if they are
                            {
                                if ((p.pyramidy1 - p.pyramidy2) == 1)
                                {
                                    Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                    click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                    click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                    i--;
                                }
                            }
                            else if ((p.pyramidy2 - p.pyramidy1) == 1)  //checks if 2 coords are the same for y and provides escape sequence if they are
                            {
                                Command.all.Find("cuboid").Use(p, p.pyramidblock); //cuboid
                                click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1); //clicks on coords
                                click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);
                                i--;
                            }
                            Command.all.Find("cuboid").Use(p, p.pyramidblock + " " + "walls");
                            click(p, p.pyramidx1 + " " + p.pyramidz2 + " " + p.pyramidy1);
                            click(p, p.pyramidx2 + " " + p.pyramidz2 + " " + p.pyramidy2);

                            if (p.pyramidx1 > p.pyramidx2)
                            {
                                p.pyramidx1--;
                                p.pyramidx2++;
                            }
                            else
                            {
                                p.pyramidx1++;
                                p.pyramidx2--;
                            }

                            if (p.pyramidy1 > p.pyramidy2)
                            {
                                p.pyramidy1--;
                                p.pyramidy2++;
                            }
                            else
                            {
                                p.pyramidy1++;
                                p.pyramidy2--;
                            }
                            p.pyramidz2++;
                        }
                        p.pyramidx1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidy1 = 0;
                        p.pyramidy2 = 0;
                        p.pyramidz1 = 0;
                        p.pyramidz2 = 0;
                        p.pyramidblock = "";
                        wait = 2;
                        p.pyramidsilent = false;
                        Player.SendMessage(p, "Pyramid Completed");
                    }
                    else
                    {
                        p.pyramidx1 = 0;
                        p.pyramidx2 = 0;
                        p.pyramidy1 = 0;
                        p.pyramidy2 = 0;
                        p.pyramidz1 = 0;
                        p.pyramidz2 = 0;
                        p.pyramidblock = "";
                        wait = 1;
                        Player.SendMessage(p, "The two edges of the pyramid must be on the same level");
                    }
                    break;
            }

            /*if (Server.forceCuboid)
            {
                int counter = 1;
                buffer.ForEach(delegate(Pos pos)
                {
                    if (counter <= p.group.maxBlocks)
                    {
                        counter++;
                        p.level.Blockchange(p, pos.x, pos.y, pos.z, type);
                    }
                });
                if (counter >= p.group.maxBlocks)
                {
                    Player.SendMessage(p, "Tried to pyramid " + buffer.Count + " blocks, but your limit is " + p.group.maxBlocks + ".");
                    Player.SendMessage(p, "Executed pyramid up to limit.");
                    wait = 2;
                }
                else
                {
                    Player.SendMessage(p, buffer.Count.ToString() + " blocks.");
                }
                wait = 2;
                if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                return;
            }

            if (buffer.Count > p.group.maxBlocks)
            {
                Player.SendMessage(p, "You tried to pyramid " + buffer.Count + " blocks.");
                Player.SendMessage(p, "You cannot pyramid more than " + p.group.maxBlocks + ".");
                wait = 1;
                return;
            }*/

            Player.SendMessage(p, total + " blocks.");

            /*buffer.ForEach(delegate(Pos pos)
            {
                p.level.Blockchange(p, pos.x, pos.y, pos.z, type);
            });
            wait = 2;*/
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        private static int calculate(Player p, CatchPos cpos)
        {
            int total = 0;
            for (int xx = Math.Min(cpos.x, p.pyramidx1); xx <= Math.Max(cpos.x, p.pyramidx1); ++xx)
                for (int zz = Math.Min(cpos.z, p.pyramidy1); zz <= Math.Max(cpos.z, p.pyramidy1); ++zz)
                {
                    total += 1;
                }
            int finaltotal = 0;
            int totald2 = (int)Math.Sqrt(total);
            int xxx = 0;
            int zzz = 0;
            int minx = Math.Min(cpos.x, p.pyramidx1);
            int maxx = Math.Max(cpos.x, p.pyramidx1);
            int minz = Math.Min(cpos.z, p.pyramidy1);
            int maxz = Math.Max(cpos.z, p.pyramidy1);
            for (int i = 0; i < totald2 / 2; i++)
            {
                for (int xx = minx; xx <= maxx; ++xx)
                    xxx += 1;
                for (int zz = minz; zz <= maxz; ++zz)
                    zzz += 1;
                finaltotal += xxx * zzz;
                xxx = 0;
                zzz = 0;
                maxx -= 2;
                maxz -= 2;
            }
            if (cpos.solid == SolidType.hollow) finaltotal = finaltotal - calculatehollow(p, cpos);
            return finaltotal;
        }
        private static int calculatehollow(Player p, CatchPos cpos)
        {
            int total = 0;
            int minx = Math.Min(cpos.x, p.pyramidx1);
            int maxx = Math.Max(cpos.x, p.pyramidx1) - 2;
            int minz = Math.Min(cpos.z, p.pyramidy1);
            int maxz = Math.Max(cpos.z, p.pyramidy1) - 2;
            for (int xx = minx; xx <= maxx; ++xx)
                for (int zz = minz; zz <= maxz; ++zz)
                {
                    total += 1;
                }
            int finaltotal = 0;
            int totald2 = (int)Math.Sqrt(total);
            int xxx = 0;
            int zzz = 0;
            for (int i = 0; i < totald2 / 2; i++)
            {
                for (int xx = minx; xx <= maxx; ++xx)
                    xxx += 1;
                for (int zz = minz; zz <= maxz; ++zz)
                    zzz += 1;
                finaltotal += xxx * zzz;
                xxx = 0;
                zzz = 0;
                maxx -= 2;
                maxz -= 2;
            }
            finaltotal -= total;
            return finaltotal;
        }
        void BufferAdd(List<Pos> list, ushort x, ushort y, ushort z)
        {
            Pos pos; pos.x = x; pos.y = y; pos.z = z; list.Add(pos);
        }
        struct Pos
        {
            public ushort x, y, z;
        }
        struct CatchPos
        {
            public SolidType solid;
            public byte type;
            public ushort x, y, z;
        }
        enum SolidType { solid, hollow, reverse };
        //<click command to stop text>
        public void click(Player p, string message)
        {
            string[] parameters = message.Split(' ');
            ushort[] click = p.lastClick;
            if (message.IndexOf(' ') != -1)
            {
                if (parameters.Length != 3)
                {
                    Help(p);
                    return;
                }
                else
                {
                    for (int value = 0; value < 3; value++)
                    {
                        if (parameters[value].ToLower() == "x" || parameters[value].ToLower() == "y" || parameters[value].ToLower() == "z")
                            click[value] = p.lastClick[value];
                        else if (isValid(parameters[value], value, p))
                            click[value] = ushort.Parse(parameters[value]);
                        else
                        {
                            Player.SendMessage(p, "\"" + parameters[value] + "\" was not valid");
                            return;
                        }
                    }
                }
            }
            p.manualChange(click[0], click[1], click[2], 0, Block.rock);
        }
        //</click command to stop text>

        //<something to do with click command>
        private bool isValid(string message, int dimension, Player p)
        {
            ushort testValue;
            try
            {
                testValue = ushort.Parse(message);
            }
            catch { return false; }
            if (testValue < 0)
                return false;
            if (testValue >= p.level.Width && dimension == 0) return false;
            else if (testValue >= p.level.Height && dimension == 1) return false;
            else if (testValue >= p.level.Length && dimension == 2) return false;
            return true;
        }
        //</something to do with click command> 
    }
}
