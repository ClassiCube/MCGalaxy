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
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Network;

namespace MCGalaxy {
    public partial class Player : IDisposable {

        public bool hasCpe, finishedCpeLogin = false;
        public string appName;
        int extensionCount;
        byte customBlockSupportLevel;
        
        void HandleExtInfo(byte[] buffer, int offset) {
            appName = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
        }

        void HandleExtEntry(byte[] buffer, int offset) {
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer, offset + 65);
            AddExtension(extName, extVersion);
            extensionCount--;
            CheckReadAllExtensions();
        }

        void HandlePlayerClicked(byte[] buffer, int offset) {
            MouseButton Button = (MouseButton)buffer[offset + 1];
            MouseAction Action = (MouseAction)buffer[offset + 2];
            ushort yaw = NetUtils.ReadU16(buffer, offset + 3);
            ushort pitch = NetUtils.ReadU16(buffer, offset + 5);
            byte entityID = buffer[offset + 7];
            ushort x = NetUtils.ReadU16(buffer, offset + 8);
            ushort y = NetUtils.ReadU16(buffer, offset + 10);
            ushort z = NetUtils.ReadU16(buffer, offset + 12);
            
            TargetBlockFace face = (TargetBlockFace)buffer[offset + 14];
            if (face > TargetBlockFace.None) face = TargetBlockFace.None;
            OnPlayerClickEvent.Call(this, Button, Action, yaw, pitch, entityID, x, y, z, face);
        }
        
