/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Commands.CPE;
using MCGalaxy.Commands.World;

namespace MCGalaxy.Commands {
    public sealed class CmdOverseer : Command {
        public override string name { get { return "overseer"; } }
        public override string shortcut { get { return "os"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("realm"), new CommandAlias("myrealm") }; }
        }
        public CmdOverseer() { }
        
        public override void Use(Player p, string message) {
            if (p.group.OverseerMaps == 0)
                p.SendMessage("Your rank is set to have 0 overseer maps. Therefore, you may not use overseer.");
            if (message == "") { Help(p); return; }
            
            string[] parts = message.SplitSpaces(3);
            string cmd = parts[0].ToUpper();
            string arg = parts.Length > 1 ? parts[1].ToUpper() : "";
            string arg2 = parts.Length > 2 ? parts[2] : "";
            byte mapNum = 0;
            
            bool mapOnly = cmd == "SPAWN" || cmd == "PRESET" || cmd == "WEATHER" || cmd == "ENV" ||
                cmd == "KICK" || cmd == "KICKALL" || cmd == "ZONE" || cmd == "LB" || cmd == "LEVELBLOCK";
            if (mapOnly && !OwnsMap(p, p.level)) {
                Player.Message(p, "You may only perform that action on your own map."); return;
            }

            if (cmd == "GO" || cmd == "GOTO" || cmd == "JOIN") {
                string map = null;
                if (arg == "" || arg == "1") {
                    map = FirstMapName(p);
                } else {
                    if (!byte.TryParse(arg, out mapNum)) {
                        Help(p); return;
                    }
                    map = p.name.ToLower() + arg;
                }
                
                Level[] loaded = LevelInfo.Loaded.Items;
                if (LevelInfo.FindExact(map) == null)
                    CmdLoad.LoadLevel(p, map, "0", Server.AutoLoad);
                if (LevelInfo.FindExact(map) != null)
                    PlayerActions.ChangeMap(p, map);
            } else if (cmd == "LB" || cmd == "LEVELBLOCK") {
                string[] lbArgs = message.SplitSpaces(2);
                string lbArg = lbArgs.Length > 1 ? lbArgs[1] : "";
                CustomBlockCommand.Execute(p, lbArg, false, "/os lb");
            } else if (cmd == "SPAWN") {
                Command.all.Find("setspawn").Use(p, "");
            } else if (cmd == "PRESET") {
                Command.all.Find("env").Use(p, "preset " + arg);
            } else if (cmd == "ENV") {
                HandleEnvCommand(p, arg, arg2);
            } else if (cmd == "MAP") {
                HandleMapCommand(p, message, arg, arg2);
            } else if (cmd == "ZONE") {
                HandleZoneCommand(p, arg, arg2);
            } else if (cmd == "KICKALL") {
                Player[] players = PlayerInfo.Online.Items;
                foreach (Player pl in players) {
                    if (pl.level == p.level && pl.name != p.name)
                        PlayerActions.ChangeMap(pl, Server.mainLevel);
                }
            } else if (cmd == "KICK") {
                if (arg == "") { p.SendMessage("You must specify a player to kick."); return; }
                
                Player pl = PlayerInfo.FindMatches(p, arg);
                if (pl != null) {
                    if (pl.level.name == p.level.name)
                        PlayerActions.ChangeMap(pl, Server.mainLevel);
                    else
                        p.SendMessage("Player is not on your level!");
                }
            } else {
                Help(p);
            }
        }
        
        void HandleEnvCommand(Player p, string type, string value) {
            string arg = value == "" ? "normal" : value;
            if (CmdEnvironment.Handle(p, type.ToLower(), arg)) return;
            Player.MessageLines(p, envHelp);
        }
        
