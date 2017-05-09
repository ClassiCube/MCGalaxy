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
namespace MCGalaxy.Commands.Building {
    public sealed class CmdPaint : Command {
        public override string name { get { return "paint"; } }
        public override string shortcut { get { return "p"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdPaint() { }

        public override void Use(Player p, string message)
        {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message != "") { Help(p); return; }
            p.painting = !p.painting;
            
            string type = p.painting ? "&aON" : "&cOFF";
            Player.Message(p, "Painting mode: " + type + "%S.");
            p.modeType = 0;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/paint");
            Player.Message(p, "%HTurns painting mode on/off.");
            Player.Message(p, "%HWhen paint mode is on, any block you delete is replaced by the block you are holding.");
        }
    }
}