        void HandleTwoWayPing(byte[] buffer, int offset) {
            bool serverToClient = buffer[offset + 1] != 0;
            ushort data = NetUtils.ReadU16(buffer, offset + 2);
            
            if (!serverToClient) {
                // Client-> server ping, immediately send reply.
                Send(Packet.TwoWayPing(false, data));
            } else {
                // Server -> client ping, set time received for reply.
                Ping.Update(data);
            }
        }

        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }
        
        public void Send(byte[] buffer, bool sync = false) { Socket.Send(buffer, sync); }
        
        public static void MessageLines(Player p, IEnumerable<string> lines) {
            foreach (string line in lines)
                SendMessage(p, line, true);
        }
        
        public static void Message(Player p, string message) {
            SendMessage(p, message, true);
        }
        
        public static void Message(Player p, string message, object a0) {
            SendMessage(p, String.Format(message, a0), true);
        }
        
        public static void Message(Player p, string message, object a0, object a1) {
            SendMessage(p, String.Format(message, a0, a1), true);
        }
        
        public static void Message(Player p, string message, object a0, object a1, object a2) {
            SendMessage(p, String.Format(message, a0, a1, a2), true);
        }
        
        public static void Message(Player p, string message, params object[] args) {
            SendMessage(p, String.Format(message, args), true);
        }

        public static void SendMessage(Player p, string message) {
            SendMessage(p, message, true);
        }
        
        public static void SendMessage(Player p, string message, bool colorParse) {
            if (p == null) {
                Logger.Log(LogType.ConsoleMessage, message);
            } else {
                p.SendMessage(0, message, colorParse);
            }
        }
        
        public void SendMessage(string message) { SendMessage(0, message, true); }        
        public void SendMessage(string message, bool colorParse) { SendMessage(0, message, colorParse); }
        
        public virtual void SendMessage(byte id, string message, bool colorParse = true) {
            // Message should start with server color if no initial color
            if (message.Length > 0 && !(message[0] == '&' || message[0] == '%')) {
                message = ServerConfig.DefaultColor + message;
            }
            message = Chat.Format(message, this, colorParse);
            
            int totalTries = 0;
            OnMessageRecievedEvent.Call(this, message);
            if (cancelmessage) { cancelmessage = false; return; }
            
            retryTag: try {
                foreach (string raw in LineWrapper.Wordwrap(message)) {
                    string line = raw;
                    if (!HasCpeExt(CpeExt.EmoteFix) && LineEndsInEmote(line))
                        line += '\'';

                    Send(Packet.Message(line, (CpeMessageType)id, hasCP437));
                }
            } catch ( Exception e ) {
                message = "&f" + message;
                totalTries++;
                if ( totalTries < 10 ) goto retryTag;
                else Logger.LogError(e);
            }
        }
        
        static bool LineEndsInEmote(string line) {
            line = line.TrimEnd(' ');
            if (line.Length == 0) return false;
            
            char last = line[line.Length - 1];
            return last.UnicodeToCp437() != last;
        }
        
        public void SendCpeMessage(CpeMessageType type, string message, bool colorParse = true) {
            if (type != CpeMessageType.Normal && !HasCpeExt(CpeExt.MessageTypes)) {
                if (type == CpeMessageType.Announcement) type = CpeMessageType.Normal;
                else return;
            }
            message = Chat.Format(message, this, colorParse);
            Send(Packet.Message(message, type, hasCP437));
        }

        public void SendMapMotd() {
            string motd = level.GetMotd(this);
            motd = ChatTokens.Apply(motd, this);
            
            byte[] packet = Packet.Motd(this, motd);
            OnSendingMotdEvent.Call(this, packet);
            Send(packet);
            
            if (!HasCpeExt(CpeExt.HackControl)) return;
            Send(Hacks.MakeHackControl(this));
            if (Game.Referee)
                Send(Packet.HackControl(true, true, true, true, true, -1));
        }
        
        public void SendMap(Level oldLevel) { SendRawMap(oldLevel, level); }
        
        readonly object joinLock = new object();
        public bool SendRawMap(Level oldLevel, Level level) {
            lock (joinLock)
                return SendRawMapCore(oldLevel, level);
        }
        
        bool SendRawMapCore(Level oldLevel, Level level) {
            if (level.blocks == null) return false;
            bool success = true;
            useCheckpointSpawn = false;
            lastCheckpointIndex = -1;
            
            AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            SendMapMotd();
            AccessResult access = level.BuildAccess.Check(this);
            AllowBuild = access == AccessResult.Whitelisted || access == AccessResult.Allowed;
            
            try {                 
                Send(Packet.LevelInitalise());
                
                if (hasBlockDefs) {
                    if (oldLevel != null && oldLevel != level) {
                        RemoveOldLevelCustomBlocks(oldLevel);
                    }
                    BlockDefinition.SendLevelCustomBlocks(this);
                    
                    if (HasCpeExt(CpeExt.InventoryOrder)) {
                        BlockDefinition.SendLevelInventoryOrder(this);
                    }
                }
                
                using (LevelChunkStream s = new LevelChunkStream(this))
                    LevelChunkStream.CompressMap(this, s);
                
                // Force players to read the MOTD (clamped to 3 seconds at most)
                if (level.Config.LoadDelay > 0)
                    System.Threading.Thread.Sleep(level.Config.LoadDelay);
                
                byte[] buffer = Packet.LevelFinalise(level.Width, level.Height, level.Length);
                Send(buffer);
                Loading = false;
                
                OnJoinedLevelEvent.Call(this, oldLevel, level);
            } catch (Exception ex) {
                success = false;
                PlayerActions.ChangeMap(this, Server.mainLevel);
                SendMessage("There was an error sending the map data, you have been sent to the main level.");
                Logger.LogError(ex);
            } finally {
                Server.DoGC();
            }
            return success;
        }
        
        void RemoveOldLevelCustomBlocks(Level oldLevel) {
            BlockDefinition[] defs = oldLevel.CustomBlockDefs;
            for (int i = 1; i < defs.Length; i++) {
                BlockDefinition def = defs[i];
                if (def == BlockDefinition.GlobalDefs[i]) continue;                
                Send(Packet.UndefineBlock((byte)i));
            }
        }
        
        /// <summary> Sends a packet indicating an absolute position + orientation change for an enity. </summary>
        public void SendPos(byte id, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            SendPos(id, new Position(x, y, z), new Orientation(rotx, roty));
        }
        
        /// <summary> Sends a packet indicating an absolute position + orientation change for an enity. </summary>
        public void SendPos(byte id, Position pos, Orientation rot) {
            if (id == Entities.SelfID) {
                Pos = pos; SetYawPitch(rot.RotY, rot.HeadX);
                pos.Y -= 22;  // NOTE: Fix for standard clients
            }           
            Send(Packet.Teleport(id, pos, rot, hasExtPositions));
        }
        
        [Obsolete("Prefer SendBlockChange with ExtBlock")]
        public void SendBlockchange(ushort x, ushort y, ushort z, byte block) {
            byte extBlock = 0;
            if (block == Block.custom_block) extBlock = level.GetExtTile(x, y, z);            
            SendBlockchange(x, y, z, new ExtBlock(block, extBlock));
        }
        
        [Obsolete("Prefer SendBlockChange with ExtBlock")]
        public void SendBlockchange(ushort x, ushort y, ushort z, byte block, byte extBlock) {
            SendBlockchange(x, y, z, new ExtBlock(block, extBlock));
        }
        
        public void SendBlockchange(ushort x, ushort y, ushort z, ExtBlock block) {
            //if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            byte[] buffer = new byte[8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            byte raw;
            if (block.BlockID == Block.custom_block) {
                raw = hasBlockDefs ? block.ExtID : level.RawFallback(block.ExtID);
            } else {
                raw = Block.Convert(block.BlockID);
            }
            if (!hasCustomBlocks) raw = Block.ConvertCPE(raw); // client doesn't support CPE
            
            // Custom block replaced a core block
            if (!hasBlockDefs && raw < Block.CpeCount) {
                BlockDefinition def = level.CustomBlockDefs[raw];
                if (def != null) raw = def.FallBack;
            }
            
            buffer[7] = raw;
            Socket.SendLowPriority(buffer);
        }

        internal void CloseSocket() { 
            Socket.Close();
            RemoveFromPending();
        }
    }
}
