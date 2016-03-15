/*
    Copyright 2011 MCForge
    
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
            Aliases = new[] { "login", "loginmessage" };
        }
        
        public override string Name { get { return "LoginMessage"; } }
        
        protected override void OnBuyCommand(Player p, string[] args) {
            if (!args[1].StartsWith("&") || !args[1].StartsWith("%")) {
                args[1] = Colors.Parse(args[1]);
                if (args[1] == "") { Player.SendMessage(p, "%cThat wasn't a color"); return; }
            }
            if (args[1] == p.titlecolor) {
                Player.SendMessage(p, "%cYou already have a " + args[1] + Colors.Name(args[1]) + "%c titlecolor"); return;
            }
            
            Command.all.Find("tcolor").Use(null, p.name + " " + Colors.Name(args[1]));
            Player.SendMessage(p, "%aYour titlecolor has been successfully changed to " + args[1] + Colors.Name(args[1]));
            MakePurchase(p, Price, "%3Titlecolor: " + args[1] + Colors.Name(args[1]));
        }
    }
    
    public sealed class LogoutMessageItem : SimpleItem {
        
        public LogoutMessageItem() {
            Aliases = new[] { "logout", "logoutmessage" };
        }
        
        public override string Name { get { return "LogoutMessage"; } }

        protected override void OnBuyCommand(Player p, string[] args) {
            if (!args[1].StartsWith("&") || !args[1].StartsWith("%")) {
                args[1] = Colors.Parse(args[1]);
                if (args[1] == "") { Player.SendMessage(p, "%cThat wasn't a color"); return; }
            }
            if (args[1] == p.color) {
                Player.SendMessage(p, "%cYou already have a " + args[1] + Colors.Name(args[1]) + "%c color"); return;
            }
            
            Command.all.Find("color").Use(null, p.name + " " + Colors.Name(args[1]));
            MakePurchase(p, Price, "%3Color: " + args[1] + Colors.Name(args[1]));
        }
    }
}
