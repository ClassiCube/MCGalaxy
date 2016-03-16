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
        
        public static string GetLoginMessage(Player p) {
            if (!Directory.Exists("text/login"))
                Directory.CreateDirectory("text/login");            
            string path = "text/login/" + p.name.ToLower() + ".txt";
            if (File.Exists(path)) return CP437Reader.ReadAllText(path); 
            // Unix is case sensitive (older files used correct casing of name)
            path = "text/login/" + p.name + ".txt";
            return File.Exists(path) ? CP437Reader.ReadAllText(path) : "joined the game.";
        }
        
        public static string GetLogoutMessage(Player p) {
            if (!Directory.Exists("text/logout"))
                Directory.CreateDirectory("text/logout");            
            string path = "text/logout/" + p.name.ToLower() + ".txt";
            if (File.Exists(path)) return CP437Reader.ReadAllText(path); 
            
            path = "text/logout/" + p.name + ".txt";
            return File.Exists(path) ? CP437Reader.ReadAllText(path) : "Disconnected";
        }
        
        public static void SetLoginMessage(string name, string value) {
        	CP437Writer.WriteAllText("text/login/" + name.ToLower() + ".txt", value);
        }
        
        public static void SetLogoutMessage(string name, string value) {
        	CP437Writer.WriteAllText("text/logout/" + name.ToLower() + ".txt", value);
        }
    }
}