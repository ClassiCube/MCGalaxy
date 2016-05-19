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
    public sealed class CmdPhysics : Command
    {
        public override string name { get { return "physics"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPhysics() { }

        public override void Use(Player p, string message) {
            if (message == "") {
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level l in loaded) {
                    if (l.physics > 0)
                        Player.Message(p, "&5" + l.name + " %Shas physics at &b" + l.physics +
                                           "%S. &cChecks: " + l.lastCheck + "; Updates: " + l.lastUpdate);
                }
                return;
            }
            
            string[] args = message.Split(' ');
            Level level = p != null ? p.level : Server.mainLevel;
            int state = 0, stateIndex = args.Length == 1 ? 0 : 1;
            if (!int.TryParse(args[stateIndex], out state)) {
                Player.Message(p, "Given physics state was not a proper number."); return;
            }
            if (state < 0 || state > 5 ) {
                Player.Message(p, "Given physics state was less than 0, or greater than 5."); return;
            }
            
            if (args.Length == 2) {
                level = LevelInfo.FindOrShowMatches(p, args[0]);
                if (level == null) return;
            }
            
            level.setPhysics(state);
            if (state == 0) level.ClearPhysics();
            string stateDesc = states[state];
            level.ChatLevel("Physics are now " + stateDesc + "%S on &b" + level.name + "%S.");
            
            stateDesc = stateDesc.Substring( 2 );       
            string logInfo = "Physics are now " + stateDesc + " on " + level.name + ".";
            Server.s.Log(logInfo);
            level.changed = true;
        }
        
        internal static string[] states = { "&cOFF", "&aNormal", "&aAdvanced", "&aHardcore", "&aInstant", "&4Doors-only" };

        public override void Help(Player p) {
            Player.Message(p, "%T/physics [map] [0/1/2/3/4/5]");
            Player.Message(p, "%HSets the physics state for the given map.");
            Player.Message(p, "%H  If no map name is given, uses the current map.");
            Player.Message(p, "%H  0 = off, 1 = on, 2 = advanced, 3 = hardcore, 4 = instant, 5 = doors only"); 
        }
    }
}
