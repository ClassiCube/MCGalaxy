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
using System.Threading;

namespace MCGalaxy {
    
    public sealed partial class Server {
        
        void DoBlockUpdates() {
            while (true) {
                Thread.Sleep(blockInterval * 1000);
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    try {
                        lvl.saveChanges();
                    } catch (Exception e) {
                        Server.ErrorLog(e);
                    }
                }
            }
        }
        
        void DoLocationChecks() {
            while (true) {
			    Player[] players = PlayerInfo.Online.Items;
			    Thread.Sleep(players.Length == 0 ? 16 : 3);
                players = PlayerInfo.Online.Items;
                
                for (int i = 0; i < players.Length; i++) {
                    try {
                        Player p = players[i];

                        if (p.frozen) {
                            p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]); continue;
                        } else if (p.following != "") {
                            Player who = PlayerInfo.FindExact(p.following);
                            if (who == null || who.level != p.level) {
                                p.following = "";
                                if (!p.canBuild)
                                    p.canBuild = true;
                                if (who != null && who.possess == p.name)
                                    who.possess = "";
                                continue;
                            }
                            
                            if (p.canBuild) {
                                p.SendPos(0xFF, who.pos[0], (ushort)(who.pos[1] - 16), who.pos[2], who.rot[0], who.rot[1]);
                            } else {
                                p.SendPos(0xFF, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1]);
                            }
                        } else if (p.possess != "") {
                            Player who = PlayerInfo.FindExact(p.possess);
                            if (who == null || who.level != p.level)
                                p.possess = "";
                        }

                        ushort x = (ushort)(p.pos[0] / 32);
                        ushort y = (ushort)(p.pos[1] / 32);
                        ushort z = (ushort)(p.pos[2] / 32);

                        if (p.level.Death)
                            p.CheckSurvival(x, y, z);
                        p.CheckBlock(x, y, z);
                        p.oldIndex = p.level.PosToInt(x, y, z);
                    } catch (Exception e) { 
                        Server.ErrorLog(e); 
                    }
                }
            }
        }        
    }
}