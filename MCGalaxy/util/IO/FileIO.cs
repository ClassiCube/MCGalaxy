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
using System.IO;

namespace MCGalaxy 
{   
    /// <summary> Provides utility methods for File I/O operations. </summary>
    public static class FileIO 
    {        
        /// <summary> Attempts to delete a file from disc, if it exists </summary>
        /// <returns> true if file was successfully deleted, false if file did not exist to begin with </returns>
        /// <remarks> See File.Delete for exceptions that can be thrown </remarks>
        public static bool TryDelete(string path) {
            try {
                File.Delete(path);
                return true;
            } catch (FileNotFoundException) {
                return false;
            }
        }
        
        /// <summary> Attempts to move a file on disc, if it exists </summary>
        /// <returns> true if file was successfully moved, false if file did not exist to begin with </returns>
        /// <remarks> See File.Move for exceptions that can be thrown </remarks>
        public static bool TryMove(string curPath, string newPath) {
            try {
                File.Move(curPath, newPath);
                return true;
            } catch (FileNotFoundException) {
                return false;
            }
        }
        
        /// <summary> Attempts to retrieve list of files from a directory, if it exists </summary>
        /// <returns> An array of matching files, null if the directory did not exist to begin with </returns>
        public static string[] TryGetFiles(string directory, string searchPattern) {
            try {
                return Directory.GetFiles(directory, searchPattern);
            } catch (DirectoryNotFoundException) {
                return null;
            }
        }
        
        /// <summary> Returns a StreamWriter that writes data to a temp file path first,
        /// and only overwrites the real file when .Dispose() is called </summary>
        /// <remarks> Reduces the chance of data corruption in full disks </remarks>
        public static StreamWriter CreateGuarded(string path) {
            return new GuardedWriter(path);
        }
        
        
        class GuardedWriter : StreamWriter
        {
            readonly string realPath;
            public GuardedWriter(string path) : base(path + ".tmp") {
                realPath = path;
            }
            
            protected override void Dispose(bool disposing) {
                base.Dispose(disposing);
                string src = realPath + ".tmp";
                string dst = realPath;
                string old = realPath + ".old";
                
                FileIO.TryDelete(old);
                bool didExist = FileIO.TryMove(dst, old);
                File.Move(src, dst);
                
                // Only delete old 'good' file if everything worked
                if (didExist) FileIO.TryDelete(old);
            }
        }
    }
}
