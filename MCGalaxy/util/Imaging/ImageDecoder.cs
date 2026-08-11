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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Util;

namespace MCGalaxy.Util.Imaging
{
    public abstract class ImageDecoder
    {
        protected byte[] buf_data;
        protected int buf_offset, buf_length;
        
        /// <summary> Attempts to advance next read offset by 'amount', then returns current read offset </summary>
        protected int AdvanceOffset(int amount) {
            int offset = buf_offset;
            
            buf_offset += amount;
            if (buf_offset > buf_length)
                throw new EndOfStreamException("End of stream reading data");
            return offset;
        }
        
        protected void SetBuffer(byte[] src) {
            buf_data   = src;
            buf_offset = 0;
            buf_length = src.Length;
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
        
        
        public abstract Bitmap2D Decode(byte[] src);
    }
    
    public sealed class ImageFormat 
    {
        public readonly string Extension;
        public Predicate<byte[]> DetectHeader;
        public Func<ImageDecoder> CreateDecoder;
        
        public ImageFormat(string ext, Predicate<byte[]> detector, Func<ImageDecoder> decoder) {
            Extension       = ext;
            DetectHeader = detector;
            CreateDecoder   = decoder;
        }
        
        
        // TODO .jpeg etc
        public static List<ImageFormat> KnownFormats = new List<ImageFormat>() {
            new ImageFormat(".png",  PngDecoder.DetectHeader, () => new  PngDecoder()),
            new ImageFormat(".jpg", JpegDecoder.DetectHeader, () => new JpegDecoder()),
            new ImageFormat(".gif",  GifDecoder.DetectHeader, () => new  GifDecoder()),
        };
                
        public static ImageDecoder DetectFrom(byte[] src) {
            foreach (var format in ImageFormat.KnownFormats)
            {
                if (!format.DetectHeader(src)) continue;
                
                return format.CreateDecoder();
            }
            return null;
        }
    }
}