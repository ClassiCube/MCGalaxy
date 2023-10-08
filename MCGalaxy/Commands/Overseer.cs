/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using MCGalaxy.Commands.CPE;
using MCGalaxy.Generator;

namespace MCGalaxy.Commands.World {

    /// <summary>
    /// Implements and manages behavior that can be called from CmdOverseer
    /// </summary>
    public static class Overseer {

        public static readonly string commandShortcut = "os";
        public static void RegisterSubCommand(SubCommand subCmd) {
            subCommandGroup.Register(subCmd);
        }

        public static void UnregisterSubCommand(SubCommand subCmd) {
            subCommandGroup.Unregister(subCmd);
        }

        static void UseCommand(Player p, string cmd, string args) {
            CommandData data = default(CommandData);
            data.Rank = LevelPermission.Owner;
            Command.Find(cmd).Use(p, args, data);
        }

        static string GetLevelName(Player p, int i) {
            string name = p.name.ToLower();
            return i == 1 ? name : name + i;
        }

        static string NextLevel(Player p) {
            int realms = p.group.OverseerMaps;

            for (int i = 1; realms > 0; i++) {
                string map = GetLevelName(p, i);
                if (!LevelInfo.MapExists(map)) return map;

                if (LevelInfo.IsRealmOwner(p.name, map)) realms--;
            }
            p.Message("You have reached the limit for your overseer maps."); return null;
        }

        #region Help messages

        static string[] blockPropsHelp = new string[] {
            "&T/os blockprops [id] [action] <args> &H- Changes properties of blocks in your map.",
            "&H  See &T/Help blockprops &Hfor how to use this command.",
            "&H  Remember to substitute /blockprops for /os blockprops when using the command based on the help",
        };

        static string[] envHelp = new string[] {
            "&T/os env [fog/cloud/sky/shadow/sun] [hex color] &H- Changes env colors of your map.",
            "&T/os env level [height] &H- Sets the water height of your map.",
            "&T/os env cloudheight [height] &H-Sets cloud height of your map.",
            "&T/os env maxfog &H- Sets the max fog distance in your map.",
            "&T/os env horizon &H- Sets the \"ocean\" block outside your map.",
            "&T/os env border &H- Sets the \"bedrock\" block outside your map.",
            "&T/os env weather [sun/rain/snow] &H- Sets weather of your map.",
            " Note: If no hex or block is given, the default will be used.",
        };

        static string[] gotoHelp = new string[] {
            "&T/os go &H- Teleports you to your first map.",
            "&T/os go [num] &H- Teleports you to your nth map.",
        };

        static string[] kickHelp = new string[] {
            "&T/os kick [name] &H- Removes that player from your map.",
        };

        static string[] kickAllHelp = new string[] {
            "&T/os kickall &H- Removes all other players from your map.",
        };

        static string[] levelBlockHelp = new string[] {
            "&T/os lb [action] <args> &H- Manages custom blocks on your map.",
            "&H  See &T/Help lb &Hfor how to use this command.",
            "&H  Remember to substitute /lb for /os lb when using the command based on the help",
        };

        static string[] mapHelp = new string[] {
            "&T/os map add [type - default is flat] &H- Creates your map (128x128x128)",
            "&T/os map add [width] [height] [length] [theme]",
            "&H  See &T/Help newlvl themes &Hfor a list of map themes.",
            "&T/os map physics [level] &H- Sets the physics on your map.",
            "&T/os map delete &H- Deletes your map",
            "&T/os map restore [num] &H- Restores backup [num] of your map",
            "&T/os map resize [width] [height] [length] &H- Resizes your map",
            "&T/os map save &H- Saves your map",
            "&T/os map pervisit [rank] &H- Sets the pervisit of you map",
            "&T/os map perbuild [rank] &H- Sets the perbuild of you map",
            "&T/os map texture [url] &H- Sets terrain.png for your map",
            "&T/os map texturepack [url] &H- Sets texture pack .zip for your map",
            "&T/os map [option] <value> &H- Toggles that map option.",
            "&H  See &T/Help map &Hfor a list of map options",
        };

