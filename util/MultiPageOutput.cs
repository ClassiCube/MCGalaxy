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
    
    /// <summary> Outputs a large range of values across a number of 'pages'. </summary>
    public static class MultiPageOutput {

        public static void Output<T>(Player p, IList<T> items, Func<T, int, string> formatter,
                                     string cmd, string type, string input, bool lines) {
            int page = 0, total = items.Count;
            int perPage = lines ? 10 : 30;
            
            if (input == "") {
                OutputPage(p, items, formatter, cmd, type, 1, lines);
                if (total <= perPage) return;
                Player.Message(p, "To see all {0}, use %T/{1} all", type, cmd);
            } else if (input.CaselessEq("all")) {
            	OutputItems(p, items, 0, items.Count, lines, formatter);
                Player.Message(p, "Showing {0} 1-{1} (out of {1})", type, items.Count);
            } else if (!int.TryParse(input, out page)) {
                Player.Message(p, "Page must be either \"all\" or an integer.");
            } else {
                OutputPage(p, items, formatter, cmd, type, page, lines);
            }
        }
        
        static void OutputPage<T>(Player p, IList<T> items, Func<T, int, string> formatter,
                                  string cmd, string type, int page, bool lines) {
            int perPage = lines ? 10 : 30;
            int total = items.Count, maxPage = total / perPage;
            page = Utils.Clamp(page - 1, 0, maxPage); // want page numbers to start at 1
            
            int entriesEnd = Math.Min((page + 1) * perPage, total);
            OutputItems(p, items, page * perPage, entriesEnd, lines, formatter);
            
            if (page < maxPage) {
                Player.Message(p, "Showing {0} {1}-{2} (out of {3}) Next: %T/{4} {5}",
                               type, page * perPage + 1, entriesEnd, total, cmd, page + 2);
            } else {
                Player.Message(p, "Showing {0} {1}-{2} (out of {3})",
                               type, page * perPage + 1, entriesEnd, total);
            }
        }
        
        static void OutputItems<T>(Player p, IList<T> items,
                                   int start, int end, bool lines,
                                   Func<T, int, string> formatter) {
            if (lines) {
                for (int i = start; i < end; i++)
                    Player.Message(p, formatter(items[i], i));
            } else {
                IEnumerable<string> output = Subset(items, start, end, formatter);
                Player.Message(p, output.Join());
            }
        }
        
        static IEnumerable<string> Subset<T>(IList<T> items, int start, int end,
                                             Func<T, int, string> formatter) {
            for (int i = start; i < end; i++)
                yield return formatter(items[i], i);
            yield break;
        }
    }
}
