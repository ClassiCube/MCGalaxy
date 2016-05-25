/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdBots : Command
    {
        public override string name { get { return "bots"; } }
        public override string shortcut { get { return "botlist"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdBots() { }

        public override void Use(Player p, string message)
        {
            message = "";
            PlayerBot[] bots = PlayerBot.Bots.Items;
            foreach (PlayerBot Pb in bots)
            {
                if (Pb.AIName != "") message += ", " + Pb.name + "(" + Pb.level.name + ")[" + Pb.AIName + "]";
                else if (Pb.hunt) message += ", " + Pb.name + "(" + Pb.level.name + ")[Hunt]";
                else message += ", " + Pb.name + "(" + Pb.level.name + ")";

                if (Pb.kill) message += "-kill";
            }

            if (message != "") Player.Message(p, "&1Bots: %S" + message.Remove(0, 2));
            else Player.Message(p, "No bots are alive.");
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/bots - Shows a list of bots, their AIs and levels");
        }
    }
}
