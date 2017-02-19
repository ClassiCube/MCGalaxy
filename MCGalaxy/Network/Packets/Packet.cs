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
        
        public static byte[] Motd(Player p, string motd) {
            byte[] buffer = new byte[131];
            buffer[0] = Opcode.Handshake;
            buffer[1] = Server.version;
            
            motd = ChatTokens.Apply(motd, p);
            if (motd.Length > 64) {
                NetUtils.Write(motd, buffer, 2, p.hasCP437);
                NetUtils.Write(motd.Substring(64), buffer, 66, p.hasCP437);
            } else {
                NetUtils.Write(Server.name, buffer, 2, p.hasCP437);
                NetUtils.Write(motd, buffer, 66, p.hasCP437);
            }

            buffer[130] = Block.canPlace(p, Block.blackrock) ? (byte)100 : (byte)0;
            return buffer;
        }
        
        public static byte[] AddEntity(byte id, string name, ushort x, ushort y,
                                       ushort z, byte rotx, byte roty, bool hasCP437) {
            byte[] buffer = new byte[74];
            buffer[0] = Opcode.AddEntity;
            buffer[1] = id;
            NetUtils.Write(name.TrimEnd('+'), buffer, 2, hasCP437);
            NetUtils.WriteU16(x, buffer, 66);
            NetUtils.WriteU16(y, buffer, 68);
            NetUtils.WriteU16(z, buffer, 70);
            buffer[72] = rotx;
            buffer[73] = roty;
            return buffer;
        }
        
        public static byte[] Teleport(byte id, ushort x, ushort y, ushort z,
                                      byte rotx, byte roty) {
            byte[] buffer = new byte[10];
            buffer[0] = Opcode.EntityTeleport;
            buffer[1] = id;
            NetUtils.WriteU16(x, buffer, 2);
            NetUtils.WriteU16(y, buffer, 4);
            NetUtils.WriteU16(z, buffer, 6);
            buffer[8] = rotx;
            buffer[9] = roty;
            return buffer;
        }
        
        public static byte[] BlankMessage() { return Message("", 0, false); }
        
        public static byte[] Message(string message, byte id, bool hasCp437) {
            byte[] buffer = new byte[66];
            buffer[0] = Opcode.Message;
            buffer[1] = id;
            NetUtils.Write(message, buffer, 2, hasCp437);
            return buffer;
        }
        
        public static byte[] UserType(Player p) {
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.SetPermission;
            buffer[1] = Block.canPlace(p, Block.blackrock) ? (byte)100 : (byte)0;
            return buffer;
        }
        
        public static byte[] Kick(string message, bool cp437) {
            byte[] buffer = new byte[65];
            buffer[0] = Opcode.Kick;
            NetUtils.Write(message, buffer, 1, cp437);
            return buffer;
        }
    }
}
