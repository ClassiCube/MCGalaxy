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
using System.Threading;

namespace MCForge
{
    public class CmdZombieSpawn : Command
    {
        public override string name { get { return "zombiespawn"; } }
        public override string shortcut { get { return "zs"; } }
        public override string type { get { return "game"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        // This is where the magic happens, naturally.
        //TOO MANY GLOBALS -_-
        public int wavesNum;
        public int wavesLength;
        public int zombiesNum;
        public CatchPos bp;
        public int thex;
        public int they;
        public int thez;
        public bool isRandom;

        public void ZombieMob(Object person)
        {
            int xBegin = 0;
            int zBegin = 0;
            Player p = (Player)person;

            if (zombiesNum % 2 == 0 && isRandom == false)
            {
                xBegin = thex - (zombiesNum / 2);
                zBegin = thez - (zombiesNum / 2);
            }

            if (zombiesNum % 2 == 1 && isRandom == false)
            {
                xBegin = thex - ((zombiesNum - 1) / 2);
                zBegin = thez - ((zombiesNum - 1) / 2);
            }


            Command.all.Find("say").Use(p, "&aInitiating zombie attack!");
            Command.all.Find("say").Use(p, "&a" + wavesNum + " wave(s)");
            Command.all.Find("say").Use(p, "&a" + wavesLength + " second(s) each wave");
            for (int num = 1; num <= wavesNum; num++)
            {
                if (isRandom == true)
                    randomZombies(p);
                else
                    placedZombies(p, xBegin, zBegin);
               
                Command.all.Find("say").Use(p, "&aZombie wave # " + num);
                Thread.Sleep(wavesLength*1000);
            }
            Command.all.Find("say").Use(p, "&aZombie attack is over.");
        }

        public void randomZombies(Player p)
        {
            Random randomCoord = new Random();
            int ranx = 0;
            int rany = 0;
            int ranz = 0;

            for (int k = 0; k < zombiesNum; k++)
            {
                ranx = randomCoord.Next(0, p.level.width);
                rany = randomCoord.Next((p.level.height / 2), p.level.height);
                ranz = randomCoord.Next(0, p.level.depth);

                Command.all.Find("place").Use(p, "zombie " + ranx + " " + rany + " " + ranz);
            }
        }

        public void placedZombies(Player p, int xBegin, int zBegin)
        {
            for (int j = xBegin; j < xBegin + zombiesNum; j++)
            {
                for (int k = zBegin; k < zBegin + zombiesNum; k++)
                {
                    Command.all.Find("place").Use(p, "zombie " + j + " " + they + " " + k);
                }
            }
        }

        public override void Use(Player theP, string message)
        {
            int number = message.Split(' ').Length;
            String[] param = message.Split(' ');

            if(number == 1)
            {
                if (String.Compare(param[0], "x", true) == 0)
                {
                     Command.all.Find("replaceall").Use(theP, "zombie air");
                     Player.SendMessage(theP, "&aAll zombies have been destroyed.");
                     return;
                 }
            }

            if (number != 4) { Help(theP); return; }

            try
            {
                if (String.Compare(param[0], "r", true) == 0)
                {
                    isRandom = true;
                }
                else if (String.Compare(param[0], "d", true) == 0)
                {
                    isRandom = false;
                }
                else
                {
                    Player.SendMessage(theP, "Flag set must be 'r' or 'd'.");
                    return;
                }

                wavesNum = Int32.Parse(param[1]);
                wavesLength = Int32.Parse(param[2]);
                zombiesNum = Int32.Parse(param[3]);

                if (isRandom == false)
                {
                    CatchPos cpos;
                    cpos.x = 0;
                    cpos.y = 0;
                    cpos.z = 0;
                    theP.blockchangeObject = cpos;

                    Player.SendMessage(theP, "Place a block for center of zombie spawn.");
                    theP.ClearBlockchange();
                    theP.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
                }
                else
                {
                    Thread t = new Thread(ZombieMob);
                    t.Start(theP);
                }
               
            }
            catch (FormatException)
            {
                Player.SendMessage(theP, "&4All parameters must be numbers!");
            }
           
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            byte b = p.level.GetTile(x, y, z);
            p.SendBlockchange(x, y, z, b);
            bp = (CatchPos)p.blockchangeObject;
            thex = x; they = y + 2; thez = z; p.blockchangeObject = bp;
            Thread t = new Thread(ZombieMob);
            t.Start(p);
        }

        // This one controls what happens when you use /help [commandname].
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/zombiespawn <flag> <x> <y> <z> - Spawns waves of zombies.");
            Player.SendMessage(p, "<flag> - 'r' for random or 'd' for diameter");
            Player.SendMessage(p, "<x> - the number of waves");
            Player.SendMessage(p, "<y> - the length of the waves in seconds");
            Player.SendMessage(p, "<z> - the number of zombies spawned/diameter of spawn");
            Player.SendMessage(p, "/zombiespawn x - Destroys all zombies.");
        }

        public struct CatchPos
        {
            public ushort x, y, z;
        }
    }
}