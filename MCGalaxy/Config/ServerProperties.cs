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
using System.IO;
using System.Net;
using System.Security.Cryptography;
using MCGalaxy.Games;
using MCGalaxy.SQL;

namespace MCGalaxy {
    
    public static class SrvProperties {
        
        public static void GenerateSalt() {
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            char[] chars = new char[16];
            byte[] one = new byte[1];
            
            for (int i = 0; i < chars.Length; ) {
                rng.GetBytes(one);
                if (!Char.IsLetterOrDigit((char)one[0])) continue;
                
                chars[i] = (char)one[0]; i++;
            }
            Server.salt = new string(chars);
        }
        
        public static void Load() {
            oldPerms = new OldPerms();
            if (PropertiesFile.Read(Paths.ServerPropsFile, ref oldPerms, LineProcessor))
                Server.SettingsUpdate();
            if (oldPerms.saveZS)
                ZombieGameProps.SaveSettings();
            ZombieGameProps.LoadSettings();
            
            Database.Backend = ServerConfig.UseMySQL ? MySQLBackend.Instance : SQLiteBackend.Instance;
            Server.DefaultColor = ServerConfig.DefaultColor;

            if (!Directory.Exists(ServerConfig.BackupDirectory))
                ServerConfig.BackupDirectory = Path.Combine(Utils.FolderPath, "levels/backups");
            Save();
        }
        
        static void LineProcessor(string key, string value, ref OldPerms perms) {
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
                case "opchat-perm":
                    perms.opchatPerm = int.Parse(value); break;
                case "adminchat-perm":
                    perms.adminchatPerm = int.Parse(value); break;
                    
                default:
                    if (!ConfigElement.Parse(Server.serverConfig, key, value, null)) {
                        // ZS used to be part of server.properties
                        if (ConfigElement.Parse(Server.zombieConfig, key, value, null)) {
                            perms.saveZS = true;
                        } else {
                            Logger.Log(LogType.Warning, "\"{0}\" was not a recognised server property key.", key);
                        }
                    }
                    break;
            }
        }
        internal static OldPerms oldPerms;
        internal class OldPerms { public int viewPerm = -1, nextPerm = -1,
            clearPerm = -1, opchatPerm = -1, adminchatPerm = -1; public bool saveZS; }
        
        static readonly object saveLock = new object();
        public static void Save() {
            try {
                lock (saveLock) {
                    using (StreamWriter w = new StreamWriter(Paths.ServerPropsFile))
                        SaveProps(w);
                }
            } catch (Exception ex) {
                Logger.LogError(ex);
                Logger.Log(LogType.Warning, "SAVE FAILED! " + Paths.ServerPropsFile);
            }
        }
        
