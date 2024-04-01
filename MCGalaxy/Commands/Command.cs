/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)

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
using System.Reflection;
using MCGalaxy.Commands;
using MCGalaxy.Commands.Bots;
using MCGalaxy.Commands.Building;
using MCGalaxy.Commands.Chatting;
using MCGalaxy.Commands.CPE;
using MCGalaxy.Commands.Eco;
using MCGalaxy.Commands.Fun;
using MCGalaxy.Commands.Info;
using MCGalaxy.Commands.Maintenance;
using MCGalaxy.Commands.Misc;
using MCGalaxy.Commands.Moderation;
using MCGalaxy.Commands.Scripting;
using MCGalaxy.Commands.World;
using MCGalaxy.Modules.Awards;
using MCGalaxy.Maths;
using MCGalaxy.Scripting;

namespace MCGalaxy 
{
    public abstract partial class Command 
    {
        /// <summary> The full name of this command (e.g. 'Copy') </summary>
        public abstract string name { get; }
        /// <summary> The shortcut/short name of this command (e.g. `"c"`) </summary>
        public virtual string shortcut { get { return ""; } }
        /// <summary> The type/group of this command (see `CommandTypes` class) </summary>
        public abstract string type { get; }
        /// <summary> Whether this command can be used in museums </summary>
        /// <remarks> Level altering (e.g. places a block) commands should return false </remarks>
        public virtual bool museumUsable { get { return true; } }
        /// <summary> The default minimum rank that is required to use this command </summary>
        public virtual LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        
        public abstract void Use(Player p, string message);
        public virtual void Use(Player p, string message, CommandData data) { Use(p, message); }
        public abstract void Help(Player p);
        public virtual void Help(Player p, string message) { Help(p); Formatter.PrintCommandInfo(p, this); }
        
        public virtual CommandPerm[] ExtraPerms { get { return null; } }
        public virtual CommandAlias[] Aliases { get { return null; } }
        
        /// <summary> Whether this command is usable by 'super' players (Console, IRC, etc) </summary>
        public virtual bool SuperUseable { get { return true; } }
        public virtual bool MessageBlockRestricted { get { return type.CaselessContains("mod"); } }
        /// <summary> Whether this command can be used when a player is frozen </summary>
        /// <remarks> Only informational commands should override this to return true </remarks>
        public virtual bool UseableWhenFrozen { get { return false; } }
        
        /// <summary> Whether using this command is logged to server logs </summary>
        /// <remarks> return false to prevent this command showing in logs (e.g. /pass) </remarks>
        public virtual bool LogUsage { get { return true; } }
        /// <summary> Whether this commands updates the 'most recent command used' by players </summary>
        /// <remarks> return false to prevent this command showing in /last (e.g. /pass, /hide) </remarks>
        public virtual bool UpdatesLastCmd { get { return true; } }
        
        public virtual CommandParallelism Parallelism { 
            get { return type.CaselessEq(CommandTypes.Information) ? CommandParallelism.NoAndWarn : CommandParallelism.Yes; }
        }
        public CommandPerms Permissions;
        
        public static List<Command> allCmds  = new List<Command>();
        public static bool IsCore(Command cmd) { 
            return cmd.GetType().Assembly == Assembly.GetExecutingAssembly(); // TODO common method
        }

        public static List<Command> CopyAll() { return new List<Command>(allCmds); }
        
        
        public static void InitAll() {
            allCmds.Clear();
            Alias.coreAliases.Clear();
            
            RegisterAllCore();
            IScripting.AutoloadCommands();
        }
        
