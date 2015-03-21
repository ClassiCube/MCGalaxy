/*
	Copyright 2010 MCLawl Team - 
    Created by Snowl (David D.) and Cazzar (Cayde D.)

	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.osedu.org/licenses/ECL-2.0
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;
namespace MCGalaxy.Commands
{
    public sealed class CmdZombieGame : Command
    {
        public override string name { get { return "zombiegame"; } }
        public override string shortcut { get { return "zg"; } }
        public override string type { get { return "game"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdZombieGame() { }
        public override void Use(Player p, string message)
        {
            if (String.IsNullOrEmpty(message)) { Help(p); return; }
            string[] s = message.ToLower().Split(' ');
            if (s[0] == "status")
            {
                switch (Server.zombie.ZombieStatus())
                {
                    case 0:
                        Player.GlobalMessage("There is no Zombie Survival game currently in progress.");
                        return;
                    case 1:
                        Player.SendMessage(p, "There is a Zombie Survival game currently in progress with infinite rounds.");
                        return;
                    case 2:
                        Player.SendMessage(p, "There is a one-time Zombie Survival game currently in progress.");
                        return;
                    case 3:
                        Player.SendMessage(p, "There is a Zombie Survival game currently in progress with a " + Server.zombie.limitRounds + " amount of rounds.");
                        return;
                    case 4:
                        Player.SendMessage(p, "There is a Zombie Survival game currently in progress, scheduled to stop after this round.");
                        return;
                    default:
                        Player.SendMessage(p, "An unknown error occurred.");
                        return;
                }
            }
            else if (s[0] == "start")
            {
                if (Server.zombie.ZombieStatus() != 0) { Player.SendMessage(p, "There is already a Zombie Survival game currently in progress."); return; }
                if (s.Length == 2)
                {
                    int i = 1;
                    bool result = int.TryParse(s[1], out i);
                    if (result == false) { Player.SendMessage(p, "You need to specify a valid option!"); return; }
                    if (s[1] == "0")
                    {
                        Server.zombie.StartGame(1, 0);
                    }
                    else
                    {
                        Server.zombie.StartGame(3, i);
                    }
                }
                else
                    Server.zombie.StartGame(2, 0);
            }
            else if (s[0] == "stop")
            {
                if (Server.zombie.ZombieStatus() == 0) { Player.SendMessage(p, "There is no Zombie Survival game currently in progress."); return; }
                Player.GlobalMessage("The current game of Zombie Survival will end this round!");
                Server.gameStatus = 4;
            }
            else if (s[0] == "force")
            {
                if (Server.zombie.ZombieStatus() == 0) { Player.SendMessage(p, "There is no Zombie Survival game currently in progress."); return; }
                Server.s.Log("Zombie Survival ended forcefully by " + p.name);
                Server.zombie.aliveCount = 0;
                Server.gameStatus = 0; Server.gameStatus = 0; Server.zombie.limitRounds = 0; Server.zombie.initialChangeLevel = false; Server.ZombieModeOn = false; Server.zombieRound = false;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/zombiegame - Shows this help menu.");
            Player.SendMessage(p, "/zombiegame start - Starts a Zombie Survival game for one round.");
            Player.SendMessage(p, "/zombiegame start 0 - Starts a Zombie Survival game for an unlimited amount of rounds.");
            Player.SendMessage(p, "/zombiegame start [x] - Starts a Zombie Survival game for [x] amount of rounds.");
            Player.SendMessage(p, "/zombiegame stop - Stops the Zombie Survival game after the round has finished.");
            Player.SendMessage(p, "/zombiegame force - Force stops the Zombie Survival game immediately.");
        }
    }
}