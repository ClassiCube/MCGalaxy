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

namespace MCGalaxy.Commands {
    public sealed class CmdSummon : Command {
        public override string name { get { return "summon"; } }
        public override string shortcut { get { return "s"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("fetch"), new CommandAlias("bring") }; }
        }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can summon all players") }; }
        }
        public CmdSummon() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            
            if (message.CaselessEq("all")) {
                if (CheckExtraPerm(p)) SummonAll(p);
            } else {
                SummonPlayer(p, message);
            }
        }
        
        static void SummonAll(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == p.level && pl != p && p.Rank > pl.Rank) {
                    pl.SendPos(Entities.SelfID, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
                    pl.SendMessage("You were summoned by " + p.ColoredName + "%S.");
                }
            }
            Chat.MessageLevel(p.level, p.ColoredName + " %Ssummoned everyone");
        }
        
        static void SummonPlayer(Player p, string message) {
            string[] args = message.Split(' ');
            bool confirmed = args.Length > 1 && args[1].CaselessEq("confirm");
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            if (p.Rank < who.Rank) {
                MessageTooHighRank(p, "summon", true); return;
            }
            
            if (p.level != who.level) {
                if (!CheckVisitPerm(p, who, confirmed)) return;
                Player.Message(p, who.ColoredName + " %Sis in a different level, moving them..");
                PlayerActions.ChangeMap(who, p.level, confirmed);
                p.BlockUntilLoad(10); // wait for them to load
            }

            if (p.level != who.level) return; // in case they were unable to move to this level
            who.SendPos(Entities.SelfID, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0);
            who.SendMessage("You were summoned by " + p.ColoredName + "%S.");
        }
        
        static bool CheckVisitPerm(Player p, Player who, bool confirmed) {
            LevelAccessResult result = p.level.VisitAccess.Check(who, confirmed);
            if (result == LevelAccessResult.Allowed) return true;
            if (result == LevelAccessResult.Whitelisted) return true;
            
            if (result == LevelAccessResult.Blacklisted) {
                Player.Message(p, "{0} %Sis blacklisted from visiting this map.", who.ColoredName);
                return false;
            } else if (result == LevelAccessResult.BelowMinRank) {
                Player.Message(p, "Only {0}%S+ may normally visit this map. {1}%S is ranked {2}",
                               Group.GetColoredName(p.level.permissionvisit),
                               who.ColoredName, who.group.ColoredName);
            } else if (result == LevelAccessResult.AboveMaxRank) {
                Player.Message(p, "Only {0}%S and below may normally visit this map. {1}%S is ranked {2}",
                               Group.GetColoredName(p.level.pervisitmax),
                               who.ColoredName, who.group.ColoredName);
            }
            
            Player.Message(p, "If you still want to summon them, type %T/summon {0} confirm", who.name);
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/summon [player]");
            Player.Message(p, "%HSummons [player] to your position.");
            Player.Message(p, "%T/summon all");
            Player.Message(p, "%HSummons all players in your map");
        }
    }
}
