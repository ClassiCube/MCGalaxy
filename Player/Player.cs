/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MCGalaxy.Drawing;
using MCGalaxy.Games;
using MCGalaxy.SQL;
using MCGalaxy.Util;

namespace MCGalaxy {
    public sealed partial class Player : IDisposable {
        
        /// <summary>
        /// Key - Name  Value - IP
        /// All players who have left this restart.
        /// </summary>
        public Dictionary<string, object> ExtraData = new Dictionary<string, object>();

        public void ClearChat() { OnChat = null; }
        public static Dictionary<string, string> left = new Dictionary<string, string>();
        
        static List<string> pendingNames = new List<string>();
        static object pendingLock = new object();
        
        public static List<Player> connections = new List<Player>(Server.players);
        System.Timers.Timer muteTimer = new System.Timers.Timer(1000);
        public static List<string> emoteList = new List<string>();
        public List<string> listignored = new List<string>();
        public static byte number { get { return (byte)PlayerInfo.Online.Count; } }
        static System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
        static MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        public static string lastMSG = "";
        
        //TpA
        public bool Request = false;
        public string senderName = "";
        public string currentTpa = "";

        public static bool storeHelp = false;
        public static string storedHelp = "";
        private string truename;
        internal bool dontmindme = false;
        public Socket socket;
        System.Timers.Timer timespent = new System.Timers.Timer(1000);
        System.Timers.Timer loginTimer = new System.Timers.Timer(1000);
        public System.Timers.Timer pingTimer = new System.Timers.Timer(2000);
        System.Timers.Timer extraTimer = new System.Timers.Timer(22000);
        public System.Timers.Timer afkTimer = new System.Timers.Timer(2000);
        public int afkCount = 0;
        public DateTime afkStart;
        public bool cmdTimer = false;
        public bool UsingWom = false;
        public string BrushName = "normal", DefaultBrushArgs = "";

        byte[] buffer = new byte[0];
        byte[] tempbuffer = new byte[0xFF];
        public bool disconnected = false;
        public TimeSpan time;
        public string name;
        public string DisplayName;
        public string SkinName;
        public string realName;
        public int warn = 0;
        public byte id;
        public int userID = -1;
        public string ip;
        public string color;
        public Group group;
        public bool hidden = false;
        public bool painting = false;
        public bool muted = false;
        public bool jailed = false;
        public bool agreed = true;
        public bool invincible = false;
        public string prefix = "";
        public string title = "";
        public string titlecolor;
        public int TotalMessagesSent = 0;
        public int passtries = 0;
        public int ponycount = 0;
        public int rdcount = 0;
        public bool hasreadrules = false;
        public bool canusereview = true;
        public float ReachDistance = 5;
        public bool hackrank;
        
        public string FullName { get { return color + prefix + DisplayName; } }
        
        public string ColoredName { get { return color + DisplayName; } }

        //Gc checks
        public string lastmsg = "";
        public int spamcount = 0, capscount = 0, floodcount = 0, multi = 0;
        public DateTime lastmsgtime = DateTime.MinValue;
        public bool canusegc = true;

        public bool deleteMode = false;
        public bool ignorePermission = false;
        public bool ignoreGrief = false;
        public bool parseSmiley = true;
        public bool smileySaved = true;
        public bool opchat = false;
        public bool adminchat = false;
        public bool onWhitelist = false;
        public bool whisper = false;
        public string whisperTo = "";
        public bool ignoreAll = false;

        public string storedMessage = "";

        public bool trainGrab = false;
        public bool onTrain = false;
        public bool allowTnt = true;

        public bool frozen = false;
        public string following = "";
        public string possess = "";

        // Only used for possession.
        //Using for anything else can cause unintended effects!
        public bool canBuild = true;

        public int money = 0, loginMoney = 0;
        public long overallBlocks = 0;

        public int loginBlocks = 0;

        public DateTime timeLogged;
        public DateTime firstLogin;
        public int totalLogins = 0;
        public int totalKicked = 0;
        public int overallDeath = 0;

        public string savedcolor = "";

        public bool staticCommands = false;
        internal bool outdatedClient = false; // for ClassicalSharp 0.98.5, which didn't reload map for BlockDefinitions

