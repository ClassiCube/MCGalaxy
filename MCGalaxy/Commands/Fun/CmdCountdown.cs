/*  Copyright 2011 MCForge
        
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
//--------------------------------------------------\\
//the whole of the game 'countdown' was made by edh649\\
//======================================================\\
using System;
using System.Net;
using System.Threading;
using MCGalaxy.Games;
using MCGalaxy.Commands.World;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.Fun {
    
    public sealed class CmdCountdown : Command {
        
        public override string name { get { return "CountDown"; } }
        public override string shortcut { get { return "CD"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can send the countdown rules to everybody"),
                    new CommandPerm(LevelPermission.Operator, "+ can setup and manage countdown"),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }

            string[] args = message.ToLower().SplitSpaces();
            string cmd = args[0], arg1 = "", arg2 = "", arg3 = "";
            if (args.Length > 1) arg1 = args[1];
            if (args.Length > 2) arg2 = args[2];
            if (args.Length > 3) arg3 = args[3];
            CountdownGame game = Server.Countdown;
            
            switch (cmd) {
                case "join":
                    HandleJoin(p, game); return;
                case "leave":
                    HandleLeave(p, game); return;
                case "players":
                    HandlePlayers(p, game); return;
                case "rules":
                    HandleRules(p, arg1); return;
                    
                case "download":
                case "generate":
                    HandleGenerate(p, game, arg1, arg2, arg3); return;
                case "enable":
                    HandleEnable(p, game); return;
                case "disable":
                case "cancel":
                    HandleDisable(p, game); return;
                    
                case "start":
                case "play":
                    HandleStart(p, game, arg1, arg2); return;
                case "end":
                    HandleEnd(p, game); return;
                case "reset":
                    HandleReset(p, game, arg1); return;
                    
                default:
                    Help(p); break;
            }
        }
        
        void HandleJoin(Player p, CountdownGame game) {
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Cannot join as countdown is not running.");
                    return;
                case CountdownGameStatus.Enabled:
                    game.PlayerJoinedGame(p);
                    return;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "Cannot join when a round is about to start. Wait until next round.");
                    return;
                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "Cannot join when a round is in progress. Wait until next round.");
                    return;
            }
        }
        
        void HandleLeave(Player p, CountdownGame game) {
            if (!game.Players.Contains(p)) {
                Player.Message(p, "Cannot leave as you did not join countdown to begin with.");
                return;
            }
            
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Cannot leave as countdown is not running.");
                    return;
                case CountdownGameStatus.Enabled:
                    game.PlayerLeftGame(p);
                    return;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "Cannot leave when a round is about to start.");
                    return;
                case CountdownGameStatus.RoundInProgress:
                    if (game.Remaining.Contains(p)) {
                        Player.Message(p, "Cannot leave when a round in progress - please wait until the round ends or you die.");
                    } else {
                        game.PlayerLeftGame(p);
                    }
                    return;
            }
        }
        
        void HandlePlayers(Player p, CountdownGame game) {
            Player[] players = game.Players.Items;
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Countdown is not running.");
                    break;

                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "Players in countdown:");
                    Player.Message(p, players.Join(pl => FormatPlayer(pl, game)));
                    break;

                default:
                    Player.Message(p, "Players in countdown: ");
                    Player.Message(p, players.Join(pl => pl.ColoredName));
                    break;
            }
        }
        
        static string FormatPlayer(Player pl, CountdownGame game) {
            if (game.Remaining.Contains(pl)) {
                return pl.ColoredName + " &a[IN]";
            } else {
                return pl.ColoredName + " &c[OUT]";
            }
        }
        
        void HandleRules(Player p, string target) {
            Player who = p;
            if (target.Length > 0 && HasExtraPerm(p, 1)) {
                who = PlayerInfo.FindMatches(p, target);
                if (who == null) return;
                
                if (p.Rank < who.Rank) {
                    MessageTooHighRank(p, "send countdown rules", true); return;
                }
            }
            
            Player.Message(who, "The aim of the game is to stay alive the longest.");
            Player.Message(who, "Don't fall in the lava!");
            Player.Message(who, "Blocks on the ground will disapear randomly, first going yellow, then orange, then red and finally disappearing.");
            Player.Message(who, "The last person alive wins!");
            
            if (p != who) {
                Player.Message(who, "Countdown rules sent to you by " + p.ColoredName);
                Player.Message(p, "Countdown rules sent to: " + who.ColoredName);
            }
        }
        
        
        void HandleGenerate(Player p, CountdownGame game, string x, string y, string z) {
            if (!CheckExtraPerm(p, 2)) return;
            
            int width, height, length;
            if(!int.TryParse(x, out width) || !int.TryParse(y, out height) || !int.TryParse(z, out length)) {
                width = 32; height = 32; length = 32;
            }
            if (width < 32 || !MapGen.OkayAxis(width)) width = 32;
            if (height < 32 || !MapGen.OkayAxis(height)) height = 32;
            if (length < 32 || !MapGen.OkayAxis(length)) length = 32;
            
            if (!CmdNewLvl.CheckMapVolume(p, width, height, length)) return;
            game.GenerateMap(p, width, height, length);
        }
        
        void HandleEnable(Player p, CountdownGame game) {
            if (!CheckExtraPerm(p, 2)) return;
            
            if (game.Status == CountdownGameStatus.Disabled) {
                game.Enable(p);
            } else {
                Player.Message(p, "Countdown has already been enabled.");
            }
        }
        
        void HandleDisable(Player p, CountdownGame game) {
            if (!CheckExtraPerm(p, 2)) return;
            
            if (game.Status == CountdownGameStatus.Disabled) {
                Player.Message(p, "Countdown is not running."); return;
            }
            game.Disable();
        }
        
        
        void HandleStart(Player p, CountdownGame game, string speed, string mode) {
            if (!CheckExtraPerm(p, 2)) return;
            
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Countdown is not running."); return;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "A round is already about to begin."); return;
                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "A round is already in progress."); return;
                case CountdownGameStatus.Enabled:
                    if (game.Players.Count < 2) {
                        Player.Message(p, "At least two players must join countdown before a round can begin."); return;
                    }
                    break;
            }

            switch (speed) {
                case "slow": game.Interval = 800; break;
                case "normal": game.Interval = 650; break;
                case "fast": game.Interval = 500; break;
                case "extreme": game.Interval = 300; break;
                case "ultimate": game.Interval = 150; break;
                default:
                    Player.Message(p, "No speed specified, playing at 'normal' speed.");
                    game.Interval = 650; speed = "normal"; break;
            }
            
            game.FreezeMode = mode == "freeze" || mode == "frozen";
            game.SpeedType = speed;
            game.BeginRound(p);
        }
        
        void HandleEnd(Player p, CountdownGame game) {
            if (!CheckExtraPerm(p, 2)) return;
 
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Countdown is not running."); break;
                case CountdownGameStatus.Enabled:
                    Player.Message(p, "No round is currently running."); break;
                default:
                    game.EndRound(null); break;
            }
        }
        
        void HandleReset(Player p, CountdownGame game, string type) {
            if (!CheckExtraPerm(p, 2)) return;
            
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Countdown is not running."); break;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "Cannot reset map as a round is about to begin."); break;
                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "Cannot reset map as a round is already in progress."); break;
                default:
                    Player.Message(p, "Resetting");
                    game.ResetMap(); break;
            }
        }
        
        
        public override void Help(Player p) {
            Player.Message(p, "%T/CD join/leave %H- joins/leaves the game");
            Player.Message(p, "%T/CD players %H- lists players currently playing");
            Player.Message(p, "%T/CD rules %H- view the rules of countdown");
            if (HasExtraPerm(p, 1)) {
                Player.Message(p, "%T/CD rules [player] %H- sends rules to that player.");
            }
            
            if (!HasExtraPerm(p, 2)) return;
            Player.Message(p, "%T/CD generate [width] [height] [length] %H- generates the countdown map (default is 32x32x32)");
            Player.Message(p, "%T/CD enable/disable %H- enables/disables countdown");
            Player.Message(p, "%T/CD start <speed> <mode> %H- starts a round of countdown");
            Player.Message(p, "%H  speed can be: slow, normal, fast, extreme or ultimate");
            Player.Message(p, "%H  mode can be: normal or freeze");
            Player.Message(p, "%T/CD end %H- force ends current round of countdown");
            Player.Message(p, "%T/CD reset %H- resets the map. %T/CD start %Halso resets map.");
        }
    }
}
