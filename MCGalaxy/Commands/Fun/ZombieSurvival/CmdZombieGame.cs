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
    public sealed class CmdZombieGame : RoundsGameCmd {
        public override string name { get { return "ZombieGame"; } }
        public override string shortcut { get { return "ZG"; } }
        protected override RoundsGame Game { get { return Server.zombie; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ZS"), new CommandAlias("ZombieSurvival") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can manage zombie survival") }; }
        }
        
        protected override void HandleStatus(Player p, RoundsGame game) {
            if (!game.Running) { Player.Message(p, "Zombie Survival is not running"); return; }            
            switch (game.RoundsLeft) {
                case int.MaxValue:
                    Player.Message(p, "Zombie Survival running with infinite rounds"); break;
                case 0:
                    Player.Message(p, "Zombie Survival running, with this round being the last round"); break;
                default:
                    Player.Message(p, "Zombie Survival running, with " + (game.RoundsLeft + 1) + " rounds left"); break;
            }

            Player.Message(p, "Running on map: " + game.Map.ColoredName);
        }
        
        protected override void HandleSet(Player p, RoundsGame game, string[] args) {
            if (!CheckExtraPerm(p, 1)) return;
            if (args.Length == 1) { Help(p, "set"); return; }
            
            ZSGame zs = (ZSGame)game;
            if (args[1].CaselessEq("hitbox"))  { SetHitbox(p,  zs, args); return; }
            if (args[1].CaselessEq("maxmove")) { SetMaxMove(p, zs, args); return; }
            Help(p, "set");
        }
        
        static void SetHitbox(Player p, ZSGame game, string[] args) {
            if (args.Length == 2) {
                Player.Message(p, "Hitbox detection is currently &a" + ZSGame.Config.HitboxPrecision + " %Sunits apart.");
                return;
            }
            
            int precision = 0;
            if (!CommandParser.GetInt(p, args[2], "Hitbox detection", ref precision, 0, 256)) return;
            
            ZSGame.Config.HitboxPrecision = precision;
            Player.Message(p, "Hitbox detection set to &a" + precision + " %Sunits apart.");
            ZSGame.Config.Save();
        }
        
        static void SetMaxMove(Player p, ZSGame game, string[] args) {
            if (args.Length == 2) {
                Player.Message(p, "Maxmium move distance is currently &a" + ZSGame.Config.MaxMoveDistance + " %Sunits apart.");
                return;
            }
            
            int distance = 0;
            if (!CommandParser.GetInt(p, args[2], "Maxmimum move distance", ref distance, 0, 256)) return;
            
            ZSGame.Config.MaxMoveDistance = distance;
            Player.Message(p, "Maximum move distance set to &a" + distance + " %Sunits apart.");
            ZSGame.Config.Save();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ZG start 0 %H- Runs Zombie Survival for infinite rounds.");
            Player.Message(p, "%T/ZG start [x] %H- Runs Zombie Survival for [x] rounds.");
            Player.Message(p, "%T/ZG end %H- Ends current round of Zombie Survival.");
            Player.Message(p, "%T/ZG stop %H- Immediately stops Zombie Survival.");
            Player.Message(p, "%T/ZG set [property] [value]");
            Player.Message(p, "%HSets a Zombie Survival game property, see %T/Help ZG set");
            Player.Message(p, "%T/ZG status %H- Outputs current status of Zombie Survival.");
            Player.Message(p, "%T/ZG go %H- Moves you to the current Zombie Survival map.");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("set")) {
                Player.Message(p, "%T/ZG set hitbox [distance]");
                Player.Message(p, "%HSets how far apart players need to be before they " +
                               "are considered touching. (32 units = 1 block).");
                Player.Message(p, "%T/ZG maxmove [distance]");
                Player.Message(p, "%HSets how far apart players are allowed to move in a " +
                               "movement packet before they are considered speedhacking. (32 units = 1 block).");
            } else {
                base.Help(p, message);
            }
        }
    }
}
