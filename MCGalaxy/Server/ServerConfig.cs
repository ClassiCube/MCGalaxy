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
        [ConfigString("server-name", "General", "[MCGalaxy] Default", false, asciiChars, 64)]
        public static string Name = "[MCGalaxy] Default";
        [ConfigString("motd", "General", "Welcome", false, asciiChars, 128)]
        public static string MOTD = "Welcome!";        
        [ConfigInt("max-players", "Server", 12, 1, 128)]
        public static int MaxPlayers = 12;
        [ConfigInt("max-guests", "Server", 10, 1, 128)]
        public static int MaxGuests = 10;
        [ConfigString("listen-ip", "Server", "0.0.0.0")]
        public static string ListenIP = "0.0.0.0";
        [ConfigInt("port", "Server", 25565, 0, 65535)]
        public static int Port = 25565;
        [ConfigBool("public", "Server", true)]
        public static bool Public = true;
        [ConfigBool("verify-names", "Server", true)]
        public static bool VerifyNames = true;
        
        [ConfigBool("autoload", "Server", true)]
        public static bool AutoLoadMaps = true;        
        /// <summary> true if maps sees server-wide chat, false if maps have level-only/isolated chat </summary>
        [ConfigBool("world-chat", "Server", true)]
        public static bool ServerWideChat = true;
        [ConfigString("main-name", "General", "main", false, "._+")]
        public static string MainLevel = "main";
        [ConfigString("xjail-map-name", "Other", "(main)", false, "()._+")]
        public static string XJailLevel = "(main)";
        [ConfigString("default-texture-url", "General", "", true, null, NetUtils.StringSize)]
        public static string DefaultTerrain = "";
        [ConfigString("default-texture-pack-url", "General", "", true, null, NetUtils.StringSize)]
        public static string DefaultTexture = "";
        
        [ConfigBool("report-back", "Error handling", true)]
        public static bool reportBack = true;
        [ConfigBool("core-secret-commands", "Other", true)]
        public static bool CoreSecretCommands = true;
        [ConfigBool("restart-on-error", "Error handling", true)]
        public static bool restartOnError = true;
        [ConfigBool("software-staff-prefixes", "Other", true)]
        public static bool SoftwareStaffPrefixes = true;
        
        [ConfigInt("position-interval", "Server", 100, 20, 2000)]
        public static int PositionUpdateInterval = 100;
        [ConfigBool("classicube-account-plus", "Server", true)]
        public static bool ClassicubeAccountPlus = true;
        [ConfigBool("agree-to-rules-on-entry", "Other", false)]
        public static bool AgreeToRulesOnEntry = false;
        [ConfigBool("admins-join-silent", "Other", false)]
        public static bool AdminsJoinSilently = false;
        
        [ConfigBool("check-updates", "Update", false)]
        public static bool CheckForUpdates = true;
        [ConfigBool("in-game-update-notify", "Server", false)]
        public static bool NotifyUpdating;
        [ConfigBool("auto-restart", "Server", false)]
        public static bool AutoRestart;
        [ConfigDateTime("restarttime", "Server")]
        public static DateTime RestartTime;        
        
        [ConfigInt("rplimit", "Other", 500, 0, 50000)]
        public static int PhysicsRestartLimit = 500;
        [ConfigInt("rplimit-norm", "Other", 10000, 0, 50000)]
        public static int PhysicsRestartNormLimit = 10000;
        [ConfigBool("physicsrestart", "Other", true)]
        public static bool PhysicsRestart = true;
        [ConfigInt("physics-undo-max", "Other", 50000)]
        public static int PhysicsUndo = 50000;
        
        [ConfigInt("backup-time", "Backup", 300, 1)]
        public static int BackupInterval = 300;
        public static int BlockDBSaveInterval = 60;
        [ConfigString("backup-location", "Backup", "")]
        public static string BackupDirectory = Path.Combine(Utils.FolderPath, "levels/backups");
        
        [ConfigInt("afk-minutes", "Other", 10)]
        public static int AutoAfkMins = 10;
        [ConfigString("default-rank", "General", "guest")]
        public static string DefaultRankName = "guest";

        [ConfigInt("max-bots-per-level", "Other", 192, 0, 256)]
        public static int MaxBotsPerLevel = 192;
        [ConfigBool("deathcount", "Other", true)]
        public static bool AnnounceDeathCount = true;
        [ConfigBool("use-whitelist", "Other", false)]
        public static bool WhitelistedOnly = false;
        [ConfigBool("force-cuboid", "Other", false)]
        public static bool forceCuboid = false;
        [ConfigBool("repeat-messages", "Other", false)]
        public static bool RepeatMBs = false;
        public static bool unsafe_plugin = true;
        [ConfigString("money-name", "Other", "moneys")]
        public static string Currency = "moneys";        
        [ConfigString("server-owner", "Other", "Notch")]
        public static string OwnerName = "Notch";
        
        [ConfigBool("guest-limit-notify", "Other", false)]
        public static bool GuestLimitNotify = false;
        [ConfigBool("guest-join-notify", "Other", true)]
        public static bool GuestJoinsNotify = true;
        [ConfigBool("guest-leave-notify", "Other", true)]
        public static bool GuestLeavesNotify = true;

        [ConfigBool("kick-on-hackrank", "Other", true)]
        public static bool HackrankKicks = true;
        [ConfigInt("hackrank-kick-time", "Other", 5)]
        public static int HackrankKickDelay = 5; // seconds
        [ConfigBool("show-empty-ranks", "Other", false)]
        public static bool ListEmptyRanks = false;
        [ConfigInt("review-cooldown", "Review", 600, 0, 600)]
        public static int ReviewCooldown = 600;
        [ConfigReal("draw-reload-threshold", "Other", 0.001f, 0, 1)]
        public static float DrawReloadThreshold = 0.001f;
        [ConfigBool("allow-tp-to-higher-ranks", "Other", true)]
        public static bool HigherRankTP = true;        
        [ConfigPerm("os-perbuild-default", "Other", LevelPermission.Nobody)]
        public static LevelPermission OSPerbuildDefault = LevelPermission.Nobody;        
        
        [ConfigBool("irc", "IRC bot", false)]
        public static bool UseIRC = false;
        [ConfigInt("irc-port", "IRC bot", 6667, 0, 65535)]
        public static int IRCPort = 6667;
        [ConfigString("irc-server", "IRC bot", "irc.esper.net")]
        public static string IRCServer = "irc.esper.net";
        [ConfigString("irc-nick", "IRC bot", "ForgeBot")]
        public static string IRCNick = "ForgeBot";
        [ConfigString("irc-channel", "IRC bot", "#changethis", true)]
        public static string IRCChannels = "#changethis";
        [ConfigString("irc-opchannel", "IRC bot", "#changethistoo", true)]
        public static string IRCOpChannels = "#changethistoo";
        [ConfigBool("irc-identify", "IRC bot", false)]
        public static bool IRCIdentify = false;
        [ConfigString("irc-nickserv-name", "IRC bot", "NickServ", true)]
        public static string IRCNickServName = "NickServ";
        [ConfigString("irc-password", "IRC bot", "", true)]
        public static string IRCPassword = "";

        [ConfigBool("UseMySQL", "Database", false)]
        public static bool UseMySQL = false;
        [ConfigString("host", "Database", "127.0.0.1")]
        public static string MySQLHost = "127.0.0.1";
        [ConfigString("SQLPort", "Database", "3306", false, "0123456789")]
        public static string MySQLPort = "3306";
        [ConfigString("Username", "Database", "root", true)]
        public static string MySQLUsername = "root";
        [ConfigString("Password", "Database", "password", true)]
        public static string MySQLPassword = "password";
        [ConfigString("DatabaseName", "Database", "MCZallDB")]
        public static string MySQLDatabaseName = "MCZallDB";
        [ConfigBool("Pooling", "Database", true)]
        public static bool DatabasePooling = true;

        [ConfigBool("irc-player-titles", "IRC bot", true)]
        public static bool IRCShowPlayerTitles = true;
        [ConfigBool("irc-show-world-changes", "IRC bot", false)]
        public static bool IRCShowWorldChanges = false;
        [ConfigBool("irc-show-afk", "IRC bot", false)]
        public static bool IRCShowAFK = false;
        [ConfigString("irc-command-prefix", "IRC bot", ".x", true)]
        public static string IRCCommandPrefix = ".x";
        [ConfigEnum("irc-controller-verify", "IRC bot", IRCControllerVerify.HalfOp, typeof(IRCControllerVerify))]
        public static IRCControllerVerify IRCVerify = IRCControllerVerify.HalfOp;
        [ConfigPerm("irc-controller-rank", "IRC bot", LevelPermission.Nobody)]
        public static LevelPermission IRCControllerRank = LevelPermission.Nobody;       
        
        [ConfigBool("tablist-rank-sorted", "Tablist", true)]
        public static bool TablistRankSorted = true;
        [ConfigBool("tablist-global", "Tablist", false)]
        public static bool TablistGlobal = true;
        [ConfigBool("tablist-bots", "Tablist", false)]
        public static bool TablistBots = false;

        [ConfigBool("parse-emotes", "Other", true)]
        public static bool ParseEmotes = true;        
        [ConfigBool("dollar-before-dollar", "Other", true)]
        public static bool DollarNames = true;
        [ConfigStringList("disabledstandardtokens", "Other")]
        internal static List<string> DisabledChatTokens = new List<string>();
        [ConfigBool("profanity-filter", "Other", false)]
        public static bool ProfanityFiltering = false;
        [ConfigString("host-state", "Other", "Alive")]
        public static string ConsoleName = "Alive";
        
        [ConfigColor("defaultColor", "Colors", "&e")]
        public static string DefaultColor = "&e";
        [ConfigColor("irc-color", "Colors", "&5")]
        public static string IRCColor = "&5";
        [ConfigColor("global-chat-color", "Colors", "&6")]
        public static string GlobalChatColor = "&6";
        [ConfigColor("help-syntax-color", "Colors", "&a")]
        public static string HelpSyntaxColor = "&a";
        [ConfigColor("help-desc-color", "Colors", "&e")]
        public static string HelpDescriptionColor = "&e";
        
        [ConfigBool("cheapmessage", "Other", true)]
        public static bool ShowInvincibleMessage = true;        
        [ConfigString("cheap-message-given", "Messages", " is now invincible")]
        public static string InvincibleMessage = " is now invincible";
        [ConfigString("custom-ban-message", "Messages", "You're banned!")]
        public static string DefaultBanMessage = "You're banned!";
        [ConfigString("custom-shutdown-message", "Messages", "Server shutdown. Rejoin in 10 seconds.")]
        public static string DefaultShutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
        [ConfigString("custom-promote-message", "Messages", "&6Congratulations for working hard and getting &2PROMOTED!")]
        public static string DefaultPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
        [ConfigString("custom-demote-message", "Messages", "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(")]
        public static string DefaultDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";       
        
        [ConfigBool("log-notes", "Other", true)]
        public static bool LogNotes = true;
        [ConfigBool("admin-verification", "Admin", true)]
        public static bool verifyadmins = true;
        [ConfigPerm("verify-admin-perm", "Admin", LevelPermission.Operator)]
        public static LevelPermission VerifyAdminsRank = LevelPermission.Operator;
        
        [ConfigBool("mute-on-spam", "Spam control", false)]
        public static bool ChatSpamCheck = false;
        [ConfigInt("spam-messages", "Spam control", 8, 0, 10000)]
        public static int ChatSpamCount = 8;
        [ConfigInt("spam-mute-time", "Spam control", 60, 0, 1000000)]
        public static int ChatSpamMuteTime = 60;
        [ConfigInt("spam-counter-reset-time", "Spam control", 5, 0, 10000)]
        public static int ChatSpamInterval = 5;
        
        [ConfigBool("cmd-spam-check", "Spam control", true)]
        public static bool CmdSpamCheck = true;
        [ConfigInt("cmd-spam-count", "Spam control", 25, 0, 10000)]
        public static int CmdSpamCount = 25;
        [ConfigInt("cmd-spam-block-time", "Spam control", 30, 0, 1000000)]
        public static int CmdSpamBlockTime = 30;
        [ConfigInt("cmd-spam-interval", "Spam control", 1, 0, 10000)]
        public static int CmdSpamInterval = 1;
        
        [ConfigBool("block-spam-check", "Spam control", true)]
        public static bool BlockSpamCheck = true;
        [ConfigInt("block-spam-count", "Spam control", 200, 0, 10000)]
        public static int BlockSpamCount = 200;
        [ConfigInt("block-spam-interval", "Spam control", 5, 0, 10000)]
        public static int BlockSpamInterval = 5;
        
        [ConfigBool("ip-spam-check", "Spam control", true)]
        public static bool IPSpamCheck = true;
        [ConfigInt("ip-spam-count", "Spam control", 25, 0, 10000)]
        public static int IPSpamCount = 10;
        [ConfigInt("ip-spam-block-time", "Spam control", 30, 0, 1000000)]
        public static int IPSpamBlockTime = 180;
        [ConfigInt("ip-spam-interval", "Spam control", 1, 0, 10000)]
        public static int IPSpamInterval = 60;
    }
}