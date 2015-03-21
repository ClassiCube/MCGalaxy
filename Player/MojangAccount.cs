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
using System.IO;

namespace MCGalaxy
{
	/// <summary>
	/// Description of MojangAccount.
	/// </summary>
	public sealed class MojangAccount
	{
		static Dictionary<string, int> users = new Dictionary<string, int>();
		public static bool HasID(string truename) 
		{
			return GetID(truename) != -1;
		}
		
		public static int GetID(string truename)
		{
			if (users.ContainsKey(truename))
				return users[truename];
			return -1;
		}
		
		public static void AddUser(string truename) 
		{
			int i = users.Count;
			users.Add(truename, i);
			Save();
		}
		
		public static void Save() 
		{
			string[] lines = new string[users.Count];
			int i = 0; //because fuck forloops
			foreach (string s in users.Keys)
			{
				lines[i] = s + ":" + users[s];
				i++;
			}
			File.WriteAllLines("extra/mojang.dat", lines);
			lines = null;
		}
		
		public static void Load()
		{
			if (!File.Exists("extra/mojang.dat")) {
				File.Create("extra/mojang.dat");
				return;
			}
			string[] lines = File.ReadAllLines("extra/mojang.dat");
			foreach (string s in lines) 
			{
				int id = int.Parse(s.Split(':')[1]);
				string user = s.Split(':')[0];
				users.Add(user, id);
			}
			lines = null;
		}
	}
}
