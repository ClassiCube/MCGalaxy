/*
    Written by Jack1312
        
    Copyright 2011 MCForge
    
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
using System.Collections.Generic;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdPatrol : Command {
        public override string name { get { return "Patrol"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Guest, " and below are patrolled") }; }
        }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }

            List<Player> candidates = GetPatrolCandidates(p);
            if (candidates.Count == 0) {
                LevelPermission perm = CommandExtraPerms.MinPerm(name);
                Player.Message(p, "No {0}players ranked {1} %Sor below are online.",
                               p.Rank <= perm ? "other " : "", // in case we can patrol ourselves
                               Group.GetColoredName(perm));
            } else {
                Player target = candidates[new Random().Next(candidates.Count)];
                Command.all.FindByName("TP").Use(p, target.name);
                Player.Message(p, "Now visiting " + target.ColoredName + "%S.");
            }
        }
        
        List<Player> GetPatrolCandidates(Player p) {
            List<Player> candidates = new List<Player>();
            LevelPermission perm = CommandExtraPerms.MinPerm(name);
            Player[] online = PlayerInfo.Online.Items;
            
            foreach (Player target in online) {
                if (target.Rank > perm || target == p || !Entities.CanSee(p, target)) continue;
                candidates.Add(target);
            }
            return candidates;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Patrol");
            LevelPermission perm = CommandExtraPerms.MinPerm(name);
            Player.Message(p, "%HTeleports you to a random {0} %Sor lower",
                           Group.GetColoredName(perm));
        }
    }
}
