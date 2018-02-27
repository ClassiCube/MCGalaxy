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
        
        static void LogAction(ModAction e, Player who, string action) {
            if (e.Announce) {
                if (who != null) {
                    Chat.MessageGlobal(who, e.FormatMessage(e.TargetName, action), false);
                } else {
                    Chat.MessageGlobal(e.FormatMessage(e.TargetName, action));
                }
            } else {
                Chat.MessageOps(e.FormatMessage(e.TargetName, action));            
            }
            
            action = Colors.Strip(action);
            string suffix = "";
            if (e.Duration.Ticks != 0) suffix = " %Sfor " + e.Duration.Shorten();
            
            Logger.Log(LogType.UserActivity, "{0} was {1} by {2}", 
                       e.Target, action, e.ActorName + suffix);
        }

        
        static void DoFreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = true;
            LogAction(e, who, "&bfrozen");

            Server.frozen.AddOrReplace(e.Target, FormatModTaskData(e));
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
            
            Server.muted.AddOrReplace(e.Target, FormatModTaskData(e));
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
                string banner = e.Actor == null ? "(console)" : e.Actor.truename;
                DateTime end = DateTime.UtcNow.Add(e.Duration);
                Server.tempBans.AddOrReplace(e.Target, Ban.PackTempBanData(e.Reason, banner, end));
                Server.tempBans.Save();

                if (who != null) who.Kick("Banned for " + e.Duration.Shorten(true) + "." + e.ReasonSuffixed);
            } else {
                if (who != null) who.color = "";
                Ban.DeleteBan(e.Target);
                Ban.BanPlayer(e.Actor, e.Target, e.Reason, !e.Announce, e.TargetGroup.Name);
                ModActionCmd.ChangeRank(e.Target, e.targetGroup, Group.BannedRank, who);
                                
                if (who != null) {
                    string msg = e.Reason.Length == 0 ? ServerConfig.DefaultBanMessage : e.Reason;
                    who.Kick("Banned by " + e.ActorName + ": " + msg);
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
            ModActionCmd.ChangeRank(e.Target, Group.BannedRank, Group.standard, who, false);
            
            string ip = PlayerInfo.FindIP(e.Target);
            if (ip != null && Server.bannedIP.Contains(ip))
                Player.Message(e.Actor, "NOTE: Their IP is still banned.");
        }
        
        
        static void DoBanIP(ModAction e) {
            LevelPermission perm = CommandExtraPerms.MinPerm("whois");
            Chat.MessageWhere(e.FormatMessage("An IP", "&8IP banned"), pl => pl.Rank < perm);
            Chat.MessageWhere(e.FormatMessage(e.TargetName, "&8IP banned"), pl => pl.Rank >= perm);
            
            Logger.Log(LogType.UserActivity, "IP-BANNED: {0} by {1}.", e.Target, e.ActorName);
            Server.bannedIP.Add(e.Target);
            Server.bannedIP.Save();
        }
        
        static void DoUnbanIP(ModAction e) {
            LevelPermission perm = CommandExtraPerms.MinPerm("whois");
            Chat.MessageWhere(e.FormatMessage("An IP", "&8IP unbanned"), pl => pl.Rank < perm);
            Chat.MessageWhere(e.FormatMessage(e.TargetName, "&8IP unbanned"), pl => pl.Rank >= perm);
            
            Logger.Log(LogType.UserActivity, "IP-UNBANNED: {0} by {1}.", e.Target, e.ActorName);
            Server.bannedIP.Remove(e.Target);
            Server.bannedIP.Save();
        }

        
        static void DoWarn(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) {
                LogAction(e, who, "&ewarned");
                if (who.warn == 0) {
                    Player.Message(who, "Do it again twice and you will get kicked!");
                } else if (who.warn == 1) {
                    Player.Message(who, "Do it one more time and you will get kicked!");
                } else if (who.warn == 2) {
                    Chat.MessageGlobal("{0} %Swas warn-kicked by {1}", who.ColoredName, e.ActorName);
                    string chatMsg = "by " + e.ActorName + "%S: " + e.Reason;
                    string kickMsg = "Kicked by " + e.ActorName + "&f: " + e.Reason;
                    who.Kick(chatMsg, kickMsg);
                }
                who.warn++;
            } else {
                if (!ServerConfig.LogNotes) {
                    Player.Message(e.Actor, "Notes logging must be enabled to warn offline players."); return;
                }
                LogAction(e, who, "&ewarned");
            }
        }
        
        
        static void DoRank(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            Group newRank = (Group)e.Metadata;
            string action = newRank.Permission >= e.TargetGroup.Permission ? "promoted to " : "demoted to ";
            LogAction(e, who, action + newRank.ColoredName);
            
            if (who != null) {
                who.SendMessage("You are now ranked " + newRank.ColoredName + "%S, type /Help for your new set of commands.");
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
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            string hour = DateTime.Now.Hour.ToString();
            string minute = DateTime.Now.Minute.ToString();
            string assigner = e.Actor == null ? "(console)" : e.Actor.name;

            string line = e.Target + " " + assigner + " " + minute + " " + hour + " " + day + " " + month
                + " " + year + " " + newRank.Name + " " + e.TargetGroup.Name + " " + e.Reason.Replace(" ", "%20");
            Server.RankInfo.Append(line);
        }
        
        static void AddTempRank(ModAction e, Group newRank) {
            string data = FormatModTaskData(e) + " " + e.TargetGroup.Name + " " + newRank.Name;
            Server.tempRanks.AddOrReplace(e.Target, data);
            ModerationTasks.TemprankCalcNextRun();
            Server.tempRanks.Save();
        }
        
        static string FormatModTaskData(ModAction e) {
            long assign = DateTime.UtcNow.ToUnixTime();
            DateTime expiryTime;
            
            if (e.Duration == TimeSpan.Zero) {
                expiryTime = DateTime.MaxValue;
            } else {
                try {
                    expiryTime = DateTime.UtcNow.Add(e.Duration);
                } catch (ArgumentOutOfRangeException) {
                    // user provided extreme expiry time
                    expiryTime = DateTime.MaxValue;
                }
            }
            
            long expiry = expiryTime.ToUnixTime();
            string assigner = e.Actor == null ? "(console)" : e.Actor.name;
            return assigner + " " + assign + " " + expiry;
        }
    }
}
