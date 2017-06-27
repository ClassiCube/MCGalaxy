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
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MCGalaxy.Config;
using MCGalaxy.Games;
using MCGalaxy.Network;
using MCGalaxy.Tasks;

namespace MCGalaxy {
    public static class ServerConfig {
        
		/// <summary> Interval in milliseconds between position updates. </summary>
        [ConfigInt("position-interval", "Server", null, 100, 20, 2000)]
        public static int PositionInterval = 100;
        [ConfigBool("classicube-account-plus", "Server", null, true)]
        public static bool ClassicubeAccountPlus = true;
        
        //auto updater stuff
        [ConfigBool("auto-update", "Update", null, false)]
        public static bool autoupdate;
        [ConfigBool("in-game-update-notify", "Server", null, false)]
        public static bool notifyPlayers;
        [ConfigInt("update-countdown", "Update", null, 10)]
        public static int restartcountdown = 10;
        [ConfigBool("auto-restart", "Server", null, false)]
        public static bool autorestart;
        [ConfigDateTime("restarttime", "Server", null)]
        public static DateTime restarttime;

        [ConfigBool("log-notes", "Other", null, true)]
        public static bool LogNotes = true;
        [ConfigBool("allow-tp-to-higher-ranks", "Other", null, true)]
        public static bool higherranktp = true;
        [ConfigBool("agree-to-rules-on-entry", "Other", null, false)]
        public static bool agreetorulesonentry = false;
        
        [ConfigPerm("os-perbuild-default", "Other", null, LevelPermission.Nobody)]
        public static LevelPermission osPerbuildDefault = LevelPermission.Nobody;
        [ConfigBool("software-staff-prefixes", "Other", null, true)]
        public static bool SoftwareStaffPrefixes = true;
        
        [ConfigBool("tablist-rank-sorted", "Tablist", null, true)]
        public static bool TablistRankSorted = true;
        [ConfigBool("tablist-global", "Tablist", null, false)]
        public static bool TablistGlobal = true;
        [ConfigBool("tablist-bots", "Tablist", null, false)]
        public static bool TablistBots = false;

        const string asciiChars = " !\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";
        [ConfigString("server-name", "General", null, "[MCGalaxy] Default", false, asciiChars, 64)]
        public static string name = "[MCGalaxy] Default";
        [ConfigString("motd", "General", null, "Welcome", false, asciiChars, 128)]
        public static string motd = "Welcome!";
        
        [ConfigInt("max-players", "Server", null, 12, 1, 128)]
        public static int players = 12;
        [ConfigInt("max-guests", "Server", null, 10, 1, 128)]
        public static int maxGuests = 10;

        [ConfigString("listen-ip", "Server", null, "0.0.0.0")]
        public static string listenIP = "0.0.0.0";
        [ConfigInt("port", "Server", null, 25565, 0, 65535)]
        public static int port = 25565;
        [ConfigBool("public", "Server", null, true)]
        public static bool pub = true;
        [ConfigBool("verify-names", "Server", null, true)]
        public static bool verify = true;
        /// <summary> true if maps sees server-wide chat, false if maps have level-only/isolated chat </summary>
        [ConfigBool("world-chat", "Server", null, true)]
        public static bool worldChat = true;

        
        //Spam Prevention
        [ConfigBool("mute-on-spam", "Spam control", null, false)]
        public static bool checkspam = false;
        [ConfigInt("spam-messages", "Spam control", null, 8, 0, 1000)]
        public static int spamcounter = 8;
        [ConfigInt("spam-mute-time", "Spam control", null, 60, 0, 1000)]
        public static int mutespamtime = 60;
        [ConfigInt("spam-counter-reset-time", "Spam control", null, 5, 0, 1000)]
        public static int spamcountreset = 5;
        
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
        
        
        [ConfigString("host-state", "Other", null, "Alive")]
        public static string ZallState = "Alive";
        [ConfigString("main-name", "General", null, "main", false, "._+")]
        public static string level = "main";
        [ConfigString("xjail-map-name", "Other", null, "(main)", false, "()._+")]
        public static string xjailLevel = "(main)";

