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
using System.IO;
using MCGalaxy.Scripting;

namespace MCGalaxy.Commands.Scripting 
{
    public sealed class CmdPlugin : Command2 
    {
        public override string name { get { return "Plugin"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Owner; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("PLoad", "load"), new CommandAlias("PUnload", "unload"),
                    new CommandAlias("Plugins", "list") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces(2);
            if (IsListCommand(args[0])) {
                string modifier = args.Length > 1 ? args[1] : "";
                
                p.Message("Loaded plugins:");
                Paginator.Output(p, Plugin.custom, pl => pl.name,
                                 "Plugins", "plugins", modifier);
                return;
            }
            if (args.Length == 1) { Help(p); return; }
            
            string cmd = args[0], name = args[1];
            if (!Formatter.ValidFilename(p, name)) return;
            
            if (cmd.CaselessEq("load")) {
                string path = IScripting.PluginPath(name);
                ScriptingOperations.LoadPlugins(p, path);
            } else if (cmd.CaselessEq("unload")) {
                UnloadPlugin(p, name);
            } else if (cmd.CaselessEq("create")) {
                p.Message("Use &T/PCreate &Sinstead");
            } else if (cmd.CaselessEq("compile")) {
                p.Message("Use &T/PCompile &Sinstead");
            } else {
                Help(p);
            }
        }
        
        static void UnloadPlugin(Player p, string name) {
            int matches;
            Plugin plugin = Matcher.Find(p, name, out matches, Plugin.custom, 
                                         null, pln => pln.name, "plugins");
            if (plugin == null) return;
            
            if (Plugin.Unload(plugin)) {
                p.Message("Plugin unloaded successfully.");
            } else {
                p.Message("&WError unloading plugin. See error logs for more information.");
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/Plugin load [filename]");
            p.Message("&HLoad a plugin from your plugins folder");
            p.Message("&T/Plugin unload [name]");
            p.Message("&HUnloads a currently loaded plugin");
            p.Message("&T/Plugin list");
            p.Message("&HLists all loaded plugins");
        }
    }
}
