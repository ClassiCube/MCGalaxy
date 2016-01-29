/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System;
using System.IO;
namespace MCGalaxy.Commands {
    
    public sealed class CmdGoto : Command {
        
        public override string name { get { return "goto"; } }
        public override string shortcut { get { return "g"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdGoto() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") { Help(p); return; }

            Level lvl = Level.FindExact(message);
            if (lvl != null) {
                GoToLevel(p, lvl, message);
            } else if (Server.AutoLoad) {
                if (!File.Exists("levels/" + message + ".lvl")) {
                    lvl = Level.Find(message);
                    if (lvl == null) {
                        Player.SendMessage(p, "Level \"" + message + "\" doesn't exist! Did you mean...");
                        Command.all.Find("search").Use(p, "levels " + message);
                    } else {
                        GoToLevel(p, lvl, message);
                    }
                } else if (Level.CheckLoadOnGoto(message)) {
                    Command.all.Find("load").Use(p, message);
                    lvl = Level.Find(message);
                    if (lvl != null) {
                        GoToLevel(p, lvl, message);
                    }
                } else {
                    if (lvl == null) {
                         Player.SendMessage(p, "Level \"" + message + "\" cannot be loaded using /goto!");
                    } else {
                        GoToLevel(p, lvl, message);
                    }                
                }
            } else {
                lvl = Level.Find(message);
                if (lvl == null) {
                    Player.SendMessage(p, "There is no level \"" + message + "\" loaded. Did you mean..");
                    Command.all.Find("search").Use(p, "levels " + message);
                } else {
                    GoToLevel(p, lvl, message);
                }
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        
        void GoToLevel(Player p, Level lvl, string message) {
            if (p.level == lvl) { Player.SendMessage(p, "You are already in \"" + lvl.name + "\"."); return; }
            if (Player.BlacklistCheck(p.name, message)) {
                Player.SendMessage(p, "You are blacklisted from " + lvl.name + "."); return;
            }
            if (!p.ignorePermission && p.group.Permission < lvl.permissionvisit) {
                Player.SendMessage(p, "You're not allowed to go to " + lvl.name + "."); return;
            }
            if (!p.ignorePermission && p.group.Permission > lvl.pervisitmax && !p.group.CanExecute(Command.all.Find("pervisitmax"))) {
                Player.SendMessage(p, "Your rank must be " + lvl.pervisitmax + " or lower to go there!"); return;
            }
            if (File.Exists("text/lockdown/map/" + message.ToLower())) {
                Player.SendMessage(p, "The level " + message + " is locked."); return;
            }

            p.Loading = true;
            foreach (Player pl in Player.players) if (p.level == pl.level && p != pl) p.SendDespawn(pl.id);
            foreach (PlayerBot b in PlayerBot.playerbots) if (p.level == b.level) p.SendDespawn(b.id);

            Player.GlobalDespawn(p, true);
            Level oldLevel = p.level;
            p.level = lvl; p.SendUserMOTD(); p.SendMap(oldLevel);

            GC.Collect();

            ushort x = (ushort)((0.5 + lvl.spawnx) * 32);
            ushort y = (ushort)((1 + lvl.spawny) * 32);
            ushort z = (ushort)((0.5 + lvl.spawnz) * 32);

            if (!p.hidden)
                Player.GlobalSpawn(p, x, y, z, lvl.rotx, lvl.roty, true, "");
            else
                p.SendPos(0xFF, x, y, z, lvl.rotx, lvl.roty);

            foreach (Player pl in Player.players)
                if (pl.level == p.level && p != pl && !pl.hidden)
                    p.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);
            foreach (PlayerBot b in PlayerBot.playerbots)
                if (b.level == p.level)
                    p.SendSpawn(b.id, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);
            
            p.Loading = false;
            bool unloadOld = true;
            if (oldLevel.unload && !oldLevel.name.Contains("&cMuseum ")) {
                foreach (Player pl in Player.players) if (pl.level == oldLevel) { unloadOld = false; break; }
                if (unloadOld && Server.AutoLoad) oldLevel.Unload(true);
            }
            
            if (!p.hidden) {
                Player.GlobalChat(p, p.color + "*" + p.DisplayName + Server.DefaultColor + " went to &b" + lvl.name, false);
                Server.IRC.Say(p.color + p.DisplayName + " %rwent to &8" + lvl.name, false, true);
            }
        }
        
        void CheckGamesJoin(Player p, Level lvl) {
            if (Server.lava.active && !Server.lava.sendingPlayers && Server.lava.map == p.level) {
                if (Server.lava.roundActive) {
                    Server.lava.AnnounceRoundInfo(p);
                    Server.lava.AnnounceTimeLeft(!Server.lava.flooded, true, p);
                } else {
                    Player.SendMessage(p, "Vote for the next map!");
                    Player.SendMessage(p, "Choices: " + Server.lava.VoteString);
                }
            }

            if (Server.zombie.GameInProgess()) {
                if (p.level.name == Server.zombie.currentLevelName)
                    Server.zombie.InfectedPlayerLogin(p);
            }

            if (p.level.name != Server.zombie.currentLevelName) {
                if(ZombieGame.alive.Contains(p))
                    ZombieGame.alive.Remove(p);
                if (ZombieGame.infectd.Contains(p))
                    ZombieGame.infectd.Remove(p);
            }
            
            if (p.inTNTwarsMap)
                p.canBuild = true;
            TntWarsGame game = TntWarsGame.Find(p.level);
            if (game != null) {
                if (game.GameStatus != TntWarsGame.TntWarsGameStatus.Finished &&
                    game.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                    p.canBuild = false;
                    Player.SendMessage(p, "TNT Wars: Disabled your building because you are in a TNT Wars map!");
                }
                p.inTNTwarsMap = true;
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/goto <mapname> - Teleports yourself to a different level.");
        }
    }
}
