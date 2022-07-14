/*
    Copyright 2011 MCForge modified by headdetect

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
using MCGalaxy.Commands;
using MCGalaxy.Scripting;

namespace MCGalaxy.Modules.Compiling 
{
    public sealed class CmdCompLoad : CmdCompile 
    {
        public override string name { get { return "CompLoad"; } }
        public override string shortcut { get { return "cml"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override CommandAlias[] Aliases {  get { return null; } }

        protected override void OnCommandCompiled(Player p, string name, string path) {
            ScriptingOperations.LoadCommands(p, path);
            // TODO print command help directly
            string cmd = name.SplitComma()[0];
            Command.Find("Help").Use(p, cmd, p.DefaultCmdData);
        }

        protected override void OnPluginCompiled(Player p, string name, string path) {
            ScriptingOperations.LoadPlugins(p, path);
        }

        public override void Help(Player p) {
            p.Message("&T/CompLoad [command]");
            p.Message("&HCompiles and loads a C# command into the server for use.");
            p.Message("&T/CompLoad plugin [plugin]");
            p.Message("&HCompiles and loads a C# plugin into the server for use.");
        }        
    }
}
#endif
