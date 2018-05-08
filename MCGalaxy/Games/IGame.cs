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

namespace MCGalaxy.Games {

    public abstract class IGame {
        public Level Map;
        public abstract bool Running { get; }
        public abstract string GameName { get; }
        public virtual bool TeleportAllowed { get { return true; } }

        public virtual bool HandlesChatMessage(Player p, string message) { return false; }        
        public virtual void PlayerJoinedGame(Player p) { }
        public virtual void PlayerLeftGame(Player p) { }
        public virtual void PlayerJoinedLevel(Player p, Level lvl, Level oldLvl) { }
        
        public virtual void AdjustPrefix(Player p, ref string prefix) { }        
        public abstract void EndRound();
        
        public void MessageMap(CpeMessageType type, string message) {
            if (!Running) return;
            Player[] online = PlayerInfo.Online.Items;
            
            foreach (Player p in online) {
                if (p.level != Map) continue;
                p.SendCpeMessage(type, message);
            }
        }
    }
    
    public abstract class RoundsGame : IGame {
        public int RoundsLeft;
        public bool RoundInProgress;
        
        public abstract void End();
        protected abstract void DoRound();
        
        public void RunGame() {
            try {
                while (Running && RoundsLeft > 0) {
                    RoundInProgress = false;
                    if (RoundsLeft != int.MaxValue) RoundsLeft--;
                    DoRound();
                }
                End();
            } catch (Exception ex) {
                Logger.LogError(ex);
                Chat.MessageGlobal("&c" + GameName + " disabled due to an error.");
                
                try {
                    End();
                } catch (Exception ex2) {
                    Logger.LogError(ex2);
                }
            }
        }
    }
}
