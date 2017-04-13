/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System;
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    public sealed class CmdZombieGame : Command {
        public override string name { get { return "zombiegame"; } }
        public override string shortcut { get { return "zg"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("zs"), new CommandAlias("zombiesurvival") }; }
        }
        public CmdZombieGame() { }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.ToLower().SplitSpaces();
            switch (args[0]) {
                    case "go": HandleGo(p, message, args); break;
                    case "status": HandleStatus(p, message, args); break;
                    case "start": HandleStart(p, message, args); break;
                    case "stop": HandleStop(p, message, args); break;
                    case "force": HandleForceStop(p, message, args); break;
                    case "hitbox": HandleHitbox(p, message, args); break;
                    case "maxmove": HandleMaxMove(p, message, args); break;
            }
        }

        static void HandleGo(Player p, string message, string[] args) {
            if (Server.zombie.Status == ZombieGameStatus.NotStarted) {
                Player.Message(p, "Zombie Survival is not currently running."); return;
            }
            PlayerActions.ChangeMap(p, Server.zombie.CurLevel);
        }
        
        static void HandleStatus(Player p, string message, string[] args) {
            switch (Server.zombie.Status) {
                case ZombieGameStatus.NotStarted:
                    Player.Message(p, "Zombie Survival is not currently running."); break;
                case ZombieGameStatus.InfiniteRounds:
                    Player.Message(p, "Zombie Survival is currently in progress with infinite rounds."); break;
                case ZombieGameStatus.SingleRound:
                    Player.Message(p, "Zombie Survival game currently in progress."); break;
                case ZombieGameStatus.VariableRounds:
                    Player.Message(p, "Zombie Survival game currently in progress with " + Server.zombie.MaxRounds + " rounds."); break;
                case ZombieGameStatus.LastRound:
                    Player.Message(p, "Zombie Survival game currently in progress, with this round being the final round."); break;
            }
            
            if (Server.zombie.Status == ZombieGameStatus.NotStarted || Server.zombie.CurLevelName == "") return;
            Player.Message(p, "Running on map: " + Server.zombie.CurLevelName);
        }
        
        static void HandleStart(Player p, string message, string[] args) {
            if (Server.zombie.Running) {
                Player.Message(p, "There is already a Zombie Survival game currently in progress."); return;
            }
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            if (args.Length == 2) {
                int rounds = 1;
                if (!CommandParser.GetInt(p, args[1], "Rounds", ref rounds, 0)) return;

                ZombieGameStatus status = rounds == 0 ?
                    ZombieGameStatus.InfiniteRounds : ZombieGameStatus.VariableRounds;
                Server.zombie.Start(status, lvl, rounds);
            } else {
                Server.zombie.Start(ZombieGameStatus.SingleRound, lvl, 0);
            }
        }
        
        static void HandleStop(Player p, string message, string[] args) {
            if (!Server.zombie.Running) {
                Player.Message(p, "There is no Zombie Survival game currently in progress."); return;
            }
            
            Chat.MessageLevel(Server.zombie.CurLevel, "The current game of Zombie Survival will end this round!");
            Server.zombie.Status = ZombieGameStatus.LastRound;
        }
        
        static void HandleForceStop(Player p, string message, string[] args) {
            if (!Server.zombie.Running) {
                Player.Message(p, "There is no Zombie Survival game currently in progress."); return;
            }
            
            string src = p == null ? "(console)" : p.name;
            Server.s.Log("Zombie Survival ended forcefully by " + src);
            Server.zombie.ResetState();
        }
        
        static void HandleHitbox(Player p, string message, string[] args) {
            if (args.Length == 1) {
                Player.Message(p, "Hitbox detection is currently &a" + ZombieGameProps.HitboxPrecision + " %Sunits apart.");
                return;
            }
            
            byte precision = 0;
            if (!CommandParser.GetByte(p, args[1], "Hitbox detection", ref precision)) return;
            
            ZombieGameProps.HitboxPrecision = precision;
            Player.Message(p, "Hitbox detection set to &a" + precision + " %Sunits apart.");
            SrvProperties.Save();
        }
        
        static void HandleMaxMove(Player p, string message, string[] args) {
            if (args.Length == 1) {
                Player.Message(p, "Maxmium move distance is currently &a" + ZombieGameProps.MaxMoveDistance + " %Sunits apart.");
                return;
            }
            
            byte distance = 0;
            if (!CommandParser.GetByte(p, args[1], "Maxmimum move distance", ref distance)) return;
            
            ZombieGameProps.MaxMoveDistance = distance;
            Player.Message(p, "Maximum move distance set to &a" + distance + " %Sunits apart.");
            SrvProperties.Save();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/zg status %H- Shows the current status of Zombie Survival.");
            Player.Message(p, "%T/zg go %H- Moves you to the Zombie Survival map.");
            Player.Message(p, "%T/zg start 0 %H- Starts Zombie Survival for an unlimited amount of rounds.");
            Player.Message(p, "%T/zg start [x] %H- Starts Zombie Survival for [x] amount of rounds.");
            Player.Message(p, "%T/zg stop %H- Stops Zombie Survival after the round has finished.");
            Player.Message(p, "%T/zg force %H- Immediately stops Zombie Survival.");
            Player.Message(p, "%T/zg hitbox [distance] %H- Sets how far apart players need to be before " +
                           "they are considered a 'collision'. (32 units = 1 block).");
            Player.Message(p, "%T/zg maxmove [distance] %H- Sets how far apart players are allowed to move in a" +
                           "movement packet before they are considered speedhacking. (32 units = 1 block).");
        }
    }
}
