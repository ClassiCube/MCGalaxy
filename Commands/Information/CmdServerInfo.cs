/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System;
namespace MCGalaxy.Commands {
    public sealed class CmdServerInfo : Command {
        public override string name { get { return "sinfo"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdServerInfo() { }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("host"), new CommandAlias("zall") }; }
        }

        public override void Use(Player p, string message) {
            if (message != "") { Help(p); return; }
            
            Player.Message(p, "Server's name: &b" + Server.name + "%S");
            Player.Message(p, "&a" + Player.number + " %Splayers online, &8"
                           + Player.GetBannedCount() + " banned%S players total.");
            Player.Message(p, "&a" + LevelInfo.Loaded.Count + " %Slevels currently loaded. " +
                           "Currency is &3" + Server.moneys + "%S.");
            
            TimeSpan up = DateTime.UtcNow - Server.StartTime;
            Player.Message(p, "Been online for &b" + WhoInfo.Shorten(up, true) +
                           "%S, and is runing &bMCGalaxy &a" + Server.VersionString +
                           "%S (based on &bMCForge %Sand &bMCLawl%S).");
            Command.all.Find("devs").Use(p, "");

            Player.Message(p, "Player positions are updated every &b"
                           + Server.updateTimer.Interval + " %Smilliseconds.");
            string owner = Server.server_owner;
            if (!owner.CaselessEq("Notch"))
                Player.Message(p, "Owner is &3" + owner + ". %SConsole state: &3" + Server.ZallState);
            else
                Player.Message(p, "Console state: &3" + Server.ZallState);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/serverinfo - Displays the server information.");
        }
    }
}
