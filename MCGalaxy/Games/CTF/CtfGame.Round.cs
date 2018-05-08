/*
    Copyright 2011 MCForge
    
    Written by fenderrock87
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Maths;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    
    public sealed partial class CTFGame : RoundsGame {
        
        protected override void DoRound() {
            if (!running) return;
            RoundInProgress = true;
            
            while (Blue.Points < Config.RoundPoints && Red.Points < Config.RoundPoints) {
                if (!running) return;
                if (!RoundInProgress) break;
                Tick();
            }
            
            if (!running) return;
            EndRound();
            Picker.AddRecentMap(Map.MapName);
            
            // TODO: Move players here
            if (RoundsLeft > 0) {
                string map = Picker.ChooseNextLevel(this);
                // (map != null) ChangeLevel(map);
            }
        }
        
        void Tick() {
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level != Map) continue;
                
                CtfTeam2 team = TeamOf(p);
                if (team == null || Get(p).tagging) continue;
                if (!OnOwnTeamSide(p.Pos.BlockZ, team)) continue;
                CtfTeam2 opposing = Opposing(team);
                
                Player[] opponents = opposing.Members.Items;
                foreach (Player other in opponents) {
                    if (!MovementCheck.InRange(p, other, 2 * 32)) continue;

                    Get(other).tagging = true;
                    Player.Message(other, p.ColoredName + " %Stagged you!");
                    Command.all.FindByName("Spawn").Use(other, "");
                    Thread.Sleep(300);
                    
                    if (Get(other).hasflag) DropFlag(p, opposing);
                    Get(p).Points += Config.Tag_PointsGained;
                    Get(other).Points -= Config.Tag_PointsLost;
                    Get(p).Tags++;
                    Get(other).tagging = false;
                }
            }
        }
        
        public override void EndRound() {
	        if (!RoundInProgress) return;
            RoundInProgress = false;
            
            if (Blue.Points >= Config.RoundPoints || Blue.Points > Red.Points) {
                Chat.MessageLevel(Map, Blue.ColoredName + " %Swon this round of CTF!");
            } else if (Red.Points >= Config.RoundPoints || Red.Points > Blue.Points) {
                Chat.MessageLevel(Map, Red.ColoredName + " %Swon this round of CTF!");
            } else {
                Chat.MessageLevel(Map, "The round ended in a tie!");
            }
            
            Thread.Sleep(4000);
            //MYSQL!
            cache.ForEach(delegate(CtfData d) {
                              d.hasflag = false;
                              Database.Backend.UpdateRows("CTF", "Points=@1, Captures=@2, tags=@3",
                                                          "WHERE Name = @0", d.p.name, d.Points, d.Captures, d.Tags);
                          });
            
            Picker.AddRecentMap(Map.name);
            string nextMap = Picker.ChooseNextLevel(this);
            
            Chat.MessageLevel(Map, "Starting a new game!");
            Blue.Members.Clear();
            Red.Members.Clear();
            Thread.Sleep(2000);
            SetMap(nextMap);
        }
        
        
        /// <summary> Called when the given player takes the opposing team's flag. </summary>
        void TakeFlag(Player p, CtfTeam2 team) {
            CtfTeam2 opposing = Opposing(team);
            Chat.MessageLevel(Map, team.Color + p.DisplayName + " took the " + opposing.ColoredName + " %Steam's FLAG");
            Get(p).hasflag = true;
        }
        
        /// <summary> Called when the given player, while holding opposing team's flag, clicks on their own flag. </summary>
        void ReturnFlag(Player p, CtfTeam2 team) {
            Vec3U16 flagPos = team.FlagPos;
            p.RevertBlock(flagPos.X, flagPos.Y, flagPos.Z);
            p.cancelBlock = true;
            
            CtfData data = Get(p);
            if (data.hasflag) {
                Chat.MessageLevel(Map, team.Color + p.DisplayName + " RETURNED THE FLAG!");
                data.hasflag = false;
                data.Points += Config.Capture_PointsGained;
                data.Captures++;
                
                CtfTeam2 opposing = Opposing(team);
                team.Points++;
                flagPos = opposing.FlagPos;
                Map.Blockchange(flagPos.X, flagPos.Y, flagPos.Z, opposing.FlagBlock);
            } else {
                Player.Message(p, "You cannot take your own flag!");
            }
        }

        /// <summary> Called when the given player drops the opposing team's flag. </summary>
        void DropFlag(Player p, CtfTeam2 team) {
            CtfData data = Get(p);
            if (!data.hasflag) return;
            
            data.hasflag = false;
            Chat.MessageLevel(Map, team.Color + p.DisplayName + " DROPPED THE FLAG!");
            data.Points -= Config.Capture_PointsLost;
            
            CtfTeam2 opposing = Opposing(team);
            Vec3U16 pos = opposing.FlagPos;
            Map.Blockchange(pos.X, pos.Y, pos.Z, opposing.FlagBlock);
        }
    }
}
