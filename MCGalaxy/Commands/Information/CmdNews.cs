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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Commands.Info {
    public sealed class CmdNews : Command {
        public override string name { get { return "news"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        
        public override void Use(Player p, string message)
        {
            if (!File.Exists(Paths.NewsFile)) {
                File.WriteAllText(Paths.NewsFile, "News have not been created. Put News in '" + Paths.NewsFile + "'."); return;
            }
            
            string[] lines = File.ReadAllLines(Paths.NewsFile);
            Player.MessageLines(p, lines);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/news");
            Player.Message(p, "%HShows server news.");
        }
    }
}