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
        
        public override string name { get { return "countdown"; } }
        public override string shortcut { get { return "cd"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can send the countdown rules to everybody"),
                    new CommandPerm(LevelPermission.Operator, "+ can setup countdown (generate/start/restart/enable/disable/cancel)"),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }

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
                    HandleDisable(p, game); return;
                case "cancel":
                    HandleCancel(p, game); return;
                case "start":
                case "play":
                    HandleStart(p, game, arg1, arg2); return;
                case "reset":
                    HandleReset(p, game, arg1); return;
                case "tutorial":
                    HandleTutorial(p); return;
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
                case CountdownGameStatus.RoundFinished:
                    Player.Message(p, "Sorry - The game has finished. Get an op to reset it.");
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
                case CountdownGameStatus.RoundFinished:
                    Player.Message(p, "You've left countdown.");
                    game.PlayerLeftGame(p);
                    return;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "Cannot leave when a round is about to start.");
                    return;
                case CountdownGameStatus.RoundInProgress:
                    if (game.PlayersRemaining.Contains(p)) {
                        Player.Message(p, "Cannot leave when a round in progress - please wait until the round ends or you die.");
                    } else {
                        game.Players.Remove(p);
                        Player.Message(p, "You've left countdown.");
                    }
                    return;
            }
        }
        
        void HandlePlayers(Player p, CountdownGame game) {
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Countdown is not running.");
                    break;

                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "Players in countdown:");
                    Player.Message(p, game.Players.Join(pl => FormatPlayer(pl, game)));
                    break;

                default:
                    Player.Message(p, "Players in countdown: ");
                    Player.Message(p, game.Players.Join(pl => pl.ColoredName));
                    break;
            }
        }
        
        static string FormatPlayer(Player pl, CountdownGame game) {
            if (game.PlayersRemaining.Contains(pl)) {
                return pl.ColoredName + " &a[IN]";
            } else {
                return pl.ColoredName + " &c[OUT]";
            }
        }
        
        void HandleRules(Player p, string target) {
            Player who = p;
            if (target != "" && CheckExtraPerm(p, 1)) {
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
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            int width, height, length;
            if(!int.TryParse(x, out width) || !int.TryParse(y, out height) || !int.TryParse(z, out length)) {
                width = 32; height = 32; length = 32;
            }
            if (width < 32 || !MapGen.OkayAxis(width)) width = 32;
            if (height < 32 || !MapGen.OkayAxis(height)) height = 32;
            if (length < 32 || !MapGen.OkayAxis(length)) length = 32;
            if (!CmdNewLvl.CheckMapVolume(p, width, height, length)) return;
            
            Level lvl = CountdownMapGen.Generate(width, height, length);
            Level oldLvl = LevelInfo.FindExact("countdown");
            if (oldLvl != null) LevelActions.Replace(oldLvl, lvl);
            else LevelInfo.Loaded.Add(lvl);
            
            lvl.Save();
            if (game.Status != CountdownGameStatus.Disabled)
                game.Map = lvl;
            
            const string format = "Generated map ({0}x{1}x{2}), sending you to it..";
            Player.Message(p, format, width, height, length);
            PlayerActions.ChangeMap(p, "countdown");
            
            Position pos = new Position(16 + 8 * 32, 32 + 23 * 32, 16 + 17 * 32);
            p.SendPos(Entities.SelfID, pos, p.Rot);
        }
        
        void HandleEnable(Player p, CountdownGame game) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (game.Status == CountdownGameStatus.Disabled) {
                CmdLoad.LoadLevel(null, "countdown");
                game.Map = LevelInfo.FindExact("countdown");
                
                if (game.Map == null) {
                    Player.Message(p, "Countdown level not found, generating..");
                    HandleGenerate(p, game, "", "", "");
                    game.Map = LevelInfo.FindExact("countdown");
                }
                
                game.Map.Config.Deletable = false;
                game.Map.Config.Buildable = false;
                game.Map.BuildAccess.Min = LevelPermission.Nobody;
                game.Map.Config.MOTD = "Welcome to the Countdown map! -hax";
                
                game.Status = CountdownGameStatus.Enabled;
                Chat.MessageGlobal("Countdown has been enabled!");
            } else {
                Player.Message(p, "Countdown has already been enabled.");
            }
        }
        
        void HandleDisable(Player p, CountdownGame game) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (game.Status == CountdownGameStatus.RoundCountdown || game.Status == CountdownGameStatus.RoundInProgress) {
                Player.Message(p, "A round is currently in progress - please wait until it is finished, or use '/cd cancel' to cancel the game"); return;
            } else if (game.Status == CountdownGameStatus.Disabled) {
                Player.Message(p, "Countdown is not running."); return;
            } else {
                foreach (Player pl in game.Players)
                    Player.Message(pl, "The countdown game was disabled.");
                game.Reset(p, true);
                game.Status = CountdownGameStatus.Disabled;
                Player.Message(p, "Countdown Disabled");
            }
        }
        
        void HandleCancel(Player p, CountdownGame game) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (game.Status == CountdownGameStatus.RoundCountdown || game.Status == CountdownGameStatus.RoundInProgress) {
                game.cancel = true;
                Thread.Sleep(1500);
                Player.Message(p, "Countdown has been canceled");
                game.Status = CountdownGameStatus.Enabled;
            } else if (game.Status == CountdownGameStatus.Disabled) {
                Player.Message(p, "Countdown is not running.");
            } else {
                foreach (Player pl in game.Players)
                    Player.Message(pl, "The countdown game was canceled");
                game.Reset(null, true);
            }
        }
        
        void HandleStart(Player p, CountdownGame game, string speed, string mode) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Countdown is not yet enabled."); return;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "A round is already about to begin."); return;
                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "A round is already in progress."); return;
                case CountdownGameStatus.RoundFinished:
                    Player.Message(p, "Game has finished"); return;
                case CountdownGameStatus.Enabled:
                    if (game.Players.Count < 2) {
                        Player.Message(p, "At least two players must join countdown before a round can begin."); return;
                    }
                    game.Status = CountdownGameStatus.RoundCountdown; break;
            }

            switch (speed) {
                case "slow":
                    game.Speed = 800; game.SpeedType = "slow"; break;
                case "normal":
                    game.Speed = 650; game.SpeedType = "normal"; break;
                case "fast":
                    game.Speed = 500; game.SpeedType = "fast"; break;
                case "extreme":
                    game.Speed = 300; game.SpeedType = "extreme"; break;
                case "ultimate":
                    game.Speed = 150; game.SpeedType = "ultimate"; break;
                default:
                    Player.Message(p, "No speed specified, playing at 'normal' speed.");
                    game.Speed = 650; game.SpeedType = "normal"; break;
            }
            
            game.FreezeMode = (mode == "freeze" || mode == "frozen");
            game.BeginRound(p);
        }
        
        void HandleReset(Player p, CountdownGame game, string type) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            switch (game.Status) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Please enable countdown first."); break;
                case CountdownGameStatus.RoundCountdown:
                    Player.Message(p, "Cannot reset as a round is about to begin."); break;
                case CountdownGameStatus.RoundInProgress:
                    Player.Message(p, "Cannot reset as a round is already in progress."); break;
                default:
                    Player.Message(p, "Reseting");
                    if (type == "map") game.Reset(p, false);
                    else if (type == "all") game.Reset(p, true);
                    else Player.Message(p, "Can only reset 'map' or 'all'");
                    break;
            }
        }
        
        void HandleTutorial(Player p) {
            Player.Message(p, "First, generate the map using /cd generate");
            Player.Message(p, "Next, type /cd enable to enable the game mode");
            Player.Message(p, "Next, type /cd join to join the game and tell other players to join aswell");
            Player.Message(p, "When some people have joined, type /cd start [speed] to start it");
            Player.Message(p, "[speed] can be 'ultimate', 'extreme', 'fast', 'normal' or 'slow'");
            Player.Message(p, "When you are done, type /cd reset [map/all]");
            Player.Message(p, "use map to reset only the map and all to reset everything.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/cd joins/leave %H- joins/leaves the game");
            Player.Message(p, "%T/cd players %H- lists players currently playing");
            Player.Message(p, "%T/cd rules %H- view the rules of countdown");
            if (CheckExtraPerm(p, 1)) {
                Player.Message(p, "%T/cd rules [player] %H- sends rules of countdown to that player.");
            }
            
            if (!CheckExtraPerm(p, 2)) return;
            Player.Message(p, "%T/cd generate [width] [height] [length] %H- generates the countdown map (default is 32x32x32)");
            Player.Message(p, "%T/cd enable/disable %H- enables/disables the game");
            Player.Message(p, "%T/cd cancel %H- cancels a game");
            Player.Message(p, "%T/cd start [speed] [mode] %H- start the game, speeds are 'slow', 'normal', 'fast', 'extreme' and 'ultimate', modes are 'normal' and 'freeze'");
            Player.Message(p, "%T/cd reset [all/map] %H- reset the whole game (all) or only the map (map)");
            Player.Message(p, "%T/cd tutorial %H- a tutorial on how to setup countdown");
        }
    }
}
