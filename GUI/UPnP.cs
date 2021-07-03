/*
Copyright 2012 MCForge
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
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Xml;
using MCGalaxy.Network;
//This upnp class comes from http://www.codeproject.com/Articles/27992/NAT-Traversal-with-UPnP-in-C, Modified for use with MCForge

namespace MCGalaxy {
    public static class UPnP {

        public static TimeSpan Timeout = TimeSpan.FromSeconds(3);
        
        const string req = 
            "M-SEARCH * HTTP/1.1\r\n" +
            "HOST: 239.255.255.250:1900\r\n" +
            "ST:upnp:rootdevice\r\n" +
            "MAN:\"ssdp:discover\"\r\n" +
            "MX:3\r\n" +
            "\r\n";

        static string _serviceUrl;
        static List<string> visitedLocations = new List<string>();
        
        
        public static bool Discover() {
            Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            
            byte[] data = Encoding.ASCII.GetBytes(req);
            IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast, 1900);
            byte[] buffer = new byte[0x1000];

            DateTime start = DateTime.UtcNow;
            s.ReceiveTimeout = 3000;
            visitedLocations.Clear();
            
            try {
                do {
                    s.SendTo(data, ep);
                    s.SendTo(data, ep);
                    s.SendTo(data, ep);

                    int length = -1;
                    do {
                        length = s.Receive(buffer);
                        string resp = Encoding.ASCII.GetString(buffer, 0, length);
                        
                        if (resp.Contains("upnp:rootdevice")) {
                            string location = resp.Substring(resp.ToLower().IndexOf("location:") + 9);
                            location = location.Substring(0, location.IndexOf("\r")).Trim();
                            
                            if (!visitedLocations.Contains(location)) {
                                visitedLocations.Add(location);
                                _serviceUrl = GetServiceUrl(location);
                                if (!String.IsNullOrEmpty(_serviceUrl)) return true;
                            }
                        }
                    } while (length > 0);
                } while (start.Subtract(DateTime.UtcNow) < Timeout);
                return false;
            } catch {
                return false;
            }
        }

        public static void ForwardPort(int port, ProtocolType protocol, string description) {
            if (String.IsNullOrEmpty(_serviceUrl) )
                throw new InvalidOperationException("No UPnP service available or Discover() has not been called");
            
            string xdoc = SOAPRequest(_serviceUrl, 
                "<u:AddPortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
                "<NewRemoteHost></NewRemoteHost>" +
                "<NewExternalPort>" + port + "</NewExternalPort>" +
                "<NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
                "<NewInternalPort>" + port + "</NewInternalPort>" +
                "<NewInternalClient>" + GetLocalIP() + "</NewInternalClient>" +
                "<NewEnabled>1</NewEnabled>" +
                "<NewPortMappingDescription>" + description + "</NewPortMappingDescription>" +
                "<NewLeaseDuration>0</NewLeaseDuration>" +
                "</u:AddPortMapping>", "AddPortMapping");
        }

        public static void DeleteForwardingRule(int port, ProtocolType protocol) {
            if (String.IsNullOrEmpty(_serviceUrl) )
                throw new InvalidOperationException("No UPnP service available or Discover() has not been called");
            
            string xdoc = SOAPRequest(_serviceUrl,
            "<u:DeletePortMapping xmlns:u=\"urn:schemas-upnp-org:service:WANIPConnection:1\">" +
            "<NewRemoteHost></NewRemoteHost>" +
            "<NewExternalPort>" + port + "</NewExternalPort>" +
            "<NewProtocol>" + protocol.ToString().ToUpper() + "</NewProtocol>" +
            "</u:DeletePortMapping>", "DeletePortMapping");
        }
        

        static string GetServiceUrl(string location) {
#if !DEBUG
            try {
#endif
                XmlDocument doc = new XmlDocument();
                WebRequest request = WebRequest.CreateDefault(new Uri(location));
                doc.Load(request.GetResponse().GetResponseStream());
                
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(doc.NameTable);
                nsMgr.AddNamespace("tns", "urn:schemas-upnp-org:device-1-0");
                XmlNode node;
                
                node = doc.SelectSingleNode("//tns:device/tns:deviceType/text()", nsMgr);                
                if (node == null || !node.Value.Contains("InternetGatewayDevice"))
                    return null;
                
                node = doc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:1\"]/tns:controlURL/text()", nsMgr);
                if (node != null) return CombineUrls(location, node.Value);
                
                // Try again with version 2
                node = doc.SelectSingleNode("//tns:service[tns:serviceType=\"urn:schemas-upnp-org:service:WANIPConnection:2\"]/tns:controlURL/text()", nsMgr);
                if (node != null) return CombineUrls(location, node.Value);
                
                return null;
#if !DEBUG
            } catch { return null; }
#endif
        }        

        static string CombineUrls(string location, string p) {
            int n = location.IndexOf("://");
            n = location.IndexOf('/', n + 3);
            return location.Substring(0, n) + p;
        }
        
        static string GetLocalIP() {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());        
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            return "?";
        }        

        /// <summary> Performs a XML SOAP request </summary>
        /// <returns> XML response from the service </returns>
        static string SOAPRequest(string url, string soap, string function) {
            string req = 
                "<?xml version=\"1.0\"?>" +
                "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">" +
                "<s:Body>" + soap + "</s:Body>" +
                "</s:Envelope>";
            
            WebRequest r = HttpWebRequest.Create(url);
            r.Method = "POST";            
            r.Headers.Add("SOAPACTION", "\"urn:schemas-upnp-org:service:WANIPConnection:1#" + function + "\"");
            r.ContentType = "text/xml; charset=\"utf-8\"";
            
            byte[] data = Encoding.UTF8.GetBytes(req);
            HttpUtil.SetRequestData(r, data);
            
            WebResponse res = r.GetResponse();
            return HttpUtil.GetResponseText(res);
        }
    }
}
