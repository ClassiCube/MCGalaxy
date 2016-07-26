/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands {
    public sealed class CmdEmote : Command {
        public override string name { get { return "emote"; } }
        public override string shortcut { get { return "<3"; } }
        public override string type { get { return CommandTypes.Chat; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdEmote() { }

        public override void Use(Player p, string message) {
            if (Player.IsSuper(p)) { MessageInGameOnly(p); }
            p.parseEmotes = !p.parseEmotes;
            
            if (p.parseEmotes) Server.noEmotes.Remove(p.name);
            else Server.noEmotes.AddOrReplace(p.name);
            Server.noEmotes.Save();           
            Player.Message(p, "Emote parsing is {0}.", p.parseEmotes ? "enabled" : "disabled");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/emote");
            Player.Message(p, "%HEnables or disables emoticon parsing");
        }
    }
}
