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

        public static bool DetectHeader(byte[] data) {
            return MatchesSignature(data, jfifSig)
                || MatchesSignature(data, exifSig);
        }
        
        public override SimpleBitmap Decode(byte[] src) {
            SetBuffer(src);
            SimpleBitmap bmp = new SimpleBitmap();
            
            ReadMarkers(src, bmp);
            Fail("JPEG decoder unfinished");
            return null;
        }
        
        
        const ushort MARKER_IMAGE_BEG = 0xFFD8;
        const ushort MARKER_IMAGE_END = 0xFFD9;
        const ushort MARKER_APP0      = 0xFFE0;
        const ushort MARKER_APP1      = 0xFFE1;
        const ushort MARKER_TBL_QUANT = 0xFFDB;
        const ushort MARKER_TBL_HUFF  = 0xFFC4;
        const ushort MARKER_FRAME_BEG = 0xFFC0;
        const ushort MARKER_SCAN_BEG  = 0xFFDA;
        
        void ReadMarkers(byte[] src, SimpleBitmap bmp) {
            for (;;)
            {
                int offset    = AdvanceOffset(2);
                ushort marker = MemUtils.ReadU16_BE(src, offset);
                
                switch (marker)
                {
                    case MARKER_IMAGE_BEG:
                        break; // Nothing to do
                    case MARKER_IMAGE_END:
                        return;
                    case MARKER_APP0:
                    case MARKER_APP1:
                        SkipMarker(src);
                        break;
                        
                    case MARKER_TBL_HUFF:
                        ReadHuffmanTable(src);
                        break;
                    case MARKER_TBL_QUANT:
                        ReadQuantisationTables(src);
                        break;
                    case MARKER_FRAME_BEG:
                        ReadFrameStart(src);
                        break;
                    case MARKER_SCAN_BEG:
                        ReadScanStart(src);
                        DecodeMCUs(src);
                        break;                        
                        
                    default:
                        Fail("unknown marker:" + marker.ToString("X4"));
                        break;
                }
            }
        }
        
        void SkipMarker(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            // length *includes* 2 bytes of length
            AdvanceOffset(length - 2);
        }
        
        void ReadQuantisationTables(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            
            // length *includes* 2 bytes of length
            offset = AdvanceOffset(length - 2);
            length -= 2;
            
            // Can have more than one quantisation table
            while (length != 0)
            {
                if (length < 65) Fail("quant table too short: " + length);
                length -= 65;
                
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
            
            // TODO multi tables
            byte flags = src[offset++];
            
            HuffmanTable table;
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
                //
                if (count != 0) {
                    table.endCodewords[i] = (ushort)(code + count);
                } else {
                    table.endCodewords[i] = 0;
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
            
            HuffmanTable[] tables = (flags >> 4) != 0 ? ac_huff_tables : dc_huff_tables;
            tables[flags & 0x03]  = table;
        }
        
        void ReadFrameStart(byte[] src) {
            int offset = AdvanceOffset(2);
            int length = MemUtils.ReadU16_BE(src, offset);
            // length *includes* 2 bytes of length
            offset = AdvanceOffset(length - 2);
            
            byte bits  = src[offset + 0];
            if (bits != 8) Fail("bits per sample");
            int height = MemUtils.ReadU16_BE(src, offset + 1);
            int width  = MemUtils.ReadU16_BE(src, offset + 3);
            
            byte numComps = src[offset + 5];
            if (!(numComps == 1 || numComps == 3)) Fail("num components");
            offset += 6;
            
            comps = new JpegComponent[numComps];
            for (int i = 0; i < numComps; i++)
            {
                JpegComponent comp = default(JpegComponent);
                comp.ID          = src[offset++];
                byte sampling    = src[offset++];
                comp.SamplingHor = (byte)(sampling >> 4);
                comp.SamplingVer = (byte)(sampling & 0x0F);
                comp.QuantTable  = src[offset++];
                comps[i] = comp;
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
            
            byte spec_lo  = src[offset++];
            byte spec_hi  = src[offset++];
            byte succ_apr = src[offset++];
            
            if (spec_lo != 0) Fail("spectral range start");
            if (spec_hi != 0 && spec_hi != 63) Fail("spectral range end");
            if (succ_apr != 0) Fail("successive approximation");
        }
        
        void SetHuffTables(byte compID, byte tables) {
            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i].ID != compID) continue;
                
                comps[i].DCHuffTable = (byte)(tables >> 4);
                comps[i].ACHuffTable = (byte)(tables & 0x0F);
                return;
            }
            Fail("unknown scan component");
        }
        
        void DecodeMCUs(byte[] src) {
            Fail("MCUs");
        }
    }
    
    struct JpegComponent
    {
        public byte ID;
        public byte SamplingHor;
        public byte SamplingVer;
        public byte QuantTable;
        public byte ACHuffTable;
        public byte DCHuffTable;
    }
    
    struct HuffmanTable
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