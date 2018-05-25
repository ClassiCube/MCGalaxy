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
        public override string name { get { return "Map"; } }
        public override string type { get { return CommandTypes.World; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "+ can edit map options"),
                    new CommandPerm(LevelPermission.Admin, "+ can set realm owners") }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ps", "physicspeed"),
                    new CommandAlias("AllowGuns", null, "guns") }; }
        }

        public override void Use(Player p, string message) {
            if (CheckSuper(p, message, "level name")) return;
            if (message.Length == 0) message = p.level.name;
            string[] args = message.SplitSpaces(3);
            Level lvl = null;
            string optName = null, value = null;
            
            if (IsMapOption(args)) {
                if (Player.IsSuper(p)) { SuperRequiresArgs(p, "level"); return; }
                lvl = p.level;
                
                optName = args[0];
                args = message.SplitSpaces(2);
                value = args.Length > 1 ? args[1] : "";
            } else if (args.Length == 1) {
                string map = Matcher.FindMaps(p, args[0]);
                if (map == null) return;
                
                PrintMapInfo(p, LevelInfo.GetConfig(map, out lvl));
                return;
            } else {
                lvl = Matcher.FindLevels(p, args[0]);
                if (lvl == null) return;
                
                optName = args[1];
                value = args.Length > 2 ? args[2] : "";
            }
            
            if (!CheckExtraPerm(p, 1)) return;
            if (optName.CaselessEq(LevelOptions.RealmOwner) && !CheckExtraPerm(p, 2)) return;
            if (!LevelInfo.ValidateAction(p, lvl, "change map settings of this level")) return;
            
            LevelOption opt = LevelOptions.Find(optName);
            if (opt == null) {
                Player.Message(p, "Could not find option entered.");
            } else {
                opt.SetFunc(p, lvl, value);
                Level.SaveSettings(lvl);
            }
        }
        
        static bool IsMapOption(string[] args) {
            LevelOption opt = LevelOptions.Find(args[0]);
            if (opt == null) return false;
            // In rare case someone uses /map motd motd My MOTD
            if (opt.Name == LevelOptions.MOTD && (args.Length == 1 || !args[1].CaselessStarts("motd "))) return true;
            
            int argsCount = HasArgument(opt.Name) ? 2 : 1;
            return args.Length == argsCount;
        }
        
        static bool HasArgument(string opt) {
            return
                opt == LevelOptions.Speed || opt == LevelOptions.Overload || opt == LevelOptions.TreeType ||
                opt == LevelOptions.Fall || opt == LevelOptions.Drown || opt == LevelOptions.RealmOwner || opt == LevelOptions.LoadDelay;
        }
        
        static void PrintMapInfo(Player p, LevelConfig cfg) {
            Player.Message(p, "%TPhysics settings:");
            Player.Message(p, "  Finite mode: {0}%S, Random flow: {1}",
                           GetBool(cfg.FiniteLiquids), GetBool(cfg.RandomFlow));
            Player.Message(p, "  Animal hunt AI: {0}%S, Edge water: {1}",
                           GetBool(cfg.AnimalHuntAI), GetBool(cfg.EdgeWater));
            Player.Message(p, "  Grass growing: {0}%S, {1} tree growing: {2}",
                           GetBool(cfg.GrassGrow), cfg.TreeType.Capitalize(), GetBool(cfg.GrowTrees));
            Player.Message(p, "  Leaf decay: {0}%S, Physics overload: {1}",
                           GetBool(cfg.LeafDecay), cfg.PhysicsOverload);
            Player.Message(p, "  Physics speed: &b{0} %Smilliseconds between ticks",
                           cfg.PhysicsSpeed);
            
            Player.Message(p, "%TSurvival settings:");
            Player.Message(p, "  Survival death: {0} %S(Fall: {1}, Drown: {2})",
                           GetBool(cfg.SurvivalDeath), cfg.FallHeight, cfg.DrownTime);
            Player.Message(p, "  Guns: {0}%S, Killer blocks: {1}",
                           GetBool(cfg.Guns), GetBool(cfg.KillerBlocks));
            
            Player.Message(p, "%TGeneral settings:");
            Player.Message(p, "  MOTD: &b" + cfg.MOTD);
            Player.Message(p, "  Roleplay (level only) chat: " + GetBool(!cfg.ServerWideChat));
            Player.Message(p, "  Load on /goto: {0}%S, Auto unload: {1}",
                           GetBool(cfg.LoadOnGoto), GetBool(cfg.AutoUnload));
            Player.Message(p, "  Buildable: {0}%S, Deletable: {1}",
                           GetBool(cfg.Buildable), GetBool(cfg.Deletable));
        }
        
        static string GetBool(bool value) { return value ? "&aON" : "&cOFF"; }

        public override void Help(Player p) {
            Player.Message(p, "%T/Map [map] [option] <value> %H- Sets [option] on that map");
            Player.Message(p, "%HPossible options: %S{0}", LevelOptions.Options.Join(opt => opt.Name));
            Player.Message(p, "%HUse %T/Help map [option] %Hto see description for that option.");
        }
        
        public override void Help(Player p, string message) {
            LevelOption opt = LevelOptions.Find(message);
            if (opt == null) {
                Player.Message(p, "Unrecognised option \"{0}\".", message); return;
            }
            
            bool isMotd = opt.Name == LevelOptions.MOTD;
            string suffix = isMotd ? " <value>" : (HasArgument(opt.Name) ? " [value]" : "");
            
            Player.Message(p, "%T/Map [level] {0}{1}", opt.Name, suffix);
            Player.Message(p, "%H" + opt.Help);
            if (isMotd) ShowMotdRules(p);
        }
        
        static void ShowMotdRules(Player p) {
            Player.Message(p, "%HSpecial rules that can be put in a motd:");
            Player.Message(p, "%T-/+hax %H- disallows/allows all hacks");
            Player.Message(p, "%T-/+fly %H- disallows/allows flying");
            Player.Message(p, "%T-/+noclip %H- disallows/allows noclipping");
            Player.Message(p, "%T-/+respawn %H- disallows/allows respawning");
            Player.Message(p, "%T-/+thirdperson %H- disallows/allows third person camera");
            Player.Message(p, "%T-/+speed %H- disallows/allows speeding");
            Player.Message(p, "%T-/+ophax %H- disallows/allows hacks for {0}%S+",
                           Group.GetColoredName(LevelPermission.Operator));
            Player.Message(p, "%T-/+push %H- disallows/allows player pushing");
            Player.Message(p, "%Tjumpheight=[height] %H- sets max height users can jump up to");
            Player.Message(p, "%Thorspeed=[speed] %H- sets base horizontal speed users move at");
            Player.Message(p, "%Tjumps=[number] %H- sets max number of consecutive jumps");
        }
    }
}
