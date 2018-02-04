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
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Building {
    public sealed class CmdRestartPhysics : Command {
        public override string name { get { return "RestartPhysics"; } }
        public override string shortcut { get { return "rp"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            PhysicsArgs extraInfo = default(PhysicsArgs);
            message = message.ToLower();
            if (message.Length > 0 && !ParseArgs(p, message, ref extraInfo)) return;

            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, "Selecting region for %SRestart physics", extraInfo, DoRestart);
        }
        
        bool ParseArgs(Player p, string message, ref PhysicsArgs args) {
            string[] parts = message.SplitSpaces();
            if (parts.Length % 2 == 1) {
                Player.Message(p, "Number of parameters must be even");
                Help(p); return false;
            }
            byte type = 0, value = 0;
            bool isExt = false;
            
            if (parts.Length >= 2) {
                if (!Parse(p, parts[0], parts[1], ref type, ref value, ref isExt)) return false;
                args.Type1 = type; args.Value1 = value;
            }
            if (parts.Length >= 4) {
                if (!Parse(p, parts[2], parts[3], ref type, ref value, ref isExt)) return false;
                args.Type2 = type; args.Value2 = value;
            }
            if (parts.Length >= 6) {
                Player.Message(p, "You can only use up to two types of physics."); return false;
            }
            
            args.ExtBlock = isExt;
            return true;
        }
        
        bool Parse(Player p, string name, string arg, ref byte type, ref byte value, ref bool isExt) {
            if (name == "revert") {
                ushort block;
                if (!CommandParser.GetBlock(p, arg, out block)) return false;

                type = PhysicsArgs.Revert; value = block.RawID;
                isExt = block.BlockID == Block.custom_block;
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
        
        bool DoRestart(Player p, Vec3S32[] m, object state, ushort block) {
            PhysicsArgs extraInfo = (PhysicsArgs)state;
            List<int> buffer = new List<int>();
            int index;
            
            for (int y = Math.Min(m[0].Y, m[1].Y); y <= Math.Max(m[0].Y, m[1].Y); y++)
                for (int z = Math.Min(m[0].Z, m[1].Z); z <= Math.Max(m[0].Z, m[1].Z); z++)
                    for (int x = Math.Min(m[0].X, m[1].X); x <= Math.Max(m[0].X, m[1].X); x++)
            {
                if (!p.level.IsAirAt((ushort)x, (ushort)y, (ushort)z, out index)) {
                    buffer.Add(index);
                }
            }

            if (extraInfo.Raw == 0) {
                if (buffer.Count > ServerConfig.PhysicsRestartNormLimit) {
                    Player.Message(p, "Cannot restart more than " + ServerConfig.PhysicsRestartNormLimit + " blocks.");
                    Player.Message(p, "Tried to restart " + buffer.Count + " blocks.");
                    return false;
                }
            } else if (buffer.Count > ServerConfig.PhysicsRestartLimit) {
                Player.Message(p, "Tried to add physics to " + buffer.Count + " blocks.");
                Player.Message(p, "Cannot add physics to more than " + ServerConfig.PhysicsRestartLimit + " blocks.");
                return false;
            }

            foreach (int index1 in buffer) {
                p.level.AddCheck(index1, true, extraInfo);
            }
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
