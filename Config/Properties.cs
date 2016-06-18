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

			reviewPerms = new ReviewPerms();
			if (PropertiesFile.Read(givenPath, ref reviewPerms, LineProcessor))
				Server.s.SettingsUpdate();
			
			if (!Directory.Exists(Server.backupLocation))
				Server.backupLocation = Application.StartupPath + "/levels/backups";
			Server.updateTimer.Interval = Server.PositionInterval;
			Save(givenPath);
		}
		
		static void LineProcessor(string key, string value, ref ReviewPerms perms) {
            switch (key.ToLower()) {
				// Backwards compatibility with old config, where review permissions where global
                case "review-enter-perm":
			    case "review-leave-perm":
                    break;
                case "review-view-perm":
                    perms.viewPerm = int.Parse(value); break;
                case "review-next-perm":
                    perms.nextPerm = int.Parse(value); break;
                case "review-clear-perm":
                    perms.clearPerm = int.Parse(value); break;
                    
                default:
                    if (!ConfigElement.Parse(Server.serverConfig, key, value, null))
				        Server.s.Log("\"" + key + "\" was not a recognised level property key.");
                    break;
			}
		}		
		internal static ReviewPerms reviewPerms;
		internal class ReviewPerms { public int viewPerm = -1, nextPerm = -1, clearPerm = -1; }
		
		public static void Save() { Save("properties/server.properties"); }

		public static void Save(string givenPath) {
			try {
				File.Create(givenPath).Dispose();
				using ( StreamWriter w = File.CreateText(givenPath) ) {
					if ( givenPath.IndexOf("server") != -1 ) {
						SaveProps(w);
					}
				}
			} catch(Exception ex) {
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
			
			ConfigElement.Serialise(Server.serverConfig, " options", w, null);
		}
	}
}
