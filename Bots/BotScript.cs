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
using System.IO;

namespace MCGalaxy.Bots {

    public static class BotScript {
        
        public static bool Parse(Player p, PlayerBot bot, string file) {
            if (!File.Exists(file)) { Player.SendMessage(p, "Could not find specified AI."); return false; }

            string[] codes = File.ReadAllLines(file);
            if (codes[0] != "#Version 2") { Player.SendMessage(p, "Invalid file version. Remake"); return false; }

            PlayerBot.Pos newPos = new PlayerBot.Pos();
            try { bot.Waypoints.Clear(); } catch { }
            bot.currentPoint = 0; bot.countdown = 0; bot.movementSpeed = 12;

            foreach (string line in codes) {
                if (line == "" || line[0] == '#') continue;
                string[] args = line.Split(' ');                
                newPos.type = args[0];
                
                try {
                    switch (args[0].ToLower()) {
                        case "walk":
                        case "teleport":
                            newPos.x = Convert.ToUInt16(args[1]);
                            newPos.y = Convert.ToUInt16(args[2]);
                            newPos.z = Convert.ToUInt16(args[3]);
                            newPos.rotx = Convert.ToByte(args[4]);
                            newPos.roty = Convert.ToByte(args[5]);
                            break;
                        case "wait":
                        case "speed":
                            newPos.seconds = Convert.ToInt16(args[1]); break;
                        case "nod":
                        case "spin":
                            newPos.seconds = Convert.ToInt16(args[1]);
                            newPos.rotspeed = Convert.ToInt16(args[2]);
                            break;
                        case "linkscript":
                            newPos.newscript = args[1]; break;
                        case "reset":
                        case "jump":
                        case "remove": 
                            break;
                        default: 
                            continue;
                    }
                    bot.Waypoints.Add(newPos);
                } catch { 
                    Player.SendMessage(p, "AI file corrupt."); return false;
                }
            }
            return true;
        }
    }
}