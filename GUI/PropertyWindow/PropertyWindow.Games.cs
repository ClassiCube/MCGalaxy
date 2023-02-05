/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Games;
using MCGalaxy.Modules.Games.Countdown;
using MCGalaxy.Modules.Games.CTF;
using MCGalaxy.Modules.Games.LS;
using MCGalaxy.Modules.Games.ZS;
using MCGalaxy.Modules.Games.TW;

namespace MCGalaxy.Gui 
{
    public partial class PropertyWindow : Form 
    {
        GamesHelper lsHelper, zsHelper, ctfHelper, twHelper, cdHelper;
        
        void LoadGameProps() {
            string[] allMaps = LevelInfo.AllMapNames();
            LoadZSSettings(allMaps);
            LoadCTFSettings(allMaps);
            LoadLSSettings(allMaps);
            LoadTWSettings(allMaps);
            LoadCDSettings(allMaps);
        }

        void SaveGameProps() {
            SaveZSSettings();
            SaveCTFSettings();
            SaveLSSettings();
            SaveTWSettings();
            SaveCDSettings();
        }
        
        GamesHelper GetGameHelper(IGame game) {
            // TODO: Find a better way of doing this
            if (game == ZSGame.Instance)  return zsHelper;
            if (game == CTFGame.Instance) return ctfHelper;
            if (game == LSGame.Instance)  return lsHelper;
            if (game == TWGame.Instance)  return twHelper;
            return null;
        }
        
        void HandleMapsChanged(RoundsGame game) {
            GamesHelper helper = GetGameHelper(game);
            if (helper == null) return;
            RunOnUI_Async(() => helper.UpdateMaps());
        }
        
        void HandleStateChanged(IGame game) {
            GamesHelper helper = GetGameHelper(game);
            if (helper == null) return;
            RunOnUI_Async(() => helper.UpdateButtons());
        }
        
        
        void LoadZSSettings(string[] allMaps) {
            zsHelper = new GamesHelper(
                ZSGame.Instance, zs_cbStart, zs_cbMap, zs_cbMain,
                zs_btnStart, zs_btnStop, zs_btnEnd,
                zs_btnAdd, zs_btnRemove, zs_lstUsed, zs_lstNotUsed);
            zsHelper.Load(allMaps);
            
            ZSConfig cfg = ZSGame.Instance.Config;
            zs_numInvHumanDur.Value  = cfg.InvisibilityDuration;
            zs_numInvHumanMax.Value  = cfg.InvisibilityPotions;
            zs_numInvZombieDur.Value = cfg.ZombieInvisibilityDuration;
            zs_numInvZombieMax.Value = cfg.ZombieInvisibilityPotions;
            
            zs_numReviveMax.Value   = cfg.ReviveTimes;
            zs_numReviveEff.Value   = cfg.ReviveChance;
            zs_numReviveLimit.Value = cfg.ReviveTooSlow;
            
            zs_txtName.Text  = cfg.ZombieName;
            zs_txtModel.Text = cfg.ZombieModel;
        }
        
        void SaveZSSettings() {
            try {
                ZSConfig cfg = ZSGame.Instance.Config;
                cfg.InvisibilityDuration = (int)zs_numInvHumanDur.Value;
                cfg.InvisibilityPotions  = (int)zs_numInvHumanMax.Value;
                cfg.ZombieInvisibilityDuration = (int)zs_numInvZombieDur.Value;
                cfg.ZombieInvisibilityPotions  = (int)zs_numInvZombieMax.Value;
                
                cfg.ReviveTimes   = (int)zs_numReviveMax.Value;
                cfg.ReviveChance  = (int)zs_numReviveEff.Value;
                cfg.ReviveTooSlow = (int)zs_numReviveLimit.Value;
                
                cfg.ZombieName  =  zs_txtName.Text.Trim();
                cfg.ZombieModel = zs_txtModel.Text.Trim();
                if (cfg.ZombieModel.Length == 0) cfg.ZombieModel = "zombie";
                
                zsHelper.Save();
            } catch (Exception ex) {
                Logger.LogError("Error saving ZS settings", ex);
            }
        }
        
        
        void LoadCTFSettings(string[] allMaps) {
            ctfHelper = new GamesHelper(
                CTFGame.Instance, ctf_cbStart, ctf_cbMap, ctf_cbMain,
                ctf_btnStart, ctf_btnStop, ctf_btnEnd,
                ctf_btnAdd, ctf_btnRemove, ctf_lstUsed, ctf_lstNotUsed);
            ctfHelper.Load(allMaps);
        }
        
