/*
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
using System;
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy
{
    public sealed class Awards
    {
        public struct playerAwards { public string playerName; public List<string> awards; }
        public class awardData
        {
            public string awardName, description;
            public void setAward(string name) { awardName = camelCase(name); }
        }

        public static List<Awards.playerAwards> playersAwards = new List<Awards.playerAwards>();
        public static List<Awards.awardData> allAwards = new List<Awards.awardData>();

        public static void Load()
        {
            if (!File.Exists("text/awardsList.txt"))
            {
                using (StreamWriter SW = File.CreateText("text/awardsList.txt"))
                {
                    SW.WriteLine("#This is a full list of awards. The server will load these and they can be awarded as you please");
                    SW.WriteLine("#Format is:");
                    SW.WriteLine("# awardName : Description of award goes after the colon");
                    SW.WriteLine();
                    SW.WriteLine("Gotta start somewhere : Built your first house");
                    SW.WriteLine("Climbing the ladder : Earned a rank advancement");
                    SW.WriteLine("Do you live here? : Joined the server a huge bunch of times");
                }
            }

            allAwards = new List<awardData>();
            foreach (string s in File.ReadAllLines("text/awardsList.txt"))
            {
                if (s == "" || s[0] == '#') continue;
                if (s.IndexOf(" : ") == -1) continue;

                awardData aD = new awardData();

                aD.setAward(s.Split(new string[] { " : " }, StringSplitOptions.None)[0]);
                aD.description = s.Split(new string[] { " : " }, StringSplitOptions.None)[1];

                allAwards.Add(aD);
            }

            playersAwards = new List<playerAwards>();
            if (File.Exists("text/playerAwards.txt"))
            {
                foreach (String s in File.ReadAllLines("text/playerAwards.txt"))
                {
                    if (s.IndexOf(" : ") == -1) continue;

                    playerAwards pA;
                    pA.playerName = s.Split(new string[] { " : " }, StringSplitOptions.None)[0].ToLower();
                    string myAwards = s.Split(new string[] { " : " }, StringSplitOptions.None)[1];

                    pA.awards = new List<string>();
                    if (myAwards.IndexOf(',') != -1)
                        foreach (string a in myAwards.Split(','))
                            pA.awards.Add(camelCase(a));
                    else if (myAwards.Trim() != "")
                        pA.awards.Add(camelCase(myAwards));

                    playersAwards.Add(pA);
                }
            }

            Save();
        }

        public static void Save()
        {
            using (StreamWriter SW = File.CreateText("text/awardsList.txt"))
            {
                SW.WriteLine("#This is a full list of awards. The server will load these and they can be awarded as you please");
                SW.WriteLine("#Format is:");
                SW.WriteLine("# awardName : Description of award goes after the colon");
                SW.WriteLine();
                foreach (awardData aD in allAwards)
                    SW.WriteLine(camelCase(aD.awardName) + " : " + aD.description);
            }
            using (StreamWriter SW = File.CreateText("text/playerAwards.txt"))
            {
                foreach (playerAwards pA in playersAwards)
                    SW.WriteLine(pA.playerName.ToLower() + " : " + string.Join(",", pA.awards.ToArray()));
            }
        }

        public static bool giveAward(string playerName, string awardName)
        {
            foreach (playerAwards pA in playersAwards)
            {
                if (pA.playerName == playerName.ToLower())
                {
                    if (pA.awards.Contains(camelCase(awardName)))
                        return false;
                    pA.awards.Add(camelCase(awardName));
                    return true;
                }
            }

            playerAwards newPlayer;
            newPlayer.playerName = playerName.ToLower();
            newPlayer.awards = new List<string>();
            newPlayer.awards.Add(camelCase(awardName));
            playersAwards.Add(newPlayer);
            return true;
        }
        public static bool takeAward(string playerName, string awardName)
        {
            foreach (playerAwards pA in playersAwards)
            {
                if (pA.playerName == playerName.ToLower())
                {
                    if (!pA.awards.Contains(camelCase(awardName)))
                        return false;
                    pA.awards.Remove(camelCase(awardName));
                    return true;
                }
            }

            return false;
        }
        public static List<string> getPlayersAwards(string playerName)
        {
            foreach (playerAwards pA in playersAwards)
                if (pA.playerName == playerName.ToLower())
                    return pA.awards;

            return new List<string>();
        }
        public static string getDescription(string awardName)
        {
            foreach (awardData aD in allAwards)
                if (camelCase(aD.awardName) == camelCase(awardName))
                    return aD.description;

            return "";
        }
        public static string awardAmount(string playerName)
        {
            foreach (playerAwards pA in playersAwards)
                if (pA.playerName == playerName.ToLower())
                    return "&f" + pA.awards.Count + "/" + allAwards.Count + " (" + Math.Round((double)((double)pA.awards.Count / allAwards.Count) * 100, 2) + "%)" + Server.DefaultColor;

            return "&f0/" + allAwards.Count + " (0%)" + Server.DefaultColor;
        }
        public static bool addAward(string awardName, string awardDescription)
        {
            if (awardExists(awardName)) return false;

            awardData aD = new awardData();
            aD.awardName = camelCase(awardName);
            aD.description = awardDescription;
            allAwards.Add(aD);
            return true;
        }
        public static bool removeAward(string awardName)
        {
            foreach (awardData aD in allAwards)
            {
                if (camelCase(aD.awardName) == camelCase(awardName))
                {
                    allAwards.Remove(aD);
                    return true;
                }
            }
            return false;
        }
        public static bool awardExists(string awardName)
        {
            foreach (awardData aD in allAwards)
                if (camelCase(aD.awardName) == camelCase(awardName))
                    return true;

            return false;
        }


        public static string camelCase(string givenName)
        {
            string returnString = "";
            if (givenName != "")
                foreach (string s in givenName.Split(' '))
                    if (s.Length > 1)
                        returnString += s[0].ToString().ToUpper() + s.Substring(1).ToLower() + " ";
                    else
                        returnString += s.ToUpper() + " ";

            return returnString.Trim();
        }
    }
}
