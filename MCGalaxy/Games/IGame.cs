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
using MCGalaxy.Commands.World;

namespace MCGalaxy.Games {

    public abstract class IGame {
        public Level Map;
        public bool Running;
        public abstract string GameName { get; }
        
        public static VolatileArray<IGame> RunningGames = new VolatileArray<IGame>(false);
        public static IGame GameOn(Level lvl) {
            if (lvl == null) return null;
            IGame[] games = RunningGames.Items;
            
            foreach (IGame game in games) {
                if (game.Map == lvl) return game;
            }
            return null;
        }

        public virtual bool HandlesChatMessage(Player p, string message) { return false; }
        public virtual void PlayerJoinedGame(Player p) { }
        public virtual void PlayerLeftGame(Player p) { }
        
        public virtual void AdjustPrefix(Player p, ref string prefix) { }
        public abstract void End();
        public abstract void EndRound();
        
        
        /// <summary> Resets all CPE status messages to blank. </summary>
        protected void ResetStatus(Player p) {
            p.SendCpeMessage(CpeMessageType.Status1, "");
            p.SendCpeMessage(CpeMessageType.Status2, "");
            p.SendCpeMessage(CpeMessageType.Status3, "");
        }
        
        /// <summary> Sends a message of the given type to all players on the level this game is running on. </summary>
        public void MessageMap(CpeMessageType type, string message) {
            if (!Running) return;
            Player[] online = PlayerInfo.Online.Items;
            
            foreach (Player p in online) {
                if (p.level != Map) continue;
                p.SendCpeMessage(type, message);
            }
        }
        
        protected virtual string FormatStatus1(Player p) { return ""; }
        protected virtual string FormatStatus2(Player p) { return ""; }
        protected virtual string FormatStatus3(Player p) { return ""; }
        
        public void UpdateAllStatus1() { UpdateAllStatus(CpeMessageType.Status1); }
        public void UpdateAllStatus2() { UpdateAllStatus(CpeMessageType.Status2); }
        public void UpdateAllStatus3() { UpdateAllStatus(CpeMessageType.Status3); }
        
        public void UpdateAllStatus() {
            UpdateAllStatus1();
            UpdateAllStatus2();
            UpdateAllStatus3();
        }
        
        void UpdateAllStatus(CpeMessageType status) {
            Player[] online = PlayerInfo.Online.Items;
            foreach (Player p in online) {
                if (p.level != Map) continue;
                
                string msg = status == CpeMessageType.Status1 ? FormatStatus1(p) :
                    (status == CpeMessageType.Status2 ? FormatStatus2(p) : FormatStatus3(p));
                p.SendCpeMessage(status, msg);
            }
        }
        
        protected void UpdateStatus1(Player p) {
            p.SendCpeMessage(CpeMessageType.Status1, FormatStatus1(p));
        }
        
        protected void UpdateStatus2(Player p) {
            p.SendCpeMessage(CpeMessageType.Status2, FormatStatus2(p));
        }
        
        protected void UpdateStatus3(Player p) {
            p.SendCpeMessage(CpeMessageType.Status3, FormatStatus3(p));
        }
        
        
        public static bool InRange(Player a, Player b, int dist) {
            int dx = Math.Abs(a.Pos.X - b.Pos.X);
            int dy = Math.Abs(a.Pos.Y - b.Pos.Y);
            int dz = Math.Abs(a.Pos.Z - b.Pos.Z);
            return dx <= dist && dy <= dist && dz <= dist;
        }
    }
}
