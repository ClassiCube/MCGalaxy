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
using System;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdSetRank : Command
    {
        public override string name { get { return "setrank"; } }
        public override string shortcut { get { return "rank"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdSetRank() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            string[] args = message.Split(trimChars, 3);
            if (args.Length < 2) { Help(p); return; }
            Player who = Player.Find(args[0]);
            Group newRank = Group.Find(args[1]);

            string reason = args.Length > 2 ? args[2] : "Congratulations!";
            if (newRank == null) { Player.SendMessage(p, "Could not find specified rank."); return; }

            Group bannedGroup = Group.findPerm(LevelPermission.Banned);
            string rankMsg;
            string rankReason = "%S. (" + reason + ")";
            if (who == null) {
                Group group = Group.findPlayerGroup(args[0]);
                if (!ChangeRank(args[0], group, newRank, null, p, reason)) return;
                
                rankMsg = args[0] + " &f(offline)" + "%S's rank was set to " + newRank.color + newRank.name + rankReason;
                Player.GlobalMessage(rankMsg);
            } else if (who == p) {
                Player.SendMessage(p, "Cannot change your own rank."); return;
            } else {
                if (!ChangeRank(who.name, who.group, newRank, who, p, reason)) return;

                rankMsg = who.FullName + "%S's rank was set to " + newRank.color + newRank.name + rankReason;
                Player.GlobalMessage(rankMsg);
                
                string oldcolor = who.group.color;
                who.group = newRank;
                if (who.color == "" || who.color == oldcolor)
                    who.color = who.group.color;
                who.SetPrefix();
                Player.GlobalDespawn(who, false);

                who.SendMessage("You are now ranked " + newRank.color + newRank.name + Server.DefaultColor + ", type /help for your new set of commands.");
                who.SendUserType(Block.canPlace(who.group.Permission, Block.blackrock));
                
                Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
            }
            Server.IRC.Say(rankMsg);
        }
        
        bool ChangeRank(string name, Group group, Group newRank, Player who, Player p, string reason) {
            Group banned = Group.findPerm(LevelPermission.Banned);
            if (group == banned || newRank == banned) {
                Player.SendMessage(p, "Cannot change the rank to or from \"" + banned.name + "\"."); return false;
            }
            if (p != null && (group.Permission >= p.group.Permission || newRank.Permission >= p.group.Permission)) {
                Player.SendMessage(p, "Cannot change the rank of someone of or to a rank equal or higher to yours."); return false;
            }
            
            if (who != null) {
                Group.because(who, newRank);
                if (Group.cancelrank) {
                    Group.cancelrank = false; return false;
                }
            }
            
            group.playerList.Remove(name);
            group.playerList.Save();
            newRank.playerList.Add(name);
            newRank.playerList.Save();
            
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            string hour = DateTime.Now.Hour.ToString();
            string minute = DateTime.Now.Minute.ToString();
            string assigner = p == null ? "(console)" : p.name;

            string line = name + " " + assigner + " " + minute + " " + hour + " " + day + " " + month
            	+ " " + year + " " + newRank.name + " " + group.name + " " + reason.Replace(" ", "%20");
            using (StreamWriter sw = File.AppendText("text/rankinfo.txt"))
                sw.WriteLine(CP437Writer.ConvertToUnicode(line));
            return true;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/rank <player> <rank> <yay> - Sets or returns a players rank.");
            Player.SendMessage(p, "Valid Ranks are: " + Group.concatList(true, true));
            Player.SendMessage(p, "<yay> is a celebratory message");
        }
    }
}
