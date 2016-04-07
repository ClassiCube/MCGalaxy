/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

        public override void Use(Player p, string message) {
            CatchPos cpos = default(CatchPos);
            message = message.ToLower();
            cpos.extraInfo = "";
            if (message != "" && !ParseArgs(p, message, ref cpos)) return;

            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place two blocks to determine the edges.");
            p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        bool ParseArgs(Player p, string args, ref CatchPos cpos) {
            string[] parts = args.Split(' ');
            if (parts.Length % 2 == 1) {
                Player.SendMessage(p, "Number of parameters must be even");
                Help(p); return false;
            }

            for (int i = 0; i < parts.Length; i++) {
                string s = parts[i];
                if (i % 2 != 0) {
                    int value;
                    if (!int.TryParse(s, out value)) {
                        Player.SendMessage(p, "/rp [type1] [num] [type2] [num]..."); return false;
                    }
                    if (value < 0) { Player.SendMessage(p, "Values must be above 0"); return false; }
                    continue;
                }
                
                switch (s) {
                    case "drop":
                    case "explode":
                    case "dissipate":
                    case "wait":
                    case "rainbow":
                        break;
                        
                    case "revert":
                        byte block = Block.Byte(parts[i + 1]);
                        if (block == Block.Zero) { Player.SendMessage(p, "Invalid block type."); return false; }
                        parts[i + 1] = block.ToString();
                        break;
                    default:
                        Player.SendMessage(p, s + " type is not supported.");
                        return false;
                }
            }
            cpos.extraInfo = string.Join(" ", parts);
            return true;
        }
        
        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos bp = (CatchPos)p.blockchangeObject;
            bp.x = x; bp.y = y; bp.z = z; p.blockchangeObject = bp;
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange2);
        }
        
        void Blockchange2(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            RevertAndClearState(p, x, y, z);
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            List<int> buffer = new List<int>();
            
            for (ushort yy = Math.Min(cpos.y, y); yy <= Math.Max(cpos.y, y); ++yy)
                for (ushort zz = Math.Min(cpos.z, z); zz <= Math.Max(cpos.z, z); ++zz)
                    for (ushort xx = Math.Min(cpos.x, x); xx <= Math.Max(cpos.x, x); ++xx)                        
            {
                int index = p.level.PosToInt(xx, yy, zz);
                if (index >= 0 && p.level.blocks[index] != Block.air)
                    buffer.Add(index);
            }

            if (cpos.extraInfo == "") {
                if (buffer.Count > Server.rpNormLimit) {
                    Player.SendMessage(p, "Cannot restart more than " + Server.rpNormLimit + " blocks.");
                    Player.SendMessage(p, "Tried to restart " + buffer.Count + " blocks.");
                    return;
                }
            } else if (buffer.Count > Server.rpLimit) {
                Player.SendMessage(p, "Tried to add physics to " + buffer.Count + " blocks.");
                Player.SendMessage(p, "Cannot add physics to more than " + Server.rpLimit + " blocks.");
                return;
            }

            foreach (int index in buffer)
                p.level.AddCheck(index, true, cpos.extraInfo);
            Player.SendMessage(p, "Activated " + buffer.Count + " blocks.");
            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public ushort x, y, z; public string extraInfo; }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/restartphysics ([type] [num]) ([type2] [num2]) (...) - Restarts every physics block in an area");
            Player.SendMessage(p, "[type] will set custom physics for selected blocks");
            Player.SendMessage(p, "Possible [types]: drop, explode, dissipate, wait, rainbow, revert");
            Player.SendMessage(p, "/rp revert takes block names");
        }
    }
}
