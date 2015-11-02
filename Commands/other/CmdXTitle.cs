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
    public sealed class CmdXTitle : Command
    {
        public override string name { get { return "xtitle"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdXTitle() { }

        public override void Use(Player p, string message)
        { 
            string query;
            if (message == "") 
            { 
                p.title = "";
                p.SetPrefix();
                Player.GlobalChat(p, p.color + p.DisplayName + Server.DefaultColor + " had their title removed.", false);
                query = "UPDATE Players SET Title = '' WHERE Name = @Name";
                Database.AddParams("@Name", p.name);
                Database.executeQuery(query);
                return; 
            }
            int pos = message.IndexOf(' ');
            string newTitle = "";
            newTitle = message;

            if (newTitle != "")
            {
                newTitle = newTitle.ToString().Trim().Replace("[", "");
                newTitle = newTitle.Replace("]", "");
            }

            if (newTitle.Length > 17) { Player.SendMessage(p, "Title must be under 17 letters."); return; }

            if (newTitle != "")
                Player.GlobalChat(p, p.color + p.DisplayName + Server.DefaultColor + " gave themself the title of &b[" + newTitle + "%b]", false);
            else Player.GlobalChat(p, p.color + p.prefix + p.DisplayName + Server.DefaultColor + " had their title removed.", false);

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

            Database.AddParams("@Title", newTitle);
            Database.AddParams("@Name", p.name);
            query = "UPDATE Players SET Title = @Title WHERE Name = @Name";
            Database.executeQuery(query);
            p.title = newTitle;
            p.SetPrefix();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xtitle [title] - Gives you the [title].");
            Player.SendMessage(p, "If no [title] is given, your title is removed.");
        }
    }
}
