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
using System.IO;
using MCGalaxy.Blocks.Extended;
using MCGalaxy.Events;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using MCGalaxy.Eco;
using System.Globalization;

namespace MCGalaxy.Core {
    internal static class EcoHandlers {
        
        internal static void HandleEcoTransaction(EcoTransaction transaction) {
            switch (transaction.Type) {
                case EcoTransactionType.Purchase:
                    HandlePurchase(transaction); break;
            }
        }
        
        static void HandlePurchase(EcoTransaction transaction) {
            Economy.EcoStats stats = Economy.RetrieveStats(transaction.TargetName);
            stats.TotalSpent += transaction.Amount;
            stats.Purchase = transaction.ItemName + "%3 for %f" + transaction.Amount + " %3" + Server.moneys
                + " on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            
            Player p = PlayerInfo.FindExact(transaction.TargetName);
            if (p != null) Player.Message(p, "Your balance is now &f{0} &3{1}", p.money, Server.moneys);
            Economy.UpdateStats(stats);
        }
    }
}
