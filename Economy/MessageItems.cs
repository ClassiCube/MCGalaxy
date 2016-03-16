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

namespace MCGalaxy.Eco {
    
    public sealed class LoginMessageItem : SimpleItem {
        
        public LoginMessageItem() {
            Aliases = new[] { "login", "loginmsg", "loginmessage" };
            NoArgsResetsItem = true;
        }
        
        public override string Name { get { return "LoginMessage"; } }
        static char[] trimChars = { ' ' };
        
        protected override void OnBuyCommand(Player p, string message, string[] args) {
            if (args.Length == 1) {
                Command.all.Find("loginmessage").Use(null, p.name + " joined the server.");
                Player.SendMessage(p, "%aYour login message was removed for free.");
                return;
            }
            
            string text = message.Split(trimChars, 2)[1]; // keep spaces this way         
            if (text == PlayerDB.GetLoginMessage(p)) {
                Player.SendMessage(p, "%cYou already have that login message."); return;
            }            
            Command.all.Find("loginmessage").Use(null, p.name + " " + text);
            Player.SendMessage(p, "%aYour login message was changed to: %f" + text);
            MakePurchase(p, Price, "%3LoginMessage: %f" + text);
        }
    }
    
    public sealed class LogoutMessageItem : SimpleItem {
        
        public LogoutMessageItem() {
            Aliases = new[] { "logout", "logoutmsg", "logoutmessage" };
            NoArgsResetsItem = true;
        }
        
        public override string Name { get { return "LogoutMessage"; } }
        static char[] trimChars = { ' ' };

        protected override void OnBuyCommand(Player p, string message, string[] args) {
            if (args.Length == 1) {
                Command.all.Find("logoutmessage").Use(null, p.name + " Disconnected.");
                Player.SendMessage(p, "%aYour logout message was removed for free.");
                return;
            }
            
            string text = message.Split(trimChars, 2)[1]; // keep spaces this way         
            if (text == PlayerDB.GetLogoutMessage(p)) {
                Player.SendMessage(p, "%cYou already have that logout message."); return;
            }            
            Command.all.Find("logoutmessage").Use(null, p.name + " " + text);
            Player.SendMessage(p, "%aYour logout message was changed to: %f" + text);
            MakePurchase(p, Price, "%3LogoutMessage: %f" + text);
        }
    }
}
