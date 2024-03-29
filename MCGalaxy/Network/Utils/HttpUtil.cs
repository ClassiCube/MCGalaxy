﻿/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
using System.Net.Security;
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
        
        /// <summary> Attempts to read the WebResponse in the given exception into a string </summary>
        /// <remarks> Returns null if the exception did not contain a readable WebResponse </remarks>
        public static string GetErrorResponse(Exception ex) {
            try {
                WebException webEx = ex as WebException;
                if (webEx != null && webEx.Response != null)
                    return GetResponseText(webEx.Response);
            } catch {  }
            return null;
        }
        
        /// <summary> Disposes the WebResponse in the given exception to avoid resource leakage </summary>
        /// <remarks> Does nothing if there is no WebResponse </remarks>
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
            if (Server.Listener.IP != null) {
                localIP = Server.Listener.IP;
            } else if (!IPAddress.TryParse(Server.Config.ListenIP, out localIP)) {
                return null;
            }
            
            // can only use same family for local bind IP
            if (remoteEP.AddressFamily != localIP.AddressFamily) return null;
            return new IPEndPoint(localIP, 0);
        }
        
        
        // TLS 1.1/1.2 do not exist in .NET 4.0 and cause a compilation failure
        public const SslProtocols TLS_11  = (SslProtocols)768;
        public const SslProtocols TLS_12  = (SslProtocols)3072;
        public const SslProtocols TLS_ALL = SslProtocols.Tls | TLS_11 | TLS_12;
        
        public static SslStream WrapSSLStream(Stream source, string host) {
            SslStream wrapped  = new SslStream(source);
            wrapped.AuthenticateAsClient(host, null, TLS_ALL, false);
            return wrapped;
        }
        
        
        const string DROPBOX_HTTP_PREFIX  = "http://www.dropbox";
        const string DROPBOX_HTTPS_PREFIX = "https://www.dropbox";
        
        /// <summary> Prefixes a URL by http:// if needed, and converts dropbox webpages to direct links. </summary>
        public static void FilterURL(ref string url) {
            if (!url.CaselessStarts("http://") && !url.CaselessStarts("https://"))
                url = "http://" + url;
            
            // a lot of people try linking to the dropbox page instead of directly to file, so auto correct
            if (url.CaselessStarts(DROPBOX_HTTP_PREFIX)) {
                url = AdjustDropbox(url, DROPBOX_HTTP_PREFIX.Length);
            } else if (url.CaselessStarts(DROPBOX_HTTPS_PREFIX)) {
                url = AdjustDropbox(url, DROPBOX_HTTPS_PREFIX.Length);
            }
            
            url = url.Replace("dl.dropboxusercontent.com", "dl.dropbox.com");
        }
        
        static string AdjustDropbox(string url, int prefixLen) {
            url = "https://dl.dropbox" + url.Substring(prefixLen);

            return url
                .Replace("dl=1", "dl=0")
                .Replace("?dl=0", "")
                .Replace("&dl=0", "")
                .Replace("%dl=0", "");
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
        
        
        public static string LookupExternalIP() {
           HttpWebRequest req = CreateRequest("http://classicube.net/api/myip/");
           
           using (WebResponse response = req.GetResponse())
           {
               return GetResponseText(response);
           }
        }
    }
}