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
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MCGalaxy.Drawing;
using MCGalaxy.SQL;

namespace MCGalaxy {
    public sealed partial class Player : IDisposable {

        public NetworkStream Stream;
        public BinaryReader Reader;

        static void Receive(IAsyncResult result) {
            //Server.s.Log(result.AsyncState.ToString());
            Player p = (Player)result.AsyncState;
            if ( p.disconnected || p.socket == null )
                return;
            try {
                int length = p.socket.EndReceive(result);
                if ( length == 0 ) { p.Disconnect(); return; }

                byte[] b = new byte[p.buffer.Length + length];
                Buffer.BlockCopy(p.buffer, 0, b, 0, p.buffer.Length);
                Buffer.BlockCopy(p.tempbuffer, 0, b, p.buffer.Length, length);

                p.buffer = p.HandleMessage(b);
                if ( p.dontmindme && p.buffer.Length == 0 ) {
                    Server.s.Log("Disconnected");
                    p.socket.Close();
                    p.disconnected = true;
                    return;
                }
                if ( !p.disconnected )
                    p.socket.BeginReceive(p.tempbuffer, 0, p.tempbuffer.Length, SocketFlags.None,
                                          new AsyncCallback(Receive), p);
            }
            catch ( SocketException ) {
                p.Disconnect();
            }
            catch ( ObjectDisposedException ) {
                // Player is no longer connected, socket was closed
                // Mark this as disconnected and remove them from active connection list
                Player.SaveUndo(p);
                if ( connections.Contains(p) )
                    connections.Remove(p);
                p.disconnected = true;
            }
            catch ( Exception e ) {
                Server.ErrorLog(e);
                p.Kick("Error!");
            }
        }
        
        public bool hasCpe = false, hasCustomBlocks = false, finishedLogin = false;
        public string appName;
        public int extensionCount;
        public List<string> extensions = new List<string>();
        public int customBlockSupportLevel;
        void HandleExtInfo( byte[] message ) {
            appName = enc.GetString( message, 0, 64 ).Trim();
            extensionCount = message[65];
        }

        void HandleExtEntry( byte[] message ) {
            AddExtension(enc.GetString(message, 0, 64).Trim(), NTHO_Int(message, 64));
            extensionCount--;
            if (extensionCount <= 0 && !finishedLogin) {
            	if (HasCpeExt(CpeExt.BlockDefinitions) || HasCpeExt(CpeExt.BlockDefinitionsExt))
            		BlockDefinition.SendAll(this);
            	CompleteLoginProcess();
            	finishedLogin = true;
            }
        }
        public static int NTHO_Int(byte[] x, int offset)
        {
            byte[] y = new byte[4];
            Buffer.BlockCopy(x, offset, y, 0, 4); Array.Reverse(y);
            return BitConverter.ToInt32(y, 0);
        }

        void HandleCustomBlockSupportLevel( byte[] message ) {
            customBlockSupportLevel = message[0];
        }

        void SendWomUsers() {
            Player.players.ForEach(
                delegate(Player p)
                {
                    if (p != this)
                    {
                        byte[] buffer = new byte[65];
                        string hereMsg = "^detail.user.here=" + p.color + p.name;
                        NetUtils.WriteAscii(hereMsg, buffer, 1);
                        SendRaw(Opcode.Message, buffer);
                        buffer = null;
                    }
                });
        }
        
        char[] characters = new char[64];
        string GetString( byte[] data, int offset ) {
            int length = 0;
            for( int i = 63; i >= 0; i-- ) {
                byte code = data[i + offset];
                if( length == 0 && !( code == 0 || code == 0x20 ) )
                    length = i + 1;
                characters[i] = (char)code;
            }
            return new String( characters, 0, length );
        }

        public void SendRaw(int id) {
        	byte[] buffer = new [] { (byte)id };
        	SendRaw(buffer);
        }
        
        public void SendRaw(int id, byte data) {
        	byte[] buffer = new [] { (byte)id, data };
        	SendRaw(buffer);
        }
        
        [Obsolete]
        public void SendRaw(int id, byte[] send) {
            byte[] buffer = new byte[send.Length + 1];
            buffer[0] = (byte)id;
            for ( int i = 0; i < send.Length; i++ )
                buffer[i + 1] = send[i];
            SendRaw(buffer);
            buffer = null;
        }
        
