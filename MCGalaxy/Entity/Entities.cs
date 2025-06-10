/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
Dual-licensed under the Educational Community License, Version 2.0 and
the GNU General Public License, Version 3 (the "Licenses"); you may
not use this file except in compliance with the Licenses. You may
obtain a copy of the Licenses at
https://opensource.org/license/ecl-2-0/
https://www.gnu.org/licenses/gpl-3.0.html
Unless required by applicable law or agreed to in writing,
software distributed under the Licenses are distributed on an "AS IS"
BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
or implied. See the Licenses for the specific language governing
permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Network;
using MCGalaxy.Maths;

namespace MCGalaxy 
{
    /// <summary> Contains methods related to the management of entities (such as players). </summary>
    public static class Entities 
    {
        public const byte SelfID = 0xFF;
        public const ushort CharacterHeight = 51;
        
        #region Spawning / Despawning
        
        /// <summary> Respawns this player to all players (including self) that can see the player in the current world. </summary>
        public static void GlobalRespawn(Player p, bool self = true) {
            GlobalDespawn(p, self);
            GlobalSpawn(p, self);
        }
        
        /// <summary> Spawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalSpawn(Player p, bool self, string possession = "") {
            GlobalSpawn(p, p.Pos, p.Rot, self, possession);
        }
        
        /// <summary> Spawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalSpawn(Player p, Position pos, Orientation rot, bool self, string possession = "") {
            Player[] players = PlayerInfo.Online.Items;
            TabList.Update(p, self);
            
