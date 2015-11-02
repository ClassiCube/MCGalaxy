/*
    Written by Jack1312
  
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
namespace MCGalaxy.Commands
{
    public sealed class CmdExplode : Command
    {
        public override string name { get { return "explode"; } }
        public override string shortcut { get { return "ex"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdExplode() { }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/explode - Satisfying all your exploding needs :)");
            Player.SendMessage(p, "/explode me - Explodes at your location");
            Player.SendMessage(p, "/explode [Player] - Explode the specified player");
            Player.SendMessage(p, "/explode [X] [Y] [Z] - Explode at the specified co-ordinates");

        }
        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            int number = message.Split(' ').Length;
            if (number > 3) { Player.SendMessage(p, "What are you on about?"); return; }
            if (message == "me")
            {
                if (p.level.physics <3)
                {
                    Player.SendMessage(p, "The physics on this level are not sufficient for exploding!");
                    return;
                }
                Command.all.Find("explode").Use(p, p.name);
                return;
            }
            if (number == 1)
            {
                if (p != null)
                {
                    if (p.level.physics < 3)
                    {
                        Player.SendMessage(p, "The physics on this level are not sufficient for exploding!");
                        return;
                    }
                        Player who = Player.Find(message);
                        ushort x = (ushort)(who.pos[0] / 32);
                        ushort y = (ushort)(who.pos[1] / 32);
                        ushort z = (ushort)(who.pos[2] / 32);
                        p.level.MakeExplosion(x, y, z, 1);
                        Player.SendMessage(p, who.color + who.DisplayName + Server.DefaultColor + " has been exploded!");
                        return;
                }
                Player.SendMessage(p, "The specified player does not exist!");
                return;
            }
            if (number == 3)
            {
                {
                    byte b = Block.Zero;
                    ushort x = 0; ushort y = 0; ushort z = 0;

                    x = (ushort)(p.pos[0] / 32);
                    y = (ushort)((p.pos[1] / 32) - 1);
                    z = (ushort)(p.pos[2] / 32);

                    try
                    {
                        switch (message.Split(' ').Length)
                        {
                            case 0: b = Block.rock; break;
                            case 1: b = Block.Byte(message); break;
                            case 3:
                                x = Convert.ToUInt16(message.Split(' ')[0]);
                                y = Convert.ToUInt16(message.Split(' ')[1]);
                                z = Convert.ToUInt16(message.Split(' ')[2]);
                                break;
                            case 4:
                                b = Block.Byte(message.Split(' ')[0]);
                                x = Convert.ToUInt16(message.Split(' ')[1]);
                                y = Convert.ToUInt16(message.Split(' ')[2]);
                                z = Convert.ToUInt16(message.Split(' ')[3]);
                                break;
                            default: Player.SendMessage(p, "Invalid parameters"); return;
                        }
                    }
                    catch { Player.SendMessage(p, "Invalid parameters"); return; }

                    Level level = p.level;

                    if (y >= p.level.depth) y = (ushort)(p.level.depth - 1);

                    if (p.level.physics < 3)
                    {
                        Player.SendMessage(p, "The physics on this level are not sufficient for exploding!");
                        return;
                    }
                        p.level.MakeExplosion(x, y, z, 1);
                        Player.SendMessage(p, "An explosion was made at (" + x + ", " + y + ", " + z + ").");
                }
            }
        }
    }
}
