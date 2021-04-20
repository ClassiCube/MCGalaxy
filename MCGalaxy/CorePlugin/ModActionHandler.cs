/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
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
using MCGalaxy.Commands;
using MCGalaxy.Commands.Moderation;
using MCGalaxy.DB;
using MCGalaxy.Events;
using MCGalaxy.Tasks;

namespace MCGalaxy.Core {
    internal static class ModActionHandler {
        
        internal static void HandleModAction(ModAction action) {
            switch (action.Type) {
                    case ModActionType.Frozen: DoFreeze(action); break;
                    case ModActionType.Unfrozen: DoUnfreeze(action); break;
                    case ModActionType.Muted: DoMute(action); break;
                    case ModActionType.Unmuted: DoUnmute(action); break;
                    case ModActionType.Ban: DoBan(action); break;
                    case ModActionType.Unban: DoUnban(action); break;
                    case ModActionType.BanIP: DoBanIP(action); break;
                    case ModActionType.UnbanIP: DoUnbanIP(action); break;
                    case ModActionType.Warned: DoWarn(action); break;
                    case ModActionType.Rank: DoRank(action); break;
            }
        }
        
        static void LogAction(ModAction e, Player target, string action) {
            if (e.Announce) {
                // TODO: Chat.MessageFrom if target is online?
                Player who = PlayerInfo.FindExact(e.Target);
                // TODO: who.SharesChatWith
                Chat.Message(ChatScope.Global, e.FormatMessage(e.TargetName, action),
                             null, null, true);
            } else {
                Chat.MessageOps(e.FormatMessage(e.TargetName, action));
            }
            
            action = Colors.StripUsed(action);
            string suffix = "";
            if (e.Duration.Ticks != 0) suffix = " &Sfor " + e.Duration.Shorten();
            
            Logger.Log(LogType.UserActivity, "{0} was {1} by {2}",
                       e.Target, action, e.Actor.name + suffix);
        }

        
        static void DoFreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = true;
            LogAction(e, who, "&bfrozen");

            Server.frozen.Update(e.Target, FormatModTaskData(e));
            ModerationTasks.FreezeCalcNextRun();
            Server.frozen.Save();
        }
        
        static void DoUnfreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = false;
            LogAction(e, who, "&adefrosted");
            
