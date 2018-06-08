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
    
    public sealed class TitleItem : SimpleItem {
        
        public TitleItem() {
            Aliases = new string[] { "titles", "title" };
            AllowsNoArgs = true;
        }
        
        public override string Name { get { return "Title"; } }
        
        protected override void DoPurchase(Player p, string message, string[] args) {
            if (args.Length == 1) {
                Command.Find("Title").Use(p, "-own");
                Player.Message(p, "%aYour title was removed for free."); return;
            }
            
            string title = message.SplitSpaces(2)[1]; // keep spaces this way
            if (title == p.title) {
                Player.Message(p, "%cYou already have that title."); return;
            }
            if (title.Length >= 20) {
                Player.Message(p, "%cTitles must be under 20 characters."); return;
            }
            
            Command.Find("Title").Use(p, "-own " + title);
            Economy.MakePurchase(p, Price, "%3Title: %f" + title);
        }
    }
    
    public sealed class NickItem : SimpleItem {
        
        public NickItem() {
            Aliases = new string[] { "nickname", "nick", "name" };
            AllowsNoArgs = true;
        }
        
        public override string Name { get { return "Nickname"; } }
        
        protected override void DoPurchase(Player p, string message, string[] args) {
            if (args.Length == 1) {
                Command.Find("Nick").Use(p, "-own");
                Player.Message(p, "%aYour nickname was removed for free."); return;
            }
            
            string nick = message.SplitSpaces(2)[1]; // keep spaces this way
            if (nick == p.DisplayName) {
                Player.Message(p, "%cYou already have that nickname."); return;
            }
            if (nick.Length >= 30) {
                Player.Message(p, "%cNicknames must be under 30 characters."); return;
            }
            
            Command.Find("Nick").Use(p, "-own " + nick);
            Economy.MakePurchase(p, Price, "%3Nickname: %f" + nick);
        }
    }
    
    public sealed class TitleColorItem : SimpleItem {
        
        public TitleColorItem() {
            Aliases = new string[] { "tcolor", "tcolour", "titlecolor", "titlecolour" };
        }
        
        public override string Name { get { return "TitleColor"; } }
        
        protected override void DoPurchase(Player p, string message, string[] args) {            
            string color = Matcher.FindColor(p, args[1]);
            if (color == null) return;
            string colName = Colors.Name(color);
            
            if (color == p.titlecolor) {
                Player.Message(p, "%cYou already have a " + color + colName + "%c titlecolor"); return;
            }
            
            Command.Find("TColor").Use(p, "-own " + colName);
            Economy.MakePurchase(p, Price, "%3Titlecolor: " + color + colName);
        }
    }
    
    public sealed class ColorItem : SimpleItem {
        
        public ColorItem() {
            Aliases = new string[] { "color", "colour" };
        }
        
        public override string Name { get { return "Color"; } }

        protected override void DoPurchase(Player p, string message, string[] args) {
            string color = Matcher.FindColor(p, args[1]);
            if (color == null) return;
            string colName = Colors.Name(color);
            
            if (color == p.color) {
                Player.Message(p, "%cYou already have a " + color + colName + "%c color"); return;
            }
            
            Command.Find("Color").Use(p, "-own " + colName);
            Economy.MakePurchase(p, Price, "%3Color: " + color + colName);
        }
    }
}
