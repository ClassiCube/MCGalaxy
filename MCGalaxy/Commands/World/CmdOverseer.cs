/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.Commands.World {
    public sealed partial class CmdOverseer : Command2 {
        public override string name { get { return "Overseer"; } }
        public override string shortcut { get { return "os"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override bool SuperUseable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("Realm"), new CommandAlias("MyRealm") }; }
        }
        public override CommandParallelism Parallelism { get { return CommandParallelism.NoAndWarn; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            SubCommand.UseSubCommands(p, message, shortcut, coreSubCommands);
        }
        public override void Help(Player p, string message) {
            message = message.SplitSpaces()[0]; // only first argument
            SubCommand.HelpSubCommands(p, message, shortcut, coreSubCommands);
        }
        
        public override void Help(Player p) {
            p.Message("&T/os [command] [args]");
            p.Message("&HAllows you to modify and manage your personal realms.");
            SubCommand.HelpSubCommands(p, shortcut, coreSubCommands);
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

        static List<SubCommand> coreSubCommands = new List<SubCommand>() {
            new SubCommand("BlockProps", HandleBlockProps, blockPropsHelp, true, new string[] { "BlockProperties" }),
            new SubCommand("Env",        HandleEnv,        envHelp),
            new SubCommand("go",         HandleGoto,       gotoHelp, false),
            new SubCommand("Kick",       HandleKick,       kickHelp),
            new SubCommand("KickAll",    HandleKickAll,    kickAllHelp),
            new SubCommand("lb",         HandleLevelBlock, levelBlockHelp, true, new string[] {"LevelBlock" }),
            new SubCommand("Map",        HandleMap,        mapHelp, false),
            new SubCommand("Preset",     HandlePreset,     presetHelp),
            new SubCommand("SetSpawn",   HandleSpawn,      spawnHelp, true, new string[] { "Spawn" }),
            new SubCommand("Zone",       HandleZone,       zoneHelp),
            new SubCommand("Zones",      HandleZones,      zonesHelp),
        };
    }

    public delegate void SubCommandHandler(Player p, string cmd, string value);
    public class SubCommand {
        public enum UsageResult { NoneFound, Success, Disallowed }

        public readonly string Group;
        public readonly SubCommandHandler Handler;
        string[] Help;
        public readonly bool MapOnly;
        string[] Aliases;

        public SubCommand(string grpName, SubCommandHandler handler, string[] help, bool mapOnly = true, string[] aliases = null) {
            Group = grpName;
            Handler = handler;
            Help = help;
            MapOnly = mapOnly;
            Aliases = aliases;
        }
        bool Match(string cmd) {
            if (Aliases != null) {
                foreach (string alias in Aliases) {
                    if (alias.CaselessEq(cmd)) { return true; }
                }
            }
            return Group.CaselessEq(cmd);
        }
        bool Allowed(Player p, string domCommand) {
            if (MapOnly && !LevelInfo.IsRealmOwner(p.level, p.name)) {
                p.Message("You may only perform &T/{0} {1}&S after you join your map.", domCommand, Group.ToLower());
                return false;
            }
            return true;
        }
        void DisplayHelp(Player p) {
            if (Help == null || Help.Length == 0) {
                p.Message("No help is available for {0}", Group);
                return;
            }
            p.MessageLines(Help);
        }


        public static UsageResult UseSubCommands(Player p, string message, string domCommandName, List<SubCommand> subCommands, bool alertNoneFound = true) {
            string[] args = message.SplitSpaces(3);
            string cmd = args[0];
            string arg1 = args.Length > 1 ? args[1] : "";
            string arg2 = args.Length > 2 ? args[2] : "";

            foreach (SubCommand subCmd in subCommands) {
                if (!subCmd.Match(cmd)) { continue; }
                if (!subCmd.Allowed(p, domCommandName)) { return UsageResult.Disallowed; }
                subCmd.Handler(p, arg1, arg2);
                return UsageResult.Success;
            }
            if (alertNoneFound) {
                p.Message("There is no {0} command \"{1}\".", domCommandName, message);
                p.Message("See &T/help {0}&S for all {0} commands.", domCommandName);
            }
            return UsageResult.NoneFound;
        }
        public static void HelpSubCommands(Player p, string message, string domCommandName, List<SubCommand> subCommands) {
            foreach (SubCommand subCmd in subCommands) {
                if (!subCmd.Match(message)) { continue; }
                subCmd.DisplayHelp(p);
                return;
            }
            p.Message("There is no {0} command \"{1}\" to display help for.", domCommandName, message);
        }
        public static void HelpSubCommands(Player p, string domCommandName, List<SubCommand> subCommands) {
            p.Message("&HCommands: &S{0}", subCommands.Join(grp => grp.Group));
            p.Message("&HUse &T/Help {0} [command] &Hfor more details", domCommandName);
        }
    }
}