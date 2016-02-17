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

namespace MCGalaxy.Commands {
	
    public class CmdModel : Command {
		
        public override string name { get { return "model"; } }
        public override string shortcut { get { return "setmodel"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") message = "normal";
            Player who = p;
            string[] args = message.Split(trimChars, 2);

            if (args.Length > 1) {
            	who = PlayerInfo.Find(args[0]);
                if (who == null) { Player.SendMessage(p, "Player \"" + args[0] + "\" does not exist"); return; }
            } else {
                if (p == null) { Player.SendMessage(p, "Console can't use this command on itself."); return; }
            }

            who.model = args[args.Length - 1];
            foreach (Player pl in PlayerInfo.players) {
                if (pl.level == who.level && pl.HasCpeExt(CpeExt.ChangeModel)) {
            		byte id = (pl == who) ? (byte)0xFF : who.id;
                    pl.SendChangeModel(id, who.model);
                }
            }
            Player.GlobalMessage(who.color + who.DisplayName + "'s %Smodel was changed to a &c" + args[args.Length - 1]);
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/model <player> [model] - Changes your player model.");
            Player.SendMessage(p, "Available models: Chicken, Creeper, Croc, Humanoid, Pig, Printer, Sheep, Spider, Skeleton, Zombie.");
            Player.SendMessage(p, "You can also place a block ID instead of a model name, to change your model into a block!");
        }
    }
}
