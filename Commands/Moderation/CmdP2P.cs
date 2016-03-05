/*
 * Written by Jack1312
 * 
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdP2P : Command {
        
        public override string name { get { return "p2p"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            if (args.Length > 2) { Help(p); return; }
            if (args.Length == 1) { Player.SendMessage(p, "You did not specify the target player."); return; }            
            Player source = PlayerInfo.Find(args[0]), target = PlayerInfo.Find(args[1]);
            
            if ((source == null || !Player.CanSee(p, source)) && (target == null || !Player.CanSee(p, target))) {
                Player.SendMessage(p, "Neither of the players specified are online."); return; 
            }
            if (source == null || !Player.CanSee(p, source)) { Player.SendMessage(p, "The source player is not online."); return; }
            if (target == null || !Player.CanSee(p, target)) { Player.SendMessage(p, "The target player is not online."); return; }
            
            if (p.group.Permission < source.group.Permission) {
                Player.SendMessage(p, "You cannot force a player of higher rank to tp to another player."); return;
            }
            Player.SendMessage(p, "Attempting to teleport " + source.name + " to " + target.name + ".");
            Command.all.Find("tp").Use(source, target.name);            
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/p2p [source] [target] - Teleports the source player to the target player.");
        }
    }
}