            foreach (Player other in players) {
                if ((other.Loading && p != other) || p.level != other.level) continue;
                
                if (p != other && other.CanSeeEntity(p)) {
                    Spawn(other, p, pos, rot, possession);
                } else if (p == other && self) {
                    other.Pos = pos; other.SetYawPitch(rot.RotY, rot.HeadX);
                    other._lastPos = other.Pos; other._lastRot = other.Rot;
                    Spawn(other, p, pos, rot, possession);
                }
            }
        }
        
        /// <summary> Despawns this player to all other players that can
        /// (or cannot if 'toVisible' is false) see the player in the current world. </summary>
        public static void GlobalDespawn(Player p, bool self, bool toVisible = true) {
            Player[] players = PlayerInfo.Online.Items;
            TabList.RemoveAll(p, self, toVisible);
            
            foreach (Player other in players) {
                if (p.level != other.level) continue;
                
                bool despawn = other.CanSeeEntity(p);
                if (!toVisible) despawn = !despawn;
                
                if (p != other && despawn) {
                    Despawn(other, p);
                } else if (p == other && self) {
                    Despawn(other, p);
                }
            }
        }


        /// <summary>
        /// Spawns p to dst and calls OnEntitySpawnedEvent
        /// </summary>
        public static void Spawn(Player dst, Player p) { Spawn(dst, p, p.Pos, p.Rot); }
        /// <summary>
        /// Spawns p to dst at the given pos and rot and calls OnEntitySpawnedEvent
        /// </summary>
        public static void Spawn(Player dst, Player p, Position pos,
                                 Orientation rot, string possession = "") {

            string name = p.color + p.truename + possession;
            string skin = p.SkinName, model = p.Model;
            OnEntitySpawnedEvent.Call(p, ref name, ref skin, ref model, dst);

            SpawnRaw(dst, p, pos, rot, skin, name, model);
            if (!Server.Config.TablistGlobal) TabList.Add(dst, p);
        }
        
        /// <summary> Spawns this player to all other players, and spawns all others players to this player. </summary>
        internal static void SpawnEntities(Player p, bool bots = true) { SpawnEntities(p, p.Pos, p.Rot, bots); }
        
        /// <summary> Spawns this player to all other players, and spawns all others players to this player. </summary>
        internal static void SpawnEntities(Player p, Position pos, Orientation rot, bool bots = true) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
                if (other.level != p.level || !p.CanSeeEntity(other) || p == other) continue;
                Spawn(p, other);
            }
            GlobalSpawn(p, pos, rot, true);

            if (!bots) return;
            PlayerBot[] botsList = p.level.Bots.Items;
            foreach (PlayerBot b in botsList) { Spawn(p, b); }
        }
        
        /// <summary> Despawns this player to all other players, and despawns all others players to this player. </summary>
        internal static void DespawnEntities(Player p, bool bots = true) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
                if (p.level == other.level && p != other) Despawn(p, other);
            }
            GlobalDespawn(p, true);
            
            if (!bots) return;
            PlayerBot[] botsList = p.level.Bots.Items;
            foreach (PlayerBot b in botsList) { Despawn(p, b); }
        }
        
        public static void Spawn(Player dst, PlayerBot b) {
            string name  = Chat.Format(b.color + b.DisplayName, dst, true, false);
            if (b.DisplayName.CaselessEq("empty")) name = "";
            string skin  = Chat.Format(b.SkinName, dst, true, false);
            string model = Chat.Format(b.Model, dst, true, false);
            
            OnEntitySpawnedEvent.Call(b, ref name, ref skin, ref model, dst);
            SpawnRaw(dst, b, b.Pos, b.Rot, skin, name, model);
            if (Server.Config.TablistBots) TabList.Add(dst, b);
        }
        
        static void SpawnRaw(Player p, Entity e, Position pos, Orientation rot,
                             string skin, string name, string model) {

            p.EntityMap.Add(e, pos, rot,
                             skin, name, model);
        }
        
        /// <summary>
        /// Despawns other to dst and calls OnEntityDespawnedEvent
        /// </summary>
        public static void Despawn(Player dst, Player other) {
            OnEntityDespawnedEvent.Call(other, dst);

            dst.EntityMap.Remove(other);

            if (!Server.Config.TablistGlobal) TabList.Remove(dst, other);
        }
        
        public static void Despawn(Player dst, PlayerBot b) {
            OnEntityDespawnedEvent.Call(b, dst);

            dst.EntityMap.Remove(b);

            if (Server.Config.TablistBots) TabList.Remove(dst, b);
        }

        #endregion
        
        public static void DisplayPosition(Player p, Entity src, string displayName) {
            Vec3S32 feet = src.Pos.FeetBlockCoords;
            int x = src.Pos.X, y = src.Pos.Y - Entities.CharacterHeight, z = src.Pos.Z;
            p.Message("{0} &Sis on {1}", displayName, src.Level.ColoredName);
            p.Message("     Block coords: &b{0} {1} {2}",
                      feet.X, feet.Y, feet.Z);
            p.Message(" Precise coords: &b{0} {1} {2}",
                      x, y, z);

            p.Message("Yaw pitch degrees: &b{0} {1}",
                      Orientation.PackedToDegrees(src.Rot.RotY),
                      Orientation.PackedToDegrees(src.Rot.HeadX));
        }

        internal static void BroadcastModel(Entity e, string m) {
            Player[] players = PlayerInfo.Online.Items;
            Level lvl = e.Level;

            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.hasChangeModel) continue;
                if (!pl.CanSeeEntity(e)) continue;

                string model = Chat.Format(m, pl, true, false);

                OnSendingModelEvent.Call(e, ref model, pl);
                pl.EntityMap.SendModel(e, model);
                pl.EntityMap.SendScales(e);
            }
        }

        public static void UpdateEntityProp(Entity e, EntityProp prop, int value) {
            Player[] players = PlayerInfo.Online.Items;
            Level lvl = e.Level;
            
            Orientation rot = e.Rot;
            byte angle = Orientation.DegreesToPacked(value);
            if (prop == EntityProp.RotX) rot.RotX = angle;
            if (prop == EntityProp.RotY) rot.RotY = angle;
            if (prop == EntityProp.RotZ) rot.RotZ = angle;
            
            e.Rot = rot;
            if (prop == EntityProp.RotY) e.SetYawPitch(rot.RotY, rot.HeadX);

            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.Supports(CpeExt.EntityProperty)) continue;
                if (!pl.CanSeeEntity(e)) continue;
                pl.EntityMap.SendProp(e, prop, Orientation.PackedToDegrees(angle));
            }
        }
        
        #region Position updates
        
        
        public static void GlobalUpdate() {
            Player[] players = PlayerInfo.Online.Items;

            PlayerBot.GlobalUpdatePosition(); //Bots update their positions internally first

            // We need to cache the player's position before iterating.
            // Avoids the very rare issue of player's position changing mid-way through iteration,
            // which can cause this player to show minorly offset to other players.
            foreach (Player p in players) {
                p._positionUpdatePos = p.Pos;
            }
            foreach (Player p in players) {
                //TODO: Maytbe set _lastPos in here
                p.EntityMap.BroadcastEntityPositions();
            }
            foreach (Player p in players) {
                p._lastPos = p._positionUpdatePos; p._lastRot = p.Rot;
            }

            PlayerBot.GlobalPostBroadcastPosition(); //Need to set bot's _lastPos and _lastRot
        }
        #endregion
    }
}
