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
    public sealed class CmdCmdCreate : Command {      
        public override string name { get { return "CmdCreate"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();

            IScripting engine = null;
            if (args.Length == 1) {
                engine = IScripting.CS;
            } else if (args[1].CaselessEq("vb")) {
                engine = IScripting.VB;
            } else {
                Help(p); return;
            }
            
            string path = engine.SourcePath(args[0]);
            if (File.Exists(path)) {
                Player.Message(p, "File {0} already exists. Choose another name.", path); return;
            }
            
            try {
                engine.CreateNew(path, args[0]);
            } catch (Exception e) {
                Logger.LogError(e);
                Player.Message(p, "An error occurred creating the class file.");
                return;
            }
            Player.Message(p, "Successfully created a new command class.");
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/CmdCreate [name]");
            Player.Message(p, "%HCreates a dummy C# command named Cmd[Name]");
            Player.Message(p, "%T/CmdCreate [name] vb");
            Player.Message(p, "%HCreates a dummy Visual Basic command named Cmd[Name].");
            Player.Message(p, "%This file can be used as the basis for creating a new command.");
        }
    }
}
