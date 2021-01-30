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

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdLimit : Command2 {        
        public override string name { get { return "Limit"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0) { Help(p); return; }
            bool hasLimit = args.Length > 1;
            
            if (args[0].CaselessEq("rt") || args[0].CaselessEq("reloadthreshold")) {
                float threshold = 0;
                if (hasLimit && !CommandParser.GetReal(p, args[1], "Limit", ref threshold, 0, 100)) return;
                
                SetLimitPercent(p, ref Server.Config.DrawReloadThreshold, threshold, hasLimit);
                return;
            }
            
            int limit = 0;
            if (hasLimit && !CommandParser.GetInt(p, args[1], "Limit", ref limit, 1)) return;
            
            switch (args[0].ToLower()) {
                case "rp":
                case "restartphysics":
                    SetLimit(p, "Custom /rp limit", ref Server.Config.PhysicsRestartLimit, limit, hasLimit);
                    return;
                case "rpnormal":
                    SetLimit(p, "Normal /rp limit", ref Server.Config.PhysicsRestartNormLimit, limit, hasLimit);
                    return;
                case "pu":
                case "physicsundo":
                    SetLimit(p, "Physics undo max entries", ref Server.Config.PhysicsUndo, limit, hasLimit);
                    return;
            }

            if (args.Length < 2) { Help(p); return; }
            if (args.Length == 2) { p.Message("You need to provide a rank name for this type."); return; }
            Group grp = Matcher.FindRanks(p, args[2]);
            if (grp == null) return;

            switch (args[0].ToLower()) {
                case "draw":
                    Chat.MessageAll(grp.ColoredName + "&S's draw limit set to &b" + limit);
                    grp.DrawLimit = limit; break;
                case "maxundo":
                    Chat.MessageAll(grp.ColoredName + "&S's undo limit set to &b" + limit);
                    grp.MaxUndo = TimeSpan.FromSeconds(limit); break;
                case "gen":
                    Chat.MessageAll(grp.ColoredName + "&S's map gen volume limit set to &b" + limit);
                    grp.GenVolume = limit; break;
                case "realms":
                    Chat.MessageAll(grp.ColoredName + "&S's max realms set to &b" + limit);
                    grp.OverseerMaps = limit; break;
                default:
                    Help(p); return;
            }
            Group.SaveAll(Group.GroupList);
        }
        
        static void SetLimitPercent(Player p, ref float target, float value, bool hasValue) {
            const string type = "Threshold before drawing reloads map";
            if (hasValue) target = value / 100.0f;
            string percent = (target * 100).ToString("F2") + "%";
            
            if (!hasValue) {
                p.Message(type + ": &b" + percent);
            } else {
                Chat.MessageAll(type + " set to &b" + percent);
                SrvProperties.Save();
            }
        }
        
        static void SetLimit(Player p, string type, ref int target, int value, bool hasValue) {
            if (!hasValue) {
                p.Message(type + ": &b" + target);
            } else {
                target = value;
                Chat.MessageAll(type + " set to &b" + target);
                SrvProperties.Save();
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Limit [type] [amount]");
            p.Message("&HSets the general limit for [type]");
            p.Message("  &HValid types: &freloadthreshold, restartphysics(rp), rpnormal, physicsundo(pu)");
            p.Message("&T/Limit [type] [amount] [rank]");
            p.Message("&HSets the limit for [type] for the given rank");
            p.Message("  &HValid types: &fdraw, maxundo, gen, realms");
        }
    }
}
