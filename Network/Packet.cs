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
        
        public static byte[] MakeMotd(Player p, bool ignoreLevelMotd) {
            byte[] buffer = new byte[131];
            buffer[0] = Opcode.Handshake;
            buffer[1] = Server.version;
            bool cp437 = p.HasCpeExt(CpeExt.FullCP437);
            Level lvl = p.level;
            
            if (ignoreLevelMotd || lvl.motd == "ignore") {
                NetUtils.Write(Server.name, buffer, 2, cp437);
                string line2 = String.IsNullOrEmpty(p.group.MOTD) ? Server.motd : p.group.MOTD;
                NetUtils.Write(line2, buffer, 66, cp437);
            } else if (lvl.motd.Length > 64) {
                NetUtils.Write(lvl.motd, buffer, 2, cp437);
                NetUtils.Write(lvl.motd.Substring(64), buffer, 66, cp437);
            } else {
                NetUtils.Write(Server.name, buffer, 2, cp437);
                NetUtils.Write(lvl.motd, buffer, 66, cp437);
            }

            buffer[130] = Block.canPlace(p, Block.blackrock) ? (byte)100 : (byte)0;
            return buffer;
        }
        
        public static byte[] MakeUserType(Player p) {
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.SetPermission;
            buffer[1] = Block.canPlace(p, Block.blackrock) ? (byte)100 : (byte)0;
            return buffer;
        }
    }
}
