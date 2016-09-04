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
using System;
using System.Globalization;

namespace MCGalaxy.Commands {
    public abstract class MoneyCmd : Command {
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }        

        protected bool ParseArgs(Player p, string message, bool canAll, 
                                 string action, out MoneyCmdData data) {
            data = default(MoneyCmdData);
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return false; }
            data.Name = args[0];
            
            if (p == null) { 
                data.SourceRaw = "(console)"; data.Source = "(console)"; 
            } else { 
                data.SourceRaw = p.color + p.name; data.Source = p.ColoredName; 
            }

            int amount = 0;
            data.All = canAll && args[1].CaselessEq("all");
            if (!data.All && !int.TryParse(args[1], out amount)) { 
                Player.Message(p, "Amount to {0} must be an integer.", action); return false; 
            }
            if (amount < 0) { 
                Player.Message(p, "You cannot {0} negative &3" + Server.moneys, action); return false; 
            }
            data.Amount = amount;
            return true;
        }
        
        protected struct MoneyCmdData {
            public string Source, SourceRaw, Name;
            public int Amount;
            public bool All;
        }
    }
}
