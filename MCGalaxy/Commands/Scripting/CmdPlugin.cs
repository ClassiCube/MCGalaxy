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
            
            string[] parts = message.SplitSpaces(2);
            if (parts.Length == 1) { Help(p); return; }
            if (!Formatter.ValidName(p, parts[1], "plugin")) return;
            
            if (parts[0].CaselessEq("load")) {
                LoadPlugin(p, parts[1]);
            } else if (parts[0].CaselessEq("unload")) {
                UnloadPlugin(p, parts[1]);
            } else if (parts[0].CaselessEq("create")) {
                CreatePlugin(p, parts[1]);
            } else if (parts[0].CaselessEq("compile")) {
                CompilePlugin(p, parts[1]);
            } else {
                Help(p);
            }
        }
        
        static void CompilePlugin(Player p, string name) {
            IScripting engine = IScripting.CS;
            string srcPath = "plugins/" + name + engine.Ext;
            string dstPath = IScripting.PluginPath(name);
            
            if (File.Exists(srcPath)) {
                if (engine.Compile(srcPath, dstPath)) {
                    p.Message("Plugin compiled successfully.");
                } else {
                    p.Message("Compilation error. See " + IScripting.ErrorPath + " for more information.");
                }
            } else {
                p.Message("File &9{0} %Snot found.", srcPath);
            }
        }
        
        static void LoadPlugin(Player p, string name) {
            string path = IScripting.PluginPath(name);
            if (File.Exists(path)) {
                if (Plugin.Load(name, false)) {
                    p.Message("Plugin loaded successfully.");
                } else {
                    p.Message("%WError loading plugin. See error logs for more information.");
                }
            } else {
                p.Message("File &9{0} %Snot found.", path);
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
                    p.Message("%WError unloading plugin. See error logs for more information.");
                }
            } else {
                p.Message("Loaded plugins: " + Plugin.all.Join(pl => pl.name));
            }
        }
        
        static void CreatePlugin(Player p, string name) {
            p.Message("Creating a plugin example source");
            string creator = p.IsSuper ? ServerConfig.Name : p.name;
            string syntax = pluginSrc.Replace(@"\t", "\t");
            syntax = string.Format(syntax, name, creator, Server.VersionString);
            File.WriteAllText("plugins/" + name + ".cs", syntax);
        }
        
        const string pluginSrc =
            @"//This is an example plugin source!
using System;
namespace MCGalaxy
{{
\tpublic class {0} : Plugin
\t{{
\t\tpublic override string name {{ get {{ return ""{0}""; }} }}
\t\tpublic override string website {{ get {{ return ""www.example.com""; }} }}
\t\tpublic override string MCGalaxy_Version {{ get {{ return ""{2}""; }} }}
\t\tpublic override int build {{ get {{ return 100; }} }}
\t\tpublic override string welcome {{ get {{ return ""Loaded Message!""; }} }}
\t\tpublic override string creator {{ get {{ return ""{1}""; }} }}
\t\tpublic override bool LoadAtStartup {{ get {{ return true; }} }}

\t\tpublic override void Load(bool startup)
\t\t{{
\t\t\t//LOAD YOUR PLUGIN WITH EVENTS OR OTHER THINGS!
\t\t}}
                        
\t\tpublic override void Unload(bool shutdown)
\t\t{{
\t\t\t//UNLOAD YOUR PLUGIN BY SAVING FILES OR DISPOSING OBJECTS!
\t\t}}
                        
\t\tpublic override void Help(Player p)
\t\t{{
\t\t\t//HELP INFO!
\t\t}}
\t}}
}}";
        
        public override void Help(Player p) {
            p.Message("%T/Plugin create [name]");
            p.Message("%HCreate a example .cs plugin file");
            p.Message("%T/Plugin compile [name]");
            p.Message("%HCompiles a .cs plugin file");
            p.Message("%T/Plugin load [filename]");
            p.Message("%HLoad a plugin from your plugins folder");
            p.Message("%T/Plugin unload [name]");
            p.Message("%HUnloads a currently loaded plugin");
            p.Message("%T/Plugin list");
            p.Message("%HLists all loaded plugins");
        }
    }
}
