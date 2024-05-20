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

using System.Collections.Generic;

namespace MCGalaxy.Commands.Chatting {
    public class CmdPronouns : Command2 {
        public override string name { get { return "Pronouns"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }

            string[] names = message.SplitSpaces();
            Dictionary<string, Pronouns> pros = new Dictionary<string, Pronouns>();

            foreach (string name in names) {
                Pronouns pro = Pronouns.FindMatch(p, name);
                if (pro == null) { HelpList(p); return; }
                pros[pro.Name] = pro;
            }
            
            // Disallow using default pronouns along with other pronouns (it's weird..?)
            if (pros.Count > 1 && pros.ContainsKey(Pronouns.Default.Name)) {
                pros.Remove(Pronouns.Default.Name);
            }

            List<Pronouns> final = new List<Pronouns>();
            foreach (var pair in pros) {
                final.Add(pair.Value);
            }

            p.pronounsList = final;
            p.Message("Your pronouns were changed to: &T{0}", Pronouns.ListFor(p, ", "));
            Pronouns.SaveFor(p);
        }

        public override void Help(Player p) {
            p.Message("&T/Pronouns [pronouns1] <pronouns2> <etc>");
            p.Message("&H[pronouns1] will be used to refer to you in server messages.");
            p.Message("&HThe list of pronouns you select will appear in &T/whois");
            HelpList(p);
            p.Message("&HYour pronouns are currently: &T{0}", Pronouns.ListFor(p, ", "));
        }
        static void HelpList(Player p) {
            p.Message("&HThe following pronouns are currently available:");
            p.Message("&H  &T{0}", Pronouns.GetNames().Join("&H, &T"));
        }
    }
}
