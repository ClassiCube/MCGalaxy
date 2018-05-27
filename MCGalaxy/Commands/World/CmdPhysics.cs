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
namespace MCGalaxy.Commands.World {
    public sealed class CmdPhysics : Command {
        public override string name { get { return "Physics"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("KillPhysics", "kill") }; }
        }

        public override void Use(Player p, string message) {
            if (message.Length == 0) { ShowPhysics(p); return; }
            if (message.CaselessEq("kill")) { KillPhysics(p); return; }
            
            string[] args = message.SplitSpaces();
            Level level = p != null ? p.level : Server.mainLevel;
            int state = 0, stateIndex = args.Length == 1 ? 0 : 1;            
            if (!CommandParser.GetInt(p, args[stateIndex], "Physics state", ref state, 0, 5)) return;
            
            if (args.Length == 2) {
                level = Matcher.FindLevels(p, args[0]);
                if (level == null) return;
            }
            
            if (!LevelInfo.ValidateAction(p, level, "set physics of this level")) return;
            SetPhysics(level, state);
        }
        
        internal static string[] states = new string[] { "&cOFF", "&aNormal", "&aAdvanced", 
            "&aHardcore", "&aInstant", "&4Doors-only" };
        
        void ShowPhysics(Player p) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                if (lvl.physics == 0) continue;
                Player.Message(p, lvl.ColoredName + " %Shas physics at &b" + lvl.physics +
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
        
        internal static void SetPhysics(Level lvl, int state) {
            lvl.SetPhysics(state);
            if (state == 0) lvl.ClearPhysics();
            string stateDesc = states[state];
            lvl.ChatLevel("Physics are now " + stateDesc + " %Son " + lvl.ColoredName);
            
            stateDesc = stateDesc.Substring( 2 );
            string logInfo = "Physics are now " + stateDesc + " on " + lvl.name;
            Logger.Log(LogType.SystemActivity, logInfo);
            lvl.Changed = true;
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/Physics [map] [0/1/2/3/4/5]");
            Player.Message(p, "%HSets the physics state for the given map.");
            Player.Message(p, "%H  If no map name is given, uses the current map.");
            Player.Message(p, "%H  0 = off, 1 = on, 2 = advanced, 3 = hardcore, 4 = instant, 5 = doors only");
            Player.Message(p, "%T/Physics kill %H- Sets physics to 0 on all loaded levels.");
        }
    }
}
