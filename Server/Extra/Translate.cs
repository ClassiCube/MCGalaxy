/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/
using System;

namespace MCGalaxy
{
    public static class Translate
    {
        /// <summary>
        /// Initializes translator to detect chat
        /// </summary>
        public static void Init()
        {
            Player.PlayerChat += MessageTrans;
        }
        /// <summary>
        /// Translates a message and sends it to all players except the sender
        /// </summary>
        /// <param name="player">The sender ( will not receive translated message )</param>
        /// <param name="message">The original message the sender sent</param>
        public static void MessageTrans(Player player, String message)
        {
            try
            {
                bool enabled = Server.transenabled;
                string langcode = Server.translang;
                string language = "";
                string result = "";
                if (enabled)
                {
                    TranslateService.LanguageServiceClient client = new TranslateService.LanguageServiceClient();
                    client = new TranslateService.LanguageServiceClient();
                    result = client.Translate("B1BFCEF6CD65FC6F03F46E9124D2228546642A3B", message, "", langcode, "text/plain", "general");
                    if (message == result)
                    {
                        return;
                    }
                    else
                    {
                        language = client.Detect("B1BFCEF6CD65FC6F03F46E9124D2228546642A3B", message);
                        switch (language)
                        {
                            case "af":
                                language = "Afrikaans";
                                break;
                            case "ar-sa":
                                language = "Arabic (Saudi Arabia)";
                                break;
                            case "ar-eg":
                                language = "Arabic (Egypt)";
                                break;
                            case "ar-dz":
                                language = "Arabic (Algeria)";
                                break;
                            case "ar-tn":
                                language = "Arabic (Tunisia)";
                                break;
                            case "ar-ye":
                                language = "Arabic (Yemen)";
                                break;
                            case "ar-jo":
                                language = "Arabic (Jordan)";
                                break;
                            case "ar-kw":
                                language = "Arabic (Kuwait)";
                                break;
                            case "ar-bh":
                                language = "Arabic (Bahrain)";
                                break;
                            case "eu":
                                language = "Basque";
                                break;
                            case "be":
                                language = "Belarusian";
                                break;
                            case "zh-tw":
                                language = "Chinese (Taiwan)";
                                break;
                            case "zh-hk":
                                language = "Chinese (Hong Kong SAR)";
                                break;
                            case "hr":
                                language = "Croatian";
                                break;
                            case "da":
                                language = "Danish";
                                break;
                            case "nl-be":
                                language = "Dutch (Belgium)";
                                break;
                            case "en-us":
                                language = "English (United States)";
                                break;
                            case "en-au":
                                language = "English (Australia)";
                                break;
                            case "en-nz":
                                language = "English (New Zealand)";
                                break;
                            case "en-za":
                                language = "English (South Africa)";
                                break;
                            case "en-tt":
                                language = "English (Trinidad)";
                                break;
                            case "fo":
                                language = "Faeroese";
                                break;
                            case "fi":
                                language = "Finnish";
                                break;
                            case "fr-be":
                                language = "French (Belgium)";
                                break;
                            case "fr-ch":
                                language = "French (Switzerland)";
                                break;
                            case "gd":
                                language = "Gaelic (Scotland)";
                                break;
                            case "de":
                                language = "German (Standard)";
                                break;
                            case "de-at":
                                language = "German (Austria)";
                                break;
                            case "de-li":
                                language = "German (Liechtenstein)";
                                break;
                            case "he":
                                language = "Hebrew";
                                break;
                            case "hu":
                                language = "Hungarian";
                                break;
                            case "id":
                                language = "Indonesian";
                                break;
                            case "it-ch":
                                language = "Italian (Switzerland)";
                                break;
                            case "ko":
                                language = "Korean";
                                break;
                            case "lv":
                                language = "Latvian";
                                break;
                            case "mk":
                                language = "Macedonian (FYROM)";
                                break;
                            case "mt":
                                language = "Maltese";
                                break;
                            case "no":
                                language = "Norwegian (Nynorsk)";
                                break;
                            case "pt-br":
                                language = "Portuguese (Brazil)";
                                break;
                            case "rm":
                                language = "Rhaeto-Romanic";
                                break;
                            case "ro-mo":
                                language = "Romanian (Republic of Moldova)";
                                break;
                            case "ru-mo":
                                language = "Russian (Republic of Moldova)";
                                break;
                            case "sr":
                                language = "Serbian (Cyrillic)";
                                break;
                            case "sk":
                                language = "Slovak";
                                break;
                            case "sb":
                                language = "Sorbian";
                                break;
                            case "es-mx":
                                language = "Spanish (Mexico)";
                                break;
                            case "es-cr":
                                language = "Spanish (Costa Rica)";
                                break;
                            case "es-do":
                                language = "Spanish (Dominican Republic)";
                                break;
                            case "es-co":
                                language = "Spanish (Colombia)";
                                break;
                            case "es-ar":
                                language = "Spanish (Argentina)";
                                break;
                            case "es-cl":
                                language = "Spanish (Chile)";
                                break;
                            case "es-py":
                                language = "Spanish (Paraguay)";
                                break;
                            case "es-sv":
                                language = "Spanish (El Salvador)";
                                break;
                            case "es-ni":
                                language = "Spanish (Nicaragua)";
                                break;
                            case "sx":
                                language = "Sutu";
                                break;
                            case "sv-fi":
                                language = "Swedish (Finland)";
                                break;
                            case "ts":
                                language = "Tsonga";
                                break;
                            case "tr":
                                language = "Turkish";
                                break;
                            case "ur":
                                language = "Urdu";
                                break;
                            case "vi":
                                language = "Vietnamese";
                                break;
                            case "ji":
                                language = "Yiddish";
                                break;
                            case "sq":
                                language = "Albanian";
                                break;
                            case "ar-iq":
                                language = "Arabic (Iraq)";
                                break;
                            case "ar-ly":
                                language = "Arabic (Libya)";
                                break;
                            case "ar-ma":
                                language = "Arabic (Morocco)";
                                break;
                            case "ar-om":
                                language = "Arabic (Oman)";
                                break;
                            case "ar-sy":
                                language = "Arabic (Syria)";
                                break;
                            case "ar-lb":
                                language = "Arabic (Lebanon)";
                                break;
                            case "ar-ae":
                                language = "Arabic (U.A.E.)";
                                break;
                            case "ar-qa":
                                language = "Arabic (Qatar)";
                                break;
                            case "bg":
                                language = "Bulgarian";
                                break;
                            case "ca":
                                language = "Catalan";
                                break;
                            case "zh-cn":
                                language = "Chinese (PRC)";
                                break;
                            case "zh-sg":
                                language = "Chinese (Singapore)";
                                break;
                            case "cs":
                                language = "Czech";
                                break;
                            case "nl":
                                language = "Dutch (Standard)";
                                break;
                            case "en":
                                language = "English";
                                break;
                            case "en-gb":
                                language = "English (United Kingdom)";
                                break;
                            case "en-ca":
                                language = "English (Canada)";
                                break;
                            case "en-ie":
                                language = "English (Ireland)";
                                break;
                            case "en-jm":
                                language = "English (Jamaica)";
                                break;
                            case "en-bz":
                                language = "English (Belize)";
                                break;
                            case "et":
                                language = "Farsi";
                                break;
                            case "fr":
                                language = "French (Standard)";
                                break;
                            case "ga":
                                language = "Irish";
                                break;
                            case "el":
                                language = "Greek";
                                break;
                            case "hi":
                                language = "Hindi";
                                break;
                            case "it":
                                language = "Italian (Standard)";
                                break;
                            case "is":
                                language = "Icelandic";
                                break;
                            case "ja":
                                language = "Japanese";
                                break;
                            case "lt":
                                language = "Lithuanian";
                                break;
                            case "ms":
                                language = "Malaysian";
                                break;
                            case "pl":
                                language = "Polish";
                                break;
                            case "pt":
                                language = "Portuguese";
                                break;
                            case "ro":
                                language = "Romanian";
                                break;
                            case "ru":
                                language = "Russian";
                                break;
                            case "sz":
                                language = "Sami (Lappish) ";
                                break;
                            case "sl":
                                language = "Slovenian ";
                                break;
                            case "es":
                                language = "Spanish";
                                break;
                            case "sv":
                                language = "Swedish";
                                break;
                            case "th":
                                language = "Thai";
                                break;
                            case "tn":
                                language = "Tswana";
                                break;
                            case "uk":
                                language = "Ukrainian";
                                break;
                            case "ve":
                                language = "Venda";
                                break;
                            case "xh":
                                language = "Xhosa";
                                break;
                            case "zu":
                                language = "Zulu";
                                break;
                            default:
                                language = "!ERROR!";
                                break;
                        }
                        switch (langcode)
                        {
                            case "af":
                                langcode = "Afrikaans";
                                break;
                            case "ar-sa":
                                langcode = "Arabic (Saudi Arabia)";
                                break;
                            case "ar-eg":
                                langcode = "Arabic (Egypt)";
                                break;
                            case "ar-dz":
                                langcode = "Arabic (Algeria)";
                                break;
                            case "ar-tn":
                                langcode = "Arabic (Tunisia)";
                                break;
                            case "ar-ye":
                                langcode = "Arabic (Yemen)";
                                break;
                            case "ar-jo":
                                langcode = "Arabic (Jordan)";
                                break;
                            case "ar-kw":
                                langcode = "Arabic (Kuwait)";
                                break;
                            case "ar-bh":
                                langcode = "Arabic (Bahrain)";
                                break;
                            case "eu":
                                langcode = "Basque";
                                break;
                            case "be":
                                langcode = "Belarusian";
                                break;
                            case "zh-tw":
                                langcode = "Chinese (Taiwan)";
                                break;
                            case "zh-hk":
                                langcode = "Chinese (Hong Kong SAR)";
                                break;
                            case "hr":
                                langcode = "Croatian";
                                break;
                            case "da":
                                langcode = "Danish";
                                break;
                            case "nl-be":
                                langcode = "Dutch (Belgium)";
                                break;
                            case "en-us":
                                langcode = "English (United States)";
                                break;
                            case "en-au":
                                langcode = "English (Australia)";
                                break;
                            case "en-nz":
                                langcode = "English (New Zealand)";
                                break;
                            case "en-za":
                                langcode = "English (South Africa)";
                                break;
                            case "en-tt":
                                langcode = "English (Trinidad)";
                                break;
                            case "fo":
                                langcode = "Faeroese";
                                break;
                            case "fi":
                                langcode = "Finnish";
                                break;
                            case "fr-be":
                                langcode = "French (Belgium)";
                                break;
                            case "fr-ch":
                                langcode = "French (Switzerland)";
                                break;
                            case "gd":
                                langcode = "Gaelic (Scotland)";
                                break;
                            case "de":
                                langcode = "German (Standard)";
                                break;
                            case "de-at":
                                langcode = "German (Austria)";
                                break;
                            case "de-li":
                                langcode = "German (Liechtenstein)";
                                break;
                            case "he":
                                langcode = "Hebrew";
                                break;
                            case "hu":
                                langcode = "Hungarian";
                                break;
                            case "id":
                                langcode = "Indonesian";
                                break;
                            case "it-ch":
                                langcode = "Italian (Switzerland)";
                                break;
                            case "ko":
                                langcode = "Korean";
                                break;
                            case "lv":
                                langcode = "Latvian";
                                break;
                            case "mk":
                                langcode = "Macedonian (FYROM)";
                                break;
                            case "mt":
                                langcode = "Maltese";
                                break;
                            case "no":
                                langcode = "Norwegian (Nynorsk)";
                                break;
                            case "pt-br":
                                langcode = "Portuguese (Brazil)";
                                break;
                            case "rm":
                                langcode = "Rhaeto-Romanic";
                                break;
                            case "ro-mo":
                                langcode = "Romanian (Republic of Moldova)";
                                break;
                            case "ru-mo":
                                langcode = "Russian (Republic of Moldova)";
                                break;
                            case "sr":
                                langcode = "Serbian (Cyrillic)";
                                break;
                            case "sk":
                                langcode = "Slovak";
                                break;
                            case "sb":
                                langcode = "Sorbian";
                                break;
                            case "es-mx":
                                langcode = "Spanish (Mexico)";
                                break;
                            case "es-cr":
                                langcode = "Spanish (Costa Rica)";
                                break;
                            case "es-do":
                                langcode = "Spanish (Dominican Republic)";
                                break;
                            case "es-co":
                                langcode = "Spanish (Colombia)";
                                break;
                            case "es-ar":
                                langcode = "Spanish (Argentina)";
                                break;
                            case "es-cl":
                                langcode = "Spanish (Chile)";
                                break;
                            case "es-py":
                                langcode = "Spanish (Paraguay)";
                                break;
                            case "es-sv":
                                langcode = "Spanish (El Salvador)";
                                break;
                            case "es-ni":
                                langcode = "Spanish (Nicaragua)";
                                break;
                            case "sx":
                                langcode = "Sutu";
                                break;
                            case "sv-fi":
                                langcode = "Swedish (Finland)";
                                break;
                            case "ts":
                                langcode = "Tsonga";
                                break;
                            case "tr":
                                langcode = "Turkish";
                                break;
                            case "ur":
                                langcode = "Urdu";
                                break;
                            case "vi":
                                langcode = "Vietnamese";
                                break;
                            case "ji":
                                langcode = "Yiddish";
                                break;
                            case "sq":
                                langcode = "Albanian";
                                break;
                            case "ar-iq":
                                langcode = "Arabic (Iraq)";
                                break;
                            case "ar-ly":
                                langcode = "Arabic (Libya)";
                                break;
                            case "ar-ma":
                                langcode = "Arabic (Morocco)";
                                break;
                            case "ar-om":
                                langcode = "Arabic (Oman)";
                                break;
                            case "ar-sy":
                                langcode = "Arabic (Syria)";
                                break;
                            case "ar-lb":
                                langcode = "Arabic (Lebanon)";
                                break;
                            case "ar-ae":
                                langcode = "Arabic (U.A.E.)";
                                break;
                            case "ar-qa":
                                langcode = "Arabic (Qatar)";
                                break;
                            case "bg":
                                langcode = "Bulgarian";
                                break;
                            case "ca":
                                langcode = "Catalan";
                                break;
                            case "zh-cn":
                                langcode = "Chinese (PRC)";
                                break;
                            case "zh-sg":
                                langcode = "Chinese (Singapore)";
                                break;
                            case "cs":
                                langcode = "Czech";
                                break;
                            case "nl":
                                langcode = "Dutch (Standard)";
                                break;
                            case "en":
                                langcode = "English";
                                break;
                            case "en-gb":
                                langcode = "English (United Kingdom)";
                                break;
                            case "en-ca":
                                langcode = "English (Canada)";
                                break;
                            case "en-ie":
                                langcode = "English (Ireland)";
                                break;
                            case "en-jm":
                                langcode = "English (Jamaica)";
                                break;
                            case "en-bz":
                                langcode = "English (Belize)";
                                break;
                            case "et":
                                langcode = "Farsi";
                                break;
                            case "fr":
                                langcode = "French (Standard)";
                                break;
                            case "ga":
                                langcode = "Irish";
                                break;
                            case "el":
                                langcode = "Greek";
                                break;
                            case "hi":
                                langcode = "Hindi";
                                break;
                            case "it":
                                langcode = "Italian (Standard)";
                                break;
                            case "is":
                                langcode = "Icelandic";
                                break;
                            case "ja":
                                langcode = "Japanese";
                                break;
                            case "lt":
                                langcode = "Lithuanian";
                                break;
                            case "ms":
                                langcode = "Malaysian";
                                break;
                            case "pl":
                                langcode = "Polish";
                                break;
                            case "pt":
                                langcode = "Portuguese";
                                break;
                            case "ro":
                                langcode = "Romanian";
                                break;
                            case "ru":
                                langcode = "Russian";
                                break;
                            case "sz":
                                langcode = "Sami (Lappish) ";
                                break;
                            case "sl":
                                langcode = "Slovenian ";
                                break;
                            case "es":
                                langcode = "Spanish";
                                break;
                            case "sv":
                                langcode = "Swedish";
                                break;
                            case "th":
                                langcode = "Thai";
                                break;
                            case "tn":
                                langcode = "Tswana";
                                break;
                            case "uk":
                                langcode = "Ukrainian";
                                break;
                            case "ve":
                                langcode = "Venda";
                                break;
                            case "xh":
                                langcode = "Xhosa";
                                break;
                            case "zu":
                                langcode = "Zulu";
                                break;
                            default:
                                langcode = "!ERROR!";
                                break;
                        }

                        foreach (Player p in Player.players)
                        {
                            if (!Server.transignore.Contains(p.name))
                            {
                                Player.SendMessage(p, player.color + player.name + "&c translated " + language + " to " + langcode + ": " + Server.DefaultColor + result);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }

    }
}
