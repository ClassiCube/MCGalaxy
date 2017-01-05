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

namespace MCGalaxy.Commands {
    public sealed class CmdPatrol : Command {
        public override string name { get { return "patrol"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Guest, " and below are patrolled") }; }
        }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message != "") { Help(p); return; }

            List<string> getpatrol = FindToPatrol();
            if (getpatrol.Count <= 0) {
                Player.Message(p, "There must be at least one guest online to use this command!"); return;
            }

            string value = getpatrol[new Random().Next(getpatrol.Count)];
            Player who = PlayerInfo.FindExact(value);
            Command.all.Find("tp").Use(p, who.name);
            Player.Message(p, "Now visiting " + who.ColoredName + "%S.");
        }
        
        List<string> FindToPatrol() {
            List<string> players = new List<string>();
            int perm = CommandOtherPerms.GetPerm(this);
            Player[] online = PlayerInfo.Online.Items;
            
            foreach (Player p in online) {
                if ((int)p.Rank <= perm) players.Add(p.name);
            }
            return players;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/patrol");
            LevelPermission perm = (LevelPermission)CommandOtherPerms.GetPerm(this);
            Player.Message(p, "%HTeleports you to a random {0} %Sor lower", Group.GetColoredName(perm));
        }
    }
}
