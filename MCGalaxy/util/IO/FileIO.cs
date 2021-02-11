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
        
    	/// <summary> Attempts to delete a file from disc, if it exists </summary>
    	/// <param name="path"> Name of the file to delete </param>
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
    	/// <param name="srcPath"> Name of the file to move </param>
    	/// <param name="dstPath"> New path for the file </param>
    	/// <returns> true if file was successfully moved, false if file did not exist to begin with </returns>
    	/// <remarks> See File.Move for exceptions that can be thrown </remarks>
        public static bool TryMove(string srcPath, string dstPath) {
            try {
                File.Move(srcPath, dstPath);
                return true;
            } catch (FileNotFoundException) {
                return false;
            }
        }
        
        public static string[] TryGetDirectoryFiles(string path, string searchPattern) {
            try {
                return Directory.GetFiles(path, searchPattern);
            } catch (DirectoryNotFoundException) {
                return null;
            }
        }
    }
}
