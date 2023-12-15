/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using System;
using MCGalaxy.Commands;
using MCGalaxy.Events;
using MCGalaxy.Events.PlayerEvents;

namespace MCGalaxy.Modules.Moderation.Review 
{
    public sealed class ReviewPlugin : Plugin 
    {
        public override string name { get { return "Review"; } }

        Command cmdReview = new CmdReview();

        public override void Load(bool startup) {
            OnPlayerConnectEvent.Register(CheckReviewList, Priority.Low);
            Command.Register(cmdReview);
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerConnectEvent.Unregister(CheckReviewList);
            Command.Unregister(cmdReview);
        }


        static void CheckReviewList(Player p) {
            if (!p.CanUse("Review")) return;
            ItemPerms checkPerms = CommandExtraPerms.Find("Review", 1);
            if (!checkPerms.UsableBy(p)) return;
            
            int count = Server.reviewlist.Count;
            if (count == 0) return;
            
            string suffix = count == 1 ? " player is " : " players are ";
            p.Message(count + suffix + "waiting for a review. Type &T/Review view");
        }
    }
}
