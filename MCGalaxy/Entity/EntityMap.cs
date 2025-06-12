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

namespace MCGalaxy {


    //Thanks fCraft
    public class VisibleEntity {
        public readonly Entity e;
        public readonly byte id;
        /// <summary>
        /// The name actually displayed to this player. May include a suffix for a player currently possessing this entity, for instance
        /// </summary>
        public readonly string displayName;

        public VisibleEntity(Entity e, byte id, string displayName) {
            this.e = e;
            this.id = id;
            this.displayName = displayName;
        }
    }

    /// <summary>
    /// Manages a collection of entities that a player is intended to see.
    /// </summary>
    public sealed class EntityMap {


        Player p;

        //List<Entity> invisible = new List<Entity>();
        ConcurrentDictionary<Entity, VisibleEntity> visible = new ConcurrentDictionary<Entity, VisibleEntity>();
        readonly object locker = new object();

        //Thanks fCraft
        Stack<byte> freeIDs;

        readonly byte maxVisible;

        public EntityMap(Player p, byte maxVisible) {
            this.p = p;
            this.maxVisible = maxVisible;

            lock (locker) {
                freeIDs = new Stack<byte>(maxVisible);
                for (int i = maxVisible; i >= 0; i--) {
                    freeIDs.Push((byte)i);
                }
            }
        }

        /// <summary>
        /// Adds the given entity to the collection. If possible, immediately spawns the entity with the given parameters.
        /// </summary>
        public void Add(Entity e, Position pos, Orientation rot, string skin, string name, string model) {
            bool self = e == p;

            lock (locker) {
                if (freeIDs.Count > 0 || self) {
                    VisibleEntity vis;
                    if (!visible.TryGetValue(e, out vis)) {
                        byte ID = self ? Entities.SelfID : freeIDs.Pop();
                        p.Message("| &a+ &S{0}&S with ID {1}", name, ID);

                        vis = new VisibleEntity(e, ID, name);
                        visible[e] = vis;
                    } else {
                        p.Message("RESPAWNING {0}&S with ID {1}", name, vis.id);
                    }
                    Spawn(vis, pos, rot, skin, name, model);
                }
            }
        }

        /// <summary>
        /// Remove the given entity from the collection and despawns it for this player if possible.
        /// </summary>
        public void Remove(Entity e) {
            lock (locker) {
                VisibleEntity vis;
                if (visible.TryGetValue(e, out vis)) {
                    vis = visible[e];
                    if (e != p) freeIDs.Push(vis.id);
                    p.Message("| &c- &S{0}&S with ID {1}", vis.displayName, vis.id);
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
            VisibleEntity vis;
            if (!visible.TryGetValue(e, out vis)) return;
            _SendProp(vis, prop, value);
        }
        void _SendProp(VisibleEntity vis, EntityProp prop, int value) {
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
        public bool Contains(Entity e) {
            return visible.ContainsKey(e);
        }

        readonly Dictionary<Entity, VisibleEntity> cachedVisible = new Dictionary<Entity, VisibleEntity>(32);
        public unsafe void BroadcastEntityPositions() {

            byte* src = stackalloc byte[16 * 256]; // 16 = size of absolute update, with extended positions
            byte* ptr = src;
            Player dst = p;


            lock (locker) {
                cachedVisible.Clear();
                //We want to avoid locking during the entire enumeration of position sending
                //We need a cached collection to prevent the collection from changing while being enumerated over
                foreach (KeyValuePair<Entity, VisibleEntity> pair in visible) {
                    cachedVisible[pair.Key] = pair.Value;
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

    }
}
