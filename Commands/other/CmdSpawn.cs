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
    public sealed class CmdSpawn : Command
    {
        public override string name { get { return "spawn"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdSpawn() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            ushort x = (ushort)((0.5 + p.level.spawnx) * 32);
            ushort y = (ushort)((1 + p.level.spawny) * 32);
            ushort z = (ushort)((0.5 + p.level.spawnz) * 32);
            if (!p.referee)
            {
                if (!p.infected && Server.zombie.GameInProgess())
                {
                    Server.zombie.InfectPlayer(p);
                }
            }
            if (p.PlayingTntWars)
            {
                TntWarsGame it = TntWarsGame.GetTntWarsGame(p);
                if (it.GameMode == TntWarsGame.TntWarsGameMode.TDM && it.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers && it.GameStatus != TntWarsGame.TntWarsGameStatus.Finished && it.RedSpawn != null && it.BlueSpawn != null)
                unchecked
                {
                    p.SendPos((byte)-1,
                                (ushort)((0.5 + (it.FindPlayer(p).Blue ? it.BlueSpawn[0] : it.RedSpawn[0]) * 32)),
                                (ushort)((1 + (it.FindPlayer(p).Blue ? it.BlueSpawn[1] : it.RedSpawn[1]) * 32)),
                                (ushort)((0.5 + (it.FindPlayer(p).Blue ? it.BlueSpawn[2] : it.RedSpawn[2]) * 32)),
                                (byte)(it.FindPlayer(p).Blue ? it.BlueSpawn[3] : it.RedSpawn[3]),
                                (byte)(it.FindPlayer(p).Blue ? it.BlueSpawn[4] : it.RedSpawn[4]));
                    return;
                }
            }
            unchecked
            {
                p.SendPos((byte)-1, x, y, z,
                            p.level.rotx,
                            p.level.roty);
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/spawn - Teleports yourself to the spawn location.");
        }
    }
}
