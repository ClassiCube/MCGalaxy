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
using System.Threading;
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
            public long lastPos;
            public uint lastRot;

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

        VisibleEntity[] visible_list;
        int visible_count;


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

            freeIDs = new Stack<byte>(maxEntityID);
            for (int i = maxEntityID; i >= 0; i--)
            {
                freeIDs.Push((byte)i);
            }

            usedTabIDs   = new bool[maxEntityID + 1];
            visible_list = new VisibleEntity[maxEntityID + 1];
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
            byte id;

            lock (locker) {
                if (LookupEntity(e, out id)) {
                    // nothing to do
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

                // TODO not lookup again
                visible_count = -1;
                VisibleEntity vis;
                visible.TryGetValue(e, out vis);

                if (vis != null) {
                    vis.lastRot = rot.Pack();
                    vis.lastPos = pos.Pack();
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
                visible_count = -1;
                
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
                byte id;
                if (!LookupEntity(e, out id)) return;
                
                p.Session.SendChangeModel(id, model);
                _SendScales(id, e);
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
                byte id;
                if (!LookupEntity(e, out id)) return;
                
                p.Session.SendEntityProperty(id, prop, value);
            }
        }

        public bool GetID(Entity e, out byte id) {
        	lock (locker) { return LookupEntity(e, out id); }
        }
        
        bool LookupEntity(Entity e, out byte id) {
        	if (e == p) { id = Entities.SelfID; return true; }
        	
        	VisibleEntity vis;
        	bool found = visible.TryGetValue(e, out vis);
        	
        	id = found ? vis.id : (byte)0;
        	return found;
        }

        /// <summary> Performs an action while holding lock on entity list </summary>
        /// <remarks> Because callback is called while holding lock on entity list,
        /// it should be a very simple function (e.g. just sending one or two packets) </remarks>
        public bool PerformAction(Entity e, Action<byte> callback) {
            lock (locker) {
                byte id;
                if (!LookupEntity(e, out id)) return false;
                
                callback(id);
            }
            return true;
        }

        int UpdateVisibleList() {
            VisibleEntity[] list = visible_list;
            // check if list hasn't changed since last iteration
            if (visible_count >= 0) return visible_count;
            int i = 0;

            foreach (KeyValuePair<Entity, VisibleEntity> pair in visible)
            {
                if (!pair.Key.autoBroadcastPosition) continue;
                list[i++] = pair.Value;
            }

            visible_count = i;
            return i;
        }

        const int MAX_ENTITIES = 256;
        internal unsafe void BroadcastEntityPositions() {
            byte* src = stackalloc byte[16 * MAX_ENTITIES]; // 16 = size of absolute update, with extended positions
            byte* ptr = src;
            Player dst = p;

            long* _curPos = stackalloc long[MAX_ENTITIES];
            long* _prvPos = stackalloc long[MAX_ENTITIES];

            uint* _curRot = stackalloc uint[MAX_ENTITIES];
            uint* _prvRot = stackalloc uint[MAX_ENTITIES];

            int count;
            VisibleEntity[] list = visible_list;

            lock (locker) {
                count = UpdateVisibleList();
                //We want to avoid locking during the entire enumeration of position sending
                //We need a cached collection to prevent the collection from changing while being enumerated over.
                for (int i = 0; i < count; i++)
                {
                    VisibleEntity vis = list[i];
                    Entity e = vis.e;

                    long pos    = Interlocked.Read(ref e._pos);
                    _prvPos[i]  = vis.lastPos;
                    _curPos[i]  = pos;
                    vis.lastPos = pos;

                    uint rot    = e._rot;
                    _prvRot[i]  = vis.lastRot;
                    _curRot[i]  = rot;
                    vis.lastRot = rot;
                }
            }

            for (int i = 0; i < count; i++)
            {
                VisibleEntity vis = list[i];
                Entity e = vis.e;
                byte id  = vis.id;

                Position curPos    = Position.Unpack(_curPos[i]);
                Position prvPos    = Position.Unpack(_prvPos[i]);

                Orientation prvRot = Orientation.Unpack(_prvRot[i]);
                Orientation curRot = Orientation.Unpack(_curRot[i]);
                
                //CODE REVIEW: How should this be done? We could maybe have the visible pitch be a virtual getter in Entity and player implements its own logic
                if (e.ShouldFlipPitch(dst)) {
                    byte pitch   = curRot.HeadX;
                    curRot.HeadX = FlippedPitch(pitch);
                }
            
                p.Session.GetPositionPacket(ref ptr, id, e.hasExtPositions, dst.hasExtPositions,
                                            curPos, prvPos, curRot, prvRot);
            }

            int size = (int)(ptr - src);
            if (size == 0) return;

            byte[] packet = new byte[size];
            for (int i = 0; i < packet.Length; i++) { packet[i] = src[i]; }
            dst.Send(packet);
        }

        static byte FlippedPitch(byte pitch) {
            if (pitch > 64 && pitch < 192) return pitch;
            else return 128;
        }

    }
}
