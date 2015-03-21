/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
/*
 *    HTTPGet.cs | C# .NET 2.0 HTTP GET Class
 *    Copyright (c) 2008, Corey Goldberg
 *
 *    HTTPGet.cs is free software; you can redistribute it and/or modify
 *    it under the terms of the GNU General Public License as published by
 *    the Free Software Foundation; either version 3 of the License, or
 *    (at your option) any later version.
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
namespace MCGalaxy
{
    public sealed class HTTPGet
    {
        private HttpWebRequest request;
        private HttpWebResponse response;

        private string responseBody;
        private string escapedBody;
        private int statusCode;
        private double responseTime;

        public string ResponseBody { get { return responseBody; } }
        public string EscapedBody { get { return GetEscapedBody(); } }
        public int StatusCode { get { return statusCode; } }
        public double ResponseTime { get { return responseTime; } }
        public string Headers { get { return GetHeaders(); } }
        public string StatusLine { get { return GetStatusLine(); } }

        public void Request(string url)
        {
            Stopwatch timer = new Stopwatch();
            StringBuilder respBody = new StringBuilder();

            this.request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                timer.Start();
                this.response = (HttpWebResponse)this.request.GetResponse();
                byte[] buf = new byte[8192];
                Stream respStream = this.response.GetResponseStream();
                int count = 0;
                do
                {
                    count = respStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                        respBody.Append(Encoding.ASCII.GetString(buf, 0, count));
                }
                while (count > 0);
                timer.Stop();

                this.responseBody = respBody.ToString();
                this.statusCode = (int)(HttpStatusCode)this.response.StatusCode;
                this.responseTime = timer.ElapsedMilliseconds / 1000.0;
            }
            catch (WebException ex)
            {
                this.response = (HttpWebResponse)ex.Response;
                this.responseBody = "No Server Response";
                this.escapedBody = "No Server Response";
                this.responseTime = 0.0;
            }
        }

        private string GetEscapedBody()
        {  // HTML escaped chars
            string escapedBody = responseBody;
            escapedBody = escapedBody.Replace("&", "&amp;");
            escapedBody = escapedBody.Replace("<", "&lt;");
            escapedBody = escapedBody.Replace(">", "&gt;");
            escapedBody = escapedBody.Replace("'", "&apos;");
            escapedBody = escapedBody.Replace("\"", "&quot;");
            this.escapedBody = escapedBody;

            return escapedBody;
        }

        private string GetHeaders()
        {
            if (response == null)
                return "No Server Response";
            else
            {
                StringBuilder headers = new StringBuilder();
                for (int i = 0; i < this.response.Headers.Count; ++i)
                    headers.Append(String.Format("{0}: {1}\n",
                        response.Headers.Keys[i], response.Headers[i]));

                return headers.ToString();
            }
        }

        private string GetStatusLine()
        {
            if (response == null)
                return "No Server Response";
            else
                return String.Format("HTTP/{0} {1} {2}", response.ProtocolVersion,
                    (int)response.StatusCode, response.StatusDescription);
        }
    }
}