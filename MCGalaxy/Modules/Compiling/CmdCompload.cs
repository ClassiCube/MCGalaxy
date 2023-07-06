/*
    Copyright 2011 MCForge modified by headdetect

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
using MCGalaxy.Commands;
using MCGalaxy.Scripting;

namespace MCGalaxy.Modules.Compiling 
{
    public sealed class CmdCompLoad : CmdCompile 
    {
        public override string name { get { return "CompLoad"; } }
        public override string shortcut { get { return "cml"; } }
        public override CommandAlias[] Aliases { get { return null; } }

        protected override void CompilePlugin(Player p, string[] paths, ICompiler compiler) {
            string dst = IScripting.PluginPath(paths[0]);
            
            UnloadPlugin(p, paths[0]);
            base.CompilePlugin(p, paths, compiler);
            ScriptingOperations.LoadPlugins(p, dst);
        }
        
        static void UnloadPlugin(Player p, string name) {
            Plugin plugin = Plugin.FindCustom(name);
            
            if (plugin == null) return;
            ScriptingOperations.UnloadPlugin(p, plugin);
        }
        
        protected override void CompileCommand(Player p, string[] paths, ICompiler compiler) {
            string cmd = paths[0];
            string dst = IScripting.CommandPath(cmd);
            
            UnloadCommand(p, cmd);
            base.CompileCommand(p, paths, compiler);
            ScriptingOperations.LoadCommands(p, dst);
        }
        
        static void UnloadCommand(Player p, string cmdName) {
            string cmdArgs = "";
            Command.Search(ref cmdName, ref cmdArgs);
            Command cmd = Command.Find(cmdName);
            
            if (cmd == null) return;
            ScriptingOperations.UnloadCommand(p, cmd);
        }

        public override void Help(Player p) {
            p.Message("&T/CompLoad [command]");
            p.Message("&HCompiles and loads (or reloads) a C# command into the server");
            p.Message("&T/CompLoad plugin [plugin]");
            p.Message("&HCompiles and loads (or reloads) a C# plugin into the server");
        }        
    }
}
#endif
