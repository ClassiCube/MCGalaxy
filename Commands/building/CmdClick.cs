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
    public sealed class CmdClick : Command
    {
        public override string name { get { return "click"; } }
        public override string shortcut { get { return "x"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdClick() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "This command can only be used in-game");
                return;
            }
            string[] parameters = message.Split(' ');
            ushort[] click = p.lastClick;

            if (message.IndexOf(' ') != -1)
            {
                if (parameters.Length != 3)
                {
                    Help(p);
                    return;
                }
                else
                {
                    for (int value = 0; value < 3; value++)
                    {
                        if (parameters[value].ToLower() == "x" || parameters[value].ToLower() == "y" || parameters[value].ToLower() == "z")
                            click[value] = p.lastClick[value];
                        else if (isValid(parameters[value], value, p))
                            click[value] = ushort.Parse(parameters[value]);
                        else
                        {
                            Player.SendMessage(p, "\"" + parameters[value] + "\" was not valid");
                            return;
                        }
                    }
                }
            }

            p.lastCMD = "click";
            p.ManualChange(click[0], click[1], click[2], 0, Block.rock);
            Player.SendMessage(p, "Clicked &b(" + click[0] + ", " + click[1] + ", " + click[2] + ")");
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/click [x y z] - Fakes a click");
            Player.SendMessage(p, "If no xyz is given, it uses the last place clicked");
            Player.SendMessage(p, "/click 200 y 200 will cause it to click at 200x, last y and 200z");
        }

        private bool isValid(string message, int dimension, Player p)
        {
            ushort testValue;
            try {
                testValue = ushort.Parse(message);
            } catch { return false; }

            if (testValue < 0)
                return false;

            if (testValue >= p.level.Width && dimension == 0) return false;
            else if (testValue >= p.level.Height && dimension == 1) return false;
            else if (testValue >= p.level.Length && dimension == 2) return false;

            return true;
        }
    }
}
