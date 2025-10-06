using System;
using System.IO;
using System.IO.Compression;
using MCGalaxy.Util;

namespace MCGalaxy
{
    unsafe sealed class SimpleBitmap //: IBitmap2D
    {
        public int Width, Height;
        public Pixel[] pixels;
    }
    
    public unsafe class PngDecoder
    {
        /*########################################################################################################################*
         *------------------------------------------------------PNG common---------------------------------------------------------*
         *#########################################################################################################################*/
        const int PNG_IHDR_SIZE = 13;
        const int PNG_PALETTE   = 256;
        const int PNG_SIG_SIZE  =  8;
        const int PNG_MAX_DIMS  = 32768;
        
        static int PNG_FourCC(char a, char b, char c, char d) {
            return ((int)a << 24) | ((int)b << 16) | ((int)c << 8) | (int)d;
        }
        
        static void Fail(string reason) {
            throw new InvalidDataException(reason);
        }
        

        static byte[] pngSig = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };

        /* 5.2 PNG signature */
        static bool Png_Detect(byte[] data, int len) {
            return len >= PNG_SIG_SIZE && Mem_Equal(data, pngSig, PNG_SIG_SIZE);
        }
        
        static bool Mem_Equal(byte[] a, byte[] b, int numBytes) {
            for (int i = 0; i < numBytes; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }
        
        
        const int PNG_COLOR_GRAYSCALE   = 0;
        const int PNG_COLOR_RGB         = 2;
        const int PNG_COLOR_INDEXED     = 3;
        const int PNG_COLOR_GRAYSCALE_A = 4;
        const int PNG_COLOR_RGB_A       = 6;
        
        const int PNG_FILTER_NONE    = 0;
        const int PNG_FILTER_SUB     = 1;
        const int PNG_FILTER_UP      = 2;
        const int PNG_FILTER_AVERAGE = 3;
        const int PNG_FILTER_PAETH   = 4;
        
        
        delegate void Png_RowExpander(int width, Pixel[] palette, byte[] src, Pixel* dst);

        // 9 Filtering
        // 13.9 Filtering
        static void Png_Reconstruct(byte type, byte bytesPerPixel, byte[] line, byte[] prior, int lineLen) {
            switch (type) {
                case PNG_FILTER_SUB:
                    for (int i = bytesPerPixel, j = 0; i < lineLen; i++, j++)
                    {
                        line[i] += line[j];
                    }
                    return;

                case PNG_FILTER_UP:
                    for (int i = 0; i < lineLen; i++)
                    {
                        line[i] += prior[i];
                    }
                    return;

                case PNG_FILTER_AVERAGE:
                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        line[i] += (byte)(prior[i] >> 1);
                    }
                    for (int j = 0, i = bytesPerPixel; i < lineLen; i++, j++)
                    {
                        line[i] += (byte)((prior[i] + line[j]) >> 1);
                    }
                    return;

                case PNG_FILTER_PAETH:
                    /* TODO: verify this is right */
                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        line[i] += prior[i];
                    }
                    for (int j = 0, i = bytesPerPixel; i < lineLen; i++, j++)
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

        /* 7.2 Scanlines */
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

        static Png_RowExpander Png_GetExpander(byte col, byte bitsPerSample) {
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

        /* Sets alpha to 0 for any pixels in the bitmap whose RGB is same as colorspace */
        static void ComputeTransparency(SimpleBitmap bmp, Pixel col)
        {
            int width = bmp.Width, height = bmp.Height;

            for (int y = 0; y < height; y++)
            {
                Pixel* row = Bitmap_GetRow(bmp, y);
                for (int x = 0; x < width; x++)
                {
                    if (row[x].R != col.R) continue;
                    if (row[x].G != col.G) continue;
                    if (row[x].B != col.B) continue;
                    
                    row[x].A = 0;
                }
            }
        }

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
        
        void Png_Decode(SimpleBitmap bmp, Stream stream) {
            byte[] tmp = new byte[64];
            int dataSize, fourCC;

            /* header variables */
            byte colorspace = 0xFF;
            byte bitsPerSample;
            byte bytesPerPixel = 0;
            Png_RowExpander rowExpander = null;
            int scanlineSize = 0;
            int scanlineBytes = 0;

            /* palette data */
            Pixel trnsColor;
            Pixel[] palette = new Pixel[PNG_PALETTE];
            int i;

            /* idat state */
            int available = 0, rowY = 0;
            byte[] buffer = new byte[PNG_PALETTE * 3];
            int read, bufferIdx = 0;
            int left, bufferLen = 0;
            int curY;

            /* idat decompressor */
            Stream compStream, datStream;
            byte* data = null;

            StreamUtils.ReadFully(stream, tmp, 0, PNG_SIG_SIZE);
            if (!Png_Detect(tmp, PNG_SIG_SIZE)) Fail("sig invalid");

            trnsColor  = BLACK;
            for (i = 0; i < PNG_PALETTE; i++) { palette[i] = BLACK; }
            for (;;) {
                StreamUtils.ReadFully(stream, tmp, 0, 8);
                dataSize = MemUtils.ReadI32_BE(tmp, 0);
                fourCC   = MemUtils.ReadI32_BE(tmp, 4);

                switch (fourCC) {
                        /* 11.2.2 IHDR Image header */
                        case ('I'<<24)|('H'<<16)|('D'<<8)|'R': {
                            if (dataSize != PNG_IHDR_SIZE) Fail("Header size");
                            StreamUtils.ReadFully(stream, tmp, 0, PNG_IHDR_SIZE);

                            bmp.Width  = MemUtils.ReadI32_BE(tmp, 0);
                            bmp.Height = MemUtils.ReadI32_BE(tmp, 4);
                            if (bmp.Width  < 0 || bmp.Width  > PNG_MAX_DIMS) Fail("too wide");
                            if (bmp.Height < 0 || bmp.Height > PNG_MAX_DIMS) Fail("too tall");

                            bitsPerSample = tmp[8];
                            colorspace = tmp[9];
                            if (bitsPerSample == 16) Fail("16 bpp");

                            rowExpander = Png_GetExpander(colorspace, bitsPerSample);
                            if (rowExpander == null) Fail("Colorspace/bpp combination");

                            if (tmp[10] != 0) Fail("Compression method");
                            if (tmp[11] != 0) Fail("Filter");
                            if (tmp[12] != 0) Fail("Interlaced unsupported");

                            bytesPerPixel = ((samplesPerPixel[colorspace] * bitsPerSample) + 7) >> 3;
                            scanlineSize  = ((samplesPerPixel[colorspace] * bitsPerSample * bmp.Width) + 7) >> 3;
                            scanlineBytes = scanlineSize + 1; /* Add 1 byte for filter byte of each scanline */

                            bmp.pixels = new Pixel[bmp.Width * bmp.Height];

                            bufferLen = bmp.Height * scanlineBytes;
                        } break;

                        /* 11.2.3 PLTE Palette */
                        case ('P'<<24)|('L'<<16)|('T'<<8)|'E': {
                            if (dataSize > PNG_PALETTE * 3) Fail("Palette size");
                            if ((dataSize % 3) != 0)        Fail("Palette align");

                            StreamUtils.ReadFully(stream, buffer, 0, dataSize);

                            for (i = 0; i < dataSize; i += 3)
                            {
                                palette[i / 3].R = buffer[i    ];
                                palette[i / 3].G = buffer[i + 1];
                                palette[i / 3].B = buffer[i + 2];
                            }
                        } break;

                        /* 11.3.2.1 tRNS Transparency */
                        case ('t'<<24)|('R'<<16)|('N'<<8)|'S': {
                            if (colorspace == PNG_COLOR_GRAYSCALE) {
                                if (dataSize != 2) Fail("tRNS size");

                                StreamUtils.ReadFully(stream, buffer, 0, dataSize);

                                /* RGB is always two bytes */
                                trnsColor = ExpandRGB(bitsPerSample, buffer[1], buffer[1], buffer[1]);
                            } else if (colorspace == PNG_COLOR_INDEXED) {
                                if (dataSize > PNG_PALETTE) Fail("tRNS size");

                                StreamUtils.ReadFully(stream, buffer, 0, dataSize);

                                /* set alpha component of palette */
                                for (i = 0; i < dataSize; i++) {
                                    palette[i].A = buffer[i];
                                }
                            } else if (colorspace == PNG_COLOR_RGB) {
                                if (dataSize != 6) Fail("tRNS size");

                                StreamUtils.ReadFully(stream, buffer, 0, dataSize);

                                /* R,G,B are always two bytes */
                                trnsColor = ExpandRGB(bitsPerSample, buffer[1], buffer[3], buffer[5]);
                            } else {
                                Fail("tRNS/colorspace combination");
                            }
                        } break;

                        /* 11.2.4 IDAT Image data */
                        case ('I'<<24)|('D'<<16)|('A'<<8)|'T': {
                            Stream_ReadonlyPortion(&datStream, stream, dataSize);
                            inflate->Source = &datStream;

                            /* TODO: This assumes zlib header will be in 1 IDAT chunk */
                            while (zlib_hdr_state != ZLIB_STATE_DONE) SkipZLibHeader(&datStream);

                            if (bmp.pixels == null) Fail("no data");
                            if (rowY >= bmp.Height) break;
                            left = bufferLen - bufferIdx;

                            res  = compStream.Read(&compStream, &data[bufferIdx], left, &read);
                            if (res) return res;
                            if (read == 0) break;

                            available += read;
                            bufferIdx += read;

                            /* Process all of the scanline(s) that have been fully decompressed */
                            /* NOTE: Need to check height too, in case IDAT is corrupted and has extra data */
                            for (; available >= scanlineBytes && rowY < bmp->height; rowY++, available -= scanlineBytes) {
                                byte* scanline = &data[rowY * scanlineBytes];
                                if (scanline[0] > PNG_FILTER_PAETH) Fail("Scanline");

                                if (rowY == 0) {
                                    /* First row, prior is assumed as 0 */
                                    Png_ReconstructFirst(scanline[0], bytesPerPixel, &scanline[1], scanlineSize);
                                } else {
                                    byte* prior = &data[(rowY - 1) * scanlineBytes];
                                    Png_Reconstruct(scanline[0], bytesPerPixel, &scanline[1], &prior[1], scanlineSize);
                                }

                                rowExpander(bmp->width, palette, &scanline[1], Bitmap_GetRow(bmp, rowY));
                            }

                            /* Check if image fully decoded or not */
                            if (bufferIdx != bufferLen) break;

                            if (trnsColor.A == 0) ComputeTransparency(bmp, trnsColor);
                            return;
                        } break;

                        /* 11.2.5 IEND Image trailer */
                    case ('I'<<24)|('E'<<16)|('N'<<8)|'D':
                        /* Reading all image data should be handled by above if in the IDAT chunk */
                        /* If we reached here, it means not all of the image data was read */
                        Fail("IEND");
                        break;

                    default:
                        SkipData(stream, dataSize);
                        break;
                }

                SkipData(stream, 4); /* Skip CRC32 */
            }
        }
        
        
        static byte[] skip_buf = new byte[64];
        static void SkipData(Stream s, int count) {
            while (count > 0)
            {
                int read = s.Read(skip_buf, 0, count);
                
                if (read == 0) throw new EndOfStreamException("End of stream reading data");
                count -= read;
            }
        }
        
        int zlib_hdr_state = ZLIB_STATE_COMPRESSIONMETHOD;
        
        const int ZLIB_STATE_COMPRESSIONMETHOD = 0;
        const int ZLIB_STATE_FLAGS             = 1;
        const int ZLIB_STATE_DONE              = 2;

        void SkipZLibHeader(Stream s) {
            int tmp;
            
            switch (zlib_hdr_state) 
            {
                case ZLIB_STATE_COMPRESSIONMETHOD:
                    tmp = s.ReadByte();
                    if ((tmp & 0x0F) != 0x08) Fail("Zlib method");
                    
                    /* Upper 4 bits are window size (ignored) */
                    zlib_hdr_state++;
                    break;
                    
                case ZLIB_STATE_FLAGS:
                    tmp = s.ReadByte();
                    if ((tmp & 0x20) != 0) Fail("Zlip flags");
                    
                    zlib_hdr_state++;
                    break;
            }
        }
    }
}