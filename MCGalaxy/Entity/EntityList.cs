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
using MCGalaxy.Network;

namespace MCGalaxy {


    /// <summary>
    /// Manages a collection of entities that a player is intended to see.
    /// </summary>
    public sealed class EntityList {

        //Thanks fCraft
        class VisibleEntity {
            public readonly Entity e;
            public readonly byte id;

            public VisibleEntity(Entity e, byte id) {
                this.e = e;
                this.id = id;
            }
        }
        
        class WaitingEntity : VisibleEntity
        {
            public readonly bool tabList;
            public WaitingEntity(Entity e, byte id, bool tabList) : base(e, id) {
                this.tabList = tabList;
            }
        }


        Player p;

        Dictionary<Entity, VisibleEntity> visible = new Dictionary<Entity, VisibleEntity>();
        List<WaitingEntity> invisible = new List<WaitingEntity>();
        WaitingEntity IsWaitingToSpawn(Entity e) {
            foreach (var vis in invisible) {
                if (vis.e == e) return vis;
            }
            return null;
        }

        //Thanks fCraft
        Stack<byte> freeIDs;
        readonly object locker = new object();


        #region TabList
        Dictionary<ITabListEntry, byte> tabMap = new Dictionary<ITabListEntry, byte>();
        bool[] usedTabIDs;

        /// <summary> Adds the given entry to that player's tab list (if their client supports it). </summary>
        public void AddTabEntry(ITabListEntry entry) {
            if (!p.hasExtList || !entry.SharesTabListWith(p)) return;
            
            string name  = entry.GetTabListName();
            string nick  = entry.GetTabListNick(p);
            string group = entry.GetTabListGroup();
            byte   rank  = entry.GetTabListRank();
            
            OnTabListEntryAddedEvent.Call(entry, ref nick, ref group, p);
            string suffix = entry.GetTabListSuffix();
            if (!string.IsNullOrEmpty(suffix)) nick += suffix;
            byte id;

            lock (locker) {
                if (entry == p) {
                    id = Entities.SelfID;
                } else if (!tabMap.TryGetValue(entry, out id)) {
                    int tentativeID = FindFreeTabID(entry);
                    if (tentativeID == -1) return;
                    // TODO this doesn't handle tablist reaching capacity..
                    // maybe handle this by setting tabObjects[e] to 255?

                    id = (byte)tentativeID;
                    usedTabIDs[id] = true;
                    tabMap[entry]  = id;
                }

                p.Session.SendAddTabEntry(id, name, nick, group, rank);
            }
        }
        
        /// <summary> Removes the given entry from player's tab list (if their client supports it). </summary>
        public void RemoveTabEntry(ITabListEntry e) {
            if (!p.hasExtList) return;
            
            OnTabListEntryRemovedEvent.Call(e, p);
            byte id;

            lock (locker) {
                if (!tabMap.TryGetValue(e, out id)) return;

                usedTabIDs[id] = false;
                tabMap.Remove(e);
                
                p.Session.SendRemoveTabEntry(id);
            }
        }
        
        int FindFreeTabID(ITabListEntry entry) {
            //Try finding a matching visible entity for the ID
            if (entry is Entity) {
                VisibleEntity vis;
                Entity e = (Entity)entry;
                
                if (visible.TryGetValue(e, out vis) && !usedTabIDs[vis.id]) {
                    // Match tablist ID to corresponding entity in the level if possible,
                    // because a few popular plugins (chatsounds, CEF) rely on this
                    return vis.id;
                }
            }

            // In this case, it's not an entity on the player's level (or entity that hasn't spawned yet)
            // Since visible entities are assigned starting from 0 and going up,
            //  assign tab list IDs going from 254 down so there's less chance
            //  of colliding with the ID of an entity on the player's level
            for (int i = maxEntityID; i >= 0; i--)
            {
                if (!usedTabIDs[i]) return i;
            }
            return -1;
        }
        #endregion

        readonly byte maxEntityID;

        public EntityList(Player p, byte maxEntityID) {
            this.p = p;
            this.maxEntityID = maxEntityID;

            lock (locker) {
                freeIDs = new Stack<byte>(maxEntityID);
                for (int i = maxEntityID; i >= 0; i--) {
                    freeIDs.Push((byte)i);
                }

                usedTabIDs = new bool[maxEntityID + 1];
            }
        }

        /// <summary>
        /// Attempts to spawn the given entity (if cannot be immediately spawned, will spawn later if enough other entities are despawned)
        /// If this returns false and tabList is true, once the entity spawns, it will be added to the tab list.
        /// </summary>
        public bool Add(Entity e, Position pos, Orientation rot, bool tabList) {
            string name  = e.GetSpawnName(p);
            string skin  = e.GetSpawnSkin(p);
            string model = e.GetSpawnModel(p);

            OnEntitySpawnedEvent.Call(e, ref name, ref skin, ref model, p);
            OnSendingModelEvent.Call(e, ref model, p);
            VisibleEntity vis;
            byte id;

            lock (locker) {
                if (e == p) {
                    id = Entities.SelfID;
                } else if (visible.TryGetValue(e, out vis)) {
                    id = vis.id;
                } else if (freeIDs.Count > 0) {
                    id = freeIDs.Pop();
                    visible[e] = new VisibleEntity(e, id);
                } else {
                    // Don't add if it's already queued
                    if (IsWaitingToSpawn(e) != null) return false;
                    
                    WaitingEntity waiting = new WaitingEntity(e, 0, tabList);
                    invisible.Add(waiting);
                    return false;
                }
                
                Spawn(id, e, pos, rot, skin, name, model);
                if (tabList) AddTabEntry(e);

                // If this entity has a matching tab entry, try to ensure IDs are synced
                // because a few popular plugins (chatsounds, CEF) rely on this
                byte tabID;
                if (tabMap.TryGetValue(e, out tabID) && tabID != id) {
                    //p.Message("%bReadding tab {0} :)", tabby.nick);
                    RemoveTabEntry(e);
                    AddTabEntry(e);
                }
                return true;
            }
        }

