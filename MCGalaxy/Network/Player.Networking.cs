﻿/*
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
using System.Collections.Generic;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy 
{
    public partial class Player : IDisposable
    {
        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasChangeModel, hasExtList, hasCP437;

        public void Send(byte[] buffer)  { Socket.Send(buffer, SendFlags.None); }

        public void MessageLines(IEnumerable<string> lines) {
            lock (messageLocker) {
                foreach (string line in lines) { Message(line); }
            }
        }

        // Put a lock on sending messages so that MessageLines is not interrupted by other messages
        readonly object messageLocker = new object();
        public void Message(string message, object a0) { Message(string.Format(message, a0)); }  
        public void Message(string message, object a0, object a1) { Message(string.Format(message, a0, a1)); }       
        public void Message(string message, object a0, object a1, object a2) { Message(string.Format(message, a0, a1, a2)); }       
        public void Message(string message, params object[] args) { Message(string.Format(message, args)); }
        
        public virtual void Message(string message) {
            // Message should start with server color if no initial color
            if (message.Length > 0 && !(message[0] == '&' || message[0] == '%')) message = "&S" + message;
            message = Chat.Format(message, this);
            lock (messageLocker) {
                SendRawMessage(message);
            }
        }
        
        void SendRawMessage(string message) {
            bool cancel = false;
            OnMessageRecievedEvent.Call(this, ref message, ref cancel);
            if (cancel) return;
            
            try {
                Session.SendChat(message);
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }

        public void SendCpeMessage(CpeMessageType type, string message) {
            SendCpeMessage(type, message, PersistentMessagePriority.Normal);
        }

        public void SendCpeMessage(CpeMessageType type, string message, PersistentMessagePriority priority = PersistentMessagePriority.Normal) {
            if (type != CpeMessageType.Normal && !Supports(CpeExt.MessageTypes)) {
                if (type == CpeMessageType.Announcement) type = CpeMessageType.Normal;
                else return;
            }
            
            message = Chat.Format(message, this);
            if (!persistentMessages.Handle(type, ref message, priority)) return;
            Session.SendMessage(type, message);
        }

        public void SendMapMotd() {
            string motd = GetMotd();
            motd = Chat.Format(motd, this);
            OnSendingMotdEvent.Call(this, ref motd);
            
            // Change -hax into +hax etc when in Referee mode
            //  (can't just do Replace('-', '+') though, that breaks -push)
            if (Game.Referee) {
                motd = motd
                    .Replace("-hax",  "+hax"  ).Replace("-noclip",  "+noclip")
                    .Replace("-speed","+speed").Replace("-respawn", "+respawn")
                    .Replace("-fly",  "+fly"  ).Replace("-thirdperson", "+thirdperson");
            }
            Session.SendMotd(motd);
        }

        readonly object joinLock = new object();
        public bool SendRawMap(Level oldLevel, Level level) {
            lock (joinLock)
                return SendRawMapCore(oldLevel, level);
        }
        
        bool SendRawMapCore(Level prev, Level level) {
            bool success = true;
            try {
                if (level.blocks == null)
                    throw new InvalidOperationException("Tried to join unloaded level");
                
                useCheckpointSpawn  = false;
                lastCheckpointIndex = -1;

                AFKCooldown = DateTime.UtcNow.AddSeconds(2);
                ZoneIn      = null;
                AllowBuild  = level.BuildAccess.CheckAllowed(this);

                SendMapMotd();
                selections.Clear();
                Session.SendLevel(prev, level);
                Loading = false;
                
                OnSentMapEvent.Call(this, prev, level);
            } catch (Exception ex) {
                success = false;
                PlayerActions.ChangeMap(this, Server.mainLevel);
                Message("&WThere was an error sending the map, you have been sent to the main level.");
                Logger.LogError(ex);
            } finally {
                Server.DoGC();
            }
            return success;
        }
        
        /// <summary> Like SendPosition, but immediately updates the player's server-side position and orientation. </summary>
        public void SendAndSetPos(Position pos, Orientation rot) {
            Pos = pos;
            SetYawPitch(rot.RotY, rot.HeadX);
            Session.SendTeleport(Entities.SelfID, pos, rot);
        }

        /// <summary> Sends a packet indicating an absolute position + orientation change for this player. </summary>
        public void SendPosition(Position pos, Orientation rot) {
            if (!Session.SendTeleport(Entities.SelfID, pos, rot, Packet.TeleportMoveMode.AbsoluteInstant)) {
                Session.SendTeleport(Entities.SelfID, pos, rot);
            }
            // Forcibly move the player since their position won't naturally update
            if (frozen || Session.Ping.IgnorePosition) Pos = pos;
        }
        
        public void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            //if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            Session.SendBlockchange(x, y, z, block);
        }


        /// <summary> Whether this player's client supports the given CPE extension at the given version </summary>
        public bool Supports(string extName, int version = 1) {
            return Session != null && Session.Supports(extName, version);
        }
        
        public string GetTextureUrl() {
            string url = level.Config.TexturePack.Length == 0 ? level.Config.Terrain : level.Config.TexturePack;
            if (url.Length == 0) {
                url = Server.Config.DefaultTexture.Length == 0 ? Server.Config.DefaultTerrain : Server.Config.DefaultTexture;
            }
            return url;
        }
        
        
        string lastUrl = "";
        public void SendCurrentTextures() {
            Zone zone = ZoneIn;
            int cloudsHeight = CurrentEnvProp(EnvProp.CloudsLevel, zone);
            int edgeHeight   = CurrentEnvProp(EnvProp.EdgeLevel,   zone);
            int maxFogDist   = CurrentEnvProp(EnvProp.MaxFog,      zone);
            
            byte side  = (byte)CurrentEnvProp(EnvProp.SidesBlock, zone);
            byte edge  = (byte)CurrentEnvProp(EnvProp.EdgeBlock,  zone);
            string url = GetTextureUrl();
            
            if (Supports(CpeExt.EnvMapAspect, 2)) {
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrlV2("", hasCP437));
                Send(Packet.EnvMapUrlV2(url, hasCP437));
            } else if (Supports(CpeExt.EnvMapAspect)) {
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrl("", hasCP437));
                Send(Packet.EnvMapUrl(url, hasCP437));
            } else if (Supports(CpeExt.EnvMapAppearance, 2)) {
                // reset all other textures back to client default.
                if (url != lastUrl) {
                    Send(Packet.MapAppearanceV2("", side, edge, edgeHeight, cloudsHeight, maxFogDist, hasCP437));
                }
                Send(Packet.MapAppearanceV2(url, side, edge, edgeHeight, cloudsHeight, maxFogDist, hasCP437));
                lastUrl = url;
            } else if (Supports(CpeExt.EnvMapAppearance)) {
                url = level.Config.Terrain.Length == 0 ? Server.Config.DefaultTerrain : level.Config.Terrain;
                Send(Packet.MapAppearance(url, side, edge, edgeHeight, hasCP437));
            }
        }

        public void SendCurrentBlockPermissions() {
            if (!Supports(CpeExt.BlockPermissions)) return;
            // Write the block permissions as one bulk TCP packet
            SendAllBlockPermissions();
        }
        
        void SendAllBlockPermissions() {
            bool extBlocks = Session.hasExtBlocks;
            int count = Session.MaxRawBlock + 1;
            int size  = extBlocks ? 5 : 4;
            byte[] bulk = new byte[count * size];
            
            for (int i = 0; i < count; i++) 
            {
                BlockID block = Block.FromRaw((BlockID)i);
                bool place  = group.CanPlace[block] && level.CanPlace;
                // NOTE: If you can't delete air, then you're no longer able to place blocks
                // (see ClassiCube client #815)
                // TODO: Maybe better solution than this?
                bool delete = group.CanDelete[block] && (level.CanDelete || i == Block.Air);
                
                // Placing air is the same as deleting existing block at that position in the world
                if (block == Block.Air) place &= delete;
                Packet.WriteBlockPermission((BlockID)i, place, delete, extBlocks, bulk, i * size);
            }
            Send(bulk);
        }
        
        
        class VisibleSelection { public object data; public byte ID; }
        VolatileArray<VisibleSelection> selections = new VolatileArray<VisibleSelection>();
        
        public bool AddVisibleSelection(string label, Vec3U16 min, Vec3U16 max, ColorDesc color, object instance) {
            lock (selections.locker) {
                byte id = FindOrAddSelection(selections.Items, instance);
                return Session.SendAddSelection(id, label, min, max, color);
            }
        }
        
        public bool RemoveVisibleSelection(object instance) {
            lock (selections.locker) {
                VisibleSelection[] items = selections.Items;
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].data != instance) continue;
                    
                    selections.Remove(items[i]);
                    return Session.SendRemoveSelection(items[i].ID);
                }
            }
            
            return false;
        }
        
        unsafe byte FindOrAddSelection(VisibleSelection[] items, object instance) {
            byte* used = stackalloc byte[256];
            for (int i = 0; i < 256; i++) used[i] = 0;
            byte id;

            for (int i = 0; i < items.Length; i++) 
            {
                id = items[i].ID;
                if (instance == items[i].data) return id;
                
                used[id] = 1;
            }
            
            // find unused ID, or 255 if none unused
            for (id = 0; id < 255; id++) 
            {
                if (used[id] == 0) break;
            }
            
            VisibleSelection sel = new VisibleSelection();
            sel.data = instance;
            sel.ID   = id;
            
            selections.Add(sel);
            return id;
        }
    }
}