        static void SaveProps(StreamWriter w) {
            w.WriteLine("#   Edit the settings below to modify how your server operates. This is an explanation of what each setting does.");
            w.WriteLine("#   server-name                   = The name which displays on classicube.net");
            w.WriteLine("#   motd                          = The message which displays when a player connects");
            w.WriteLine("#   port                          = The port to operate from");
            w.WriteLine("#   console-only                  = Run without a GUI (useful for Linux servers with mono)");
            w.WriteLine("#   verify-names                  = Verify the validity of names");
            w.WriteLine("#   public                        = Set to true to appear in the public server list");
            w.WriteLine("#   max-players                   = The maximum number of connections");
            w.WriteLine("#   max-guests                    = The maximum number of guests allowed");
            w.WriteLine("#   max-maps                      = The maximum number of maps loaded at once");
            w.WriteLine("#   world-chat                    = Set to true to enable world chat");
            w.WriteLine("#   irc                           = Set to true to enable the IRC bot");
            w.WriteLine("#   irc-nick                      = The name of the IRC bot");
            w.WriteLine("#   irc-server                    = The server to connect to");
            w.WriteLine("#   irc-channel                   = The channel to join");
            w.WriteLine("#   irc-opchannel                 = The channel to join (posts OpChat)");
            w.WriteLine("#   irc-port                      = The port to use to connect");
            w.WriteLine("#   irc-identify                  = (true/false)    Do you want the IRC bot to Identify itself with nickserv. Note: You will need to register it's name with nickserv manually.");
            w.WriteLine("#   irc-password                  = The password you want to use if you're identifying with nickserv");
            w.WriteLine("#   anti-tunnels                  = Stops people digging below max-depth");
            w.WriteLine("#   max-depth                     = The maximum allowed depth to dig down");
            w.WriteLine("#   backup-time                   = The number of seconds between automatic backups");
            w.WriteLine("#   overload                      = The higher this is, the longer the physics is allowed to lag.  Default 1500");
            w.WriteLine("#   use-whitelist                 = Switch to allow use of a whitelist to override IP bans for certain players.  Default false.");
            w.WriteLine("#   premium-only                  = Only allow premium players (paid for minecraft) to access the server. Default false.");
            w.WriteLine("#   force-cuboid                  = Run cuboid until the limit is hit, instead of canceling the whole operation.  Default false.");
            w.WriteLine("#   profanity-filter              = Replace certain bad words in the chat.  Default false.");
            w.WriteLine("#   notify-on-join-leave          = Show a balloon popup in tray notification area when a player joins/leaves the server.  Default false.");
            w.WriteLine("#   allow-tp-to-higher-ranks      = Allows the teleportation to players of higher ranks");
            w.WriteLine("#   agree-to-rules-on-entry       = Forces all new players to the server to agree to the rules before they can build or use commands.");
            w.WriteLine("#   adminchat-perm                = The rank required to view adminchat. Default rank is superop.");
            w.WriteLine("#   admins-join-silent            = Players who have adminchat permission join the game silently. Default true");
            w.WriteLine("#   server-owner                  = The minecraft name, of the owner of the server.");
            w.WriteLine("#   total-undo                    = Track changes made by the last X people logged on for undo purposes. Folder is rotated when full, so when set to 200, will actually track around 400.");
            w.WriteLine("#   guest-limit-notify            = Show -Too Many Guests- message in chat when maxGuests has been reached. Default false");
            w.WriteLine("#   guest-join-notify             = Shows when guests and lower ranks join server in chat and IRC. Default true");
            w.WriteLine("#   guest-leave-notify            = Shows when guests and lower ranks leave server in chat and IRC. Default true");
            w.WriteLine();
            w.WriteLine("#   UseMySQL                      = Use MySQL (true) or use SQLite (false)");
            w.WriteLine("#   Host                          = The host name for the database (usually 127.0.0.1)");
            w.WriteLine("#   SQLPort                       = Port number to be used for MySQL.  Unless you manually changed the port, leave this alone.  Default 3306.");
            w.WriteLine("#   Username                      = The username you used to create the database (usually root)");
            w.WriteLine("#   Password                      = The password set while making the database");
            w.WriteLine("#   DatabaseName                  = The name of the database stored (Default = MCZall)");
            w.WriteLine();
            w.WriteLine("#   defaultColor                  = The color code of the default messages (Default = &e)");
            w.WriteLine();
            w.WriteLine("#   kick-on-hackrank              = Set to true if hackrank should kick players");
            w.WriteLine("#   hackrank-kick-time            = Number of seconds until player is kicked");
            w.WriteLine("#   custom-rank-welcome-messages  = Decides if different welcome messages for each rank is enabled. Default true.");
            w.WriteLine("#   ignore-ops                    = Decides whether or not an operator can be ignored. Default false.");
            w.WriteLine();
            w.WriteLine("#   admin-verification            = Determines whether admins have to verify on entry to the server.  Default true.");
            w.WriteLine("#   verify-admin-perm             = The minimum rank required for admin verification to occur.");
            w.WriteLine();
            w.WriteLine("#   mute-on-spam                  = If enabled it mutes a player for spamming.  Default false.");
            w.WriteLine("#   spam-messages                 = The amount of messages that have to be sent \"consecutively\" to be muted.");
            w.WriteLine("#   spam-mute-time                = The amount of seconds a player is muted for spam.");
            w.WriteLine("#   spam-counter-reset-time       = The amount of seconds the \"consecutive\" messages have to fall between to be considered spam.");
            w.WriteLine();
            w.WriteLine("#   As an example, if you wanted the spam to only mute if a user posts 5 messages in a row within 2 seconds, you would use the folowing:");
            w.WriteLine("#   mute-on-spam                  = true");
            w.WriteLine("#   spam-messages                 = 5");
            w.WriteLine("#   spam-mute-time                = 60");
            w.WriteLine("#   spam-counter-reset-time       = 2");
            w.WriteLine("#   bufferblocks                  = Should buffer blocks by default for maps?");
            w.WriteLine();
            
            ConfigElement.Serialise(Server.serverConfig, " options", w, null);
        }
    }
}
