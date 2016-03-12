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
namespace MCGalaxy.Commands {
    
    public sealed class CmdBounty : Command {
        
        public override string name { get { return "bounty"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override bool Enabled { get { return Server.ZombieModeOn; } }       
        public CmdBounty() { }
        
        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length < 2) { Help(p); return; }
            
            Player who = PlayerInfo.FindOrShowMatches(p, args[0]);
            if (who == null) return;
            byte amount = 0;
            if (!byte.TryParse(args[1], out amount)) {
                Player.SendMessage(p, "The bounty amount must be an positive integer less than 256."); return;
            }
            if (p.money < amount) {
                Player.SendMessage(p, "You do not have enough " + Server.moneys + " to place such a large bountry."); return;
            }
            
            BountyData data;
            if (Server.zombie.Bounties.TryGetValue(who.name, out data) && data.Amount >= amount) {
                Player.SendMessage(p, "There is already a larger active bounty for " + who.name + "."); return;
            }
            // TODO here - actually announce the bounty and place it
            // "Looks like someone wants the brain of <name>! An bounty for x <money> was placed on them.
            // "<name> is popular! The bounty on them was increased from <old> to <new> money.
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/bounty [name] [amount]");
            Player.SendMessage(p, "%HSets a bounty on the given player.");
        }
    }
}
