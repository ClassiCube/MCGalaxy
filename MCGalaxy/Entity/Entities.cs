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
using System.Text;
using MCGalaxy.Games;

namespace MCGalaxy {

    /// <summary> Contains methods related to the management of entities (such as players). </summary>
    public static class Entities {

        public const byte SelfID = 0xFF;
        public const ushort CharacterHeight = 51;
        
        #region Spawning / Despawning
        
        /// <summary> Spawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalSpawn(Player p, bool self, string possession = "") {
            GlobalSpawn(p, p.Pos, p.Rot, self, possession);
        }
        
        /// <summary> Spawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalSpawn(Player p, Position pos, Orientation rot, bool self, string possession = "") {
            Player[] players = PlayerInfo.Online.Items;
            p.Game.lastSpawnColor = p.Game.Infected ? ZombieGame.InfectCol : p.color;  
            TabList.Update(p, self);
            
            foreach (Player other in players) {
                if ((other.Loading && p != other) || p.level != other.level) continue;
                
                if (p != other && Entities.CanSeeEntity(other, p)) {
                    Spawn(other, p, pos, rot, possession);
                } else if (p == other && self) {
                    other.Pos = pos; other.SetYawPitch(rot.RotY, rot.HeadX);
                    other.lastPos = other.Pos; other.lastRot = other.Rot;
                    Spawn(other, p, pos, rot, possession);
                }
            }
        }
        
        /// <summary> Despawns this player to all other players that cannot 
        /// (or can if 'toVisible' is true) see the player in the current world. </summary>
        public static void GlobalDespawn(Player p, bool self, bool toVisible = false) {
            Player[] players = PlayerInfo.Online.Items;
            TabList.RemoveAll(p, self, toVisible);
            
            foreach (Player other in players) {
                if (p.level != other.level) continue;
                
                bool despawn = !Entities.CanSeeEntity(other, p);
                if (toVisible) despawn = !despawn;
                if (p != other && despawn) {
                    Despawn(other, p.id);
                } else if (p == other && self) {
                    Despawn(other, Entities.SelfID);
                }
            }
        }
        
        internal static void Spawn(Player dst, Player p) { Spawn(dst, p, p.Pos, p.Rot); }
        
        internal static void Spawn(Player dst, Player p, Position pos, 
                                   Orientation rot, string possession = "") {
            byte id = p == dst ? Entities.SelfID : p.id;
            
            if (!Server.TablistGlobal)
                TabList.Add(dst, p, id);
            if (!Server.zombie.Running || !p.Game.Infected) {
                string col = GetSupportedCol(dst, p.color);
                if (dst.hasExtList) {
                    dst.SendExtAddEntity2(id, p.skinName, col + p.truename + possession, p.model, pos, rot);
                } else {
                    dst.SendSpawn(id, col + p.truename + possession, p.model, pos, rot);
                }
                return;
            }
            
            string name = p.truename, skinName = p.skinName;
            if (ZombieGameProps.ZombieName != "" && !dst.Game.Aka) {
                name = ZombieGameProps.ZombieName; skinName = name;
            }
            
            string model = id == Entities.SelfID ? p.model : ZombieGameProps.ZombieModel;
            if (dst.hasExtList) {
                dst.SendExtAddEntity2(id, skinName, Colors.red + name + possession, model, pos, rot);
            } else {
                dst.SendSpawn(id, Colors.red + name + possession, model, pos, rot);
            }
        }
        
        /// <summary> Spawns this player to all other players, and spawns all others players to this player. </summary>
        internal static void SpawnEntities(Player p, bool bots = true) { SpawnEntities(p, p.Pos, p.Rot, bots); }
        
        /// <summary> Spawns this player to all other players, and spawns all others players to this player. </summary>
        internal static void SpawnEntities(Player p, Position pos, Orientation rot, bool bots = true) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
                if (other.level != p.level || !CanSeeEntity(p, other) || p == other) continue;
                Spawn(p, other);
            }           
            GlobalSpawn(p, pos, rot, true);

            if (!bots) return;            
            PlayerBot[] botsList = PlayerBot.Bots.Items;
            foreach (PlayerBot b in botsList)
                if (b.level == p.level) Spawn(p, b);
        }
        
        /// <summary> Despawns this player to all other players, and despawns all others players to this player. </summary>
        internal static void DespawnEntities(Player p, bool bots = true) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player other in players) {
                if (p.level == other.level && p != other) Despawn(p, other.id);
            }
            GlobalDespawn(p, true, true);
            
            if (!bots) return;
            PlayerBot[] botsList = PlayerBot.Bots.Items;
            foreach (PlayerBot b in botsList) {
                if (p.level == b.level) Despawn(p, b);
            }           
        }
        
        internal static void Spawn(Player dst, PlayerBot b) {
            string name = Chat.Format(b.color + b.DisplayName, dst, true, true, false);
            if (b.DisplayName.CaselessEq("empty")) name = "";
            string skin = Chat.Format(b.SkinName, dst, true, true, false);

            if (dst.hasExtList) {
                dst.SendExtAddEntity2(b.id, skin, name, b.model, b.Pos, b.Rot);
            } else {
                dst.SendSpawn(b.id, name, b.model, b.Pos, b.Rot);
            }
            if (Server.TablistBots)
                TabList.Add(dst, b);
        }
        
        internal static void Despawn(Player dst, byte id) {
            dst.SendRaw(Opcode.RemoveEntity, id);
            if (!Server.TablistGlobal)
                TabList.Remove(dst, id);
        }
        
        internal static void Despawn(Player dst, PlayerBot b) {
            dst.SendRaw(Opcode.RemoveEntity, b.id);
            if (Server.TablistBots)
                TabList.Remove(dst, b.id);
        }

        #endregion 
        
        internal static string GetSupportedCol(Player dst, string col) {
            if (col == null) return null;
            if (col.Length >= 2 && !Colors.IsStandardColor(col[1]) && !dst.HasCpeExt(CpeExt.TextColors))
                col = "&" + Colors.GetFallback(col[1]);
            return col;
        }

        
        /// <summary> Returns whether the given player is able to see the target player (e.g. in /who). </summary>
        public static bool CanSee(Player p, Player target) {
            if (p == null || target == null || !target.hidden || p == target) return true;
            if (target.otherRankHidden) return p.Rank >= target.oHideRank;
            return p.Rank >= target.Rank;
        }
        
        /// <summary> Returns whether the given player is able to see the target player as an in-game entity. </summary>
        public static bool CanSeeEntity(Player p, Player target) {
            bool mayBeHidden = target.hidden;
            mayBeHidden |= (target.Game.Referee || target.Game.Invisible) && Server.zombie.Running;
            if (p == null || !mayBeHidden || p == target) return true;
            if (target.Game.Referee && !p.Game.Referee 
                && Server.zombie.Running) return false;
            if (target.Game.Invisible && !p.Game.Referee 
                && Server.zombie.Running) return false;
            if (target.otherRankHidden) return p.Rank >= target.oHideRank;
            return p.Rank >= target.Rank;
        }
        
        /// <summary> Updates the model of the entity with the specified id to all other players. </summary>
        public static void UpdateModel(byte id, string model, Level lvl, Player who) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.HasCpeExt(CpeExt.ChangeModel)) continue;
                if (who != null && !CanSeeEntity(pl, who)) continue;
                
                byte sendId = (pl.id == id) ? Entities.SelfID : id;
                pl.SendChangeModel(sendId, model);
            }
        }
        
        #region Position updates
        
        public static byte[] GetPositionPacket(PlayerBot bot) {
            // TODO: not sure why bots only work with absolute packets
            byte[] buffer = new byte[10];
            buffer[0] = Opcode.EntityTeleport;
            buffer[1] = bot.id;
            NetUtils.WriteU16((ushort)bot.Pos.X, buffer, 2);
            NetUtils.WriteU16((ushort)bot.Pos.Y, buffer, 4);
            NetUtils.WriteU16((ushort)bot.Pos.Z, buffer, 6);
            buffer[8] = bot.Rot.RotY;
            buffer[9] = bot.Rot.HeadX;
            return buffer;
        }
        
        public unsafe static void GetPositionPacket(ref byte* ptr, byte id, Position pos, 
                                                    Position oldPos, Orientation rot, Orientation oldRot) {
            Position delta = GetDelta(pos, oldPos);            
            bool posChanged = delta.X != 0 || delta.Y != 0 || delta.Z != 0;
            bool oriChanged = rot.RotY != oldRot.RotY || rot.HeadX != oldRot.HeadX;
            bool absPosUpdate = Math.Abs(delta.X) > 32 || Math.Abs(delta.Y) > 32 || Math.Abs(delta.Z) > 32;

            if (absPosUpdate) {
                *ptr = Opcode.EntityTeleport; ptr++;
                *ptr = id; ptr++;
                *ptr = (byte)(pos.X >> 8); ptr++; *ptr = (byte)pos.X; ptr++;
                *ptr = (byte)(pos.Y >> 8); ptr++; *ptr = (byte)pos.Y; ptr++;      
                *ptr = (byte)(pos.Z >> 8); ptr++; *ptr = (byte)pos.Z; ptr++;
            } else if (posChanged && oriChanged) {
                *ptr = Opcode.RelPosAndOrientationUpdate; ptr++;
                *ptr = id; ptr++;
                *ptr = (byte)(delta.X); ptr++;
                *ptr = (byte)(delta.Y); ptr++;
                *ptr = (byte)(delta.Z); ptr++;
            } else if (posChanged) {
                *ptr = Opcode.RelPosUpdate; ptr++; 
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
        
        static Position GetDelta(Position pos, Position oldPos) {
            return new Position(pos.X - oldPos.X, pos.Y - oldPos.Y, pos.Z - oldPos.Z);
        	// TODO: proper delta calculation for 16 bit clients
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
        
        unsafe static void UpdatePosition(Player p) {
             Player[] players = PlayerInfo.Online.Items;
             byte* src = stackalloc byte[10 * 256]; // 10 = size of absolute update
             byte* ptr = src;
             
             foreach (Player pl in players) {
                 if (p == pl || p.level != pl.level || !CanSeeEntity(p, pl)) continue;
                 
                 Orientation rot = pl.Rot;
                 rot.HeadX = p.hasChangeModel ? MakePitch(pl, rot.HeadX) : MakeClassicPitch(pl, rot.HeadX);
                 Entities.GetPositionPacket(ref ptr, pl.id, pl.tempPos, pl.lastPos, rot, pl.lastRot);
             }
             
             int count = (int)(ptr - src);
             if (count == 0) return;
             byte[] packet = new byte[count];
             for (int i = 0; i < packet.Length; i++)
                 packet[i] = src[i];
             p.Send(packet);
        }
        
        static byte MakePitch(Player p, byte pitch) {
            if (Server.flipHead || p.flipHead)
                if (pitch > 64 && pitch < 192) return pitch;
                else return 128;
            return pitch;
        }
        
        static byte MakeClassicPitch(Player p, byte pitch) {
            if (Server.flipHead || p.flipHead || p.Game.Infected)
                if (pitch > 64 && pitch < 192) return pitch;
                else return 128;
            return pitch;
        }
        #endregion
    }
}
