/*
    Copyright 2011 MCGalaxy
        
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
namespace MCGalaxy.Commands
{
    public sealed class CmdKill : Command
    {
        public override string name { get { return "kill"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdKill() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }         
            bool explode = false;
            string deathMessage;
            string killer = p == null ? "(console)" : p.color + p.DisplayName;
            string[] args = message.Split(trimChars, 2);
            
            Player who = PlayerInfo.Find(args[0]);
            if (args.Length >= 2) {
                if(args[1].ToLower() == "explode") {
                    deathMessage = " was exploded by " + killer;
                    explode = true;
                } else {
                    deathMessage = " " + args[1];
                }
            } else {
                deathMessage = " was killed by " + killer;
            }           

            if (who == null) {
                if (p != null)
                    p.HandleDeath(Block.rock, " killed themselves in their confusion");
                Player.SendMessage(p, "Could not find player"); return;
            }

            if (p != null && who.group.Permission > p.group.Permission) {
                p.HandleDeath(Block.rock, " was killed by " + who.color + who.DisplayName);
                Player.SendMessage(p, "Cannot kill someone of higher rank"); return;
            }
            who.HandleDeath(Block.rock, deathMessage, explode);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/kill <name> [explode] <message>");
            Player.SendMessage(p, "Kills <name> with <message>. Causes explosion if [explode] is written");
        }
    }
}
