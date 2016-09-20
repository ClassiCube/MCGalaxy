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
using System;
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Eco;

namespace MCGalaxy.Commands {
    
    public sealed class CmdLottery : Command {
        public override string name { get { return "lottery"; } }
        public override string shortcut { get { return "luck"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandEnable Enabled { get { return CommandEnable.Zombie | CommandEnable.Lava; } }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (!p.EnoughMoney(10)) {
                Player.Message(p, "You need &f10 " + Server.moneys + " %Sto enter the lottery."); return;
            }
            
            string[] players = Server.zombie.Lottery.Items;
            for (int i = 0; i < players.Length; i++) {
                if (players[i].CaselessEq(p.name)) {
                    Player.Message(p, "You are already in the lottery, which has &a"
                                       + players.Length + " %Splayers in it."); return;
                }
            }
            
            p.SetMoney(p.money - 10);
            Server.zombie.Lottery.Add(p.name);
            if (Server.zombie.CurLevel != null)
                Server.zombie.CurLevel.ChatLevel(p.ColoredName + " %Sentered the lottery");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/lottery %H-Enters lottery for &f10 " + Server.moneys);
            Player.Message(p, "%HThe winner is calculated at the end of each round.");
            Player.Message(p, "%HYou are &cnot refunded %Hif you disconnect.");
        }
    }
}
