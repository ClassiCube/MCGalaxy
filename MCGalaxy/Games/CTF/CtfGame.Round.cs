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
using BlockID = System.UInt16;

namespace MCGalaxy.Games {
    
    public sealed partial class CTFGame : RoundsGame {
        
        protected override void DoRound() {
            if (!running) return;
            
            RoundInProgress = true;
            while (Blue.Points < Config.RoundPoints && Red.Points < Config.RoundPoints) {
                if (!running) return;
                if (!RoundInProgress) break;
                Tick();
                Thread.Sleep(300);
            }
            
            if (running) EndRound();
            if (running) VoteAndMoveToNextMap();
        }
        
        void Tick() {
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level != Map) continue;
                CtfTeam team = TeamOf(p);
                CtfData data = Get(p);
                
                // Draw flag above player head
                if (data.HasFlag) DrawPlayerFlag(p, data);
                
                if (team == null || data.TagCooldown) continue;
                if (!OnOwnTeamSide(p.Pos.BlockZ, team)) continue;
                CtfTeam opposing = Opposing(team);
                
                Player[] opponents = opposing.Members.Items;
                foreach (Player other in opponents) {
                    if (!MovementCheck.InRange(p, other, 2 * 32)) continue;
                    CtfData otherData = Get(other);

                    otherData.TagCooldown = true;
                    Player.Message(other, p.ColoredName + " %Stagged you!");
                    Command.Find("Spawn").Use(other, "");
                    Thread.Sleep(300); // TODO: get rid of this
                    
                    if (otherData.HasFlag) DropFlag(p, opposing);
                    data.Points += Config.Tag_PointsGained;
                    otherData.Points -= Config.Tag_PointsLost;
                    data.Tags++;
                    otherData.TagCooldown = false;
                }
            }
        }
        
        void ResetPlayerFlag(Player p, CtfData data) {
            Vec3S32 last = data.LastHeadPos;
            ushort x = (ushort)last.X, y = (ushort)last.Y, z = (ushort)last.Z;
            data.LastHeadPos = default(Vec3S32);
            
            BlockID origBlock = Map.GetBlock(x, y, z);
            if (origBlock != Block.Invalid) {
                Player.GlobalBlockchange(Map, x, y, z, origBlock);
            }
        }
        
        void DrawPlayerFlag(Player p, CtfData data) {
            Vec3S32 coords = p.Pos.BlockCoords; coords.Y += 3;
            if (coords == data.LastHeadPos) return;         
            ResetPlayerFlag(p, data);
            
            data.LastHeadPos = coords;
            ushort x = (ushort)coords.X, y = (ushort)coords.Y, z = (ushort)coords.Z;
            CtfTeam opposing = Opposing(TeamOf(p));
            Player.GlobalBlockchange(Map, x, y, z, opposing.FlagBlock);
        }
        
        public override void EndRound() {
            if (!RoundInProgress) return;
            RoundInProgress = false;
            
            if (Blue.Points > Red.Points) {
                Map.Message(Blue.ColoredName + " %Swon this round of CTF!");
            } else if (Red.Points > Blue.Points) {
                Map.Message(Red.ColoredName + " %Swon this round of CTF!");
            } else {
                Map.Message("The round ended in a tie!");
            }
            
            Blue.Points = 0;
            Red.Points = 0;
            ResetFlagsState();
            
            Thread.Sleep(4000);
            SaveDB();
            Map.Message("Starting next round!");
        }
        
        void SaveDB() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != Map) continue;
                CtfData data = Get(p);
                
                // TODO: Move to MySQL save event
                data.HasFlag = false;
                Database.Backend.UpdateRows("CTF", "Points=@1, Captures=@2, tags=@3",
                                            "WHERE Name = @0", p.name, data.Points, data.Captures, data.Tags);
            }
        }
        
        
        /// <summary> Called when the given player takes the opposing team's flag. </summary>
        void TakeFlag(Player p, CtfTeam team) {
            CtfTeam opposing = Opposing(team);
            Map.Message(team.Color + p.DisplayName + " took the " + opposing.ColoredName + " %Steam's FLAG");
            
            CtfData data = Get(p);
            data.HasFlag = true;
            DrawPlayerFlag(p, data);
        }
        
        /// <summary> Called when the given player, while holding opposing team's flag, clicks on their own flag. </summary>
        void ReturnFlag(Player p, CtfTeam team) {
            Vec3U16 flagPos = team.FlagPos;
            p.RevertBlock(flagPos.X, flagPos.Y, flagPos.Z);
            p.cancelBlock = true;
            
            CtfData data = Get(p);
            if (data.HasFlag) {
                Map.Message(team.Color + p.DisplayName + " RETURNED THE FLAG!");
                data.HasFlag = false;
                ResetPlayerFlag(p, data);
                
                data.Points += Config.Capture_PointsGained;
                data.Captures++;
                team.Points++;
                
                CtfTeam opposing = Opposing(team);
                opposing.RespawnFlag(Map);
            } else {
                Player.Message(p, "You cannot take your own flag!");
            }
        }

        /// <summary> Called when the given player drops the opposing team's flag. </summary>
        void DropFlag(Player p, CtfTeam team) {
            CtfData data = Get(p);
            if (!data.HasFlag) return;
            
            data.HasFlag = false;
            ResetPlayerFlag(p, data);
            
            Map.Message(team.Color + p.DisplayName + " DROPPED THE FLAG!");
            data.Points -= Config.Capture_PointsLost;
            
            CtfTeam opposing = Opposing(team);
            opposing.RespawnFlag(Map);
        }
    }
}
