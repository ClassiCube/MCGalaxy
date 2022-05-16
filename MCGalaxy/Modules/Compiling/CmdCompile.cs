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
#if !DISABLE_COMPILING
using System;
using System.IO;
using MCGalaxy.Commands;
using MCGalaxy.Scripting;

namespace MCGalaxy.Modules.Compiling 
{
    public sealed class CmdCompile : Command2 
    {
        public override string name { get { return "Compile"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Owner; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("PCompile", "plugin") }; }
        }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            bool plugin   = args[0].CaselessEq("plugin");
            string name, lang;

            if (plugin) {
                // compile plugin [name] <language>
                name = args.Length > 1 ? args[1] : "";
                lang = args.Length > 2 ? args[2] : "";
            } else {
                // compile [name] <language>
                name = args[0];
                lang = args.Length > 1 ? args[1] : "";
            }
            
            if (name.Length == 0) { Help(p); return; }
            if (!Formatter.ValidFilename(p, name)) return;
            
            ICompiler compiler = CompilerOperations.GetCompiler(p, lang);
            if (compiler == null) return;
 
            if (plugin) {
                CompilePlugin(p,  name, compiler);
            } else {
                CompileCommand(p, name, compiler);
            }
        }
        
        static void CompilePlugin(Player p, string name, ICompiler compiler) {
            // either "source" or "source1,source2,source3"
            string[] paths = name.SplitComma();
            string dstPath = IScripting.PluginPath(paths[0]);
            
            for (int i = 0; i < paths.Length; i++) 
            {
                 paths[i] = compiler.PluginPath(paths[i]);
            }
            CompilerOperations.Compile(p, compiler, "Plugin", paths, dstPath);
        }
        
        static void CompileCommand(Player p, string name, ICompiler compiler) {
            // either "source" or "source1,source2,source3"
            string[] paths = name.SplitComma();
            string dstPath = IScripting.CommandPath(paths[0]);
            
            for (int i = 0; i < paths.Length; i++) 
            {
                 paths[i] = compiler.CommandPath(paths[i]);
            }
            CompilerOperations.Compile(p, compiler, "Command", paths, dstPath);
        }

        // TODO avoid duplication and use compiler.CommandPath instead
        public override void Help(Player p) {
            p.Message("&T/Compile [command name]");
            p.Message("&HCompiles a .cs file containing a C# command into a DLL");
            p.Message("&H  Compiles from &f" + ICompiler.SOURCE_DIR_COMMANDS + "Cmd&H<name>&f.cs");
            p.Message("&T/Compile plugin [plugin name]");
            p.Message("&HCompiles a .cs file containing a C# plugin into a DLL");
            p.Message("&H  Compiles from &f" + ICompiler.SOURCE_DIR_PLUGINS + "&H<name>&f.cs");
            p.Message("&HFor compiling Visual Basic commands, see &T/help compile VB");
        }
        
        public override void Help(Player p, string message) {
            if (!message.CaselessEq("VB")) { base.Help(p, message); return; }
            
            p.Message("&T/Compile [command name] vb");
            p.Message("&HCompiles a .vb file containing a Visual Basic command into a DLL");
            p.Message("&H  Compiles from &f" + ICompiler.SOURCE_DIR_COMMANDS + "Cmd&H<name>&f.vb");
            p.Message("&T/Compile plugin [plugin name] vb");
            p.Message("&HCompiles a .vb file containing a Visual Basic plugin into a DLL");
            p.Message("&H  Compiles from &f" + ICompiler.SOURCE_DIR_PLUGINS + "&H<name>&f.vb");
        }
    }
}
#endif
