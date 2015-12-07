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
        
        public bool hasCpe = false;
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
                        Player.StringFormat("^detail.user.here=" + p.color + p.name, 64).CopyTo(buffer, 1);
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
            SendRaw(id, new byte[0]);
        }
        
        public void SendRaw(int id, byte send) {
            SendRaw(id, new byte[] { send });
        }
        
        public void SendRaw(int id, byte[] send) {
            // Abort if socket has been closed
            if ( socket == null || !socket.Connected )
                return;
            byte[] buffer = new byte[send.Length + 1];
            buffer[0] = (byte)id;
            for ( int i = 0; i < send.Length; i++ ) {
                buffer[i + 1] = send[i];
            }
            
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
            
            p.SendMessage(p.id, Server.DefaultColor + message, colorParse);
        }
        public void SendMessage(string message) {
            SendMessage(message, true);
        }
        public void SendMessage(string message, bool colorParse) {
            if ( this == null ) { Server.s.Log(message); return; }
            SendMessage(this.id, Server.DefaultColor + message, colorParse);
        }
        public void SendChat(Player p, string message) {
            if ( this == null ) { Server.s.Log(message); return; }
            Player.SendMessage(p, message);
        }
        public void SendMessage(byte id, string message) {
            SendMessage(id, message, true);
        }
        
        public static string StripColours( string value ) {
            if( value.IndexOf( '%' ) == -1 ) {
                return value;
            }
            
            char[] output = new char[value.Length];
            int usedChars = 0;
            
            for( int i = 0; i < value.Length; i++ ) {
                char token = value[i];
                if( token == '%' ) {
                    i++; // Skip over the following colour code.
                } else {
                    output[usedChars++] = token;
                }
            }
            return new String( output, 0, usedChars );
        }
        
        //string DisplayNameNoColors = StripColours(DisplayName);
        
        
        public void SendMessage(byte id, string message, bool colorParse) {
            if ( this == null ) { Server.s.Log(message); return; }
            if ( ZoneSpam.AddSeconds(2) > DateTime.Now && message.Contains("This zone belongs to ") ) return;

            byte[] buffer = new byte[65];
            unchecked { buffer[0] = id; }

            StringBuilder sb = new StringBuilder(message);

            if ( colorParse ) {
                sb.Replace("%r", "&f");
                for ( int i = 0; i < 10; i++ ) {
                    sb.Replace("%" + i, "&" + i);
                    //sb.Replace("&" + i + " &", " &");
                }
                for ( char ch = 'a'; ch <= 'f'; ch++ ) {
                    sb.Replace("%" + ch, "&" + ch);
                    //sb.Replace("&" + ch + " &", " &");
                }
                // Begin fix to replace all invalid color codes typed in console or chat with "."
                for ( char ch = (char)0; ch <= (char)47; ch++ ) // Characters that cause clients to disconnect
                    sb.Replace("&" + ch, String.Empty);
                for ( char ch = (char)58; ch <= (char)96; ch++ ) // Characters that cause clients to disconnect
                    sb.Replace("&" + ch, String.Empty);
                for ( char ch = (char)103; ch <= (char)127; ch++ ) // Characters that cause clients to disconnect
                    sb.Replace("&" + ch, String.Empty);
                // End fix
            }

            
            if ( Server.dollardollardollar )
                sb.Replace("$name", "$" + StripColours(DisplayName));
            else
                sb.Replace("$name", StripColours(DisplayName));
            sb.Replace("$date", DateTime.Now.ToString("yyyy-MM-dd"));
            sb.Replace("$time", DateTime.Now.ToString("HH:mm:ss"));
            sb.Replace("$ip", ip);
            sb.Replace("$serverip", IsLocalIpAddress(ip) ? ip : Server.IP);
            if ( colorParse ) sb.Replace("$color", color);
            sb.Replace("$rank", group.name);
            sb.Replace("$level", level.name);
            sb.Replace("$deaths", overallDeath.ToString());
            sb.Replace("$money", money.ToString());
            sb.Replace("$blocks", overallBlocks.ToString());
            sb.Replace("$first", firstLogin.ToString());
            sb.Replace("$kicked", totalKicked.ToString());
            sb.Replace("$server", Server.name);
            sb.Replace("$motd", Server.motd);
            sb.Replace("$banned", Player.GetBannedCount().ToString());
            sb.Replace("$irc", Server.ircServer + " > " + Server.ircChannel);

            foreach ( var customReplacement in Server.customdollars ) {
                if ( !customReplacement.Key.StartsWith("//") ) {
                    try {
                        sb.Replace(customReplacement.Key, customReplacement.Value);
                    }
                    catch { }
                }
            }

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
                        if (!HasExtension("EmoteFix"))
                        {
                            newLine += '\'';
                        }
                    }
                    if(HasExtension("FullCP437"))
                        StringFormat437(newLine, 64).CopyTo(buffer, 1);
                    else
                        StringFormat(newLine, 64).CopyTo(buffer, 1);
                    SendRaw(Opcode.Message, buffer);
                }
            }
            catch ( Exception e ) {
                message = "&f" + message;
                totalTries++;
                if ( totalTries < 10 ) goto retryTag;
                else Server.ErrorLog(e);
            }
        }

        public void SendMotd() {
            byte[] buffer = new byte[130];
            buffer[0] = (byte)8;
            StringFormat(Server.name, 64).CopyTo(buffer, 1);

            if ( Server.UseTextures )
                StringFormat("&0cfg=" + ( IsLocalIpAddress(ip) ? ip : Server.IP ) + ":" + Server.port + "/" + level.name + "~motd", 64).CopyTo(buffer, 65);
            else {
                if ( !String.IsNullOrEmpty(group.MOTD) ) StringFormat(group.MOTD, 64).CopyTo(buffer, 65);
                else StringFormat(Server.motd, 64).CopyTo(buffer, 65);
            }

            if ( Block.canPlace(this, Block.blackrock) )
                buffer[129] = 100;
            else
                buffer[129] = 0;
            if ( OnSendMOTD != null ) {
                OnSendMOTD(this, buffer);
            }
            SendRaw(Opcode.Handshake, buffer);
        }

        public void SendUserMOTD() {
            byte[] buffer = new byte[130];
            Random rand = new Random();
            buffer[0] = Server.version;
            if ( UsingWom && ( level.textures.enabled || level.motd == "texture" ) && group.Permission >= level.textures.LowestRank.Permission ) { StringFormat(Server.name, 64).CopyTo(buffer, 1); StringFormat("&0cfg=" + ( IsLocalIpAddress(ip) ? ip : Server.IP ) + ":" + Server.port + "/" + level.name, 64).CopyTo(buffer, 65); }
            if ( level.motd == "ignore" ) {
                StringFormat(Server.name, 64).CopyTo(buffer, 1);
                if ( !String.IsNullOrEmpty(group.MOTD) ) StringFormat(group.MOTD, 64).CopyTo(buffer, 65);
                else StringFormat(Server.motd, 64).CopyTo(buffer, 65);
            }

            else StringFormat(level.motd, 128).CopyTo(buffer, 1);

            if ( Block.canPlace(this.group.Permission, Block.blackrock) )
                buffer[129] = 100;
            else
                buffer[129] = 0;
            SendRaw(Opcode.Handshake, buffer);
        }

        public void SendMap() {
            if ( level.blocks == null ) return;
            try {
                byte[] buffer = new byte[level.blocks.Length + 4];
                BitConverter.GetBytes(IPAddress.HostToNetworkOrder(level.blocks.Length)).CopyTo(buffer, 0);
                //ushort xx; ushort yy; ushort zz;

                for (int i = 0; i < level.blocks.Length; ++i)
                {
                    if (hasCpe)
                    {
                        buffer[4 + i] = (byte)Block.Convert(level.blocks[i]);
                    }
                    else
                    {
                        //Fallback
                        buffer[4 + i] = (byte)Block.Convert(Block.ConvertCPE(level.blocks[i]));
                    }
                }
                SendRaw(Opcode.LevelInitialise);

                buffer = buffer.GZip();
                int number = (int)Math.Ceiling(( (double)buffer.Length ) / 1024);
                for ( int i = 1; buffer.Length > 0; ++i ) {
                    short length = (short)Math.Min(buffer.Length, 1024);
                    byte[] send = new byte[1027];
                    HTNO(length).CopyTo(send, 0);
                    Buffer.BlockCopy(buffer, 0, send, 2, length);
                    byte[] tempbuffer = new byte[buffer.Length - length];
                    Buffer.BlockCopy(buffer, length, tempbuffer, 0, buffer.Length - length);
                    buffer = tempbuffer;
                    send[1026] = (byte)( i * 100 / number );
                    //send[1026] = (byte)(100 - (i * 100 / number)); // Backwards progress lololol...
                    SendRaw(Opcode.LevelDataChunk, send);
                    if ( ip == "127.0.0.1" ) { }
                    else if ( Server.updateTimer.Interval > 1000 ) Thread.Sleep(100);
                    else Thread.Sleep(10);
                } buffer = new byte[6];
                HTNO((short)level.Width).CopyTo(buffer, 0);
                HTNO((short)level.Height).CopyTo(buffer, 2);
                HTNO((short)level.Length).CopyTo(buffer, 4);
                SendRaw(Opcode.LevelFinalise, buffer);
                Loading = false;
                
                if (HasExtension("EnvWeatherType"))
                    SendSetMapWeather(level.weather);
                if (HasExtension("EnvColors"))
                	SendCurrentMapAppearance();
                if (HasExtension("EnvMapAppearance"))
                	SendCurrentMapAppearance();
                if ( OnSendMap != null )
                    OnSendMap(this, buffer);
            } catch ( Exception ex ) {
                Command.all.Find("goto").Use(this, Server.mainLevel.name);
                SendMessage("There was an error sending the map data, you have been sent to the main level.");
                Server.ErrorLog(ex);
            } finally {
                //DateTime start = DateTime.Now;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                //Server.s.Log((DateTime.Now - start).TotalMilliseconds.ToString()); // We dont want random numbers showing up do we?
            }
        }
        
        
        public void SendSpawn(byte id, string name, ushort x, ushort y, ushort z, byte rotx, byte roty)
        {
            byte[] buffer = new byte[73]; buffer[0] = id;
            StringFormat(name.TrimEnd('+'), 64).CopyTo(buffer, 1);
            HTNO(x).CopyTo(buffer, 65);
            HTNO(y).CopyTo(buffer, 67);
            HTNO(z).CopyTo(buffer, 69);
            buffer[71] = rotx; buffer[72] = roty;
            SendRaw(Opcode.AddEntity, buffer);

            if (HasExtension("ChangeModel"))
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

            byte[] buffer = new byte[9]; 
            buffer[0] = id;
            HTNO(x).CopyTo(buffer, 1);
            HTNO(y).CopyTo(buffer, 3);
            HTNO(z).CopyTo(buffer, 5);
            buffer[7] = rotx; 
            buffer[8] = roty;
            SendRaw(Opcode.EntityTeleport, buffer);
        }
        // Update user type for weather or not they are opped
        public void SendUserType(bool op) {
            SendRaw(Opcode.SetPermission, op ? (byte)100 : (byte)0);
        }
        //TODO: Figure a way to SendPos without changing rotation
        public void SendDie(byte id) { 
        	SendRaw(Opcode.RemoveEntity, new byte[1] { id }); 
        }
        public void SendBlockchange(ushort x, ushort y, ushort z, byte type) {
            if (x < 0 || y < 0 || z < 0) return;
            if (x >= level.Width || y >= level.Height || z >= level.Length) return;
            bool skip = false;
            if (type == Block.block_definitions)
            {
                skip = true;
                byte[] chunk = level.CustomBlocks[(x >> 4) + (z >> 4) * level.ChunksX +
                                                  (y >> 4) * level.ChunksX * level.ChunksZ];
                if (chunk == null)
                    type = Block.stone;
                else
                    type = chunk[(x & 0xF) | (y & 0xF) << 4 | (z & 0x0F) << 8];
            }

            byte[] buffer = new byte[7];
            HTNO(x).CopyTo(buffer, 0);
            HTNO(y).CopyTo(buffer, 2);
            HTNO(z).CopyTo(buffer, 4);
            if(!skip)
            {
                if (hasCpe == true)
                {
                    buffer[6] = (byte)Block.Convert(type);
                }
                else
                {
                    buffer[6] = (byte)Block.Convert(Block.ConvertCPE(type));
                }
            }
            SendRaw(Opcode.SetBlock , buffer);
        }
        
        void SendKick(string message) { 
        	SendRaw(Opcode.Kick, StringFormat(message, 64)); 
        }
        
        void SendPing() { 
        	SendRaw(Opcode.Ping);
        }
        
        void SendExtInfo( byte count ) {
            byte[] buffer = new byte[66];
            StringFormat( "MCGalaxy " + Server.Version, 64 ).CopyTo( buffer, 0 );
            HTNO( count ).CopyTo( buffer, 64 );
            SendRaw( Opcode.CpeExtInfo, buffer );
        }
        
        void SendExtEntry( string name, int version ) {
            byte[] version_ = BitConverter.GetBytes(version);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(version_);
            byte[] buffer = new byte[68];
            StringFormat(name, 64).CopyTo(buffer, 0);
            version_.CopyTo(buffer, 64);
            SendRaw( Opcode.CpeExtEntry, buffer );
        }
        
        void SendClickDistance( short distance ) {
            byte[] buffer = new byte[2];
            HTNO( distance ).CopyTo( buffer, 0 );
            SendRaw( Opcode.CpeSetClickDistance, buffer );
        }
        
        void SendCustomBlockSupportLevel(byte level) {
            byte[] buffer = new byte[1];
            buffer[0] = level;
            SendRaw( Opcode.CpeCustomBlockSupportLevel, buffer );
        }
        
        void SendHoldThis( byte type, byte locked ) { // if locked is on 1, then the player can't change their selected block.
            byte[] buffer = new byte[2];
            buffer[0] = type;
            buffer[1] = locked;
            SendRaw( Opcode.CpeHoldThis, buffer );
        }
        
        void SendTextHotKey( string label, string command, int keycode, byte mods ) {
            byte[] buffer = new byte[133];
            StringFormat( label, 64 ).CopyTo( buffer, 0 );
            StringFormat( command, 64 ).CopyTo( buffer, 64 );
            BitConverter.GetBytes( keycode ).CopyTo( buffer, 128 );
            buffer[132] = mods;
            SendRaw( Opcode.CpeSetTextHotkey, buffer );
        }
        
        public void SendExtAddPlayerName(short id, string name, Group grp, string displayname = "")
        {
            byte[] buffer = new byte[195];
            HTNO(id).CopyTo(buffer, 0);
            StringFormat(name, 64).CopyTo(buffer, 2);
            if (displayname == "") { displayname = name; }
            StringFormat(displayname, 64).CopyTo(buffer, 66);
            StringFormat(grp.color + grp.name.ToUpper() + "s:", 64).CopyTo(buffer, 130);
            buffer[194] = (byte)grp.Permission.GetHashCode();
            SendRaw(Opcode.CpeExtAddPlayerName, buffer);
        }

        public void SendExtAddEntity(byte id, string name, string displayname = "")
        {
            byte[] buffer = new byte[129];
            buffer[0] = id;
            StringFormat(name, 64).CopyTo(buffer, 1);
            if (displayname == "") { displayname = name; }
            StringFormat(displayname, 64).CopyTo(buffer, 65);
            SendRaw( Opcode.CpeExtAddEntity, buffer);
        }
        
        public void SendDeletePlayerName( byte id ) {
            byte[] buffer = new byte[2];
            HTNO( (short)id ).CopyTo( buffer, 0 );
            SendRaw( Opcode.CpeExtRemovePlayerName, buffer );
        }
        
        public void SendEnvColor( byte type, short r, short g, short b ) {
            byte[] buffer = new byte[7];
            buffer[0] = type;
            HTNO( r ).CopyTo( buffer, 1 );
            HTNO( g ).CopyTo( buffer, 3 );
            HTNO( b ).CopyTo( buffer, 5 );
            SendRaw( Opcode.CpeEnvColors, buffer );
        }
        
        public void SendMakeSelection( byte id, string label, short smallx, short smally, short smallz, short bigx, short bigy, short bigz, short r, short g, short b, short opacity ) {
            byte[] buffer = new byte[85];
            buffer[0] = id;
            StringFormat( label, 64 ).CopyTo( buffer, 1 );
            HTNO( smallx ).CopyTo( buffer, 65 );
            HTNO( smally ).CopyTo( buffer,67 );
            HTNO( smallz ).CopyTo( buffer,69 );
            HTNO( bigx ).CopyTo( buffer, 71 );
            HTNO( bigy ).CopyTo( buffer, 73 );
            HTNO( bigz ).CopyTo( buffer, 75 );
            HTNO( r ).CopyTo( buffer, 77 );
            HTNO( g ).CopyTo( buffer, 79);
            HTNO( b ).CopyTo( buffer, 81 );
            HTNO( opacity ).CopyTo( buffer, 83 );
            SendRaw( Opcode.CpeMakeSelection, buffer );
        }
        
        public void SendDeleteSelection( byte id ) {
            byte[] buffer = new byte[1];
            buffer[0] = id;
            SendRaw( Opcode.CpeRemoveSelection, buffer );
        }
        void SendSetBlockPermission( byte type, byte canplace, byte candelete ) {
            byte[] buffer = new byte[3];
            buffer[0] = type;
            buffer[1] = canplace;
            buffer[2] = candelete;
            SendRaw( Opcode.CpeSetBlockPermission, buffer );
        }
        
        public void SendChangeModel( byte id, string model ) {
            byte[] buffer = new byte[65];
            buffer[0] = id;
            StringFormat( model, 64 ).CopyTo( buffer, 1 );
            SendRaw( Opcode.CpeChangeModel, buffer );
        }
        
        public void SendSetMapAppearance( string url, byte sideblock, byte edgeblock, short sidelevel ) {
            byte[] buffer = new byte[68];
            StringFormat( url, 64 ).CopyTo( buffer, 0 );
            buffer[64] = sideblock;
            buffer[65] = edgeblock;
            HTNO( sidelevel ).CopyTo( buffer, 66 );
            SendRaw( Opcode.CpeEnvSetMapApperance, buffer );
        }
        
        public void SendSetMapWeather( byte weather ) { // 0 - sunny; 1 - raining; 2 - snowing
            byte[] buffer = new byte[1];
            buffer[0] = weather;
            SendRaw( Opcode.CpeEnvWeatherType, buffer );
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
            HTNO( maxjumpheight ).CopyTo( buffer, 6 );
            SendRaw( Opcode.CpeHackControl, buffer );
        }
        
        public void SendBlockDefinitions(BlockDefinitions bd) {
            byte[] buffer = new byte[79];
            buffer[0] = bd.ID;
            StringFormat(bd.Name, 64).CopyTo(buffer, 1);
            buffer[65] = bd.Solidity;
            buffer[66] = bd.MovementSpeed;
            buffer[67] = bd.TopT;
            buffer[68] = bd.SideT;
            buffer[69] = bd.BottomT;
            buffer[70] = bd.TransmitsLight;
            buffer[71] = bd.WalkSound;
            buffer[72] = bd.FullBright;
            buffer[73] = bd.Shape;
            buffer[74] = bd.BlockDraw;
            buffer[75] = bd.FogD;
            buffer[76] = bd.FogR;
            buffer[77] = bd.FogG;
            buffer[78] = bd.FogB;
            SendRaw(Opcode.CpeDefineBlock, buffer);
        }
        
        void UpdatePosition() {

            //pingDelayTimer.Stop();

            // Shameless copy from JTE's Server
            byte changed = 0; //Denotes what has changed (x,y,z, rotation-x, rotation-y)
            // 0 = no change - never happens with this code.
            // 1 = position has changed
            // 2 = rotation has changed
            // 3 = position and rotation have changed
            // 4 = Teleport Required (maybe something to do with spawning)
            // 5 = Teleport Required + position has changed
            // 6 = Teleport Required + rotation has changed
            // 7 = Teleport Required + position and rotation has changed
            //NOTE: Players should NOT be teleporting this often. This is probably causing some problems.
            if ( oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2] )
                changed |= 1;

            if ( oldrot[0] != rot[0] || oldrot[1] != rot[1] ) {
                changed |= 2;
            }
            /*if (Math.Abs(pos[0] - basepos[0]) > 32 || Math.Abs(pos[1] - basepos[1]) > 32 || Math.Abs(pos[2] - basepos[2]) > 32)
changed |= 4;

if ((oldpos[0] == pos[0] && oldpos[1] == pos[1] && oldpos[2] == pos[2]) && (basepos[0] != pos[0] || basepos[1] != pos[1] || basepos[2] != pos[2]))
changed |= 4;*/
            if ( Math.Abs(pos[0] - oldpos[0]) > 32 || Math.Abs(pos[1] - oldpos[1]) > 32 || Math.Abs(pos[2] - oldpos[2]) > 32 )
                changed |= 4;
            if ( changed == 0 ) { if ( oldpos[0] != pos[0] || oldpos[1] != pos[1] || oldpos[2] != pos[2] ) changed |= 1; }

            byte[] buffer = new byte[0]; byte msg = 0;
            if ( ( changed & 4 ) != 0 ) {
                msg = 8; //Player teleport - used for spawning or moving too fast
                buffer = new byte[9]; buffer[0] = id;
                HTNO(pos[0]).CopyTo(buffer, 1);
                HTNO(pos[1]).CopyTo(buffer, 3);
                HTNO(pos[2]).CopyTo(buffer, 5);
                buffer[7] = rot[0];

                if ( Server.flipHead || ( this.flipHead && this.infected ) )
                    if ( rot[1] > 64 && rot[1] < 192 )
                        buffer[8] = rot[1];
                    else
                        buffer[8] = (byte)( rot[1] - ( rot[1] - 128 ) );
                    else
                        buffer[8] = rot[1];

                //Realcode
                //buffer[8] = rot[1];
            }
            else if ( changed == 1 ) {
                try {
                    msg = 10; //Position update
                    buffer = new byte[4]; buffer[0] = id;
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte)( pos[0] - oldpos[0] )), 0, buffer, 1, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte)( pos[1] - oldpos[1] )), 0, buffer, 2, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte)( pos[2] - oldpos[2] )), 0, buffer, 3, 1);
                }
                catch { }
            }
            else if ( changed == 2 ) {
                msg = 11; //Orientation update
                buffer = new byte[3]; buffer[0] = id;
                buffer[1] = rot[0];

                if ( Server.flipHead || ( this.flipHead && this.infected ) )
                    if ( rot[1] > 64 && rot[1] < 192 )
                        buffer[2] = rot[1];
                    else
                        buffer[2] = (byte)( rot[1] - ( rot[1] - 128 ) );
                    else
                        buffer[2] = rot[1];

                //Realcode
                //buffer[2] = rot[1];
            }
            else if ( changed == 3 ) {
                try {
                    msg = 9; //Position and orientation update
                    buffer = new byte[6]; buffer[0] = id;
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte)( pos[0] - oldpos[0] )), 0, buffer, 1, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte)( pos[1] - oldpos[1] )), 0, buffer, 2, 1);
                    Buffer.BlockCopy(System.BitConverter.GetBytes((sbyte)( pos[2] - oldpos[2] )), 0, buffer, 3, 1);
                    buffer[4] = rot[0];

                    if ( Server.flipHead || ( this.flipHead && this.infected ) )
                        if ( rot[1] > 64 && rot[1] < 192 )
                            buffer[5] = rot[1];
                        else
                            buffer[5] = (byte)( rot[1] - ( rot[1] - 128 ) );
                        else
                            buffer[5] = rot[1];

                    //Realcode
                    //buffer[5] = rot[1];
                }
                catch { }
            }

            oldpos = pos; oldrot = rot;
            if ( changed != 0 )
                try {
                foreach ( Player p in players ) {
                    if ( p != this && p.level == level ) {
                        p.SendRaw(msg, buffer);
                    }
                }
            }
            catch { }
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

        public static byte[] StringFormat(string str, int size) {
            byte[] bytes = new byte[size];
            bytes = enc.GetBytes(str.PadRight(size).Substring(0, size));
            return bytes;
        }

        public static byte[] StringFormat437(string str, int size)
        {
            byte[] bytes = new byte[size];
            for (int i = 0; i < size; i++)
                bytes[i] = (byte)' ';

            for (int i = 0; i < Math.Min(str.Length, size); i++)
                bytes[i] = (byte)str[i];
            return bytes;
        }
        
        public static byte[] HTNO(ushort x) {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }
        public static ushort NTHO(byte[] x, int offset) {
            byte[] y = new byte[2];
            Buffer.BlockCopy(x, offset, y, 0, 2); Array.Reverse(y);
            return BitConverter.ToUInt16(y, 0);
        }
        public static byte[] HTNO(short x) {
            byte[] y = BitConverter.GetBytes(x); Array.Reverse(y); return y;
        }

        public string ReadString(int count = 64) {
            if ( Reader == null ) return null;
            var chars = new byte[count];
            Reader.Read(chars, 0, count);
            return Encoding.UTF8.GetString(chars).TrimEnd().Replace("\0", string.Empty);

        }
    }
}
