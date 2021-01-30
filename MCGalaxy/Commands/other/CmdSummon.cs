/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands.Misc {
    public sealed class CmdSummon : Command2 {
        public override string name { get { return "Summon"; } }
        public override string shortcut { get { return "s"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Fetch"), new CommandAlias("Bring"), new CommandAlias("BringAll", "all") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "can summon all players") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            
            if (!message.CaselessEq("all")) {
                SummonPlayer(p, message, data);
            } else {
                if (!CheckExtraPerm(p, data, 1)) return;
                Player[] players = PlayerInfo.Online.Items;
                
                foreach (Player target in players) {
                    if (target.level == p.level && target != p && data.Rank > target.Rank) {
                        target.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
                        target.SendPos(Entities.SelfID, p.Pos, p.Rot);
                        target.Message("You were summoned by {0}&S.", target.FormatNick(p));
                    }
                }
                Chat.MessageFromLevel(p, "λNICK &Ssummoned everyone");
            }
        }
        
        static void SummonPlayer(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            bool confirmed = args.Length > 1 && args[1].CaselessEq("confirm");
            
            Player target = PlayerInfo.FindMatches(p, args[0]);
            if (target == null) return;
            if (!CheckRank(p, data, target, "summon", true)) return;
            
            if (p.level != target.level) {
                if (!CheckVisitPerm(p, target, confirmed)) return;
                p.Message("{0} &Sis in a different level, moving them..", p.FormatNick(target));
                
                target.summonedMap = p.level.name;
                PlayerActions.ChangeMap(target, p.level);
                target.summonedMap = null;
                p.BlockUntilLoad(10); // wait for them to load
            }

            if (p.level != target.level) return; // in case they were unable to move to this level
            
            target.AFKCooldown = DateTime.UtcNow.AddSeconds(2);
            target.SendPos(Entities.SelfID, p.Pos, p.Rot);
            target.Message("You were summoned by {0}&S.", target.FormatNick(p));
        }
        
        static bool CheckVisitPerm(Player p, Player target, bool confirmed) {
            AccessResult result = p.level.VisitAccess.Check(target.name, target.Rank);
            if (result == AccessResult.Allowed) return true;
            if (result == AccessResult.Whitelisted) return true;
            if (result == AccessResult.AboveMaxRank && confirmed) return true;
            if (result == AccessResult.BelowMinRank && confirmed) return true;
            
            if (result == AccessResult.Blacklisted) {
                p.Message("{0} &Sis blacklisted from visiting this map.", p.FormatNick(target));
                return false;
            } else if (result == AccessResult.BelowMinRank) {
                p.Message("Only {0}&S+ may normally visit this map. {1}&S is ranked {2}",
                          Group.GetColoredName(p.level.VisitAccess.Min),
                          p.FormatNick(target), target.group.ColoredName);
            } else if (result == AccessResult.AboveMaxRank) {
                p.Message("Only {0}&S and below may normally visit this map. {1}&S is ranked {2}",
                          Group.GetColoredName(p.level.VisitAccess.Max),
                          p.FormatNick(target), target.group.ColoredName);
            }
            
            p.Message("If you still want to summon them, type &T/Summon {0} confirm", target.name);
            return false;
        }
        
        public override void Help(Player p) {
            p.Message("&T/Summon [player]");
            p.Message("&HSummons [player] to your position.");
            p.Message("&T/Summon all");
            p.Message("&HSummons all players in your map");
        }
    }
}
