/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using MCGalaxy.Config;

namespace MCGalaxy {
    public static class ServerConfig {

        const string asciiChars = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        [ConfigString("server-name", "General", null, "[MCGalaxy] Default", false, asciiChars, 64)]
        public static string Name = "[MCGalaxy] Default";
        [ConfigString("motd", "General", null, "Welcome", false, asciiChars, 128)]
        public static string MOTD = "Welcome!";        
        [ConfigInt("max-players", "Server", null, 12, 1, 128)]
        public static int MaxPlayers = 12;
        [ConfigInt("max-guests", "Server", null, 10, 1, 128)]
        public static int MaxGuests = 10;
        [ConfigString("listen-ip", "Server", null, "0.0.0.0")]
        public static string ListenIP = "0.0.0.0";
        [ConfigInt("port", "Server", null, 25565, 0, 65535)]
        public static int Port = 25565;
        [ConfigBool("public", "Server", null, true)]
        public static bool Public = true;
        [ConfigBool("verify-names", "Server", null, true)]
        public static bool VerifyNames = true;
        
        [ConfigBool("autoload", "Server", null, true)]
        public static bool AutoLoadMaps = true;        
        /// <summary> true if maps sees server-wide chat, false if maps have level-only/isolated chat </summary>
        [ConfigBool("world-chat", "Server", null, true)]
        public static bool ServerWideChat = true;
        [ConfigString("main-name", "General", null, "main", false, "._+")]
        public static string MainLevel = "main";
        [ConfigString("xjail-map-name", "Other", null, "(main)", false, "()._+")]
        public static string XJailLevel = "(main)";
        [ConfigString("default-texture-url", "General", null, "", true, null, NetUtils.StringSize)]
        public static string DefaultTerrain = "";
        [ConfigString("default-texture-pack-url", "General", null, "", true, null, NetUtils.StringSize)]
        public static string DefaultTexture = "";
        
        [ConfigBool("report-back", "Error handling", null, true)]
        public static bool reportBack = true;
        [ConfigBool("core-secret-commands", "Other", null, true)]
        public static bool CoreSecretCommands = true;
        [ConfigBool("restart-on-error", "Error handling", null, true)]
        public static bool restartOnError = true;
        [ConfigBool("software-staff-prefixes", "Other", null, true)]
        public static bool SoftwareStaffPrefixes = true;
        
        [ConfigInt("position-interval", "Server", null, 100, 20, 2000)]
        public static int PositionUpdateInterval = 100;
        [ConfigBool("classicube-account-plus", "Server", null, true)]
        public static bool ClassicubeAccountPlus = true;
        [ConfigBool("agree-to-rules-on-entry", "Other", null, false)]
        public static bool AgreeToRulesOnEntry = false;
        [ConfigBool("admins-join-silent", "Other", null, false)]
        public static bool AdminsJoinSilently = false;
        
        [ConfigBool("check-updates", "Update", null, false)]
        public static bool CheckForUpdates = true;
        [ConfigBool("auto-update", "Update", null, false)]
        public static bool AutoUpdate;
        [ConfigBool("in-game-update-notify", "Server", null, false)]
        public static bool NotifyUpdating;
        [ConfigInt("update-countdown", "Update", null, 10)]
        public static int UpdateRestartDelay = 10;
        [ConfigBool("auto-restart", "Server", null, false)]
        public static bool AutoRestart;
        [ConfigDateTime("restarttime", "Server", null)]
        public static DateTime RestartTime;        
        
        [ConfigInt("rplimit", "Other", null, 500, 0, 50000)]
        public static int PhysicsRestartLimit = 500;
        [ConfigInt("rplimit-norm", "Other", null, 10000, 0, 50000)]
        public static int PhysicsRestartNormLimit = 10000;
        [ConfigBool("physicsrestart", "Other", null, true)]
        public static bool PhysicsRestart = true;
        [ConfigInt("physics-undo-max", "Other", null, 20000)]
        public static int PhysicsUndo = 20000;
        
        [ConfigInt("backup-time", "Backup", null, 300, 1)]
        public static int BackupInterval = 300;
        public static int BlockDBSaveInterval = 60;
        [ConfigString("backup-location", "Backup", null, "")]
        public static string BackupDirectory = Path.Combine(Utils.FolderPath, "levels/backups");       
        
        [ConfigInt("afk-minutes", "Other", null, 10)]
        public static int AutoAfkMins = 10;
        [ConfigInt("afk-kick", "Other", null, 45)]
        public static int AfkKickMins = 45;
        [ConfigPerm("afk-kick-perm", "Other", null, LevelPermission.AdvBuilder)]
        public static LevelPermission AfkKickRank = LevelPermission.AdvBuilder;
        
