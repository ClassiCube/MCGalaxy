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
                    new CommandPerm(LevelPermission.Operator, "+ can setup countdown (download/start/restart/enable/disable/cancel)"),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }

            string[] args = message.ToLower().SplitSpaces();
            string cmd = args[0], arg1 = "", arg2 = "", arg3 = "";
            if (args.Length > 1) arg1 = args[1];
            if (args.Length > 2) arg2 = args[2];
            if (args.Length > 3) arg3 = args[3];
            
            switch (cmd) {
                case "join":
                    HandleJoin(p); return;
                case "leave":
                    HandleLeave(p); return;
                case "players":
                    HandlePlayers(p); return;
                case "rules":
                    HandleRules(p, arg1); return;
                case "download":
                case "generate":
                    HandleGenerate(p, arg1, arg2, arg3); return;
                case "enable":
                    HandleEnable(p); return;
                case "disable":
                    HandleDisable(p); return;
                case "cancel":
                    HandleCancel(p); return;
                case "start":
                case "play":
                    HandleStart(p, arg1, arg2); return;
                case "reset":
                    HandleReset(p, arg1); return;
                case "tutorial":
                    HandleTutorial(p); return;
                default:
                    Help(p); break;
            }
        }
        
        void HandleJoin(Player p) {
            switch (Server.Countdown.gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Sorry - Countdown isn't enabled yet");
                    return;
                case CountdownGameStatus.Enabled:
                    Server.Countdown.PlayerJoinedGame(p);
                    return;
                case CountdownGameStatus.AboutToStart:
                    Player.Message(p, "Sorry - The game is about to start");
                    return;
                case CountdownGameStatus.InProgress:
                    Player.Message(p, "Sorry - The game is already in progress.");
                    return;
                case CountdownGameStatus.Finished:
                    Player.Message(p, "Sorry - The game has finished. Get an op to reset it.");
                    return;
            }
        }
        
        void HandleLeave(Player p) {
            if (Server.Countdown.players.Contains(p)) {
                switch (Server.Countdown.gamestatus) {
                    case CountdownGameStatus.Disabled:
                        Player.Message(p, "Sorry - Countdown isn't enabled yet");
                        return;
                    case CountdownGameStatus.Enabled:
                        Player.Message(p, "You've left the game.");
                        Server.Countdown.PlayerLeftGame(p);
                        break;
                    case CountdownGameStatus.AboutToStart:
                        Player.Message(p, "Sorry - The game is about to start");
                        return; ;
                    case CountdownGameStatus.InProgress:
                        Player.Message(p, "Sorry - you are in a game that is in progress, please wait till its finished or till you've died.");
                        return;
                    case CountdownGameStatus.Finished:
                        Server.Countdown.players.Remove(p);
                        Server.Countdown.playersleftlist.Remove(p);
                        p.playerofcountdown = false;
                        Player.Message(p, "You've left the game.");
                        break;
                }
            } else if (!(Server.Countdown.playersleftlist.Contains(p)) && Server.Countdown.players.Contains(p)) {
                Server.Countdown.players.Remove(p);
                Player.Message(p, "You've left the game.");
            } else {
                Player.Message(p, "You haven't joined the game yet!!");
            }
        }
        
        void HandlePlayers(Player p) {
            switch (Server.Countdown.gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "The game has not been enabled yet.");
                    break;

                case CountdownGameStatus.Enabled:
                    Player.Message(p, "Players who have joined:");
                    foreach (Player plya in Server.Countdown.players)
                        Player.Message(p, plya.ColoredName);
                    break;

                case CountdownGameStatus.AboutToStart:
                    Player.Message(p, "Players who are about to play:");
                    foreach (Player plya in Server.Countdown.players)
                        Player.Message(p, plya.ColoredName);
                    break;

                case CountdownGameStatus.InProgress:
                    Player.Message(p, "Players left playing:");
                    foreach (Player plya in Server.Countdown.players) {
                        if (Server.Countdown.playersleftlist.Contains(plya))
                            Player.Message(p, plya.ColoredName + " %Swho is &aIN");
                        else
                            Player.Message(p, plya.ColoredName + " %Swho is &cOUT");
                    }
                    break;

                case CountdownGameStatus.Finished:
                    Player.Message(p, "Players who were playing:");
                    foreach (Player plya in Server.Countdown.players)
                        Player.Message(p, plya.ColoredName);
                    break;
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
        
        void HandleGenerate(Player p, string arg1, string arg2, string arg3) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            int width, height, length;
            if(!int.TryParse(arg1, out width) || !int.TryParse(arg2, out height) || !int.TryParse(arg3, out length)) {
                width = 32; height = 32; length = 32;
            }
            if (width < 32 || !MapGen.OkayAxis(width)) width = 32;
            if (height < 32 || !MapGen.OkayAxis(height)) height = 32;
            if (length < 32 || !MapGen.OkayAxis(length)) length = 32;
            if (!CmdNewLvl.CheckMapVolume(p, width, height, length)) return;
            
            Level lvl = CountdownMapGen.Generate(width, height, length);
            lvl.Deletable = false;
            lvl.Buildable = false;
            lvl.permissionbuild = LevelPermission.Nobody;
            lvl.motd = "Welcome to the Countdown map! -hax";
            
            Level oldLvl = LevelInfo.FindExact("countdown");
            if (oldLvl != null) LevelActions.Replace(oldLvl, lvl);
            else LevelInfo.Loaded.Add(lvl);
            
            lvl.Save();
            if (Server.Countdown.gamestatus != CountdownGameStatus.Disabled)
                Server.Countdown.mapon = lvl;
            
            const string format = "Generated map ({0}x{1}x{2}), sending you to it..";
            Player.Message(p, format, width, height, length);
            PlayerActions.ChangeMap(p, "countdown");
            
            Position pos = new Position(16 + 8 * 32, 32 + 23 * 32, 16 + 17 * 32);
            p.SendPos(Entities.SelfID, pos, p.Rot);
        }
        
        void HandleEnable(Player p) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (Server.Countdown.gamestatus == CountdownGameStatus.Disabled) {
                CmdLoad.LoadLevel(null, "countdown");
                Server.Countdown.mapon = LevelInfo.FindExact("countdown");
                
                if (Server.Countdown.mapon == null) {
                    Player.Message(p, "countdown level not found, generating..");
                    HandleGenerate(p, "", "", "");
                    Server.Countdown.mapon = LevelInfo.FindExact("countdown");
                }
                
                Server.Countdown.mapon.Deletable = false;
                Server.Countdown.mapon.Buildable = false;
                Server.Countdown.mapon.permissionbuild = LevelPermission.Nobody;
                Server.Countdown.mapon.motd = "Welcome to the Countdown map! -hax";
                
                Server.Countdown.gamestatus = CountdownGameStatus.Enabled;
                Chat.MessageGlobal("Countdown has been enabled!!");
            } else {
                Player.Message(p, "A Game is either already enabled or is already progress");
            }
        }
        
        void HandleDisable(Player p) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (Server.Countdown.gamestatus == CountdownGameStatus.AboutToStart || Server.Countdown.gamestatus == CountdownGameStatus.InProgress) {
                Player.Message(p, "A game is currently in progress - please wait until it is finished, or use '/cd cancel' to cancel the game"); return;
            } else if (Server.Countdown.gamestatus == CountdownGameStatus.Disabled) {
                Player.Message(p, "Already disabled!!"); return;
            } else {
                foreach (Player pl in Server.Countdown.players)
                    Player.Message(pl, "The countdown game was disabled.");
                Server.Countdown.Reset(p, true);
                Server.Countdown.gamestatus = CountdownGameStatus.Disabled;
                Player.Message(p, "Countdown Disabled");
            }
        }
        
        void HandleCancel(Player p) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (Server.Countdown.gamestatus == CountdownGameStatus.AboutToStart || Server.Countdown.gamestatus == CountdownGameStatus.InProgress) {
                Server.Countdown.cancel = true;
                Thread.Sleep(1500);
                Player.Message(p, "Countdown has been canceled");
                Server.Countdown.gamestatus = CountdownGameStatus.Enabled;
            } else if (Server.Countdown.gamestatus == CountdownGameStatus.Disabled) {
                Player.Message(p, "The game is disabled!!");
            } else {
                foreach (Player pl in Server.Countdown.players)
                    Player.Message(pl, "The countdown game was canceled");
                Server.Countdown.Reset(null, true);
            }
        }
        
        void HandleStart(Player p, string par1, string par2) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (Server.Countdown.gamestatus != CountdownGameStatus.Enabled) {
                Player.Message(p, "Either a game is already in progress or it hasn't been enabled"); return;
            }
            if (Server.Countdown.players.Count < 2) {
                Player.Message(p, "Sorry, there aren't enough players to play."); return;
            }
            
            Server.Countdown.playersleftlist = Server.Countdown.players;
            CountdownGame game = Server.Countdown;
            switch (par1) {
                case "slow":
                    game.speed = 800; game.speedtype = "slow"; break;
                case "normal":
                    game.speed = 650; game.speedtype = "normal"; break;
                case "fast":
                    game.speed = 500; game.speedtype = "fast"; break;
                case "extreme":
                    game.speed = 300; game.speedtype = "extreme"; break;
                case "ultimate":
                    game.speed = 150; game.speedtype = "ultimate"; break;
                default:
                    Player.Message(p, "You didn't specify a speed, resorting to 'normal'");
                    game.speed = 650; game.speedtype = "normal"; break;
            }
            Server.Countdown.freezemode = (par2 == "freeze" || par2 == "frozen");
            Server.Countdown.GameStart(p);
        }
        
        void HandleReset(Player p, string par1) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            switch (Server.Countdown.gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.Message(p, "Please enable countdown first."); break;
                case CountdownGameStatus.AboutToStart:
                    Player.Message(p, "Sorry - The game is about to start"); break;
                case CountdownGameStatus.InProgress:
                    Player.Message(p, "Sorry - The game is already in progress."); break;
                default:
                    Player.Message(p, "Reseting");
                    if (par1 == "map")
                        Server.Countdown.Reset(p, false);
                    else if (par1 == "all")
                        Server.Countdown.Reset(p, true);
                    else
                        Player.Message(p, "Please specify whether it is 'map' or 'all'");
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
