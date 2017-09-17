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
using MCGalaxy.Commands;
using MCGalaxy.Generator.Foliage;

namespace MCGalaxy {
    
    public static class LevelOptions {
        
        public delegate void OptionSetter(Player p, Level lvl, string value);
        
        public delegate bool OptionIntValidator(Player p, int value);
        
        public static Dictionary<string, OptionSetter> Options = new Dictionary<string, OptionSetter>() {
            { "motd", SetMotd },
            { "RealmOwner", SetRealmOwner },
            { "TreeType", SetTreeType },
            { "PhysicSpeed", (p, l, value) => SetInt(p, l, ref l.Config.PhysicsSpeed, value, "Physics speed", SpeedValidator) },
            { "Overload", (p, l, value) => SetInt(p, l, ref l.Config.PhysicsOverload, value, "Physics overload", OverloadValidator) },
            { "Fall", (p, l, value) => SetInt(p, l, ref l.Config.FallHeight, value, "Fall distance") },
            { "Drown", (p, l, value) => SetInt(p, l, ref l.Config.DrownTime, value, "Drown time (in tenths of a second)") },
            { "Finite", (p, l, value) => Toggle(p, l, ref l.Config.FiniteLiquids, "Finite mode") },
            { "AI", (p, l, value) => Toggle(p, l, ref l.Config.AnimalHuntAI, "Animal AI") },
            { "Edge", (p, l, value) => Toggle(p, l, ref l.Config.EdgeWater, "Edge water") },
            { "Grass", (p, l, value) => Toggle(p, l, ref l.Config.GrassGrow, "Growing grass") },
            { "Death", (p, l, value) => Toggle(p, l, ref l.Config.SurvivalDeath, "Survival death") },
            { "Killer", (p, l, value) => Toggle(p, l, ref l.Config.KillerBlocks, "Killer blocks") },
            { "Unload", (p, l, value) => Toggle(p, l, ref l.Config.AutoUnload, "Auto unload") },
            { "LoadOnGoto", (p, l, value) => Toggle(p, l, ref l.Config.LoadOnGoto, "Load on goto") },
            { "LeafDecay", (p, l, value) => Toggle(p, l, ref l.Config.LeafDecay, "Leaf decay") },
            { "RandomFlow", (p, l, value) => Toggle(p, l, ref l.Config.RandomFlow, "Random flow") },
            { "GrowTrees", (p, l, value) => Toggle(p, l, ref l.Config.GrowTrees, "Tree growing") },
            { "Chat", (p, l, value) => Toggle(p, l, ref l.Config.ServerWideChat, "Roleplay (level only) chat", true) },
            { "Guns", ToggleGuns },
            { "Buildable", (p, l, value) => TogglePerms(p, l, ref l.Config.Buildable, "Buildable") },
            { "Deletable", (p, l, value) => TogglePerms(p, l, ref l.Config.Deletable, "Deletable") },
            { "LoadDelay", (p, l, value) => SetInt(p, l, ref l.Config.LoadDelay, value, "Load delay", DelayValidator) },
        };
        
        public static Dictionary<string, string> Help = new Dictionary<string, string>() {
            { "motd", "%HSets the motd for this map. (leave blank to use default motd)" },
            { "RealmOwner", "%HSets the players allowed to use /realm on this map." },
            { "TreeType", "%HSets the type of trees saplings grow into." },
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
            { "LoadDelay", "%HSets the delay before the end of the map is sent. Only useful for forcing players to see the map's MOTD at the loading screen." },
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
            lvl.Config.MOTD = value.Length == 0 ? "ignore" : value;
            lvl.ChatLevel("Map's MOTD was changed to: &b" + lvl.Config.MOTD);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != lvl || !pl.HasCpeExt(CpeExt.HackControl)) continue;
                pl.Send(Hacks.MakeHackControl(pl));
            }
        }
        
        static void SetRealmOwner(Player p, Level lvl, string value) {
            lvl.Config.RealmOwner = value.Replace(' ', ',');
            if (value.Length == 0) Player.Message(p, "Removed realm owner for this level.");
            else Player.Message(p, "Set realm owner/owners of this level to {0}.", value);
        }
        
        static void SetTreeType(Player p, Level lvl, string value) {
            if (value.Length == 0) {
                Player.Message(p, "Reset tree type to default.");
                lvl.Config.TreeType = "fern";
                return;
            }
            
            Tree tree = Tree.Find(value);
            if (tree == null) {
                Player.Message(p, "Tree type {0} not found.", value);
                Player.Message(p, "Tree types: {0}", Tree.TreeTypes.Join(t => t.Key));
                return;
            }
            
            lvl.Config.TreeType = value.ToLower();
            Player.Message(p, "Set tree type that saplings grow into to {0}.", value);
        }
        
        
        
        static bool SpeedValidator(Player p, int raw) {
            if (raw < 10) { Player.Message(p, "Physics speed cannot be below 10 milliseconds."); return false; }
            return true;
        }
        
        static bool OverloadValidator(Player p, int raw) {
            if (raw < 500) {
                Player.Message(p, "Physics overload cannot go below 500 (default is 1500)"); return false;
            }
            if (p != null && p.Rank < LevelPermission.Admin && raw > 2500) {
                Player.Message(p, "Only SuperOPs may set physics overload higher than 2500"); return false;
            }
            return true;
        }
        
        static bool DelayValidator(Player p, int raw) {
            if (raw < 0) {
                Player.Message(p, "Load delay cannot go below 0 milliseconds. (default is 0)"); return false;
            }
            if (raw > 2000) {
                Player.Message(p, "Load delay cannot go above 2000 milliseconds."); return false;
            }
            return true;
        }
        
        static void SetInt(Player p, Level lvl, ref int target, string value, string name,
                           OptionIntValidator validator = null) {
            if (value.Length == 0) { Player.Message(p, "You must provide an integer."); return; }
            int raw = 0;
            if (!CommandParser.GetInt(p, value, name, ref raw)) return;
            
            if (validator != null && !validator(p, raw)) return;
            target = raw;
            lvl.ChatLevel(name + ": &b" + target);
        }
        
        
        static void ToggleGuns(Player p, Level lvl, string value) {
            Toggle(p, lvl, ref lvl.Config.Guns, "Guns allowed");
            if (lvl.Config.Guns) return;
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level.name.CaselessEq(lvl.name) && pl.aiming) {
                    pl.aiming = false;
                    pl.ClearBlockchange();
                }
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
