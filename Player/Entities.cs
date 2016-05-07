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
using MCGalaxy.Games;

namespace MCGalaxy {

    /// <summary> Contains methods related to the management of entities (such as players). </summary>
    public static class Entities {

        #region Spawning / Despawning
        
        /// <summary> Spawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalSpawn(Player p, bool self, string possession = "") {
            GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], self, possession);
        }
        
        /// <summary> Spawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalSpawn(Player p, ushort x, ushort y, ushort z,
                                       byte rotx, byte roty, bool self, string possession = "") {
            Player[] players = PlayerInfo.Online.Items;
            p.Game.lastSpawnColor = p.Game.Infected ? ZombieGame.InfectCol : p.color;          
            
            foreach (Player other in players) {
                if ((other.Loading && p != other) || p.level != other.level) continue;
                
                if (p != other && Entities.CanSeeEntity(other, p)) {
                    Spawn(other, p, p.id, x, y, z, rotx, roty, possession);
                } else if (p == other && self) {
                    other.pos = new ushort[3] { x, y, z }; other.rot = new byte[2] { rotx, roty };
                    other.oldpos = other.pos; other.oldrot = other.rot;
                    Spawn(other, p, 0xFF, x, y, z, rotx, roty, possession);
                }
            }
        }
        
        /// <summary> Despawns this player to all other players that can see the player in the current world. </summary>
        public static void GlobalDespawn(Player p, bool self, bool fromCanSeeUs = false) {
            Player[] players = PlayerInfo.Online.Items;
            
            foreach (Player other in players) {
                if (p.level != other.level) continue;
                
                // If same world, despawn if we can't see them.
                bool despawn = Entities.CanSeeEntity(other, p);
                if (!fromCanSeeUs) despawn = !despawn;
                if (p != other && despawn) {
                    Despawn(other, p.id);
                } else if (p == other && self) {
                    Despawn(other, 0xFF);
                }
            }
        }
        
        
        

        internal static void Spawn(Player dst, Player p, byte id, ushort x, ushort y, ushort z,
                                   byte rotx, byte roty, string possession = "") {
            TabList.Add(dst, p, id);
            if (!Server.zombie.Running || !p.Game.Infected) {
                string col = GetSupportedCol(dst, p.color);             
                if (dst.hasExtList) {
                    dst.SendExtAddEntity2(id, p.skinName, col + p.truename + possession, x, y, z, rotx, roty);
                } else {
                    dst.SendSpawn(id, col + p.truename + possession, x, y, z, rotx, roty);
                }
                return;
            }
            
            string name = p.truename, skinName = p.skinName;
            if (ZombieGame.ZombieName != "" && !dst.Game.Aka) {
                name = ZombieGame.ZombieName; skinName = name;
            }
            
            if (dst.hasExtList) {
                dst.SendExtAddEntity2(id, skinName, Colors.red + name + possession, x, y, z, rotx, roty);
            } else {
                dst.SendSpawn(id, Colors.red + name + possession, x, y, z, rotx, roty);
            }
            
            if (dst.hasChangeModel && id != 0xFF)
                dst.SendChangeModel(id, ZombieGame.ZombieModel);
        }
        
        /// <summary> Spawns this player to all other players, and spawns all others players to this player. </summary>
        internal static void SpawnEntities(Player p) { SpawnEntities(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]); }
        
        /// <summary> Spawns this player to all other players, and spawns all others players to this player. </summary>
        internal static void SpawnEntities(Player p, ushort x, ushort y, ushort z, byte rotX, byte rotY) {
        	Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
        		if (pl.level != p.level || !CanSeeEntity(p, pl) || p == pl) continue;
                Spawn(p, pl, pl.id, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1], "");
            }           
            GlobalSpawn(p, x, y, z, rotX, rotY, true);
            
            PlayerBot[] bots = PlayerBot.Bots.Items;
            foreach (PlayerBot b in bots)
            	if (b.level == p.level) Spawn(p, b);
        }
        
        /// <summary> Despawns this player to all other players, and despawns all others players to this player. </summary>
        internal static void DespawnEntities(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (p.level == pl.level && p != pl) Despawn(p, pl.id);
            }
            PlayerBot[] bots = PlayerBot.Bots.Items;
            foreach (PlayerBot b in bots) {
                if (p.level == b.level) Despawn(p, b.id);
            }
            Entities.GlobalDespawn(p, true, true);
        }
        
        internal static void Spawn(Player dst, PlayerBot b) {
            TabList.Add(dst, b);
            if (dst.hasExtList) {
                dst.SendExtAddEntity2(b.id, b.skinName, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);
            } else {
                dst.SendSpawn(b.id, b.color + b.skinName, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);
            }
        }
        
        internal static void Despawn(Player dst, byte id) {
            dst.SendRaw(Opcode.RemoveEntity, id);
            TabList.Remove(dst, id);
        }

        #endregion 
        
        internal static string GetSupportedCol(Player dst, string col) {
            if (col.Length >= 2 && !Colors.IsStandardColor(col[1]) && !dst.HasCpeExt(CpeExt.TextColors))
                col = "&" + Colors.GetFallback(col[1]);
            return col;
        }

        
        /// <summary> Returns whether the given player is able to see the other player (e.g. in /who). </summary>
        public static bool CanSee(Player p, Player who) {
            if (p == null || !who.hidden || p == who) return true;
            if (who.otherRankHidden) return p.group.Permission >= who.oHideRank;
            return p.group.Permission > who.group.Permission;
        }
        
        /// <summary> Returns whether the given player is able to see the other player as an in-game entity. </summary>
        public static bool CanSeeEntity(Player p, Player who) {
            bool mayBeHidden = who.hidden;
            mayBeHidden |= (who.Game.Referee || who.Game.Invisible) && Server.zombie.Running;
            if (p == null || !mayBeHidden || p == who) return true;
            if (who.Game.Referee && !p.Game.Referee 
                && Server.zombie.Running) return false;
            if (who.Game.Invisible && !p.Game.Referee 
                && Server.zombie.Running) return false;
            if (who.otherRankHidden) return p.group.Permission >= who.oHideRank;            
            return p.group.Permission >= who.group.Permission;
        }
        
        /// <summary> Updates the model of the entity with the specified id to all other players. </summary>
        public static void UpdateModel(byte id, string model, Level lvl, Player who) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.HasCpeExt(CpeExt.ChangeModel)) continue;
                if (who != null && !CanSeeEntity(pl, who)) continue;
                
                byte sendId = (pl.id == id) ? (byte)0xFF : id;
                pl.SendChangeModel(sendId, model);
            }
        }
        
        #region Position updates
        
        public static byte[] GetPositionPacket(byte id, ushort[] pos, ushort[] oldpos,
                                               byte[] rot, byte[] oldrot, byte realPitch, bool bot) {
            bool posChanged = false, oriChanged = false, absPosUpdate = false;
            if (oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2])
                posChanged = true;
            if (oldrot[0] != rot[0] || oldrot[1] != rot[1])
                oriChanged = true;
            if (Math.Abs(pos[0] - oldpos[0]) > 32 || Math.Abs(pos[1] - oldpos[1]) > 32 || Math.Abs(pos[2] - oldpos[2]) > 32)
                absPosUpdate = true;
            // TODO: not sure why this is necessary for bots
            if (bot)
                absPosUpdate = true;

            byte[] buffer = null;
            if (absPosUpdate) {
                buffer = new byte[10];
                buffer[0] = Opcode.EntityTeleport;
                buffer[1] = id;
                NetUtils.WriteU16(pos[0], buffer, 2);
                NetUtils.WriteU16(pos[1], buffer, 4);
                NetUtils.WriteU16(pos[2], buffer, 6);
                buffer[8] = rot[0];
                buffer[9] = realPitch;
            } else if (posChanged && oriChanged) {
                buffer = new byte[7];
                buffer[0] = Opcode.RelPosAndOrientationUpdate;
                buffer[1] = id;
                buffer[2] = (byte)(pos[0] - oldpos[0]);
                buffer[3] = (byte)(pos[1] - oldpos[1]);
                buffer[4] = (byte)(pos[2] - oldpos[2]);
                buffer[5] = rot[0];
                buffer[6] = realPitch;
            } else if (posChanged) {
                buffer = new byte[5];
                buffer[0] = Opcode.RelPosUpdate;
                buffer[1] = id;
                buffer[2] = (byte)(pos[0] - oldpos[0]);
                buffer[3] = (byte)(pos[1] - oldpos[1]);
                buffer[4] = (byte)(pos[2] - oldpos[2]);
            } else if (oriChanged) {
                buffer = new byte[4];
                buffer[0] = Opcode.OrientationUpdate;
                buffer[1] = id;
                buffer[2] = rot[0];
                buffer[3] = realPitch;
            }
            return buffer;
        }
        
        public static void GlobalUpdate() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players)
                UpdatePosition(p);
        }
        
        static void UpdatePosition(Player p) {
            //pingDelayTimer.Stop();
            byte[] packet = Entities.GetPositionPacket(p.id, p.pos, p.oldpos, p.rot, 
                                                       p.oldrot, MakePitch(p), false);            
            if (packet == null) return;
            byte[] oldPacket = null;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl == p || pl.level != p.level || !CanSeeEntity(pl, p)) continue;
                
                // For clients that don't support ChangeModel, we still need to provide
                // some visual indication that they are infected.
                if (!pl.hasChangeModel && oldPacket == null) {
                    oldPacket = Entities.GetPositionPacket(p.id, p.pos, p.oldpos, p.rot, 
                                                           p.oldrot, MakeClassicPitch(p), false);
                }
                pl.SendRaw(pl.hasChangeModel ? packet : oldPacket);
            }
            p.oldpos = p.pos; p.oldrot = p.rot;
        }
        
        static byte MakePitch(Player p) {
            if (Server.flipHead || p.flipHead)
                if (p.rot[1] > 64 && p.rot[1] < 192) return p.rot[1];
                else return 128;
            return p.rot[1];
        }
        
        static byte MakeClassicPitch(Player p) {
            if (Server.flipHead || p.flipHead || p.Game.Infected)
                if (p.rot[1] > 64 && p.rot[1] < 192) return p.rot[1];
                else return 128;
            return p.rot[1];
        }
        #endregion
    }
}
