/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy {

    public static partial class Packet {
        
        public static byte[] ExtInfo(byte count) {
            byte[] buffer = new byte[67];
            buffer[0] = Opcode.CpeExtInfo;
            NetUtils.WriteAscii("MCGalaxy " + Server.Version, buffer, 1);
            NetUtils.WriteI16((short)count, buffer, 65);
            return buffer;
        }
        
        public static byte[] ExtEntry(string name, int version) {
            byte[] buffer = new byte[69];
            buffer[0] = Opcode.CpeExtEntry;
            NetUtils.WriteAscii(name, buffer, 1);
            NetUtils.WriteI32(version, buffer, 65);
            return buffer;
        }
        
        public static byte[] ClickDistance(short distance) {
            byte[] buffer = new byte[3];
            buffer[0] = Opcode.CpeSetClickDistance;
            NetUtils.WriteI16(distance, buffer, 1);
            return buffer;
        }
        
        public static byte[] HoldThis(byte block, bool locked) {
            byte[] buffer = new byte[3];
            buffer[0] = Opcode.CpeHoldThis;
            buffer[1] = block;
            buffer[2] = (byte)(locked ? 1 : 0);
            return buffer;
        }
        
        public static byte[] TextHotKey(string label, string input, int keycode, byte mods) {
            byte[] buffer = new byte[134];
            buffer[0] = Opcode.CpeSetTextHotkey;
            NetUtils.WriteAscii(label, buffer, 1);
            NetUtils.WriteAscii(input, buffer, 65);
            NetUtils.WriteI32(keycode, buffer, 129);
            buffer[133] = mods;
            return buffer;
        }

        public static byte[] EnvColor(byte type, short r, short g, short b) {
            byte[] buffer = new byte[8];
            buffer[0] = Opcode.CpeEnvColors;
            buffer[1] = type;
            NetUtils.WriteI16(r, buffer, 2);
            NetUtils.WriteI16(g, buffer, 4);
            NetUtils.WriteI16(b, buffer, 6);
            return buffer;
        }
        
        public static byte[] MakeSelection(byte id, string label, Vec3U16 p1, Vec3U16 p2,
                                           short r, short g, short b, short opacity ) {
            byte[] buffer = new byte[86];
            buffer[0] = Opcode.CpeMakeSelection;
            buffer[1] = id;
            NetUtils.WriteAscii(label, buffer, 2);
            
            NetUtils.WriteU16(p1.X, buffer, 66);
            NetUtils.WriteU16(p1.Y, buffer, 68);
            NetUtils.WriteU16(p1.Z, buffer, 70);
            NetUtils.WriteU16(p2.X, buffer, 72);
            NetUtils.WriteU16(p2.Y, buffer, 74);
            NetUtils.WriteU16(p2.Z, buffer, 76);
            
            NetUtils.WriteI16(r, buffer, 78);
            NetUtils.WriteI16(g, buffer, 80);
            NetUtils.WriteI16(b, buffer, 82);
            NetUtils.WriteI16(opacity, buffer, 84);
            return buffer;
        }
        
        public static byte[] DeleteSelection(byte id) {
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.CpeRemoveSelection;
            buffer[1] = id;
            return buffer;
        }
        
        public static byte[] BlockPermission(byte type, bool place, bool delete) {
            byte[] buffer = new byte[4];
            WriteBlockPermission(type, place, delete, buffer, 0);
            return buffer;
        }
        
        public static void WriteBlockPermission(byte type, bool place, bool delete, byte[] buffer, int index) {
            buffer[index + 0] = Opcode.CpeSetBlockPermission;
            buffer[index + 1] = type;
            buffer[index + 2] = place ? (byte)1 : (byte)0;
            buffer[index + 3] = delete ? (byte)1 : (byte)0;            
        }
        
        public static byte[] MapAppearance(string url, byte side, byte edge, int sideLevel) {
            byte[] buffer = new byte[69];
            WriteMapAppearance(buffer, url, side, edge, sideLevel);
            return buffer;
        }
        
        public static byte[] MapAppearanceV2(string url, byte side, byte edge, int sideLevel,
                                             int cloudHeight, int maxFog) {
            byte[] buffer = new byte[73];
            WriteMapAppearance(buffer, url, side, edge, sideLevel);
            NetUtils.WriteI16((short)cloudHeight, buffer, 69);
            NetUtils.WriteI16((short)maxFog, buffer, 71);
            return buffer;
        }
        
        static void WriteMapAppearance(byte[] buffer, string url, byte side, byte edge, int sideLevel) {
            buffer[0] = Opcode.CpeEnvSetMapApperance;
            NetUtils.WriteAscii(url, buffer, 1);
            buffer[65] = side;
            buffer[66] = edge;
            NetUtils.WriteI16((short)sideLevel, buffer, 67);
        }

        public static byte[] EnvWeatherType(byte type) { // 0 - sunny; 1 - raining; 2 - snowing
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.CpeEnvWeatherType;
            buffer[1] = type;
            return buffer;
        }
        
        public static byte[] HackControl(bool canFly, bool canNoclip,
                                         bool canSpeed, bool canRespawn,
                                         bool can3rdPerson, short maxJumpHeight) {
            byte[] buffer = new byte[8];
            buffer[0] = Opcode.CpeHackControl;
            buffer[1] = (byte)(canFly ? 1 : 0);
            buffer[2] = (byte)(canNoclip ? 1 : 0);
            buffer[3] = (byte)(canSpeed ? 1 : 0);
            buffer[4] = (byte)(canRespawn ? 1 : 0);
            buffer[5] = (byte)(can3rdPerson ? 1 : 0);
            NetUtils.WriteI16(maxJumpHeight, buffer, 6);
            return buffer;
        }

        
        public static byte[] EnvMapUrl(string url) {
            byte[] buffer = new byte[65];
            buffer[0] = Opcode.CpeSetMapEnvUrl;
            NetUtils.WriteAscii(url, buffer, 1);
            return buffer;
        }
        
        public static byte[] EnvMapProperty(EnvProp prop, int value) {
            byte[] buffer = new byte[6];
            buffer[0] = Opcode.CpeSetMapEnvProperty;
            buffer[1] = (byte)prop;
            NetUtils.WriteI32(value, buffer, 2);
            return buffer;
        }
		
        public static byte[] EntityProperty(EntityProp prop, int value) {
            byte[] buffer = new byte[6];
            buffer[0] = Opcode.CpeSetEntityProperty;
            buffer[1] = (byte)prop;
            NetUtils.WriteI32(value, buffer, 2);
            return buffer;
        }
    }
}
