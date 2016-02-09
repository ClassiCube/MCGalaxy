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
namespace MCGalaxy.Commands
{
    public sealed class CmdUnload : Command
    {
        public override string name { get { return "unload"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdUnload() { }

        public override void Use(Player p, string message) {
            string name = message.ToLower();
            if (name == "" && p == null) {
                Player.SendMessage(p, "You must specify a map name when unloading from console."); return;
            }
            
            if (name == "") {
                if (!p.level.Unload())
                    Player.SendMessage(p, "You cannot unload this level.");
            } else if (name == "empty") {
                for (int i = 0; i < Server.levels.Count; i++) {
                    Level l = Server.levels[i];
                    if (l.players.Count <= 0 && l.Unload(true, true))
                        i--;
                }
            } else {
                Level level = LevelInfo.Find(name);
                if (level == null) {
                    Player.SendMessage(p, "There is no level \"" + name + "\" loaded.");
                } else if (!level.Unload()) {
                    Player.SendMessage(p, "You cannot unload this level.");
                }
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/unload [map name]");
            Player.SendMessage(p, "%HUnloads the given map.");
            Player.SendMessage(p, "%H  If map name is \"empty\", unloads all maps with no players in them.");
            Player.SendMessage(p, "%H  If no map name is given, unloads the current map."); 
        }
    }
}
