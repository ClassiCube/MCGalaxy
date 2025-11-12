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
    public sealed class SimpleBitmap //: IBitmap2D
    {
        public int Width, Height;
        public Pixel[] pixels;
    }
    
    public abstract class ImageDecoder
    {
        protected byte[] buf_data;
        protected int buf_offset, buf_length;       
        
        protected int AdvanceOffset(int amount) {
            int offset = buf_offset;
            
            buf_offset += amount;
            if (buf_offset > buf_length)
                throw new EndOfStreamException("End of stream reading data");
            return offset;
        }
        
        protected static void Fail(string reason) {
            throw new InvalidDataException(reason);
        }
        
        /// <summary> Checks if starting bytes of data match given signature </summary>
        /// <remarks> Ignores parts of sig that are &lt; 0 values </remarks>
        protected static bool MatchesSignature(byte[] data, byte[] sig) {
            if (data.Length < sig.Length) return false;
            
            for (int i = 0; i < sig.Length; i++)
            {
                if (data[i] != sig[i]) return false;
            }
            return true;
        }
        
        
        public static SimpleBitmap DecodeFrom(byte[] src) {
            ImageDecoder decoder = DetectFrom(src);
            if (decoder != null) return decoder.Decode(src);
            
            throw new InvalidDataException("Unsupported or invalid image format");
        }
        
        static ImageDecoder DetectFrom(byte[] src) {
            if ( PngDecoder.DetectHeader(src)) return new PngDecoder();
            if ( GifDecoder.DetectHeader(src)) return new GifDecoder();
            if (JpegDecoder.DetectHeader(src)) return new JpegDecoder();
            
            return null;
        }
        
        public abstract SimpleBitmap Decode(byte[] src);
    }
}