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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Blocks;
using MCGalaxy.Commands;
using MCGalaxy.Events;

namespace MCGalaxy {
    /// <summary> This is the group object, where ranks and their data are stored </summary>
    public sealed partial class Group {
        
        const int mapGenLimitAdmin = 225 * 1000 * 1000;
        const int mapGenLimit = 30 * 1000 * 1000;
        public static bool cancelrank = false;
        
        /// <summary> Name of this rank. </summary>
        public string Name;
        
        /// <summary> Default color code applied to members of this rank. </summary>
        public string Color;
        
        /// <summary> Name of this rank, prefixed by its color code. </summary>
        public string ColoredName { get { return Color + Name; } }

        /// <summary> Permission level of this rank. </summary>
        public LevelPermission Permission = LevelPermission.Null;
        
        /// <summary> Maximum number of blocks members of this group can use in draw commands. </summary>
        public int DrawLimit;
        
        /// <summary> Maximum number of seconds members of this group can /undo up to. </summary>
        public int MaxUndo;
        
        /// <summary> Maximum volume of a map that members of this group can generate. </summary>
        public int GenVolume = mapGenLimit;
        
        /// <summary> Whether members of this rank are auto kicked for being AFK. </summary>
        public bool AfkKicked = true;
        
        /// <summary> Number of minutes members of this rank can be AFK for, before they are auto kicked. </summary>
        public int AfkKickMinutes = 45;
        
        /// <summary> Optional MOTD shown to members of this group, instead of server's default MOTD. </summary>
        /// <remarks> If a level has a custom MOTD, it overrides this. </remarks>
        public string MOTD = "";

        /// <summary> Maxmimum number of personal/realm worlds allowed for members of this rank. </summary>
        public byte OverseerMaps = 3;
        
        /// <summary> Optional prefix shown in chat before titles and player name. </summary>
        public string Prefix = "";
        
        /// <summary> List of players who are members of this group.  </summary>
        public PlayerList Players;
        
        /// <summary> List of commands members of this group can use. </summary>
        public List<Command> Commands;
        
        /// <summary> List of blocks members of this group can use. </summary>
        public bool[] Blocks = new bool[256];
        
        internal string filename;
        public Group() { }
        
        private Group(LevelPermission perm, int maxB, int maxUn, string name, char colCode) {
            Permission = perm;
            DrawLimit = maxB;
            MaxUndo = maxUn;
            Name = name;
            Color = "&" + colCode;
            GenVolume = perm < LevelPermission.Admin ? mapGenLimit : mapGenLimitAdmin;
            AfkKicked = perm <= LevelPermission.AdvBuilder;
        }
        
        
        /// <summary> Sets all the commands that members of this group can use. </summary>
        public void SetUsableCommands() {
            Commands = CommandPerms.AllCommandsUsableBy(Permission);
        }
        
        /// <summary> Sets all the blocks that this group can use. </summary>
        public void SetUsableBlocks() {
            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = BlockPerms.UsableBy(Permission, (byte)i);
        }

        /// <summary> Returns true if members of this group can use the given command. </summary>
        public bool CanExecute(string cmdName) {
            Command cmd = Command.all.Find(cmdName);
            return cmd != null && Commands.Contains(cmd);
        }
        
        /// <summary> Returns true if members of this group can use the given command. </summary>
        public bool CanExecute(Command cmd) { return Commands.Contains(cmd); }
        
        
        /// <summary> Creates a copy of this group, except for members list and usable commands and blocks. </summary>
        public Group CopyConfig() {
            Group copy = new Group();
            copy.Name = Name; copy.Color = Color; copy.Permission = Permission;
            copy.DrawLimit = DrawLimit; copy.MaxUndo = MaxUndo; copy.MOTD = MOTD;
            copy.GenVolume = GenVolume; copy.OverseerMaps = OverseerMaps;
            copy.AfkKicked = AfkKicked; copy.AfkKickMinutes = AfkKickMinutes;
            copy.Prefix = Prefix; copy.filename = filename;
            return copy;            
        }
        
        public static LevelPermission ParsePermOrName(string value, LevelPermission defPerm) {
            if (value == null) return defPerm;
            
            sbyte perm;
            if (sbyte.TryParse(value, out perm))
                return (LevelPermission)perm;
            
            Group grp = Find(value);
            return grp != null ? grp.Permission : defPerm;
        }
        
        
        void LoadPlayers() {
            string desired = (int)Permission + "_rank";
            // Try to use the auto filename format
            if (filename == null || !filename.StartsWith(desired))
                MoveToDesired(desired);
            
            Players = PlayerList.Load(filename);
        }
        
        void MoveToDesired(string desired) {
            // rank doesn't exist to begin with
            if (filename == null || !File.Exists("ranks/" + filename)) {
                filename = desired + ".txt";
                // TODO: should start backwards from z to a
            } else if (MoveToFile(desired + ".txt")) {
            } else {
                // try appending a and z if duplicate file
                for (char c = 'a'; c <= 'z'; c++) {
                    string newFile = desired + c + ".txt";
                    if (MoveToFile(newFile)) return;
                }
            }
        }
        
        bool MoveToFile(string newFile) {
            if (File.Exists("ranks/" + newFile)) return false;
            
            try {
                File.Move("ranks/" + filename, "ranks/" + newFile);
                filename = newFile;
                return true;
            } catch (Exception ex) {
                Logger.LogError(ex);
                return false;
            }
        }

    }
}
