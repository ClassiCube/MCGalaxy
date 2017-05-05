/* 
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun
{
    public sealed class CmdCTF : Command
    {
        public override string name { get { return "ctf"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdCTF() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            
            if (message.CaselessEq("start")) {
                if (Server.ctf == null)  {
                    Player.Message(p, "Initialising CTF..");
                    Server.ctf = new CTFGame();
                }
                
                if (!Server.ctf.Start(p)) return;
                Chat.MessageGlobal("A CTF GAME IS STARTING AT CTF! TYPE /goto CTF to join!");
            } else if (message == "stop")  {
                if (Server.ctf == null || !Server.ctf.started) {
                    Player.Message(p, "No CTF game is active."); return;
                }
                Server.ctf.Stop();
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ctf start/stop");
            Player.Message(p, "%HStarts/Stops the CTF game.");
        }
    }
}
