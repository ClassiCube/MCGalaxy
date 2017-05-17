/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy {

    /// <summary> Importance. Higher priority plugins have their handlers called before lower priority plugins. </summary>
    public enum Priority : byte {
        Low = 0,
        Normal = 1,
        High = 2,
        Critical = 3,
        System_Level = 4
    }
  
    /// <summary> This class provides for more advanced modification to MCGalaxy </summary>
    public abstract partial class Plugin {

        /// <summary> Use this to load all your events and everything you need. </summary>
        /// <param name="startup">True if this was used from the server startup and not loaded from the command.</param>
        public abstract void Load(bool startup);
        
        /// <summary> Use this method to dispose of everything you used. </summary>
        /// <param name="shutdown">True if this was used by the server shutting down and not a command.</param>
        public abstract void Unload(bool shutdown);
        
        /// <summary> This method is runned when a player does /help <pluginame>
        /// Use it to show player's what this command is about. </summary>
        /// <param name="p">Player who runned this command.</param>
        public abstract void Help(Player p);
        
        /// <summary> Name of the plugin. </summary>
        public abstract string name { get; }
        
        /// <summary> Your website. </summary>
        public abstract string website { get; }
        
        /// <summary> Oldest version of MCGalaxy the plugin is compatible with. </summary>
        public abstract string MCGalaxy_Version { get; }
        
        /// <summary> Version of your plugin. </summary>
        public abstract int build { get; }
        
        /// <summary> Message to display once plugin is loaded. </summary>
        public abstract string welcome { get; }
        
        /// <summary> The creator/author of this plugin. (Your name) </summary>
        public abstract string creator { get; }
        
        /// <summary> Whether or not to load this plugin at startup. </summary>
        public abstract bool LoadAtStartup { get; }
    }
    
    public abstract class Plugin_Simple : Plugin {
        
        /// <summary> This method is runned when a player does /help <pluginame>
        /// Use it to show player's what this command is about. </summary>
        /// <param name="p">Player who runned this command.</param>
        public override void Help(Player p) {
            Player.Message(p, "No help is available for this plugin.");
        }
        
        /// <summary> Your website. </summary>
        public override string website { get { return "http://www.example.org"; } }
        
        /// <summary> Version of your plugin. </summary>
        public override int build { get { return 0; } }
        
        /// <summary> Message to display once plugin is loaded. </summary>
        public override string welcome { get { return "Plugin " + name + " loaded."; } }
        
        /// <summary> Whether or not to load this plugin at startup. </summary>
        public override bool LoadAtStartup { get { return true; } }
    }
}