         public void SendRaw(byte[] buffer) {
            // Abort if socket has been closed
            if ( socket == null || !socket.Connected )
                return;
            
            try {
                // must send ExtEntry and ExtInfo packets synchronously.
                if (id == Opcode.CpeExtEntry || id == Opcode.CpeExtInfo)
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
            }
        }

        public static void SendMessage(Player p, string message) {
            if ( p == null ) { Server.s.Log(message); return; }
            if (p.name == "IRC")
            {
                Server.IRC.Say(message, false, true);
            }
            SendMessage(p, message, true);
        }
        
        public static void SendMessage(Player p, string message, bool colorParse) {
            if ( p == null ) {
                if ( storeHelp ) {
                    storedHelp += message + "\r\n";
                }
                else {
                    if ( !Server.irc || String.IsNullOrEmpty(Server.IRC.usedCmd) )
                        Server.s.Log(message);
                    else
                        Server.IRC.Pm(Server.IRC.usedCmd, message);
                    //IRCBot.Say(message, true);
                }
                return;
            }
            
            p.SendMessage(0, Server.DefaultColor + message, colorParse);
        }
        
        public void SendMessage(string message) {
           SendMessage(0, Server.DefaultColor + message, true);
        }
        
        public void SendMessage(string message, bool colorParse) {
            SendMessage(0, Server.DefaultColor + message, colorParse);
        }
        
        public void SendMessage(byte id, string message, bool colorParse = true) {
            if (colorParse)
            	message = Chat.EscapeColours(message);
            StringBuilder sb = new StringBuilder(message);

            if (colorParse) {                
                // Begin fix to replace all invalid color codes typed in console or chat with "."
                for ( char ch = '\0'; ch <= '/'; ch++ ) // Characters that cause clients to disconnect
                    sb.Replace("&" + ch, String.Empty);
                for ( char ch = ':'; ch <= '`'; ch++ ) // Characters that cause clients to disconnect
                    sb.Replace("&" + ch, String.Empty);
                for ( char ch = 'g'; ch <= '\u007F'; ch++ ) // Characters that cause clients to disconnect
                    sb.Replace("&" + ch, String.Empty);
                // End fix
            }
            
            Chat.ApplyDollarTokens(sb, this, colorParse);
            if ( Server.parseSmiley && parseSmiley ) {
                sb.Replace(":)", "(darksmile)");
                sb.Replace(":D", "(smile)");
                sb.Replace("<3", "(heart)");
            }

            message = EmotesHandler.ReplaceEmoteKeywords(sb.ToString());
            message = FullCP437Handler.Replace(message);
            int totalTries = 0;
            if ( MessageRecieve != null )
                MessageRecieve(this, message);
            if ( OnMessageRecieve != null )
                OnMessageRecieve(this, message);
            OnMessageRecieveEvent.Call(this, message);
            if ( cancelmessage ) {
                cancelmessage = false;
                return;
            }
            retryTag: try {
                foreach ( string line in Wordwrap(message) ) {
                    string newLine = line;
                    if ( newLine.TrimEnd(' ')[newLine.TrimEnd(' ').Length - 1] < '!' ) {
                        if (!HasCpeExt(CpeExt.EmoteFix))
                            newLine += '\'';
                    }
                    
                    byte[] buffer = new byte[66];
                    buffer[0] = Opcode.Message;
                    buffer[1] = id;
                    if(HasCpeExt(CpeExt.FullCP437))
                    	NetUtils.WriteCP437(newLine, buffer, 2);
                    else
                        NetUtils.WriteAscii(newLine, buffer, 2);
                    SendRaw(buffer);
                }
            } catch ( Exception e ) {
                message = "&f" + message;
                totalTries++;
                if ( totalTries < 10 ) goto retryTag;
                else Server.ErrorLog(e);
            }
        }

        public void SendMotd() {
            byte[] buffer = new byte[130];
            buffer[0] = (byte)8;
            NetUtils.WriteAscii(Server.name, buffer, 1);

            if ( Server.UseTextures ) {
            	string msg = "&0cfg=" + (IsLocalIpAddress(ip) ? ip : Server.IP) + ":" + Server.port + "/" + level.name + "~motd";
            	NetUtils.WriteAscii(msg, buffer, 65);
            } else {
                if ( !String.IsNullOrEmpty(group.MOTD) ) 
                	NetUtils.WriteAscii(group.MOTD, buffer, 65);
                else 
                	NetUtils.WriteAscii(Server.motd, buffer, 65);
            }

            bool canPlace = Block.canPlace(this, Block.blackrock);
            buffer[129] = canPlace ? (byte)100 : (byte)0;
            if ( OnSendMOTD != null ) {
                OnSendMOTD(this, buffer);
            }
            SendRaw(Opcode.Handshake, buffer);
        }

