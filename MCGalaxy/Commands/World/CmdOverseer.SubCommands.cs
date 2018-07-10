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
    public sealed partial class CmdOverseer : Command2 {

        static void HandleBlockProps(Player p, string arg1, string arg2) {
            string args = ("level " + arg1 + " " + arg2).Trim();
            Command.Find("BlockProperties").Use(p, args);
        }
        
        static void HandleEnv(Player p, string type, string value) {
            type = type.ToLower();
            if (type == "preset") {
                Command.Find("Environment").Use(p, "preset " + value); return;
            }
            
            Level lvl = p.level;
            string arg = value.Length == 0 ? "normal" : value;
            
            Predicate<Player> selector = pl => pl.level == lvl;
            if (CmdEnvironment.Handle(p, selector, type, arg, lvl.Config, lvl.ColoredName)) return;
            p.MessageLines(envHelp);
        }

        static void HandleGoto(Player p, string map, string ignored) {
            byte mapNum = 0;
            if (map.Length == 0 || map == "1") {
                map = FirstMapName(p);
            } else {
                if (!byte.TryParse(map, out mapNum)) {
                    p.MessageLines(gotoHelp);
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
            if (name.Length == 0) { p.Message("You must specify a player to kick."); return; }
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;
            
            if (pl.level == p.level) {
                PlayerActions.ChangeMap(pl, Server.mainLevel);
            } else {
                p.Message("Player is not on your level!");
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
            if (mapOnly && !LevelInfo.IsRealmOwner(p.name, p.level.name)) {
                p.Message("You may only perform that action on your own map."); return;
            }
            
            if (IsCreateCommand(cmd)) {
                AddMap(p, value);
            } else if (cmd == "PHYSICS") {
                if (value == "0" || value == "1" || value == "2" || value == "3" || value == "4" || value == "5") {
                    CmdPhysics.SetPhysics(p.level, int.Parse(value));
                } else {
                    p.Message("Accepted numbers are: 0, 1, 2, 3, 4 or 5");
                }
            } else if (IsDeleteCommand(cmd)) {
                DeleteMap(p, value);
            } else if (cmd == "SAVE") {
                Command.Find("Save").Use(p, "");
            } else if (cmd == "RESTORE") {
                Command.Find("Restore").Use(p, value);
            } else if (cmd == "RESIZE") {
                value = p.level.name + " " + value;
                string[] args = value.SplitSpaces();
                if (args.Length < 4) { Command.Find("ResizeLvl").Help(p); return; }

                if (CmdResizeLvl.DoResize(p, args)) return;
                p.Message("Type %T/os map resize {0} {1} {2} confirm %Sif you're sure.",
                               args[1], args[2], args[3]);
            } else if (cmd == "PERVISIT") {
                // Older realm maps didn't put you on visit whitelist, so make sure we put the owner here
                AccessController access = p.level.VisitAccess;
                if (!access.Whitelisted.CaselessContains(p.name)) {
                    access.Whitelist(Player.Console, p.level, p.name);
                }
                
                string rank = value.Length == 0 ? ServerConfig.DefaultRankName : value;
                Group grp = Matcher.FindRanks(p, rank);
                if (grp != null) access.SetMin(Player.Console, p.level, grp);
            } else if (cmd == "PERBUILD") {
                string rank = value.Length == 0 ? ServerConfig.DefaultRankName : value;
                Group grp = Matcher.FindRanks(p, rank);
                if (grp != null) p.level.BuildAccess.SetMin(Player.Console, p.level, grp);
            } else if (cmd == "TEXTURE" || cmd == "TEXTUREZIP" || cmd == "TEXTUREPACK") {
                if (value.Length == 0) value = "normal";
                Command.Find("Texture").Use(p, "levelzip " + value);
            } else {
                LevelOption opt = LevelOptions.Find(cmd);
                if (opt == null) {
                    p.MessageLines(mapHelp);
                } else if (DisallowedMapOption(opt.Name)) {
                    p.Message("%WYou cannot change that map option via /os map."); return;
                } else {
                    opt.SetFunc(p, p.level, value);
                    Level.SaveSettings(p.level);
                }
            }
        }
        
        static bool DisallowedMapOption(string opt) {
            return opt == LevelOptions.Speed || opt == LevelOptions.Overload || opt == LevelOptions.RealmOwner;
        }
        
        static void AddMap(Player p, string value) {
            if (p.group.OverseerMaps == 0) {
                p.Message("Your rank is not allowed to create any /os maps."); return;
            }
            string level = NextLevel(p);
            if (level == null) return;

            if (value.Length == 0) value = "128 128 128 flat";
            else if (value.IndexOf(' ') == -1) value = "128 128 128 " + value;
            
            string[] args = value.TrimEnd().SplitSpaces();
            if (args.Length == 3) value += " flat";

            CmdNewLvl newLvl = (CmdNewLvl)Command.Find("NewLvl"); // TODO: this is a nasty hack, find a better way
            args = (level + " " + value).SplitSpaces();
            Level lvl = newLvl.GenerateMap(p, args);
            if (lvl == null) return;
            
            if (SetPerms(p, lvl)) {
                Group grp = Group.Find(ServerConfig.OSPerbuildDefault);
                p.Message("Use %T/os zone add [name] %Sto allow " +
                               "players ranked below " + grp.ColoredName + " %Sto build in the map.");
            }
            
            try {
                lvl.Save(true);
            } finally {
                lvl.Dispose();
                Server.DoGC();
            }
        }
        
        internal static bool SetPerms(Player p, Level lvl) {
            lvl.Config.RealmOwner = p.name;
            lvl.BuildAccess.Whitelist(Player.Console, lvl, p.name);
            lvl.VisitAccess.Whitelist(Player.Console, lvl, p.name);

            Group grp = Group.Find(ServerConfig.OSPerbuildDefault);
            if (grp == null) return false;
            
            lvl.BuildAccess.SetMin(Player.Console, lvl, grp);
            return true;
        }
        
        static void DeleteMap(Player p, string value) {
            if (value.Length > 0) {
                p.Message("To delete your current map, type %T/os map delete");
                return;
            }
            
            string map = p.level.name;
            p.Message("Created backup.");
            if (LevelActions.Delete(map)) {
                p.Message("Map " + map + " was removed.");
            } else {
                p.Message(LevelActions.DeleteFailedMessage);
            }
        }


        static void HandlePreset(Player p, string preset, string ignored) {
            HandleEnv(p, "preset", preset);
        }

        static void HandleSpawn(Player p, string ignored1, string ignored2) {
            Command.Find("SetSpawn").Use(p, "");
        }
        
        
        static void HandleZone(Player p, string cmd, string name) {
            cmd = cmd.ToUpper();
            if (cmd == "LIST") {
                Command.Find("ZoneList").Use(p, "");
            } else if (cmd == "ADD") {
                if (name.Length == 0) { p.Message("You need to provide a player name."); return; }
                AddBuildPlayer(p, name);
            } else if (IsDeleteCommand(cmd)) {
                if (name.Length == 0) { p.Message("You need to provide a player name."); return; }
                DeleteBuildPlayer(p, name);
            } else if (cmd == "BLOCK") {
                if (name.Length == 0) { p.Message("You need to provide a player name."); return; }
                name = PlayerInfo.FindMatchesPreferOnline(p, name);
                if (name == null) return;
                
                if (name.CaselessEq(p.name)) { p.Message("You can't blacklist yourself"); return; }
                RemoveVisitPlayer(p, name);
            } else if (cmd == "UNBLOCK") {
                if (name.Length == 0) { p.Message("You need to provide a player name."); return; }
                if (!Formatter.ValidName(p, name, "player")) return;
                AddVisitPlayer(p, name);
            } else if (cmd == "BLACKLIST") {
                List<string> blacklist = p.level.VisitAccess.Blacklisted;
                if (blacklist.Count > 0) {
                    p.Message("Blacklisted players: " + blacklist.Join());
                } else {
                    p.Message("There are no blacklisted players on this map.");
                }
            } else {
                p.MessageLines(zoneHelp);
            }
        }
        
        static void AddBuildPlayer(Player p, string rawArgs) {
            string[] args = rawArgs.SplitSpaces();
            string reason = args.Length > 1 ? args[1] : "";
            string name = ModActionCmd.FindName(p, "zone", "os zone add", "", args[0], ref reason);
            if (name == null) return;
            
            p.Message("Added zone for &b" + name);
            LevelAccessController access = p.level.BuildAccess;
            if (access.Blacklisted.CaselessRemove(name)) {
                access.OnListChanged(p, p.level, name, true, true);
            }
            if (!access.Whitelisted.CaselessContains(name)) {
                access.Whitelisted.Add(name);
                access.OnListChanged(p, p.level, name, true, false);
            }
        }
        
        static void DeleteBuildPlayer(Player p, string name) {
            if (!Formatter.ValidName(p, name, "player")) return;
            
            LevelAccessController access = p.level.BuildAccess;
            if (access.Whitelisted.CaselessRemove(name)) {
                access.OnListChanged(p, p.level, name, false, true);
            } else {
                p.Message(name + " was not whitelisted.");
            }
        }
        
        static void AddVisitPlayer(Player p, string name) {
            List<string> blacklist = p.level.VisitAccess.Blacklisted;
            if (!blacklist.CaselessContains(name)) {
                p.Message(name + " is not blacklisted."); return;
            }
            blacklist.CaselessRemove(name);
            p.level.VisitAccess.OnListChanged(p, p.level, name, true, true);
        }
        
        static void RemoveVisitPlayer(Player p, string name) {
            List<string> blacklist = p.level.VisitAccess.Blacklisted;
            if (blacklist.CaselessContains(name)) {
                p.Message(name + " is already blacklisted."); return;
            }
            blacklist.Add(name);
            p.level.VisitAccess.OnListChanged(p, p.level, name, false, false);
        }
        
        static void HandleZones(Player p, string cmd, string args) {
            if (args.Length == 0) {
                p.Message("Arguments required. See %T/Help zone");
            } else {
                Command.Find("Zone").Use(p, cmd + " " + args);
            }
        }
    }
}