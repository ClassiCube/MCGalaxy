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
        
        const int perPage = 30;
        
        public static void Output<T>(Player p, IEnumerable<T> items, Func<T, string> formatter,
                                     string cmd, string type, string input) {
            List<T> all = new List<T>(items);
            int page = 0, total = all.Count;
            
            if (input == "") {
                OutputPage(p, all, formatter, cmd, type, 0);
                if (total <= perPage) return;
                Player.Message(p, "To see all {0}, use %T/{1} all", type, cmd);
            } else if (input.CaselessEq("all")) {
                Player.Message(p, all.Join(formatter, ", "));
                Player.Message(p, "Showing {0} 1-{1} (out of {1})", type, all.Count);
            } else if (!int.TryParse(input, out page)) {
                Player.Message(p, "Page must be either \"all\" or an integer.");
            } else {
                OutputPage(p, all, formatter, cmd, type, page);
            }
        }
        
        static void OutputPage<T>(Player p, List<T> all, Func<T, string> formatter,
                                  string cmd, string type, int page) {
            int total = all.Count, maxPage = total / perPage;
            page = Utils.Clamp(page, 0, maxPage);           
            all.RemoveRange(0, page * perPage);
            if (all.Count > perPage)
                all.RemoveRange(perPage, all.Count - perPage);
            
            Player.Message(p, all.Join(formatter, ", "));            
            int entriesEnd = Math.Min((page + 1) * perPage, total);
            if (page < maxPage) {
                Player.Message(p, "Showing {0} {1}-{2} (out of {3}) Next: %T/{4} {5}",
                               type, page * perPage + 1, entriesEnd, total, cmd, page + 1);
            } else {
                Player.Message(p, "Showing {0} {1}-{2} (out of {3})",
                               type, page * perPage + 1, entriesEnd, total);
            }

        }
    }
}
