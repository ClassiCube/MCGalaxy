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
using MCGalaxy.Network;

namespace MCGalaxy {
    public sealed partial class Player : IDisposable {

        static void Receive(IAsyncResult result) {
            //Server.s.Log(result.AsyncState.ToString());
            Player p = (Player)result.AsyncState;
            if (p.disconnected || p.socket == null) return;
            
            try {
                int length = p.socket.EndReceive(result);
                if (length == 0) { p.Disconnect(); return; }

                byte[] allData = new byte[p.leftBuffer.Length + length];
                Buffer.BlockCopy(p.leftBuffer, 0, allData, 0, p.leftBuffer.Length);
                Buffer.BlockCopy(p.tempbuffer, 0, allData, p.leftBuffer.Length, length);
                p.leftBuffer = p.ProcessReceived(allData);
                
                if (p.dontmindme && p.leftBuffer.Length == 0) {
                    Server.s.Log("Disconnected");
                    p.socket.Close();
                    p.disconnected = true;
                    return;
                }
                if ( !p.disconnected )
                    p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None,
                                          new AsyncCallback(Receive), p);
            } catch ( SocketException ) {
                p.Disconnect();
            }  catch ( ObjectDisposedException ) {
                // Player is no longer connected, socket was closed
                // Mark this as disconnected and remove them from active connection list
                connections.Remove(p);
                p.RemoveFromPending();
                p.disconnected = true;
            } catch ( Exception e ) {
                Server.ErrorLog(e);
                p.Leave("Error!");
            }
        }
        
        public bool hasCpe, finishedCpeLogin = false;
        public string appName;
        public int extensionCount;
        public List<string> extensions = new List<string>();
        public int customBlockSupportLevel;
        
        void HandleExtInfo(byte[] packet) {
            appName = NetUtils.ReadString(packet, 1);
            extensionCount = packet[66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
        }

        void HandleExtEntry(byte[] packet) {
            AddExtension(NetUtils.ReadString(packet, 1), NetUtils.ReadI32(packet, 65));
            extensionCount--;
            CheckReadAllExtensions();
        }

        void HandlePlayerClicked(byte[] packet) {
            MouseButton Button = (MouseButton)packet[1];
            MouseAction Action = (MouseAction)packet[2];
            ushort Yaw = NetUtils.ReadU16(packet, 3);
            ushort Pitch = NetUtils.ReadU16(packet, 5);
            byte EntityID = packet[7];
            ushort X = NetUtils.ReadU16(packet, 8);
            ushort Y = NetUtils.ReadU16(packet, 10);
            ushort Z = NetUtils.ReadU16(packet, 12);
            byte Face = packet[14];

            TargetBlockFace face = TargetBlockFace.None;
            if (Face < (byte)face)
                face = (TargetBlockFace)Face;
            if (OnPlayerClick != null) OnPlayerClick(this, Button, Action, Yaw, Pitch, EntityID, X, Y, Z, face);
            OnPlayerClickEvent.Call(this, Button, Action, Yaw, Pitch, EntityID, X, Y, Z, face);
        }

        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }
        
