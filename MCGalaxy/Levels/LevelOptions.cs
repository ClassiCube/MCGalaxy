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
        
        public static Dictionary<string, OptionSetter> Options = new Dictionary<string, OptionSetter>() {
            { "motd", SetMotd }, { "RealmOwner", SetRealmOwner }, { "TreeType", SetTreeType },
            { "PhysicSpeed", SetPhysicSpeed }, { "Overload", SetOverload }, { "Fall", SetFall }, { "Drown", SetDrown },
            { "Finite", SetFinite }, { "AI", SetAI }, { "Edge", SetEdge }, { "Grass", SetGrass },
            { "Death", SetDeath }, { "Killer", SetKiller }, { "Unload", SetUnload }, { "LoadOnGoto", SetLoadOnGoto },
            { "LeafDecay", SetLeafDecay }, { "RandomFlow", SetRandomFlow }, { "GrowTrees", SetGrowTrees },
            { "Chat", SetChat }, { "Guns", ToggleGuns }, { "Buildable", SetBuildable }, { "Deletable", SetDeletable },
            { "LoadDelay", SetLoadDelay },
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

        static void SetMotd(Player p, Level lvl, string value) {
            lvl.Config.MOTD = value.Length == 0 ? "ignore" : value;
            lvl.ChatLevel("Map's MOTD was changed to: &b" + lvl.Config.MOTD);
            
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
            	// Unfortunately, some clients will freeze or crash if we send a MOTD packet,
            	// but don't follow it up by a new map. Thus, we have to use the ugly approach
            	// of only sending to whitelisted clients. 
                if (pl.appName != null && pl.appName.CaselessStarts("classicalsharp")) {
                    pl.SendMapMotd();
                } else {
                    LevelActions.ReloadMap(p, pl, false);
                    if (pl.HasCpeExt(CpeExt.HackControl)) pl.Send(Hacks.MakeHackControl(pl));
                }
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
        
        static void SetFinite(Player p, Level l, string v) { Toggle(p, l, ref l.Config.FiniteLiquids, "Finite mode"); }
        static void SetAI(Player p, Level l, string v) { Toggle(p, l, ref l.Config.AnimalHuntAI, "Animal AI"); }
        static void SetEdge(Player p, Level l, string v) { Toggle(p, l, ref l.Config.EdgeWater, "Edge water"); }
        static void SetGrass(Player p, Level l, string v) { Toggle(p, l, ref l.Config.GrassGrow, "Growing grass"); }
        static void SetDeath(Player p, Level l, string v) { Toggle(p, l, ref l.Config.SurvivalDeath, "Survival death"); }
        static void SetKiller(Player p, Level l, string v) { Toggle(p, l, ref l.Config.KillerBlocks, "Killer blocks"); }
        static void SetUnload(Player p, Level l, string v) { Toggle(p, l, ref l.Config.AutoUnload, "Auto unload"); }
        static void SetLoadOnGoto(Player p, Level l, string v) { Toggle(p, l, ref l.Config.LoadOnGoto, "Load on goto"); }
        static void SetLeafDecay(Player p, Level l, string v) { Toggle(p, l, ref l.Config.LeafDecay, "Leaf decay"); }
        static void SetRandomFlow(Player p, Level l, string v) { Toggle(p, l, ref l.Config.RandomFlow, "Random flow"); }
        static void SetGrowTrees(Player p, Level l, string v) { Toggle(p, l, ref l.Config.GrowTrees, "Tree growing"); }
        static void SetBuildable(Player p, Level l, string v) { TogglePerms(p, l, ref l.Config.Buildable, "Buildable"); }
        static void SetDeletable(Player p, Level l, string v) { TogglePerms(p, l, ref l.Config.Deletable, "Deletable"); }
        static void SetChat(Player p, Level l, string v) {
            Toggle(p, l, ref l.Config.ServerWideChat, "Roleplay (level only) chat", true);
        }
        
        static void SetLoadDelay(Player p, Level l, string value) {
            int raw = 0;
            if (!CommandParser.GetInt(p, value, "Load delay", ref raw, 0, 2000)) return;
            SetInt(l, raw, ref l.Config.LoadDelay, "Load delay");
        }
        
        static void SetPhysicSpeed(Player p, Level l, string value) {
            int raw = 0;
            if (!CommandParser.GetInt(p, value, "Physics speed", ref raw, 10)) return;
            SetInt(l, raw, ref l.Config.PhysicsSpeed, "Physics speed");
        }
        
        static void SetOverload(Player p, Level l, string value) {
            int raw = 0;
            if (!CommandParser.GetInt(p, value, "Physics overload", ref raw, 500)) return;
            
            if (p != null && p.Rank < LevelPermission.Admin && raw > 2500) {
                Player.Message(p, "Only SuperOPs may set physics overload higher than 2500"); return;
            }
            SetInt(l, raw, ref l.Config.PhysicsOverload, "Physics overload");
        }
        
        static void SetFall(Player p, Level l, string value) {
            int raw = 0;
            if (!CommandParser.GetInt(p, value, "Fall distance", ref raw)) return;
            SetInt(l, raw, ref l.Config.FallHeight, "Fall distance");
        }
        
        static void SetDrown(Player p, Level l, string value) {
            int raw = 0;
            if (!CommandParser.GetInt(p, value, "Drown time (in tenths of a second)", ref raw)) return;
            SetInt(l, raw, ref l.Config.DrownTime, "Drown time (in tenths of a second)");
        }
        
        static void SetInt(Level lvl, int raw, ref int target, string name) {
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
            string targetStr = display ? "&aON" : "&cOFF";
            lvl.ChatLevel(name + ": " + targetStr);
            
            if (p == null || p.level != lvl) {
                Player.Message(p, name + ": " + targetStr);
            }
        }
    }
}
