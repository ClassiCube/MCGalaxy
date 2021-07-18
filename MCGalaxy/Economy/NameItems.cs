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
        }
        
        public override string Name { get { return "Title"; } }
        
        protected internal override void OnPurchase(Player p, string title) {
            if (title.Length == 0) {
                PlayerOperations.SetTitle(p, p.name, "");
                p.Message("&aYour title was removed for free."); return;
            }
            
            if (!CheckPrice(p)) return;
            if (title == p.title) {
                p.Message("&WYou already have that title."); return;
            }

            if (!PlayerOperations.SetTitle(p, p.name, title)) return;
            Economy.MakePurchase(p, Price, "%3Title: %f" + title);
        }
    }
    
    public sealed class NickItem : SimpleItem {
        
        public NickItem() {
            Aliases = new string[] { "nickname", "nick", "name" };
        }
        
        public override string Name { get { return "Nickname"; } }
        
        protected internal override void OnPurchase(Player p, string nick) {
            if (nick.Length == 0) {
                PlayerOperations.SetNick(p, p.name, "");
                p.Message("&aYour nickname was removed for free."); return;
            }
            
            if (!CheckPrice(p)) return;
            if (nick == p.DisplayName) {
                p.Message("&WYou already have that nickname."); return;
            }
            if (nick.Length >= 30) {
                p.Message("&WNicknames must be under 30 characters."); return;
            }
            
            if (!PlayerOperations.SetNick(p, p.name, nick)) return;
            Economy.MakePurchase(p, Price, "%3Nickname: %f" + nick);
        }
    }
    
    public sealed class TitleColorItem : SimpleItem {
        
        public TitleColorItem() {
            Aliases = new string[] { "tcolor", "tcolour", "titlecolor", "titlecolour" };
        }
        
        public override string Name { get { return "TitleColor"; } }
        
        protected internal override void OnPurchase(Player p, string args) {
            if (args.Length == 0) { OnStoreCommand(p); return; }
            string color = Matcher.FindColor(p, args);
            
            if (color == null) return;
            string name = Colors.Name(color);
            if (!CheckPrice(p)) return;
            
            if (color == p.titlecolor) {
                p.Message("&WYour title color is already " + color + name); return;
            }
            
            if (!PlayerOperations.SetTitleColor(p, p.name, name)) return;
            Economy.MakePurchase(p, Price, "%3Titlecolor: " + color + name);
        }
    }
    
    public sealed class ColorItem : SimpleItem {
        
        public ColorItem() {
            Aliases = new string[] { "color", "colour" };
        }
        
        public override string Name { get { return "Color"; } }

        protected internal override void OnPurchase(Player p, string args) {
            if (args.Length == 0) { OnStoreCommand(p); return; }
            string color = Matcher.FindColor(p, args);
            
            if (color == null) return;
            string name = Colors.Name(color);
            if (!CheckPrice(p)) return;
            
            if (color == p.color) {
                p.Message("&WYour color is already " + color + name); return;
            }
            
            if (!PlayerOperations.SetColor(p, p.name, name)) return;
            Economy.MakePurchase(p, Price, "%3Color: " + color + name);
        }
    }
}
