/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Bots {
    public static class Instructions {
        
        public static Dictionary<string, Func<PlayerBot, bool>> Defined = 
            new Dictionary<string, Func<PlayerBot, bool>>{
            { "walk", DoWalk }, { "teleport", DoTeleport },
        };
            
        static bool DoWalk(PlayerBot bot) {
            bot.foundPos[0] = bot.Waypoints[bot.currentPoint].x;
            bot.foundPos[1] = bot.Waypoints[bot.currentPoint].y;
            bot.foundPos[2] = bot.Waypoints[bot.currentPoint].z;
            bot.movement = true;

            if ((ushort)(bot.pos[0] / 32) == (ushort)(bot.Waypoints[bot.currentPoint].x / 32)) {
                if ((ushort)(bot.pos[2] / 32) == (ushort)(bot.Waypoints[bot.currentPoint].z / 32)) {
                    bot.rot[0] = bot.Waypoints[bot.currentPoint].rotx;
                    bot.rot[1] = bot.Waypoints[bot.currentPoint].roty;
                    bot.movement = false;
                    bot.NextInstruction();
                    return false;
                }
            }
            bot.AdvanceRotation();
            return true;
        }
        
        static bool DoTeleport(PlayerBot bot) {
            bot.pos[0] = bot.Waypoints[bot.currentPoint].x;
            bot.pos[1] = bot.Waypoints[bot.currentPoint].y;
            bot.pos[2] = bot.Waypoints[bot.currentPoint].z;
            bot.rot[0] = bot.Waypoints[bot.currentPoint].rotx;
            bot.rot[1] = bot.Waypoints[bot.currentPoint].roty;
            bot.NextInstruction();
            return true;
        }
        
       static bool DoWait(PlayerBot bot) {
            if (bot.countdown != 0) {
                bot.countdown--;
                if (bot.countdown == 0) {
                    bot.NextInstruction();
                    return false;
                }
            } else {
                bot.countdown = bot.Waypoints[bot.currentPoint].seconds;
            }
            return true;
        }
    }
}
