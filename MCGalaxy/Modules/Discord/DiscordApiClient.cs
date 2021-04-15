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
using System.Net;
using MCGalaxy.Config;
using MCGalaxy.Network;

namespace MCGalaxy.Modules.Discord {
    
	/// <summary> Implements a basic web client for communicating with Discord's API </summary>
	/// <remarks> https://discord.com/developers/docs/reference </remarks>
	/// <remarks> https://discord.com/developers/docs/resources/channel#create-message </remarks>
    public sealed class DiscordApiClient {
        public string Token;
        
        const string host = "https://discord.com/api";
        
        public void MakeRequest(string path, JsonObject obj) {
            // TODO HttpWebRequest
            string data = Json.SerialiseObject(obj);
            
        	using (WebClient client = HttpUtil.CreateWebClient()) {
            	client.Headers[HttpRequestHeader.ContentType] = "application/json";
            	client.Headers[HttpRequestHeader.Authorization] = "Bot " + Token;

            	string resp = client.UploadString(host + path, data);
            	Logger.Log(LogType.SystemActivity, resp);
        	}
        }
        
        public void SendMessage(string channelID, string message) {
        	JsonObject obj = new JsonObject();
        	obj["content"] = message;
        	
        	string path = "/channels/" + channelID + "/messages";
        	MakeRequest(path, obj);
        }
    }
}
