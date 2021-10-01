using System.Net;

namespace MCGalaxy.Network 
{
    /// <summary> Repeatedly sends a heartbeat every certain interval to a web server. </summary>
    public abstract class Heartbeat
    {
        public Heartbeat(string URL)
        {
            this.URL = URL;
        }

        /// <summary> The max number of retries attempted for a heartbeat. </summary>
        public int maxRetries = 3;

        /// <summary> Gets the URL the heartbeat is sent to. </summary>
        public string URL;
        
        /// <summary> Salt used for verifying player names </summary>
        public string Salt = "";

        /// <summary> Initialises data for this heartbeat. </summary>
        public abstract void Init();
        
        /// <summary> Gets the data to be sent for a heartbeat. </summary>
        public abstract string GetHeartbeatData();
        
        /// <summary> Called when a request is about to be sent to the web server. </summary>
        public abstract void OnRequest(HttpWebRequest request);
        
        /// <summary> Called when a response is received from the web server. </summary>
        public abstract void OnResponse(string response);
    }
}
