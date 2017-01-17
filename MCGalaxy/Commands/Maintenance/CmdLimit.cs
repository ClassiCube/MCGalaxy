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
    
    public sealed class CmdLimit : Command {
        
        public override string name { get { return "limit"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdLimit() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (message == "") { Help(p); return; }
            int limit = 0;
            bool hasLimit = args.Length > 1;
            
            if (hasLimit && (!int.TryParse(args[1], out limit) || limit <= 0)) {
                Player.Message(p, "Limit amount must be a whole number and greater than 0."); return;
            }
            
            switch (args[0].ToLower()) {
                case "rt":
                case "reloadthreshold":
                    SetLimit(p, "Threshold before drawing reloads map", ref Server.DrawReloadLimit, limit, hasLimit);
                    return;
                case "rp":
                case "restartphysics":
                    SetLimit(p, "Custom /rp limit", ref Server.rpLimit, limit, hasLimit);
                    return;
                case "rpnormal":
                    SetLimit(p, "Normal /rp limit", ref Server.rpNormLimit, limit, hasLimit);
                    return;
                case "pu":
                case "physicsundo":
                    SetLimit(p, "Physics undo max entries", ref Server.physUndo, limit, hasLimit);
                    return;
                case "gen":
                case "genlimit":
                    SetLimit(p, "Maximum volume of maps players can generate", ref Server.MapGenLimit, limit, hasLimit);
                    return;
                case "genadmin":
                case "genadminlimit":
                case "admingen":
                case "admingenlimit":
                    SetLimit(p, "Maximum volume of maps admins can generate", ref Server.MapGenLimitAdmin, limit, hasLimit);
                    return;
            }

            if (args.Length < 2) { Help(p); return; }
            if (args.Length == 2) { Player.Message(p, "You need to provide a rank name for this type."); return; }
            Group grp = Group.FindMatches(p, args[2]);
            if (grp == null) return;

            switch (args[0].ToLower()) {
                case "dl":
                case "drawlimit":
                    Chat.MessageAll("{0}%S's draw limit set to &b{1}", grp.ColoredName, limit);
                    grp.maxBlocks = limit; break;
                case "mu":
                case "maxundo":
                    Chat.MessageAll("{0}%S's undo limit set to &b{1}", grp.ColoredName, limit);
                    grp.maxUndo = limit; break;
                default:
                    Help(p); return;
            }
            Group.saveGroups(Group.GroupList);
        }
        
        static void SetLimit(Player p, string format, ref int target, int value, bool hasValue) {
            if (!hasValue) {
                Player.Message(p, format + ": &b" + target);
            } else {
                target = value;
                Chat.MessageAll(format + " set to &b" + target);                
                SrvProperties.Save();
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/limit [type] [amount] <rank>");
            Player.Message(p, "%HSets the limit for [type]");
            Player.Message(p, "%HValid types: %Sreloadthreshold(rt), restartphysics(rp), " +
                           "rpnormal, physicsundo(pu), drawlimit(dl), maxundo(mu), genlimit(gen), " +
                           "admingenlimit(admingen)");
            Player.Message(p, "%H<rank> is required for drawlimit and maxundo types.");
        }
    }
}
