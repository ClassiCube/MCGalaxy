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
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.World {
    public sealed partial class CmdOverseer : Command2 {

        static void HandleBlockProps(Player p, string arg1, string arg2) {
            string args = ("level " + arg1 + " " + arg2).Trim();
            UseCommand(p, "BlockProperties", args);
        }
        
        static void HandleEnv(Player p, string type, string value) {
		    Level lvl = p.level;
            if (CmdEnvironment.Handle(p, lvl, type, value, lvl.Config, lvl.ColoredName)) return;
            p.MessageLines(envHelp);
        }

        static void HandleGoto(Player p, string map, string ignored) {
            byte mapNum = 0;
            if (map.Length == 0) map = "1";
            
            if (!byte.TryParse(map, out mapNum)) {
                p.MessageLines(gotoHelp); return;
            }
            map = GetLevelName(p, mapNum);
            
            if (LevelInfo.FindExact(map) == null)
                LevelActions.Load(p, map, !Server.Config.AutoLoadMaps);
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
            CustomBlockCommand.Execute(p, lbArgs, p.DefaultCmdData, false, "/os lb");
        }
        
        
        static void HandleMap(Player p, string cmd, string value) {
            cmd = cmd.ToUpper();
            bool mapOnly = !(cmd.Length == 0 || IsCreateCommand(cmd));
            if (mapOnly && !LevelInfo.IsRealmOwner(p.level, p.name)) {
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
                UseCommand(p, "Save", "");
            } else if (cmd == "RESTORE") {
                UseCommand(p, "Restore", value);
            } else if (cmd == "RESIZE") {
                value = p.level.name + " " + value;
                string[] args = value.SplitSpaces();
                if (args.Length < 4) { Command.Find("ResizeLvl").Help(p); return; }

                bool needConfirm;
                if (CmdResizeLvl.DoResize(p, args, p.DefaultCmdData, out needConfirm)) return;
                
                if (!needConfirm) return;
                p.Message("Type &T/os map resize {0} {1} {2} confirm &Sif you're sure.",
                          args[1], args[2], args[3]);
            } else if (cmd == "PERVISIT") {
                // Older realm maps didn't put you on visit whitelist, so make sure we put the owner here
                AccessController access = p.level.VisitAccess;
                if (!access.Whitelisted.CaselessContains(p.name)) {
                    access.Whitelist(Player.Console, LevelPermission.Nobody, p.level, p.name);
                }
                
                if (value.Length > 0) value = p.level.name + " " + value;
                UseCommand(p, "PerVisit", value);
            } else if (cmd == "PERBUILD") {
                if (value.Length > 0) value = p.level.name + " " + value;
                UseCommand(p, "PerBuild", value);
            } else if (cmd == "TEXTURE" || cmd == "TEXTUREZIP" || cmd == "TEXTUREPACK") {
                if (value.Length == 0) value = "normal";
                UseCommand(p, "Texture", "levelzip " + value);
            } else {
                LevelOption opt = LevelOptions.Find(cmd);
                if (opt == null) {
                    p.MessageLines(mapHelp);
                } else if (DisallowedMapOption(opt.Name)) {
                    p.Message("&WYou cannot change that map option via /os map."); return;
                } else {
                    opt.SetFunc(p, p.level, value);
                    p.level.SaveSettings();
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
            
            Level lvl = newLvl.GenerateMap(p, args, p.DefaultCmdData);
            if (lvl == null) return;
            
            MapGen.SetRealmPerms(p, lvl);
            p.Message("Use &T/os zone add [name] &Sto allow other players to build in the map.");
            
            try {
                lvl.Save(true);
            } finally {
                lvl.Dispose();
                Server.DoGC();
            }
        }
        
        static void DeleteMap(Player p, string value) {
            if (value.Length > 0) {
                p.Message("To delete your current map, type &T/os map delete");
                return;
            }
            UseCommand(p, "DeleteLvl", p.level.name);
        }


        static void HandlePreset(Player p, string preset, string ignored) {
            HandleEnv(p, "preset", preset);
        }

        static void HandleSpawn(Player p, string ignored1, string ignored2) {
            UseCommand(p, "SetSpawn", "");
        }
        
        
        static void HandleZone(Player p, string cmd, string name) {
            cmd = cmd.ToUpper();
            if (cmd == "LIST") {
                UseCommand(p, "ZoneList", "");
            } else if (cmd == "ADD") {
                UseCommand(p, "PerBuild", "+" + name);
            } else if (IsDeleteCommand(cmd)) {
                UseCommand(p, "PerBuild", "-" + name);
            } else if (cmd == "BLOCK") {
                UseCommand(p, "PerVisit", "-" + name);
            } else if (cmd == "UNBLOCK") {
                UseCommand(p, "PerVisit", "+" + name);
            } else if (cmd == "BLACKLIST") {
                List<string> blacklist = p.level.VisitAccess.Blacklisted;
                if (blacklist.Count > 0) {
                    p.Message("Blacklisted players: " + blacklist.Join());
                } else {
                    p.Message("No players are blacklisted from visiting this map.");
                }
            } else {
                p.MessageLines(zoneHelp);
            }
        }
        static void HandleZones(Player p, string cmd, string args) {
            if (args.Length == 0) {
                p.Message("Arguments required. See &T/Help zone");
            } else {
                UseCommand(p, "Zone", cmd + " " + args);
            }
        }
    }
}