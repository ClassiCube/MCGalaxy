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

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdTp : Command {
        public override string name { get { return "tp"; } }
        public override string shortcut { get { return "move"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            string[] args = message.SplitSpaces();
            if (args.Length > 3) { Help(p); return; }
            if (args.Length == 3) { TeleportCoords(p, args); return; }
            
            Player target = null;
            PlayerBot bot = null;
            if (args.Length == 1) {
                target = PlayerInfo.FindMatches(p, args[0]);
                if (target == null) return;
                if (!CheckPlayer(p, target)) return;
            } else if (args[0].CaselessEq("bot")) {
                bot = Matcher.FindBotsInLevel(p, args[1]);
                if (bot == null) return;
            } else {
                Help(p); return;
            }
            
            p.beforeTeleportMap = p.level.name;
            p.beforeTeleportPos = p.Pos;
            Level lvl = bot != null ? bot.level : target.level;

            if (p.level != lvl) PlayerActions.ChangeMap(p, lvl.name);
            if (target != null && target.Loading) {
                Player.Message(p, "Waiting for " + target.ColoredName + " %Sto spawn..");
                target.BlockUntilLoad(10);
            }
            
            Position pos = bot != null ? bot.Pos : target.Pos;
            Orientation rot = bot != null ? bot.Rot : target.Rot;
            p.BlockUntilLoad(10);  //Wait for player to spawn in new map
            p.SendPos(Entities.SelfID, pos, rot);
        }
        
        static void TeleportCoords(Player p, string[] args) {
            int x, y, z;
            if (!ParseCoord(p, args[0], p.Pos.X,                            "X", out x)) return;
            if (!ParseCoord(p, args[1], p.Pos.Y - Entities.CharacterHeight, "Y", out y)) return;
            if (!ParseCoord(p, args[2], p.Pos.Z,                            "Z", out z)) return;

            PlayerActions.MoveCoords(p, x, y, z, p.Rot.RotY, p.Rot.HeadX);
        }
        
        static bool ParseCoord(Player p, string arg, int cur, string axis, out int value) {
            bool relative = arg[0] == '~';
            if (relative) arg = arg.Substring(1);
            value = 0;
            
            if (!CommandParser.GetInt(p, arg, axis + " coordinate", ref value)) return false;            
            if (relative) value += (cur / 32);
            return true;
        }
        
        static bool CheckPlayer(Player p, Player target) {
            if (target.level.IsMuseum) {
                Player.Message(p, target.ColoredName + " %Sis in a museum."); return false;
            }
            
            if (!Server.higherranktp && p.Rank < target.group.Permission) {
                MessageTooHighRank(p, "teleport to", true); return false;
            }
            
            IGame game = target.level.CurrentGame();
            if (!p.Game.Referee && game != null && !game.TeleportAllowed) {
                Player.Message(p, "You can only teleport to players who are " +
                               "playing a game when you are in referee mode."); return false;
            }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/tp [x y z]");
            Player.Message(p, "%HTeleports yourself to the given block coordinates.");
            Player.Message(p, "%H  Use ~ before a coordinate to move relative to current position.");
            Player.Message(p, "%T/tp [player]");
            Player.Message(p, "%HTeleports yourself to that player.");
            Player.Message(p, "%T/tp bot [name]");
            Player.Message(p, "%HTeleports yourself to that bot.");
            Player.Message(p, "%H  Use /p2p to teleport a given player to a different player.");
        }
    }
}
