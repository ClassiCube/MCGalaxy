/*
    Copyright 2011 MCForge
        
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
namespace MCGalaxy.Commands
{
    public sealed class CmdLowlag : Command
    {
        public override string name { get { return "lowlag"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdLowlag() { }

        public override void Use(Player p, string message) {
            if (message == "" && Server.updateTimer.Interval > 1000) {
                Server.PositionInterval = 100;
                Player.GlobalMessage("&dLow lag %Sturned &cOFF %S- positions update every &b100%S ms.");
            } else if (message == "") {
                Server.PositionInterval = 2000;
                Player.GlobalMessage("&dLow lag %Sturned &aON %S- positions update every &b2000%S ms.");
            } else {
                int interval;
                if (!int.TryParse(message, out interval)) {
                    Player.Message(p, "Interval given must be an integer."); return;
                }
                if (interval < 20 || interval > 2000) {
                    Player.Message(p, "Interval must be between 20 and 2000 milliseconds."); return;
                }
                Server.PositionInterval = interval;
                Player.GlobalMessage("Positions now update every &b" + interval + " %Smilliseconds.");
            }
            Server.updateTimer.Interval = Server.PositionInterval;
            SrvProperties.Save();
        }

        public override void Help(Player p) {
            Player.Message(p, "%T/lowlag [interval in milliseconds]");
            Player.Message(p, "%HSets the interval between sending of position packets.");
            Player.Message(p, "%HIf no interval is given, then 2000 ms is used if the current interval" + 
                               " is less than 1000 ms, otherwise 200 ms is used for the interval.");
        }
    }
}
