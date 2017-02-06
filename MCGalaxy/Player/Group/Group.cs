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
            playerList = PlayerList.Load(fileName);
            if (OnGroupLoaded != null)
                OnGroupLoaded(this);
            OnGroupLoadedEvent.Call(this);
        }
        
        /// <summary> Fill the commands that this group can use </summary>
        public void fillCommands() {
            CommandList _commands = new CommandList();
            GrpCommands.AddCommands(out _commands, Permission);
            commands = _commands;
        }
        
        /// <summary> Fill the blocks that this group can use </summary>
        public void FillBlocks() {
            for (int i = 0; i < CanModify.Length; i++)
                CanModify[i] = Block.canPlace(Permission, (byte)i);
        }
        
        public bool CanExecute(string cmdName) {
            return commands.Contains(Command.all.Find(cmdName));
        }
        
        /// <summary> Check to see if this group can excute cmd </summary>
        /// <param name="cmd">The command object to check</param>
        /// <returns>True if this group can use it, false if they cant</returns>
        public bool CanExecute(Command cmd) { return commands.Contains(cmd); }

        public static List<Group> GroupList = new List<Group>();
        static readonly object saveLock = new object();
        
        /// <summary> Load up all server groups </summary>
        public static void InitAll() {
            GroupList = new List<Group>();
            if (File.Exists("properties/ranks.properties")) {
                GroupProperties.InitAll();
            } else {
                // Add some default ranks
                GroupList.Add(new Group(LevelPermission.Builder, 400, 300, "Builder", '2', String.Empty, "builders.txt"));
                GroupList.Add(new Group(LevelPermission.AdvBuilder, 1200, 900, "AdvBuilder", '3', String.Empty, "advbuilders.txt"));
                GroupList.Add(new Group(LevelPermission.Operator, 2500, 5400, "Operator", 'c', String.Empty, "operators.txt"));
                GroupList.Add(new Group(LevelPermission.Admin, 65536, int.MaxValue, "SuperOP", 'e', String.Empty, "uberOps.txt"));
            }

            if (BannedRank == null)
                GroupList.Add(new Group(LevelPermission.Banned, 1, 1, "Banned", '8', String.Empty, "banned.txt"));
            if (GuestRank == null)
                GroupList.Add(new Group(LevelPermission.Guest, 1, 120, "Guest", '7', String.Empty, "guest.txt"));
            GroupList.Add(new Group(LevelPermission.Nobody, 65536, -1, "Nobody", '0', String.Empty, "nobody.txt"));
            GroupList.Sort((a, b) => a.Permission.CompareTo(b.Permission));

            if (Find(Server.defaultRank) != null) {
                standard = Group.Find(Server.defaultRank);
            } else {
                standard = GuestRank;
            }

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                pl.group = findPerm(pl.group.Permission);
                if (pl.group == null) pl.group = standard;
            }
            if (OnGroupLoad != null)
                OnGroupLoad();
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
        
        /// <summary> Find the group(s) which contain the given name. </summary>
        public static Group FindMatches(Player p, string name, out int matches) {
            name = name.ToLower();
            MapName(ref name);
            return Matcher.Find<Group>(p, name, out matches,
                                       GroupList, g => true, g => g.name, "ranks");
        }
        
        /// <summary> Find the group(s) which contain the given name. </summary>
        public static Group FindMatches(Player p, string name) {
            int matches = 0; return FindMatches(p, name, out matches);
        }
        
        static void MapName(ref string name) {
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
        

        /// <summary> Get the group name that player /playerName/ is in </summary>
        /// <param name="name">The player Name</param>
        /// <returns>The group name</returns>
        public static string findPlayer(string name) { return findPlayerGroup(name).name; }

        /// <summary> Find the group object that the player /playerName/ is in </summary>
        /// <param name="name">The player name</param>
        /// <returns>The group object that the player is in</returns>
        public static Group findPlayerGroup(string name) {
            foreach (Group grp in Group.GroupList) {
                if (grp.playerList.Contains(name)) return grp;
            }
            return Group.standard;
        }

        public static string concatList(bool includeColor = true, bool skipExtra = false, bool permissions = false) {
            string sum = "";
            foreach (Group grp in Group.GroupList) {
                if (skipExtra && (grp.Permission < LevelPermission.Guest || grp.Permission >= LevelPermission.Nobody)) continue;
                
                if (includeColor) sum += ", " + grp.ColoredName + Server.DefaultColor;
                else if (permissions) sum += ", " + ((int)grp.Permission);
                else sum += ", " + grp.name;
            }

            if (includeColor) sum = sum.Remove(sum.Length - 2);
            return sum.Remove(0, 2);
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
