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
using System.Threading;
namespace MCGalaxy.Commands.World {
    
    public sealed class CmdLockdown : Command2 {
        public override string name { get { return "Lockdown"; } }
        public override string shortcut { get { return "ld"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("WLock"), new CommandAlias("WUnlock") }; }
        }
        
        public override void Use(Player p, string map, CommandData data) {
            if (map.Length == 0) { Help(p); return; }
            if (!Formatter.ValidMapName(p, map)) return;
            
            map = Matcher.FindMaps(p, map);
            if (map == null) return;

            if (Server.lockdown.Remove(map)) {
                Chat.MessageGlobal("Map " + map + " was unlocked");
                Chat.MessageFromOps(p, "Map " + map + " unlocked by: λNICK");
            } else {
                Server.lockdown.Add(map);
                Chat.MessageGlobal("Map " + map + " was locked");
                Chat.MessageFromOps(p, "Map " + map + " locked by: λNICK");
            }
            Server.lockdown.Save();
        }
        
        public override void Help(Player p) {
            p.Message("&T/Lockdown [level]");
            p.Message("&HPrevents new players from joining that level.");
            p.Message("&HUsing /lockdown again will unlock that level");
        }
    }
}