        static void RegisterAllCore() {
            RegisterCore(typeof(CmdBot),       typeof(CmdBotAI),      typeof(CmdBots),       typeof(CmdBotSet), 
                         typeof(CmdBotSummon));
            
            RegisterCore(typeof(CmdAbort),     typeof(CmdBezier),     typeof(CmdBind),       typeof(CmdBrush), 
                         typeof(CmdCalculate), typeof(CmdCenter),     typeof(CmdCopySlot),   typeof(CmdDoNotMark), 
                         typeof(CmdMark),      typeof(CmdCmdBind),    typeof(CmdCopy),       typeof(CmdCuboid), 
                         typeof(CmdDelete),    typeof(CmdDraw),       typeof(CmdDrill),      typeof(CmdFill), 
                         typeof(CmdHollow),    typeof(CmdImageprint), typeof(CmdLine),       typeof(CmdMaze), 
                         typeof(CmdMeasure),   typeof(CmdMessageBlock),typeof(CmdMirror),    typeof(CmdMode), 
                         typeof(CmdOutline),   typeof(CmdPaint),      typeof(CmdPalette),    typeof(CmdPaste), 
                         typeof(CmdPlace),     typeof(CmdPortal),     typeof(CmdPyramid),    typeof(CmdRainbow), 
                         typeof(CmdRedo),      typeof(CmdReplaceAll), typeof(CmdReplaceBrush), 
                         typeof(CmdReplaceNotBrush),typeof(CmdSphere),typeof(CmdTransform), 
                         typeof(CmdTriangle),  typeof(CmdReplace),    typeof(CmdReplaceNot), 
                         typeof(CmdRestartPhysics),typeof(CmdSpheroid),typeof(CmdSpin),      typeof(CmdSPlace), 
                         typeof(CmdStatic),    typeof(CmdTorus),      typeof(CmdTree),       typeof(CmdUndo), 
                         typeof(CmdWriteText), typeof(CmdWrite));
            
            RegisterCore(typeof(CmdPronouns),  typeof(Cmd8Ball),      typeof(CmdAdminChat),  typeof(CmdAfk), 
                         typeof(CmdColor),     typeof(CmdEat),        typeof(CmdEmote),      typeof(CmdHug), 
                         typeof(CmdIgnore),    typeof(CmdInbox),      typeof(CmdLoginMessage), 
                         typeof(CmdLogoutMessage),typeof(CmdMe),      typeof(CmdNick),       typeof(CmdOpChat), 
                         typeof(CmdClear),     typeof(CmdRankMsg),    typeof(CmdRoll),       typeof(CmdSay), 
                         typeof(CmdSend),      typeof(CmdTColor),     typeof(CmdTitle),      typeof(CmdVote),
                         typeof(CmdWhisper),   typeof(CmdHigh5));

            RegisterCore(typeof(CmdCustomColors),typeof(CmdEntityRot),typeof(CmdEnvironment),typeof(CmdHold), 
                         typeof(CmdModel),     typeof(CmdModelScale), typeof(CmdPing),       typeof(CmdReachDistance), 
                         typeof(CmdSkin),      typeof(CmdTexture),    typeof(CmdGlobalBlock),typeof(CmdLevelBlock));

            RegisterCore(typeof(CmdBuy),       typeof(CmdEconomy),    typeof(CmdGive),       typeof(CmdBalance), 
                         typeof(CmdPay),       typeof(CmdStore),      typeof(CmdTake));

            RegisterCore(typeof(CmdExplode),   typeof(CmdFlipHead),   typeof(CmdFlipHeads),  typeof(CmdGun), 
                         typeof(CmdMissile),   typeof(CmdReferee),    typeof(CmdSlap),       typeof(CmdTeam), 
                         typeof(CmdLike),      typeof(CmdDislike));

            RegisterCore(typeof(CmdAbout),     typeof(CmdBanInfo),    typeof(CmdBlocks),     typeof(CmdClones), 
                         typeof(CmdCommands),  typeof(CmdFaq),        typeof(CmdHasirc),     typeof(CmdHelp), 
                         typeof(CmdRankInfo),  typeof(CmdServerInfo), typeof(CmdLastCmd),    typeof(CmdLoaded), 
                         typeof(CmdMapInfo),   typeof(CmdNews),       typeof(CmdOpRules),    typeof(CmdOpStats), 
                         typeof(CmdPClients),  typeof(CmdPlayers),    typeof(CmdRules),      typeof(CmdSearch), 
                         typeof(CmdSeen),      typeof(CmdTime),       typeof(CmdTop),        typeof(CmdLevels), 
                         typeof(CmdView),      typeof(CmdViewRanks),  typeof(CmdWhere),      typeof(CmdWhois), 
                         typeof(CmdWhoNick));

            RegisterCore(typeof(CmdBlockDB),   typeof(CmdBlockSpeed), typeof(CmdInfoSwap),   typeof(CmdLimit), 
                         typeof(CmdLowlag),    typeof(CmdPlayerEdit), typeof(CmdRestart),    typeof(CmdServer), 
                         typeof(CmdShutdown),  typeof(CmdUpdate),     typeof(CmdBan),        typeof(CmdBanEdit), 
                         typeof(CmdBanip),     typeof(CmdBlockSet),   typeof(CmdCmdSet),     typeof(CmdFollow), 
                         typeof(CmdFreeze),    typeof(CmdHide),       typeof(CmdHighlight),  typeof(CmdJoker), 
                         typeof(CmdKick),      typeof(CmdLocation),   typeof(CmdModerate),   typeof(CmdMoveAll), 
                         typeof(CmdMute),      typeof(CmdOHide),      typeof(CmdP2P),        typeof(CmdPatrol), 
                         typeof(CmdPossess),   typeof(CmdReport),     typeof(CmdRestoreSelection),typeof(CmdSetRank), 
                         typeof(CmdTempBan),   typeof(CmdTempRank),   typeof(CmdTrust),      typeof(CmdUnban), 
                         typeof(CmdUnbanip),   typeof(CmdUndoPlayer), typeof(CmdVIP),        typeof(CmdVoice), 
                         typeof(CmdWarn),      typeof(CmdWhitelist),  typeof(CmdXban),       typeof(CmdZone), 
                         typeof(CmdZoneTest),  typeof(CmdZoneList),   typeof(CmdZoneMark));

            RegisterCore(typeof(CmdAscend),    typeof(CmdBack),       typeof(CmdDelay),      typeof(CmdDescend), 
                         typeof(CmdFakeRank),  typeof(CmdFly),        typeof(CmdHackRank),   typeof(CmdInvincible), 
                         typeof(CmdKill),      typeof(CmdRide),       typeof(CmdSendCmd),    typeof(CmdSummon), 
                         typeof(CmdTimer),     typeof(CmdTp),         typeof(CmdTpA),        typeof(CmdRagequit), 
                         typeof(CmdQuit),      typeof(CmdCrashServer),typeof(CmdHacks));

            RegisterCore(typeof(CmdCmdLoad),   typeof(CmdCmdUnload),  typeof(CmdPlugin));

            RegisterCore(typeof(CmdBlockProperties),typeof(CmdCopyLvl),typeof(CmdDeleteLvl), typeof(CmdFixGrass), 
                         typeof(CmdGoto),      typeof(CmdImport),     typeof(CmdLoad),       typeof(CmdLockdown), 
                         typeof(CmdMain),      typeof(CmdMap),        typeof(CmdMuseum),     typeof(CmdNewLvl), 
                         typeof(CmdOverseer),  typeof(CmdPause),      typeof(CmdPhysics),    typeof(CmdRenameLvl),
                         typeof(CmdResizeLvl), typeof(CmdRestore),    typeof(CmdReload),     typeof(CmdSave), 
                         typeof(CmdSetspawn),  typeof(CmdSpawn),      typeof(CmdUnflood),    typeof(CmdUnload), 
                         typeof(CmdPermissionBuild),typeof(CmdPermissionVisit));

            RegisterCore(typeof(CmdAward),     typeof(CmdAwardMod),   typeof(CmdAwards));            
        }
        
