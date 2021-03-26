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
using System;
using System.IO;
using MCGalaxy.Scripting;

namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdCmdCreate : Command2 {      
        public override string name { get { return "CmdCreate"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();

            string language  = args.Length > 1 ? args[1] : "";
            ICompiler engine = ICompiler.Lookup(language, p);
            if (engine == null) return;
            
            string path = engine.CommandPath(args[0]);
            if (File.Exists(path)) {
                p.Message("File {0} already exists. Choose another name.", path); return;
            }
            
            try {
            	string source = engine.GenExampleCommand(args[0]);
            	File.WriteAllText(path, source);
            } catch (Exception ex) {
                Logger.LogError("Error saving new command to " + path, ex);
                p.Message("An error occurred creating the command.");
                return;
            }
            p.Message("Successfully created a new command class.");
        }

        public override void Help(Player p) {
            p.Message("&T/CmdCreate [name]");
            p.Message("&HCreates a dummy C# command named Cmd[Name]");
            p.Message("&T/CmdCreate [name] vb");
            p.Message("&HCreates a dummy Visual Basic command named Cmd[Name].");
            p.Message("&TThis file can be used as the basis for creating a new command.");
        }
    }
}
