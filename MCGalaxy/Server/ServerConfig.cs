﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Config;
using MCGalaxy.Generator;
using MCGalaxy.Modules.Relay.IRC;

namespace MCGalaxy 
{
    public sealed class ServerConfig : EnvConfig 
    {
        [ConfigString("server-name", "Server", "[MCGalaxy] Default", false, " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~")]
        public string Name = "[MCGalaxy] Default";
        [ConfigString("motd", "Server", "Welcome", false)]
        public string MOTD = "Welcome!";
        [ConfigInt("max-players", "Server", 16, 1, Server.MAX_PLAYERS)]
        public int MaxPlayers = 16;
        [ConfigInt("max-guests", "Server", 14, 1, Server.MAX_PLAYERS)]
        public int MaxGuests = 14;
        [ConfigInt("port", "Server", 25565, 0, 65535)]
        public int Port = 25565;
        [ConfigBool("public", "Server", false)]
        public bool Public = false;
        [ConfigBool("verify-names", "Server", true)]
        public bool VerifyNames = true;
        [ConfigString("default-rank", "Server", "guest")]
        public string DefaultRankName = "guest";
        [ConfigString("server-owner", "Server", "the owner")]
        public string OwnerName = "the owner";

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

        [ConfigBool("use-whitelist", "Security", false)]
        public bool WhitelistedOnly = false;        
        [ConfigBool("admin-verification", "Security", true)]
        public bool verifyadmins = true;
        [ConfigPerm("verify-admin-perm", "Security", LevelPermission.Operator)]
        public LevelPermission VerifyAdminsRank = LevelPermission.Operator;
        [ConfigPerm("reset-password-perm", "Security", LevelPermission.Operator)]
        public LevelPermission ResetPasswordRank = LevelPermission.Owner;
        
        [ConfigBool("support-web-client", "Webclient", true)]
        public bool WebClient = true;
        [ConfigBool("allow-ip-forwarding", "Webclient", true)]
        public bool AllowIPForwarding = true;

        [ConfigString("HeartbeatURL", "Other", "http://www.classicube.net/heartbeat.jsp", false, ":/.,")]
        public string HeartbeatURL = "http://www.classicube.net/heartbeat.jsp";        
        [ConfigBool("core-secret-commands", "Other", true)]
        public bool CoreSecretCommands = true;
        [ConfigBool("restart-on-error", "Error handling", true)]
        public bool restartOnError = true;
        [ConfigBool("software-staff-prefixes", "Other", true)]
        public bool SoftwareStaffPrefixes = true;
        
        [ConfigInt("position-interval", "Other", 100, 20, 2000)]
        public int PositionUpdateInterval = 100;
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
        [ConfigBool("repeat-messages", "Other", false)]
        public bool RepeatMBs = false;
        [ConfigTimespan("announcement-interval", "Other", 5, true)]
        public TimeSpan AnnouncementInterval = TimeSpan.FromMinutes(5);
        [ConfigString("money-name", "Other", "moneys")]
        public string Currency = "moneys";
        
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
        [ConfigPerm("os-perbuild-default", "Other", LevelPermission.Owner)]
        public LevelPermission OSPerbuildDefault = LevelPermission.Owner;
        [ConfigBool("os-rename-allowed", "Other", true)]
        public bool OSRenameAllowed = true;
        [ConfigBool("protect-staff-ips", "Other", true)]
        public bool ProtectStaffIPs = true;
        [ConfigBool("classicube-account-plus", "Other", false)]
        public bool ClassicubeAccountPlus = false;
        // technically a Server option, but it's a common mistake to think
        //  this option needs changing to server's IP (0.0.0.0 = listen on all network interfaces)
        [ConfigString("listen-ip", "Other", "0.0.0.0")]
        public string ListenIP = "0.0.0.0";
        [ConfigStringList("disabled-commands", "Other")]
        public List<string> DisabledCommands = new List<string>();
        [ConfigStringList("disabled-modules", "Other")]
        public List<string> DisabledModules = new List<string>();
        [ConfigTimespan("death-invulnerability-cooldown", "Other", 2, false)]
        public TimeSpan DeathCooldown = TimeSpan.FromSeconds(2);
        [ConfigBool("verify-lan-ips", "Other", false)]
        public bool VerifyLanIPs = false;

        [ConfigBool("irc", "IRC bot", false)]
        public bool UseIRC = false;
        [ConfigInt("irc-port", "IRC bot", 6697, 0, 65535)]
        public int IRCPort = 6697;
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
        [ConfigString("irc-ignored-nicks", "IRC bot", "", true)]
        public string IRCIgnored = "";
        
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
        public List<string> DisabledChatTokens = new List<string>();
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
        [ConfigColor("help-syntax-color", "Colors", "&a")]
        public string HelpSyntaxColor = "&a";
        [ConfigColor("help-desc-color", "Colors", "&e")]
        public string HelpDescriptionColor = "&e";
        [ConfigColor("warning-error-color", "Colors", "&c")]
        public string WarningErrorColor = "&c";

        [ConfigBool("cheapmessage", "Messages", true)]
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
        [ConfigString("default-login-message", "Messages", "connected")]
        public string DefaultLoginMessage = "connected";
        [ConfigString("default-logout-message", "Messages", "disconnected")]
        public string DefaultLogoutMessage = "disconnected";

        [ConfigString("default-mapgen-theme", "Mapgen", "flat")]
        public string DefaultMapGenTheme = "flat";
        [ConfigString("default-mapgen-biome", "Mapgen", MapGenBiome.FOREST)]
        public string DefaultMapGenBiome = MapGenBiome.FOREST;

        static readonly bool[] defLogLevels = new bool[] { 
            true,true,true,true,true,true, true,true,true, 
            true,true,true,true,true,true, true,true };
        [ConfigBool("log-notes", "Logging", true)]
        public bool LogNotes = true;
        [ConfigBoolArray("file-logging", "Logging", true, 17)]
        public bool[] FileLogging = defLogLevels;
        [ConfigBoolArray("console-logging", "Logging", true, 17)]
        public bool[] ConsoleLogging = defLogLevels;
        
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