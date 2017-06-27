/*
    Copyright 2011 MCForge
        
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
using System.Threading;
using MCGalaxy.Games;
using MCGalaxy.Commands.World;

namespace MCGalaxy {
    public static class PlayerActions {
        
        /// <summary> Moves the player to the specified block coordinates. (bY is treated as player feet) </summary>
        public static void MoveCoords(Player p, int bX, int bY, int bZ,
                                      byte rotX, byte rotY) {
            Position pos = Position.FromFeet(16 + bX * 32, bY * 32, 16 + bZ * 32);
            p.SendPos(Entities.SelfID, pos, new Orientation(rotX, rotY));
        }
        
        /// <summary> Moves the player to the specified map. </summary>
        public static bool ChangeMap(Player p, string name) { 
            return ChangeMap(p, null, name); 
        }
        
        /// <summary> Moves the player to the specified map. </summary>
        public static bool ChangeMap(Player p, Level lvl) { 
            return ChangeMap(p, lvl, null); 
        }
        
        static bool ChangeMap(Player p, Level lvl, string name) {
            if (Interlocked.CompareExchange(ref p.UsingGoto, 1, 0) == 1) {
                Player.Message(p, "Cannot use /goto, already joining a map."); return false; 
            }
            Level oldLevel = p.level;
            bool didJoin = false;
            
            try {
                didJoin = name == null ? GotoLevel(p, lvl) : GotoMap(p, name);
            } finally {
                Interlocked.Exchange(ref p.UsingGoto, 0);
                Server.DoGC();
            }
            
            if (!didJoin) return false;
            Unload(oldLevel);
            return true;
        }
        
        
        static bool GotoMap(Player p, string name) {
            Level lvl = LevelInfo.FindExact(name);
            if (lvl != null) return GotoLevel(p, lvl);
            
            if (ServerConfig.AutoLoad) {
                string map = Matcher.FindMaps(p, name);
                if (map == null) return false;
                
                lvl = LevelInfo.FindExact(map);
                if (lvl != null) return GotoLevel(p, lvl);
                return LoadOfflineLevel(p, map);
            } else {
                lvl = Matcher.FindLevels(p, name);
                if (lvl == null) {
                    Player.Message(p, "There is no level \"{0}\" loaded. Did you mean..", name);
                    Command.all.Find("search").Use(p, "levels " + name);
                    return false;
                }
                return GotoLevel(p, lvl);
            }
        }
        
        static bool LoadOfflineLevel(Player p, string name) {
            if (!Level.CheckLoadOnGoto(name)) {
                Player.Message(p, "Level \"{0}\" cannot be loaded using /goto.", name);
                return false;
            }
            
            CmdLoad.LoadLevel(p, name, true);
            Level lvl = LevelInfo.FindExact(name);
            if (lvl != null) return GotoLevel(p, lvl);

            Player.Message(p, "Level \"{0}\" failed to be auto-loaded.", name);
            return false;
        }
        
        static bool GotoLevel(Player p, Level lvl) {
            if (p.level == lvl) { Player.Message(p, "You are already in {0}%S.", lvl.ColoredName); return false; }
            if (!lvl.CanJoin(p)) return false;
            if (!Server.zombie.PlayerCanJoinLevel(p, lvl, p.level)) return false;

            p.Loading = true;
            Entities.DespawnEntities(p);
            Level oldLevel = p.level;
            p.level = lvl; 
            p.SendMap(oldLevel);

            Entities.SpawnEntities(p, lvl.SpawnPos, lvl.SpawnRot);
            CheckGamesJoin(p, oldLevel);
            
            if (p.level.ShouldShowJoinMessage(oldLevel)) {
                string msg = p.level.IsMuseum ? " %Swent to the " : " %Swent to ";
                Chat.MessageGlobal(p, p.ColoredName + msg + lvl.ColoredName, false, true);
                Player.RaisePlayerAction(p, PlayerAction.JoinWorld, lvl.name);
            }
            return true;
        }
        
        internal static void CheckGamesJoin(Player p, Level oldLvl) {
            Server.lava.PlayerJoinedLevel(p, p.level, oldLvl);
            Server.zombie.PlayerJoinedLevel(p, p.level, oldLvl);
            if (Server.ctf != null) Server.ctf.SpawnPlayer(p);
            
            if (p.inTNTwarsMap) p.canBuild = true;
            TntWarsGame game = TntWarsGame.Find(p.level);
            if (game == null) return;
            
            if (game.GameStatus != TntWarsGame.TntWarsGameStatus.Finished &&
                game.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                p.canBuild = false;
                Player.Message(p, "TNT Wars: Disabled your building because you are in a TNT Wars map!");
            }
            p.inTNTwarsMap = true;
        }
        
        static void Unload(Level lvl) {
            bool unloadOld = true;
            if (lvl.IsMuseum || !lvl.unload) return;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == lvl) { unloadOld = false; break; }
            }
            if (unloadOld && ServerConfig.AutoLoad) lvl.Unload(true);
        }
    }
}
