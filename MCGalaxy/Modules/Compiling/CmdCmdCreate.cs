/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified for use with MCGalaxy)
 
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

namespace MCGalaxy.Modules.Compiling
{
    public sealed class CmdCmdCreate : Command2 
    {
        public override string name { get { return "CmdCreate"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Owner; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            if (!Formatter.ValidFilename(p, args[0])) return;

            string language  = args.Length > 1 ? args[1] : "";
            ICompiler engine = CompilerOperations.GetCompiler(p, language);
            if (engine == null) return;
            
            string path = engine.CommandPath(args[0]);
            if (File.Exists(path)) {
                p.Message("File {0} already exists. Choose another name.", path); return;
            }
            
            string source = engine.GenExampleCommand(args[0]);
            File.WriteAllText(path, source);
            p.Message("Successfully saved example command &fCmd{0} &Sto {1}", args[0], path);
        }

        public override void Help(Player p) {
            p.Message("&T/CmdCreate [name]");
            p.Message("&HCreates an example C# command named Cmd[Name]");
            p.Message("&T/CmdCreate [name] vb");
            p.Message("&HCreates an example Visual Basic command named Cmd[Name]");
            p.Message("&TThis file can be used as the basis for creating a new command.");
        }
    }
}
#endif
