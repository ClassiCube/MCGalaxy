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
    public sealed class CmdPhysics : Command2 {
        public override string name { get { return "Physics"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandAlias[] Aliases {
            get { return new CommandAlias[] { new CommandAlias("KillPhysics", "kill") }; }
        }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { ShowPhysics(p); return; }
            if (message.CaselessEq("kill")) { KillPhysics(p); return; }
            
            string[] args = message.SplitSpaces();
            Level lvl = p.IsSuper ? Server.mainLevel : p.level;
            
            int state = 0, stateI = args.Length == 1 ? 0 : 1;            
            if (!CommandParser.GetInt(p, args[stateI], "Physics state", ref state, 0, 5)) return;
            
            if (args.Length == 2) {
                lvl = Matcher.FindLevels(p, args[0]);
                if (lvl == null) return;
            }
            
            if (!LevelInfo.Check(p, data.Rank, lvl, "set physics of this level")) return;
            SetPhysics(lvl, state);
        }
        
        internal static string[] states = new string[] { "&cOFF", "&aNormal", "&aAdvanced", 
            "&aHardcore", "&aInstant", "&4Doors-only" };
        
        void ShowPhysics(Player p) {
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded) {
                if (lvl.physics == 0) continue;
                p.Message("{0} &Shas physics at &b{1}&S. &cChecks: {2}; Updates: {3}", 
                               lvl.ColoredName, lvl.physics, lvl.lastCheck, lvl.lastUpdate);
            }
        }
        
        void KillPhysics(Player p) {
            Level[] levels = LevelInfo.Loaded.Items;
            foreach (Level lvl in levels) {
                if (lvl.physics == 0) continue;
                SetPhysics(lvl, 0);
            }
            p.Message("Physics killed on all levels.");
        }
        
        internal static void SetPhysics(Level lvl, int state) {
            lvl.SetPhysics(state);
            if (state == 0) lvl.ClearPhysics();
            string stateDesc = states[state];
            lvl.Message("Physics are now " + stateDesc + " &Son " + lvl.ColoredName);
            
            stateDesc = stateDesc.Substring( 2 );
            string logInfo = "Physics are now " + stateDesc + " on " + lvl.name;
            Logger.Log(LogType.SystemActivity, logInfo);
            lvl.SaveSettings();
        }

        public override void Help(Player p) {
            p.Message("&T/Physics [level] [0/1/2/3/4/5]");
            p.Message("&HSets the physics state for the given level.");
            p.Message("&H  If [level] is not given, uses the current level.");
            p.Message("&H  0 = off, 1 = on, 2 = advanced, 3 = hardcore, 4 = instant, 5 = doors only");
            p.Message("&T/Physics kill &H- Sets physics to 0 on all loaded levels.");
        }
    }
}
