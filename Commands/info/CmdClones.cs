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
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
	public sealed class CmdClones : Command
	{
		public override string name { get { return "clones"; } }
		public override string shortcut { get { return "alts"; } }
		public override string type { get { return CommandTypes.Information; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
		public CmdClones() { }

		public override void Use(Player p, string message)
		{
			if ( !Regex.IsMatch(message.ToLower(), @".*%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])%([0-9]|[a-f]|[k-r])") ) {
				if (Regex.IsMatch(message.ToLower(), @".*%([0-9]|[a-f]|[k-r])(.+?).*")) {
					Regex rg = new Regex(@"%([0-9]|[a-f]|[k-r])(.+?)");
					MatchCollection mc = rg.Matches(message.ToLower());
					if (mc.Count > 0) {
						Match ma = mc[0];
						GroupCollection gc = ma.Groups;
						message.Replace("%" + gc[1].ToString().Substring(1), "&" + gc[1].ToString().Substring(1));
					}
				}
			}
			if (message == "") message = p.name;

			string originalName = message.ToLower();

			Player who = Player.Find(message);
			if (who == null)
			{
				Player.SendMessage(p, "Could not find player. Searching Player DB.");

                Database.AddParams("@Name", message);
				DataTable FindIP = Database.fillData("SELECT IP FROM Players WHERE Name=@Name");

				if (FindIP.Rows.Count == 0) { Player.SendMessage(p, "Could not find any player by the name entered."); FindIP.Dispose(); return; }

				message = FindIP.Rows[0]["IP"].ToString();
				FindIP.Dispose();
			}
			else
			{
				message = who.ip;
			}
            Database.AddParams("@IP", message);
			DataTable Clones = Database.fillData("SELECT Name FROM Players WHERE IP=@IP");

			if (Clones.Rows.Count == 0) { Player.SendMessage(p, "Could not find any record of the player entered."); return; }

			List<string> foundPeople = new List<string>();
			for (int i = 0; i < Clones.Rows.Count; ++i)
			{
				if (!foundPeople.Contains(Clones.Rows[i]["Name"].ToString().ToLower()))
					foundPeople.Add(Clones.Rows[i]["Name"].ToString().ToLower());
			}

			Clones.Dispose();
			if (foundPeople.Count <= 1) { Player.SendMessage(p, originalName + " has no clones."); return; }

			Player.SendMessage(p, "These people have the same IP address:");
			Player.SendMessage(p, string.Join(", ", foundPeople.ToArray()));
		}

		public override void Help(Player p)
		{
			Player.SendMessage(p, "/clones <name> - Finds everyone with the same IP as <name>"); //Fixed typo
		}
	}
}