        static void RegisterCore(params Type[] types) {
            foreach (Type type in types)
            {
                Command cmd = (Command)Activator.CreateInstance(type);
                if (Server.Config.DisabledCommands.CaselessContains(cmd.name)) continue;
                Register(cmd);
            }
        }
        
        public static void Register(Command cmd) {
            allCmds.Add(cmd);            
            cmd.Permissions = CommandPerms.GetOrAdd(cmd.name, cmd.defaultRank);
            
            CommandPerm[] extra = cmd.ExtraPerms;
            if (extra != null) {
                for (int i = 0; i < extra.Length; i++) 
                {
                    CommandExtraPerms exPerms = CommandExtraPerms.GetOrAdd(cmd.name, i + 1, extra[i].Perm);
                    exPerms.Desc = extra[i].Description;
                }
            }           
            Alias.RegisterDefaults(cmd);
        }

        public static void TryRegister(bool announce, params Command[] commands)
        {
            foreach (Command cmd in commands)
            {
                if (Find(cmd.name) != null) continue;

                Register(cmd);
                if (announce) Logger.Log(LogType.SystemActivity, "Command /{0} loaded", cmd.name);
            }
        }
        
        public static bool Unregister(Command cmd) {
            bool removed = allCmds.Remove(cmd);
            
            // typical usage: Command.Unregister(Command.Find("xyz"))
            // So don't throw exception if Command.Find returned null
            if (cmd != null) Alias.UnregisterDefaults(cmd);
            return removed;
        }
        
