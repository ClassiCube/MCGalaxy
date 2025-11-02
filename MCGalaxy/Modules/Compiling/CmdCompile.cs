/*
    Copyright 2011 MCForge
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
#if !MCG_STANDALONE
using System;
using MCGalaxy.Commands;
using MCGalaxy.Scripting;
using System.IO;

namespace MCGalaxy.Modules.Compiling 
{
    class CmdCompile : Command2 
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
            
            // either "source" or "source1,source2,source3"
            string[] paths = name.SplitComma();
 
            if (plugin) {
                CompilePlugin(p,  paths, compiler);
            } else {
                CompileCommand(p, paths, compiler);
            }
        }
        
        protected virtual void CompilePlugin(Player p, string[] paths, ICompiler compiler) {
            string pln     = paths[0];
            string dstPath = IScripting.PluginPath(pln);
            
            for (int i = 0; i < paths.Length; i++) 
            {
                 paths[i] = compiler.PluginPath(paths[i]);
            }
            paths = TryDirectory(ICompiler.PLUGINS_SOURCE_DIR, pln, paths, compiler);
            
            CompilerOperations.Compile(p, compiler, "Plugin", paths, dstPath);
        }
        
        protected virtual void CompileCommand(Player p, string[] paths, ICompiler compiler) {
            string cmd     = paths[0];
            string dstPath = IScripting.CommandPath(cmd);
            
            for (int i = 0; i < paths.Length; i++) 
            {
                 paths[i] = compiler.CommandPath(paths[i]);
            }
            paths = TryDirectory(ICompiler.COMMANDS_SOURCE_DIR, cmd, paths, compiler);

            CompilerOperations.Compile(p, compiler, "Command", paths, dstPath);
        }
        
        // If first source file doesn't exist, try treating it as a directory instead
        string[] TryDirectory(string root, string name, string[] srcPaths, ICompiler compiler) {
            if (File.Exists(srcPaths[0])) return srcPaths;
            
            string dir = Path.Combine(root, name);
            if (!Directory.Exists(dir)) return srcPaths;
            
            return Directory.GetFiles(dir, "*" + compiler.FileExtension);
        }

        public override void Help(Player p) {
            ICompiler compiler = ICompiler.Compilers[0];
            p.Message("&T/Compile [command name]");
            p.Message("&HCompiles a .cs file containing a C# command into a DLL");
            p.Message("&H  Compiles from &f{0}", compiler.CommandPath("&H<name>&f"));
            p.Message("&T/Compile plugin [plugin name]");
            p.Message("&HCompiles a .cs file containing a C# plugin into a DLL");
            p.Message("&H  Compiles from &f{0}", compiler.PluginPath("&H<name>&f"));
        }
    }
}
#endif
