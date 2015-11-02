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
    public sealed class CmdPhysics : Command
    {
        public override string name { get { return "physics"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPhysics() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                foreach (Level l in Server.levels)
                {
                    if (l.physics > 0)
                        Player.SendMessage(p, "&5" + l.name + Server.DefaultColor + " has physics at &b" + l.physics + Server.DefaultColor + ". &cChecks: " + l.lastCheck + "; Updates: " + l.lastUpdate);
                }
                return;
            }
            try
            {
                int temp = 0; Level level = null;
                if (message.Split(' ').Length == 1)
                {
                    temp = int.Parse(message);
                    if (p != null)
                    {
                        level = p.level;
                    }
                    else
                    {
                        level = Server.mainLevel;
                    }
                }
                else
                {
                    temp = System.Convert.ToInt16(message.Split(' ')[1]);
                    string nameStore = message.Split(' ')[0];
                    level = Level.Find(nameStore);
                }
                if (temp >= 0 && temp <= 5)
                {
                    level.setPhysics(temp);
                    switch (temp)
                    {
                        case 0:
                            level.ClearPhysics();
                            Player.GlobalMessage("Physics are now &cOFF" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now OFF on " + level.name + ".");
                            //IRCBot.Say("Physics are now OFF on " + level.name + ".");
                            Server.IRC.Say("Physics are now OFF on " + level.name + ".");
                            break;

                        case 1:
                            Player.GlobalMessage("Physics are now &aNormal" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now Normal on " + level.name + ".");
                            //IRCBot.Say("Physics are now Normal on " + level.name + ".");
                            Server.IRC.Say("Physics are now Normal on " + level.name + ".");
                            break;

                        case 2:
                            Player.GlobalMessage("Physics are now &aAdvanced" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now Advanced on " + level.name + ".");
                            //IRCBot.Say("Physics are now Advanced on " + level.name + ".");
                            Server.IRC.Say("Physics are now Advanced on " + level.name + ".");
                            break;

                        case 3:
                            Player.GlobalMessage("Physics are now &aHardcore" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now Hardcore on " + level.name + ".");
                            //IRCBot.Say("Physics are now Hardcore on " + level.name + ".");
                            Server.IRC.Say("Physics are now Hardcore on " + level.name + ".");
                            break;

                        case 4:
                            Player.GlobalMessage("Physics are now &aInstant" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now Instant on " + level.name + ".");
                            //IRCBot.Say("Physics are now Instant on " + level.name + ".");
                            Server.IRC.Say("Physics are now Instant on " + level.name + ".");
                            break;
                        case 5:
                            Player.GlobalMessage("Physics are now &4Doors-Only" + Server.DefaultColor + " on &b" + level.name + Server.DefaultColor + ".");
                            Server.s.Log("Physics are now Doors-Only on " + level.name + ".");
                            //IRCBot.Say("Physics are now Doors-Only on " + level.name + ".");
                            Server.IRC.Say("Physics are now Doors-Only on " + level.name + ".");
                            break;
                    }

                    level.changed = true;
                }
                else
                {
                    Player.SendMessage(p, "Not a valid setting");
                }
            }
            catch
            {
                Player.SendMessage(p, "INVALID INPUT");
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/physics [map] <0/1/2/3/4/5> - Set the [map]'s physics, 0-Off 1-On 2-Advanced 3-Hardcore 4-Instant 5-Doors_Only");
            Player.SendMessage(p, "If [map] is blank, uses Current level");
        }
    }
}
