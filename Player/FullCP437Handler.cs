using System;
using System.Collections.Generic;
using System.Text;

namespace MCGalaxy
{
    public static class FullCP437Handler
    {
        public static readonly Dictionary<string, char> Replacements = new Dictionary<string, char> {
            { "big-c-cedilla", '\u0080' }, // Ç
			{ "small-u-diaeresis", '\u0081' }, // ü
			{ "small-e-acute", '\u0082' }, // é
			{ "small-a-circumflex", '\u0083' }, // â
			{ "small-a-diaeresis", '\u0084' }, // ä
			{ "small-a-grave", '\u0085' }, // à
			{ "small-a-ring above", '\u0086' }, // å
			{ "small-c-cedilla", '\u0087' }, // ç
			{ "small-e-circumflex", '\u0088' }, // ê
			{ "small-e-diaeresis", '\u0089' }, // ë
			{ "small-e-grave", '\u008A' }, // è
			{ "small-i-diaeresis", '\u008B' }, // ï
			{ "small-i-circumflex", '\u008C' }, // î
			{ "small-i-grave", '\u008D' }, // ì
			{ "big-a-diaeresis", '\u008E' }, // Ä
			{ "big-a-ring above", '\u008F' }, // Å
			{ "big-e-acute", '\u0090' }, // É
			{ "small-ae", '\u0091' }, // æ
			{ "big-ae", '\u0092' }, // Æ
			{ "small-o-circumflex", '\u0093' }, // ô
			{ "small-o-diaeresis", '\u0094' }, // ö
			{ "small-o-grave", '\u0095' }, // ò
			{ "small-u-circumflex", '\u0096' }, // û
			{ "small-u-grave", '\u0097' }, // ù
			{ "small-y-diaeresis", '\u0098' }, // ÿ
			{ "big-o-diaeresis", '\u0099' }, // Ö
			{ "big-u-diaeresis", '\u009A' }, // Ü
			{ "cents", '\u009B' }, // ¢
			{ "pound", '\u009C' }, // £
			{ "yen", '\u009D' }, // ¥
			{ "peseta", '\u009E' }, // ₧
			{ "small-f-hook", '\u009F' }, // ƒ
			{ "small-a-acute", '\u00A0' }, // á
			{ "small-i-acute", '\u00A1' }, // í
			{ "small-o-acute", '\u00A2' }, // ó
			{ "small-u-acute", '\u00A3' }, // ú
			{ "small-n-tilde", '\u00A4' }, // ñ
			{ "big-n-tilde", '\u00A5' }, // Ñ
			{ "feminine-ordinal-indicator", '\u00A6' }, // ª
			{ "masculine-ordinal-indicator", '\u00A7' }, // º
			{ "inverted-question", '\u00A8' }, // ¿
			{ "reversed-not", '\u00A9' }, // ⌐
			{ "not", '\u00AA' }, // ¬
			{ "fraction-half", '\u00AB' }, // ½
			{ "fraction-quarter", '\u00AC' }, // ¼
			{ "inverted-exclamation", '\u00AD' }, // ¡
			{ "left-pointing-quotation", '\u00AE' }, // «
			{ "right-pointing-quotation", '\u00AF' }, // »
			{ "square-light", '\u00B0' }, // ░
			{ "square-medium", '\u00B1' }, // ▒
			{ "square-dark", '\u00B2' }, // ▓

			{ "small-alpha", '\u00E0' }, // α
			{ "big-double-s", '\u00E1' }, // ß
			{ "big-gamma", '\u00E2' }, // Γ
			{ "pi", '\u00E3' }, // π
			{ "big-sigma", '\u00E4' }, // Σ
			{ "small-sigma", '\u00E5' }, // σ
			{ "micro", '\u00E6' }, // µ
			{ "tau", '\u00E7' }, // τ
			{ "big-phi", '\u00E8' }, // Φ
			{ "theta", '\u00E9' }, // Θ
			{ "omega", '\u00EA' }, // Ω
			{ "delta", '\u00EB' }, // δ
			{ "infinity", '\u00EC' }, // ∞
			{ "small-phi", '\u00ED' }, // φ
			{ "epsilon", '\u00EE' }, // ε
			{ "intersect", '\u00EF' }, // ∩
			{ "identical", '\u00F0' }, // ≡
			{ "plus-minus", '\u00F1' }, // ±
			{ "gequal", '\u00F2' }, // ≥
			{ "lequal", '\u00F3' }, // ≤
			{ "top-integral", '\u00F4' }, // ⌠
			{ "bottom-integral", '\u00F5' }, // ⌡
			{ "divide", '\u00F6' }, // ÷
			{ "almost-equal", '\u00F7' }, // ≈
			{ "degree", '\u00F8' }, // °
			{ "bullet-operator", '\u00F9' }, // ∙
			{ "middle-dot", '\u00FA' }, // ·
			{ "root", '\u00FB' }, // √
			{ "sqroot", '\u00FB' }, // √
			{ "superscript-n", '\u00FC' }, // ⁿ
			{ "superscript-two", '\u00FD' }, // ²
			{ "black-square", '\u00FE' }, // ■
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
