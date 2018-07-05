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
using System.IO;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Games;
using MCGalaxy.Maths;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        TntWarsGame1 tw_selected;
        TWMapConfig twCfg;
        GamesHelper lsHelper;
        
        void LoadGameProps() {
            string[] allMaps = LevelInfo.AllMapNames();
            
            lsHelper = new GamesHelper(
                LSGame.Instance, ls_cbStart, ls_cbMap, ls_cbMain,
                ls_btnStart, ls_btnStop, ls_btnEnd,
                ls_btnAdd, ls_btnRemove, ls_lstUsed, ls_lstNotUsed);
            ls_numMax.Value = LSGame.Config.MaxLives;
            lsHelper.Load(allMaps);
        }

        void SaveGameProps() {
            try {
                LSGame.Config.MaxLives = (int)ls_numMax.Value;
                lsHelper.Save();
                SaveLavaMapSettings();
            } catch (Exception ex) {
                Logger.LogError("Error saving Lava Survival settings", ex);
            }
        }

        void UpdateLavaControls() {
            lsHelper.UpdateButtons();
        }
        
        GamesHelper GetGameHelper(IGame game) {
            // TODO: Find a better way of doing this
            if (game == LSGame.Instance) return lsHelper;
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
        
        public void RunOnUI_Async(Action act) { BeginInvoke(act); }

        string lsCurMap;
        void lsMapUse_SelectedIndexChanged(object sender, EventArgs e) {
            SaveLavaMapSettings();
            if (ls_lstUsed.SelectedIndex == -1) {
                ls_grpMapSettings.Text = "Map settings";
                pg_lavaMap.SelectedObject = null;
                return;
            }
            
            lsCurMap = ls_lstUsed.Items[ls_lstUsed.SelectedIndex].ToString();
            ls_grpMapSettings.Text = "Map settings (" + lsCurMap + ")";
            
            try {
                LSMapConfig cfg = new LSMapConfig();
                cfg.Load(lsCurMap);
                pg_lavaMap.SelectedObject = new LavaMapProperties(cfg);
            } catch (Exception ex) {
                Logger.LogError(ex);
                pg_lavaMap.SelectedObject = null;
            }
        }
        
        void SaveLavaMapSettings() {
            if (pg_lavaMap.SelectedObject == null) return;
            LavaMapProperties props = (LavaMapProperties)pg_lavaMap.SelectedObject;
            props.m.Save(lsCurMap);
        }

        public void LoadTNTWarsTab(object sender, EventArgs e) {
            if (tw_selected == null) {
                //Clear all
                //Top
                SlctdTntWrsLvl.Text = "";
                tw_txtStatus.Text = "";
                tw_txtPlayers.Text = "";
                //Difficulty
                TntWrsDiffCombo.Text = "";
                TntWrsDiffCombo.Enabled = false;
                TntWrsDiffSlctBt.Enabled = false;
                //scores
                tw_numScoreLimit.Value = 150;
                tw_numScoreLimit.Enabled = false;
                tw_numScorePerKill.Value = 10;
                tw_numScorePerKill.Enabled = false;
                tw_cbScoreAssists.Checked = true;
                tw_cbScoreAssists.Enabled = false;
                tw_numScoreAssists.Value = 5;
                tw_numScoreAssists.Enabled = false;
                tw_cbMultiKills.Checked = true;
                tw_cbMultiKills.Enabled = false;
                tw_numMultiKills.Value = 5;
                tw_numMultiKills.Enabled = false;
                //Grace period
                TntWrsGracePrdChck.Checked = true;
                TntWrsGracePrdChck.Enabled = false;
                TntWrsGraceTimeChck.Value = 30;
                TntWrsGraceTimeChck.Enabled = false;
                //Teams
                TntWrsTmsChck.Checked = true;
                TntWrsTmsChck.Enabled = false;
                tw_cbBalanceTeams.Checked = true;
                tw_cbBalanceTeams.Enabled = false;
                tw_cbTeamKills.Checked = false;
                tw_cbTeamKills.Enabled = false;
                //Status
                tw_btnStartGame.Enabled = false;
                tw_btnEndGame.Enabled = false;
                tw_btnResetGame.Enabled = false;
                tw_btnDeleteGame.Enabled = false;
                //Other
                tw_cbStreaks.Checked = true;
                tw_cbStreaks.Enabled = false;
                //New game
                if (TntWrsMpsList.SelectedIndex < 0) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                tw_lstGames.Items.Clear();
                TntWrsDiffCombo.Items.Clear();
                
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    TntWarsGame1 game = TntWarsGame1.Find(lvl);
                    if (game == null) {
                        TntWrsMpsList.Items.Add(lvl.name);
                    } else {
                        string desc = DescribeTNTWars(lvl, game);
                        tw_lstGames.Items.Add(desc);
                    }
                }
                
                TntWrsDiffCombo.Items.Add("Easy");
                TntWrsDiffCombo.Items.Add("Normal");
                TntWrsDiffCombo.Items.Add("Hard");
                TntWrsDiffCombo.Items.Add("Extreme");
            } else {
                //Load settings
                //Top
                SlctdTntWrsLvl.Text = tw_selected.lvl.name;
                twCfg = new TWMapConfig();
                twCfg.SetDefaults(tw_selected.lvl);
                twCfg.Load(tw_selected.lvl.name);
                
                if (tw_selected.GameStatus == TntWarsGame1.TntWarsStatus.WaitingForPlayers) tw_txtStatus.Text = "Waiting For Players";
                if (tw_selected.GameStatus == TntWarsGame1.TntWarsStatus.AboutToStart) tw_txtStatus.Text = "Starting";
                if (tw_selected.GameStatus == TntWarsGame1.TntWarsStatus.GracePeriod) tw_txtStatus.Text = "Started";
                if (tw_selected.GameStatus == TntWarsGame1.TntWarsStatus.InProgress) tw_txtStatus.Text = "In Progress";
                if (tw_selected.GameStatus == TntWarsGame1.TntWarsStatus.Finished) tw_txtStatus.Text = "Finished";
                tw_txtPlayers.Text = tw_selected.PlayingPlayers().ToString(CultureInfo.InvariantCulture);
                
                //Difficulty
                if (tw_selected.GameStatus == TntWarsGame1.TntWarsStatus.WaitingForPlayers) {
                    TntWrsDiffCombo.Enabled = true;
                    TntWrsDiffSlctBt.Enabled = true;
                } else {
                    TntWrsDiffCombo.Enabled = false;
                    TntWrsDiffSlctBt.Enabled = false;
                }
                
                TntWrsDiffCombo.SelectedIndex = TntWrsDiffCombo.FindString(tw_selected.Difficulty.ToString());
                //scores
                tw_numScoreLimit.Value = twCfg.ScoreRequired;
                tw_numScoreLimit.Enabled = true;
                tw_numScorePerKill.Value = twCfg.ScorePerKill;
                tw_numScorePerKill.Enabled = true;
                
                if (twCfg.AssistScore == 0) {
                    tw_cbScoreAssists.Checked = false;
                    tw_cbScoreAssists.Enabled = true;
                    tw_numScoreAssists.Enabled = false;
                } else {
                    tw_numScoreAssists.Value = twCfg.AssistScore;
                    tw_numScoreAssists.Enabled = true;
                    tw_cbScoreAssists.Checked = true;
                    tw_cbScoreAssists.Enabled = true;
                }
                
                if (twCfg.MultiKillBonus == 0) {
                    tw_cbMultiKills.Checked = false;
                    tw_cbMultiKills.Enabled = true;
                    tw_numMultiKills.Enabled = false;
                } else {
                    tw_numMultiKills.Value = twCfg.MultiKillBonus;
                    tw_numMultiKills.Enabled = true;
                    tw_cbMultiKills.Checked = true;
                    tw_cbMultiKills.Enabled = true;
                }
                
                //Grace period
                TntWrsGracePrdChck.Checked = twCfg.InitialGracePeriod;
                TntWrsGracePrdChck.Enabled = true;
                TntWrsGraceTimeChck.Value = twCfg.GracePeriodSeconds;
                TntWrsGraceTimeChck.Enabled = twCfg.InitialGracePeriod;
                
                //Teams
                TntWrsTmsChck.Checked = tw_selected.GameMode == TntWarsGameMode.TDM;
                TntWrsTmsChck.Enabled = true;
                tw_cbBalanceTeams.Checked = twCfg.BalanceTeams;
                tw_cbBalanceTeams.Enabled = true;
                tw_cbTeamKills.Checked = twCfg.TeamKills;
                tw_cbTeamKills.Enabled = true;
                
                //Status
                switch (tw_selected.GameStatus) {
                    case TntWarsGame1.TntWarsStatus.WaitingForPlayers:
                        tw_btnStartGame.Enabled = true;
                        tw_btnEndGame.Enabled = false;
                        tw_btnResetGame.Enabled = false;
                        tw_btnDeleteGame.Enabled = true;
                        break;

                    case TntWarsGame1.TntWarsStatus.AboutToStart:
                    case TntWarsGame1.TntWarsStatus.GracePeriod:
                    case TntWarsGame1.TntWarsStatus.InProgress:
                        tw_btnStartGame.Enabled = false;
                        tw_btnEndGame.Enabled = true;
                        tw_btnResetGame.Enabled = false;
                        tw_btnDeleteGame.Enabled = false;
                        break;

                    case TntWarsGame1.TntWarsStatus.Finished:
                        tw_btnStartGame.Enabled = false;
                        tw_btnEndGame.Enabled = false;
                        tw_btnResetGame.Enabled = true;
                        tw_btnDeleteGame.Enabled = true;
                        break;

                }
                //Other
                tw_cbStreaks.Checked = twCfg.Streaks;
                tw_cbStreaks.Enabled = true;
                //New game
                if (TntWrsMpsList.SelectedIndex < 0) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                tw_lstGames.Items.Clear();
                TntWrsDiffCombo.Items.Clear();
                
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    TntWarsGame1 game = TntWarsGame1.Find(lvl);
                    if (game == null) {
                        TntWrsMpsList.Items.Add(lvl.name);
                    } else {
                        string desc = "";
                        if (game == tw_selected) desc += "-->  ";
                        desc += DescribeTNTWars(lvl, game);
                        tw_lstGames.Items.Add(desc);
                    }
                }
                
                TntWrsDiffCombo.Items.Add("Easy");
                TntWrsDiffCombo.Items.Add("Normal");
                TntWrsDiffCombo.Items.Add("Hard");
                TntWrsDiffCombo.Items.Add("Extreme");

                //Disable things because game is in progress
                if (tw_selected.GameStatus != TntWarsGame1.TntWarsStatus.WaitingForPlayers) {
                    //Difficulty
                    TntWrsDiffCombo.Enabled = false;
                    TntWrsDiffSlctBt.Enabled = false;
                    //scores
                    tw_numScoreLimit.Enabled = false;
                    tw_numScorePerKill.Enabled = false;
                    tw_cbScoreAssists.Enabled = false;
                    tw_numScoreAssists.Enabled = false;
                    tw_cbMultiKills.Enabled = false;
                    tw_numMultiKills.Enabled = false;
                    //Grace period
                    TntWrsGracePrdChck.Enabled = false;
                    TntWrsGraceTimeChck.Enabled = false;
                    //Teams
                    TntWrsTmsChck.Enabled = false;
                    tw_cbBalanceTeams.Enabled = false;
                    tw_cbTeamKills.Enabled = false;
                    //Other
                    tw_cbStreaks.Enabled = false;
                }
            }
        }
        
        string DescribeTNTWars(Level lvl, TntWarsGame1 game) {
            string msg = lvl.name;

            msg += " - ";
            if (game.GameMode == TntWarsGameMode.FFA) msg += "FFA";
            if (game.GameMode == TntWarsGameMode.TDM) msg += "TDM";
            
            msg += " - ";
            if (game.Difficulty == TntWarsDifficulty.Easy)    msg += "(Easy)";
            if (game.Difficulty == TntWarsDifficulty.Normal)  msg += "(Normal)";
            if (game.Difficulty == TntWarsDifficulty.Hard)    msg += "(Hard)";
            if (game.Difficulty == TntWarsDifficulty.Extreme) msg += "(Extreme)";
            
            msg += " - ";
            if (game.GameStatus == TntWarsGame1.TntWarsStatus.WaitingForPlayers) msg += "(Waiting For Players)";
            if (game.GameStatus == TntWarsGame1.TntWarsStatus.AboutToStart)      msg += "(Starting)";
            if (game.GameStatus == TntWarsGame1.TntWarsStatus.GracePeriod)       msg += "(Started)";
            if (game.GameStatus == TntWarsGame1.TntWarsStatus.InProgress)        msg += "(In Progress)";
            if (game.GameStatus == TntWarsGame1.TntWarsStatus.Finished)          msg += "(Finished)";
            
            return msg;
        }

        void tabControl2_Click(object sender, EventArgs e) {
            LoadTNTWarsTab(sender, e);
        }

        void EditTntWarsGameBT_Click(object sender, EventArgs e) {
            try {
                string slctd = tw_lstGames.Items[tw_lstGames.SelectedIndex].ToString();
                if (slctd.StartsWith("-->")) {
                    LoadTNTWarsTab(sender, e);
                    return;
                }
                string[] split = slctd.Split(new string[] { " - " }, StringSplitOptions.None);
                tw_selected = TntWarsGame1.Find(LevelInfo.FindExact(split[0]));
                LoadTNTWarsTab(sender, e);
            }
            catch { }
        }

        void TntWrsMpsList_SelectedIndexChanged(object sender, EventArgs e) {
            TntWrsCrtNwTntWrsBt.Enabled = TntWrsMpsList.SelectedIndex >= 0;
        }

        void TntWrsCrtNwTntWrsBt_Click(object sender, EventArgs e) {
            TntWarsGame1 it = null;
            try {
                it = new TntWarsGame1(LevelInfo.FindExact(TntWrsMpsList.Items[TntWrsMpsList.SelectedIndex].ToString()));
            }
            catch { }
            if (it == null) return;
            TntWarsGame1.GameList.Add(it);
            tw_selected = it;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsDiffSlctBt_Click(object sender, EventArgs e) {
            TWGame game = TWGame.Instance;
            int diff = TntWrsDiffCombo.SelectedIndex;
            
            if (diff >= 0) game.SetDifficulty((TntWarsDifficulty)diff);
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsScrLmtUpDwn_ValueChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.ScoreRequired = (int)tw_numScoreLimit.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsScrPrKlUpDwn_ValueChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.ScorePerKill = (int)tw_numScorePerKill.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsAsstChck_CheckedChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            if (!tw_cbScoreAssists.Checked) {
                twCfg.AssistScore = 0;
                tw_numScoreAssists.Enabled = false;
            } else {
                twCfg.AssistScore = (int)tw_numScoreAssists.Value;
                tw_numScoreAssists.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsAstsScrUpDwn_ValueChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.AssistScore = (int)tw_numScoreAssists.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsMltiKlChck_CheckedChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            if (!tw_cbMultiKills.Checked) {
                twCfg.MultiKillBonus = 0;
                tw_numMultiKills.Enabled = false;
            } else {
                twCfg.MultiKillBonus = (int)tw_numMultiKills.Value;
                tw_numMultiKills.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsMltiKlScPrUpDown_ValueChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.MultiKillBonus = (int)tw_numMultiKills.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsGracePrdChck_CheckedChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.InitialGracePeriod = TntWrsGracePrdChck.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsGraceTimeChck_ValueChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.GracePeriodSeconds = (int)TntWrsGraceTimeChck.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsTmsChck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            TWGame game = TWGame.Instance;
            
            if (TntWrsTmsChck.Checked) {
                if (TWGame.Config.Mode == TntWarsGameMode.FFA) game.ModeTDM();
            } else {
                if (TWGame.Config.Mode == TntWarsGameMode.TDM) game.ModeFFA();
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsBlnceTeamsChck_CheckedChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.BalanceTeams = tw_cbBalanceTeams.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsTKchck_CheckedChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.TeamKills = tw_cbTeamKills.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsStreaksChck_CheckedChanged(object sender, EventArgs e) {
            if (twCfg == null) return;
            twCfg.Streaks = tw_cbStreaks.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsStrtGame_Click(object sender, EventArgs e) {
            if (tw_selected == null) return;
            if (tw_selected.PlayingPlayers() >= 2) {
                new Thread(tw_selected.Start).Start();
            } else {
                Popup.Warning("Not enough players (2 or more needed!)");
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsEndGame_Click(object sender, EventArgs e) {
            if (tw_selected == null) return;
            foreach (TntWarsGame1.player pl in tw_selected.Players) {
                pl.p.canBuild = true;
                pl.p.PlayingTntWars = false;
                pl.p.CurrentAmountOfTnt = 0;
            }
            tw_selected.GameStatus = TntWarsGame1.TntWarsStatus.Finished;
            tw_selected.MessageAll("TNT wars: Game has been stopped!");
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsRstGame_Click(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.GameStatus = TntWarsGame1.TntWarsStatus.WaitingForPlayers;
            Command.Find("Restore").Use(null, tw_selected.BackupNumber + tw_selected.lvl.name);
            tw_selected.RedScore = 0;
            tw_selected.BlueScore = 0;
            foreach (TntWarsGame1.player pl in tw_selected.Players) {
                pl.Score = 0;
                pl.spec = false;
                pl.p.TntWarsKillStreak = 0;
                pl.p.TNTWarsLastKillStreakAnnounced = 0;
                pl.p.CurrentAmountOfTnt = 0;
                pl.p.CurrentTntGameNumber = tw_selected.GameNumber;
                pl.p.PlayingTntWars = false;
                pl.p.canBuild = true;
                pl.p.TntWarsHealth = 2;
                pl.p.TntWarsScoreMultiplier = 1f;
                pl.p.inTNTwarsMap = true;
                pl.p.HarmedBy = null;
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsDltGame_Click(object sender, EventArgs e) {
            if (tw_selected == null) return;
            foreach (TntWarsGame1.player pl in tw_selected.Players) {
                pl.p.CurrentTntGameNumber = -1;
                Player.Message(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                pl.p.PlayingTntWars = false;
                pl.p.canBuild = true;
                TntWarsGame1.SetTitlesAndColor(pl, true);
            }
            TntWarsGame1.GameList.Remove(tw_selected);
            tw_selected = null;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsDiffAboutBt_Click(object sender, EventArgs e) {
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
