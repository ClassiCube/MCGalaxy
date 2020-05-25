﻿/*
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
using System.Threading;
using MCGalaxy.Games;
using MCGalaxy.Network;
using MCGalaxy.Tasks;

namespace MCGalaxy {
    public sealed partial class Server {
        public static bool cancelcommand;        
        public delegate void OnConsoleCommand(string cmd, string message);
        public static event OnConsoleCommand ConsoleCommand;
        public delegate void MessageEventHandler(string message);
        public delegate void VoidHandler();
        
        public static event MessageEventHandler OnURLChange;
        public static event VoidHandler OnSettingsUpdate;
        public static ServerConfig Config = new ServerConfig();
        
        public static IRCBot IRC;
        public static Thread locationChecker;
        public static DateTime StartTime;
        
        public static PlayerList RealmCmdsWhitelist;
        public static PlayerExtList AutoloadMaps;
        public static PlayerMetaList RankInfo = new PlayerMetaList("text/rankinfo.txt");
        public static PlayerMetaList Notes = new PlayerMetaList("text/notes.txt");
        
        /// <summary> *** DO NOT USE THIS! *** Use VersionString, as this field is a constant and is inlined if used. </summary>
        public const string InternalVersion = "1.9.2.0";
        public static Version Version { get { return new Version(InternalVersion); } }
        public static string VersionString { get { return InternalVersion; } }
        
        public static string SoftwareName = "MCGalaxy";
        public static string SoftwareNameVersioned { get { return SoftwareName + " " + VersionString; } }

        // URL hash for connecting to the server
        public static string Hash = String.Empty, URL = String.Empty;
        public static INetListen Listener;

        //Other
        public static bool SetupFinished = false;
        
        public static PlayerList bannedIP, whiteList, ircControllers, invalidIds;
        public static PlayerList ignored, hidden, agreed, vip, noEmotes, lockdown;
        public static PlayerExtList models, skins, reach, rotations, modelScales;
        public static PlayerExtList frozen, muted, tempBans, tempRanks;
        
        public static readonly List<string> Devs = new List<string>(), Mods = new List<string>();
        public static readonly List<string> Opstats = new List<string>(
            new string[] { "ban", "tempban", "xban", "banip", "kick", "warn", "mute", "freeze", "setrank" }
        );

        public static Level mainLevel;
        [Obsolete("Use LevelInfo.Loaded.Items")]
        public static List<Level> levels;

        public static PlayerList reviewlist = new PlayerList();
        static string[] announcements = new string[0];
        [Obsolete("Use %S or Server.Config.DefaultColor")]
        public static string DefaultColor;
        [Obsolete("Use Server.Config.Currency")]
        public static string moneys;
        public static string IP;
        public static string RestartPath;

        // Extra storage for custom commands
        public static ExtrasCollection Extras = new ExtrasCollection();
        
        public static int YesVotes = 0, NoVotes = 0;
        public static bool voting = false;
        
        public static Scheduler MainScheduler = new Scheduler("MCG_MainScheduler");
        public static Scheduler Background = new Scheduler("MCG_BackgroundScheduler");
        public static Scheduler Critical = new Scheduler("MCG_CriticalScheduler");
        public static Server s = new Server();

        public const byte version = 7;
        public static string salt = "";
        public static bool chatmod = false, flipHead = false;
        public static bool shuttingDown = false;
    }
}