/*
    Copyright 2015 MCGalaxy
        
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


namespace MCGalaxy {

    /// <summary>
    /// Describes options for cinematic gui. It may be sent to a player using Player.Session.SendCinematicGui
    /// </summary>
    public class CinematicGui {

        public bool hideCrosshair;
        public bool hideHand;
        public bool hideHotbar;
        /// <summary>
        /// The color of the cinematic bars, if visible
        /// </summary>
        public ColorDesc barColor = new ColorDesc(0, 0, 0);
        /// <summary>
        /// From 0 to 1 where 0 is not visible and 1 is screen fully covered
        /// </summary>
        public float barSize;
    }
}
