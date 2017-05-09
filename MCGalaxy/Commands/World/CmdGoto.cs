/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Commands.World {
    public sealed class CmdGoto : Command {
        public override string name { get { return "goto"; } }
        public override string shortcut { get { return "g"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("j"), new CommandAlias("join"),
                    new CommandAlias("gr", "-random"), new CommandAlias("gotorandom", "-random") }; }
        }
        public CmdGoto() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") { Help(p); return; }
            
            if (message.CaselessEq("-random")) {
                string[] files = LevelInfo.AllMapFiles();
                string map = files[new Random().Next(files.Length)];
                
                map = Path.GetFileNameWithoutExtension(map);
                PlayerActions.ChangeMap(p, map);
            } else if (Formatter.ValidName(p, message, "level")) {
                PlayerActions.ChangeMap(p, message);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/goto [map name]");
            Player.Message(p, "%HTeleports yourself to a different level.");
            Player.Message(p, "%T/goto -random");
            Player.Message(p, "%HTeleports yourself to a random level.");
        }
    }
}
