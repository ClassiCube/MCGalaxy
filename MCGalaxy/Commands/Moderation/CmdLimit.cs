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
            if (args.Length < 2) { Help(p); return; }
            int limit;
            if (!int.TryParse(args[1], out limit) || limit <= 0) {
                Player.Message(p, "Limit amount must be a whole number and greater than 0."); return;
            }
            
            switch (args[0].ToLower()) {
                case "rt":
                case "reloadthreshold":
                    SetLimit("Threshold before drawing reloads map set to {0}", ref Server.DrawReloadLimit, limit);
                    return;
                case "rp":
                case "restartphysics":
                    SetLimit("Custom /rp's limit was changed to {0}", ref Server.rpLimit, limit);
                    return;
                case "rpnormal":
                    SetLimit("Normal /rp's limit set to {0}", ref Server.rpNormLimit, limit);
                    return;
                case "pu":
                case "physicsundo":
                    SetLimit("Physics undo max entries set to {0}", ref Server.physUndo, limit);
                    return;
                case "gen":
                case "genlimit":
                    SetLimit("Maximum volume of maps players can generate set to {0}", ref Server.MapGenLimit, limit);
                    return;
                case "genadmin":
                case "genadminlimit":
                case "admingen":
                case "admingenlimit":
                    SetLimit("Maximum volume of maps admins can generate set to &b{0}", ref Server.MapGenLimitAdmin, limit);
                    return;
            }

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
        
        static void SetLimit(string format, ref int target, int limit) {
            Chat.MessageAll(format, "&b" + limit);
            target = limit;
            SrvProperties.Save();
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
