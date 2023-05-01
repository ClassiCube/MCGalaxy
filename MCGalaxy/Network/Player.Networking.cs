/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
using System.Collections.Generic;
using MCGalaxy.Events.PlayerEvents;
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
            foreach (string line in lines) { Message(line); }
        }
        
        public void Message(string message, object a0) { Message(string.Format(message, a0)); }  
        public void Message(string message, object a0, object a1) { Message(string.Format(message, a0, a1)); }       
        public void Message(string message, object a0, object a1, object a2) { Message(string.Format(message, a0, a1, a2)); }       
        public void Message(string message, params object[] args) { Message(string.Format(message, args)); }
        
        public virtual void Message(string message) {
            // Message should start with server color if no initial color
            if (message.Length > 0 && !(message[0] == '&' || message[0] == '%')) message = "&S" + message;
            message = Chat.Format(message, this);
            
            SendRawMessage(message);
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
            if (type != CpeMessageType.Normal && !Supports(CpeExt.MessageTypes)) {
                if (type == CpeMessageType.Announcement) type = CpeMessageType.Normal;
                else return;
            }
            
            message = Chat.Format(message, this);
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
        
        /// <summary> Sends a packet indicating an absolute position + orientation change for an enity. </summary>
        public void SendPos(byte id, Position pos, Orientation rot) {
            if (id == Entities.SelfID) {
                Pos = pos; SetYawPitch(rot.RotY, rot.HeadX);
            }
            Session.SendTeleport(id, pos, rot);
        }

        /// <summary> Sends a packet indicating an absolute position + orientation change for this player. </summary>
        public void SendPosition(Position pos, Orientation rot) {
            if (!Session.SendTeleport(Entities.SelfID, pos, rot, Packet.TeleportMoveMode.AbsoluteInstant)) {
                Session.SendTeleport(Entities.SelfID, pos, rot);
            }
            // when frozen, position updates from the client are ignored
            if (frozen) Pos = pos;
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
            
            byte side = (byte)CurrentEnvProp(EnvProp.SidesBlock, zone);
            byte edge = (byte)CurrentEnvProp(EnvProp.EdgeBlock,  zone);

            string url = GetTextureUrl();
            if (Supports(CpeExt.EnvMapAspect)) {
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
            
            for (int i = 0; i < count; i++) {
                BlockID block = Block.FromRaw((BlockID)i);
                bool place  = group.Blocks[block] && level.CanPlace;
                // NOTE: If you can't delete air, then you're no longer able to place blocks
                // (see ClassiCube client #815)
                // TODO: Maybe better solution than this?
                bool delete = group.Blocks[block] && (level.CanDelete || i == Block.Air);
                
                // Placing air is the same as deleting existing block at that position in the world
                if (block == Block.Air) place &= delete;
                Packet.WriteBlockPermission((BlockID)i, place, delete, extBlocks, bulk, i * size);
            }
            Send(bulk);
        }
    }
}