        static string[] presetHelp = new string[] {
            "&T/os preset [name] &H- Sets the env settings of your map to that preset's.",
        };

        static string[] spawnHelp = new string[] {
            "&T/os setspawn &H- Sets the map's spawn point to your current position.",
        };

        static string[] zoneHelp = new string[] {
            "&T/os zone add [name] &H- Allows them to build in your map.",
            "&T/os zone del all &H- Deletes all zones in your map.",
            "&T/os zone del [name] &H- Prevents them from building in your map.",
            "&T/os zone list &H- Shows zones affecting a particular block.",
            "&T/os zone block [name] &H- Prevents them from joining your map.",
            "&T/os zone unblock [name] &H- Allows them to join your map.",
            "&T/os zone blacklist &H- Shows currently blacklisted players.",
        };

        static string[] zonesHelp = new string[] {
            "&T/os zones [cmd] [args]",
            "&HManages zones in your map. See &T/Help zone",
        };
        #endregion

        internal static SubCommandGroup subCommandGroup = new SubCommandGroup(commandShortcut,
                new List<SubCommand>() {
                new SubCommand("BlockProps", HandleBlockProps, blockPropsHelp, true, new string[] { "BlockProperties" }),
                new SubCommand("Env",        HandleEnv,        envHelp),
                new SubCommand("Go",         HandleGoto,       gotoHelp, false),
                new SubCommand("Kick",       HandleKick,       kickHelp),
                new SubCommand("KickAll",    HandleKickAll,    kickAllHelp),
                new SubCommand("LB",         HandleLevelBlock, levelBlockHelp, true, new string[] {"LevelBlock" }),
                new SubCommand("Map",        HandleMap,        mapHelp, false),
                new SubCommand("Preset",     HandlePreset,     presetHelp),
                new SubCommand("SetSpawn",   HandleSpawn,      spawnHelp, true, new string[] { "Spawn" }),
                new SubCommand("Zone",       HandleZone,       zoneHelp),
                new SubCommand("Zones",      HandleZones,      zonesHelp), }
            );

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
            string message = (cmd + " " + value).Trim();
            if (message.Length == 0) {
                p.MessageLines(mapHelp);
                return;
            }

            SubCommandGroup.UsageResult result = mapSubCommandGroup.Use(p, message, false);
            if (result != SubCommandGroup.UsageResult.NoneFound) { return; }

            LevelOption opt = LevelOptions.Find(cmd);
            if (opt == null) {
                p.Message("Could not find map command or map option \"{0}\".", cmd);
                p.Message("See &T/help {0} map &Sto display every command.", commandShortcut);
                return;
            }
            if (DisallowedMapOption(opt.Name)) {
                p.Message("&WYou cannot change the {0} map option via /{1} map.", opt.Name, commandShortcut);
                return;
            }
            if (!LevelInfo.IsRealmOwner(p.level, p.name)) {
                p.Message("You may only use &T/{0} map {1}&S after you join your map.", commandShortcut, opt.Name);
                return;
            }
            opt.SetFunc(p, p.level, value);
            p.level.SaveSettings();
        }
        static bool DisallowedMapOption(string opt) {
            return
                opt == LevelOptions.Speed || opt == LevelOptions.Overload || opt == LevelOptions.RealmOwner ||
                opt == LevelOptions.Goto || opt == LevelOptions.Unload;
        }

