/*    
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
    
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
                logs_txtGeneral.Text = ReadAllText(path);
            } catch (FileNotFoundException) {
                logs_txtGeneral.Text = "No logs found for: " + date;
            } catch (Exception ex) {
                logs_txtGeneral.Text = null;
                
                Logger.LogError("Opening " + path, ex);
                Popup.Error("Failed to open logfile " + path);
            }
        }

        static string ReadAllText(string path) {
            // can't just use File.ReadAllText, because it'll fail with sharing violation
            //  (due to FileLogger using FileShare.ReadWrite, while File.ReadAllText uses FileShare.Read)
            // so try with just FileShare.Read first, then fall back onto FileShare.ReadWrite
            using (Stream stream = OpenFile(path))
            {
                return new StreamReader(stream).ReadToEnd();
            }
        }

        static Stream OpenFile(string path) {
            try {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,      4096, FileOptions.SequentialScan);
            } catch (IOException) {
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.SequentialScan);
            }
        }
    }
}
