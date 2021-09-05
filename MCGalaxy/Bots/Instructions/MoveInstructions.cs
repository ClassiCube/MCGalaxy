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

namespace MCGalaxy.Bots 
{   
    /// <summary> Causes the bot to instantly teleport to a position </summary>
    public class TeleportInstruction : BotInstruction 
    {
        public TeleportInstruction() { Name = "teleport"; }
        public Position Target;
        public byte Yaw, Pitch;

        public override bool Execute(PlayerBot bot) {
            bot.Pos = Target;
            bot.SetYawPitch(Yaw, Pitch);
            
            bot.NextInstruction();
            return true;
        }
        
        
        public override string Serialise() { return "teleport " + SerialiseArgs(); }
        protected string SerialiseArgs() {
            return Target.X + " " + Target.Y + " " + Target.Z + " " + Yaw + " " + Pitch;
        }
        
        public override void Deserialise(string value) {
            string[] args = value.SplitSpaces();
            Target.X = int.Parse(args[0]);
            Target.Y = int.Parse(args[1]);
            Target.Z = int.Parse(args[2]);
            Yaw      = byte.Parse(args[3]);
            Pitch    = byte.Parse(args[4]);
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            Target = p.Pos;
            Yaw    = p.Rot.RotY;
            Pitch  = p.Rot.HeadX;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] teleport",
            "&HCauses the bot to instantly teleport to a position.",
            "&H  Note: The position saved to the AI is your current position.",
        };
    }
    
    /// <summary> Causes the bot to gradually move to to a position. </summary>
    public sealed class WalkInstruction : TeleportInstruction 
    {
        public WalkInstruction() { Name = "walk"; }

        public override bool Execute(PlayerBot bot) {
            bot.TargetPos = Target;
            bot.movement  = true;

            if (bot.Pos.BlockX == bot.TargetPos.BlockX && bot.Pos.BlockZ == bot.TargetPos.BlockZ) {
                bot.SetYawPitch(Yaw, Pitch);
                bot.movement = false;
                bot.NextInstruction(); return false;
            }
            
            bot.AdvanceRotation(); return true;
        }
        
        
        public override string Serialise() { return "walk " + SerialiseArgs(); }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] walk",
            "&HCauses the bot to walk towards to a position.",
            "&H  Note: The position saved to the AI is your current position.",
        };
    }
    
    /// <summary> Causes the bot to begin jumping </summary>
    public sealed class JumpInstruction : BotInstruction 
    {
        public JumpInstruction() { Name = "jump"; }

        public override bool Execute(PlayerBot bot) {
            if (bot.curJump <= 0) bot.curJump = 1;
            bot.NextInstruction(); return false;
        }
        
        
        public override string Serialise() { return "jump"; }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "&T/BotAI add [name] jump",
            "&HCauses the bot to perform a jump.",
            "&H  Note bots can also do other instructions while jumping",
            "&H  (e.g. For a \"jump\" then a \"walk\" instruction, the bot will jump while also walking",
        };
    }
    
    /// <summary> Causes the bot to change how fast it moves </summary>
    public sealed class SpeedInstruction : BotInstruction 
    {
        public short Speed = 100;

        public override bool Execute(PlayerBot bot) {
            bot.movementSpeed = (int)Math.Round(3m * Speed / 100m);
            if (bot.movementSpeed == 0) bot.movementSpeed = 1;
            bot.NextInstruction(); return false;
        }
        
        
        public override string Serialise() { return "speed " + Speed; }
        
        public override void Deserialise(string value) {
            Speed = short.Parse(value);
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            string time = args.Length > 3 ? args[3] : "10";
            Speed = short.Parse(time);
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