        public static void Unregister(params Command[] commands) {
            foreach (Command cmd in commands) Unregister(cmd);
        }
        
        
        public static string GetColoredName(Command cmd) {
            LevelPermission perm = cmd.Permissions.MinRank;
            return Group.GetColor(perm) + cmd.name;
        }
        
        public static Command Find(string name) {
            foreach (Command cmd in allCmds) 
            {
                if (cmd.name.CaselessEq(name)) return cmd;
            }
            return null;
        }
        
        public static void Search(ref string cmdName, ref string cmdArgs) {
            if (cmdName.Length == 0) return;
            Alias alias = Alias.Find(cmdName);
            
            // Aliases override built in command shortcuts
            if (alias == null) {
                foreach (Command cmd in allCmds) 
                {
                    if (!cmd.shortcut.CaselessEq(cmdName)) continue;
                    cmdName = cmd.name; return;
                }
                return;
            }
            
            cmdName = alias.Target;
            string format = alias.Format;
            if (format == null) return;
            
            if (format.Contains("{args}")) {
                cmdArgs = format.Replace("{args}", cmdArgs);
            } else {
                cmdArgs = format + " " + cmdArgs;
            }
            cmdArgs = cmdArgs.Trim();
        }
    }
    
    public enum CommandContext : byte 
    {
        Normal, Static, SendCmd, Purchase, MessageBlock
    }
    
    public struct CommandData 
    {
        public LevelPermission Rank;
        public CommandContext Context;
        public Vec3S32 MBCoords;
    }
    
    // Clunky design, but needed to stay backwards compatible with custom commands
    public abstract class Command2 : Command 
    {
        public override void Use(Player p, string message) {
            Use(p, message, p.DefaultCmdData);
        }
    }

    public enum CommandParallelism
    {
        NoAndSilent, NoAndWarn, Yes
    }
}

namespace MCGalaxy.Commands 
{
    public struct CommandPerm 
    {
        public LevelPermission Perm;
        public string Description;
        
        public CommandPerm(LevelPermission perm, string desc) {
            Perm = perm; Description = desc;
        }
    }
    
    public struct CommandAlias 
    {
        public string Trigger, Format;
        
        public CommandAlias(string cmd, string format = null) {
            Trigger = cmd; Format = format;
        }
    }
}
