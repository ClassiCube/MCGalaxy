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
using MCGalaxy.Events;

namespace MCGalaxy {
    /// <summary> This is the group object, where ranks and their data are stored </summary>
    public sealed partial class Group {
        
        public delegate void GroupSave();
        [Obsolete("Please use OnGroupSaveEvent.Register()")]
        public static event GroupSave OnGroupSave;
        public delegate void GroupLoad();
        [Obsolete("Please use OnGroupLoadEvent.Register()")]
        public static event GroupLoad OnGroupLoad;
        public delegate void GroupLoaded(Group mGroup);
        [Obsolete("Please use OnGroupLoadedEvent.Register()")]
        public static event GroupLoaded OnGroupLoaded;
        
        public static Group BannedRank { get { return findPerm(LevelPermission.Banned); } }
        public static Group GuestRank { get { return findPerm(LevelPermission.Guest); } }
        public static Group NobodyRank { get { return findPerm(LevelPermission.Nobody); } }
        public static Group standard;
        
        public static void Register(Group grp) {
            GroupList.Add(grp);
            grp.LoadPlayers();
            
            if (OnGroupLoaded != null)
                OnGroupLoaded(grp);
            OnGroupLoadedEvent.Call(grp);
        }

        public static List<Group> GroupList = new List<Group>();
        static readonly object saveLock = new object();
        
        /// <summary> Load up all server groups </summary>
        public static void InitAll() {
            GroupList = new List<Group>();
            if (File.Exists(Paths.RankPropsFile)) {
                GroupProperties.InitAll();
            } else {
                // Add some default ranks
                Register(new Group(LevelPermission.Builder, 400, 300, "Builder", '2', "", null));
                Register(new Group(LevelPermission.AdvBuilder, 1200, 900, "AdvBuilder", '3', "", null));
                Register(new Group(LevelPermission.Operator, 2500, 5400, "Operator", 'c', "", null));
                Register(new Group(LevelPermission.Admin, 65536, int.MaxValue, "SuperOP", 'e', "", null));
            }

            if (BannedRank == null)
                Register(new Group(LevelPermission.Banned, 1, 1, "Banned", '8', "", null));
            if (GuestRank == null)
                Register(new Group(LevelPermission.Guest, 1, 120, "Guest", '7', "", null));
            Register(new Group(LevelPermission.Nobody, 65536, -1, "Nobody", '0', "", null));
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
        
        
        /// <summary> Find the group which has the given name. </summary>
        public static Group Find(string name) {
            MapName(ref name);
            
            const StringComparison comp = StringComparison.OrdinalIgnoreCase;
            foreach (Group grp in GroupList) {
                if (grp.Name.Equals(name, comp)) return grp;
            }
            return null;
        }
        
        internal static void MapName(ref string name) {
            if (name.CaselessEq("op")) name = "operator";
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
                if (grp.Players.Contains(name)) return grp;
            }
            return Group.standard;
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
            if (grp != null) return grp.Color;
            return Colors.white;
        }
    }
}
