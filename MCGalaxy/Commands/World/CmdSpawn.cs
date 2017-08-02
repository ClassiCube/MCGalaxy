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
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;

namespace MCGalaxy.Commands.World {
    public sealed class CmdSpawn : Command {
        public override string name { get { return "Spawn"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override bool SuperUseable { get { return false; } }

        public override void Use(Player p, string message) {
            if (message.Length > 0) { Help(p); return; }
            bool cpSpawn = p.useCheckpointSpawn;
            Position pos;
            
            pos.X = 16 + (cpSpawn ? p.checkpointX : p.level.spawnx) * 32;
            pos.Y = 32 + (cpSpawn ? p.checkpointY : p.level.spawny) * 32;
            pos.Z = 16 + (cpSpawn ? p.checkpointZ : p.level.spawnz) * 32;
            byte yaw = cpSpawn ? p.checkpointRotX : p.level.rotx;
            byte pitch = cpSpawn ? p.checkpointRotY : p.level.roty;
            OnPlayerSpawningEvent.Call(p, ref pos, ref yaw, ref pitch, true);
            
            if (p.PlayingTntWars) {
                TntWarsGame game = TntWarsGame.GameIn(p);
                if (game.GameMode == TntWarsGame.TntWarsGameMode.TDM && game.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers
                    && game.GameStatus != TntWarsGame.TntWarsGameStatus.Finished && game.RedSpawn != null && game.BlueSpawn != null) {
                    bool blue = game.FindPlayer(p).Blue;
                    
                    pos.X = 16 + (blue ? game.BlueSpawn[0] : game.RedSpawn[0]) * 32;
                    pos.Y = 32 + (blue ? game.BlueSpawn[1] : game.RedSpawn[1]) * 32;
                    pos.Z = 16 + (blue ? game.BlueSpawn[2] : game.RedSpawn[2]) * 32;                    
                    yaw = (byte)(blue ? game.BlueSpawn[3] : game.RedSpawn[3]);
                    pitch = (byte)(blue ? game.BlueSpawn[4] : game.RedSpawn[4]);
                }
            }
            
            p.SendPos(Entities.SelfID, pos, new Orientation(yaw, pitch));
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Spawn");
            Player.Message(p, "%HTeleports you to the spawn location of the level.");
        }
    }
}