        void SaveCTFSettings() {
            try {
                ctfHelper.Save();
            } catch (Exception ex) {
                Logger.LogError("Error saving CTF settings", ex);
            }
        }
        

        void LoadLSSettings(string[] allMaps) {
             lsHelper = new GamesHelper(
                LSGame.Instance, ls_cbStart, ls_cbMap, ls_cbMain,
                ls_btnStart, ls_btnStop, ls_btnEnd,
                ls_btnAdd, ls_btnRemove, ls_lstUsed, ls_lstNotUsed);            
            lsHelper.Load(allMaps);
            
            LSConfig cfg = LSGame.Instance.Config;
            ls_numMax.Value = cfg.MaxLives;
        }
        
        void SaveLSSettings() {
            try {
                LSConfig cfg = LSGame.Instance.Config;
                cfg.MaxLives = (int)ls_numMax.Value;
                
                lsHelper.Save();
                SaveLSMapSettings();
            } catch (Exception ex) {
                Logger.LogError("Error saving Lava Survival settings", ex);
            }
        }
        
        string lsCurMap;
        LSMapConfig lsCurCfg;
        void lsMapUse_SelectedIndexChanged(object sender, EventArgs e) {
            SaveLSMapSettings();
            if (ls_lstUsed.SelectedIndex == -1) {
                ls_grpMapSettings.Text = "Map settings";
                ls_grpMapSettings.Enabled = false;
                lsCurCfg = null;
                return;
            }
            
            lsCurMap = ls_lstUsed.SelectedItem.ToString();
            ls_grpMapSettings.Text = "Map settings (" + lsCurMap + ")";
            ls_grpMapSettings.Enabled = true;
            
            try {
                lsCurCfg = new LSMapConfig();
                lsCurCfg.Load(lsCurMap);
            } catch (Exception ex) {
                Logger.LogError(ex);
                lsCurCfg = null;
            }
            
            if (lsCurCfg == null) return;
            LSConfig cfg = LSGame.Instance.Config;
            
            ls_numWater.Value   = lsCurCfg.WaterChance;
            ls_numFast.Value    = lsCurCfg.FastChance;
            //ls_numDestroy.Value = lsCurCfg.DestroyChance;
            ls_numFloodUp.Value = lsCurCfg.FloodUpChance;
            
            ls_numLayer.Value = lsCurCfg.LayerChance;
            ls_numCount.Value = lsCurCfg.LayerCount;
            ls_numHeight.Value = lsCurCfg.LayerHeight;
            
            ls_numRound.Value = cfg.GetRoundTime(lsCurCfg);
            ls_numFlood.Value = cfg.GetFloodTime(lsCurCfg);
            ls_numLayerTime.Value = cfg.GetLayerInterval(lsCurCfg);
        }
        
        void SaveLSMapSettings() {
            if (lsCurCfg == null) return;
            LSConfig cfg = LSGame.Instance.Config;
            
            lsCurCfg.WaterChance   = (int)ls_numWater.Value;
            lsCurCfg.FastChance    = (int)ls_numFast.Value;
            //lsCurCfg.DestroyChance = (int)ls_numDestroy.Value;
            lsCurCfg.FloodUpChance = (int)ls_numFloodUp.Value;
            
            lsCurCfg.LayerChance = (int)ls_numLayer.Value;
            lsCurCfg.LayerCount  = (int)ls_numCount.Value;
            lsCurCfg.LayerHeight = (int)ls_numHeight.Value;
            
            // TODO function for this
            if (ls_numRound.Value     != cfg.DefaultRoundTime)     lsCurCfg._RoundTime = ls_numRound.Value;
            if (ls_numFlood.Value     != cfg.DefaultFloodTime)     lsCurCfg._FloodTime = ls_numFlood.Value;
            if (ls_numLayerTime.Value != cfg.DefaultLayerInterval) lsCurCfg._LayerInterval = ls_numLayerTime.Value;
            
            lsCurCfg.Save(lsCurMap);
            lsHelper.UpdateMapConfig(lsCurMap);
        }
        
        
        void LoadTWSettings(string[] allMaps) {
             twHelper = new GamesHelper(
                TWGame.Instance, tw_cbStart, tw_cbMap, tw_cbMain,
                tw_btnStart, tw_btnStop, tw_btnEnd,
                tw_btnAdd, tw_btnRemove, tw_lstUsed, tw_lstNotUsed);
            twHelper.Load(allMaps);
            
            TWConfig cfg = TWGame.Instance.Config;
            tw_cmbDiff.SelectedIndex = (int)cfg.Difficulty;
            tw_cmbMode.SelectedIndex = (int)cfg.Mode;
        }
        
