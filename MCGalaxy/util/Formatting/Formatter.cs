﻿/*
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
using System.Text;
using MCGalaxy.Authentication;
using MCGalaxy.Blocks;
using MCGalaxy.Commands;

namespace MCGalaxy 
{
    public static class Formatter 
    { 
        public static void PrintCommandInfo(Player p, Command cmd) {
            p.Message("Usable by: " + cmd.Permissions.Describe());
            PrintAliases(p, cmd);
            
            List<CommandExtraPerms> extraPerms = CommandExtraPerms.FindAll(cmd.name);
            if (cmd.ExtraPerms == null) extraPerms.Clear();
            if (extraPerms.Count == 0) return;
            
            p.Message("&TExtra permissions:");
            foreach (CommandExtraPerms extra in extraPerms) 
            {
                p.Message("{0}) {1} {2}", extra.Num, extra.Describe(), extra.Desc);
            }
        }
        
        static void PrintAliases(Player p, Command cmd) {
            StringBuilder dst = new StringBuilder("Shortcuts: &T");
            if (!String.IsNullOrEmpty(cmd.shortcut)) {
                dst.Append('/').Append(cmd.shortcut).Append(", ");
            }
            FindAliases(Alias.coreAliases, cmd, dst);
            FindAliases(Alias.aliases, cmd, dst);
            
            if (dst.Length == "Shortcuts: &T".Length) return;
            p.Message(dst.ToString(0, dst.Length - 2));
        }
        
        static void FindAliases(List<Alias> aliases, Command cmd, StringBuilder dst) {
            foreach (Alias a in aliases) 
            {
                if (!a.Target.CaselessEq(cmd.name)) continue;
                
                dst.Append('/').Append(a.Trigger);
                if (a.Format == null) { dst.Append(", "); continue; }
                
                string name = String.IsNullOrEmpty(cmd.shortcut) ? cmd.name : cmd.shortcut;
                if (name.Length > cmd.name.Length) name = cmd.name;
                string args = a.Format.Replace("{args}", "[args]");
                
                dst.Append(" for /").Append(name + " " + args);
                dst.Append(", ");
            }
        }
        
        public static void MessageNeedMinPerm(Player p, string action, LevelPermission perm) {
            p.Message("Only {0}&S{1}", Group.GetColoredName(perm), action);
        }
        
        
        public static bool ValidName(Player p, string name, string type) {
            const string alphabet = Player.USERNAME_ALPHABET + "+"; // compatibility with ClassiCubeAccountPlus
            return IsValidName(p, name, type, alphabet);
        }
        
        public static bool ValidPlayerName(Player p, string name) {
            string alphabet = Player.USERNAME_ALPHABET + "+"; // compatibility with ClassiCubeAccountPlus
            
            foreach (AuthService service in AuthService.Services)
            {
                alphabet += service.NameSuffix;
            }
            return IsValidName(p, name, "player", alphabet);
        }
        
        public static bool IsValidName(Player p, string name, string type, string alphabet) {
            if (name.Length > 0 && name.ContainsAllIn(alphabet)) return true;
            p.Message("\"{0}\" is not a valid {1} name.", name, type);
            return false;
        }
        
        public static bool ValidMapName(Player p, string name) {
            if (LevelInfo.ValidName(name)) return true;
            p.Message("\"{0}\" is not a valid level name.", name);
            return false;
        }
        
        static char[] separators = { '/', '\\', ':' };
        static char[] invalid    = { '<', '>', '|', '"', '*', '?' };
        /// <summary> Checks that the input is a valid filename (non-empty and no directory separator) </summary>
        /// <remarks> If the input is invalid, messages the player the reason why </remarks>
        public static bool ValidFilename(Player p, string name) {
            if (string.IsNullOrEmpty(name)) {
                p.Message("&WFilename cannot be empty"); 
                return false;
            }
            
            if (name.IndexOfAny(separators) >= 0) {
                p.Message("&W\"{0}\" includes a directory separator (/, : or \\), which is not allowed", name);
                return false;
            }

            if (name.IndexOfAny(invalid) >= 0) {
                p.Message("&W\"{0}\" includes a prohibited character (<, >, |, \", *, or ?)", name);
                return false;
            }

            if (name.ContainsAllIn(".")) {
                p.Message("&W\"{0}\" cannot consist entirely of dot characters", name);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Sorts a list of strings such that a1 a2 a3 comes before a10 a11 a12 and so on.
    /// </summary>
    public class AlphanumComparator : IComparer<string> {

        // Simplified but based off of https://www.dotnetperls.com/alphanumeric-sorting
        public int Compare(string a, string b) {
            int result;
            int aLen, bLen;
            int aDigit = GetDigits(a, out aLen);
            int bDigit = GetDigits(b, out bLen);
            if (aDigit != -1 && bDigit != -1) {
                result = aDigit.CompareTo(bDigit);
            } else {
                result = a.Substring(0, aLen).CompareTo(b.Substring(0, bLen));
            }
            if (result != 0) return result;
            return a.Length - b.Length;
        }

        /// <summary>
        /// Returns the digits on the end of the string. -1 if no integer found.
        /// </summary>
        static int GetDigits(string name, out int nameLength) {
            nameLength = name.Length;
            if (!Char.IsDigit(name[name.Length - 1])) return -1;

            int decimalShift = 1;
            int number = 0;
            for (int i = name.Length - 1; i >= 0; i--) {
                if (!Char.IsDigit(name[i])) return number;

                nameLength--;
                int digit = name[i] - '0'; //Paige Ruten: here's the most insane way to convert a digit char to an integer 
                number += digit * decimalShift;
                decimalShift *= 10;
            }

            return number;
        }
    }
}
