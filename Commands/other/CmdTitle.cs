using System.Text.RegularExpressions;
/*
	Copyright 2011 MCGalaxy
		
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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdTitle : Command
    {
        public override string name { get { return "title"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdTitle() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            int pos = message.IndexOf(' ');
            Player who = Player.Find(message.Split(' ')[0]);
            if (who == null) { Player.SendMessage(p, "Could not find player."); return; }
            if (p != null && who.group.Permission > p.group.Permission)
            {
                Player.SendMessage(p, "Cannot change the title of someone of greater rank");
                return;
            }
			if (who != p && (int)p.group.Permission >= CommandOtherPerms.GetPerm(this))
            {
            	string query;
            	string newTitle = "";
            	if (message.Split(' ').Length > 1) newTitle = message.Substring(pos + 1);
            	else
            	{
                	who.title = "";
                	who.SetPrefix();
                	Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " had their title removed.", false);
                	query = "UPDATE Players SET Title = '' WHERE Name = @Name";
                	Database.AddParams("@Name", who.name);
                	Database.executeQuery(query);
                	return;
            	}

            	if (newTitle != "")
            	{ //remove the brackets from the given title
                	newTitle = newTitle.ToString().Trim().Replace("[", "");
                	newTitle = newTitle.Replace("]", "");
            	}

            	if (newTitle.Length > 17) { Player.SendMessage(p, "Title must be under 17 letters."); return; }


            /*string title = newTitle.ToLower();
            foreach (char c in Server.ColourCodesNoPercent) { title = title.Replace("%" + c, ""); title = title.Replace("&" + c, ""); }
            foreach (string occur in Server.BadTitles) {
                if (title.Contains(occur)) { Player.SendMessage(p, "%cYou're not a developer! Stop pretending you are!"); return; }
            }*/


            	if (newTitle != "")
                	Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " was given the title of &b[" + newTitle + "%b]", false);
            	else Player.GlobalChat(who, who.color + who.prefix + who.name + Server.DefaultColor + " had their title removed.", false);

            	if (!Regex.IsMatch(newTitle.ToLower(), @".*%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])"))
            	{
                	if (Regex.IsMatch(newTitle.ToLower(), @".*%([0-9]|[a-f]|[k-r])(.+?).*"))
                	{
                    	Regex rg = new Regex(@"%([0-9]|[a-f]|[k-r])(.+?)");
                    	MatchCollection mc = rg.Matches(newTitle.ToLower());
                    	if (mc.Count > 0)
                    	{
                        	Match ma = mc[0];
                        	GroupCollection gc = ma.Groups;
                        	newTitle.Replace("%" + gc[1].ToString().Substring(1), "&" + gc[1].ToString().Substring(1));
                    	}
                	}
            	}

            	if (newTitle == "")
            	{
                	query = "UPDATE Players SET Title = '' WHERE Name = @Name";
                	Database.AddParams("@Name", who.name);
            	}
            	else
            	{
                	query = "UPDATE Players SET Title = @Title WHERE Name = @Name";
                	Database.AddParams("@Title", newTitle);
                	Database.AddParams("@Name", who.name);
            	}
            	Database.executeQuery(query);
            	who.title = newTitle;
            	who.SetPrefix();
            }
            else if (who == p)
            {
                string query;
                string newTitle = "";
                if (message.Split(' ').Length > 1) newTitle = message.Substring(pos + 1);
                else
                {
                    p.title = "";
                    p.SetPrefix();
                    Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " had their title removed.", false);
                    query = "UPDATE Players SET Title = '' WHERE Name = @Name";
                    Database.AddParams("@Name", p.name);
                    Database.executeQuery(query);
                    return;
                }

                if (newTitle != "")
                { //remove the brackets from the given title
                    newTitle = newTitle.ToString().Trim().Replace("[", "");
                    newTitle = newTitle.Replace("]", "");
                }

                if (newTitle.Length > 17) { Player.SendMessage(p, "Title must be under 17 letters."); return; }


                /*string title = newTitle.ToLower();
                foreach (char c in Server.ColourCodesNoPercent) { title = title.Replace("%" + c, ""); title = title.Replace("&" + c, ""); }
                foreach (string occur in Server.BadTitles) {
                    if (title.Contains(occur)) { Player.SendMessage(p, "%cYou're not a developer! Stop pretending you are!"); return; }
                }*/


                if (newTitle != "")
                    Player.GlobalChat(who, who.color + who.name + Server.DefaultColor + " was given the title of &b[" + newTitle + "%b]", false);
                else Player.GlobalChat(who, who.color + who.prefix + who.name + Server.DefaultColor + " had their title removed.", false);

                if (!Regex.IsMatch(newTitle.ToLower(), @".*%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])"))
                {
                    if (Regex.IsMatch(newTitle.ToLower(), @".*%([0-9]|[a-f]|[k-r])(.+?).*"))
                    {
                        Regex rg = new Regex(@"%([0-9]|[a-f]|[k-r])(.+?)");
                        MatchCollection mc = rg.Matches(newTitle.ToLower());
                        if (mc.Count > 0)
                        {
                            Match ma = mc[0];
                            GroupCollection gc = ma.Groups;
                            newTitle.Replace("%" + gc[1].ToString().Substring(1), "&" + gc[1].ToString().Substring(1));
                        }
                    }
                }

                if (newTitle == "")
                {
                    query = "UPDATE Players SET Title = '' WHERE Name = @Name";
                    Database.AddParams("@Name", p.name);
                }
                else
                {
                    query = "UPDATE Players SET Title = @Title WHERE Name = @Name";
                    Database.AddParams("@Title", newTitle);
                    Database.AddParams("@Name", p.name);
                }
                Database.executeQuery(query);
                p.title = newTitle;
                p.SetPrefix();
            }
            else
            {
                Player.SendMessage(p, "Cannot change the title of someone else");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/title <player> [title] - Gives <player> the [title].");
            Player.SendMessage(p, "If no [title] is given, the player's title is removed.");
        }
    }
}
