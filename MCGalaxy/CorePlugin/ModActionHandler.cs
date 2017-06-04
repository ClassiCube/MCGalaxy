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
                    case ModActionType.Jailed: DoJail(action); break;
                    case ModActionType.Unjailed: DoUnjail(action); break;
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
            if (who != null) {
                Chat.MessageGlobal(who, e.FormatMessage(e.TargetName, action), false);
            } else {
                Chat.MessageGlobal(e.FormatMessage(e.TargetName, action));
            }
            
            action = Colors.StripColors(action);
            string suffix = "";
            if (e.Duration.Ticks != 0) suffix = " %Sfor " + e.Duration.Shorten();
            Server.s.Log(e.Target + " was " + action + " by " + e.ActorName + suffix);
        }

        
        static void DoFreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = true;
            LogAction(e, who, "&bfrozen");

            Server.frozen.AddIfNotExists(e.Target);
            Server.frozen.Save();
        }
        
        static void DoUnfreeze(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who != null) who.frozen = false;
            LogAction(e, who, "&adefrosted");
            
            Server.frozen.Remove(e.Target);
            Server.frozen.Save();
        }
        
        
        static void DoJail(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who == null) return;
            
            who.jailed = true;
            Server.jailed.AddOrReplace(who.name, who.level.name);
            LogAction(e, who, "&8jailed");
            
            Entities.GlobalDespawn(who, false);
            Position pos = new Position(who.level.jailx, who.level.jaily, who.level.jailz);
            Orientation rot = new Orientation(who.level.jailrotx, who.level.jailroty);
            Entities.GlobalSpawn(who, pos, rot, true);
            Server.jailed.Save(true);
        }
        
        static void DoUnjail(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            if (who == null) return;
            
            who.jailed = false;
            Server.jailed.Remove(who.name);
            LogAction(e, who, "&afreed from jail");
            
            Command.all.Find("spawn").Use(who, "");
            Server.jailed.Save(true);
        }
        
        
        static void DoMute(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            Server.muted.AddIfNotExists(e.Target);
            Server.muted.Save();
            
            if (who != null) who.muted = true;
            LogAction(e, who, "&8muted");
        }
        
        static void DoUnmute(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            Server.muted.Remove(e.Target);
            Server.muted.Save();
            
            if (who != null) who.muted = false;
            LogAction(e, who, "&aun-muted");
        }
        
        
        static void DoBan(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            LogAction(e, who, "&8banned");
            
            if (e.Duration.Ticks != 0) {
                string banner = e.Actor == null ? "(console)" : e.Actor.truename;
                DateTime end =  DateTime.UtcNow.Add(e.Duration);
                Server.tempBans.AddOrReplace(e.Target, Ban.PackTempBanData(e.Reason, banner, end));
                Server.tempBans.Save();

                if (who != null) who.Kick("Banned for " + e.Duration.Shorten(true) + "." + e.ReasonSuffixed);
            } else {
                if (who != null) who.color = "";
                Ban.DeleteBan(e.Target);
                Ban.BanPlayer(e.Actor, e.Target, e.Reason, false, e.TargetGroup.name);
                ModActionCmd.ChangeRank(e.Target, e.targetGroup, Group.BannedRank, who);
            }
        }
        
        static void DoUnban(ModAction e) {
            Player who = PlayerInfo.FindExact(e.Target);
            LogAction(e, who, "&8unbanned");
            
            if (Server.tempBans.Remove(e.Target)) {
                Server.tempBans.Save();
            }
            if (!Group.BannedRank.playerList.Contains(e.Target)) return;
            
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
            
            Server.s.Log("IP-BANNED: " + e.Target + " by " + e.ActorName + ".");
            Server.bannedIP.Add(e.Target);
            Server.bannedIP.Save();
        }
        
        static void DoUnbanIP(ModAction e) {
            LevelPermission perm = CommandExtraPerms.MinPerm("whois");
            Chat.MessageWhere(e.FormatMessage("An IP", "&8IP unbanned"), pl => pl.Rank < perm);
            Chat.MessageWhere(e.FormatMessage(e.TargetName, "&8IP unbanned"), pl => pl.Rank >= perm);
            
            Server.s.Log("IP-UNBANNED: " + e.Target + " by " + e.ActorName + ".");
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
                if (!Server.LogNotes) {
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
                who.SendMessage("You are now ranked " + newRank.ColoredName + "%S, type /help for your new set of commands.");
            }
            if (Server.tempRanks.Remove(e.Target)) {
                ServerTasks.TemprankCalcNextRun();
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
                + " " + year + " " + newRank.name + " " + e.TargetGroup.name + " " + e.Reason.Replace(" ", "%20");
            Server.RankInfo.Append(line);
        }
        
        static void AddTempRank(ModAction e, Group newRank) {
            long assign = DateTime.UtcNow.ToUnixTime();
            long expiry = DateTime.UtcNow.Add(e.Duration).ToUnixTime();
            string assigner = e.Actor == null ? "(console)" : e.Actor.name;

            string data = assigner + " " + assign + " " + expiry + " " + e.TargetGroup.name + " " + newRank.name;
            Server.tempRanks.AddOrReplace(e.Target, data);
            ServerTasks.TemprankCalcNextRun();
            Server.tempRanks.Save();
        }
    }
}
