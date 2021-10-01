using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Cache;
using System.Text;
using MCGalaxy.Tasks;

namespace MCGalaxy.Network
{
    public class HeartbeatService
    {
        /// <summary> List of all heartbeats to pump. </summary>
        private List<Heartbeat> heartbeats = new List<Heartbeat>() { };

        public ReadOnlyCollection<Heartbeat> ReadHeartbeats()
        {
            return heartbeats.AsReadOnly();
        }
        
        public void RegisterHeartbeat(Heartbeat heartbeat)
        {
            heartbeats.Add(heartbeat);
            heartbeat.Init();
            heartbeat.Salt = Server.GenerateSalt();
        }

        public void StartBeating()
        {
            if (heartbeatTask != null) return;
            heartbeatTask = Server.Background.QueueRepeat(OnBeat, null, 
                TimeSpan.FromSeconds(30));
        }

        SchedulerTask heartbeatTask;
        void OnBeat(SchedulerTask task) {
            foreach (Heartbeat beat in heartbeats) { Pump(beat); }
        }
        

        /// <summary> Pumps the specified heartbeat. </summary>
        public void Pump(Heartbeat beat) {
            byte[] data = Encoding.ASCII.GetBytes(beat.GetHeartbeatData());
            Exception lastEx = null;

            for (int i = 0; i < beat.maxRetries; i++) {
                try {
                    HttpWebRequest req = HttpUtil.CreateRequest(beat.URL);
                    req.Method = "POST";
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                    req.Timeout = 10000;
                    
                    beat.OnRequest(req);
                    HttpUtil.SetRequestData(req, data);
                    WebResponse res = req.GetResponse();
                    
                    string response = HttpUtil.GetResponseText(res);
                    beat.OnResponse(response);
                    return;
                } catch (Exception ex) {
                    HttpUtil.DisposeErrorResponse(ex);
                    lastEx = ex;
                }
            }
            
            string hostUrl = new Uri(beat.URL).Host;
            Logger.Log(LogType.Warning, "Failed to send heartbeat to {0} ({1})", hostUrl, lastEx.Message);
        }
    }
}