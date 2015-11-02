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
using System.Collections;
using System.Security.Cryptography;
namespace MCGalaxy.Commands
{
    public sealed class CmdMaze : Command
    {
        public override string name { get { return "maze"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public static int randomizer = 0;
        public static bool[,] wall;
        public override void Use(Player p, string message)
        {
            String[] split = message.Split(' ');
            if (split.Length >= 1&&message.Length>0)
            {
                try
                {
                    randomizer = int.Parse(split[0]);
                }
                catch (Exception)
                {
                    this.Help(p); return;
                }
            }
            Player.SendMessage(p, "Place two blocks to determine the edges");
            
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            p.blockchangeObject = new CatchPos(x, y, z);
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            Player.SendMessage(p, "Generating maze... this could take a while");
            CatchPos first = (CatchPos)p.blockchangeObject;
            int width = Math.Max(x, first.X) - Math.Min(x, first.X);
            if (width % 2 != 0) { width++;x--; }
            width -= 2;
            int height = Math.Max(z, first.Z) - Math.Min(z, first.Z);
            if (height % 2 != 0) { height++;z--; }
            height -= 2;
            //substract 2 cause we will just make the inner. the outer wall is made seperately
            wall = new bool[width+1, height+1];//+1 cause we begin at 0 so we need one object more
            for (int w = 0; w <= width; w++)
            {
                for (int h = 0; h <= height; h++)
                {
                    wall[w, h] = true;
                }
            }
            GridNode.maxX = width;
            GridNode.maxY = height;
            //Make a Stack
            Stack s = new Stack(width * height);
            //Random rand = new Random(DateTime.Now.Millisecond);//ha yeah randomized :P
            //lets begin in the lower left corner eh?(0,0)
            s.Push(new GridNode(0, 0));
            wall[0, 0] = false;
            while (true)
            {
                GridNode node = (GridNode)s.Peek();
                if (node.turnsPossible())
                {
                    GridNode[] nodearray = node.getRandomNext();
                    wall[nodearray[0].X, nodearray[0].Y] = false;
                    wall[nodearray[1].X, nodearray[1].Y] = false;
                    s.Push(nodearray[1]);
                    //we get the next two nodes
                    //the first is a middle node from which there shouldnt start a new corridor
                    //the second is added to the stack. next try will be with this node
                    //i hope this will work this time...
                }
                else
                {
                    s.Pop();//if this node is a dead and it will be removed
                }

                if (s.Count < 1)
                {
                    break;//if no nodes are free anymore we will end the generation here
                }
            }
            Player.SendMessage(p, "Maze is generated. now painting...");
            //seems to be there are no more moves possible
            //paint that shit :P
            ushort minx = Math.Min(x, first.X);
            ushort minz = Math.Min(z, first.Z);
            ushort maxx = Math.Max(x, first.X);
            maxx++;
            ushort maxz = Math.Max(z, first.Z);
            maxz++;
            for (ushort xx = 0; xx <= width; xx++)
            {
                for (ushort zz = 0; zz <= height; zz++)
                {
                    if (wall[xx, zz])
                    {
                        p.level.Blockchange(p, (ushort)(xx + minx+1), y, (ushort)(zz + minz+1), Block.staircasefull);
                        p.level.Blockchange(p, (ushort)(xx + minx+1), (ushort)(y + 1), (ushort)(zz + minz+1), Block.leaf);
                        p.level.Blockchange(p, (ushort)(xx + minx+1), (ushort)(y + 2), (ushort)(zz + minz+1), Block.leaf);
                    }
                }
            }
            p.ignorePermission = true;
            Command.all.Find("cuboid").Use(p, "walls");
            p.manualChange(minx, y, minz, 0, Block.staircasefull);
            p.manualChange(maxx, y, maxz, 0, Block.staircasefull);
            Command.all.Find("cuboid").Use(p, "walls");
            p.manualChange(minx, (ushort)(y + 1), minz, 0, Block.leaf);
            p.manualChange(maxx, (ushort)(y + 2), maxz, 0, Block.leaf);
            Player.SendMessage(p, "Maze painted. Build your entrance and exit yourself");
            randomizer = 0;
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/maze: generates a maze");
        }
        private class CatchPos
        {
            public ushort X;
            public ushort Y;
            public ushort Z;
            //public byte type;

            public CatchPos(ushort x, ushort y, ushort z)
            {
                this.X = x;
                this.Y = y;
                this.Z = z;
            }
        }

        private class GridNode
        {
            public static int maxX = 0;
            public static int maxY = 0;
            public ushort X;
            public ushort Y;
            private Random rand2 = new Random(Environment.TickCount);
            public GridNode[] getRandomNext()
            {
                byte[] r = new byte[1];
                switch (randomizer)
                {
                    case 0:
                        RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
                        rand.GetBytes(r);
                        r[0] /= (255 / 4);
                        break;
                    case 1:
                        r[0] = (byte)rand2.Next(4);
                        break;
                    default:
                        Random rand3 = new Random(Environment.TickCount);
                        r[0] = (byte)rand2.Next(4);
                        break;
                }
                ushort rx = 0, ry = 0, rx2 = 0, ry2 = 0;
                    switch (r[0])
                    {
                        case 0:
                            if (isWall(X, Y + 2))
                            {
                                //go up
                                rx = X;
                                rx2 = X;
                                ry = (ushort)(Y + 1);
                                ry2 = (ushort)(Y + 2);
                            }
                            else
                            {
                                return this.getRandomNext();
                            }
                            break;
                        case 1:
                            if (isWall(X, Y - 2))
                            {
                                //go down
                                rx = X;
                                rx2 = X;
                                ry = (ushort)(Y - 1);
                                ry2 = (ushort)(Y - 2);
                            }
                            else
                            {
                                return this.getRandomNext();
                            }
                            break;
                        case 2:
                            if (isWall(X + 2, Y))
                            {
                                //go right
                                rx = (ushort)(X + 1);
                                rx2 = (ushort)(X + 2);
                                ry = Y;
                                ry2 = Y;
                            }
                            else
                            {
                                return this.getRandomNext();
                            }
                            break;
                        case 3:
                            if (isWall(X - 2, Y))
                            {
                                //go left
                                rx = (ushort)(X - 1);
                                rx2 = (ushort)(X - 2);
                                ry = Y;
                                ry2 = Y;
                            }
                            else
                            {
                                return this.getRandomNext();
                            }
                            break;
                    }
                return new GridNode[] { new GridNode(rx, ry), new GridNode(rx2, ry2) };
            }
            public bool turnsPossible()
            {
                return (isWall(X, Y + 2) || isWall(X, Y - 2) || isWall(X + 2, Y) || isWall(X - 2, Y));
                
            }

            private bool isWall(int x, int y)
            {
                try
                {
                    return wall[x, y];
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }
            public GridNode(ushort x, ushort y) {
                X = x;
                Y = y;
            }
        }
    }

    
}
