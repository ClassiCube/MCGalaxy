/*
    Copyright 2015 MCGalaxy team
        
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
using MCGalaxy;

namespace MCGalaxy.Core {
    internal static class ConnectHandler {
        
        internal static void HandleConnect(Player p) {
            CheckReviewList(p);
            if (p.group.commands.Contains("reachdistance"))
                LoadReach(p);
            
            LoadWaypoints(p);
            LoadIgnores(p);
            CheckLoginJailed(p);
        }
        
        static void CheckReviewList(Player p) {
            Command cmd = Command.all.Find("review");
            int perm = CommandOtherPerms.GetPerm(cmd, 1);
            
            if ((int)p.group.Permission < perm || !p.group.commands.Contains(cmd)) return;
            int count = Server.reviewlist.Count;
            if (count == 0) return;
            
            string suffix = count == 1 ? " player is " : " players are ";
            p.SendMessage(count + suffix + "waiting for a review. Type %T/review view");
        }
        
        static void LoadReach(Player p) {
            string line = Server.reach.Find(p.name);
            if (line == null) return;
            int space = line.IndexOf(' ');
            if (space == -1) return;
            string reach = line.Substring(space + 1);
            
            short reachDist;
            if (!short.TryParse(reach, out reachDist)) return;
            p.ReachDistance = reachDist / 32f;
            if (p.HasCpeExt(CpeExt.ClickDistance))
                p.Send(Packet.ClickDistance(reachDist));
        }
        
        static void LoadWaypoints(Player p) {
            try {
                p.Waypoints.Load(p);
            } catch (IOException ex) {
                p.SendMessage("Error loading waypoints.");
                Server.ErrorLog(ex);
            }
        }
        
        static void LoadIgnores(Player p) {
            string path = "ranks/ignore/" + p.name + ".txt";
            if (!File.Exists(path)) return;
            
            try {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines) {
                    if (line == "&global") continue; // deprecated /ignore global
                    if (line == "&all") p.ignoreAll = true;
                    else if (line == "&irc") p.ignoreIRC = true;
                    else if (line == "&8ball") p.ignore8ball = true;
                    else if (line == "&titles") p.ignoreTitles = true;
                    else if (line == "&nicks") p.ignoreNicks = true;
                    else p.listignored.Add(line);
                }
            } catch (IOException ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to load ignore list for: " + p.name);
            }
            
            if (p.ignoreAll || p.ignoreIRC || p.ignoreTitles || p.ignoreNicks || p.listignored.Count > 0)
                p.SendMessage("&cType &a/ignore list &cto see who you are still ignoring");
        }
        
        static void CheckLoginJailed(Player p) {
            string line = Server.jailed.Find(p.name);
            if (line == null) return;
            int space = line.IndexOf(' ');
            if (space == -1) return;
            string level = line.Substring(space + 1);
            
            try {
                PlayerActions.ChangeMap(p, level);
                Command.all.Find("jail").Use(null, p.name);
            } catch (Exception ex) {
                p.Leave("Error occured", "Error occured", true);
                Server.ErrorLog(ex);
            }
        }
    }
}
