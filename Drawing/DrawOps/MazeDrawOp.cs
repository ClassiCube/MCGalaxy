/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Security.Cryptography;
using MCGalaxy.Drawing.Brushes;

namespace MCGalaxy.Drawing.Ops {
	
	public class MazeDrawOp : CuboidHollowsDrawOp {
		
		public override string Name { get { return "Maze"; } }
		
		public static int randomizer = 0;
		public static bool[,] wall;
		
		public override int GetBlocksAffected(Level lvl, Vector3U16[] marks) {
			int lenX = (Math.Abs(marks[1].X - marks[0].X) + 1) / 2;
			int lenZ = (Math.Abs(marks[1].Z - marks[0].Z) + 1) / 2;
			return lenX * lenZ * 3;
		}
		
		public override void Perform(Vector3U16[] marks, Player p, Level lvl, Brush brush) {
			int width = Max.X - Min.X;
			if (width % 2 != 0) { width++; Min.X--; }
			width -= 2;
			int length = Max.Z - Min.Z;
			if (length % 2 != 0) { length++; Min.Z--; }
			length -= 2;
			
			if (width < 0 || length < 0) {
				Player.SendMessage(p, "The corners of the maze need to be further apart."); return;
			}
			Player.SendMessage(p, "Generating maze... this could take a while");
			//substract 2 cause we will just make the inner. the outer wall is made seperately
			wall = new bool[width+1, length+1];//+1 cause we begin at 0 so we need one object more
			for (int w = 0; w <= width; w++)
				for (int h = 0; h <= length; h++)
			{
				wall[w, h] = true;
			}
			
			GridNode.maxX = width;
			GridNode.maxY = length;
			//Make a Stack
			Stack<GridNode> s = new Stack<GridNode>(width * length);
			//Random rand = new Random(DateTime.Now.Millisecond);//ha yeah randomized :P
			//lets begin in the lower left corner eh?(0,0)
			s.Push(new GridNode(0, 0));
			wall[0, 0] = false;
			while (true) {
				GridNode node = s.Peek();
				if (node.turnsPossible()) {
					GridNode[] nodearray = node.getRandomNext();
					wall[nodearray[0].X, nodearray[0].Y] = false;
					wall[nodearray[1].X, nodearray[1].Y] = false;
					s.Push(nodearray[1]);
					//we get the next two nodes
					//the first is a middle node from which there shouldnt start a new corridor
					//the second is added to the stack. next try will be with this node
					//i hope this will work this time...
				} else {
					s.Pop();//if this node is a dead and it will be removed
				}
				if (s.Count < 1) break;//if no nodes are free anymore we will end the generation here
			}
			
			Player.SendMessage(p, "Maze is generated. now painting...");
			//seems to be there are no more moves possible
			ushort minX = Min.X, minZ = Min.Z, maxX = Max.X, maxZ = Max.Z, y = Min.Y;
			maxX++; maxZ++;
			
			for (ushort xx = 0; xx <= width; xx++)
				for (ushort zz = 0; zz <= length; zz++)
					if (wall[xx, zz])
			{
				PlaceBlock(p, lvl, (ushort)(xx + minX + 1), y, (ushort)(zz + minZ + 1), Block.staircasefull, 0);
				PlaceBlock(p, lvl, (ushort)(xx + minX + 1), (ushort)(y + 1), (ushort)(zz + minZ + 1), Block.leaf, 0);
				PlaceBlock(p, lvl, (ushort)(xx + minX + 1), (ushort)(y + 2), (ushort)(zz + minZ + 1), Block.leaf, 0);
			}
			
			brush = new SolidBrush(Block.staircasefull, 0);
			QuadX(minX, y, minZ, y, maxZ, p, lvl, brush);
			QuadX(maxX, y, minZ, y, maxZ, p, lvl, brush);
			QuadZ(minZ, y, minX, y, maxX, p, lvl, brush);
			QuadZ(maxZ, y, minX, y, maxX, p, lvl, brush);
			
			brush = new SolidBrush(Block.leaf, 0);
			QuadX(minX, (ushort)(y + 1), minZ, (ushort)(y + 2), maxZ, p, lvl, brush);
			QuadX(maxX, (ushort)(y + 1), minZ, (ushort)(y + 2), maxZ, p, lvl, brush);
			QuadZ(minZ, (ushort)(y + 1), minX, (ushort)(y + 2), maxX, p, lvl, brush);
			QuadZ(maxZ, (ushort)(y + 1), minX, (ushort)(y + 2), maxX, p, lvl, brush);
		    
            Player.SendMessage(p, "Maze painted. Build your entrance and exit yourself");
            randomizer = 0;
        }
        
        private class GridNode
        {
            public static int maxX = 0;
            public static int maxY = 0;
            public ushort X;
            public ushort Y;
            private Random rand2 = new Random(Environment.TickCount);
            
            public GridNode[] getRandomNext() {
                byte[] r = new byte[1];
                switch (randomizer) {
                    case 0:
						RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
						rand.GetBytes(r);
						r[0] /= (255 / 4);
						break;
					default:
						r[0] = (byte)rand2.Next(4);
						break;
				}
				ushort rx = 0, ry = 0, rx2 = 0, ry2 = 0;
				switch (r[0]) {
					case 0: //go up
						if (isWall(X, Y + 2)) {
							rx = X;
							rx2 = X;
							ry = (ushort)(Y + 1);
							ry2 = (ushort)(Y + 2);
						} else {
							return this.getRandomNext();
						}
						break;
					case 1: //go down
						if (isWall(X, Y - 2)) {
							rx = X;
							rx2 = X;
							ry = (ushort)(Y - 1);
							ry2 = (ushort)(Y - 2);
						} else {
							return this.getRandomNext();
						}
						break;
					case 2: //go right
						if (isWall(X + 2, Y)) {
							rx = (ushort)(X + 1);
							rx2 = (ushort)(X + 2);
							ry = Y;
							ry2 = Y;
						} else {
							return this.getRandomNext();
						}
						break;
					case 3:
						if (isWall(X - 2, Y)) {
							//go left
							rx = (ushort)(X - 1);
							rx2 = (ushort)(X - 2);
							ry = Y;
							ry2 = Y;
						} else {
							return this.getRandomNext();
						}
						break;
				}
				return new GridNode[] { new GridNode(rx, ry), new GridNode(rx2, ry2) };
			}
			
			public bool turnsPossible() {
				return (isWall(X, Y + 2) || isWall(X, Y - 2) || isWall(X + 2, Y) || isWall(X - 2, Y));				
			}

			private bool isWall(int x, int y) {
				try {
					return wall[x, y];
				} catch (IndexOutOfRangeException) {
					return false;
				}
			}
			
			public GridNode(ushort x, ushort y) {
				X = x; Y = y;
			}
		}
	}
}
