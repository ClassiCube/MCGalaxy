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
using System.IO;
using MCGalaxy.Events;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdJail : Command {
        public override string name { get { return "jail"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdJail() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            
            string reason = args.Length > 1 ? args[1] : "";
            reason = ModActionCmd.ExpandReason(p, reason);
            if (reason == null) return;
            
            if (message.CaselessEq("set") && p != null) {
                p.level.jailx = p.Pos.X; p.level.jaily = p.Pos.Y; p.level.jailz = p.Pos.Z;
                p.level.jailrotx = p.Rot.RotY; p.level.jailroty = p.Rot.HeadX;
                Player.Message(p, "Set Jail point.");
                return;
            }
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            
            if (!who.jailed) {
                if (p != null && who.Rank >= p.Rank) { 
                    MessageTooHighRank(p, "jail", false); return;
                }
            	ModAction action = new ModAction(who.name, p, ModActionType.Jailed, reason);
                OnModActionEvent.Call(action);
            } else {
                ModAction action = new ModAction(who.name, p, ModActionType.Unjailed, reason);
                OnModActionEvent.Call(action);
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/jail [user] <reason>");
            Player.Message(p, "%HPlaces [user] in jail unable to use commands.");
            Player.Message(p, "%HFor <reason>, @number can be used as a shortcut for that rule.");
            Player.Message(p, "%T/jail set");
            Player.Message(p, "%HCreates the jail point for the map.");
            Player.Message(p, "%H  This has been deprecated in favor of /xjail.");
        }
    }
}
