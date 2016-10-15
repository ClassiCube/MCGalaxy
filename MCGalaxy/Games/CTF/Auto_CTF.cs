/*
	Copyright 2011 MCForge
	
	Written by fenderrock87
	
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
using System.Threading;
using MCGalaxy.Commands.World;
using MCGalaxy.SQL;

namespace MCGalaxy.Games
{
    /// <summary> This is the team class for CTF </summary>
    public sealed class Teams
    {
        public string color;
        public int points = 0;
        public List<Player> members;
        
        /// <summary> Create a new Team Object </summary>
        public Teams(string color) {
            color = Colors.Parse(color);
            members = new List<Player>();
        }
        /// <summary> Add a player to the team </summary>
        public void Add(Player p) {
            members.Add(p);
        }
        
        /// <summary> Checks to see if the player is on this team </summary>
        public bool isOnTeam(Player p) {
            return members.IndexOf(p) != -1;
        }
    }
    internal sealed class Data
    {
        public Player p;
        public int cap = 0;
        public int tag = 0;
        public int points = 0;
        public bool hasflag;
        public bool blue;
        public bool tagging = false;
        public bool chatting = false;
        public Data(bool team, Player p)
        {
            blue = team; this.p = p;
        }
    }
    internal sealed class Base
    {
        public ushort x;
        public ushort y;
        public ushort z;
        public ushort spawnx = 0;
        public ushort spawny = 0;
        public ushort spawnz = 0;
        public byte block;
        public void SendToSpawn(Level mainlevel, Auto_CTF game, Player p1)
        {
            Random rand = new Random();
            if (spawnx == 0 && spawny == 0 && spawnz == 0)
            {
                ushort xx = (ushort)(rand.Next(0, mainlevel.Width));
                ushort yy = (ushort)(rand.Next(0, mainlevel.Height));
                ushort zz = (ushort)(rand.Next(0, mainlevel.Length));
                while (mainlevel.GetTile(xx, yy, zz) != Block.air && game.OnSide((ushort)(zz * 32), this))
                {
                    xx = (ushort)(rand.Next(0, mainlevel.Width));
                    yy = (ushort)(rand.Next(0, mainlevel.Height));
                    zz = (ushort)(rand.Next(0, mainlevel.Length));
                }
                p1.SendPos(Entities.SelfID, (ushort)(xx * 32), (ushort)(yy * 32), (ushort)(zz * 32), p1.rot[0], p1.rot[1]);
            }
            else
                p1.SendPos(Entities.SelfID, spawnx, spawny, spawnz, p1.rot[0], p1.rot[1]);

        }
        public Base(ushort x, ushort y, ushort z, Teams team)
        {
            this.x = x; this.y = y; this.z = z;
        }
        public Base()
        {
        }
    }
    /// <summary>
    /// This is the CTF gamemode
    /// </summary>
    public sealed class Auto_CTF
    {
        public System.Timers.Timer tagging = new System.Timers.Timer(500);
        public bool voting = false;
        int vote1 = 0;
        int vote2 = 0;
        int vote3 = 0;
        string map1 = "";
        string map2 = "";
        string map3 = "";
        public int xline;
        public bool started = false;
        public int zline;
        public int yline;
        int tagpoint = 5;
        int cappoint = 10;
        int taglose = 5;
        int caplose = 10;
        bool look = false;
        public int maxpoints = 3;
        Teams redteam;
        Teams blueteam;
        Base bluebase;
        Base redbase;
        Level mainlevel;
        List<string> maps = new List<string>();
        List<Data> cache = new List<Data>();
        string mapname = "";
        
        /// <summary> Load a map into CTF </summary>
        /// <param name="map">The map to load</param>
        public void LoadMap(string map) {
            mapname = map;
            PropertiesFile.Read("CTF/" + mapname + ".config", LineProcessor);
            Command.all.Find("unload").Use(null, "ctf");
            if (File.Exists("levels/ctf.lvl"))
                File.Delete("levels/ctf.lvl");
            File.Copy("CTF/maps/" + mapname + ".lvl", "levels/ctf.lvl");
            CmdLoad.LoadLevel(null, "ctf");
            mainlevel = LevelInfo.FindExact("ctf");
        }
        
        void LineProcessor(string key, string value) {
        	switch (key.ToLower()) {
                case "base.red.x":
                    redbase.x = ushort.Parse(value); break;
                case "base.red.y":
                    redbase.y = ushort.Parse(value); break;
                case "game.maxpoints":
                    maxpoints = int.Parse(value); break;
                case "game.tag.points-gain":
                    tagpoint = int.Parse(value); break;
                case "game.tag.points-lose":
                    taglose = int.Parse(value); break;
                case "game.capture.points-gain":
                    cappoint = int.Parse(value); break;
                case "game.capture.points-lose":
                    caplose = int.Parse(value); break;
                case "auto.setup":
                    look = bool.Parse(value); break;
                case "base.red.z":
                    redbase.z = ushort.Parse(value); break;
                case "base.red.block":
                    redbase.block = Block.Byte(value); break;
                case "base.blue.block":
                    bluebase.block = Block.Byte(value); break;
                case "base.blue.spawnx":
                    bluebase.spawnx = ushort.Parse(value); break;
                case "base.blue.spawny":
                    bluebase.spawny = ushort.Parse(value); break;
                case "base.blue.spawnz":
                    bluebase.spawnz = ushort.Parse(value); break;
                case "base.red.spawnx":
                    redbase.spawnx = ushort.Parse(value); break;
                case "base.red.spawny":
                    redbase.spawny = ushort.Parse(value); break;
                case "base.red.spawnz":
                    redbase.spawnz = ushort.Parse(value); break;
                case "base.blue.x":
                    bluebase.x = ushort.Parse(value); break;
                case "base.blue.y":
                    bluebase.y = ushort.Parse(value); break;
                case "base.blue.z":
                    bluebase.z = ushort.Parse(value); break;
                case "map.line.z":
                    zline = ushort.Parse(value); break;
            }
        }
        
        /// <summary> Create a new CTF object </summary>
        public Auto_CTF() {
            //Load some configs
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            if (!File.Exists("CTF/maps.config"))
            {
                Server.s.Log("No maps were found!");
                return;
            }
            string[] lines = File.ReadAllLines("CTF/maps.config");
            foreach (string l in lines)
                maps.Add(l);
            if (maps.Count == 0) { Server.s.Log("No maps were found!"); return; }
            
            redbase = new Base();
            bluebase = new Base();
            Start();
            //Lets get started
            Player.PlayerDeath += new Player.OnPlayerDeath(Player_PlayerDeath);
            Player.PlayerChat += new Player.OnPlayerChat(Player_PlayerChat);
            Player.PlayerCommand += new Player.OnPlayerCommand(Player_PlayerCommand);
            Player.PlayerBlockChange += new Player.BlockchangeEventHandler2(Player_PlayerBlockChange);
            Player.PlayerDisconnect += new Player.OnPlayerDisconnect(Player_PlayerDisconnect);
            Level.LevelUnload += new Level.OnLevelUnload(mainlevel_LevelUnload);
            tagging.Elapsed += new System.Timers.ElapsedEventHandler(tagging_Elapsed);
            tagging.Start();
        }
        
        /// <summary> Stop the CTF game (if its running) </summary>
        public void Stop() {
            tagging.Stop();
            tagging.Dispose();
            mainlevel = null;
            started = false;
            if (LevelInfo.FindExact("ctf") != null)
                Command.all.Find("unload").Use(null, "ctf");
        }
        
        void tagging_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
        	Player[] online = PlayerInfo.Online.Items; 
        	foreach (Player p in online) {
                if (p.level == mainlevel)
                {
                    ushort x = p.pos[0];
                    ushort y = p.pos[1];
                    ushort z = p.pos[2];
                    Base b = null;
                    if (redteam.members.Contains(p))
                        b = redbase;
                    else if (blueteam.members.Contains(p))
                        b = bluebase;
                    else
                        continue;
                    if (GetPlayer(p).tagging)
                        continue;
                    if (OnSide(p, b))
                    {
                        List<Player> temp = redteam.members;
                        if (redteam.members.Contains(p))
                            temp = blueteam.members;
                        foreach (Player p1 in temp)
                        {
                            if (Math.Abs((p1.pos[0] / 32) - (x / 32)) < 5 && Math.Abs((p1.pos[1] / 32) - (y / 32)) < 5 && Math.Abs((p1.pos[2] / 32) - (z / 32)) < 5 && !GetPlayer(p).tagging)
                            {
                                GetPlayer(p1).tagging = true;
                                Player.Message(p1, p.ColoredName + " %Stagged you!");
                                b.SendToSpawn(mainlevel, this, p1);
                                Thread.Sleep(300);
                                if (GetPlayer(p1).hasflag)
                                {
                                    Chat.MessageLevel(mainlevel, redteam.color + p.name + " DROPPED THE FLAG!");
                                    GetPlayer(p1).points -= caplose;
                                    mainlevel.Blockchange(b.x, b.y, b.z, b.block);
                                    GetPlayer(p1).hasflag = false;
                                }
                                GetPlayer(p).points += tagpoint;
                                GetPlayer(p1).points -= taglose;
                                GetPlayer(p).tag++;
                                GetPlayer(p1).tagging = false;
                            }
                        }
                    }
                }
                Thread.Sleep(100);
            }
        }

        void Player_PlayerDisconnect(Player p, string reason)
        {
            if (p.level == mainlevel)
            {
                if (blueteam.members.Contains(p))
                {
                    //cache.Remove(GetPlayer(p));
                    blueteam.members.Remove(p);
                    Chat.MessageLevel(mainlevel, p.ColoredName + " " + blueteam.color + "left the ctf game");
                }
                else if (redteam.members.Contains(p))
                {
                    //cache.Remove(GetPlayer(p));
                    redteam.members.Remove(p);
                    Chat.MessageLevel(mainlevel, p.ColoredName + " " + redteam.color + "left the ctf game");
                }
            }
        }

        void mainlevel_LevelUnload(Level l)
        {
            if (started && l == mainlevel)
            {
                Server.s.Log("Failed!, A ctf game is curretnly going on!");
                Plugin.CancelLevelEvent(LevelEvents.LevelUnload, l);
            }

        }

        static ColumnParams[] createSyntax = {
            new ColumnParams("ID", ColumnType.Integer, priKey: true, autoInc: true, notNull: true),
            new ColumnParams("Name", ColumnType.VarChar, 20),
            new ColumnParams("Points", ColumnType.UInt24),
            new ColumnParams("Captures", ColumnType.UInt24),
            new ColumnParams("tags", ColumnType.UInt24),
        };
        
        /// <summary> Start the CTF game </summary>
        public void Start()
        {
            if (LevelInfo.FindExact("ctf") != null)
            {
                Command.all.Find("unload").Use(null, "ctf");
                Thread.Sleep(1000);
            }
            if (started)
                return;
            blueteam = new Teams("blue");
            redteam = new Teams("red");
            LoadMap(maps[new Random().Next(maps.Count)]);
            if (look)
            {
                for (ushort y = 0; y < mainlevel.Height; y++)
                    for (ushort z = 0; z < mainlevel.Length; z++)
                        for (ushort x = 0; x < mainlevel.Width; x++)
                {
                    if (mainlevel.GetTile(x, y, z) == Block.red)
                    {
                        redbase.x = x; redbase.y = y; redbase.z = z;
                    }
                    else if (mainlevel.GetTile(x, y, z) == Block.blue || mainlevel.GetTile(x, y, z) == Block.cyan)
                    {
                        bluebase.x = x; bluebase.y = y; bluebase.z = z;
                    }
                }
                zline = mainlevel.Length / 2;
            }
            redbase.block = Block.red;
            bluebase.block = Block.blue;
            Server.s.Log("[Auto_CTF] Running...");
            started = true;
            
            Database.Backend.CreateTable("CTF", createSyntax);
        }
        
        string Vote()
        {
            started = false;
            vote1 = 0;
            vote2 = 0;
            vote3 = 0;
            Random rand = new Random();
            List<string> maps1 = maps;
            map1 = maps1[rand.Next(maps1.Count)];
            maps1.Remove(map1);
            map2 = maps1[rand.Next(maps1.Count)];
            maps1.Remove(map2);
            map3 = maps1[rand.Next(maps1.Count)];
            Chat.MessageLevel(mainlevel, "%2VOTE:");
            Chat.MessageLevel(mainlevel, "1. " + map1 + " 2. " + map2 + " 3. " + map3);
            voting = true;
            int seconds = rand.Next(15, 61);
            Chat.MessageLevel(mainlevel, "You have " + seconds + " seconds to vote!");
            Thread.Sleep(seconds * 1000);
            voting = false;
            Chat.MessageLevel(mainlevel, "VOTING ENDED!");
            Thread.Sleep(rand.Next(1, 10) * 1000);
            if (vote1 > vote2 && vote1 > vote3)
            {
                Chat.MessageLevel(mainlevel, map1 + " WON!");
                return map1;
            }
            if (vote2 > vote1 && vote2 > vote3)
            {
                Chat.MessageLevel(mainlevel, map2 + " WON!");
                return map2;
            }
            if (vote3 > vote2 && vote3 > vote1)
            {
                Chat.MessageLevel(mainlevel, map3 + " WON!");
                return map3;
            }
            else
            {
                Chat.MessageLevel(mainlevel, "There was a tie!");
                Chat.MessageLevel(mainlevel, "I'll choose!");
                return maps[rand.Next(maps.Count)];
            }
        }
        void End()
        {
            started = false;
            string nextmap = "";
            string winner = "";
            Teams winnerteam = null;
            if (blueteam.points >= maxpoints || blueteam.points > redteam.points)
            {
                winnerteam = blueteam;
                winner = "blue team";
            }
            else if (redteam.points >= maxpoints || redteam.points > blueteam.points)
            {
                winnerteam = redteam;
                winner = "red team";
            }
            else
            {
                Chat.MessageLevel(mainlevel, "The game ended in a tie!");
            }
            Chat.MessageLevel(mainlevel, "The winner was " + winnerteam.color + winner + "!!");
            Thread.Sleep(4000);
            //MYSQL!
            cache.ForEach(delegate(Data d) {
                d.hasflag = false;
                Database.Backend.UpdateRows("CTF", "Points=@1, Captures=@2, tags=@3", 
                                            "WHERE Name = @0", d.p.name, d.points, d.cap, d.tag);
            });
            nextmap = Vote();
            Chat.MessageLevel(mainlevel, "Starting a new game!");
            redbase = null;
            redteam = null;
            bluebase = null;
            blueteam = null;
            bluebase = new Base();
            redbase = new Base();
            Thread.Sleep(2000);
            LoadMap(nextmap);
        }
        void Player_PlayerBlockChange(Player p, ushort x, ushort y, ushort z, byte type, byte extType)
        {
            if (started)
            {
                if (p.level == mainlevel && !blueteam.members.Contains(p) && !redteam.members.Contains(p))
                {
                    p.RevertBlock(x, y, z);
                    Player.Message(p, "You are not on a team!");
                    Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
                }
                if (p.level == mainlevel && blueteam.members.Contains(p) && x == redbase.x && y == redbase.y && z == redbase.z && mainlevel.GetTile(redbase.x, redbase.y, redbase.z) != Block.air)
                {
                    Chat.MessageLevel(mainlevel, blueteam.color + p.name + " took the " + redteam.color + " red team's FLAG!");
                    GetPlayer(p).hasflag = true;
                }
                if (p.level == mainlevel && redteam.members.Contains(p) && x == bluebase.x && y == bluebase.y && z == bluebase.z && mainlevel.GetTile(bluebase.x, bluebase.y, bluebase.z) != Block.air)
                {
                    Chat.MessageLevel(mainlevel, redteam.color + p.name + " took the " + blueteam.color + " blue team's FLAG");
                    GetPlayer(p).hasflag = true;
                }
                if (p.level == mainlevel && blueteam.members.Contains(p) && x == bluebase.x && y == bluebase.y && z == bluebase.z && mainlevel.GetTile(bluebase.x, bluebase.y, bluebase.z) != Block.air)
                {
                    if (GetPlayer(p).hasflag)
                    {
                        Chat.MessageLevel(mainlevel, blueteam.color + p.name + " RETURNED THE FLAG!");
                        GetPlayer(p).hasflag = false;
                        GetPlayer(p).cap++;
                        GetPlayer(p).points += cappoint;
                        blueteam.points++;
                        mainlevel.Blockchange(redbase.x, redbase.y, redbase.z, Block.red);
                        p.RevertBlock(x, y, z);
                        Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
                        if (blueteam.points >= maxpoints)
                        {
                            End();
                            return;
                        }
                    }
                    else
                    {
                        Player.Message(p, "You cant take your own flag!");
                        p.RevertBlock(x, y, z);
                        Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
                    }
                }
                if (p.level == mainlevel && redteam.members.Contains(p) && x == redbase.x && y == redbase.y && z == redbase.z && mainlevel.GetTile(redbase.x, redbase.y, redbase.z) != Block.air)
                {
                    if (GetPlayer(p).hasflag)
                    {
                        Chat.MessageLevel(mainlevel, redteam.color + p.name + " RETURNED THE FLAG!");
                        GetPlayer(p).hasflag = false;
                        GetPlayer(p).points += cappoint;
                        GetPlayer(p).cap++;
                        redteam.points++;
                        mainlevel.Blockchange(bluebase.x, bluebase.y, bluebase.z, Block.blue);
                        p.RevertBlock(x, y, z);
                        Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
                        if (redteam.points >= maxpoints)
                        {
                            End();
                            return;
                        }
                    }
                    else
                    {
                        Player.Message(p, "You cant take your own flag!");
                        p.RevertBlock(x, y, z);
                        Plugin.CancelPlayerEvent(PlayerEvents.BlockChange, p);
                    }
                }
            }
        }
        internal Data GetPlayer(Player p)
        {
            foreach (Data d in cache)
            {
                if (d.p == p)
                    return d;
            }
            return null;
        }
        void Player_PlayerCommand(string cmd, Player p, string message)
        {
            if (started)
            {
                if (cmd == "teamchat" && p.level == mainlevel)
                {
                    if (GetPlayer(p) != null)
                    {
                        Data d = GetPlayer(p);
                        if (d.chatting)
                        {
                            Player.Message(d.p, "You are no longer chatting with your team!");
                            d.chatting = !d.chatting;
                        }
                        else
                        {
                            Player.Message(d.p, "You are now chatting with your team!");
                            d.chatting = !d.chatting;
                        }
                        Plugin.CancelPlayerEvent(PlayerEvents.PlayerCommand, p);
                    }
                }
                if (cmd == "goto")
                {
                    if (message == "ctf" && p.level != mainlevel)
                    {
                        if (blueteam.members.Count > redteam.members.Count)
                        {
                            if (GetPlayer(p) == null)
                                cache.Add(new Data(false, p));
                            else
                            {
                                GetPlayer(p).hasflag = false;
                                GetPlayer(p).blue = false;
                            }
                            redteam.Add(p);
                            Chat.MessageLevel(mainlevel, p.ColoredName + " " + Colors.red + "joined the RED Team");
                            Player.Message(p, Colors.red + "You are now on the red team!");
                        }
                        else if (redteam.members.Count > blueteam.members.Count)
                        {
                            if (GetPlayer(p) == null)
                                cache.Add(new Data(true, p));
                            else
                            {
                                GetPlayer(p).hasflag = false;
                                GetPlayer(p).blue = true;
                            }
                            blueteam.Add(p);
                            Chat.MessageLevel(mainlevel, p.ColoredName + " " + Colors.blue + "joined the BLUE Team");
                            Player.Message(p, Colors.blue + "You are now on the blue team!");
                        }
                        else if (new Random().Next(2) == 0)
                        {
                            if (GetPlayer(p) == null)
                                cache.Add(new Data(false, p));
                            else
                            {
                                GetPlayer(p).hasflag = false;
                                GetPlayer(p).blue = false;
                            }
                            redteam.Add(p);
                            Chat.MessageLevel(mainlevel, p.ColoredName + " " + Colors.red + "joined the RED Team");
                            Player.Message(p, Colors.red + "You are now on the red team!");
                        }
                        else
                        {
                            if (GetPlayer(p) == null)
                                cache.Add(new Data(true, p));
                            else
                            {
                                GetPlayer(p).hasflag = false;
                                GetPlayer(p).blue = true;
                            }
                            blueteam.Add(p);
                            Chat.MessageLevel(mainlevel, p.ColoredName + " " + Colors.blue + "joined the BLUE Team");
                            Player.Message(p, Colors.blue + "You are now on the blue team!");
                        }
                    }
                    else if (message != "ctf" && p.level == mainlevel)
                    {
                        if (blueteam.members.Contains(p))
                        {
                            //cache.Remove(GetPlayer(p));
                            blueteam.members.Remove(p);
                            Chat.MessageLevel(mainlevel, p.ColoredName + " " + blueteam.color + "left the ctf game");
                        }
                        else if (redteam.members.Contains(p))
                        {
                            //cache.Remove(GetPlayer(p));
                            redteam.members.Remove(p);
                            Chat.MessageLevel(mainlevel, p.ColoredName + " " + redteam.color + "left the ctf game");
                        }
                    }
                }
            }
        }
        void Player_PlayerChat(Player p, string message)
        {
            if (voting)
            {
            	if (message == "1" || message.CaselessEq(map1))
                {
                    Player.Message(p, "Thanks for voting :D");
                    vote1++;
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                }
            	else if (message == "2" || message.CaselessEq(map2))
                {
                    Player.Message(p, "Thanks for voting :D");
                    vote2++;
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                }
            	else if (message == "3" || message.CaselessEq(map3))
                {
                    Player.Message(p, "Thanks for voting :D");
                    vote3++;
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                }
                else
                {
                    Player.Message(p, "%2VOTE:");
                    Player.Message(p, "1. " + map1 + " 2. " + map2 + " 3. " + map3);
                    Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                }
            }
            if (started)
            {
                if (p.level == mainlevel)
                {
                    if (GetPlayer(p).chatting)
                    {
                        if (blueteam.members.Contains(p))
                        {
                        	Player[] online = PlayerInfo.Online.Items; 
                        	foreach (Player p1 in online) {
                                if (blueteam.members.Contains(p1))
                                    Player.Message(p1, "(Blue) " + p.ColoredName + ":&f " + message);
                            }
                            Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                        }
                        if (redteam.members.Contains(p))
                        {
                        	Player[] online = PlayerInfo.Online.Items; 
                        	foreach (Player p1 in online) {
                                if (redteam.members.Contains(p1))
                                    Player.Message(p1, "(Red) " + p.ColoredName + ":&f " + message);
                            }
                            Plugin.CancelPlayerEvent(PlayerEvents.PlayerChat, p);
                        }
                    }
                }
            }
        }
        void Player_PlayerDeath(Player p, byte deathblock)
        {
            if (started)
            {
                if (p.level == mainlevel)
                {
                    if (GetPlayer(p).hasflag)
                    {
                        if (redteam.members.Contains(p))
                        {
                            Chat.MessageLevel(mainlevel, redteam.color + p.name + " DROPPED THE FLAG!");
                            GetPlayer(p).points -= caplose;
                            mainlevel.Blockchange(redbase.x, redbase.y, redbase.z, Block.red);
                        }
                        else if (blueteam.members.Contains(p))
                        {
                            Chat.MessageLevel(mainlevel, blueteam.color + p.name + " DROPPED THE FLAG!");
                            GetPlayer(p).points -= caplose;
                            mainlevel.Blockchange(bluebase.x, bluebase.y, bluebase.z, Block.blue);
                        }
                        GetPlayer(p).hasflag = false;
                    }
                }
            }
        }
        internal bool OnSide(ushort z, Base b)
        {
            if (b.z < zline && z / 32 < zline)
                return true;
            else if (b.z > zline && z / 32 > zline)
                return true;
            else
                return false;
        }
        bool OnSide(Player p, Base b)
        {
            if (b.z < zline && p.pos[2] / 32 < zline)
                return true;
            else if (b.z > zline && p.pos[2] / 32 > zline)
                return true;
            else
                return false;
        }
    }
}
