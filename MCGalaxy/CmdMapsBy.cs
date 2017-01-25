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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCGalaxy.Commands {
    
    public sealed class CmdMapsBy : Command {
        public override string name { get { return "mapsby"; } }
        public override string shortcut { get { return "madeby"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string author = PlayerInfo.FindOfflineNameMatches(p, message);
            if (author == null) return;
            
            string[] maps = Directory.GetFiles("levels", "*.lvl");
            for (int i = 0; i < maps.Length; i++) {
                maps[i] = Path.GetFileNameWithoutExtension(maps[i]);
            }
            
            List<string> madeBy = new List<string>();
            foreach (string map in maps) {
                if (!IsOwnedBy(map, author)) continue;
                madeBy.Add(map);
            }
            
            author = PlayerInfo.GetColoredName(p, author);
            if (madeBy.Count == 0) {
                Player.Message(p, "{0} %Shas not made any maps", author);
            } else {
                Player.Message(p, "{0} %Sauthored these maps: {1}", author, madeBy.Join());
            }
        }
        
        static bool IsOwnedBy(string map, string name) {
            if (map.CaselessStarts(name)) return true;
            string file = LevelInfo.FindPropertiesFile(map);
            if (file == null) return false;
            
            string realmOwner = null;
            PropertiesFile.Read(file, ref realmOwner, ProcessLine);
            if (realmOwner == null) return false;
            
            string[] owners = realmOwner.Replace(" ", "").Split(',');
            foreach (string owner in owners) {
                if (owner.CaselessEq(name)) return true;
            }
            return false;
        }
        
        static void ProcessLine(string key, string value, ref string realmOwner) {
            if (key.CaselessEq("realmowner")) {
                realmOwner = value;
            }
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/mapsby %H[player]");
            Player.Message(p, "%HLists all maps authored by the given player.");
        }
    }
}
