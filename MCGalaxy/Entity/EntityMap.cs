/*
    Copyright 2015-2024 MCGalaxy
        
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

using MCGalaxy.Events.EntityEvents;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using MCGalaxy.Network;

namespace MCGalaxy {


    //Thanks fCraft
    public class VisibleEntity {
        public readonly Entity e;
        public readonly byte id;
        public readonly string displayName;

        public VisibleEntity(Entity e, byte id, string displayName) {
            this.e = e;
            this.id = id;
            this.displayName = displayName;
        }
    }
    public class TabEntity {
        public readonly Entity e;
        public readonly byte id;
        public readonly string name;
        public readonly string nick;
        public readonly string group;
        public readonly byte groupRank;

        public TabEntity(Entity e, byte id, string name, string nick, string group, byte groupRank) {
            this.e = e;
            this.id = id;
            this.name = name;
            this.nick = nick;
            this.group = group;
            this.groupRank = groupRank;
        }
    }

    /// <summary>
    /// Manages a collection of entities that a player is intended to see.
    /// </summary>
    public sealed class EntityMap {


        Player p;

        //List<Entity> invisible = new List<Entity>();
        ConcurrentDictionary<Entity, VisibleEntity> visible = new ConcurrentDictionary<Entity, VisibleEntity>();
        //Thanks fCraft
        Stack<byte> freeIDs;
        readonly object locker = new object();


        #region TabList
        Dictionary<Entity, TabEntity> tabEntities = new Dictionary<Entity, TabEntity>();
        bool[] usedTabIDs;
        readonly object tabLocker = new object();

        public void SendAddTabEntry(Entity e, string name, string nick, string group, byte groupRank) {
            if (!p.hasExtList) return;

            bool self = e == p;
            lock (tabLocker) {

                int tentativeID = -1;

                VisibleEntity vis;
                //We need to match the tablist ID to the matching entity in the level if possible,
                //because a few popular plugins (chatsounds, CEF) rely on this
                if (visible.TryGetValue(e, out vis) && usedTabIDs[vis.id] != true) {
                    //p.Message("Found {0}&S in level, using ID {1}", vis.displayName, vis.id);
                    tentativeID = vis.id;
                } else {
                    if (self) { tentativeID = Entities.SelfID; }
                    else {
                        //In this case, it's an entry for an Entity not on your level (or one that hasn't spawned yet)
                        //Since visible entities are assigned starting from 0 and going up,
                        //we'll find tab list IDs going from 254 down so there's less chance of
                        //an entity from another level sharing an ID with an entity on your level
                        for (int i = maxEntityID; i >= 0; i--) {
                            if (usedTabIDs[i] == false) {
                                tentativeID = i;
                                usedTabIDs[i] = true;
                                break;
                            }
                        }
                    }
                }
                if (tentativeID == -1) return; //No IDs left :(


                TabEntity tabby;
                if (!tabEntities.TryGetValue(e, out tabby)) {
                    byte ID = (byte)tentativeID;
                    //p.Message("| &a+TAB &S{0}&S with ID {1}", name, ID);
                    tabby = new TabEntity(e, ID, name, nick, group, groupRank);
                    tabEntities[e] = tabby;
                } else {
                    //p.Message("RETABBING {0}&S with ID {1}", name, tabby.id);
                }

                p.Session.SendAddTabEntry(tabby.id, tabby.name, tabby.nick, tabby.group, tabby.groupRank);
            }
        }

        public void SendRemoveTabEntry(Entity e) {
            if (!p.hasExtList) return;

            lock (tabLocker) {
                TabEntity tabby;
                if (tabEntities.TryGetValue(e, out tabby)) {
                    tabby = tabEntities[e];
                    if (e != p) usedTabIDs[tabby.id] = false;
                    //p.Message("| &c-TAB &S{0}&S with ID {1}", tabby.name, tabby.id);
                    tabEntities.Remove(e);
                    p.Session.SendRemoveTabEntry(tabby.id);
                } else {
                    //Seems to happen when reconnecting
                    //Logger.Log(LogType.Warning, "{0}'s entitymap: Tried removing Tablist ({0}) that wasn't in the collection...", p.name, e.SkinName);
                }
            }
        }
        #endregion

        readonly byte maxEntityID;

        public EntityMap(Player p, byte maxEntityID) {
            this.p = p;
            this.maxEntityID = maxEntityID;

            lock (locker) {
                freeIDs = new Stack<byte>(maxEntityID);
                for (int i = maxEntityID; i >= 0; i--) {
                    freeIDs.Push((byte)i);
                }
            }

            lock (tabLocker) {
                usedTabIDs = new bool[maxEntityID+1];
            }
        }

        /// <summary>
        /// Adds the given entity. If possible, immediately spawns the entity with the given parameters.
        /// </summary>
        public void Add(Entity e, Position pos, Orientation rot, string skin, string name, string model) {
            bool self = e == p;

            lock (locker) {
                if (freeIDs.Count > 0 || self) {
                    VisibleEntity vis;
                    if (!visible.TryGetValue(e, out vis)) {
                        byte ID = self ? Entities.SelfID : freeIDs.Pop();
                        //p.Message("| &a+ &S{0}&S with ID {1}", name, ID);

                        vis = new VisibleEntity(e, ID, name);
                        visible[e] = vis;
                    } else {
                        //p.Message("RESPAWNING {0}&S with ID {1}", name, vis.id);
                    }
                    Spawn(vis, pos, rot, skin, name, model);

                    //If this entity has a matching tab entry, we need to make sure the IDs get synced
                    //because a few popular plugins (chatsounds, CEF) rely on this
                    lock (tabLocker) {
                        TabEntity tabby;
                        if (tabEntities.TryGetValue(vis.e, out tabby) && tabby.id != vis.id) {
                            //p.Message("%bReadding tab {0} :)", tabby.nick);
                            SendRemoveTabEntry(vis.e);
                            SendAddTabEntry(vis.e, tabby.name, tabby.nick, tabby.group, tabby.groupRank);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Remove the given entity and despawns it for this player.
        /// </summary>
        public void Remove(Entity e) {
            lock (locker) {
                VisibleEntity vis;
                if (visible.TryGetValue(e, out vis)) {
                    vis = visible[e];
                    if (e != p) freeIDs.Push(vis.id);
                    //p.Message("| &c- &S{0}&S with ID {1}", vis.displayName, vis.id);
                    visible.TryRemove(e, out vis);
                    Despawn(vis);
                } else {
                    //Seems to happen when reconnecting
                    //Logger.Log(LogType.Warning, "{0}'s entitymap: Tried removing an entity ({0}) that wasn't in the collection...", p.name, e.SkinName);
                }
            }
        }

        void Spawn(VisibleEntity vis, Position pos, Orientation rot, string skin, string name, string model) {
            p.Session.SendSpawnEntity(vis.id, name, skin, pos, rot);
            _SendModel(vis, model);
            _SendRot(vis, rot);
            _SendScales(vis);
        }
        void Despawn(VisibleEntity vis) {
            p.Session.SendRemoveEntity(vis.id);
        }

        public void SendModel(Entity e, string model) {
            VisibleEntity vis;
            if (!visible.TryGetValue(e, out vis)) return;
            _SendModel(vis, model);
        }
        void _SendModel(VisibleEntity vis, string model) {
            if (p.hasChangeModel) {
                OnSendingModelEvent.Call(vis.e, ref model, p);
                if (!model.CaselessEq("humanoid")) p.Session.SendChangeModel(vis.id, model);
            }
        }
        void _SendRot(VisibleEntity vis, Orientation rot) {
            if (p.Supports(CpeExt.EntityProperty)) {
                p.Session.SendEntityProperty(vis.id, EntityProp.RotX, Orientation.PackedToDegrees(rot.RotX));
                p.Session.SendEntityProperty(vis.id, EntityProp.RotZ, Orientation.PackedToDegrees(rot.RotZ));
            }
        }


        public void SendScales(Entity e) {
            VisibleEntity vis;
            if (!visible.TryGetValue(e, out vis)) return;
            _SendScales(vis);
        }
        void _SendScales(VisibleEntity vis) {
            if (!p.Supports(CpeExt.EntityProperty)) return;

            float max = ModelInfo.MaxScale(vis.e, vis.e.Model);
            _SendScale(vis, EntityProp.ScaleX, vis.e.ScaleX, max);
            _SendScale(vis, EntityProp.ScaleY, vis.e.ScaleY, max);
            _SendScale(vis, EntityProp.ScaleZ, vis.e.ScaleZ, max);
        }
        void _SendScale(VisibleEntity vis, EntityProp axis, float value, float max) {
            if (value == 0) return;
            value = Math.Min(value, max);

            int packed = (int)(value * 1000);
            if (packed == 0) return;
            p.Session.SendEntityProperty(vis.id, axis, packed);
        }


        public void SendProp(Entity e, EntityProp prop, int value) {
            if (!p.Supports(CpeExt.EntityProperty)) return;
            VisibleEntity vis;
            if (!visible.TryGetValue(e, out vis)) return;
            p.Session.SendEntityProperty(vis.id, prop, value);
        }


        public bool GetID(Entity e, out byte id) {
            VisibleEntity vis;
            if (visible.TryGetValue(e, out vis)) {
                id = vis.id;
                return true;
            }
            id = 0;
            return false;
        }

        /// <summary>
        /// For plugins. Unused in base MCGalaxy.
        /// </summary>
        public void SendTeleport(Entity e, Position pos, Orientation rot, Packet.TeleportMoveMode mode) {
            VisibleEntity vis;
            if (!visible.TryGetValue(e, out vis)) return;
            if (!p.Session.SendTeleport(vis.id, pos, rot, mode)) {
                p.Session.SendTeleport(vis.id, pos, rot);
            }
        }
        /// <summary>
        /// For plugins. Unused in base MCGalaxy.
        /// </summary>
        public void SendTeleport(Entity e, Position pos, Orientation rot) {
            VisibleEntity vis;
            if (!visible.TryGetValue(e, out vis)) return;
            p.Session.SendTeleport(vis.id, pos, rot);
        }

        readonly Dictionary<Entity, VisibleEntity> cachedVisible = new Dictionary<Entity, VisibleEntity>(32);
        internal unsafe void BroadcastEntityPositions() {

            byte* src = stackalloc byte[16 * 256]; // 16 = size of absolute update, with extended positions
            byte* ptr = src;
            Player dst = p;


            lock (locker) {
                cachedVisible.Clear();
                //We want to avoid locking during the entire enumeration of position sending
                //We need a cached collection to prevent the collection from changing while being enumerated over.
                //Also, ignore entities that we don't want to automatically update the position of.
                foreach (KeyValuePair<Entity, VisibleEntity> pair in visible) {
                    if (!pair.Key.autoBroadcastPosition) continue;

                    cachedVisible[pair.Key] = pair.Value;
                    if (pair.Key.untracked) {
                        pair.Key._positionUpdatePos = pair.Key.Pos;
                    }
                }
            }

            foreach (KeyValuePair<Entity, VisibleEntity> pair in cachedVisible) {
                Entity e = pair.Key;
                byte id = pair.Value.id;

                if (dst == e || dst.level != e.Level || !dst.CanSeeEntity(e)) continue;

                Orientation rot = e.Rot; byte pitch = rot.HeadX;
                //CODE REVIEW: How should this be done? We could maybe have the visible pitch be a virtual getter in Entity and player implements its own logic
                if (e is Player) {
                    Player p = (Player)e; //No pattern matching because we're ancient C#
                    if (Server.flipHead || p.flipHead) pitch = FlippedPitch(pitch);
                    // flip head when infected in ZS, but doesn't support model
                    if (!dst.hasChangeModel && p.infected)
                        pitch = FlippedPitch(pitch);
                }

                rot.HeadX = pitch;
                p.Session.GetPositionPacket(ref ptr, id, e.hasExtPositions, dst.hasExtPositions,
                                           e._positionUpdatePos, e._lastPos, rot, e._lastRot);

                //bool rotChanged = rot.RotY != e._lastRot.RotY || rot.HeadX != e._lastRot.HeadX;
                //if (pair.Value.displayName.CaselessContains("temp") && (e._lastPos != e._positionUpdatePos || rotChanged)) {
                //    p.Message("Moving {0} to {1}", pair.Value.displayName, e._positionUpdatePos.ToVec3F32().ToString());
                //}
            }

            int count = (int)(ptr - src);
            if (count == 0) return;

            byte[] packet = new byte[count];
            for (int i = 0; i < packet.Length; i++) { packet[i] = src[i]; }
            dst.Send(packet);

            foreach (KeyValuePair<Entity, VisibleEntity> pair in cachedVisible) {
                if (pair.Key.untracked) {
                    pair.Key._lastPos = pair.Key._positionUpdatePos; pair.Key._lastRot = pair.Key.Rot;
                }
            }
        }
        static byte FlippedPitch(byte pitch) {
            if (pitch > 64 && pitch < 192) return pitch;
            else return 128;
        }

    }
}
