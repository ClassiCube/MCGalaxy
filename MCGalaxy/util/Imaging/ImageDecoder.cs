using System;
using System.IO;
using System.IO.Compression;
using MCGalaxy.Util;

namespace MCGalaxy
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
        
        protected static bool MatchesSignature(byte[] data, byte[] sig) {
            if (data.Length < sig.Length) return false;
            
            for (int i = 0; i < sig.Length; i++)
            {
                if (data[i] != sig[i]) return false;
            }
            return true;
        }
        
        
        public static SimpleBitmap DecodeFrom(byte[] src) {
            if (PngDecoder.DetectHeader(src))
                return new PngDecoder().Decode(src);
            
            throw new InvalidDataException("Unsupported or invalid image format");
        }
    }
}