        void HandleMapCommand(Player p, string message, string cmd, string value) {
            bool mapOnly = !(cmd == "ADD" || cmd == "DELETE" || cmd == "SAVE");
            if (mapOnly && !OwnsMap(p, p.level)) {
                Player.Message(p, "You may only perform that action on your own map."); return;
            }
            
            if (cmd == "ADD") {
                AddMap(p, value);
            } else if (cmd == "PHYSICS") {
                if (value == "0" || value == "1" || value == "2" || value == "3" || value == "4" || value == "5") {
                    CmdPhysics.SetPhysics(p.level, int.Parse(value));
                } else {
                    Player.Message(p, "Accepted numbers are: 0, 1, 2, 3, 4 or 5");
                }
            } else if (cmd == "DELETE") {
                DeleteMap(p, value);
            } else if (cmd == "SAVE") {
                byte mapNum = 0;
                if (value == "") {
                    Player.Message(p, "To save one of your maps type %T/os map save [map number]");
                } else if (value == "1") {
                    Command.all.Find("save").Use(p, FirstMapName(p));
                    Player.Message(p, "Map 1 has been saved.");
                } else if (byte.TryParse(value, out mapNum)) {
                    Command.all.Find("save").Use(p, p.name.ToLower() + value);
                    Player.Message(p, "Map " + value + " has been saved.");
                } else {
                    Help(p);
                }
            } else if (cmd == "RESTORE") {
                Command.all.Find("restore").Use(p, value);
            } else if (cmd == "PERVISIT") {
                string rank = value == "" ? Server.defaultRank : value;
                Command.all.Find("pervisit").Use(p, rank);
            } else if (cmd == "PERBUILD") {
                string rank = value == "" ? Server.defaultRank : value;
                Command.all.Find("perbuild").Use(p, rank);
            } else if (cmd == "TEXTURE") {
                if (value == "") {
                    Command.all.Find("texture").Use(p, "level normal");
                } else {
                    Command.all.Find("texture").Use(p, "level " + value);
                }
            } else if (cmd == "TEXTUREZIP") {
                if (value == "") {
                    Command.all.Find("texture").Use(p, "levelzip normal");
                } else {
                    Command.all.Find("texture").Use(p, "levelzip " + value);
                }
            } else {
                string opt = LevelOptions.Map(cmd.ToLower());
                if (opt == "physicspeed" || opt == "overload" || opt == "realmowner") {
                    Player.Message(p, "&cYou cannot change that map option via /os map."); return;
                }
                if (CmdMap.SetMapOption(p, p.level, opt, value)) return;
                
                Player.MessageLines(p, mapHelp);
            }
        }
        
        void AddMap(Player p, string value) {
            string level = NextLevel(p);
            if (level == null) return;

            if (value == "") value = "128 64 128 flat";
            else if (value.IndexOf(' ') == -1) value = "128 64 128 " + value;
            
            string[] args = value.TrimEnd().Split(' ');
            if (args.Length == 3) value += " flat";

            CmdNewLvl newLvl = (CmdNewLvl)Command.all.Find("newlvl"); // TODO: this is a nasty hack, find a better way
            if (!newLvl.GenerateMap(p, level + " " + value)) return;
            
            // Set default perbuild permissions
            CmdLoad.LoadLevel(null, level);
            Level lvl = LevelInfo.FindExact(level);
            if (lvl == null) return;
            
            lvl.RealmOwner = p.name;
            Command.all.Find("perbuild").Use(null, lvl.name + " +" + p.name);
            
            LevelPermission osPerm = Server.osPerbuildDefault;
            if (osPerm == LevelPermission.Nobody)
                osPerm = GrpCommands.MinPerm(this);
            Group grp = Group.findPerm(osPerm);
            if (grp == null) return;
            
            Command.all.Find("perbuild").Use(null, lvl.name + " " + grp.name);
            Player.Message(p, "Use %T/os zone add [name] %Sto allow " +
                           "players ranked below " + grp.ColoredName + " %Sto build in the map.");
        }
        
