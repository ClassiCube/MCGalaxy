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
            
            ReadGlobalHeader(src);
            ReadMarkers(src);
            Fail("GIF decoder unfinished");
            return null;
        }
        
        const int LOGICAL_DESC_SIZE = 7;
        void ReadGlobalHeader(byte[] src) {
            // read logical screen descriptor
            int offset = AdvanceOffset(LOGICAL_DESC_SIZE);
            
            ushort width  = MemUtils.ReadU16_LE(src, offset + 0);
            ushort height = MemUtils.ReadU16_LE(src, offset + 2);
            
            byte flags   = src[offset + 4];
            byte bgIndex = src[offset + 5];
            // src[offset + 6] is pixel aspect ratio - not used
            
            bool hasGlobalPal = (flags & 0x80) != 0;
            int globalPalSize = 1 << ((flags & 0x7) + 1);
            if (hasGlobalPal) ReadGlobalPalette(src, globalPalSize);
            
            if (hasGlobalPal && bgIndex < globalPalSize)
                bgColor = globalPal[bgIndex];
        }
        
        void ReadGlobalPalette(byte[] src, int size) {
            Pixel[] pal = new Pixel[size];
            int offset  = AdvanceOffset(3 * size);
            
            for (int i = 0; i < pal.Length; i++)
            {
                pal[i].R = src[offset++];
                pal[i].G = src[offset++];
                pal[i].B = src[offset++];
                pal[i].A = 255;
            }
            globalPal = pal;
        }
        
        
        const byte MARKER_EXTENSION = 0x21;
        const byte MARKER_IMAGE_END = 0x3B;
        const byte MARKER_IMAGE_BEG = 0x2C;
        
        void ReadMarkers(byte[] src) {
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
                        ReadImage(src);
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
        
        
        void ReadImage(byte[] src) {
            // Read image descriptor header
            int offset = AdvanceOffset(2 + 2 + 2 + 2 + 1);
            
            ushort imageX = MemUtils.ReadU16_LE(src, offset + 0);
            ushort imageY = MemUtils.ReadU16_LE(src, offset + 2);
            ushort imageW = MemUtils.ReadU16_LE(src, offset + 4);
            ushort imageH = MemUtils.ReadU16_LE(src, offset + 6);
            byte flags    = src[offset + 8];
            
            // Read image data
            offset = AdvanceOffset(1);
            byte minCodeSize = src[offset];
            
            // Init LZW variables
            int codeLen   = minCodeSize + 1;
            int codeMask  = (1 << codeLen) - 1;
            int clearCode = (1 << minCodeSize) + 0;
            int stopCode  = (1 << minCodeSize) + 1;
            int dictEnd;
            DictEntry[] dict = new DictEntry[1 << codeLen];
            
            // Bit buffer state
            uint bufVal = 0;
            int bufLen  = 0;
            
            // Spec says clear code _should_ be sent first, but not required
            for (dictEnd = 0; dictEnd < (1 << minCodeSize); dictEnd++)
            {
                dict[dictEnd].value = (byte)dictEnd;
                dict[dictEnd].prev  = -1;
                dict[dictEnd].len   =  1;
            }
            dictEnd += 2; // "clear code" and "stop code" entries
            
            for (;;)
            {
                int code = 0;
                
                if (bufLen < codeLen) {
                    
                }
                
                code = (int)(bufVal & codeMask);
                bufVal >>= codeMask;
                bufLen -= codeLen;
                
                if (code == clearCode) {
                    codeLen  = minCodeSize + 1;
                    codeMask = (1 << codeLen) - 1;
                    
                    // Clear dictionary
                    for (dictEnd = 0; dictEnd < (1 << minCodeSize); dictEnd++)
                    {
                        dict[dictEnd].value = (byte)dictEnd;
                        dict[dictEnd].prev  = -1;
                        dict[dictEnd].len   =  1;
                    }
                    dictEnd += 2; // "clear code" and "stop code" entries
                } else if (code == stopCode) {
                    break;
                }
            }
            
            //SkipSubBlocks(src);
            Fail("GIF decoder unfinished");
        }
        
        struct DictEntry
        {
            public byte value;
            public short prev, len;
        }
    }
}