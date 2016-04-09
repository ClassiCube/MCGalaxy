/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace MCGalaxy {
	
	public static class SrvProperties {
		
		public static void Load(string givenPath, bool skipsalt = false) {
			RandomNumberGenerator prng = RandomNumberGenerator.Create();
			StringBuilder sb = new StringBuilder();
			byte[] oneChar = new byte[1];
			while (sb.Length < 16) {
				prng.GetBytes(oneChar);
				if (Char.IsLetterOrDigit((char)oneChar[0]))
					sb.Append((char)oneChar[0]);
			}
			Server.salt = sb.ToString();

			if (PropertiesFile.Read(givenPath, LineProcessor))
				Server.s.SettingsUpdate();
			Save(givenPath);
		}
		
		static void LineProcessor(string key, string value) {
			string color = "";
			switch (key.ToLower()) {
				case "server-name":
					if ( ValidString(value, "![]:.,{}~-+()?_/\\' ") ) {
						Server.name = value;
					}
					else { Server.s.Log("server-name invalid! setting to default."); }
					break;
				case "motd":
					if ( ValidString(value, "=![]&:.,{}~-+()?_/\\' ") ) // allow = in the motd
					{
						Server.motd = value;
					}
					else { Server.s.Log("motd invalid! setting to default."); }
					break;
				case "port":
					try { Server.port = Convert.ToInt32(value); }
					catch { Server.s.Log("port invalid! setting to default."); }
					break;
				case "verify-names":
					Server.verify = value.ToLower() == "true";
					break;
				case "public":
					Server.pub = value.ToLower() == "true";
					break;
				case "world-chat":
					Server.worldChat = value.ToLower() == "true";
					break;
					//case "guest-goto":
					//    Server.guestGoto = value.ToLower() == "true";
					//    break;
				case "max-players":
					try {
						if ( Convert.ToByte(value) > 128 ) {
							value = "128"; Server.s.Log("Max players has been lowered to 128.");
						}
						else if ( Convert.ToByte(value) < 1 ) {
							value = "1"; Server.s.Log("Max players has been increased to 1.");
						}
						Server.players = Convert.ToByte(value);
					}
					catch { Server.s.Log("max-players invalid! setting to default."); }
					break;
				case "max-guests":
					try {
						if ( Convert.ToByte(value) > Server.players ) {
							value = Server.players.ToString(); Server.s.Log("Max guests has been lowered to " + Server.players.ToString());
						}
						else if ( Convert.ToByte(value) < 0 ) {
							value = "0"; Server.s.Log("Max guests has been increased to 0.");
						}
						Server.maxGuests = Convert.ToByte(value);
					}
					catch { Server.s.Log("max-guests invalid! setting to default."); }
					break;
				case "irc":
					Server.irc = value.ToLower() == "true";
					break;
				case "irc-colorsenable":
					Server.ircColorsEnable = value.ToLower() == "true";
					break;
				case "irc-server":
					Server.ircServer = value;
					break;
				case "irc-nick":
					Server.ircNick = value;
					break;
				case "irc-channel":
					Server.ircChannel = value;
					break;
				case "irc-opchannel":
					Server.ircOpChannel = value;
					break;
				case "irc-port":
					try {
						Server.ircPort = Convert.ToInt32(value);
					}
					catch {
						Server.s.Log("irc-port invalid! setting to default.");
					}
					break;
				case "irc-identify":
					try {
						Server.ircIdentify = Convert.ToBoolean(value);
					}
					catch {
						Server.s.Log("irc-identify boolean value invalid! Setting to the default of: " + Server.ircIdentify + ".");
					}
					break;
				case "irc-password":
					Server.ircPassword = value;
					break;

				case "rplimit":
					try { Server.rpLimit = Convert.ToUInt16(value); }
					catch { Server.s.Log("rpLimit invalid! setting to default."); }
					break;
				case "rplimit-norm":
					try { Server.rpNormLimit = Convert.ToUInt16(value); }
					catch { Server.s.Log("rpLimit-norm invalid! setting to default."); }
					break;


				case "report-back":
					Server.reportBack = value.ToLower() == "true";
					break;
				case "backup-time":
					if ( Convert.ToInt32(value) > 1 ) { Server.backupInterval = Convert.ToInt32(value); }
					break;
				case "backup-location":
					if (!value.Contains("System.Windows.Forms.TextBox, Text:"))
						Server.backupLocation = value;
					if (!Directory.Exists(value))
						Server.backupLocation = Application.StartupPath + "/levels/backups";;
					break;
				case "physicsrestart":
					Server.physicsRestart = value.ToLower() == "true";
					break;
				case "deathcount":
					Server.deathcount = value.ToLower() == "true";
					break;
				case "usemysql":
					Server.useMySQL = value.ToLower() == "true";
					break;
				case "host":
					Server.MySQLHost = value;
					break;
				case "sqlport":
					Server.MySQLPort = value;
					break;
				case "username":
					Server.MySQLUsername = value;
					break;
				case "password":
					Server.MySQLPassword = value;
					break;
				case "databasename":
					Server.MySQLDatabaseName = value;
					break;
				case "pooling":
					try { Server.DatabasePooling = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "defaultcolor":
					ParseColor(value, ref Server.DefaultColor); break;
				case "irc-color":
					ParseColor(value, ref Server.IRCColour); break;
				case "global-chat-color":
					ParseColor(value, ref Server.GlobalChatColor); break;
				case "help-syntax-color":
					ParseColor(value, ref Server.HelpSyntaxColor); break;
				case "help-desc-color":
					ParseColor(value, ref Server.HelpDescriptionColor); break;
					
				case "opchat-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.opchatperm = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
					break;
				case "adminchat-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.adminchatperm = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
					break;
				case "log-heartbeat":
					try { Server.logbeat = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
					break;
				case "force-cuboid":
					try { Server.forceCuboid = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
					break;
				case "profanity-filter":
					try { Server.profanityFilter = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "cheapmessage":
					try { Server.cheapMessage = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "cheap-message-given":
					if ( value != "" ) Server.cheapMessageGiven = value;
					break;
				case "custom-ban-message":
					if ( value != "" ) Server.defaultBanMessage = value;
					break;
				case "custom-shutdown-message":
					if ( value != "" ) Server.shutdownMessage = value;
					break;
				case "custom-promote-message":
					if ( value != "" ) Server.defaultPromoteMessage = value;
					break;
				case "custom-demote-message":
					if ( value != "" ) Server.defaultDemoteMessage = value;
					break;
				case "default-rank":
					try { Server.defaultRank = value.ToLower(); }
					catch { }
					break;
				case "afk-minutes":
					try {
						Server.afkminutes = Convert.ToInt32(value);
					}
					catch {
						Server.s.Log("irc-port invalid! setting to default.");
					}
					break;
				case "afk-kick":
					try { Server.afkkick = Convert.ToInt32(value); }
					catch { Server.s.Log("irc-port invalid! setting to default."); }
					break;
				case "afk-kick-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.afkkickperm = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
					break;
				case "check-updates":
					try { Server.autonotify = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "auto-update":
					Server.autoupdate = value.ToLower() == "true";
					break;
				case "in-game-update-notify":
					Server.notifyPlayers = value.ToLower() == "true";
					break;
				case "update-countdown":
					try { Server.restartcountdown = Convert.ToInt32(value).ToString(); }
					catch { Server.restartcountdown = "10"; }
					break;
				case "autoload":
					try { Server.AutoLoad = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "auto-restart":
					try { Server.autorestart = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "restarttime":
					try { Server.restarttime = DateTime.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using defualt."); break; }
					break;
				case "parse-emotes":
					try { Server.parseSmiley = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); break; }
					break;
				case "use-whitelist":
					Server.useWhitelist = value.ToLower() == "true";
					break;
				case "allow-tp-to-higher-ranks":
					Server.higherranktp = value.ToLower() == "true";
					break;
				case "agree-to-rules-on-entry":
					try { Server.agreetorulesonentry = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "admins-join-silent":
					try { Server.adminsjoinsilent = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "main-name":
					if ( Player.ValidName(value) ) Server.level = value;
					else Server.s.Log("Invalid main name");
					break;
				case "default-texture-url":
					if (!value.StartsWith("http://") && !value.StartsWith("https://"))
						Server.defaultTerrainUrl = "";
					else
						Server.defaultTerrainUrl = value;
					break;
				case "default-texture-pack-url":
					if (!value.StartsWith("http://") && !value.StartsWith("https://"))
						Server.defaultTexturePackUrl = "";
					else
						Server.defaultTexturePackUrl = value;
					break;
				case "dollar-before-dollar":
					try { Server.dollarNames = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "money-name":
					if ( value != "" ) Server.moneys = value;
					break;
					/*case "mono":
								try { Server.mono = bool.Parse(value); }
								catch { Server.s.Log("Invalid " + key + ". Using default."); }
								break;*/
				case "restart-on-error":
					try { Server.restartOnError = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "repeat-messages":
					try { Server.repeatMessage = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "host-state":
					if ( value != "" )
						Server.ZallState = value;
					break;
				case "kick-on-hackrank":
					try { Server.hackrank_kick = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "hackrank-kick-time":
					try { Server.hackrank_kick_time = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "server-owner":
					if ( value != "" )
						Server.server_owner = value;
					break;
				case "zombie-on-server-start":
					try { Server.zombie.StartImmediately = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "no-respawning-during-zombie":
					try { Server.zombie.noRespawn = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "no-pillaring-during-zombie":
					try { Server.zombie.noPillaring = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "zombie-name-while-infected":
					if ( value != "" )
						Server.zombie.ZombieName = value;
					break;
				case "enable-changing-levels":
					try { Server.zombie.ChangeLevels = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "zombie-survival-only-server":
					try { Server.zombie.SetMainLevel = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "zombie-levels-list":
					if (value == "") Server.zombie.LevelList = new List<string>();
					else Server.zombie.LevelList = value.Replace(" ", "").Split(',').ToList<string>();
					break;
				case "zombie-ignores-list":
					if (value == "") Server.zombie.IgnoredLevelList = new List<string>();
					else Server.zombie.IgnoredLevelList = value.Replace(" ", "").Split(',').ToList<string>();
					break;
				case "zombie-save-blockchanges":
					try { Server.zombie.SaveLevelBlockchanges = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;					
				case "guest-limit-notify":
					try { Server.guestLimitNotify = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "guest-join-notify":
					try { Server.guestJoinNotify = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "guest-leave-notify":
					try { Server.guestLeaveNotify = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "admin-verification":
					try { Server.verifyadmins = bool.Parse(value); }
					catch { Server.s.Log("invalid " + key + ". Using default"); }
					break;
				case "verify-admin-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.verifyadminsrank = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ".  Using default."); break; }
					break;
				case "mute-on-spam":
					try { Server.checkspam = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "spam-messages":
					try { Server.spamcounter = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "spam-mute-time":
					try { Server.mutespamtime = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "spam-counter-reset-time":
					try { Server.spamcountreset = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "show-empty-ranks":
					try { Server.showEmptyRanks = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "global-chat-enabled":
					try { Server.UseGlobalChat = bool.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;

				case "total-undo":
					try { Server.totalUndo = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); }
					break;
				case "phys-max-undo":
					try { Server.physUndo = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); Server.physUndo = 20000; }
					break;
				case "draw-reload-limit":
					try { Server.DrawReloadLimit = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); Server.DrawReloadLimit = 10000; }
					break;
				case "map-gen-limit":
					try { Server.MapGenLimit = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); Server.MapGenLimit = 30 * 1000 * 1000; }
					break;
				case "map-gen-limit-admin":
					try { Server.MapGenLimitAdmin = int.Parse(value); }
					catch { Server.s.Log("Invalid " + key + ". Using default"); Server.MapGenLimitAdmin = 225 * 1000 * 1000; }
					break;
					
				case "review-view-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.reviewview = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "review-enter-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.reviewenter = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "review-leave-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.reviewleave = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "review-cooldown":
					try {
						Server.reviewcooldown = Convert.ToInt32(value.ToLower());
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "review-clear-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.reviewclear = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "review-next-perm":
					try {
						sbyte parsed = sbyte.Parse(value);
						if ( parsed < -50 || parsed > 120 ) {
							throw new FormatException();
						}
						Server.reviewnext = (LevelPermission)parsed;
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "bufferblocks":
					try {
						Server.bufferblocks = bool.Parse(value);
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
				case "disabledstandardtokens":
					{
						if (value == "") return;
						string[] tokens = value.Split(',');
						foreach (string token in tokens)
							Chat.standardTokens.Remove(token);
						Chat.disabledTokens = value;
					} break;
				case "enable-http-api":
					try {
						Server.EnableHttpApi = bool.Parse(value);
					}
					catch { Server.s.Log("Invalid " + key + ". Using default."); }
					break;
			}
		}
		
		static void ParseColor(string value, ref string target) {
			string color = Colors.Parse(value);
			if (color == "") {
				color = Colors.Name(value);
				if (color != "") color = value;
				else { Server.s.Log("Could not find " + value); return; }
			}
			target = color;
		}
		
		public static bool ValidString(string str, string allowed) {
			string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz01234567890" + allowed;
			foreach ( char ch in str ) {
				if ( allowedchars.IndexOf(ch) == -1 ) {
					return false;
				}
			} return true;
		}
		
		public static void Save() { Save("properties/server.properties"); }

		public static void Save(string givenPath) {
			try {
				File.Create(givenPath).Dispose();
				using ( StreamWriter w = File.CreateText(givenPath) ) {
					if ( givenPath.IndexOf("server") != -1 ) {
						SaveProps(w);
					}
				}
			}
			catch(Exception ex) {
				Server.ErrorLog(ex);
				Server.s.Log("SAVE FAILED! " + givenPath);
			}
		}
		
		public static void SaveProps(StreamWriter w) {
			w.WriteLine("#   Edit the settings below to modify how your server operates. This is an explanation of what each setting does.");
			w.WriteLine("#   server-name\t\t\t\t= The name which displays on classicube.net");
			w.WriteLine("#   motd\t\t\t\t= The message which displays when a player connects");
			w.WriteLine("#   port\t\t\t\t= The port to operate from");
			w.WriteLine("#   console-only\t\t\t= Run without a GUI (useful for Linux servers with mono)");
			w.WriteLine("#   verify-names\t\t\t= Verify the validity of names");
			w.WriteLine("#   public\t\t\t\t= Set to true to appear in the public server list");
			w.WriteLine("#   max-players\t\t\t\t= The maximum number of connections");
			w.WriteLine("#   max-guests\t\t\t\t= The maximum number of guests allowed");
			w.WriteLine("#   max-maps\t\t\t\t= The maximum number of maps loaded at once");
			w.WriteLine("#   world-chat\t\t\t\t= Set to true to enable world chat");
			w.WriteLine("#   guest-goto\t\t\t\t= Set to true to give guests goto and levels commands (Not implemented yet)");
			w.WriteLine("#   irc\t\t\t\t\t= Set to true to enable the IRC bot");
			w.WriteLine("#   irc-nick\t\t\t\t= The name of the IRC bot");
			w.WriteLine("#   irc-server\t\t\t\t= The server to connect to");
			w.WriteLine("#   irc-channel\t\t\t\t= The channel to join");
			w.WriteLine("#   irc-opchannel\t\t\t= The channel to join (posts OpChat)");
			w.WriteLine("#   irc-port\t\t\t\t= The port to use to connect");
			w.WriteLine("#   irc-identify\t\t\t= (true/false)\tDo you want the IRC bot to Identify itself with nickserv. Note: You will need to register it's name with nickserv manually.");
			w.WriteLine("#   irc-password\t\t\t= The password you want to use if you're identifying with nickserv");
			w.WriteLine("#   anti-tunnels\t\t\t= Stops people digging below max-depth");
			w.WriteLine("#   max-depth\t\t\t\t= The maximum allowed depth to dig down");
			w.WriteLine("#   backup-time\t\t\t\t= The number of seconds between automatic backups");
			w.WriteLine("#   overload\t\t\t\t= The higher this is, the longer the physics is allowed to lag.  Default 1500");
			w.WriteLine("#   use-whitelist\t\t\t= Switch to allow use of a whitelist to override IP bans for certain players.  Default false.");
			w.WriteLine("#   premium-only\t\t\t= Only allow premium players (paid for minecraft) to access the server. Default false.");
			w.WriteLine("#   force-cuboid\t\t\t= Run cuboid until the limit is hit, instead of canceling the whole operation.  Default false.");
			w.WriteLine("#   profanity-filter\t\t\t= Replace certain bad words in the chat.  Default false.");
			w.WriteLine("#   notify-on-join-leave\t\t= Show a balloon popup in tray notification area when a player joins/leaves the server.  Default false.");
			w.WriteLine("#   allow-tp-to-higher-ranks\t\t= Allows the teleportation to players of higher ranks");
			w.WriteLine("#   agree-to-rules-on-entry\t\t= Forces all new players to the server to agree to the rules before they can build or use commands.");
			w.WriteLine("#   adminchat-perm\t\t\t= The rank required to view adminchat. Default rank is superop.");
			w.WriteLine("#   admins-join-silent\t\t\t= Players who have adminchat permission join the game silently. Default true");
			w.WriteLine("#   server-owner\t\t\t= The minecraft name, of the owner of the server.");
			w.WriteLine("#   zombie-on-server-start\t\t= Starts Zombie Survival when server is started.");
			w.WriteLine("#   no-respawning-during-zombie\t\t= Disables respawning (Pressing R) while Zombie is on.");
			w.WriteLine("#   no-pillaring-during-zombie\t\t= Disables pillaring while Zombie Survival is activated.");
			w.WriteLine("#   zombie-name-while-infected\t\t= Sets the zombies name while actived if there is a value.");
			w.WriteLine("#   enable-changing-levels\t\t= After a Zombie Survival round has finished, will change the level it is running on.");
			w.WriteLine("#   zombie-survival-only-server\t\t= iEXPERIMENTAL! Makes the server only for Zombie Survival (etc. changes main level)");
			w.WriteLine("#   use-level-list\t\t\t= Only gets levels for changing levels in Zombie Survival from zombie-level-list.");
			w.WriteLine("#   zombie-level-list\t\t\t= List of levels for changing levels (Must be comma seperated, no spaces. Must have changing levels and use level list enabled.)");
			w.WriteLine("#   total-undo\t\t\t\t= Track changes made by the last X people logged on for undo purposes. Folder is rotated when full, so when set to 200, will actually track around 400.");
			w.WriteLine("#   guest-limit-notify\t\t\t= Show -Too Many Guests- message in chat when maxGuests has been reached. Default false");
			w.WriteLine("#   guest-join-notify\t\t\t= Shows when guests and lower ranks join server in chat and IRC. Default true");
			w.WriteLine("#   guest-leave-notify\t\t\t= Shows when guests and lower ranks leave server in chat and IRC. Default true");
			w.WriteLine();
			w.WriteLine("#   UseMySQL\t\t\t\t= Use MySQL (true) or use SQLite (false)");
			w.WriteLine("#   Host\t\t\t\t= The host name for the database (usually 127.0.0.1)");
			w.WriteLine("#   SQLPort\t\t\t\t= Port number to be used for MySQL.  Unless you manually changed the port, leave this alone.  Default 3306.");
			w.WriteLine("#   Username\t\t\t\t= The username you used to create the database (usually root)");
			w.WriteLine("#   Password\t\t\t\t= The password set while making the database");
			w.WriteLine("#   DatabaseName\t\t\t= The name of the database stored (Default = MCZall)");
			w.WriteLine();
			w.WriteLine("#   defaultColor\t\t\t= The color code of the default messages (Default = &e)");
			w.WriteLine();
			w.WriteLine("#   Super-limit\t\t\t\t= The limit for building commands for SuperOPs");
			w.WriteLine("#   Op-limit\t\t\t\t= The limit for building commands for Operators");
			w.WriteLine("#   Adv-limit\t\t\t\t= The limit for building commands for AdvBuilders");
			w.WriteLine("#   Builder-limit\t\t\t= The limit for building commands for Builders");
			w.WriteLine();
			w.WriteLine("#   kick-on-hackrank\t\t\t= Set to true if hackrank should kick players");
			w.WriteLine("#   hackrank-kick-time\t\t\t= Number of seconds until player is kicked");
			w.WriteLine("#   custom-rank-welcome-messages\t= Decides if different welcome messages for each rank is enabled. Default true.");
			w.WriteLine("#   ignore-ops\t\t\t\t= Decides whether or not an operator can be ignored. Default false.");
			w.WriteLine();
			w.WriteLine("#   admin-verification\t\t\t= Determines whether admins have to verify on entry to the server.  Default true.");
			w.WriteLine("#   verify-admin-perm\t\t\t= The minimum rank required for admin verification to occur.");
			w.WriteLine();
			w.WriteLine("#   mute-on-spam\t\t\t= If enabled it mutes a player for spamming.  Default false.");
			w.WriteLine("#   spam-messages\t\t\t= The amount of messages that have to be sent \"consecutively\" to be muted.");
			w.WriteLine("#   spam-mute-time\t\t\t= The amount of seconds a player is muted for spam.");
			w.WriteLine("#   spam-counter-reset-time\t\t= The amount of seconds the \"consecutive\" messages have to fall between to be considered spam.");
			w.WriteLine();
			w.WriteLine("#   As an example, if you wanted the spam to only mute if a user posts 5 messages in a row within 2 seconds, you would use the folowing:");
			w.WriteLine("#   mute-on-spam\t\t\t= true");
			w.WriteLine("#   spam-messages\t\t\t= 5");
			w.WriteLine("#   spam-mute-time\t\t\t= 60");
			w.WriteLine("#   spam-counter-reset-time\t\t= 2");
			w.WriteLine("#   bufferblocks\t\t\t= Should buffer blocks by default for maps?");
			w.WriteLine();
			w.WriteLine("# Server options");
			w.WriteLine("server-name = " + Server.name);
			w.WriteLine("motd = " + Server.motd);
			w.WriteLine("port = " + Server.port.ToString());
			w.WriteLine("verify-names = " + Server.verify.ToString().ToLower());
			w.WriteLine("public = " + Server.pub.ToString().ToLower());
			w.WriteLine("max-players = " + Server.players.ToString());
			w.WriteLine("max-guests = " + Server.maxGuests.ToString());
			w.WriteLine("world-chat = " + Server.worldChat.ToString().ToLower());
			w.WriteLine("check-updates = " + Server.checkUpdates.ToString().ToLower());
			w.WriteLine("auto-update = " + Server.autoupdate.ToString().ToLower());
			w.WriteLine("in-game-update-notify = " + Server.notifyPlayers.ToString().ToLower());
			w.WriteLine("update-countdown = " + Server.restartcountdown.ToString().ToLower());
			w.WriteLine("autoload = " + Server.AutoLoad.ToString().ToLower());
			w.WriteLine("auto-restart = " + Server.autorestart.ToString().ToLower());
			w.WriteLine("restarttime = " + Server.restarttime.ToShortTimeString());
			w.WriteLine("restart-on-error = " + Server.restartOnError);
			w.WriteLine("main-name = " + Server.level);
			w.WriteLine("default-texture-url = " + Server.defaultTerrainUrl);
			w.WriteLine("default-texture-pack-url = " + Server.defaultTexturePackUrl);
			w.WriteLine("enable-http-api = " + Server.EnableHttpApi);
			w.WriteLine();
			w.WriteLine("# irc bot options");
			w.WriteLine("irc = " + Server.irc.ToString().ToLower());
			w.WriteLine("irc-colorsenable = " + Server.ircColorsEnable.ToString().ToLower());
			w.WriteLine("irc-nick = " + Server.ircNick);
			w.WriteLine("irc-server = " + Server.ircServer);
			w.WriteLine("irc-channel = " + Server.ircChannel);
			w.WriteLine("irc-opchannel = " + Server.ircOpChannel);
			w.WriteLine("irc-port = " + Server.ircPort.ToString());
			w.WriteLine("irc-identify = " + Server.ircIdentify.ToString());
			w.WriteLine("irc-password = " + Server.ircPassword);
			w.WriteLine();
			w.WriteLine("# other options");
			w.WriteLine("rplimit = " + Server.rpLimit.ToString().ToLower());
			w.WriteLine("rplimit-norm = " + Server.rpNormLimit.ToString().ToLower());
			w.WriteLine("physicsrestart = " + Server.physicsRestart.ToString().ToLower());
			w.WriteLine("deathcount = " + Server.deathcount.ToString().ToLower());
			w.WriteLine("afk-minutes = " + Server.afkminutes.ToString());
			w.WriteLine("afk-kick = " + Server.afkkick.ToString());
			w.WriteLine("afk-kick-perm = " + ( (sbyte)Server.afkkickperm ).ToString());
			w.WriteLine("parse-emotes = " + Server.parseSmiley.ToString().ToLower());
			w.WriteLine("dollar-before-dollar = " + Server.dollarNames.ToString().ToLower());
			w.WriteLine("use-whitelist = " + Server.useWhitelist.ToString().ToLower());
			w.WriteLine("money-name = " + Server.moneys);
			w.WriteLine("opchat-perm = " + ( (sbyte)Server.opchatperm ).ToString());
			w.WriteLine("adminchat-perm = " + ( (sbyte)Server.adminchatperm ).ToString());
			w.WriteLine("log-heartbeat = " + Server.logbeat.ToString());
			w.WriteLine("force-cuboid = " + Server.forceCuboid.ToString());
			w.WriteLine("profanity-filter = " + Server.profanityFilter.ToString());
			w.WriteLine("repeat-messages = " + Server.repeatMessage.ToString());
			w.WriteLine("host-state = " + Server.ZallState.ToString());
			w.WriteLine("agree-to-rules-on-entry = " + Server.agreetorulesonentry.ToString().ToLower());
			w.WriteLine("admins-join-silent = " + Server.adminsjoinsilent.ToString().ToLower());
			w.WriteLine("server-owner = " + Server.server_owner.ToString());
			w.WriteLine("zombie-on-server-start = " + Server.zombie.StartImmediately);
			w.WriteLine("no-respawning-during-zombie = " + Server.zombie.noRespawn);
			w.WriteLine("no-pillaring-during-zombie = " + Server.zombie.noPillaring);
			w.WriteLine("zombie-name-while-infected = " + Server.zombie.ZombieName);
			w.WriteLine("enable-changing-levels = " + Server.zombie.ChangeLevels);
			w.WriteLine("zombie-survival-only-server = " + Server.zombie.SetMainLevel);
			w.WriteLine("zombie-levels-list = " + string.Join(",", Server.zombie.LevelList));
			w.WriteLine("zombie-ignores-list = " + string.Join(",", Server.zombie.IgnoredLevelList));
			w.WriteLine("zombie-save-blockchanges = " + Server.zombie.SaveLevelBlockchanges);
			w.WriteLine("guest-limit-notify = " + Server.guestLimitNotify.ToString().ToLower());
			w.WriteLine("guest-join-notify = " + Server.guestJoinNotify.ToString().ToLower());
			w.WriteLine("guest-leave-notify = " + Server.guestLeaveNotify.ToString().ToLower());
			w.WriteLine("total-undo = " + Server.totalUndo);
			w.WriteLine("physics-undo-max = " + Server.physUndo);
			w.WriteLine("draw-reload-limit = " + Server.DrawReloadLimit);
			w.WriteLine("map-gen-limit = " + Server.MapGenLimit);
			w.WriteLine("map-gen-limit-admin = " + Server.MapGenLimitAdmin);
			w.WriteLine();
			w.WriteLine("# backup options");
			w.WriteLine("backup-time = " + Server.backupInterval.ToString());
			w.WriteLine("backup-location = " + Server.backupLocation);
			w.WriteLine();
			w.WriteLine("#Error logging");
			w.WriteLine("report-back = " + Server.reportBack.ToString().ToLower());
			w.WriteLine();
			w.WriteLine("#MySQL information");
			w.WriteLine("UseMySQL = " + Server.useMySQL);
			w.WriteLine("Host = " + Server.MySQLHost);
			w.WriteLine("SQLPort = " + Server.MySQLPort);
			w.WriteLine("Username = " + Server.MySQLUsername);
			w.WriteLine("Password = " + Server.MySQLPassword);
			w.WriteLine("DatabaseName = " + Server.MySQLDatabaseName);
			w.WriteLine("Pooling = " + Server.DatabasePooling);
			w.WriteLine();
			w.WriteLine("#Colors");
			w.WriteLine("defaultColor = " + Server.DefaultColor);
			w.WriteLine("irc-color = " + Server.IRCColour);
			w.WriteLine("global-chat-color = " + Server.GlobalChatColor);
			w.WriteLine("help-syntax-color = " + Server.HelpSyntaxColor);
			w.WriteLine("help-desc-color = " + Server.HelpDescriptionColor);
			w.WriteLine();
			w.WriteLine("#Custom Messages");
			w.WriteLine("custom-ban-message = " + Server.defaultBanMessage);
			w.WriteLine("custom-shutdown-message = " + Server.shutdownMessage);
			w.WriteLine("custom-promote-message = " + Server.defaultPromoteMessage);
			w.WriteLine("custom-demote-message = " + Server.defaultDemoteMessage);
			w.WriteLine("allow-tp-to-higher-ranks = " + Server.higherranktp.ToString().ToLower());
			w.WriteLine();
			w.WriteLine("cheapmessage = " + Server.cheapMessage.ToString().ToLower());
			w.WriteLine("cheap-message-given = " + Server.cheapMessageGiven);
			try { w.WriteLine("default-rank = " + Server.defaultRank); }
			catch { w.WriteLine("default-rank = guest"); }
			w.WriteLine();
			w.WriteLine("kick-on-hackrank = " + Server.hackrank_kick.ToString().ToLower());
			w.WriteLine("hackrank-kick-time = " + Server.hackrank_kick_time.ToString());
			w.WriteLine();
			w.WriteLine("#Admin Verification");
			w.WriteLine("admin-verification = " + Server.verifyadmins.ToString().ToLower());
			w.WriteLine("verify-admin-perm = " + ( (sbyte)Server.verifyadminsrank ).ToString());
			w.WriteLine();
			w.WriteLine("#Spam Control");
			w.WriteLine("mute-on-spam = " + Server.checkspam.ToString().ToLower());
			w.WriteLine("spam-messages = " + Server.spamcounter.ToString());
			w.WriteLine("spam-mute-time = " + Server.mutespamtime.ToString());
			w.WriteLine("spam-counter-reset-time = " + Server.spamcountreset.ToString());
			w.WriteLine();
			w.WriteLine("#Review settings");
			w.WriteLine("review-view-perm = " + ( (sbyte)Server.reviewview ).ToString());
			w.WriteLine("review-enter-perm = " + ( (sbyte)Server.reviewenter ).ToString());
			w.WriteLine("review-leave-perm = " + ( (sbyte)Server.reviewleave ).ToString());
			w.WriteLine("review-cooldown = " + Server.reviewcooldown.ToString());
			w.WriteLine("review-clear-perm = " + ( (sbyte)Server.reviewclear ).ToString());
			w.WriteLine("review-next-perm = " + ( (sbyte)Server.reviewnext ).ToString());
			w.WriteLine("bufferblocks = " + Server.bufferblocks);
			w.WriteLine();
			w.WriteLine("global-chat-enabled = " + Server.UseGlobalChat.ToString().ToLower());
			w.WriteLine("show-empty-ranks = " + Server.showEmptyRanks.ToString().ToLower());
			w.WriteLine("disabledstandardtokens = " + Chat.disabledTokens);
		}
	}
}
