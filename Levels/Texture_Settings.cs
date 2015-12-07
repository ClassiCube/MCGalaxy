/*
	Copyright 2011 MCGalaxy
		
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
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
namespace MCGalaxy.Levels.Textures
{
    public sealed class LevelTextures
    {
        #region ==VARS==
        string cachecfg = "";
        public Group LowestRank { get { return lowest; } }
        Group lowest = Group.Find(Server.defaultRank);
        bool update = false;
        public bool autou = true;
        public bool enabled = false;
        public string terrainid = "";
        private string edgeid = "";
        public string side = "";
        private int level = -1;
        private int cloudcolor = -1;
        private int fog = -1;
        private int sky = -1;
        public bool sendwomid = true;
        public string servername = Server.name;
        public string MOTD = Server.motd;
        public string detail = "";
        private Level l;
        #endregion


        #region ==OBJECT==
        public LevelTextures(Level level) { 
            l = level;
            if (File.Exists("extra/cfg/" + l.name + ".cfg"))
                cachecfg = GetCFG();
            if (!Directory.Exists("extra/cfg/internal")) Directory.CreateDirectory("extra/cfg/internal");
            if (File.Exists("extra/cfg/" + l.name + ".internal"))
                LoadSettings();
        }
        #endregion


        #region ==SERVER-TO-WOM==
        public void SendDetail(Player p, string text = "")
        {
            byte[] buffer = new byte[65];
            if (p.level == l && p.UsingWom)
            {
                if (text == "")
                {
                    Player.StringFormat(detail, 64).CopyTo(buffer, 1);
                    p.SendRaw(Opcode.Message, buffer);
                }
                else if (!text.StartsWith("^"))
                {
                    Player.StringFormat("^detail.user=" + text, 64).CopyTo(buffer, 1);
                    p.SendRaw(Opcode.Message, buffer);
                }
                else
                {
                    Player.StringFormat(text, 64).CopyTo(buffer, 1);
                    p.SendRaw(Opcode.Message, buffer);
                }
            }
            buffer = null;
        }
        public void GlobalSendDetail(string text = "")
        {
            Player.players.ForEach(p => SendDetail(p, text));
        }
        #endregion


        #region ==SETTINGS==
        public void SetGroup(string name) { lowest = Group.Find(name); if (lowest == null) lowest = Group.Find(Server.defaultRank); }
        public void ChangeLevel(int size) { if (size == 0) level = -1; else level = size; }
        public void ChangeDetail(string text) { detail = "^detail.user=" + text; }
        public void ChangeSky(string hex) { sky = ParseHexColor(hex); }
        public void ChangeFog(string hex) { fog = ParseHexColor(hex); }
        public void ChangeCloud(string hex) { cloudcolor = ParseHexColor(hex); }
        public void ChangeEdge(byte b) { edgeid = GetBlockTexture(b); }
        public void ChangeEdge(string id) { edgeid = id; }
        #endregion


        #region ==INTERNAL SETTINGS==
        #region ==CONFIG==
        public void SaveSettings()
        {
            File.WriteAllLines("extra/cfg/" + l.name + ".internal", new[] { "USE:" + enabled, "AUTOU:" + autou, "GROUP:" + lowest.name });
        }
        public void LoadSettings()
        {
            string[] lines = File.ReadAllLines("extra/cfg/" + l.name + ".internal");
            foreach (string ll in lines)
            {
                string setting = ll.Split(':')[0];
                string value = ll.Split(':')[1];
                switch (setting)
                {
                    case "USE":
                        enabled = bool.Parse(value);
                        break;
                    case "AUTOU":
                        autou = bool.Parse(value);
                        break;
                    case "GROUP":
                        lowest = Group.Find(value) ?? Group.Find(Server.defaultRank);
                        break;
                }
            }
            if (lowest == null)
                lowest = Group.Find(Server.defaultRank);
            SaveSettings();
        }
        #endregion


        #region ==GET==
        static int ParseHexColor(string text)
        {
            return int.Parse((text[0] == '#') ? text.Substring(1) : text, System.Globalization.NumberStyles.HexNumber);
        }
        static string ParseDecColor(int dec)
        {
            return dec.ToString("X");
        }
        public static byte GetBlock(string texture)
        {
            switch (texture)
            {
                case "4a781d8aaabca8eeab78e14a072a905816f201bf":
                    return Block.water;
                case "d0b6dc6d40dd3141fd58dc0aff7370fd623899df":
                    return Block.lava;
                case  "eea1b7e0a62d90b5b681f142bd2f483a671ba160":
                    return Block.blue;
                case "b4a23c66dc4ba488a97becd62f2bae8d61eb8ad2":
                    return Block.brick;
                case  "1f9eb8aff893a43860fcd1f9c1e7ef84e0bfd77b":
                    return Block.coal;
                case "b4d9c39d00102f1b3b67c9e885b62cb8e27efd03":
                    return Block.rock;
                case "2532a657b5525ad10a0ccab78bd4343d44a0bfb7":
                    return Block.cyan;
                case "e35227f0b78041e45523c3bf250f4922e82585e2":
                    return Block.dirt;
                case "1f61ef253653b9cd8f98a92922b3dbf50d939d09":
                    return Block.goldrock;
                case "7e2a41d578bde6fc253863ccc9a25eb099ff6daf":
                    return Block.goldsolid;
                case "1acfce7a8cd70b8ca6047b66a5734e9a3c1d737d":
                    return Block.glass;
                case "1cf2d2b250184516b22f351fa804c243d3ed64fe":
                    return Block.lightgrey;
                case "06f5ba518c5f943f14adf09cc257674e43d8133c":
                    return Block.darkgrey;
                case "e61083cd5396f207267391d5a1f0491c1ce6d404":
                    return Block.gravel;
                case "8f4be9678eb1b6cc4175ff7f45b78fc9f0d76962":
                    return Block.green;
                case "6ec104eba32c595dd7c8c08bb99c422e0e2fc1b7":
                    return Block.iron;
                case "6b8ad341eb0f3209e67f4a1723ca8994f9517fae":
                    return Block.ironrock;
                case "b6e1831c9b30d4e6f7012dd8b2f39e1150ef67fb":
                    return Block.lightgreen;
                case "f3a13b17c5d906d165581c019b2a44eddd0ad5b7":
                    return Block.trunk;
                case "eaecd6ec9c24ed8a2c20ffb10e83409f04409ddd":
                    return Block.sponge;
                case "9106fb8ac7a4eb6f30ce28921f071e6b31bdd74b":
                    return Block.staircasestep;
                case "c2eaac7631e184e4e7f6eeca4c4d6a74f6d953f9":
                    return Block.stone;
                case "7314851e18cdfe9dd1513f9eab86901221421239":
                    return Block.tnt;
                case "182bf0fe9cf4476a573df4f470ac1b7e55936543":
                    return Block.stonevine;
                case "73963ffce5d7d845eb3216a6766655fc405b473c":
                    return Block.obsidian;
                case "cfd84200707e41556d1bb0ace3ca37c69b51cc54":
                    return Block.orange;
                case "19fcc81e8204de91fdbfdc2b59cffe0bfb2ba823":
                    return Block.pink;
                case "be9c5e2ff1d4bbfcd0826c04db5684359acecf28":
                    return Block.red;
                case "1a2dda7ed25ad5e94da4c6a0ac7e63f4a9a72590":
                    return Block.sand;
                case "7abdd25d9229087f29655a1974aed01cbd3eb753":
                    return Block.blackrock;
                case "a171372d9fca63df911485602a5120fd5422f2b9":
                    return Block.purple;
                case "2d9077489d1d86217c89685b12c5a206b23b976f":
                    return Block.white;
                case "af65cd0d0756d357a1abd5390b8de2e5ad1f29af":
                    return Block.wood;
                case "eff6823a987deb65ad21020a3151bb809d3d062c":
                    return Block.yellow;
                default:
                    return Block.Zero;
            }
        }
        public static string GetBlockTexture(byte b)
        {
            switch (b)
            {
                case Block.water:
                    return "4a781d8aaabca8eeab78e14a072a905816f201bf";
                case Block.lava:
                    return "d0b6dc6d40dd3141fd58dc0aff7370fd623899df";
                case Block.blue:
                    return "eea1b7e0a62d90b5b681f142bd2f483a671ba160";
                case Block.brick:
                    return "b4a23c66dc4ba488a97becd62f2bae8d61eb8ad2";
                case Block.coal:
                    return "1f9eb8aff893a43860fcd1f9c1e7ef84e0bfd77b";
                case Block.rock:
                    return "b4d9c39d00102f1b3b67c9e885b62cb8e27efd03";
                case Block.cyan:
                    return "2532a657b5525ad10a0ccab78bd4343d44a0bfb7";
                case Block.dirt:
                    return "e35227f0b78041e45523c3bf250f4922e82585e2";
                case Block.goldrock:
                    return "1f61ef253653b9cd8f98a92922b3dbf50d939d09";
                case Block.goldsolid:
                    return "7e2a41d578bde6fc253863ccc9a25eb099ff6daf";
                case Block.glass:
                    return "1acfce7a8cd70b8ca6047b66a5734e9a3c1d737d";
                case Block.lightgrey:
                    return "1cf2d2b250184516b22f351fa804c243d3ed64fe";
                case Block.darkgrey:
                    return "06f5ba518c5f943f14adf09cc257674e43d8133c";
                case Block.gravel:
                    return "e61083cd5396f207267391d5a1f0491c1ce6d404";
                case Block.green:
                    return "8f4be9678eb1b6cc4175ff7f45b78fc9f0d76962";
                case Block.iron:
                    return "6ec104eba32c595dd7c8c08bb99c422e0e2fc1b7";
                case Block.ironrock:
                    return "6b8ad341eb0f3209e67f4a1723ca8994f9517fae";
                case Block.lightgreen:
                    return "b6e1831c9b30d4e6f7012dd8b2f39e1150ef67fb";
                case Block.trunk:
                    return "f3a13b17c5d906d165581c019b2a44eddd0ad5b7";
                case Block.sponge:
                    return "eaecd6ec9c24ed8a2c20ffb10e83409f04409ddd";
                case Block.staircasestep:
                case Block.staircasefull:
                    return "9106fb8ac7a4eb6f30ce28921f071e6b31bdd74b";
                case Block.stone:
                    return "c2eaac7631e184e4e7f6eeca4c4d6a74f6d953f9";
                case Block.tnt:
                    return "7314851e18cdfe9dd1513f9eab86901221421239";
                case Block.stonevine:
                    return "182bf0fe9cf4476a573df4f470ac1b7e55936543";
                case Block.obsidian:
                    return "73963ffce5d7d845eb3216a6766655fc405b473c";
                case Block.orange:
                    return "cfd84200707e41556d1bb0ace3ca37c69b51cc54";
                case Block.pink:
                    return "19fcc81e8204de91fdbfdc2b59cffe0bfb2ba823";
                case Block.red:
                    return "be9c5e2ff1d4bbfcd0826c04db5684359acecf28";
                case Block.sand:
                    return "1a2dda7ed25ad5e94da4c6a0ac7e63f4a9a72590";
                case Block.blackrock:
                    return "7abdd25d9229087f29655a1974aed01cbd3eb753";
                case Block.purple:
                    return "a171372d9fca63df911485602a5120fd5422f2b9";
                case Block.white:
                    return "2d9077489d1d86217c89685b12c5a206b23b976f";
                case Block.wood:
                    return "af65cd0d0756d357a1abd5390b8de2e5ad1f29af";
                case Block.yellow:
                    return "eff6823a987deb65ad21020a3151bb809d3d062c";
                default:
                    return "";
            }
        }
        #endregion
        
        
        #region ==CFG==
        public string[] GetCFGLines()
        {
            return File.ReadAllLines("extra/cfg/" + l.name + ".cfg");
        }
        public string GetCFG()
        {
            return File.ReadAllText("extra/cfg/" + l.name + ".cfg");
        }
        //Create CFG
        public void CreateCFG()
        {
            if (l.motd == "ignore")
                MOTD = Server.motd;
            else
                MOTD = l.motd;
            List<string> temp = new List<string>();
            if (terrainid != "")
                temp.Add("environment.terrain = " + terrainid);
            if (edgeid != "")
                temp.Add("environment.edge = " + edgeid);
            if (side != "")
                temp.Add("environment.side = " + side);
            if (level != -1)
                temp.Add("environment.level = " + level);
            if (cloudcolor != -1)
                temp.Add("environment.cloud = " + cloudcolor);
            if (fog != -1)
                temp.Add("environment.fog = " + fog);
            if (sky != -1)
                temp.Add("environment.sky = " + sky);
            if (servername != "")
                temp.Add("server.name = " + servername);
            if (MOTD != "")
                temp.Add("server.detail = " + MOTD);
            if (detail != "")
                temp.Add("detail.user = " + detail);
            temp.Add("server.sendwomid = " + sendwomid.ToString().ToLower());
            if (!Directory.Exists("extra/cfg")) Directory.CreateDirectory("extra/cfg");
            File.WriteAllLines("extra/cfg/" + l.name + ".cfg", temp.ToArray());
            temp.Clear();
            Server.s.Log("CFG File created for " + l.name);
            update = true;
            if (autou && enabled)
            {
                Player.players.ForEach(delegate(Player p1)
                {
                    if (p1.level == l)
                        Command.all.Find("reveal").Use(p1, p1.name);
                });
            }
            Player.GlobalMessage("Level textures updated for " + l.name);
        }
        #endregion
        #endregion


        #region Thanks FCRAFT
        //All credit for the code below goes to fcraft
        //Thanks fcraft...your awesome :D
        //Modified for use in MCGalaxy
        #region ==SERVER-TO-WOM==
        static readonly Regex HttpFirstLine = new Regex("GET /([a-zA-Z0-9_]{1,16})(~motd)? .+", RegexOptions.Compiled);
        public void ServeCfg(Player p, byte[] buffer)
        {
            var stream = new NetworkStream(p.socket);
            using (var reader = new StreamReader(stream))
            {
                using (var textWriter = new StreamWriter(stream, Encoding.ASCII))
                {
                    string firstLine = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
                    var match = HttpFirstLine.Match(firstLine);
                    if (match.Success)
                    {
                    }
                    else
                        textWriter.WriteLine("HTTP/1.1 400 Bad Request");
                    p.dontmindme = true;
                }
            }
            stream.Close();
            stream.Dispose();
            p.socket.Close();
            p.disconnected = true;
        }
        private bool isMusic(string s, Player p) {
            foreach (string file in p.sounds.Keys) {
                if (s == file)
                    return true;
            }
            return false;
        }
        private byte[] getData(string s) {
            if (!File.Exists(s))
                return new byte[0];
            return File.ReadAllBytes(s);
        }
        #endregion
        #endregion
    }
}
