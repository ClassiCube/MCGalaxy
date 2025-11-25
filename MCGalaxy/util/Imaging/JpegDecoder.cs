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
            
            ComputeIDCTFactors();
            ReadMarkers(src, bmp);
            return bmp;
        }
        
        
        const ushort MARKER_IMAGE_BEG = 0xFFD8;
        const ushort MARKER_IMAGE_END = 0xFFD9;
        const ushort MARKER_APP0      = 0xFFE0;
        const ushort MARKER_APP15     = 0xFFEF;
        const ushort MARKER_TBL_QUANT = 0xFFDB;
        const ushort MARKER_TBL_HUFF  = 0xFFC4;
        const ushort MARKER_FRAME_BEG = 0xFFC0;
        const ushort MARKER_SCAN_BEG  = 0xFFDA;
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
                } else if (marker == MARKER_COMMENT) {
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
            
            // length *includes* 2 bytes of length
            offset = AdvanceOffset(length - 2);
            length -= 2;
            
            // Can have more than one quantisation table
            while (length != 0)
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
        
        const int HUFF_MAX_BITS = 16;
        const int HUFF_MAX_VALS = 256;
        void ReadHuffmanTable(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            // length *includes* 2 bytes of length
            offset = AdvanceOffset(length - 2);
            
            byte flags = src[offset++];
            
            HuffmanTable table    = new HuffmanTable();
            HuffmanTable[] tables = (flags >> 4) != 0 ? ac_huff_tables : dc_huff_tables;
            tables[flags & 0x03]  = table;
            
            table.firstCodewords = new ushort[HUFF_MAX_BITS];
            table.endCodewords   = new ushort[HUFF_MAX_BITS];
            table.firstOffsets   = new ushort[HUFF_MAX_BITS];
            table.values         = new byte[HUFF_MAX_VALS];
            
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
            total = 0;

            // Read values for each codeword.
            //  Note that although codewords are ordered, values may not be.
            //  Some values may also not be assigned to any codeword.
            for (int i = 0; i < counts.Length; i++)
            {
                for (int j = 0; j < counts[i]; j++)
                {
                    table.values[total++] = src[offset++];
                }
            }
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
                comps[i].BlocksPerMcuX = comps[i].SamplingHor;
                comps[i].BlocksPerMcuY = comps[i].SamplingVer;
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
            int[] block = new int[BLOCK_SIZE];
            float[] output = new float[BLOCK_SIZE];
            
            YCbCr[] colors = new YCbCr[mcu_w * mcu_h];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i].Cr = 128;
                colors[i].Cb = 128;
            }
            
            for (int mcuY = 0; mcuY < mcus_y; mcuY++)
                for (int mcuX = 0; mcuX < mcus_x; mcuX++)
            {
                for (int i = 0; i < comps.Length; i++)
                {
                    JpegComponent comp = comps[i];
                    
                    for (int by = 0; by < comp.BlocksPerMcuY; by++)
                        for (int bx = 0; bx < comp.BlocksPerMcuX; bx++)
                    {
                        DecodeBlock(comp, src, block, output);
                        IDCT(block, output);
                        
                        int samplesX = lowestHor / comp.SamplingHor;
                        int samplesY = lowestVer / comp.SamplingVer;
                        
                        for (int y = 0; y < BLOCK_SAMPLES; y++)
                            for (int x = 0; x < BLOCK_SAMPLES; x++)
                        {
                            for (int py = 0; py < samplesY; py++)
                                for (int px = 0; px < samplesX; px++)
                            {
                                int YY = (by * BLOCK_SAMPLES + y) * samplesY + py;
                                int XX = (bx * BLOCK_SAMPLES + x) * samplesX + px;
                                int idx = YY * mcu_w + XX;
                                
                                if (i == 0)
                                    colors[idx].Y  = output[y * BLOCK_SAMPLES + x];
                                else if (i == 1)
                                    colors[idx].Cb = output[y * BLOCK_SAMPLES + x];
                                else if (i == 2)
                                    colors[idx].Cr = output[y * BLOCK_SAMPLES + x];
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
                        float y  = colors[j].Y;
                        float cr = colors[j].Cr;
                        float cb = colors[j].Cb;
                        
                        float r =  1.40200f * (cr - 128) + y;
                        float g = -0.34414f * (cb - 128) - 0.71414f * (cr - 128) + y;
                        float b =  1.77200f * (cb - 128) + y;
                        
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
        
        void DecodeBlock(JpegComponent comp, byte[] src, int[] block, float[] output) {
            // DC value is relative to DC value from prior block
            var table    = dc_huff_tables[comp.DCHuffTable];
            int dc_code  = ReadHuffman(table, src);
            int dc_delta = ReadBiasedValue(src, dc_code);
            
            int dc_value = comp.PredDCValue + dc_delta;
            comp.PredDCValue = dc_value;
            
            byte[] dequant = quant_tables[comp.QuantTable];
            for (int j = 0; j < block.Length; j++) block[j] = 0;
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
            
            IDCT(block, output);
        }
        
        float[] idct_factors;
        void ComputeIDCTFactors() {
            float[] factors = new float[128];
            
            for (int xy = 0; xy < 8; xy++)
            {
                for (int uv = 0; uv < 8; uv++)
                {
                    float cuv   = uv == 0 ? 0.70710678f : 1.0f;
                    float cosuv = (float)Math.Cos((2 * xy + 1) * uv * Math.PI / 16.0);
                    
                    factors[(2 * xy + 1) * uv] = cuv * cosuv;
                } 
            }
            idct_factors = factors;
        }
        
        void IDCT(int[] block, float[] output) {
            float[] factors = idct_factors;
            
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
            {
                float sum = 0.0f;
                for (int v = 0; v < 8; v++)
                    for (int u = 0; u < 8; u++)
                {
                    float suv = block[v*8+u];
                    float cu_cosu = factors[(2 * x + 1) * u];
                    float cv_cosv = factors[(2 * y + 1) * v];
                    
                    sum += cu_cosu * cv_cosv * suv;
                }
                output[y*8+x] = (sum / 4.0f) + 128.0f; // undo level shift at end
            }
        }
        
        uint bit_buf;
        int bit_cnt;
        bool end;
        
        int ReadBits(byte[] src, int count) {
            while (bit_cnt <= 24 && !end) {
                byte next = src[buf_offset++];
                // JPEG byte stuffing
                // TODO restart markers ?
                if (next == 0xFF) {
                    byte type = src[buf_offset++];
                    if (type == 0xD9) { end = true; buf_offset -= 2; }
                    else if (type != 0) Fail("unexpected marker");
                }
                
                bit_buf <<= 8;
                bit_buf |= next;
                bit_cnt += 8;
            }
            
            int read = bit_cnt - count;
            int bits = (int)(bit_buf >> read);
            
            bit_buf &= (uint)((1 << read) - 1);
            bit_cnt -= count;
            return bits;
        }
        
        byte ReadHuffman(HuffmanTable table, byte[] src) {
            int codeword = 0;
            // TODO optimise
            for (int i = 0; i < HUFF_MAX_BITS; i++)
            {
                codeword <<= 1;
                codeword |= ReadBits(src, 1);
                
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
            
            int value = ReadBits(src, bits);
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
        
        public byte SamplingHor;
        public byte SamplingVer;
        public byte BlocksPerMcuX;
        public byte BlocksPerMcuY;
        
        public byte ACHuffTable;
        public byte DCHuffTable;
        public int  PredDCValue;
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
        public byte[] values;
    }
}