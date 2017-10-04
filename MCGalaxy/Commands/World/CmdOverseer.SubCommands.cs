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
using MCGalaxy.Commands.Moderation;

namespace MCGalaxy.Commands.World {
    public sealed partial class CmdOverseer : Command {

        static void HandleBlockProps(Player p, string arg1, string arg2) {
            string args = ("level " + arg1 + " " + arg2).Trim();
            Command.all.FindByName("BlockProperties").Use(p, args);
        }
        
        static void HandleEnv(Player p, string type, string value) {
            string arg = value.Length == 0 ? "normal" : value;
            if (CmdEnvironment.Handle(p, type.ToLower(), arg)) return;
            Player.MessageLines(p, envHelp);
        }

        static void HandleGoto(Player p, string map, string ignored) {
            byte mapNum = 0;
            if (map.Length == 0 || map == "1") {
                map = FirstMapName(p);
            } else {
                if (!byte.TryParse(map, out mapNum)) {
                    Player.MessageLines(p, gotoHelp);
                    return;
                }
                map = p.name.ToLower() + map;
            }
            
            if (LevelInfo.FindExact(map) == null)
                CmdLoad.LoadLevel(p, map, ServerConfig.AutoLoadMaps);
            if (LevelInfo.FindExact(map) != null)
                PlayerActions.ChangeMap(p, map);
        }
        
        static void HandleKick(Player p, string name, string ignored) {
            if (name.Length == 0) { Player.Message(p, "You must specify a player to kick."); return; }
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;
            
            if (pl.level == p.level) {
                PlayerActions.ChangeMap(pl, Server.mainLevel);
            } else {
                Player.Message(p, "Player is not on your level!");
            }
        }
        
