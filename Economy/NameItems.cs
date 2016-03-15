
using System;

namespace MCGalaxy.Eco {
    
    public sealed class TitleItem : SimpleItem {
        
        public TitleItem() {
            Aliases = new[] { "titles", "title" };
        }
        
        public override string Name { get { return "Title"; } }
        
        protected internal override void OnBuyCommand(Command cmd, Player p, string[] args) {
            if (args.Length < 2) { cmd.Help(p); return; }
            if (!p.EnoughMoney(Economy.Settings.TitlePrice)) {
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a title"); return;
            }
            if (args[1] == p.title) {
                Player.SendMessage(p, "%cYou already have that title"); return;
            }
            if (args[1].Length > 17) {
                Player.SendMessage(p, "%cTitles cannot be longer than 17 characters"); return;
            }
            var regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9-_\\.]*$");
            if (!regex.IsMatch(args[1])) {
                Player.SendMessage(p, "%cInvalid title! Titles may only contain alphanumeric characters and .-_");
                return;
            }
            
            bool free = args[1] == "";
            Command.all.Find("title").Use(null, p.name + " " + args[1]);
            if (!free) {
                Player.SendMessage(p, "%aYour title has been successfully changed to [" + p.titlecolor + args[1] + "%a]");
                MakePurchase(p, Price, "%3Title: %f" + args[1]);
            } else {
                Player.SendMessage(p, "%aYour title has been successfully removed for free");
                Player.SendMessage(p, "%aYour balance is now %f" + p.money + " %3" + Server.moneys);
            }
        }
    }
    
    public sealed class TitleColorItem : SimpleItem {
        
        public TitleColorItem() {
            Aliases = new[] { "tcolor", "tcolors", "titlecolor", "titlecolors", "tc" };
        }
        
        public override string Name { get { return "TitleColor"; } }
        
        protected internal override void OnBuyCommand(Command cmd, Player p, string[] args) {
            if (args.Length < 2) { cmd.Help(p); return; }
            if (!p.EnoughMoney(Economy.Settings.TColorPrice)) {
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a titlecolor"); return;
            }
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
    
    public sealed class ColorItem : SimpleItem {
        
        public ColorItem() {
            Aliases = new[] { "colors", "color", "colours", "colour" };
        }
        
        public override string Name { get { return "Color"; } }

        protected internal override void OnBuyCommand(Command cmd, Player p, string[] args) {
            if (args.Length < 2) { cmd.Help(p); return; }
            if (!p.EnoughMoney(Economy.Settings.ColorPrice)) {
                Player.SendMessage(p, "%cYou don't have enough %3" + Server.moneys + "%c to buy a color"); return;
            }
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
