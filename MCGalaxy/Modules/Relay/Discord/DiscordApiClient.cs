/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Network;

namespace MCGalaxy.Modules.Relay.Discord
{
    /// <summary> Implements a basic web client for sending messages to the Discord API </summary>
    /// <remarks> https://discord.com/developers/docs/reference </remarks>
    /// <remarks> https://discord.com/developers/docs/resources/channel#create-message </remarks>
    public sealed class DiscordApiClient : AsyncWorker<DiscordApiMessage>
    {
        public string Token;
        public string Host;
        
        DiscordApiMessage GetNextRequest() {
            if (queue.Count == 0) return null;
            DiscordApiMessage first = queue.Dequeue();
            
            // try to combine messages to minimise API calls
            while (queue.Count > 0) {
                DiscordApiMessage next = queue.Peek();
                if (!next.CombineWith(first)) break;
                queue.Dequeue();
            }
            return first;
        }
        
        protected override string ThreadName { get { return "Discord-ApiClient"; } }
        protected override void HandleNext() {
            DiscordApiMessage msg = null;
            WebResponse res = null;
            
            lock (queueLock) { msg = GetNextRequest(); }
            if (msg == null) { WaitForWork(); return;  }
            
            for (int retry = 0; retry < 10; retry++) 
            {
                try {
                    HttpWebRequest req = HttpUtil.CreateRequest(Host + msg.Path);
                    req.Method         = msg.Method;
                    req.ContentType    = "application/json";
                    req.Headers[HttpRequestHeader.Authorization] = "Bot " + Token;
                    
                    string data = Json.SerialiseObject(msg.ToJson());
                    HttpUtil.SetRequestData(req, Encoding.UTF8.GetBytes(data));
                    res = req.GetResponse();
                    
                    string resp = HttpUtil.GetResponseText(res);
                    msg.ProcessResponse(resp);
                    break;
                } catch (WebException ex) {
                    bool canRetry = HandleErrorResponse(ex, msg, retry);
                    HttpUtil.DisposeErrorResponse(ex);
                    
                    if (!canRetry) return;
                } catch (Exception ex) {
                    LogError(ex, msg);
                    return;
                }
            }
            
            // Avoid triggering HTTP 429 error if possible
            string remaining = res.Headers["X-RateLimit-Remaining"];
            if (remaining == "1") SleepForRetryPeriod(res);
        }
        
        static bool HandleErrorResponse(WebException ex, DiscordApiMessage msg, int retry) {
            string err = HttpUtil.GetErrorResponse(ex);
            HttpStatusCode status = GetStatus(ex);
            
            // 429 errors simply require retrying after sleeping for a bit
            if (status == (HttpStatusCode)429) {
                SleepForRetryPeriod(ex.Response);
                return true;
            }
            
            // 500 errors might be temporary Discord outage, so still retry a few times
            if (status >= (HttpStatusCode)500 && status <= (HttpStatusCode)504) {
                LogWarning(ex);
                LogResponse(err);
                return retry < 2;
            }
            
            // If unable to reach Discord at all, immediately give up
            if (ex.Status == WebExceptionStatus.NameResolutionFailure) {
                LogWarning(ex);
                return false;
            }
            
            // May be caused by connection dropout/reset, so still retry a few times
            if (ex.InnerException is IOException) {
                LogWarning(ex);
                return retry < 2;
            }
            
            LogError(ex, msg);
            LogResponse(err);
            return false;
        }
        
        
        static HttpStatusCode GetStatus(WebException ex) {
            if (ex.Response == null) return 0;            
            return ((HttpWebResponse)ex.Response).StatusCode;
        }
        
        static void LogError(Exception ex, DiscordApiMessage msg) {
            string target = "(" + msg.Method + " " + msg.Path + ")";
            Logger.LogError("Error sending request to Discord API " + target, ex);
        }
        
        static void LogWarning(Exception ex) {
            Logger.Log(LogType.Warning, "Error sending request to Discord API - " + ex.Message);
        }
        
        static void LogResponse(string err) {
            if (string.IsNullOrEmpty(err)) return;
            
            // Discord sometimes returns <html>..</html> responses for internal server errors
            //  most of this is useless content, so just truncate these particular errors 
            if (err.Length > 200) err = err.Substring(0, 200) + "...";
            
            Logger.Log(LogType.Warning, "Discord API returned: " + err);
        }
        
        
        static void SleepForRetryPeriod(WebResponse res) {
            string resetAfter = res.Headers["X-RateLimit-Reset-After"];
            string retryAfter = res.Headers["Retry-After"];
            float delay;
            
            if (NumberUtils.TryParseSingle(resetAfter, out delay) && delay > 0) {
                // Prefer Discord "X-RateLimit-Reset-After" (millisecond precision)
            } else if (NumberUtils.TryParseSingle(retryAfter, out delay) && delay > 0) {
                // Fallback to general "Retry-After" header
            } else {
                // No recommended retry delay.. 30 seconds is a good bet
                delay = 30;
            }

            Logger.Log(LogType.SystemActivity, "Discord bot ratelimited! Trying again in {0} seconds..", delay);
            Thread.Sleep(TimeSpan.FromSeconds(delay + 0.5f));
        }
    }
}
