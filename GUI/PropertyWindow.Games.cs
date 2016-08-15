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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MCGalaxy.Games;

namespace MCGalaxy.Gui {
    public partial class PropertyWindow : Form {
        System.Timers.Timer lavaUpdateTimer;

        private void LoadLavaSettings() {
            lsCmbSetupRank.SelectedIndex = ( Group.findPerm(Server.lava.setupRank) == null ) ? 0 : lsCmbSetupRank.Items.IndexOf(Group.findPerm(Server.lava.setupRank).name);
            lsCmbControlRank.SelectedIndex = ( Group.findPerm(Server.lava.controlRank) == null ) ? 0 : lsCmbControlRank.Items.IndexOf(Group.findPerm(Server.lava.controlRank).name);
            lsChkStartOnStartup.Checked = Server.lava.startOnStartup;
            lsChkSendAFKMain.Checked = Server.lava.sendAfkMain;
            lsNudVoteCount.Value = Server.lava.voteCount;
            lsNudLives.Value = MathHelper.Clamp((decimal)Server.lava.lifeNum, 0, 1000);
            lsNudVoteTime.Value = (decimal)MathHelper.Clamp(Server.lava.voteTime, 1, 1000);
        }

        private void SaveLavaSettings() {
            Server.lava.setupRank = Group.GroupList.Find(grp => grp.name == lsCmbSetupRank.Items[lsCmbSetupRank.SelectedIndex].ToString()).Permission;
            Server.lava.controlRank = Group.GroupList.Find(grp => grp.name == lsCmbControlRank.Items[lsCmbControlRank.SelectedIndex].ToString()).Permission;
            Server.lava.startOnStartup = lsChkStartOnStartup.Checked;
            Server.lava.sendAfkMain = lsChkSendAFKMain.Checked;
            Server.lava.voteCount = (byte)lsNudVoteCount.Value;
            Server.lava.voteTime = (double)lsNudVoteTime.Value;
            Server.lava.lifeNum = (int)lsNudLives.Value;
            Server.lava.SaveSettings();
        }

        private void UpdateLavaControls() {
            try {
                lsBtnStartGame.Enabled = !Server.lava.active;
                lsBtnStopGame.Enabled = Server.lava.active;
                lsBtnEndRound.Enabled = Server.lava.roundActive;
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

                int useIndex = lsMapUse.SelectedIndex, noUseIndex = lsMapNoUse.SelectedIndex;
                if ( useList ) lsMapUse.Items.Clear();
                if ( noUseList ) lsMapNoUse.Items.Clear();

                if ( useList ) {
                    lsMapUse.Items.AddRange(Server.lava.Maps.ToArray());
                    try { if ( useIndex > -1 ) lsMapUse.SelectedIndex = useIndex; }
                    catch { }
                }
                if ( noUseList ) {
                    string[] files = Directory.GetFiles("levels", "*.lvl");
                    foreach (string file in files) {
                        try {
                    	    string name = Path.GetFileNameWithoutExtension(file);
                            if ( name.ToLower() != Server.mainLevel.name && !Server.lava.HasMap(name) )
                                lsMapNoUse.Items.Add(name);
                        }
                        catch ( NullReferenceException ) { }
                    }
                    try { if ( noUseIndex > -1 ) lsMapNoUse.SelectedIndex = noUseIndex; }
                    catch { }
                }
            }
            catch ( ObjectDisposedException ) { }  //Y U BE ANNOYING 
            catch ( Exception ex ) { Server.ErrorLog(ex); }
        }

        private void lsAddMap_Click(object sender, EventArgs e) {
            try {
                Server.lava.Stop(); // Doing this so we don't break something...
                UpdateLavaControls();

                string name;
                try { name = lsMapNoUse.Items[lsMapNoUse.SelectedIndex].ToString(); }
                catch { return; }

                if ( LevelInfo.Find(name) == null )
                    Command.all.Find("load").Use(null, name);
                Level level = LevelInfo.Find(name);
                if ( level == null ) return;

                Server.lava.AddMap(name);

                LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(level.name);
                settings.blockFlood = new Vec3U16((ushort)( level.Width / 2 ), (ushort)( level.Height - 1 ), (ushort)( level.Length / 2 ));
                settings.blockLayer = new Vec3U16(0, (ushort)( level.Height / 2 ), 0);
                ushort x = (ushort)( level.Width / 2 ), y = (ushort)( level.Height / 2 ), z = (ushort)( level.Length / 2 );
                settings.safeZone = new Vec3U16[] { new Vec3U16((ushort)( x - 3 ), y, (ushort)( z - 3 )), new Vec3U16((ushort)( x + 3 ), (ushort)( y + 4 ), (ushort)( z + 3 )) };
                Server.lava.SaveMapSettings(settings);

                level.motd = "Lava Survival: " + level.name.Capitalize();
                level.overload = 1000000;
                level.unload = false;
                level.loadOnGoto = false;
                Level.SaveSettings(level);
                level.Unload(true);

                UpdateLavaMapList();
            }
            catch ( Exception ex ) { Server.ErrorLog(ex); }
        }

