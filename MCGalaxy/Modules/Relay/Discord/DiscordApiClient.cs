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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Network;

namespace MCGalaxy.Modules.Relay.Discord {
    
    /// <summary> Represents an abstract Discord API message </summary>
    public abstract class DiscordApiMessage {
        /// <summary> The path/route that will handle this message </summary>
        public string Path;
        
        /// <summary> Converts this message into its JSON representation </summary>
        public abstract JsonObject ToJson();
        
        /// <summary> Attempts to combine this message with a prior message to reduce API calls </summary>
        public virtual bool CombineWith(DiscordApiMessage prior) { return false; }
    }
    
    /// <summary> Message for sending text to a channel </summary>
    public class ChannelSendMessage : DiscordApiMessage {
        StringBuilder content;
        
        public ChannelSendMessage(string channelID, string message) {
            Path    = "/channels/" + channelID + "/messages";
            content = new StringBuilder(message);
        }
        
        public override JsonObject ToJson() {
            // no pinging everyone
            JsonObject allowed = new JsonObject()
            {
                { "parse", new JsonArray() { "users", "roles" } }
            };
            JsonObject obj = new JsonObject()
            {
                { "content", content.ToString() },
                { "allowed_mentions", allowed }
            };
            return obj;
        }
        
        public override bool CombineWith(DiscordApiMessage prior) {
            ChannelSendMessage msg = prior as ChannelSendMessage;
            if (msg == null || msg.Path != Path) return false;
            
            if (content.Length + msg.content.Length > 1024) return false;
            
            // TODO: is stringbuilder even beneficial here
            msg.content.Append('\n');
            msg.content.Append(content.ToString());
            content.Clear();
            return true;
        }
    }
    
    public class ChannelSendEmbed : ChannelSendMessage {
        public string Title;
        public Dictionary<string, string> Fields = new Dictionary<string, string>();
        
        public ChannelSendEmbed(string channelID) : base(channelID, null) { }
        
        JsonArray GetFields() {
            JsonArray arr = new JsonArray();
            foreach (var raw in Fields) {
                
                JsonObject field = new JsonObject()
                {
                    { "name",   raw.Key  },
                    { "value", raw.Value }
                };
                arr.Add(field);
            }
            return arr;
        }
        
        public override JsonObject ToJson() {
            JsonObject obj = base.ToJson();
            obj.Remove("content");
            
            obj["embed"] = new JsonObject()
            {
                { "title", Title },
                { "color", 9758051 },
                { "fields", GetFields() }
            };
            return obj;
        }
        
        public override bool CombineWith(DiscordApiMessage prior) { return false; }
    }
    
    /// <summary> Implements a basic web client for sending messages to the Discord API </summary>
    /// <remarks> https://discord.com/developers/docs/reference </remarks>
    /// <remarks> https://discord.com/developers/docs/resources/channel#create-message </remarks>
    public sealed class DiscordApiClient {
        public string Token;
        const string host = "https://discord.com/api/v8";
        AutoResetEvent handle = new AutoResetEvent(false);
        volatile bool terminating;
        
        Queue<DiscordApiMessage> requests = new Queue<DiscordApiMessage>();
        readonly object reqLock = new object();
            
        DiscordApiMessage GetNextRequest() {
            if (requests.Count == 0) return null;
            DiscordApiMessage first = requests.Dequeue();
            
            // try to combine messages to minimise API calls
            while (requests.Count > 0) {
                DiscordApiMessage next = requests.Peek();
                if (!next.CombineWith(first)) break;
                requests.Dequeue();
            }
            return first;
        }
        
        void HandleNext() {
            DiscordApiMessage msg = null;
            lock (reqLock)   { msg = GetNextRequest(); }
            if (msg == null) { handle.WaitOne(); return; }
            
            HttpWebRequest req = HttpUtil.CreateRequest(host + msg.Path);
            req.Method         = "POST";
            req.ContentType    = "application/json";
            req.Headers[HttpRequestHeader.Authorization] = "Bot " + Token;
            
            string data = Json.SerialiseObject(msg.ToJson());
            HttpUtil.SetRequestData(req, Encoding.UTF8.GetBytes(data));
            WebResponse res = req.GetResponse();
            
            string response = HttpUtil.GetResponseData(res);
            Logger.Log(LogType.SystemActivity, response);
        }
        
        void SendLoop() {
            for (;;) {
                if (terminating) break;
                
                try {
                    HandleNext();
                } catch (Exception ex) {
                    Logger.LogError(ex);
                }
            }
            
            // cleanup state
            try { 
                lock (reqLock) requests.Clear();
                handle.Dispose(); 
            } catch {
            }
        }
        
        
        void WakeupWorker() {
            try {
                handle.Set();
            } catch (ObjectDisposedException) {
                // for very rare case where handle's already been destroyed
            }
        }
        
        public void RunAsync() {
            Thread worker = new Thread(SendLoop);
            worker.Name   = "Discord-ApiClient";
            worker.IsBackground = true;
            worker.Start();
        }
        
        public void StopAsync() {
            terminating = true;
            WakeupWorker();
        }       
        
        /// <summary> Asynchronously sends a message to the Discord API </summary>
        public void SendAsync(DiscordApiMessage msg) {
            lock (reqLock) requests.Enqueue(msg);
            WakeupWorker();
        }
        
        public void SendMessageAsync(string channelID, string message) {
            SendAsync(new ChannelSendMessage(channelID, message));
        }
    }
}