        [ConfigString("default-rank", "General", null, "guest")]
        public static string DefaultRankName = "guest";
        [ConfigInt("map-gen-limit-admin", "Other", null, 225 * 1000 * 1000)]
        public static int MapGenLimitAdmin = 225 * 1000 * 1000;
        [ConfigInt("map-gen-limit", "Other", null, 30 * 1000 * 1000)]
        public static int MapGenLimit = 30 * 1000 * 1000;

        [ConfigBool("deathcount", "Other", null, true)]
        public static bool AnnounceDeathCount = true;
        [ConfigBool("use-whitelist", "Other", null, false)]
        public static bool WhitelistedOnly = false;
        [ConfigBool("force-cuboid", "Other", null, false)]
        public static bool forceCuboid = false;
        [ConfigBool("repeat-messages", "Other", null, false)]
        public static bool RepeatMBs = false;
        public static bool unsafe_plugin = true;
        [ConfigString("money-name", "Other", null, "moneys")]
        public static string Currency = "moneys";        
        [ConfigBool("log-heartbeat", "Other", null, false)]
        public static bool LogHeartbeat = false;
        [ConfigString("server-owner", "Other", null, "Notch")]
        public static string OwnerName = "Notch";
        
        [ConfigBool("guest-limit-notify", "Other", null, false)]
        public static bool GuestLimitNotify = false;
        [ConfigBool("guest-join-notify", "Other", null, true)]
        public static bool GuestJoinsNotify = true;
        [ConfigBool("guest-leave-notify", "Other", null, true)]
        public static bool GuestLeavesNotify = true;

        [ConfigBool("kick-on-hackrank", "Other", null, true)]
        public static bool HackrankKicks = true;
        [ConfigInt("hackrank-kick-time", "Other", null, 5)]
        public static int HackrankKickDelay = 5; // seconds
        [ConfigBool("show-empty-ranks", "Other", null, false)]
        public static bool ListEmptyRanks = false;
        [ConfigInt("review-cooldown", "Review", null, 600, 0, 600)]
        public static int ReviewCooldown = 600;
        [ConfigInt("draw-reload-limit", "Other", null, 10000)]
        public static int DrawReloadLimit = 10000;
        [ConfigBool("allow-tp-to-higher-ranks", "Other", null, true)]
        public static bool HigherRankTP = true;        
        [ConfigPerm("os-perbuild-default", "Other", null, LevelPermission.Nobody)]
        public static LevelPermission OSPerbuildDefault = LevelPermission.Nobody;        
        
        [ConfigBool("irc", "IRC bot", null, false)]
        public static bool UseIRC = false;
        [ConfigInt("irc-port", "IRC bot", null, 6667, 0, 65535)]
        public static int IRCPort = 6667;
        [ConfigString("irc-server", "IRC bot", null, "irc.esper.net")]
        public static string IRCServer = "irc.esper.net";
        [ConfigString("irc-nick", "IRC bot", null, "ForgeBot")]
        public static string IRCNick = "ForgeBot";
        [ConfigString("irc-channel", "IRC bot", null, "#changethis", true)]
        public static string IRCChannels = "#changethis";
        [ConfigString("irc-opchannel", "IRC bot", null, "#changethistoo", true)]
        public static string IRCOpChannels = "#changethistoo";
        [ConfigBool("irc-identify", "IRC bot", null, false)]
        public static bool IRCIdentify = false;
        [ConfigString("irc-password", "IRC bot", null, "", true)]
        public static string IRCPassword = "";

        [ConfigBool("UseMySQL", "Database", null, false)]
        public static bool UseMySQL = false;
        [ConfigString("host", "Database", null, "127.0.0.1")]
        public static string MySQLHost = "127.0.0.1";
        [ConfigString("SQLPort", "Database", null, "3306", false, "0123456789")]
        public static string MySQLPort = "3306";
        [ConfigString("Username", "Database", null, "root", true)]
        public static string MySQLUsername = "root";
        [ConfigString("Password", "Database", null, "password", true)]
        public static string MySQLPassword = "password";
        [ConfigString("DatabaseName", "Database", null, "MCZallDB")]
        public static string MySQLDatabaseName = "MCZallDB";
        [ConfigBool("Pooling", "Database", null, true)]
        public static bool DatabasePooling = true;

        [ConfigBool("irc-player-titles", "IRC bot", null, true)]
        public static bool IRCShowPlayerTitles = true;
        [ConfigBool("irc-show-world-changes", "IRC bot", null, false)]
        public static bool IRCShowWorldChanges = false;
        [ConfigBool("irc-show-afk", "IRC bot", null, false)]
        public static bool IRCShowAFK = false;
        [ConfigString("irc-command-prefix", "IRC bot", null, ".x", true)]
        public static string IRCCommandPrefix = ".x";
        [ConfigEnum("irc-controller-verify", "IRC bot", null, IRCControllerVerify.HalfOp, typeof(IRCControllerVerify))]
        public static IRCControllerVerify IRCVerify = IRCControllerVerify.HalfOp;
        [ConfigPerm("irc-controller-rank", "IRC bot", null, LevelPermission.Nobody)]
        public static LevelPermission IRCControllerRank = LevelPermission.Nobody;       
        
