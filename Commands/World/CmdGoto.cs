/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands {
    
    public sealed class CmdGoto : Command {
        
        public override string name { get { return "goto"; } }
        public override string shortcut { get { return "g"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("j"), new CommandAlias("join") }; }
        }
        public CmdGoto() { }

        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") { Help(p); return; }            
            if (p.usingGoto) { Player.SendMessage(p, "Cannot use /goto, already loading a map."); return; }
            
            Level oldLevel = p.level;
            p.usingGoto = true;
            bool didJoin = false;
            try {
                didJoin = HandleGoto(p, message);
            } finally {
                p.usingGoto = false;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            if (!didJoin) return;
            bool unloadOld = true;
            if (oldLevel.unload && !oldLevel.name.Contains("&cMuseum ")) {
                Player[] players = PlayerInfo.Online.Items; 
                foreach (Player pl in players) 
                    if (pl.level == oldLevel) { unloadOld = false; break; }
                if (unloadOld && Server.AutoLoad) oldLevel.Unload(true);
            }
        }
        
        bool HandleGoto(Player p, string message) {
            Level lvl = LevelInfo.FindExact(message);
            if (lvl != null) {
                return GoToLevel(p, lvl, message);
            } else if (Server.AutoLoad) {
                if (!LevelInfo.ExistsOffline(message)) {
                    lvl = LevelInfo.Find(message);
                    if (lvl == null) {
                        Player.SendMessage(p, "Level \"" + message + "\" doesn't exist! Did you mean...");
                        Command.all.Find("search").Use(p, "levels " + message);
                        return false;
                    } else {
                        return GoToLevel(p, lvl, message);
                    }
                } else if (Level.CheckLoadOnGoto(message)) {
                    Command.all.Find("load").Use(p, message);
                    lvl = LevelInfo.Find(message);
                    if (lvl != null) {
                        return GoToLevel(p, lvl, message);
                    } else {
                        Player.SendMessage(p, "Level \"" + message + "\" failed to be auto-loaded.");
                        return false;
                    }
                } else {
                    if (lvl == null) {
                        Player.SendMessage(p, "Level \"" + message + "\" cannot be loaded using /goto.");
                        return false;
                    } else {
                        return GoToLevel(p, lvl, message);
                    }
                }
            } else {
                lvl = LevelInfo.Find(message);
                if (lvl == null) {
                    Player.SendMessage(p, "There is no level \"" + message + "\" loaded. Did you mean..");
                    Command.all.Find("search").Use(p, "levels " + message);
                    return false;
                } else {
                    return GoToLevel(p, lvl, message);
                }
            }
        }
        
        bool GoToLevel(Player p, Level lvl, string message) {
            if (p.level == lvl) { Player.SendMessage(p, "You are already in \"" + lvl.name + "\"."); return false; }
            if (Player.BlacklistCheck(p.name, message) || lvl.VisitBlacklist.CaselessContains(p.name)) {
                Player.SendMessage(p, "You are blacklisted from " + lvl.name + "."); return false;
            }
            
            bool whitelisted = lvl.VisitWhitelist.CaselessContains(p.name);
            if (!p.ignorePermission && !whitelisted && p.group.Permission < lvl.permissionvisit) {
                Player.SendMessage(p, "You're not allowed to go to " + lvl.name + "."); return false;
            }
            if (!p.ignorePermission && !whitelisted && p.group.Permission > lvl.pervisitmax 
                && !p.group.CanExecute("pervisitmax")) {
                Player.SendMessage(p, "Your rank must be " + lvl.pervisitmax + " or lower to go there!"); return false;
            }
            if (File.Exists("text/lockdown/map/" + message.ToLower())) {
                Player.SendMessage(p, "The level " + message + " is locked."); return false;
            }
            if (!Server.zombie.PlayerCanJoinLevel(p, lvl, p.level)) return false;

            p.Loading = true;
            CmdReveal.DespawnEntities(p);
            Level oldLevel = p.level;
            p.level = lvl; p.SendUserMOTD(); p.SendMap(oldLevel);

            ushort x = (ushort)((0.5 + lvl.spawnx) * 32);
            ushort y = (ushort)((1 + lvl.spawny) * 32);
            ushort z = (ushort)((0.5 + lvl.spawnz) * 32);
            SpawnEntities(p, x, y, z, lvl.rotx, lvl.roty);
            p.Loading = false;
            CheckGamesJoin(p, oldLevel);
            
            bool showJoin = p.level.ShouldSaveChanges() || (oldLevel != null && oldLevel.ShouldSaveChanges());
            if (!p.hidden && showJoin) {
                Player.SendChatFrom(p, p.color + "*" + p.DisplayName + " %Swent to &b" + lvl.name, false);
                Server.IRC.Say(p.color + p.DisplayName + " %rwent to &8" + lvl.name, false, true);
            }
            return true;
        }
        
        internal static void SpawnEntities(Player p, ushort x, ushort y, ushort z, byte rotX, byte rotY) {
        	Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
        		if (pl.level != p.level || !Entities.CanSeeEntity(p, pl) || p == pl) continue;
                p.SpawnEntity(pl, pl.id, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1], "");
            }           
            Entities.GlobalSpawn(p, x, y, z, rotX, rotY, true);
            
            PlayerBot[] bots = PlayerBot.Bots.Items;
            foreach (PlayerBot b in bots)
            	if (b.level == p.level) Entities.Spawn(p, b);
        }
        
        internal static void CheckGamesJoin(Player p, Level oldLvl) {
            Server.lava.PlayerJoinedLevel(p, p.level, oldLvl);
            Server.zombie.PlayerJoinedLevel(p, p.level, oldLvl);
 
            if (p.inTNTwarsMap) p.canBuild = true;
            TntWarsGame game = TntWarsGame.Find(p.level);
            if (game == null) return;
            
            if (game.GameStatus != TntWarsGame.TntWarsGameStatus.Finished &&
                game.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                p.canBuild = false;
                Player.SendMessage(p, "TNT Wars: Disabled your building because you are in a TNT Wars map!");
            }
            p.inTNTwarsMap = true;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/goto <mapname>");
            Player.SendMessage(p, "%HTeleports yourself to a different level.");
        }
    }
}
