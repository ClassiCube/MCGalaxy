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

namespace MCGalaxy.Util 
{
    public static class ImageUtils 
    {       
        public static IBitmap2D DecodeImage(byte[] data, Player p) {
            IBitmap2D bmp = null;
            try {
                bmp = IBitmap2D.Create();
                bmp.Decode(data);
                return bmp;
            } catch (ArgumentException ex) {
                // GDI+ throws ArgumentException when data is not an image
                // This is a fairly expected error - e.g. when a user tries to /imgprint
                //   the webpage an image is hosted on, instead of the actual image itself. 
                // So don't bother logging a full error for this case
                Logger.Log(LogType.Warning, "Error decoding image: " + ex.Message);
                OnDecodeError(p, bmp);
                return null;
            } catch (Exception ex) {
                Logger.LogError("Error decoding image", ex);
                OnDecodeError(p, bmp);
                return null;
            }
        }

        static void OnDecodeError(Player p, IBitmap2D bmp) {
            if (bmp != null) bmp.Dispose();
            // TODO failed to decode the image. make sure you are using the URL of the image directly, not just the webpage it is hosted on              
            p.Message("&WThere was an error reading the downloaded image.");
            p.Message("&WThe url may need to end with its extension (such as .jpg).");
        }
    }
}