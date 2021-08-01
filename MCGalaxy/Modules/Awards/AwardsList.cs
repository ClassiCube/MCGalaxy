/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)

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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Modules.Awards
{
    public class Award { public string Name, Description; }
	
    /// <summary> Manages the awards the server has </summary>
    public static class AwardsList 
    {  
        /// <summary> List of currently defined awards </summary>
        public static List<Award> Awards = new List<Award>();
        
        
        public static bool Add(string name, string desc) {
            if (Exists(name)) return false;

            Award award = new Award();
            award.Name  = name.Trim();
            award.Description = desc.Trim();
            Awards.Add(award);
            return true;
        }

        public static bool Remove(string name) {
            Award award = FindExact(name);
            if (award == null) return false;
            
            Awards.Remove(award);
            return true;
        }

        public static bool Exists(string name) { return FindExact(name) != null; }
        
        public static Award FindExact(string name) {
            foreach (Award award in Awards) {
                if (award.Name.CaselessEq(name)) return award;
            }
            return null;
        }
        
        
        static readonly object saveLock = new object();
        public static void Save() {
            lock (saveLock)
                using (StreamWriter w = new StreamWriter("text/awardsList.txt"))
            {
                WriteHeader(w);
                foreach (Award a in Awards) {
                    w.WriteLine(a.Name + " : " + a.Description);
                }
            }
        }
        
        public static void Load() {
            if (!File.Exists("text/awardsList.txt")) {
                using (StreamWriter w = new StreamWriter("text/awardsList.txt")) {
                    WriteHeader(w);
                    w.WriteLine("Gotta start somewhere : Built your first house");
                    w.WriteLine("Climbing the ladder : Earned a rank advancement");
                    w.WriteLine("Do you live here? : Joined the server a huge bunch of times");
                }
            }

            Awards = new List<Award>();
            PropertiesFile.Read("text/awardsList.txt", ProcessLine, ':');
        }
        
        static void ProcessLine(string key, string value) {
            if (value.Length == 0) return;
            Add(key, value);
        }
        
        static void WriteHeader(StreamWriter w) {
            w.WriteLine("#This is a full list of awards. The server will load these and they can be awarded as you please");
            w.WriteLine("#Format is:");
            w.WriteLine("# AwardName : Description of award goes after the colon");
            w.WriteLine();
        }
    }
}
