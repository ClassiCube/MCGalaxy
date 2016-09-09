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
    public static class ScriptFile {
        
        public static bool Parse(Player p, PlayerBot bot, string file) {
            if (!File.Exists(file)) { Player.Message(p, "Could not find specified AI."); return false; }

            string[] codes = File.ReadAllLines(file);
            if (codes[0] != "#Version 2") { Player.Message(p, "Invalid file version. Remake"); return false; }

            InstructionData newPos = new InstructionData();
            try { bot.Waypoints.Clear(); } catch { }
            bot.cur = 0; bot.countdown = 0; bot.movementSpeed = 3;

            foreach (string line in codes) {
                if (line == "" || line[0] == '#') continue;
                string[] args = line.Split(' ');
                newPos.type = args[0];
                
                try {
                    switch (args[0].ToLower()) {
                        case "walk":
                        case "teleport":
                            newPos.x = ushort.Parse(args[1]);
                            newPos.y = ushort.Parse(args[2]);
                            newPos.z = ushort.Parse(args[3]);
                            newPos.rotx = byte.Parse(args[4]);
                            newPos.roty = byte.Parse(args[5]);
                            break;
                        case "wait":
                        case "speed":
                            newPos.seconds = short.Parse(args[1]); break;
                        case "nod":
                        case "spin":
                            newPos.seconds = short.Parse(args[1]);
                            newPos.rotspeed = short.Parse(args[2]);
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
                    Player.Message(p, "AI file corrupt."); return false;
                }
            }
            return true;
        }
        
        public static void Append(Player p, string ai, string action = "", 
                                  string extra = "10", string more = "2") {
            using (StreamWriter w = new StreamWriter("bots/" + ai, true)) {
                switch (action.ToLower()) {
                    case "":
                    case "walk":
                        w.WriteLine("walk " + p.pos[0] + " " + p.pos[1] + " " + p.pos[2] + " " + p.rot[0] + " " + p.rot[1]);
                        break;
                    case "teleport":
                    case "tp":
                        w.WriteLine("teleport " + p.pos[0] + " " + p.pos[1] + " " + p.pos[2] + " " + p.rot[0] + " " + p.rot[1]);
                        break;
                    case "wait":
                        w.WriteLine("wait " + int.Parse(extra)); break;
                    case "nod":
                        w.WriteLine("nod " + int.Parse(extra) + " " + int.Parse(more)); break;
                    case "speed":
                        w.WriteLine("speed " + int.Parse(extra)); break;
                    case "remove":
                        w.WriteLine("remove"); break;
                    case "reset":
                        w.WriteLine("reset"); break;
                    case "spin":
                        w.WriteLine("spin " + int.Parse(extra) + " " + int.Parse(more)); break;
                    case "linkscript":
                        if (extra != "10") w.WriteLine("linkscript " + extra); else Player.Message(p, "Linkscript requires a script as a parameter");
                        break;
                    case "jump":
                        w.WriteLine("jump"); break;
                    default:
                        Player.Message(p, "Could not find \"" + action + "\""); break;
                }
            }
        }
    }
}