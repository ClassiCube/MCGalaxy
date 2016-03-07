/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Data;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdWhowas : Command
    {
        public override string name { get { return "whowas"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override CommandPerm[] AdditionalPerms {
        	get { return new[] { new CommandPerm(LevelPermission.AdvBuilder, "The lowest rank which can see the target player's ip and if they are whitelisted") }; }
        } 
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            Player pl = PlayerInfo.Find(message);
            if (pl != null && Player.CanSee(p, pl)) {
                Player.SendMessage(p, pl.color + pl.name + " %Sis online, using /whois instead.");
                Command.all.Find("whois").Use(p, message);
                return;
            }

            if (message.IndexOf("'") != -1) { Player.SendMessage(p, "Cannot parse request."); return; }
            OfflinePlayer target = PlayerInfo.FindOffline(message, true);
            
            string plGroup = Group.findPlayer(message.ToLower());
            Group group = Group.Find(plGroup);
            if (target == null) { Player.SendMessage(p, group.color + message + " %Shas the rank of " + group.color + plGroup); return; }
            string color = target.color == "" ? group.color : target.color;
            string prefix = target.title == "" ? "" : "[" + target.titleColor + target.title + color + "] ";
            Player.SendMessage(p, color + prefix + target.name + " %Shas :");
            Player.SendMessage(p, "> > the rank of " + group.color + plGroup);
            
            Group nobody = Group.findPerm(LevelPermission.Nobody);
            if (nobody == null || (!nobody.commands.Contains("pay") && !nobody.commands.Contains("give") && !nobody.commands.Contains("take")))
                Player.SendMessage(p, "> > &a" + target.money + " %S" + Server.moneys);
                
            Player.SendMessage(p, "> > &cdied &a" + target.deaths + " %Stimes");
            Player.SendMessage(p, "> > &bmodified &a" + target.blocks + " &eblocks.");
            Player.SendMessage(p, "> > was last seen on &a" + target.lastLogin);
            Player.SendMessage(p, "> > " + TotalTime(target.totalTime));
            Player.SendMessage(p, "> > first logged into the server on &a" + target.firstLogin);
            Player.SendMessage(p, "> > logged in &a" + target.logins + " %Stimes, &c" + target.kicks + " %Sof which ended in a kick.");
            Player.SendMessage(p, "> > " + Awards.awardAmount(message) + " awards");
            if (Ban.IsBanned(message)) {
                string[] data = Ban.GetBanData(message);
                Player.SendMessage(p, "> > was banned by " + data[0] + " for " + data[1] + " on " + data[2]);
            }

            if (Server.Devs.ContainsInsensitive(message)) Player.SendMessage(p, "> > Player is a &9Developer");
            else if (Server.Mods.ContainsInsensitive(message)) Player.SendMessage(p, "> > Player is a &9MCGalaxy Moderator");
            else if (Server.GCmods.ContainsInsensitive(message)) Player.SendMessage(p, "> > Player is a &9Global Chat Moderator");

            if (p == null || (int)p.group.Permission >= CommandOtherPerms.GetPerm(this)) {
                if (Server.bannedIP.Contains(target.ip))
                    target.ip = "&8" + target.ip + ", which is banned";
                Player.SendMessage(p, "> > the IP of " + target.ip);
                if (Server.useWhitelist&& Server.whiteList.Contains(message))
                    Player.SendMessage(p, "> > Player is &fWhitelisted");
            }
        }
        
        string TotalTime(string time) {
        	TimeSpan value = time.ParseDBTime();
            return "time spent on server: " + value.Days + " Days, " + value.Hours + " Hours, " + value.Minutes + " Minutes, " + value.Seconds + " Seconds.";
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/whowas <name> - Displays information about someone who left.");
        }    
    }
}
