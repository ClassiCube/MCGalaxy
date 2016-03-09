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

namespace MCGalaxy.Commands {
    
    public sealed class CmdCountdown : Command {
        
        public override string name { get { return "countdown"; } }
        public override string shortcut { get { return "cd"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "The lowest rank that can send the countdown rules to everybody", 1),
                    new CommandPerm(LevelPermission.Operator, "The lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)", 2),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (p == null) { MessageInGameOnly(p); return; }

            string[] args = message.ToLower().Split(' ');
            string cmd = args[0], arg1 = "", arg2 = "", arg3 = "";
            if (args.Length > 1) arg1 = args[1];
            if (args.Length > 2) arg2 = args[2];
            if (args.Length > 3) arg3 = args[3]; 
            
            switch (cmd) {
                case "help":
                    Command.all.Find("help").Use(p, "countdown"); return;
                case "goto":
                    Command.all.Find("goto").Use(p, "countdown"); return;
                case "join":
                    HandleJoin(p); return;
                case "leave":
                    HandleLeave(p); return;
                case "players":
                    HandlePlayers(p); return;
                case "rules":
                    HandleRules(p, arg1); return;
                default:
                    break;
            }

            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2)) {
                switch (cmd) {
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
                }
            }
            p.SendMessage("Sorry, you aren't a high enough rank or that wasn't a correct command addition.");
        }
        
        void HandleJoin(Player p) {
            switch (Server.Countdown.gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.SendMessage(p, "Sorry - Countdown isn't enabled yet");
                    return;
                case CountdownGameStatus.Enabled:
                    Server.Countdown.PlayerJoinedGame(p);
                    return;
                case CountdownGameStatus.AboutToStart:
                    Player.SendMessage(p, "Sorry - The game is about to start");
                    return;
                case CountdownGameStatus.InProgress:
                    Player.SendMessage(p, "Sorry - The game is already in progress.");
                    return;
                case CountdownGameStatus.Finished:
                    Player.SendMessage(p, "Sorry - The game has finished. Get an op to reset it.");
                    return;
            }
        }
        
        void HandleLeave(Player p) {
            if (Server.Countdown.players.Contains(p)) {
                switch (Server.Countdown.gamestatus) {
                    case CountdownGameStatus.Disabled:
                        Player.SendMessage(p, "Sorry - Countdown isn't enabled yet");
                        return;
                    case CountdownGameStatus.Enabled:
                        Player.SendMessage(p, "You've left the game.");
                        Server.Countdown.PlayerLeftGame(p);
                        break;
                    case CountdownGameStatus.AboutToStart:
                        Player.SendMessage(p, "Sorry - The game is about to start");
                        return; ;
                    case CountdownGameStatus.InProgress:
                        Player.SendMessage(p, "Sorry - you are in a game that is in progress, please wait till its finished or till you've died.");
                        return;
                    case CountdownGameStatus.Finished:
                        Server.Countdown.players.Remove(p);
                        Server.Countdown.playersleftlist.Remove(p);
                        p.playerofcountdown = false;
                        Player.SendMessage(p, "You've left the game.");
                        break;
                }
            } else if (!(Server.Countdown.playersleftlist.Contains(p)) && Server.Countdown.players.Contains(p)) {
                Server.Countdown.players.Remove(p);
                Player.SendMessage(p, "You've left the game.");
            } else {
                Player.SendMessage(p, "You haven't joined the game yet!!");
            }
        }
        
        void HandlePlayers(Player p) {
            switch (Server.Countdown.gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.SendMessage(p, "The game has not been enabled yet.");
                    break;

                case CountdownGameStatus.Enabled:
                    Player.SendMessage(p, "Players who have joined:");
                    foreach (Player plya in Server.Countdown.players)
                        Player.SendMessage(p, plya.color + plya.name);
                    break;

                case CountdownGameStatus.AboutToStart:
                    Player.SendMessage(p, "Players who are about to play:");
                    foreach (Player plya in Server.Countdown.players)
                        Player.SendMessage(p, plya.color + plya.name);
                    break;

                case CountdownGameStatus.InProgress:
                    Player.SendMessage(p, "Players left playing:");
                    foreach (Player plya in Server.Countdown.players) {
                        if (Server.Countdown.playersleftlist.Contains(plya))
                            Player.SendMessage(p, plya.color + plya.name + Server.DefaultColor + " who is &aIN");
                        else
                            Player.SendMessage(p, plya.color + plya.name + Server.DefaultColor + " who is &cOUT");
                    }
                    break;

                case CountdownGameStatus.Finished:
                    Player.SendMessage(p, "Players who were playing:");
                    foreach (Player plya in Server.Countdown.players)
                        Player.SendMessage(p, plya.color + plya.name);
                    break;
            }
        }
        
        void HandleRules(Player p, string target) {
            bool hasPerm = (int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1);
            if (target == "" || !hasPerm) {
                Player.SendMessage(p, "The aim of the game is to stay alive the longest.");
                Player.SendMessage(p, "Don't fall in the lava!!");
                Player.SendMessage(p, "Blocks on the ground will disapear randomly, first going yellow, then orange, then red and finally disappering.");
                Player.SendMessage(p, "The last person alive will win!!");
            } else if (hasPerm) {
                if (target == "all") {
                    Player.GlobalMessage("Countdown Rules being sent to everyone by " + p.color + p.name + ":");
                    Player.GlobalMessage("The aim of the game is to stay alive the longest.");
                    Player.GlobalMessage("Don't fall in the lava!!");
                    Player.GlobalMessage("Blocks on the ground will disapear randomly, first going yellow, then orange, then red and finally disappering.");
                    Player.GlobalMessage("The last person alive will win!!");
                    Player.SendMessage(p, "Countdown rules sent to everyone");
                    return;
                } else if (target == "map") {
                    Chat.GlobalMessageLevel(p.level, "Countdown Rules being sent to " + p.level.name + " by " + p.color + p.name + ":");
                    Chat.GlobalMessageLevel(p.level, "The aim of the game is to stay alive the longest.");
                    Chat.GlobalMessageLevel(p.level, "Don't fall in the lava!!");
                    Chat.GlobalMessageLevel(p.level, "Blocks on the ground will disapear randomly, first going yellow, then orange, then red and finally disappering.");
                    Chat.GlobalMessageLevel(p.level, "The last person alive will win!!");
                    Player.SendMessage(p, "Countdown rules sent to: " + p.level.name);
                    return;
                }

                Player who = PlayerInfo.Find(target);
                if (who == null) {
                    Player.SendMessage(p, "That wasn't an online player."); return;
                } else if (p.group.Permission < who.group.Permission) {
                    Player.SendMessage(p, "You can't send rules to someone of a higher rank than yourself!!"); return;
                } else {
                    Player.SendMessage(who, "Countdown rules sent to you by " + p.color + p.name);
                    Player.SendMessage(who, "The aim of the game is to stay alive the longest.");
                    Player.SendMessage(who, "Don't fall in the lava!!");
                    Player.SendMessage(who, "Blocks on the ground will disapear randomly, first going yellow, then orange, then red and finally disawhowhoering.");
                    Player.SendMessage(who, "The last person alive will win!!");
                    Player.SendMessage(p, "Countdown rules sent to: " + who.color + who.name);
                }
            }
        }
        
        void HandleGenerate(Player p, string arg1, string arg2, string arg3) {
            int width, height, length;
            if(!int.TryParse(arg1, out width) || !int.TryParse(arg2, out height) || !int.TryParse(arg3, out length)) {
                width = 32; height = 32; length = 32;
            }
            if (width < 32 || !MapGen.OkayAxis(width)) width = 32;
            if (height < 32 || !MapGen.OkayAxis(height)) height = 32;
            if (length < 32 || !MapGen.OkayAxis(length)) length = 32;

            Level oldLevel = LevelInfo.FindExact("countdown");
            if (oldLevel != null) {
                oldLevel.permissionbuild = LevelPermission.Guest;
                Command.all.Find("deletelvl").Use(p, "countdown");
            }
            Level lvl = CountdownMapGen.Generate(width, height, length);
            lvl.Save();
            if (Server.Countdown.gamestatus != CountdownGameStatus.Disabled)
            	Server.Countdown.mapon = lvl;
            
            const string format = "Generated map ({0}x{1}x{2}), sending you to it..";
            Player.SendMessage(p, String.Format(format, width, height, length));
            Command.all.Find("load").Use(p, "countdown");
            Command.all.Find("goto").Use(p, "countdown");

            p.level.permissionbuild = LevelPermission.Nobody;
            p.level.motd = "Welcome to the Countdown map!!!! -hax";
            const ushort x = 8 * 32 + 16;
            const ushort y = 23 * 32 + 32;
            const ushort z = 17 * 32 + 16;
            p.SendPos(0xFF, x, y, z, p.rot[0], p.rot[1]);
        }
        
        void HandleEnable(Player p) {
            if (Server.Countdown.gamestatus == CountdownGameStatus.Disabled) {
                Command.all.Find("load").Use(null, "countdown");
                Server.Countdown.mapon = LevelInfo.FindExact("countdown");
                if (Server.Countdown.mapon == null ) {
                    Player.SendMessage(p, "countdown level not found, generating..");
                    HandleGenerate(p, "", "", "");
                    Server.Countdown.mapon = LevelInfo.FindExact("countdown");
                }
                
                Server.Countdown.gamestatus = CountdownGameStatus.Enabled;
                Player.GlobalMessage("Countdown has been enabled!!");
            } else {
                Player.SendMessage(p, "A Game is either already enabled or is already progress");
            }
        }
        
        void HandleDisable(Player p) {
            if (Server.Countdown.gamestatus == CountdownGameStatus.AboutToStart || Server.Countdown.gamestatus == CountdownGameStatus.InProgress) {
                Player.SendMessage(p, "A game is currently in progress - please wait until it is finished, or use '/cd cancel' to cancel the game"); return;
            } else if (Server.Countdown.gamestatus == CountdownGameStatus.Disabled) {
                Player.SendMessage(p, "Already disabled!!"); return;
            } else {
                foreach (Player pl in Server.Countdown.players)
                    Player.SendMessage(pl, "The countdown game was disabled.");
                Server.Countdown.Reset(p, true);
                Server.Countdown.gamestatus = CountdownGameStatus.Disabled;
                Player.SendMessage(p, "Countdown Disabled");
            }
        }
        
        void HandleCancel(Player p) {
            if (Server.Countdown.gamestatus == CountdownGameStatus.AboutToStart || Server.Countdown.gamestatus == CountdownGameStatus.InProgress) {
                Server.Countdown.cancel = true;
                Thread.Sleep(1500);
                Player.SendMessage(p, "Countdown has been canceled");
                Server.Countdown.gamestatus = CountdownGameStatus.Enabled;
            } else if (Server.Countdown.gamestatus == CountdownGameStatus.Disabled) {
                Player.SendMessage(p, "The game is disabled!!");
            } else {
                foreach (Player pl in Server.Countdown.players)
                    Player.SendMessage(pl, "The countdown game was canceled");
                Server.Countdown.Reset(null, true);
            }
        }
        
        void HandleStart(Player p, string par1, string par2) {
            if (Server.Countdown.gamestatus != CountdownGameStatus.Enabled) {
                Player.SendMessage(p, "Either a game is already in progress or it hasn't been enabled"); return;
            }
            if (Server.Countdown.players.Count < 2) {
                Player.SendMessage(p, "Sorry, there aren't enough players to play."); return;
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
                    p.SendMessage("You didn't specify a speed, resorting to 'normal'");
                    game.speed = 650; game.speedtype = "normal"; break;
            }
            Server.Countdown.freezemode = (par2 == "freeze" || par2 == "frozen");
            Server.Countdown.GameStart(p);
        }
        
        void HandleReset(Player p, string par1) {
            switch (Server.Countdown.gamestatus) {
                case CountdownGameStatus.Disabled:
                    Player.SendMessage(p, "Please enable countdown first."); break;
                case CountdownGameStatus.AboutToStart:
                    Player.SendMessage(p, "Sorry - The game is about to start"); break;
                case CountdownGameStatus.InProgress:
                    Player.SendMessage(p, "Sorry - The game is already in progress."); break;
                default:
                    Player.SendMessage(p, "Reseting");
                    if (par1 == "map")
                        Server.Countdown.Reset(p, false);
                    else if (par1 == "all")
                        Server.Countdown.Reset(p, true);
                    else
                        Player.SendMessage(p, "Please specify whether it is 'map' or 'all'");
                    break;
            }
        }
        
        void HandleTutorial(Player p) {
            p.SendMessage("First, generate the map using /cd generate");
            p.SendMessage("Next, type /cd enable to enable the game mode");
            p.SendMessage("Next, type /cd join to join the game and tell other players to join aswell");
            p.SendMessage("When some people have joined, type /cd start [speed] to start it");
            p.SendMessage("[speed] can be 'ultimate', 'extreme', 'fast', 'normal' or 'slow'");
            p.SendMessage("When you are done, type /cd reset [map/all]");
            p.SendMessage("use map to reset only the map and all to reset everything.");
        }
        
        public override void Help(Player p) {
            p.SendMessage("/cd join - join the game");
            p.SendMessage("/cd leave - leave the game");
            p.SendMessage("/cd goto - goto the countdown map");
            p.SendMessage("/cd players - view players currently playing");
            
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1))
                p.SendMessage("/cd rules <all/map/player> - the rules of countdown. with send: all to send to all, map to send to map and have a player's name to send to a player");
            else
                p.SendMessage("/cd rules - view the rules of countdown");
            
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2)) {
                p.SendMessage("/cd generate [width] [height] [length] - generates the countdown map (default size is 32x32x32)");
                p.SendMessage("/cd enable - enable the game");
                p.SendMessage("/cd disable - disable the game");
                p.SendMessage("/cd cancel - cancels a game");
                p.SendMessage("/cd start [speed] [mode] - start the game, speeds are 'slow', 'normal', 'fast', 'extreme' and 'ultimate', modes are 'normal' and 'freeze'");
                p.SendMessage("/cd reset [all/map] - reset the whole game (all) or only the map (map)");
                p.SendMessage("/cd tutorial - a tutorial on how to setup countdown");
            }
        }
    }
}
