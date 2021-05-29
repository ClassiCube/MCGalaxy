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
    
    /// <summary> Provides methods for atomic File I/O operations. </summary>
    public static class AtomicIO {
        
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
    }
    
    /// <summary> Provides File I/O methods that log errors instead of throwing exceptions on failure </summary>
    public static class UnsafeIO {
        
        public static bool CreateDirectory(string directory) {
            try {
                Directory.CreateDirectory(directory);
                return true;
            } catch (Exception ex) {
                Logger.LogError("Creating directory " + directory, ex);
                return false;
            }
        }
    }
}
