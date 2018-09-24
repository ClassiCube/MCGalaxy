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
using MCGalaxy.Bots;
using MCGalaxy.Commands.Misc;
    
namespace MCGalaxy.Commands.Bots {
    public sealed class CmdBotSummon : Command2 {
        public override string name { get { return "BotSummon"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            if (!LevelInfo.Check(p, data.Rank, p.level, "summon that bot")) return;
            
            string[] args = message.SplitSpaces(2);
            PlayerBot bot = Matcher.FindBots(p, args[0]);
            if (bot == null) return;
            
            Position pos; byte yaw, pitch;            
            if (args.Length == 1) {
                pos = p.Pos; yaw = p.Rot.RotY; pitch = p.Rot.HeadX;
            } else {
                args = args[1].SplitSpaces();
                
                if (args.Length < 3) { Help(p); return; }                
                if (!CmdTp.GetTeleportCoords(p, bot, args, false, out pos, out yaw, out pitch)) return;
            }
            
            bot.Pos = pos; bot.SetYawPitch(yaw, pitch);
            BotsFile.Save(p.level);
        }
        
        public override void Help(Player p) {   
            p.Message("%T/BotSummon [name] [x y z] <yaw> <pitch>");
            p.Message("%HTeleports a bot to the given block coordinates.");
            p.Message("%HUse ~ before a coordinate to move relative to current position");
            p.Message("%T/BotSummon [name]");
            p.Message("%HSummons a bot to your position.");
        }
    }
}