        void DeleteMap(Player p, string value) {
            byte mapNum = 0;
            if (value == "") {
                Player.Message(p, "To delete one of your maps, type %T/os map delete [map number]");
            } else if (value == "1") {
                string map = FirstMapName(p);
                if (!LevelInfo.ExistsOffline(map)) {
                    Player.Message(p, "You don't have a map with that map number."); return;
                }
                
                Player.Message(p, "Created backup.");
                LevelActions.Delete(map);
                Player.Message(p, "Map 1 has been removed.");
            } else if (byte.TryParse(value, out mapNum)) {
                string map = p.name.ToLower() + value;
                if (!LevelInfo.ExistsOffline(map)) {
                    Player.Message(p, "You don't have a map with that map number."); return;
                }
                
                Player.Message(p, "Created backup.");
                LevelActions.Delete(map);
                Player.Message(p, "Map " + value + " has been removed.");
            } else {
                Help(p);
            }
        }
        void HandleZoneCommand(Player p, string cmd, string name) {
            if (cmd == "LIST") {
                Command.all.Find("zone").Use(p, "");
            } else if (cmd == "ADD") {
                if (name == "") {
                    Player.Message(p, "You did not specify a name to allow building on your map."); return;
                }
                
                string[] zoneArgs = name.Split(' ');
                name = zoneArgs[0];
                string reason = zoneArgs.Length > 1 ? zoneArgs[1] : "";
                name = CmdZone.FindZoneOwner(p, "os zone add", name, ref reason);
                if (name == null) return;
                
                CmdZone.ZoneAll(p.level, name);
                Player.Message(p, "Added zone for &b" + name);
                Player.Message(p, name + " has been allowed building on your map.");
            } else if (cmd == "DEL") {
                // TODO: Delete zone by name
                if (name.CaselessEq("ALL") || name == "")
                    CmdZone.DeleteAll(p);
            } else if (cmd == "BLOCK") {
                if (name == "") {
                    Player.Message(p, "You did not specify a name to blacklist from your map."); return;
                }
                Player blocked = PlayerInfo.FindMatches(p, name);
                if (blocked == null) return;
                if (blocked.name.CaselessEq(p.name)) { Player.Message(p, "You can't blacklist yourself"); return; }
                
                List<string> blacklist = p.level.VisitAccess.Blacklisted;
                if (blacklist.CaselessContains(blocked.name)) {
                    Player.Message(p, blocked.name + " is already blacklisted."); return;
                }
                blacklist.Add(blocked.name);
                
                Level.SaveSettings(p.level);
                Player.Message(p, blocked.name + " has been blacklisted from your map.");
                if (blocked.level.name == p.level.name) {
                    PlayerActions.ChangeMap(blocked, Server.mainLevel);
                }
            } else if (cmd == "UNBLOCK") {
                if (name == "") {
                    Player.Message(p, "You did not specify a name to blacklist from your map."); return;
                }
                
                List<string> blacklist = p.level.VisitAccess.Blacklisted;
                if (!blacklist.CaselessContains(name)) {
                    Player.Message(p, name + " is not blacklisted."); return;
                }
                blacklist.CaselessRemove(name);
                
                Level.SaveSettings(p.level);
                Player.Message(p, name + " has been removed from your map's blacklist.");
            } else if (cmd == "BLACKLIST") {
                List<string> blacklist = p.level.VisitAccess.Blacklisted;
                if (blacklist.Count > 0) {
                    Player.Message(p, "Blacklisted players: " + blacklist.Join());
                } else {
                    Player.Message(p, "There are no blacklisted players on this map.");
                }
            } else {
                Player.MessageLines(p, zoneHelp);
            }
        }
        
        
        static string NextLevel(Player p) {
            string level = p.name.ToLower();
            if (LevelInfo.ExistsOffline(level) || LevelInfo.ExistsOffline(level + "00")) {
                for (int i = 2; i < p.group.OverseerMaps + 2; i++) {
                    if (LevelInfo.ExistsOffline(p.name.ToLower() + i)) continue;
                    if(i > p.group.OverseerMaps) {
                        p.SendMessage("You have reached the limit for your overseer maps."); return null;
                    }
                    return p.name.ToLower() + i;
                }
                if (level == p.name.ToLower()) {
                    p.SendMessage("You have reached the limit for your overseer maps."); return null;
                }
            }
            return level;
        }

