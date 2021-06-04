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
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;

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
        
        public static void SetRequestData(WebRequest request, byte[] data) {
            request.ContentLength = data.Length;
            using (Stream w = request.GetRequestStream()) {
                w.Write(data, 0, data.Length);
            }
        }
        
        public static string GetResponseText(WebResponse response) {
            using (StreamReader r = new StreamReader(response.GetResponseStream())) {
                return r.ReadToEnd().Trim();
            }
        }
        
        /// <summary> Disposes the WebResponse in the given exception 
        /// (if there is one) to avoid resource leakage </summary>
        public static void DisposeErrorResponse(Exception ex) {
            try {
                WebException webEx = ex as WebException;
                if (webEx != null && webEx.Response != null) webEx.Response.Close();
            } catch { }
        }
        

        class CustomWebClient : WebClient {
            protected override WebRequest GetWebRequest(Uri address) {
                HttpWebRequest req = (HttpWebRequest)base.GetWebRequest(address);
                req.ServicePoint.BindIPEndPointDelegate = BindIPEndPointCallback;
                req.UserAgent = Server.SoftwareNameVersioned;
                return (WebRequest)req;
            }
        }
        
        static IPEndPoint BindIPEndPointCallback(ServicePoint servicePoint, IPEndPoint remoteEP, int retryCount) {
            IPAddress localIP = null;
            if (Server.Listener != null) {
                localIP = Server.Listener.IP;
            } else if (!IPAddress.TryParse(Server.Config.ListenIP, out localIP)) {
                return null;
            }
            
            // can only use same family for local bind IP
            if (remoteEP.AddressFamily != localIP.AddressFamily) return null;
            return new IPEndPoint(localIP, 0);
        }
        
        
        // these do not exist in .NET 4.0 and cause a compilation failure
        const SslProtocols tls_11 = (SslProtocols)768;
        const SslProtocols tls_12 = (SslProtocols)3072;
        
        public static SslStream WrapSSLStream(Stream source, string host) {
            SslStream wrapped  = new SslStream(source);
            SslProtocols flags = SslProtocols.Tls | tls_11 | tls_12;
            wrapped.AuthenticateAsClient(host, null, flags, false);
            return wrapped;
        }
        
        /// <summary> Prefixes a URL by http:// if needed, and converts dropbox webpages to direct links. </summary>
        public static void FilterURL(ref string url) {
            if (!url.CaselessStarts("http://") && !url.CaselessStarts("https://"))
                url = "http://" + url;
            
            // a lot of people try linking to the dropbox page instead of directly to file, so auto correct
            if (url.CaselessStarts("http://www.dropbox")) {
                url = "http://dl.dropbox" + url.Substring("http://www.dropbox".Length);
                url = url.Replace("?dl=0", "");
            } else if (url.CaselessStarts("https://www.dropbox")) {
                url = "https://dl.dropbox" + url.Substring("https://www.dropbox".Length);
                url = url.Replace("?dl=0", "");
            }
            
            url = url.Replace("dl.dropboxusercontent.com", "dl.dropbox.com");
        }
        
        static bool CheckHttpOrHttps(Player p, string url) {
            Uri uri;
            // only check valid URLs here
            if (!url.Contains("://")) return true;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) return true;
            
            string scheme = uri.Scheme;
            if (scheme.CaselessEq("http") || scheme.CaselessEq("https")) return true;
            
            p.Message("&WOnly http:// or https:// urls are supported, " +
                      "{0} is a {1}:// url", url, scheme);
            return false;
        }
        
        /// <summary> Prefixes a URL by http:// if needed, and converts dropbox webpages to direct links. </summary>
        /// <remarks> Ensures URL is a valid http/https URI. </remarks>
        public static Uri GetUrl(Player p, ref string url) {
            Uri uri;
            if (!CheckHttpOrHttps(p, url)) return null;
            FilterURL(ref url);
            
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri)) {
                p.Message("&W{0} is not a valid URL.", url); return null;
            }
            return uri;
        }
        
        static string DescribeError(Exception ex) {
            try {
                WebException webEx = (WebException)ex;
                // prefer explicit http status error codes if possible
                try {
                    int status = (int)((HttpWebResponse)webEx.Response).StatusCode;
                    return "(" + status + " error) from ";
                } catch {
                    return "(" + webEx.Status + ") from ";
                }
            } catch {
                return null;
            }
        }
        
        public static byte[] DownloadData(string url, Player p) {
            Uri uri = GetUrl(p, ref url);
            if (uri == null) return null;
            return DownloadData(p, url, uri);
        }
        
        static byte[] DownloadData(Player p, string url, Uri uri) {
            byte[] data = null;
            try {
                using (WebClient client = CreateWebClient()) {
                    p.Message("Downloading file from: &f" + url);
                    data = client.DownloadData(uri);
                }
                p.Message("Finished downloading.");
            } catch (Exception ex) {                
                string msg = DescribeError(ex);
                
                if (msg == null) {
                    // unexpected error, log full error details
                    msg = "from ";
                    Logger.LogError("Error downloading " + url, ex);
                } else {
                    // known error, so just log a warning
                    string logMsg = msg + url + Environment.NewLine + ex.Message;
                    Logger.Log(LogType.Warning, "Error downloading " + logMsg);
                }
                
                p.Message("&WFailed to download {0}&f{1}", msg, url);
                return null;
            }
            return data;
        }
        
        public static byte[] DownloadImage(string url, Player p) {
            Uri uri = GetUrl(p, ref url);
            if (uri == null) return null;
            
            byte[] data = DownloadData(p, url, uri);
            if (data == null) p.Message("&WThe url may need to end with its extension (such as .jpg).");
            return data;
        }
    }
}