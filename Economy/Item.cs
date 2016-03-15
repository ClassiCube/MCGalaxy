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
    
    /// <summary> An abstract object that can be bought in the economy. (e.g. a rank, title, levels, etc) </summary>
    public abstract class Item {
        
        /// <summary> Simple name for this item. </summary>
        public abstract string Name { get; }
        
        /// <summary> Other common names for this item. </summary>
        public abstract string[] Aliases { get; }
        
        /// <summary> Whether this item can currently be bought in the economy. </summary>
        public bool Enabled { get; set; }
        
        /// <summary> Reads the properties of this item from the economy.properties file. </summary>
        public abstract void Parse(StreamReader reader);
        
        /// <summary> Writes the properties of this item to the economy.properties file. </summary>
        public virtual void Serialise(StreamWriter writer) {
            writer.WriteLine(Name + ":enabled:" + Enabled);
        }
    }
}
