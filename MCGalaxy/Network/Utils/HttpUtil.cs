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
using System.Net;

namespace MCGalaxy.Network {
    /// <summary> Static class for assisting with making web requests. </summary>
    public static class HttpUtil {

        public static WebClient CreateWebClient() { return new CustomWebClient(); }
        
        public static HttpWebRequest CreateRequest(string uri) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.ServicePoint.BindIPEndPointDelegate = BindIPEndPointCallback;
            req.UserAgent = Server.SoftwareNameVersioned;
            return req;
        }


        class CustomWebClient : WebClient {
            protected override WebRequest GetWebRequest(Uri address) {
                HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
                req.ServicePoint.BindIPEndPointDelegate = BindIPEndPointCallback;
                req.UserAgent = Server.SoftwareNameVersioned;
                return (WebRequest)req;
            }
        }        
        
        static IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) {
            IPAddress localIP = null;
            if (Server.Listener != null) {
                localIP = Server.Listener.LocalIP;
            } else if (!IPAddress.TryParse(Server.listenIP, out localIP)) {
                return null;
            }
            
            // can only use same family for local bind IP
            if (remoteEndPoint.AddressFamily != localIP.AddressFamily) return null;
            return new IPEndPoint(localIP, 0);
        }
    }
}