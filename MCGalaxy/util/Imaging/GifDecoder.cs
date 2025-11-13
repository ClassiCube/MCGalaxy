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
    public class GifDecoder : ImageDecoder
    {
        static byte[] gif87Sig = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }; // "GIF87a"
        static byte[] gif89Sig = new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }; // "GIF89a"
        
        Pixel[] globalPal;
        Pixel bgColor;

        public static bool DetectHeader(byte[] data) {
            return MatchesSignature(data, gif87Sig)
                || MatchesSignature(data, gif89Sig);
        }
        
        public override SimpleBitmap Decode(byte[] src) {
            SetBuffer(src);
            if (!DetectHeader(src)) Fail("sig invalid");
            AdvanceOffset(gif87Sig.Length);
            
            SimpleBitmap bmp = new SimpleBitmap();
            ReadGlobalHeader(src, bmp);
            ReadMarkers(src, bmp);
            return bmp;
        }
        
        const int LOGICAL_DESC_SIZE = 7;
        void ReadGlobalHeader(byte[] src, SimpleBitmap bmp) {
            // read logical screen descriptor
            int offset = AdvanceOffset(LOGICAL_DESC_SIZE);
            
            bmp.Width  = MemUtils.ReadU16_LE(src, offset + 0);
            bmp.Height = MemUtils.ReadU16_LE(src, offset + 2);
            
            byte flags   = src[offset + 4];
            byte bgIndex = src[offset + 5];
            // src[offset + 6] is pixel aspect ratio - not used
            
            bool hasGlobalPal = (flags & 0x80) != 0;
            if (hasGlobalPal)
                globalPal = ReadPalette(src, flags);
            if (hasGlobalPal && bgIndex < globalPal.Length)
                bgColor   = globalPal[bgIndex];
            
            bmp.AllocatePixels();
        }
        
        Pixel[] ReadPalette(byte[] src, byte flags) {
            int size    = 1 << ((flags & 0x7) + 1);
            Pixel[] pal = new Pixel[size];
            int offset  = AdvanceOffset(3 * size);
            
            for (int i = 0; i < pal.Length; i++)
            {
                pal[i].R = src[offset++];
                pal[i].G = src[offset++];
                pal[i].B = src[offset++];
                pal[i].A = 255;
            }
            return pal;
        }
        
        
        const byte MARKER_EXTENSION = 0x21;
        const byte MARKER_IMAGE_END = 0x3B;
        const byte MARKER_IMAGE_BEG = 0x2C;
        
        void ReadMarkers(byte[] src, SimpleBitmap bmp) {
            for (;;)
            {
                int offset  = AdvanceOffset(1);
                byte marker = src[offset];
                switch (marker)
                {
                    case MARKER_EXTENSION:
                        ReadExtension(src);
                        break;
                    case MARKER_IMAGE_BEG:
                        ReadImage(src, bmp);
                        return; // NOTE: stops reading at first frame
                    case MARKER_IMAGE_END:
                        return;
                    default:
                        Fail("unknown marker");
                        break;
                }
            }
        }
        
        const byte EXT_GRAPHICS_CONTROL = 0xF9;
        void ReadExtension(byte[] src) {
            int offset  = AdvanceOffset(1);
            byte type   = src[offset++];
            
            if (type == EXT_GRAPHICS_CONTROL) {
                ReadGraphicsControl(src);
            } else {
                SkipSubBlocks(src);
            }
        }
        
        // Always a simple sub block
        void ReadGraphicsControl(byte[] src) {
            int offset  = AdvanceOffset(1);
            byte length = src[offset];
            if (length < 4) Fail("graphics control extension too short");
            
            // Look for transparent colour index
            offset = AdvanceOffset(length);
            bool hasTrans = (src[offset + 0] & 0x01) != 0;
            byte tcIndex  = src[offset + 3];
            
            Pixel[] pal = globalPal;
            if (hasTrans && pal != null && tcIndex < pal.Length)
                pal[tcIndex].A = 0;
            
            // should only be one sub block
            offset = AdvanceOffset(1);
            length = src[offset];
            if (length != 0) Fail("graphics control should be one sub block");
        }
        
        void SkipSubBlocks(byte[] src) {
            for (;;)
            {
                int offset  = AdvanceOffset(1);
                byte length = src[offset++];
                
                if (length == 0) return;
                AdvanceOffset(length);
            }
        }
        
        const int MAX_CODE_LEN = 12;
        const int MAX_CODES = 1 << MAX_CODE_LEN;
        byte curSubBlockLeft;
        bool subBlocksEnd;
        int subBlocksOffset;
        
        void ReadImage(byte[] src, SimpleBitmap bmp) {
            // Read image descriptor header
            int offset = AdvanceOffset(2 + 2 + 2 + 2 + 1);
            
            ushort imageX = MemUtils.ReadU16_LE(src, offset + 0);
            ushort imageY = MemUtils.ReadU16_LE(src, offset + 2);
            ushort imageW = MemUtils.ReadU16_LE(src, offset + 4);
            ushort imageH = MemUtils.ReadU16_LE(src, offset + 6);
            byte flags    = src[offset + 8];
            
            if ((flags & 0x40) != 0) Fail("Interlaced GIF unsupported");
            if (imageX + imageW > bmp.Width)  Fail("Invalid X dimensions");
            if (imageY + imageH > bmp.Height) Fail("Invalid Y dimensions");
            
            bool hasLocalPal = (flags & 0x80) != 0;
            Pixel[] localPal = null;
            if (hasLocalPal)
                localPal = ReadPalette(src, flags);
            
            Pixel[] pal = localPal ?? globalPal;
            int dst_index = 0;
            
            // Read image data
            offset = AdvanceOffset(1);
            byte minCodeSize = src[offset];
            if (minCodeSize >= MAX_CODE_LEN) Fail("codesize too long");
            
            curSubBlockLeft = 0;
            subBlocksEnd    = false;
            
            // Init LZW variables
            int codeLen   = minCodeSize + 1;
            int codeMask  = (1 << codeLen) - 1;
            int clearCode = (1 << minCodeSize) + 0;
            int stopCode  = (1 << minCodeSize) + 1;
            int prevCode, availCode;
            DictEntry[] dict = new DictEntry[1 << codeLen];
            
            // Bit buffer state
            uint bufVal = 0;
            int bufLen  = 0;
            
            // Spec says clear code _should_ be sent first, but not required
            for (availCode = 0; availCode < (1 << minCodeSize); availCode++)
            {
                dict[availCode].value = (byte)availCode;
                dict[availCode].prev  = -1;
                dict[availCode].len   =  1;
            }
            
            availCode += 2; // "clear code" and "stop code" entries
            prevCode = -1;
            
            for (;;)
            {
                // Refill buffer when needed
                if (bufLen < codeLen) {
                    int read;
                    while (bufLen <= 24 && (read = ReadNextByte()) >= 0) {
                        bufVal |= (uint)read << bufLen;
                        bufLen += 8;
                    }
                    
                    if (bufLen < codeLen) Fail("not enough bits for code");
                }
                
                int code = (int)(bufVal & codeMask);
                bufVal >>= codeLen;
                bufLen  -= codeLen;
                
                if (code == clearCode) {
                    codeLen  = minCodeSize + 1;
                    codeMask = (1 << codeLen) - 1;
                    
                    // Clear dictionary
                    for (availCode = 0; availCode < (1 << minCodeSize); availCode++)
                    {
                        dict[availCode].value = (byte)availCode;
                        dict[availCode].prev  = -1;
                        dict[availCode].len   =  1;
                    }
                    
                    availCode += 2; // "clear code" and "stop code" entries
                    prevCode = -1;
                } else if (code == stopCode) {
                    break;
                }
                
                if (code > availCode) Fail("invalid code");
                
                // Add new entry to code table unless it's full
                // GIF spec allows this as per 'deferred clear codes'
                if (prevCode >= 0 && availCode < MAX_CODES) {
                    int firstCode = code == availCode ? prevCode : code;
                    // Follow chain back to find first value
                    // TODO optimise this...
                    while (dict[firstCode].prev != -1)
                    {
                        firstCode = dict[firstCode].prev;
                    }
                    
                    dict[availCode].value = dict[firstCode].value;
                    dict[availCode].prev  = (short)prevCode;
                    dict[availCode].len   = (short)(dict[prevCode].len + 1);
                    
                    // Check if inserted code in last free entry of table
                    // If this is the case, then the table is immediately expanded
                    if (availCode == codeMask && availCode != (MAX_CODES - 1)) {
                        codeLen++;
                        codeMask = (1 << codeLen) - 1;
                        Array.Resize(ref dict, 1 << codeLen);
                    }
                    availCode++;
                }
                
                prevCode = code;
                // TODO output code
                
                // "top" entry is actually last entry in chain
                int chain_len = dict[code].len;
                for (int i = chain_len - 1; i >= 0; i--)
                {
                    int index = dst_index + i;
                    byte palIndex = dict[code].value;
                    
                    //int localX = index % imageW;
                    //int localY = index / imageW;
                    int globalX = imageX + (index % imageW);
                    int globalY = imageY + (index / imageW);                    
                    bmp.pixels[globalY * bmp.Width + globalX] = pal[palIndex];
                    
                    code = dict[code].prev;
                }

                dst_index += chain_len;
            }
        }
        
        struct DictEntry
        {
            public byte value;
            public short prev, len;
        }
        
        int ReadNextByte() {
            if (curSubBlockLeft == 0) {
                if (subBlocksEnd) return -1;
                
                subBlocksOffset = AdvanceOffset(1);
                curSubBlockLeft = buf_data[subBlocksOffset++];
                
                // If sub block length is 0, then reached end of sub blocks
                if (curSubBlockLeft == 0) {
                    subBlocksEnd = true;
                    return -1;
                }
            }
            
            curSubBlockLeft--;
            return buf_data[subBlocksOffset++];
        }
    }
}