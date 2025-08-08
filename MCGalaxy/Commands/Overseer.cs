/*
    Copyright 2015-2024 MCGalaxy
        
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
        
        static void AnnounceRenamed(Player p, string oldName, string newName) {
            p.Message("Note that &T/{0} {1}&S has been renamed to &T/{0} {2}", 
                      commandShortcut, oldName, newName);
        }
        

        static string GetLevelName(Player p, string mapSubName) {
            return p.name.ToLower() + mapSubName;
        }

        static string NextLevel(Player p) {
            string[] allLevelNames = LevelInfo.AllMapNames();
            List<string> levelNames = Wildcard.Filter(allLevelNames, p.name.ToLower() + "*", levelName => levelName);

            int realmsOwned = 0;
            for (int i = 0; i < levelNames.Count; i++) {
                string levelName = levelNames[i];

                if (LevelInfo.IsRealmOwner(p.name, levelName)) {
                    realmsOwned += 1;
                    if (realmsOwned >= p.group.OverseerMaps) {
                        break;
                    }
                }
            }

            if (realmsOwned < p.group.OverseerMaps) {
                for (int i = 1; ; i++) {
                    string levelName = GetLevelName(p, i == 1 ? "" : "" + i);

                    if (!LevelInfo.MapExists(levelName)) {
                        return levelName;
                    }
                }
            }

            p.Message("You have reached the limit for your overseer maps.");
            return null;
        }

        static string[] addHelp = new string[] {
            "&T/os add &H- Creates a flat map (128x128x128).",
            "&T/os add [theme] &H- Creates a map with [theme] terrain.",
            "&H  Use &T/Help newlvl themes &Hfor a list of map themes.",
            "&T/os add [width] [height] [length] [theme]",
            "&H  Creates a map with custom size and theme.",
        };
        static void HandleAdd(Player p, string message) {
            if (p.group.OverseerMaps == 0) {
                p.Message("Your rank is not allowed to create any /{0} maps.", commandShortcut); return;
            }

            string level = NextLevel(p);
            if (level == null) return;
            string[] bits = message.SplitSpaces();

            if (message.Length == 0) message = "128 128 128";
            else if (bits.Length < 3) message = "128 128 128 " + message;
            string[] genArgs = (level + " " + message.TrimEnd()).SplitSpaces(6);

            CmdNewLvl newLvl = (CmdNewLvl)Command.Find("NewLvl"); // TODO: this is a nasty hack, find a better way
            Level lvl = newLvl.GenerateMap(p, genArgs, p.DefaultCmdData);
            if (lvl == null) return;

            MapGen.SetRealmPerms(p, lvl);
            p.Message("Use &T/{0} allow [name] &Sto allow other players to build in the map.", commandShortcut);

            try {
                lvl.Save(true);
            } finally {
                lvl.Dispose();
                Server.DoGC();
            }
        }

        static string[] deleteHelp = new string[] {
            "&T/os delete &H- Deletes your map.",
            "&T/os delete "+CmdDeleteLvl.BACKUP_FLAG+" [backup]",
            "&H  -Permanently- deletes [backup] from your map.",
        };
        static void HandleDelete(Player p, string message) {
            if (message.CaselessStarts(CmdDeleteLvl.BACKUP_FLAG)) {
                string[] args = message.SplitSpaces(2); //"flag", "other args"
                if (args.Length == 1) {
                    p.Message("You must provide a backup to delete.");
                    p.Message("A backup is usually a number, but may also be named.");
                    p.Message("See &T/{0} restore &7to display backups.", commandShortcut);
                    return;
                }
                CmdDeleteLvl.UseBackup(p, p.level.MapName +" "+ args[1], true);
                return;
            }

            if (message.Length > 0) {
                p.Message("To delete your current map, type &T/{0} delete", commandShortcut);
                return;
            }
            UseCommand(p, "DeleteLvl", p.level.name);
        }

        static string[] allowHelp = new string[] {
            "&T/os allow [player]",
            "&H  Allows [player] to build in your map.",
        };
        static void HandleAllow(Player p, string message) {
            _HandlePerm(p, message, "allow", "perbuild", "+");
        }
        static string[] disallowHelp = new string[] {
            "&T/os disallow [player]",
            "&H  Disallows [player] from building in your map.",
        };
        static void HandleDisallow(Player p, string message) {
            _HandlePerm(p, message, "disallow", "perbuild", "-");
        }
        static string[] banHelp = new string[] {
            "&T/os ban [player]",
            "&H  Bans [player] from visiting your map.",
        };
        static void HandleBan(Player p, string message) {
            _HandlePerm(p, message, "ban", "pervisit", "-");
        }
        static string[] unbanHelp = new string[] {
            "&T/os unban [player]",
            "&H  Unbans [player] from visiting your map.",
        };
        static void HandleUnban(Player p, string message) {
            _HandlePerm(p, message, "unban", "pervisit", "+");
        }
        static void _HandlePerm(Player p, string message, string action, string cmd, string prefix) {
            if (message.Length == 0) { p.Message("&WYou need to type a player name to {0}.", action); return; }
            UseCommand(p, cmd, prefix + message);
        }

        static string[] blockPropsHelp = new string[] {
            "&T/os blockprops [id] [action] <args> &H- Changes properties of blocks in your map.",
            "&H  See &T/Help blockprops &Hfor how to use this command.",
            "&H  Remember to substitute /blockprops for /os blockprops when using the command based on the help",
        };
        static void HandleBlockProps(Player p, string message) {
            if (message.Length == 0) { p.MessageLines(blockPropsHelp); return; }
            UseCommand(p, "BlockProperties", "level " + message);
        }

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
        static void HandleEnv(Player p, string raw) {
            string[] args = raw.SplitExact(2);
            Level lvl     = p.level;
            
            if (CmdEnvironment.Handle(p, lvl, args[0], args[1], lvl.Config, lvl.ColoredName)) return;
            p.MessageLines(envHelp);
        }

        static string[] gotoHelp = new string[] {
            "&T/os go &H- Teleports you to your first map.",
            "&T/os go [num] &H- Teleports you to your nth map.",
        };
        static void HandleGoto(Player p, string message) {
            string map = GetLevelName(p, message);

            if (message.Length == 0) {
                GotoExact(p, map); return;
            }

            int mapNumber;
            if (NumberUtils.TryParseInt32(message, out mapNumber)) {
                //If it's a number, use exact goto logic like before
                GotoExact(p, map); return;
            }

            if (Formatter.ValidMapName(p, map)) {
                //Allow partial match for going to renamed os map
                //Works funky with level auto load off...? TOO BAD! No one should ever use that setting
                PlayerActions.ChangeMap(p, map);
            }
        }
        static void GotoExact(Player p, string map) {
            if (LevelInfo.FindExact(map) == null)
                LevelActions.Load(p, map, !Server.Config.AutoLoadMaps);
            if (LevelInfo.FindExact(map) != null)
                PlayerActions.ChangeMap(p, map);
        }

        static string[] kickHelp = new string[] {
            "&T/os kick [name] &H- Removes that player from your map.",
        };
        static void HandleKick(Player p, string name) {
            if (name.Length == 0) { p.Message("You must specify a player to kick."); return; }
            Player pl = PlayerInfo.FindMatches(p, name);
            if (pl == null) return;

            if (pl.level == p.level) {
                PlayerActions.ChangeMap(pl, Server.mainLevel);
            } else {
                p.Message("Player is not on your level!");
            }
        }

        static string[] kickAllHelp = new string[] {
            "&T/os kickall &H- Removes all other players from your map.",
        };
        static void HandleKickAll(Player p, string unused) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level == p.level && pl != p)
                    PlayerActions.ChangeMap(pl, Server.mainLevel);
            }
        }

        static string[] levelBlockHelp = new string[] {
            "&T/os lb [action] <args> &H- Manages custom blocks on your map.",
            "&H  Use &T/Help lb &Hfor how to use this command.",
            "&H  Remember to substitute /lb for /os lb when using the command based on the help",
        };
        static void HandleLevelBlock(Player p, string lbArgs) {
            CustomBlockCommand.Execute(p, lbArgs, p.DefaultCmdData, false, "/os lb");
        }

        static string[] mapHelp = new string[] {
            "&T/os map [option] <value> &H- Toggles that map option.",
            "&H  See &T/Help map &Hfor a list of map options",
        };
        static void HandleMap(Player p, string raw) {
            if (raw.Length == 0) {
                p.MessageLines(mapHelp); return;
            }

            SubCommandGroup.UsageResult result = mapSubCommandGroup.Use(p, raw, false);
            if (result != SubCommandGroup.UsageResult.NoneFound) return;
            
            string[] args = raw.SplitExact(2);
            string cmd    = args[0];
            string value  = args[1];

            LevelOption opt = LevelOptions.Find(cmd);
            if (opt == null) {
                p.Message("Could not find map option \"{0}\".", cmd);
                p.Message("Use &T/help map options &Sto see all.");
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
                opt == LevelOptions.Goto  || opt == LevelOptions.Unload;
        }

        static void MapMoved(Player p, string message, string name, SubCommand.Behavior behaviour, bool mapOnly = true) {
            AnnounceRenamed(p, "map " + name, name);
            if (mapOnly && !LevelInfo.IsRealmOwner(p.level, p.name)) {
                p.Message("You may only use &T/{0} {1}&S after you join your map.", commandShortcut, name);
                return;
            }
            behaviour(p, message);
        }
        
        static SubCommandGroup mapSubCommandGroup = new SubCommandGroup(commandShortcut + " map",
                new List<SubCommand>() {
                    new SubCommand("Physics",  (p, arg) => { MapMoved(p, arg, "physics",  HandlePhysics);   }),
                    new SubCommand("Add",      (p, arg) => { MapMoved(p, arg, "add",      HandleAdd, false);}, false, new string[] { "create", "new" } ),
                    new SubCommand("Delete",   (p, arg) => { MapMoved(p, arg, "delete",   HandleDelete);    }, false, new string[] { "del", "remove" } ),
                    new SubCommand("Save",     (p, arg) => { MapMoved(p, arg, "save",     HandleSave);      }),
                    new SubCommand("Rename",   (p, arg) => { MapMoved(p, arg, "rename",   HandleRename);    }),
                    new SubCommand("Restore",  (p, arg) => { MapMoved(p, arg, "restore",  HandleRestore);   }),
                    new SubCommand("Resize",   (p, arg) => { MapMoved(p, arg, "resize",   HandleResize);    }),
                    new SubCommand("PerVisit", (p, arg) => { MapMoved(p, arg, "pervisit", HandlePervisit);  }),
                    new SubCommand("PerBuild", (p, arg) => { MapMoved(p, arg, "perbuild", HandlePerbuild);  }),
                    new SubCommand("Texture",  (p, arg) => { MapMoved(p, arg, "texture",  HandleTexture);   }, false, new string[] { "texturezip", "texturepack" } ),
                }
            );

        static string[] physicsHelp = new string[] {
            "&T/os physics [number]",
            "&H  Changes the physics settings in your map.",
            "&H  See &T/help physics &Hfor details."
        };
        static void HandlePhysics(Player p, string message) {
            if (message.Length == 0) {
                p.MessageLines(physicsHelp);
                return;
            }
            int level = 0;
            if (!CommandParser.GetInt(p, message, "Physics level", ref level, 0, 5)) return;
            
            CmdPhysics.SetPhysics(p.level, level);
        }

        static string[] resizeHelp = new string[] {
            "&T/os resize [width] [height] [length]",
            "&H  Resizes your map.",
        };
        static void HandleResize(Player p, string message) {
            message = p.level.name + " " + message;
            string[] args = message.SplitSpaces();
            if (args.Length < 4) {
                p.Message("Not enough args provided! Usage:");
                p.Message("&T/{0} resize [width] [height] [length]", commandShortcut);
                return;
            }

            bool needConfirm;
            if (CmdResizeLvl.DoResize(p, args, p.DefaultCmdData, out needConfirm)) return;

            if (!needConfirm) return;
            p.Message("Type &T/{0} resize {1} {2} {3} confirm &Sif you're sure.",
                      commandShortcut, args[1], args[2], args[3]);
        }


        static string[] pervisitHelp = new string[] {
            "&T/os pervisit [rank]",
            "&H  Changes the rank required to visit your map.",
        };
        static void HandlePervisit(Player p, string message) {
            // Older realm maps didn't put you on visit whitelist, so make sure we put the owner here
            AccessController access = p.level.VisitAccess;
            if (!access.Whitelisted.CaselessContains(p.name)) {
                access.Whitelist(Player.Console, LevelPermission.Console, p.level, p.name);
            }
            if (message.Length == 0) {
                p.Message("See &T/help pervisit &Sfor how to use this command, but don't include [level].");
                return;
            }
            message = p.level.name + " " + message;
            UseCommand(p, "PerVisit", message);
        }

        static string[] perbuildHelp = new string[] {
            "&T/os perbuild [rank]",
            "&H  Changes the rank required to build in your map.",
        };
        static void HandlePerbuild(Player p, string message) {
            if (message.Length == 0) {
                p.Message("See &T/help perbuild &Sfor how to use this command, but don't include [level].");
                return;
            }
            message = p.level.name + " " + message;
            UseCommand(p, "PerBuild", message);
        }

        static string[] textureHelp = new string[] {
            "&T/os texture [url]",
            "&H  Changes the textures used in your map.",
        };
        
        static void HandleTexture(Player p, string message) {
            if (message.Length == 0) { message = "normal"; }
            UseCommand(p, "Texture", "levelzip " + message);
        }

        static string[] presetHelp = new string[] {
            "&T/os preset [name] &H- Changes the environment color preset in your map.",
        };
        static void HandlePreset(Player p, string preset) {
            string raw = ("preset " + preset).Trim();
            HandleEnv(p, raw);
        }


        static string[] spawnHelp = new string[] {
            "&T/os setspawn &H- Sets the map's spawn point to your current position.",
        };
        static void HandleSpawn(Player p, string unused) {
            UseCommand(p, "SetSpawn", "");
        }

        static string[] plotHelp = new string[] {
            "&T/os plot [args]",
            "&H  Plots are zones that can change permissions and environment." +
            "&H  See &T/Help zone &Hto learn what args you can use.",
        };
        static void HandlePlot(Player p, string raw) {
            string[] args = raw.SplitSpaces(2);

            if (args.Length == 1) {
                p.Message("This command is the &T/{0} &Sversion of &T/zone&S.", commandShortcut);
                p.Message("To learn how to use it, read &T/help zone&S");
            } else {
                UseCommand(p, "Zone", args[0] + " " + args[1]);
            }
        }

        static string[] saveHelp = new string[] {
            "&T/os save",
            "&H  Creates a backup of your map.",
            "&H  Your map is saved automatically, so this is only useful",
            "&H  If you want to save a specific state to restore later.",
        };
        static void HandleSave(Player p, string unused) {
            UseCommand(p, "Save", "");
        }

        static string[] renameHelp = new string[] {
            "&T/os rename <name>",
            "&H  Renames your current map. Your player name is always kept as prefix.",
            "&H  Without <name>, renames to just your player name.",
            "&H  With <name>, renames to 'yourname[name]'.",
        };
        static void HandleRename(Player p, string args) {
            if (args.Length > 0 && !Formatter.IsValidName(p, args, "os name", Player.USERNAME_ALPHABET)) {
                return;
            }
            UseCommand(p, "RenameLvl", p.level.name + " " + GetLevelName(p, args));
        }
        
        static string[] restoreHelp = new string[] {
            "&T/os restore <number>",
            "&H  Restores a backup of your map.",
            "&H  Use without a number to see total backup count.",
        };
        static void HandleRestore(Player p, string args) {
            UseCommand(p, "Restore", args);
        }
        
        //Placed at the end so that the help arrays aren't null
        internal static SubCommandGroup subCommandGroup = new SubCommandGroup(commandShortcut,
                new List<SubCommand>() {
                    new SubCommand("Add",        HandleAdd,        addHelp,  false, new string[] { "create", "new" }),
                    new SubCommand("Go",         HandleGoto,       gotoHelp, false),
                    new SubCommand("Allow",      HandleAllow,      allowHelp),
                    new SubCommand("Disallow",   HandleDisallow,   disallowHelp),
                    new SubCommand("Ban",        HandleBan,        banHelp),
                    new SubCommand("Unban",      HandleUnban,      unbanHelp),
                    new SubCommand("SetSpawn",   HandleSpawn,      spawnHelp, true, new string[] { "spawn" }),
                    new SubCommand("Env",        HandleEnv,        envHelp),
                    new SubCommand("Preset",     HandlePreset,     presetHelp),
                    new SubCommand("Map",        HandleMap,        mapHelp, false),

                    new SubCommand("Plot",       HandlePlot,       plotHelp, true, new string[] { "plots" }),
                    new SubCommand("PerBuild",   HandlePerbuild,   perbuildHelp),
                    new SubCommand("PerVisit",   HandlePervisit,   pervisitHelp),
                    new SubCommand("Physics",    HandlePhysics,    physicsHelp),

                    new SubCommand("LB",         HandleLevelBlock, levelBlockHelp, true, new string[] {"LevelBlock" }),
                    new SubCommand("BlockProps", HandleBlockProps, blockPropsHelp, true, new string[] { "BlockProperties" }),
                    new SubCommand("Texture",    HandleTexture,    textureHelp,    true, new string[] { "texturezip", "texturepack" }),
                    new SubCommand("Kick",       HandleKick,       kickHelp),
                    new SubCommand("KickAll",    HandleKickAll,    kickAllHelp),

                    new SubCommand("Resize",     HandleResize,     resizeHelp),
                    new SubCommand("Save",       HandleSave,       saveHelp),
                    new SubCommand("Delete",     HandleDelete,     deleteHelp, true, new string[] { "del", "remove" } ),
                    new SubCommand("Rename",     HandleRename,     renameHelp),
                    new SubCommand("Restore",    HandleRestore,    restoreHelp),
                }
            );


        static void HandleZone(Player p, string raw) {
            string[] args = raw.SplitExact(2);
            string cmd  = args[0];
            string name = args[1];
            string old  = "zone " + cmd;
            cmd = cmd.ToUpper();
            
            if (cmd == "ADD") {
                AnnounceRenamed(p, old, "allow");
                HandleAllow(p, name);
            } else if (Command.IsDeleteAction(cmd)) {
                AnnounceRenamed(p, old, "disallow");
                HandleDisallow(p, name);
            } else if (cmd == "BLOCK") {
                AnnounceRenamed(p, old, "ban");
                HandleBan(p, name);
            } else if (cmd == "UNBLOCK") {
                AnnounceRenamed(p, old, "unban");
                HandleUnban(p, name);
            } else if (cmd == "LIST") {
                p.Message("To see a list of zones in a level, use &T/zonelist");
                UseCommand(p, "ZoneList", name);
            } else if (cmd == "BLACKLIST") {
                p.Message("To see who is disallowed from visiting, use &T/mapinfo");
            } else {
                p.Message("&T  /{0} zone &Hwas used for managing permissions in your map.", commandShortcut);
                p.Message("&H  It has now been replaced by the following &T/{0} &Hcommands:", commandShortcut);
                p.Message("&T  Allow, Disallow, Ban, Unban");
                p.Message("&H  To manage zoned areas in your map, use &T/{0} plot", commandShortcut);
            }
        }
        
        static void ZonesMoved(Player p, string raw) {
            AnnounceRenamed(p, "zones", "plot");
            HandlePlot(p, raw);
        }

        internal static SubCommandGroup deprecatedSubCommandGroup = new SubCommandGroup(commandShortcut,
                new List<SubCommand>() {
                    new SubCommand("Zone",  HandleZone, false),
                    new SubCommand("Zones", ZonesMoved, false),
                }
            );
    }
}
