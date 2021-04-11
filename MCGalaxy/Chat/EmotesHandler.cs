using System;
using System.Collections.Generic;
using System.Text;

namespace MCGalaxy {
    
    public static class EmotesHandler {
        
        /// <summary> Mapping of emote keywords to unicode characters </summary>
        public static readonly Dictionary<string, char> Keywords = new Dictionary<string, char> {
            { "darksmile", '☺' },
            { "smile", '☻' },
            { "heart", '♥' }, { "hearts", '♥' },
            { "diamond", '♦' }, { "diamonds", '♦' }, { "rhombus", '♦' },
            { "club", '♣' }, { "clubs", '♣' }, { "clover", '♣' }, { "shamrock", '♣' },
            { "spade", '♠' }, { "spades", '♠' },
            { "*", '•' }, { "bullet", '•' }, { "dot", '•' }, { "point", '•' },
            { "hole", '◘' },
            { "circle", '○' }, { "o", '○' },
            { "inversecircle", '◙' },
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
            Dictionary<string, char> tokens = Keywords;
            int begIndex = message.IndexOf('(');
            if (begIndex == -1) return message;

            StringBuilder output  = new StringBuilder(message.Length);
            int lastAppendedIndex = 0;
            while (begIndex != -1) {
                int endIndex = message.IndexOf(')', begIndex + 1);
                if (endIndex == -1) break;

                bool escaped = false;
                for (int i = begIndex - 1; i >= 0 && message[i] == '\\'; i--) {
                    escaped = !escaped;
                }

                string keyword = message.Substring(begIndex + 1, endIndex - begIndex - 1);
                char substitute;
                if (tokens.TryGetValue(keyword.ToLowerInvariant(), out substitute))
                {
                    if (escaped) {
                        begIndex++;
                        output.Append(message, lastAppendedIndex, begIndex - lastAppendedIndex - 2);
                        lastAppendedIndex = begIndex - 1;
                    } else {
                        output.Append(message, lastAppendedIndex, begIndex - lastAppendedIndex);
                        output.Append(substitute);
                        begIndex = endIndex + 1;
                        lastAppendedIndex = begIndex;
                    }
                } else {
                    begIndex++;
                }
                begIndex = message.IndexOf('(', begIndex);
            }
            output.Append(message, lastAppendedIndex, message.Length - lastAppendedIndex);
            return output.ToString();
        }
    }
}
