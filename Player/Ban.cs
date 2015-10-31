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
using System;
using System.IO;
using System.Text;

namespace MCGalaxy
{
	public static class Ban
	{
		/// <summary>
		/// with Ban you can check the info about someone's ban, find out if there's info about someone, and add / remove someone to the baninfo (NOT THE BANNED.TXT !)
		/// </summary>
		/// <param name="p">The player who executed the command</param>
		/// <param name="who">The player that's banned</param>
		/// <param name="reason">The reason for the ban</param>
		/// <param name="stealth">bool, to check if the ban is a stealth ban.</param>
		/// <param name="oldrank">The rank the who player used to have.</param>
		public static void Banplayer(Player p, string who, string reason, bool stealth, string oldrank)
		{
			// Getting date and time.
			string dayname = DateTime.Now.DayOfWeek.ToString();
			string daynumber = DateTime.Now.Day.ToString();
			string month = DateTime.Now.Month.ToString();
			string year = DateTime.Now.Year.ToString();
			string hour = DateTime.Now.Hour.ToString();
			string minute = DateTime.Now.Minute.ToString();
			// Creating date + time string that looks nice to read:
			string datetime = dayname + "%20" + daynumber + "%20" + month + "%20" + year + ",%20at%20" + hour + ":" + minute;
			// checking if p = player or console
			string player;
			if (p == null) player = "Console";
			else player = p.name.ToLower();
			// Checking stealth
			string stealthn;
			if (stealth) stealthn = "true";
			else stealthn = "false";
			if (reason == "") reason = "&c-";
			Write(player, who.ToLower(), reason, stealthn, datetime, oldrank);
		}
		static void Write(string pl, string whol, string reasonl, string stealthstr, string datetimel, string oldrankl)
		{
			if (!File.Exists("text/bans.txt"))
			{
				File.CreateText("text/bans.txt").Close();
			}
			File.AppendAllText("text/bans.txt", pl + " " + whol + " " + reasonl + " " + stealthstr + " " + datetimel + " " + oldrankl + "\r\n");
		}
        /// <summary>
        /// Checks if there's a ban record found with the specified username
        /// </summary>
        /// <param name="who">the Player's username to check</param>
        /// <returns>if the player is banned</returns>
		public static bool Isbanned(string who)
		{
			who = who.ToLower();
			foreach (string line in File.ReadAllLines("text/bans.txt"))
			{
				if (line.Split(' ')[1] == who) return true;
			}
			return false;
		}
        /// <summary>
        /// Gives info about the ban
        /// </summary>
        /// <param name="who">the username to check</param>
        /// <returns>A string array with { banned by, ban reason, stealth ban, date and time, previous rank } or if not found {"","","","",""}</returns>
		public static string[] Getbandata(string who)
		{
			who = who.ToLower();
			string bannedby = "", reason = "", timedate = "", oldrank = "", stealth = "";
			foreach (string line in File.ReadAllLines("text/bans.txt"))
			{
				if (line.Split(' ')[1] == who)
				{
					bannedby = line.Split(' ')[0];
					reason = line.Split(' ')[2];
					stealth = line.Split(' ')[3];
					timedate = line.Split(' ')[4];
					oldrank = line.Split(' ')[5];
				}
			}
			string[] end = { bannedby, reason, timedate, oldrank, stealth };
			return end;
		}
        /// <summary>
        /// Unbans a user
        /// </summary>
        /// <param name="name">username to unban</param>
        /// <returns>If the unban was successfull or not</returns>
		public static bool Deleteban(string name)
		{
			name = name.ToLower();
			bool success = false;
			StringBuilder sb = new StringBuilder();
			foreach (string line in File.ReadAllLines("text/bans.txt"))
			{
				if (line.Split(' ')[1] != name)
					sb.Append(line + "\r\n");
				else
					success = true;
			}
			File.WriteAllText("text/bans.txt", sb.ToString());
			return success;
		}
        /// <summary>
        /// Change the banreason for a specific player
        /// </summary>
        /// <param name="who">The username to edit the ban reason from</param>
        /// <param name="reason">The new banreason</param>
        /// <returns>empty string if succesfull, otherwise error message</returns>
		public static string Editreason(string who, string reason)
		{
			who = who.ToLower();
			bool found = false;
			string endproduct = "";
			if (Isbanned(who))
			{
				foreach (string line in File.ReadAllLines("text/bans.txt"))
				{
					if (line.Split(' ')[1] == who)
					{
						string replacethis = line.Split(' ')[2];
						string oldline = line;
						string newline = oldline.Replace(replacethis, reason);
						endproduct = endproduct + newline + "\r\n";
						found = true;
					}
					else
					{
						endproduct = endproduct + line + "\r\n";
					}
				}
				if (found)
				{
					File.WriteAllText("text/bans.txt", endproduct);
					return "";
				}
				else
				{
					return "Couldn't find baninfo about this player!";
				}
			}
			else
			{
				return "This player isn't banned!";
			}
		}
	}
}