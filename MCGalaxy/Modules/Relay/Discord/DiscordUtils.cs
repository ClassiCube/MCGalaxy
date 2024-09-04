/*
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

namespace MCGalaxy.Modules.Relay.Discord 
{
    public static class DiscordUtils
    {        
        static readonly string[] markdown_special = {  @"\",  @"*",  @"_",  @"~",  @"`",  @"|",  @"-",  @"#" };
        static readonly string[] markdown_escaped = { @"\\", @"\*", @"\_", @"\~", @"\`", @"\|", @"\-", @"\#" };
        public static string EscapeMarkdown(string message) {
            // don't let user use bold/italic etc markdown
            for (int i = 0; i < markdown_special.Length; i++) 
            {
                message = message.Replace(markdown_special[i], markdown_escaped[i]);
            }
            return message;
        }
        
        
        // these characters are chosen specifically to lie within the unspecified unicode range,
        //  as those characters are "application defined" (EDCX = Escaped Discord Character #X)
        //  https://en.wikipedia.org/wiki/Private_Use_Areas
        public const char UNDERSCORE = '\uEDC1'; // _
        public const char TILDE      = '\uEDC2'; // ~
        public const char STAR       = '\uEDC3'; // *
        public const char GRAVE      = '\uEDC4'; // `
        public const char BAR        = '\uEDC5'; // |
        
        public const string UNDERLINE     = "\uEDC1\uEDC1"; // __
        public const string BOLD          = "\uEDC3\uEDC3"; // **
        public const string ITALIC        = "\uEDC1"; // _
        public const string CODE          = "\uEDC4"; // `
        public const string SPOILER       = "\uEDC5\uEDC5"; // ||
        public const string STRIKETHROUGH = "\uEDC2\uEDC2"; // ~~
        
        public static string MarkdownToSpecial(string input) {
            return input
                .Replace('_', UNDERSCORE)
                .Replace('~', TILDE)
                .Replace('*', STAR)
                .Replace('`', GRAVE)
                .Replace('|', BAR);
        }
        
        public static string SpecialToMarkdown(string input) {
            return input
                .Replace(UNDERSCORE, '_')
                .Replace(TILDE,      '~')
                .Replace(STAR,       '*')
                .Replace(GRAVE,      '`')
                .Replace(BAR,        '|');
        }
    }
}
