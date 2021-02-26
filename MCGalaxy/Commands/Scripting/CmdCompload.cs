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
namespace MCGalaxy.Commands.Scripting {
    public sealed class CmdCompLoad : Command2 {
        public override string name { get { return "CompLoad"; } }
        public override string shortcut { get { return "cml"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }
        public override bool MessageBlockRestricted { get { return true; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0) { Help(p); return; }

            if (args.Length == 1 || args[1].CaselessEq("vb")) {
                Command.Find("Compile").Use(p, message, data);
                Command.Find("CmdLoad").Use(p, args[0], data);
                Command.Find("Help").Use(p, args[0], data);
            } else { 
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            p.Message("&T/CompLoad [command]");
            p.Message("&HCompiles and loads a C# command into the server for use.");
            p.Message("&T/CompLoad [command] vb");
            p.Message("&HCompiles and loads a Visual basic command into the server for use.");
        }        
    }
}