        public DateTime ZoneSpam;
        public bool ZoneCheck = false;
        public bool zoneDel = false;

        public bool aiming;
        public bool isFlying = false;

        public bool joker = false;
        public bool adminpen = false;

        public bool voice = false;
        public string voicestring = "";

        public bool useCheckpointSpawn = false;
        public int lastCheckpointIndex = -1;
        public ushort checkpointX, checkpointY, checkpointZ;

        public Team GameTeam;
        public string GameTeamInvite;
        //CTF
        public CtfTeam team;
        public CtfTeam hasflag;

        //Countdown
        public bool playerofcountdown = false;
        public bool incountdown = false;
        public ushort countdowntempx;
        public ushort countdowntempz;
        public bool countdownsettemps = false;

        //Zombie
        public bool referee = false;
        internal int blockCount = 50;
        public bool voted = false;
        internal int blocksStacked = 0;
        internal int lastYblock = 0, lastXblock = 0, lastZblock = 0;
        public bool infected = false;
        public bool aka = false;
        public bool flipHead = false;
        internal int playersInfected = 0;
        internal string lastSpawnColor = "";
        internal bool ratedMap = false;
        internal bool pledgeSurvive = false;
        internal List<string> infectMessages = null;

        //Tnt Wars
        public bool PlayingTntWars = false;
        public int CurrentAmountOfTnt = 0;
        public int CurrentTntGameNumber; //For keeping track of which game is which
        public int TntWarsHealth = 2;
        public int TntWarsKillStreak = 0;
        public float TntWarsScoreMultiplier = 1f;
        public int TNTWarsLastKillStreakAnnounced = 0;
        public bool inTNTwarsMap = false;
        public Player HarmedBy = null; //For Assists

        //Copy
        public CopyState CopyBuffer;
        public int[] copyoffset = new int[3] { 0, 0, 0 };
        public ushort[] copystart = new ushort[3] { 0, 0, 0 };
        
        //Center
        public int[] centerstart = new int[3] { 0, 0, 0 };
        public int[] centerend = new int[3] { 0, 0, 0 };
        
        // GlobalBlock
        internal int gbStep = 0, gbTargetId = 0;
        internal BlockDefinition gbBlock;
        internal int lbStep = 0, lbTargetId = 0;
        internal BlockDefinition lbBlock;
        
        public string model = "humanoid";
        public bool spawned = false;

        public bool Mojangaccount {
            get { return truename.Contains('@'); }
        }

        //Undo
        public struct UndoPos { public ushort x, y, z; public byte type, extType, newtype, newExtType; public string mapName; public int timeDelta; }
        public UndoCache UndoBuffer = new UndoCache();
        public UndoCache RedoBuffer = new UndoCache();
        public VolatileArray<UndoDrawOpEntry> UndoDrawOps = new VolatileArray<UndoDrawOpEntry>(false);

        public bool showPortals = false;
        public bool showMBs = false;

        public string prevMsg = "";

        //Block Change variable holding
        public int[] BcVar;

        //Movement
        public int oldIndex = -1, oldFallY = 10000;
        public int fallCount = 0, drownCount = 0;

        //Games
        public DateTime lastDeath = DateTime.Now;

        public byte modeType;
        public byte[] bindings = new byte[128];
        public string[] cmdBind = new string[10];
        public string[] messageBind = new string[10];
        public string lastCMD = "";
        public sbyte c4circuitNumber = -1;

        public Level level = Server.mainLevel;
        public bool Loading = true; //True if player is loading a map.
        internal bool usingGoto = false;
        public ushort[] lastClick = new ushort[] { 0, 0, 0 };
        public ushort[] beforeTeleportPos = new ushort[] { 0, 0, 0 };
        public string beforeTeleportMap = "";
        public ushort[] pos = new ushort[] { 0, 0, 0 };
        ushort[] oldpos = new ushort[] { 0, 0, 0 };
        ushort[] basepos = new ushort[] { 0, 0, 0 };
        public byte[] rot = new byte[] { 0, 0 };
        byte[] oldrot = new byte[] { 0, 0 };

