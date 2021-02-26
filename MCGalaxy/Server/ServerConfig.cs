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
    public sealed class ServerConfig : EnvConfig {

        [ConfigString("server-name", "Server", "[MCGalaxy] Default", false, " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~")]
        public string Name = "[MCGalaxy] Default";
        [ConfigString("motd", "Server", "Welcome", false)]
        public string MOTD = "Welcome!";
        [ConfigInt("max-players", "Server", 12, 1, 128)]
        public int MaxPlayers = 12;
        [ConfigInt("max-guests", "Server", 10, 1, 128)]
        public int MaxGuests = 10;
        [ConfigString("listen-ip", "Server", "0.0.0.0")]
        public string ListenIP = "0.0.0.0";
        [ConfigInt("port", "Server", 25565, 0, 65535)]
        public int Port = 25565;
        [ConfigBool("public", "Server", false)]
        public bool Public = false;
        [ConfigBool("verify-names", "Server", true)]
        public bool VerifyNames = true;
        [ConfigBool("support-web-client", "Server", true)]
        public bool WebClient = true;
        [ConfigString("default-rank", "Server", "guest")]
        public string DefaultRankName = "guest";
        
        [ConfigBool("autoload", "Level", true)]
        public bool AutoLoadMaps = true;        
        /// <summary> true if maps sees server-wide chat, false if maps have level-only/isolated chat </summary>
        [ConfigBool("world-chat", "Level", true)]
        public bool ServerWideChat = true;
        [ConfigString("main-name", "Level", "main", false, "()._+,-")]
        public string MainLevel = "main";
        [ConfigString("default-texture-url", "Level", "", true)]
        public string DefaultTerrain = "";
        [ConfigString("default-texture-pack-url", "Level", "", true)]
        public string DefaultTexture = "";
        
        [ConfigString("ssl-certificate-path", "Other", "", true)]
        public string SslCertPath = "";
        [ConfigString("ssl-certificate-password", "Other", "", true)]
        public string SslCertPass = "";
        [ConfigString("HeartbeatURL", "Other", "http://www.classicube.net/heartbeat.jsp", false, ":/.")]
        public string HeartbeatURL = "http://www.classicube.net/heartbeat.jsp";
        
        [ConfigBool("core-secret-commands", "Other", true)]
        public bool CoreSecretCommands = true;
        [ConfigBool("restart-on-error", "Error handling", true)]
        public bool restartOnError = true;
        [ConfigBool("software-staff-prefixes", "Other", true)]
        public bool SoftwareStaffPrefixes = true;
        
        [ConfigInt("position-interval", "Other", 100, 20, 2000)]
        public int PositionUpdateInterval = 100;
        [ConfigBool("classicube-account-plus", "Server", true)]
        public bool ClassicubeAccountPlus = true;
        [ConfigBool("agree-to-rules-on-entry", "Other", false)]
        public bool AgreeToRulesOnEntry = false;
        [ConfigBool("admins-join-silent", "Other", false)]
        public bool AdminsJoinSilently = false;
        
        [ConfigBool("check-updates", "Update", false)]
        public bool CheckForUpdates = true;
        [ConfigBool("enable-cpe", "Server", true)]
        public bool EnableCPE = true;
        [ConfigBool("checkpoints-respawn-clientside", "Other", true)]
        public bool CheckpointsRespawnClientside = true;
        
        [ConfigInt("rplimit", "Other", 500, 0, 50000)]
        public int PhysicsRestartLimit = 500;
        [ConfigInt("rplimit-norm", "Other", 10000, 0, 50000)]
        public int PhysicsRestartNormLimit = 10000;
        [ConfigBool("physicsrestart", "Other", true)]
        public bool PhysicsRestart = true;
        [ConfigInt("physics-undo-max", "Other", 50000)]
        public int PhysicsUndo = 50000;
        
        [ConfigTimespan("backup-time", "Backup", 300, false)]
        public TimeSpan BackupInterval = TimeSpan.FromSeconds(300);
        [ConfigTimespan("blockdb-backup-time", "Backup", 60, false)]
        public TimeSpan BlockDBSaveInterval = TimeSpan.FromSeconds(60);
        [ConfigString("backup-location", "Backup", "")]
        public string BackupDirectory = "levels/backups";
        
        [ConfigTimespan("afk-minutes", "Other", 10, true)]
        public TimeSpan AutoAfkTime = TimeSpan.FromMinutes(10);

        [ConfigInt("max-bots-per-level", "Other", 192, 0, 256)]
        public int MaxBotsPerLevel = 192;
        [ConfigBool("deathcount", "Other", true)]
        public bool AnnounceDeathCount = true;
        [ConfigBool("use-whitelist", "Other", false)]
        public bool WhitelistedOnly = false;
        [ConfigBool("repeat-messages", "Other", false)]
        public bool RepeatMBs = false;
        [ConfigTimespan("announcement-interval", "Other", 5, true)]
        public TimeSpan AnnouncementInterval = TimeSpan.FromMinutes(5);
        [ConfigString("money-name", "Other", "moneys")]
        public string Currency = "moneys";        
        [ConfigString("server-owner", "Other", "the owner")]
        public string OwnerName = "the owner";
        
        [ConfigBool("guest-limit-notify", "Other", false)]
        public bool GuestLimitNotify = false;
        [ConfigBool("guest-join-notify", "Other", true)]
        public bool GuestJoinsNotify = true;
        [ConfigBool("guest-leave-notify", "Other", true)]
        public bool GuestLeavesNotify = true;
        [ConfigBool("show-world-changes", "Other", true)]
        public bool ShowWorldChanges = true;

        [ConfigBool("kick-on-hackrank", "Other", true)]
        public bool HackrankKicks = true;
        [ConfigTimespan("hackrank-kick-time", "Other", 5, false)]
        public TimeSpan HackrankKickDelay = TimeSpan.FromSeconds(5);
        [ConfigBool("show-empty-ranks", "Other", false)]
        public bool ListEmptyRanks = false;
        [ConfigTimespan("review-cooldown", "Review", 600, false)]
        public TimeSpan ReviewCooldown = TimeSpan.FromSeconds(600);
        [ConfigFloat("draw-reload-threshold", "Other", 0.001f, 0, 1)]
        public float DrawReloadThreshold = 0.001f;
        [ConfigBool("allow-tp-to-higher-ranks", "Other", true)]
        public bool HigherRankTP = true;        
        [ConfigPerm("os-perbuild-default", "Other", LevelPermission.Nobody)]
        public LevelPermission OSPerbuildDefault = LevelPermission.Nobody;        
        
        [ConfigBool("irc", "IRC bot", false)]
        public bool UseIRC = false;
        [ConfigInt("irc-port", "IRC bot", 6667, 0, 65535)]
        public int IRCPort = 6667;
        [ConfigString("irc-server", "IRC bot", "irc.esper.net")]
        public string IRCServer = "irc.esper.net";
        [ConfigString("irc-nick", "IRC bot", "ForgeBot")]
        public string IRCNick = "ForgeBot";
        [ConfigString("irc-channel", "IRC bot", "#changethis", true)]
        public string IRCChannels = "#changethis";
        [ConfigString("irc-opchannel", "IRC bot", "#changethistoo", true)]
        public string IRCOpChannels = "#changethistoo";
        [ConfigBool("irc-identify", "IRC bot", false)]
        public bool IRCIdentify = false;
        [ConfigString("irc-nickserv-name", "IRC bot", "NickServ", true)]
        public string IRCNickServName = "NickServ";
        [ConfigString("irc-password", "IRC bot", "", true)]
        public string IRCPassword = "";
        [ConfigBool("irc-ssl", "IRC bot", false)]
        public bool IRCSSL = false;

        [ConfigBool("UseMySQL", "Database", false)]
        public bool UseMySQL = false;
        [ConfigString("host", "Database", "127.0.0.1")]
        public string MySQLHost = "127.0.0.1";
        [ConfigString("SQLPort", "Database", "3306", false, "0123456789")]
        public string MySQLPort = "3306";
        [ConfigString("Username", "Database", "root", true)]
        public string MySQLUsername = "root";
        [ConfigString("Password", "Database", "password", true)]
        public string MySQLPassword = "password";
        [ConfigString("DatabaseName", "Database", "MCZallDB")]
        public string MySQLDatabaseName = "MCZallDB";
        [ConfigBool("Pooling", "Database", true)]
        public bool DatabasePooling = true;

        [ConfigBool("irc-player-titles", "IRC bot", true)]
        public bool IRCShowPlayerTitles = true;
        [ConfigBool("irc-show-world-changes", "IRC bot", false)]
        public bool IRCShowWorldChanges = false;
        [ConfigBool("irc-show-afk", "IRC bot", false)]
        public bool IRCShowAFK = false;
        [ConfigString("irc-command-prefix", "IRC bot", ".x", true)]
        public string IRCCommandPrefix = ".x";
        [ConfigEnum("irc-controller-verify", "IRC bot", IRCControllerVerify.HalfOp, typeof(IRCControllerVerify))]
        public IRCControllerVerify IRCVerify = IRCControllerVerify.HalfOp;
        [ConfigPerm("irc-controller-rank", "IRC bot", LevelPermission.Admin)]
        public LevelPermission IRCControllerRank = LevelPermission.Admin;
        
        [ConfigBool("tablist-rank-sorted", "Tablist", true)]
        public bool TablistRankSorted = true;
        [ConfigBool("tablist-global", "Tablist", false)]
        public bool TablistGlobal = true;
        [ConfigBool("tablist-bots", "Tablist", false)]
        public bool TablistBots = false;

        [ConfigBool("parse-emotes", "Chat", true)]
        public bool ParseEmotes = true;        
        [ConfigBool("dollar-before-dollar", "Chat", true)]
        public bool DollarNames = true;
        [ConfigStringList("disabledstandardtokens", "Chat")]
        internal List<string> DisabledChatTokens = new List<string>();
        [ConfigBool("profanity-filter", "Chat", false)]
        public bool ProfanityFiltering = false;
        [ConfigString("profanity-replacement", "Chat", "*")]
        public string ProfanityReplacement = "*";
        [ConfigString("host-state", "Chat", "Alive")]
        public string ConsoleName = "Alive";
        
        [ConfigColor("defaultColor", "Colors", "&e")]
        public string DefaultColor = "&e";
        [ConfigColor("irc-color", "Colors", "&5")]
        public string IRCColor = "&5";
        [ConfigColor("global-chat-color", "Colors", "&6")]
        public string GlobalChatColor = "&6";
        [ConfigColor("help-syntax-color", "Colors", "&a")]
        public string HelpSyntaxColor = "&a";
        [ConfigColor("help-desc-color", "Colors", "&e")]
        public string HelpDescriptionColor = "&e";
        [ConfigColor("warning-error-color", "Colors", "&c")]
        public string WarningErrorColor = "&c";
        
        [ConfigBool("cheapmessage", "Other", true)]
        public bool ShowInvincibleMessage = true;        
        [ConfigString("cheap-message-given", "Messages", " is now invincible")]
        public string InvincibleMessage = " is now invincible";
        [ConfigString("custom-ban-message", "Messages", "You're banned!")]
        public string DefaultBanMessage = "You're banned!";
        [ConfigString("custom-shutdown-message", "Messages", "Server shutdown. Rejoin in 10 seconds.")]
        public string DefaultShutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
        [ConfigString("custom-promote-message", "Messages", "&6Congratulations for working hard and getting &2PROMOTED!")]
        public string DefaultPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
        [ConfigString("custom-demote-message", "Messages", "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(")]
        public string DefaultDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";       
        [ConfigString("custom-restart-message", "Messages", "Server restarted. Sign in again and rejoin.")]
        public string DefaultRestartMessage = "Server restarted. Sign in again and rejoin.";
        [ConfigString("custom-whitelist-message", "Messages", "This is a private server!")]
        public string DefaultWhitelistMessage = "This is a private server!";
        
        static readonly bool[] defLogLevels = new bool[] { 
            true,true,true,true,true,true, true,true,true, 
            true,true,true,true,true,true, true,true };
        [ConfigBool("log-notes", "Logging", true)]
        public bool LogNotes = true;
        [ConfigBoolArray("file-logging", "Logging", true, 17)]
        public bool[] FileLogging = defLogLevels;
        [ConfigBoolArray("console-logging", "Logging", true, 17)]
        public bool[] ConsoleLogging = defLogLevels;
        
        [ConfigBool("admin-verification", "Admin", true)]
        public bool verifyadmins = true;
        [ConfigPerm("verify-admin-perm", "Admin", LevelPermission.Operator)]
        public LevelPermission VerifyAdminsRank = LevelPermission.Operator;
        
        [ConfigBool("mute-on-spam", "Spam control", false)]
        public bool ChatSpamCheck = false;
        [ConfigInt("spam-messages", "Spam control", 8, 0, 10000)]
        public int ChatSpamCount = 8;
        [ConfigTimespan("spam-mute-time", "Spam control", 60, false)]
        public TimeSpan ChatSpamMuteTime = TimeSpan.FromSeconds(60);
        [ConfigTimespan("spam-counter-reset-time", "Spam control", 5, false)]
        public TimeSpan ChatSpamInterval = TimeSpan.FromSeconds(5);
        
        [ConfigBool("cmd-spam-check", "Spam control", true)]
        public bool CmdSpamCheck = true;
        [ConfigInt("cmd-spam-count", "Spam control", 25, 0, 10000)]
        public int CmdSpamCount = 25;
        [ConfigTimespan("cmd-spam-block-time", "Spam control", 30, false)]
        public TimeSpan CmdSpamBlockTime = TimeSpan.FromSeconds(30);
        [ConfigTimespan("cmd-spam-interval", "Spam control", 1, false)]
        public TimeSpan CmdSpamInterval = TimeSpan.FromSeconds(1);
        
        [ConfigBool("block-spam-check", "Spam control", true)]
        public bool BlockSpamCheck = true;
        [ConfigInt("block-spam-count", "Spam control", 200, 0, 10000)]
        public int BlockSpamCount = 200;
        [ConfigTimespan("block-spam-interval", "Spam control", 5, false)]
        public TimeSpan BlockSpamInterval = TimeSpan.FromSeconds(5);
        
        [ConfigBool("ip-spam-check", "Spam control", true)]
        public bool IPSpamCheck = true;
        [ConfigInt("ip-spam-count", "Spam control", 25, 0, 10000)]
        public int IPSpamCount = 10;
        [ConfigTimespan("ip-spam-block-time", "Spam control", 180, false)]
        public TimeSpan IPSpamBlockTime = TimeSpan.FromSeconds(180);
        [ConfigTimespan("ip-spam-interval", "Spam control", 60, false)]
        public TimeSpan IPSpamInterval = TimeSpan.FromSeconds(60);
    }
}