        static void HandleKickAll(Player p, string ignored1, string ignored2) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == p.level && pl != p)
                    PlayerActions.ChangeMap(pl, Server.mainLevel);
            }
        }
        
        static void HandleLevelBlock(Player p, string arg1, string arg2) {
            string lbArgs = (arg1 + " " + arg2).Trim();
            CustomBlockCommand.Execute(p, lbArgs, false, "/os lb");
        }
        
        
        static void HandleMap(Player p, string cmd, string value) {
            cmd = cmd.ToUpper();
            bool mapOnly = !(cmd == "ADD" || cmd.Length == 0);
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
            } else if (cmd == "DELETE" || cmd == "REMOVE") {
                DeleteMap(p, value);
            } else if (cmd == "SAVE") {
                Command.all.FindByName("Save").Use(p, "");
                Player.Message(p, "Map has been saved.");
            } else if (cmd == "RESTORE") {
                Command.all.FindByName("Restore").Use(p, value);
            } else if (cmd == "RESIZE") {
                value = p.level.name + " " + value;
                string[] args = value.SplitSpaces();
                if (args.Length < 4) { Command.all.FindByName("ResizeLvl").Help(p); return; }

                if (CmdResizeLvl.DoResize(p, args)) return;
                Player.Message(p, "Type %T/os map resize {0} {1} {2} confirm %Sif you're sure.",
                               args[1], args[2], args[3]);
            } else if (cmd == "PERVISIT") {
                string rank = value.Length == 0 ? ServerConfig.DefaultRankName : value;
                Group grp = Matcher.FindRanks(p, rank);
                if (grp != null) p.level.VisitAccess.SetMin(null, grp);
            } else if (cmd == "PERBUILD") {
                string rank = value.Length == 0 ? ServerConfig.DefaultRankName : value;
                Group grp = Matcher.FindRanks(p, rank);
                if (grp != null) p.level.BuildAccess.SetMin(null, grp);
            } else if (cmd == "TEXTURE") {
                if (value.Length == 0) {
                    Command.all.FindByName("Texture").Use(p, "level normal");
                } else {
                    Command.all.FindByName("Texture").Use(p, "level " + value);
                }
            } else if (cmd == "TEXTUREZIP") {
                if (value.Length == 0) {
                    Command.all.FindByName("Texture").Use(p, "levelzip normal");
                } else {
                    Command.all.FindByName("Texture").Use(p, "levelzip " + value);
                }
            } else {
                cmd = LevelOptions.Map(cmd.ToLower());
                if (cmd == "physicspeed" || cmd == "overload" || cmd == "realmowner") {
                    Player.Message(p, "&cYou cannot change that map option via /os map."); return;
                }
                if (CmdMap.SetMapOption(p, p.level, cmd, value)) return;
                
                Player.MessageLines(p, mapHelp);
            }
        }
        
        static void AddMap(Player p, string value) {
            if (p.group.OverseerMaps == 0) {
                Player.Message(p, "Your rank is not allowed to create any /os maps."); return;
            }
            string level = NextLevel(p);
            if (level == null) return;

            if (value.Length == 0) value = "128 64 128 flat";
            else if (value.IndexOf(' ') == -1) value = "128 64 128 " + value;
            
            string[] args = value.TrimEnd().SplitSpaces();
            if (args.Length == 3) value += " flat";

            CmdNewLvl newLvl = (CmdNewLvl)Command.all.FindByName("NewLvl"); // TODO: this is a nasty hack, find a better way
            args = (level + " " + value).SplitSpaces();
            Level lvl = newLvl.GenerateMap(p, args);
            if (lvl == null) return;
            SetPerms(p, lvl);
            
            try {
                lvl.Save(true);
            } finally {
                lvl.Dispose();
                Server.DoGC();
            }
        }
        
        static void SetPerms(Player p, Level lvl) {
            lvl.Config.RealmOwner = p.name;
            lvl.BuildAccess.Whitelist(null, p.name);
            lvl.VisitAccess.Whitelist(null, p.name);
            CmdZone.ZoneAll(lvl, p.name);
            
            LevelPermission osPerm = ServerConfig.OSPerbuildDefault;
            if (osPerm == LevelPermission.Nobody)
                osPerm = CommandPerms.MinPerm(Command.all.Find("overseer"));
            
            Group grp = Group.Find(osPerm);
            if (grp == null) return;
            
            lvl.BuildAccess.SetMin(null, grp);
            Player.Message(p, "Use %T/os zone add [name] %Sto allow " +
                           "players ranked below " + grp.ColoredName + " %Sto build in the map.");
        }
        
        static void DeleteMap(Player p, string value) {
            if (value.Length > 0) {
                Player.Message(p, "To delete your current map, type %T/os map delete");
                return;
            }
            
            string map = p.level.name;
            if (!OwnsMap(p, p.level)) return;
            
            Player.Message(p, "Created backup.");
            LevelActions.Delete(map);
            Player.Message(p, "Map " + map + " was removed.");
        }


        static void HandlePreset(Player p, string preset, string ignored) {
            Command.all.FindByName("Environment").Use(p, "preset " + preset);
        }

        static void HandleSpawn(Player p, string ignored1, string ignored2) {
            Command.all.FindByName("SetSpawn").Use(p, "");
        }
        
        
        static void HandleZone(Player p, string cmd, string name) {
            cmd = cmd.ToUpper();
            if (cmd == "LIST") {
                Command.all.FindByName("Zone").Use(p, "");
            } else if (cmd == "ADD") {
                if (name.Length == 0) { Player.Message(p, "You need to provide a player name."); return; }
                AddBuildPlayer(p, name);
            } else if (cmd == "DEL") {
                if (name.Length == 0) { Player.Message(p, "You need to provide a player name, or \"ALL\"."); return; }
                DeleteBuildPlayer(p, name);
            } else if (cmd == "BLOCK") {
                if (name.Length == 0) { Player.Message(p, "You need to provide a player name."); return; }
                name = PlayerInfo.FindMatchesPreferOnline(p, name);
                if (name == null) return;
                
                if (name.CaselessEq(p.name)) { Player.Message(p, "You can't blacklist yourself"); return; }
                RemoveVisitPlayer(p, name);
            } else if (cmd == "UNBLOCK") {
                if (name.Length == 0) { Player.Message(p, "You need to provide a player name."); return; }
                if (!Formatter.ValidName(p, name, "player")) return;
                AddVisitPlayer(p, name);
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
        
        static void AddBuildPlayer(Player p, string name) {
            string[] zoneArgs = name.SplitSpaces();
            name = zoneArgs[0];
            string reason = zoneArgs.Length > 1 ? zoneArgs[1] : "";
            name = CmdZone.FindZoneOwner(p, "os zone add", name, ref reason);
            if (name == null) return;
            
            CmdZone.ZoneAll(p.level, name);
            Player.Message(p, "Added zone for &b" + name);

            LevelAccessController access = p.level.BuildAccess;
            if (access.Blacklisted.CaselessRemove(name)) {
                access.OnListChanged(p, name, true, true);
            }
            if (!access.Whitelisted.CaselessContains(name)) {
                access.Whitelisted.Add(name);
                access.OnListChanged(p, name, true, false);
            }
        }
        
        static void DeleteBuildPlayer(Player p, string name) {
            if (name.CaselessEq("all")) {
                CmdZone.DeleteAll(p);
            } else if (Formatter.ValidName(p, name, "player")) {
                CmdZone.DeleteWhere(p, zone => zone.Owner.CaselessEq(name));
                LevelAccessController access = p.level.BuildAccess;
                if (access.Whitelisted.CaselessRemove(name)) {
                    access.OnListChanged(p, name, false, true);
                }
            }
        }
        
        static void AddVisitPlayer(Player p, string name) {
            List<string> blacklist = p.level.VisitAccess.Blacklisted;
            if (!blacklist.CaselessContains(name)) {
                Player.Message(p, name + " is not blacklisted."); return;
            }
            blacklist.CaselessRemove(name);
            p.level.VisitAccess.OnListChanged(p, name, true, true);
        }
        
        static void RemoveVisitPlayer(Player p, string name) {
            List<string> blacklist = p.level.VisitAccess.Blacklisted;
            if (blacklist.CaselessContains(name)) {
                Player.Message(p, name + " is already blacklisted."); return;
            }
            blacklist.Add(name);
            p.level.VisitAccess.OnListChanged(p, name, false, false);
        }
    }
}