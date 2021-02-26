/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Collections.Generic;

namespace MCGalaxy {
    
    /// <summary> Outputs a large range of values across a number of 'pages'. (Pagination) </summary>
    public static class MultiPageOutput {

        /// <summary> Outputs a large range of values across a number of 'pages'. (Pagination) </summary>
        /// <param name="lines">true if each item is printed on a separate line, false if combined. </param>
        public static void Output<T>(Player p, IList<T> items, StringFormatter<T> formatter,
                                     string cmd, string type, string modifier, bool lines) {
            int page = 0, total = items.Count;
            int perPage = lines ? 8 : 30;
            
            if (modifier.Length == 0) {
                OutputPage(p, items, formatter, cmd, type, 1, lines);
                if (total <= perPage) return;
                p.Message("To see all {0}, use &T/{1} all", type, cmd);
            } else if (modifier.CaselessEq("all")) {
                OutputItems(p, items, 0, items.Count, lines, formatter);
                p.Message("Showing {0} 1-{1} (out of {1})", type, items.Count);
            } else if (!int.TryParse(modifier, out page)) {
                p.Message("Input must be either \"all\" or an integer.");
            } else {
                OutputPage(p, items, formatter, cmd, type, page, lines);
            }
        }
        
        static void OutputPage<T>(Player p, IList<T> items, StringFormatter<T> formatter,
                                  string cmd, string type, int start, bool lines) {
            int perPage = lines ? 8 : 30;
            start = Utils.Clamp(start - 1, 0, items.Count - 1); // want item numbers to start at 1         
            int end = Math.Min(start + perPage, items.Count);
            OutputItems(p, items, start, end, lines, formatter);
            
            if (items.Count == 0) {
                p.Message("Showing {0} 0-0 (out of 0)", type);
            } else if (end < items.Count) {
                p.Message("Showing {0} {1}-{2} (out of {3}) Next: &T/{4} {5}",
                               type, start + 1, end, items.Count, cmd, start + 1 + perPage);
            } else {
                p.Message("Showing {0} {1}-{2} (out of {3})",
                               type, start + 1, end, items.Count);
            }
        }
        
        static void OutputItems<T>(Player p, IList<T> items,
                                   int start, int end, bool lines,
                                   StringFormatter<T> formatter) {
            if (lines) {
                for (int i = start; i < end; i++)
                    p.Message(formatter(items[i]));
            } else {
                IEnumerable<string> output = Subset(items, start, end, formatter);
                p.Message(output.Join());
            }
        }
        
        static IEnumerable<string> Subset<T>(IList<T> items, int start, int end,
                                             StringFormatter<T> formatter) {
            for (int i = start; i < end; i++)
                yield return formatter(items[i]);
            yield break;
        }
    }
}