        //ushort[] clippos = new ushort[3] { 0, 0, 0 };
        //byte[] cliprot = new byte[2] { 0, 0 };

        // grief/spam detection
        public static int spamBlockCount = 200;
        public static int spamBlockTimer = 5;
        Queue<DateTime> spamBlockLog = new Queue<DateTime>(spamBlockCount);

        public int consecutivemessages;
        private System.Timers.Timer resetSpamCount = new System.Timers.Timer(Server.spamcountreset * 1000);
        //public static int spamChatCount = 3;
        //public static int spamChatTimer = 4;
        //Queue<DateTime> spamChatLog = new Queue<DateTime>(spamChatCount);

        // CmdVoteKick
        public VoteKickChoice voteKickChoice = VoteKickChoice.HasntVoted;

        // Extra storage for custom commands
        public ExtrasCollection Extras = new ExtrasCollection();

        //Chatrooms
        public string Chatroom;
        public List<string> spyChatRooms = new List<string>();
        public DateTime lastchatroomglobal;

        public List<Waypoint> Waypoints = new List<Waypoint>();

        public Random random = new Random();

        //Global Chat
        public bool ignoreGlobalChat;

        public bool loggedIn;
        public bool InGlobalChat { get; set; }
        public Dictionary<string, string> sounds = new Dictionary<string, string>();

        public bool isDev, isMod, isGCMod; //is this player a dev/mod/gcmod?
        public bool isStaff;
        public bool verifiedName;

        public static string CheckPlayerStatus(Player p) {
            if ( p.hidden ) return "hidden";
            if ( Server.afkset.Contains(p.name) ) return "afk";
            return "active";
        }
        
        public static bool CanSee(Player p, Player who) {
            if (p == null || !who.hidden) return true;
            return p.group.Permission > who.group.Permission;
        }
        
        public void SetPrefix() {
        	Team team = GameTeam;
        	prefix = team != null ? "<" + team.Color + team.Name + color + "> " : "";
        		
            string viptitle = isDev ? string.Format("{1}[{0}Dev{1}] ", Colors.blue, color) : 
        	    isMod ? string.Format("{1}[{0}Mod{1}] ", Colors.lime, color) 
        	    : isGCMod ? string.Format("{1}[{0}GCMod{1}] ", Colors.gold, color) : "";
        	prefix = prefix + viptitle;
            prefix = (title == "") ? prefix : prefix + color + "[" + titlecolor + title + color + "] ";            
        }
        
        public bool CheckIfInsideBlock() {
            ushort x = (ushort)(pos[0] / 32), y = (ushort)(pos[1] / 32), z = (ushort)(pos[2] / 32);
            byte head = level.GetTile(x, y, z);
            byte feet = level.GetTile(x, (ushort)(y - 1), z);

            return !(Block.Walkthrough(Block.Convert(head)) || head == Block.Zero)
                && !(Block.Walkthrough(Block.Convert(feet)) || feet == Block.Zero);
        }

        //This is so that plugin devs can declare a player without needing a socket..
        //They would still have to do p.Dispose()..
        public Player(string playername) { name = playername; if (playername == "IRC") { group = Group.Find("nobody"); color = Colors.lime; } }

        public Player(Socket s) {
            try {
                socket = s;
                ip = socket.RemoteEndPoint.ToString().Split(':')[0];

                /*if (IPInPrivateRange(ip))
                    exIP = ResolveExternalIP(ip);
                else
                    exIP = ip;*/

                Server.s.Log(ip + " connected to the server.");

                for ( byte i = 0; i < 128; ++i ) bindings[i] = i;

                socket.BeginReceive(tempbuffer, 0, tempbuffer.Length, SocketFlags.None, new AsyncCallback(Receive), this);
                InitTimers();
                connections.Add(this);
            }
            catch ( Exception e ) { Kick("Login failed!"); Server.ErrorLog(e); }
        }


