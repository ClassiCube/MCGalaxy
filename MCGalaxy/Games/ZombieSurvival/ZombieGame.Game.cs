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

namespace MCGalaxy.Games {
    
    public sealed partial class ZombieGame : IGame {
        
        /// <summary> Whether players are allowed to teleport to others when not in referee mode. </summary>
        public override bool TeleportAllowed { get { return !RoundInProgress; } }
        
        public override bool HandlesManualChange(Player p, ushort x, ushort y, ushort z,
                                                 byte action, byte block, byte old) {
            if (!Running || (p.level == null || !p.level.name.CaselessEq(CurLevelName))) return false;
            if (CurLevel.BuildType == BuildType.NoModify) {
                p.RevertBlock(x, y, z); return true;
            }
            if (CurLevel.BuildType == BuildType.ModifyOnly && Block.Props[old].OPBlock) {
                p.RevertBlock(x, y, z); return true;
            }
            
            if (action == 1 && !CurLevel.Pillaring && !p.Game.Referee) {
                if (NotPillaring(block, old)) {
                    p.Game.BlocksStacked = 0;
                } else if (p.Game.LastY == y - 1 && p.Game.LastX == x && p.Game.LastZ == z ) {
                    p.Game.BlocksStacked++;
                } else {
                    p.Game.BlocksStacked = 0;
                }
                
                if (MessagePillaring(p)) return true;
            }
            p.Game.LastX = x; p.Game.LastY = y; p.Game.LastZ = z;
            
            if (action == 1 || (action == 0 && p.painting)) {
                if (!p.level.name.CaselessEq(CurLevelName) || p.Game.Referee) return false;
                
                if (p.Game.BlocksLeft == 0) {
                    p.SendMessage("You have no blocks left.");
                    p.RevertBlock(x, y, z); return true;
                }

                p.Game.BlocksLeft--;
                if ((p.Game.BlocksLeft % 10) == 0 || (p.Game.BlocksLeft >= 0 && p.Game.BlocksLeft <= 10))
                    p.SendMessage("Blocks Left: &4" + p.Game.BlocksLeft);
            }
            return false;
        }
        
        static bool MessagePillaring(Player p) {
            if (p.Game.BlocksStacked == 2) {
                TimeSpan delta = DateTime.UtcNow - p.Game.LastPillarWarn;
                if (delta.TotalSeconds >= 5) {
                    Chat.MessageOps(  "&cWarning: " + p.ColoredName + " %Sis pillaring!");
                    p.Game.LastPillarWarn = DateTime.UtcNow;
                }
                
                string action = p.Game.PillarFined ? "kicked" : "fined 10 " + Server.moneys;
                p.SendMessage("You are pillaring! &cStop before you are " + action + "!");
            } else if (p.Game.BlocksStacked == 4) {
                if (!p.Game.PillarFined) {
                    Chat.MessageOps("  &cWarning: " + p.ColoredName + " %Sis pillaring!");
                    Command.all.Find("take").Use(null, p.name + " 10 Auto fine for pillaring");
                    p.SendMessage("  &cThe next time you pillar, you will be &4kicked&c.");
                } else {
                    p.Kick("No pillaring allowed!");
                    Player.AddNote(p.name, null, "K", "Auto kick for pillaring");
                    return true;
                }
        		
        	    p.Game.PillarFined = true;
                p.Game.BlocksStacked = 0;
            }
            return false;
        }
        
        static bool NotPillaring(byte block, byte old) {
            if (block == Block.shrub) return true;
            if (block >= Block.yellowflower && block <= Block.redmushroom) return true;
            
            old = Block.Convert(old);
            return old >= Block.water && block <= Block.lavastill;
        }
        
        public override bool HandlesMovement(Player p, ushort x, ushort y, ushort z,
                                             byte rotX, byte rotY) {
            if (!Running || (p.level == null || !p.level.name.CaselessEq(CurLevelName))) return false;
            if (!p.Game.Referee && noRespawn) {
                if (Math.Abs(p.pos[0] - x) >= MaxMoveDistance || Math.Abs(p.pos[2] - z) >= MaxMoveDistance) {
                    p.SendPos(0xFF, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1]);
                    return true;
                }
            }
            return false;
        }
        
        public override bool HandlesChatMessage(Player p, string message) {
            if (!Running || (p.level == null || !p.level.name.CaselessEq(CurLevelName))) return false;
            if (Server.votingforlevel && HandleVote(p, message)) return true;
            
            if (message[0] == '~' && message.Length > 1) {
                Player[] players = PlayerInfo.Online.Items;
                string type = p.Game.Infected ? " &cto zombies%S: " : " &ato humans%S: ";
                
                foreach (Player pl in players) {
                    if (!pl.level.name.CaselessEq(CurLevelName)) continue;
                    if (pl.Game.Referee || pl.Game.Infected == p.Game.Infected)
                        pl.SendMessage(p.ColoredName + type + message.Substring(1));
                }
                return true;
            } else if (message[0] == '`' && message.Length > 1) {
                if (p.Game.Team == null) {
                    p.SendMessage("You are not on a team, so cannot send a team message."); return true;
                }
                p.Game.Team.Chat(p, message.Substring(1));
                return true;
            }
            return false;
        }
        