            Server.frozen.Remove(e.Target);
            ModerationTasks.FreezeCalcNextRun();
            Server.frozen.Save();
        }
        
        
        static void DoMute(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.muted = true;
            LogAction(e, who, "&8muted");
            
            Server.muted.Update(e.Target, FormatModTaskData(e));
            ModerationTasks.MuteCalcNextRun();
            Server.muted.Save();
        }
        
        static void DoUnmute(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.muted = false;
            LogAction(e, who, "&aun-muted");
            
            Server.muted.Remove(e.Target);
            ModerationTasks.MuteCalcNextRun();
            Server.muted.Save();
        }
        
        
        static void DoBan(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            LogAction(e, who, "&8banned");
            
            if (e.Duration.Ticks != 0) {
                string banner = e.Actor.truename;
                DateTime end = DateTime.UtcNow.Add(e.Duration);
                Server.tempBans.Update(e.Target, Ban.PackTempBanData(e.Reason, banner, end));
                Server.tempBans.Save();

                if (who != null) who.Kick("Banned for " + e.Duration.Shorten(true) + "." + e.ReasonSuffixed);
            } else {
                Ban.DeleteBan(e.Target);
                Ban.BanPlayer(e.Actor, e.Target, e.Reason, !e.Announce, e.TargetGroup.Name);
                ModActionCmd.ChangeRank(e.Target, e.targetGroup, Group.BannedRank, who);
                
                if (who != null) {
                    string msg = e.Reason.Length == 0 ? Server.Config.DefaultBanMessage : e.Reason;
                    who.Kick("Banned by " + e.Actor.ColoredName + ": &S" + msg,
                             "Banned by " + e.Actor.ColoredName + ": &f" + msg);
                }
            }
        }
        
        static void DoUnban(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            LogAction(e, who, "&8unbanned");
            
            if (Server.tempBans.Remove(e.Target)) {
                Server.tempBans.Save();
            }
            if (!Group.BannedRank.Players.Contains(e.Target)) return;
            
            Ban.DeleteUnban(e.Target);
            Ban.UnbanPlayer(e.Actor, e.Target, e.Reason);
            ModActionCmd.ChangeRank(e.Target, Group.BannedRank, Group.DefaultRank, who, false);
            
            string ip = PlayerDB.FindIP(e.Target);
            if (ip != null && Server.bannedIP.Contains(ip)) {
                e.Actor.Message("NOTE: Their IP is still banned.");
            }
        }
        
        
        static void LogIPAction(ModAction e, string type) {
            ItemPerms perms = CommandExtraPerms.Find("WhoIs", 1);
            Chat.Message(ChatScope.Global, e.FormatMessage("An IP", type), perms,
                         FilterNotItemPerms, true);
            Chat.Message(ChatScope.Global, e.FormatMessage(e.Target, type), perms,
                         Chat.FilterPerms, true);
        }
        
        static bool FilterNotItemPerms(Player pl, object arg) {
            return !Chat.FilterPerms(pl, arg);
        }
        
        static void DoBanIP(ModAction e) {
            LogIPAction(e, "&8IP banned");
            Logger.Log(LogType.UserActivity, "IP-BANNED: {0} by {1}.", e.Target, e.Actor.name);
            Server.bannedIP.Add(e.Target);
            Server.bannedIP.Save();
        }
        
        static void DoUnbanIP(ModAction e) {
            LogIPAction(e, "&8IP unbanned");
            Logger.Log(LogType.UserActivity, "IP-UNBANNED: {0} by {1}.", e.Target, e.Actor.name);
            Server.bannedIP.Remove(e.Target);
            Server.bannedIP.Save();
        }

        
        static void DoWarn(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) {
                LogAction(e, who, "&ewarned");
                if (who.warn == 0) {
                    who.Message("Do it again twice and you will get kicked!");
                } else if (who.warn == 1) {
                    who.Message("Do it one more time and you will get kicked!");
                } else if (who.warn == 2) {
                    Chat.MessageGlobal("{0} &Swas warn-kicked by {1}", who.ColoredName, e.Actor.ColoredName);
                    string chatMsg = "by " + e.Actor.ColoredName + "&S: " + e.Reason;
                    string kickMsg = "Kicked by " + e.Actor.ColoredName + ": &f" + e.Reason;
                    who.Kick(chatMsg, kickMsg);
                }
                who.warn++;
            } else {
                if (!Server.Config.LogNotes) {
                    e.Actor.Message("Notes logging must be enabled to warn offline players."); return;
                }
                LogAction(e, who, "&ewarned");
            }
        }
        
        
        static void DoRank(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            Group newRank = (Group)e.Metadata;
            string action = newRank.Permission >= e.TargetGroup.Permission ? "promoted to " : "demoted to ";
            LogAction(e, who, action + newRank.ColoredName);
            
            if (who != null && e.Announce) {
                who.Message("You are now ranked " + newRank.ColoredName + "&S, type /Help for your new set of commands.");
            }
            if (Server.tempRanks.Remove(e.Target)) {
                ModerationTasks.TemprankCalcNextRun();
                Server.tempRanks.Save();
            }
            
            WriteRankInfo(e, newRank);
            if (e.Duration != TimeSpan.Zero) AddTempRank(e, newRank);
            ModActionCmd.ChangeRank(e.Target, e.TargetGroup, newRank, who);
        }
        
        static void WriteRankInfo(ModAction e, Group newRank) {
            string assigner = e.Actor.name;
            long time = DateTime.UtcNow.ToUnixTime();

            string line = e.Target + " " + assigner + " " + time + " " + newRank.Name
                + " " + e.TargetGroup.Name + " " + e.Reason.Replace(" ", "%20");
            Server.RankInfo.Append(line);
        }
        
        static void AddTempRank(ModAction e, Group newRank) {
            string data = FormatModTaskData(e) + " " + e.TargetGroup.Name + " " + newRank.Name;
            Server.tempRanks.Update(e.Target, data);
            ModerationTasks.TemprankCalcNextRun();
            Server.tempRanks.Save();
        }
        
        static string FormatModTaskData(ModAction e) {
            long assign  = DateTime.UtcNow.ToUnixTime();
            DateTime end = DateTime.MaxValue.AddYears(-1);
            
            if (e.Duration != TimeSpan.Zero) {
                try {
                    end = DateTime.UtcNow.Add(e.Duration);
                } catch (ArgumentOutOfRangeException) {
                    // user provided extreme expiry time, ignore it
                }
            }
            
            long expiry = end.ToUnixTime();
            string assigner = e.Actor.name;
            return assigner + " " + assign + " " + expiry;
        }
    }
}
