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

namespace MCGalaxy.Commands.World {
    public sealed class CmdMap : Command {
        public override string name { get { return "map"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can edit map options"),
                  new CommandPerm(LevelPermission.Admin, "+ can set realm owners") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ps", "ps") }; }
        }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "level name")) return;
            if (message == "") message = p.level.name;
            string[] args = message.SplitSpaces(3);
            Level lvl = null;
            string opt = null, value = null;
            
            if (IsMapOption(args)) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "level"); return; }
                
                lvl = p.level; opt = args[0];
                args = message.SplitSpaces(2);
                value = args.Length > 1 ? args[1] : "";
            } else {
                lvl = LevelInfo.FindMatches(p, args[0]);
                if (lvl == null) return;
                if (args.Length == 1) { PrintMapInfo(p, lvl); return; }
                
                opt = args[1];
                value = args.Length > 2 ? args[2] : "";
            }
            
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, "set map options."); return; }
            if (opt.CaselessEq("realmowner") && !CheckExtraPerm(p, 2)) { 
                MessageNeedExtra(p, "set personal realm owners.", 2); return; 
            }
            SetMapOption(p, lvl, opt, value);
        }
        
        static bool IsMapOption(string[] args) {
            string opt = args[0].ToLower();
            const string opts = "theme|finite|ai|edge|grass|ps|physicspeed|overload|motd|death|killer|fall|drown|unload"
                + "|realmowner|chat|load|loadongoto|leaf|leafdecay|flow|randomflow|tree|growtrees|buildable|deletable";
            if (!opts.Contains(opt)) return false;
            // In rare case someone uses /map motd motd My MOTD
            if (opt == "motd" && (args.Length == 1 || !args[1].CaselessStarts("motd "))) return true;
            
            bool optHasArg = opt == "ps" || opt == "physicspeed" || opt == "overload" 
                || opt == "fall" || opt == "drown" || opt == "realmowner";
            return args.Length == (optHasArg ? 2 : 1);
        }
        
        internal static void SetMapOption(Player p, Level lvl, string opt, string value) {
            switch (opt.ToLower()) {
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
                    SetInt(p, lvl, ref lvl.speedPhysics, value, "Physics speed", PhysicsSpeedValidator); break;
                case "overload":
                    SetInt(p, lvl, ref lvl.overload, value, "Physics overload", PhysicsOverloadValidator); break;
                case "motd":
                    lvl.motd = value == "" ? "ignore" : value;
                    lvl.ChatLevel("Map's MOTD was changed to: &b" + lvl.motd);
                    break;
                case "death":
                    SetBool(p, lvl, ref lvl.Death, "Survival death: "); break;
                case "killer":
                    SetBool(p, lvl, ref lvl.Killer, "Killer blocks: "); break;
                case "fall":
                    SetInt(p, lvl, ref lvl.fall, value, "Fall distance", null); break;
                case "drown":
                    SetInt(p, lvl, ref lvl.drown, value, "Drown time (in tenths of a second)", null); break;
                case "unload":
                    SetBool(p, lvl, ref lvl.unload, "Auto unload: "); break;
                case "chat":
                    SetBool(p, lvl, ref lvl.worldChat, "Roleplay (level only) chat: ", true); break;
                case "load":
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
                case "realmowner":
                    lvl.RealmOwner = value;
                    if (value == "") Player.Message(p, "Removed realm owner for this level.");
                    else Player.Message(p, "Set realm owner of this level to {0}.", value);
                    break;
                default:
                    Player.Message(p, "Could not find option entered."); return;
            }
            Level.SaveSettings(lvl);
        }
        
        static void PrintMapInfo(Player p, Level lvl) {
            Player.Message(p, "%TPhysics settings:");
            Player.Message(p, "  Finite mode: {0}%S, Random flow: {1}", 
                           GetBool(lvl.finite), GetBool(lvl.randomFlow));
            Player.Message(p, "  Animal AI: {0}%S, Edge water: {1}", 
                           GetBool(lvl.ai), GetBool(lvl.edgeWater));
            Player.Message(p, "  Grass growing: {0}%S, Tree growing: {1}", 
                           GetBool(lvl.GrassGrow), GetBool(lvl.growTrees));
            Player.Message(p, "  Leaf decay: {0}%S, Physics overload: {1}", 
                           GetBool(lvl.leafDecay), lvl.overload);
            Player.Message(p, "  Physics speed: &b{0} %Smilliseconds between ticks", 
                           lvl.speedPhysics);
            
            Player.Message(p, "%TSurvival settings:");
            Player.Message(p, "  Survival death: {0} %S(Fall: {1}, Drown: {2})",
                           GetBool(lvl.Death), lvl.fall, lvl.drown);
            Player.Message(p, "  Guns: {0}%S, Killer blocks: {1}",
                           GetBool(lvl.guns), GetBool(lvl.Killer));
            
            Player.Message(p, "%TGeneral settings:");
            Player.Message(p, "  MOTD: &b" + lvl.motd);
            Player.Message(p, "  Roleplay (level only) chat: " + GetBool(!lvl.worldChat));
            Player.Message(p, "  Load on /goto: {0}%S, Auto unload: {1}", 
                           GetBool(lvl.loadOnGoto), GetBool(lvl.unload));
            Player.Message(p, "  Buildable: {0}%S, Deletable: {1}", 
                           GetBool(lvl.Buildable), GetBool(lvl.Deletable));
        }
        
        
        static bool PhysicsSpeedValidator(Player p, int raw) {
            if (raw < 10) { Player.Message(p, "Physics speed cannot be below 10 milliseconds."); return false; }
            return true;
        }
        
        static bool PhysicsOverloadValidator(Player p, int raw) {
            if (raw < 500) { 
                Player.Message(p, "Physics overload cannot go below 500 (default is 1500)"); return false; 
            }
            if (p != null && p.Rank < LevelPermission.Admin && raw > 2500) { 
                Player.Message(p, "Only SuperOPs may set physics overload higher than 2500"); return false; 
            }
            return true;
        }
        
        internal static void SetBool(Player p, Level lvl, ref bool target, string name, bool negate = false) {
            target = !target;
            bool display = negate ? !target : target;
            lvl.ChatLevel(name + GetBool(display));
            
            if (p == null || p.level != lvl)
                Player.Message(p, name + GetBool(display, p == null));
        }
        
        static void SetInt(Player p, Level lvl, ref int target, string value, string name,
                           Func<Player, int, bool> validator) {
            if (value == "") { Player.Message(p, "You must provide an integer."); return; }
            int raw;
            if (!int.TryParse(value, out raw)) { Player.Message(p, "\"{0}\" is not a valid integer.", value); return; }
            
            if (validator != null && !validator(p, raw)) return;
            target = raw;
            lvl.ChatLevel(name + ": &b" + target);
        }
        
        static string GetBool(bool value, bool console = false) {
            return console ? (value ? "ON" : "OFF") : (value ? "&aON" : "&cOFF");
        }

        public override void Help(Player p) {
            Player.Message(p, "/map [level] [toggle] - Sets [toggle] on [level]");
            Player.Message(p, "Possible toggles: Theme, Finite, RandomFlow, AI, Edge, Grass, GrowTrees, LeafDecay, ps, Overload, motd, " +
                           "Death, Fall, Drown, Unload, LoadOnGoto, Killer, Chat, Buildable, Deletable, RealmOwner");
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
            Player.Message(p, "RealmOwner allows that person to use /os commands in the map.");
        }
    }
}