        [ConfigBool("tablist-rank-sorted", "Tablist", null, true)]
        public static bool TablistRankSorted = true;
        [ConfigBool("tablist-global", "Tablist", null, false)]
        public static bool TablistGlobal = true;
        [ConfigBool("tablist-bots", "Tablist", null, false)]
        public static bool TablistBots = false;

        [ConfigBool("parse-emotes", "Other", null, true)]
        public static bool ParseEmotes = true;        
        [ConfigBool("dollar-before-dollar", "Other", null, true)]
        public static bool DollarBeforeNamesToken = true;
        [ConfigStringList("disabledstandardtokens", "Other", null)]
        internal static List<string> DisabledChatTokens = new List<string>();
        [ConfigBool("profanity-filter", "Other", null, false)]
        public static bool ProfanityFiltering = false;
        [ConfigString("host-state", "Other", null, "Alive")]
        public static string ConsoleName = "Alive";
        
        [ConfigColor("defaultColor", "Colors", null, "&e")]
        public static string DefaultColor = "&e";
        [ConfigColor("irc-color", "Colors", null, "&5")]
        public static string IRCColour = "&5";
        [ConfigColor("global-chat-color", "Colors", null, "&6")]
        public static string GlobalChatColor = "&6";
        [ConfigColor("help-syntax-color", "Colors", null, "&a")]
        public static string HelpSyntaxColor = "&a";
        [ConfigColor("help-desc-color", "Colors", null, "&e")]
        public static string HelpDescriptionColor = "&e";
        
        [ConfigBool("cheapmessage", "Other", null, true)]
        public static bool ShowInvincibleMessage = true;        
        [ConfigString("cheap-message-given", "Messages", null, " is now invincible")]
        public static string InvincibleMessage = " is now invincible";
        [ConfigString("custom-ban-message", "Messages", null, "You're banned!")]
        public static string DefaultBanMessage = "You're banned!";
        [ConfigString("custom-shutdown-message", "Messages", null, "Server shutdown. Rejoin in 10 seconds.")]
        public static string DefaultShutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
        [ConfigString("custom-promote-message", "Messages", null, "&6Congratulations for working hard and getting &2PROMOTED!")]
        public static string DefaultPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
        [ConfigString("custom-demote-message", "Messages", null, "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(")]
        public static string DefaultDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";       
        
        [ConfigBool("log-notes", "Other", null, true)]
        public static bool LogNotes = true;
        [ConfigBool("admin-verification", "Admin", null, true)]
        public static bool verifyadmins = true;
        [ConfigPerm("verify-admin-perm", "Admin", null, LevelPermission.Operator)]
        public static LevelPermission VerifyAdminsRank = LevelPermission.Operator;
        
        [ConfigBool("mute-on-spam", "Spam control", null, false)]
        public static bool ChatSpamCheck = false;
        [ConfigInt("spam-messages", "Spam control", null, 8, 0, 1000)]
        public static int ChatSpamCount = 8;
        [ConfigInt("spam-mute-time", "Spam control", null, 60, 0, 1000)]
        public static int ChatSpamMuteTime = 60;
        [ConfigInt("spam-counter-reset-time", "Spam control", null, 5, 0, 1000)]
        public static int ChatSpamInterval = 5;
        
        [ConfigBool("cmd-spam-check", "Spam control", null, true)]
        public static bool CmdSpamCheck = true;
        [ConfigInt("cmd-spam-count", "Spam control", null, 25, 0, 1000)]
        public static int CmdSpamCount = 25;
        [ConfigInt("cmd-spam-block-time", "Spam control", null, 30, 0, 1000)]
        public static int CmdSpamBlockTime = 30;
        [ConfigInt("cmd-spam-interval", "Spam control", null, 1, 0, 1000)]
        public static int CmdSpamInterval = 1;
        
        [ConfigBool("block-spam-check", "Spam control", null, true)]
        public static bool BlockSpamCheck = true;
        [ConfigInt("block-spam-count", "Spam control", null, 200, 0, 1000)]
        public static int BlockSpamCount = 200;
        [ConfigInt("block-spam-interval", "Spam control", null, 5, 0, 1000)]
        public static int BlockSpamInterval = 5;
        
        [ConfigBool("ip-spam-check", "Spam control", null, true)]
        public static bool IPSpamCheck = true;
        [ConfigInt("ip-spam-count", "Spam control", null, 25, 0, 1000)]
        public static int IPSpamCount = 10;
        [ConfigInt("ip-spam-block-time", "Spam control", null, 30, 0, 1000)]
        public static int IPSpamBlockTime = 180;
        [ConfigInt("ip-spam-interval", "Spam control", null, 1, 0, 1000)]
        public static int IPSpamInterval = 60;
    }
}