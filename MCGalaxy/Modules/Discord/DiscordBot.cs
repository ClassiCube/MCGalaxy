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

    public sealed class DiscordBot {
        DiscordWebsocket socket;
        DiscordConfig config;
        
        string lastSequence;
        Thread thread;
        
        const int OPCODE_DISPATCH        = 0;
        const int OPCODE_HEARTBEAT       = 1;
        const int OPCODE_IDENTIFY        = 2;
        const int OPCODE_STATUS_UPDATE   = 3;
        const int OPCODE_VOICE_STATE_UPDATE = 4;
        const int OPCODE_RESUME          = 6;
        const int OPCODE_REQUEST_SERVER_MEMBERS = 8;
        const int OPCODE_INVALID_SESSION = 9;
        const int OPCODE_HELLO           = 10;
        const int OPCODE_HEARTBEAT_ACK   = 11;
        
        void HandlePacket(string value) {
            JsonReader ctx = new JsonReader(value);
            JsonObject obj = (JsonObject)ctx.Parse();
            
            Logger.Log(LogType.SystemActivity, value);
            if (obj == null) return;
            
            int opcode = int.Parse((string)obj["op"]);
            DispatchPacket(opcode, obj);
        }
        
        void DispatchPacket(int opcode, JsonObject obj) {
            if (opcode == OPCODE_DISPATCH) HandleDispatch(obj);
            if (opcode == OPCODE_HELLO)    HandleHello(obj);
        }
        
        
        void HandleHello(JsonObject obj) {
            JsonObject data = (JsonObject)obj["d"];
            string interval = (string)data["heartbeat_interval"];            
            int msInterval  = int.Parse(interval);
            
            Server.Background.QueueRepeat(SendHeartbeat, null, 
                                          TimeSpan.FromMilliseconds(msInterval));
            SendIdentify();
        }
        
        void HandleDispatch(JsonObject obj) {
            // update last sequence number
            object sequence;
            if (obj.TryGetValue("s", out sequence)) 
                lastSequence = (string)sequence;
            
            // actually handle the event
            string eventName = (string)obj["t"];
            if (eventName == "MESSAGE_CREATE") HandleMessageEvent(obj);
        }
        
        void HandleMessageEvent(JsonObject obj) {
            JsonObject data   = (JsonObject)obj["d"];
            JsonObject author = (JsonObject)data["author"];
            string message    = (string)data["content"];
            
            string user = (string)author["username"];
            string msg  = "&I(Discord) " + user + ": &f" + message;
            Logger.Log(LogType.IRCChat, msg);
            Chat.Message(ChatScope.Global, msg, null, null);
        }
        
        
        void SendHeartbeat(SchedulerTask task) {
            JsonObject obj = new JsonObject();
            obj["op"] = OPCODE_HEARTBEAT;
            
            if (lastSequence != null) {
                obj["d"] = int.Parse(lastSequence);
            } else {
                obj["d"] = null;
            }
            socket.SendMessage(obj);
        }
        
        const int INTENT_GUILD_MESSAGES = 1 << 9;
        
        void SendIdentify() {
            JsonObject data = new JsonObject();
            
            JsonObject props = new JsonObject();
            props["$os"] = "linux";
            props["$browser"] = "MCGRelayBot";
            props["$device"]  = "MCGRelayBot";
            
            data["token"]   = socket.Token;
            data["intents"] = INTENT_GUILD_MESSAGES;
            data["properties"] = props;
            data["presence"]   = MakePresence();
            socket.SendMessage(OPCODE_IDENTIFY, data);
        }
        
        void SendUpdateStatus() {
        	JsonObject data = MakePresence();
        	socket.SendMessage(OPCODE_STATUS_UPDATE, data);
        }
        
        JsonObject MakePresence() {
        	string online = PlayerInfo.NonHiddenCount().ToString();
            JsonObject activity = new JsonObject();
        	activity["name"]    = config.Status.Replace("{PLAYERS}", online);
            activity["type"]    = 0;
            
            JsonArray activites = new JsonArray();
            activites.Add(activity);
            
            JsonObject obj = new JsonObject();
            obj["activities"] = activites;
            obj["status"]     = "online";
            obj["afk"]        = false;
            return obj;
        }
        
        
        public void RunAsync(DiscordConfig conf) {
            config = conf;
            socket = new DiscordWebsocket();
            socket.Token   = config.BotToken;
            socket.Handler = HandlePacket;
                
            thread      = new Thread(IOThread);
            thread.Name = "MCG-DiscordRelay";
            thread.IsBackground = true;
            thread.Start();
        } // todo hide / unhide
        
        void IOThread() {
            try {
                socket.Connect();
                socket.ReadLoop();
            } catch (Exception ex) {
                Logger.LogError("Discord relay error", ex);
            }
        }
    }
}