        static string FirstMapName(Player p) {
            /* Returns the proper name of the User Level. By default the User Level will be named
             * "UserName" but was earlier named "UserName00". Therefore the Script checks if the old
             * map name exists before trying the new (and correct) name. All Operations will work with
             * both map names (UserName and UserName00)
             * I need to figure out how to add a system to do this with the players second map.
             */
            if (LevelInfo.ExistsOffline(p.name.ToLower() + "00"))
                return p.name.ToLower() + "00";
            return p.name.ToLower();
        }
        
        static bool OwnsMap(Player p, Level lvl) {
            if (lvl.name.CaselessStarts(p.name)) return true;
            string[] owners = lvl.RealmOwner.Replace(" ", "").Split(',');
            
            foreach (string owner in owners) {
                if (owner.CaselessEq(p.name)) return true;
            }
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/os [command] [args]");
            Player.Message(p, "%HAllows you to modify and manage your personal realms.");
            Player.Message(p, "%HCommands: %Sgo, map, spawn, zone, kick, " +
                           "kickall, env, preset, levelblock(lb)");
            Player.Message(p, "%T/os zone add [name] %H- allows [name] to build in the world.");
        }
        
        #region Help messages
        
        static string[] envHelp = {
            "%T/os env [fog/cloud/sky/shadow/sun] [hex color] %H- Changes env colors of your map.",
            "%T/os env level [height] %H- Sets the water height of your map.",
            "%T/os env cloudheight [height] %H-Sets cloud height of your map.",
            "%T/os env maxfog %H- Sets the max fog distance in your map.",
            "%T/os env horizon %H- Sets the \"ocean\" block outside your map.",
            "%T/os env border %H- Sets the \"bedrock\" block outside your map.",
            "%T/os env weather [sun/rain/snow] %H- Sets weather of your map.",
            " Note: Shrub, flowers, mushrooms, rope, fire cannot be used for horizon/bedrock.",
            " Note: If no hex or block is given, the default will be used.",
        };
        
        static string[] mapHelp = {
            "%T/os map add [type - default is flat] %H- Creates your map (128x64x128)",
            "%T/os map add [width] [height] [length] [type]",
            "%H  See %T/help newlvl types %Hfor a list of map types.",
            "%T/os map physics [level] %H- Sets the physics on your map.",
            "%T/os map delete %H- Deletes your map",
            "%T/os map restore [num] %H- Restores backup [num] of your map",
            "%T/os map save %H- Saves your map",
            "%T/os map pervisit [rank] %H- Sets the pervisit of you map",
            "%T/os map perbuild [rank] %H- Sets the perbuild of you map",
            "%T/os map texture [url] %H- Sets terrain.png for your map",
            "%T/os map texturezip [url] %H- Sets texture .zip for your map",
            "%T/os map [option] <value> %H- Toggles that map option.",
            "%H  See %T/help map %Hfor a list of map options",
        };
        
        static string[] zoneHelp = {
            "%T/os zone add [player/rank] %H- Adds a zone for a player or a rank, " +
                "allowing them to always build in your map.",
            "%T/os zone del all %H- Deletes all zones in your map.",
            "%T/os zone list %H- Shows zones affecting a particular block.",
            "%T/os zone block [name] %H- Prevents them from joining your map.",
            "%T/os zone unblock [name] %H- Allows them to join your map.",
            "%T/os zone blacklist %H- Shows currently blacklisted players.",
        };
        #endregion
    }
}