/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdQueue : Command2 {
        public override string name { get { return "Queue"; } }
        public override string shortcut { get { return "qz"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length != 2) { Help(p); return; }
            string value = args[1];
            
            if (args[0].CaselessEq("zombie")) {
                ZSGame.Instance.SetQueuedZombie(p, value);
            } else if (args[0].CaselessEq("level")) {
                ZSGame.Instance.SetQueuedLevel(p, value);
            } else {
                Help(p);
            }
        }

        public override void Help(Player p) {
            p.Message("&T/Queue zombie [name]");
            p.Message("&HNext round [name] will be infected/start zombie");
            p.Message("&T/Queue level [level]");
            p.Message("&HNext round [level] will be the level used");
        }
    }
}
