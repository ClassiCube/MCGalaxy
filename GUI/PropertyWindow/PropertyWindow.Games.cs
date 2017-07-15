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
        System.Timers.Timer lavaUpdateTimer;

        private void LoadLavaSettings() {
        }

        private void SaveLavaSettings() {
            Server.lava.SaveSettings();
            SaveLavaMapSettings();
        }

        private void UpdateLavaControls() {
            try {
                ls_btnStartGame.Enabled = !Server.lava.active;
                ls_btnStopGame.Enabled = Server.lava.active;
                ls_btnEndRound.Enabled = Server.lava.roundActive;
                lsBtnEndVote.Enabled = Server.lava.voteActive;
            }
            catch { }
        }

        private void lsBtnStartGame_Click(object sender, EventArgs e) {
            if ( !Server.lava.active ) Server.lava.Start();
            UpdateLavaControls();
        }

        private void lsBtnStopGame_Click(object sender, EventArgs e) {
            if ( Server.lava.active ) Server.lava.Stop();
            UpdateLavaControls();
        }

        private void lsBtnEndRound_Click(object sender, EventArgs e) {
            if ( Server.lava.roundActive ) Server.lava.EndRound();
            UpdateLavaControls();
        }

        private void UpdateLavaMapList(bool useList = true, bool noUseList = true) {
            if ( !useList && !noUseList ) return;
            try {
                if ( this.InvokeRequired ) {
                    this.Invoke(new MethodInvoker(delegate { try { UpdateLavaMapList(useList, noUseList); } catch { } }));
                    return;
                }

                int useIndex = ls_lstUsed.SelectedIndex, noUseIndex = ls_lstNotUsed.SelectedIndex;
                if ( useList ) ls_lstUsed.Items.Clear();
                if ( noUseList ) ls_lstNotUsed.Items.Clear();

                if ( useList ) {
                    ls_lstUsed.Items.AddRange(Server.lava.Maps.ToArray());
                    try { if ( useIndex > -1 ) ls_lstUsed.SelectedIndex = useIndex; }
                    catch { }
                }
                if ( noUseList ) {
                    string[] files = LevelInfo.AllMapFiles();
                    foreach (string file in files) {
                        try {
                            string name = Path.GetFileNameWithoutExtension(file);
                            if ( name.ToLower() != Server.mainLevel.name && !Server.lava.HasMap(name) )
                                ls_lstNotUsed.Items.Add(name);
                        }
                        catch ( NullReferenceException ) { }
                    }
                    try { if ( noUseIndex > -1 ) ls_lstNotUsed.SelectedIndex = noUseIndex; }
                    catch { }
                }
            }
            catch ( ObjectDisposedException ) { }  //Y U BE ANNOYING 
            catch ( Exception ex ) { Logger.LogError(ex); }
        }

        private void lsAddMap_Click(object sender, EventArgs e) {
            try {
                Server.lava.Stop(); // Doing this so we don't break something...
                UpdateLavaControls();

                string name;
                try { name = ls_lstNotUsed.Items[ls_lstNotUsed.SelectedIndex].ToString(); }
                catch { return; }

                if ( LevelInfo.FindExact(name) == null )
                    Command.all.Find("load").Use(null, name);
                Level level = LevelInfo.FindExact(name);
                if ( level == null ) return;

                Server.lava.AddMap(name);

                LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(level.name);
                settings.blockFlood = new Vec3U16((ushort)( level.Width / 2 ), (ushort)( level.Height - 1 ), (ushort)( level.Length / 2 ));
                settings.blockLayer = new Vec3U16(0, (ushort)( level.Height / 2 ), 0);
                ushort x = (ushort)( level.Width / 2 ), y = (ushort)( level.Height / 2 ), z = (ushort)( level.Length / 2 );
                settings.safeZone = new Vec3U16[] { new Vec3U16((ushort)( x - 3 ), y, (ushort)( z - 3 )), new Vec3U16((ushort)( x + 3 ), (ushort)( y + 4 ), (ushort)( z + 3 )) };
                Server.lava.SaveMapSettings(settings);

                level.Config.MOTD = "Lava Survival: " + level.name.Capitalize();
                level.Config.PhysicsOverload = 1000000;
                level.Config.AutoUnload = false;
                level.Config.LoadOnGoto = false;
                Level.SaveSettings(level);
                level.Unload(true);

                UpdateLavaMapList();
            }
            catch ( Exception ex ) { Logger.LogError(ex); }
        }

        private void lsRemoveMap_Click(object sender, EventArgs e) {
            try {
                Server.lava.Stop(); // Doing this so we don't break something...
                UpdateLavaControls();

                string name;
                try { name = ls_lstUsed.Items[ls_lstUsed.SelectedIndex].ToString(); }
                catch { return; }

                if ( LevelInfo.FindExact(name) == null )
                    Command.all.Find("load").Use(null, name);
                Level level = LevelInfo.FindExact(name);
                if ( level == null ) return;

                Server.lava.RemoveMap(name);
                level.Config.MOTD = "ignore";
                level.Config.PhysicsOverload = 1500;
                level.Config.AutoUnload = true;
                level.Config.LoadOnGoto = true;
                Level.SaveSettings(level);
                level.Unload(true);

                UpdateLavaMapList();
            }
            catch ( Exception ex ) { Logger.LogError(ex); }
        }

        private void lsMapUse_SelectedIndexChanged(object sender, EventArgs e) {
            SaveLavaMapSettings();
            if (ls_lstUsed.SelectedIndex == -1) {
                ls_grpMapSettings.Text = "Map settings";
                pg_lavaMap.SelectedObject = null;
                return;
            }
            
            string name = ls_lstUsed.Items[ls_lstUsed.SelectedIndex].ToString();
            ls_grpMapSettings.Text = "Map settings (" + name + ")";
            
            try {
                LavaSurvival.MapSettings m = Server.lava.LoadMapSettings(name);
                pg_lavaMap.SelectedObject = new LavaMapProperties(m);
            } catch (Exception ex) { 
                Logger.LogError(ex); 
                pg_lavaMap.SelectedObject = null;
            }
        }
        
        void SaveLavaMapSettings() {
            if (pg_lavaMap.SelectedObject == null) return;
            LavaMapProperties props = (LavaMapProperties)pg_lavaMap.SelectedObject;
            Server.lava.SaveMapSettings(props.m);
        }

        private void lsBtnEndVote_Click(object sender, EventArgs e) {
            if ( Server.lava.voteActive ) Server.lava.EndVote();
            UpdateLavaControls();
        }

        public void LoadTNTWarsTab(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) {
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
                if ( TntWrsMpsList.SelectedIndex < 0 ) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                tw_lstGames.Items.Clear();
                TntWrsDiffCombo.Items.Clear();
                foreach ( Level lvl in Server.levels ) {
                    if ( TntWarsGame.Find(lvl) == null ) {
                        TntWrsMpsList.Items.Add(lvl.name);
                    }
                    else {
                        TntWarsGame T = TntWarsGame.Find(lvl);
                        string msg = lvl.name + " - ";
                        if ( T.GameMode == TntWarsGame.TntWarsGameMode.FFA ) msg += "FFA";
                        if ( T.GameMode == TntWarsGame.TntWarsGameMode.TDM ) msg += "TDM";
                        msg += " - ";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Easy ) msg += "(Easy)";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Normal ) msg += "(Normal)";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Hard ) msg += "(Hard)";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Extreme ) msg += "(Extreme)";
                        msg += " - ";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) msg += "(Waiting For Players)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ) msg += "(Starting)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ) msg += "(Started)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress ) msg += "(In Progress)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.Finished ) msg += "(Finished)";
                        tw_lstGames.Items.Add(msg);
                    }
                }
                TntWrsDiffCombo.Items.Add("Easy");
                TntWrsDiffCombo.Items.Add("Normal");
                TntWrsDiffCombo.Items.Add("Hard");
                TntWrsDiffCombo.Items.Add("Extreme");
            }
            else {
                //Load settings
                //Top
                SlctdTntWrsLvl.Text = TntWarsGame.GuiLoaded.lvl.name;
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) tw_txtStatus.Text = "Waiting For Players";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ) tw_txtStatus.Text = "Starting";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ) tw_txtStatus.Text = "Started";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress ) tw_txtStatus.Text = "In Progress";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.Finished ) tw_txtStatus.Text = "Finished";
                tw_txtPlayers.Text = TntWarsGame.GuiLoaded.PlayingPlayers().ToString(CultureInfo.InvariantCulture);
                //Difficulty
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) {
                    TntWrsDiffCombo.Enabled = true;
                    TntWrsDiffSlctBt.Enabled = true;
                }
                else {
                    TntWrsDiffCombo.Enabled = false;
                    TntWrsDiffSlctBt.Enabled = false;
                }
                TntWrsDiffCombo.SelectedIndex = TntWrsDiffCombo.FindString(TntWarsGame.GuiLoaded.Difficulty.ToString());
                //scores
                tw_numScoreLimit.Value = TntWarsGame.GuiLoaded.ScoreLimit;
                tw_numScoreLimit.Enabled = true;
                tw_numScorePerKill.Value = TntWarsGame.GuiLoaded.ScorePerKill;
                tw_numScorePerKill.Enabled = true;
                if ( TntWarsGame.GuiLoaded.ScorePerAssist == 0 ) {
                    tw_cbScoreAssists.Checked = false;
                    tw_cbScoreAssists.Enabled = true;
                    tw_numScoreAssists.Enabled = false;
                }
                else {
                    tw_numScoreAssists.Value = TntWarsGame.GuiLoaded.ScorePerAssist;
                    tw_numScoreAssists.Enabled = true;
                    tw_cbScoreAssists.Checked = true;
                    tw_cbScoreAssists.Enabled = true;
                }
                if ( TntWarsGame.GuiLoaded.MultiKillBonus == 0 ) {
                    tw_cbMultiKills.Checked = false;
                    tw_cbMultiKills.Enabled = true;
                    tw_numMultiKills.Enabled = false;
                }
                else {
                    tw_numMultiKills.Value = TntWarsGame.GuiLoaded.MultiKillBonus;
                    tw_numMultiKills.Enabled = true;
                    tw_cbMultiKills.Checked = true;
                    tw_cbMultiKills.Enabled = true;
                }
                //Grace period
                TntWrsGracePrdChck.Checked = TntWarsGame.GuiLoaded.GracePeriod;
                TntWrsGracePrdChck.Enabled = true;
                TntWrsGraceTimeChck.Value = TntWarsGame.GuiLoaded.GracePeriodSecs;
                TntWrsGraceTimeChck.Enabled = TntWarsGame.GuiLoaded.GracePeriod;
                //Teams
                TntWrsTmsChck.Checked = TntWarsGame.GuiLoaded.GameMode == TntWarsGame.TntWarsGameMode.TDM;
                TntWrsTmsChck.Enabled = true;
                tw_cbBalanceTeams.Checked = TntWarsGame.GuiLoaded.BalanceTeams;
                tw_cbBalanceTeams.Enabled = true;
                tw_cbTeamKills.Checked = TntWarsGame.GuiLoaded.TeamKills;
                tw_cbTeamKills.Enabled = true;
                //Status
                switch ( TntWarsGame.GuiLoaded.GameStatus ) {
                    case TntWarsGame.TntWarsGameStatus.WaitingForPlayers:
                        if ( TntWarsGame.GuiLoaded.CheckAllSetUp(null, false, false) ) tw_btnStartGame.Enabled = true;
                        tw_btnEndGame.Enabled = false;
                        tw_btnResetGame.Enabled = false;
                        tw_btnDeleteGame.Enabled = true;
                        break;

                    case TntWarsGame.TntWarsGameStatus.AboutToStart:
                    case TntWarsGame.TntWarsGameStatus.GracePeriod:
                    case TntWarsGame.TntWarsGameStatus.InProgress:
                        tw_btnStartGame.Enabled = false;
                        tw_btnEndGame.Enabled = true;
                        tw_btnResetGame.Enabled = false;
                        tw_btnDeleteGame.Enabled = false;
                        break;

                    case TntWarsGame.TntWarsGameStatus.Finished:
                        tw_btnStartGame.Enabled = false;
                        tw_btnEndGame.Enabled = false;
                        tw_btnResetGame.Enabled = true;
                        tw_btnDeleteGame.Enabled = true;
                        break;

                }
                //Other
                tw_cbStreaks.Checked = TntWarsGame.GuiLoaded.Config.Streaks;
                tw_cbStreaks.Enabled = true;
                //New game
                if ( TntWrsMpsList.SelectedIndex < 0 ) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                tw_lstGames.Items.Clear();
                TntWrsDiffCombo.Items.Clear();
                foreach ( Level lvl in Server.levels ) {
                    if ( TntWarsGame.Find(lvl) == null ) {
                        TntWrsMpsList.Items.Add(lvl.name);
                    }
                    else {
                        TntWarsGame T = TntWarsGame.Find(lvl);
                        string msg = "";
                        if ( T == TntWarsGame.GuiLoaded ) { msg += "-->  "; }
                        msg += lvl.name + " - ";
                        if ( T.GameMode == TntWarsGame.TntWarsGameMode.FFA ) msg += "FFA";
                        if ( T.GameMode == TntWarsGame.TntWarsGameMode.TDM ) msg += "TDM";
                        msg += " - ";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Easy ) msg += "(Easy)";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Normal ) msg += "(Normal)";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Hard ) msg += "(Hard)";
                        if ( T.Difficulty == TntWarsGame.TntWarsDifficulty.Extreme ) msg += "(Extreme)";
                        msg += " - ";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) msg += "(Waiting For Players)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ) msg += "(Starting)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ) msg += "(Started)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress ) msg += "(In Progress)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.Finished ) msg += "(Finished)";
                        tw_lstGames.Items.Add(msg);
                    }
                }
                TntWrsDiffCombo.Items.Add("Easy");
                TntWrsDiffCombo.Items.Add("Normal");
                TntWrsDiffCombo.Items.Add("Hard");
                TntWrsDiffCombo.Items.Add("Extreme");

                //Disable things because game is in progress
                if ( !TntWarsEditable(sender, e) ) {
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

        private bool TntWarsEditable(object sender, EventArgs e) {
            return TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers;
        }

        private void tabControl2_Click(object sender, EventArgs e) {
            LoadTNTWarsTab(sender, e);
        }

        private void EditTntWarsGameBT_Click(object sender, EventArgs e) {
            try {
                string slctd = tw_lstGames.Items[tw_lstGames.SelectedIndex].ToString();
                if ( slctd.StartsWith("-->") ) {
                    LoadTNTWarsTab(sender, e);
                    return;
                }
                string[] split = slctd.Split(new string[] { " - " }, StringSplitOptions.None);
                TntWarsGame.GuiLoaded = TntWarsGame.Find(LevelInfo.FindExact(split[0]));
                LoadTNTWarsTab(sender, e);
            }
            catch { }
        }

        private void TntWrsMpsList_SelectedIndexChanged(object sender, EventArgs e) {
            TntWrsCrtNwTntWrsBt.Enabled = TntWrsMpsList.SelectedIndex >= 0;
        }

        private void TntWrsCrtNwTntWrsBt_Click(object sender, EventArgs e) {
            TntWarsGame it = null;
            try {
                it = new TntWarsGame(LevelInfo.FindExact(TntWrsMpsList.Items[TntWrsMpsList.SelectedIndex].ToString()));
            }
            catch { }
            if ( it == null ) return;
            TntWarsGame.GameList.Add(it);
            TntWarsGame.GuiLoaded = it;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsDiffSlctBt_Click(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            switch ( TntWrsDiffCombo.Items[TntWrsDiffCombo.SelectedIndex].ToString() ) {
                case "Easy":
                    TntWarsGame.GuiLoaded.Difficulty = TntWarsGame.TntWarsDifficulty.Easy;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to easy!");
                    TntWarsGame.GuiLoaded.TeamKills = false;
                    break;

                case "Normal":
                    TntWarsGame.GuiLoaded.Difficulty = TntWarsGame.TntWarsDifficulty.Normal;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to normal!");
                    TntWarsGame.GuiLoaded.TeamKills = false;
                    break;

                case "Hard":
                    TntWarsGame.GuiLoaded.Difficulty = TntWarsGame.TntWarsDifficulty.Hard;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to hard!");
                    TntWarsGame.GuiLoaded.TeamKills = true;
                    break;

                case "Extreme":
                    TntWarsGame.GuiLoaded.Difficulty = TntWarsGame.TntWarsDifficulty.Extreme;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to extreme!");
                    TntWarsGame.GuiLoaded.TeamKills = true;
                    break;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsScrLmtUpDwn_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.ScoreLimit = (int)tw_numScoreLimit.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsScrPrKlUpDwn_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.ScorePerKill = (int)tw_numScorePerKill.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsAsstChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            if ( tw_cbScoreAssists.Checked == false ) {
                TntWarsGame.GuiLoaded.ScorePerAssist = 0;
                tw_numScoreAssists.Enabled = false;
            }
            else {
                TntWarsGame.GuiLoaded.ScorePerAssist = (int)tw_numScoreAssists.Value;
                tw_numScoreAssists.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsAstsScrUpDwn_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.ScorePerAssist = (int)tw_numScoreAssists.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsMltiKlChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            if ( tw_cbMultiKills.Checked == false ) {
                TntWarsGame.GuiLoaded.MultiKillBonus = 0;
                tw_numMultiKills.Enabled = false;
            }
            else {
                TntWarsGame.GuiLoaded.MultiKillBonus = (int)tw_numMultiKills.Value;
                tw_numMultiKills.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsMltiKlScPrUpDown_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.MultiKillBonus = (int)tw_numMultiKills.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsGracePrdChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.GracePeriod = TntWrsGracePrdChck.Checked;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsGraceTimeChck_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.GracePeriodSecs = (int)TntWrsGraceTimeChck.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsTmsChck_CheckedChanged(object sender, EventArgs e) {
            switch ( TntWrsTmsChck.Checked ) {
                case true:
                    if ( TntWarsGame.GuiLoaded.GameMode == TntWarsGame.TntWarsGameMode.FFA ) {
                        TntWarsGame.GuiLoaded.GameMode = TntWarsGame.TntWarsGameMode.TDM;
                        foreach ( TntWarsGame.player pl in TntWarsGame.GuiLoaded.Players ) {
                            {
                                Player.Message(pl.p, "TNT Wars: Changed gamemode to Team Deathmatch");
                                pl.Red = false;
                                pl.Blue = false;
                                if ( TntWarsGame.GuiLoaded.BlueTeam() > TntWarsGame.GuiLoaded.RedTeam() ) {
                                    pl.Red = true;
                                }
                                else if ( TntWarsGame.GuiLoaded.RedTeam() > TntWarsGame.GuiLoaded.BlueTeam() ) {
                                    pl.Blue = true;
                                }
                                else if ( TntWarsGame.GuiLoaded.RedScore > TntWarsGame.GuiLoaded.BlueScore ) {
                                    pl.Blue = true;
                                }
                                else if ( TntWarsGame.GuiLoaded.BlueScore > TntWarsGame.GuiLoaded.RedScore ) {
                                    pl.Red = true;
                                }
                                else {
                                    pl.Red = true;
                                }
                            }
                            {
                                string mesg = pl.p.ColoredName + " %Sis now";
                                if ( pl.Red ) {
                                    mesg += " on the " + Colors.red + "red team";
                                }
                                if ( pl.Blue ) {
                                    mesg += " on the " + Colors.blue + "blue team";
                                }
                                if ( pl.spec ) {
                                    mesg += " (as a spectator)";
                                }
                                Chat.MessageGlobal(mesg);
                            }
                        }
                        if ( TntWarsGame.GuiLoaded.ScoreLimit == TntWarsGame.Properties.DefaultFFAmaxScore ) {
                            TntWarsGame.GuiLoaded.ScoreLimit = TntWarsGame.Properties.DefaultTDMmaxScore;
                        }
                    }
                    break;

                case false:
                    if ( TntWarsGame.GuiLoaded.GameMode == TntWarsGame.TntWarsGameMode.TDM ) {
                        TntWarsGame.GuiLoaded.GameMode = TntWarsGame.TntWarsGameMode.FFA;
                        TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed gamemode to Free For All");
                        if ( TntWarsGame.GuiLoaded.ScoreLimit == TntWarsGame.Properties.DefaultTDMmaxScore ) {
                            TntWarsGame.GuiLoaded.ScoreLimit = TntWarsGame.Properties.DefaultFFAmaxScore;
                        }
                        foreach ( TntWarsGame.player pl in TntWarsGame.GuiLoaded.Players ) {
                            pl.p.color = pl.OldColor;
                            pl.p.SetPrefix();
                        }
                    }
                    break;
            }
        }

        private void TntWrsBlnceTeamsChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.BalanceTeams = tw_cbBalanceTeams.Checked;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsTKchck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.TeamKills = tw_cbTeamKills.Checked;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsStreaksChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.Config.Streaks = tw_cbStreaks.Checked;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsStrtGame_Click(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            if ( TntWarsGame.GuiLoaded.PlayingPlayers() >= 2 ) {
                new Thread(TntWarsGame.GuiLoaded.Start).Start();
            }
            else {
                MessageBox.Show("Not enough players (2 or more needed!)", "More players needed!");
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsEndGame_Click(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            foreach ( TntWarsGame.player pl in TntWarsGame.GuiLoaded.Players ) {
                pl.p.canBuild = true;
                pl.p.PlayingTntWars = false;
                pl.p.CurrentAmountOfTnt = 0;
            }
            TntWarsGame.GuiLoaded.GameStatus = TntWarsGame.TntWarsGameStatus.Finished;
            TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT wars: Game has been stopped!");
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsRstGame_Click(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.GameStatus = TntWarsGame.TntWarsGameStatus.WaitingForPlayers;
            Command.all.Find("restore").Use(null, TntWarsGame.GuiLoaded.BackupNumber + TntWarsGame.GuiLoaded.lvl.name);
            TntWarsGame.GuiLoaded.RedScore = 0;
            TntWarsGame.GuiLoaded.BlueScore = 0;
            foreach ( TntWarsGame.player pl in TntWarsGame.GuiLoaded.Players ) {
                pl.Score = 0;
                pl.spec = false;
                pl.p.TntWarsKillStreak = 0;
                pl.p.TNTWarsLastKillStreakAnnounced = 0;
                pl.p.CurrentAmountOfTnt = 0;
                pl.p.CurrentTntGameNumber = TntWarsGame.GuiLoaded.GameNumber;
                pl.p.PlayingTntWars = false;
                pl.p.canBuild = true;
                pl.p.TntWarsHealth = 2;
                pl.p.TntWarsScoreMultiplier = 1f;
                pl.p.inTNTwarsMap = true;
                pl.p.HarmedBy = null;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsDltGame_Click(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            foreach ( TntWarsGame.player pl in TntWarsGame.GuiLoaded.Players ) {
                pl.p.CurrentTntGameNumber = -1;
                Player.Message(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                pl.p.PlayingTntWars = false;
                pl.p.canBuild = true;
                TntWarsGame.SetTitlesAndColor(pl, true);
            }
            TntWarsGame.GameList.Remove(TntWarsGame.GuiLoaded);
            TntWarsGame.GuiLoaded = null;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsDiffAboutBt_Click(object sender, EventArgs e) {
            string msg = "Difficulty:";
            msg += Environment.NewLine;
            msg += "Easy (2 Hits to die, TNT has long delay)";
            msg += Environment.NewLine;
            msg += "Normal (2 Hits to die, TNT has normal delay)";
            msg += Environment.NewLine;
            msg += "Hard (1 Hit to die, TNT has short delay and team kills are on)";
            msg += Environment.NewLine;
            msg += "Extreme (1 Hit to die, TNT has short delay, big explosion and team kills are on)";
            MessageBox.Show(msg, "Difficulty");
        }
    }
}
