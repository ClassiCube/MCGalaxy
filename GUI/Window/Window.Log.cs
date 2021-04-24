/*    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;
using System.Windows.Forms;

namespace MCGalaxy.Gui {
    public partial class Window : Form {
        
        void logs_dateGeneral_Changed(object sender, EventArgs e) {
            string date = logs_dateGeneral.Value.ToString("yyyy-MM-dd");
            string path = Path.Combine("logs", date + ".txt");

            try {
                logs_txtGeneral.Text = File.ReadAllText(path);
            } catch (FileNotFoundException) {
                logs_txtGeneral.Text = "No logs found for: " + date;
            } catch (Exception ex) {
                logs_txtGeneral.Text = null;
                
                Logger.LogError("Opening " + path, ex);
                Popup.Error("Failed to open logfile " + path);
            }
        }
    }
}
