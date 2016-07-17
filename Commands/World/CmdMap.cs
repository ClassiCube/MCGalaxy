/*
    Copyright 2011 MCForge
        
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
    public sealed class CmdMap : Command {
        public override string name { get { return "map"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can edit map options") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ps", "ps") }; }
        }

        public override void Use(Player p, string message) {
            if (message == "") message = p.level.name;
            Level lvl;
            string[] parts = message.SplitSpaces(3);
            string opt = parts[0].ToLower();
            string value = "";

            if (parts.Length == 1) {
                lvl = LevelInfo.Find(opt);
                if (lvl == null) {
                    if (p != null) lvl = p.level;
                } else {
                    PrintMapInfo(p, lvl); return;
                }
            } else {
                lvl = LevelInfo.Find(opt);
                if (lvl == null || opt == "ps" || opt == "rp") {
                    lvl = p.level;
                    value = parts[1];
                } else {
                    opt = parts[1];
                    value = parts.Length > 2 ? parts[2] : "";
                }
            }
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "can set map options."); return; }

            try {
                if (lvl == null) Player.Message(p, "derp");
                switch (opt) {
                    case "theme":
                        lvl.theme = value;
                        lvl.ChatLevel("Map theme: &b" + lvl.theme); break;
                    case "finite":
                        SetBool(p, lvl, ref lvl.finite, "Finite mode: "); break;
                    case "ai":
                        SetBool(p, lvl, ref lvl.ai, "Animal AI: "); break;
                    case "edge":
                        SetBool(p, lvl, ref lvl.edgeWater, "Edge water: "); break;
                    case "grass":
                        SetBool(p, lvl, ref lvl.GrassGrow, "Growing grass: "); break;
                    case "ps":
                    case "physicspeed":
                        if (int.Parse(value) < 10) { Player.Message(p, "Cannot go below 10"); return; }
                        lvl.speedPhysics = int.Parse(value);
                        lvl.ChatLevel("Physics speed: &b" + lvl.speedPhysics);
                        break;
                    case "overload":
                        if (int.Parse(value) < 500) { Player.Message(p, "Cannot go below 500 (default is 1500)"); return; }
                        if (p != null && p.Rank < LevelPermission.Admin && int.Parse(value) > 2500) { Player.Message(p, "Only SuperOPs may set higher than 2500"); return; }
                        lvl.overload = int.Parse(value);
                        lvl.ChatLevel("Physics overload: &b" + lvl.overload);
                        break;
                    case "motd":
                        lvl.motd = value == "" ? "ignore" : value;
                        lvl.ChatLevel("Map's MOTD was changed to: &b" + lvl.motd);
                        break;
                    case "death":
                        SetBool(p, lvl, ref lvl.Death, "Survival death: "); break;
                    case "killer":
                        SetBool(p, lvl, ref lvl.Killer, "Killer blocks: "); break;
                    case "fall":
                        lvl.fall = int.Parse(value);
                        lvl.ChatLevel("Fall distance: &b" + lvl.fall); break;
                    case "drown":
                        lvl.drown = int.Parse(value);
                        lvl.ChatLevel("Drown time: &b" + ((float)lvl.drown / 10)); break;
                    case "unload":
                        SetBool(p, lvl, ref lvl.unload, "Auto unload: "); break;
                    case "chat":
                        SetBool(p, lvl, ref lvl.worldChat, "Roleplay (level only) chat: ", true); break;
                    case "load":
                    case "autoload":
                    case "loadongoto":
                        SetBool(p, lvl, ref lvl.loadOnGoto, "Load on goto: "); break;
                    case "leaf":
                    case "leafdecay":
                        SetBool(p, lvl, ref lvl.leafDecay, "Leaf deacy: "); break;
                    case "flow":
                    case "randomflow":
                        SetBool(p, lvl, ref lvl.randomFlow, "Random flow: "); break;
                    case "tree":
                    case "growtrees":
                        SetBool(p, lvl, ref lvl.growTrees, "Tree growing: "); break;
                    case "buildable":
                        SetBool(p, lvl, ref lvl.Buildable, "Buildable: ");
                        lvl.UpdateBlockPermissions(); break;
                    case "deletable":
                        SetBool(p, lvl, ref lvl.Deletable, "Deletable: ");
                        lvl.UpdateBlockPermissions(); break;
                        
                    default:
                        Player.Message(p, "Could not find option entered."); return;
                }
                lvl.changed = true;
                if (p != null && p.level != lvl) Player.Message(p, "/map finished!");
            }
            catch { Player.Message(p, "INVALID INPUT"); }
        }
        
        void PrintMapInfo(Player p, Level lvl) {
            Player.Message(p, "MOTD: &b" + lvl.motd);
            Player.Message(p, "Finite mode: " + GetBool(lvl.finite));
            Player.Message(p, "Random flow: " + GetBool(lvl.randomFlow));
            Player.Message(p, "Animal AI: " + GetBool(lvl.ai));
            Player.Message(p, "Edge water: " + GetBool(lvl.edgeWater));
            Player.Message(p, "Grass growing: " + GetBool(lvl.GrassGrow));
            Player.Message(p, "Tree growing: " + GetBool(lvl.growTrees));
            Player.Message(p, "Leaf decay: " + GetBool(lvl.leafDecay));
            Player.Message(p, "Physics speed: &b" + lvl.speedPhysics);
            Player.Message(p, "Physics overload: &b" + lvl.overload);
            Player.Message(p, "Survival death: " + GetBool(lvl.Death) + "(Fall: " + lvl.fall + ", Drown: " + lvl.drown + ")");
            Player.Message(p, "Killer blocks: " + GetBool(lvl.Killer));
            Player.Message(p, "Unload: " + GetBool(lvl.unload));
            Player.Message(p, "Load on /goto: " + GetBool(lvl.loadOnGoto));
            Player.Message(p, "Roleplay (level only) chat: " + GetBool(!lvl.worldChat));
            Player.Message(p, "Guns: " + GetBool(lvl.guns));
            Player.Message(p, "Buildable: " + GetBool(lvl.Buildable));
            Player.Message(p, "Deletable: " + GetBool(lvl.Deletable));
        }
        
        void SetBool(Player p, Level lvl, ref bool target, string message, bool negate = false) {
            target = !target;
            Level.SaveSettings(lvl);
            bool display = negate ? !target : target;
            lvl.ChatLevel(message + GetBool(display));
            
            if (p == null || p.level != lvl)
                Player.Message(p, message + GetBool(display, p == null));
        }
        
        string GetBool(bool value, bool console = false) {
            return console ? (value ? "ON" : "OFF") : (value ? "&aON" : "&cOFF");
        }

        public override void Help(Player p) {
            Player.Message(p, "/map [level] [toggle] - Sets [toggle] on [level]");
            Player.Message(p, "Possible toggles: theme, finite, randomflow, ai, edge, grass, growtrees, leafdecay, ps, overload, motd, " +
                           "death, fall, drown, unload, loadongoto, rp, killer, chat, buildable, deletable, levelonlydeath");
            Player.Message(p, "Edge will cause edge water to flow.");
            Player.Message(p, "Grass will make grass not grow without physics.");
            Player.Message(p, "Tree growing will make saplings grow into trees after a while.");
            Player.Message(p, "Leaf decay will make leaves not connected to a log within 4 blocks disappear randomly.");
            Player.Message(p, "Finite will cause all liquids to be finite.");
            Player.Message(p, "Random flow makes mass flooding liquids flow less uniformly.");
            Player.Message(p, "AI will make animals hunt or flee.");
            Player.Message(p, "PS will set the map's physics speed.");
            Player.Message(p, "Overload will change how easy/hard it is to kill physics.");
            Player.Message(p, "MOTD will set a custom motd for the map. (leave blank to reset)");
            Player.Message(p, "Death will allow survival-style dying (falling, drowning)");
            Player.Message(p, "Fall/drown set the distance/time before dying from each.");
            Player.Message(p, "Drowning value is 10 for one second of air.");
            Player.Message(p, "Killer turns killer blocks on and off.");
            Player.Message(p, "Unload sets whether the map unloads when no one's there.");
            Player.Message(p, "Load on /goto sets whether the map can be loaded when some uses /goto. Only works if the load on /goto server option is enabled.");
            Player.Message(p, "Buildable sets whether any blocks can be placed by any player");
            Player.Message(p, "Deletable sets whether any blocks can be deleted by any player");
        }
    }
}