        [ConfigBool("report-back", "Error handling", null, true)]
        public static bool reportBack = true;
        [ConfigBool("core-secret-commands", "Other", null, true)]
        public static bool CoreSecretCommands = true;

        [ConfigBool("irc", "IRC bot", null, false)]
        public static bool irc = false;
        [ConfigBool("irc-player-titles", "IRC bot", null, true)]
        public static bool ircPlayerTitles = true;
        [ConfigBool("irc-show-world-changes", "IRC bot", null, false)]
        public static bool ircShowWorldChanges = false;
        [ConfigBool("irc-show-afk", "IRC bot", null, false)]
        public static bool ircShowAFK = false;
        [ConfigInt("irc-port", "IRC bot", null, 6667, 0, 65535)]
        public static int ircPort = 6667;
        [ConfigString("irc-nick", "IRC bot", null, "ForgeBot")]
        public static string ircNick = "ForgeBot";
        [ConfigString("irc-server", "IRC bot", null, "irc.esper.net")]
        public static string ircServer = "irc.esper.net";
        [ConfigString("irc-channel", "IRC bot", null, "#changethis", true)]
        public static string ircChannel = "#changethis";
        [ConfigString("irc-opchannel", "IRC bot", null, "#changethistoo", true)]
        public static string ircOpChannel = "#changethistoo";
        [ConfigString("irc-command-prefix", "IRC bot", null, ".x", true)]
        public static string ircCommandPrefix = ".x";
        [ConfigBool("irc-identify", "IRC bot", null, false)]
        public static bool ircIdentify = false;
        [ConfigString("irc-password", "IRC bot", null, "", true)]
        public static string ircPassword = "";
        [ConfigEnum("irc-controller-verify", "IRC bot", null, IRCControllerVerify.HalfOp, typeof(IRCControllerVerify))]
        public static IRCControllerVerify IRCVerify = IRCControllerVerify.HalfOp;
        [ConfigPerm("irc-controller-rank", "IRC bot", null, LevelPermission.Nobody)]
        public static LevelPermission ircControllerRank = LevelPermission.Nobody;
        
        [ConfigBool("admin-verification", "Admin", null, true)]
        public static bool verifyadmins = true;
        [ConfigPerm("verify-admin-perm", "Admin", null, LevelPermission.Operator)]
        public static LevelPermission verifyadminsrank = LevelPermission.Operator;

        [ConfigBool("restart-on-error", "Error handling", null, true)]
        public static bool restartOnError = true;
        [ConfigInt("rplimit", "Other", null, 500, 0, 50000)]
        public static int rpLimit = 500;
        [ConfigInt("rplimit-norm", "Other", null, 10000, 0, 50000)]
        public static int rpNormLimit = 10000;

        [ConfigInt("backup-time", "Backup", null, 300, 1)]
        public static int backupInterval = 300;
        public static int blockInterval = 60;
        [ConfigString("backup-location", "Backup", null, "")]
        public static string backupLocation = Path.Combine(Utils.FolderPath, "levels/backups");
        [ConfigStringList("disabledstandardtokens", "Other", null)]
        internal static List<string> disabledChatTokens = new List<string>();

        [ConfigBool("physicsrestart", "Other", null, true)]
        public static bool physicsRestart = true;
        [ConfigBool("deathcount", "Other", null, true)]
        public static bool deathcount = true;
        [ConfigBool("autoload", "Server", null, true)]
        public static bool AutoLoad = true;
        [ConfigInt("physics-undo-max", "Other", null, 20000)]
        public static int physUndo = 20000;
        [ConfigBool("parse-emotes", "Other", null, true)]
        public static bool parseSmiley = true;
        [ConfigBool("use-whitelist", "Other", null, false)]
        public static bool useWhitelist = false;
        [ConfigBool("force-cuboid", "Other", null, false)]
        public static bool forceCuboid = false;
        [ConfigBool("profanity-filter", "Other", null, false)]
        public static bool profanityFilter = false;
        [ConfigBool("repeat-messages", "Other", null, false)]
        public static bool repeatMessage = false;

