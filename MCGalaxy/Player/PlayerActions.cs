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
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Commands.World;

namespace MCGalaxy {
    public static class PlayerActions {
        
        public static bool ChangeMap(Player p, string name) { return ChangeMap(p, null, name); }
        public static bool ChangeMap(Player p, Level lvl)   { return ChangeMap(p, lvl, null); }
        
        static bool ChangeMap(Player p, Level lvl, string name) {
            if (Interlocked.CompareExchange(ref p.UsingGoto, 1, 0) == 1) {
                p.Message("Cannot use /goto, already joining a map."); return false;
            }
            Level oldLevel = p.level;
            bool didJoin   = false;
            
            try {
                didJoin = name == null ? GotoLevel(p, lvl) : GotoMap(p, name);
            } finally {
                Interlocked.Exchange(ref p.UsingGoto, 0);
                Server.DoGC();
            }
            
            if (!didJoin) return false;
            oldLevel.AutoUnload();
            return true;
        }
        
        
        static bool GotoMap(Player p, string name) {
            Level lvl = LevelInfo.FindExact(name);
            if (lvl != null) return GotoLevel(p, lvl);
            
            if (Server.Config.AutoLoadMaps) {
                string map = Matcher.FindMaps(p, name);
                if (map == null) return false;
                
                lvl = LevelInfo.FindExact(map);
                if (lvl != null) return GotoLevel(p, lvl);
                return LoadOfflineLevel(p, map);
            } else {
                lvl = Matcher.FindLevels(p, name);
                if (lvl == null) {
                    p.Message("There is no level \"{0}\" loaded. Did you mean..", name);
                    Command.Find("Search").Use(p, "levels " + name);
                    return false;
                }
                return GotoLevel(p, lvl);
            }
        }
        
        static bool LoadOfflineLevel(Player p, string map) {
            string propsPath = LevelInfo.PropsPath(map);
            LevelConfig cfg = new LevelConfig();
            cfg.Load(propsPath);
            
            if (!cfg.LoadOnGoto) {
                p.Message("Level \"{0}\" cannot be loaded using &T/Goto.", map);
                return false;
            }
            
            AccessController visitAccess = new LevelAccessController(cfg, map, true);
            bool skip = p.summonedMap != null && p.summonedMap.CaselessEq(map);
            LevelPermission plRank = skip ? LevelPermission.Nobody : p.Rank;
            if (!visitAccess.CheckDetailed(p, plRank)) return false;
            
            LevelActions.Load(p, map, false);
            Level lvl = LevelInfo.FindExact(map);
            if (lvl != null) return GotoLevel(p, lvl);

            p.Message("Level \"{0}\" failed to be auto-loaded.", map);
            return false;
        }
        
        static bool GotoLevel(Player p, Level lvl) {
            if (p.level == lvl) { p.Message("You are already in {0}&S.", lvl.ColoredName); return false; }
            
            bool canJoin = lvl.CanJoin(p);
            OnJoiningLevelEvent.Call(p, lvl, ref canJoin);
            if (!canJoin) return false;

            p.Loading = true;
            Entities.DespawnEntities(p);
            Level prev = p.level; p.level = lvl;
            
            p.SendRawMap(prev, lvl);
            PostSentMap(p, prev, lvl, true);
            p.Loading = false;
            return true;
        }
        
        internal static void ReloadMap(Player p) {
            p.Loading = true;
            Entities.DespawnEntities(p);
            p.SendRawMap(p.level, p.level);
            Entities.SpawnEntities(p, p.Pos, p.Rot);
            p.Loading = false;
        }
        
        internal static void PostSentMap(Player p, Level prev, Level lvl, bool announce) {
            Position pos = lvl.SpawnPos;
            Orientation rot = p.Rot;
            byte yaw = lvl.rotx, pitch = lvl.roty;
            // in case player disconnected mid-way through loading map
            if (p.Socket.Disconnected) return;
            
            OnPlayerSpawningEvent.Call(p, ref pos, ref yaw, ref pitch, false);
            rot.RotY = yaw; rot.HeadX = pitch;
            p.Pos = pos;
            p.SetYawPitch(yaw, pitch);
            if (p.Socket.Disconnected) return;
            
            Entities.SpawnEntities(p, pos, rot);
            OnJoinedLevelEvent.Call(p, prev, lvl, ref announce);
            if (!announce || !Server.Config.ShowWorldChanges) return;
            
            announce = !p.hidden && Server.Config.IRCShowWorldChanges;
            string msg = p.level.IsMuseum ? "λNICK &Swent to the " : "λNICK &Swent to ";
            Chat.MessageFrom(ChatScope.All, p, msg + lvl.ColoredName,
                             null, FilterGoto(p, prev, lvl), announce);
        }
        
        static ChatMessageFilter FilterGoto(Player source, Level prev, Level lvl) {
            return (pl, obj) => 
                pl.CanSee(source) && !pl.Ignores.WorldChanges &&
                (Chat.FilterGlobal(pl, obj) || Chat.FilterLevel(pl, prev) || Chat.FilterLevel(pl, lvl));
        }
        
        public static void Respawn(Player p) {
            bool cpSpawn = p.useCheckpointSpawn;
            Position pos;
            
            pos.X = 16 + (cpSpawn ? p.checkpointX : p.level.spawnx) * 32;
            pos.Y = 32 + (cpSpawn ? p.checkpointY : p.level.spawny) * 32;
            pos.Z = 16 + (cpSpawn ? p.checkpointZ : p.level.spawnz) * 32;
            byte yaw   = cpSpawn ? p.checkpointRotX : p.level.rotx;
            byte pitch = cpSpawn ? p.checkpointRotY : p.level.roty;
            OnPlayerSpawningEvent.Call(p, ref pos, ref yaw, ref pitch, true);
            
            p.SendPos(Entities.SelfID, pos, new Orientation(yaw, pitch));
        }
    }
}
