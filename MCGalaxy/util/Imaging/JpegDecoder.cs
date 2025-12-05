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
    public unsafe class JpegDecoder : ImageDecoder
    {
        static byte[] jfifSig = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // "SOI", "APP0"
        static byte[] exifSig = new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 }; // "SOI", "APP1"
        
        byte[][] quant_tables = new byte[4][];
        HuffmanTable[] ac_huff_tables = new HuffmanTable[4];
        HuffmanTable[] dc_huff_tables = new HuffmanTable[4];
        JpegComponent[] comps;
        byte lowestHor = 1;
        byte lowestVer = 1;

        public static bool DetectHeader(byte[] data) {
            return MatchesSignature(data, jfifSig)
                || MatchesSignature(data, exifSig);
        }
        
        public override SimpleBitmap Decode(byte[] src) {
            SetBuffer(src);
            SimpleBitmap bmp = new SimpleBitmap();
            
            ReadMarkers(src, bmp);
            return bmp;
        }
        
        
        const ushort MARKER_FRAME_BEG = 0xFFC0;
        const ushort MARKER_TBL_HUFF  = 0xFFC4;
        const ushort MARKER_RST_MRKR0 = 0xFFD0;
        const ushort MARKER_RST_MRKR7 = 0xFFD7;
        const ushort MARKER_IMAGE_BEG = 0xFFD8;
        const ushort MARKER_IMAGE_END = 0xFFD9;
        const ushort MARKER_SCAN_BEG  = 0xFFDA;
        const ushort MARKER_TBL_QUANT = 0xFFDB;
        const ushort MARKER_RST_NTRVL = 0xFFDD;
        const ushort MARKER_APP0      = 0xFFE0;
        const ushort MARKER_APP15     = 0xFFEF;
        const ushort MARKER_COMMENT   = 0xFFFE;
        
        void ReadMarkers(byte[] src, SimpleBitmap bmp) {
            for (;;)
            {
                int offset    = AdvanceOffset(2);
                ushort marker = MemUtils.ReadU16_BE(src, offset);
                
                if (marker == MARKER_IMAGE_BEG) {
                    // Nothing to do
                } else if (marker == MARKER_IMAGE_END) {
                    return;
                } else if (marker >= MARKER_APP0 && marker <= MARKER_APP15) {
                    SkipMarker(src);
                } else if (marker == MARKER_COMMENT || marker == MARKER_RST_NTRVL) {
                    SkipMarker(src);
                } else if (marker == MARKER_TBL_HUFF) {
                    ReadHuffmanTable(src);
                } else if (marker == MARKER_TBL_QUANT) {
                    ReadQuantisationTables(src);
                } else if (marker == MARKER_FRAME_BEG) {
                    ReadFrameStart(src, bmp);
                } else if (marker == MARKER_SCAN_BEG) {
                    ReadScanStart(src);
                    DecodeMCUs(src, bmp);
                } else {
                    Fail("unknown marker:" + marker.ToString("X4"));
                }
            }
        }
        
        void SkipMarker(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            // length *includes* 2 bytes of length
            AdvanceOffset(length - 2);
        }
        
        const int QUANT_DATA_LEN = 65; // 1 byte flags, 64 byte values
        void ReadQuantisationTables(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);

            length -= 2; // length *includes* 2 bytes of length
            offset = AdvanceOffset(length);
            
            // Can have more than one quantisation table
            while (length > 0)
            {
                if (length < QUANT_DATA_LEN) Fail("quant table too short: " + length);
                length -= QUANT_DATA_LEN;
                
                byte flags = src[offset++];
                // As per Table B.4, 16 bit quantisation tables not in baseline JPEG
                if ((flags & 0xF0) != 0) Fail("16 bit quant table unsupported");
                
                int idx = flags & 0x03;
                if (quant_tables[idx] == null)
                    quant_tables[idx] = new byte[64];
                
                byte[] table = quant_tables[idx];
                for (int i = 0; i < table.Length; i++)
                    table[i] = src[offset++];
            }
        }
        
        const int HUFF_MAX_BITS  = 16;
        const int HUFF_MAX_VALS  = 256;
        const int HUFF_FAST_BITS = 8;
        const int HUFF_FAST_LEN_SHIFT = 8;
        
        void ReadHuffmanTable(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            
            length -= 2; // length *includes* 2 bytes of length
            offset = AdvanceOffset(length);
            
            // Can have more than one huffman table
            while (length > 0)
            {
                byte flags = src[offset++];
                
                HuffmanTable table    = new HuffmanTable();
                HuffmanTable[] tables = (flags >> 4) != 0 ? ac_huff_tables : dc_huff_tables;
                tables[flags & 0x03]  = table;
                
                int read = DecodeHuffmanTable(src, table, ref offset);
                length -= 1 + read; // 1 byte for flags
            }
        }
        
        int DecodeHuffmanTable(byte[] src, HuffmanTable table, ref int offset) {
            table.firstCodewords = new ushort[HUFF_MAX_BITS];
            table.endCodewords   = new ushort[HUFF_MAX_BITS];
            table.firstOffsets   = new ushort[HUFF_MAX_BITS];
            table.values         = new byte[HUFF_MAX_VALS];
            table.fast           = new ushort[1 << HUFF_FAST_BITS];
            
            // Compute the codewords for the huffman tree.
            //  Codewords are ordered, so consider this example tree:
            //    2 of length 2, 3 of length 3, 1 of length 4
            //  Codewords produced would be: 00,01 100,101,110, 1110
            int code  = 0;
            int total = 0;
            byte[] counts = new byte[HUFF_MAX_BITS];
            
            for (int i = 0; i < HUFF_MAX_BITS; i++)
            {
                byte count = src[offset++];
                if (count > (1 << (i+1))) Fail("too many codewords for bit length");
                counts[i]  = count;

                table.firstCodewords[i] = (ushort)code;
                table.firstOffsets[i]   = (ushort)total;
                total += count;

                // Last codeword is actually: code + (count - 1)
                //  However, when decoding we peform < against this value though, so need to add 1 here.
                //  This way, don't need to special case bit lengths with 0 codewords when decoding.
                if (count != 0) {
                    table.endCodewords[i] = (ushort)(code + count);
                }
                code = (code + count) << 1;
            }
            if (total > HUFF_MAX_VALS) Fail("too many values");
            int valueIdx = 0;

            // Read values for each codeword.
            //  Note that although codewords are ordered, values may not be.
            //  Some values may also not be assigned to any codeword.
            for (int i = 0; i < counts.Length; i++)
            {
                for (int j = 0; j < counts[i]; j++)
                {
                    byte value = src[offset++];
                    table.values[valueIdx++] = value;
                    
                    // Codeword too long to fast pack?
                    int len = i + 1;
                    if (len > HUFF_FAST_BITS) continue;
                    
                    ushort packed = (ushort)((len << HUFF_FAST_LEN_SHIFT) | value);
                    int codeword  = table.firstCodewords[i] + j;
                    codeword <<= (HUFF_FAST_BITS - len);

                    for (int k = 0; k < 1 << (HUFF_FAST_BITS - len); k++)
                    {
                        table.fast[codeword + k] = packed;
                    }
                }
            }
            
            return HUFF_MAX_BITS + total;
        }
        
        void ReadFrameStart(byte[] src, SimpleBitmap bmp) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            // length *includes* 2 bytes of length
            offset = AdvanceOffset(length - 2);
            
            byte bits  = src[offset + 0];
            if (bits != 8) Fail("bits per sample");
            
            bmp.Height = MemUtils.ReadU16_BE(src, offset + 1);
            bmp.Width  = MemUtils.ReadU16_BE(src, offset + 3);
            bmp.AllocatePixels();
            
            byte numComps = src[offset + 5];
            if (!(numComps == 1 || numComps == 3)) Fail("num components");
            offset += 6;
            
            comps = new JpegComponent[numComps];
            
            for (int i = 0; i < numComps; i++)
            {
                JpegComponent comp = new JpegComponent();
                comp.ID          = src[offset++];
                byte sampling    = src[offset++];
                comp.SamplingHor = (byte)(sampling >> 4);
                comp.SamplingVer = (byte)(sampling & 0x0F);
                comp.QuantTable  = src[offset++];
                
                lowestHor = Math.Max(lowestHor, comp.SamplingHor);
                lowestVer = Math.Max(lowestVer, comp.SamplingVer);
                comps[i]  = comp;
            }
            
            // In most JPEG images there is chroma sub-sampling
            for (int i = 0; i < numComps; i++)
            {
                JpegComponent comp = comps[i];
                comp.BlocksPerMcuX = comp.SamplingHor;
                comp.BlocksPerMcuY = comp.SamplingVer;
                
                comp.SamplesPerBlockX = (byte)(lowestHor / comp.BlocksPerMcuX);
                comp.SamplesPerBlockY = (byte)(lowestVer / comp.BlocksPerMcuY);
            }
        }
        
        void ReadScanStart(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            // length *includes* 2 bytes of length
            offset = AdvanceOffset(length - 2);
            
            byte numComps = src[offset++];
            for (int i = 0; i < numComps; i++)
            {
                byte compID = src[offset++];
                byte tables = src[offset++];
                SetHuffTables(compID, tables);
            }
            
            // Spectral lo/hi data and successive approximation irrelevant
            offset += 3;
        }
        
        void SetHuffTables(byte compID, byte tables) {
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i].ID != compID) continue;
                
                comps[i].DCHuffTable = (byte)(tables >> 4);
                comps[i].ACHuffTable = (byte)(tables & 0x0F);
                comps[i].PredDCValue = 0;
                return;
            }
            Fail("unknown scan component");
        }
        
        static byte[] zigzag_to_linear = new byte[64]
        {
            0,  1,  8, 16,  9,  2,  3, 10,
            17, 24, 32, 25, 18, 11,  4,  5,
            12, 19, 26, 33, 40, 48, 41, 34,
            27, 20, 13,  6,  7, 14, 21, 28,
            35, 42, 49, 56, 57, 50, 43, 36,
            29, 22, 15, 23, 30, 37, 44, 51,
            58, 59, 52, 45, 38, 31, 39, 46,
            53, 60, 61, 54, 47, 55, 62, 63,
        };
        
        const int BLOCK_SAMPLES = 8;
        const int BLOCK_SIZE    = 8 * 8;
        
        struct YCbCr { public float Y, Cb, Cr; };
        
        void DecodeMCUs(byte[] src, SimpleBitmap bmp) {
            int mcu_w  = lowestHor * BLOCK_SAMPLES;
            int mcu_h  = lowestVer * BLOCK_SAMPLES;
            int mcus_x = Utils.CeilDiv(bmp.Width,  mcu_w);
            int mcus_y = Utils.CeilDiv(bmp.Height, mcu_h);
            
            JpegComponent[] comps = this.comps;
            int* block    = stackalloc int[BLOCK_SIZE + 32];
            float* output = stackalloc float[BLOCK_SIZE];
            // NOTE: block has extra data at end, in case of invalid huffman code
            //  resulting in 'num zeroes' going past limit of 64
            // Although there can be at most 15 extra zeroes, add 32 padding to be safe
            
            YCbCr[] colors = new YCbCr[mcu_w * mcu_h];
            
            for (int mcuY = 0; mcuY < mcus_y; mcuY++)
                for (int mcuX = 0; mcuX < mcus_x; mcuX++)
            {
                // Technically, reset0-7 markers should only occur every # MCUs
                //  as per the 'restart interval' marker. However, since proper
                //  spec compliant decoding is unimportant, just hackily decode
                if (hit_rst) {
                    hit_rst = false;
                    // Align bit reader to next byte, then reset DC prediction value
                    ConsumeBits(bit_cnt & 0x07);
                    for (int i = 0; i < comps.Length; i++) comps[i].PredDCValue = 0;
                }
                
                for (int i = 0; i < comps.Length; i++)
                {
                    JpegComponent comp = comps[i];
                    
                    for (int by = 0; by < comp.BlocksPerMcuY; by++)
                        for (int bx = 0; bx < comp.BlocksPerMcuX; bx++)
                    {
                        DecodeBlock(comp, src, block);
                        IDCT(block, output);
                        
                        int samplesX = comp.SamplesPerBlockX;
                        int samplesY = comp.SamplesPerBlockY;
                        
                        for (int y = 0; y < BLOCK_SAMPLES; y++)
                            for (int x = 0; x < BLOCK_SAMPLES; x++)
                        {
                            float sample = output[y * BLOCK_SAMPLES + x];
                            
                            for (int py = 0; py < samplesY; py++)
                                for (int px = 0; px < samplesX; px++)
                            {
                                int YY = (by * BLOCK_SAMPLES + y) * samplesY + py;
                                int XX = (bx * BLOCK_SAMPLES + x) * samplesX + px;
                                int idx = YY * mcu_w + XX;
                                
                                if (i == 0)      colors[idx].Y  = sample;
                                else if (i == 1) colors[idx].Cb = sample;
                                else if (i == 2) colors[idx].Cr = sample;
                            }
                        }
                    }
                }
                
                int baseX = mcuX * mcu_w;
                int baseY = mcuY * mcu_h;
                int j = 0;
                
                for (int YY = 0; YY < mcu_w; YY++)
                    for (int XX = 0; XX < mcu_h; XX++, j++)
                {
                    int globalX = baseX + XX;
                    int globalY = baseY + YY;
                    
                    if (globalX < bmp.Width && globalY < bmp.Height) {
                        // Following standard code assumes JPEG DCT output is level shifted
                        //   float r =  1.40200f * (cr - 128) + y;
                        //   float g = -0.34414f * (cb - 128) - 0.71414f * (cr - 128) + y;
                        //   float b =  1.77200f * (cb - 128) + y;
                        // Use a slightly different algorithm to avoid level shifting
                        
                        float y  = colors[j].Y + 128.0f;
                        float cr = colors[j].Cr;
                        float cb = colors[j].Cb;
                        
                        float r =  1.40200f * cr + y;
                        float g = -0.34414f * cb - 0.71414f * cr + y;
                        float b =  1.77200f * cb + y;
                        
                        Pixel p = new Pixel(ByteClamp(r), ByteClamp(g), ByteClamp(b), 255);
                        bmp.pixels[globalY * bmp.Width + globalX] = p;
                    }
                }
            }
        }
        
        static byte ByteClamp(float v) {
            if (v < 0) return 0;
            if (v > 255) return 255;
            return (byte)v;
        }
        
        void DecodeBlock(JpegComponent comp, byte[] src, int* block) {
            // DC value is relative to DC value from prior block
            var table    = dc_huff_tables[comp.DCHuffTable];
            int dc_code  = ReadHuffman(table, src);
            int dc_delta = ReadBiasedValue(src, dc_code);
            
            int dc_value = comp.PredDCValue + dc_delta;
            comp.PredDCValue = dc_value;
            
            byte[] dequant = quant_tables[comp.QuantTable];
            for (int j = 0; j < BLOCK_SIZE; j++) block[j] = 0;
            block[0] = dc_value * dequant[0];
            
            // 63 AC values
            table = ac_huff_tables[comp.ACHuffTable];
            int idx = 1;
            
            do {
                int code = ReadHuffman(table, src);
                if (code == 0) break;
                
                int bits = code & 0x0F;
                int num_zeros = code >> 4;
                
                if (bits == 0) {
                    if (code == 0) break; // 0 value - end of block
                    // TODO is this right?
                    if (num_zeros != 15) Fail("too many zeroes");
                    idx += 16;
                } else {
                    idx += num_zeros;
                    int lin = zigzag_to_linear[idx];
                    block[lin] = ReadBiasedValue(src, bits) * dequant[idx];
                    idx++;
                }
            } while (idx < 64);
        }

        const float A1 = 0.98078528040f; // cos(1pi/16)
        const float A2 = 0.92387953251f; // cos(2pi/16)
        const float A3 = 0.83146961230f; // cos(3pi/16)
        const float A4 = 0.70710678118f; // cos(4pi/16)
        const float A5 = 0.55557023302f; // cos(5pi/16)
        const float A6 = 0.38268343236f; // cos(6pi/16)
        const float A7 = 0.19509032201f; // cos(7pi/16)
        
        const float C1 = 0.98078528040f / 4.0f; // cos(1pi/16)
        const float C2 = 0.92387953251f / 4.0f; // cos(2pi/16)
        const float C3 = 0.83146961230f / 4.0f; // cos(3pi/16)
        const float C4 = 0.70710678118f / 4.0f; // cos(4pi/16)
        const float C5 = 0.55557023302f / 4.0f; // cos(5pi/16)
        const float C6 = 0.38268343236f / 4.0f; // cos(6pi/16)
        const float C7 = 0.19509032201f / 4.0f; // cos(7pi/16)
        
        const int DCT_SIZE = 8;
        static void IDCT(int* block, float* output) {
            float* tmp = stackalloc float[DCT_SIZE * DCT_SIZE];
            
            for (int col = 0; col < DCT_SIZE; col++)
            {
                float B0 = block[0 * DCT_SIZE + col], B1 = block[1 * DCT_SIZE + col];
                float B2 = block[2 * DCT_SIZE + col], B3 = block[3 * DCT_SIZE + col];
                float B4 = block[4 * DCT_SIZE + col], B5 = block[5 * DCT_SIZE + col];
                float B6 = block[6 * DCT_SIZE + col], B7 = block[7 * DCT_SIZE + col];
                
                /* Phase 1 */ float a4 = A4 * B0;
                /* Phase 5 */ float e4 = A4 * B4;
                /* Phase 3 */ float c2 = A2 * B2, c6 = A6 * B2;
                /* Phase 7 */ float g2 = A2 * B6, g6 = A6 * B6;
                /* Phase 2 */ float b1 = A1 * B1, b3 = A3 * B1, b5 = A5 * B1, b7 = A7 * B1;
                /* Phase 4 */ float d1 = A1 * B3, d3 = A3 * B3, d5 = A5 * B3, d7 = A7 * B3;
                /* Phase 6 */ float f1 = A1 * B5, f3 = A3 * B5, f5 = A5 * B5, f7 = A7 * B5;
                /* Phase 8 */ float h1 = A1 * B7, h3 = A3 * B7, h5 = A5 * B7, h7 = A7 * B7;

                /* Phase 1+5 */ float w1 = a4 + e4, w2 = a4 - e4;
                /* Phase 3+7 */ float x1 = c2 + g6, x2 = c6 - g2;
                /* Phase 2+6 */ float y1 = b1 + d3, y2 = b3 - d7, y3 = b5 - d1, y4 = b7 - d5;
                /* Phase 4+8 */ float z1 = f5 + h7, z2 = f1 + h5, z3 = f7 + h3, z4 = f3 - h1;
                
                /* Phase 1+3+5+7 */ float u1 = w1 + x1, u2 = w2 + x2, u3 = w2 - x2, u4 = w1 - x1;
                /* Phase 2+6+4+8 */ float v1 = y1 + z1, v2 = y2 - z2, v3 = y3 + z3, v4 = y4 + z4;

                tmp[0 * DCT_SIZE + col] = u1 + v1;
                tmp[1 * DCT_SIZE + col] = u2 + v2;
                tmp[2 * DCT_SIZE + col] = u3 + v3;
                tmp[3 * DCT_SIZE + col] = u4 + v4;
                tmp[4 * DCT_SIZE + col] = u4 - v4;
                tmp[5 * DCT_SIZE + col] = u3 - v3;
                tmp[6 * DCT_SIZE + col] = u2 - v2;
                tmp[7 * DCT_SIZE + col] = u1 - v1;
            }
            
            for (int row = 0; row < DCT_SIZE; row++)
            {
                float B0 = tmp[row * DCT_SIZE + 0], B1 = tmp[row * DCT_SIZE + 1];
                float B2 = tmp[row * DCT_SIZE + 2], B3 = tmp[row * DCT_SIZE + 3];
                float B4 = tmp[row * DCT_SIZE + 4], B5 = tmp[row * DCT_SIZE + 5];
                float B6 = tmp[row * DCT_SIZE + 6], B7 = tmp[row * DCT_SIZE + 7];
                
                /* Phase 1 */ float a4 = C4 * B0;
                /* Phase 5 */ float e4 = C4 * B4;
                /* Phase 3 */ float c2 = C2 * B2, c6 = C6 * B2;
                /* Phase 7 */ float g2 = C2 * B6, g6 = C6 * B6;
                /* Phase 2 */ float b1 = C1 * B1, b3 = C3 * B1, b5 = C5 * B1, b7 = C7 * B1;
                /* Phase 4 */ float d1 = C1 * B3, d3 = C3 * B3, d5 = C5 * B3, d7 = C7 * B3;
                /* Phase 6 */ float f1 = C1 * B5, f3 = C3 * B5, f5 = C5 * B5, f7 = C7 * B5;
                /* Phase 8 */ float h1 = C1 * B7, h3 = C3 * B7, h5 = C5 * B7, h7 = C7 * B7;

                /* Phase 1+5 */ float w1 = a4 + e4, w2 = a4 - e4;
                /* Phase 3+7 */ float x1 = c2 + g6, x2 = c6 - g2;
                /* Phase 2+6 */ float y1 = b1 + d3, y2 = b3 - d7, y3 = b5 - d1, y4 = b7 - d5;
                /* Phase 4+8 */ float z1 = f5 + h7, z2 = f1 + h5, z3 = f7 + h3, z4 = f3 - h1;
                
                /* Phase 1+3+5+7 */ float u1 = w1 + x1, u2 = w2 + x2, u3 = w2 - x2, u4 = w1 - x1;
                /* Phase 2+6+4+8 */ float v1 = y1 + z1, v2 = y2 - z2, v3 = y3 + z3, v4 = y4 + z4;

                output[row * DCT_SIZE + 0] = u1 + v1;
                output[row * DCT_SIZE + 1] = u2 + v2;
                output[row * DCT_SIZE + 2] = u3 + v3;
                output[row * DCT_SIZE + 3] = u4 + v4;
                output[row * DCT_SIZE + 4] = u4 - v4;
                output[row * DCT_SIZE + 5] = u3 - v3;
                output[row * DCT_SIZE + 6] = u2 - v2;
                output[row * DCT_SIZE + 7] = u1 - v1;
            }
        }
        
        uint bit_buf;
        int bit_cnt;
        bool hit_end, hit_rst;
        
        void RefillBits(byte[] src) {
            while (bit_cnt <= 24 && !hit_end) {
                byte next = src[buf_offset++];
                // JPEG byte stuffing
                if (next == 0xFF) {
                    byte type = src[buf_offset++];
                    
                    if (type == (MARKER_IMAGE_END & 0xFF)) {
                        next = 0;
                        hit_end = true;
                        buf_offset -= 2;
                    } else if (type >= (MARKER_RST_MRKR0 & 0xFF) && type <= (MARKER_RST_MRKR7 & 0xFF)) {
                        hit_rst = true;
                        continue;
                    } else if (type != 0) {
                        Fail("unexpected marker");
                    }
                }
                
                bit_buf <<= 8;
                bit_buf |= next;
                bit_cnt += 8;
            }
        }
        
        // Duplicates PeekBits/ConsumeBits for faster functionality
        int ReadBits(int count) {
            int read = bit_cnt - count;
            int bits = (int)(bit_buf >> read);
            
            bit_buf &= (uint)((1 << read) - 1);
            bit_cnt -= count;
            return bits;
        }
        
        int PeekBits(int count) {
            int read = bit_cnt - count;
            int bits = (int)(bit_buf >> read);
            return bits;
        }
        
        void ConsumeBits(int count) {
            int read = bit_cnt - count;
            
            bit_buf &= (uint)((1 << read) - 1);
            bit_cnt -= count;
        }
        
        byte ReadHuffman(HuffmanTable table, byte[] src) {
            RefillBits(src);
            int codeword = PeekBits(HUFF_FAST_BITS);
            
            int packed = table.fast[codeword];
            if (packed != 0) {
                ConsumeBits(packed >> HUFF_FAST_LEN_SHIFT);
                return (byte)packed;
            }
            
            ConsumeBits(HUFF_FAST_BITS);
            for (int i = HUFF_FAST_BITS; i < HUFF_MAX_BITS; i++)
            {
                codeword <<= 1;
                codeword |= ReadBits(1);
                
                if (codeword < table.endCodewords[i]) {
                    int offset = table.firstOffsets[i] + (codeword - table.firstCodewords[i]);
                    byte value = table.values[offset];
                    return value;
                }
            }
            
            Fail("no huffman code");
            return 0;
        }
        
        int ReadBiasedValue(byte[] src, int bits) {
            if (bits == 0) return 0;
            RefillBits(src);
            
            int value = ReadBits(bits);
            int midpoint = 1 << (bits - 1);
            
            // E.g. for two bits, bits/values are:
            // 00, 01, 10, 11
            // -3, -2,  2,  3
            if (value < midpoint) {
                value += (-1 << bits) + 1;
            }
            return value;
        }
    }
    
    class JpegComponent
    {
        public byte ID;
        public byte QuantTable;    
        public byte ACHuffTable;
        public byte DCHuffTable;      
        public int  PredDCValue;
        
        public byte BlocksPerMcuX;
        public byte BlocksPerMcuY;        
        public byte SamplesPerBlockX;
        public byte SamplesPerBlockY;
        
        public byte SamplingHor;
        public byte SamplingVer;
    }
    
    class HuffmanTable
    {
        // Maximum of 256 values/symbols and 16 bit length codes
        
        // Starting codeword for each bit length
        public ushort[] firstCodewords;
        // (Last codeword + 1) for each bit length. 0 is ignored.
        public ushort[] endCodewords;
        // Base offset into Values for codewords of each bit length.
        public ushort[] firstOffsets;
        // Values/Symbols list
        public byte[]   values;
        // Fast lookup table for huffman codes
        public ushort[] fast;
    }
}