        void SaveTWSettings() {
            try {
                TWConfig cfg = TWGame.Instance.Config;
                if (tw_cmbDiff.SelectedIndex >= 0) 
                    cfg.Difficulty = (TWDifficulty)tw_cmbDiff.SelectedIndex;
                if (tw_cmbMode.SelectedIndex >= 0)
                    cfg.Mode = (TWGameMode)tw_cmbMode.SelectedIndex;
                twHelper.Save();
                SaveTWMapSettings();
            } catch (Exception ex) {
                Logger.LogError("Error saving TNT wars settings", ex);
            }
        }
        
        string twCurMap;
        TWMapConfig twCurCfg;
        void twMapUse_SelectedIndexChanged(object sender, EventArgs e) {
            SaveTWMapSettings();
            if (tw_lstUsed.SelectedIndex == -1) {
                tw_grpMapSettings.Text = "Map settings";
                tw_grpMapSettings.Enabled = false;
                twCurCfg = null;
                return;
            }
            
            twCurMap = tw_lstUsed.SelectedItem.ToString();
            tw_grpMapSettings.Text = "Map settings (" + twCurMap + ")";
            tw_grpMapSettings.Enabled = true;
            
            try {
                twCurCfg = new TWMapConfig();
                twCurCfg.Load(twCurMap);
            } catch (Exception ex) {
                Logger.LogError(ex);
                twCurCfg = null;
            }
            
            if (twCurCfg == null) return;
            tw_numScoreLimit.Value = twCurCfg.ScoreRequired;
            tw_numScorePerKill.Value = twCurCfg.ScorePerKill;
            tw_numScoreAssists.Value = twCurCfg.AssistScore;
            tw_numMultiKills.Value = twCurCfg.MultiKillBonus;
            tw_cbStreaks.Checked = twCurCfg.Streaks;
            
            tw_cbGrace.Checked = twCurCfg.GracePeriod;
            tw_numGrace.Value = twCurCfg.GracePeriodTime;
            tw_cbBalance.Checked = twCurCfg.BalanceTeams;
            tw_cbKills.Checked = twCurCfg.TeamKills;
        }
        
        void SaveTWMapSettings() {
            if (twCurCfg == null) return;
            twCurCfg.ScoreRequired = (int)tw_numScoreLimit.Value;
            twCurCfg.ScorePerKill = (int)tw_numScorePerKill.Value;
            twCurCfg.AssistScore = (int)tw_numScoreAssists.Value;
            twCurCfg.MultiKillBonus = (int)tw_numMultiKills.Value;
            twCurCfg.Streaks = tw_cbStreaks.Checked;
            
            twCurCfg.GracePeriod = tw_cbGrace.Checked;
            twCurCfg.GracePeriodTime = tw_numGrace.Value;
            twCurCfg.BalanceTeams = tw_cbBalance.Checked;
            twCurCfg.TeamKills = tw_cbKills.Checked;
            
            twCurCfg.Save(twCurMap);          
            twHelper.UpdateMapConfig(twCurMap);
        } 
        
        
        void LoadCDSettings(string[] allMaps) {
            cdHelper = new GamesHelper(
                CountdownGame.Instance, cd_cbStart, cd_cbMap, cd_cbMain,
                cd_btnStart, cd_btnStop, cd_btnEnd,
                cd_btnAdd, cd_btnRemove, cd_lstUsed, cd_lstNotUsed);
            cdHelper.Load(allMaps);
        }
        
        void SaveCDSettings() {
            try {
                cdHelper.Save();
            } catch (Exception ex) {
                Logger.LogError("Error saving Countdown settings", ex);
            }
        }
    }
}
