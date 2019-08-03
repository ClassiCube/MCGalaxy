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
using System.Collections.Generic;
using System.Windows.Forms;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public sealed class GamesHelper {
        CheckBox cbStart, cbMap, cbMain;
        Button btnStart, btnStop, btnEnd, btnAdd, btnDel;
        ListBox lbUsed, lbNotUsed;
        
        RoundsGame game;
        public GamesHelper(RoundsGame game,
                           CheckBox start_, CheckBox map, CheckBox main,
                           Button start, Button stop, Button end,
                           Button add, Button del, ListBox used, ListBox notUsed) {
            this.game = game;
            cbStart = start_; cbMap = map; cbMain = main;
            btnStart = start; btnStop = stop; btnEnd = end;
            btnAdd = add; btnDel = del; lbUsed = used; lbNotUsed = notUsed;
            
            start.Click += StartGame_Click;
            stop.Click  += StopGame_Click;
            end.Click   += EndRound_Click;
            
            add.Click += AddMap_Click;
            del.Click += DelMap_Click;
        }
        
        public void Load(string[] allMaps) {
            RoundsGameConfig cfg = game.GetConfig();
            cbStart.Checked = cfg.StartImmediately;
            cbMap.Checked   = cfg.MapInHeartbeat;
            cbMain.Checked  = cfg.SetMainLevel;
            
            UpdateButtons();
            UpdateUsedMaps();
            UpdateNotUsedMaps(allMaps);
        }
        
        public void Save() {
            RoundsGameConfig cfg = game.GetConfig();
            cfg.StartImmediately = cbStart.Checked;
            cfg.MapInHeartbeat   = cbMap.Checked;
            cfg.SetMainLevel     = cbMain.Checked;
            
            try {
                cfg.Save();
            } catch (Exception ex) {
                Logger.LogError("Error saving " + game.GameName + " settings", ex);
            }
        }
        
        public void UpdateButtons() {
            btnStart.Enabled = !game.Running;
            btnStop.Enabled  = game.Running;
            btnEnd.Enabled   = game.Running; // && game.RoundInProgress;
        }
        
        void StartGame_Click(object sender, EventArgs e) {
            if (!game.Running) game.Start(Player.Console, "", int.MaxValue);
            UpdateButtons();
        }

        void StopGame_Click(object sender, EventArgs e) {
            if (game.Running) game.End();
            UpdateButtons();
        }

        void EndRound_Click(object sender, EventArgs e) {
            if (game.RoundInProgress) game.EndRound();
            UpdateButtons();
        }
        
        
        void AddMap_Click(object sender, EventArgs e) {
            try {
                object selected = lbNotUsed.SelectedItem;
                if (selected == null) { Popup.Warning("No map selected"); return; }
                string map = (string)selected;

                Level lvl;
                LevelConfig lvlCfg = LevelInfo.GetConfig(map, out lvl);
                RoundsGameConfig.AddMap(Player.Console, map, lvlCfg, game);
            } catch (Exception ex) { 
                Logger.LogError("Error adding map to game", ex); 
            }
        }

        void DelMap_Click(object sender, EventArgs e) {
            try {
                object selected = lbUsed.SelectedItem;
                if (selected == null) { Popup.Warning("No map selected"); return; }
                string map = (string)selected;

                Level lvl;
                LevelConfig lvlCfg = LevelInfo.GetConfig(map, out lvl);
                RoundsGameConfig.RemoveMap(Player.Console, map, lvlCfg, game);
            } catch (Exception ex) { 
                Logger.LogError("Error removing map from game", ex); 
            }
        }
        
        
        public void UpdateMapConfig(string map) {
            if (game.Running && game.Map.name == map) {
                game.UpdateMapConfig();
            }
        }
        
        public void UpdateMaps() {
            UpdateUsedMaps();
            UpdateNotUsedMaps(null);
        }
        
        public void UpdateUsedMaps() {
            lbUsed.SelectedIndex = -1;
            object selected = lbUsed.SelectedItem;
            lbUsed.Items.Clear();
            
            List<string> maps = game.GetConfig().Maps;
            foreach (string map in maps) {
                lbUsed.Items.Add(map);
            }
            
            Reselect(lbUsed, selected);
        }
        
        public void UpdateNotUsedMaps(string[] allMaps) {
            lbNotUsed.SelectedIndex = -1;
            object selected = lbNotUsed.SelectedItem;
            lbNotUsed.Items.Clear();
            
            // relatively expensive, so avoid if possible
            if (allMaps == null) allMaps = LevelInfo.AllMapNames();
            List<string> maps = game.GetConfig().Maps;
            foreach (string map in allMaps) {
                if (maps.CaselessContains(map)) continue;
                lbNotUsed.Items.Add(map);
            }
            
            Reselect(lbNotUsed, selected);
        }
        
        void Reselect(ListBox box, object selected) {
            int i = -1;
            if (selected != null) i = box.Items.IndexOf(selected);
            box.SelectedIndex = i;
        }
    }
}