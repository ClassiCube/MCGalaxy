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
using MCGalaxy.Network;

namespace MCGalaxy {
    public static class Hacks {
        
        public static bool CanUseHacks(Player p, Level lvl) {
            byte[] packet = MakeHackControl(p, lvl.GetMotd(p));
            return packet[1] != 0 && packet[2] != 0 && packet[3] != 0 && packet[4] != 0 && packet[5] != 0;
        }
        
        public static bool CanUseFly(Player p, Level lvl) {
            return MakeHackControl(p, lvl.GetMotd(p))[1] != 0;
        }
        
        public static bool CanUseNoclip(Player p, Level lvl) {
            return MakeHackControl(p, lvl.GetMotd(p))[2] != 0;
        }
        
        public static bool CanUseSpeed(Player p, Level lvl) {
            return MakeHackControl(p, lvl.GetMotd(p))[3] != 0;
        }
        
        public static byte[] MakeHackControl(Player p, string motd) {
            motd = Colors.Strip(motd);
            bool isOp = p.Rank >= LevelPermission.Operator;
            
            bool fly = true, noclip = true, speed = true, respawn = true, _3rdPerson = true;
            short maxJump = -1;
            string[] parts = motd.SplitSpaces();
            for (int i = 0; i < parts.Length; i++) {
                string part = parts[i];
                if (part.CaselessEq("-hax")) {
                    fly = false; noclip = false; speed = false; respawn = false; _3rdPerson = false;
                } else if (part.CaselessEq("-ophax") && isOp) {
                    fly = false; noclip = false; speed = false; respawn = false; _3rdPerson = false;
                } else if (part.CaselessEq("+ophax") && isOp) {
                    fly = true; noclip = true; speed = true; respawn = true; _3rdPerson = true;
                }
                
                if (part.CaselessEq("+noclip")) { noclip = true; }
                else if (part.CaselessEq("+fly")) { fly = true; }
                else if (part.CaselessEq("+speed")) { speed = true; }
                else if (part.CaselessEq("+respawn")) { respawn = true; }
                else if (part.CaselessEq("+thirdperson")) { _3rdPerson = true; }
                
                if (part.CaselessEq("-noclip")) { noclip = false; }
                else if (part.CaselessEq("-fly")) { fly = false; }
                else if (part.CaselessEq("-speed")) { speed = false; }
                else if (part.CaselessEq("-respawn")) { respawn = false; }
                else if (part.CaselessEq("-thirdperson")) { _3rdPerson = false; }
                
                if (!part.CaselessStarts("jumpheight=")) continue;
                string heightPart = part.Substring(part.IndexOf('=') + 1);
                float value;
                if (Utils.TryParseSingle(heightPart, out value))
                    maxJump = (short)(value * 32);
            }            
            return Packet.HackControl(fly, noclip, speed, respawn, _3rdPerson, maxJump);
        }
    }
}