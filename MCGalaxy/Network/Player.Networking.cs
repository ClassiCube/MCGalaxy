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
using BlockID = System.UInt16;
using BlockRaw = System.Byte;

namespace MCGalaxy 
{
    public partial class Player : IDisposable
    {
        public void Send(byte[] buffer)  { Socket.Send(buffer, SendFlags.None); }
        
        public void MessageLines(IEnumerable<string> lines) {
            foreach (string line in lines) { Message(line); }
        }
        
        public void Message(string message, object a0) { Message(string.Format(message, a0)); }  
        public void Message(string message, object a0, object a1) { Message(string.Format(message, a0, a1)); }       
        public void Message(string message, object a0, object a1, object a2) { Message(string.Format(message, a0, a1, a2)); }       
        public void Message(string message, params object[] args) { Message(string.Format(message, args)); }
        
        public void Message(string message) { Message(0, message); }
        
        public virtual void Message(byte type, string message) {
            // Message should start with server color if no initial color
            if (message.Length > 0 && !(message[0] == '&' || message[0] == '%')) message = "&S" + message;
            message = Chat.Format(message, this);
            
            bool cancel = false;
            OnMessageRecievedEvent.Call(this, ref message, ref cancel);
            if (cancel) return;
            
            try {
                message = LineWrapper.CleanupColors(message, this);
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
            message = LineWrapper.CleanupColors(message, this);
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
            Session.SendTeleport(Entities.SelfID, pos, rot);
        }
        
        public void SendBlockchange(ushort x, ushort y, ushort z, BlockID block) {
            //if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            Session.SendBlockchange(x, y, z, block);
        }
    }
}