        public void SendUserMOTD() {
            byte[] buffer = new byte[130];
            buffer[0] = Server.version;
            if ( UsingWom && ( level.textures.enabled || level.motd == "texture" ) && group.Permission >= level.textures.LowestRank.Permission ) { 
            	NetUtils.WriteAscii(Server.name, buffer, 1);
            	string womMsg = "&0cfg=" + ( IsLocalIpAddress(ip) ? ip : Server.IP ) + ":" + Server.port + "/" + level.name;
            	NetUtils.WriteAscii(womMsg, buffer, 65);
            }
            if (level.motd == "ignore") {
                NetUtils.WriteAscii(Server.name, buffer, 1);
                if (!String.IsNullOrEmpty(group.MOTD) ) 
                	NetUtils.WriteAscii(group.MOTD, buffer, 65);
                else 
                	NetUtils.WriteAscii(Server.motd, buffer, 65);
            } else {
            	NetUtils.WriteAscii(level.motd, buffer, 1);
            	if (level.motd.Length > 64)
            		NetUtils.WriteAscii(level.motd.Substring(64), buffer, 65);
            }

            bool canPlace = Block.canPlace(this, Block.blackrock);
            buffer[129] = canPlace ? (byte)100 : (byte)0;
            SendRaw(Opcode.Handshake, buffer);
        }

        public void SendMap() { SendRawMap(level); }
        
