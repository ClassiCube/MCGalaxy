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

namespace MCGalaxy.Games.ZS {
    internal static class HUD {
        
        internal static void UpdateAllPrimary(ZombieGame game) {
            int left = (int)(game.RoundEnd - DateTime.UtcNow).TotalSeconds;
            string status = FormatPrimary(game, left);
            MessageAll(game, CpeMessageType.Status1, status);
        }
        
        internal static void UpdatePrimary(ZombieGame game, Player p) {
            int left = (int)(game.RoundEnd - DateTime.UtcNow).TotalSeconds;
            string status = FormatPrimary(game, left);
            p.SendCpeMessage(CpeMessageType.Status1, status);
        }

        internal static void UpdateAllSecondary(ZombieGame game) {
            string status = FormatSecondary(game);
            MessageAll(game, CpeMessageType.Status2, status);
        }
        
        internal static void UpdateSecondary(ZombieGame game, Player p) {
            string status = FormatSecondary(game);
            p.SendCpeMessage(CpeMessageType.Status2, status);
        }
        
        internal static void UpdateTertiary(Player p) {
            string status = FormatTertiary(p);
            p.SendCpeMessage(CpeMessageType.Status3, status);
        }
        
        
        internal static string GetTimeLeft(int seconds) {
            if (seconds < 0) return "";
            if (seconds <= 10) return "10s left";
            if (seconds <= 30) return "30s left";
            if (seconds <= 60) return "1m left";
            return ((seconds + 59) / 60) + "m left";
        }
        
        internal static void Reset(Player p) {
            p.SendCpeMessage(CpeMessageType.Status1, "");
            p.SendCpeMessage(CpeMessageType.Status2, "");
            p.SendCpeMessage(CpeMessageType.Status3, "");
            p.SendCpeMessage(CpeMessageType.BottomRight1, "");
            p.SendCpeMessage(CpeMessageType.BottomRight2, "");
            p.SendCpeMessage(CpeMessageType.BottomRight3, "");
        }

        
        static string FormatPrimary(ZombieGame game, int seconds) {
            string timespan = GetTimeLeft(seconds);
            if (timespan.Length > 0) {
                const string format = "&a{0} %Salive %S({2}, map: {1})";
                return String.Format(format, game.Alive.Count, game.CurLevelName, timespan);
            } else {
                const string format = "&a{0} %Salive %S(map: {1})";
                return String.Format(format, game.Alive.Count, game.CurLevelName);
            }
        }
        
        static string FormatSecondary(ZombieGame game) {
            string pillar = "%SPillaring " + (game.CurLevel.Pillaring ? "&aYes" : "&cNo");
            string type = "%S, Type is &a" + game.CurLevel.BuildType;
            return pillar + type;
        }

        static string FormatTertiary(Player p) {
            string money = "&a" + p.money + " %S" + ServerConfig.Currency;
            string state = ", you are " + (p.Game.Infected ? "&cdead" : "&aalive");
            return money + state;
        }
        
        static void MessageAll(ZombieGame game, CpeMessageType type, string message) {
            if (!game.Running) return;
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (!p.level.name.CaselessEq(game.CurLevelName)) continue;
                p.SendCpeMessage(type, message);
            }
        }
    }
}
