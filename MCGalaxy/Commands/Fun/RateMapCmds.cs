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

namespace MCGalaxy.Commands {   
    public class CmdMapLike : Command {
        public override string name { get { return "maplike"; } }
        public override string shortcut { get { return "like"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie | CommandEnable.Lava; } }
        
        public override void Use(Player p, string message) { RateMap(p, true); }
        
        protected bool RateMap(Player p, bool like) {
            string prefix = like ? "" : "dis";
            if (p == null) { MessageInGameOnly(p); return false; }
            
            if (p.Game.RatedMap) {
            	prefix = p.Game.LikedMap ? "" : "dis";
                Player.Message(p, "You have already {0}liked this map.", prefix); return false; 
            }
            if (CheckIsAuthor(p)) {
                Player.Message(p, "Cannot {0}like this map as you are an author of it.", prefix); return false;
            }
            
            if (like) p.level.Likes++;
            else p.level.Dislikes++;
            p.Game.RatedMap = true;
            p.Game.LikedMap = like;
            Level.SaveSettings(p.level);
            
            prefix = like ? "&a" : "&cdis";
            Player.Message(p, "You have {0}liked %Sthis map.", prefix);
            return true;
        }
        
        protected static bool CheckIsAuthor(Player p) {
            string[] authors = p.level.Authors.Split(',');
            for (int i = 0; i < authors.Length; i++) {
                if (authors[i].CaselessEq(p.name)) return true;
            }
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/maplike");
            Player.Message(p, "%HIncrements the number of times this map has been liked.");
        }
    }
    
    public sealed class CmdMapDislike : CmdMapLike {
        public override string name { get { return "mapdislike"; } }
        public override string shortcut { get { return "dislike"; } }        
        public override void Use(Player p, string message) { RateMap(p, false); }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/mapdislike");
            Player.Message(p, "%HIncrements the number of times this map has been disliked.");
        }
    }
}
