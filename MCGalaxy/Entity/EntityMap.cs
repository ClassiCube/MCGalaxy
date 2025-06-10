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

        List<Entity> invisible = new List<Entity>();

        //Dictionary<byte, Entity> visible = new Dictionary<byte, Entity>();
        Dictionary<Entity, VisibleEntity> visible = new Dictionary<Entity, VisibleEntity>();

        //Thanks fCraft
        Stack<byte> freeIDs;

        readonly byte maxVisible;

        public EntityMap(Player p, byte maxVisible) {
            this.p = p;
            this.maxVisible = maxVisible;

            freeIDs = new Stack<byte>(maxVisible);
            for (int i = 0; i <= maxVisible; i++) {
                freeIDs.Push((byte)i);
            }
        }

        /// <summary>
        /// Adds the given entity to the collection. If possible, immediately spawns the entity with the given parameters.
        /// </summary>
        public void Add(Entity e, Position pos, Orientation rot, string skin, string name, string model) {
            //Maybe it Just Works if we add multiple of the same
            //if (theoreticalCollection.Contains(e)) throw new InvalidOperationException(string.Format("Entity {0} was already added to {1}'s EntityMap.", name, p.name));
            bool self = e == p;

            if (freeIDs.Count > 0 || self) {
                byte ID = self ? Entities.SelfID : freeIDs.Pop();
                p.Message("Adding {0}&S with ID {1}", name, ID);
                VisibleEntity vis = new VisibleEntity(e, ID, name);
                visible.Add(e, vis);
                Spawn(vis, pos, rot, skin, name, model);
            } else {
                invisible.Add(e);
            }
        }

        /// <summary>
        /// Remove the given entity from the collection and despawns it for this player if possible.
        /// </summary>
        public void Remove(Entity e) {
            invisible.Remove(e);
            if (visible.ContainsKey(e)) {
                VisibleEntity vis = visible[e];
                if (e != p) freeIDs.Push(vis.id);
                visible.Remove(e);
                Despawn(vis);
                //TODO: Bring another entity back if there were any invisible
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


        public unsafe void BroadcastEntityPositions() {

            byte* src = stackalloc byte[16 * 256]; // 16 = size of absolute update, with extended positions
            byte* ptr = src;
            Player dst = p;

            foreach (var pair in visible) {
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


        //unsafe byte NextFreeId() {
        //    byte* used = stackalloc byte[256];
        //    for (int i = 0; i < 256; i++) used[i] = 0;
        //
        //    Player[] players = PlayerInfo.Online.Items;
        //    for (int i = 0; i < players.Length; i++) {
        //        byte id = players[i].id;
        //        used[id] = 1;
        //    }
        //
        //    for (byte i = 0; i < 255; i++) {
        //        if (used[i] == 0) return i;
        //    }
        //    return 1;
        //}
    }
}
