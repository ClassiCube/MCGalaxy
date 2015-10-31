/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the	Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the License is distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace MCGalaxy
{
    public static class c
    {
        public const string black = "&0";
        public const string navy = "&1";
        public const string green = "&2";
        public const string teal = "&3";
        public const string maroon = "&4";
        public const string purple = "&5";
        public const string gold = "&6";
        public const string silver = "&7";
        public const string gray = "&8";
        public const string blue = "&9";
        public const string lime = "&a";
        public const string aqua = "&b";
        public const string red = "&c";
        public const string pink = "&d";
        public const string yellow = "&e";
        public const string white = "&f";

        public static string Parse(string str)
        {
            switch (str.ToLower())
            {
                case "black": return black;
                case "navy": return navy;
                case "green": return green;
                case "teal": return teal;
                case "maroon": return maroon;
                case "purple": return purple;
                case "gold": return gold;
                case "silver": return silver;
                case "gray": return gray;
                case "blue": return blue;
                case "lime": return lime;
                case "aqua": return aqua;
                case "red": return red;
                case "pink": return pink;
                case "yellow": return yellow;
                case "white": return white;
                default: return "";
            }
        }
        public static string Name(string str)
        {
            switch (str)
            {
                case black: return "black";
                case navy: return "navy";
                case green: return "green";
                case teal: return "teal";
                case maroon: return "maroon";
                case purple: return "purple";
                case gold: return "gold";
                case silver: return "silver";
                case gray: return "gray";
                case blue: return "blue";
                case lime: return "lime";
                case aqua: return "aqua";
                case red: return "red";
                case pink: return "pink";
                case yellow: return "yellow";
                case white: return "white";
                default: return "";
            }
        }

        public static byte MCtoIRC(string str)
        {
            switch (str)
            {
                case black: return 1;
                case navy: return 2;
                case green: return 3;
                case teal: return 10;
                case maroon: return 5;
                case purple: return 6;
                case gold: return 7;
                case silver: return 15;
                case gray: return 14;
                case blue: return 12;
                case lime: return 9;
                case aqua: return 11;
                case red: return 4;
                case pink: return 13;
                case yellow: return 8;
                case white: return 0;
                default: return 0;
            }
        }
        public static readonly Dictionary<string, string> MinecraftToIRCColors = new Dictionary<string, string> {
            { white, "\u000300" },
            { black, "\u000301" },
            { navy, "\u000302" },
            { green, "\u000303" },
            { red, "\u000304" },
            { maroon, "\u000305" },
            { purple, "\u000306" },
            { gold, "\u000307" },
            { yellow, "\u000308" },
            { lime, "\u000309" },
            { teal, "\u000310" },
            { aqua, "\u000311" },
            { blue, "\u000312" },
            { pink, "\u000313" },
            { gray, "\u000314" },
            { silver, "\u000315" },
        };
        static readonly Regex IrcTwoColorCode = new Regex("(\x03\\d{1,2}),\\d{1,2}");
        public static string IrcToMinecraftColors(string input)
        {
            if (input == null) throw new ArgumentNullException("input");
            input = IrcTwoColorCode.Replace(input, "$1");
            StringBuilder sb = new StringBuilder(input);
            foreach (var codePair in MinecraftToIRCColors)
            {
                sb.Replace(codePair.Value, codePair.Key);
            }
            sb.Replace("\u0003", white); // color reset
            sb.Replace("\u000f", white); // reset
            return sb.ToString();
        }
        public static string IRCtoMC(byte str)
        {
            switch (str)
            {
                case 0: return white;
                case 1: return black;
                case 2: return navy;
                case 3: return green;
                case 4: return red;
                case 5: return maroon;
                case 6: return purple;
                case 7: return gold;
                case 8: return yellow;
                case 9: return lime;
                case 10: return teal;
                case 11: return aqua;
                case 12: return blue;
                case 13: return pink;
                case 14: return gray;
                case 15: return silver;
                default: return "";
            }
        }
        public static string CraftIRCtoMC(byte str)
        {
            switch (str)
            {
                case 0: return white;
                case 1: return black;
                case 2: return navy;
                case 3: return green;
                case 4: return red;
                case 5: return maroon;
                case 6: return purple;
                case 7: return gold;
                case 8: return yellow;
                case 9: return lime;
                case 10: return teal;
                case 11: return aqua;
                case 12: return blue;
                case 13: return pink;
                case 14: return gray;
                case 15: return silver;
                default: return "";
            }
        }
        /// <summary> Replaces Minecraft color codes with equivalent IRC color codes, in the given StringBuilder.
        /// Opposite of IrcToMinecraftColors method. </summary>
        /// <param name="sb"> StringBuilder objects, the contents of which will be processed. </param>
        /// <exception cref="ArgumentNullException"> sb is null. </exception>
        public static void MinecraftToIrcColors(StringBuilder sb)
        {
            if (sb == null) throw new ArgumentNullException("sb");
            for (int i = 0; i < 10; i++)
            {
                sb.Replace("%" + i, "&" + i);
            }
            for (char ch = 'a'; ch <= 'f'; ch++)
            {
                sb.Replace("%" + ch, "&" + ch);
            }
            foreach (var codePair in MinecraftToIRCColors)
            {
                sb.Replace(codePair.Key, codePair.Value);
            }
        }


        /// <summary> Replaces Minecraft color codes with equivalent IRC color codes, in the given string.
        /// Opposite of IrcToMinecraftColors method. </summary>
        /// <param name="input"> String to process. </param>
        /// <returns> A processed string. </returns>
        /// <exception cref="ArgumentNullException"> input is null. </exception>
        public static string MinecraftToIrcColors(string input)
        {
            if (input == null) throw new ArgumentNullException("input");
            StringBuilder sb = new StringBuilder(input);
            MinecraftToIrcColors(sb);
            return sb.ToString();
        }
    }
}