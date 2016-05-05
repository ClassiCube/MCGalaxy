/*
    Written by BeMacized
    Assisted by RedNoodle
    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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

namespace MCGalaxy.Commands
{
    public sealed class CmdReview : Command
    {
        public override string name { get { return "review"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdReview() { }

        public override void Use(Player p, string message) {
            if (p != null && message == "") message = "enter";
            
            switch (message.ToLower()) {
                case "enter":
                    HandleEnter(p); break;
                case "list":
                case "view":
                    HandleView(p); break;
                case "leave":
                    HandleLeave(p); break;
                case "next":
                    HandleNext(p); break;
                case "clear":
                    HandleClear(p); break;
                default:
                    Help(p); break;
            }
        }
        
        void HandleEnter(Player p) {
            if (p == null) { Player.Message(p, "Console cannot enter the review queue."); return; }
            if (DateTime.UtcNow < p.NextReviewTime) {
                Player.Message(p, "You have to wait " + Server.reviewcooldown + " seconds everytime you use this command");
                return;
            }
            
            Group grp = Group.findPerm(Server.reviewenter);
            if (grp == null) { MessageNoPerm(p, Server.reviewenter); return; }
            if (grp.Permission > p.group.Permission) {
                MessageNeedMinPerm(p, "enter the review queue", (int)grp.Permission); return;
            }
            
            foreach (string name in Server.reviewlist) {
                if (name != p.name) continue;
                Player.Message(p, "You already entered the review queue!"); return;
            }

            bool opsOn = false;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.group.Permission >= Server.reviewnext && Entities.CanSee(p, pl)) {
                    opsOn = true; break;
                }
            }
            
            if (opsOn) {
                Server.reviewlist.Add(p.name);
                int pos = Server.reviewlist.IndexOf(p.name);
                if (pos > 1) { Player.Message(p, "You entered the &creview %Squeue. You have &c" + pos + " %Speople in front of you in the queue"); }
                if (pos == 1) { Player.Message(p, "You entered the &creview %Squeue. There is &c1 %Sperson in front of you in the queue"); }
                if (pos == 0) { Player.Message(p, "You entered the &creview %Squeue. You are &cfirst %Sin line!"); }
                Player.Message(p, "The Online Operators have been notified. Someone should be with you shortly.");
                
                string start = pos > 0 ? "There are now &c" + (pos + 1) + " %Speople" : "There is now &c1 %Sperson";
                Chat.GlobalMessageMinPerms(p.color + p.name + " %Sentered the review queue", Server.reviewnext);
                Chat.GlobalMessageMinPerms(start + " waiting for a &creview!", Server.reviewnext);
                p.NextReviewTime = DateTime.UtcNow.AddSeconds(Server.reviewcooldown);
            } else {
                Player.Message(p, "&cThere are no operators on to review your build. Please wait for one to come on and try again.");
            }
        }
        
        void HandleView(Player p) {
            Group grp = Group.findPerm(Server.reviewview);
            if (grp == null) { MessageNoPerm(p, Server.reviewview); return; }
            
            if (p != null && grp.Permission > p.group.Permission) {
                MessageNeedMinPerm(p, "view the review queue", (int)grp.Permission); return;
            }
            if (Server.reviewlist.Count == 0) {
                Player.Message(p, "There are no players in the review queue!"); return;
            }
            
            Player.Message(p, "&9Players in the review queue:");
            int pos = 1;
            foreach (string who in Server.reviewlist) {
                string rank = Group.findPlayer(who.ToLower());
                Player.Message(p, "&a" + pos + ". &f" + who + "&a - Current Rank: " + Group.Find(rank).color + rank);
                pos++;
            }
        }
        
        void HandleLeave(Player p) {
            if (p == null) { Player.Message(p, "Console cannot leave the review queue."); return; }
            Group grp = Group.findPerm(Server.reviewleave);
            if (grp == null) { MessageNoPerm(p, Server.reviewleave); return; }
            
            if (grp.Permission > p.group.Permission) {
                MessageNeedMinPerm(p, "leave the review queue", (int)grp.Permission); return;
            }

            bool inQueue = false;
            foreach (string who in Server.reviewlist)
                inQueue |= who == p.name;
            if (!inQueue) {
                Player.Message(p, "You aren't in the review queue so you cannot leave it."); return;
            }
            Server.reviewlist.Remove(p.name);
            MessageReviewPosChanged();
            Player.Message(p, "You have left the review queue!");
        }
        
        void HandleNext(Player p) {
            if (p == null) { Player.Message(p, "Console cannot answer the review queue."); return; }
            Group grp = Group.findPerm(Server.reviewnext);
            if (grp == null) { MessageNoPerm(p, Server.reviewnext); }
            
            if (grp.Permission > p.group.Permission) {
                MessageNeedMinPerm(p, "answer the review queue", (int)grp.Permission); return;
            }
            if (Server.reviewlist.Count == 0) {
                Player.Message(p, "There are no players in the review queue."); return;
            }
            
            string user = Server.reviewlist[0];
            Player who = PlayerInfo.FindExact(user);
            if (who == null) {
                Player.Message(p, "Player " + user + " doesn't exist or is offline. " + user + " has been removed from the review queue");
                Server.reviewlist.Remove(user);
                return;
            } else if (who == p) {
                Player.Message(p, "Cannot teleport to yourself. You have been removed from the review queue.");
                Server.reviewlist.Remove(user);
                return;
            }
            Server.reviewlist.Remove(user);
            Command.all.Find("tp").Use(p, who.name);
            Player.Message(p, "You have been teleported to " + user);
            Player.Message(who, "Your review request has been answered by " + p.name + ".");
            MessageReviewPosChanged();
        }
        
        void HandleClear(Player p) {
            Group grp = Group.findPerm(Server.reviewclear);
            if (grp == null) { MessageNoPerm(p, Server.reviewclear); return; }
            
            if (p != null && grp.Permission > p.group.Permission) {
                MessageNeedMinPerm(p, "clear the review queue", (int)grp.Permission); return;
            }
            Server.reviewlist.Clear();
            Player.Message(p, "The review queue has been cleared");
        }
        
        static void MessageNoPerm(Player p, LevelPermission perm) {
            Player.Message(p, "There is something wrong with the system.  A message has been sent to the admin to fix");
            Chat.GlobalMessageAdmins(p.name + " tryed to use /review, but a system error occurred. Make sure your groups are formatted correctly");
            Chat.GlobalMessageAdmins("The group permission that is messed up is: " + perm + " (" + (int)perm+ ")");
        }
        
        static void MessageReviewPosChanged() {
            int count = 0;
            foreach (string name in Server.reviewlist) {
                Player who = PlayerInfo.FindExact(name);
                if (who == null) continue;
                Player.Message(who, "The review queue has changed. You now have " + count + " players in front of you.");
                count++;
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/review <enter/view/leave/next/clear>");
            Player.Message(p, "%HLets you enter, view, leave, or clear the reviewlist or" +
                               "teleport you to the next player in the review queue.");
        }
    }
}
