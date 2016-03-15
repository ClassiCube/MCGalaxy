using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCGalaxy {
    
    public static class PlayerDB {
        
        static char[] trimChars = {'='};
        public static bool Load( Player p ) {
            if (!File.Exists("players/" + p.name + "DB.txt")) {
                Save(p); return false;
            }
            
            foreach (string line in File.ReadAllLines( "players/" + p.name + "DB.txt")) {
                if (string.IsNullOrEmpty(line) || line[0] == '#') continue;
                string[] parts = line.Split(trimChars, 2);
                if (parts.Length < 2) continue;             
                string key = parts[0].Trim(), value = parts[1].Trim();

                if (key.CaselessEquals("nick"))
                    p.DisplayName = value;
                p.timeLogged = DateTime.Now;
            }
            p.SetPrefix();
            return true;
        }

        public static void Save(Player p) {
            using (StreamWriter sw = new StreamWriter("players/" + p.name + "DB.txt", false))
                sw.WriteLine("Nick = " + p.DisplayName);
        }
    }
}