        bool HandleVote(Player p, string message) {
            message = message.ToLower();
            if (Player.CheckVote(message, p, "1", "one", ref Level1Vote) ||
                Player.CheckVote(message, p, "2", "two", ref Level2Vote) ||
                Player.CheckVote(message, p, "3", "three", ref Level3Vote))
                return true;
            return false;
        }
        
        public override void PlayerLeftServer(Player p) {
            Alive.Remove(p);
            Infected.Remove(p);
            p.Game.Infected = false;
            AssignFirstZombie();
            UpdateAllPlayerStatus();
        }
        
        public override void PlayerJoinedServer(Player p) {
            if (!Running || ZombieGame.SetMainLevel) return;
            Player.Message(p, "Zombie Survival is running! Type %T/g " + CurLevelName + " %Sto join.");
        }
        
        public override void PlayerJoinedLevel(Player p, Level lvl, Level oldLvl) {
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            p.SendCpeMessage(CpeMessageType.BottomRight1, "");
            if (RoundInProgress && lvl.name.CaselessEq(CurLevelName)) {
                if (Running && p != null) {
                    p.SendMessage("You joined in the middle of a round. &cYou are now infected!");
                    p.Game.BlocksLeft = 50;
                    InfectPlayer(p);
                }
            }
            
            if (lvl.name.CaselessEq(CurLevelName)) {
                double startLeft = (RoundStart - DateTime.UtcNow).TotalSeconds;
                if (startLeft >= 0)
                    p.SendMessage("%a" + (int)startLeft + " %Sseconds left until the round starts. %aRun!");
                p.SendMessage("This map has &a" + CurLevel.Likes +
                              " likes %Sand &c" + CurLevel.Dislikes + " dislikes");
                p.SendMessage("This map's win chance is &a" + CurLevel.WinChance + "%S%");
                p.SendCpeMessage(CpeMessageType.Status2,
                                 "%SPillaring " + (CurLevel.Pillaring ? "&aYes" : "&cNo") +
                                 "%S, Type is &a" + CurLevel.BuildType);
                
                if (CurLevel.Authors != "") {
                    string[] authors = CurLevel.Authors.Replace(" ", "").Split(',');
                    Player.Message(p, "It was created by {0}",
                                   authors.Join(n => PlayerInfo.GetColoredName(p, n)));
                }
                PlayerMoneyChanged(p);
                UpdatePlayerStatus(p);
                
                if (Server.votingforlevel)
                    SendVoteMessage(p, lastLevel1, lastLevel2);
                return;
            }

            p.SetPrefix();
            ResetCpeMessages(p);
            Alive.Remove(p);
            Infected.Remove(p);
            if (oldLvl != null && oldLvl.name.CaselessEq(CurLevelName))
                UpdateAllPlayerStatus();
        }
        
        void ResetCpeMessages(Player p) {
            p.SendCpeMessage(CpeMessageType.Status1, "");
            p.SendCpeMessage(CpeMessageType.Status2, "");
            p.SendCpeMessage(CpeMessageType.Status3, "");
            p.SendCpeMessage(CpeMessageType.BottomRight1, "");
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
        }
        
        public override bool PlayerCanJoinLevel(Player p, Level lvl, Level oldLvl) {
            if (!oldLvl.name.CaselessEq(CurLevelName)) return true;
            if (lvl.name.CaselessEq(CurLevelName)) return true;
            
            if (RoundInProgress && !p.Game.Referee) {
                p.SendMessage("Sorry, you cannot leave a zombie survival map until the current round has ended.");
                return false;
            }
            return true;
        }
        
        public override void PlayerMoneyChanged(Player p) {
            if (!Running || !p.level.name.CaselessEq(CurLevelName)) return;
            string moneyMsg = "&a" + p.money + " %S" + Server.moneys;
            string stateMsg = ", you are " + (p.Game.Infected ? "&cdead" : "&aalive");
            p.SendCpeMessage(CpeMessageType.Status3, moneyMsg + stateMsg);
        }
        
        public override void OnHeartbeat(ref string name) {
            if (!Running || !IncludeMapInHeartbeat || CurLevelName == null) return;
            name += " (" + CurLevelName + ")";
        }
        
        public override void AdjustPrefix(Player p, ref string prefix) {
            int winStreak = p.Game.CurrentRoundsSurvived;
            
            if (winStreak == 1) prefix += "&4*" + p.color;
            else if (winStreak == 2) prefix += "&7*"+ p.color;
            else if (winStreak == 3) prefix += "&6*"+ p.color;
            else if (winStreak > 0) prefix += "&6" + winStreak + p.color;
        }
        
        public override void GetTabName(Player p, Player dst,
                                        ref string name, ref string group) {
            if (p.Game.Referee) {
                group = "&2Referees";
            } else if (p.Game.Infected) {
                group = "&cZombies";
                if (ZombieGame.ZombieName != "" && !dst.Game.Aka) {
                    name = "&c" + ZombieGame.ZombieName;
                } else {
                    name = "&c" + p.truename;
                }
            } else {
                group = "&fHumans";
            }
        }
    }
}