        private void lsRemoveMap_Click(object sender, EventArgs e) {
            try {
                Server.lava.Stop(); // Doing this so we don't break something...
                UpdateLavaControls();

                string name;
                try { name = lsMapUse.Items[lsMapUse.SelectedIndex].ToString(); }
                catch { return; }

                if ( LevelInfo.Find(name) == null )
                    Command.all.Find("load").Use(null, name);
                Level level = LevelInfo.Find(name);
                if ( level == null ) return;

                Server.lava.RemoveMap(name);
                level.motd = "ignore";
                level.overload = 1500;
                level.unload = true;
                level.loadOnGoto = true;
                Level.SaveSettings(level);
                level.Unload(true);

                UpdateLavaMapList();
            }
            catch ( Exception ex ) { Server.ErrorLog(ex); }
        }

        private void lsMapUse_SelectedIndexChanged(object sender, EventArgs e) {
            string name;
            try { name = lsMapUse.Items[lsMapUse.SelectedIndex].ToString(); }
            catch { return; }

            lsLoadedMap = name;
            try {
                LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(name);
                lsNudFastLava.Value = MathHelper.Clamp((decimal)settings.fast, 0, 100);
                lsNudKiller.Value = MathHelper.Clamp((decimal)settings.killer, 0, 100);
                lsNudDestroy.Value = MathHelper.Clamp((decimal)settings.destroy, 0, 100);
                lsNudWater.Value = MathHelper.Clamp((decimal)settings.water, 0, 100);
                lsNudLayer.Value = MathHelper.Clamp((decimal)settings.layer, 0, 100);
                lsNudLayerHeight.Value = MathHelper.Clamp((decimal)settings.layerHeight, 1, 1000);
                lsNudLayerCount.Value = MathHelper.Clamp((decimal)settings.layerCount, 1, 1000);
                lsNudLayerTime.Value = (decimal)MathHelper.Clamp(settings.layerInterval, 1, 1000);
                lsNudRoundTime.Value = (decimal)MathHelper.Clamp(settings.roundTime, 1, 1000);
                lsNudFloodTime.Value = (decimal)MathHelper.Clamp(settings.floodTime, 1, 1000);
            }
            catch ( Exception ex ) { Server.ErrorLog(ex); }
        }

        private void lsBtnEndVote_Click(object sender, EventArgs e) {
            if ( Server.lava.voteActive ) Server.lava.EndVote();
            UpdateLavaControls();
        }

        private void lsBtnSaveSettings_Click(object sender, EventArgs e) {
            if ( String.IsNullOrEmpty(lsLoadedMap) ) return;

            try {
                LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(lsLoadedMap);
                settings.fast = (byte)lsNudFastLava.Value;
                settings.killer = (byte)lsNudKiller.Value;
                settings.destroy = (byte)lsNudDestroy.Value;
                settings.water = (byte)lsNudWater.Value;
                settings.layer = (byte)lsNudLayer.Value;
                settings.layerHeight = (int)lsNudLayerHeight.Value;
                settings.layerCount = (int)lsNudLayerCount.Value;
                settings.layerInterval = (double)lsNudLayerTime.Value;
                settings.roundTime = (double)lsNudRoundTime.Value;
                settings.floodTime = (double)lsNudFloodTime.Value;
                Server.lava.SaveMapSettings(settings);
            }
            catch ( Exception ex ) { Server.ErrorLog(ex); }
        }