        [ConfigBool("check-updates", "Update", null, false)]
        public static bool checkUpdates = true;

        [ConfigBool("UseMySQL", "Database", null, false)]
        public static bool useMySQL = false;
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
        
        [ConfigBool("global-chat-enabled", "Other", null, true)]
        public static bool UseGlobalChat = true;
        [ConfigInt("afk-minutes", "Other", null, 10)]
        public static int afkminutes = 10;
        [ConfigInt("afk-kick", "Other", null, 45)]
        public static int afkkick = 45;
        [ConfigPerm("afk-kick-perm", "Other", null, LevelPermission.AdvBuilder)]
        public static LevelPermission afkkickperm = LevelPermission.AdvBuilder;
        [ConfigString("default-rank", "General", null, "guest")]
        public static string defaultRank = "guest";

        [ConfigBool("dollar-before-dollar", "Other", null, true)]
        public static bool dollarNames = true;
        public static bool unsafe_plugin = true;
        [ConfigBool("cheapmessage", "Other", null, true)]
        public static bool cheapMessage = true;
        
        [ConfigString("cheap-message-given", "Messages", null, " is now invincible")]
        public static string cheapMessageGiven = " is now invincible";
        [ConfigString("custom-ban-message", "Messages", null, "You're banned!")]
        public static string defaultBanMessage = "You're banned!";
        [ConfigString("custom-shutdown-message", "Messages", null, "Server shutdown. Rejoin in 10 seconds.")]
        public static string shutdownMessage = "Server shutdown. Rejoin in 10 seconds.";
        [ConfigString("custom-promote-message", "Messages", null, "&6Congratulations for working hard and getting &2PROMOTED!")]
        public static string defaultPromoteMessage = "&6Congratulations for working hard and getting &2PROMOTED!";
        [ConfigString("custom-demote-message", "Messages", null, "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(")]
        public static string defaultDemoteMessage = "&4DEMOTED! &6We're sorry for your loss. Good luck on your future endeavors! &1:'(";

        [ConfigString("money-name", "Other", null, "moneys")]
        public static string moneys = "moneys";
        
        [ConfigBool("log-heartbeat", "Other", null, false)]
        public static bool logbeat = false;
        [ConfigBool("admins-join-silent", "Other", null, false)]
        public static bool adminsjoinsilent = false;
        [ConfigString("server-owner", "Other", null, "Notch")]
        public static string server_owner = "Notch";
        
        [ConfigBool("guest-limit-notify", "Other", null, false)]
        public static bool guestLimitNotify = false;
        [ConfigBool("guest-join-notify", "Other", null, true)]
        public static bool guestJoinNotify = true;
        [ConfigBool("guest-leave-notify", "Other", null, true)]
        public static bool guestLeaveNotify = true;
        [ConfigString("default-texture-url", "General", null, "", true, null, NetUtils.StringSize)]
        public static string defaultTerrainUrl = "";
        [ConfigString("default-texture-pack-url", "General", null, "", true, null, NetUtils.StringSize)]
        public static string defaultTextureUrl = "";

        //hackrank stuff
        [ConfigBool("kick-on-hackrank", "Other", null, true)]
        public static bool hackrank_kick = true;
        [ConfigInt("hackrank-kick-time", "Other", null, 5)]
        public static int hackrank_kick_time = 5; //seconds, it converts it to milliseconds in the command.
        [ConfigBool("show-empty-ranks", "Other", null, false)]
        public static bool showEmptyRanks = false;

        [ConfigInt("review-cooldown", "Review", null, 600, 0, 600)]
        public static int reviewcooldown = 600;

        [ConfigInt("draw-reload-limit", "Other", null, 10000)]
        public static int DrawReloadLimit = 10000;
        [ConfigInt("map-gen-limit-admin", "Other", null, 225 * 1000 * 1000)]
        public static int MapGenLimitAdmin = 225 * 1000 * 1000;
        [ConfigInt("map-gen-limit", "Other", null, 30 * 1000 * 1000)]
        public static int MapGenLimit = 30 * 1000 * 1000;
    }
}