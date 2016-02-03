/*
Copyright 2011 MCGalaxy
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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdTempRank : Command
    {
        public override string name { get { return "temprank"; } }
        public override string shortcut { get { return "tr"; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdTempRank() { }

        public override void Use(Player p, string message)
        {
            string player = "", rank = "", period = "";
            try
            {
                player = message.Split(' ')[0];
                rank = message.Split(' ')[1];
                period = message.Split(' ')[2];
            }
            catch
            {
                Help(p);
            }
           
            Player who = PlayerInfo.Find(player);
            
            
            if (player == "") { Player.SendMessage(p, "&cYou have to enter a player!"); return; }          
            if (who == null) { Player.SendMessage(p, "&cPlayer &a" + player + "&c not found!"); return; }
            if (rank == "") { Player.SendMessage(p, "&cYou have to enter a rank!"); return; }
            else
            {
                Group groupNew = Group.Find(rank);
                if (groupNew == null)
                {
                    Player.SendMessage(p, "&cRank &a" + rank + "&c does not exist");
                    return;
                }
            }
            if (period == "") { Player.SendMessage(p, "&cYou have to enter a time period!"); return; }
            Boolean isnumber = true;
            try
            {
                Convert.ToInt32(period);
            }
            catch
            {
                isnumber = false;
            }
            if (!isnumber)
            {
                Player.SendMessage(p, "&cThe period needs to be a number!");
                return;
            }

            string alltext = File.ReadAllText("text/tempranks.txt");
            if (alltext.Contains(player))
            {
                Player.SendMessage(p, "&cThe player already has a temporary rank assigned!");
                return;
            }
            bool byconsole;
            if (p == null)
            {
                byconsole = true;
                goto skipper;
            }
            else
            {
                byconsole = false;
            }
            if (player == p.name)
            {
                Player.SendMessage(p, "&cYou cannot assign yourself a temporary rank!");
                return;
            }
            Player who3 = PlayerInfo.Find(player);
            if (who3.group.Permission >= p.group.Permission)
            {
                Player.SendMessage(p, "Cannot change the temporary rank of someone equal or higher to yourself.");
                return;
            }
            Group newRank2 = Group.Find(rank);
            if (newRank2.Permission >= p.group.Permission)
            {
                Player.SendMessage(p, "Cannot change the temporary rank to a higher rank than yourself");
                return;
            }
        skipper:
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            string hour = DateTime.Now.Hour.ToString();
            string minute = DateTime.Now.Minute.ToString();
            string oldrank = who.group.name;
            string assigner;
            if (byconsole)
            {
                assigner = "Console";
            }
            else {
                assigner = p.name;
            }
        
            Boolean tryer = true;
            try
            {
                StreamWriter sw;
                sw = File.AppendText("text/tempranks.txt");
                sw.WriteLine(who.name + " " + rank + " " + oldrank + " " + period + " " + minute + " " + hour + " " + day + " " + month + " " + year + " " + assigner);
                sw.Close();
            }
            catch
            {
                tryer = false;
            }

            
                if (!tryer)
                {
                    Player.SendMessage(p, "&cAn error occurred!");
                }
                else
                {
                    Group newgroup = Group.Find(rank);
                    Command.all.Find("setrank").Use(null, who.name + " " + newgroup.name);
                    Player.SendMessage(p, "Temporary rank (" + rank + ") is assigned succesfully to " + player + " for " + period + " hours");
                    Player who2 = PlayerInfo.Find(player);
                    Player.SendMessage(who2, "Your Temporary rank (" + rank + ") is assigned succesfully for " + period + " hours");
                }
            
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/temprank <player> <rank> <period(hours)> - Sets a temporary rank for the specified player.");
        }
            
    }
}
