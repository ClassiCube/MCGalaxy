/*
    Copyright 2012 MCForge
 
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
using System.Net;

namespace MCGalaxy {
    public interface IBeat {
        
        /// <summary> Gets the URL. </summary>
        string URL { get; }
        
        /// <summary> Gets whether this IBeat has periodically repeating beats. </summary>
        bool Persistance { get; }
        
        /// <summary> Initialises persistent data for this beat instance. </summary>
        void Init();
        
        /// <summary> Prepares the data for the next beat of this this instance. </summary>
        string PrepareBeat();
        
        /// <summary> Called when a response is recieved. </summary>
        void OnRequest(HttpWebRequest request);
        
        /// <summary> Called when a response is recieved. </summary>
        void OnResponse(string resonse);
    }
}
