/*
	Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
   
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
namespace MCGalaxy.Commands
{
    public sealed class CmdFixGrass : Command
    {
        public override string name { get { return "fixgrass"; } }
        public override string shortcut { get { return "fg"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdFixGrass() { }

        public override void Use(Player p, string message)
        {
            int totalFixed = 0;

            switch (message.ToLower())
            {
                case "":
                    for (int i = 0; i < p.level.blocks.Length; i++)
                    {
                        try
                        {
                            ushort x, y, z;
                            p.level.IntToPos(i, out x, out y, out z);

                            if (p.level.blocks[i] == Block.dirt)
                            {
                                if (Block.LightPass(p.level.blocks[p.level.IntOffset(i, 0, 1, 0)]))
                                {
                                    p.level.Blockchange(p, x, y, z, Block.grass);
                                    totalFixed++;
                                }
                            }
                            else if (p.level.blocks[i] == Block.grass)
                            {
                                if (!Block.LightPass(p.level.blocks[p.level.IntOffset(i, 0, 1, 0)]))
                                {
                                    p.level.Blockchange(p, x, y, z, Block.dirt);
                                    totalFixed++;
                                }
                            }
                        }
                        catch { }
                    } break;
                case "light":
                    for (int i = 0; i < p.level.blocks.Length; i++)
                    {
                        try
                        {
                            ushort x, y, z; bool skipMe = false;
                            p.level.IntToPos(i, out x, out y, out z);

                            if (p.level.blocks[i] == Block.dirt)
                            {
                                for (int iL = 1; iL < (p.level.depth - y); iL++)
                                {
                                    if (!Block.LightPass(p.level.blocks[p.level.IntOffset(i, 0, iL, 0)]))
                                    {
                                        skipMe = true; break;
                                    }
                                }
                                if (!skipMe)
                                {
                                    p.level.Blockchange(p, x, y, z, Block.grass);
                                    totalFixed++;
                                }
                            }
                            else if (p.level.blocks[i] == Block.grass)
                            {
                                for (int iL = 1; iL < (p.level.depth - y); iL++)
                                {
                                    // Used to change grass to dirt only if all the upper blocks weren't Lightpass.
                                    if (!Block.LightPass(p.level.blocks[p.level.IntOffset(i, 0, iL, 0)]))
                                    {
                                        skipMe = true; break;
                                    }
                                }
                                if (skipMe)
                                {
                                    p.level.Blockchange(p, x, y, z, Block.dirt);
                                    totalFixed++;
                                }
                            }
                        }
                        catch { }
                    } break;
                case "grass":
                    for (int i = 0; i < p.level.blocks.Length; i++)
                    {
                        try
                        {
                            ushort x, y, z;
                            p.level.IntToPos(i, out x, out y, out z);

                            if (p.level.blocks[i] == Block.grass)
                                if (!Block.LightPass(p.level.blocks[p.level.IntOffset(i, 0, 1, 0)]))
                                {
                                    p.level.Blockchange(p, x, y, z, Block.dirt);
                                    totalFixed++;
                                }
                        }
                        catch { }
                    } break;
                case "dirt":
                    for (int i = 0; i < p.level.blocks.Length; i++)
                    {
                        try
                        {
                            ushort x, y, z;
                            p.level.IntToPos(i, out x, out y, out z);

                            if (p.level.blocks[i] == Block.dirt)
                                if (Block.LightPass(p.level.blocks[p.level.IntOffset(i, 0, 1, 0)]))
                                {
                                    p.level.Blockchange(p, x, y, z, Block.grass);
                                    totalFixed++;
                                }
                        }
                        catch { }
                    } break;
                default:
                    Help(p);
                    return;
            }

            Player.SendMessage(p, "Fixed " + totalFixed + " blocks.");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/fixgrass <type> - Fixes grass based on type");
            Player.SendMessage(p, "<type> as \"\": Any grass with something on top is made into dirt, dirt with nothing on top is made grass");
            Player.SendMessage(p, "<type> as \"light\": Only dirt/grass in sunlight becomes grass");
            Player.SendMessage(p, "<type> as \"grass\": Only turns grass to dirt when under stuff");
            Player.SendMessage(p, "<type> as \"dirt\": Only turns dirt with nothing on top to grass");
        }
    }
}
