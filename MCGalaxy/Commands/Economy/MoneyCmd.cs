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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    public abstract class MoneyCmd : Command {
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override CommandEnable Enabled { get { return CommandEnable.Economy; } }

        protected bool ParseArgs(Player p, string message, bool canAll,
                                 string action, out MoneyCmdData data) {
            data = default(MoneyCmdData);
            string[] args = message.SplitSpaces(3);
            if (args.Length < 2) { Help(p); return false; }
            data.Name = args[0];
            data.Reason = args.Length > 2 ? args[2] : "";
            
            if (p == null) {
                data.SourceRaw = "(console)"; data.Source = "(console)";
            } else {
                data.SourceRaw = p.color + p.name; data.Source = p.ColoredName;
            }

            int amount = 0;
            data.All = canAll && args[1].CaselessEq("all");
            if (!data.All && !CommandParser.GetInt(p, args[1], "Amount", ref amount, 0))  return false;
            
            data.Amount = amount;
            return true;
        }
        
        protected struct MoneyCmdData {
            public string Source, SourceRaw, Name, Reason;
            public int Amount;
            public bool All;
        }
        
        protected static void MessageAll(Player p, string format, 
                                         string target, MoneyCmdData data) {
            string targetName = PlayerInfo.GetColoredName(p, target);
            string msgReason = data.Reason == "" ? "" : " %S(" + data.Reason + "%S)";
            
            Chat.MessageAll(format, data.Source, targetName, 
                            data.Amount, Server.moneys, msgReason);
        }

        protected static string Format(Player p, string action,
                                       MoneyCmdData data) {
            string entry = "%f" + data.Amount + "%3 " + Server.moneys + action
                + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string reason = data.Reason;
            
            if (reason == "") return entry;
            if (!Database.Backend.EnforcesTextLength)
                return entry + " (" + reason + ")";
            
            int totalLen = entry.Length + 3 + reason.Length;
            if (totalLen >= 256) {
                int truncatedLen = reason.Length - (totalLen - 255);
                reason = reason.Substring(0, truncatedLen);
                Player.Message(p, "Reason too long, truncating to: {0}", reason);
            }
            return entry + " (" + reason + ")";
        }
    }
}
