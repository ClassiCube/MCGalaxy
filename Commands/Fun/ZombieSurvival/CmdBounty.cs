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

namespace MCGalaxy.Commands {
    
    public sealed class CmdBounty : Command {
        
        public override string name { get { return "bounty"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie; } }
        
        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 2) { Help(p); return; }
            
            Player who = PlayerInfo.FindMatches(p, args[0]);
            if (who == null) return;
            byte amount = 0;
            if (!byte.TryParse(args[1], out amount)) {
                Player.Message(p, "The bounty amount must be an positive integer less than 256."); return;
            }
            if (p.money < amount) {
                Player.Message(p, "You do not have enough " + Server.moneys + " to place such a large bountry."); return;
            }
            
            BountyData old;
            if (Server.zombie.Bounties.TryGetValue(who.name, out old) && old.Amount >= amount) {
                Player.Message(p, "There is already a larger active bounty for " + who.name + "."); return;
            }
            
            if (old == null) {
                Player.GlobalMessage("Looks like someone really wants the brains of " + who.ColoredName
                                     + "%S! A bounty of &a" + amount + " %S" + Server.moneys + " was placed on them.");
            } else {
                Player.GlobalMessage(who.ColoredName + " %Sis popular! The bounty on them was increased from &a" + old.Amount + 
                                     " %Sto &a" + amount + " %S" + Server.moneys + ".");
            }
            Server.zombie.Bounties[who.name] = new BountyData(p, amount);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/bounty [name] [amount]");
            Player.Message(p, "%HSets a bounty on the given player.");
        }
    }
}
