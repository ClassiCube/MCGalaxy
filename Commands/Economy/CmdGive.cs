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
using System;
using System.Globalization;
namespace MCGalaxy.Commands
{
    public sealed class CmdGive : Command
    {
        public override string name { get { return "give"; } }
        public override string shortcut { get { return "gib"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdGive() { }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length != 2) { Help(p); return; }

            string giver = null, giverRaw = null;
            if (p == null) { giverRaw = "(console)"; giver = "(console)"; } 
            else { giverRaw = p.color + p.name; giver = p.FullName; }

            int amount;
            if (!int.TryParse(args[1], out amount)) {
                Player.SendMessage(p, "Amount must be an integer."); return;
            }
            if (amount < 0) { Player.SendMessage(p, "Cannot give negative %3" + Server.moneys); return; }
            Player who = PlayerInfo.Find(args[0]);
            if (p != null && p == who) { Player.SendMessage(p, "You cannot give yourself %3" + Server.moneys); return; }
            Economy.EcoStats ecos;

            if (who == null) {
                OfflinePlayer off = PlayerInfo.FindOffline(args[0]);
                if (off == null) { Player.SendMessage(p, "The player \"&a" + args[0] + "%S\" was not found at all."); return; }

                ecos = Economy.RetrieveEcoStats(args[0]);
                if (ReachedMax(p, ecos.money, amount)) return;
                Player.GlobalMessage(giver + " %Sgave %f" + ecos.playerName + "%S(offline)" + " %f" + amount + " %3" + Server.moneys);
            } else {
                if (ReachedMax(p, who.money, amount)) return;
                ecos.money = who.money;
                who.money += amount;
                who.OnMoneyChanged();
                ecos = Economy.RetrieveEcoStats(who.name);
                Player.GlobalMessage(giver + " %Sgave " + who.FullName + " %f" + amount + " %3" + Server.moneys);
            }
            
            ecos.money += amount;
            ecos.salary = "%f" + amount + "%3 " + Server.moneys + " by " +
                giverRaw + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            Economy.UpdateEcoStats(ecos);
        }
        
        static bool ReachedMax(Player p, int current, int amount) {
            if (current + amount > 16777215) {
                Player.SendMessage(p, "%cPlayers cannot have over %316,777,215 %3" + Server.moneys); return true;
            }
            return false;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/give [player] <amount>");
            Player.SendMessage(p, "%HGives [player] <amount> %3" + Server.moneys);
        }
    }
}
