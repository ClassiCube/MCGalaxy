/*
	Copyright 2011 MCGalaxy
	
	Author: fenderrock87
	
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
using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace MCGalaxy
{
    public static class Permalink
    {
        public static Uri URL;
        public static string UniqueHash
        {
            get
            {
                return GenerateUniqueHash();
            }
        }

        private static string GenerateUniqueHash()
        {
            string macs = "";

            // get network interfaces' physical addresses
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                PhysicalAddress pa = ni.GetPhysicalAddress();
                macs += pa.ToString();
            }

            // also add the server's current port, so that one machine may run multiple servers
            macs += Server.port.ToString();

            // generate hash
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] originalBytes = Encoding.ASCII.GetBytes(macs);
                byte[] hash = md5.ComputeHash(originalBytes);

                // convert hash to hex string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                // the the final hash as a string
                return sb.ToString();
            }
        }
    }
}
