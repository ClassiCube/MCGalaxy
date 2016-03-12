/*
	Copyright 2011 MCForge
	
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
namespace MCGalaxy
{
    public sealed class CtfTeam
    {
        public char color;
        public int points = 0;
        public ushort[] flagBase = { 0, 0, 0 };
        public ushort[] flagLocation = { 0, 0, 0 };
        public List<Spawn> spawns = new List<Spawn>();
        public List<Player> players = new List<Player>();
        public Level mapOn;
        public bool flagishome;
        public bool spawnset;
        public bool flagmoved;
        public string teamstring = "";
        public Player holdingFlag = null;
        public CatchPos tempFlagblock;
        public CatchPos tfb;
        public int ftcount = 0;

        public void AddMember(Player p)
        {
            if (p.team != this)
            {
                if (p.team != null) { p.team.RemoveMember(p); }
                p.team = this;
                Player.GlobalDespawn(p, false);
                //p.CTFtempcolor = p.color;
                //p.CTFtempprefix = p.prefix;
                p.color = "&" + color;
                //p.carryingFlag = false;
                p.hasflag = null;
                p.prefix = p.color + "[" + Colors.Name("&" + color).ToUpper() + "] ";
                players.Add(p);
                mapOn.ChatLevel(p.color + p.prefix + p.name + Server.DefaultColor + " has joined the " + teamstring + ".");
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            }
        }

        public void RemoveMember(Player p)
        {
            if (p.team == this)
            {
                p.team = null;
                Player.GlobalDespawn(p, false);
                //p.color = p.CTFtempcolor;
                //p.prefix = p.CTFtempprefix;
                //p.carryingFlag = false;
                p.hasflag = null;
                players.Remove(p);
                mapOn.ChatLevel(p.color + p.prefix + p.name + Server.DefaultColor + " has left the " + teamstring + ".");
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            }
        }

        public void SpawnPlayer(Player p)
        {
            //p.spawning = true;
            if (spawns.Count != 0)
            {
                Random random = new Random();
                int rnd = random.Next(0, spawns.Count);
                ushort x, y, z, rotx;

                x = spawns[rnd].x;
                y = spawns[rnd].y;
                z = spawns[rnd].z;

                ushort x1 = (ushort)((0.5 + x) * 32);
                ushort y1 = (ushort)((1 + y) * 32);
                ushort z1 = (ushort)((0.5 + z) * 32);
                rotx = spawns[rnd].rotx;
                p.SendSpawn((byte)0xFF, p.name, x1, y1, z1, (byte)rotx, 0);
                //p.health = 100;
            }
            else
            {
                ushort x = (ushort)((0.5 + mapOn.spawnx) * 32);
                ushort y = (ushort)((1 + mapOn.spawny) * 32);
                ushort z = (ushort)((0.5 + mapOn.spawnz) * 32);
                ushort rotx = mapOn.rotx;
                ushort roty = mapOn.roty;
                p.SendSpawn(0xFF, p.name, x, y, z, (byte)rotx, (byte)roty);
            }
            //p.spawning = false;
        }

        public void AddSpawn(ushort x, ushort y, ushort z, ushort rotx, ushort roty)
        {
            Spawn workSpawn = new Spawn();
            workSpawn.x = x;
            workSpawn.y = y;
            workSpawn.z = z;
            workSpawn.rotx = rotx;
            workSpawn.roty = roty;

            spawns.Add(workSpawn);
        }

        public void Drawflag()
        {
            
            ushort x = flagLocation[0];
            ushort y = flagLocation[1];
            ushort z = flagLocation[2];

            if (mapOn.GetTile(x, (ushort)(y - 1), z) == Block.air)
            {
                flagLocation[1] = (ushort)(flagLocation[1] - 1);
            }

            mapOn.Blockchange(tfb.x, tfb.y, tfb.z, tfb.type);
            mapOn.Blockchange(tfb.x, (ushort)(tfb.y + 1), tfb.z, Block.air);
            mapOn.Blockchange(tfb.x, (ushort)(tfb.y + 2), tfb.z, Block.air);

            if (holdingFlag == null)
            {
                //DRAW ON GROUND SHIT HERE

                tfb.type = mapOn.GetTile(x, y, z);

                if (mapOn.GetTile(x, y, z) != Block.flagbase) { mapOn.Blockchange(x, y, z, Block.flagbase); }
                if (mapOn.GetTile(x, (ushort)(y + 1), z) != Block.mushroom) { mapOn.Blockchange(x, (ushort)(y + 1), z, Block.mushroom); }
                if (mapOn.GetTile(x, (ushort)(y + 2), z) != GetColorBlock(color)) { mapOn.Blockchange(x, (ushort)(y + 2), z, GetColorBlock(color)); }

                tfb.x = x;
                tfb.y = y;
                tfb.z = z;

            }
            else
            {
                //DRAW ON PLAYER HEAD
                x = (ushort)(holdingFlag.pos[0] / 32);
                y = (ushort)(holdingFlag.pos[1] / 32 + 3);
                z = (ushort)(holdingFlag.pos[2] / 32);

                if (tempFlagblock.x == x && tempFlagblock.y == y && tempFlagblock.z == z) { return; }


                mapOn.Blockchange(tempFlagblock.x, tempFlagblock.y, tempFlagblock.z, tempFlagblock.type);

                tempFlagblock.type = mapOn.GetTile(x, y, z);

                mapOn.Blockchange(x, y, z, GetColorBlock(color));

                tempFlagblock.x = x;
                tempFlagblock.y = y;
                tempFlagblock.z = z;
            }
            

        }

        public static byte GetColorBlock(char color)
        {
            if (color == '2')
                return Block.green;
            if (color == '5')
                return Block.purple;
            if (color == '8')
                return Block.darkgrey;
            if (color == '9')
                return Block.blue;
            if (color == 'c')
                return Block.red;
            if (color == 'e')
                return Block.yellow;
            if (color == 'f')
                return Block.white;
            else
                return Block.air;
        }

        public struct CatchPos { public ushort x, y, z; public byte type; }
        public struct Spawn { public ushort x, y, z, rotx, roty; }
    }
}