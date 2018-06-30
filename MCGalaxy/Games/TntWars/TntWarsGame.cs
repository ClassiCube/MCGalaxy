/*
    Copyright 2011 MCForge
        
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
///////--|----------------------------------|--\\\\\\\
//////---|  TNT WARS - Coded by edh649      |---\\\\\\
/////----|                                  |----\\\\\
////-----|  Note: Double click on // to see |-----\\\\
///------|        them in the sidebar!!     |------\\\
//-------|__________________________________|-------\\
using System;
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.Events;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Games {

    class TntWarsLevelPicker : LevelPicker {
        public override List<string> GetCandidateMaps() { return new List<string>() { }; }
    }
    
    public enum TntWarsGameMode { FFA, TDM };
    public enum TntWarsDifficulty {
        Easy,    //2 Hits to die, Tnt has long delay
        Normal,  //2 Hits to die, Tnt has normal delay
        Hard,    //1 Hit to die, Tnt has short delay
        Extreme, //1 Hit to die, Tnt has short delay and BIG exlosion
    }
    
    public sealed partial class TntWarsGame : RoundsGame {
        public override string GameName { get { return "TNT wars"; } }
        public TntWarsGameMode GameMode = TntWarsGameMode.TDM;
        public TntWarsDifficulty Difficulty = TntWarsDifficulty.Normal;
        
        sealed class TntWarsTeam {
            public string Name, Color;
            public string ColoredName { get { return Color + Name; } }
            public int Score;
            public Vec3U16 SpawnPos;
            public VolatileArray<Player> Members = new VolatileArray<Player>();
            
            public TntWarsTeam(string name, string color) { Name = name; Color = color; }
        }
        
        TntWarsTeam Red  = new TntWarsTeam("Red", Colors.red);
        TntWarsTeam Blue = new TntWarsTeam("Blue", Colors.blue);
        
        public TntWarsGame() {
            Picker = new TntWarsLevelPicker();
        }
        
        protected override List<Player> GetPlayers() {
            List<Player> playing = new List<Player>();
            playing.AddRange(Red.Members.Items);
            playing.AddRange(Blue.Members.Items);
            return playing;
        }
        
        public override void OutputStatus(Player p) {
            // TODO: Implement this
        }

        protected override void StartGame() {
            ResetTeams();
        }
        
        protected override void EndGame() {
            if (RoundInProgress) EndRound();
            ResetTeams();
        }
        
        void ResetTeams() {
            Blue.Members.Clear();
            Red.Members.Clear();
            Blue.Score = 0;
            Red.Score = 0;
        }
        
        public override void PlayerJoinedGame(Player p) {
            bool announce = false;
            HandleJoinedLevel(p, Map, Map, ref announce);
        }
        
        public override void PlayerLeftGame(Player p) {
            TntWarsTeam team = TeamOf(p);
            if (team == null) return;

            team.Members.Remove(p);
            Map.Message(team.Color + p.DisplayName + " %Sleft TNT wars");
        }
        
        void JoinTeam(Player p, TntWarsTeam team) {
            team.Members.Add(p);
            Map.Message(p.ColoredName + " %Sjoined the " + team.ColoredName + " %Steam");
            Player.Message(p, "You are now on the " + team.ColoredName + " team!");
            TabList.Update(p, true);
        }
        
        TntWarsTeam TeamOf(Player p) {
            if (Red.Members.Contains(p)) return Red;
            if (Blue.Members.Contains(p)) return Blue;
            return null;
        }
    }
}
