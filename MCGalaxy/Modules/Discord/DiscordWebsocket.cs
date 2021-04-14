/*
    Copyright 2015 MCGalaxy
        
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
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Network;
using MCGalaxy.Tasks;

namespace MCGalaxy.Modules.Discord {
    
    public sealed class DiscordWebsocket : ClientWebSocket {
        public string Token;
        public Action<string> Handler;
        TcpClient client;
        SslStream stream;
        
        public DiscordWebsocket() {
            path = "/?v=6&encoding=json";
        }
        
        const string host = "gateway.discord.gg";
        // stubs
        public override bool LowLatency { set { } }
        public override string IP { get { return ""; } }
        
        public void Connect() {
            client = new TcpClient();
            client.Connect(host, 443);

            stream   = HttpUtil.WrapSSLStream(client.GetStream(), host);
            protocol = this;
            Init();
        }
        
        public void SendMessage(int opcode, JsonObject data) {
            JsonObject obj = new JsonObject();
            obj["op"] = opcode;
            obj["d"]  = data;
            SendMessage(obj);
        }
        
        public void SendMessage(JsonObject obj) {
            StringWriter dst  = new StringWriter();
            JsonWriter   w    = new JsonWriter(dst);
            w.SerialiseObject = raw => JsonSerialisers.WriteObject(w, raw);
            w.WriteObject(obj);
            
            string str = dst.ToString();
            Send(Encoding.UTF8.GetBytes(str), SendFlags.None);
        }
        
        public void ReadLoop() {
            byte[] data = new byte[4096];
            for (;;) {
                int len = stream.Read(data, 0, 4096);
                if (len == 0) break; // disconnected
                HandleReceived(data, len);
            }
        }
        
        protected override void HandleData(byte[] data, int len) {
            string value = Encoding.UTF8.GetString(data, 0, len);
            Handler(value);
        }
        
        protected override void SendRaw(byte[] data, SendFlags flags) {
            stream.Write(data);
        }
        
        public override void Close() {
            client.Close();
        }
        
        protected override void Disconnect(int reason) {
            base.Disconnect(reason);
            Close();
        }
        
        protected override void WriteCustomHeaders() {
            WriteHeader("Authorization: Bot " + Token);
            WriteHeader("Host: " + host);
        }
    }
}
