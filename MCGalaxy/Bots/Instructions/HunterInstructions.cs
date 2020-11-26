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
using MCGalaxy.Maths;

namespace MCGalaxy.Bots {
    
    /// <summary> Causes the bot to move towards the closest player, within a defined search radius. </summary>
    public sealed class HuntInstruction : BotInstruction {
        public HuntInstruction() { Name = "hunt"; }
        
        public override bool Execute(PlayerBot bot, InstructionData data) {
            int search = 75;
            if (data.Metadata != null) search = (ushort)data.Metadata;
            Player closest = ClosestPlayer(bot, search);
            
            if (closest == null) { bot.NextInstruction(); return false; }
            bool overlapsPlayer = MoveTowards(bot, closest);
            if (overlapsPlayer) { bot.NextInstruction(); return false; }
            return true;
        }
        
        internal static Player ClosestPlayer(PlayerBot bot, int search) {
            int maxDist = search * 32;
            Player[] players = PlayerInfo.Online.Items;
            Player closest = null;
            
            foreach (Player p in players) {
                if (p.level != bot.level || p.invincible || p.hidden) continue;
                
                int dx = p.Pos.X - bot.Pos.X, dy = p.Pos.Y - bot.Pos.Y, dz = p.Pos.Z - bot.Pos.Z;
                int playerDist = Math.Abs(dx) + Math.Abs(dy) + Math.Abs(dz);
                if (playerDist >= maxDist) continue;
                
                closest = p;
                maxDist = playerDist;
            }
            return closest;
        }
        
        static bool MoveTowards(PlayerBot bot, Player p) {
            int dx = p.Pos.X - bot.Pos.X, dy = p.Pos.Y - bot.Pos.Y, dz = p.Pos.Z - bot.Pos.Z;
            bot.TargetPos = p.Pos;
            bot.movement = true;
            
            Vec3F32 dir = new Vec3F32(dx, dy, dz);
            dir = Vec3F32.Normalise(dir);
            Orientation rot = bot.Rot;
            DirUtils.GetYawPitch(dir, out rot.RotY, out rot.HeadX);
            
            dx = Math.Abs(dx); dy = Math.Abs(dy); dz = Math.Abs(dz);
            
            // If we are very close to a player, switch from trying to look
            // at them to just facing the opposite direction to them
            if (dx < 4 && dz < 4) {
                rot.RotY = (byte)(p.Rot.RotY + 128);
            }
            bot.Rot = rot;
            
            return dx <= 8 && dy <= 16 && dz <= 8;
        }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            if (args.Length > 1)
                data.Metadata = ushort.Parse(args[1]);
            return data;
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            if (args.Length > 3) {
                w.WriteLine(Name + " " + ushort.Parse(args[3]));
            } else {
                w.WriteLine(Name);
            }
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { 
            "%T/BotAI add [name] hunt <radius>",
            "%HCauses the bot to move towards the closest player in the search radius.",
            "%H  <radius> defaults to 75 blocks.",
        };
    }
    
    /// <summary> Causes the bot to kill nearby players. </summary>
    public sealed class KillInstruction : BotInstruction {
        public KillInstruction() { Name = "kill"; }

        public override bool Execute(PlayerBot bot, InstructionData data) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != bot.level || p.invincible) continue;
                
                int dx = Math.Abs(bot.Pos.X - p.Pos.X);
                int dy = Math.Abs(bot.Pos.Y - p.Pos.Y);
                int dz = Math.Abs(bot.Pos.Z - p.Pos.Z);
                
                if (dx <= 8 && dy <= 16 && dz <= 8) {
                    string msg = bot.DeathMessage;
                    if (msg == null) msg = "@p %Swas &cterminated.";
                    p.HandleDeath(Block.Cobblestone, msg);
                }
            }
            bot.NextInstruction(); return true;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] {
            "%T/BotAI add [name] kill",
            "%HCauses the bot to kill any players it is touching.",
        };
    }
    
    public sealed class StareInstruction : BotInstruction {
        public StareInstruction() { Name = "stare"; }
        
        public override bool Execute(PlayerBot bot, InstructionData data) {
            int search = 20000;
            if (data.Metadata != null) search = (ushort)data.Metadata;
            Player closest = HuntInstruction.ClosestPlayer(bot, search);
            
            if (closest == null) return true;
            FaceTowards(bot, closest);
            return true;
        }
        
        public override InstructionData Parse(string[] args) {
            InstructionData data = default(InstructionData);
            if (args.Length > 1)
                data.Metadata = ushort.Parse(args[1]);
            return data;
        }
        
        public override void Output(Player p, string[] args, TextWriter w) {
            if (args.Length > 3) {
                w.WriteLine(Name + " " + ushort.Parse(args[3]));
            } else {
                w.WriteLine(Name);
            }
        }
        
        static void FaceTowards(PlayerBot bot, Player p) {
            int srcHeight = ModelInfo.CalcEyeHeight(p);
            int dstHeight = ModelInfo.CalcEyeHeight(bot);
            
            int dx = p.Pos.X - bot.Pos.X, dy = (p.Pos.Y + srcHeight) - (bot.Pos.Y + dstHeight), dz = p.Pos.Z - bot.Pos.Z;
            Vec3F32 dir = new Vec3F32(dx, dy, dz);
            dir = Vec3F32.Normalise(dir);
            
            Orientation rot = bot.Rot;
            DirUtils.GetYawPitch(dir, out rot.RotY, out rot.HeadX);
            bot.Rot = rot;
        }
        
        public override string[] Help { get { return help; } }
        static string[] help = new string[] { 
            "%T/BotAI add [name] stare <radius>",
            "%HCauses the bot to stare at the closest player in the search radius.",
            "%H  <radius> defaults to 20000 blocks.",
        };
    }
}
