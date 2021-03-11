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
using System.Globalization;
using MCGalaxy.Events.EconomyEvents;
using MCGalaxy.Eco;
using MCGalaxy.SQL;

namespace MCGalaxy.Core {
    internal static class EcoHandlers {
        
        internal static void HandleEcoTransaction(EcoTransaction transaction) {
            switch (transaction.Type) {
                case EcoTransactionType.Purchase:
                    HandlePurchase(transaction); break;
                case EcoTransactionType.Take:
                    HandleTake(transaction); break;
                case EcoTransactionType.Give:
                    HandleGive(transaction); break;
                case EcoTransactionType.Payment:
                    HandlePayment(transaction); break;
            }
        }
        
        static void HandlePurchase(EcoTransaction data) {
            Economy.EcoStats stats = Economy.RetrieveStats(data.TargetName);
            stats.TotalSpent += data.Amount;
            stats.Purchase = data.ItemDescription + "%3 for %f" + data.Amount + " %3" + Server.Config.Currency
                + " on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            
            Player p = PlayerInfo.FindExact(data.TargetName);
            if (p != null) p.Message("Your balance is now &f{0} &3{1}", p.money, Server.Config.Currency);
            Economy.UpdateStats(stats);
        }
        
        static void HandleTake(EcoTransaction data) {
            MessageAll("{0} &Stook &f{2} &3{3} &Sfrom {1}{4}", data);
            Economy.EcoStats stats = Economy.RetrieveStats(data.TargetName);
            stats.Fine = Format(" by " + data.Source.name, data);
            Economy.UpdateStats(stats);
        }
        
        static void HandleGive(EcoTransaction data) {
            MessageAll("{0} &Sgave {1} &f{2} &3{3}{4}", data);
            Economy.EcoStats stats = Economy.RetrieveStats(data.TargetName);
            stats.Salary = Format(" by " + data.Source.name, data);
            Economy.UpdateStats(stats);
        }
        
        static void HandlePayment(EcoTransaction data) {
            MessageAll("{0} &Spaid {1} &f{2} &3{3}{4}", data);
            Economy.EcoStats stats = Economy.RetrieveStats(data.TargetName);
            stats.Salary = Format(" by " + data.Source.name, data);
            Economy.UpdateStats(stats);
            
            if (data.Source.IsSuper) return;
            
            stats = Economy.RetrieveStats(data.Source.name);
            stats.Payment = Format(" to " + data.TargetName, data);
            Economy.UpdateStats(stats);
            data.Source.SetMoney(data.Source.money - data.Amount);
        }
        
        
        static void MessageAll(string format, EcoTransaction data) {
            string reason = data.Reason == null ? "" : " &S(" + data.Reason + "&S)";
            string msg = string.Format(format, data.Source.ColoredName, data.TargetFormatted,
                                       data.Amount, Server.Config.Currency, reason);
            Chat.MessageGlobal(msg);
        }

        static string Format(string action, EcoTransaction data) {
            string entry = "%f" + data.Amount + "%3 " + Server.Config.Currency + action
                + "%3 on %f" + DateTime.Now.ToString(CultureInfo.InvariantCulture);
            string reason = data.Reason;
            
            if (reason == null) return entry;
            if (!Database.Backend.EnforcesTextLength)
                return entry + " (" + reason + ")";
            
            int totalLen = entry.Length + 3 + reason.Length;
            if (totalLen >= 256) {
                int truncatedLen = reason.Length - (totalLen - 255);
                reason = reason.Substring(0, truncatedLen);
                data.Source.Message("Reason too long, truncating to: {0}", reason);
            }
            return entry + " (" + reason + ")";
        }
    }
}
