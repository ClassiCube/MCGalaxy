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
namespace MCGalaxy.Commands {
    public sealed class CmdPhysics : Command {
        public override string name { get { return "physics"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("killphysics", "kill") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") { ShowPhysics(p); return; }
            if (message.CaselessEq("kill")) { KillPhysics(p); return; }
            
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
                level = LevelInfo.FindMatches(p, args[0]);
                if (level == null) return;
            }
            SetPhysics(level, state);
        }
        
        internal static string[] states = { "&cOFF", "&aNormal", "&aAdvanced", "&aHardcore", "&aInstant", "&4Doors-only" };
        
        void ShowPhysics(Player p) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                if (lvl.physics == 0) continue;
                Player.Message(p, "&5" + lvl.name + " %Shas physics at &b" + lvl.physics +
                               "%S. &cChecks: " + lvl.lastCheck + "; Updates: " + lvl.lastUpdate);
            }
        }
        
        void KillPhysics(Player p) {
            Level[] levels = LevelInfo.Loaded.Items;
            foreach (Level lvl in levels) {
                if (lvl.physics == 0) continue;
                SetPhysics(lvl, 0);
            }
            Player.Message(p, "Physics killed on all levels.");
        }
        
        static void SetPhysics(Level lvl, int state) {
            lvl.setPhysics(state);
            if (state == 0) lvl.ClearPhysics();
            string stateDesc = states[state];
            lvl.ChatLevel("Physics are now " + stateDesc + "%S on &b" + lvl.name + "%S.");
            
            stateDesc = stateDesc.Substring( 2 );
            string logInfo = "Physics are now " + stateDesc + " on " + lvl.name + ".";
            Server.s.Log(logInfo);
            lvl.changed = true;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/physics [map] [0/1/2/3/4/5]");
            Player.Message(p, "%HSets the physics state for the given map.");
            Player.Message(p, "%H  If no map name is given, uses the current map.");
            Player.Message(p, "%H  0 = off, 1 = on, 2 = advanced, 3 = hardcore, 4 = instant, 5 = doors only");
            Player.Message(p, "%T/physics kill %H- Sets physics to 0 on all loaded levels.");
        }
    }
}
