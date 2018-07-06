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
using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        GamesHelper lsHelper, ctfHelper, twHelper;
        
        void LoadGameProps() {
            string[] allMaps = LevelInfo.AllMapNames();
            
            ctfHelper = new GamesHelper(
                CTFGame.Instance, ctf_cbStart, ctf_cbMap, ctf_cbMain,
                ctf_btnStart, ctf_btnStop, ctf_btnEnd,
                ctf_btnAdd, ctf_btnRemove, ctf_lstUsed, ctf_lstNotUsed);
            ctfHelper.Load(allMaps);
            
            lsHelper = new GamesHelper(
                LSGame.Instance, ls_cbStart, ls_cbMap, ls_cbMain,
                ls_btnStart, ls_btnStop, ls_btnEnd,
                ls_btnAdd, ls_btnRemove, ls_lstUsed, ls_lstNotUsed);
            ls_numMax.Value = LSGame.Config.MaxLives;
            lsHelper.Load(allMaps);
            
            twHelper = new GamesHelper(
                TWGame.Instance, tw_cbStart, tw_cbMap, tw_cbMain,
                tw_btnStart, tw_btnStop, tw_btnEnd,
                tw_btnAdd, tw_btnRemove, tw_lstUsed, tw_lstNotUsed);
            twHelper.Load(allMaps);
        }

        void SaveGameProps() {
        	try {
                ctfHelper.Save();
            } catch (Exception ex) {
                Logger.LogError("Error saving CTF settings", ex);
            }
        	
        	try {
                LSGame.Config.MaxLives = (int)ls_numMax.Value;
                lsHelper.Save();
                SaveLSMapSettings();
            } catch (Exception ex) {
                Logger.LogError("Error saving Lava Survival settings", ex);
            }
        	
        	try {
                twHelper.Save();
            } catch (Exception ex) {
                Logger.LogError("Error saving TNT wars settings", ex);
            }
        }
        
        GamesHelper GetGameHelper(IGame game) {
            // TODO: Find a better way of doing this
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
            ls_numKiller.Value  = lsCurCfg.KillerChance;
            ls_numFast.Value    = lsCurCfg.FastChance;
            ls_numWater.Value   = lsCurCfg.WaterChance;
            ls_numDestroy.Value = lsCurCfg.DestroyChance;
            
            ls_numLayer.Value = lsCurCfg.LayerChance;
            ls_numCount.Value = lsCurCfg.LayerCount;
            ls_numHeight.Value = lsCurCfg.LayerHeight;
        }
        
        void SaveLSMapSettings() {
            if (lsCurCfg == null) return;
            lsCurCfg.KillerChance  = (int)ls_numKiller.Value;
            lsCurCfg.FastChance    = (int)ls_numFast.Value;
            lsCurCfg.WaterChance   = (int)ls_numWater.Value;
            lsCurCfg.DestroyChance = (int)ls_numDestroy.Value;
            
            lsCurCfg.LayerChance = (int)ls_numLayer.Value;
            lsCurCfg.LayerCount  = (int)ls_numCount.Value;
            lsCurCfg.LayerHeight = (int)ls_numHeight.Value;
            
            lsCurCfg.Save(lsCurMap);
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
        }
        
        void SaveTWMapSettings() {
            if (twCurCfg == null) return;
            twCurCfg.Save(twCurMap);
        }       

        void Tw_btnAboutClick(object sender, EventArgs e) {
        	string msg = "Difficulty:";
            msg += Environment.NewLine;
            msg += "Easy (2 Hits to die, TNT has long delay)";
            msg += Environment.NewLine;
            msg += "Normal (2 Hits to die, TNT has normal delay)";
            msg += Environment.NewLine;
            msg += "Hard (1 Hit to die, TNT has short delay and team kills are on)";
            msg += Environment.NewLine;
            msg += "Extreme (1 Hit to die, TNT has short delay, big explosion and team kills are on)";
            
            Popup.Message(msg, "Difficulty");
        }
    }
}
