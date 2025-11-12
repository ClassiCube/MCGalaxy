/*
    Copyright 2015-2024 MCGalaxy
        
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
using System.IO.Compression;
using MCGalaxy.Util;

namespace MCGalaxy.Util.Imaging
{
    public class JpegDecoder : ImageDecoder
    {
        static byte[] jfifSig = new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }; // "SOI", "APP0"

        public static bool DetectHeader(byte[] data) {
            return MatchesSignature(data, jfifSig);
        }
        
        public override SimpleBitmap Decode(byte[] src) {
            Fail("JPEG decoder unfinished");
            return null;
        }
    }
}