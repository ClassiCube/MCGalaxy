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
            get { return new[] { new CommandAlias("ps", "physicspeed"),
                    new CommandAlias("allowguns", null, "guns") }; }
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
                lvl = Matcher.FindLevels(p, args[0]);
                if (lvl == null) return;
                if (args.Length == 1) { PrintMapInfo(p, lvl); return; }
                
                opt = args[1];
                value = args.Length > 2 ? args[2] : "";
            }
            
            if (!CheckExtraPerm(p)) { MessageNeedExtra(p, 1); return; }
            if (opt.CaselessEq("realmowner") && !CheckExtraPerm(p, 2)) {
                MessageNeedExtra(p, 2); return;
            }
            
            if (SetMapOption(p, lvl, opt, value)) return;
            Player.Message(p, "Could not find option entered.");
        }
        
        static bool IsMapOption(string[] args) {
            string opt = LevelOptions.Map(args[0].ToLower());
            if (!ValidOption(opt)) return false;
            // In rare case someone uses /map motd motd My MOTD
            if (opt == "motd" && (args.Length == 1 || !args[1].CaselessStarts("motd "))) return true;
            
            int argsCount = HasArgument(opt) ? 2 : 1;
            return args.Length == argsCount;
        }
        
        internal static bool SetMapOption(Player p, Level lvl, string opt, string value) {
            opt = LevelOptions.Map(opt.ToLower());
            foreach (var option in LevelOptions.Options) {
                if (!option.Key.CaselessEq(opt)) continue;
                
                option.Value(p, lvl, value);
                Level.SaveSettings(lvl);
                return true;
            }
            return false;
        }
        
        
        static bool ValidOption(string opt) {
            foreach (var option in LevelOptions.Options) {
                if (option.Key.CaselessEq(opt)) return true;
            }
            return false;
        }
        
        static bool HasArgument(string opt) {
            return opt == "physicspeed" || opt == "overload" || opt == "treetype"
                || opt == "fall" || opt == "drown" || opt == "realmowner";
        }
        
        static void PrintMapInfo(Player p, Level lvl) {
            Player.Message(p, "%TPhysics settings:");
            Player.Message(p, "  Finite mode: {0}%S, Random flow: {1}",
                           GetBool(lvl.finite), GetBool(lvl.randomFlow));
            Player.Message(p, "  Animal AI: {0}%S, Edge water: {1}",
                           GetBool(lvl.ai), GetBool(lvl.edgeWater));
            Player.Message(p, "  Grass growing: {0}%S, {1} tree growing: {2}",
                           GetBool(lvl.GrassGrow), lvl.TreeType.Capitalize(), GetBool(lvl.growTrees));
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
        
        internal static void SetBool(Player p, Level lvl, ref bool target, string name, bool negate = false) {
            target = !target;
            bool display = negate ? !target : target;
            lvl.ChatLevel(name + GetBool(display));
            
            if (p == null || p.level != lvl)
                Player.Message(p, name + GetBool(display, p == null));
        }
        
        static string GetBool(bool value, bool console = false) {
            return console ? (value ? "ON" : "OFF") : (value ? "&aON" : "&cOFF");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/map [level] [option] <value> %H- Sets [option] on [level]");
            Player.Message(p, "%HPossible options: %S{0}", LevelOptions.Options.Keys.Join());
            Player.Message(p, "%HUse %T/help map [option] %Hto see a description for that option.");
        }
        
        public override void Help(Player p, string message) {
            string opt = LevelOptions.Map(message.ToLower());
            foreach (var help in LevelOptions.Help) {
                if (!help.Key.CaselessEq(opt)) continue;
                Player.Message(p, "%T/map [level] {0}{1}", opt, Suffix(opt));
                Player.Message(p, "%H" + help.Value);
                return;
            }
            Player.Message(p, "Unrecognised option \"{0}\".", opt);
        }
        
        static string Suffix(string opt) {
            if (opt == "motd") return " <value>";
            return HasArgument(opt) ? " [value]" : "";
        }
    }
}
