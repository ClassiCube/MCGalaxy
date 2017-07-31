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
using MCGalaxy.Commands;
using MCGalaxy.Network;
using MCGalaxy.Events;

namespace MCGalaxy.Core {
    internal static class ConnectHandler {
        
        internal static void HandleConnect(Player p) {
            CheckReviewList(p);
            if (p.group.CanExecute("reachdistance"))
                LoadReach(p);
            
            LoadWaypoints(p);
            LoadIgnores(p);
            CheckLoginJailed(p);
        }
        
        static void CheckReviewList(Player p) {
            Command cmd = Command.all.FindByName("review");
            LevelPermission perm = CommandExtraPerms.MinPerm("review");
            
            if (p.group.Permission < perm || !p.group.CanExecute(cmd)) return;
            int count = Server.reviewlist.Count;
            if (count == 0) return;
            
            string suffix = count == 1 ? " player is " : " players are ";
            Player.Message(p, count + suffix + "waiting for a review. Type %T/Review view");
        }
        
        static void LoadReach(Player p) {
            string reach = Server.reach.FindData(p.name);
            if (reach == null) return;
            
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
                Player.Message(p, "Error loading waypoints.");
                Logger.LogError(ex);
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
                    else if (line == "&drawoutput") p.ignoreDrawOutput = true;
                    else if (line == "&titles") p.ignoreTitles = true;
                    else if (line == "&nicks") p.ignoreNicks = true;
                    else p.listignored.Add(line);
                }
            } catch (IOException ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "Failed to load ignore list for: " + p.name);
            }
            
            if (p.ignoreAll || p.ignoreIRC || p.ignore8ball || p.ignoreDrawOutput
                || p.ignoreTitles || p.ignoreNicks || p.listignored.Count > 0) {
                Player.Message(p, "&cType &a/ignore list &cto see who you are still ignoring");
            }
        }
        
        static void CheckLoginJailed(Player p) {
            string level = Server.jailed.FindData(p.name);
            if (level == null) return;
            
            try {
                PlayerActions.ChangeMap(p, level);
                ModAction action = new ModAction(p.name, null, ModActionType.Jailed, "Auto jail");
                action.Announce = false;
                OnModActionEvent.Call(action);
                Chat.MessageGlobal(p, p.DisplayName + " &cis still jailed from previously.", false);
            } catch (Exception ex) {
                p.Leave("Error occured", "Error occured", true);
                Logger.LogError(ex);
            }
        }
    }
}