        static SubCommandGroup mapSubCommandGroup = new SubCommandGroup(commandShortcut + " map",
                new List<SubCommand>() {
                new SubCommand("Physics",  HandleMapPhysics, null),
                new SubCommand("Add",      HandleMapAdd,     null, false, new string[] { "create", "new" } ),
                new SubCommand("Delete",   HandleMapDelete,  null, true , new string[] { "del", "remove" } ),
                new SubCommand("Save",     (p, _, __)  => { UseCommand(p, "Save", ""); }, null),
                new SubCommand("Restore",  (p, arg, _) => { UseCommand(p, "Restore", arg); }, null),
                new SubCommand("Resize",   HandleMapResize,   null),
                new SubCommand("PerVisit", HandleMapPerVisit, null),
                new SubCommand("PerBuild", HandleMapPerBuild, null),
                new SubCommand("Texture",  HandleMapTexture,  null, true, new string[] { "texturezip", "texturepack" } ), }
            );

        static void HandleMapPhysics(Player p, string value, string ignored) {
            if (value == "0" || value == "1" || value == "2" || value == "3" || value == "4" || value == "5") {
                CmdPhysics.SetPhysics(p.level, int.Parse(value));
            } else {
                p.Message("Accepted numbers are: 0, 1, 2, 3, 4 or 5");
            }
        }
        static void HandleMapAdd(Player p, string value, string value2) {
            if (p.group.OverseerMaps == 0) {
                p.Message("Your rank is not allowed to create any /{0} maps.", commandShortcut); return;
            }
            value = (value + " " + value2).Trim();

            string level = NextLevel(p);
            if (level == null) return;
            string[] bits = value.SplitSpaces();

            if (value.Length == 0) value = "128 128 128";
            else if (bits.Length < 3) value = "128 128 128 " + value;
            string[] args = (level + " " + value.TrimEnd()).SplitSpaces(6);

            CmdNewLvl newLvl = (CmdNewLvl)Command.Find("NewLvl"); // TODO: this is a nasty hack, find a better way
            Level lvl = newLvl.GenerateMap(p, args, p.DefaultCmdData);
            if (lvl == null) return;

            MapGen.SetRealmPerms(p, lvl);
            p.Message("Use &T/{0} zone add [name] &Sto allow other players to build in the map.", commandShortcut);

            try {
                lvl.Save(true);
            } finally {
                lvl.Dispose();
                Server.DoGC();
            }
        }
        static void HandleMapDelete(Player p, string value, string ignored) {
            if (value.Length > 0) {
                p.Message("To delete your current map, type &T/{0} map delete", commandShortcut);
                return;
            }
            UseCommand(p, "DeleteLvl", p.level.name);
        }
        static void HandleMapResize(Player p, string value, string value2) {
            value = (value + " " + value2).Trim();
            value = p.level.name + " " + value;
            string[] args = value.SplitSpaces();
            if (args.Length < 4) { Command.Find("ResizeLvl").Help(p); return; }

            bool needConfirm;
            if (CmdResizeLvl.DoResize(p, args, p.DefaultCmdData, out needConfirm)) return;

            if (!needConfirm) return;
            p.Message("Type &T/{0} map resize {1} {2} {3} confirm &Sif you're sure.",
                      commandShortcut, args[1], args[2], args[3]);
        }
        static void HandleMapPerVisit(Player p, string value, string value2) {
            // Older realm maps didn't put you on visit whitelist, so make sure we put the owner here
            AccessController access = p.level.VisitAccess;
            if (!access.Whitelisted.CaselessContains(p.name)) {
                access.Whitelist(Player.Console, LevelPermission.Console, p.level, p.name);
            }
            value = (value + " " + value2).Trim();
            if (value.Length > 0) { value = p.level.name + " " + value; }
            UseCommand(p, "PerVisit", value);
        }
        static void HandleMapPerBuild(Player p, string value, string value2) {
            value = (value + " " + value2).Trim();
            if (value.Length > 0) { value = p.level.name + " " + value; }
            UseCommand(p, "PerBuild", value);
        }
        static void HandleMapTexture(Player p, string value, string value2) {
            value = (value + " " + value2).Trim();
            if (value.Length == 0) { value = "normal"; }
            UseCommand(p, "Texture", "levelzip " + value);
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
            } else if (Command.IsDeleteCommand(cmd)) {
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
