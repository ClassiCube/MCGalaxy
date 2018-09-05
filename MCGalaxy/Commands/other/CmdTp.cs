/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using MCGalaxy.Games;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdTp : Command2 {
        public override string name { get { return "TP"; } }
        public override string shortcut { get { return "Move"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("Teleport"), new CommandAlias("TPP", "-precise") }; }
        }
        
        const string precisePrefix = "-precise ";
        public override void Use(Player p, string message, CommandData data) {           
            if (message.Length == 0) { Help(p); return; }
            
            bool preciseTP = message.CaselessStarts(precisePrefix);        
            if (preciseTP) {
                message = message.Substring(precisePrefix.Length);
            }
            
            string[] args = message.SplitSpaces();            
            if (args.Length >= 3) { TeleportCoords(p, args, preciseTP); return; }
            
            Player target = null;
            PlayerBot bot = null;
            if (args.Length == 1) {
                target = PlayerInfo.FindMatches(p, args[0]);
                if (target == null) return;
                if (!CheckPlayer(p, target, data)) return;
            } else if (args[0].CaselessEq("bot")) {
                bot = Matcher.FindBots(p, args[1]);
                if (bot == null) return;
            } else {
                Help(p); return;
            }
            
            SavePreTeleportState(p);
            Level lvl = bot != null ? bot.level : target.level;

            if (p.level != lvl) PlayerActions.ChangeMap(p, lvl.name);
            if (target != null && target.Loading) {
                p.Message("Waiting for " + target.ColoredName + " %Sto spawn..");
                target.BlockUntilLoad(10);
            }
            
            // Player wasn't able to join target map, so don't move
            if (p.level != lvl) return;
            
            Position pos = bot != null ? bot.Pos : target.Pos;
            Orientation rot = bot != null ? bot.Rot : target.Rot;
            p.BlockUntilLoad(10);  //Wait for player to spawn in new map
            p.SendPos(Entities.SelfID, pos, rot);
        }
        
        internal static bool GetTeleportCoords(Player p, Entity ori, string[] args, bool precise, 
                                               out Position pos, out byte yaw, out byte pitch) {
            Vec3S32 P;
            pos = p.Pos; yaw = ori.Rot.RotY; pitch = ori.Rot.HeadX;
            
            if (!precise) {
                // relative to feet block coordinates
                P = p.Pos.FeetBlockCoords;
                if (!CommandParser.GetCoords(p, args, 0, ref P)) return false;
                pos = Position.FromFeetBlockCoords(P.X, P.Y, P.Z);
            } else {
                // relative to feet position exactly
                P = new Vec3S32(p.Pos.X, p.Pos.Y + Entities.CharacterHeight, p.Pos.Z);
                if (!CommandParser.GetCoords(p, args, 0, ref P)) return false;
                pos = new Position(P.X, P.Y - Entities.CharacterHeight, P.Z);
            }
            
            int angle = 0;            
            if (args.Length > 3) {
                if (!CommandParser.GetInt(p, args[3], "Yaw angle", ref angle, -360, 360)) return false;
                yaw = Orientation.DegreesToPacked(angle);
            }            
            if (args.Length > 4) {
                if (!CommandParser.GetInt(p, args[4], "Pitch angle", ref angle, -360, 360)) return false;
                pitch = Orientation.DegreesToPacked(angle);
            }
            return true;
        }
        
        static void TeleportCoords(Player p, string[] args, bool precise) {
            Position pos; byte yaw, pitch;
            if (!GetTeleportCoords(p, p, args, precise, out pos, out yaw, out pitch)) return;

            SavePreTeleportState(p);
            p.SendPos(Entities.SelfID, pos, new Orientation(yaw, pitch));
        }
        
        static void SavePreTeleportState(Player p) {
            p.PreTeleportMap = p.level.name;
            p.PreTeleportPos = p.Pos;
            p.PreTeleportRot = p.Rot;
        }
        
        static bool CheckPlayer(Player p, Player target, CommandData data) {
            if (target.level.IsMuseum) {
                p.Message(target.ColoredName + " %Sis in a museum."); return false;
            }          
            if (!ServerConfig.HigherRankTP && !CheckRank(p, data, target, "teleport to", true)) return false;
            
            IGame game = IGame.GameOn(target.level);
            if (!p.Game.Referee && game != null) {
                p.Message("You can only teleport to players in " +
                               "a game when you are in referee mode."); return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            p.Message("%HUse ~ before a coordinate to move relative to current position");
            p.Message("%T/TP [x y z] <yaw> <pitch>");
            p.Message("%HTeleports yourself to the given block coordinates.");
            p.Message("%T/TP -precise [x y z] <yaw> <pitch>");
            p.Message("%HTeleports using precise units. (32 units = 1 block)");
            p.Message("%T/TP [player]");
            p.Message("%HTeleports yourself to that player.");
            p.Message("%T/TP bot [name]");
            p.Message("%HTeleports yourself to that bot.");
        }
    }
}
