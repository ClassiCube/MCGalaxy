/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MCGalaxy.Tasks;

namespace MCGalaxy {

    public class Pronouns {
        const string CONFIG_FILE = "properties/pronouns.properties";
        const string PLAYER_PATH = "text/pronouns/";

        static readonly object locker = new object();
        static List<Pronouns> Loaded = new List<Pronouns>();

        public static Pronouns Default;
        /// <summary>
        /// Called once to initialize the defaults and write/read the config file as necessary.
        /// </summary>
        public static void Init(SchedulerTask task) {

            if (!Directory.Exists(PLAYER_PATH)) {
                Directory.CreateDirectory(PLAYER_PATH);
            }

            Default = new Pronouns("default", "they", "their", "themselves", true);

            if (!File.Exists(CONFIG_FILE)) {

                Loaded.Add(new Pronouns("they/them", "they", "their", "themselves", true));
                Loaded.Add(new Pronouns("he/him", "he", "his", "himself", false));
                Loaded.Add(new Pronouns("she/her", "she", "her", "herself", false));

                using (StreamWriter w = new StreamWriter(CONFIG_FILE)) {
                    w.WriteLine("# Below are the pronouns that players may choose from by using /pronouns");
                    w.WriteLine("# Lines starting with # are ignored");
                    w.WriteLine("# Each pronouns is on its own line, and is formatted like so:");
                    w.WriteLine("# Name [subject form] [object form] [reflexive form] [singular or plural]");
                    w.WriteLine();
                    foreach (Pronouns p in Loaded) {
                        p.Write(w);
                    }
                }
                Logger.Log(LogType.SystemActivity, "CREATED NEW: " + CONFIG_FILE);
            }
            Events.ServerEvents.OnConfigUpdatedEvent.Register(OnConfigUpdated, Priority.Low);

            OnConfigUpdated();
        }
        static void OnConfigUpdated() {
            lock (locker) {
                Loaded.Clear();
                Loaded.Add(Default);

                try {
                    using (StreamReader r = new StreamReader(CONFIG_FILE)) {
                        while (!r.EndOfStream) {
                            string line = r.ReadLine();
                            if (string.IsNullOrEmpty(line) || line.StartsWith("#")) { continue; }
                            LoadFrom(line);
                        }
                    }
                } catch (Exception e) {
                    Logger.LogError(e);
                }
            }

            //In case any were deleted or changed
            foreach (Player p in PlayerInfo.Online.Items) {
                p.pronounsList = GetFor(p.name);
            }
        }
        static void LoadFrom(string line) {
            string[] words = line.ToLower().SplitSpaces();
            const int defWordCount = 5;
            if (words.Length != defWordCount) {
                Logger.Log(LogType.Warning, "Failed to load malformed pronouns \"{0}\" from config (expected {1} words, got {2}).", line, defWordCount, words.Length);
                return;
            }
            bool plural;
            if (words[4].CaselessEq("singular")) { plural = false; }
            else if (words[4].CaselessEq("plural")) { plural = true; }
            else {
                Logger.Log(LogType.Warning, "Failed to load the pronouns \"{0}\" because the 5th argument was not \"singular\" or \"plural\"", words[0]);
                return;
            }
            if (FindExact(words[0]) != null) {
                Logger.Log(LogType.Warning, "Cannot load pronouns \"{0}\" because it is already defined.", words[0]);
                return;
            }
            Loaded.Add(new Pronouns(words[0], words[1], words[2], words[3], plural));
        }

        static string PlayerPath(string playerName) { return PLAYER_PATH + playerName + ".txt"; }
        /// <summary>
        /// Find the pronouns associated with the playerName. Returns Default pronouns if none were specified.
        /// </summary>
        public static List<Pronouns> GetFor(string playerName) {
            string myPath = PlayerPath(playerName);
            try {

                string data;
                lock (locker) {
                    if (!File.Exists(myPath)) { return new List<Pronouns> { Default }; }
                    data = File.ReadAllText(myPath);
                }				
                data = data.Trim();		
                if (data.Length == 0) return new List<Pronouns> { Default };

                List<Pronouns> pros = new List<Pronouns>();
                string[] names = data.SplitSpaces();
                foreach (string name in names) {
                    Pronouns p = FindExact(name);
                    if (p == null) continue;
                    pros.Add(p);
                }
                if (pros.Count != 0) return pros;

                return new List<Pronouns> { Default };

            } catch (Exception e) {
                Logger.LogError(e);
                return new List<Pronouns> { Default };
            }
        }

        /// <summary>
        /// Returns the Pronoun with a name that caselessly matches the input. Returns null if no matches found.
        /// </summary>
        public static Pronouns FindExact(string name) {
            lock (locker) {
                foreach (Pronouns p in Loaded) {
                    if (name.CaselessEq(p.Name)) { return p; }
                }
            }
            return null;
        }
        /// <summary> Finds partial matches of 'name' against the list of all pronouns </summary>
        public static Pronouns FindMatch(Player p, string name) {
            int matches;
            lock(locker) {
                Pronouns pronouns = Matcher.Find(p, name, out matches, Loaded,
                                           null, pro => pro.Name, "pronouns");
                return pronouns;
            }
        }

        /// <summary>
        /// Returns a list of the names of all currently available pronouns.
        /// </summary>
        public static List<string> GetNames() {
            List<string> names = new List<string>();
            lock (locker) {
                foreach (Pronouns p in Loaded) {
                    names.Add(p.Name);
                }
            }
            return names;
        }
        public static string ListFor(Player p, string separator) {
            return p.pronounsList.Join((pro) => pro.Name, separator);
        }


        public readonly string Name;
        /// <summary>
        /// They/He/She/It
        /// </summary>
        public readonly string Subject;
        /// <summary>
        /// Their/His/Her/Its
        /// </summary>
        public readonly string Object;

        /// <summary>
        /// Themselves/Himself/Herself/Itself
        /// </summary>
        public readonly string Reflexive;

        //TODO: Third Person Singular Objective (Them/Him/Her)

        /// <summary>
        /// They/them uses "plural" style verbs, so this is required for grammar that sounds correct
        /// </summary>
        public readonly bool Plural;
        /// <summary>
        /// are, is
        /// </summary>
        public readonly string PresentVerb;
        /// <summary>
        /// were, was
        /// </summary>
        public readonly string PastVerb;
        /// <summary>
        /// have, has
        /// </summary>
        public readonly string PresentPerfectVerb;

        private Pronouns(string name, string subject, string @object, string reflexive, bool plural) {
            Name = name;
            Subject = subject;
            Object = @object;
            Reflexive = reflexive;
            Plural = plural;

            PresentVerb = Plural ? "are" : "is";
            PastVerb = Plural ? "were" : "was";
            PresentPerfectVerb = Plural ? "have" : "has";
        }
        void Write(StreamWriter w) {
            w.WriteLine(string.Format("{0} {1} {2} {3} {4}", Name, Subject, Object, Reflexive, (Plural ? "plural" : "singular") ));
            w.WriteLine();
        }
        public static void SaveFor(Player p) {
            string path = PlayerPath(p.name);
            try {
                //Reduce clutter by simply erasing the file if it's default
                if (p.pronounsList.Count == 1 && p.pronounsList[0] == Default) { File.Delete(path); return; }

                File.WriteAllText(path, ListFor(p, " "));

            } catch (Exception e) {
                Logger.LogError(e);
                p.Message("&WThere was an error when saving your pronouns: &S{0}", e.Message);
            }
        }
    }
}
