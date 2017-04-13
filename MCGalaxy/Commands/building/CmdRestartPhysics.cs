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
using MCGalaxy.Blocks.Physics;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdRestartPhysics : Command {
        public override string name { get { return "restartphysics"; } }
        public override string shortcut { get { return "rp"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdRestartPhysics() { }

        public override void Use(Player p, string message) {
            PhysicsArgs extraInfo = default(PhysicsArgs);
            message = message.ToLower();
            if (message != "" && !ParseArgs(p, message, ref extraInfo)) return;

            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, extraInfo, DoRestart);
        }
        
        bool ParseArgs(Player p, string message, ref PhysicsArgs extraInfo) {
            string[] parts = message.SplitSpaces();
            if (parts.Length % 2 == 1) {
                Player.Message(p, "Number of parameters must be even");
                Help(p); return false;
            }
            byte type = 0, value = 0;
            
            if (parts.Length >= 2) {
                if (!Parse(p, parts[0], parts[1], ref type, ref value)) return false;
                extraInfo.Type1 = type; extraInfo.Value1 = value;
            }
            if (parts.Length >= 4) {
                if (!Parse(p, parts[2], parts[3], ref type, ref value)) return false;
                extraInfo.Type2 = type; extraInfo.Value2 = value;
            }
            if (parts.Length >= 6) {
                Player.Message(p, "You can only use up to two types of physics."); return false;
            }
            return true;
        }
        
        bool Parse(Player p, string name, string arg, ref byte type, ref byte value) {
            if (name == "revert") {
                byte block = Block.Byte(arg);
                if (block == Block.Invalid) { Player.Message(p, "Invalid block type."); return false; }
                type = PhysicsArgs.Revert; value = block;
                return true;
            }
            
            byte temp = 0;
            if (!CommandParser.GetByte(p, arg, "Value", ref temp)) return false;
            value = (byte)temp;
            
            switch (name) {
                case "drop": type = PhysicsArgs.Drop; return true;
                case "explode": type = PhysicsArgs.Explode; return true;
                case "dissipate": type = PhysicsArgs.Dissipate; return true;
                case "wait": type = PhysicsArgs.Wait; return true;
                case "rainbow": type = PhysicsArgs.Rainbow; return true;
            }
            Player.Message(p, name + " type is not supported.");
            return false;
        }
        
        bool DoRestart(Player p, Vec3S32[] m, object state, byte type, byte extType) {
            PhysicsArgs extraInfo = (PhysicsArgs)state;
            List<int> buffer = new List<int>();
            
            for (int y = Math.Min(m[0].Y, m[1].Y); y <= Math.Max(m[0].Y, m[1].Y); y++)
                for (int z = Math.Min(m[0].Z, m[1].Z); z <= Math.Max(m[0].Z, m[1].Z); z++)
                    for (int x = Math.Min(m[0].X, m[1].X); x <= Math.Max(m[0].X, m[1].X); x++)
            {
                int index = p.level.PosToInt((ushort)x, (ushort)y, (ushort)z);
                if (index >= 0 && p.level.blocks[index] != Block.air)
                    buffer.Add(index);
            }

            if (extraInfo.Raw == 0) {
                if (buffer.Count > Server.rpNormLimit) {
                    Player.Message(p, "Cannot restart more than " + Server.rpNormLimit + " blocks.");
                    Player.Message(p, "Tried to restart " + buffer.Count + " blocks.");
                    return false;
                }
            } else if (buffer.Count > Server.rpLimit) {
                Player.Message(p, "Tried to add physics to " + buffer.Count + " blocks.");
                Player.Message(p, "Cannot add physics to more than " + Server.rpLimit + " blocks.");
                return false;
            }

            foreach (int index in buffer)
                p.level.AddCheck(index, true, extraInfo);
            Player.Message(p, "Activated " + buffer.Count + " blocks.");
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/restartphysics ([type] [num]) ([type2] [num2]) - Restarts every physics block in an area");
            Player.Message(p, "[type] will set custom physics for selected blocks");
            Player.Message(p, "Possible [types]: drop, explode, dissipate, wait, rainbow, revert");
            Player.Message(p, "/rp revert takes block names");
        }
    }
}
