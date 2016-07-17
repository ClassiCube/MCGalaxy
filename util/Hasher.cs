/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MCGalaxy.Util {
    internal sealed class PasswordHasher {

        const string path = "extra/passwords/{0}.dat";

        internal static byte[] Compute(string salt, string plainText) {
            if (string.IsNullOrEmpty(salt) )
                throw new ArgumentNullException("salt", "fileName is null or empty");
            if ( string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText", "plainText is null or empty");

            salt = salt.Replace("<", "(");
            salt = salt.Replace(">", ")");
            plainText = plainText.Replace("<", "(");
            plainText = plainText.Replace(">", ")");

            MD5 hash = MD5.Create();

            byte[] textBuffer = Encoding.ASCII.GetBytes(plainText);
            byte[] saltBuffer = Encoding.ASCII.GetBytes(salt);

            byte[] hashedTextBuffer = hash.ComputeHash(textBuffer);
            byte[] hashedSaltBuffer = hash.ComputeHash(saltBuffer);
            return hash.ComputeHash(hashedSaltBuffer.Concat(hashedTextBuffer).ToArray());
        }

        internal static void StoreHash(string salt, string plainText) {
            byte[] hashed = Compute(salt, plainText);
            using (Stream stream = File.Create(string.Format(path, salt)))
                stream.Write(hashed, 0, hashed.Length);
        }

        internal static bool MatchesPass(string salt, string plainText) {
            if (!File.Exists(string.Format(path, salt))) return false;

            byte[] hashed = File.ReadAllBytes(string.Format(path, salt));
            byte[] computed = Compute(salt, plainText);
            if (hashed.Length != computed.Length) return false;
            
            for (int i = 0; i < hashed.Length; i++) {
                if (hashed[i] != computed[i]) return false;
            }
            return true;
        }
    }
}
