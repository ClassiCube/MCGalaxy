﻿/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.Events;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Games;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy.Modules.Games.Countdown
{
    public partial class CountdownGame : RoundsGame 
    {
        public VolatileArray<Player> Players = new VolatileArray<Player>();
        public VolatileArray<Player> Remaining = new VolatileArray<Player>();
        
        public CountdownConfig Config = new CountdownConfig();
        public override string GameName { get { return "Countdown"; } }
        public override RoundsGameConfig GetConfig() { return Config; }
        
        protected override string WelcomeMessage {
            get { return "&aCountdown &Sis running! Type &T/CD join &Sto join"; }
        }
                
        public bool FreezeMode;
        public int Interval;
        public CountdownSpeed SpeedType;
        
        public static CountdownGame Instance = new CountdownGame();
        public CountdownGame() { Picker = new SimpleLevelPicker(); }
        
        public override void UpdateMapConfig() { }
        
        protected override List<Player> GetPlayers() {
            List<Player> playing = new List<Player>();
            playing.AddRange(Players.Items);
            return playing;
        }
        
        public override void OutputStatus(Player p) {
            Player[] players = Players.Items;            
            p.Message("Players in countdown:");
            
            if (RoundInProgress) {               
                p.Message(players.Join(pl => FormatPlayer(pl)));
            } else {
                p.Message(players.Join(pl => pl.ColoredName));
            }
            
            p.Message(squaresLeft.Count + " squares left");
        }
        
        string FormatPlayer(Player pl) {
            string suffix = Remaining.Contains(pl) ? " &a[IN]" : " &c[OUT]";
            return pl.ColoredName + suffix;
        }
        
        protected override string GetStartMap(Player p, string forcedMap) {
            if (!LevelInfo.MapExists("countdown")) {
                p.Message("Countdown level not found, generating..");
                GenerateMap(p, 32, 32, 32);
            }
            return "countdown";
        }

        protected override void StartGame() { }
        protected override void EndGame() {
            Players.Clear();
            Remaining.Clear();
            squaresLeft.Clear();
        }
        
        public void GenerateMap(Player p, int width, int height, int length) {
            Level lvl = CountdownMapGen.Generate(width, height, length);
            Level cur = LevelInfo.FindExact("countdown");
            if (cur != null) LevelActions.Replace(cur, lvl);
            else LevelInfo.Add(lvl);
            
            lvl.Save();
            Map = lvl;
            
            const string format = "Generated map ({0}x{1}x{2}), sending you to it..";
            p.Message(format, width, height, length);
            PlayerActions.ChangeMap(p, "countdown");
        }
        
        public override void PlayerJoinedGame(Player p) {
            if (!Players.Contains(p)) {
                if (p.level != Map && !PlayerActions.ChangeMap(p, "countdown")) return;
                Players.Add(p);
                p.Message("You've joined countdown!");
                Chat.MessageFrom(p, "λNICK &Sjoined countdown!");              
            } else {
                p.Message("You've already joined countdown. To leave, go to another map.");
            }
        }
        
        public override void PlayerLeftGame(Player p) {
            Players.Remove(p);
            OnPlayerDied(p);
        }
        
        protected override string FormatStatus1(Player p) {
            return RoundInProgress ? squaresLeft.Count + " squares left" : "";
        }
        
        protected override string FormatStatus2(Player p) {
            return RoundInProgress ? Remaining.Count + " players left" : "";
        }
        
        public void SetSpeed(CountdownSpeed speed) {
            switch (speed) {
                case CountdownSpeed.Slow:     Interval = 800; break;
                case CountdownSpeed.Normal:   Interval = 650; break;
                case CountdownSpeed.Fast:     Interval = 500; break;
                case CountdownSpeed.Extreme:  Interval = 300; break;
                case CountdownSpeed.Ultimate: Interval = 150; break;
            }
            SpeedType = speed;
        }
    }
}
