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
        TntWarsGame tw_selected;

        void LoadLavaSettings() {
        }

        void SaveLavaSettings() {
            Server.lava.SaveSettings();
            SaveLavaMapSettings();
        }

        void UpdateLavaControls() {
            try {
                ls_btnStartGame.Enabled = !Server.lava.Running;
                ls_btnStopGame.Enabled = Server.lava.Running;
                ls_btnEndRound.Enabled = Server.lava.RoundInProgress;
            }
            catch { }
        }

        void lsBtnStartGame_Click(object sender, EventArgs e) {
            if (!Server.lava.Running) Server.lava.Start(null, "", int.MaxValue);
            UpdateLavaControls();
        }

        void lsBtnStopGame_Click(object sender, EventArgs e) {
            if (Server.lava.Running) Server.lava.End();
            UpdateLavaControls();
        }

        void lsBtnEndRound_Click(object sender, EventArgs e) {
            if (Server.lava.RoundInProgress) Server.lava.EndRound();
            UpdateLavaControls();
        }

        void UpdateLavaMapList(bool useList = true, bool noUseList = true) {
            if (!useList && !noUseList) return;
            try {
                if (this.InvokeRequired) {
                    this.Invoke(new MethodInvoker(delegate { try { UpdateLavaMapList(useList, noUseList); } catch { } }));
                    return;
                }

                int useIndex = ls_lstUsed.SelectedIndex, noUseIndex = ls_lstNotUsed.SelectedIndex;
                if (useList) ls_lstUsed.Items.Clear();
                if (noUseList) ls_lstNotUsed.Items.Clear();

                if (useList) {
                    ls_lstUsed.Items.AddRange(Server.lava.Maps.ToArray());
                    try { if (useIndex > -1) ls_lstUsed.SelectedIndex = useIndex; }
                    catch { }
                }
                if (noUseList) {
                    string[] allMaps = LevelInfo.AllMapNames();
                    foreach (string map in allMaps) {
                        try {
                            if (map.ToLower() != Server.mainLevel.name && !Server.lava.HasMap(map))
                                ls_lstNotUsed.Items.Add(map);
                        }
                        catch (NullReferenceException) { }
                    }
                    try { if (noUseIndex > -1) ls_lstNotUsed.SelectedIndex = noUseIndex; }
                    catch { }
                }
            }
            catch (ObjectDisposedException) { }  //Y U BE ANNOYING 
            catch (Exception ex) { Logger.LogError(ex); }
        }

        void lsAddMap_Click(object sender, EventArgs e) {
            try {
                Server.lava.End(); // Doing this so we don't break something...
                UpdateLavaControls();

                string name;
                try { name = ls_lstNotUsed.Items[ls_lstNotUsed.SelectedIndex].ToString(); }
                catch { return; }

                if (LevelInfo.FindExact(name) == null) {
                    Command.Find("Load").Use(null, name);
                }
                Level level = LevelInfo.FindExact(name);
                if (level == null) return;

                Server.lava.AddMap(name);

                LSGame.MapSettings settings = Server.lava.LoadMapSettings(level.name);
                settings.FloodPos = new Vec3U16((ushort)(level.Width / 2), (ushort)(level.Height - 1), (ushort)(level.Length / 2));
                settings.LayerPos = new Vec3U16(0, (ushort)(level.Height / 2), 0);
                ushort x = (ushort)(level.Width / 2), y = (ushort)(level.Height / 2), z = (ushort)(level.Length / 2);
                settings.safeZone = new Vec3U16[] { new Vec3U16((ushort)(x - 3), y, (ushort)(z - 3)), new Vec3U16((ushort)(x + 3), (ushort)(y + 4), (ushort)(z + 3)) };
                Server.lava.SaveMapSettings(settings);

                level.Config.MOTD = "Lava Survival: " + level.name.Capitalize();
                level.Config.PhysicsOverload = 1000000;
                level.Config.LoadOnGoto = false;
                Level.SaveSettings(level);
                level.Unload(true);

                UpdateLavaMapList();
            }
            catch (Exception ex) { Logger.LogError(ex); }
        }

        void lsRemoveMap_Click(object sender, EventArgs e) {
            try {
                Server.lava.End(); // Doing this so we don't break something...
                UpdateLavaControls();

                string name;
                try { name = ls_lstUsed.Items[ls_lstUsed.SelectedIndex].ToString(); }
                catch { return; }

                if (LevelInfo.FindExact(name) == null) {
                    Command.Find("Load").Use(null, name);
                }
                Level level = LevelInfo.FindExact(name);
                if (level == null) return;

                Server.lava.RemoveMap(name);
                level.Config.MOTD = "ignore";
                level.Config.PhysicsOverload = 1500;
                level.Config.AutoUnload = true;
                level.Config.LoadOnGoto = true;
                Level.SaveSettings(level);
                level.Unload(true);

                UpdateLavaMapList();
            }
            catch (Exception ex) { Logger.LogError(ex); }
        }

        void lsMapUse_SelectedIndexChanged(object sender, EventArgs e) {
            SaveLavaMapSettings();
            if (ls_lstUsed.SelectedIndex == -1) {
                ls_grpMapSettings.Text = "Map settings";
                pg_lavaMap.SelectedObject = null;
                return;
            }
            
            string name = ls_lstUsed.Items[ls_lstUsed.SelectedIndex].ToString();
            ls_grpMapSettings.Text = "Map settings (" + name + ")";
            
            try {
                LSGame.MapSettings m = Server.lava.LoadMapSettings(name);
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
                    TntWarsGame game = TntWarsGame.Find(lvl);
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
                if (tw_selected.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers) tw_txtStatus.Text = "Waiting For Players";
                if (tw_selected.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart) tw_txtStatus.Text = "Starting";
                if (tw_selected.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod) tw_txtStatus.Text = "Started";
                if (tw_selected.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress) tw_txtStatus.Text = "In Progress";
                if (tw_selected.GameStatus == TntWarsGame.TntWarsGameStatus.Finished) tw_txtStatus.Text = "Finished";
                tw_txtPlayers.Text = tw_selected.PlayingPlayers().ToString(CultureInfo.InvariantCulture);
                
                //Difficulty
                if (tw_selected.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
                    TntWrsDiffCombo.Enabled = true;
                    TntWrsDiffSlctBt.Enabled = true;
                } else {
                    TntWrsDiffCombo.Enabled = false;
                    TntWrsDiffSlctBt.Enabled = false;
                }
                
                TntWrsDiffCombo.SelectedIndex = TntWrsDiffCombo.FindString(tw_selected.Difficulty.ToString());
                //scores
                tw_numScoreLimit.Value = tw_selected.ScoreLimit;
                tw_numScoreLimit.Enabled = true;
                tw_numScorePerKill.Value = tw_selected.Config.ScorePerKill;
                tw_numScorePerKill.Enabled = true;
                
                if (tw_selected.Config.AssistScore == 0) {
                    tw_cbScoreAssists.Checked = false;
                    tw_cbScoreAssists.Enabled = true;
                    tw_numScoreAssists.Enabled = false;
                } else {
                    tw_numScoreAssists.Value = tw_selected.Config.AssistScore;
                    tw_numScoreAssists.Enabled = true;
                    tw_cbScoreAssists.Checked = true;
                    tw_cbScoreAssists.Enabled = true;
                }
                
                if (tw_selected.Config.MultiKillBonus == 0) {
                    tw_cbMultiKills.Checked = false;
                    tw_cbMultiKills.Enabled = true;
                    tw_numMultiKills.Enabled = false;
                } else {
                    tw_numMultiKills.Value = tw_selected.Config.MultiKillBonus;
                    tw_numMultiKills.Enabled = true;
                    tw_cbMultiKills.Checked = true;
                    tw_cbMultiKills.Enabled = true;
                }
                
                //Grace period
                TntWrsGracePrdChck.Checked = tw_selected.Config.InitialGracePeriod;
                TntWrsGracePrdChck.Enabled = true;
                TntWrsGraceTimeChck.Value = tw_selected.Config.GracePeriodSeconds;
                TntWrsGraceTimeChck.Enabled = tw_selected.Config.InitialGracePeriod;
                //Teams
                TntWrsTmsChck.Checked = tw_selected.GameMode == TntWarsGame.TntWarsGameMode.TDM;
                TntWrsTmsChck.Enabled = true;
                tw_cbBalanceTeams.Checked = tw_selected.Config.BalanceTeams;
                tw_cbBalanceTeams.Enabled = true;
                tw_cbTeamKills.Checked = tw_selected.Config.TeamKills;
                tw_cbTeamKills.Enabled = true;
                //Status
                switch (tw_selected.GameStatus) {
                    case TntWarsGame.TntWarsGameStatus.WaitingForPlayers:
                        if (tw_selected.CheckAllSetUp(null, false, false)) tw_btnStartGame.Enabled = true;
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
                tw_cbStreaks.Checked = tw_selected.Config.Streaks;
                tw_cbStreaks.Enabled = true;
                //New game
                if (TntWrsMpsList.SelectedIndex < 0) TntWrsCrtNwTntWrsBt.Enabled = false;
                //Load lists
                TntWrsMpsList.Items.Clear();
                tw_lstGames.Items.Clear();
                TntWrsDiffCombo.Items.Clear();
                
                Level[] loaded = LevelInfo.Loaded.Items;
                foreach (Level lvl in loaded) {
                    TntWarsGame game = TntWarsGame.Find(lvl);
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
                if (tw_selected.GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers) {
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
        
        string DescribeTNTWars(Level lvl, TntWarsGame game) {
            string msg = lvl.name;

            msg += " - ";
            if (game.GameMode == TntWarsGame.TntWarsGameMode.FFA) msg += "FFA";
            if (game.GameMode == TntWarsGame.TntWarsGameMode.TDM) msg += "TDM";
            
            msg += " - ";            
            if (game.Difficulty == TntWarsGame.TntWarsDifficulty.Easy)    msg += "(Easy)";
            if (game.Difficulty == TntWarsGame.TntWarsDifficulty.Normal)  msg += "(Normal)";
            if (game.Difficulty == TntWarsGame.TntWarsDifficulty.Hard)    msg += "(Hard)";
            if (game.Difficulty == TntWarsGame.TntWarsDifficulty.Extreme) msg += "(Extreme)";
            
            msg += " - ";
            if (game.GameStatus == TntWarsGame.TntWarsGameStatus.WaitingForPlayers) msg += "(Waiting For Players)";
            if (game.GameStatus == TntWarsGame.TntWarsGameStatus.AboutToStart)      msg += "(Starting)";
            if (game.GameStatus == TntWarsGame.TntWarsGameStatus.GracePeriod)       msg += "(Started)";
            if (game.GameStatus == TntWarsGame.TntWarsGameStatus.InProgress)        msg += "(In Progress)";
            if (game.GameStatus == TntWarsGame.TntWarsGameStatus.Finished)          msg += "(Finished)";
            
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
                tw_selected = TntWarsGame.Find(LevelInfo.FindExact(split[0]));
                LoadTNTWarsTab(sender, e);
            }
            catch { }
        }

        void TntWrsMpsList_SelectedIndexChanged(object sender, EventArgs e) {
            TntWrsCrtNwTntWrsBt.Enabled = TntWrsMpsList.SelectedIndex >= 0;
        }

        void TntWrsCrtNwTntWrsBt_Click(object sender, EventArgs e) {
            TntWarsGame it = null;
            try {
                it = new TntWarsGame(LevelInfo.FindExact(TntWrsMpsList.Items[TntWrsMpsList.SelectedIndex].ToString()));
            }
            catch { }
            if (it == null) return;
            TntWarsGame.GameList.Add(it);
            tw_selected = it;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsDiffSlctBt_Click(object sender, EventArgs e) {
            if (tw_selected == null) return;
            switch (TntWrsDiffCombo.Items[TntWrsDiffCombo.SelectedIndex].ToString()) {
                case "Easy":
                    tw_selected.Difficulty = TntWarsGame.TntWarsDifficulty.Easy;
                    tw_selected.SendAllPlayersMessage("TNT Wars: Changed difficulty to easy!");
                    tw_selected.Config.TeamKills = false;
                    break;

                case "Normal":
                    tw_selected.Difficulty = TntWarsGame.TntWarsDifficulty.Normal;
                    tw_selected.SendAllPlayersMessage("TNT Wars: Changed difficulty to normal!");
                    tw_selected.Config.TeamKills = false;
                    break;

                case "Hard":
                    tw_selected.Difficulty = TntWarsGame.TntWarsDifficulty.Hard;
                    tw_selected.SendAllPlayersMessage("TNT Wars: Changed difficulty to hard!");
                    tw_selected.Config.TeamKills = true;
                    break;

                case "Extreme":
                    tw_selected.Difficulty = TntWarsGame.TntWarsDifficulty.Extreme;
                    tw_selected.SendAllPlayersMessage("TNT Wars: Changed difficulty to extreme!");
                    tw_selected.Config.TeamKills = true;
                    break;
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsScrLmtUpDwn_ValueChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.ScoreLimit = (int)tw_numScoreLimit.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsScrPrKlUpDwn_ValueChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.ScorePerKill = (int)tw_numScorePerKill.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsAsstChck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            if (tw_cbScoreAssists.Checked == false) {
                tw_selected.Config.AssistScore = 0;
                tw_numScoreAssists.Enabled = false;
            }
            else {
                tw_selected.Config.AssistScore = (int)tw_numScoreAssists.Value;
                tw_numScoreAssists.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsAstsScrUpDwn_ValueChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.AssistScore = (int)tw_numScoreAssists.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsMltiKlChck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            if (tw_cbMultiKills.Checked == false) {
                tw_selected.Config.MultiKillBonus = 0;
                tw_numMultiKills.Enabled = false;
            }
            else {
                tw_selected.Config.MultiKillBonus = (int)tw_numMultiKills.Value;
                tw_numMultiKills.Enabled = true;
            }
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsMltiKlScPrUpDown_ValueChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.MultiKillBonus = (int)tw_numMultiKills.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsGracePrdChck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.InitialGracePeriod = TntWrsGracePrdChck.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsGraceTimeChck_ValueChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.GracePeriodSeconds = (int)TntWrsGraceTimeChck.Value;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsTmsChck_CheckedChanged(object sender, EventArgs e) {
            switch (TntWrsTmsChck.Checked) {
                case true:
                    if (tw_selected.GameMode == TntWarsGame.TntWarsGameMode.FFA) {
                        tw_selected.GameMode = TntWarsGame.TntWarsGameMode.TDM;
                        foreach (TntWarsGame.player pl in tw_selected.Players) {
                            {
                                Player.Message(pl.p, "TNT Wars: Changed gamemode to Team Deathmatch");
                                pl.Red = false;
                                pl.Blue = false;
                                if (tw_selected.BlueTeam() > tw_selected.RedTeam()) {
                                    pl.Red = true;
                                }
                                else if (tw_selected.RedTeam() > tw_selected.BlueTeam()) {
                                    pl.Blue = true;
                                }
                                else if (tw_selected.RedScore > tw_selected.BlueScore) {
                                    pl.Blue = true;
                                }
                                else if (tw_selected.BlueScore > tw_selected.RedScore) {
                                    pl.Red = true;
                                }
                                else {
                                    pl.Red = true;
                                }
                            }
                            {
                                string mesg = pl.p.ColoredName + " %Sis now";
                                if (pl.Red) {
                                    mesg += " on the " + Colors.red + "red team";
                                }
                                if (pl.Blue) {
                                    mesg += " on the " + Colors.blue + "blue team";
                                }
                                if (pl.spec) {
                                    mesg += " (as a spectator)";
                                }
                                Chat.MessageGlobal(mesg);
                            }
                        }
                        if (tw_selected.ScoreLimit == TntWarsConfig.Default.ScoreMaxFFA) {
                            tw_selected.ScoreLimit = TntWarsConfig.Default.ScoreMaxTDM;
                        }
                    }
                    break;

                case false:
                    if (tw_selected.GameMode == TntWarsGame.TntWarsGameMode.TDM) {
                        tw_selected.GameMode = TntWarsGame.TntWarsGameMode.FFA;
                        tw_selected.SendAllPlayersMessage("TNT Wars: Changed gamemode to Free For All");
                        if (tw_selected.ScoreLimit == TntWarsConfig.Default.ScoreMaxTDM) {
                            tw_selected.ScoreLimit = TntWarsConfig.Default.ScoreMaxFFA;
                        }
                        foreach (TntWarsGame.player pl in tw_selected.Players) {
                            pl.p.color = pl.OldColor;
                            pl.p.SetPrefix();
                        }
                    }
                    break;
            }
        }

        void TntWrsBlnceTeamsChck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.BalanceTeams = tw_cbBalanceTeams.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsTKchck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.TeamKills = tw_cbTeamKills.Checked;
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsStreaksChck_CheckedChanged(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.Config.Streaks = tw_cbStreaks.Checked;
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
            foreach (TntWarsGame.player pl in tw_selected.Players) {
                pl.p.canBuild = true;
                pl.p.PlayingTntWars = false;
                pl.p.CurrentAmountOfTnt = 0;
            }
            tw_selected.GameStatus = TntWarsGame.TntWarsGameStatus.Finished;
            tw_selected.SendAllPlayersMessage("TNT wars: Game has been stopped!");
            LoadTNTWarsTab(sender, e);
        }

        void TntWrsRstGame_Click(object sender, EventArgs e) {
            if (tw_selected == null) return;
            tw_selected.GameStatus = TntWarsGame.TntWarsGameStatus.WaitingForPlayers;
            Command.Find("Restore").Use(null, tw_selected.BackupNumber + tw_selected.lvl.name);
            tw_selected.RedScore = 0;
            tw_selected.BlueScore = 0;
            foreach (TntWarsGame.player pl in tw_selected.Players) {
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
            foreach (TntWarsGame.player pl in tw_selected.Players) {
                pl.p.CurrentTntGameNumber = -1;
                Player.Message(pl.p, "TNT Wars: The TNT Wars game you are currently playing has been deleted!");
                pl.p.PlayingTntWars = false;
                pl.p.canBuild = true;
                TntWarsGame.SetTitlesAndColor(pl, true);
            }
            TntWarsGame.GameList.Remove(tw_selected);
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
