/*
    Copyright 2011 MCGalaxy
        
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
using System.Linq;
namespace MCGalaxy.Commands
{
    public sealed class CmdOverseer : Command
    {
        public override string name { get { return "overseer"; } }
        public override string shortcut { get { return "os"; } }
        public override string type { get { return "commands"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdOverseer() { }
        static char[] trimChars = { ' ' };
        
        public override void Use(Player p, string message) {
            if (p.group.OverseerMaps == 0)
                p.SendMessage("Your rank is set to have 0 overseer maps. Therefore, you may not use overseer.");
            if (message == "") { Help(p); return; }
            
            string[] parts = message.Split(' ');        
            string cmd = parts[0].ToUpper();
            string arg = parts.Length > 1 ? parts[1].ToUpper() : "";
            string arg2 = parts.Length > 2 ? parts[2] : "";
            byte mapNum = 0;
            
            bool mapOnly = cmd == "SPAWN" || cmd == "PRESET" || cmd == "WEATHER" || cmd == "ENV" ||
                cmd == "KICK" || cmd == "KICKALL" || cmd == "ZONE" || cmd == "LB" || cmd == "LEVELBLOCK";
            if (mapOnly && !p.level.name.ToUpper().StartsWith(p.name.ToUpper())) {
                Player.SendMessage(p, "You may only perform that action on your own map.");
                return;
            }

            if (cmd == "GO" || cmd == "GOTO") {
                string mapname = null;
                if (arg == "" || arg == "1") {
                    mapname = FirstMapName(p);
                } else {
                    if (!byte.TryParse(arg, out mapNum)) {
                        Help(p); return;
                    }
                    mapname = p.name.ToLower() + arg;
                }
                if (!Server.levels.Any(l => l.name == mapname))
                    Command.all.Find("load").Use(p, mapname);
                Command.all.Find("goto").Use(p, mapname);
            } else if (cmd == "LB" || cmd == "LEVELBLOCK") {
                string[] lbArgs = message.Split(trimChars, 2);
                string lbArg = lbArgs.Length > 1 ? lbArgs[1] : "";
                Command.all.Find("levelblock").Use(p, lbArg);
            } else if (cmd == "SPAWN") {
                Command.all.Find("setspawn").Use(p, "");
            } else if (cmd == "PRESET") {
                Command.all.Find("env").Use(p, "l preset " + arg);
            } else if (cmd == "WEATHER") {
                if (arg == "SUN" || arg == "NORMAL") {
                    Command.all.Find("env").Use(p, "l weather 0");
                } else if (arg == "RAIN") {
                    Command.all.Find("env").Use(p, "l weather 1");
                } else if (arg == "SNOW") {
                    Command.all.Find("env").Use(p, "l weather 2");
                } else {
                    Player.SendMessage(p, "/os weather [sun/rain/snow/normal] -- Changes the weather of your map.");
                }
            } else if (cmd == "ENV") {
                HandleEnvCommand(p, arg, arg2);
            } else if (cmd == "MAP") {
                HandleMapCommand(p, message, arg, arg2);
            } else if (cmd == "ZONE") {
                HandleZoneCommand(p, arg, arg2);
            } else if (cmd == "KICKALL") {
                Player.players.ForEach(
                    delegate(Player pl)
                    {
                        if (pl.level == p.level && pl.name != p.name)
                            Command.all.Find("goto").Use(pl, Server.mainLevel.name);
                    });
            } else if (cmd == "KICK") {
                if (arg == "") {
                    p.SendMessage("You must specify a player to kick.");
                    return;
                }
                
                Player kicked = Player.Find(arg);
                if (kicked == null) {
                    p.SendMessage("Error: Player not found.");
                } else {
                    if (kicked.level.name == p.level.name)
                        Command.all.Find("goto").Use(kicked, Server.mainLevel.name);
                    else
                        p.SendMessage("Player is not on your level!");
                }
            } else {
                Help(p);
            }
        }
        
        void HandleEnvCommand(Player p, string type, string value) {
            if (type == "FOG") {
                if (value.Length != 6 && value != "") {
                    Player.SendMessage(p, "Fog color must be a 6 digit color hex code."); return;
                }
                string col = value == "" ? "-1" : value;
                Command.all.Find("env").Use(p, "l fog " + col);
            } else if (type == "CLOUD") {
                if (value.Length != 6 && value != "") {
                    Player.SendMessage(p, "Clouds color must be a 6 digit color hex code."); return;
                }
                string col = value == "" ? "-1" : value;
                Command.all.Find("env").Use(p, "l clouds " + col);
            } else if (type == "SKY") {
                if (value.Length != 6 && value != "") {
                    Player.SendMessage(p, "Sky color must be a 6 digit color hex code."); return;
                }
                string col = value == "" ? "-1" : value;
                Command.all.Find("env").Use(p, "l sky " + col);
            } else if (type == "SHADOW") {
                if (value.Length != 6 && value != "") {
                    Player.SendMessage(p, "Shadow color must be a 6 digit color hex code."); return;
                }
                string col = value == "" ? "-1" : value;
                Command.all.Find("env").Use(p, "l shadow " + col);
            } else if (type == "SUN") {
                if (value.Length != 6 && value != "") {
                    Player.SendMessage(p, "Sun color must be a 6 digit color hex code."); return;
                }
                string col = value == "" ? "-1" : value;
                Command.all.Find("env").Use(p, "l sun " + col);
            } else if (type == "LEVEL") {
                string level = value == "" ? "normal" : value;
                Command.all.Find("env").Use(p, "l level " + level);
            } else if (type == "HORIZON") {
                string block = value == "" ? "normal" : value;
                Command.all.Find("env").Use(p, "l horizon " + block);
            } else if (type == "BORDER") {
                string block = value == "" ? "normal" : value;
                Command.all.Find("env").Use(p, "l border " + block);
            } else {
                Player.SendMessage(p, "/os env [fog/cloud/sky/shadow/sun] [hex color code] -- Changes env colors of your map");
                Player.SendMessage(p, "/os env level -- Sets the water height of your map");
                Player.SendMessage(p, "/os env horizon -- Sets what block the \"ocean\" shows outside your map");
                Player.SendMessage(p, "/os env border -- Sets what block replaces the bedrock below sea level in your map");
                Player.SendMessage(p, "  Warning: Air,Shrub,Glass,Flowers,Mushroom,Rope,Fire cannot be used for horizon/bedrock.");
                Player.SendMessage(p, "  Note: If no hex or block is given, the default will be used.");
            }
        }
        
        void HandleMapCommand(Player p, string message, string cmd, string value) {
            bool mapOnly = cmd == "PHYSICS" || cmd == "MOTD" || cmd == "GUNS" ||
                cmd == "PERVISIT" || cmd == "TEXTURE";
            if (mapOnly && !p.level.name.ToUpper().StartsWith(p.name.ToUpper())) {
                Player.SendMessage(p, "You may only perform that action on your own map.");
                return;
            }
            byte mapNum = 0;
            
            if (cmd == "ADD") {
                string level = p.name.ToLower();
                if ((File.Exists("levels/" + level + ".lvl")) || (File.Exists("levels/" + level + "00.lvl"))) {
                    for (int i = 2; i < p.group.OverseerMaps + 2; i++) {
                        if (File.Exists("levels/" + p.name.ToLower() + i + ".lvl"))
                            continue;
                        if(i > p.group.OverseerMaps) {
                            p.SendMessage("You have reached the limit for your overseer maps."); return;
                        }
                        level = p.name.ToLower() + i;
                        break;
                    }
                    if (level == p.name.ToLower()) {
                        p.SendMessage("You have reached the limit for your overseer maps."); return;
                    }
                }
                
                if (value == "" || MapGen.IsRecognisedFormat(value)) {
                    string type = value == "" ? "flat" : value;
                    Player.SendMessage(p, "Creating a new map for you: " + level);
                    Command.all.Find("newlvl").Use(p, level + " 128 64 128 " + type);
                } else {
                    Player.SendMessage(p, "Invalid map type was specified.");
                    MapGen.PrintValidFormats(p);
                }
            } else if (cmd == "PHYSICS") {
                if (value == "0" || value == "1" || value == "2" || value == "3" || value == "4" || value == "5")
                    Command.all.Find("physics").Use(p, p.level.name + " " + value);
                else
                    Player.SendMessage(p, "You didn't enter a number! " +
                                       "Please enter one of these numbers: 0, 1, 2, 3, 4, 5");
            } else if (cmd == "DELETE") {
                if (value == "") {
                    Player.SendMessage(p, "To delete one of your maps type /os map delete <map number>");
                } else if (value == "1") {
                    Command.all.Find("deletelvl").Use(p, FirstMapName(p));
                    Player.SendMessage(p, "Map 1 has been removed.");
                } else if (byte.TryParse(value, out mapNum)) {
                    Command.all.Find("deletelvl").Use(p, p.name.ToLower() + value);
                    Player.SendMessage(p, "Map " + value + " has been removed.");
                } else {
                    Help(p);
                }
            } else if (cmd == "SAVE") {
                if (value == "") {
                    Player.SendMessage(p, "To save one of your maps type /os map save <map number>");
                } else if (value == "1") {
                    Command.all.Find("save").Use(p, FirstMapName(p));
                    Player.SendMessage(p, "Map 1 has been saved.");
                } else if (byte.TryParse(value, out mapNum)) {
                    Command.all.Find("save").Use(p, p.name.ToLower() + value);
                    Player.SendMessage(p, "Map " + value + " has been saved.");
                } else {
                    Help(p);
                }
            } else if (cmd == "MOTD") {
                int pos = message.IndexOf("motd ");
                string newMotd = "";
                if (message.Split(' ').Length > 2) newMotd = message.Substring(pos + 5);

                if (newMotd == "") {
                    Command.all.Find("map").Use(p, "motd ignore");
                    p.level.Save();
                    Level.SaveSettings(p.level);
                } else if (newMotd.Length > 30) {
                    Player.SendMessage(p, "Your motd can be no longer than %b30" + Server.DefaultColor + " characters.");
                } else {
                    Command.all.Find("map").Use(p, "motd " + newMotd);
                    p.level.Save();
                    Level.SaveSettings(p.level);
                }
            } else if (cmd == "GUNS") {
                Command.all.Find("allowguns").Use(p, null);
            } else if (cmd == "PERVISIT") {
                string rank = value == "" ? Server.defaultRank : value;
                Command.all.Find("pervisit").Use(p, rank);
            } else if (cmd == "TEXTURE") {
                if (value == "") {
                    Player.SendMessage(p, "Removing current texture.");
                    Command.all.Find("texture").Use(p, "level http://null.png");
                } else if (!value.EndsWith(".png")) {
                    Player.SendMessage(p, "Your texture image must end in .png");
                } else {
                    Command.all.Find("texture").Use(p, "level " + value);
                    Player.SendMessage(p, "Your texture has been updated!");
                }
            } else {
                Player.SendMessage(p, "/os map add [type - default is flat] -- Creates your map");
                Player.SendMessage(p, "/os map physics -- Sets the physics on your map.");
                Player.SendMessage(p, "/os map delete -- Deletes your map");
                Player.SendMessage(p, "/os map save -- Saves your map");
                Player.SendMessage(p, "/os map motd -- Changes the motd of your map");
                Player.SendMessage(p, "/os map guns -- Toggles if guns can be used on your map");
                Player.SendMessage(p, "/os map pervisit %b[default is " + Server.defaultRank + "]" + Server.DefaultColor + " -- Changes the pervisit of you map");
                Player.SendMessage(p, "/os map texture -- Add a texture to your map");
                Player.SendMessage(p, "  Textures: If your URL is too long, use the \"<\" symbol to continue it on another line.");
                Player.SendMessage(p, "  Map Types: Desert, flat, forest, island, mountians, ocean, pixel, empty and space");
                Player.SendMessage(p, "  Motd: If no message is provided, the default message will be used.");
            }
        }
        
        void HandleZoneCommand(Player p, string cmd, string value) {
            if (cmd == "LIST") {
                // List zones on a single block(dont need to touch this :) )
                Command.all.Find("zone").Use(p, "");
            } else if (cmd == "ADD") {
                // Add Zone to your personal map(took a while to get it to work(it was a big derp))
                if (value != "") {
                    Command.all.Find("ozone").Use(p, value);
                    Player.SendMessage(p, value + " has been allowed building on your map.");
                } else {
                    Player.SendMessage(p, "You did not specify a name to allow building on your map.");
                }
            } else if (cmd == "DEL") {
                // I need to add the ability to delete a single zone, I need help!
                if (value == "ALL" || value == "") {
                    Command zone = Command.all.Find("zone");
                    Command click = Command.all.Find("click");
                    zone.Use(p, "del all");
                    click.Use(p, "0 0 0");
                }
            } else if (cmd == "BLOCK") {
                if (value == "") {
                    Player.SendMessage(p, "You did not specify a name to blacklist from your map."); return;
                }
                Player blocked = Player.Find(value);
                if (blocked.name.StartsWith(p.name)) { Player.SendMessage(p, "You can't blacklist yourself"); return; }
                if (blocked == null) { Player.SendMessage(p, "Cannot find player."); return; }
                
                string path = "levels/blacklists/" + p.level.name + ".txt";
                if (File.Exists(path) && File.ReadAllText(path).Contains(blocked.name)) {
                    Player.SendMessage(p, blocked.name + " is already blacklisted."); return;
                }
                EnsureFileExists(path);
                try {
                    using (StreamWriter sw = File.AppendText(path)) {
                        DateTime when = DateTime.Now;
                        sw.WriteLine(when.Day + "." + when.Month + "." + when.Year + ": " + blocked.name + "+");
                    }
                } catch {
                    Server.s.Log("Error saving level blacklist");
                }
                Player.SendMessage(p, blocked.name + " has been blacklisted from your map.");
                if (blocked.level.name == p.level.name) { 
                    Command.all.Find("goto").Use(blocked, Server.mainLevel.name); return; 
                }
            } else if (cmd == "UNBLOCK") {
                if (value == "") {
                    Player.SendMessage(p, "You did not specify a name to blacklist from your map."); return;
                }
                
                string path = "levels/blacklists/" + p.level.name + ".txt";
                EnsureFileExists(path);
                value = value.Replace("+", "");
                if (!File.ReadAllText(path).Contains(value)) {
                    Player.SendMessage(p, value + " is not blacklisted."); return;
                }
                value = value + "+";
                
                try {
                    var oldLines = File.ReadAllLines(path);
                    var newLines = oldLines.Where(line => !line.Contains(value));
                    File.WriteAllLines(path, newLines);
                } catch {
                    Server.s.Log("Error saving level unblock");
                }
                Player.SendMessage(p, value + " has been removed from your map's blacklist.");
            } else if (cmd == "BLACKLIST") {
                string path = "levels/blacklists/" + p.level.name + ".txt";
                if (!File.Exists(path)) {
                    Player.SendMessage(p, "There are no blacklisted players on this map.");
                } else {
                    Player.SendMessage(p, "Current blocked players on level &b" + p.level.name + Server.DefaultColor + ":");
                    string blocked = "";
                    string[] lines = File.ReadAllLines(path);
                    foreach (string line in lines) {
                        string player = line.Split(' ')[1];
                        blocked += player + ", ";
                    }
                    Player.SendMessage(p, blocked);
                }
            } else {
                Player.SendMessage(p, "/os zone add [playername or rank] -- Add a zone for a player or a rank."); ;
                Player.SendMessage(p, "/os zone del [all] -- Deletes all zones.");
                Player.SendMessage(p, "/os zone list -- show active zones on brick.");
                Player.SendMessage(p, "/os zone block - Blacklist a player from joining your map.");
                Player.SendMessage(p, "/os zone unblock - Unblocks a player from your map.");
                Player.SendMessage(p, "/os zone blacklist - Show current blacklisted players.");
                Player.SendMessage(p, "You can only delete all zones for now.");
            }
        }
        
        static void EnsureFileExists(string path) {
            if (!Directory.Exists("levels/blacklists/"))
                Directory.CreateDirectory("levels/blacklists/");
            if (!File.Exists(path))
                File.Create(path).Dispose();
        }

        static string FirstMapName(Player p) {
            /* Returns the proper name of the User Level. By default the User Level will be named
             * "UserName" but was earlier named "UserName00". Therefore the Script checks if the old
             * map name exists before trying the new (and correct) name. All Operations will work with
             * both map names (UserName and UserName00)
             * I need to figure out how to add a system to do this with the players second map.
             */
            if (File.Exists("levels/" + p.name.ToLower() + "00.lvl"))
                return p.name.ToLower() + "00";
            return p.name.ToLower();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/os [command string] - sends command to The Overseer");
            Player.SendMessage(p, "Accepted commands:");
            Player.SendMessage(p, "go, map, spawn, zone, kick, kickall, env, " +
                               "weather, preset, levelblock/lb");
        }
    }
}