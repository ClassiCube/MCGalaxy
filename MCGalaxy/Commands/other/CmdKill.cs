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
namespace MCGalaxy.Commands {
    public sealed class CmdKill : Command {
        public override string name { get { return "kill"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdKill() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            if (!MessageCmd.CanSpeak(p, name)) return; // do not allow using kill to spam every 2 secs
            
            string[] args = message.SplitSpaces(2);
            Player target = PlayerInfo.FindMatches(p, args[0]);
            
            if (target == null) {
                if (p != null) p.HandleDeath(Block.rock, 0, " killed themselves in their confusion");
                return;
            }
            if (p != null && (target != p && target.Rank >= p.Rank)) {
                MessageTooHighRank(p, "kill", false); return;
            }
            
            bool explode = false;
            string killer = p == null ? "(console)" : p.ColoredName;            
            string deathMsg = GetDeathMessage(args, killer, ref explode);
            target.HandleDeath(Block.rock, 0, deathMsg, explode);
        }
        
        static string GetDeathMessage(string[] args, string killer, ref bool explode) {
            if (args.Length < 2) return " was killed by " + killer;
            
            if (args[1].CaselessEq("explode")) {
                explode = true;
                return " was exploded by " + killer;
            }
            return " " + args[1];
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/kill [name] <message>");
            Player.Message(p, "%HKills [name], with <message> if given.");
            Player.Message(p, "%HCauses an explosion if \"explode\" is used for <message>");
        }
    }
}
