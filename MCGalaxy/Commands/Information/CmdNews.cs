/*
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
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdNews : Command2 {
        public override string name { get { return "News"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool UseableWhenFrozen { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            TextFile newsText = TextFile.Files["News"];
            newsText.EnsureExists();
            
            string[] lines = newsText.GetText();
            p.MessageLines(lines);
        }
        
        public override void Help(Player p) {
            p.Message("&T/News");
            p.Message("&HShows server news.");
        }
    }
}