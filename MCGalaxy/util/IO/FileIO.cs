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
using System.IO;

namespace MCGalaxy {
    
    /// <summary> Provides utility methods for File I/O operations. </summary>
    public static class FileIO {
        
        public static bool DeleteIfExists(string path) {
            try {
                File.Delete(path);
                return true;
            } catch (FileNotFoundException) {
                return false;
            }
        }
        
        public static bool MoveIfExists(string srcPath, string dstPath) {
            try {
                File.Move(srcPath, dstPath);
                return true;
            } catch (FileNotFoundException) {
                return false;
            }
        }
    }
}