        public void LoadTNTWarsTab(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) {
                //Clear all
                //Top
                SlctdTntWrsLvl.Text = "";
                SlctdTntWrdStatus.Text = "";
                SlctdTntWrsPlyrs.Text = "";
                //Difficulty
                TntWrsDiffCombo.Text = "";
                TntWrsDiffCombo.Enabled = false;
                TntWrsDiffSlctBt.Enabled = false;
                //scores
                TntWrsScrLmtUpDwn.Value = 150;
                TntWrsScrLmtUpDwn.Enabled = false;
                TntWrsScrPrKlUpDwn.Value = 10;
                TntWrsScrPrKlUpDwn.Enabled = false;
                TntWrsAsstChck.Checked = true;
                TntWrsAsstChck.Enabled = false;
                TntWrsAstsScrUpDwn.Value = 5;
                TntWrsAstsScrUpDwn.Enabled = false;
                TntWrsMltiKlChck.Checked = true;
                TntWrsMltiKlChck.Enabled = false;
                TntWrsMltiKlScPrUpDown.Value = 5;
                TntWrsMltiKlScPrUpDown.Enabled = false;
                //Grace period
                TntWrsGracePrdChck.Checked = true;
                TntWrsGracePrdChck.Enabled = false;
                TntWrsGraceTimeChck.Value = 30;
                TntWrsGraceTimeChck.Enabled = false;
                //Teams
                TntWrsTmsChck.Checked = true;
                TntWrsTmsChck.Enabled = false;
                TntWrsBlnceTeamsChck.Checked = true;
                TntWrsBlnceTeamsChck.Enabled = false;
                TntWrsTKchck.Checked = false;
                TntWrsTKchck.Enabled = false;
                //Status
                TntWrsStrtGame.Enabled = false;
                TntWrsEndGame.Enabled = false;
                TntWrsRstGame.Enabled = false;
                TntWrsDltGame.Enabled = false;
                //Other
                TntWrsStreaksChck.Checked = true;
                TntWrsStreaksChck.Enabled = false;
                //New game
                if ( TntWrsMpsList.SelectedIndex < 0 ) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                TntWarsGamesList.Items.Clear();
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
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Easy ) msg += "(Easy)";
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Normal ) msg += "(Normal)";
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Hard ) msg += "(Hard)";
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Extreme ) msg += "(Extreme)";
                        msg += " - ";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) msg += "(Waiting For Players)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ) msg += "(Starting)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ) msg += "(Started)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress ) msg += "(In Progress)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.Finished ) msg += "(Finished)";
                        TntWarsGamesList.Items.Add(msg);
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
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) SlctdTntWrdStatus.Text = "Waiting For Players";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ) SlctdTntWrdStatus.Text = "Starting";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ) SlctdTntWrdStatus.Text = "Started";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress ) SlctdTntWrdStatus.Text = "In Progress";
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.Finished ) SlctdTntWrdStatus.Text = "Finished";
                SlctdTntWrsPlyrs.Text = TntWarsGame.GuiLoaded.PlayingPlayers().ToString(CultureInfo.InvariantCulture);
                //Difficulty
                if ( TntWarsGame.GuiLoaded.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) {
                    TntWrsDiffCombo.Enabled = true;
                    TntWrsDiffSlctBt.Enabled = true;
                }
                else {
                    TntWrsDiffCombo.Enabled = false;
                    TntWrsDiffSlctBt.Enabled = false;
                }
                TntWrsDiffCombo.SelectedIndex = TntWrsDiffCombo.FindString(TntWarsGame.GuiLoaded.GameDifficulty.ToString());
                //scores
                TntWrsScrLmtUpDwn.Value = TntWarsGame.GuiLoaded.ScoreLimit;
                TntWrsScrLmtUpDwn.Enabled = true;
                TntWrsScrPrKlUpDwn.Value = TntWarsGame.GuiLoaded.ScorePerKill;
                TntWrsScrPrKlUpDwn.Enabled = true;
                if ( TntWarsGame.GuiLoaded.ScorePerAssist == 0 ) {
                    TntWrsAsstChck.Checked = false;
                    TntWrsAsstChck.Enabled = true;
                    TntWrsAstsScrUpDwn.Enabled = false;
                }
                else {
                    TntWrsAstsScrUpDwn.Value = TntWarsGame.GuiLoaded.ScorePerAssist;
                    TntWrsAstsScrUpDwn.Enabled = true;
                    TntWrsAsstChck.Checked = true;
                    TntWrsAsstChck.Enabled = true;
                }
                if ( TntWarsGame.GuiLoaded.MultiKillBonus == 0 ) {
                    TntWrsMltiKlChck.Checked = false;
                    TntWrsMltiKlChck.Enabled = true;
                    TntWrsMltiKlScPrUpDown.Enabled = false;
                }
                else {
                    TntWrsMltiKlScPrUpDown.Value = TntWarsGame.GuiLoaded.MultiKillBonus;
                    TntWrsMltiKlScPrUpDown.Enabled = true;
                    TntWrsMltiKlChck.Checked = true;
                    TntWrsMltiKlChck.Enabled = true;
                }
                //Grace period
                TntWrsGracePrdChck.Checked = TntWarsGame.GuiLoaded.GracePeriod;
                TntWrsGracePrdChck.Enabled = true;
                TntWrsGraceTimeChck.Value = TntWarsGame.GuiLoaded.GracePeriodSecs;
                TntWrsGraceTimeChck.Enabled = TntWarsGame.GuiLoaded.GracePeriod;
                //Teams
                TntWrsTmsChck.Checked = TntWarsGame.GuiLoaded.GameMode == TntWarsGame.TntWarsGameMode.TDM;
                TntWrsTmsChck.Enabled = true;
                TntWrsBlnceTeamsChck.Checked = TntWarsGame.GuiLoaded.BalanceTeams;
                TntWrsBlnceTeamsChck.Enabled = true;
                TntWrsTKchck.Checked = TntWarsGame.GuiLoaded.TeamKills;
                TntWrsTKchck.Enabled = true;
                //Status
                switch ( TntWarsGame.GuiLoaded.GameStatus ) {
                    case TntWarsGame.TntWarsGameStatus.WaitingForPlayers:
                        if ( TntWarsGame.GuiLoaded.CheckAllSetUp(null, false, false) ) TntWrsStrtGame.Enabled = true;
                        TntWrsEndGame.Enabled = false;
                        TntWrsRstGame.Enabled = false;
                        TntWrsDltGame.Enabled = true;
                        break;

                    case TntWarsGame.TntWarsGameStatus.AboutToStart:
                    case TntWarsGame.TntWarsGameStatus.GracePeriod:
                    case TntWarsGame.TntWarsGameStatus.InProgress:
                        TntWrsStrtGame.Enabled = false;
                        TntWrsEndGame.Enabled = true;
                        TntWrsRstGame.Enabled = false;
                        TntWrsDltGame.Enabled = false;
                        break;

                    case TntWarsGame.TntWarsGameStatus.Finished:
                        TntWrsStrtGame.Enabled = false;
                        TntWrsEndGame.Enabled = false;
                        TntWrsRstGame.Enabled = true;
                        TntWrsDltGame.Enabled = true;
                        break;

                }
                //Other
                TntWrsStreaksChck.Checked = TntWarsGame.GuiLoaded.Streaks;
                TntWrsStreaksChck.Enabled = true;
                //New game
                if ( TntWrsMpsList.SelectedIndex < 0 ) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                TntWarsGamesList.Items.Clear();
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
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Easy ) msg += "(Easy)";
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Normal ) msg += "(Normal)";
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Hard ) msg += "(Hard)";
                        if ( T.GameDifficulty == TntWarsGame.TntWarsDifficulty.Extreme ) msg += "(Extreme)";
                        msg += " - ";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers ) msg += "(Waiting For Players)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart ) msg += "(Starting)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod ) msg += "(Started)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress ) msg += "(In Progress)";
                        if ( T.GameStatus == TntWarsGame.TntWarsGameStatus.Finished ) msg += "(Finished)";
                        TntWarsGamesList.Items.Add(msg);
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
                    TntWrsScrLmtUpDwn.Enabled = false;
                    TntWrsScrPrKlUpDwn.Enabled = false;
                    TntWrsAsstChck.Enabled = false;
                    TntWrsAstsScrUpDwn.Enabled = false;
                    TntWrsMltiKlChck.Enabled = false;
                    TntWrsMltiKlScPrUpDown.Enabled = false;
                    //Grace period
                    TntWrsGracePrdChck.Enabled = false;
                    TntWrsGraceTimeChck.Enabled = false;
                    //Teams
                    TntWrsTmsChck.Enabled = false;
                    TntWrsBlnceTeamsChck.Enabled = false;
                    TntWrsTKchck.Enabled = false;
                    //Other
                    TntWrsStreaksChck.Enabled = false;
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
                string slctd = TntWarsGamesList.Items[TntWarsGamesList.SelectedIndex].ToString();
                if ( slctd.StartsWith("-->") ) {
                    LoadTNTWarsTab(sender, e);
                    return;
                }
                string[] split = slctd.Split(new string[] { " - " }, StringSplitOptions.None);
                TntWarsGame.GuiLoaded = TntWarsGame.Find(LevelInfo.Find(split[0]));
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
                it = new TntWarsGame(LevelInfo.Find(TntWrsMpsList.Items[TntWrsMpsList.SelectedIndex].ToString()));
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
                    TntWarsGame.GuiLoaded.GameDifficulty = TntWarsGame.TntWarsDifficulty.Easy;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to easy!");
                    if ( TntWarsGame.GuiLoaded.TeamKills ) {
                        TntWarsGame.GuiLoaded.TeamKills = false;
                    }
                    break;

                case "Normal":
                    TntWarsGame.GuiLoaded.GameDifficulty = TntWarsGame.TntWarsDifficulty.Normal;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to normal!");
                    if ( TntWarsGame.GuiLoaded.TeamKills ) {
                        TntWarsGame.GuiLoaded.TeamKills = false;
                    }
                    break;

                case "Hard":
                    TntWarsGame.GuiLoaded.GameDifficulty = TntWarsGame.TntWarsDifficulty.Hard;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to hard!");
                    if ( TntWarsGame.GuiLoaded.TeamKills == false ) {
                        TntWarsGame.GuiLoaded.TeamKills = true;
                    }
                    break;

                case "Extreme":
                    TntWarsGame.GuiLoaded.GameDifficulty = TntWarsGame.TntWarsDifficulty.Extreme;
                    TntWarsGame.GuiLoaded.SendAllPlayersMessage("TNT Wars: Changed difficulty to extreme!");
                    if ( TntWarsGame.GuiLoaded.TeamKills == false ) {
                        TntWarsGame.GuiLoaded.TeamKills = true;
                    }
                    break;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsScrLmtUpDwn_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.ScoreLimit = (int)TntWrsScrLmtUpDwn.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsScrPrKlUpDwn_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.ScorePerKill = (int)TntWrsScrPrKlUpDwn.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsAsstChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            if ( TntWrsAsstChck.Checked == false ) {
                TntWarsGame.GuiLoaded.ScorePerAssist = 0;
                TntWrsAstsScrUpDwn.Enabled = false;
            }
            else {
                TntWarsGame.GuiLoaded.ScorePerAssist = (int)TntWrsAstsScrUpDwn.Value;
                TntWrsAstsScrUpDwn.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsAstsScrUpDwn_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.ScorePerAssist = (int)TntWrsAstsScrUpDwn.Value;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsMltiKlChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            if ( TntWrsMltiKlChck.Checked == false ) {
                TntWarsGame.GuiLoaded.MultiKillBonus = 0;
                TntWrsMltiKlScPrUpDown.Enabled = false;
            }
            else {
                TntWarsGame.GuiLoaded.MultiKillBonus = (int)TntWrsMltiKlScPrUpDown.Value;
                TntWrsMltiKlScPrUpDown.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsMltiKlScPrUpDown_ValueChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.MultiKillBonus = (int)TntWrsMltiKlScPrUpDown.Value;
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
                                Player.SendMessage(pl.p, "TNT Wars: Changed gamemode to Team Deathmatch");
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
                                string mesg = pl.p.color + pl.p.name + Server.DefaultColor + " " + "is now";
                                if ( pl.Red ) {
                                    mesg += " on the " + Colors.red + "red team";
                                }
                                if ( pl.Blue ) {
                                    mesg += " on the " + Colors.blue + "blue team";
                                }
                                if ( pl.spec ) {
                                    mesg += Server.DefaultColor + " (as a spectator)";
                                }
                                Chat.MessageAll(mesg);
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
            TntWarsGame.GuiLoaded.BalanceTeams = TntWrsBlnceTeamsChck.Checked;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsTKchck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.TeamKills = TntWrsTKchck.Checked;
            LoadTNTWarsTab(sender, e);
        }

        private void TntWrsStreaksChck_CheckedChanged(object sender, EventArgs e) {
            if ( TntWarsGame.GuiLoaded == null ) return;
            TntWarsGame.GuiLoaded.Streaks = TntWrsStreaksChck.Checked;
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
                Player.SendMessage(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
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
