/*
    Copyright 2011 MCForge
    
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
using System.Collections.Generic;
using MCGalaxy.Util;

namespace MCGalaxy.Eco 
{
    public sealed class SnackItem : SimpleItem 
    {
        public SnackItem() {
            Aliases = new string[] { "snack" };
            Price = 0;
        }
        
        public override string Name { get { return "Snack"; } }
        
        public override void OnPurchase(Player p, string args) {
            if (DateTime.UtcNow < p.NextEat) {
                p.Message("You're still full - you need to wait at least " +
                          "10 seconds between snacks."); return;
            }          

            if (!CheckPrice(p)) return;
            TextFile eatFile = TextFile.Files["Eat"];
            eatFile.EnsureExists();
            if (!p.CheckCanSpeak("eat a snack")) return;
            
            string[] lines = eatFile.GetText();
            List<string> actions = new List<string>(lines.Length);
            foreach (string line in lines) { if (!line.StartsWith("//")) actions.Add(line); }

            string action = "ate some food.";

            if (actions.Count > 0) {
                action = actions[new Random().Next(actions.Count)];
                action = Pronouns.ParseInto(action, p, "@p{0}");
                action = action.Replace("@p", "λNICK&S");
                //     had @p.object food stolen by a dog and @p.subject began to cry. ->
                //zoey had her       food stolen by a dog and she        began to cry.
            }

            if (!action.Contains("λNICK")) {
                //Allows eat message to specify where the player's name appears rather than hardcoded at the beginning with a space
                action = "λNICK&S " + action;
            }
            Chat.MessageFrom(p, action, null, true);
            p.CheckForMessageSpam();
            
            p.NextEat = DateTime.UtcNow.AddSeconds(10);
            // intentionally not using Economy.MakePurchase here
            p.SetMoney(p.money - Price);
        }
    }
}