        public void save() {
            //safe against SQL injects because no user input is provided
            string query =
                "UPDATE Players SET IP='" + ip + "'" +
                ", LastLogin='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" +
                ", totalLogin=" + totalLogins +
                ", totalDeaths=" + overallDeath +
                ", Money=" + money +
                ", totalBlocks=" + overallBlocks +
                ", totalKicked=" + totalKicked +
                ", TimeSpent='" + time.ToDBTime() +
                "' WHERE Name='" + name + "'";
            if ( MySQLSave != null )
                MySQLSave(this, query);
            OnMySQLSaveEvent.Call(this, query);
            if ( cancelmysql ) {
                cancelmysql = false;
                return;
            }
            Database.executeQuery(query);
            if (Economy.Enabled && loginMoney != money) {
                Economy.EcoStats ecos = Economy.RetrieveEcoStats(name);
                ecos.money = money;
                Economy.UpdateEcoStats(ecos);
            }           	

            try {
                if ( !smileySaved ) {
                    if ( parseSmiley )
                        emoteList.RemoveAll(s => s == name);
                    else
                        emoteList.Add(name);

                    File.WriteAllLines("text/emotelist.txt", emoteList.ToArray());
                    smileySaved = true;
                }
            }
            catch ( Exception e ) {
                Server.ErrorLog(e);
            }
            try {
                SaveUndo(this);
            } catch (Exception e) {
                Server.s.Log("Error saving undo data.");
                Server.ErrorLog(e);
            }
        }

        #region == GLOBAL MESSAGES ==
        
        public static void GlobalBlockchange(Level level, int b, byte type, byte extType) {
            ushort x, y, z;
            level.IntToPos(b, out x, out y, out z);
            GlobalBlockchange(level, x, y, z, type, extType);
        }
        
        public static void GlobalBlockchange(Level level, ushort x, ushort y, ushort z, byte type, byte extType) {
        	Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) { 
                if (p.level == level) p.SendBlockchange(x, y, z, type, extType); 
            }
        }

        [Obsolete("Use SendChatFrom() instead.")]
        public static void GlobalChat(Player from, string message) { SendChatFrom(from, message, true); }
        [Obsolete("Use SendChatFrom() instead.")]
        public static void GlobalChat(Player from, string message, bool showname) { SendChatFrom(from, message, showname); }
        
