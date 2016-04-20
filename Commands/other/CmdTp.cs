/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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
 using MCGalaxy.Games;
 
namespace MCGalaxy.Commands {
    
    public sealed class CmdTp : Command {
        
        public override string name { get { return "tp"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length > 1) { Help(p); return; }
            
            Player target = PlayerInfo.FindOrShowMatches(p, message);
            if (target == null) return;
            if (target.level.IsMuseum) {
                Player.SendMessage(p, "Player \"" + message + "\" is in a museum!"); return;
            }
        
            if (!Server.higherranktp && p.group.Permission < target.group.Permission) {
                MessageTooHighRank(p, "teleport to", true); return;
            }
            
            IGame game = target.level.CurrentGame();
            if (!p.Game.Referee && !game.TeleportAllowed) {
                Player.SendMessage(p, "You can only teleport to players who are " +
                                   "playing a game when you are in referee mode."); return;
            }
            
            p.beforeTeleportMap = p.level.name;
            p.beforeTeleportPos = p.pos;
            
            if (p.level != target.level)
                Command.all.Find("goto").Use(p, target.level.name);            
            if (target.Loading) {
                Player.SendMessage(p, "Waiting for " + target.ColoredName + " %Sto spawn...");
                target.BlockUntilLoad(10);
            }
            p.BlockUntilLoad(10);  //Wait for player to spawn in new map
            p.SendPos(0xFF, target.pos[0], target.pos[1], target.pos[2], target.rot[0], 0);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/tp [target] - Teleports yourself to that player.");
            Player.SendMessage(p, "Use /p2p to teleport a given player to a different player.");
        }
    }
}
