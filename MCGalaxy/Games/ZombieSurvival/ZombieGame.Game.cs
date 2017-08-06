/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Games.ZS;

namespace MCGalaxy.Games {
    
    public sealed partial class ZSGame : IGame {
        
        public LevelPicker Picker = new ZSLevelPicker();
        /// <summary> Whether players are allowed to teleport to others when not in referee mode. </summary>
        public override bool TeleportAllowed { get { return !RoundInProgress; } }
        
        public override void PlayerLeftGame(Player p) {
            Alive.Remove(p);
            Infected.Remove(p);
            p.Game.Infected = false;
            RemoveBounties(p);
            
            AssignFirstZombie();
            HUD.UpdateAllPrimary(this);
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || (p.level == null || !p.level.name.CaselessEq(MapName))) return false;
            if (Server.votingforlevel && HandleVote(p, message)) return true;
            
            if (message[0] == '~' && message.Length > 1) {
                Player[] players = PlayerInfo.Online.Items;
                string type = p.Game.Infected ? " &cto zombies%S: " : " &ato humans%S: ";
                
                foreach (Player pl in players) {
                    if (!pl.level.name.CaselessEq(MapName)) continue;
                    if (pl.Game.Referee || pl.Game.Infected == p.Game.Infected)
                        pl.SendMessage(p.ColoredName + type + message.Substring(1));
                }
                return true;
            } else if (message[0] == '`' && message.Length > 1) {
                if (p.Game.Team == null) {
                    Player.Message(p, "You are not on a team, so cannot send a team message."); return true;
                }
                p.Game.Team.Chat(p, message.Substring(1));
                return true;
            }
            return false;
        }
        
        bool HandleVote(Player p, string message) {
            message = message.ToLower();
            if (Player.CheckVote(message, p, "1", "one",   ref Picker.Votes1) ||
                Player.CheckVote(message, p, "2", "two",   ref Picker.Votes2) ||
                Player.CheckVote(message, p, "3", "three", ref Picker.Votes3))
                return true;
            return false;
        }
        
        internal void RemoveBounties(Player p) {
            BountyData[] bounties = Bounties.Items;
            foreach (BountyData b in bounties) {
                if (!(b.Origin.CaselessEq(p.name) || b.Target.CaselessEq(p.name))) continue;
                
                string target = PlayerInfo.GetColoredName(p, b.Target);
                Map.ChatLevel("Bounty on " + target + " %Sis no longer active.");
                Bounties.Remove(b);
                
                Player setter = PlayerInfo.FindExact(b.Origin);
                if (setter != null) setter.SetMoney(setter.money + b.Amount);
            }
        }
        
        public override void PlayerJoinedLevel(Player p, Level lvl, Level oldLvl) {
            p.SendCpeMessage(CpeMessageType.BottomRight3, "");
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            p.SendCpeMessage(CpeMessageType.BottomRight1, "");
            if (RoundInProgress && lvl.name.CaselessEq(MapName)) {
                if (Running && p != null) {
                    Player.Message(p, "You joined in the middle of a round. &cYou are now infected!");
                    p.Game.BlocksLeft = 25;
                    InfectPlayer(p, null);
                }
            }
            if (RoundInProgress && oldLvl == Map) {
                PlayerLeftGame(p);
            }
            
            if (lvl.name.CaselessEq(MapName)) {
                double startLeft = (RoundStart - DateTime.UtcNow).TotalSeconds;
                if (startLeft >= 0)
                    Player.Message(p, "%a" + (int)startLeft + " %Sseconds left until the round starts. %aRun!");
                Player.Message(p, "This map has &a" + Map.Config.Likes +
                              " likes %Sand &c" + Map.Config.Dislikes + " dislikes");
                Player.Message(p, "This map's win chance is &a" + Map.WinChance + "%S%");
                
                if (Map.Config.Authors.Length > 0) {
                    string[] authors = Map.Config.Authors.Replace(" ", "").Split(',');
                    Player.Message(p, "It was created by {0}",
                                   authors.Join(n => PlayerInfo.GetColoredName(p, n)));
                }

                HUD.UpdatePrimary(this, p);
                HUD.UpdateSecondary(this, p);
                HUD.UpdateTertiary(p);
                
                if (Server.votingforlevel) Picker.SendVoteMessage(p);
                return;
            }

            p.SetPrefix();
            HUD.Reset(p);
            Alive.Remove(p);
            Infected.Remove(p);
            if (oldLvl != null && oldLvl.name.CaselessEq(MapName))
                HUD.UpdateAllPrimary(this);
        }
        
        public override void OnHeartbeat(ref string name) {
            if (!Running || !ZSConfig.IncludeMapInHeartbeat || MapName == null) return;
            name += " (map: " + MapName + ")";
        }
        
        public override void AdjustPrefix(Player p, ref string prefix) {
            int winStreak = p.Game.CurrentRoundsSurvived;
            
            if (winStreak == 1) prefix += "&4*" + p.color;
            else if (winStreak == 2) prefix += "&7*"+ p.color;
            else if (winStreak == 3) prefix += "&6*"+ p.color;
            else if (winStreak > 0) prefix += "&6" + winStreak + p.color;
        }
    }
}