        public bool SendRawMap(Level level) {
            if ( level.blocks == null ) return false;
            bool success = true;
            bool hasBlockDefinitions = HasCpeExt(CpeExt.BlockDefinitions);
            
            try { 
                byte[] buffer = new byte[level.blocks.Length + 4];
                NetUtils.WriteI32(level.blocks.Length, buffer, 0);
                if (hasCustomBlocks) {
                	for (int i = 0; i < level.blocks.Length; ++i) {
                		byte block = level.blocks[i];
                		if (block == Block.custom_block) {
                			if (hasBlockDefinitions)
                				buffer[i + 4] = level.GetExtTile(i);
                			else
                				buffer[i + 4] = BlockDefinition.Fallback(level.GetExtTile(i));
                		} else {
                			buffer[i + 4] = Block.Convert(block);
                		}
                	}
                } else {
                	for (int i = 0; i < level.blocks.Length; ++i) {
                		byte block = level.blocks[i];
                		if (block == Block.custom_block) {
                			if (hasBlockDefinitions)
                				buffer[i + 4] = Block.ConvertCPE(level.GetExtTile(i));
                			else
                				buffer[i + 4] = Block.ConvertCPE(
                					BlockDefinition.Fallback(level.GetExtTile(i)));
                		} else {
                			buffer[i + 4] = Block.Convert(Block.ConvertCPE(level.blocks[i]));
                		}
                	}
                }
                
                SendRaw(Opcode.LevelInitialise);
                buffer = buffer.GZip();
                int totalRead = 0;                
                
                while (totalRead < buffer.Length) {   
                    byte[] packet = new byte[1028]; // need each packet separate for Mono
                    packet[0] = Opcode.LevelDataChunk;
                    short length = (short)Math.Min(buffer.Length - totalRead, 1024);
                    NetUtils.WriteI16(length, packet, 1);
                    Buffer.BlockCopy(buffer, totalRead, packet, 3, length);
                    packet[1027] = (byte)(100 * (float)totalRead / buffer.Length);
                    
                    SendRaw(packet);            
                    if (ip != "127.0.0.1") {
                    	Thread.Sleep(Server.updateTimer.Interval > 1000 ? 100 : 10);
                    }
                    totalRead += length;
                }
                
                buffer = new byte[7];
                buffer[0] = Opcode.LevelFinalise;
                NetUtils.WriteI16((short)level.Width, buffer, 1);
				NetUtils.WriteI16((short)level.Height, buffer, 3);
				NetUtils.WriteI16((short)level.Length, buffer, 5);
                SendRaw(buffer);
                Loading = false;
                
                if (HasCpeExt(CpeExt.EnvWeatherType))
                    SendSetMapWeather(level.weather);
                if (HasCpeExt(CpeExt.EnvColors))
                	SendCurrentEnvColors();
                if (HasCpeExt(CpeExt.EnvMapAppearance))
                	SendCurrentMapAppearance();
                if ( OnSendMap != null )
                    OnSendMap(this, buffer);
                if (!level.guns)
                	aiming = false;
            } catch( Exception ex ) {
            	success = false;
                Command.all.Find("goto").Use(this, Server.mainLevel.name);
                SendMessage("There was an error sending the map data, you have been sent to the main level.");
                Server.ErrorLog(ex);
            } finally {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            if (HasCpeExt(CpeExt.BlockPermissions))
                SendCurrentBlockPermissions();
            return success;
        }  
        
        public void SendSpawn(byte id, string name, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            byte[] buffer = new byte[74];
            buffer[0] = Opcode.AddEntity;
            buffer[1] = id;
            NetUtils.WriteAscii(name.TrimEnd('+'), buffer, 2);
            NetUtils.WriteU16(x, buffer, 66);
            NetUtils.WriteU16(y, buffer, 68);
            NetUtils.WriteU16(z, buffer, 70);
            buffer[72] = rotx; 
            buffer[73] = roty;
            SendRaw(buffer);

            if (HasCpeExt(CpeExt.ChangeModel))
            	UpdateModels();
        }
        
        public void SendPos(byte id, ushort x, ushort y, ushort z, byte rotx, byte roty) {
            if ( x < 0 ) x = 32;
            if ( y < 0 ) y = 32;
            if ( z < 0 ) z = 32;
            if ( x > level.Width * 32 ) x = (ushort)( level.Width * 32 - 32 );
            if ( z > level.Length * 32 ) z = (ushort)( level.Length * 32 - 32 );
            if ( x > 32767 ) x = 32730;
            if ( y > 32767 ) y = 32730;
            if ( z > 32767 ) z = 32730;

            pos[0] = x; pos[1] = y; pos[2] = z;
            rot[0] = rotx; rot[1] = roty;

            byte[] buffer = new byte[10]; 
            buffer[0] = Opcode.EntityTeleport;
            buffer[1] = id;
            NetUtils.WriteU16(x, buffer, 2);
            NetUtils.WriteU16(y, buffer, 4);
            NetUtils.WriteU16(z, buffer, 6);
            buffer[8] = rotx; 
            buffer[9] = roty;
            SendRaw(buffer);
        }
        
        public void SendUserType(bool op) {
            SendRaw(Opcode.SetPermission, op ? (byte)100 : (byte)0);
        }
        
        //TODO: Figure a way to SendPos without changing rotation
        public void SendDespawn(byte id) { 
        	SendRaw(Opcode.RemoveEntity, id); 
        }
        
        public void SendBlockchange(ushort x, ushort y, ushort z, byte type) {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            byte[] buffer = new byte[8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            if (type == Block.custom_block) {
            	if (HasCpeExt(CpeExt.BlockDefinitions))
            		buffer[7] = level.GetExtTile(x, y, z);
            	else
            		buffer[7] = BlockDefinition.Fallback(level.GetExtTile(x, y, z));
            } else if (hasCustomBlocks) {
            	buffer[7] = Block.Convert(type);
            } else {
            	buffer[7] = Block.Convert(Block.ConvertCPE(type));
            }
            SendRaw(buffer);
        }
        
        // Duplicated as this packet needs to have maximum optimisation.
        public void SendBlockchange(ushort x, ushort y, ushort z, byte type, byte extType) {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;

            byte[] buffer = new byte[8];
            buffer[0] = Opcode.SetBlock;
            NetUtils.WriteU16(x, buffer, 1);
            NetUtils.WriteU16(y, buffer, 3);
            NetUtils.WriteU16(z, buffer, 5);
            
            if (type == Block.custom_block) {
            	if (HasCpeExt(CpeExt.BlockDefinitions))
            		buffer[7] = extType;
            	else
            		buffer[7] = BlockDefinition.Fallback(extType);
            } else if (hasCustomBlocks) {
            	buffer[7] = Block.Convert(type);
            } else {
            	buffer[7] = Block.Convert(Block.ConvertCPE(type));
            }
            SendRaw(buffer);
        }
        
        void SendKick(string message) {
        	byte[] buffer = new byte[65];
        	buffer[0] = Opcode.Kick;
        	NetUtils.WriteAscii(message, buffer, 1);
        	SendRaw(buffer); 
        }
        
        void SendPing() { 
        	SendRaw(Opcode.Ping);
        }
        
        void SendExtInfo( byte count ) {
            byte[] buffer = new byte[67];
            buffer[0] = Opcode.CpeExtInfo;
            NetUtils.WriteAscii("MCGalaxy " + Server.Version, buffer, 1);
            NetUtils.WriteI16((short)count, buffer, 65);
            SendRaw(buffer);
        }
        
        void SendExtEntry( string name, int version ) {
        	byte[] buffer = new byte[69];
        	buffer[0] = Opcode.CpeExtEntry;
            NetUtils.WriteAscii(name, buffer, 1);
            NetUtils.WriteI32(version, buffer, 65);
            SendRaw(buffer);
        }
        
       public void SendClickDistance( short distance ) {
            byte[] buffer = new byte[3];
            buffer[0] = Opcode.CpeSetClickDistance;
            NetUtils.WriteI16(distance, buffer, 1);
            SendRaw(buffer);
        }
        
        void SendCustomBlockSupportLevel(byte level) {
            SendRaw(Opcode.CpeCustomBlockSupportLevel, level);
        }
        
        void SendHoldThis( byte type, byte locked ) { // if locked is on 1, then the player can't change their selected block.
            byte[] buffer = new byte[3];
            buffer[0] = Opcode.CpeHoldThis;
            buffer[1] = type;
            buffer[2] = locked;
            SendRaw(buffer);
        }
        
        void SendTextHotKey( string label, string command, int keycode, byte mods ) {
            byte[] buffer = new byte[133];
            NetUtils.WriteAscii(label, buffer, 0);
            NetUtils.WriteAscii(command, buffer, 64);
            NetUtils.WriteI32(keycode, buffer, 128);
            buffer[132] = mods;
            SendRaw(Opcode.CpeSetTextHotkey, buffer);
        }
        
        public void SendExtAddPlayerName(short id, string name, Group grp, string displayname = "") {
            byte[] buffer = new byte[195];
            NetUtils.WriteI16(id, buffer, 0);
            NetUtils.WriteAscii(name, buffer, 2);
            if (displayname == "") 
            	displayname = name;
            NetUtils.WriteAscii(displayname, buffer, 66);
            NetUtils.WriteAscii(grp.color + grp.name.ToUpper() + "s:", buffer, 130);
            buffer[194] = (byte)grp.Permission.GetHashCode();
            SendRaw(Opcode.CpeExtAddPlayerName, buffer);
        }

        public void SendExtAddEntity(byte id, string name, string displayname = "") {
            byte[] buffer = new byte[129];
            buffer[0] = id;
            NetUtils.WriteAscii(name, buffer, 1);
            if (displayname == "") 
            	displayname = name;
            NetUtils.WriteAscii(displayname, buffer, 65);
            SendRaw(Opcode.CpeExtAddEntity, buffer);
        }
        
        public void SendDeletePlayerName( byte id ) {
            byte[] buffer = new byte[2];
            NetUtils.WriteI16(id, buffer, 0);
            SendRaw(Opcode.CpeExtRemovePlayerName, buffer);
        }
        
        public void SendEnvColor( byte type, short r, short g, short b ) {
            byte[] buffer = new byte[8];
            buffer[0] = Opcode.CpeEnvColors;
            buffer[1] = type;
            NetUtils.WriteI16( r, buffer, 2 );
            NetUtils.WriteI16( g, buffer, 4 );
            NetUtils.WriteI16( b, buffer, 6 );
            SendRaw(buffer);
        }
        
        public void SendMakeSelection( byte id, string label, short smallx, short smally, short smallz, short bigx, short bigy, short bigz, short r, short g, short b, short opacity ) {
            byte[] buffer = new byte[85];
            buffer[0] = id;
            NetUtils.WriteAscii(label, buffer, 1);
            NetUtils.WriteI16( smallx, buffer, 65 );
            NetUtils.WriteI16( smally, buffer,67 );
            NetUtils.WriteI16( smallz, buffer,69 );
            NetUtils.WriteI16( bigx, buffer, 71 );
            NetUtils.WriteI16( bigy, buffer, 73 );
            NetUtils.WriteI16( bigz, buffer, 75 );
            NetUtils.WriteI16( r, buffer, 77 );
            NetUtils.WriteI16( g, buffer, 79);
            NetUtils.WriteI16( b, buffer, 81 );
            NetUtils.WriteI16( opacity, buffer, 83 );
            SendRaw(Opcode.CpeMakeSelection, buffer);
        }
        
        public void SendDeleteSelection( byte id ) {
            SendRaw(Opcode.CpeRemoveSelection, id);
        }
        
        public void SendSetBlockPermission( byte type, bool canplace, bool candelete ) {
            byte[] buffer = new byte[4];
            buffer[0] = Opcode.CpeSetBlockPermission;
            buffer[1] = type;
            buffer[2] = canplace ? (byte)1 : (byte)0;
            buffer[3] = candelete ? (byte)1 : (byte)0;
            SendRaw(buffer);
        }
        
        public void SendChangeModel( byte id, string model ) {
            byte[] buffer = new byte[66];
            buffer[0] = Opcode.CpeChangeModel;
            buffer[1] = id;
            NetUtils.WriteAscii(model, buffer, 2);
            SendRaw(buffer);
        }
        
        public void SendSetMapAppearance( string url, byte sideblock, byte edgeblock, short sidelevel ) {
        	byte[] buffer = new byte[69];
        	buffer[0] = Opcode.CpeEnvSetMapApperance;
            NetUtils.WriteAscii(url, buffer, 1);
            buffer[65] = sideblock;
            buffer[66] = edgeblock;
            NetUtils.WriteI16(sidelevel, buffer, 67);
            SendRaw(buffer);
        }
        
        public void SendSetMapWeather( byte weather ) { // 0 - sunny; 1 - raining; 2 - snowing
            SendRaw(Opcode.CpeEnvWeatherType, weather);
        }
        
        void SendHackControl( byte allowflying, byte allownoclip, byte allowspeeding, byte allowrespawning, 
                             byte allowthirdperson, byte allowchangingweather, short maxjumpheight ) {
            byte[] buffer = new byte[7];
            buffer[0] = allowflying;
            buffer[1] = allownoclip;
            buffer[2] = allowspeeding;
            buffer[3] = allowrespawning;
            buffer[4] = allowthirdperson;
            buffer[5] = allowchangingweather;
            NetUtils.WriteI16(maxjumpheight, buffer, 6);
            SendRaw( Opcode.CpeHackControl, buffer );
        }
        
        void UpdatePosition() {
        	//pingDelayTimer.Stop();
        	byte[] packet = NetUtils.GetPositionPacket(id, pos, oldpos, rot, oldrot, MakePitch(), false);
        	oldpos = pos; oldrot = rot;
        	if (packet == null) return;
        	
        	try {
        		foreach (Player p in players) {
        			if (p != this && p.level == level)
        				p.SendRaw(packet);
        		}
        	} catch { }
        }
        
        byte MakePitch() {
        	if (Server.flipHead || (flipHead && infected))
        		if (rot[1] > 64 && rot[1] < 192)
        			return rot[1];
        		else
        			return 128;
        	return rot[1];
        }

        internal void CloseSocket() {
            // Try to close the socket.
            // Sometimes its already closed so these lines will cause an error
            // We just trap them and hide them from view :P
            try {
                // Close the damn socket connection!
                socket.Shutdown(SocketShutdown.Both);
                #if DEBUG
                Server.s.Log("Socket was shutdown for " + this.name ?? this.ip);
                #endif
            }
            catch ( Exception e ) {
                #if DEBUG
                Exception ex = new Exception("Failed to shutdown socket for " + this.name ?? this.ip, e);
                Server.ErrorLog(ex);
                #endif
            }

            try {
                socket.Close();
                #if DEBUG
                Server.s.Log("Socket was closed for " + this.name ?? this.ip);
                #endif
            }
            catch ( Exception e ) {
                #if DEBUG
                Exception ex = new Exception("Failed to close socket for " + this.name ?? this.ip, e);
                Server.ErrorLog(ex);
                #endif
            }
        }

        public string ReadString(int count = 64) {
            if ( Reader == null ) return null;
            var chars = new byte[count];
            Reader.Read(chars, 0, count);
            return Encoding.UTF8.GetString(chars).TrimEnd().Replace("\0", string.Empty);

        }
    }
}
