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
    public sealed class CmdXJail : Command {        
        public override string name { get { return "xjail"; } }
        public override string shortcut { get { return "xj"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool museumUsable { get { return true; } }

        public override void Use(Player p, string message) {
            string xjailMap = Server.xjailLevel;
            if (xjailMap == "(main)") xjailMap = Server.mainLevel.name;
            if (message == "") { Help(p); return; }
            
            Command jail = Command.all.Find("jail");
            if (message == "set") {
                if (!p.level.IsMuseum) {
                    jail.Use(p, "set");
                    Server.xjailLevel = p.level.name;
                    SrvProperties.Save();
                    Player.Message(p, "The xjail map was set from '" + xjailMap + "' to '" + p.level.name + "'");
                } else {
                    Player.Message(p, "You are in a museum!");
                }
                return;
            }
            
            Player pl = PlayerInfo.FindMatches(p, message);
            if (pl == null) return;

            Command spawn = Command.all.Find("spawn");
            Command freeze = Command.all.Find("freeze");
            Command mute = Command.all.Find("mute");
            
            string data = Server.jailed.Find(pl.name);
            if (data == null) {
                if (!pl.muted) mute.Use(p, message);
                if (!pl.frozen) freeze.Use(p, message);
                
                PlayerActions.ChangeMap(pl, xjailMap);
                pl.BlockUntilLoad(10);
                jail.Use(p, message);
                Player.GlobalMessage(pl.ColoredName + " %Swas XJailed!");
            } else {
                if (pl.muted) mute.Use(p, message);
                if (pl.frozen) freeze.Use(p, message);
                
                PlayerActions.ChangeMap(pl, Server.mainLevel.name);
                pl.BlockUntilLoad(10);
                
                jail.Use(p, message);
                spawn.Use(pl, "");
                Player.GlobalMessage(pl.ColoredName + " %Swas released from XJail!");
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/xjail <player>");
            Player.Message(p, "%HMutes, freezes, and sends <player> to the XJail map");
            Player.Message(p, "%HIf <player> is already jailed, <player> will be spawned, unfrozen and unmuted");
            Player.Message(p, "%T/xjail set");
            Player.Message(p, "%HSets the xjail map to your current map and sets jail to current location");
        }
    }
}
