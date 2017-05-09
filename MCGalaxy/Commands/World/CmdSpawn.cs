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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.World {
    public sealed class CmdSpawn : Command {
        public override string name { get { return "spawn"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdSpawn() { }

        public override void Use(Player p, string message)
        {
            if (message != "") { Help(p); return; }
            bool cpSpawn = p.useCheckpointSpawn;
            int x = 16 + (cpSpawn ? p.checkpointX : p.level.spawnx) * 32;
            int y = 32 + (cpSpawn ? p.checkpointY : p.level.spawny) * 32;
            int z = 16 + (cpSpawn ? p.checkpointZ : p.level.spawnz) * 32;
            byte yaw = cpSpawn ? p.checkpointRotX : p.level.rotx;
            byte pitch = cpSpawn ? p.checkpointRotY : p.level.roty;
            
            if (!p.Game.Referee && !p.Game.Infected && Server.zombie.RoundInProgress)
                Server.zombie.InfectPlayer(p, null);
            
            if (p.PlayingTntWars) {
                TntWarsGame game = TntWarsGame.GetTntWarsGame(p);
                if (game.GameMode == TntWarsGame.TntWarsGameMode.TDM && game.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers
                    && game.GameStatus != TntWarsGame.TntWarsGameStatus.Finished && game.RedSpawn != null && game.BlueSpawn != null) {
                    bool blue = game.FindPlayer(p).Blue;
                    
                    x = 16 + (blue ? game.BlueSpawn[0] : game.RedSpawn[0]) * 32;
                    y = 32 + (blue ? game.BlueSpawn[1] : game.RedSpawn[1]) * 32;
                    z = 16 + (blue ? game.BlueSpawn[2] : game.RedSpawn[2]) * 32;                    
                    yaw = (byte)(blue ? game.BlueSpawn[3] : game.RedSpawn[3]);
                    pitch = (byte)(blue ? game.BlueSpawn[4] : game.RedSpawn[4]);
                }
            }
            
            p.SendPos(Entities.SelfID, new Position(x, y, z), 
                      new Orientation(yaw, pitch));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/spawn");
            Player.Message(p, "%HTeleports you to the spawn location of the level.");
        }
    }
}
