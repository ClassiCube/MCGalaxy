/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Threading;

namespace MCGalaxy.Commands.Misc {  
    public sealed class CmdTpA : Command2 {        
        public override string name { get { return "TPA"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("TPAccept", "accept"), new CommandAlias("TPDeny", "deny") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            
            if (message.CaselessEq("accept")) {
                DoAccept(p);
            } else if (message.CaselessEq("deny")) {
                DoDeny(p);
            } else {
                DoTpa(p, message);
            }
        }
        
        void DoTpa(Player p, string message) {
            Player target = PlayerInfo.FindMatches(p, message);
            if (target == null) return;
            if (target == p) { p.Message("You cannot /tpa to yourself."); return; }
            if (target.Ignores.Names.CaselessContains(p.name)) { ShowSentMessage(p, target); return; }
            
            if (target.name.CaselessEq(p.currentTpa)) {
                p.Message("You still have a pending teleport request with this player."); return; 
            }
            if (p.level != target.level && target.level.IsMuseum) {
                p.Message("Player \"{0}\" is in a museum.", p.FormatNick(target)); return;
            }
            if (target.Loading) {
            	p.Message("Waiting for {0} &Sto spawn...", p.FormatNick(target));
                target.BlockUntilLoad(10);
            }
            
            ShowSentMessage(p, target);
            ShowRequestMessage(p, target);
            target.senderName = p.name;
            target.Request = true;
            p.currentTpa = target.name;
            
            Thread.Sleep(90000);
            if (target.Request) {
                p.Message("Your teleport request has timed out.");
                target.Message("Pending teleport request has timed out.");
                
                target.Request = false;
                target.senderName = "";
                p.currentTpa = "";
            }
        }
        
        static void ShowSentMessage(Player p, Player target) {
            p.Message("Your teleport request has been sent to {0}", p.FormatNick(target));
            p.Message("This request will timeout after &b90 &Sseconds.");
        }
        
        static void ShowRequestMessage(Player p, Player target) {
            if (Chat.Ignoring(target, p)) return;
            
            target.Message("{0} &Swould like to teleport to you.", target.FormatNick(p));
            target.Message("Type &2/tpaccept &Sor &4/tpdeny&S.");
            target.Message("This request will timeout after &b90 &Sseconds.");
        }
        
        void DoAccept(Player p) {
            if (!p.Request) { p.Message("You do not have any pending teleport requests."); return; }
            
            Player sender = PlayerInfo.FindExact(p.senderName);
            p.Request = false;
            p.senderName = "";
            if (sender == null) {
                p.Message("The player who requested to teleport to you isn't online anymore."); return;
            }
            
            p.Message("You have accepted {0}&S's teleportation request.", p.FormatNick(sender));
            sender.Message("{0} &Shas accepted your request. Teleporting now...", sender.FormatNick(p));
            sender.currentTpa = "";
            Thread.Sleep(1000);
            if (p.level != sender.level) {
                PlayerActions.ChangeMap(sender, p.level);
                Thread.Sleep(1000);
            }

            sender.SendPos(Entities.SelfID, p.Pos, p.Rot);
        }
        
        void DoDeny(Player p) {
            if (!p.Request) { p.Message("You do not have any pending teleport requests."); return; }
            
            Player sender = PlayerInfo.FindExact(p.senderName);
            p.Request = false;
            p.senderName = "";
            if (sender == null) {
                p.Message("The player who requested to teleport to you isn't online anymore."); return;
            }
            
            p.Message("You have denied {0}&S's teleportation request.", p.FormatNick(sender));
            sender.Message("{0} &Shas denied your request.", sender.FormatNick(p));
            sender.currentTpa = "";            
        } 

        public override void Help(Player p) {
            p.Message("&T/TPA [player] &H- Sends a teleport request to that player");
            p.Message("&T/TPA accept &H- Accepts a teleport request");
            p.Message("&T/TPA deny &H- Denies a teleport request");
        }
    }
}
