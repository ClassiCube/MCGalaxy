/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdUnban : Command {
        public override string name { get { return "unban"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdUnban() { }
        
        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            
            Player who = PlayerInfo.Find(args[0]);
            string name = who == null ? args[0] : who.name;
            string reason = args.Length > 1 ? args[1] : "(none given)";
            Unban(p, name, reason);
        }
        
        void Unban(Player p, string name, string reason) {
            string srcFull = p == null ? "(console)" : p.ColoredName + "%S";
            string src = p == null ? "(console)" : p.name;
            Group banned = Group.findPerm(LevelPermission.Banned);

            // Check tempbans first
            foreach (Server.TempBan tban in Server.tempBans) {
                if (!tban.name.CaselessEq(name)) continue;
                
                Server.tempBans.Remove(tban);
                Chat.MessageAll("{0} had their temporary ban lifted by {1}.", name, srcFull);
                Server.s.Log("UNBANNED: " + name + " by " + src);
                Server.IRC.Say(name + " was unbanned by " + src + ".");
                
                if (banned.playerList.Contains(name))
                    UnbanPlayer(p, name, src, srcFull, reason);
                return;
            }            
            
            int matches = 0;
            name = banned.playerList.FindMatches(p, name, "banned players", out matches);
            if (name == null) return;
            UnbanPlayer(p, name, src, srcFull, reason);
        }
        
        static void UnbanPlayer(Player p, string name, string src, string srcFull, string reason) {
            Chat.MessageAll("{0} was &8(unbanned) %Sby {1}.", name, srcFull);
            Server.s.Log("UNBANNED: " + name + " by " + src);
            Server.IRC.Say(name + " was unbanned by " + src + ".");
            
            Ban.UnbanPlayer(p, name, reason);
            Group banned = Group.findPerm(LevelPermission.Banned);
            Player who = PlayerInfo.Find(name);
            RankCmd.ChangeRank(name, banned, Group.standard, who, false);
            
            string ip = PlayerInfo.FindIP(name);
            if (ip != null && Server.bannedIP.Contains(ip))
                Player.Message(p, "NOTE: Their IP is still banned.");
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/unban <player> [reason]");
            Player.Message(p, "%HUnbans a player. This includes temporary bans.");
        }
    }
}
