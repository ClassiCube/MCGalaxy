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
    public sealed class Group {
        public delegate void RankSet(Player p, Group newrank);
        [Obsolete("Please use OnPlayerRankSetEvent.Register()")]
        public static event RankSet OnPlayerRankSet;
        public delegate void GroupSave();
        [Obsolete("Please use OnGroupSaveEvent.Register()")]
        public static event GroupSave OnGroupSave;
        public delegate void GroupLoad();
        [Obsolete("Please use OnGroupLoadEvent.Register()")]
        public static event GroupLoad OnGroupLoad;
        public delegate void GroupLoaded(Group mGroup);
        [Obsolete("Please use OnGroupLoadedEvent.Register()")]
        public static event GroupLoaded OnGroupLoaded;
        public static bool cancelrank = false;
        //Move along...nothing to see here...
        internal static void because(Player p, Group newrank) { if (OnPlayerRankSet != null) { OnPlayerRankSet(p, newrank); } OnPlayerRankSetEvent.Call(p, newrank); }
        
        public string name;
        public string trueName;
        public string color;
        public string ColoredName { get { return color + trueName; } }
        public byte OverseerMaps = 3;
        public string prefix = "";
        public LevelPermission Permission;
        public int maxBlocks;
        public long maxUndo;
        public CommandList commands;
        public string fileName;
        public PlayerList playerList;
        public string MOTD = "";
        public bool[] CanModify = new bool[256];
        public int PlayerCount { get { return playerList.Count; } }
        
        public static Group BannedRank { get { return findPerm(LevelPermission.Banned); } }
        public static Group GuestRank { get { return findPerm(LevelPermission.Guest); } }
        public static Group NobodyRank { get { return findPerm(LevelPermission.Nobody); } }
        public static Group standard;
        
        /// <summary> Create a new group object </summary>
        public Group() {
            Permission = LevelPermission.Null;
        }

        /// <summary> Create a new group object </summary>
        /// <param name="Perm">The permission of the group</param>
        /// <param name="maxB">The maxblocks this group can cuboid</param>
        /// <param name="maxUn">The max undo this group can do</param>
        /// <param name="fullName">The group full name</param>
        /// <param name="newColor">The color of the group (Not including the &amp;)</param>
        /// <param name="motd">the custom MOTD for the group</param>
        /// <param name="file">The file path where the current players of this group are stored</param>
        public Group(LevelPermission Perm, int maxB, long maxUn, string fullName, char newColor, string motd, string file, byte maps = 3, string prefix = "")
        {
            Permission = Perm;
            maxBlocks = maxB;
            maxUndo = maxUn;
            trueName = fullName;
            name = trueName.ToLower();
            color = "&" + newColor;
            MOTD = motd;
            fileName = file;
            OverseerMaps = maps;
            this.prefix = prefix;
        }
        
        public static void AddAndLoadGroup(Group grp) {
            GroupList.Add(grp);
            grp.LoadPlayers();
            
            if (OnGroupLoaded != null)
                OnGroupLoaded(grp);
            OnGroupLoadedEvent.Call(grp);
        }
        
        /// <summary> Fill the commands that this group can use </summary>
        public void fillCommands() {
            commands = CommandPerms.AllCommandsUsableBy(Permission);
        }
        
        /// <summary> Fill the blocks that this group can use </summary>
        public void FillBlocks() {
            for (int i = 0; i < CanModify.Length; i++)
                CanModify[i] = BlockPerms.CanModify(Permission, (byte)i);
        }

        /// <summary> Returns true if players in this group can use the given command. </summary>        
        public bool CanExecute(string cmdName) {
            Command cmd = Command.all.Find(cmdName);
            return cmd != null && commands.Contains(cmd);
        }
        
        /// <summary> Returns true if players in this group can use the given command. </summary>
        public bool CanExecute(Command cmd) { return commands.Contains(cmd); }

        public static List<Group> GroupList = new List<Group>();
        static readonly object saveLock = new object();
        
        /// <summary> Load up all server groups </summary>
        public static void InitAll() {
            GroupList = new List<Group>();
            if (File.Exists(Paths.RankPropsFile)) {
                GroupProperties.InitAll();
            } else {
                // Add some default ranks
                AddAndLoadGroup(new Group(LevelPermission.Builder, 400, 300, "Builder", '2', "", null));
                AddAndLoadGroup(new Group(LevelPermission.AdvBuilder, 1200, 900, "AdvBuilder", '3', "", null));
                AddAndLoadGroup(new Group(LevelPermission.Operator, 2500, 5400, "Operator", 'c', "", null));
                AddAndLoadGroup(new Group(LevelPermission.Admin, 65536, int.MaxValue, "SuperOP", 'e', "", null));
            }

            if (BannedRank == null)
                AddAndLoadGroup(new Group(LevelPermission.Banned, 1, 1, "Banned", '8', "", null));
            if (GuestRank == null)
                AddAndLoadGroup(new Group(LevelPermission.Guest, 1, 120, "Guest", '7', "", null));
            AddAndLoadGroup(new Group(LevelPermission.Nobody, 65536, -1, "Nobody", '0', "", null));
            GroupList.Sort((a, b) => a.Permission.CompareTo(b.Permission));

            if (Find(ServerConfig.DefaultRankName) != null) {
                standard = Group.Find(ServerConfig.DefaultRankName);
            } else {
                standard = GuestRank;
            }

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                pl.group = findPerm(pl.group.Permission);
                if (pl.group == null) pl.group = standard;
            }
            
            if (OnGroupLoad != null) OnGroupLoad();
            OnGroupLoadEvent.Call();
            saveGroups(GroupList);
        }
        
        /// <summary> Save givenList group </summary>
        /// <param name="givenList">The list of groups to save</param>
        public static void saveGroups(List<Group> givenList) {
            lock (saveLock)
                GroupProperties.SaveGroups(givenList);
            
            if (OnGroupSave != null) OnGroupSave();
            OnGroupSaveEvent.Call();
        }
        
        void LoadPlayers() {
            string desired = (int)Permission + "_rank";
            // Try to use the auto filename format
            if (fileName == null || !fileName.StartsWith(desired))
                MoveToDesired(desired);
            
            playerList = PlayerList.Load(fileName);
        }
        
        void MoveToDesired(string desired) {
            // rank doesn't exist to begin with
            if (fileName == null || !File.Exists("ranks/" + fileName)) {
                fileName = desired + ".txt";
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
                File.Move("ranks/" + fileName, "ranks/" + newFile);
                fileName = newFile;
                return true;
            } catch (Exception ex) {
                Logger.LogError(ex);
                return false;
            }
        }
        
        
        /// <summary> Check whether a group with that name exists. </summary>
        public static bool Exists(string name) {
            name = name.ToLower();
            foreach (Group grp in GroupList) {
                if (grp.name == name) return true;
            }
            return false;
        }
        
        
        /// <summary> Find the group which has the given name. </summary>
        public static Group Find(string name) {
            name = name.ToLower();
            MapName(ref name);
            
            foreach (Group grp in GroupList) {
                if (grp.name == name) return grp;
            }
            return null;
        }
        
        internal static void MapName(ref string name) {
            if (name == "adv") name = "advbuilder";
            if (name == "op") name = "operator";
            if (name == "super" || (name == "admin" && !Exists("admin"))) name = "superop";
            if (name == "noone") name = "nobody";
        }
        
        /// <summary> Finds the group which has the given permission level. </summary>
        public static Group findPerm(LevelPermission Perm) {
            return GroupList.Find(grp => grp.Permission == Perm);
        }

        /// <summary> Find the group which has the given permission level. </summary>
        public static Group findPermInt(int Perm) {
            return GroupList.Find(grp => (int)grp.Permission == Perm);
        }

        /// <summary> Find the group object that the player /playerName/ is in </summary>
        /// <param name="name">The player name</param>
        /// <returns>The group object that the player is in</returns>
        public static Group findPlayerGroup(string name) {
            foreach (Group grp in Group.GroupList) {
                if (grp.playerList.Contains(name)) return grp;
            }
            return Group.standard;
        }
        
        
        /// <summary> Returns whether the given player is in the banned rank. </summary>
        public static bool IsBanned(string name) {
            Group grp = BannedRank;
            return grp != null && grp.playerList.Contains(name);
        }
        
        public static string GetName(LevelPermission perm) {
            Group grp = findPerm(perm);
            if (grp != null) return grp.trueName;
            return ((int)perm).ToString();
        }
        
        public static string GetColoredName(LevelPermission perm) {
            Group grp = findPerm(perm);
            if (grp != null) return grp.ColoredName;
            return Colors.white + ((int)perm);
        }
        
        public static string GetColoredName(string rankName) {
            Group grp = Find(rankName);
            if (grp != null) return grp.ColoredName;
            return Colors.white + rankName;
        }
        
        public static string GetColor(LevelPermission perm) {
            Group grp = findPerm(perm);
            if (grp != null) return grp.color;
            return Colors.white;
        }
        
        public static LevelPermission ParsePermOrName(string value) {
            if (value == null) return LevelPermission.Null;
            
            sbyte perm;
            if (sbyte.TryParse(value, out perm))
                return (LevelPermission)perm;
            
            Group grp = Find(value);
            return grp != null ? grp.Permission : LevelPermission.Null;
        }
    }
}
