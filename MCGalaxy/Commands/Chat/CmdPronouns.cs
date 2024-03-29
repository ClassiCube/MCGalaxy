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

namespace MCGalaxy.Commands.Chatting {
    public class CmdPronouns : Command2 {
        public override string name { get { return "Pronouns"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }

            Pronouns pro = Pronouns.FindMatch(p, message);
            if (pro == null) { HelpList(p); return; }

            p.pronouns = pro;
            p.Message("Your pronouns were changed to: &H{0}", pro.Name);
            pro.SaveFor(p);
        }

        public override void Help(Player p) {
            p.Message("&T/Pronouns [pronouns]");
            p.Message("&HChanges the pronouns used to refer to you in server messages.");
            HelpList(p);
            p.Message("&HYour pronouns are currently: &T{0}", p.pronouns.Name);
        }
        static void HelpList(Player p) {
            p.Message("&HThe following pronouns are currently available:");
            p.Message("&H  &T{0}", Pronouns.GetNames().Join("&H, &T"));
        }
    }
}
