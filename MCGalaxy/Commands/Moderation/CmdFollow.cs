/*
    Copyright 2011 MCForge
    
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

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdFollow : Command {
        public override string name { get { return "follow"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (!p.canBuild) { Player.Message(p, "You're currently being &4possessed%S!"); return; }
            string[] args = message.SplitSpaces(2);
            string name = args[0];
            
            bool stealth = false;
            if (message == "#") {
                if (p.following != "") { stealth = true; name = ""; }
                else { Help(p); return; }
            } else if (args.Length > 1 && args[0] == "#") {
                if (p.hidden) stealth = true;
                name = args[1];
            }
            
            if (name.Length == 0 && p.following.Length == 0) { Help(p); return; }
            if (name.CaselessEq(p.following) || (name.Length == 0 && p.following != ""))
                Unfollow(p, stealth);
            else
                Follow(p, name, stealth);
        }
        
        static void Unfollow(Player p, bool stealth) {
            Player who = PlayerInfo.FindExact(p.following);
            Player.Message(p, "Stopped following ", who == null ? p.following : who.ColoredName);
            p.following = "";
            if (who != null) Entities.Spawn(p, who);
            
            if (!p.hidden) return;
            if (!stealth) {
                Command.all.Find("hide").Use(p, "");
            } else {
                Player.Message(p, "You are still hidden.");
            }
        }
        
        static void Follow(Player p, string name, bool stealth) {
            Player who = PlayerInfo.FindMatches(p, name);
            if (who == null) return;
            if (who == p) { Player.Message(p, "Cannot follow yourself."); return; }
            if (who.Rank >= p.Rank) { MessageTooHighRank(p, "follow", false); return;}
            if (who.following != "") { Player.Message(p, who.DisplayName + " is already following " + who.following); return; }

            if (!p.hidden) Command.all.Find("hide").Use(p, "");

            if (p.level != who.level) Command.all.Find("tp").Use(p, who.name);
            if (p.following != "") {
                Player old = PlayerInfo.FindExact(p.following);
                if (old != null) Entities.Spawn(p, old);
            }
            
            p.following = who.name;
            Player.Message(p, "Following " + who.ColoredName + "%S. Use \"/follow\" to stop.");
            Entities.Despawn(p, who);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/follow [name]");
            Player.Message(p, "%HFollows <name> until the command is cancelled");
            Player.Message(p, "%T/follow # [name]");
            Player.Message(p, "%HWill cause /hide not to be toggled");
        }
    }
}
