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
using System.CodeDom.Compiler;
using System.IO;
using MCGalaxy.Scripting;

namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdPlugin : Command2 {
        public override string name { get { return "Plugin"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("PLoad", "load"), new CommandAlias("PUnload", "unload"),
                    new CommandAlias("PCreate", "create"), new CommandAlias("PCompile", "compile"),
                    new CommandAlias("Plugins", "list") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (IsListCommand(message)) {
                p.Message("Loaded plugins: " + Plugin.all.Join(pl => pl.name));
                return;
            }
            
            string[] args = message.SplitSpaces(3);
            if (args.Length == 1) { Help(p); return; }
            
            string cmd = args[0], name = args[1];
            if (!Formatter.ValidName(p, name, "plugin")) return;
            string language = args.Length > 2 ? args[2] : "";
            
            if (cmd.CaselessEq("load")) {
                LoadPlugin(p, name);
            } else if (cmd.CaselessEq("unload")) {
                UnloadPlugin(p, name);
            } else if (cmd.CaselessEq("create")) {
                CreatePlugin(p, name, language);
            } else if (cmd.CaselessEq("compile")) {
                CompilePlugin(p, name, language);
            } else {
                Help(p);
            }
        }
        
        static void CompilePlugin(Player p, string name, string language) {
            ICompiler engine = ICompiler.Lookup(language, p);
            if (engine == null) return;
            
            string srcPath = "plugins/" + name + engine.FileExtension;
            string dstPath = IScripting.PluginPath(name);  
            if (!File.Exists(srcPath)) {
                p.Message("File &9{0} &Snot found.", srcPath); return;
            }
               
            CompilerResults results = engine.Compile(srcPath, dstPath);
            if (!results.Errors.HasErrors) {
                p.Message("Plugin compiled successfully.");
            } else {
                ICompiler.SummariseErrors(results, p);
                p.Message("&WCompilation error. See " + ICompiler.ErrorPath + " for more information.");
            }
        }
        
        static void LoadPlugin(Player p, string name) {
            string path = IScripting.PluginPath(name);
            if (!File.Exists(path)) {
                p.Message("File &9{0} &Snot found.", path); return;
            }
            
            if (IScripting.LoadPlugin(path, false)) {
                p.Message("Plugin loaded successfully.");
            } else {
                p.Message("&WError loading plugin. See error logs for more information.");
            }
        }
        
        static void UnloadPlugin(Player p, string name) {
            int matches;
            Plugin plugin = Matcher.Find(p, name, out matches, Plugin.all, 
                                         null, pln => pln.name, "plugins");
            if (plugin == null) return;
            
            if (Plugin.core.Contains(plugin)) {
                p.Message(plugin.name + " is a core plugin and cannot be unloaded.");
                return;
            }
            
            if (plugin != null) {
                if (Plugin.Unload(plugin, false)) {
                    p.Message("Plugin unloaded successfully.");
                } else {
                    p.Message("&WError unloading plugin. See error logs for more information.");
                }
            } else {
                p.Message("Loaded plugins: " + Plugin.all.Join(pl => pl.name));
            }
        }
        
        static void CreatePlugin(Player p, string name, string language) {
            ICompiler engine = ICompiler.Lookup(language, p);
            if (engine == null) return;
            
            string path = engine.PluginPath(name);
            p.Message("Creating a plugin example source");
            
            string creator = p.IsSuper ? Server.Config.Name : p.truename;
            string source  = engine.GenExamplePlugin(name, creator);
            File.WriteAllText(path, source);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Plugin create [name]");
            p.Message("&HCreate a example .cs plugin file");
            p.Message("&T/Plugin compile [name]");
            p.Message("&HCompiles a .cs plugin file");
            p.Message("&T/Plugin load [filename]");
            p.Message("&HLoad a plugin from your plugins folder");
            p.Message("&T/Plugin unload [name]");
            p.Message("&HUnloads a currently loaded plugin");
            p.Message("&T/Plugin list");
            p.Message("&HLists all loaded plugins");
        }
    }
}