        /// <summary> Attempts to despawn the given entity </summary>
        /// <returns> Whether the given entity was previously spawned to the player </returns>
        public bool Remove(Entity e, bool tabList) {
            if (e == p) return false; // TODO still remove tab entry?
            OnEntityDespawnedEvent.Call(e, p);
            
            lock (locker) {

                //If we're removing a currently invisible entity...
                WaitingEntity waiting = IsWaitingToSpawn(e);
                if (waiting != null) {
                    invisible.Remove(waiting);
                    //p.Message("Removed invisible {0}", waiting.displayName);
                    return false;
                }

                VisibleEntity vis;
                if (!visible.TryGetValue(e, out vis)) return false;
                
                freeIDs.Push(vis.id);
                visible.Remove(e);
                
                if (tabList) RemoveTabEntry(e);
                Despawn(vis.id);

                //Now that we've removed a visible entity, try spawning a waiting invisible one
                if (invisible.Count > 0 && freeIDs.Count > 0) {
                    waiting = invisible[0];
                    invisible.RemoveAt(0);
                    //p.Message("Adding {0} who was invisible :)", waiting.displayName);
                    Add(waiting.e, waiting.e.Pos, waiting.e.Rot, waiting.tabList);
                }

                return true;
            }
        }

        void Spawn(byte id, Entity e, Position pos, Orientation rot, string skin, string name, string model) {
            p.Session.SendSpawnEntity(id, name, skin, pos, rot);
            p.Session.SendChangeModel(id, model);
            p.Session.SendEntityProperty(id, EntityProp.RotX, Orientation.PackedToDegrees(rot.RotX));
            p.Session.SendEntityProperty(id, EntityProp.RotZ, Orientation.PackedToDegrees(rot.RotZ));
            _SendScales(id, e);
        }
        
        void Despawn(byte id) {
            p.Session.SendRemoveEntity(id);
        }

        /// <summary> Attempts to update the model + scales of the given entity </summary>
        public void UpdateModel(Entity e, string model) {
            OnSendingModelEvent.Call(e, ref model, p);
            
            lock (locker) {
                VisibleEntity vis;
                if (!visible.TryGetValue(e, out vis)) return;
                
                p.Session.SendChangeModel(vis.id, model);
                _SendScales(vis.id, e);
            }
        }
        
        void _SendScales(byte id, Entity e) {
            if (!p.Supports(CpeExt.EntityProperty)) return;

            float max = ModelInfo.MaxScale(e, e.Model);
            _SendScale(id, EntityProp.ScaleX, e.ScaleX, max);
            _SendScale(id, EntityProp.ScaleY, e.ScaleY, max);
            _SendScale(id, EntityProp.ScaleZ, e.ScaleZ, max);
        }
        
        void _SendScale(byte id, EntityProp axis, float value, float max) {
            if (value == 0) return;
            value = Math.Min(value, max);

            int packed = (int)(value * 1000);
            if (packed == 0) return;
            p.Session.SendEntityProperty(id, axis, packed);
        }

        public void SendProp(Entity e, EntityProp prop, int value) {
            if (!p.Supports(CpeExt.EntityProperty)) return;
            lock (locker) {
                VisibleEntity vis;
                if (!visible.TryGetValue(e, out vis)) return;
                p.Session.SendEntityProperty(vis.id, prop, value);
            }
        }

        public bool GetID(Entity e, out byte id) {
            lock (locker) {
                VisibleEntity vis;
                if (visible.TryGetValue(e, out vis)) {
                    id = vis.id;
                    return true;
                }
            }
            id = 0;
            return false;
        }

        public delegate void EntityAction(Entity e, byte id);
        /// <summary> Performs an action while holding lock on entity list </summary>
        /// <remarks> Becaus callback is called while holding lock on entity list,
        /// it should be a very simple function (e.g. just sending one or two packets) </remarks>
        public bool PerformAction(Entity e, EntityAction callback) {
            lock (locker) {
                VisibleEntity vis;
                if (!visible.TryGetValue(e, out vis)) return false;
                
                callback(e, vis.id);
            }
            return true;
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

                if (dst.level != e.Level || !dst.CanSeeEntity(e)) continue;

                Orientation rot = e.Rot; byte pitch = rot.HeadX;
                //CODE REVIEW: How should this be done? We could maybe have the visible pitch be a virtual getter in Entity and player implements its own logic
                if (e is Player) {
                    Player pl = (Player)e; //No pattern matching because we're ancient C#
                    if (Server.flipHead || pl.flipHead) pitch = FlippedPitch(pitch);
                    // flip head when infected in ZS, but doesn't support model
                    if (!dst.hasChangeModel && pl.infected)
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