        public void Send(byte[] buffer, bool sync = false) {
            // Abort if socket has been closed
            if (disconnected || socket == null || !socket.Connected) return;
            
            try {
                if (sync)
                    socket.Send(buffer, 0, buffer.Length, SocketFlags.None);
                else
                    socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, delegate(IAsyncResult result) { }, null);
                buffer = null;
            } catch (SocketException e) {
                buffer = null;
                Disconnect();
                #if DEBUG
                Server.ErrorLog(e);
                #endif
            } catch (ObjectDisposedException) {
                // socket was already closed by another thread.
                buffer = null;
            }
        }
        
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
                if (storeHelp)
                    storedHelp += message + "\r\n";
                else
                    Server.s.Log(message);
            } else if (p.ircChannel != null) {
                Server.IRC.Message(p.ircChannel, message);
            } else if (p.ircNick != null) {
               Server.IRC.Pm(p.ircNick, message);
            }  else {
                p.SendMessage(0, Server.DefaultColor + message, colorParse);
            }
        }
        
        public void SendMessage(string message) {
            SendMessage(0, Server.DefaultColor + message, true);
        }
        
        public void SendMessage(string message, bool colorParse) {
            SendMessage(0, Server.DefaultColor + message, colorParse);
        }
        
        public void SendMessage(byte id, string message, bool colorParse = true) {
            message = Chat.Format(message, this, colorParse);
            
            int totalTries = 0;
            if (MessageRecieve != null)
                MessageRecieve(this, message);
            if (OnMessageRecieve != null)
                OnMessageRecieve(this, message);
            OnMessageRecieveEvent.Call(this, message);
            if (cancelmessage) { cancelmessage = false; return; }
            
            retryTag: try {
                foreach (string raw in LineWrapper.Wordwrap(message)) {
                    string line = raw;
                    if (!HasCpeExt(CpeExt.EmoteFix) && line.TrimEnd(' ')[line.TrimEnd(' ').Length - 1] < '!')
                        line += '\'';

                    Send(Packet.Message(line, (CpeMessageType)id, hasCP437));
                }
            } catch ( Exception e ) {
                message = "&f" + message;
                totalTries++;
                if ( totalTries < 10 ) goto retryTag;
                else Server.ErrorLog(e);
            }
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
            if (OnSendMOTD != null) OnSendMOTD(this, packet);
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
            
            SendMapMotd();
            AccessResult access = level.BuildAccess.Check(this);
            AllowBuild = access == AccessResult.Whitelisted || access == AccessResult.Allowed;
            
            try {                 
                Send(Packet.LevelInitalise());
                
                if (hasBlockDefs) {
                    if (oldLevel != null && oldLevel != level)
                        RemoveOldLevelCustomBlocks(oldLevel);
                    BlockDefinition.SendLevelCustomBlocks(this);
                }
                
                using (LevelChunkStream s = new LevelChunkStream(this))
                    LevelChunkStream.CompressMap(this, s);
                
                // Force players to read the MOTD (clamped to 3 seconds at most)
                if (level.LoadDelay > 0)
                    System.Threading.Thread.Sleep(level.LoadDelay);
                
                byte[] buffer = Packet.LevelFinalise(level.Width, level.Height, level.Length);
                Send(buffer);
                Loading = false;
                
                if (OnJoinedLevel != null) OnJoinedLevel(this, oldLevel, level);
                OnJoinedLevelEvent.Call(this, oldLevel, level);                
                if (OnSendMap != null) OnSendMap(this, buffer);
            } catch (Exception ex) {
                success = false;
                PlayerActions.ChangeMap(this, Server.mainLevel);
                SendMessage("There was an error sending the map data, you have been sent to the main level.");
                Server.ErrorLog(ex);
            } finally {
                Server.DoGC();
            }
            return success;
        }
        
        void RemoveOldLevelCustomBlocks(Level oldLevel) {
            BlockDefinition[] defs = oldLevel.CustomBlockDefs;
            for (int i = 1; i < 256; i++) {
                BlockDefinition def = defs[i];
                if (def == null || def == BlockDefinition.GlobalDefs[i]) continue;
                
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
        
        [Obsolete("Prefer SendBlockChange(x, y, z, block, extBlock)")]
        public void SendBlockchange(ushort x, ushort y, ushort z, byte block) {
            byte extBlock = 0;
            if (block == Block.custom_block) extBlock = level.GetExtTile(x, y, z);
            SendBlockchange(x, y, z, block, extBlock);
        }
        
        public void SendBlockchange(ushort x, ushort y, ushort z, byte block, byte extBlock) {
            //if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            byte[] buffer = new byte[8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            if (block == Block.custom_block) {
                block = hasBlockDefs ? extBlock : level.RawFallback(extBlock);
            } else {
                block = Block.Convert(block);
            }
            if (!hasCustomBlocks) block = Block.ConvertCPE(block); // client doesn't support CPE
            
            // Custom block replaced a core block
            if (!hasBlockDefs && block < Block.CpeCount) {
                BlockDefinition def = level.CustomBlockDefs[block];
                if (def != null) block = def.FallBack;
            }
            
            buffer[7] = block;
            Send(buffer);
        }
        
        public void SendExtAddPlayerName(byte id, string listName, string displayName, string grp, byte grpRank) {
            Send(Packet.ExtAddPlayerName(id, listName, displayName, grp, grpRank, hasCP437));
        }

        internal void CloseSocket() {
            // Try to close the socket.
            // Sometimes its already closed so these lines will cause an error
            // We just trap them and hide them from view :P
            try {
                // Close the damn socket connection!
                socket.Shutdown(SocketShutdown.Both);
                #if DEBUG
                Server.s.Log("Socket was shutdown for " + name ?? ip);
                #endif
            }
            catch ( Exception e ) {
                #if DEBUG
                Exception ex = new Exception("Failed to shutdown socket for " + name ?? ip, e);
                Server.ErrorLog(ex);
                #endif
            }

            try {
                socket.Close();
                #if DEBUG
                Server.s.Log("Socket was closed for " + name ?? ip);
                #endif
            }
            catch ( Exception e ) {
                #if DEBUG
                Exception ex = new Exception("Failed to close socket for " + name ?? ip, e);
                Server.ErrorLog(ex);
                #endif
            }
            RemoveFromPending();
        }
    }
}
