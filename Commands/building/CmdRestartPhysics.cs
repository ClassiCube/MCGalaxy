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
namespace MCGalaxy.Commands
{
    public sealed class CmdRestartPhysics : Command
    {
        public override string name { get { return "restartphysics"; } }
        public override string shortcut { get { return "rp"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRestartPhysics() { }

        public override void Use(Player p, string message)
        {
            CatchPos cpos;
            cpos.x = 0; cpos.y = 0; cpos.z = 0;

            message = message.ToLower();
            cpos.extraInfo = "";

            if (message != "")
            {
                int currentLoop = 0; string[] storedArray; bool skip = false;

            retry: foreach (string s in message.Split(' '))
                {
                    if (currentLoop % 2 == 0)
                    {
                        switch (s)
                        {
                            case "drop":
                            case "explode":
                            case "dissipate":
                            case "finite":
                            case "wait":
                            case "rainbow":
                                break;
                            case "revert":
                                if (skip) break;
                                storedArray = message.Split(' ');
                                try
                                {
                                    storedArray[currentLoop + 1] = Block.Byte(message.Split(' ')[currentLoop + 1].ToString().ToLower()).ToString();
                                    if (storedArray[currentLoop + 1].ToString() == "255") throw new OverflowException();
                                }
                                catch { Player.SendMessage(p, "Invalid block type."); return; }

                                message = string.Join(" ", storedArray);
                                skip = true; currentLoop = 0;

                                goto retry;
                            default:
                                Player.SendMessage(p, s + " is not supported."); return;
                        }
                    }
                    else
                    {
                        try
                        {
                            if (int.Parse(s) < 1) { Player.SendMessage(p, "Values must be above 0"); return; }
                        }
                        catch { Player.SendMessage(p, "/rp [text] [num] [text] [num]"); return; }
                    }

                    currentLoop++;
                }

                if (currentLoop % 2 != 1) cpos.extraInfo = message;
                else { Player.SendMessage(p, "Number of parameters must be even"); Help(p); return; }
            }

            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/restartphysics ([type] [num]) ([type2] [num2]) (...) - Restarts every physics block in an area");
            Player.SendMessage(p, "[type] will set custom physics for selected blocks");
            Player.SendMessage(p, "Possible [types]: drop, explode, dissipate, finite, wait, rainbow, revert");
            Player.SendMessage(p, "/rp revert takes block names");
        }
        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        public void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type)
        {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            List<CatchPos> buffer = new List<CatchPos>();
            CatchPos pos = new CatchPos();
            //int totalChecks = 0;

            //if (Math.Abs(cpos.x - x) * Math.Abs(cpos.y - y) * Math.Abs(cpos.z - z) > 8000) { Player.SendMessage(p, "Tried to restart too many blocks. You may only restart 8000"); return; }

            for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)
            {
                for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                {
                    for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    {
                        if (p.level.GetTile(xx, yy, zz) != Block.air)
                        {
                            pos.x = xx; pos.y = yy; pos.z = zz;
                            pos.extraInfo = cpos.extraInfo;
                            buffer.Add(pos);
                        }
                    }
                }
            }

            try
            {
                if (cpos.extraInfo == "")
                {
                    if (buffer.Count > Server.rpNormLimit)
                    {
                        Player.SendMessage(p, "Cannot restart more than " + Server.rpNormLimit + " blocks.");
                        Player.SendMessage(p, "Tried to restart " + buffer.Count + " blocks.");
                        return;
                    }
                }
                else
                {
                    if (buffer.Count > Server.rpLimit)
                    {
                        Player.SendMessage(p, "Tried to add physics to " + buffer.Count + " blocks.");
                        Player.SendMessage(p, "Cannot add physics to more than " + Server.rpLimit + " blocks.");
                        return;
                    }
                }
            }
            catch { return; }

            foreach (CatchPos pos1 in buffer)
            {
                p.level.AddCheck(p.level.PosToInt(pos1.x, pos1.y, pos1.z), pos1.extraInfo, true);
            }

            Player.SendMessage(p, "Activated " + buffer.Count + " blocks.");
            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public ushort x, y, z; public string extraInfo; }
    }
}
