/*
    Copyright 2015 MCGalaxy
    
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {    
    public sealed class CmdBounty : Command2 {        
        public override string name { get { return "Bounty"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        
        public override void Use(Player p, string message, CommandData data) {
            string[] args = message.SplitSpaces();
            if (args.Length < 2) { Help(p); return; }
            
            Player target = PlayerInfo.FindMatches(p, args[0]);
            if (target == null) return;
            
            int amount = 0;
            if (!CommandParser.GetInt(p, args[1], "Bounty amount", ref amount, 1, 256)) return;
            
            if (p.money < amount) {
                p.Message("You do not have enough " + Server.Config.Currency + " to place such a large bountry."); return;
            }
            
            BountyData old = ZSGame.Instance.FindBounty(target.name);
            if (old != null && old.Amount >= amount) {
                p.Message("There is already a larger active bounty for " + p.FormatNick(target)); return;
            }
            
            string msg;
            if (old == null) {
                msg = string.Format("Looks like someone really wants the brains of {0}&S! A bounty of &a{1} &S{2} was placed on them.", 
                                    target.ColoredName, amount, Server.Config.Currency);
            } else {
                msg = string.Format("{0} &Sis popular! The bounty on them was increased from &a{3} &Sto &a{1} &S{2}.", 
                                    target.ColoredName, amount, Server.Config.Currency, old.Amount);
                ZSGame.Instance.Bounties.Remove(old);
            }
            ZSGame.Instance.Map.Message(msg);
            
            ZSGame.Instance.Bounties.Add(new BountyData(p.name, target.name, amount));
            p.SetMoney(p.money - amount);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Bounty [name] [amount]");
            p.Message("&HSets a bounty on the given player.");
        }
    }
}
