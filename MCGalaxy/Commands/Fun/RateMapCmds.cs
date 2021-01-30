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

namespace MCGalaxy.Commands.Fun {

    public class CmdLike : Command2 {
        public override string name { get { return "Like"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie | CommandEnable.Lava; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) { RateMap(p, true); }
        
        protected bool RateMap(Player p, bool like) {
            string prefix = like ? "" : "dis";
            
            if (p.Game.RatedMap) {
                prefix = p.Game.LikedMap ? "" : "dis";
                p.Message("You have already {0}liked this map.", prefix); return false; 
            }
            if (CheckIsAuthor(p)) {
                p.Message("Cannot {0}like this map as you are an author of it.", prefix); return false;
            }
            
            if (like) p.level.Config.Likes++;
            else p.level.Config.Dislikes++;
            
            p.Game.RatedMap = true;
            p.Game.LikedMap = like;
            p.level.SaveSettings();
            
            prefix = like ? "&a" : "&cdis";
            p.Message("You have {0}liked &Sthis map.", prefix);
            return true;
        }
        
        protected static bool CheckIsAuthor(Player p) {
            string[] authors = p.level.Config.Authors.SplitComma();
            return authors.CaselessContains(p.name);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Like");
            p.Message("&HIncrements the number of times this map has been liked.");
        }
    }
    
    public sealed class CmdDislike : CmdLike {
        public override string name { get { return "Dislike"; } }        
        public override void Use(Player p, string message, CommandData data) { RateMap(p, false); }
        
        public override void Help(Player p) {
            p.Message("&T/Dislike");
            p.Message("&HIncrements the number of times this map has been disliked.");
        }
    }
}
