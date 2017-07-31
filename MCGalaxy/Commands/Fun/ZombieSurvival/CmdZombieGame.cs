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

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdZombieGame : Command {
        public override string name { get { return "zombiegame"; } }
        public override string shortcut { get { return "zg"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("zs"), new CommandAlias("zombiesurvival") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.ToLower().SplitSpaces();
            ZSGame game = Server.zombie;
            
            switch (args[0]) {
                    case "go": HandleGo(p, game, args); break;
                    case "status": HandleStatus(p, game, args); break;
                    case "start": HandleStart(p, game, args); break;
                    case "end": HandleEnd(p, game, args); break;
                    case "stop": HandleStop(p, game, args); break;
                    case "set": HandleSet(p, game, args); break;
                    default: Help(p); break;
            }
        }

        static void HandleGo(Player p, ZSGame game, string[] args) {
            if (game.Status == ZombieGameStatus.NotStarted) {
                Player.Message(p, "Zombie Survival is not currently running."); return;
            }
            PlayerActions.ChangeMap(p, game.CurLevel);
        }
        
        static void HandleStatus(Player p, ZSGame game, string[] args) {
            switch (game.Status) {
                case ZombieGameStatus.NotStarted:
                    Player.Message(p, "Zombie Survival is not currently running."); break;
                case ZombieGameStatus.InfiniteRounds:
                    Player.Message(p, "Zombie Survival is currently in progress with infinite rounds."); break;
                case ZombieGameStatus.SingleRound:
                    Player.Message(p, "Zombie Survival game currently in progress."); break;
                case ZombieGameStatus.VariableRounds:
                    Player.Message(p, "Zombie Survival game currently in progress with " + game.MaxRounds + " rounds."); break;
                case ZombieGameStatus.LastRound:
                    Player.Message(p, "Zombie Survival game currently in progress, with this round being the final round."); break;
            }
            
            if (game.Status == ZombieGameStatus.NotStarted || game.CurLevelName.Length == 0) return;
            Player.Message(p, "Running on map: " + game.CurLevelName);
        }
        
        static void HandleStart(Player p, ZSGame game, string[] args) {
            if (game.Running) {
                Player.Message(p, "There is already a Zombie Survival game currently in progress."); return;
            }
            Level lvl = Player.IsSuper(p) ? null : p.level;
            
            if (args.Length == 2) {
                int rounds = 1;
                if (!CommandParser.GetInt(p, args[1], "Rounds", ref rounds, 0)) return;

                ZombieGameStatus status = rounds == 0 ?
                    ZombieGameStatus.InfiniteRounds : ZombieGameStatus.VariableRounds;
                game.Start(status, lvl, rounds);
            } else {
                game.Start(ZombieGameStatus.SingleRound, lvl, 0);
            }
        }
        
        static void HandleEnd(Player p, ZSGame game, string[] args) {
            if (game.RoundInProgress) {
                game.EndRound();
            } else {
                Player.Message(p, "No round is currently in progress.");
            }
        }
        
        static void HandleStop(Player p, ZSGame game, string[] args) {
            if (!game.Running) {
                Player.Message(p, "There is no Zombie Survival game currently in progress."); return;
            }
            
            string src = p == null ? "(console)" : p.ColoredName;
            Level lvl = game.CurLevel;
            if (lvl != null) {
                Chat.MessageLevel(game.CurLevel, "Zombie Survival was stopped by " + src);
            }
            
            src = p == null ? "(console)" : p.name;
            Logger.Log(LogType.GameActivity, "Zombie Survival stopped by " + src);
            game.End();
        }
        
       void HandleSet(Player p, ZSGame game, string[] args) {
            if (args.Length == 1) { Help(p, "set"); return; }
            
            if (args[1].CaselessEq("hitbox")) { HandleHitbox(p, game, args); return; }
            if (args[1].CaselessEq("maxmove")) { HandleMaxMove(p, game, args); return; }
            Help(p, "set");
        }
        
        static void HandleHitbox(Player p, ZSGame game, string[] args) {
            if (args.Length == 2) {
                Player.Message(p, "Hitbox detection is currently &a" + ZSConfig.HitboxPrecision + " %Sunits apart.");
                return;
            }
            
            byte precision = 0;
            if (!CommandParser.GetByte(p, args[2], "Hitbox detection", ref precision)) return;
            
            ZSConfig.HitboxPrecision = precision;
            Player.Message(p, "Hitbox detection set to &a" + precision + " %Sunits apart.");
            ZSConfig.SaveSettings();
        }
        
        static void HandleMaxMove(Player p, ZSGame game, string[] args) {
            if (args.Length == 2) {
                Player.Message(p, "Maxmium move distance is currently &a" + ZSConfig.MaxMoveDistance + " %Sunits apart.");
                return;
            }
            
            byte distance = 0;
            if (!CommandParser.GetByte(p, args[2], "Maxmimum move distance", ref distance)) return;
            
            ZSConfig.MaxMoveDistance = distance;
            Player.Message(p, "Maximum move distance set to &a" + distance + " %Sunits apart.");
            ZSConfig.SaveSettings();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/zg status %H- Outputs current status of Zombie Survival.");
            Player.Message(p, "%T/zg go %H- Moves you to the current Zombie Survival map.");
            Player.Message(p, "%T/zg start 0 %H- Runs Zombie Survival for infinite rounds.");         
            Player.Message(p, "%T/zg start [x] %H- Runs Zombie Survival for [x] rounds.");
            Player.Message(p, "%T/zg end %H- Ends current round of Zombie Survival.");
            Player.Message(p, "%T/zg stop %H- Immediately stops Zombie Survival.");
            Player.Message(p, "%T/zg set [property] [value]");
            Player.Message(p, "%HSets a Zombie Survival game property, see %T/help zs set");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/zg set hitbox [distance]");
                Player.Message(p, "%HSets how far apart players need to be before they " +
                               "are considered touching. (32 units = 1 block).");
                Player.Message(p, "%T/zg maxmove [distance]");
                Player.Message(p, "%HSets how far apart players are allowed to move in a " +
                               "movement packet before they are considered speedhacking. (32 units = 1 block).");
            } else {
                Help(p);
            }
        }
    }
}
