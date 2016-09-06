/*
    Copyright 2011 MCForge
    
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
namespace MCGalaxy.Commands {  
    public sealed class CmdFakePay : MoneyCmd {        
        public override string name { get { return "fakepay"; } }
        public override string shortcut { get { return "fpay"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }   
        
        public override void Use(Player p, string message) {
            MoneyCmdData data;
            if (!ParseArgs(p, message, false, "fakepay", out data)) return;   
            Player who = PlayerInfo.FindMatches(p, data.Name);
            if (who == null) return;     
            if (data.Amount >= 16777215) { Player.Message(p, "You can only fakepay up to 16777215."); return; }

            MessageAll(p, "{0} %Sgave {1} &f{2} &3{3}{4}", who.name, data);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/fakepay [name] [amount] <reason>");
            Player.Message(p, "%HSends a fake %T/give %Hchange message.");
        }
    }
}

