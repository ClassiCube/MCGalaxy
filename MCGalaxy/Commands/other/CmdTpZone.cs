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
 
namespace MCGalaxy.Commands {
    public sealed class CmdTpZone : Command {
        public override string name { get { return "tpzone"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdTpZone() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args[0] == "" || args[0].CaselessEq("list")) {
                string modifier = args.Length > 1 ? args[1] : "";
                MultiPageOutput.Output(p, p.level.ZoneList, FormatZone,
                                       "tpzone list", "zones", modifier, true);
            } else {
                int id;
                if (!int.TryParse(message, out id)) { Help(p); return; }
                if (id <= 0 || id > p.level.ZoneList.Count) {
                    Player.Message(p, "This zone doesn't exist"); return;
                }

                Level.Zone zone = p.level.ZoneList[id - 1];
                p.SendOwnFeetPos((ushort)(zone.bigX * 32 + 16), (ushort)(zone.bigY * 32 + 32),
                                 (ushort)(zone.bigZ * 32 + 16), p.rot[0], p.rot[1]);

                Player.Message(p, "Teleported to zone &c" + id + " &b(" +
                               zone.bigX + ", " + zone.bigY + ", " + zone.bigZ + ") &f" +
                               zone.Owner);
            }
        }
        
        static string FormatZone(Level.Zone zone, int i) {
        	return "&c" + (i + 1) + " &b(" +
                zone.smallX + "-" + zone.bigX + ", " +
                zone.smallY + "-" + zone.bigY + ", " +
                zone.smallZ + "-" + zone.bigZ + ") &f" + zone.Owner;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/tpzone [id] %H- Teleports to the zone with ID of [id]");
            Player.Message(p, "%T/tpzone list %H- Lists all zones");
        }
    }
}
