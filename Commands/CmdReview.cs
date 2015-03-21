/*
    Written by BeMacized
    Assisted by RedNoodle
    
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
namespace MCGalaxy.Commands
{
    public sealed class CmdReview : Command
    {
        public override string name { get { return "review"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdReview() { }

        public override void Use(Player p, string message)
        {
            if (p != null && message == "")
            {
                message = "enter";
            }
            switch (message.ToLower())
            {
                case "enter":
                    if (p == null)
                    {
                        Player.SendMessage(p, "You can't execute this command as Console!");
                        return;
                    }
                    if (p.canusereview)
                    {
                        Group gre = Group.findPerm(Server.reviewenter);
                        if (gre == null)
                        {
                            Player.SendMessage(p, "There is something wrong with the system.  A message has been sent to the admin to fix");
                            Player.GlobalMessageAdmins(p.name + " tryed to use /review, but a system error occurred. Make sure your groups are formatted correctly");
                            Player.GlobalMessageAdmins("The group permission that is messed up is: " + Server.reviewenter.ToString() + " (" + (int)Server.reviewenter + ")");
                            return;
                        }
                        LevelPermission lpe = gre.Permission;
                        if (p.group.Permission >= lpe)
                        {
                            foreach (string testwho in Server.reviewlist)
                            {
                                if (testwho == p.name)
                                {
                                    Player.SendMessage(p, "You already entered the review queue!");
                                    return;
                                }
                            }

                            bool isopson = false;
                            try
                            {
                                foreach (Player pl in Player.players)
                                {
                                    if (pl.group.Permission >= Server.opchatperm && !pl.hidden)
                                    {
                                        isopson = true;
                                        break; // We're done, break out of this loop
                                    }
                                }
                            }
                            catch/* (Exception e)*/
                            {
                                isopson = true;
                            }
                            if (isopson == true)
                            {
                                Server.reviewlist.Add(p.name);
                                int reviewlistpos = Server.reviewlist.IndexOf(p.name);
                                if (reviewlistpos > 1) { Player.SendMessage(p, "You entered the &creview " + Server.DefaultColor + "queue. You have &c" + reviewlistpos.ToString() + Server.DefaultColor + " people in front of you in the queue"); }
                                if (reviewlistpos == 1) { Player.SendMessage(p, "You entered the &creview " + Server.DefaultColor + "queue. There is &c1 " + Server.DefaultColor + "person in front of you in the queue"); }
                                if ((reviewlistpos + 1) == 1) { Player.SendMessage(p, "You entered the &creview " + Server.DefaultColor + "queue. You are &cfirst " + Server.DefaultColor + "in line!"); }
                                Player.SendMessage(p, "The Online Operators have been notified. Someone should be with you shortly.");
                                Player.GlobalMessageOps(p.color + " - " + p.name + " - " + Server.DefaultColor + "entered the review queue");
                                if ((reviewlistpos + 1) > 1) { Player.GlobalMessageOps("There are now &c" + (reviewlistpos + 1) + Server.DefaultColor + " people waiting for &creview!"); }
                                else { Player.GlobalMessageOps("There is now &c1 " + Server.DefaultColor + "person waiting for &creview!"); }
                                p.ReviewTimer();
                            }
                            else
                            {
                                Player.SendMessage(p, "&cThere are no operators on to review your build. Please wait for one to come on and try again.");
                            }
                        }
                    }
                    else
                    {
                        Player.SendMessage(p, "You have to wait " + Server.reviewcooldown + " seconds everytime you use this command");
                    }
                    break;

                case "list":
                case "view":
                    if (p == null)
                    {
                        if (Server.reviewlist.Count != 0)
                        {
                            Player.SendMessage(p, "Players in the review queue:");
                            int viewnumb = 1;
                            foreach (string golist in Server.reviewlist)
                            {
                                string FoundRank = Group.findPlayer(golist.ToLower());
                                Player.SendMessage(p, viewnumb.ToString() + ". " + golist + " - Current Rank: " + FoundRank);
                                viewnumb++;
                            }
                        }
                        else
                        {
                            Player.SendMessage(p, "There are no players in the review queue!");
                        }
                        return;
                    }
                    Group grv = Group.findPerm(Server.reviewview);

                    if (grv == null)
                    {
                        Player.SendMessage(p, "There is something wrong with the system.  A message has been sent to the admin to fix");
                        Player.GlobalMessageAdmins(p.name + " tryed to use /review, but a system error occurred. Make sure your groups are formatted correctly");
                        Player.GlobalMessageAdmins("The group permission that is messed up is: " + Server.reviewview.ToString() + " (" + (int)Server.reviewview + ")");
                        return;
                    }

                    LevelPermission lpv = grv.Permission;
                    if (p.group.Permission >= lpv && p != null)
                    {
                        if (Server.reviewlist.Count != 0)
                        {
                            Player.SendMessage(p, "&9Players in the review queue:");
                            int viewnumb = 1;
                            foreach (string golist in Server.reviewlist)
                            {
                                string FoundRank = Group.findPlayer(golist.ToLower());
                                Player.SendMessage(p, "&a" + viewnumb.ToString() + ". &f" + golist + "&a - Current Rank: " + Group.Find(FoundRank).color + FoundRank);
                                viewnumb++;
                            }
                        }
                        else
                        {
                            Player.SendMessage(p, "There are no players in the review queue!");
                        }
                    }
                    break;

                case "leave":
                    if (p == null)
                    {
                        Player.SendMessage(p, "You can't execute this command as Console!");
                        return;
                    }
                    Group grl = Group.findPerm(Server.reviewleave);

                    if (grl == null)
                    {
                        Player.SendMessage(p, "There is something wrong with the system.  A message has been sent to the admin to fix");
                        Player.GlobalMessageAdmins(p.name + " tryed to use /review, but a system error occurred. Make sure your groups are formatted correctly");
                        Player.GlobalMessageAdmins("The group permission that is messed up is: " + Server.reviewleave.ToString() + " (" + (int)Server.reviewleave + ")");
                        return;
                    }

                    LevelPermission lpl = grl.Permission;
                    if (p.group.Permission >= lpl)
                    {
                        bool leavetest = false;
                        foreach (string testwho2 in Server.reviewlist)
                        {
                            if (testwho2 == p.name)
                            {
                                leavetest = true;
                            }
                        }
                        if (!leavetest)
                        {
                            Player.SendMessage(p, "You aren't in the review queue so you can't leave it!");
                            return;
                        }
                        Server.reviewlist.Remove(p.name);
                        int toallplayerscount = 1;
                        foreach (string toallplayers in Server.reviewlist)
                        {
                            Player tosend = Player.Find(toallplayers);
                            Player.SendMessage(tosend, "The review queue has changed. Your now on spot " + toallplayerscount.ToString() + ".");
                            toallplayerscount++;
                        }
                        Player.SendMessage(p, "You have left the review queue!");
                        return;
                    }
                    break;

                case "next":
                    if (p == null)
                    {
                        Player.SendMessage(p, "You can't execute this command as Console!");
                        return;
                    }
                    Group grn = Group.findPerm(Server.reviewnext);

                    if (grn == null)
                    {
                        Player.SendMessage(p, "There is something wrong with the system.  A message has been sent to the admin to fix");
                        Player.GlobalMessageAdmins(p.name + " tryed to use /review, but a system error occurred. Make sure your groups are formatted correctly");
                        Player.GlobalMessageAdmins("The group permission that is messed up is: " + Server.reviewnext.ToString() + " (" + (int)Server.reviewnext + ")");
                        return;
                    }

                    LevelPermission lpn = grn.Permission;
                    if (p.group.Permission >= lpn)
                    {
                        if (Server.reviewlist.Count == 0)
                        {
                            Player.SendMessage(p, "There are no players in the review queue!");
                            return;
                        }
                        string[] user = Server.reviewlist.ToArray();
                        Player who = Player.Find(user[0]);
                        if (who == null)
                        {
                            Player.SendMessage(p, "Player " + user[0] + " doesn't exist or is offline. " + user[0] + " has been removed from the review queue");
                            Server.reviewlist.Remove(user[0]);
                            return;
                        }
                        if (who == p)
                        {
                            Player.SendMessage(p, "You can't teleport to yourself! You have been removed from the review queue.");
                            Server.reviewlist.Remove(user[0]);
                            return;
                        }
                        Server.reviewlist.Remove(user[0]);
                        Command.all.Find("tp").Use(p, who.name);
                        Player.SendMessage(p, "You have been teleported to " + user[0]);
                        Player.SendMessage(who, "Your request has been answered by " + p.name + ".");
                        int toallplayerscount = 0;
                        foreach (string toallplayers in Server.reviewlist)
                        {
                            Player who2 = Player.Find(toallplayers);
                            Player.SendMessage(who2, "The review queue has been rotated. you now have " + toallplayerscount.ToString() + " players waiting in front of you");
                            toallplayerscount++;
                        }
                    }
                    else
                    {
                        Player.SendMessage(p, "&cYou have no permission to use the review queue!");
                    }
                    break;

                case "clear":
                    if (p == null)
                    {
                        Server.reviewlist.Clear();
                        Player.SendMessage(p, "The review queue has been cleared");
                        return;
                    }
                    Group grc = Group.findPerm(Server.reviewclear);

                    if (grc == null)
                    {
                        Player.SendMessage(p, "There is something wrong with the system.  A message has been sent to the admin to fix");
                        Player.GlobalMessageAdmins(p.name + " tryed to use /review, but a system error occurred. Make sure your groups are formatted correctly");
                        Player.GlobalMessageAdmins("The group permission that is messed up is: " + Server.reviewclear.ToString() + " (" + (int)Server.reviewclear + ")");
                        return;
                    }

                    LevelPermission lpc = grc.Permission;
                    if (p.group.Permission >= lpc)
                    {
                        Server.reviewlist.Clear();
                        Player.SendMessage(p, "The review queue has been cleared");
                        return;
                    }
                    else
                    {
                        Player.SendMessage(p, "&cYou have no permission to clear the Review Queue!");
                    }
                    break;
                default: Help(p); return;
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/review <enter/view/leave/next/clear> - Lets you enter, view, leave, or clear the reviewlist or teleport you to the next player in the review queue.");
        }
    }
}
