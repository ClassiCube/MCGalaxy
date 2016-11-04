/*
    Copyright 2015 MCGalaxy team
    
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
using System.Collections.Generic;

namespace MCGalaxy {
    
    public static class LevelOptions {
        
        public delegate void OptionSetter(Player p, Level lvl, string value);
        
        public static Dictionary<string, OptionSetter> Options = new Dictionary<string, OptionSetter>() {
            { "motd", SetMotd },
            { "RealmOwner", SetRealmOwner },
            { "PhysicSpeed", (p, l, value) => SetPhysicsSpeed(p, l, value, "Physics speed") },
            { "Overload", (p, l, value) => SetPhysicsOverload(p, l, value, "Physics overload") },
            { "Fall", (p, l, value) => SetInt(p, l, ref l.fall, value, "Fall distance") },
            { "Drown", (p, l, value) => SetInt(p, l, ref l.drown, value, "Drown time (in tenths of a second)") },
            { "Finite", (p, l, value) => Toggle(p, l, ref l.finite, "Finite mode") },
            { "AI", (p, l, value) => Toggle(p, l, ref l.ai, "Animal AI") },
            { "Edge", (p, l, value) => Toggle(p, l, ref l.edgeWater, "Edge water") },
            { "Grass", (p, l, value) => Toggle(p, l, ref l.GrassGrow, "Growing grass") },
            { "Death", (p, l, value) => Toggle(p, l, ref l.Death, "Survival death") },
            { "Killer", (p, l, value) => Toggle(p, l, ref l.Killer, "Killer blocks") },
            { "Unload", (p, l, value) => Toggle(p, l, ref l.unload, "Auto unload") },
            { "LoadOnGoto", (p, l, value) => Toggle(p, l, ref l.loadOnGoto, "Load on goto") },
            { "LeafDecay", (p, l, value) => Toggle(p, l, ref l.leafDecay, "Leaf decay") },
            { "RandomFlow", (p, l, value) => Toggle(p, l, ref l.randomFlow, "Random flow") },
            { "GrowTrees", (p, l, value) => Toggle(p, l, ref l.growTrees, "Tree growing") },
            { "Chat", (p, l, value) => Toggle(p, l, ref l.worldChat, "Roleplay (level only) chat: ", true) },
            { "Guns", ToggleGuns },
            { "Buildable", (p, l, value) => TogglePerms(p, l, ref l.Buildable, "Buildable") },
            { "Deletable", (p, l, value) => TogglePerms(p, l, ref l.Deletable, "Deletable") },
        };
        
        public static Dictionary<string, string> Help = new Dictionary<string, string>() {
            { "motd", "%HSets the custom motd for this map. (leave blank to use server default)" },
            { "RealmOwner", "%HSets the players allowed to use /realm on this map." },
            { "PhysicSpeed", "%HSets the delay (in milliseconds) between physics ticks." },
            { "Overload", "%HSets how hard (high values) or easy (low values) it is to kill physics." },
            { "Fall", "%HSets how many blocks you can fall before dying." },
            { "Drown", "%HSets how long you can stay underwater (in tenths of a second) before drowning." },
            { "Finite", "%HWhether all liquids are finite." },
            { "AI", "%HAI will make animals hunt or flee." },
            { "Edge", "%HWhether water flows from the map edges." },
            { "Grass", "%HWhether grass auto grows or not." },
            { "Death", "%HWhether you can die from falling or drowning." },
            { "Killer", "%HWhether certain blocks (e.g. nerve_gas) kill you." },
            { "Unload", "%HWhether the map auto unloads when no one's there." },
            { "LoadOnGoto", "%HWhether the map auto loads when /goto is used." },
            { "LeafDecay", "%HWhether leaves not connected to log blocks within 4 blocks randomly disappear." },
            { "RandomFlow", "%HWhether flooding liquids flow less uniformly." },
            { "GrowTrees", "%HWhether saplings grow into trees after a while." },
            { "Chat", "%HWhether chat is only seen from and sent to players in the map." },
            { "Guns", "%HWhether guns and missiles can be used" },
            { "Buildable", "%HWhether any blocks can be placed by players." },
            { "Deletable", "%HWhether any blocks can be deleted by players." },
        };
        
        
        public static string Map(string opt) {
            if (opt == "ps") return "physicspeed";
            if (opt == "load") return "loadongoto";
            if (opt == "leaf") return "leafdecay";
            if (opt == "flow") return "randomflow";
            if (opt == "tree") return "growtrees";
            return opt;
        }
        
        static string GetBool(bool value) { return value ? "&aON" : "&cOFF"; }

        static void SetMotd(Player p, Level lvl, string value) {
            lvl.motd = value == "" ? "ignore" : value;
            lvl.ChatLevel("Map's MOTD was changed to: &b" + lvl.motd);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.HasCpeExt(CpeExt.HackControl)) continue;
                pl.Send(Hacks.MakeHackControl(pl));
            }
        }
        
        static void SetRealmOwner(Player p, Level lvl, string value) {
            lvl.RealmOwner = value;
            if (value == "") Player.Message(p, "Removed realm owner for this level.");
            else Player.Message(p, "Set realm owner/owners of this level to {0}.", value);
        }
        
        
        static void SetPhysicsSpeed(Player p, Level lvl, string value, string name) {
            SetInt(p, lvl, ref lvl.speedPhysics, value, name, PhysicsSpeedValidator);
        }
        
        static bool PhysicsSpeedValidator(Player p, int raw) {
            if (raw < 10) { Player.Message(p, "Physics speed cannot be below 10 milliseconds."); return false; }
            return true;
        }

        static void SetPhysicsOverload(Player p, Level lvl, string value, string name) {
            SetInt(p, lvl, ref lvl.overload, value, name, PhysicsOverloadValidator);
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
        
        static void SetInt(Player p, Level lvl, ref int target, string value, string name,
                           Func<Player, int, bool> validator = null) {
            if (value == "") { Player.Message(p, "You must provide an integer."); return; }
            int raw;
            if (!int.TryParse(value, out raw)) { Player.Message(p, "\"{0}\" is not a valid integer.", value); return; }
            
            if (validator != null && !validator(p, raw)) return;
            target = raw;
            lvl.ChatLevel(name + ": &b" + target);
        }
        
        
        static void ToggleGuns(Player p, Level lvl, string value) {
            Toggle(p, lvl, ref lvl.guns, "Guns allowed");
            if (lvl.guns) return;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level.name.CaselessEq(lvl.name))
                    pl.aiming = false;
            }
        }
        
        static void TogglePerms(Player p, Level lvl, ref bool target, string name) {
            Toggle(p, lvl, ref target, name);
            lvl.UpdateBlockPermissions();
        }
        
        static void Toggle(Player p, Level lvl, ref bool target, string name, bool not = false) {
            target = !target;
            bool display = not ? !target : target;
            lvl.ChatLevel(name + ": " + GetBool(display));
            
            if (p == null || p.level != lvl)
                Player.Message(p, name + ": " + GetBool(display));
        }
    }
}
