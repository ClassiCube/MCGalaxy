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
    
    public unsafe class PngDecoder
    {
        byte[] buf_data;
        int buf_offset, buf_length;
        
        int bytesPerPixel;
        RowExpander rowExpander;
        int scanline_size;
        
        /*########################################################################################################################*
         *------------------------------------------------------PNG common---------------------------------------------------------*
         *#########################################################################################################################*/
        const int IHDR_SIZE    = 13;
        const int MAX_PALETTE  = 256;
        const int PNG_SIG_SIZE =  8;
        const int MAX_PNG_DIMS = 32768;
        

        static byte[] pngSig = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        /* 5.2 PNG signature */
        static bool Png_Detect(byte[] data) {
            if (data.Length < PNG_SIG_SIZE) return false;
            
            for (int i = 0; i < PNG_SIG_SIZE; i++)
            {
                if (data[i] != pngSig[i]) return false;
            }
            return true;
        }
        
        
        const int PNG_COLOR_GRAYSCALE   = 0;
        const int PNG_COLOR_RGB         = 2;
        const int PNG_COLOR_INDEXED     = 3;
        const int PNG_COLOR_GRAYSCALE_A = 4;
        const int PNG_COLOR_RGB_A       = 6;

        static Pixel ExpandRGB(byte bitsPerSample, int r, int g, int b) {
            switch (bitsPerSample) {
                case 1:
                    r *= SCALE_1BPP; g *= SCALE_1BPP; b *= SCALE_1BPP; break;
                case 2:
                    r *= SCALE_2BPP; g *= SCALE_2BPP; b *= SCALE_2BPP; break;
                case 4:
                    r *= SCALE_4BPP; g *= SCALE_4BPP; b *= SCALE_4BPP; break;
            }
            return new Pixel((byte)r, (byte)g, (byte)b, 0);
        }

        static byte[] samplesPerPixel = new byte [] { 1, 0, 3, 1, 2, 0, 4 };
        static Pixel BLACK = new Pixel(0, 0, 0, 255);
        
        public void Decode(SimpleBitmap bmp, byte[] src) {
            byte colorspace = 0xFF;
            byte bitsPerSample = 0;

            Pixel trnsColor = BLACK;
            Pixel[] palette = null;

            MemoryStream all_idat = new MemoryStream();
            
            buf_data   = src;
            buf_offset = 0;
            buf_length = src.Length;

            if (!Png_Detect(src)) Fail("sig invalid");
            AdvanceOffset(PNG_SIG_SIZE);
            bool reachedEnd = false;
            
            while (!reachedEnd)
            {
                int offset   = AdvanceOffset(4 + 4);
                int dataSize = MemUtils.ReadI32_BE(src, offset + 0);
                int fourCC   = MemUtils.ReadI32_BE(src, offset + 4);

                switch (fourCC) {
                        // 11.2.2 IHDR Image header
                    case ('I'<<24)|('H'<<16)|('D'<<8)|'R':
                        {
                            if (dataSize != IHDR_SIZE) Fail("Header size");
                            offset = AdvanceOffset(IHDR_SIZE);

                            bmp.Width  = MemUtils.ReadI32_BE(src, offset + 0);
                            bmp.Height = MemUtils.ReadI32_BE(src, offset + 4);
                            if (bmp.Width  < 0 || bmp.Width  > MAX_PNG_DIMS) Fail("too wide");
                            if (bmp.Height < 0 || bmp.Height > MAX_PNG_DIMS) Fail("too tall");

                            bitsPerSample = src[offset + 8];
                            colorspace    = src[offset + 9];
                            if (bitsPerSample == 16) Fail("16 bpp");

                            rowExpander = Png_GetExpander(colorspace, bitsPerSample);
                            if (rowExpander == null) Fail("Colorspace/bpp combination");

                            if (src[offset + 10] != 0) Fail("Compression method");
                            if (src[offset + 11] != 0) Fail("Filter");
                            if (src[offset + 12] != 0) Fail("Interlaced unsupported");

                            bytesPerPixel  = ((samplesPerPixel[colorspace] * bitsPerSample) + 7) >> 3;
                            scanline_size  = ((samplesPerPixel[colorspace] * bitsPerSample * bmp.Width) + 7) >> 3;

                            bmp.pixels = new Pixel[bmp.Width * bmp.Height];
                        } break;

                        // 11.2.3 PLTE Palette
                    case ('P'<<24)|('L'<<16)|('T'<<8)|'E':
                        {
                            if (dataSize > MAX_PALETTE * 3) Fail("Palette size");
                            if ((dataSize % 3) != 0)        Fail("Palette align");

                            offset  = AdvanceOffset(dataSize);
                            palette = palette ?? CreatePalette();

                            for (int i = 0; i < dataSize; i += 3)
                            {
                                palette[i / 3].R = src[offset + i    ];
                                palette[i / 3].G = src[offset + i + 1];
                                palette[i / 3].B = src[offset + i + 2];
                            }
                        } break;

                        // 11.3.2.1 tRNS Transparency
                    case ('t'<<24)|('R'<<16)|('N'<<8)|'S':
                        {
                            if (colorspace == PNG_COLOR_GRAYSCALE) {
                                if (dataSize != 2) Fail("tRNS size");

                                offset = AdvanceOffset(dataSize);

                                // RGB is always two bytes
                                byte rgb  = src[offset + 1];
                                trnsColor = ExpandRGB(bitsPerSample, rgb, rgb, rgb);
                            } else if (colorspace == PNG_COLOR_INDEXED) {
                                if (dataSize > MAX_PALETTE) Fail("tRNS size");

                                offset  = AdvanceOffset(dataSize);
                                palette = palette ?? CreatePalette();

                                // Set alpha component of palette
                                for (int i = 0; i < dataSize; i++)
                                {
                                    palette[i].A = src[offset + i];
                                }
                            } else if (colorspace == PNG_COLOR_RGB) {
                                if (dataSize != 6) Fail("tRNS size");

                                offset = AdvanceOffset(dataSize);

                                // R,G,B are always two bytes
                                byte r = src[offset + 1];
                                byte g = src[offset + 3];
                                byte b = src[offset + 5];
                                trnsColor = ExpandRGB(bitsPerSample, r, g, b);
                            } else {
                                Fail("tRNS/colorspace combination");
                            }
                        } break;

                        // 11.2.4 IDAT Image data
                    case ('I'<<24)|('D'<<16)|('A'<<8)|'T':
                        {
                            if (!read_zlib_header) {
                                SkipZLibHeader(src);
                                dataSize -= 2;
                            }
                            
                            offset = AdvanceOffset(dataSize);
                            all_idat.Write(src, offset, dataSize);
                        } break;

                    case ('I'<<24)|('E'<<16)|('N'<<8)|'D':
                        reachedEnd = true;
                        break;

                    default:
                        AdvanceOffset(dataSize);
                        break;
                }

                AdvanceOffset(4); // Skip CRC32
            }
            
            all_idat.Position = 0;
            using (DeflateStream comp = new DeflateStream(all_idat, CompressionMode.Decompress))
            {
                DecompressImage(comp, bmp, palette, trnsColor);
            }
        }
        
        static Pixel[] CreatePalette() {
            Pixel[] pal = new Pixel[MAX_PALETTE];
            
            for (int i = 0; i < pal.Length; i++) pal[i] = BLACK;
            return pal;
        }
        
        void DecompressImage(Stream src, SimpleBitmap bmp, Pixel[] palette, Pixel trnsColor) {
            if (bmp.pixels == null) Fail("no data");
            
            // TODO offset by 1 so one less read call
            byte[] line  = new byte[scanline_size];
            byte[] prior = new byte[scanline_size];
            byte[] one   = new byte[1]; // stream.ReadByte() allocates one byte array each time called

            fixed (Pixel* dst = bmp.pixels)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    int read = src.Read(one, 0, 1);
                    if (read == 0) Fail("scanline");
                    
                    byte method = one[0];
                    if (method > PNG_FILTER_PAETH) Fail("Scanline");
                    StreamUtils.ReadFully(src, line, 0, scanline_size);

                    Png_Reconstruct(method, bytesPerPixel, line, prior, scanline_size);
                    rowExpander(bmp.Width, palette, line, dst + y * bmp.Width);
                    
                    // Swap current and prior line
                    byte[] tmp = line; line = prior; prior = tmp;
                }
            }

            if (trnsColor.A == 0) MakeTransparent(bmp.pixels, trnsColor);
            return;
        }

        // Sets alpha to 0 for any pixels in the bitmap whose RGB is same as color
        static void MakeTransparent(Pixel[] img, Pixel color)
        {
            for (int i = 0; i < img.Length; i++)
            {
                if (img[i].R != color.R) continue;
                if (img[i].G != color.G) continue;
                if (img[i].B != color.B) continue;
                
                img[i].A = 0;
            }
        }
        
        
        int AdvanceOffset(int amount) {
            int offset = buf_offset;
            
            buf_offset += amount;
            if (buf_offset > buf_length)
                throw new EndOfStreamException("End of stream reading data");
            return offset;
        }
        
        static void Fail(string reason) {
            throw new InvalidDataException(reason);
        }
        
        bool read_zlib_header;
        void SkipZLibHeader(byte[] src) {
            int offset = AdvanceOffset(2);
            
            byte method = src[offset + 0];
            if ((method & 0x0F) != 0x08) Fail("Zlib method");
            // Upper 4 bits are window size
            
            byte flags = src[offset + 1];
            if ((flags & 0x20) != 0) Fail("Zlip flags");
            
            read_zlib_header = true;
        }
        
        
        #region Row filtering        
        const int PNG_FILTER_NONE    = 0;
        const int PNG_FILTER_SUB     = 1;
        const int PNG_FILTER_UP      = 2;
        const int PNG_FILTER_AVERAGE = 3;
        const int PNG_FILTER_PAETH   = 4;
        
        static void Png_Reconstruct(byte type, int bytesPerPixel, byte[] line, byte[] prior, int lineLen) {
            int i, j;
            
            switch (type) {
                case PNG_FILTER_SUB:
                    for (i = bytesPerPixel, j = 0; i < lineLen; i++, j++)
                    {
                        line[i] += line[j];
                    }
                    return;

                case PNG_FILTER_UP:
                    for (i = 0; i < lineLen; i++)
                    {
                        line[i] += prior[i];
                    }
                    return;

                case PNG_FILTER_AVERAGE:
                    for (i = 0; i < bytesPerPixel; i++)
                    {
                        line[i] += (byte)(prior[i] >> 1);
                    }
                    for (j = 0; i < lineLen; i++, j++)
                    {
                        line[i] += (byte)((prior[i] + line[j]) >> 1);
                    }
                    return;

                case PNG_FILTER_PAETH:
                    /* TODO: verify this is right */
                    for (i = 0; i < bytesPerPixel; i++)
                    {
                        line[i] += prior[i];
                    }
                    for (j = 0; i < lineLen; i++, j++)
                    {
                        byte a = line[j], b = prior[i], c = prior[j];
                        int p  = a + b - c;
                        
                        int pa = Math.Abs(p - a);
                        int pb = Math.Abs(p - b);
                        int pc = Math.Abs(p - c);

                        if (pa <= pb && pa <= pc) { line[i] += a; }
                        else if (pb <= pc) {        line[i] += b; }
                        else {                      line[i] += c; }
                    }
                    return;
            }
        }
        #endregion
        

        #region Row expansion
        delegate void RowExpander(int width, Pixel[] palette, byte[] src, Pixel* dst);

        static int Get_1BPP(byte[] src, int i) {
            int j = 7 - (i & 7);
            return (src[i >> 3] >> j) & 0x01;
        }
        
        static int Get_2BPP(byte[] src, int i) {
            int j = (3 - (i & 3)) * 2;
            return (src[i >> 2] >> j) & 0x03;
        }
        
        static int Get_4BPP(byte[] src, int i) {
            int j = (1 - (i & 1)) * 4;
            return (src[i >> 1] >> j) & 0x0F;
        }
        
        const byte SCALE_1BPP = 255;
        const byte SCALE_2BPP =  85;
        const byte SCALE_4BPP =  17;

        static void Png_Expand_GRAYSCALE_1(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte rgb = (byte)(Get_1BPP(src, i) * SCALE_1BPP);
                dst[i]   = new Pixel(rgb, rgb, rgb, 255);
            }
        }

        static void Png_Expand_GRAYSCALE_2(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte rgb = (byte)(Get_2BPP(src, i) * SCALE_2BPP);
                dst[i]   = new Pixel(rgb, rgb, rgb, 255);
            }
        }

        static void Png_Expand_GRAYSCALE_4(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte rgb = (byte)(Get_4BPP(src, i) * SCALE_4BPP);
                dst[i]   = new Pixel(rgb, rgb, rgb, 255);
            }
        }

        static void Png_Expand_GRAYSCALE_8(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte rgb = src[i];
                dst[i]   = new Pixel(rgb, rgb, rgb, 255);
            }
        }

        static void Png_Expand_RGB_8(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte r = src[i * 3 + 0];
                byte g = src[i * 3 + 1];
                byte b = src[i * 3 + 2];
                dst[i] = new Pixel(r, g, b, 255);
            }
        }

        static void Png_Expand_INDEXED_1(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++) { dst[i] = palette[Get_1BPP(src, i)]; }
        }

        static void Png_Expand_INDEXED_2(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++) { dst[i] = palette[Get_2BPP(src, i)]; }
        }

        static void Png_Expand_INDEXED_4(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++) { dst[i] = palette[Get_4BPP(src, i)]; }
        }

        static void Png_Expand_INDEXED_8(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++) { dst[i] = palette[src[i]]; }
        }

        static void Png_Expand_GRAYSCALE_A_8(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte rgb = src[i * 2 + 0];
                byte a   = src[i * 2 + 1];
                dst[i] = new Pixel(rgb, rgb, rgb, a);
            }
        }

        static void Png_Expand_RGB_A_8(int width, Pixel[] palette, byte[] src, Pixel* dst) {
            for (int i = 0; i < width; i++)
            {
                byte r = src[i * 4 + 0];
                byte g = src[i * 4 + 1];
                byte b = src[i * 4 + 2];
                byte a = src[i * 4 + 3];
                dst[i] = new Pixel(r, g, b, a);
            }
        }
        

        static RowExpander Png_GetExpander(byte col, byte bitsPerSample) {
            switch (col) {
                case PNG_COLOR_GRAYSCALE:
                    switch (bitsPerSample) {
                            case 1:  return Png_Expand_GRAYSCALE_1;
                            case 2:  return Png_Expand_GRAYSCALE_2;
                            case 4:  return Png_Expand_GRAYSCALE_4;
                            case 8:  return Png_Expand_GRAYSCALE_8;
                    }
                    return null;

                case PNG_COLOR_RGB:
                    switch (bitsPerSample) {
                            case 8:  return Png_Expand_RGB_8;
                    }
                    return null;

                case PNG_COLOR_INDEXED:
                    switch (bitsPerSample) {
                            case 1: return Png_Expand_INDEXED_1;
                            case 2: return Png_Expand_INDEXED_2;
                            case 4: return Png_Expand_INDEXED_4;
                            case 8: return Png_Expand_INDEXED_8;
                    }
                    return null;

                case PNG_COLOR_GRAYSCALE_A:
                    switch (bitsPerSample) {
                            case 8:  return Png_Expand_GRAYSCALE_A_8;
                    }
                    return null;

                case PNG_COLOR_RGB_A:
                    switch (bitsPerSample) {
                            case 8:  return Png_Expand_RGB_A_8;
                    }
                    return null;
            }
            return null;
        }
        #endregion
    }
}