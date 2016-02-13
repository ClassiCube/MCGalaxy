/*
    Copyright 2011 MCGalaxy
        
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
                Player.SendMessage(p, "Limit amount must be a whole number and greater than 0."); return;
            }
            
            switch (args[0].ToLower()) {
                case "rt":
                case "reloadthreshold":
                    Player.GlobalMessage("Threshold before drawing reloads map changed to &b" + limit);
                    Server.DrawReloadLimit = limit; return;
                case "rp":
                case "restartphysics":
                    Player.GlobalMessage("Custom /rp's limit was changed to &b" + limit);
                    Server.rpLimit = limit; return;
                case "rpnormal":
                    Player.GlobalMessage("Normal /rp's limit was changed to &b" + limit);
                    Server.rpNormLimit = limit; return;
                case "pu":
                case "physicsundo":
                    Player.GlobalMessage("Physics undo max entries was changed to &b" + limit);
                    Server.physUndo = limit; return;
            }

            if (args.Length == 2) { Player.SendMessage(p, "You need to provide a rank name for this type."); return; }
            Group grp = Group.Find(args[2]);
            if (grp == null) { Player.SendMessage(p, "No rank found matching: " + args[2]); return; }

            switch (args[0].ToLower()) {
                case "dl":
                case "drawlimit":            
                    Player.GlobalMessage(grp.color + grp.name + "%S's draw limit was set to &b" + limit);
                    grp.maxBlocks = limit; break;
                case "mu":
                case "maxundo":
                    Player.GlobalMessage(grp.color + grp.name + "%S's undo limit was set to &b" + limit);
                    grp.maxUndo = limit; break;                    
                default:
                    Help(p); return;
            }
            Group.saveGroups(Group.GroupList);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/limit <type> <amount> [rank] - Sets the limit for <type>");
            Player.SendMessage(p, "Valid types: reloadthreshold(rt), restartphysics(rp), " +
                               "rpnormal, physicsundo(pu), drawlimit(dl), maxundo(mu)");
            Player.SendMessage(p, "Rank is required for drawlimit and maxundo types.");
        }
    }
}
