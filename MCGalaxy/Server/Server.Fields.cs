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
    public sealed partial class Server {
        public static bool cancelcommand;
        public static bool canceladmin;
        public static bool cancellog;
        public static bool canceloplog;
        public static string apppath = Utils.FolderPath;
        
        public delegate void OnConsoleCommand(string cmd, string message);
        public static event OnConsoleCommand ConsoleCommand;
        public delegate void HeartBeatHandler();
        public delegate void MessageEventHandler(string message);
        public delegate void VoidHandler();
        
        public static event HeartBeatHandler HeartBeatFail;
        public static event MessageEventHandler OnURLChange;
        public static event VoidHandler OnPlayerListChange;
        public static event VoidHandler OnSettingsUpdate;
        
        public static IRCBot IRC;
        public static Thread locationChecker;
        public static DateTime StartTime, StartTimeLocal;
        
        public static PlayerExtList AutoloadMaps;
        public static PlayerMetaList RankInfo = new PlayerMetaList("text/rankinfo.txt");
        public static PlayerMetaList Notes = new PlayerMetaList("text/notes.txt");
        
        /// <summary> *** DO NOT USE THIS! *** Use VersionString, as this field is a constant and is inlined if used. </summary>
        public const string InternalVersion = "1.8.9.4";
        public static Version Version { get { return new Version(InternalVersion); } }
        public static string VersionString { get { return InternalVersion; } }
        
        public static string SoftwareName = "MCGalaxy";
        public static string SoftwareNameVersioned { get { return SoftwareName + " " + VersionString; } }

        // URL hash for connecting to the server
        public static string Hash = String.Empty, URL = String.Empty;
        public static INetworkListen Listener;

        //Chatrooms
        public static List<string> Chatrooms = new List<string>();
        //Other
        public static bool UseCTF = false;
        public static bool ServerSetupFinished = false;
        public static CTFGame ctf = null;
        public static PlayerList bannedIP, whiteList, ircControllers, invalidIds;
        public static PlayerList ignored, hidden, agreed, vip, noEmotes, lockdown;
        public static PlayerExtList models, skins, reach, rotations;
        public static PlayerExtList frozen, muted, jailed, tempBans, tempRanks;
        
        public static readonly List<string> Devs = new List<string>(), Mods = new List<string>();

        internal static readonly List<string> opstats = new List<string>(
            new string[] { "ban", "tempban", "xban", "banip", "kickban", "kick",
                "warn", "mute", "freeze", "demote", "promote", "setrank" }
        );
        public static List<string> Opstats { get { return opstats; } }

        public static PerformanceCounter PCCounter = null;
        public static PerformanceCounter ProcessCounter = null;

        public static Level mainLevel;
        [Obsolete("Use LevelInfo.Loaded.Items")]
        public static List<Level> levels;
        //reviewlist intitialize
        public static List<string> reviewlist = new List<string>();
        public static List<string> ircafkset = new List<string>();
        public static List<string> messages = new List<string>();

        public static string IP;
        
        //Global VoteKick In Progress Flag
        public static bool voteKickInProgress = false;
        public static int voteKickVotesNeeded = 0;

        // Extra storage for custom commands
        public static ExtrasCollection Extras = new ExtrasCollection();

        // Games
        public static ZombieGame zombie;
        
        public static int YesVotes = 0, NoVotes = 0;
        public static bool voting = false, votingforlevel = false;

        public static LavaSurvival lava;
        public static CountdownGame Countdown;
        
        public static Scheduler MainScheduler = new Scheduler("MCG_MainScheduler");
        public static Scheduler Background = new Scheduler("MCG_BackgroundScheduler");
        public static Scheduler Critical = new Scheduler("MCG_CriticalScheduler");
        public static Server s = new Server();

        public const byte version = 7;
        public static string salt = "";
        public static bool chatmod = false, flipHead = false;

        public static bool shuttingDown = false, restarting = false;
        public static bool mono { get { return (Type.GetType("Mono.Runtime") != null); } }
    }
}