        public static void SendChatFrom(Player from, string message) { SendChatFrom(from, message, true); }
        public static void SendChatFrom(Player from, string message, bool showname) {
            if ( from == null ) return; // So we don't fucking derp the hell out!
            
            if (Last50Chat.Count() == 50)
                Last50Chat.RemoveAt(0);
            var chatmessage = new ChatMessage();
            chatmessage.text = message;
            chatmessage.username = from.color + from.name;
            chatmessage.time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");


            Last50Chat.Add(chatmessage);
            if (showname) {
                String referee = "";
                if (from.referee)
                    referee = Colors.green + "[Referee] ";
                message = referee + from.color + from.voicestring + from.color + from.prefix + from.DisplayName + ": %r&f" + message;
            }
            
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.level.worldChat && p.Chatroom == null) {
                    if (from != null && p.listignored.Contains(from.name)) continue;
                   
                    if (!p.ignoreAll || (from != null && from == p))
                        Player.SendMessage(p, message);
                }
            }
        }

        public static List<ChatMessage> Last50Chat = new List<ChatMessage>();
        public static void GlobalMessage(string message) {
            GlobalMessage(message, false);
        }
        public static void GlobalMessage(string message, bool global) {
            if ( !global )
                //message = message.Replace("%", "&");
                message = Colors.EscapeColors(message);
            else
                message = message.Replace("%G", Server.GlobalChatColor);
            
            Player[] players = PlayerInfo.Online.Items; 
            foreach (Player p in players) {
                if (p.ignoreAll || (global && p.ignoreGlobalChat)) continue;
                
                if (p.level.worldChat && p.Chatroom == null)
                    p.SendMessage(message, !global);
            }
        }
        
        public static void GlobalSpawn(Player p, ushort x, ushort y, ushort z, 
                                       byte rotx, byte roty, bool self, string possession = "") {
            Player[] players = PlayerInfo.Online.Items;
            p.lastSpawnColor = p.infected ? ZombieGame.InfectCol : p.color;
            foreach (Player other in players) {
            	if ((other.Loading && p != other) || p.level != other.level) continue;
            	if ((p.hidden || p.referee) && !self) continue;
                
                if (p != other) {
                    SpawnEntity(p, other, p.id, x, y, z, rotx, roty, possession);
                } else if (self) {
                    other.pos = new ushort[3] { x, y, z }; other.rot = new byte[2] { rotx, roty };
                    other.oldpos = other.pos; other.basepos = other.pos; other.oldrot = other.rot;
                    SpawnEntity(p, other, 0xFF, x, y, z, rotx, roty, possession);
                }
            }
        }
        
        internal static void SpawnEntity(Player p, Player dst, byte id, ushort x, ushort y, ushort z, 
                                       byte rotx, byte roty, string possession = "") {
            if (!Server.ZombieModeOn || !p.infected) {
                dst.SendSpawn(id, p.color + p.name + possession, x, y, z, rotx, roty); return;
            }
            
            if (Server.zombie.ZombieName != "" && !dst.aka)
                dst.SendSpawn(id, Colors.red + Server.zombie.ZombieName + possession, x, y, z, rotx, roty);
            else
                dst.SendSpawn(id, Colors.red + p.name + possession, x, y, z, rotx, roty);
            if (dst.HasCpeExt(CpeExt.ChangeModel) && id != 0xFF)
                dst.SendChangeModel(id, "zombie");
        }
        
        public static void GlobalDespawn(Player p, bool self) {
        	Player[] players = PlayerInfo.Online.Items; 
        	foreach (Player other in players) {
                if (p.level != other.level || (p.hidden && !self) ) continue;
                if (p != other) { other.SendDespawn(p.id); }
                else if (self) { other.SendDespawn(255); }
            }
        }

        public bool MarkPossessed(string marker = "") {
            if (marker != "") {
                Player controller = PlayerInfo.FindExact(marker);
                if (controller == null) return false;
                marker = " (" + controller.color + controller.name + color + ")";
            }
        	
            GlobalDespawn(this, true);
            GlobalSpawn(this, pos[0], pos[1], pos[2], rot[0], rot[1], true, marker);
            return true;
        }

        public static void GlobalUpdate() { 
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (!p.hidden) p.UpdatePosition();
            }
        }
        #endregion
        #region == DISCONNECTING ==
        
        public void Disconnect() { LeaveServer("Disconnected", PlayerDB.GetLogoutMessage(this)); }
        public void Kick(string kickString) { LeaveServer(kickString, null); }
        public void Kick(string kickString, bool sync = false) { LeaveServer(kickString, null, sync); }

        [Obsolete("Use LeaveServer() instead")]
        public void leftGame(string kickMsg = "") { LeaveServer(kickMsg, null); }

        public void LeaveServer(string kickMsg, string discMsg, bool sync = false) {
        	if (discMsg != null) discMsg = Colors.EscapeColors(discMsg);
        	if (kickMsg != null) kickMsg = Colors.EscapeColors(kickMsg);
        	
            OnPlayerDisconnectEvent.Call(this, discMsg ?? kickMsg);
            //Umm...fixed?
            if (name == "") {
                if (socket != null) CloseSocket();
                connections.Remove(this);
                SaveUndo(this);
                disconnected = true;
                return;
            }
            
            Server.reviewlist.Remove(name);
            try { 
                if (disconnected) {
                    CloseSocket();
                    connections.Remove(this);
                    return;
                }
                // FlyBuffer.Clear();
                disconnected = true;
                SaveIgnores();
                pingTimer.Stop();
                pingTimer.Dispose();
                afkTimer.Stop();
                afkTimer.Dispose();
                muteTimer.Stop();
                muteTimer.Dispose();
                timespent.Stop();
                timespent.Dispose();
                afkCount = 0;
                afkStart = DateTime.Now;
                Server.afkset.Remove(name);
                isFlying = false;
                aiming = false;
                
                SendKick(kickMsg, sync);
                if (!loggedIn) {
                	connections.Remove(this);
                	RemoveFromPending();
                    Server.s.Log(ip + " disconnected.");
                    return;
                }

                Server.zombie.PlayerLeftServer(this);
                if ( team != null ) team.RemoveMember(this);
                Server.Countdown.PlayerLeftServer(this);
                TntWarsGame tntwarsgame = TntWarsGame.GetTntWarsGame(this);
                if ( tntwarsgame != null ) {
                	tntwarsgame.Players.Remove(tntwarsgame.FindPlayer(this));
                	tntwarsgame.SendAllPlayersMessage("TNT Wars: " + color + name + Server.DefaultColor + " has left TNT Wars!");
                }

                GlobalDespawn(this, false);
                if (discMsg != null) {
                	if (!hidden) {
                		string leavem = "&c- " + FullName + " %S" + discMsg;
                		if ((Server.guestLeaveNotify && group.Permission <= LevelPermission.Guest) || group.Permission > LevelPermission.Guest) {
                			Player[] players = PlayerInfo.Online.Items; 
                			foreach (Player pl in players) { Player.SendMessage(pl, leavem); }
                		}
                	}
                	Server.s.Log(name + "disconnected (" + discMsg + ").");
                } else {
                	totalKicked++;
                	SendChatFrom(this, "&c- " + color + prefix + DisplayName + " %Skicked (" + kickMsg + "%S).", false);
                	Server.s.Log(name + " kicked (" + kickMsg + ").");
                }

                try { save(); }
                catch ( Exception e ) { Server.ErrorLog(e); }

                PlayerInfo.Online.Remove(this);
                Server.s.PlayerListUpdate();
                if (name != null)
                	left[name.ToLower()] = ip;
                if (PlayerDisconnect != null)
                	PlayerDisconnect(this, discMsg ?? kickMsg);
                if (Server.AutoLoad && level.unload && !level.IsMuseum && IsAloneOnCurrentLevel())
                	level.Unload(true);
                Dispose();
            } catch ( Exception e ) { 
                Server.ErrorLog(e); 
            } finally {
                CloseSocket();
            }
        }
        
        public static void SaveUndo(Player p) {
            try {
                UndoFile.SaveUndo(p);
            } catch (Exception e) { 
                Server.s.Log("Error saving undo data for " + p.name + "!"); Server.ErrorLog(e); 
            }
        }

        public void Dispose() {
            connections.Remove(this);
            RemoveFromPending();
            Extras.Clear();
            if (CopyBuffer != null)
                CopyBuffer.Clear();
            RedoBuffer.Clear();
            UndoDrawOps.Clear();
            UndoBuffer.Clear();
            spamBlockLog.Clear();
            //spamChatLog.Clear();
            spyChatRooms.Clear();
            /*try
{
//this.commThread.Abort();
}
catch { }*/
        }

        public bool IsAloneOnCurrentLevel() {
            return PlayerInfo.players.All(pl => pl.level != level || pl == this);
        }

        #endregion
        #region == OTHER ==
        
        [Obsolete("Use PlayerInfo.players")]
        public static List<Player> players;
        
        [Obsolete("Use PlayerInfo.Find(name)")]
        public static Player Find(string name) { return PlayerInfo.Find(name); }
        
        [Obsolete("Use PlayerInfo.FindExact(name)")]
        public static Player FindExact(string name) { return PlayerInfo.FindExact(name); }
        
        [Obsolete("Use PlayerInfo.FindNick(name)")]
        public static Player FindNick(string name) { return PlayerInfo.FindNick(name); }
        
        static byte FreeId() {
            /*
for (byte i = 0; i < 255; i++)
{
foreach (Player p in players)
{
if (p.id == i) { goto Next; }
} return i;
Next: continue;
} unchecked { return 0xFF; }*/

            for ( byte i = 0; i < 255; i++ ) {
                bool used = PlayerInfo.players.Any(p => p.id == i);

                if ( !used )
                    return i;
            }
            return (byte)1;
        }

        // TODO: Optimize this using a StringBuilder
        static List<string> Wordwrap(string message) {
            List<string> lines = new List<string>();
            message = Regex.Replace(message, @"(&[0-9a-f])+(&[0-9a-f])", "$2");
            message = Regex.Replace(message, @"(&[0-9a-f])+$", "");

            int limit = 64; string color = "";
            while ( message.Length > 0 ) {
                //if (Regex.IsMatch(message, "&a")) break;

                if ( lines.Count > 0 ) {
                    if ( message[0].ToString() == "&" )
                        message = "> " + message.Trim();
                    else
                        message = "> " + color + message.Trim();
                }

                if ( message.IndexOf("&") == message.IndexOf("&", message.IndexOf("&") + 1) - 2 )
                    message = message.Remove(message.IndexOf("&"), 2);

                if ( message.Length <= limit ) { lines.Add(message); break; }
                for ( int i = limit - 1; i > limit - 20; --i )
                    if ( message[i] == ' ' ) {
                        lines.Add(message.Substring(0, i));
                        goto Next;
                    }

            retry:
                if ( message.Length == 0 || limit == 0 ) { return lines; }

                try {
                    if ( message.Substring(limit - 2, 1) == "&" || message.Substring(limit - 1, 1) == "&" ) {
                        message = message.Remove(limit - 2, 1);
                        limit -= 2;
                        goto retry;
                    }
                    else if ( message[limit - 1] < 32 || message[limit - 1] > 127 ) {
                        message = message.Remove(limit - 1, 1);
                        limit -= 1;
                        //goto retry;
                    }
                }
                catch { return lines; }
                lines.Add(message.Substring(0, limit));

            Next: message = message.Substring(lines[lines.Count - 1].Length);
                if ( lines.Count == 1 ) limit = 60;

                int index = lines[lines.Count - 1].LastIndexOf('&');
                if ( index != -1 ) {
                    if ( index < lines[lines.Count - 1].Length - 1 ) {
                        char next = lines[lines.Count - 1][index + 1];
                        if ( Colors.MapColor(ref next) ) color = "&" + next;
                        if ( index == lines[lines.Count - 1].Length - 1 ) {
                            lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 2);
                        }
                    }
                    else if ( message.Length != 0 ) {
                        char next = message[0];
                        if ( Colors.MapColor(ref next) ) color = "&" + next;
                        lines[lines.Count - 1] = lines[lines.Count - 1].Substring(0, lines[lines.Count - 1].Length - 1);
                        message = message.Substring(1);
                    }
                }
            }
            for ( int i = 0; i < lines.Count; i++ ) // Gotta do it the old fashioned way...
            {
            	char[] temp = lines[i].ToCharArray();
                if ( temp[temp.Length - 2] == '%' || temp[temp.Length - 2] == '&' ) {
                    temp[temp.Length - 1] = ' ';
                    temp[temp.Length - 2] = ' ';
                }
                lines[i] = new string(temp);
            }
            return lines;
        }
        public static bool ValidName(string name) {
            string allowedchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890._+";
            return name.All(ch => allowedchars.IndexOf(ch) != -1);
        }

        public static int GetBannedCount() {
            try {
                return File.ReadAllLines("ranks/banned.txt").Length;
            }
            catch/* (Exception ex)*/
            {
                return 0;
            }
        }
        #endregion

        public void BlockUntilLoad(int sleep) {
            while (Loading) 
                Thread.Sleep(sleep);
        }
        public void RevertBlock(ushort x, ushort y, ushort z) {
            byte b = level.GetTile(x, y, z);
            SendBlockchange(x, y, z, b);
        }
        
        bool CheckBlockSpam() {
            if ( spamBlockLog.Count >= spamBlockCount ) {
                DateTime oldestTime = spamBlockLog.Dequeue();
                double spamTimer = DateTime.UtcNow.Subtract(oldestTime).TotalSeconds;
                if ( spamTimer < spamBlockTimer && !ignoreGrief ) {
                    Kick("You were kicked by antigrief system. Slow down.");
                    SendMessage(Colors.red + DisplayName + " was kicked for suspected griefing.");
                    Server.s.Log(name + " was kicked for block spam (" + spamBlockCount + " blocks in " + spamTimer + " seconds)");
                    return true;
                }
            }
            spamBlockLog.Enqueue(DateTime.UtcNow);
            return false;
        }

        public static bool IPInPrivateRange(string ip) {
            //range of 172.16.0.0 - 172.31.255.255
            if (ip.StartsWith("172.") && (int.Parse(ip.Split('.')[1]) >= 16 && int.Parse(ip.Split('.')[1]) <= 31))
                return true;
            return IPAddress.IsLoopback(IPAddress.Parse(ip)) || ip.StartsWith("192.168.") || ip.StartsWith("10.");
            //return IsLocalIpAddress(ip);
        }

        public static bool IsLocalIpAddress(string host) {
            try { // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);
                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach ( IPAddress hostIP in hostIPs ) {
                    // is localhost
                    if ( IPAddress.IsLoopback(hostIP) ) return true;
                    // is local address
                    foreach ( IPAddress localIP in localIPs ) {
                        if ( hostIP.Equals(localIP) ) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        public bool EnoughMoney(int amount) {
        	return money >= amount;
        }
        
        public void OnMoneyChanged() {
            if (Server.ZombieModeOn) Server.zombie.PlayerMoneyChanged(this);
            if (Server.lava.active) Server.lava.PlayerMoneyChanged(this);
        }
        
        public void ReviewTimer() {
            this.canusereview = false;
            System.Timers.Timer Clock = new System.Timers.Timer(1000 * Server.reviewcooldown);
            Clock.Elapsed += delegate { this.canusereview = true; Clock.Dispose(); };
            Clock.Start();
        }

        public void TntAtATime() {
            new Thread(() => {
                CurrentAmountOfTnt += 1;
                switch ( TntWarsGame.GetTntWarsGame(this).GameDifficulty ) {
                    case TntWarsGame.TntWarsDifficulty.Easy:
                        Thread.Sleep(3250);
                        break;

                    case TntWarsGame.TntWarsDifficulty.Normal:
                        Thread.Sleep(2250);
                        break;

                    case TntWarsGame.TntWarsDifficulty.Hard:
                    case TntWarsGame.TntWarsDifficulty.Extreme:
                        Thread.Sleep(1250);
                        break;
                }
                CurrentAmountOfTnt -= 1;
            }).Start();
        }
        
        public static bool BlacklistCheck(string name, string foundLevel) {
            string path = "levels/blacklists/" + foundLevel + ".txt";
            if (!File.Exists(path)) { return false; }
            if (File.ReadAllText(path).Contains(name)) { return true; }
            return false;
        }
        
        internal static bool CheckVote(string message, Player p, string a, string b, ref int totalVotes) {
            if (!p.voted && (message == a || message == b)) {
                totalVotes++;
                p.SendMessage(Colors.red + "Thanks for voting!");
                p.voted = true;
                return true;
            }
            return false;
        }
        
        void SaveIgnores() {
            string path = "ranks/ignore/" + name + ".txt";
            if (!File.Exists(path)) return;
            
            try {
                using (StreamWriter w = new StreamWriter(path)) {
            	    if (ignoreGlobalChat) w.WriteLine("&global");
                    if (ignoreAll) w.WriteLine("&all");                    
                    foreach (string line in listignored)
                        w.WriteLine(line);
                }
            } catch (Exception ex) {
            	Server.ErrorLog(ex);
                Server.s.Log("Failed to save ignored list for player: " + this.name);
            }
        }
        
        void LoadIgnores() {
            string path = "ranks/ignore/" + name + ".txt";
            if (!File.Exists(path)) return;
            
            try {
                string[] lines = File.ReadAllLines(path);
                foreach (string line in lines) {
                    if (line == "&global") ignoreGlobalChat = true;
                    else if (line == "&all") ignoreAll = true;
                    else listignored.Add(line);
                }
            } catch (Exception ex) {
                Server.ErrorLog(ex);
                Server.s.Log("Failed to load ignore list for: " + name);
            }
            if (ignoreAll || ignoreGlobalChat || listignored.Count > 0) {
                SendMessage("&cYou are still ignoring some people from your last login.");
                SendMessage("&cType &a/ignore list &cto see the list.");
            }
        }
        
        internal void RemoveInvalidUndos() {
            UndoDrawOpEntry[] items = UndoDrawOps.Items;
            for (int i = 0; i < items.Length; i++) {
                if (items[i].End < UndoBuffer.LastClear)
                    UndoDrawOps.Remove(items[i]);
            }
        }
    }
}
