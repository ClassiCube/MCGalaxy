using System;
using System.Collections.Generic;
using System.Text;

namespace MCGalaxy
{
    public static class FullCP437Handler
    {
        public static readonly Dictionary<string, char> Replacements = new Dictionary<string, char> {
            { "big-c-cedilla", 'Ç' },  
            { "small-u-diaeresis", 'ü' },  
            { "small-e-acute", 'é' },  
            { "small-a-circumflex", 'â' },  
            { "small-a-diaeresis", 'ä' },  
            { "small-a-grave", 'à' },  
            { "small-a-ring above", 'å' },  
            { "small-c-cedilla", 'ç' },  
            { "small-e-circumflex", 'ê' },  
            { "small-e-diaeresis", 'ë' },  
            { "small-e-grave", 'è' },  
            { "small-i-diaeresis", 'ï' },  
            { "small-i-circumflex", 'î' },  
            { "small-i-grave", 'ì' },  
            { "big-a-diaeresis", 'Ä' },  
            { "big-a-ring above", 'Å' },  
            { "big-e-acute", 'É' },  
            { "small-ae", 'æ' },  
            { "big-ae", 'Æ' },  
            { "small-o-circumflex", 'ô' },  
            { "small-o-diaeresis", 'ö' },  
            { "small-o-grave", 'ò' },  
            { "small-u-circumflex", 'û' },  
            { "small-u-grave", 'ù' },  
            { "small-y-diaeresis", 'ÿ' },  
            { "big-o-diaeresis", 'Ö' },  
            { "big-u-diaeresis", 'Ü' },  
            { "cents", '¢' },  
            { "pound", '£' },  
            { "yen", '¥' },  
            { "peseta", '₧' },  
            { "small-f-hook", 'ƒ' },  
            { "small-a-acute", 'á' },  
            { "small-i-acute", 'í' },  
            { "small-o-acute", 'ó' },  
            { "small-u-acute", 'ú' },  
            { "small-n-tilde", 'ñ' },  
            { "big-n-tilde", 'Ñ' },  
            { "feminine-ordinal-indicator", 'ª' },  
            { "masculine-ordinal-indicator", 'º' },  
            { "inverted-question", '¿' },  
            { "reversed-not", '⌐' },  
            { "not", '¬' },  
            { "fraction-half", '½' },  
            { "fraction-quarter", '¼' },  
            { "inverted-exclamation", '¡' },  
            { "left-pointing-quotation", '«' }, 
            { "right-pointing-quotation", '»' },
            { "square-light", '░' },
            { "square-medium", '▒' }, 
            { "square-dark", '▓' },

            { "small-alpha", 'α' },  
            { "big-double-s", 'ß' },  
            { "big-gamma", 'Γ' },  
            { "pi", 'π' },  
            { "big-sigma", 'Σ' },  
            { "small-sigma", 'σ' },  
            { "micro", 'µ' },  
            { "tau", 'τ' },  
            { "big-phi", 'Φ' },  
            { "theta", 'Θ' },  
            { "omega", 'Ω' },  
            { "delta", 'δ' },  
            { "infinity", '∞' },  
            { "small-phi", 'φ' },  
            { "epsilon", 'ε' },  
            { "intersect", '∩' },  
            { "identical", '≡' },  
            { "plus-minus", '±' },  
            { "gequal", '≥' },  
            { "lequal", '≤' },  
            { "top-integral", '⌠' },  
            { "bottom-integral", '⌡' },  
            { "divide", '÷' },  
            { "almost-equal", '≈' },  
            { "degree", '°' },  
            { "bullet-operator", '∙' },  
            { "middle-dot", '·' },  
            { "root", '√' },  
            { "sqroot", '√' },  
            { "superscript-n", 'ⁿ' },
            { "superscript-two", '²' },
            { "black-square", '■' }, 
    };

        public static string Replace(string message) {
            return EmotesHandler.Unescape(message, '{', '}', Replacements);
        }
        
        /// <summary> Conversion for code page 437 characters from index 0 to 31 to unicode. </summary>
        public const string ControlCharReplacements = "\0☺☻♥♦♣♠•◘○◙♂♀♪♫☼►◄↕‼¶§▬↨↑↓→←∟↔▲▼";
        
        /// <summary> Conversion for code page 437 characters from index 127 to 255 to unicode. </summary>
        public const string ExtendedCharReplacements = "⌂ÇüéâäàåçêëèïîìÄÅÉæÆôöòûùÿÖÜ¢£¥₧ƒáíóúñÑªº¿⌐¬½¼¡«»" +
            "░▒▓│┤╡╢╖╕╣║╗╝╜╛┐└┴┬├─┼╞╟╚╔╩╦╠═╬╧╨╤╥╙╘╒╓╫╪┘┌" +
            "█▄▌▐▀αßΓπΣσµτΦΘΩδ∞φε∩≡±≥≤⌠⌡÷≈°∙·√ⁿ²■\u00a0";
    }
}
