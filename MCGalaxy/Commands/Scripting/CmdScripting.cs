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

namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdScripting : Command {
        public override string name { get { return "scripting"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("pcreate", "pcreate"),
                    new CommandAlias("pload", "pload"), new CommandAlias("punload", "punload") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] parts = message.SplitSpaces(2);
            if (parts.Length == 1) { Help(p); return; }
            if (!Formatter.ValidName(p, parts[1], "plugin")) return;
            
            if (parts[0].CaselessEq("pload")) {
                LoadPlugin(p, parts[1]);
            } else if (parts[0].CaselessEq("punload")) {
                UnloadPlugin(p, parts[1]);
            } else if (parts[0].CaselessEq("pcreate")) {
                CreatePlugin(p, parts[1]);
            } else {
                Help(p);
            }
        }
        
        static void LoadPlugin(Player p, string name) {
            if (File.Exists("plugins/" + name + ".dll")) {
                Plugin.Load(name, false);
            } else {
                Player.Message(p, "File &9" + name + ".dll %Snot found.");
            }
        }
        
        static void UnloadPlugin(Player p, string name) {
            Plugin plugin = Plugin.Find(name);
            if (Plugin.core.Contains(plugin)) {
                Player.Message(p, plugin.name + " is a core plugin and cannot be unloaded.");
                return;
            }
            
            if (plugin != null) {
                Plugin.Unload(plugin, false);
            } else {
                Player.Message(p, "Loaded plugins: " + Plugin.all.Join(pl => pl.name));
            }
        }
        
        static void CreatePlugin(Player p, string name) {
            Player.Message(p, "Creating a plugin example source");
            string user = p == null ? ServerConfig.name : p.name;
            if (!Directory.Exists("plugin_source"))
                Directory.CreateDirectory("plugin_source");
            
            string syntax = pluginSrc.Replace(@"\t", "\t");
            syntax = String.Format(syntax, name, user, Server.VersionString);
            File.WriteAllText("plugin_source/" + name + ".cs", syntax);
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
            Player.Message(p, "%T/scripting pcreate [name]");
            Player.Message(p, "%HCreate a example .cs file!");
            Player.Message(p, "%T/scripting pload [filename]");
            Player.Message(p, "%HLoad a plugin from your plugins folder.");
            Player.Message(p, "%T/scripting punload [name]");
            Player.Message(p, "%HUnloads a currently loaded plugin.");
        }
    }
}
