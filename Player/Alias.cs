using System;
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy
{
    public class Alias
    {
        public static List<Alias> aliases = new List<Alias>();
        public static string levelsFile = "text/aliases.txt";

        public string trigger;
        public string Command;

        public Alias(string Trigger, string command)
        {
            trigger = Trigger;
            Command = command;
        }

        public static void Load()
        {
            aliases = new List<Alias>();
            if (File.Exists(levelsFile))
            {
                foreach (string line in File.ReadAllLines(levelsFile))
                {
                    if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                    {
                        aliases.Add(new Alias(line.Split(':')[0].Trim(), line.Split(':')[1].Trim()));
                    }
                }
            }
            else
            {
                Save();
            }
        }

        public static void Save()
        {
            StreamWriter sw = new StreamWriter(File.Create(levelsFile));
            sw.WriteLine("# The format goes Trigger : Command");
            foreach (Alias lvl in aliases)
            {
                sw.WriteLine(lvl.trigger + ":" + lvl.Command);
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        public static Alias Find(string id)
        {
            foreach (Alias alias in aliases)
            {
                if (alias.trigger == id)
                {
                    return alias;
                }
            }
            return null;
        }
    }
}
