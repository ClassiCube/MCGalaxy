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
namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdLimit : Command {        
        public override string name { get { return "limit"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message == "") { Help(p); return; }
            bool hasLimit = args.Length > 1;
            
            if (args[0].CaselessEq("rt") || args[0].CaselessEq("reloadthreshold")) {
                float threshold = 0;
                if (hasLimit && !CommandParser.GetReal(p, args[1], "Limit", ref threshold, 0, 100)) return;
                
                SetLimitPercent(p, ref ServerConfig.DrawReloadThreshold, threshold, hasLimit);
                return;
            }
            
            int limit = 0;
            if (hasLimit && !CommandParser.GetInt(p, args[1], "Limit", ref limit, 1)) return;
            
            switch (args[0].ToLower()) {
                case "rp":
                case "restartphysics":
                    SetLimit(p, "Custom /rp limit", ref ServerConfig.PhysicsRestartLimit, limit, hasLimit);
                    return;
                case "rpnormal":
                    SetLimit(p, "Normal /rp limit", ref ServerConfig.PhysicsRestartNormLimit, limit, hasLimit);
                    return;
                case "pu":
                case "physicsundo":
                    SetLimit(p, "Physics undo max entries", ref ServerConfig.PhysicsUndo, limit, hasLimit);
                    return;
            }

            if (args.Length < 2) { Help(p); return; }
            if (args.Length == 2) { Player.Message(p, "You need to provide a rank name for this type."); return; }
            Group grp = Matcher.FindRanks(p, args[2]);
            if (grp == null) return;

            switch (args[0].ToLower()) {
                case "dl":
                case "drawlimit":
                    Chat.MessageGlobal("{0}%S's draw limit set to &b{1}", grp.ColoredName, limit);
                    grp.MaxBlocks = limit; break;
                case "mu":
                case "maxundo":
                    Chat.MessageGlobal("{0}%S's undo limit set to &b{1}", grp.ColoredName, limit);
                    grp.MaxUndo = limit; break;
                case "gen":
                case "genlimit":
                    Chat.MessageGlobal("{0}%S's map gen volume limit set to &b{1}", grp.ColoredName, limit);
                    grp.GenVolume = limit; break;
                default:
                    Help(p); return;
            }
            Group.SaveList(Group.GroupList);
        }
        
        static void SetLimitPercent(Player p, ref float target, float value, bool hasValue) {
            const string type = "Threshold before drawing reloads map";
            if (hasValue) target = value / 100.0f;
            string percent = (target * 100).ToString("F2") + "%";
            
            if (!hasValue) {
                Player.Message(p, type + ": &b" + percent);
            } else {
                Chat.MessageGlobal(type + " set to &b" + percent);
                SrvProperties.Save();
            }
        }
        
        static void SetLimit(Player p, string type, ref int target, int value, bool hasValue) {
            if (!hasValue) {
                Player.Message(p, type + ": &b" + target);
            } else {
                target = value;
                Chat.MessageGlobal(type + " set to &b" + target);                
                SrvProperties.Save();
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/limit [type] [amount] <rank>");
            Player.Message(p, "%HSets the limit for [type]");
            Player.Message(p, "%HValid types: %Sreloadthreshold(rt), restartphysics(rp), " +
                           "rpnormal, physicsundo(pu), drawlimit(dl), maxundo(mu), genlimit(gen)");
            Player.Message(p, "%H<rank> is required for drawlimit, maxundo, gen types.");
        }
    }
}
