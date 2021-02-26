/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;

namespace MCGalaxy.Bots {
    
    /// <summary> Causes the bot to instantly teleport to a position. </summary>
    public class TeleportInstruction : BotInstruction {
        public TeleportInstruction() { Name = "teleport"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Coords coords = (Coords)data.Metadata;
            bot.Pos = new Position(coords.X, coords.Y, coords.Z);
            bot.SetYawPitch(coords.RotX, coords.RotY);
            
            bot.NextInstruction();
            return true;
        }
        
        public override InstructionData Parse(string[] args) {
            Coords coords;
            coords.X = int.Parse(args[1]);
            coords.Y = int.Parse(args[2]);
            coords.Z = int.Parse(args[3]);
            coords.RotX = byte.Parse(args[4]);
            coords.RotY = byte.Parse(args[5]);
            
            InstructionData data = default(InstructionData);
            data.Metadata = coords;
            return data;
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            w.WriteLine(Name + " " + p.Pos.X + " " + p.Pos.Y + " " + p.Pos.Z + " " + p.Rot.RotY + " " + p.Rot.HeadX);
        }
        
        protected struct Coords {
            public int X, Y, Z;
            public byte RotX, RotY;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] teleport",
            "&HCauses the bot to instantly teleport to a position.",
            "&H  Note: The position saved to the AI is your current position.",
        };
    }
    
    /// <summary> Causes the bot to gradually move to to a position. </summary>
    public sealed class WalkInstruction : TeleportInstruction {
        public WalkInstruction() { Name = "walk"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Coords target = (Coords)data.Metadata;
            bot.TargetPos = new Position(target.X, target.Y, target.Z);
            bot.movement = true;

            if (bot.Pos.BlockX == bot.TargetPos.BlockX && bot.Pos.BlockZ == bot.TargetPos.BlockZ) {
                bot.SetYawPitch(target.RotX, target.RotY);
                bot.movement = false;
                bot.NextInstruction(); return false;
            }
            
            bot.AdvanceRotation(); return true;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] walk",
            "&HCauses the bot to walk towards to a position.",
            "&H  Note: The position saved to the AI is your current position.",
        };
    }
    
    /// <summary> Causes the bot to begin jumping. </summary>
    public sealed class JumpInstruction : BotInstruction {
        public JumpInstruction() { Name = "jump"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            if (bot.curJump <= 0) bot.curJump = 1;
            bot.NextInstruction(); return false;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] jump",
            "&HCauses the bot to perform a jump.",
            "&H  Note bots can also do other instructions while jumping",
            "&H  (e.g. For a \"jump\" then a \"walk\" instruction, the bot will jump while also walking",
        };
    }
    
    /// <summary> Causes the bot to change how fast it moves. </summary>
    public sealed class SpeedInstruction : BotInstruction {
        public SpeedInstruction() { Name = "speed"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            bot.movementSpeed = (int)Math.Round(3m * (short)data.Metadata / 100m);
            if (bot.movementSpeed == 0) bot.movementSpeed = 1;
            bot.NextInstruction(); return false;
        }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            data.Metadata = short.Parse(args[1]);
            return data;
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            string time = args.Length > 3 ? args[3] : "10";
            w.WriteLine(Name + " " + short.Parse(time));
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] speed [percentage]",
            "&HSets how fast the bot moves, relative to its normal speed.",
            "&H  100 means it moves at normal speed",
            "&H  50 means it moves at half speed",
        };
    }
}
