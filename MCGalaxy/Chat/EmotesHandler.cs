using System;
using System.Collections.Generic;
using System.Text;

namespace MCGalaxy {
    
    public static class EmotesHandler {
        
        public static readonly Dictionary<string, char> EmoteKeywords = new Dictionary<string, char> {
            { "darksmile", '☺' },
            { "smile", '☻' },
            { "heart", '♥' }, { "hearts", '♥' },
            { "diamond", '♦' }, { "diamonds", '♦' }, { "rhombus", '♦' },
            { "club", '♣' }, { "clubs", '♣' }, { "clover", '♣' }, { "shamrock", '♣' },
            { "spade", '♠' }, { "spades", '♠' },
            { "*", '•' }, { "bullet", '•' }, { "dot", '•' }, { "point", '•' },
            { "hole", '◘' },
            { "circle", '○' }, { "o", '○' },
            { "male", '♂' }, { "mars", '♂' },
            { "female", '♀' }, { "venus", '♀' },
            { "8", '♪' }, { "note", '♪' }, { "quaver", '♪' },
            { "notes", '♫' }, { "music", '♫' },
            { "sun", '☼' }, { "celestia", '☼' },
            { ">>", '►' }, { "right", '►' },
            { "<<", '◄' }, { "left", '◄' },
            { "updown", '↕' }, { "^v", '↕' },
            { "!!", '‼' },
            { "p", '¶' }, { "para", '¶' }, { "pilcrow", '¶' }, { "paragraph", '¶' },
            { "s", '§' }, { "sect", '§' }, { "section", '§' },
            { "-", '▬' }, { "_", '▬' }, { "bar", '▬' }, { "half", '▬' },
            { "updown2", '↨' }, { "^v_", '↨' },
            { "^", '↑' }, { "uparrow", '↑' },
            { "v", '↓' }, { "downarrow", '↓' },
            { "->", '→' }, { "rightarrow", '→' },
            { "<-", '←' }, { "leftarrow", '←' },
            { "l", '∟' }, { "angle", '∟' }, { "corner", '∟' },
            { "<>", '↔' }, { "<->", '↔' }, { "leftright", '↔' },
            { "^^", '▲' }, { "up", '▲' },
            { "vv", '▼' }, { "down", '▼' },
            { "house", '⌂' }
        };
        
        /// <summary> Conversion for code page 437 characters from index 0 to 31 to unicode. </summary>
        public const string ControlCharReplacements = "\0☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼";
        
        /// <summary> Conversion for code page 437 characters from index 127 to 255 to unicode. </summary>
        public const string ExtendedCharReplacements = "⌂ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜ¢£¥₧ƒáíóúñÑªº¿⌐¬½¼¡«»" +
            "░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌" +
            "█▄▌▐▀αßΓπΣσµτΦΘΩδ∞φε∩≡±≥≤⌠⌡÷≈°∙·√ⁿ²■\u00a0";
        
        public static string Replace(string message) {
            return Unescape(message, '(', ')', EmoteKeywords);
        }
        
        public static string Unescape(string message, char start, char end, 
                                      Dictionary<string, char> tokens)
        {
            if (message == null)
                throw new ArgumentNullException("message");
            int startIndex = message.IndexOf(start);
            if (startIndex == -1) return message;

            StringBuilder output = new StringBuilder(message.Length);
            int lastAppendedIndex = 0;
            while (startIndex != -1) {
                int endIndex = message.IndexOf(end, startIndex + 1);
                if (endIndex == -1)
                    break;

                bool escaped = false;
                for (int i = startIndex - 1; i >= 0 && message[i] == '\\'; i--) {
                    escaped = !escaped;
                }

                string keyword = message.Substring(startIndex + 1, endIndex - startIndex - 1);
                char substitute;
                if (tokens.TryGetValue(keyword.ToLowerInvariant(), out substitute))
                {
                    if (escaped) {
                        startIndex++;
                        output.Append(message, lastAppendedIndex, startIndex - lastAppendedIndex - 2);
                        lastAppendedIndex = startIndex - 1;
                    } else {
                        output.Append(message, lastAppendedIndex, startIndex - lastAppendedIndex);
                        output.Append(substitute);
                        startIndex = endIndex + 1;
                        lastAppendedIndex = startIndex;
                    }
                } else {
                    startIndex++;
                }
                startIndex = message.IndexOf(start, startIndex);
            }
            output.Append(message, lastAppendedIndex, message.Length - lastAppendedIndex);
            return output.ToString();
        }
    }
}
