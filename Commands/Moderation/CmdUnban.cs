/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands {
    
    public sealed class CmdUnban : Command {
        public override string name { get { return "unban"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdUnban() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            bool totalUnban = false;
            if (message[0] == '@') {
                totalUnban = true;
                message = message.Remove(0, 1).Trim();
            }

            Player who = PlayerInfo.Find(message);
            string name = who == null ? message : who.name;
            Unban(p, name, who);
            if (totalUnban)
                Command.all.Find("unbanip").Use(p, "@" + message);
        }
        
        void Unban(Player p, string name, Player who) {
            string srcFull = p == null ? "(console)" : p.ColoredName + "%S";
            string src = p == null ? "(console)" : p.name;
            
            if (Group.findPlayerGroup(name) != Group.findPerm(LevelPermission.Banned)) {
                foreach (Server.TempBan tban in Server.tempBans) {
                    if (!tban.name.CaselessEq(name)) continue;
                    
                    Server.tempBans.Remove(tban);
                    Player.GlobalMessage(name + " had their temporary ban lifted by " + srcFull + ".");
                    Server.s.Log("UNBANNED: by " + src);
                    Server.IRC.Say(name + " was unbanned by " + src + ".");
                    return;
                }
                Player.Message(p, "Player \"" + name + "\" is not banned.");
            } else {
                Player.GlobalMessage(name + " was &8(unbanned) %Sby " + srcFull + ".");
                Server.s.Log("UNBANNED: by " + src);
                Server.IRC.Say(name + " was unbanned by " + src + ".");
                
                Group.findPerm(LevelPermission.Banned).playerList.Remove(name);
                Group.findPerm(LevelPermission.Banned).playerList.Save();
                if (Ban.DeleteBan(name))
                    Player.Message(p, "Deleted ban information about " + name + ".");
                else
                    Player.Message(p, "No info found about " + name + ".");
                
                if (who != null) {
                    who.group = Group.standard; who.color = who.group.color;
                    Entities.GlobalDespawn(who, false);
                    Entities.GlobalSpawn(who, false);
                }
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/unban <player> - Unbans a player. This includes temporary bans.");
        }
    }
}
