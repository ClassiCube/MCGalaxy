/*
    Copyright 2015 MCGalaxy

    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Net;
using MCGalaxy.Config;
using MCGalaxy.Network;

namespace MCGalaxy.Commands.Moderation {
    public class CmdLocation : Command2 {
        public override string name { get { return "Location"; } }
        public override string shortcut { get { return "GeoIP"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "can see state/province") }; }
        }
        
        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) {
                if (p.IsSuper) { SuperRequiresArgs(p, "player name or IP"); return; }
                message = p.name;
            }
            
            string name, ip = ModActionCmd.FindIP(p, message, "Location", out name);
            if (ip == null) return;
            
            if (IPUtil.IsPrivate(IPAddress.Parse(ip))) {
                p.Message("&WPlayer has an internal IP, cannot trace"); return;
            }

            string json;
            try {
                WebRequest req  = HttpUtil.CreateRequest("http://ipinfo.io/" + ip + "/geo");
                WebResponse res = req.GetResponse();
                json = HttpUtil.GetResponseText(res);
            } catch (Exception ex) {
                HttpUtil.DisposeErrorResponse(ex);
                throw;
            }
            
            JsonReader reader = new JsonReader(json);
            JsonObject obj    = (JsonObject)reader.Parse();
            if (obj == null) { p.Message("&WError parsing GeoIP info"); return; }
            
            object region = null, country = null;
            obj.TryGetValue("region",   out region);
            obj.TryGetValue("country", out country);
            string fullName = CountryUtils.GetCountryName(country.ToString());
            if (fullName != null) country = fullName;
            
            string suffix = HasExtraPerm(p, data.Rank, 1) ? "&b{1}&S/&b{2}" : "&b{2}";
            string nick   = name == null ? ip : "of " + p.FormatNick(name);
            p.Message("The IP {0} &Straces to: " + suffix, nick, region, country);
        }
        
        public override void Help(Player p) {
            p.Message("&T/Location [name/IP]");
            p.Message("&HTracks down location of the given IP, or IP player is on.");
        }
    }

    public static class CountryUtils {
        public static string GetCountryName(string twoLetterCode) {
            twoLetterCode = twoLetterCode.ToUpperInvariant();
            if (!CodesToNames.ContainsKey(twoLetterCode)) { return null; }
            return CodesToNames[twoLetterCode];
        }
        public static Dictionary<string, string> CodesToNames = new Dictionary<string, string>() {
            { "AF", "Afghanistan" },
            { "AL", "Albania" },
            { "DZ", "Algeria" },
            { "AS", "American Samoa" },
            { "AD", "Andorra" },
            { "AO", "Angola" },
            { "AI", "Anguilla" },
            { "AQ", "Antarctica" },
            { "AG", "Antigua and Barbuda" },
            { "AR", "Argentina" },
            { "AM", "Armenia" },
            { "AW", "Aruba" },
            { "AU", "Australia" },
            { "AT", "Austria" },
            { "AZ", "Azerbaijan" },
            { "BS", "Bahamas" },
            { "BH", "Bahrain" },
            { "BD", "Bangladesh" },
            { "BB", "Barbados" },
            { "BY", "Belarus" },
            { "BE", "Belgium" },
            { "BZ", "Belize" },
            { "BJ", "Benin" },
            { "BM", "Bermuda" },
            { "BT", "Bhutan" },
            { "BO", "Plurinational State of Bolivia" },
            { "BQ", "Bonaire, Sint Eustatius and Saba" },
            { "BA", "Bosnia and Herzegovina" },
            { "BW", "Botswana" },
            { "BV", "Bouvet Island" },
            { "BR", "Brazil" },
            { "IO", "The British Indian Ocean Territory" },
            { "BN", "Brunei Darussalam" },
            { "BG", "Bulgaria" },
            { "BF", "Burkina Faso" },
            { "BI", "Burundi" },
            { "CV", "Cabo Verde" },
            { "KH", "Cambodia" },
            { "CM", "Cameroon" },
            { "CA", "Canada" },
            { "KY", "The Cayman Islands" },
            { "CF", "The Central African Republic" },
            { "TD", "Chad" },
            { "CL", "Chile" },
            { "CN", "China" },
            { "CX", "Christmas Island" },
            { "CC", "The Cocos (Keeling) Islands" },
            { "CO", "Colombia" },
            { "KM", "Comoros" },
            { "CD", "The Democratic Republic of the Congo" },
            { "CG", "The Congo" },
            { "CK", "The Cook Islands" },
            { "CR", "Costa Rica" },
            { "HR", "Croatia" },
            { "CU", "Cuba" },
            { "CW", "Curaçao" },
            { "CY", "Cyprus" },
            { "CZ", "Czechia" },
            { "CI", "Côte d'Ivoire" },
            { "DK", "Denmark" },
            { "DJ", "Djibouti" },
            { "DM", "Dominica" },
            { "DO", "The Dominican Republic" },
            { "EC", "Ecuador" },
            { "EG", "Egypt" },
            { "SV", "El Salvador" },
            { "GQ", "Equatorial Guinea" },
            { "ER", "Eritrea" },
            { "EE", "Estonia" },
            { "SZ", "Eswatini" },
            { "ET", "Ethiopia" },
            { "FK", "The Falkland Islands [Malvinas]" },
            { "FO", "The Faroe Islands" },
            { "FJ", "Fiji" },
            { "FI", "Finland" },
            { "FR", "France" },
            { "GF", "French Guiana" },
            { "PF", "French Polynesia" },
            { "TF", "The French Southern Territories" },
            { "GA", "Gabon" },
            { "GM", "The Gambia" },
            { "GE", "Georgia" },
            { "DE", "Germany" },
            { "GH", "Ghana" },
            { "GI", "Gibraltar" },
            { "GR", "Greece" },
            { "GL", "Greenland" },
            { "GD", "Grenada" },
            { "GP", "Guadeloupe" },
            { "GU", "Guam" },
            { "GT", "Guatemala" },
            { "GG", "Guernsey" },
            { "GN", "Guinea" },
            { "GW", "Guinea-Bissau" },
            { "GY", "Guyana" },
            { "HT", "Haiti" },
            { "HM", "Heard Island and McDonald Islands" },
            { "VA", "The Holy See" },
            { "HN", "Honduras" },
            { "HK", "Hong Kong" },
            { "HU", "Hungary" },
            { "IS", "Iceland" },
            { "IN", "India" },
            { "ID", "Indonesia" },
            { "IR", "Islamic Republic of Iran" },
            { "IQ", "Iraq" },
            { "IE", "Ireland" },
            { "IM", "Isle of Man" },
            { "IL", "Israel" },
            { "IT", "Italy" },
            { "JM", "Jamaica" },
            { "JP", "Japan" },
            { "JE", "Jersey" },
            { "JO", "Jordan" },
            { "KZ", "Kazakhstan" },
            { "KE", "Kenya" },
            { "KI", "Kiribati" },
            { "KP", "North Korea" },
            { "KR", "South Korea" },
            { "KW", "Kuwait" },
            { "KG", "Kyrgyzstan" },
            { "LA", "The Lao People's Democratic Republic" },
            { "LV", "Latvia" },
            { "LB", "Lebanon" },
            { "LS", "Lesotho" },
            { "LR", "Liberia" },
            { "LY", "Libya" },
            { "LI", "Liechtenstein" },
            { "LT", "Lithuania" },
            { "LU", "Luxembourg" },
            { "MO", "Macao" },
            { "MG", "Madagascar" },
            { "MW", "Malawi" },
            { "MY", "Malaysia" },
            { "MV", "Maldives" },
            { "ML", "Mali" },
            { "MT", "Malta" },
            { "MH", "The Marshall Islands" },
            { "MQ", "Martinique" },
            { "MR", "Mauritania" },
            { "MU", "Mauritius" },
            { "YT", "Mayotte" },
            { "MX", "Mexico" },
            { "FM", "Federated States of Micronesia" },
            { "MD", "The Republic of Moldova)" },
            { "MC", "Monaco" },
            { "MN", "Mongolia" },
            { "ME", "Montenegro" },
            { "MS", "Montserrat" },
            { "MA", "Morocco" },
            { "MZ", "Mozambique" },
            { "MM", "Myanmar" },
            { "NA", "Namibia" },
            { "NR", "Nauru" },
            { "NP", "Nepal" },
            { "NL", "The Netherlands" },
            { "NC", "New Caledonia" },
            { "NZ", "New Zealand" },
            { "NI", "Nicaragua" },
            { "NE", "The Niger" },
            { "NG", "Nigeria" },
            { "NU", "Niue" },
            { "NF", "Norfolk Island" },
            { "MP", "The Northern Mariana Islands" },
            { "NO", "Norway" },
            { "OM", "Oman" },
            { "PK", "Pakistan" },
            { "PW", "Palau" },
            { "PS", "State of Palestine" },
            { "PA", "Panama" },
            { "PG", "Papua New Guinea" },
            { "PY", "Paraguay" },
            { "PE", "Peru" },
            { "PH", "The Philippines" },
            { "PN", "Pitcairn" },
            { "PL", "Poland" },
            { "PT", "Portugal" },
            { "PR", "Puerto Rico" },
            { "QA", "Qatar" },
            { "MK", "Republic of North Macedonia" },
            { "RO", "Romania" },
            { "RU", "The Russian Federation" },
            { "RW", "Rwanda" },
            { "RE", "Réunion" },
            { "BL", "Saint Barthélemy" },
            { "SH", "Saint Helena, Ascension and Tristan da Cunha" },
            { "KN", "Saint Kitts and Nevis" },
            { "LC", "Saint Lucia" },
            { "MF", "Saint Martin (French part)" },
            { "PM", "Saint Pierre and Miquelon" },
            { "VC", "Saint Vincent and the Grenadines" },
            { "WS", "Samoa" },
            { "SM", "San Marino" },
            { "ST", "Sao Tome and Principe" },
            { "SA", "Saudi Arabia" },
            { "SN", "Senegal" },
            { "RS", "Serbia" },
            { "SC", "Seychelles" },
            { "SL", "Sierra Leone" },
            { "SG", "Singapore" },
            { "SX", "Sint Maarten (Dutch part)" },
            { "SK", "Slovakia" },
            { "SI", "Slovenia" },
            { "SB", "Solomon Islands" },
            { "SO", "Somalia" },
            { "ZA", "South Africa" },
            { "GS", "South Georgia and the South Sandwich Islands" },
            { "SS", "South Sudan" },
            { "ES", "Spain" },
            { "LK", "Sri Lanka" },
            { "SD", "The Sudan" },
            { "SR", "Suriname" },
            { "SJ", "Svalbard and Jan Mayen" },
            { "SE", "Sweden" },
            { "CH", "Switzerland" },
            { "SY", "Syrian Arab Republic" },
            { "TW", "Taiwan" },
            { "TJ", "Tajikistan" },
            { "TZ", "United Republic of Tanzania" },
            { "TH", "Thailand" },
            { "TL", "Timor-Leste" },
            { "TG", "Togo" },
            { "TK", "Tokelau" },
            { "TO", "Tonga" },
            { "TT", "Trinidad and Tobago" },
            { "TN", "Tunisia" },
            { "TR", "Turkey" },
            { "TM", "Turkmenistan" },
            { "TC", "The Turks and Caicos Islands" },
            { "TV", "Tuvalu" },
            { "UG", "Uganda" },
            { "UA", "Ukraine" },
            { "AE", "The United Arab Emirates" },
            { "GB", "The United Kingdom of Great Britain and Northern Ireland" },
            { "UM", "The United States Minor Outlying Islands" },
            { "US", "The United States of America" },
            { "UY", "Uruguay" },
            { "UZ", "Uzbekistan" },
            { "VU", "Vanuatu" },
            { "VE", "Bolivarian Republic of Venezuela" },
            { "VN", "Viet Nam" },
            { "VG", "Virgin Islands (British)" },
            { "VI", "Virgin Islands (U.S.)" },
            { "WF", "Wallis and Futuna" },
            { "EH", "Western Sahara" },
            { "YE", "Yemen" },
            { "ZM", "Zambia" },
            { "ZW", "Zimbabwe" },
            { "AX", "Åland Islands" },
        };
    }
}