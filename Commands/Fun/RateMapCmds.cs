/*
    Copyright 2015 MCGalaxy
    
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
using System.Collections.Generic;
using System.Threading;

namespace MCGalaxy.Commands {
    
    public sealed class CmdMapLike : Command {
        public override string name { get { return "maplike"; } }
        public override string shortcut { get { return "like"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override bool Enabled { get { return Server.zombie.Running || Server.lava.active; } }
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (p.ratedMap) { Player.SendMessage(p, "You have already liked this map."); return; }
            p.level.Likes++;
            p.ratedMap = true;
            Player.SendMessage(p, "You have liked this map.");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/maplike");
            Player.SendMessage(p, "%HIncrements the number of times this map has been liked.");
        }
    }
    
    public sealed class CmdMapDislike : Command {
        public override string name { get { return "mapdislike"; } }
        public override string shortcut { get { return "dislike"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override bool Enabled { get { return Server.zombie.Running || Server.lava.active; } }
        
        public override void Use(Player p, string message) {
            if (p == null) { MessageInGameOnly(p); return; }
            if (p.ratedMap) { Player.SendMessage(p, "You have already disliked this map."); return; }
            p.level.Dislikes++;
            p.ratedMap = true;
            Player.SendMessage(p, "You have disliked this map.");
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/mapdislike");
            Player.SendMessage(p, "%HIncrements the number of times this map has been disliked.");
        }
    }
}
