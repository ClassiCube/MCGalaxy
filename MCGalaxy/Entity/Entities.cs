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
using MCGalaxy.Events.EntityEvents;
using MCGalaxy.Games;
using MCGalaxy.Network;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy {

    /// <summary> Contains methods related to the management of entities (such as players). </summary>
    public static class Entities {

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
                    other.lastPos = other.Pos; other.lastRot = other.Rot;
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
        
        public static void Spawn(Player dst, Player p) { Spawn(dst, p, p.Pos, p.Rot); }       
        public static void Spawn(Player dst, Player p, Position pos,
                                 Orientation rot, string possession = "") {
            byte id = p == dst ? Entities.SelfID : p.id;
            string name = p.color + p.truename + possession;
            string skin = p.SkinName, model = p.Model;
            OnEntitySpawnedEvent.Call(p, ref name, ref skin, ref model, dst);

            SpawnRaw(dst, id, p, pos, rot, skin, name, model);
            if (!Server.Config.TablistGlobal) TabList.Add(dst, p, id);
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
            SpawnRaw(dst, b.id, b, b.Pos, b.Rot, skin, name, model);
            if (Server.Config.TablistBots) TabList.Add(dst, b);
        }
        
        static void SpawnRaw(Player dst, byte id, Entity e, Position pos, Orientation rot,
                             string skin, string name, string model) {
            // NOTE: Fix for standard clients
            if (id == Entities.SelfID) pos.Y -= 22;
            name = LineWrapper.CleanupColors(name, dst);
            
            if (dst.Supports(CpeExt.ExtPlayerList, 2)) {
                dst.Send(Packet.ExtAddEntity2(id, skin, name, pos, rot, dst.hasCP437, dst.hasExtPositions));
            } else if (dst.hasExtList) {
                dst.Send(Packet.ExtAddEntity(id, skin, name, dst.hasCP437));
                dst.Send(Packet.Teleport(id, pos, rot, dst.hasExtPositions));
            } else {
                dst.Send(Packet.AddEntity(id, name, pos, rot, dst.hasCP437, dst.hasExtPositions));
            }

            if (dst.hasChangeModel) {
                OnSendingModelEvent.Call(e, ref model, dst);
                if (!model.CaselessEq("humanoid")) SendModel(dst, id, model);
            }
            
            if (dst.Supports(CpeExt.EntityProperty)) {
                dst.Send(Packet.EntityProperty(id, EntityProp.RotX, Orientation.PackedToDegrees(rot.RotX)));
                dst.Send(Packet.EntityProperty(id, EntityProp.RotZ, Orientation.PackedToDegrees(rot.RotZ)));
                SendModelScales(dst, id, e);
            }
        }
        
        public static void Despawn(Player dst, Player other) {
            OnEntityDespawnedEvent.Call(other, dst);
            byte id = other == dst ? SelfID : other.id;
            dst.Send(Packet.RemoveEntity(id));
            if (!Server.Config.TablistGlobal) TabList.Remove(dst, other);
        }
        
        public static void Despawn(Player dst, PlayerBot b) {
            OnEntityDespawnedEvent.Call(b, dst);
            dst.Send(Packet.RemoveEntity(b.id));
            if (Server.Config.TablistBots) TabList.Remove(dst, b);
        }

        #endregion
        
        
        [Obsolete("Use p.CanSee")]
        public static bool CanSee(Player p, Player target) { return p.CanSee(target); }
        [Obsolete("Use p.CanSee")]
        public static bool CanSee(CommandData data, Player p, Player target) {       
            return p == target || target == null || !target.hidden || data.Rank >= target.hideRank;
        }

        [Obsolete("Use entity.UpdateModel")]
        public static void UpdateModel(Entity e, string model) { e.UpdateModel(model); }

        internal static void BroadcastModel(Entity e, string m) {
            Player[] players = PlayerInfo.Online.Items;
            Level lvl = e.Level;
            
            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.hasChangeModel) continue;
                if (!pl.CanSeeEntity(e)) continue;
                
                byte id = (pl == e) ? Entities.SelfID : e.EntityID;
                string model = Chat.Format(m, pl, true, false);
                
                OnSendingModelEvent.Call(e, ref model, pl);
                SendModel(pl, id, model);
                SendModelScales(pl, id, e);
            }
        }
        
        static void SendModelScales(Player dst, byte id, Entity e) {
            if (!dst.Supports(CpeExt.EntityProperty)) return;
            
            float max = ModelInfo.MaxScale(e, e.Model);
            SendModelScale(dst, id, EntityProp.ScaleX, e.ScaleX, max);
            SendModelScale(dst, id, EntityProp.ScaleY, e.ScaleY, max);
            SendModelScale(dst, id, EntityProp.ScaleZ, e.ScaleZ, max);
        }
        
        static void SendModelScale(Player dst, byte id, EntityProp axis, float value, float max) {
            if (value == 0) return;
            value = Math.Min(value, max);
            
            int packed = (int)(value * 1000);
            if (packed == 0) return;
            dst.Send(Packet.EntityProperty(id, axis, packed));
        }
        
        static void SendModel(Player dst, byte id, string model) {
            BlockID raw;
            if (BlockID.TryParse(model, out raw) && raw > dst.MaxRawBlock) {
                BlockID block = Block.FromRaw(raw);
                if (block >= Block.ExtendedCount) {
                    model = "humanoid"; // invalid block ids
                } else {
                    model = dst.ConvertBlock(block).ToString();
                }                
            }
            dst.Send(Packet.ChangeModel(id, model, dst.hasCP437));
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
                
                byte id = (pl == e) ? Entities.SelfID : e.EntityID;
                pl.Send(Packet.EntityProperty(id, prop,
                                              Orientation.PackedToDegrees(angle)));
            }
        }
        
        #region Position updates
        
        public unsafe static void GetPositionPacket(ref byte* ptr, byte id, bool srcExtPos, bool extPos,
                                                    Position pos, Position oldPos, Orientation rot, Orientation oldRot) {
            Position delta = GetDelta(pos, oldPos, srcExtPos);
            bool posChanged = delta.X != 0 || delta.Y != 0 || delta.Z != 0;
            bool oriChanged = rot.RotY != oldRot.RotY || rot.HeadX != oldRot.HeadX;
            bool absPosUpdate = Math.Abs(delta.X) > 32 || Math.Abs(delta.Y) > 32 || Math.Abs(delta.Z) > 32;

            if (absPosUpdate) {
                *ptr = Opcode.EntityTeleport; ptr++;
                *ptr = id; ptr++;
                
                if (extPos) {
                    WriteI32(ref ptr, pos.X); WriteI32(ref ptr, pos.Y); WriteI32(ref ptr, pos.Z);
                } else {
                    WriteI16(ref ptr, (short)pos.X); WriteI16(ref ptr, (short)pos.Y); WriteI16(ref ptr, (short)pos.Z);
                }
            } else if (posChanged) {
                byte opcode = oriChanged ? Opcode.RelPosAndOrientationUpdate : Opcode.RelPosUpdate;
                *ptr = opcode; ptr++;
                *ptr = id; ptr++;
                
                *ptr = (byte)(delta.X); ptr++;
                *ptr = (byte)(delta.Y); ptr++;
                *ptr = (byte)(delta.Z); ptr++;
            } else if (oriChanged) {
                *ptr = Opcode.OrientationUpdate; ptr++;
                *ptr = id; ptr++;
            }
            
            if (absPosUpdate || oriChanged) {
                *ptr = rot.RotY; ptr++;
                *ptr = rot.HeadX; ptr++;
            }
        }
        
        unsafe static void WriteI32(ref byte* ptr, int value) {
            *ptr = (byte)(value >> 24); ptr++; *ptr = (byte)(value >> 16); ptr++;
            *ptr = (byte)(value >> 8); ptr++; *ptr = (byte)value; ptr++;
        }
        
        unsafe static void WriteI16(ref byte* ptr, short value) {
            *ptr = (byte)(value >> 8); ptr++; *ptr = (byte)value; ptr++;
        }
        
        static Position GetDelta(Position pos, Position old, bool extPositions) {
            Position delta = new Position(pos.X - old.X, pos.Y - old.Y, pos.Z - old.Z);
            if (extPositions) return delta;
            
            delta.X = (short)delta.X; delta.Y = (short)delta.Y; delta.Z = (short)delta.Z;
            return delta;
        }
        
        
        public static void GlobalUpdate() {
            Player[] players = PlayerInfo.Online.Items;
            // We need to cache the player's position before iterating.
            // Avoids the very rare issue of player's position changing mid-way through iteration,
            // which can cause this player to show minorly offset to other players.
            foreach (Player p in players)
                p.tempPos = p.Pos;
            
            foreach (Player p in players)
                UpdatePosition(p);
            foreach (Player p in players) {
                p.lastPos = p.tempPos; p.lastRot = p.Rot;
            }
        }
        
        unsafe static void UpdatePosition(Player dst) {
            Player[] players = PlayerInfo.Online.Items;
            byte* src = stackalloc byte[16 * 256]; // 16 = size of absolute update, with extended positions
            byte* ptr = src;
            
            foreach (Player p in players) {
                if (dst == p || dst.level != p.level || !dst.CanSeeEntity(p)) continue;
                
                Orientation rot = p.Rot; byte pitch = rot.HeadX;
                if (Server.flipHead || p.flipHead) pitch = FlippedPitch(pitch);
                
                // flip head when infected, but doesn't support model
                if (!dst.hasChangeModel) {
                    ZSData data = ZSGame.TryGet(p);
                    if (data != null && data.Infected) pitch = FlippedPitch(pitch);
                }
            
                rot.HeadX = pitch;
                Entities.GetPositionPacket(ref ptr, p.id, p.hasExtPositions, dst.hasExtPositions,
                                           p.tempPos, p.lastPos, rot, p.lastRot);
            }
            
            int count = (int)(ptr - src);
            if (count == 0) return;
            
            byte[] packet = new byte[count];
            for (int i = 0; i < packet.Length; i++) { packet[i] = src[i]; }
            dst.Send(packet);
        }
        
        static byte FlippedPitch(byte pitch) {
             if (pitch > 64 && pitch < 192) return pitch;
             else return 128;
        }
        #endregion
    }
}
